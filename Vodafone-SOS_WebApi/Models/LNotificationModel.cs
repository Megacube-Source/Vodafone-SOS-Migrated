using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class LNotificationModel
    {
        public int id { get; set; }
        public string WorkFlowName { get; set; }
        public string StepName { get; set; }
        public string Recipient { get; set; }
        public string Description { get; set; }
        public Boolean IsActive { get; set; }
        public string LetEmailSubject { get; set; }
        public string LetEmailBody { get; set; }
    }
}