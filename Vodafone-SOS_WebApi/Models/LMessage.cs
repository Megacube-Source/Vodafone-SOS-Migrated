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
    
    public partial class LMessage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LMessage()
        {
            this.LMessageRecipients = new HashSet<LMessageRecipient>();
        }
    
        public int Id { get; set; }
        public string Message { get; set; }
        public Nullable<bool> IsImportant { get; set; }
        public string SenderID { get; set; }
        public string CreatedByID { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public string UpdatedByID { get; set; }
        public Nullable<System.DateTime> UpdatedDateTime { get; set; }
        public int SenderRoleID { get; set; }
        public int CompanyID { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual GCompany GCompany { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LMessageRecipient> LMessageRecipients { get; set; }
    }
}
