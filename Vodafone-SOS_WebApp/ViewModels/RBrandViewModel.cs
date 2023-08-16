using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class RBrandViewModel
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The Brand Name can be maximum 50 characters")]
        [Required(ErrorMessage = "Brand Name is required")]
        [Display(Name = "Brand Name")]
        [RestrictSpecialChar]
        public string RbName { get; set; }

        [MaxLength(2000, ErrorMessage = "The Brand Description can be maximum 2000 characters")]
        [Display(Name = "Brand Description")]
        [RestrictSpecialChar]
        public string RbDescription { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int RbCompanyId { get; set; }

        //object of parent table is defined to get desired objects instead of Table data of entire table
        [Display(Name = "Company Name")]
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        [RestrictSpecialChar]
        public string RoleList { get; set; }

        [Required(ErrorMessage = "Is Active is required")]
        [Display(Name = "Is Active")]
        public bool RbIsActive { get; set; }
    }
}