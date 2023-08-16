using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;



namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class MGMenusAspnetRoleViewModel
    {
        public int Id { get; set; }

        [MaxLength(128, ErrorMessage = "Role Id can be maximum 128 characters")]
        [Required(ErrorMessage = " Role Id  is required")]
        [Display(Name = " Role Id")]
        [RestrictSpecialChar]
        public string MgmarRoleId { get; set; }

        [Required(ErrorMessage = " Menu Id  is required")]
        [Display(Name = " Menu Id")]
        public int MgmarMenuId { get; set; }

        [RestrictSpecialChar]
        public string GmMenuName { get; set; }

        [RestrictSpecialChar]
        public string Name { get; set; }
        public Nullable<int> GmParentId { get; set; }

        [RestrictSpecialChar]
        public string GmMenuUrl { get; set; }
        public int GmOrdinalPosition { get; set; }
    }
}