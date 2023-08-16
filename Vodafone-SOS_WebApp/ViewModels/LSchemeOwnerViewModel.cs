using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;
using System.ComponentModel.DataAnnotations;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LSchemeOwnerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Company Id Is Required")]
        [Display(Name ="Company Id")]
        public int  CompanyId { get; set; }

        [MaxLength(128, ErrorMessage = "Created By Id  can be maximum 128 characters")]
        [Required(ErrorMessage = "Created By Id is required")]
        [Display(Name = "Created By")]
        [RestrictSpecialChar]
        public string  CreatedById { get; set; }

        [MaxLength(128, ErrorMessage = "Updated By Id  can be maximum 128 characters")]
        [Required(ErrorMessage = "Updated By Id is required")]
        [Display(Name = "Updated By")]
        [RestrictSpecialChar]
        public string  UpdatedById { get; set; }

        [MaxLength(100, ErrorMessage = "Scheme Name can be maximum 255 characters")]
        [Required(ErrorMessage = "Scheme Name is required")]
        [Display(Name = "Scheme Name")]
        [RestrictSpecialChar]
        public string Scheme { get; set; }
        [MaxLength(100, ErrorMessage = "Owner can be maximum 255 characters")]
        [Required(ErrorMessage = "Owner is required")]
        [Display(Name = "Owner(Reporting Analyst)")]
        public string OwnerId { get; set; }

        [Display(Name = "Created Date Time")]
        public  DateTime  CreatedDateTime { get; set; }

        [Display(Name = "Updated  Date Time")]
        public   DateTime   UpdatedDateTime { get; set; }
        public string OwnerName { get; set; }


    }
}