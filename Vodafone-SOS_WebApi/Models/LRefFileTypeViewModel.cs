using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class LRefFileTypeViewModel
    {
        public int Id { get; set; }
        public int LrftCompanyId { get; set; }
        public string LrftCreatedById { get; set; }
        public string LrftUpdatedById { get; set; }
        public string LrftName { get; set; }
        public string LrftDescription { get; set; }
        public Nullable<System.DateTime> LrftCreatedDateTime { get; set; }
        public Nullable<System.DateTime> LrftUpdatedDateTime { get; set; }

        //object to carry values of Grid columns to be inserted or updated
        public string[] ModelData { get; set; }
    }
}