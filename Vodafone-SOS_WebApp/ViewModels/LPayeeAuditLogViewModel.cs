using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LPayeeAuditLogViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
         public string LpalUpdatedById { get; set; }

        [RestrictSpecialChar]
         public string LpalAction { get; set; }
        public System.DateTime LpalUpdatedDateTime { get; set; }
        public int LpalPayeeId { get; set; }
        //public string FullName { get; set; }
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        [RestrictSpecialChar]
        public string LpPayeeCode { get; set; }

        [RestrictSpecialChar]
        public string CreatedBy { get; set; }

        [RestrictSpecialChar]
        public string UpdatedBy { get; set; }
        public DateTime LpCreatedDateTime { get; set; }

        [RestrictSpecialChar]
        public string FullName { get; set; }

      //  public virtual AspNetUser AspNetUser { get; set; }

      
    }
}