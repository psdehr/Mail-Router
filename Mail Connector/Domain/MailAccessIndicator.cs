using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mail_Connector.Domain
{
    public enum MailAccessIndicator
    {
        LoginError,
        LoggedIn,
        MailFetchError,
        MailChecked,
        MailFetched
    }
}
