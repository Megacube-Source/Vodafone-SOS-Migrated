using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class GenericGridTabDetails
    {
        [RestrictSpecialChar]
        public string TabName { get; set; }
        public int RecordCount { get; set; }
    }

    public partial class GenericGridGridColumnDetails
    {
        [RestrictSpecialChar]
        public string TabName { get; set; } //This tab name prepaned with "Grid" will become GridId in CSHTML view.

        [RestrictSpecialChar]
        public string ColumnName { get; set; }

        [RestrictSpecialChar]
        public string ColumnLabel { get; set; }

        [RestrictSpecialChar]
        public string DataType { get; set; }  
        public bool ShouldBeVisible  { get; set; }  
    }

    public partial class GenericGridDetails
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string DataType { get; set; }

        [RestrictSpecialChar]
        public string UserRole { get; set; }
        public bool LwfcCanCreate { get; set; }
        public Nullable<int> LwfcOrdinalNumber { get; set; }
        [RestrictSpecialChar]
        public string RwfBaseTableName { get; set; }

        [RestrictSpecialChar]
        public string RwfName { get; set; }

        [RestrictSpecialChar]
        public string RwfUILabel { get; set; }

        [RestrictSpecialChar]
        public string LwfgcColumnName { get; set; }

        [RestrictSpecialChar]
        public string LwfgcAscDesc { get; set; }

       
        public Nullable<int> LwfgcOrderByOrdinal { get; set; }
        public bool LwfgcShouldBeVisible { get; set; }

        [RestrictSpecialChar]
        public string LwfgcUILabel { get; set; }
        public int LwfgcWfConfigId { get; set; }
        public Nullable<int> LwfgcOrdinal { get; set; }
        public bool RwfCRAllowed { get; set; }

        [RestrictSpecialChar]
        public string LwfcBanner { get; set; }
    }

    public class OtherAPIData
    {
        [RestrictSpecialChar]
        public string TransactionID { get; set; }
    }

    public class OtherAPIDataNew
    {
        [RestrictSpecialChar]
        public string[] TransactionID { get; set; }
    }
    public class GenericGridRequestData
    {
        public int WorkflowConfigId;

        [RestrictSpecialChar]
        public string LoggedInRoleId;

        [RestrictSpecialChar]
        public string LoggedInUserId;

        [RestrictSpecialChar]
        public string WorkflowName;
        public int CompanyId;
        public int PageSize;
        public int PageNumber;

        [RestrictSpecialChar]
        public string sortdatafield;

        [RestrictSpecialChar]
        public string sortorder;

        [RestrictSpecialChar]
        public string FilterQuery;

        [RestrictSpecialChar]
        public string TabName;

        [RestrictSpecialChar]
        public string PortfolioList;
    }
}


