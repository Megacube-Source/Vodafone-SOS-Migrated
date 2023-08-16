using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class GKeyValues
    {
        public int Id { get; set; }
        public string GkvKey { get; set; }
        public string GkvValue { get; set; }
        public string GkvDescription { get; set; }
        public string GcCode { get; set; }

    }
}