using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS_Connector.Domain
{
    public class EmailDocument
    {
        public string DestinationCase;
        public string DocumentPath;
        public string DocumentId;
        public string DestinationDocumentId;
        public string DestinationUrl;
        public bool RoutedSuccessfully;
        public bool RoutedDeletedSuccessfully;
        public bool IsPatent;
        public bool IsTrademark;
        public string UniqueId;
        public string DocumentType;
        public string DocumentSubtype;
    }
}
