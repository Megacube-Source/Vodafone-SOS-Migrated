using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class XSchemaTablesViewModel
    {
        public string TABLE_NAME { get; set; }
        public string LrdtName { get; set; }
        public Nullable<int> RawDataTableId { get; set; }
        //new column used in mapping raw Data tables
        public bool IsRawDataTableMapped { get; set; }
    }
}