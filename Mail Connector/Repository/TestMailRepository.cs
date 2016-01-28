using Mail_Connector.Domain;
using Mail_Connector.RepositoryInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Mail_Connector.Repository
{
    public class TestMailRepository : IEmailRepository
    {
        Dictionary<string, string> Config = new Dictionary<string, string>();
        private DateTime TimeLastChecked;
        MailAccessIndicator CurrentMailAccessIndicator;

        public TestMailRepository()
        {
            LoadConfig();
        }

        public List<Email> GetMail()
        {
            bool newOnly = Config["GetMailType"].Equals("NEW") ? true : false;

            List<Email> emails = new List<Email>();

            Email email1 = new Email();
            email1.Subject = "Test Body " + DateTime.Now;
            email1.Body = "Test Body " + DateTime.Now;
            email1.From = "mail@company.com";
            email1.To = "mail@company.com";

            CurrentMailAccessIndicator = MailAccessIndicator.MailFetched;

            String attachmentFileName = DateTime.Now.ToString("yyyyMMddHHMMss-") + "test.txt";
            emails.Add(email1);
            
            TimeLastChecked = DateTime.Now;
            return emails;
        }


        public DateTime GetTimeLastChecked()
        {
            return TimeLastChecked;
        }

        public MailAccessIndicator GetMailAccessIndicator()
        {
            return CurrentMailAccessIndicator;
        }

        public void LoadConfig()
        {
            try
            {
                Config.Add("GetMailType", System.Configuration.ConfigurationManager.AppSettings["GetMailType"].ToString());
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}
