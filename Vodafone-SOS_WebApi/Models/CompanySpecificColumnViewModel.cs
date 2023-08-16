using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class CompanySpecificColumnViewModel
    {
        public string ColumnName { get; set; }
        public string IsNullable { get; set; }
        public string TableName { get; set; }
        public string DataType { get; set; }

        public string LcscDataType { get; set; }
        public string LcscColumnName { get; set; }
        public int Id { get; set; }
        public string LcscLabel { get; set; }
        public string LcscTooltip { get; set; }
        public bool LcscDisplayOnForm { get; set; }
        public Nullable<int> LcscOrdinalPosition { get; set; }
        public Nullable<bool> LcscIsMandatory { get; set; }
        public Nullable<int> LcscDropDownId { get; set; }
        public string LdName { get; set; }
        public Nullable<bool> LcscIsReportParameter { get; set; }
        public Nullable<int> LcscReportParameterOrdinal { get; set; }
    }
}