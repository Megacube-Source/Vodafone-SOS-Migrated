using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class GenericGridViewModel
    {
        public int Id { get; set; }
        public string DataType { get; set; }
        public string UserRole { get; set; }
        public bool LwfcCanCreate { get; set; }
        public Nullable<int> LwfcOrdinalNumber { get; set; }
        public string RwfBaseTableName { get; set; }
        public string RwfName { get; set; }
        public string RwfUILabel { get; set; }
        public string LwfgcColumnName { get; set; }
        public string LwfgcAscDesc { get; set; }
        public Nullable<int> LwfgcOrderByOrdinal { get; set; }
        public bool LwfgcShouldBeVisible { get; set; }
        public string LwfgcUILabel { get; set; }
        public int LwfgcWfConfigId { get; set; }
       public bool RwfCRAllowed { get; set; }
        public Nullable<int> LwfgcOrdinal { get; set; }
       public string LwfgcFunctionName { get; set; }
        public string LwfcBanner { get; set; }
        public string LwfgcSelectAs { get; set; }
        public string LwfgcJoinTable { get; set; }
        public string LwfgcJoinTableColumn { get; set; }
        public string LwfgcBaseTableJoinColumn { get; set; }
    }

    public class OtherAPIData
    {
        public string TransactionID { get; set; }
    }
    public class OtherAPIDataNew
    {
        public string[] TransactionID { get; set; }
    }
    public class GenericGridRequestData
    {
        public int WorkflowConfigId;
        public string LoggedInRoleId;
        public string LoggedInUserId;
        public string WorkflowName;
        public int CompanyId;
        public int PageSize;
        public int PageNumber;
        public string sortdatafield;
        public string sortorder;
        public string FilterQuery;
        public string TabName;
        public string PortfolioList;
    }
}