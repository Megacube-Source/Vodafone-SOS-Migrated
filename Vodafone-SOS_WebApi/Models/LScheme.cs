//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vodafone_SOS_WebApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LScheme
    {
        public int Id { get; set; }
        public string LsName { get; set; }
        public string LsDescription { get; set; }
        public bool LsIsSchemeTested { get; set; }
        public string LsCreatedById { get; set; }
        public string LsUpdatedById { get; set; }
        public int LsCompanyId { get; set; }
        public System.DateTime LsCreatedDateTime { get; set; }
        public System.DateTime LsUpdatedDateTime { get; set; }
        public string WFRequesterId { get; set; }
        public string WFAnalystId { get; set; }
        public string WFManagerId { get; set; }
        public string WFCurrentOwnerId { get; set; }
        public string WFRequesterRoleId { get; set; }
        public Nullable<int> WFOrdinal { get; set; }
        public string WFStatus { get; set; }
        public string WFType { get; set; }
        public int WFCompanyId { get; set; }
        public string WFComments { get; set; }
        public string ParameterCarrier { get; set; }
        public Nullable<System.DateTime> WFUpdatedDateTime { get; set; }
    
        public virtual AspNetRole AspNetRole { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual AspNetUser AspNetUser3 { get; set; }
        public virtual AspNetUser AspNetUser4 { get; set; }
        public virtual AspNetUser AspNetUser5 { get; set; }
        public virtual GCompany GCompany { get; set; }
        public virtual GCompany GCompany1 { get; set; }
    }
}