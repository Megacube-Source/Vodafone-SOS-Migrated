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
    
    public partial class LSchemeOwner
    {
        public int Id { get; set; }
        public string Scheme { get; set; }
        public string OwnerId { get; set; }
        public int CompanyId { get; set; }
        public string CreatedById { get; set; }
        public string UpdatedById { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public System.DateTime UpdatedDateTime { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual GCompany GCompany { get; set; }
    }
}
