// Services/GmailService.cs
/*using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Ankets.Services
{
    public class GmailService
    {
        private readonly IConfiguration _configuration;
        private const string ApplicationName = "Ankets API";

        public GmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var gmailApi = _configuration.GetSection("GmailApi");

            var credential = GoogleCredential.FromJson($$"""
            {
                "client_id": "{{gmailApi["ClientId"]}}",
                "client_secret": "{{gmailApi["ClientSecret"]}}",
                "refresh_token": "{{gmailApi["RefreshToken"]}}",
                "type": "authorized_user"
            }
            """)
            .CreateScoped(GmailService.Scope.GmailSend);

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Ankets", gmailApi["UserEmail"]));
            mimeMessage.To.Add(MailboxAddress.Parse(toEmail));
            mimeMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            var gmailMessage = new Message
            {
                Raw = Base64UrlEncode(mimeMessage.ToString())
            };

            await service.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
        }

        private static string Base64UrlEncode(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }
    }
}*/