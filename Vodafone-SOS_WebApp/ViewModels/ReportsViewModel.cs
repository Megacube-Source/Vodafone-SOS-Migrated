using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class ReportsViewModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }

        [RestrictSpecialChar]
        public string XCommissionPeriod { get; set; }

        [RestrictSpecialChar]
        public string XDisplayLocation { get; set; }

        [RestrictSpecialChar]
        public string XName { get; set; }
        //Graphs

        [RestrictSpecialChar]
        public string PayeeName { get; set; }

        [RestrictSpecialChar]
        public string KPIValue { get; set; }
    }

    public class MyReportsViewModel
    {
        [RestrictSpecialChar]
        public String strFileName { get; set; }

        [RestrictSpecialChar]
        public string strParent { get; set; }
    }
}