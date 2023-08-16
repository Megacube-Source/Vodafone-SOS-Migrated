using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class GUserActivityLogViewModel
    {
        public int Id { get; set; }
        [RestrictSpecialChar]
        public string UalActivity { get; set; }
        [RestrictSpecialChar]
        public string UalRemarks { get; set; }

        public bool UalIsActivitySucceeded { get; set; }
        [RestrictSpecialChar]
        public string UalHostIP { get; set; }
        [RestrictSpecialChar]
        public string UalHostBrowserDetails { get; set; }
        [RestrictSpecialChar]
        public string UalHostTimeZone { get; set; }
        public Nullable<System.DateTime> UalActivityDateTime { get; set; }
        [RestrictSpecialChar]
        public string UalUserId { get; set; }
        [RestrictSpecialChar]
        public string UalActionById { get; set; }
        public int UalCompanyId { get; set; }


        //added by RS
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Enter Email")]
        [EmailAddress]
        public string EmailId { get; set; }

        
    }
}