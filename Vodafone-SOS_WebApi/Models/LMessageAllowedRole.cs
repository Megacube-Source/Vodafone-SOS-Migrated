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
    
    public partial class LMessageAllowedRole
    {
        public int Id { get; set; }
        public string SenderRoleID { get; set; }
        public string ReciepientRoleId { get; set; }
    
        public virtual AspNetRole AspNetRole { get; set; }
        public virtual AspNetRole AspNetRole1 { get; set; }
    }
}
