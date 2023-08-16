using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class RDeviceTypeViewModel
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The Device Type Name can be maximum 50 characters")]
        [Required(ErrorMessage = "Device Type Name is required")]
        [Display(Name = "Device Type Name")]
        [RestrictSpecialChar]
        public string RdtName { get; set; }

        [MaxLength(2000, ErrorMessage = "The Device Type Description can be maximum 2000 characters")]
        [Display(Name = "Device Type Description")]
        [RestrictSpecialChar]
        public string RdtDescription { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int RdtCompanyId { get; set; }

        //object of parent table is defined to receive only required objects from api and not the parent table as a whole
        [Display(Name = "Company Name")]
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        [Required(ErrorMessage = "Is Active is required")]
        [Display(Name = "Is Active")]
        public bool RdtIsActive { get; set; }
    }
}