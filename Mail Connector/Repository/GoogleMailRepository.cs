using ImapX;
using ImapX.Collections;
using Mail_Connector.Domain;
using Mail_Connector.RepositoryInterface;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Mail_Connector.Repository
{
    public class GoogleMailRepository : IEmailRepository
    {
        Dictionary<string, string> Config = new Dictionary<string, string>();
        private DateTime TimeLastChecked;
        private MailAccessIndicator CurrentMailAccessIndicator;

        public GoogleMailRepository()
        {
            LoadConfig();
        }

        public List<Email> GetMail()
        {
            string getAllMail = Config["GetMailType"].Equals("GETALLMAIL123") ? "ALL" : "UNSEEN";
            MessageCollection remoteEmails;
            List<Email> emails = new List<Email>();
            ImapClient imapClient = new ImapClient(Config["Host_Google"], true, false);
            bool imapConnected = false;
            bool imapLoggedIn = false;

            try
            {
                imapConnected = imapClient.Connect();
                imapLoggedIn = imapClient.Login(Config["User_Google"], Config["Password_Google"]);
                CurrentMailAccessIndicator = MailAccessIndicator.LoggedIn;
            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Error, "Connection to Google Mail server cannot be established.", e);
            }

            if (imapConnected && imapLoggedIn)
            {
                // Get messages from remote source
                ImapX.Folder InboxFolder = imapClient.Folders.Inbox;
                remoteEmails = InboxFolder.Messages;
                remoteEmails.Download(getAllMail);
                CurrentMailAccessIndicator = MailAccessIndicator.MailChecked;
                
                foreach (Message remoteEmail in remoteEmails)
                {
                    try
                    {
                        Email email = new Email
                        {
                            Date = remoteEmail.Date,
                            From = remoteEmail.From.Address.ToString(),
                            Subject = remoteEmail.Subject,
                            Body = remoteEmail.Body.Text
                        };

                        if (remoteEmail.Attachments.Length > 0)
                        {
                            email.Attachments = new List<Domain.Attachment>();

                            foreach (ImapX.Attachment anAttachment in remoteEmail.Attachments)
                            {
                                anAttachment.Download();
                                string attachmentFileName = DateTime.Now.ToString("yyyyMMddHHMMss.f") + anAttachment.FileName;
                                email.Attachments.Add(
                                    new Domain.Attachment {
                                        FileName = attachmentFileName,
                                        Content = anAttachment.FileData
                                    });
                            }
                        }
                        remoteEmail.Seen = Config["MarkAsSeen"].ToString().Equals("True") ? true : false;
                        emails.Add(email);
                    }
                    catch (Exception e)
                    {
                        Logger logger = LogManager.GetCurrentClassLogger();
                        logger.Log(LogLevel.Error, "Exception occurred when saving emails", e);
                        CurrentMailAccessIndicator = MailAccessIndicator.MailFetchError;

                    }
                    CurrentMailAccessIndicator = MailAccessIndicator.MailFetched;
                }
            }

            imapClient.Dispose();

            TimeLastChecked = DateTime.Now;
            return emails;
        }

        public MailAccessIndicator GetMailAccessIndicator()
        {
            return CurrentMailAccessIndicator;
        }

        public DateTime GetTimeLastChecked()
        {
            return TimeLastChecked;
        }

        public void LoadConfig()
        {
            try
            {
                Config.Add("Host_Google", System.Configuration.ConfigurationManager.AppSettings["Host"].ToString());
                Config.Add("User_Google", System.Configuration.ConfigurationManager.AppSettings["User"].ToString());
                Config.Add("Password_Google", System.Configuration.ConfigurationManager.AppSettings["Password"].ToString());
                Config.Add("MarkAsSeen", System.Configuration.ConfigurationManager.AppSettings["MarkAsSeen"].ToString());
                Config.Add("GetMailType", System.Configuration.ConfigurationManager.AppSettings["GetMailType"].ToString());
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}
