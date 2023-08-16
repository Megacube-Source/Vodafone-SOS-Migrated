using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LRawDataRowCountsViewModel

    {
        [RestrictSpecialChar]
        public string Status { get; set; }


        public int RowCounts { get; set; }
    }

    public partial class LRawDataViewModel
    {
        //This view model is being referenced by only one method which is updating the status column of the table and hene only status column can be used here.
        public int Id { get; set; }

        [Required(ErrorMessage = "SOS Batch No is required")]
        [Display(Name = "SOS Batch No")]
        public int LrdSOSBatchNumber { get; set; }

        [MaxLength(50, ErrorMessage = "Status can be maximum 50 characters")]
        [Display(Name = "Status")]
        [RestrictSpecialChar]
        public string LrdStatus { get; set; }

        [MaxLength(255, ErrorMessage = "Exclusion Comments can be maximum 255 characters")]
        [Display(Name = "Exclusion Comments")]

        [RestrictSpecialChar]
        public string LrdExclusionComments { get; set; }
    }
}