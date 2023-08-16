using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class GCompanyViewModel
    {
        public int Id { get; set; }

        [MaxLength(200, ErrorMessage = " Company Name can be maximum 200 characters")]
        [Required(ErrorMessage = "Company Name is required")]
        [Display(Name = "Company Name")]
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        [MaxLength(2, ErrorMessage = " Company Code can be maximum 2 characters")]
        [Required(ErrorMessage = "Company Code is required")]
        [Display(Name = "Company Code")]
        [RestrictSpecialChar]
        public string GcCode { get; set; }
    }
}