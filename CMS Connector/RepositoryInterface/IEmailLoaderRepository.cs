using Mail_Connector.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mail_Router.RepositoryInterface
{
    public interface IEmailLoaderRepositoryInterface
    {
        void LoadConfig();
        void LoadMail(List<Email> emails);
        void LoadMail(Email email);
        bool ServiceAvailable();
        DateTime GetTimeLastLoaded();
    }
}
