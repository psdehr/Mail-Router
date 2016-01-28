using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMS_Connector.RepositoryInterface
{
    public interface IDocumentRouterInterface
    {
        void LoadConfig();
        void RouteDocuments();
    }
}
