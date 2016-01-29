using Domino;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Company.ServiceModel.LotusNotesMailService
{
    public class LotusNotesMailService : ILotusNotesMail
    {
        Dictionary<string, string> Config = new Dictionary<string, string>();

        public List<Email> GetData()
        {
            return GetMail(false);
        }

        public List<Email> GetMail(bool getAllMail = false)
        {
            LoadConfig();
            List<Email> emails = new List<Email>();

            try
            {
                NotesSession notesSession = new NotesSession();
                notesSession.Initialize(Config["PasswordLotusNotesAccount"]);
                NotesDatabase notesDb = notesSession.GetDatabase(Config["HostLotusNotesServer"], Config["PathLotusNotesDatabase"], false);
                NotesDateTime startDateMailAll = notesSession.CreateDateTime(Config["StartDateMailAll"]);
                NotesDateTime startDateMailNew = notesSession.CreateDateTime(Config["StartDateMailNew"]);
                NotesDocument profileDocument = notesDb.GetProfileDocument("Profile");

                if (!getAllMail) // If only new mail get the last time we checked email
                {
                    if (profileDocument != null && profileDocument.HasItem("UntilTime"))
                    {
                        object[] dateTime = profileDocument.GetItemValue("UntilTime");
                        if (dateTime != null && dateTime.Length > 0)
                            startDateMailNew = notesSession.CreateDateTime(Convert.ToString(dateTime[0]));
                    }
                }

                NotesDateTime startingDateTime = (getAllMail) ? startDateMailAll : startDateMailNew;
                var documentCollection = notesDb.GetModifiedDocuments(startingDateTime);
                NotesDocument lotusNotesEmail = documentCollection.GetFirstDocument();

                while (lotusNotesEmail != null)
                {
                    if (lotusNotesEmail.IsValid
                        && (!lotusNotesEmail.IsDeleted))
                    {
                        NotesItem fromNotesItem = lotusNotesEmail.GetFirstItem("From");
                        NotesItem toNotesItem = lotusNotesEmail.GetFirstItem("INetSendTo");
                        NotesItem subjectNotesItem = lotusNotesEmail.GetFirstItem("Subject");
                        NotesItem bodyNotesItem = lotusNotesEmail.GetFirstItem("Body");

                        emails.Add(new Email
                        {
                            From = fromNotesItem == null ? "N/A" : fromNotesItem.Text,
                            To = toNotesItem == null ? "N/A" : toNotesItem.Text,
                            Subject = subjectNotesItem == null ? "N/A" : subjectNotesItem.Text,
                            Body = bodyNotesItem == null ? "N/A" : bodyNotesItem.Text,
                            Date = lotusNotesEmail.Created == null ? DateTime.MinValue : Convert.ToDateTime(lotusNotesEmail.Created.ToString()),
                            Attachments = ExtractAttachments(lotusNotesEmail)
                        });

                        ReleaseComResources(new object[] { fromNotesItem, toNotesItem, subjectNotesItem, bodyNotesItem });
                        // Always last line:
                        lotusNotesEmail = documentCollection.GetNextDocument(lotusNotesEmail);
                    }
                }

                // Time-stamp most recent mail pick-up (if not getting all mail)
                if (!getAllMail)
                {
                    SetMailAsRead(profileDocument, documentCollection);
                }

                // Release Notes objects
                ReleaseComResources(new object[] { notesDb, notesSession, profileDocument, documentCollection });

            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Error, e);
            }

            return emails;
        }

        private void SetMailAsRead(NotesDocument profileDocument, IDocumentCollection documentCollection)
        {
            try
            {
                profileDocument.ReplaceItemValue("UntilTime", documentCollection.UntilTime);
                profileDocument.Save(true, true, true);
            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Error, e);
            }
        }

        public void SetMailAsRead(DateTime dateTime)
        {
            LoadConfig();

            NotesSession notesSession = new NotesSession();
            notesSession.Initialize(Config["PasswordLotusNotesAccount"]);
            NotesDatabase notesDb = notesSession.GetDatabase(Config["HostLotusNotesServer"], Config["PathLotusNotesDatabase"], false);
            NotesDateTime lnDateTime = notesSession.CreateDateTime(dateTime.ToShortDateString());
            NotesDocument profileDocument = notesDb.GetProfileDocument("Profile");

            try
            {
                profileDocument.ReplaceItemValue("UntilTime", lnDateTime);
                profileDocument.Save(true, true, true);
            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Error, e);
            }

            ReleaseComResources(new object[] { notesDb, notesSession, profileDocument, lnDateTime });
        }

        /*
        To begin with, retrieving mail using WCF is not a great idea because of binary attachments.
        It will work perfectly however if if we save all attachments of picked-up mail on server disk
        and return file names with the returned mail.
        Using the web service client we later pick up the attachments using GetEmailAttachments(Email) method,
        at which time these files will also be removed.
        */
        private List<Attachment> ExtractAttachments(NotesDocument lotusNotesEmail)
        {
            List<Attachment> attachmentFileNames = new List<Attachment>();
            List<string> filePathsToDelete = new List<string>();
            object[] items = (object[])lotusNotesEmail.Items;

            foreach (var item in items)
            {
                NotesItem notesItem = (NotesItem)item;

                if (notesItem != null && notesItem.Values != null && !(notesItem.Values is string))
                {
                    object[] itemValues = (object[])notesItem.Values;

                    if (itemValues != null)
                    {
                        if (notesItem.type == Domino.IT_TYPE.ATTACHMENT)
                        {
                            if (itemValues[0] != null && notesItem.Name != null)
                            {
                                string fileNameOriginal = itemValues[0].ToString();
                                string fileNameValid = GetValidFileName(fileNameOriginal);
                                string fileNameDated = GetPrependedDateTime() + fileNameValid;
                                string filePath = Config["FilepathLocalTempFiles"] + fileNameDated;

                                try
                                {
                                    int existingDocumentsWithSameName = (from att in attachmentFileNames where att.FileNameOriginal.Equals(fileNameOriginal) select att).Count(); // Beware of duplicates from Lotus Notes
                                    if (existingDocumentsWithSameName == 0)
                                    {
                                        var attachment = lotusNotesEmail.GetAttachment(fileNameOriginal); // Extracting attachment using the original name (fileNameOriginal)
                                        attachment.ExtractFile(filePath); // Saving file to disk using valid filename (filePath)
                                        attachmentFileNames.Add(new Attachment { FileName = fileNameDated, FilePath = filePath, FileNameOriginal = fileNameOriginal }); // Putting in filepath for now, load byte[] data later
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger logger = LogManager.GetCurrentClassLogger();
                                    logger.Log(LogLevel.Debug, e);
                                }
                            }
                        }
                    }
                }
            }

            return attachmentFileNames;
        }

        public List<Attachment> GetEmailAttachments(Email email)
        {
            List<string> filePathsToDelete = new List<string>();

            if (email.Attachments.Count > 0)
            {
                foreach (Attachment attachment in email.Attachments)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(attachment.FilePath);
                        if (IsFileLocked(fileInfo))
                        {
                            attachment.Status = "Locked";
                        }
                        else
                        {
                            attachment.Content = File.ReadAllBytes(attachment.FilePath);
                            attachment.Status = "Ready";
                            filePathsToDelete.Add(attachment.FilePath);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger logger = LogManager.GetCurrentClassLogger();
                        logger.Log(LogLevel.Debug, e);
                    }
                }
            }

            if (filePathsToDelete.Count > 0)
                DeletelLocalTempFiles(filePathsToDelete);

            return email.Attachments;
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private static void DeletelLocalTempFiles(List<string> filePathsToDelete)
        {
            // Delete the temporary files
            try
            {
                foreach (string filePath in filePathsToDelete)
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Debug, e);
            }
        }

        private static string GetValidFileName(string name)
        {
            string fileName = name;

            try
            {
                string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
                invalidChars = invalidChars + "#;";
                string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
                fileName = System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Debug, e);
            }

            return fileName;
        }

        private void ReleaseComResources(object[] comObjects)
        {
            try
            {
                foreach (var o in comObjects)
                {
                    Marshal.ReleaseComObject(o);
                }
            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Error, e);
            }
        }

        // For use with renaming files when loading them to make them unique
        private string GetPrependedDateTime()
        {
            string currentDateTime = DateTime.Now.ToString("yyyyMMddhhmmssfff");
            return currentDateTime + "_";
        }

        private void LoadConfig()
        {
            Config.Add("HostLotusNotesServer", System.Configuration.ConfigurationManager.AppSettings["HostLotusNotesServer"].ToString());
            Config.Add("PathLotusNotesDatabase", System.Configuration.ConfigurationManager.AppSettings["PathLotusNotesDatabase"].ToString());
            Config.Add("PasswordLotusNotesAccount", System.Configuration.ConfigurationManager.AppSettings["PasswordLotusNotesAccount"].ToString());
            Config.Add("FilepathLocalTempFiles", System.Configuration.ConfigurationManager.AppSettings["FilepathLocalTempFiles"].ToString());
            Config.Add("StartDateMailAll", System.Configuration.ConfigurationManager.AppSettings["StartDateMailAll"].ToString());
            Config.Add("StartDateMailNew", System.Configuration.ConfigurationManager.AppSettings["StartDateMailNew"].ToString());
        }

    }
}
