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
    
    public partial class LWorkFlowActionItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LWorkFlowActionItem()
        {
            this.LWorkFlowActionParameters = new HashSet<LWorkFlowActionParameter>();
        }
    
        public int Id { get; set; }
        public string LwfaiActionItemName { get; set; }
        public string LwfaiUILabel { get; set; }
        public string LwfaiActionDescription { get; set; }
        public int LwfaiLoginWFConfigId { get; set; }
        public bool LwfaiIsButtonOnWfGrid { get; set; }
        public bool LwfaiIsButtonOnForm { get; set; }
        public string LwfaiActionURL { get; set; }
        public int LwfaiShowInTabWFConfigId { get; set; }
        public int LwfaiOrdinal { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LWorkFlowActionParameter> LWorkFlowActionParameters { get; set; }
        public virtual LWorkFlowConfig LWorkFlowConfig { get; set; }
        public virtual LWorkFlowConfig LWorkFlowConfig1 { get; set; }
    }
}
