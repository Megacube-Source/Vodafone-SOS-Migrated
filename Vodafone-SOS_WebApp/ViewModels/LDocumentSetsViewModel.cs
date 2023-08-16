using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LDocumentSetsViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string ParameterCarrier { get; set; }

        [RestrictSpecialChar]
        [Required]
        [Display(Name ="Name")]
        public string LdsName { get; set; }

        [RestrictSpecialChar]
        public string LdsDescription { get; set; }

        [Required]
        [Display(Name = "Commission Period")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LdsCommissionPeriod { get; set; }

        [Display(Name = "Send Email")]
        public bool LdsSendEmail { get; set; }

        [RestrictSpecialChar]
        public string LdsCreatedById { get; set; }

        [RestrictSpecialChar]
        public string LdsUpdatedById { get; set; }
        public System.DateTime LdsCreatedDateTime { get; set; }
        public System.DateTime LdsUpdatedDateTime { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterId { get; set; }

        [RestrictSpecialChar]
        public string WFAnalystId { get; set; }

        [RestrictSpecialChar]
        public string WFManagerId { get; set; }

        [RestrictSpecialChar]
        public string WFCurrentOwnerId { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterRoleId { get; set; }
        public Nullable<int> WFOrdinal { get; set; }

        [RestrictSpecialChar]
        public string LdsPayeeList { get; set; }
        [RestrictSpecialChar]
        public string LdsDocumentList { get; set; }

        [RestrictSpecialChar]
        public string WFStatus { get; set; }

        [RestrictSpecialChar]
        public string WFType { get; set; }
        public int WFCompanyId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Comments")]
        public string WFComments { get; set; }

        [RestrictSpecialChar]
        public string PayeeSelection { get; set; }

        [RestrictSpecialChar]
        public string AttachedFiles { get; set; }

        [RestrictSpecialChar]
        public string SupportingDocumentFiles { get; set; }

        [RestrictSpecialChar]
        public string PayeeListCarrier { get; set; }
    }
}