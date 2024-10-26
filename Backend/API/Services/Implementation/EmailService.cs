
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Backend.API.Services.Interface
{
    public class EmailService: IEmailService
    {
        private string address = string.Empty;
        private string password = string.Empty;
        private bool disposedValue;

        public EmailService(IConfiguration configuration)
        {
            this.address = configuration.GetValue<string>("Email:Address")!;
            this.password = configuration.GetValue<string>("Email:AccessKey")!;
        }

        public async Task SendEmailAsync(string address, string subject, string body)
        {
            SmtpClient mailClient = new SmtpClient();

            // Setup Smtp client
            mailClient.Host = "smtp.gmail.com.";
            mailClient.Port = 25;
            mailClient.UseDefaultCredentials = false;
            mailClient.Credentials = new NetworkCredential(this.address, password);
            mailClient.EnableSsl = true;

            MailMessage mail = new MailMessage();

            // Setup email configuration
            mail.From = new MailAddress(this.address);
            mail.To.Add(new MailAddress(address, "EventP Manager"));
            mail.SubjectEncoding = Encoding.UTF8;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;

            // Setup email content
            mail.Subject = subject;
            mail.Body = body;
            
            await mailClient.SendMailAsync(mail);
        }

        public async Task SendEmailAsync(IEnumerable<string> target_list, string subject, string body)
        {
            SmtpClient mailClient = new SmtpClient();

            // Setup Smtp client
            mailClient.Host = "smtp.gmail.com";
            mailClient.Port = 465;
            mailClient.Credentials = new NetworkCredential(address, password);
            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailClient.EnableSsl = true;

            MailMessage mail = new MailMessage();

            // Setup email configuration
            mail.From = new MailAddress(address, "EventP", Encoding.UTF8);

            foreach (string target in target_list)
            {
                mail.To.Add(new MailAddress(target));
            }
            
            mail.SubjectEncoding = Encoding.UTF8;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;

            // Setup email content
            mail.Subject = subject;
            mail.Body = body;
            
            await mailClient.SendMailAsync(mail);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
