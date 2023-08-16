using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial  class LPayeeDecryptedViewModel
    {
            public int Id { get; set; }
            
            public bool LpBlockNotification { get; set; }
            public Nullable<int> LpBatchNumber { get; set; }

            public string LpTradingName { get; set; }

            public string LpPrimaryChannel { get; set; }

            public Nullable<int> LpChannelId { get; set; }

            public Nullable<int> LpSubChannelId { get; set; }

            public string LpPayeeCode { get; set; }

            public string LpPhone { get; set; }

            public string LpEmail { get; set; }

            public string LpAddress { get; set; }
        
            public string Comments { get; set; }
        
            public System.DateTime LpEffectiveStartDate { get; set; }
        
            public Nullable<System.DateTime> LpEffectiveEndDate { get; set; }
        
            public string LpFileNames { get; set; }
        
            public string LpUserFriendlyFileNames { get; set; }

            public string LpBusinessUnit { get; set; }
        
            public string RcName { get; set; }
        
            public string RscName { get; set; }
         
            public string LpUserId { get; set; }
            public int LpCompanyId { get; set; }

            public string LpCreatedById { get; set; }

            public string LpUpdatedById { get; set; }
        
            public string LpFirstName { get; set; }
        
            public string LpLastName { get; set; }

        
            public string LpTIN { get; set; }
        
            public bool LpCanRaiseClaims { get; set; }
        
            public string LpChannelManager { get; set; }
            public Nullable<System.DateTime> LpCreatedDateTime { get; set; }
            public Nullable<System.DateTime> LpUpdatedDateTime { get; set; }

            public string LpDistributionChannel { get; set; }

            public string LpPosition { get; set; }

            public string WFRequesterId { get; set; }

            public string WFAnalystId { get; set; }

            public string WFManagerId { get; set; }
            public Nullable<int> WFOrdinal { get; set; }

            public string WFCurrentOwnerId { get; set; }
            
            public string WFStatus { get; set; }

            public string WFType { get; set; }

            public string WFRequesterRoleId { get; set; }
            public int WFCompanyId { get; set; }

            public string WFComments { get; set; }

            public string CreatedBy { get; set; }

            public string UpdatedBy { get; set; }

            public string ParentName { get; set; }

            public string ParentCode { get; set; }

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

            public Nullable<DateTime> LcrEffectiveStartDate { get; set; }
        
            public Nullable<int> LppParentPayeeId { get; set; }
            public System.DateTime LppEffectiveStartDate { get; set; }
            public Nullable<System.DateTime> LppEffectiveEndDate { get; set; }
        
            public bool LpCreateLogin { get; set; }

            public string ParameterCarrier { get; set; }//to store extra parameter supplied to api
          public string LpFinOpsRoles { get; set; }
        public bool LpCreatedByForm { get; set; }

    }
}