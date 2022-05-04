using Application.Contracts.V1.Activities;
using Application.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CsvHelper;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Domain.Enums;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Utils.AWS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.DataTransferObjects;
using Shared.ResourceParameters;
using System.Dynamic;
using System.Globalization;
using System.Net;
using Infrastructure.Utils.Email;
using System.Reflection;
using LinqKit;

namespace Application.Services.V1.Activities
{
    public class EventService : IEventService
    {
        #region Setup
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly IWebHelper _webHelper;
        private readonly IAwsS3Client _awsS3Client;
        private readonly IEmailManager _emailManager;
        private readonly UserManager<User> _userManager;
        public EventService(
            IMapper mapper,
            IRepositoryManager repository,
            IWebHelper webHelper,
            IAwsS3Client awsS3Client, 
            IEmailManager emailManager, 
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _repository = repository;
            _webHelper = webHelper;
            _awsS3Client = awsS3Client;
            _userManager = userManager;
            _emailManager = emailManager;

        }
        #endregion
        //create event
        public async Task<SuccessResponse<GetEventDTO>> CreateEvent(CreateEventDTO eventDTO)
        {
            var organizationId = _webHelper.User().OrganizationId;
            var activity = await GetEventActivity();

            await ProgrammeExist(eventDTO.ProgrammeId, organizationId);

            await CheckDuplicateTitle(eventDTO.Title, eventDTO.ProgrammeId);

            var learningTracks = await GetLearningTracks(eventDTO.LearningTracks, organizationId);

            var eventModel = _mapper.Map<Event>(eventDTO);
            eventModel.Status = EProgrammeStatus.Upcoming.ToString();
            eventModel.ActivityId = activity.Id;
            eventModel.CreatedById = _webHelper.User().UserId;

            await _repository.Event.AddAsync(eventModel);
            if (learningTracks.Any())
            {
                eventModel.EventLearningTracks = GetEventLearningTracksForCreation(eventDTO.LearningTracks, eventModel.Id);
            }

            UserActivity userActivity = AuditLog.UserActivity(eventModel, _webHelper.User().UserId, nameof(eventModel), $"Create an Event - {eventModel.Title}", eventModel.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var getEventDTO = _mapper.Map<GetEventDTO>(eventModel);
            getEventDTO.LearningTracks = learningTracks;

            return new SuccessResponse<GetEventDTO>
            {
                Message = ResponseMessages.EventCreatedSuccessfully,
                Data = getEventDTO
            };
        }

        public async Task<SuccessResponse<GetEventDTO>> AddEventDetail(Guid id, CreateEventDetailInputDto request)
        {
            var eventModel = await GetEventById(id);

            string bannerUrl;
            if (request.Banner != null)
                bannerUrl = await _awsS3Client.UploadFileAsync(request.Banner);
            else
                bannerUrl = "https://mcdan-bucket.s3.eu-west-2.amazonaws.com/uploads/132937244759104268-pru2.png";

            eventModel.BannerUrl = bannerUrl;
            eventModel.EventDetail = request.Details;
            eventModel.UpdatedAt = DateTime.Now;

            _repository.Event.Update(eventModel);
            await _repository.SaveChangesAsync();

            var getEventDto = _mapper.Map<GetEventDTO>(eventModel);

            return new SuccessResponse<GetEventDTO>
            {
                Message = ResponseMessages.EventDetailCreatedSuccessfully,
                Data = getEventDto
            };
        }

        public async Task<SuccessResponse<GetEventDTO>> UpdateEvent(Guid eventId, UpdateEventDTO eventDTO)
        {
            var organizationId = _webHelper.User().OrganizationId;
            var eventModel = await GetEventById(eventId);

            var learningTracks = await GetLearningTracks(eventDTO.LearningTracks, organizationId);

            if (eventModel.Title.Equals(eventDTO.Title, StringComparison.CurrentCultureIgnoreCase))
            {
                await CheckDuplicateTitle(eventDTO.Title, eventModel.ProgrammeId);
            }

            _mapper.Map(eventDTO, eventModel);
            eventModel.UpdatedAt = DateTime.UtcNow;

            var eventLearningTracks = await _repository.EventLearningTrack.Get(x => x.EventId == eventId).ToListAsync();
            var learningTrackIds = eventLearningTracks.Select(x => x.LearningTrackId);

            var learningTrackForCreation = eventDTO.LearningTracks.Except(learningTrackIds);
            var eventLearningTracksForCreation = GetEventLearningTracksForCreation(learningTrackForCreation, eventId);
            await _repository.EventLearningTrack.AddRangeAsync(eventLearningTracksForCreation);

            var learningTrackToRemove = learningTrackIds.Except(eventDTO.LearningTracks);
            var assessmentLearningTracksToRemove =
                eventLearningTracks.Where(x => learningTrackToRemove.Contains(x.LearningTrackId));
            _repository.EventLearningTrack.RemoveRange(assessmentLearningTracksToRemove);


            UserActivity userActivity = AuditLog.UserActivity(eventModel, _webHelper.User().UserId, nameof(eventModel), $"Created an Event - {eventModel.Title}", eventModel.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetEventDTO>(eventModel);
            var learningTrackDto = learningTracks.Where(x => !learningTrackToRemove.Contains(x.Id)).ToList();
            response.LearningTracks = learningTrackDto;

            return new SuccessResponse<GetEventDTO>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = response
            };
        }
        
        public async Task<PagedResponse<IEnumerable<GetAllEventDto>>> GetEventsAsync(EventResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var organizationId = _webHelper.User().OrganizationId;
            var userId = _webHelper.User().UserId;

            var userProgramme = await _repository.ApprovedApplicant.QueryAll(x => x.UserId == userId)
                .Include(x => x.ApprovedApplicantProgrammes)
                .Select(x => new
                {
                    ProgrammeIds = x.ApprovedApplicantProgrammes.Select(x => x.ProgrammeId),
                    LearningTrackIds = x.ApprovedApplicantProgrammes.Select(x => x.LearningTrackId)
                }).FirstOrDefaultAsync();

            var eventQuery = _repository.Event.QueryAll()
                .Include(x => x.Programme)
                .Include(x => x.EventLearningTracks)
                .Where(x => x.Programme.OrganizationId == organizationId)
                .Select(x => new GetAllEventDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    BannerUrl = x.BannerUrl,
                    Status = x.Status,
                    IsOnline = x.IsOnline,
                    EventTime = x.EventTime,
                    CreatedById = x.CreatedById,
                    LearningTracks = x.EventLearningTracks.Select(x => new GetLearningTrackDto
                    {
                        Id = x.LearningTrackId,
                        Title = x.LearningTrack.Title
                    })
                });

            var user = await _repository.User.GetByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Any(role => role == ERole.Facilitator.ToString()))
            {
                eventQuery = eventQuery.Where(x => x.LearningTracks.Any(l => userProgramme.LearningTrackIds.Contains(l.Id)) || x.CreatedById == userId);
            }

            if (roles.Contains(ERole.Beneficiary.ToString()) && !roles.Contains(ERole.Facilitator.ToString()))
            {
                eventQuery = eventQuery.Where(x => x.LearningTracks.Any(l => userProgramme.LearningTrackIds.Contains(l.Id)));
            }

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                var search = parameter.Search.Trim().ToLower();
                eventQuery = eventQuery.Where(x => x.Title.Contains(search));
            }

            ExpressionStarter<GetAllEventDto> predicate = GetEventsQueryPredicateBuilder(parameter);

            if (!string.IsNullOrWhiteSpace(parameter.EventType))
            {
                var eventType = parameter.EventType.Trim().ToLower();
                if (eventType.Equals("Physical", StringComparison.OrdinalIgnoreCase))
                {
                    eventQuery = eventQuery.Where(x => x.IsOnline == false);
                }

                if (eventType.Equals("Online", StringComparison.OrdinalIgnoreCase))
                {
                    eventQuery = eventQuery.Where(x => x.IsOnline == true);
                }
            }

            eventQuery = eventQuery
                .Where(predicate)
                .OrderByDescending(x => x.StartDate);

            var events = await PagedList<GetAllEventDto>.Create(eventQuery, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var dynamicParameters = PageUtility<GetAllEventDto>.GenerateResourceParameters(parameter, events);
            var page = PageUtility<GetAllEventDto>.CreateResourcePageUrl(dynamicParameters, name, events, urlHelper);

            return new PagedResponse<IEnumerable<GetAllEventDto>>
            {
                Message = "Data successfully retrieved",
                Data = events,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
        }

        private static ExpressionStarter<GetAllEventDto> GetEventsQueryPredicateBuilder(EventResourceParameter parameter)
        {
            var predicate = PredicateBuilder.New<GetAllEventDto>(true);

            if (parameter.Upcoming)
            {
                var upcoming = EProgrammeStatus.Upcoming.ToString();
                predicate = predicate.Or(p => p.Status.ToLower() == upcoming);
            }

            if (parameter.Completed)
            {
                var completed = EProgrammeStatus.Completed.ToString();
                predicate = predicate.Or(p => p.Status.ToLower() == completed);
            }

            if (parameter.Cancelled)
            {
                var cancelled = EProgrammeStatus.Cancelled.ToString();
                predicate = predicate.Or(p => p.Status.ToLower() == cancelled);
            }

            if (parameter.Ongoing)
            {
                var ongoing = EProgrammeStatus.Ongoing.ToString();
                predicate = predicate.Or(p => p.Status.ToLower() == ongoing);
            }

            return predicate;
        }

        public async Task<SuccessResponse<GetEventDTO>> GetEvent(Guid eventId)
        {
            var eventModel = await _repository.Event.Get(x => x.Id == eventId)
                .Include(x => x.EventLearningTracks)
                .ThenInclude(x => x.LearningTrack)
                .FirstOrDefaultAsync();

            if (eventModel == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.EventNotFound}");
            }

            var getEventDTO = _mapper.Map<GetEventDTO>(eventModel);
            return new SuccessResponse<GetEventDTO>
            {
                Message = ResponseMessages.DeletionSuccessResponse,
                Data = getEventDTO
            };
        }
        private void CheckActivityDate(DateTime endDate)
        {
            if (endDate > DateTime.Now)
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.ProgrammeInProgress}");
        }

        public async Task<SuccessResponse<object>> DeleteEvent(Guid eventId)
        {
            var eventModel = await GetEventById(eventId);

            CheckActivityDate(eventModel.EndDate);

            _repository.Event.Remove(eventModel);

            UserActivity userActivity = AuditLog.UserActivity(eventModel, _webHelper.User().UserId, nameof(eventModel), $"Deleted an Event - {eventModel.Title}", eventModel.Id);

            await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<object>
            {
                Message = ResponseMessages.DeletionSuccessResponse
            };
        }

        private async Task<Activity> GetEventActivity()
        {
            var activity = await _repository.Activity.FirstOrDefault(x => x.Type == EActivityType.Events.ToString());
            
            if (activity == null)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.ActivityNotFound}");
            }

            return activity;
        }

        private async Task ProgrammeExist(Guid programmeId, Guid organizationId)
        {
            var programmeExists = await _repository.Programme.ExistsAsync(x => x.Id == programmeId && x.OrganizationId == organizationId);
            if (!programmeExists)
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.ProgrammeNotFound);
        }

        private async Task CheckDuplicateTitle(string title, Guid programmeId)
        {
            var titleExist =
                await _repository.Event.ExistsAsync(x => x.Title == title && x.ProgrammeId != programmeId);
 
            if (titleExist)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.DuplicateTitle}");
            }
        }

        private async Task<List<GetLearningTrackEventDTO>> GetLearningTracks(List<Guid> learningTracksRequest, Guid organizationId)
        {
            var learningTracks = await _repository.LearningTrack.QueryAll(x => learningTracksRequest.Contains(x.Id))
                .Include(x => x.Programme)
                .Where(x => x.Programme.OrganizationId == organizationId)
                .Select(x => new GetLearningTrackEventDTO{Id = x.Id, Title = x.Title})
                .ToListAsync();
 
            if (learningTracks.Count != learningTracksRequest.Count)
            {
                var learningTrackIds = learningTracks.Select(x => x.Id);
                var learningTrackIdsNotFound = learningTracksRequest.Except(learningTrackIds);
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.LearningTrackNotFound} {string.Join(",", learningTrackIdsNotFound)}");
            }
 
            return learningTracks;
        }

        private List<EventLearningTrack> GetEventLearningTracksForCreation(IEnumerable<Guid> learningTrackIds, Guid eventId)
        {
            var eventLearningTracks = new List<EventLearningTrack>();

            foreach (var learningTrackId in learningTrackIds)
            {
                eventLearningTracks.Add(new EventLearningTrack{EventId =  eventId, LearningTrackId = learningTrackId});
            }

            return eventLearningTracks;
        }

        private async Task<Event> GetEventById(Guid eventId)
        {
            var eventModel = await _repository.Event.GetByIdAsync(eventId);

            if (eventModel == null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"{ResponseMessages.EventNotFound}");
            }

            return eventModel;
        }

        public async Task<SuccessResponse<string>> RegisterForEvent(Guid eventId, RegisterForEventDTO model)
        {
            //Checking if event exist
            var eventToRegisterFor = await _repository.Event.GetByIdAsync(eventId);

            if (eventToRegisterFor == null)
                throw new RestException(HttpStatusCode.NotFound, "Event not found");

            //Checking if approvedApplicant exist
            var approvedApplicant = await _repository.ApprovedApplicant.Get(x => x.Id == model.ApplicantId)
              .Include(b => b.ApprovedApplicantProgrammes)
              .FirstOrDefaultAsync();

            if (approvedApplicant == null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            //Checking if the approvedApplicant has already registered for this event
            var getApplicantEventRecord = _repository.EventRegistration.Get(x => x.EventId == eventId)
                .Where(e => e.ApprovedApplicantId == model.ApplicantId);

            if (getApplicantEventRecord.Any())
                throw new RestException(HttpStatusCode.Unauthorized, "You have already registered for this event");

            if (eventToRegisterFor.Status != EEvent.Ongoing.ToString() && eventToRegisterFor.Status != EEvent.Upcoming.ToString())
                throw new RestException(HttpStatusCode.Unauthorized, "This event is nolonger open for registration");

            //checking if the approvedApplicant is a beneficiary or facilitator under the event programme
            if (!CanRegisterBaseOnProgramme(eventToRegisterFor, approvedApplicant))
                throw new RestException(HttpStatusCode.Unauthorized, "You are not Eligible to register for this event");

            //checking if the approved applicant belongs to the event's learning tracks
            await CheckEligibilityBasedOnLearningTrack(eventId, eventToRegisterFor, approvedApplicant);

            var eventRegistration = new EventRegistration
            {
                EventId = eventId,
                ApprovedApplicantId = model.ApplicantId,
                CreatedAt = DateTime.UtcNow,
            };

            await _repository.EventRegistration.AddAsync(eventRegistration);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<string>()
            {
                Data="",
                Message = "Your registration was successful",
                Success = true,
            };
        }

        public async Task<PagedResponse<IEnumerable<GetEventApplicantsDTO>>> GetEventApplicants(Guid eventId, ResourceParameter parameter, string actionName, IUrlHelper urlHelper)
        {
            var eventRegistersQuery = _repository.EventRegistration.Get(x => x.EventId == eventId)
            .Include(e => e.Event)
            .Include(a => a.ApprovedApplicant)
                .ThenInclude(u => u.User)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetEventApplicantsDTO()
            {
                FirstName = x.ApprovedApplicant.User.FirstName,
                LastName = x.ApprovedApplicant.User.LastName,
                Email = x.ApprovedApplicant.User.Email,
                PhoneNumber = x.ApprovedApplicant.User.PhoneNumber,
                RegisteredAt = x.CreatedAt,
                LearningTrack = GetLearningTrack(x.Event.EventLearningTracks.Select(x => new GetLearningTrackDto 
                    {
                       Title = x.LearningTrack.Title,
                       Id = x.LearningTrackId
                    }), 
                    x.ApprovedApplicant.ApprovedApplicantProgrammes.Select(x => new GetLearningTrackDto 
                        {
                          Title = x.learningTrack.Title,
                          Id = (Guid)x.LearningTrackId
                        }
                    )
                )
            });

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                string search = parameter.Search.Trim();
                eventRegistersQuery = eventRegistersQuery.Where(x =>
                    x.FirstName.Contains(search) ||
                    x.LastName.Contains(search) ||
                    x.PhoneNumber.Contains(search) ||
                    x.Email.Contains(search));
            }

            var eventRegisters = await PagedList<GetEventApplicantsDTO>.Create(eventRegistersQuery, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetEventApplicantsDTO>.CreateResourcePageUrl(parameter, actionName, eventRegisters, urlHelper);

            return new PagedResponse<IEnumerable<GetEventApplicantsDTO>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = eventRegisters,
                Meta = new Meta
                {
                    Pagination = page
                },
            };
        }

        public async Task<SuccessResponse<string>> CancelAnEvent(Guid eventId, CancelEventDTO cancelEventDto)
        {
            var eventToCancel = await _repository.Event.FirstOrDefault(x => x.Id == eventId);
            if (eventToCancel == null)
                throw new RestException(HttpStatusCode.NotFound, "Event not found");

            //checking if the event to be cancelled was created by the user trying to cancell it 
            if (_webHelper.User().UserId != eventToCancel.CreatedById)
                throw new RestException(HttpStatusCode.Unauthorized, "You don't have authorisation to delete this event");

            //changing the status of the event to "cancelled" and adding reanson for cancelling
            eventToCancel.ReasonForCancellation = cancelEventDto.ReasonForCancellation;
            eventToCancel.Status = EEvent.Cancelled.ToString();
            _repository.Event.Update(eventToCancel);
            await _repository.SaveChangesAsync();

            //getting the email of all applicant that registered for the event
            var eventRegisteredUsersEmail = await _repository.EventRegistration.Get(x => x.EventId == eventId)
                .Select(z => z.ApprovedApplicant.User.Email).ToArrayAsync();

            //sending emeil to notify all that registered for the event that it has been cancelled
            if(eventRegisteredUsersEmail.Any())
            {
                _emailManager.SendBulkEmail(eventRegisteredUsersEmail, cancelEventDto.ReasonForCancellation, $"{eventToCancel.Title} Event cancellation");
            }
         
            return new SuccessResponse<string>
            {
                Data = "",
                Message = "Event successfully cancelled",
                Success = true
            };

        }

        #region Private Methods

        private static GetLearningTrackDto GetLearningTrack(IEnumerable<GetLearningTrackDto> eventLearningTracks, IEnumerable<GetLearningTrackDto> applicantLearningTracks)
        {
            GetLearningTrackDto result = new GetLearningTrackDto();
            foreach (GetLearningTrackDto eventLearningTrack in eventLearningTracks)
            {
                if(applicantLearningTracks.Any(x => x.Title == eventLearningTrack.Title))
                    result = eventLearningTrack;
            }
            return result;
        }

        private async Task CheckEligibilityBasedOnLearningTrack(Guid eventId, Event eventToRegister, ApprovedApplicant approvedApplicant)
        {
            var eventLearningTrackIds = await _repository.EventLearningTrack.Get(x => x.EventId == eventId).Select(e => e.LearningTrackId).ToArrayAsync();

            Guid  approvedApplicantLearningTrackId = (Guid)await _repository.ApprovedApplicantProgramme.Get(x => x.ProgrammeId == eventToRegister.ProgrammeId)
                                                            .Select(z => z.LearningTrackId).FirstOrDefaultAsync();

            if (!eventLearningTrackIds.Contains(approvedApplicantLearningTrackId))
                throw new RestException(HttpStatusCode.Unauthorized, "This event is not for your learning track");
        }

        private static bool CanRegisterBaseOnProgramme(Event eventToRegister, ApprovedApplicant approvedApplicant)
        {
            int count = 0;
            var approvedApplicantPrograms = approvedApplicant.ApprovedApplicantProgrammes;

            foreach (var program in approvedApplicantPrograms)
            {
                if (program.ProgrammeId == eventToRegister.ProgrammeId)
                    count++;
            }

            return count > 0;
        }

        public async Task<PagedResponse<IEnumerable<GetRegisteredBeneficaryDto>>> GetRegisteredBeneficiaries(ResourceParameter parameter, string name, IUrlHelper urlHelper)
        {
            var regBeneficiariesQuery = _repository.EventRegistration.QueryAll()
                .Include(x => x.ApprovedApplicant)
                    .ThenInclude(x => x.User)
                .Include(x => x.ApprovedApplicant)
                    .ThenInclude(x => x.ApprovedApplicantProgrammes)
                .Select(x => new GetRegisteredBeneficaryDto
                {
                    EventId = x.EventId,
                    FirstName = x.ApprovedApplicant.User.FirstName,
                    LastName = x.ApprovedApplicant.User.LastName,
                    Email = x.ApprovedApplicant.User.Email,
                    PhoneNumber = x.ApprovedApplicant.User.PhoneNumber,
                    RegisteredDate = x.CreatedAt,
                    LearningTracks = x.ApprovedApplicant.ApprovedApplicantProgrammes.Select(x => new GetLearningTrackDto
                    {
                        Id = x.LearningTrackId.Value,
                        Title = x.learningTrack.Title
                    })
                });

            if (!string.IsNullOrWhiteSpace(parameter.Search))
            {
                var search = parameter.Search.Trim().ToLower();
                regBeneficiariesQuery = regBeneficiariesQuery.Where(x => x.FirstName.Contains(search) || x.LastName.Contains(search) || x.Email.Contains(search));
            }

            var getRegisteredBeneficaries = await PagedList<GetRegisteredBeneficaryDto>.Create(regBeneficiariesQuery, parameter.PageNumber, parameter.PageSize, parameter.Sort);
            var page = PageUtility<GetRegisteredBeneficaryDto>.CreateResourcePageUrl(parameter, name, getRegisteredBeneficaries, urlHelper);

            return new PagedResponse<IEnumerable<GetRegisteredBeneficaryDto>>
            {
                Message = ResponseMessages.DataRetrievedSuccessfully,
                Data = getRegisteredBeneficaries,
                Meta = new Meta
                {
                    Pagination = page
                }
            };
        }

        public async Task<SuccessResponse<IEnumerable<GetAllEventDto>>> GetScheduledEvents(Guid userId)
        {
            var user = await _repository.User.FirstOrDefaultNoTracking(x => x.Id == userId && x.IsActive);
            if (user is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var userProgramme = await _repository.ApprovedApplicant.QueryAll(x => x.UserId == userId)
                .Include(x => x.ApprovedApplicantProgrammes)
                 .Select(x => new
                 {
                     ProgrammeIds = x.ApprovedApplicantProgrammes.Select(x => x.ProgrammeId),
                     LearningTrackIds = x.ApprovedApplicantProgrammes.Select(x => x.LearningTrackId)
                 }).FirstOrDefaultAsync();

            var eventQuery = _repository.Event.QueryAll()
               .Include(x => x.EventLearningTracks)
               .Where(x => userProgramme.ProgrammeIds.Contains(x.ProgrammeId))
               .Select(x => new GetAllEventDto
               {
                   Id = x.Id,
                   Title = x.Title,
                   StartDate = x.StartDate,
                   EndDate = x.EndDate,
                   BannerUrl = x.BannerUrl,
                   Status = x.Status,
                   IsOnline = x.IsOnline,
                   EventTime = x.EventTime,
                   EventLink = x.EventLink,
                   LearningTracks = x.EventLearningTracks.Select(x => new GetLearningTrackDto
                   {
                       Id = x.LearningTrackId,
                       Title = x.LearningTrack.Title
                   }),
               });

            var scheduledEvents = await eventQuery
                .Where(x => x.LearningTracks
                .Any(l => userProgramme.LearningTrackIds.Contains(l.Id)))
                .ToListAsync();

            return new SuccessResponse<IEnumerable<GetAllEventDto>>
            {
                Message = ResponseMessages.DataRetrievedSuccessfully,
                Data = scheduledEvents
            };
        }

        public async Task<SuccessResponse<IEnumerable<GetEventDto>>> GetUpcomingEvents(Guid userId)
        {
            var user = await _repository.User.FirstOrDefaultNoTracking(x => x.Id == userId && x.IsActive);
            if (user is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.UserNotFound);

            var userProgramme = await _repository.ApprovedApplicant
                .QueryAll(x => x.UserId == userId)
                .Include(x => x.ApprovedApplicantProgrammes)
                .Select(x => new
                {
                    ProgrammeIds = x.ApprovedApplicantProgrammes.Select(x => x.ProgrammeId)
                })
                .FirstOrDefaultAsync();

            var events = await _repository.Event.QueryAll(x => userProgramme.ProgrammeIds.Contains(x.ProgrammeId))
               .Where(x => x.StartDate > DateTime.Now)
               .Select(x => new GetEventDto
               {
                   Id = x.Id,
                   Title = x.Title,
                   StartDate = x.StartDate,
                   BannerUrl = x.BannerUrl,
                   EventTime = x.EventTime,
                   EndDate = x.EndDate,
                   Status = x.Status,
                   IsOnline = x.IsOnline,
                   EventLink = x.EventLink
               })
               .Take(3)
               .ToListAsync();

            return new SuccessResponse<IEnumerable<GetEventDto>>
            {
                Message = ResponseMessages.DataRetrievedSuccessfully,
                Data = events
            };
        }

        public async Task<MemoryStream> GetRegisteredBeneficiariesCsv()
        {
            var regBeneficiaries = await _repository.EventRegistration.QueryAll()
                .Include(x => x.ApprovedApplicant)
                    .ThenInclude(x => x.User)
                .Include(x => x.ApprovedApplicant)
                    .ThenInclude(x => x.ApprovedApplicantProgrammes)
                .Select(x => new GetRegisteredBeneficaryCsvDto
                {
                    EventId = x.EventId,
                    FirstName = x.ApprovedApplicant.User.FirstName,
                    LastName = x.ApprovedApplicant.User.LastName,
                    Email = x.ApprovedApplicant.User.Email,
                    PhoneNumber = x.ApprovedApplicant.User.PhoneNumber,
                    RegisteredDate = x.CreatedAt
                })
                .ToListAsync();

            var stream = new MemoryStream();
            using (var writeFile = new StreamWriter(stream, leaveOpen: true))
            using (var csvResult = new CsvWriter(writeFile, CultureInfo.InvariantCulture))
            {
                csvResult.WriteRecords(regBeneficiaries);
            }
            stream.Position = 0;

            return stream;
        }
        #endregion
    }
}