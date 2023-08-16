using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class GKeyValueViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name="Company Id")]
        
        public int GkvCompanyId { get; set; }

        [MaxLength(100, ErrorMessage = "The Key  can be maximum 100 characters")]
        [Required(ErrorMessage = "Key is required")]
        [Display(Name = "Key")]
        [RestrictSpecialChar]
        public string GkvKey { get; set; }

        //[MaxLength(4000, ErrorMessage = "Key Value  can be maximum 4000 characters")]
        //[Required(ErrorMessage = "Key Value is required")]
        [Display(Name = "Key Value")]
        //[RestrictSpecialChar] as required by Jas to comment it out
        public string GkvValue { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        // [RestrictSpecialChar] as required by Jas to comment it out
        public string GkvDescription { get; set; }

        [Display(Name = "Company Name")]
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

      //  [RestrictSpecialChar] as not required for dropdown fields
        [Display(Name = "GcCode")]
        [Required(ErrorMessage = "CompanyCode is required")]
        public string GcCode { get; set; }

        //RS: this is used in this model, because on L2Admin Page, there is need to get the values from this model also by this we can access two models values from  a single model.
        public List<GUserActivityLogViewModel> GUserActivityLogViewModel { get; set; }
    }
}