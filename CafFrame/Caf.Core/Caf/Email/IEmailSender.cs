using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Core.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);

        Task SendAttachmentEmailAsync(string email, string subject, string body,
          Stream stream = null, string fileName = null, string cc = null);

        Task SendAttachmentEmailAsync(string email, string subject, string body,
          List<Attachment> attachments, string cc = null);
    }
}
