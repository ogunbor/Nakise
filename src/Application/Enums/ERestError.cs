﻿namespace Application.Enums
{
    public enum ERestError
    {
        DuplicateEmail,
        CreationSuccessResponse,
        LoginSuccessResponse,
        WrongEmailOrPassword,
        UserNotFound,
        RetrievalSuccessResponse,
        InvalidToken,
        InvalidExpiredToken,
        TokenExpired,
        UnAuthorized,
        RoleNotFound,
        DuplicateOrganization,
        OrganizationDoesNotExist,
        UpdateSuccessResponse,
        DeleteSuccessResponse,
        TokenConfirmedSuccessfully,
        PasswordSetSuccessfully,
        PasswordResetSuccessfully,
        ManagerNotFound,
        ProgrammeNotFound,
        DeletionSuccessResponse,
        FileFailedToUpload,
        DuplicateLearningTrack,
        LearningTrackNotFound,
        FacilitatorNotFound,
        DuplicateProgrammeSponsor,
        DuplicateTitle,
        InvalidActivity,
        FormNotFound,
        ActivityNotFound,
        InvalidDate,
        CallForApplicationCreatedSuccessfully,
        CallForApplicationUpdatedSuccessfully,
        CallForApplicationNotFound,
        ProgrammeInProgress,
        AssessmentNotFound,
        SurveyNotFound,
        EventNotFound,
        LearningTrackIdNotFound,
        TrainingNotFound,
        ApplicantDetailNotFound,
        StageNotFound,
        ApplicantDetailsNotFound,
        InternalServerError,
        UnsupportedMediaType,
        LinkNotAvailable,
        CallForApplicationClosed
    }
}
