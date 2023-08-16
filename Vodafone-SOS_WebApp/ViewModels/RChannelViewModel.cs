using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class RChannelViewModel
    {
        public RChannelViewModel()
            {
            RcIsActive = true;
            }
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int RcCompanyId { get; set; }

        [MaxLength(100, ErrorMessage = "The Channel Name can be maximum 100 characters")]
        [Required(ErrorMessage = "Channel Name is required")]
        [Display(Name = "Channel Name")]
        [RestrictSpecialChar]
        public string RcName { get; set; }

        [MaxLength(2000, ErrorMessage = "The Channel Description can be maximum 2000 characters")]
        [Display(Name = "Channel Description")]
        [RestrictSpecialChar]
        public string RcDescription { get; set; }

        [MaxLength(50, ErrorMessage = "The Primary Channel can be maximum 50 characters")]
        [Required(ErrorMessage = "Primary Channel is required")]
        [Display(Name = "Primary Channel")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string RcPrimaryChannel { get; set; }

         [Display(Name = "Company")]
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        [Required(ErrorMessage = "Is Active is required")]
        [Display(Name = "Is Active")]
        public bool RcIsActive { get; set; }
    }
}