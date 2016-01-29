using NLog;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Mail_Connector.Domain;
using Mail_Connector.RepositoryInterface;
using Mail_Connector.LotusNotesMailService;

namespace Mail_Connector.Repository
{
    public class LotusNotesMailRepository : IEmailRepository
    {
        Dictionary<string, string> Config = new Dictionary<string, string>();
        MailAccessIndicator CurrentMailAccessIndicator;

        public LotusNotesMailRepository()
        {
            LoadConfig();
        }

        public List<Domain.Email> GetMail()
        {
            bool getAllMail = Config["GetMailType"].Equals("GETALLMAIL123")? true : false;
            List<Domain.Email> emails = new List<Domain.Email>();

            try
            {
                using (LotusNotesMailClient client = new LotusNotesMailClient())
                {
                    LotusNotesMailService.Email[] emailsLotusNotes = client.GetMail(getAllMail);

                    if (emailsLotusNotes != null)
                    {
                        CurrentMailAccessIndicator = MailAccessIndicator.MailChecked;

                        foreach (var emailLotusNotes in emailsLotusNotes)
                        {
                            Domain.Email localEmail = new Domain.Email();
                            localEmail.From = emailLotusNotes.From;
                            localEmail.To = emailLotusNotes.To;
                            localEmail.Subject = emailLotusNotes.Subject;
                            localEmail.Body = emailLotusNotes.Body;

                            if (emailLotusNotes.Attachments != null && emailLotusNotes.Attachments.Length > 0)
                            {
                                try
                                {
                                    using (LotusNotesMailClient clientFetchAttachments = new LotusNotesMailClient())
                                    {
                                        emailLotusNotes.Attachments = clientFetchAttachments.GetEmailAttachments(emailLotusNotes);

                                        foreach (LotusNotesMailService.Attachment attachment in emailLotusNotes.Attachments)
                                        {
                                            Domain.Attachment localAttachment = new Domain.Attachment();
                                            localAttachment.FileName = attachment.FileName;
                                            localAttachment.FilePath = attachment.FilePath;
                                            localAttachment.Content = attachment.Content;

                                            localEmail.Attachments.Add(localAttachment);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger logger = LogManager.GetCurrentClassLogger();
                                    logger.Log(LogLevel.Error, "Exception occurred when getting attachments for the email", e);
                                    CurrentMailAccessIndicator = MailAccessIndicator.MailFetchError;
                                }
                            }
                            emails.Add(localEmail);
                            CurrentMailAccessIndicator = MailAccessIndicator.MailFetched;
                        }
                    }
                }
            }
            catch (CommunicationException ce)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Error, "Exception occurred when connecting to the Lotus Notes mail server", ce);
                CurrentMailAccessIndicator = MailAccessIndicator.LoginError;
            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Error, "Exception occurred when retrieving mail from Lotus Notes", e);
                CurrentMailAccessIndicator = MailAccessIndicator.MailFetchError;
            }

            return emails;
        }

        public MailAccessIndicator GetMailAccessIndicator()
        {
            return CurrentMailAccessIndicator;
        }


        public DateTime GetTimeLastChecked()
        {
            return DateTime.Now;
        }

        public void LoadConfig()
        {
            Config.Add("GetMailType", System.Configuration.ConfigurationManager.AppSettings["GetMailType"].ToString());
        }
    }
}
