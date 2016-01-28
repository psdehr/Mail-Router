using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using SharepointLog.sitesWebServiceLists;
using Microsoft.SharePoint.SoapServer;


namespace SharepointLog
{
    class Program
    {
        static void Main(string[] args)
        {
            LogToSharepoint("Connected to Google mail and finished uploading documents to library."); 
                       
        }

        static public void LogToSharepoint(String LogType)
        {
            sitesWebServiceLists.Lists listService = new sitesWebServiceLists.Lists();

            listService.Credentials =
            System.Net.CredentialCache.DefaultCredentials;

            // LIVE URL listService.Url = "http://connect.pall.net/depts/legal/ip/_vti_bin/lists.asmx";
            listService.Url = "http://nypw-connect/depts/legal/ip/_vti_bin/lists.asmx";

            System.Xml.XmlNode ndListView = listService.GetListAndView("InboxLog", "");
            string strListID = ndListView.ChildNodes[0].Attributes["Name"].Value;
            string strViewID = ndListView.ChildNodes[1].Attributes["Name"].Value;

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.XmlElement batchElement = doc.CreateElement("Batch");
            batchElement.SetAttribute("OnError", "Continue");
            batchElement.SetAttribute("ListVersion", "1");
            batchElement.SetAttribute("ViewName", strViewID);

            //batchElement.InnerXml = "<Method ID='1' Cmd='New'>" + "<Field Name='Title'>TEST Successfully Completed IP Inbox Load</Field></Method>";
            batchElement.InnerXml = "<Method ID='1' Cmd='New'>" + "<Field Name='Title'>" + LogType + "</Field></Method>";


            
            try
            {
                listService.UpdateListItems(strListID, batchElement);
            }
            catch (SoapServerException ex)
            {

            }
        }

    }

    

}
