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
    
    public partial class LDropDownValue
    {
        public int Id { get; set; }
        public int LdvDropDownId { get; set; }
        public string LdvValue { get; set; }
        public string LdvDescription { get; set; }
    
        public virtual LDropDown LDropDown { get; set; }
    }
}
