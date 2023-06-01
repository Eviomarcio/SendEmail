using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SendEmail.Entities
{
    public class Email
    {
        public string Provider { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }

        public Email(string provider, string userName, string password)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public void SendEmail(List<string> emailsTo, string subject, string body, List<string> attachments = null)
        {
            var message = PreparateMessage(emailsTo, subject, body, attachments);
            SendEmailBySmtp(message);
        }

        private MailMessage PreparateMessage(List<string> emailsTo, string subject, string body, List<string> attachments)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(UserName);

            foreach (var email in emailsTo)
            {
                if (ValidateEmail(email))
                {
                    mail.To.Add(email);
                }
            }

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            if (attachments != null && attachments.Any())
            {
                AttachFiles(mail, attachments);
            }

            return mail;
        }

        private bool ValidateEmail(string email)
        {
            Regex expression = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (expression.IsMatch(email))
            {
                return true;
            }

            return false;
        }

        private void AttachFiles(MailMessage mail, List<string> attachments)
        {
            foreach (var file in attachments)
            {
                var data = new Attachment(file, MediaTypeNames.Application.Octet);
                ContentDisposition disposition = data.ContentDisposition;
                disposition.CreationDate = File.GetCreationTime(file);
                disposition.ModificationDate = File.GetLastWriteTime(file);
                disposition.ReadDate = File.GetLastAccessTime(file);

                mail.Attachments.Add(data);
            }
        }

        private void SendEmailBySmtp(MailMessage message)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = Provider;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(UserName, Password);
            smtpClient.Send(message);
            smtpClient.Dispose();
        }
    }
}