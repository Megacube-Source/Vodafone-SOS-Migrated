using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class RRejectionReasonViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int RrrCompanyId { get; set; }

        [Required(ErrorMessage = "Rejection Reason is required")]
        [MaxLength(200, ErrorMessage = "The Rejection Reason can be maximum 200 characters")]
        [Display(Name = "Rejection Reason")]
        //[RestrictSpecialChar] already commented by Shubham
        public string RrrReason { get; set; }

        [MaxLength(2000, ErrorMessage = "The Reason Description can be maximum 2000 characters")]
        [Display(Name = "Reason Description")]
        [RestrictSpecialChar]
        public string RrrDescription { get; set; }
     
        [Display(Name = "Company")]
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        [Required(ErrorMessage = "Is Active is required")]
        [Display(Name = "Is Active")]
        public bool RrrIsActive { get; set; }
    }
}