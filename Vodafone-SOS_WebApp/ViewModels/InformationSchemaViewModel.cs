using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class InformationSchemaViewModel
    {
        [RestrictSpecialChar]
        public string COLUMN_NAME { get; set; }
    }
}