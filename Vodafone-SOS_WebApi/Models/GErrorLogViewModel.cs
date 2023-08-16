using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class GErrorLogViewModel
    {

        public int Id { get; set; }

       
        public System.DateTime GelErrorDateTime { get; set; }

       
        public string GelSourceProject { get; set; }

       
        public string GelController { get; set; }

        
        public string GelMethod { get; set; }

       
        public string GelStackTrace { get; set; }
       
        public string GelUserName { get; set; }

        
        public string GelErrorType { get; set; }

       
        public string GelErrorDescription { get; set; }

        
        public string GelResolution { get; set; }

       
        public string GelErrorOwner { get; set; }

       
        public string GelFieldName { get; set; }

        public Nullable<int> GelSOSBatchNumber { get; set; }

        public int counts { get; set; }
       // public int row { get; set; }

    }
}