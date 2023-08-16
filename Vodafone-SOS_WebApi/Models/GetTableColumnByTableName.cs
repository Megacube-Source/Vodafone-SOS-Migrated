
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class GetTableColumnByTableName
    {
        public string ColumnName { get; set; }
        public string IsNullable { get; set; }
        public string DisplayOnForm { get; set; }
        public string DataType { get; set; }
        public string ColumnLabel { get; set; }
    }
}