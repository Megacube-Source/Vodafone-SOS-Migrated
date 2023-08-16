using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LRawDataColumnViewModel
    {
        public int Id { get; set; }
        public int LrdcRawDataTypeId { get; set; }

        [RestrictSpecialChar]
        public string LrdcColumnName { get; set; }

        [RestrictSpecialChar]
        public string LrdcDisplayLabel { get; set; }

        [RestrictSpecialChar]
        public string LrdcDataType { get; set; }
        public int LrdcOrdinalPosition { get; set; }
        public bool LrdcIsDisplayable { get; set; }

        [RestrictSpecialChar]
        public string RcName { get; set; }
    }
}