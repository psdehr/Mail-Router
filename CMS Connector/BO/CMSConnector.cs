using CMS_Connector.RepositoryInterface;
using Mail_Connector.Domain;
using Mail_Router.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS_Connector.BO
{
    public class CMSConnector
    {
        IEmailLoaderRepositoryInterface RepositoryEmailLoader;
        IDocumentRouterInterface RepositoryDocumentRouter;

        public CMSConnector(IEmailLoaderRepositoryInterface repository)
        {
            RepositoryEmailLoader = repository;
        }

        public CMSConnector(IDocumentRouterInterface repository)
        {
            RepositoryDocumentRouter = repository;
        }

        public void LoadMail(List<Email> emails)
        {
            RepositoryEmailLoader.LoadMail(emails);
        }

        public void RouteDocuments()
        {
            RepositoryDocumentRouter.RouteDocuments();
        }

        public bool ServiceAvailable()
        {
            return RepositoryEmailLoader.ServiceAvailable();
        }
    }
}
