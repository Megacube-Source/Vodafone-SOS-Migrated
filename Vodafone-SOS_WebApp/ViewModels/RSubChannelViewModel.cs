using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial  class RSubChannelViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sub Channel Name is required")]
        [MaxLength(100, ErrorMessage = "The Sub Channel Name can be maximum 100 characters")]
        [Display(Name="Sub Channel Name")]
        [RestrictSpecialChar]
        public string RscName { get; set; }

        [MaxLength(2000, ErrorMessage = "The Sub Channel Description can be maximum 2000 characters")]
        [Display(Name = "Sub Channel Description")]
        [RestrictSpecialChar]
        public string RscDescription { get; set; }

        [Required(ErrorMessage = "Sub Channel Id is required")]
        [Display(Name = "Sub Channel Id")]
        public int RscChannelId { get; set; }

        [Display(Name = "Company Name")]
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        [Required(ErrorMessage ="Channel Name is Required")]
        [RestrictSpecialChar]
        public string RcName { get; set; }

        [Required(ErrorMessage = "Is Active is required")]
        [Display(Name = "Is Active")]
        public bool RscIsActive { get; set; }
    }
}