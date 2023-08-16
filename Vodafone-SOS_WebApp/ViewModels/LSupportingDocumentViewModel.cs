using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LSupportingDocumentViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string LsdCreatedById { get; set; }

        [RestrictSpecialChar]
        public string LsdUpdatedById { get; set; }

        [RestrictSpecialChar]
        public string LsdFileName { get; set; }

        [RestrictSpecialChar]
        public string LsdFilePath { get; set; }

        [RestrictSpecialChar]
        public string LsdEntityType { get; set; }
        public int LsdEntityId { get; set; }
        public System.DateTime LsdCreatedDateTime { get; set; }
        public System.DateTime LsdUpdatedDateTime { get; set; }

    }
}