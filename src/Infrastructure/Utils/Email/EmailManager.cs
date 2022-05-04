using Hangfire;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Utils.Email
{
    public class EmailManager : IEmailManager
    {
        private readonly SendGridClient _clientKey;
        private readonly IConfiguration _config;
        private readonly EmailAddress _from;

        public EmailManager(IConfiguration configuration)
        {
            _config = configuration;
            var sendGridKey = configuration["SENDGRID_KEY"];
            var senderEmail = configuration["SENDER_EMAIL"];

            _clientKey = new SendGridClient(sendGridKey);
            _from = new EmailAddress(senderEmail);
        }


        public void SendBulkEmail(string[] receiverAddress, string message, string subject)
        {
            BackgroundJob.Enqueue(() => SendBulkMail(receiverAddress, message, subject));
        }

        public void SendSingleEmail(string receiverAddress, string message, string subject)
        {
            BackgroundJob.Enqueue(()=> SendSingleMail(receiverAddress, message, subject));
        }

        public void SendSingleEmailWithAttachment(string receiverAddress, string message, string subject, string fileName, string fileContent, string type)
        {
            BackgroundJob.Enqueue(() => SendSingleMailWithAttachment(receiverAddress, message, subject, fileName, fileContent, type ));
        }
        
        public async Task SendSingleMail(string receiverAddress, string message, string subject)
        {
           
            var To = new EmailAddress(receiverAddress);
            var plainText = message;
            var htmlContent = message;

            var msg = MailHelper.CreateSingleEmail(_from, To, subject, plainText, htmlContent);
           var response  =  await _clientKey.SendEmailAsync(msg);

            //Throw an exception if the response is not successful, so that hangfire can retry
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());

        }

        public async Task SendBulkMail(string[] receiverAddress, string message, string subject)
        {
            var Tos = new List<EmailAddress>();

            foreach (var item in receiverAddress)
                Tos.Add(new EmailAddress(item));
            

            var plainText = "";
            var htmlContent = @$"
                <html><body><p>{message}</p></body></html>
            ";

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(_from,Tos, subject,plainText,htmlContent);
            var response = await _clientKey.SendEmailAsync(msg);

            //Throw an exception if the response is not successful, so that hangfire can retry
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());

        }

        /// <summary>
        /// Access modifier should be public for Background service
        /// <see cref="https://stackoverflow.com/questions/54437221/how-to-resolve-only-public-methods-can-be-invoked-in-the-background-in-hangfire"/>
        /// </summary>
        public async Task SendSingleMailWithAttachment(string receiverAddress, string message, string subject,
            string fileName, string fileContent, string type = "application/pdf")
        {
            var To = new EmailAddress(receiverAddress);
            var plainText = message;
            var htmlContent = message;
            var msg = MailHelper.CreateSingleEmail(_from, To, subject, plainText, htmlContent);


            var attachment = new Attachment
            {
                Content = fileContent,
                ContentId = Guid.NewGuid().ToString(),
                Disposition = "inline",
                Filename = fileName,
                Type = type

            };

            msg.AddAttachment(attachment);
            var response = await _clientKey.SendEmailAsync(msg);

            //Throw an exception if the response is not successful, so that hangfire can retry
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());
        }
        
        public string GetEmployeeWelcomeEmailTemplate(string companyName, string firstName, 
            string lastName, string email, string emailLink)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "WelcomeEmployee.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{companyName}", companyName)
                .Replace("{firstName}", firstName)
                .Replace("{lastName}", lastName)
                .Replace("{email_link}", emailLink)
                .Replace("{email}", email);
            return msgBody;
        }

        public string GetConfirmEmailTemplate(string emailLink, string email)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "ConfirmEmail.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{email_link}", emailLink).
                Replace("{email}", email);
            
            return msgBody;
        }
        public string GetProgrammeManagerInvitationTemplate(string emailLink, string name)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "ProgrammeManagerInvitation.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{email_link}", emailLink).
                Replace("{name}", name);

            return msgBody;
        }

        public string GetBeneficiaryTemplate(string emailLink, string name, string programTitle)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "BeneficiaryWelcome.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{name}", name)
                .Replace("{email_link}", emailLink)
                .Replace("{programTitle}", programTitle);

            return msgBody;
        }

        public string GetBeneficiaryRejectionTemplate(string name, string programTitle)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "BeneficiaryRejection.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{name}", name)
                .Replace("{programTitle}", programTitle);

            return msgBody;
        }

        public string GetResetPasswordEmailTemplate(string emailLink, string email)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "ResetPassword.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{email_link}", emailLink).
                Replace("{email}", email);

            return msgBody;
        }

        public string GetApplicantEmailTemplate(string programmeName, string name)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "ApplicantEmail.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{{programme_name}}", programmeName).
                Replace("{{name}}", name);

            return msgBody;
        }

        public string GetApprovedApplicantProfileSetupEmailTemplate(string profileLink, string email)
        {
            string body;
            var folderName = Path.Combine("wwwroot", "Templates", "ApprovedApplicantProfileSetup.html");
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (File.Exists(filepath))
                body = File.ReadAllText(filepath);
            else
                return null;

            string msgBody = body.Replace("{profile_link}", profileLink).
                Replace("{email}", email);

            return msgBody;
        }

    }
}
