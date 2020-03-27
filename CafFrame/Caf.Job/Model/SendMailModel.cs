using Caf.Job.Entity;

namespace Caf.Job.Entity
{
    public class SendMailModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public MailEntity MailInfo { get; set; } = null;
    }
}
