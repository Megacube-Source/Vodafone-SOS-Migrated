using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LSchemeViewModel
    {
        //
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string ParameterCarrier { get; set; }
        [Required]
        [Display(Name ="Name")]
        [RestrictSpecialChar]
        public string LsName { get; set; }

        [Display(Name = "Description")]
        [RestrictSpecialChar]
        public string LsDescription { get; set; }
       /* public string LsComments { get; set; }replaced with WFComments*/
        public bool LsIsSchemeTested { get; set; }
        [RestrictSpecialChar]
        public string LsCreatedById { get; set; }
        [RestrictSpecialChar]
        public string LsUpdatedById { get; set; }
        public int LsCompanyId { get; set; }
        public System.DateTime LsCreatedDateTime { get; set; }
        public System.DateTime LsUpdatedDateTime { get; set; }
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
        public string WFStatus { get; set; }
        [RestrictSpecialChar]
        public string WFType { get; set; }
        public int WFCompanyId { get; set; }

        [Display(Name ="Comments")]
        [RestrictSpecialChar]
        public string WFComments { get; set; }
    }
}