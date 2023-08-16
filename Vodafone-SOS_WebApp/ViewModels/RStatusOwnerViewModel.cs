using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class RStatusOwnerViewModel
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The Status Owner can be maximum 50 characters")]
        [Required(ErrorMessage = "Status Owner  is required")]
        [Display(Name = "Status Owner ")]
        [RestrictSpecialChar]
        public string RsoStatusOwner { get; set; }
    }
}