using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class RProductCodeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int RpcCompanyId { get; set; }

        [Display(Name ="Product Code")]
        [Required(ErrorMessage = "Product Code is required")]
        [MaxLength(50, ErrorMessage = "The Product Code can be maximum 50 characters")]
        [RestrictSpecialChar]
        public string RpcProductCode { get; set; }

        [Display(Name ="Product Code Description")]
        [MaxLength(2000, ErrorMessage = "The Product Code Description can be maximum 2000 characters")]
        [RestrictSpecialChar]
        public string RpcDescription { get; set; }

        [Display(Name = "Company")]
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        [Required(ErrorMessage = "Is Active is required")]
        [Display(Name = "Is Active")]
        public bool RpcIsActive { get; set; }
    }
}