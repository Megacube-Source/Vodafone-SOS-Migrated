using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LPayeeViewModel
    {
        [RestrictSpecialChar]
        public string GkvValue { get; set; }
        public bool LpBlockNotification { get; set; }

        public int Id { get; set; }

        // [Display(Name = "Parent Payee")]
        //public Nullable<int> LpParentPayeeId { get; set; }
        public bool IsParent { get; set; }
        [Display(Name = "Batch")]
         public Nullable<int> LpBatchNumber { get; set; }

          [MaxLength(200)]
         // [RegularExpression(Globals.RegExAlphanumericWithSpace, ErrorMessage = "Only alphanumerics and white spaces are allowed in Trading Name")]
        [Display(Name = "Trading Name")]
        [RestrictSpecialChar]
         public string LpTradingName { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Primary Channel is required")] //commenting as Primary channel will be populated as per portfolios
        [Display(Name = "Primary Channel")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LpPrimaryChannel { get; set; }

        //[Required]
        [Display(Name = "Channel/Region")]
        [Range(0, int.MaxValue,ErrorMessage = "Channel/Region is required")]
        public Nullable<int> LpChannelId { get; set; }

        //[Required(ErrorMessage = "Sub Channel is required")]
        [Display(Name = "Sub Channel/Sub Region")]
        public Nullable<int> LpSubChannelId { get; set; }

       

         [MaxLength(20, ErrorMessage = "Payee Code can be maximum 20 characters long")]
        //  [RegularExpression(Globals.RegExAlphanumericWithSpace, ErrorMessage = "Only alphanumerics and white spaces are allowed in Payee Code")]
        // [Required(ErrorMessage = "Payee Code is required")]
        [Required(ErrorMessage = "Payee Code is required")]
        [Display(Name = "Payee Code")]
        [RestrictSpecialChar]
        public string LpPayeeCode { get; set; }

       

        //[Display(Name = "SalesForce ID")]
        //public Nullable<long> LpSFID { get; set; }

        [Display(Name = "Phone")]
        //[Required]Removed Required attribute as directed by JS
        [MaxLength(13)]
        [RestrictSpecialChar]
        public string LpPhone { get; set; }

        [EmailAddress]
        [MaxLength(100)]
       // [Required(ErrorMessage = "Email Address is required")]
        [Display(Name = "Email Address")]
        //[Required(ErrorMessage = "Email Address is required")]
        [RestrictSpecialChar]
        public string LpEmail { get; set; }

        [Display(Name = "Address")]
        [MaxLength(1000)]
        // [Required(ErrorMessage="Address is required for top level Parent Payee")]
        [RestrictSpecialChar]
        public string LpAddress { get; set; }


        //[MaxLength(4000)]
        //[Display(Name = "Existing Comments")]
        //[RestrictSpecialChar]
        //public string LpComments { get; set; }//column replaced with wfcomments

        [Display(Name = "Comments")]
        [MaxLength(4000)]
        [RestrictSpecialChar]
        public string Comments { get; set; }

       // [Required(ErrorMessage = "Start Date is required")]
        [Display(Name = "Start Date")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime LpEffectiveStartDate { get; set; }

        [Display(Name = "End Date")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> LpEffectiveEndDate { get; set; }


        [RestrictSpecialChar]
        public string LpFileNames { get; set; }

        [Display(Name = "Attachments")]
        [RestrictSpecialChar]
        public string LpUserFriendlyFileNames { get; set; }

       // [Required(ErrorMessage = "Business Unit is required")]
        [Display(Name = "Business Unit")]
        [MaxLength(50)]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LpBusinessUnit { get; set; }


        [Display(Name = "Channel")]
        [RestrictSpecialChar]
        public string RcName { get; set; }

        [Display(Name = "Sub Channel")]
        [RestrictSpecialChar]
        public string RscName { get; set; }
        //[Required]
        //[EmailAddress]
        //[Display(Name = "Email")]
        //public string LoginEmail { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        //public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }

        [RestrictSpecialChar]
        public string LpUserId { get; set; }
        public int LpCompanyId { get; set; }



        [RestrictSpecialChar]
        public string LpCreatedById { get; set; }

        [RestrictSpecialChar]
        public string LpUpdatedById { get; set; }


        [MaxLength(100, ErrorMessage = "First Name can be maximum 100 characters long")]
        //[RegularExpression(Globals.RegExAlphanumericWithSpace, ErrorMessage = "Only alphanumerics and white spaces are allowed in First Name")]
       // [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        [RestrictSpecialChar]
        public string LpFirstName { get; set; }

        [MaxLength(100, ErrorMessage = "Last Name can be maximum 100 characters long")]
       // [RegularExpression(Globals.RegExAlphanumericWithSpace, ErrorMessage = "Only alphanumerics and white spaces are allowed in Last Name")]
       // [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        [RestrictSpecialChar]
        public string LpLastName { get; set; }

      
         [Display(Name = "TIN")]
        [RestrictSpecialChar]
        public string LpTIN { get; set; }
      
       

        [Display(Name = "Can Raise Claims")]
        public bool LpCanRaiseClaims { get; set; }

        [Display(Name = "Channel Manager")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LpChannelManager { get; set; }
        public Nullable<System.DateTime> LpCreatedDateTime { get; set; }
        public Nullable<System.DateTime> LpUpdatedDateTime { get; set; }

        [Display(Name = "Distribution Channel")]
        [RestrictSpecialChar]
        public string LpDistributionChannel { get; set; }

         [Display(Name = "Position")]
        [RestrictSpecialChar]
        public string LpPosition { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterId { get; set; }

        [RestrictSpecialChar]
        public string WFAnalystId { get; set; }

        [RestrictSpecialChar]
        public string WFManagerId { get; set; }
        public Nullable<int> WFOrdinal { get; set; }

        [RestrictSpecialChar]
        public string WFCurrentOwnerId { get; set; }
        //
        [RestrictSpecialChar]
        public string WFStatus { get; set; }

        [RestrictSpecialChar]
        public string WFType { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterRoleId { get; set; }
        public int WFCompanyId { get; set; }

        [Display(Name = "Existing Comments")]
        [RestrictSpecialChar]
        public string WFComments { get; set; }

        [RestrictSpecialChar]
        public string CreatedBy { get; set; }

        [RestrictSpecialChar]
        public string UpdatedBy { get; set; }

        [RestrictSpecialChar]
        public string ParentName { get; set; }

        [RestrictSpecialChar]
        public string ParentCode { get; set; }

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

        //to get change request effective date
         public Nullable<DateTime> LcrEffectiveStartDate { get; set; }
         public DateTime PayeeAsOfDate { get; set; }
        //object to get parent payee details
        [Display(Name="Parent Payee")]
         public Nullable<int> LppParentPayeeId { get; set; }
         public System.DateTime LppEffectiveStartDate { get; set; }
         public Nullable<System.DateTime> LppEffectiveEndDate { get; set; }

        [Display(Name ="Create Login")]
        public bool LpCreateLogin { get; set; }

        //This object is defined to pass information to view whether form is create /edit/review
        [RestrictSpecialChar]
        public string FormType { get; set; }

        [RestrictSpecialChar]
        public string FullName { get; set; }


        //to send start date and end date in formatted string extra objects are defined
        [RestrictSpecialChar]
        public string StartDate { get; set; }

        [RestrictSpecialChar]
        public string EndDate { get; set; }

        [RestrictSpecialChar]
        public string ErrorList { get; set; }

        [RestrictSpecialChar]
        public string LbfFileName { get; set; }

        [RestrictSpecialChar]
        public string ErrorMessage { get; set; }//to get error message

        [RestrictSpecialChar]
        public string FilePath { get; set; }

        [RestrictSpecialChar]
        public string LpFinOpsRoles { get; set; }

        [RestrictSpecialChar]
        public string FinOpsRoleString { get; set; }

        //RK added following to use on upload page
        [Display(Name = "Upload")]
        public bool isUploadFile { get; set; }

        //SS Adding Portfolio to model
        [RestrictSpecialChar]
        public string PortfolioList { get; set; }

        [RestrictSpecialChar]
        public string ParameterCarrier { get; set; }//to store extra parameter supplied to api

        [RestrictSpecialChar]
        public string PortfolioId { get; set; }

        public string SubmitClicked { get; set; }
    }

    public  class LPayeeAddViewModel
    {
       
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string LpFirstName { get; set; }
        [RestrictSpecialChar]
        public string LpLastName { get; set; }
        [RestrictSpecialChar]
        public string LpPayeeCode { get; set; }


    }


    public partial class XPayeeUploadViewModel
    {
        public int Id { get; set; }
        public string XEmail { get; set; }
        public string XPrimaryChannel { get; set; }
        public string XPayeeCode { get; set; }
        public string XCanRaiseClaims { get; set; }
        public string XCreateLogin { get; set; }
        public string XBusinessUnit { get; set; }
        public string XStatus { get; set; }
        public string XValidationMessage { get; set; }
    }

    public partial class PayeeUploadViewModelForReviewGrid
    {
        public string ColumnName { get; set; }
        public string ColumnLabel { get; set; }
    }
         
}