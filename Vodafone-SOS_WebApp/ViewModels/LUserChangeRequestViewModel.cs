using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LUserChangeRequestViewModel
    {
        [Display(Name = "Reports To")]
        [RestrictSpecialChar]
        public string LuReportsToId { get; set; }

        [Display(Name = "First Name")]
        [Required]
        [RestrictSpecialChar]
        [MaxLength(200)]
        public string LuFirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        [RestrictSpecialChar]
        [MaxLength(200)]
        public string LuLastName { get; set; }

        [Display(Name = "Email")]
        [Required]
        [RestrictSpecialChar]
        [MaxLength(200)]
        public string LuEmail { get; set; }

        [Display(Name = "Phone")]
        [MaxLength(20)]
        [RestrictSpecialChar]
        public string LuPhone { get; set; }


        [Display(Name = "Band")]
        [MaxLength(20)]
        [RestrictSpecialChar]
        public string LuBand { get; set; }
    }
}