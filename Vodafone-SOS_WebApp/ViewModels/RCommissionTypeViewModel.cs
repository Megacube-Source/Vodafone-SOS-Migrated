using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class RCommissionTypeViewModel
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The Commission Type Name can be maximum 50 characters")]
        [Required(ErrorMessage = "Commission Type Name is required")]
        [Display(Name = "Commission Type Name")]
        [RestrictSpecialChar]
        public string RctName { get; set; }

        [MaxLength(2000, ErrorMessage = "The Commission Type Description can be maximum 2000 characters")]
        [Display(Name = "Commission Type Description")]
        [RestrictSpecialChar]
        public string RctDescription { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int RctCompanyId { get; set; }

        //object of parent table is defined to get desired objects instead of Table data of entire table
        [Display(Name = "Company Name")]
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        [Required(ErrorMessage = "Is Active is required")]
        [Display(Name = "Is Active")]
        public bool RctIsActive { get; set; }
    }
}