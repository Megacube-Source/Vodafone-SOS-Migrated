using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LPayRowCountsViewModel
    {
        public int RowCounts { get; set; }
    }

    public partial class LPayViewModel
    {
        public int Id { get; set; }

        [MaxLength(2, ErrorMessage = "The OpCoCode can be maximum 2 characters")]
        [Required(ErrorMessage = "OpCoCode is required")]
        [Display(Name = "OpCoCode")]
        [RestrictSpecialChar]
        public string LpOpCoCode { get; set; }

        [Display(Name = " SOS Batch Number")]
        public Nullable<int> LpSOSBatchNumber { get; set; }

        [Display(Name = " AlteryxTransaction Number")]
         public Nullable<long> LpAlteryxTransactionNumber { get; set; }

        [MaxLength(255, ErrorMessage = "The Primary Channel can be maximum 255 characters")]
        [Required(ErrorMessage = "Primary Channel is required")]
        [Display(Name = "Primary Channel")]
        [RestrictSpecialChar]
        public string LpPrimaryChannel { get; set; }

        [MaxLength(255, ErrorMessage = "The Payee can be maximum 255 characters")]
        [Required(ErrorMessage = "Payee  is required")]
        [Display(Name = "Payee ")]
        [RestrictSpecialChar]
        public string LpPayee { get; set; }

        [MaxLength(255, ErrorMessage = "The Parent Payee can be maximum 255 characters")]
        [Display(Name = "Parent Payee ")]
        [RestrictSpecialChar]
        public string LpParentPayee { get; set; }

        [Display(Name = "Order Date ")]
         public Nullable<System.DateTime> LpOrderDate { get; set; }

        [Display(Name = "Connection Date ")]
        public Nullable<System.DateTime> LpConnectionDate { get; set; }

        [Display(Name = "Termination Date ")]
        public Nullable<System.DateTime> LpTerminationDate { get; set; }

        [MaxLength(255, ErrorMessage = "The Subscriber Number can be maximum 255 ")]
        [Display(Name = "Subscriber Number ")]
        [RestrictSpecialChar]
        public string LpSubscriberNumber { get; set; }

        [MaxLength(255, ErrorMessage = "The Ban can be maximum 255 ")]
        [Display(Name = "Ban")]
        [RestrictSpecialChar]
        public string LpBAN { get; set; }

        [MaxLength(255, ErrorMessage = "The Activity Type can be maximum 255 ")]
        [Display(Name = "Activity Type")]
        [RestrictSpecialChar]
        public string LpActivityType { get; set; }

        [MaxLength(255, ErrorMessage = "The Plan Description can be maximum 255 ")]
        [Display(Name = "Plan Description")]
        [RestrictSpecialChar]
        public string LpPlanDescrition { get; set; }

        [MaxLength(255, ErrorMessage = "The Product Code can be maximum 255 ")]
        [Display(Name = "Product Code")]
        [RestrictSpecialChar]
        public string LpProductCode { get; set; }

        [MaxLength(255, ErrorMessage = "The Upgrade Code can be maximum 255 ")]
        [Display(Name = "Upgrade Code")]
        [RestrictSpecialChar]
        public string LpUpgradeCode { get; set; }

        [MaxLength(255, ErrorMessage = "The IMEI can be maximum 255 ")]
        [Display(Name = "IMEI")]
        [RestrictSpecialChar]
        public string LpIMEI { get; set; }

        [MaxLength(255, ErrorMessage = "The Device Code can be maximum 255 ")]
        [Display(Name = "Device Code")]
        [RestrictSpecialChar]
        public string LpDevieCode { get; set; }

        [MaxLength(255, ErrorMessage = "The Device Type can be maximum 255 ")]
        [Display(Name = "Device Type")]
        [RestrictSpecialChar]
        public string LpDeviceType { get; set; }

        [MaxLength(255, ErrorMessage = "The Comm Type can be maximum 255 ")]
        [Display(Name = "Comm Type")]
        [RestrictSpecialChar]
        public string LpCommType { get; set; }

        [Display(Name = "Contact Duration")]
        public Nullable<int> LpContractDuration { get; set; }

        [MaxLength(255, ErrorMessage = "The Contract Id can be maximum 255 ")]
        [Display(Name = "Contract Id")]
        [RestrictSpecialChar]
        public string LpContractId { get; set; }

        [Display(Name = "CommAmtEx Tax")]
         public Nullable<decimal> LpCommAmtExTax { get; set; }

        [Display(Name = "Tax")]
         public Nullable<decimal> LpTax { get; set; }

        [Display(Name = "CommAmtInc Tax")]
        public Nullable<decimal> LpCommAmtIncTax { get; set; }

        [MaxLength(255, ErrorMessage = "The Comments can be maximum 255 characters")]
        [Display(Name = "Comments ")]
        [RestrictSpecialChar]
        public string LpComments { get; set; }

        [MaxLength(255, ErrorMessage = "The A01 can be maximum 255 characters")]
        [Display(Name = "A01 ")]
        public byte[] A01 { get; set; }

        [MaxLength(255, ErrorMessage = "The A02 can be maximum 255 characters")]
        [Display(Name = "A02 ")]

        [RestrictSpecialChar]
        public string A02 { get; set; }

        [MaxLength(255, ErrorMessage = "The A03 can be maximum 255 characters")]
        [Display(Name = "A03 ")]
        [RestrictSpecialChar]
        public string A03 { get; set; }

        [MaxLength(255, ErrorMessage = "The A04 can be maximum 255 characters")]
        [Display(Name = "A04 ")]
        [RestrictSpecialChar]
        public string A04 { get; set; }

        [MaxLength(255, ErrorMessage = "The A05 can be maximum 255 characters")]
        [Display(Name = "A05 ")]
        [RestrictSpecialChar]
        public string A05 { get; set; }

        [Display(Name = "AN01")]

        public Nullable<decimal> AN01 { get; set; }

        [Display(Name = "AN02")]
        public Nullable<decimal> AN02 { get; set; }

        [Display(Name = "AN03")]
        public Nullable<decimal> AN03 { get; set; }

        [Display(Name = "AN04")]
        public Nullable<decimal> AN04 { get; set; }

        [Display(Name = "AN05")]
        public Nullable<decimal> AN05 { get; set; }

        [Display(Name = "AD01")]
        public Nullable<System.DateTime> AD01 { get; set; }

        [Display(Name = "AD02")]
         public Nullable<System.DateTime> AD02 { get; set; }

        [Display(Name = "AD03")]

        public Nullable<System.DateTime> AD03 { get; set; }

        [Display(Name = "AD04")]

        public Nullable<System.DateTime> AD04 { get; set; }

        [Display(Name = "AD05")]

        public Nullable<System.DateTime> AD05 { get; set; }
    }
}
