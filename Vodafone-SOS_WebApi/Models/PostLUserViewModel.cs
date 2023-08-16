using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class PostLUserViewModel
    {
        public int Id { get; set; }
        public string LuUserId { get; set; }
        public int LuCompanyId { get; set; }
        public string LuCreatedById { get; set; }
        public string LuUpdatedById { get; set; }
        public string LuFirstName { get; set; }
        public string LuLastName { get; set; }
        public string LuEmail { get; set; }
        public string LuPhone { get; set; }
        public bool LuIsManager { get; set; }
        public string LuBand { get; set; }
        public string LuStatus { get; set; }
        public System.DateTime LuCreatedDateTime { get; set; }
        public System.DateTime LuUpdatedDateTime { get; set; }
        public string A01 { get; set; }
        public string A02 { get; set; }
        public string A03 { get; set; }
        public string A04 { get; set; }
        public string A05 { get; set; }
        public Nullable<decimal> AN01 { get; set; }
        public Nullable<decimal> AN02 { get; set; }
        public Nullable<decimal> AN03 { get; set; }
        public Nullable<decimal> AN04 { get; set; }
        public Nullable<decimal> AN05 { get; set; }
        public Nullable<System.DateTime> AD01 { get; set; }
        public Nullable<System.DateTime> AD02 { get; set; }
        public Nullable<System.DateTime> AD03 { get; set; }
        public Nullable<System.DateTime> AD04 { get; set; }
        public Nullable<System.DateTime> AD05 { get; set; }
        public string LuComments { get; set; }
        public string LuReportsToId { get; set; }
        public bool LuIsAlteryxUser{get;set;}
        public List<string> Roles { get; set; }
        public string FileNames { get; set; }
        public bool LuBlockNotification { get; set; }
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
        public string ParameterCarrier { get; set; }
        public bool? LuCreateLogin { get; set; }
        public bool? IsSuperUser { get; set; }
    }
    public class UserAsDropdownViewModel
    {
        public string LuEmail { get; set; }
        public int Id { get; set; } 
        public string LuUserId { get; set; }
    }
}