using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mail_Connector.Domain;

namespace Mail_Connector.RepositoryInterface
{
    public interface IEmailRepository
    {
        void LoadConfig();
        List<Email> GetMail();
        DateTime GetTimeLastChecked();
        MailAccessIndicator GetMailAccessIndicator();
    }
}
