using CMS_Connector.BO;
using CMS_Connector.Repository;
using Mail_Connector.BO;
using Mail_Connector.Domain;
using Mail_Connector.Repository;
using NLog;
using System.Collections.Generic;

namespace Mail_Router
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. A standard to retrieve the emails (Google, Lotus Notes, Exchange, etc.)
            MailConnector mailConnector = new MailConnector(new LotusNotesMailRepository());
            CMSConnector cmsDocumentRouterConnector = new CMSConnector(new SharePointDocumentRouterRepository());
            CMSConnector cmsMailLoaderConnector = new CMSConnector(new SharePointMailLoaderRepository());

            if (cmsMailLoaderConnector.ServiceAvailable())
            {
                List<Email> emails = mailConnector.GetMail();

                // 2. A standard to load emails to another system
                if (mailConnector.GetMailAccessIndicator() == MailAccessIndicator.MailFetched)
                {
                    cmsMailLoaderConnector.LoadMail(emails);
                }

                // 3. A standard way to route documents from Inbox to patent or trademark cases
                cmsDocumentRouterConnector.RouteDocuments();
            }
            else
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Error, "CMS server is unavailable: " + System.Configuration.ConfigurationManager.AppSettings["SharePointSiteUrl"].ToString());
            }
        }
    }
}
