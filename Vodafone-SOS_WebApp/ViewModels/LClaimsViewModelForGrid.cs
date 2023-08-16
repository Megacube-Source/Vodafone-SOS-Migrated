using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LClaimsViewModelForGrid
    {
        public int Id { get; set; }
        public int LcCompanyId { get; set; }
       
        public long LcClaimId { get; set; }

        [RestrictSpecialChar]
        public string LcPayeeCode { get; set; }
        public Nullable<System.DateTime> LcConnectionDate { get; set; }
        public Nullable<System.DateTime> LcOrderDate { get; set; }
       
        public Nullable<decimal> LcExpectedCommissionAmount { get; set; }
       
        public Nullable<System.DateTime> LcAllocationDate { get; set; }

        [RestrictSpecialChar]
        public string LcCommentsInternal { get; set; }
        public bool LcIsReclaim { get; set; }
        public Nullable<System.DateTime> LcSentForApprovalDate { get; set; }
        public Nullable<System.DateTime> LcApprovalDate { get; set; }
        public Nullable<System.DateTime> LcPaymentDate { get; set; }
        public Nullable<System.DateTime> LcWithdrawnDate { get; set; }
        public Nullable<System.DateTime> LcRejectionDate { get; set; }
        public Nullable<decimal> LcPaymentAmount { get; set; }

        [RestrictSpecialChar]
        public string ApprovedBy { get; set; }

        [RestrictSpecialChar]
        public string AllocatedTo { get; set; }

        [RestrictSpecialChar]
        public string AllocatedBy { get; set; }

        [RestrictSpecialChar]
        public string SentForApprovalBy { get; set; }
    
    }
}