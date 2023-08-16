using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LCompanySpecificColumnViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int LcscCompanyId { get; set; }

        [MaxLength(500, ErrorMessage = " Table Name can be maximum 500 characters")]
        [Required(ErrorMessage = "Table Name is required")]
        [Display(Name = "Table Name")]
        [RestrictSpecialChar]

        public string LcscTableName { get; set; }

        [MaxLength(500, ErrorMessage = " Column Name can be maximum 500 characters")]
        [Required(ErrorMessage = "Column Name is required")]
        [Display(Name = "Column Name")]
        [RestrictSpecialChar]

        public string LcscColumnName { get; set; }

        [MaxLength(100, ErrorMessage = "Label can be maximum 100 characters")]
        [Display(Name = "Label Name")]
        [RestrictSpecialChar]

        public string LcscLabel { get; set; }

        [Display(Name = "DisplayOn Form")]
        public bool LcscDisplayOnForm { get; set; }

        [Display(Name = "Ordinal Position")]
        public int LcscOrdinalPosition { get; set; }

        public bool LcscIsMandatory { get; set; }
        public Nullable<int> LcscDropDownId { get; set; }

        [RestrictSpecialChar]
        public string LcscDataType { get; set; }

        [RestrictSpecialChar]
        public string LdName { get; set; }
        //objects defined to get array of data in post Method
        [RestrictSpecialChar]
        public string ColumnLabel { get; set; }


        public string LcscTooltip { get; set; }
        

        [RestrictSpecialChar]
        public string ColumnName { get; set; }
        public bool CanBeDisplayed { get; set; }

        //object defined to get additional info
        [RestrictSpecialChar]
        public string IsNullable { get; set; }

        [RestrictSpecialChar]
        public string DataType { get; set; }

        [RestrictSpecialChar]        
        [MaxLength(4000, ErrorMessage = "Banner Text can be maximum 4000 characters")]
        public string BannerText { get; set; }

        public Nullable<bool> LcscIsReportParameter { get; set; }
        public Nullable<int> LcscReportParameterOrdinal { get; set; }

        public LCompanySpecificColumnViewModel()
        {
            LcscIsMandatory = false; //setting default value when view model is initialized
        }
    }
}