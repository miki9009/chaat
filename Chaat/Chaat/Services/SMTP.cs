using System;
using System.Threading.Tasks;

using MimeKit;
using MailKit.Net.Smtp;

namespace Chaat.Services
{
    public class SMTP : IEmailSender
    {
        public static string host = "smtp-mail.outlook.com";
        public static Int16 port = 587;
        public static bool SSL = true;
        public static string userMail = "e.kancelaria@outlook.com";
        public static string userName = "Mikolaj";
        public static string password = "Galicjanka1";

        public async Task SendEmailAsync(string email, string subject, string messageHtml)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(userName, userMail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = messageHtml;
            emailMessage.Body = bodyBuilder.ToMessageBody();
            /*
            emailMessage.Body = new Html("plain")
            {
                Text = message
            };*/

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(host, port, false);
                await client.AuthenticateAsync(userMail, password, System.Threading.CancellationToken.None);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public void SendMailAsync(string to, string subject, string message)
        {

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(userName, userMail));
            emailMessage.To.Add(new MailboxAddress("", to));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                client.Connect(host, port, false);
                client.Authenticate(userMail, password, System.Threading.CancellationToken.None);
                client.Send(emailMessage);
                client.Disconnect(true);
            }

        }
    }
}
