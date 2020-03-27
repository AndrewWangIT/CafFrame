using Caf.Core.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Core.Email
{
    public class EmailSender : IEmailSender, ITransient
    {
        private readonly SmtpClient _client;
        private readonly EmailOptions _options;
        public EmailSender(IOptions<EmailOptions> options)
        {
            _options = options.Value;

            _client = new SmtpClient
            {
                Host = _options.Host,
                Port = _options.Port > 0 ? _options.Port : 25,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(_options.UserName, _options.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = _options.EnableSsl,
                Timeout = _options.Timeout > 0 ? _options.Timeout : 30000
            };
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = CreateMail(email, subject, message);
            await _client.SendMailAsync(mail);
        }

        public async Task SendAttachmentEmailAsync(string email, string subject, string body,
            Stream stream = null, string fileName = null, string cc = null)
        {
            MailMessage mail = CreateMail(email, subject, body, cc);
            if (stream != null && fileName != null)
            {
                string contentTypeName = null;
                var attachement = new Attachment(stream, contentTypeName);
                attachement.ContentDisposition.FileName = fileName;

                mail.Attachments.Add(attachement);
            }
            await _client.SendMailAsync(mail);
        }

        public async Task SendAttachmentEmailAsync(string email, string subject, string body,
           List<Attachment> attachments, string cc = null)
        {
            MailMessage mail = CreateMail(email, subject, body, cc);

            if (attachments != null)
            {
                foreach (Attachment attachment in attachments)
                {
                    mail.Attachments.Add(attachment);
                }
            }
            await _client.SendMailAsync(mail);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="cc">多个用,分割</param>
        /// <returns></returns>
        private MailMessage CreateMail(string to, string subject, string body, string cc = null)
        {
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(_options.Email, _options.UserName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8
            };
            mail.To.Add(to);

            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(cc);//多个用,分割
            }
            return mail;
        }

    }
}
