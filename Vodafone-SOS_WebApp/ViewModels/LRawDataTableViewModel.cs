using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;



namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LRawDataTableViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "RawData Table Company Id is required")]
        [Display(Name = "RawData Table Company Id")]
        public int LrdtCompanyId { get; set; }

        [MaxLength(200, ErrorMessage = "RawData Table Name can be maximum 200 characters")]
        [Required(ErrorMessage = "RawData Table  Name is required")]
        [Display(Name = "RawData Table  Name")]
        [RestrictSpecialChar]
        public string LrdtName { get; set; }

        [MaxLength(2000, ErrorMessage = "RawData Table Description can be maximum 2000 characters")]
        [Required(ErrorMessage = "RawData Table  Description is required")]
        [Display(Name = "RawData Table  Description")]
        [RestrictSpecialChar]
        public string LrdtDescription { get; set; }

        //new column used in mapping raw Data tables
        public bool IsRawDataTableMapped { get; set; }
    }
}