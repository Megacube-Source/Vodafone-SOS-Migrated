using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class LEmailBucketViewModel
    {
        public int Id { get; set; }
        public string LebRecipientList { get; set; }
        public string LebCCList { get; set; }
        public string LebBCCList { get; set; }
        public string LebReplyToList { get; set; }
        public string LebSubject { get; set; }
        public string LebBody { get; set; }
        public bool LebIsHTML { get; set; }
        public string LebEmailType { get; set; }
        public string LebPriority { get; set; }
        public string LebAttachmentList { get; set; }
        public string LebStatus { get; set; }
        public string LebComments { get; set; }
        public System.DateTime? LebCreatedDateTime { get; set; }
        public System.DateTime? LebUpdatedDateTime { get; set; }
        public string LebCreatedById { get; set; }
        public string LebUpdatedById { get; set; }
        public int LebSenderConfigId { get; set; }
        public string GcCode { get; set; }
        public int EmailSent { get; set; }
    }
}