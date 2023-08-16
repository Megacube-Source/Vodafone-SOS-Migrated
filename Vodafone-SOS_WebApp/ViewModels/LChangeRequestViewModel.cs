using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LChangeRequestViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Entity Name is required")]
        [Display(Name = "Entity Name")]
        [RestrictSpecialChar]
        public string LcrEntityName { get; set; }

        [Display(Name = "Row Id")]
        public Nullable<int> LcrRowId { get; set; }

        [Required(ErrorMessage = "Column Name is required")]
        [Display(Name = "Column Name")]
        [RestrictSpecialChar]
        public string LcrColumnName { get; set; }

        [Required(ErrorMessage = "Column Label is required")]
        [Display(Name = "Column Label")]
        [RestrictSpecialChar]
        public string LcrColumnLabel { get; set; }

         [Display(Name = "Old Value")]
        [RestrictSpecialChar]
        public string LcrOldValue { get; set; }

         [Display(Name = "New Value")]
        [RestrictSpecialChar]
        public string LcrNewValue { get; set; }

         [Required(ErrorMessage = "Requested Date Time is required")]
         [Display(Name = "Requested Date Time")]
        public System.DateTime LcrCreatedDateTime { get; set; }

        
         [Display(Name = "Review Date Time")]
        public Nullable<System.DateTime> LcrUpdatedDateTime { get; set; }

         [Required(ErrorMessage = "Rejected Date Time is required")]
         [Display(Name = "Rejected Date Time")]
        public Nullable<System.DateTime> LcrApprovalRejectionDateTime { get; set; }

        [RestrictSpecialChar]
        public string LcrCreatedById { get; set; }

        [RestrictSpecialChar]
        public string LcrNextActionByRoleId { get; set; }

        [RestrictSpecialChar]
        public string LcrUpdatedById { get; set; }

        [RestrictSpecialChar]
        public string LcrApprovedRejectedById { get; set; }

        [RestrictSpecialChar]
        public string LcrAction { get; set; }

        // [Display(Name = "Comments")]
        //[RestrictSpecialChar]
        //public string LcrComments { get; set; }

         public Nullable<DateTime> LcrEffectiveStartDate { get; set; }
        [RestrictSpecialChar]
        public string LcrOldId { get; set; }
        [RestrictSpecialChar]
        public string LcrNewId { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterId { get; set; }

        [RestrictSpecialChar]
        public string WFStatus { get; set; }
        [RestrictSpecialChar]
        public string WFAnalystId { get; set; }

        [RestrictSpecialChar]
        public string WFManagerId { get; set; }

        [RestrictSpecialChar]
        public string WFCurrentOwnerId { get; set; }

        [RestrictSpecialChar]
        public string WFType { get; set; }
        public int WFCompanyId { get; set; }

        [Display(Name = "Existing Comments")]
        [RestrictSpecialChar]
        public string WFComments { get; set; }



        //[Required(ErrorMessage = "Status is required")]
        // [Display(Name = "Status")]
        // public int LcrStatusId { get; set; }
         public int LcrCompanyId { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterRoleId { get; set; }

        [RestrictSpecialChar]
        public string RsStatus { get; set; }

        //if entity is payee its objects will be required

        [RestrictSpecialChar]
        public string LpPayeeCode { get;set; }

        [RestrictSpecialChar]
        public string FullName { get; set; }
        //public string LpLastName { get; set; }

        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        [RestrictSpecialChar]
        public string CreatedBy { get; set; }

        [RestrictSpecialChar]
        public string UpdatedBy { get; set; }

        [RestrictSpecialChar]
        [EmailAddress]
        public string EmailID { get; set; }
        public bool LcrCreatedByForm { get; set; }

        //added by RS need to access for payeecr and usercr review page
        public List<LChangeRequestViewModel> LChangeRequest { get; set; }
    }
}