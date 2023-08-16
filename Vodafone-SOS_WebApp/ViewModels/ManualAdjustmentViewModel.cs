using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class ManualAdjustmentViewModel
    {
        [Required]
        [RestrictSpecialChar]
        [Display(Name ="Batch Name")]
        public string BatchName { get; set; }

        [RestrictSpecialChar]
        public string PortfolioList { get; set; }

        [Required]
        //  [RestrictSpecialChar] as not required for dropdown fields
        [Display(Name ="Commission Period")]
        public string CommissionPeriod { get; set; }
    }
}