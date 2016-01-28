using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mail_Connector.RepositoryInterface;
using Mail_Connector.Domain;

namespace Mail_Connector.BO
{
    public class MailConnector
    {
        IEmailRepository Repository;

        public MailConnector(IEmailRepository repository)
        {
            Repository = repository;
        }

        public List<Email> GetMail()
        {
            return Repository.GetMail();
        }

        public DateTime GetTimeLastChecked()
        {
            return Repository.GetTimeLastChecked();
        }

        public MailAccessIndicator GetMailAccessIndicator()
        {
            return Repository.GetMailAccessIndicator();
        }

    }
}
