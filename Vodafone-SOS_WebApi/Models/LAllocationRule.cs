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
    
    public partial class LAllocationRule
    {
        public int Id { get; set; }
        public int LarOrdinalPosition { get; set; }
        public string LarKey { get; set; }
        public string LarValue { get; set; }
        public int LarCompanyId { get; set; }
        public string LarAllocatedToId { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual GCompany GCompany { get; set; }
    }
}
