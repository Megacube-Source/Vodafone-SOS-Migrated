using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class ADUserViewModel
    {
        public ADUserViewModel()
        {
           // newPassword = "Vodafone!23";
            Status = true;
        }
        [RestrictSpecialChar]        
        //SamAccountName or username
        public string SamAccountName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "UserName")]
        public string UserPrincipalName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RestrictSpecialChar]
        public string CurrentPassword { get; set; }

        [RestrictSpecialChar]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("CurrentPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        

        [Display(Name =  "Password")]
        [RestrictSpecialChar]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }
        public bool Status { get; set; }

    }
}