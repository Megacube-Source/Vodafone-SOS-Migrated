using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class AspnetRoleViewModel
    {
        public string Id { get; set; }


        [Required(ErrorMessage ="Role Name is required")]
        [MaxLength(256, ErrorMessage = "The Role Name can be maximum 256 characters")]
        [Display(Name = "Role Name")]
        [RestrictSpecialChar]
        public string Name { get; set; }

        [Required(ErrorMessage = "Company Code is required")]
        [MaxLength(2, ErrorMessage = "The Company Code can be maximum 2 characters")]
        [Display(Name = "Company Code")]
        [RestrictSpecialChar]
        public string CompanyCode { get; set; }

        public bool ShowDashboard { get; set; }

        [Display(Name = "Assign Menu")]
        [RestrictSpecialChar]
        public string MenuList { get; set; }

        public bool Selected { get; set; }//This variable is used to get role is assigned for current user
    }
}