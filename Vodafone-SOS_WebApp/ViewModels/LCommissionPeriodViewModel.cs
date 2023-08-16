using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LCommissionPeriodViewModelForGrid
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string PeriodName { get; set; }
        public DateTime CreatedDateTime { get; set; }

        [RestrictSpecialChar]
        public string Status { get; set; }
    }

    public class LCommissionPeriodViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int LcpCompanyId { get; set; }

        [MaxLength(128, ErrorMessage = "CreatedBy Id  can be maximum 128 characters")]
        [Required(ErrorMessage = "Created By id  Name is required")]
        [Display(Name = "Created By")]
        [RestrictSpecialChar]
        public string LcpCreatedById { get; set; }

        [MaxLength(128, ErrorMessage = "UpdatedBy Id  can be maximum 128 characters")]
        [Required(ErrorMessage = "UpdatedBy id  Name is required")]
        [Display(Name = "Updated By")]
        [RestrictSpecialChar]
        public string LcpUpdatedById { get; set; }

        [MaxLength(20, ErrorMessage = "Status can be maximum 20 characters")]
        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        [RestrictSpecialChar]
        public string LcpStatus { get; set; }

        [MaxLength(20, ErrorMessage = "Period Name   can be maximum 20 characters")]
        [Required(ErrorMessage = "Period  Name is required")]
        [Display(Name = "Period Name")]
        [RestrictSpecialChar]
        public string LcpPeriodName { get; set; }

        [Required(ErrorMessage = "Created Date Time is required")]
        [Display(Name = "Created Date Time")]
        public System.DateTime LcpCreatedDateTime { get; set; }

        [Required(ErrorMessage = "Updated Date Time is required")]
        [Display(Name = "Updated  Date Time")]
        public System.DateTime LcpUpdatedDateTime { get; set; }
    }
}