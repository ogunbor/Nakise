namespace Infrastructure.Utils.Email
{
    public interface IEmailManager
    {
        void SendSingleEmail(string receiverAddress, string message, string subject);
        void SendBulkEmail(string [] receiverAddress, string message, string subject);
        void SendSingleEmailWithAttachment(string receiverAddress, string message, string subject, string fileName, 
            string fileContent, string type);
        string GetConfirmEmailTemplate(string emailLink, string email);
        string GetEmployeeWelcomeEmailTemplate(string companyName, string firstname, string lastname, string email, string emailLink);
        string GetResetPasswordEmailTemplate(string emailLink, string email);
        string GetApplicantEmailTemplate(string programmeName, string name);
        string GetApprovedApplicantProfileSetupEmailTemplate(string profileLink, string email);
        string GetBeneficiaryTemplate(string emailLink, string name, string programTitle);
        string GetBeneficiaryRejectionTemplate(string name, string programTitle);
        string GetProgrammeManagerInvitationTemplate(string emailLink, string name);
    }
}
