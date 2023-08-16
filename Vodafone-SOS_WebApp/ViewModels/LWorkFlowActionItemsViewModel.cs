using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LWorkFlowActionItemsViewModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "ActionItem Name")]
        [MaxLength(100, ErrorMessage = "Action Item Name can be maximum 100 characters")]
        [RestrictSpecialChar]
        public string LwfaiActionItemName { get; set; }

        [Required]
        [Display(Name = "UI Label")]
        [RestrictSpecialChar]
        public string LwfaiUILabel { get; set; }

        [Display(Name = "Action Description")]
        [RestrictSpecialChar]
        public string LwfaiActionDescription { get; set; }

        [Required]
        [Display(Name = "Login WF Config")]
        public int LwfaiLoginWFConfigId { get; set; }

        [Required]
        [Display(Name = "Is Button On WF Grid")]
        public bool LwfaiIsButtonOnWfGrid { get; set; }

        [Required]
        [Display(Name = "IS Button on Form")]
        public bool LwfaiIsButtonOnForm { get; set; }

        [Display(Name = "ActionURL")]
        [RestrictSpecialChar]
        public string LwfaiActionURL { get; set; }

        [Display(Name = "ShowInTabWFConfig")]
        public Nullable<int> LwfaiShowInTabWFConfigId { get; set; }

        [Display(Name = "Ordinal")]
        public Nullable<int> LwfaiOrdinal { get; set; }

        [RestrictSpecialChar]
        public string ActingAs { get; set; }

        [RestrictSpecialChar]
        public string Role { get; set; }
        public int RoleId { get; set; }

        [RestrictSpecialChar]
        public string LoginConfigName { get; set; }

        [RestrictSpecialChar]
        public string ShowInTabConfigName { get; set; }

    }
}