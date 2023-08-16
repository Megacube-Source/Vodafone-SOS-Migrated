using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LAllocationRuleViewModel
    {
        public int Id { get; set; }
       
        [Required(ErrorMessage = "Ordinal Position is required")]
        [Display(Name = "Ordinal Position")]
        public int LarOrdinalPosition { get; set; }

        [MaxLength(100, ErrorMessage = " Key  can be maximum 100 characters")]
        [Required(ErrorMessage = "key is required")]
        [Display(Name = "Key")]
        [RestrictSpecialChar]
        public string LarKey { get; set; }

        [MaxLength(1000, ErrorMessage = " Value can be maximum 1000 characters")]
        [Required(ErrorMessage = "Value is required")]
        [Display(Name = "Value")]
        [RestrictSpecialChar]
        public string LarValue { get; set; }
        
        [Required(ErrorMessage = "Company ID is required")]
        [Display(Name = "Company ID")]
      
        public int LarCompanyId { get; set; }

        [MaxLength(128, ErrorMessage = "The Allocated To ID can be maximum 128 characters")]
        [Required(ErrorMessage = "Allocated To ID is required")]
        [Display(Name = "Allocated TO  ")]
        [RestrictSpecialChar]
        public string LarAllocatedToId { get; set; }

        [RestrictSpecialChar]
        public string UserName { get; set; }

        [RestrictSpecialChar]
        //string to store grid data 
        public string GridData { get; set; }
    }
}