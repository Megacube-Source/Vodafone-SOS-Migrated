using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LCompanySpecificRawDataColumnViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Raw Data Table Id  is required")]
        [Display(Name = "Raw Data Table Id")]
        public int LcsrdcRawDataTableId { get; set; }

        [MaxLength(50, ErrorMessage = "Display Label1 can be maximum 50 characters")]
        [Required(ErrorMessage = "Display Label1 is required")]
        [Display(Name = "Display Label1")]
        [RestrictSpecialChar]

        public string LcsrdcDisplayLabel { get; set; }


        [MaxLength(10, ErrorMessage = "Column DataType can be maximum 10 characters")]
        [Required(ErrorMessage = "Column DataType is required")]
        [Display(Name = "Column DataType")]
        [RestrictSpecialChar]

        public string LcsrdcDataType { get; set; }


        [Required(ErrorMessage = "Ordinal Position is required")]
        [Display(Name = "Ordinal Position")]
        public int LcsrdcOrdinalPosition { get; set; }

        [Display(Name = "Is Displayable")]
        public bool LcsrdcIsDisplayable { get; set; }

        //[MaxLength(255, ErrorMessage = "LColumn Name can be maximum 255 characters")]
        //[Display(Name = "LColumn Name")]
        //[RestrictSpecialChar]

        //public string LcsrdcLColumnName { get; set; }

        [MaxLength(255, ErrorMessage = "XColumn Name can be maximum 255 characters")]
        [Display(Name = "XColumn Name")]
        [RestrictSpecialChar]

        public string LcsrdcXColumnName { get; set; }
    }
}