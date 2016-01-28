using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Mail_Connector.Domain
{
    public class Email
    {
        public DateTime? Date;
        public String From;
        public String Subject;
        public String Body;
        public String To;
        public List<Attachment> Attachments = new List<Attachment>();
    }
}
