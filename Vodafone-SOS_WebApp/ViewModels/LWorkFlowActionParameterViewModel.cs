using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LWorkFlowActionParameterViewModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "ParameterName")]
        [RestrictSpecialChar]
        public string ParameterName { get; set; }
        
        [Required]
        [Display(Name = "Parameter VallueType")]
        [RestrictSpecialChar]
        public string ParameterValueType { get; set; }

        [Required]
        [Display(Name = "Parameter Value")]
        [RestrictSpecialChar]
        public string ParameterValue { get; set; }

        [Required]
        [Display(Name = "WFActionItem")]
        public int WFActionItemId { get; set; }

        [RestrictSpecialChar]
        public string ActionName { get; set; }
    }
}