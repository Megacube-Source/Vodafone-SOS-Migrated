using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class Sp_UpdateStatusViewModel
    {
        [RestrictSpecialChar]
        public string ClaimsList { get; set; }

        [RestrictSpecialChar]
        public string StatusName { get; set; }
        public Nullable<DateTime> AllocationDate { get; set; }

        [Required]
        [Display(Name = "Allocated To")]

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string AllocateTo { get; set; }

        [RestrictSpecialChar]
        public string AllocatedBy { get; set; }

        public Nullable<DateTime> ApprovedDate { get; set; }

        [RestrictSpecialChar]
        public string ApprovedBy { get; set; }

        [Required]
        [Display(Name = "Rejection Reason")]
        public Nullable<int> RejectionReasonId { get; set; }
        public Nullable<DateTime> LastReClaimDate { get; set; }
        public bool IsReClaim { get; set; }

        [RestrictSpecialChar]
        public string Comments { get; set; }
    }
}