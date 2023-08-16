using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class GMenuViewModel
    {
        public int Id { get; set; }

        [Display(Name="Parent Id")]
        public Nullable<int> GmParentId { get; set; }

        [MaxLength(50, ErrorMessage = " Menu Name can be maximum 50 characters")]
        [Display(Name = "Menu Name")]
        [RestrictSpecialChar]
        public string GmMenuName { get; set; }

        [Display(Name = "Role List")]
        [RestrictSpecialChar]
        public string RoleList { get; set; }

        [MaxLength(2000, ErrorMessage = "The Menu Url  can be maximum 2000 characters")]
        [Required(ErrorMessage = "Menu Url is required")]
        [Display(Name = "Menu Url")]
        [RestrictSpecialChar]
        public string GmMenuUrl { get; set; }
       
        [Display(Name = "Ordinal Position")]
        public int GmOrdinalPosition { get; set; }
        [RestrictSpecialChar]
        public string ParentName { get; set; }
    }
}