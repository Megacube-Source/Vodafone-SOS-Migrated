//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UserAccessManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class LEmailTemplate
    {
        public int Id { get; set; }
        public string LetTemplateName { get; set; }
        public string LetEmailSubject { get; set; }
        public string LetEmailBody { get; set; }
        public string LetSignature { get; set; }
        public int LetCompanyId { get; set; }
    
        public virtual GCompany GCompany { get; set; }
    }
}