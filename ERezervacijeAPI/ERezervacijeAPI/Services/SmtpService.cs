using System.Net.Mail;
using System.Net;
using ERezervacijeAPI.Helpers;

namespace ERezervacijeAPI.Services
{
    public class SmtpService
    {
        private readonly SmtpSettings settings;
        public SmtpService(SmtpSettings _settings)
        {
            settings = _settings;
        }
        public async Task<bool> sendEmail(string subject, string tekst, string emailTo)
        {
            var body = $"<div style='width:100%;height:100px;background-color:rgb(115,82,1);margin-bottom:10px;'>" +
                $"<p style='margin:0;color:rgb(115,82,1)'>1</p> <img style='display:block;width:200px;margin-left:auto;margin-right:auto;' src='{AddressesHelper.ApiAddress}GetLogo'>" +
                $"</div>";
            body += tekst;
            var mailMessage = new MailMessage
            {
                From = new MailAddress(settings.SenderEmail, settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(emailTo);
            try
            {
                var client = new SmtpClient(settings.Server, settings.Port);

                client.Credentials = new NetworkCredential(settings.Username, settings.Password);
                client.EnableSsl = true;

                await client.SendMailAsync(mailMessage);
            }catch(Exception ex)
            {
               return false;
            }
            return true;
        }
    }
}
