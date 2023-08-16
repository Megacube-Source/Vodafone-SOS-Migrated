using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LWorkflowConfigViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string Name { get; set; }

        [Display(Name = "WorkFlow")]
        public int LwfcWorkFlowId { get; set; }

        [Display(Name = "Role")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LwfcRoleId { get; set; }

        [RestrictSpecialChar]
        public string RoleName { get; set; }

        [Display(Name = "Company")]
        public int LwfcCompanyId { get; set; }

        [Display(Name = "Ordinal")]
        public int LwfcOrdinalNumber { get; set; }

        [Display(Name = "Description")]
        [RestrictSpecialChar]
        public string LwfcDescription { get; set; }

        [Display(Name = "CanCreate")]
        public bool LwfcCanCreate { get; set; }

        [Display(Name = "DoNotNotify")]
        public bool LwfcDoNotNotify { get; set; }

        [Display(Name = "ActingAs")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LwfcActingAs { get; set; }

        [Required]
        [Display(Name = "Skip")]
        public bool LwfcSkip { get; set; }

        [Display(Name = "SkipFunctionName")]
        [RestrictSpecialChar]
        public string LwfcSkipFunctionName { get; set; }


    }
}