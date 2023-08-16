using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;



namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LPortfolioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int LpCompanyId { get; set; }

        //[MaxLength(20, ErrorMessage = "The Primary Channel be maximum 20 characters")]
        //[Required(ErrorMessage = "Primary Channel is required")]
        //[Display(Name = "Primary Channel")]
        //[RestrictSpecialChar]
        //public string LpPrimaryChannel { get; set; }

        [MaxLength(20, ErrorMessage = "The Business Unit be maximum 20 characters")]
        [Required(ErrorMessage = "Business Unit is required")]
        [Display(Name = "Business Unit")]
        [RestrictSpecialChar]
        public string LpBusinessUnit { get; set; }

        [Required(ErrorMessage = " Channel Id is required")]
        [Display(Name = " Channel Id")]
        public int LpChannelId { get; set; }

        [MaxLength(20, ErrorMessage = "The Status be maximum 20 characters")]
        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        [RestrictSpecialChar]
        public string LpStatus { get; set; }

        [RestrictSpecialChar]
        public string RcName { get; set; }//object defined to get channel name
        [RestrictSpecialChar]
        public string RcPrimaryChannel { get; set; }
        public bool Select { get; set; }
        //For Payee Report
        [RestrictSpecialChar]
        public string LpPayeeCode { get; set; }
    }
}