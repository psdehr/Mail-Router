using CMS_Connector.Domain;
using CMS_Connector.RepositoryInterface;
using CMS_Connector.SPDocumentCopyService;
using CMS_Connector.SPGetList;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace CMS_Connector.Repository
{
    public class SharePointDocumentRouterRepository : IDocumentRouterInterface
    {
        Dictionary<string, string> Config = new Dictionary<string, string>();

        public SharePointDocumentRouterRepository()
        {
            LoadConfig();
        }

        public void RouteDocuments()
        {
            List<EmailDocument> routedDocuments = GetRoutedDocuments();
            MoveRoutedDocuments(routedDocuments);
        }

        private void MoveRoutedDocuments(List<EmailDocument> routedDocuments)
        {
            using (SPDocumentCopyService.Copy spDocumentCopyService = new SPDocumentCopyService.Copy())
            {
                spDocumentCopyService.Url = Config["SharePointSiteUrl"] + "_vti_bin/Copy.asmx";
                spDocumentCopyService.Credentials = CredentialCache.DefaultCredentials;

                foreach (EmailDocument document in routedDocuments)
                {
                    FieldInformation myFieldInfo = new FieldInformation();
                    FieldInformation[] basicFieldInfoArray = { myFieldInfo };
                    byte[] myByteArray;

                    uint myGetUint = spDocumentCopyService.GetItem(document.DocumentPath, out basicFieldInfoArray, out myByteArray);

                    CopyResult myCopyResult1 = new CopyResult();
                    CopyResult myCopyResult2 = new CopyResult();
                    CopyResult[] myCopyResultArray = { myCopyResult1, myCopyResult2 };

                    document.DocumentType = GetFilePropertyValue("Document Type", basicFieldInfoArray);
                    document.DocumentSubtype = GetFilePropertyValue("Document Subtype", basicFieldInfoArray);

                    FieldInformation fieldDocumentType = new FieldInformation { DisplayName = "Document Type Literal", InternalName = "DocumentTypeLiteral", Type = FieldType.Text, Value = document.DocumentType };
                    FieldInformation fieldDocumentSubType = new FieldInformation { DisplayName = "Document Subtype Literal", InternalName = "DocumentSubtypeLiteral", Type = FieldType.Text, Value = document.DocumentSubtype };

                    try
                    {
                        FieldInformation[] fullFieldInfoArray = { myFieldInfo, fieldDocumentType, fieldDocumentSubType };

                        uint myCopyUint = spDocumentCopyService.CopyIntoItems(document.DocumentPath, new string[] { document.DestinationUrl }, fullFieldInfoArray, myByteArray, out myCopyResultArray);

                        CopyResult copyResult = myCopyResultArray[0];

                        if (copyResult != null && copyResult.ErrorCode == SPDocumentCopyService.CopyErrorCode.Success)
                        {
                            document.RoutedSuccessfully = true;
                            document.RoutedDeletedSuccessfully = DeleteRoutedDocument(document);
                        }
                        else
                            document.RoutedSuccessfully = false;

                        if (myCopyUint != 0 || copyResult.ErrorCode != SPDocumentCopyService.CopyErrorCode.Success)
                        {
                            Logger logger = LogManager.GetCurrentClassLogger();
                            logger.Log(LogLevel.Error, "CopyRoutedDocuments: Exception occurred when copying a routed document", String.Concat(myCopyResultArray));
                        }

                        if (document.RoutedSuccessfully && document.RoutedDeletedSuccessfully)
                        {
                            LogDocumentRouting(document);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger logger = LogManager.GetCurrentClassLogger();
                        logger.Log(LogLevel.Error, "CopyRoutedDocuments: Exception occurred when copying a routed document", e);
                    }
                }
            }
        }

        private void LogDocumentRouting(EmailDocument document)
        {
            using (Lists listService = new Lists())
            {
                listService.Credentials = CredentialCache.DefaultCredentials;
                listService.Url = Config["SharePointSiteUrl"] + "_vti_bin/lists.asmx";

                try
                {
                    string xml = "<Method ID='1' Cmd='New'>" +
                                    "<Field Name='ID'/>" +
                                    "<Field Name='Title'>" + document.DestinationCase + "</Field>" +
                                    "<Field Name='From'>" + document.DocumentPath + "</Field>" +
                                    "<Field Name='To'>" + document.DestinationUrl + "</Field>" +
                                    "</Method>";

                    /*Get Name attribute values (GUIDs) for list and view. */
                    System.Xml.XmlNode ndListView = listService.GetListAndView(Config["RoutingLog"].Replace("/", ""), "");
                    string strListID = ndListView.ChildNodes[0].Attributes["Name"].Value;
                    string strViewID = ndListView.ChildNodes[1].Attributes["Name"].Value;

                    /*Create an XmlDocument object and construct a Batch element and its
                    attributes. Note that an empty ViewName parameter causes the method to use the default view. */
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    System.Xml.XmlElement batchElement = doc.CreateElement("Batch");
                    batchElement.SetAttribute("OnError", "Continue");
                    batchElement.SetAttribute("ListVersion", "1");
                    batchElement.SetAttribute("ViewName", "");

                    /*Specify methods for the batch post using CAML. To update or delete, 
                    specify the ID of the item, and to update or add, specify 
                    the value to place in the specified column.*/
                    batchElement.InnerXml = xml;
                    XmlNode item = listService.UpdateListItems(Config["RoutingLog"].Replace("/", ""), batchElement);
                }
                catch (Exception e)
                {
                    Logger logger = LogManager.GetCurrentClassLogger();
                    logger.Log(LogLevel.Error, "LogDocumentRouting: Exception occurred when logging a successful routing of document.  \nSource: " + document.DocumentPath + "\nTo: " + document.DestinationUrl, e);
                }

            }
        }

        private bool DeleteRoutedDocument(EmailDocument routedDocument)
        {
            bool deletedSuccessfully = false;

            using (Lists listService = new Lists())
            {
                listService.Credentials = CredentialCache.DefaultCredentials;
                listService.Url = Config["SharePointSiteUrl"] + "_vti_bin/lists.asmx";

                if (routedDocument.RoutedSuccessfully)
                {
                    try
                    {
                        string xml = "<Method ID='1' Cmd='Delete'>" +
                                        "<Field Name='ID'>" + routedDocument.DocumentId + "</Field>" +
                                        "<Field Name='FileRef'>" + routedDocument.DocumentPath + "</Field>" +
                                        "</Method>";

                        /*Get Name attribute values (GUIDs) for list and view. */
                        System.Xml.XmlNode ndListView = listService.GetListAndView(Config["SharePointLibraryName"].Replace("/", ""), "");
                        string strListID = ndListView.ChildNodes[0].Attributes["Name"].Value;
                        string strViewID = ndListView.ChildNodes[1].Attributes["Name"].Value;

                        /*Create an XmlDocument object and construct a Batch element and its
                        attributes. Note that an empty ViewName parameter causes the method to use the default view. */
                        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                        System.Xml.XmlElement batchElement = doc.CreateElement("Batch");
                        batchElement.SetAttribute("OnError", "Continue");
                        batchElement.SetAttribute("ListVersion", "1");
                        batchElement.SetAttribute("ViewName", "");

                        /*Specify methods for the batch post using CAML. To update or delete, 
                        specify the ID of the item, and to update or add, specify 
                        the value to place in the specified column.*/
                        batchElement.InnerXml = xml;
                        XmlNode item = listService.UpdateListItems(Config["SharePointLibraryName"].Replace("/", ""), batchElement);
                        deletedSuccessfully = true;
                    }
                    catch (Exception e)
                    {
                        Logger logger = LogManager.GetCurrentClassLogger();
                        logger.Log(LogLevel.Error, "DeleteRoutedDocuments: Exception occurred when deleting a routed document: " + routedDocument.DocumentPath, e);
                    }
                }
            }

            return deletedSuccessfully;
        }

        private List<EmailDocument> GetRoutedDocuments()
        {
            List<EmailDocument> routedDocuments = new List<EmailDocument>();

            using (Lists listService = new Lists())
            {
                listService.Credentials = CredentialCache.DefaultCredentials;
                listService.Url = Config["SharePointSiteUrl"] + "_vti_bin/lists.asmx";

                // Query to get any items that have Patents or Trademarks
                XmlDocument xmlDoc = new System.Xml.XmlDocument();
                XmlNode itemsQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
                XmlNode itemsViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
                XmlNode itemsQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

                itemsQueryOptions.InnerXml = "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns><DateInUtc>TRUE</DateInUtc>";
                itemsViewFields.InnerXml = @"<FieldRef Name='ID' />
                                         <FieldRef Name='GUID' />
                                         <FieldRef Name='Name' />
                                         <FieldRef Name='Patent'/>
                                         <FieldRef Name='Trademarks'/>";

                itemsQuery.InnerXml = @"<Where>
                                        <Or>
                                            <IsNotNull>
                                                <FieldRef Name='Patent' />
                                            </IsNotNull>
                                            <IsNotNull>
                                                <FieldRef Name='Trademarks' />
                                            </IsNotNull>
                                        </Or>
                                </Where>";

                XmlNode listItems = listService.GetListItems("Inbox", null, itemsQuery, itemsViewFields, null, itemsQueryOptions, null);
                XElement data = GetXElement(listItems);
                XElement dataRowset = data.Element(XName.Get("data", "urn:schemas-microsoft-com:rowset"));

                IEnumerable<XElement> documentRows = dataRowset.Elements(XName.Get("row", "#RowsetSchema"));
                foreach (XElement documentRow in documentRows)
                {
                    try
                    {
                        FieldValueIdPair fileReference = GetFieldValueIdPair(documentRow, "ows_FileRef");
                        string fileId = fileReference.FieldId;
                        string filePath = Config["SharePointSiteUrl"].Substring(0, Config["SharePointSiteUrl"].IndexOf("depts/")) + fileReference.FieldValue;

                        FieldValueIdPair destinationPatent = GetFieldValueIdPair(documentRow, "ows_Patent");
                        FieldValueIdPair destinationTrademark = GetFieldValueIdPair(documentRow, "ows_Trademarks");
                        FieldValueIdPair fileNameReference = GetFieldValueIdPair(documentRow, "ows_FileLeafRef");
                        FieldValueIdPair uniqueId = GetFieldValueIdPair(documentRow, "ows_UniqueId");


                        string destinationCase = string.Empty;
                        bool isPatent = false;
                        bool isTrademark = false;

                        if (destinationPatent.FieldId != null && destinationPatent.FieldId != string.Empty && !destinationPatent.FieldId.Equals("0"))
                        {
                            destinationCase = Config["SharePointSiteUrl"] + Config["PatentSharePointLibraryName"] + destinationPatent.FieldValue + "/" + fileNameReference.FieldValue;
                            isPatent = true;
                        }
                        else if (destinationTrademark.FieldId != null && destinationTrademark.FieldId != string.Empty && !destinationTrademark.FieldId.Equals("0"))
                        {
                            destinationCase = Config["SharePointSiteUrl"] + Config["TrademarkSharePointLibraryName"] + destinationTrademark.FieldValue + "/" + fileNameReference.FieldValue;
                            isTrademark = true;
                        }

                        routedDocuments.Add(new EmailDocument
                        {
                            DocumentId = fileId,
                            DocumentPath = filePath,
                            DestinationUrl = destinationCase,
                            IsPatent = isPatent,
                            IsTrademark = isTrademark,
                            DestinationCase = (destinationPatent.FieldId != null && destinationPatent.FieldId != string.Empty && !destinationPatent.FieldId.Equals("0")) ? destinationPatent.FieldValue : destinationTrademark.FieldValue,
                            UniqueId = uniqueId.FieldValue
                        });
                    }
                    catch (Exception e)
                    {
                        Logger logger = LogManager.GetCurrentClassLogger();
                        logger.Log(LogLevel.Error, "GetRoutedDocuments: Exception occurred when determining a routed document filepath and ID", e);
                    }
                }
            }
            return routedDocuments;
        }

        private XElement GetXElement(XmlNode node)
        {
            XDocument xdoc = new XDocument();
            using (XmlWriter xmlWriter = xdoc.CreateWriter())
            {
                node.WriteTo(xmlWriter);
            }
            return xdoc.Root;
        }

        private string GetFilePropertyValue(string propertyName, FieldInformation[] basicFieldInfoArray)
        {
            string value = string.Empty;

            try
            {
                IEnumerable<string> fieldPropertyValue = (from fieldInfo in basicFieldInfoArray where fieldInfo.DisplayName.Equals(propertyName) select fieldInfo.Value);
                if (fieldPropertyValue != null && fieldPropertyValue.First() != null)
                {
                    value = fieldPropertyValue.First();
                }
            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Error, "GetFilePropertyValue: Exception occurred when trying to get document type or subtype (most likely)", e);
            }

            return value;
        }

        private FieldValueIdPair GetFieldValueIdPair(XElement element, string sharePointFieldId)
        {
            FieldValueIdPair fieldValueIdPair = new FieldValueIdPair();

            try
            {
                IEnumerable<XAttribute> filePathOriginal = element.Attributes(XName.Get(sharePointFieldId));

                if (filePathOriginal != null && filePathOriginal.Count() > 0)
                {
                    string filePathString = filePathOriginal.First().Value;
                    string filePath = filePathString.Substring(filePathString.IndexOf("#"), (filePathString.Length - filePathString.IndexOf("#")));
                    string fileId = filePathString.Substring(0, filePathString.IndexOf(";"));
                    fieldValueIdPair.FieldId = fileId;
                    fieldValueIdPair.FieldValue = filePath.Replace("#", "");
                }
            }
            catch (Exception e)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Log(LogLevel.Error, "GetFieldValueIdPair: Exception occurred", e);
            }

            return fieldValueIdPair;
        }

        class FieldValueIdPair
        {
            public string FieldValue;
            public string FieldId;
        }

        public void LoadConfig()
        {
            try
            {
                Config.Add("SharePointSiteUrl", System.Configuration.ConfigurationManager.AppSettings["SharePointSiteUrl"].ToString());
                Config.Add("SharePointLibraryName", System.Configuration.ConfigurationManager.AppSettings["SharePointLibraryName"].ToString());
                Config.Add("TrademarkSharePointLibraryName", System.Configuration.ConfigurationManager.AppSettings["TrademarkSharePointLibraryName"].ToString());
                Config.Add("PatentSharePointLibraryName", System.Configuration.ConfigurationManager.AppSettings["PatentSharePointLibraryName"].ToString());
                Config.Add("RoutingLog", System.Configuration.ConfigurationManager.AppSettings["RoutingLog"].ToString());
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
