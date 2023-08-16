using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LCalcRowCountsViewModel
    {
        public int RowCounts { get; set; }
    }
    public partial class LCalcViewModel
    {
        public int Id { get; set; }
        [MaxLength(2, ErrorMessage = "The OpCoCode can be maximum 2 characters")]
        [Required(ErrorMessage = "OpCoCode is required")]
        [Display(Name = "OpCoCode ")]
        [RestrictSpecialChar]
        public string LcOpCoCode { get; set; }

        [MaxLength(255, ErrorMessage = " Source can be maximum 255 characters")]
        [Display(Name = "Source")]
        [RestrictSpecialChar]
        public string LcSource { get; set; }

        [MaxLength(255, ErrorMessage = "The Adjustmen Code can be maximum 255 characters")]
        [Display(Name = "Adjustmen Code")]
        [RestrictSpecialChar]
        public string LcAdjustmenCode { get; set; }

        [Display(Name = "SOS Batch Number")]
        public Nullable<int> LcSOSBatchNumber { get; set; }

        [Display(Name = "Alteryx Transaction Number")]
        public Nullable<long> LcAlteryxTransactionNumber { get; set; }

        [MaxLength(255, ErrorMessage = " Primary Channel can be maximum 255 characters")]
        [Required(ErrorMessage = "Primary Channel is required")]
        [Display(Name = "Primary Channel ")]
        [RestrictSpecialChar]
        public string LcPrimaryChannel { get; set; }

        [MaxLength(255, ErrorMessage = " Payee can be maximum 255 characters")]
        [Required(ErrorMessage = "Payee is required")]
        [Display(Name = "Payee ")]
        [RestrictSpecialChar]
        public string LcPayee { get; set; }

        [MaxLength(255, ErrorMessage = " Parent Payee can be maximum 255 characters")]
        [Display(Name = "Parent Payee")]
        [RestrictSpecialChar]
        public string LcParentPayee { get; set; }

        [Display(Name = "Upload Order Date ")]
        public Nullable<System.DateTime> LcOrderDate { get; set; }

        [Display(Name = "Upload Connection Date")]
        public Nullable<System.DateTime> LcConnectionDate { get; set; }

        [Display(Name = "Upload Termination Date")]
        public Nullable<System.DateTime> LcTerminationDate { get; set; }

        [MaxLength(255, ErrorMessage = " Subscriber Number can be maximum 255 characters")]
        [Display(Name = "Subscriber Number ")]
        [RestrictSpecialChar]
        public string LcSubscriberNumber { get; set; }

        [MaxLength(255, ErrorMessage = " Ban can be maximum 255 characters")]
        [Display(Name = "Ban ")]
        [RestrictSpecialChar]
        public string LcBAN { get; set; }

        [MaxLength(255, ErrorMessage = " Activity Type can be maximum 255 characters")]
        [Display(Name = "Activity Type ")]
        [RestrictSpecialChar]
        public string LcActivityType { get; set; }

        [MaxLength(255, ErrorMessage = "The Plan Description can be maximum 255 characters")]
        [Display(Name = "Plan Description")]
        [RestrictSpecialChar]
        public string LcPlanDescrition { get; set; }

        [MaxLength(255, ErrorMessage = " Product Code can be maximum 255 characters")]
        [Display(Name = "Product Code ")]
        [RestrictSpecialChar]
        public string LcProductCode { get; set; }

        [MaxLength(255, ErrorMessage = " Upgrate code can be maximum 255 characters")]
        [Display(Name = "Upgrate Code ")]
        [RestrictSpecialChar]
        public string LcUpgradeCode { get; set; }

        [MaxLength(255, ErrorMessage = " IMEI can be maximum 255 characters")]
        [Display(Name = "IMEI ")]
        [RestrictSpecialChar]
        public string LcIMEI { get; set; }

        [MaxLength(255, ErrorMessage = "Device code can be maximum 255 characters")]
        [Display(Name = "Device Code ")]
        [RestrictSpecialChar]
        public string LcDevieCode { get; set; }

        [MaxLength(255, ErrorMessage = " Device Type can be maximum 255 characters")]
        [Display(Name = "Device Type ")]
        [RestrictSpecialChar]
        public string LcDeviceType { get; set; }

        [MaxLength(255, ErrorMessage = " CommType can be maximum 255 characters")]
        [Display(Name = "Comm Type")]
        [RestrictSpecialChar]
        public string LcCommType { get; set; }

        [Display(Name = "Contract Duration")]
        public Nullable<int> LcContractDuration { get; set; }

        [MaxLength(255, ErrorMessage = "The Contract Id can be maximum 255 characters")]
        [Display(Name = "Contract Id")]
        [RestrictSpecialChar]
        public string LcContractId { get; set; }

        [Display(Name = "CommAmtEx Tax")]
        public Nullable<decimal> LcCommAmtExTax { get; set; }

        [Display(Name = "Tax")]
        public Nullable<decimal> LcTax { get; set; }

        [Display(Name = "CommAmtInc Tax")]
        public Nullable<decimal> LcCommAmtIncTax { get; set; }

        [Display(Name = "Comments")]
        [RestrictSpecialChar]
        public string LcComments { get; set; }

        [MaxLength(255, ErrorMessage = "The A01 can be maximum 255 characters")]
        [Display(Name = "A01")]
        public byte[] A01 { get; set; }
        [MaxLength(255, ErrorMessage = "The A02 can be maximum 255 characters")]
        [Display(Name = "A02")]
        [RestrictSpecialChar]
        public string A02 { get; set; }

        [MaxLength(255, ErrorMessage = "The A03 can be maximum 255 characters")]
        [Display(Name = "A03")]
        [RestrictSpecialChar]
        public string A03 { get; set; }

        [MaxLength(255, ErrorMessage = "The A04 can be maximum 255 characters")]
        [Display(Name = "A04")]
        [RestrictSpecialChar]
        public string A04 { get; set; }

        [RestrictSpecialChar]
        [MaxLength(255, ErrorMessage = "The A05 can be maximum 255 characters")]
        [Display(Name = "A05")]
       
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

        [Required(ErrorMessage = "Commision Period Is Required")]
        [Display(Name = "Commission Period")]
        [RestrictSpecialChar]
        public string CommissionPeriod { get; set; }//object defined to pass commission period in Manual Adjustments

        //New Columns has been added to LCalc
        [Display(Name = "Commission Period")]
        public bool LcIsPayeeAccepted { get; set; }

        [Display(Name = "Payee AttachmentId")]
        public int LcPayeeAttachmentId { get; set; }

        [Display(Name = "Accepted DateTime")]
        public Nullable<System.DateTime> LcAcceptedDateTime { get; set; }

        [Display(Name = "AcceptedById")]
        [RestrictSpecialChar]
        public string LcAcceptedById { get; set; }

        //Added by SG for getting extra fields in Summary Tab for Payee Calculations
        [RestrictSpecialChar]
        public string LbCommissionPeriod { get; set; }

        [RestrictSpecialChar]
        public string WFStatus { get; set; }


    }
}
