using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class Sp_UpdateItemStatusViewModel
    {
        [RestrictSpecialChar]
        public string ItemList { get; set; }

        [RestrictSpecialChar]
        public string Comments { get; set; }

        [RestrictSpecialChar]
        public string StatusName { get; set; }
        public int StatusId { get; set; }


        [RestrictSpecialChar]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}