using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class AspnetUserViewModel
    {
        [RestrictSpecialChar]
        public string Id { get; set; }

        [MaxLength(256, ErrorMessage = "The User Name can be maximum 256 characters")]
        [Required(ErrorMessage = "User Name is required")]
        [Display(Name = "User  Name")]
        [RestrictSpecialChar]
        public string UserName { get; set; }

        [RestrictSpecialChar]
         public string RolesList { get; set; }

        [RestrictSpecialChar]
        public string FullName { get; set; }

        [RestrictSpecialChar]
        [EmailAddress]
        public string Email { get; set; }

        public Nullable<int> GcCompanyId { get; set; }
    }
}