using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class RStatusViewModel
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The Status  can be maximum 50 characters")]
        [Required(ErrorMessage = "Status  is required")]
        [Display(Name = "Status ")]
        [RestrictSpecialChar]
        public string RsStatus { get; set; }

        [MaxLength(2000, ErrorMessage = "The Status Description can be maximum 2000 characters")]
        [Display(Name = "Status Description")]
        [RestrictSpecialChar]
        public string RsStatusDescription { get; set; }

        [Required(ErrorMessage = "Owner Id is required")]
        [Display(Name = "Owner Id")]
        public int RsOwnerId { get; set; }

        //Object of Parent Table defined to get selected objects from table and table as a whole

        [Display(Name = "Status Owner")]
        [RestrictSpecialChar]
        public string RsoStatusOwner { get; set; }

    }
}