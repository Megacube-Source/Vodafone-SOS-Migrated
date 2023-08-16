using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class Sp_UpdateClaimsStatusViewModel
    {
       public  string ClaimsList{get;set;}
       public  string StatusName{get;set;}
       public  Nullable<DateTime>AllocationDate{get;set;}

        [Required]
        [Display(Name="Allocated To")]
       public string AllocateTo{get;set;}
        
        public string AllocatedBy{get;set;}
       public  Nullable<DateTime>ApprovedDate{get;set;}
       public  string ApprovedBy{get;set;}

       [Required]
       [Display(Name = "Rejection Reason")]
       public  Nullable<int> RejectionReasonId{get;set;}
      public Nullable<DateTime> LastReClaimDate{get;set;}
      public bool IsReClaim { get; set; }

      public string Comments { get; set; }
    }
}