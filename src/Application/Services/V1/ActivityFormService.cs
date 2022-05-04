using Application.Contracts.V1;
using Application.Enums;
using Application.Helpers;
using Application.Resources;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Activities;
using Domain.Entities.Actvities;
using Domain.Enums;
using Domian.Entities.ActivityForms;
using Infrastructure.Contracts;
using Infrastructure.Utils.AWS;
using Infrastructure.Utils.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.DataTransferObjects;
using System.Net;
using System.Reflection;

namespace Application.Services.V1
{
    #region Setup
    public class ActivityFormService : IActivityFormService
    { 
        private readonly IMapper _mapper;
        private readonly IAwsS3Client _awsS3Client;
        private readonly IRepositoryManager _repository;
        private readonly IWebHelper _webHelper;
        private readonly IEmailManager _mailerService;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public ActivityFormService(
            IMapper mapper,
            IAwsS3Client awsS3Client, IWebHelper webHelper,
            IRepositoryManager repository,
            IEmailManager mailerService,
            UserManager<User> userManager,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _awsS3Client = awsS3Client;
            _webHelper = webHelper;
            _repository = repository;
            _mailerService = mailerService;
            _userManager = userManager;
            _configuration = configuration;
        }

        #endregion
       #region implementation
        public async Task<SuccessResponse<GetDefaultFormDto>> CreateDefaultForm(Guid id, CreateFormInputDto inputDto)
        {
            GetDefaultFormDto response = await CreateFormType(id, inputDto, EFormType.Default);

            return new SuccessResponse<GetDefaultFormDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }
        public async Task<SuccessResponse<GetDefaultFormDto>> CreateCustomForm(Guid id, CreateFormInputDto inputDto)
        {
            GetDefaultFormDto response = await CreateFormType(id, inputDto, EFormType.Custom);

            return new SuccessResponse<GetDefaultFormDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }
        public async Task<SuccessResponse<GetDefaultFormDto>> GetFormByType(Guid formId, string formType)
        {
            var form = await _repository.ActivityForm.FirstOrDefaultNoTracking(x => x.Id == formId);
            if (form == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

            if (form.Type == EActivityType.CallForApplication.ToString())
            {
                await CheckIfCfaIsClosed(form.ActivityId);
            }

            ActivityFormDto activityForm = await GetActivityType(form);

            var formFieldsQuery = _repository.ActivityForm.Get(x => x.Id == formId);

            if (!string.IsNullOrWhiteSpace(formType) && string.Equals(formType.Trim(), EFormType.Default.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                formFieldsQuery = formFieldsQuery
                 .Include(x => x.FormFields.Where(x => x.FormType == EFormType.Default.ToString()).OrderBy(x => x.Index))
                     .ThenInclude(x => x.FormFieldValue)
                 .Include(x => x.FormFields.Where(x => x.FormType == EFormType.Default.ToString()).OrderBy(x => x.Index))
                     .ThenInclude(x => x.Options);
            }

            if (!string.IsNullOrWhiteSpace(formType) && string.Equals(formType.Trim(), EFormType.Custom.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                formFieldsQuery = formFieldsQuery
                 .Include(x => x.FormFields.Where(x => x.FormType == EFormType.Custom.ToString()).OrderBy(x => x.Index))
                     .ThenInclude(x => x.FormFieldValue)
                 .Include(x => x.FormFields.Where(x => x.FormType == EFormType.Custom.ToString()).OrderBy(x => x.Index))
                     .ThenInclude(x => x.Options);
            }

            if (string.IsNullOrWhiteSpace(formType))
            {
                formFieldsQuery = formFieldsQuery
                    .Include(x => x.FormFields.OrderBy(x => x.Index))
                        .ThenInclude(x => x.FormFieldValue)
                    .Include(x => x.FormFields.OrderBy(x => x.Index))
                        .ThenInclude(x => x.Options);
            }

            var formFields = await formFieldsQuery.FirstOrDefaultAsync();

            var response = _mapper.Map<GetDefaultFormDto>(formFields);
            response.Activity = _mapper.Map<GetActivityDto>(activityForm);

            return new SuccessResponse<GetDefaultFormDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = response
            };
        }
        public async Task<SuccessResponse<GetDefaultFormDto>> CreateFormFieldValues(Guid formId, CreateFormFieldValueInputDto inputDto)
        {
            var form = await _repository.ActivityForm.Get(x => x.Id == formId)
                .Include(x => x.FormFields)
                    .ThenInclude(x => x.FormFieldValue)
                .Include(x => x.FormFields)
                    .ThenInclude(x => x.Options)
                .FirstOrDefaultAsync();

            if (form == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.FormNotFound);

            var learningTrack = new LearningTrack();
            if (inputDto.LearningTrackId != Guid.Empty)
            {
                learningTrack = await _repository.LearningTrack.FirstOrDefault(x => x.Id == inputDto.LearningTrackId);
                if (learningTrack is null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.LearningTrackNotFound);
            }

            var programmeId = await GetProgrammeId(form);
            
            ApplicantDetail applicantDetail = new();
            applicantDetail.Id = Guid.NewGuid();
            applicantDetail.FormId = form.Id;
            applicantDetail.ActivityType = form.Type;
            applicantDetail.ActivityId = form.ActivityId;
            applicantDetail.LearningTrackId = inputDto.LearningTrackId == Guid.Empty ? null : inputDto.LearningTrackId;
            applicantDetail.ProgrammeId = programmeId;
            applicantDetail.Status = EApplicantStatus.Pending.ToString();

            List<FormFieldValue> fieldValues = new();         
            foreach (var fieldValue in inputDto.FieldValues)
            {             
                var value = _mapper.Map<FormFieldValue>(fieldValue);
                value.FormId = formId;
                value.ApplicantDetailId = applicantDetail.Id;

                List<string> fileUrls = new();
                if (fieldValue.Files.Count > 0)
                {
                    foreach(var file in fieldValue.Files)
                    {
                        if (file is not null)
                        {
                            var fileUrl = await _awsS3Client.UploadFileAsync(file);
                            fileUrls.Add(fileUrl);
                        }
                    }
                    var urls = string.Join(";", fileUrls);
                    fieldValue.Value  = urls;
                }

                SetApplicantDetailProperty(fieldValue.Key, fieldValue.Value, applicantDetail);

                fieldValues.Add(value);
            }

            await _repository.Applicant.AddAsync(applicantDetail);
            await _repository.FormFieldValue.AddRangeAsync(fieldValues);
            await _repository.SaveChangesAsync();

            var activity = await _repository.Activity.FirstOrDefaultNoTracking(x => x.Id == form.ActivityId && x.Type == form.Type);
            var response = _mapper.Map<GetDefaultFormDto>(form);
            response.Activity = _mapper.Map<GetActivityDto>(activity);

            return new SuccessResponse<GetDefaultFormDto>
            {
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<GetFormStatusDto>> GetFormStatus(Guid formId)
        {
            var form = await _repository.ActivityForm.FirstOrDefault(x => x.Id == formId);
            if (form == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.FormNotFound);

            var activityForm = await GetActivityType(form);

            return new SuccessResponse<GetFormStatusDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = new GetFormStatusDto
                {
                    IsActivated = activityForm.Status == ECallForApplicationStatus.Activate.ToString(),
                }
            };
        }

        public async Task<SuccessResponse<GetApplicantFormDetailsDto>> GetApplicantFormDetails(Guid formId, Guid applicantId)
        {
            var form = await _repository.ActivityForm.FirstOrDefaultNoTracking(x => x.Id == formId);
            if (form == null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

            var applicant = await _repository.Applicant.FirstOrDefaultNoTracking(x => x.Id == applicantId);
            if (applicant is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ApplicantDetailNotFound);

            var formFields = await _repository.FormField.QueryAll(x => x.ActivityFormId == formId)
                .Include(x => x.FormFieldValue)
                .Include(x => x.Options)
                .OrderBy(x => x.Index)
                .Where(x => x.FormFieldValue.ApplicantDetailId == applicantId)
                .ToListAsync();

            var customFields = formFields.Where(x => x.FormType == EFormType.Custom.ToString()).ToList();
            var defaultFields = formFields.Where(x => x.FormType == EFormType.Default.ToString()).ToList();

            var response = new GetApplicantFormDetailsDto
            {
                FormId = formId,
                CustomFormFields = _mapper.Map<IEnumerable<GetFormFieldDto>>(customFields),
                DefaultFormFields = _mapper.Map<IEnumerable<GetFormFieldDto>>(defaultFields)
            };

            return new SuccessResponse<GetApplicantFormDetailsDto>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = response
            };
        }

        public async Task<SuccessResponse<GetApplicantFormStatusDto>> UpdateApplicantStage(Guid applicantId, Guid stageId)
        {
            ApplicantDetail applicant = await ValidateApplicant(applicantId);

            var stage = await _repository.Stage.FirstOrDefault(x => x.Id == stageId);
            if (stage is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.StageNotFound);

            var cfa = await _repository.CallForApplication.Get(x => x.Id == stage.CallForApplicationId)
                .Include(x => x.Programme)
                .FirstOrDefaultAsync();

            SetApplicantStatus(applicant, stage);
            SendApplicantEmail(cfa.Programme.Title, applicant.FirstName, stage, applicant.Email);

            applicant.StageId = stage.Id;

            UserActivity userActivity = AuditLog.UserActivity(stage, _webHelper.User().UserId, nameof(stage), $"Updated an Applicant stage - {stage.Name}", stage.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            _repository.Applicant.Update(applicant);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<GetApplicantFormStatusDto>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = _mapper.Map<GetApplicantFormStatusDto>(applicant)
            };
        }

        public async Task<SuccessResponse<GetBulkApplicantForStatusDto>> BulkUpdateApplicantStage(BulkApplicantStageDto model)
        {
            var stage = await _repository.Stage.FirstOrDefault(x => x.Id == model.StageId);
            if (stage is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.StageNotFound);

            var ids = model.Ids.Distinct();
            var applicants  = await _repository.Applicant.Get(x => ids.Contains(x.Id)).ToListAsync();
            var invalidApplicantIds = model.Ids.Except(applicants.Select(x => x.Id).ToArray()).ToArray();

            if (model.Ids.Length == invalidApplicantIds.Length)
            {
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ApplicantDetailsNotFound);
            }

            var cfa = await _repository.CallForApplication.Get(x => x.Id == stage.CallForApplicationId)
                .Include(x => x.Programme)
                .FirstOrDefaultAsync();

            foreach (var applicant in applicants)
            {
                SetApplicantStatus(applicant, stage);
                SendApplicantEmail(cfa.Programme.Title, applicant.FirstName, stage, applicant.Email);
                applicant.StageId = stage.Id;
            }

            UserActivity userActivity = AuditLog.UserActivity(stage, _webHelper.User().UserId, nameof(stage), $"Performed a bulk updated - {stage.Name}", stage.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            _repository.Applicant.UpdateRange(applicants);
            await _repository.SaveChangesAsync();

            return new SuccessResponse<GetBulkApplicantForStatusDto>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = new GetBulkApplicantForStatusDto
                {
                    InvalidApplicantIds = invalidApplicantIds,
                    Applicants = _mapper.Map<GetApplicantFormStatusDto[]>(applicants.ToArray())
                }
            };
        }

        public async Task<SuccessResponse<GetApplicantFormStatusDto>> ApproveOrRejectApplicant(Guid id, ApplicantStatusDto model)
        {
            ApplicantDetail applicant = await ValidateApplicant(id);
            var programme = await _repository.Programme.FirstOrDefaultNoTracking(x => x.Id == applicant.ProgrammeId);

            var applicantStatus = model.Status.Equals(EUpdateApplicantStatus.Approve.ToString(), StringComparison.OrdinalIgnoreCase)
                ? EApplicantStatus.Approved.ToString()
                : EApplicantStatus.Rejected.ToString();

            applicant.Status = applicantStatus;

            _repository.Applicant.Update(applicant);

            if (model.Status.Equals(EUpdateApplicantStatus.Approve.ToString(), StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(applicant.Email))
            {
                var isExist = await CheckIfEmailAlreadyExist(applicant);
                if (!isExist)
                {
                    var (user, role) = await CreateUser(applicant);
                    await InviteUser(user, programme.Title);
                    var applicantId = await CreateApprovedApplicant(applicant, user, role);
                    await CreateApplicantProgramme(applicant, applicantId);
                }
            }
            else
            {
                var message = _mailerService.GetBeneficiaryRejectionTemplate(applicant.FirstName, programme.Title);
                _mailerService.SendSingleEmail(applicant.Email, message, "Application Rejected");
            }

            var activity = model.Status.Equals(EUpdateApplicantStatus.Approve.ToString(), StringComparison.OrdinalIgnoreCase)
                ? "Approved an Applicant"
                : "Rejected an Applicant";
            UserActivity userActivity = AuditLog.UserActivity(applicant, _webHelper.User().UserId, nameof(applicant), $"{activity} - {applicant.FirstName} {applicant.LastName}", applicant.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.SaveChangesAsync();


            return new SuccessResponse<GetApplicantFormStatusDto>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = _mapper.Map<GetApplicantFormStatusDto>(applicant)
            };
        }

        public async Task<SuccessResponse<GetBulkApplicantForStatusDto>> BulkApproveOrRejectApplicants(BulkApplicantStatusDto model)
        {
            var ids = model.Ids.Distinct();
            var applicants = await _repository.Applicant.Get(x => ids.Contains(x.Id)).ToListAsync();
            var invalidApplicantIds = model.Ids.Except(applicants.Select(x => x.Id).ToArray()).ToArray();

            if (model.Ids.Length == invalidApplicantIds.Length)
            {
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ApplicantDetailsNotFound);
            }

            var applicantStatus = model.Status.Equals(EUpdateApplicantStatus.Approve.ToString(), StringComparison.OrdinalIgnoreCase)
                ? EApplicantStatus.Approved.ToString()
                : EApplicantStatus.Rejected.ToString();

            foreach (var applicant in applicants)
            {
                applicant.Status = applicantStatus;
            }

            _repository.Applicant.UpdateRange(applicants);

            await InviteUserAndCreateApprovedApplicant(model, applicants);

            await _repository.SaveChangesAsync();

            return new SuccessResponse<GetBulkApplicantForStatusDto>
            {
                Message = ResponseMessages.UpdateSuccessResponse,
                Data = new GetBulkApplicantForStatusDto
                {
                    InvalidApplicantIds = invalidApplicantIds,
                    Applicants = _mapper.Map<GetApplicantFormStatusDto[]>(applicants.ToArray())
                }
            };
        }

        private async Task InviteUserAndCreateApprovedApplicant(BulkApplicantStatusDto model, List<ApplicantDetail> applicants)
        {
            foreach (var applicant in applicants)
            {
                if (model.Status.Equals(EUpdateApplicantStatus.Approve.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(applicant.Email))
                    {
                        var isExist = await CheckIfEmailAlreadyExist(applicant);
                        if (!isExist)
                        {
                            var programme = await _repository.Programme.FirstOrDefaultNoTracking(x => x.Id == applicant.ProgrammeId);

                            var (user, role) = await CreateUser(applicant);
                            await InviteUser(user, programme.Title);
                            var applicantId = await CreateApprovedApplicant(applicant, user, role);
                            await CreateApplicantProgramme(applicant, applicantId);
                        }

                    }
                }
                else
                {
                    var programme = await _repository.Programme.FirstOrDefaultNoTracking(x => x.Id == applicant.ProgrammeId);
                    var message = _mailerService.GetBeneficiaryRejectionTemplate(applicant.FirstName, programme.Title);

                    _mailerService.SendSingleEmail(applicant.Email, message, "Application Rejected");
                }
            }
        }

        public async Task<string> GetActivityTarget(Guid activityId, string activityType)
        {
            if (activityType.Equals(EActivityType.CallForApplication.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var cfa = await _repository.CallForApplication.FirstOrDefaultNoTracking(x => x.Id == activityId);
                if (cfa is not null) return cfa.Target;
            }
            if (activityType.Equals(EActivityType.Assessment.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var assessment = await _repository.Assessment.FirstOrDefaultNoTracking(x => x.Id == activityId);
                if (assessment is not null) return assessment.Target;
            }
            if (activityType.Equals(EActivityType.Forms.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var form = await _repository.Form.FirstOrDefaultNoTracking(x => x.Id == activityId);
                if (form is not null) return form.Target;
            }
            if (activityType.Equals(EActivityType.Survey.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var survey = await _repository.Survey.FirstOrDefaultNoTracking(x => x.Id == activityId);
                if (survey is not null) return survey.Target;
            }
            if (activityType.Equals(EActivityType.Training.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return string.Empty;
        }

        public async Task<(User user, string role)> CreateUser(ApplicantDetail applicant)
        {
            var organizationId = _webHelper.User().OrganizationId;

            var user = new User
            {
                FirstName = applicant.FirstName,
                LastName = applicant.LastName,
                Email = applicant.Email,
                Status = EUserStatus.Pending.ToString(),
                UserName = applicant.Email,
                Verified = false,
                OrganizationId = organizationId,
                PhoneNumber = applicant.PhoneNumber,
            };

            var password = _userManager.PasswordHasher.HashPassword(user, "Password@1");
            user.Password = password;

            var result = await _userManager.CreateAsync(user, "Password@1");
            if (!result.Succeeded)
                throw new RestException(HttpStatusCode.BadRequest, result.Errors.FirstOrDefault().Description);

            var role = await GetActivityTarget(applicant.ActivityId, applicant.ActivityType);
            await _userManager.AddToRoleAsync(user, role);


            var userActivity = new UserActivity
            {
                EventType = nameof(ApproveOrRejectApplicant),
                UserId = user.Id,
                ObjectClass = "USER",
                Details = "Signed up",
                ObjectId = user.Id
            };

            await _repository.UserActivity.AddAsync(userActivity);

            return (user, role);
        }
        
        public async Task<SuccessResponse<GetDefaultFormDto>> SubmitActivityFormValues(Guid approvedApplicantId, SubmitActivityFormFieldValueDto model)
        {
            var approvedApplicant = _repository.ApprovedApplicant.FirstOrDefaultNoTracking(x => x.Id == approvedApplicantId);
            if (approvedApplicant == null)
                throw new RestException(HttpStatusCode.NotFound, "User not found");

            var activityForm = await _repository.ActivityForm.Get(x => x.ActivityId == model.ActivityId)
               .Include(x => x.FormFields)
                   .ThenInclude(x => x.FormFieldValue)
               .Include(x => x.FormFields)
                   .ThenInclude(x => x.Options)
               .FirstOrDefaultAsync();
            if (activityForm == null)
                throw new RestException(HttpStatusCode.NotFound, "Form not found");

            bool isSubmissionApproved = await CanSubmitActivityForm(activityForm.Type, activityForm.ActivityId, approvedApplicantId);
            if (!isSubmissionApproved)
                throw new RestException(HttpStatusCode.Unauthorized, "You are not autorised to submit this form");

            List<FormFieldValue> fieldValues = new();
            foreach (var fieldValue in model.FieldValues)
            {
                var value = _mapper.Map<FormFieldValue>(fieldValue);
                value.FormId = activityForm.ActivityId;
                value.ApplicantDetailId = approvedApplicantId;

                List<string> fileUrls = new();
                if (fieldValue.Files.Count > 0)
                {
                    foreach (var file in fieldValue.Files)
                    {
                        if (file is not null)
                        {
                            var fileUrl = await _awsS3Client.UploadFileAsync(file);
                            fileUrls.Add(fileUrl);
                        }
                    }
                    var urls = string.Join(";", fileUrls);
                    fieldValue.Value = urls;
                }
                fieldValues.Add(value);
            }

            //UserActivity userActivity = AuditLog.UserActivity(fieldValues, _webHelper.User().UserId, "activity", $"Submtted activityForm under - {programme.Title}", model.ActivityId);

            await _repository.FormFieldValue.AddRangeAsync(fieldValues);
            await RecordActivityResponse(activityForm.Type, activityForm.ActivityId, approvedApplicantId);
            //await _repository.UserActivity.AddAsync(userActivity);
            await _repository.SaveChangesAsync();

            var activity = await _repository.Activity.FirstOrDefaultNoTracking(x => x.Id == activityForm.ActivityId && x.Type == activityForm.Type);
            var response = _mapper.Map<GetDefaultFormDto>(activityForm);
            response.Activity = _mapper.Map<GetActivityDto>(activity);

            return new SuccessResponse<GetDefaultFormDto>
            { 
                Message = ResponseMessages.CreationSuccessResponse,
                Data = response
            };
        }
        #endregion

        #region Reuseables
        private Guid ValidateUser()
        {
            var organizationId = _webHelper.User().OrganizationId;
            if (organizationId == Guid.Empty)
                throw new RestException(HttpStatusCode.Unauthorized, ResponseMessages.UnAuthorized);

            return organizationId;
        }

        private async Task<ActivityFormDto> GetActivityType(ActivityForm form)
        {
            ActivityFormDto activityForm;
            if (form.Type == EActivityType.CallForApplication.ToString())
            {
                var activityType = await _repository.CallForApplication.Get(x => x.Id == form.ActivityId)
                    .Include(x => x.Programme)
                    .FirstOrDefaultAsync();
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                activityForm = _mapper.Map<ActivityFormDto>(activityType);
                activityForm.Type = EActivityType.CallForApplication.ToString();
            }
            else if (form.Type == EActivityType.Training.ToString())
            {
                var activityType = await _repository.Training.Get(x => x.Id == form.ActivityId)
                    .Include(x => x.Programme)
                    .FirstOrDefaultAsync();
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                activityForm = _mapper.Map<ActivityFormDto>(activityType);
                activityForm.Type = EActivityType.Training.ToString();
            }
            else if (form.Type == EActivityType.Survey.ToString())
            {
                var activityType = await _repository.Survey.Get(x => x.Id == form.ActivityId)
                    .Include(x => x.Programme)
                    .FirstOrDefaultAsync();
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                activityForm = _mapper.Map<ActivityFormDto>(activityType);
                activityForm.Type = EActivityType.Survey.ToString();
            }
            else if (form.Type == EActivityType.Events.ToString())
            {
                var activityType = await _repository.Event.Get(x => x.Id == form.ActivityId)
                    .Include(x => x.Programme)
                    .FirstOrDefaultAsync();

                activityForm = _mapper.Map<ActivityFormDto>(activityType);
                activityForm.Type = EActivityType.Events.ToString();
            }
            else if (form.Type == EActivityType.Assessment.ToString())
            {
                var activityType = await _repository.Assessment.Get(x => x.Id == form.ActivityId)
                    .Include(x => x.Programme)
                    .FirstOrDefaultAsync();
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                activityForm = _mapper.Map<ActivityFormDto>(activityType);
                activityForm.Type = EActivityType.Assessment.ToString();
            }
            else
            {
                var activityType = await _repository.Form.Get(x => x.Id == form.ActivityId)
                    .Include(x => x.Programme)
                    .FirstOrDefaultAsync();
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                activityForm = _mapper.Map<ActivityFormDto>(activityType);
                activityForm.Type = EActivityType.Forms.ToString();
            }

            return activityForm;
        }
        private async Task<GetActivityDto> GetActivityType(Guid id, CreateFormInputDto inputDto)
        {
            GetActivityDto activity;
            if (inputDto.ActivityType == EActivityType.CallForApplication.ToString())
            {
                var activityType = await _repository.CallForApplication.FirstOrDefault(x => x.Id == id);
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                activity = _mapper.Map<GetActivityDto>(activityType);
                activity.Type = EActivityType.CallForApplication.ToString();
            }
            else if (inputDto.ActivityType == EActivityType.Events.ToString())
            {
                var activityType = await _repository.Event.FirstOrDefault(x => x.Id == id);
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                activity = _mapper.Map<GetActivityDto>(activityType);
                activity.Type = EActivityType.Events.ToString();
            }
            else if (inputDto.ActivityType == EActivityType.Assessment.ToString())
            {
                var activityType = await _repository.Assessment.FirstOrDefault(x => x.Id == id);
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                activity = _mapper.Map<GetActivityDto>(activityType);
                activity.Type = EActivityType.Assessment.ToString();
            }
            else if (inputDto.ActivityType == EActivityType.Forms.ToString())
            {
                var activityType = await _repository.Form.FirstOrDefault(x => x.Id == id);
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                activity = _mapper.Map<GetActivityDto>(activityType);
                activity.Type = EActivityType.Forms.ToString();
            }
            else if (inputDto.ActivityType == EActivityType.Survey.ToString())
            {
                var activityType = await _repository.Survey.FirstOrDefault(x => x.Id == id);
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                activity = _mapper.Map<GetActivityDto>(activityType);
                activity.Type = EActivityType.Survey.ToString();
            }
            else
            {
                var activityType = await _repository.Training.FirstOrDefault(x => x.Id == id);
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                activity = _mapper.Map<GetActivityDto>(activityType);
                activity.Type = EActivityType.Training.ToString();
            }

            return activity;
        }
        private async Task<ApplicantDetail> ValidateApplicant(Guid applicantId)
        {
            var applicant = await _repository.Applicant.FirstOrDefault(x => x.Id == applicantId);
            if (applicant is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ApplicantDetailNotFound);

            return applicant;
        }
        private async Task CreateApplicantProgramme(ApplicantDetail applicant, Guid applicantId)
        {
            var beneficiaryProgramme = new ApprovedApplicantProgramme
            {
                ApprovedApplicantId = applicantId,
                ProgrammeId = applicant.ProgrammeId,
                LearningTrackId = (Guid)applicant.LearningTrackId
            };

            await _repository.ApprovedApplicantProgramme.AddAsync(beneficiaryProgramme);
            return;
        }
        private async Task<Guid> CreateApprovedApplicant(ApplicantDetail applicant, User user, string role)
        {
            var approvedApplicant = _mapper.Map<ApprovedApplicant>(applicant);
            approvedApplicant.UserId = user.Id;
            approvedApplicant.Role = role;

            await _repository.ApprovedApplicant.AddAsync(approvedApplicant);

            return approvedApplicant.Id;
        }

        private async Task<bool> CheckIfEmailAlreadyExist(ApplicantDetail applicant)
        {
            var email = applicant.Email.Trim().ToLower();
            var isEmailExist = await _repository.User.ExistsAsync(x => x.Email == applicant.Email);
            if (isEmailExist)
                return true;

            return false;
        }

        private async Task InviteUser(User user, string programmeTitle)
        {
            var token = CustomToken.GenerateRandomString(128);
            var tokenEntity = new Token
            {
                UserId = user.Id,
                Value = token,
                TokenType = ETokenType.InviteUser.ToString()
            };

            await _repository.Token.AddAsync(tokenEntity);

            string emailLink = $"{_configuration["CLIENT_URL"]}/signup?token={token}";
            var message = _mailerService.GetBeneficiaryTemplate(emailLink, user.FirstName, programmeTitle);

            _mailerService.SendSingleEmail(user.Email, message, "Application Approved");
        }

        private static void SetApplicantStatus(ApplicantDetail applicant, Stage stage)
        {
            applicant.Status = EApplicantStatus.InReview.GetDescription();
        }

        private void SendApplicantEmail(string programmeName, string name, Stage stage, string email)
        {
            if (stage.Name.Equals(EStageDefaultStatus.Approve.ToString()))
            {
                var message = _mailerService.GetApplicantEmailTemplate(programmeName, name);
                string subject = "Nakise";

                _mailerService.SendSingleEmail(email, message, subject);
            }
        }

        private async Task<Guid> GetProgrammeId(ActivityForm form)
        {
            Guid? programmeId = null;
            if (form.Type.Equals(EActivityType.CallForApplication.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var cfa = await _repository.CallForApplication.FirstOrDefaultNoTracking(x => x.Id == form.ActivityId);
                programmeId = cfa?.ProgrammeId;
            }
            if (form.Type.Equals(EActivityType.Assessment.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var assessment = await _repository.Assessment.FirstOrDefaultNoTracking(x => x.Id == form.ActivityId);
                programmeId = assessment?.ProgrammeId;
            }
            if (form.Type.Equals(EActivityType.Events.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var eventEntity = await _repository.Event.FirstOrDefaultNoTracking(x => x.Id == form.ActivityId);
                programmeId = eventEntity?.ProgrammeId;
            }
            if (form.Type.Equals(EActivityType.Forms.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var formEntity = await _repository.Form.FirstOrDefaultNoTracking(x => x.Id == form.ActivityId);
                programmeId = formEntity?.ProgrammeId;
            }
            if (form.Type.Equals(EActivityType.Survey.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var survey = await _repository.Survey.FirstOrDefaultNoTracking(x => x.Id == form.ActivityId);
                programmeId = survey?.ProgrammeId;
            }
            if (form.Type.Equals(EActivityType.Training.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var training = await _repository.Training.FirstOrDefaultNoTracking(x => x.Id == form.ActivityId);
                programmeId = training?.ProgrammeId;
            }

            return programmeId.Value;
        }

        private async Task<GetDefaultFormDto> CreateFormType(Guid id, CreateFormInputDto inputDto, EFormType formType)
        {
            var activityForm = await _repository.ActivityForm.FirstOrDefault(x => x.Id == inputDto.FormId);
            if (activityForm is null)
                throw new RestException(HttpStatusCode.NotFound, ResponseMessages.FormNotFound);

            var formFields = await _repository.FormField.QueryAll(x => x.ActivityFormId == inputDto.FormId && x.FormType == formType.ToString()).ToListAsync();
            if (formFields.Any())
                _repository.FormField.RemoveRange(formFields);

            GetActivityDto activity = await GetActivityType(id, inputDto);

            var fields = new List<FormField>();
            var fieldOptions = new List<FieldOption>();
            foreach (var inputField in inputDto.FormFields)
            {
                var formField = _mapper.Map<FormField>(inputField);
                formField.Id = Guid.NewGuid();
                formField.FormType = formType.ToString();
                formField.ActivityFormId = activityForm.Id;
                formField.FileNumberLimit = inputField.File?.NumberLimit;
                formField.SingleFileSizeLimit = inputField.File?.SingleFileSizeLimit;
                formField.FileType = inputField.File?.Type;

                fields.Add(formField);

                if (inputField.Options is not null)
                {
                    if (inputField.Options.Any())
                    {
                        foreach (var inputOption in inputField.Options)
                        {
                            var fieldOption = _mapper.Map<FieldOption>(inputOption);
                            fieldOption.Id = Guid.NewGuid();
                            fieldOption.FormFieldId = formField.Id;

                            fieldOptions.Add(fieldOption);
                        }
                        await _repository.FieldOption.AddRangeAsync(fieldOptions);
                    }
                }
            }

            UserActivity userActivity = AuditLog.UserActivity(activityForm, _webHelper.User().UserId, nameof(activityForm), $"Created an application form - {activityForm.Name}", activityForm.Id);
            await _repository.UserActivity.AddAsync(userActivity);

            await _repository.FormField.AddRangeAsync(fields);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<GetDefaultFormDto>(activityForm);
            response.Activity = activity;
            return response;
        }
        private async Task CheckIfCfaIsClosed(Guid id)
        {
            var cfa = await _repository.CallForApplication.GetByIdAsync(id);
            if (cfa is not null)
            {
                if (cfa.EndDate.Date < DateTime.Now.Date)
                {
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.CallForApplicationClosed);
                }
            }
        }

        private static void SetApplicantDetailProperty(string propertyName, string value, object obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);

            if (propertyInfo != null)
            {
                if (propertyInfo.Name.Equals("DateOfBirth", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        var dob = DateTime.Parse(value);
                        propertyInfo.SetValue(obj, dob, null);

                        return;
                    }
                }

                propertyInfo.SetValue(obj, value, null);
            }
        }

        private async Task<bool> CanSubmitActivityForm(string activityType, Guid activityId, Guid ApprovedApplicantId)
        {
            Guid programmeId = Guid.Empty;
            if (activityType == EActivityType.Training.ToString())
            {
                var activity = await _repository.Training.Get(x => x.Id == activityId)
                    .Include(x => x.Programme)
                       .Include(z => z.TrainingLearningTracks)
                          .FirstOrDefaultAsync();
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                programmeId = activity.ProgrammeId;
            }
            else if (activityType == EActivityType.Survey.ToString())
            {
                var activity = await _repository.Survey.Get(x => x.Id == activityId)
                    .Include(x => x.Programme)
                       .Include(z => z.SurveyLearningTracks)
                          .FirstOrDefaultAsync();
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                programmeId = activity.ProgrammeId;
            }
            else if (activityType == EActivityType.Events.ToString())
            {
                var activity = await _repository.Event.Get(x => x.Id == activityId)
                    .Include(x => x.Programme)
                       .Include(z => z.EventLearningTracks)
                          .FirstOrDefaultAsync();
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                programmeId = activity.ProgrammeId;
            }
            else if (activityType == EActivityType.Assessment.ToString())
            {
                var activity = await _repository.Assessment.Get(x => x.Id == activityId)
                    .Include(x => x.Programme)
                       .Include(z => z.AssessmentLearningTracks)
                          .FirstOrDefaultAsync();
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                programmeId = activity.ProgrammeId;
            }
            else if(activityType == EActivityType.Forms.ToString())
            {
                var activity = await _repository.Form.Get(x => x.Id == activityId)
                    .Include(x => x.Programme)
                       .Include(z => z.FormLearningTracks)
                           .FirstOrDefaultAsync();
                if (activityType == null)
                    throw new RestException(HttpStatusCode.NotFound, ResponseMessages.ActivityNotFound);

                programmeId = activity.ProgrammeId;
            }
            else
            {
                return false;
            }

            var approvedProgram = await _repository.ApprovedApplicantProgramme.Get(x =>
                    x.ApprovedApplicantId == ApprovedApplicantId && x.ProgrammeId == programmeId)
                       .ToArrayAsync();
            if (approvedProgram == null)
                return false;

            return true;
        }

        private async Task RecordActivityResponse(string activityType, Guid activityId, Guid ApprovedApplicantId)
        {
            if (activityType == EActivityType.Training.ToString())
            {
                var activity = await _repository.Training.Get(x => x.Id == activityId)
                    .Include(x => x.Programme)
                          .FirstOrDefaultAsync();

            }
            else if (activityType == EActivityType.Survey.ToString())
            {
                var activity = await _repository.Survey.Get(x => x.Id == activityId)
                    .Include(x => x.Programme)
                          .FirstOrDefaultAsync();

                SurveyResponse surveyResponse = new()
                {
                    ProgramId = activity.ProgrammeId,
                    SurveyId = activityId,
                    ApprovedApplicantId = ApprovedApplicantId,
                    CreatedAt = DateTime.Now,
                };
                await _repository.SurveyResponse.AddAsync(surveyResponse);
            }
            else if (activityType == EActivityType.Events.ToString())
            {
                var activity = await _repository.Event.Get(x => x.Id == activityId)
                    .Include(x => x.Programme)
                          .FirstOrDefaultAsync();

            }
            else if (activityType == EActivityType.Assessment.ToString())
            {
                var activity = await _repository.Assessment.Get(x => x.Id == activityId)
                    .Include(x => x.Programme)
                          .FirstOrDefaultAsync();

            }
            else if (activityType == EActivityType.Forms.ToString())
            {
                var activity = await _repository.Form.Get(x => x.Id == activityId)
                    .Include(x => x.Programme)
                           .FirstOrDefaultAsync();

            }
        }

        #endregion
    }
}
