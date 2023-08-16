using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;



namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LWorkflowGridColumnViewModel
    {
        public int Id { get; set; }

        [MaxLength(200, ErrorMessage = "The Work Flow Grid Column Name can be maximum 200 characters")]
        [Required(ErrorMessage = " Work Flow Grid Column Name  is required")]
        [Display(Name = " WFGrid Column  Name")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LwfgcColumnName { get; set; }

        [MaxLength(50, ErrorMessage = "The UI Label Name can be maximum 50 characters")]
        [Display(Name = " UI Label  Name")]
        [RestrictSpecialChar]
        public string LwfgcUILabel { get; set; }

        [Required]
        [Display(Name = " Should Be Visible")]
        public bool LwfgcShouldBeVisible { get; set; }

        [Display(Name = " OrderByOrdinal")]
        public Nullable<int> LwfgcOrderByOrdinal { get; set; }

        [MaxLength(1, ErrorMessage = "The AscDesc can be maximum 1 characters")]
        [Display(Name = " Asc Desc")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LwfgcAscDesc { get; set; }

        [Required(ErrorMessage = " Config Id  is required")]
        [Display(Name = " Config")]
        public int LwfgcWfConfigId { get; set; }

        [Display(Name = " Ordinal")]

        public Nullable<int> LwfgcOrdinal { get; set; }

        [Display(Name = " Function Name")]
        [RestrictSpecialChar]
        public string LwfgcFunctionName { get; set; }
        [RestrictSpecialChar]
        public string ActingAs { get; set; }
        public int RoleId { get; set; }

        [RestrictSpecialChar]
        public string Role { get; set; }

    }
}