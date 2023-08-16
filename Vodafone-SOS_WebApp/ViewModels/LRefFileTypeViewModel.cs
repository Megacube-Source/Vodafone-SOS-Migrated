using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;
using System.ComponentModel.DataAnnotations;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LRefFileTypeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Company Id Is Required")]
        [Display(Name ="Company Id")]
        public int LrftCompanyId { get; set; }

        [MaxLength(128, ErrorMessage = "Created By Id  can be maximum 128 characters")]
        [Required(ErrorMessage = "Created By Id is required")]
        [Display(Name = "Created By")]
        [RestrictSpecialChar]
        public string LrftCreatedById { get; set; }

        [MaxLength(128, ErrorMessage = "Updated By Id  can be maximum 128 characters")]
        [Required(ErrorMessage = "Updated By Id is required")]
        [Display(Name = "Updated By")]
        [RestrictSpecialChar]
        public string LrftUpdatedById { get; set; }

        [MaxLength(100, ErrorMessage = "Ref File Name can be maximum 100 characters")]
        [Required(ErrorMessage = "Ref File Name is required")]
        [Display(Name = "Ref File Name")]
        [RestrictSpecialChar]
        public string LrftName { get; set; }

        [MaxLength(1000, ErrorMessage = "Ref File Description can be maximum 1000 characters")]
       // [Required(ErrorMessage = "Ref File Description is required")]
        [Display(Name = "Ref File Description")]
        [RestrictSpecialChar]
        public string LrftDescription { get; set; }

        [Display(Name = "Created Date Time")]
        public Nullable<System.DateTime> LrftCreatedDateTime { get; set; }

        [Display(Name = "Updated  Date Time")]
        public Nullable<System.DateTime> LrftUpdatedDateTime { get; set; }
        //object defined to get data of grid in comma seperated list
        [RestrictSpecialChar]
        public string[] ModelData { get; set; }
    }
}