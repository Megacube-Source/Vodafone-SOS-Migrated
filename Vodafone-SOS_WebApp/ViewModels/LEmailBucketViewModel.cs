using System;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LEmailBucketViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string LebRecipientList { get; set; }

        [RestrictSpecialChar]
        public string LebCCList { get; set; }

        [RestrictSpecialChar]
        public string LebBCCList { get; set; }

        [RestrictSpecialChar]
        public string LebReplyToList { get; set; }

        [RestrictSpecialChar]
        public string LebSubject { get; set; }

        [RestrictSpecialChar]
        public string LebBody { get; set; }
        public bool LebIsHTML { get; set; }
        [RestrictSpecialChar]
        public string LebEmailType { get; set; }

        [RestrictSpecialChar]
        public string LebPriority { get; set; }

        [RestrictSpecialChar]
        public string LebAttachmentList { get; set; }

        [RestrictSpecialChar]
        public string LebStatus { get; set; }

        [RestrictSpecialChar]
        public string LebComments { get; set; }
      
        public DateTime? LebCreatedDateTime { get; set; }
        public DateTime? LebUpdatedDateTime { get; set; }

        [RestrictSpecialChar]
        public string LebCreatedById { get; set; }

        [RestrictSpecialChar]
        public string LebUpdatedById { get; set; }
        public int LebSenderConfigId { get; set; }

        [RestrictSpecialChar]
        public string GcCode { get; set; }
        public int EmailSent { get; set; }
        public int Counts { get; set; }
    }
}