using Mail_Connector.Domain;
using Mail_Router.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using CMS_Connector.SPDocumentCopyService;
using NLog;

namespace CMS_Connector.Repository
{
    public class SharePointMailLoaderRepository : IEmailLoaderRepositoryInterface
    {
        Dictionary<string, string> Config = new Dictionary<string, string>();
        private DateTime TimeLastLoaded;
        
        public SharePointMailLoaderRepository()
        {
            LoadConfig();
        }

        public void LoadMail(List<Email> emails)
        {
            using (SPDocumentCopyService.Copy spDocumentCopyService = new SPDocumentCopyService.Copy())
            {
                spDocumentCopyService.Credentials = CredentialCache.DefaultCredentials;

                foreach (Email email in emails)
                {

                    CopyResult myCopyResult1 = new CopyResult();
                    CopyResult myCopyResult2 = new CopyResult();
                    CopyResult[] myCopyResultArray = { myCopyResult1, myCopyResult2 };

                    FieldInformation from = new FieldInformation { DisplayName = "From Email", InternalName = "From Email", Type = FieldType.Text, Value = email.From };
                    FieldInformation title = new FieldInformation { DisplayName = "Title", InternalName = "Title", Type = FieldType.Text, Value = email.Subject };
                    FieldInformation subject = new FieldInformation { DisplayName = "Email Subject", InternalName = "Email Subject", Type = FieldType.Text, Value = email.Subject };
                    FieldInformation body = new FieldInformation { DisplayName = "Email Body", InternalName = "Email Body", Type = FieldType.Text, Value = email.Body };
                    FieldInformation dateReceived = new FieldInformation { DisplayName = "Date Received", InternalName = "Date Received", Type = FieldType.Text, Value = DateTime.Now.ToString() };

                    FieldInformation[] myFieldInfoArray = { from, dateReceived, subject, body };

                    if (email.Attachments.Count > 0)
                    {
                        foreach (Attachment attachment in email.Attachments)
                        {
                            string copySource = attachment.FileName;
                            string[] copyDest = { Config["SharePointSiteUrl"] + Config["SharePointLibraryName"] + attachment.FileName };

                            try
                            {
                                byte[] myByteArray = attachment.Content;

                                uint myCopyUint = spDocumentCopyService.CopyIntoItems(copySource, copyDest, myFieldInfoArray, myByteArray, out myCopyResultArray);

                                if (myCopyUint != 0)
                                {
                                    Logger logger = LogManager.GetCurrentClassLogger();
                                    logger.Log(LogLevel.Error, String.Concat(myCopyResultArray));
                                }
                            }
                            catch (Exception e)
                            {
                                Logger logger = LogManager.GetCurrentClassLogger();
                                logger.Log(LogLevel.Error, "Exception occurred when loading mail into SharePoint library", e);
                            }
                        }
                    }
                    else // An email without attachments:
                    {
                        try
                        {
                            string[] copyDest = { Config["SharePointSiteUrl"] + Config["SharePointLibraryName"] + GetPrependedDateTime() + "No_Attachment" };
                            byte[] myByteArray = { byte.MinValue };

                            uint myCopyUint = spDocumentCopyService.CopyIntoItems("nofile", copyDest, myFieldInfoArray, myByteArray, out myCopyResultArray);

                            if (myCopyUint != 0)
                            {
                                Logger logger = LogManager.GetCurrentClassLogger();
                                logger.Log(LogLevel.Error, String.Concat(myCopyResultArray));
                            }
                        }
                        catch (Exception e)
                        {
                            Logger logger = LogManager.GetCurrentClassLogger();
                            logger.Log(LogLevel.Error, "Exception occurred when loading mail into SharePoint library", e);
                        }
                    }
                }
            }
            TimeLastLoaded = DateTime.Now;
        }

        public void LoadMail(Email email)
        {
            throw new NotImplementedException();
        }

        public DateTime GetTimeLastLoaded()
        {
            return TimeLastLoaded;
        }

        public bool ServiceAvailable()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(Config["SharePointSiteUrl"] + Config["SharePointLibraryName"]);
                request.Credentials = CredentialCache.DefaultCredentials;
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Error, "Exception occurred when checking availability of the CMS library", e);
            }
            return false;
        }

        // For use with renaming files when loading them to make them unique
        private string GetPrependedDateTime()
        {
            string currentDateTime = DateTime.Now.ToString("yyyyMMddhhmmssfff");
            return currentDateTime + "_";
        }

        public void LoadConfig()
        {
            try
            {
                Config.Add("SharePointSiteUrl", System.Configuration.ConfigurationManager.AppSettings["SharePointSiteUrl"].ToString());
                Config.Add("SharePointLibraryName", System.Configuration.ConfigurationManager.AppSettings["SharePointLibraryName"].ToString());
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
