using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LAccrualViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public String ParameterCarrier { get; set; }

        [Display(Name ="Name")]
        [Required]
        [RestrictSpecialChar]
        public string LaName { get; set; }

        [Display(Name = "Description")]
        [RestrictSpecialChar]
        public string LaDescription { get; set; }
        [RestrictSpecialChar]
        public string LaCreatedById { get; set; }
        [RestrictSpecialChar]
        public string LaUpdatedById { get; set; }
        public System.DateTime LaCreatedDateTime { get; set; }
        public System.DateTime LaUpdatedDateTime { get; set; }

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

        [RestrictSpecialChar]
        [Display(Name ="Comments")]
        public string WFComments { get; set; }

        [Display(Name = "Commission Period")]
        [Required]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LaCommissionPeriod { get; set; }

        [Display(Name = "Accrual Amount")]
        [Required]
        [Range(0.1,double.MaxValue,ErrorMessage ="Accrual Amount should be greater than zero")]
        public decimal LaAccrualAmount { get; set; }
    }
}