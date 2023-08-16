using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class SPDBContextEncryptedViewModel
    {
            public string DatabaseName { get; set; }
            public string HostName { get; set; }
            public string LoginId { get; set; }
            public string Password { get; set; }
            public string SchemaName { get; set; }
            public string LinkServerName { get; set; }
    }
}