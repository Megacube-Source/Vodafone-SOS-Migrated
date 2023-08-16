using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class CompanySpecificLabelViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string ColumnName { get; set; }

        [RestrictSpecialChar]
        public string IsNullable { get; set; }

        [RestrictSpecialChar]
        public string TableName { get; set; }

        [RestrictSpecialChar]
        public string DataType { get; set; }

        [RestrictSpecialChar]
        public string ColumnLabel { get; set; }

        //The below objects will contain Company SpecificRawData Columns data
        public bool DisplayOnForm { get; set; }

        [RestrictSpecialChar]
        public string RawDataColumn { get; set; }
        public Nullable<bool> LcscIsReportParameter { get; set; }
        public Nullable<int> LcscReportParameterOrdinal { get; set; }
        public int Ordinal { get; set; }
    }
}