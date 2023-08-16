using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LNotificationViewModel
    {
        public int id { get; set; }

        [RestrictSpecialChar]
        public string WorkFlowName { get; set; }
        [RestrictSpecialChar]
        public string StepName { get; set; }

        [RestrictSpecialChar]
        public string Recipient { get; set; }

        [RestrictSpecialChar]
        public string Description { get; set; }
        public Boolean IsActive { get; set; }

        [RestrictSpecialChar]
        public string LetEmailSubject { get; set; }

        [RestrictSpecialChar]
        public string LetEmailBody { get; set; }


    }
}