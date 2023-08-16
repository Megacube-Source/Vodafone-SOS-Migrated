using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class DownloadPayeeGridByTabNameViewModel
    {
        [RestrictSpecialChar]
        [Display(Name ="Authorised Payee Verification")]
       public string AuthorisedPayeeVerification { get; set; }

        [Display(Name ="Can Raise Claims")]
        [RestrictSpecialChar]
        public string CanRaiseClaims { get; set; }

        [Display(Name ="Channel")]
        [RestrictSpecialChar]
        public string RcName { get; set; }

        [Display(Name ="Email")]
        [RestrictSpecialChar]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name ="First Name")]
        [RestrictSpecialChar]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [RestrictSpecialChar]
        public string LastName { get; set; }

        [Display(Name = "Trading Name")]
        [RestrictSpecialChar]
        public string TradingName { get; set; }

        [Display(Name ="Payee Code")]
        [RestrictSpecialChar]
        public string PayeeCode { get; set; }

        [Display(Name ="Primary Channel")]
        [RestrictSpecialChar]
        public string PrimaryChannel { get; set; }

        [Display(Name ="Status")]
        [RestrictSpecialChar]
        public string Status { get; set; }

        [Display(Name ="Address")]
        [RestrictSpecialChar]
        public string Address { get; set; }

        [Display(Name ="Business Unit")]
        [RestrictSpecialChar]
        public string BusinessUnit { get; set; }

        [Display(Name ="Channel Manager")]
        [RestrictSpecialChar]
        public string ChannelManager { get; set; }

        [Display(Name ="Distribution Channel")]
        [RestrictSpecialChar]
        public string DistributionChannel { get; set; }

        [Display(Name ="Phone")]
        [RestrictSpecialChar]
        public string Phone { get; set; }

        [Display(Name ="Position")]
        [RestrictSpecialChar]
        public string Position { get; set; }

        [Display(Name ="Sub Channel")]
        [RestrictSpecialChar]
        public string SubChannel { get; set; }

        [Display(Name ="Tin")]
        [RestrictSpecialChar]
        public string Tin { get; set; }

        [Display(Name ="Parent Name")]
        [RestrictSpecialChar]
        public string ParentName { get; set; }

        [Display(Name ="Effective Start Date")]
        [RestrictSpecialChar]
        public string EffectiveStartDate { get; set; }

        [Display(Name ="Effective End Date")]
        [RestrictSpecialChar]
        public string EffectiveEndDate { get; set; }

        [Display(Name = "Created By")]
        [RestrictSpecialChar]
        public string CreatedBy { get; set; }

        [Display(Name ="Updated By")]
        [RestrictSpecialChar]
        public string UpdatedBy { get; set; }

        [MaxLength(255)]
        [RestrictSpecialChar]
        public string A01 { get; set; }

        [MaxLength(255)]
        [RestrictSpecialChar]
        public string A02 { get; set; }

        [MaxLength(255)]
        [RestrictSpecialChar]
        public string A03 { get; set; }

        [MaxLength(255)]
        [RestrictSpecialChar]
        public string A04 { get; set; }

        [MaxLength(255)]
        [RestrictSpecialChar]
        public string A05 { get; set; }

        [MaxLength(255)]
        [RestrictSpecialChar]
        public string A06 { get; set; }

        [MaxLength(255)]
        [RestrictSpecialChar]
        public string A07 { get; set; }

        [MaxLength(255)]
        [RestrictSpecialChar]
        public string A08 { get; set; }

        [MaxLength(255)]
        [RestrictSpecialChar]
        public string A09 { get; set; }

        [MaxLength(255)]
        [RestrictSpecialChar]
        public string A10 { get; set; }
    }
}