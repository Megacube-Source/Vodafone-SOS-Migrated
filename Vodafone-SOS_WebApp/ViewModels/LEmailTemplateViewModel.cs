using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LEmailTemplateViewModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Template Name is required")]
        [Display(Name = "Template Name")]
        [RestrictSpecialChar]
        public string LetTemplateName { get; set; }


        [Required(ErrorMessage = "Email Subject is required")]
        [Display(Name = "Email Subject")]
        [RestrictSpecialChar]
        public string LetEmailSubject { get; set; }

        
        [Required(ErrorMessage = "Email Body is required")]
        [Display(Name = "Email Body")]
        [AllowHtml]
        [RestrictSpecialChar]
        public string LetEmailBody { get; set; }

        
        [Display(Name = "Signature")]
        [AllowHtml]
        [RestrictSpecialChar]
        public string LetSignature { get; set; }
        public int LetCompanyId { get; set; }

    }
}