using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class UploadPayeeViewModel
    {
        public int Id { get; set; }
        public int LpCompanyId { get; set; }
        public int LpChannelId { get; set; }
        public Nullable<int> LpSubChannelId { get; set; }
        public string LpStatus { get; set; }
        public string LpCreatedById { get; set; }
        public string LpUpdatedById { get; set; }
        public string LpFirstName { get; set; }
        public string LpLastName { get; set; }
        public string LpTradingName { get; set; }
        public string LpPrimaryChannel { get; set; }
        public string LpPayeeCode { get; set; }
        public string LpPhone { get; set; }
        public string LpEmail { get; set; }
        public string LpAddress { get; set; }
       public string LpComments { get; set; }
       public System.DateTime LpEffectiveStartDate { get; set; }
        public Nullable<System.DateTime> LpEffectiveEndDate { get; set; }
        public string LpBusinessUnit { get; set; }
        //public Nullable<bool> LpCanRaiseClaims { get; set; }
        //RK Added
        public bool LpCanRaiseClaims { get; set; }
       public string LpChannelManager { get; set; }
        public string LpTIN { get; set; }
        public string LpDistributionChannel { get; set; }
        public string LpPosition { get; set; }
        public string WFRequesterId { get; set; }
        public string WFCurrentOwnerId { get; set; }
        ////to get objects required to create payee and batch from webApp extra objects are defined
        public string ParentCode { get; set; }
        public string ErrorMessage { get; set; }
        public int WFCompanyId { get; set; }
        public string WFComments { get; set; }
        public string A01 { get; set; }
        public string A02 { get; set; }
        public string A03 { get; set; }
        public string A04 { get; set; }
        public string A05 { get; set; }
        public string A06 { get; set; }
        public string A07 { get; set; }
        public string A08 { get; set; }
        public string A09 { get; set; }
        public string A10 { get; set; }
        public bool LpCreateLogin { get; set; }

        public string LpFinOpsRoles { get; set; }
        //Commented below columns on 22 Apr 2017 to avoid Error while uploading Payee While excel Upload
        //to return error message
        ////public bool IsParent { get; set; }
        //public Nullable<int> LpBatchId { get; set; }
        //public Nullable<int> LppParentPayeeId { get; set; }
        //public string CreatedBy { get; set; }
        //public string UpdatedBy { get; set; }
        //public string FullName { get; set; }
        //public string LbfFileName { get; set; }
        ////Object defined to get date selected by user to view tree
        //public Nullable<DateTime> PayeeAsOfDate { get; set; }
        //public string WFStatus { get; set; }
        //public string WFType { get; set; }
        //public string WFAnalystId { get; set; }
        //public string WFManagerId { get; set; }
        //public Nullable<int> WFOrdinal { get; set; }
        //public Nullable<bool> LpCreateLogin { get; set; }
        //public Nullable<System.DateTime> LpCreatedDateTime { get; set; }
        //public Nullable<System.DateTime> LpUpdatedDateTime { get; set; }
        //public Nullable<bool> LpAuthorisedPayeeVerification { get; set; }

    }
}