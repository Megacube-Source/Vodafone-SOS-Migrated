using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
using System.Globalization;
using System.Configuration;
using CsvHelper;
using System.IO;
using System.Reflection;

namespace Vodafone_SOS_WebApi.Controllers
{
    public class LPayController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        [HttpPost]
        public IHttpActionResult GetLPaySummary(PayeePayViewModel model, int CompanyId, string CommissionPeriodIdList, bool BatchNo, bool BatchName, bool PrimaryChannelchecked, bool CommPeriodchecked, bool Payeechecked, String Status, string UserName, string Workflow,string UserRole, string LoggedInUserId, bool CommyTypechecked)
        {
            var WFStatus = Status;
            if (Status == null || Status.Equals("") || Status.ToUpper().Equals("ALL"))
            {
                WFStatus = "PayPublished";
            }
           
            var PayeeCodeList = "";
            string ChildPayeeStr = "";
            if (string.IsNullOrEmpty(model.PayeeList))
            {
                //Section to get Payee Data from Stored Procedure starts Here
                var Query = "Exec SPGetPayeeData @UserRole,@LoggedInUserId,@CompanyId,@PortfolioList,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,null";
                SqlCommand cmd = new SqlCommand(Query);
                cmd.Parameters.AddWithValue("@PortfolioList", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@UserRole", UserRole);
                cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@sortdatafield", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@sortorder", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@pagesize", 99999);
                cmd.Parameters.AddWithValue("@pagenum", 0);
                cmd.Parameters.AddWithValue("@FilterQuery", (object)System.DBNull.Value);
                DataSet ds = Globals.GetData(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                    PayeeCodeList = string.Join(",", ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LpPayeeCode")).ToArray());
                //Added By sachin for Bug reprted on 2.1 - Error while accessing DownloadPayeePayFile method from Lpay Controller.
                string[] newData = PayeeCodeList.Split(',');
                PayeeCodeList = "";
                for (int j = 0; j < newData.Length; j = j + 1)
                {
                    PayeeCodeList += "'" + newData[j] + "',";
                }
            }
            else
            {
                //Split the PayeeIdList and store in array
                string[] Payees = model.PayeeList.Split(',');
                for (int j = 0; j < Payees.Length; j = j + 1)
                {
                    //find out the PayeeCode for each PayeeId
                    var PayeeId = Convert.ToInt32(Payees[j]);
                    var PayeeCode = db.LPayees.Where(p => p.Id == PayeeId).FirstOrDefault().LpPayeeCode;
                    PayeeCodeList += "'" + PayeeCode + "',";

                    //SS Commented Children Query on 4Feb2018
                    //Find out the child if any for the corresponding PayeeId
                    //var ChildPayeeQuery = "WITH parent AS (select LppParentPayeeId from LPayeeParents where LppParentPayeeId= {0} ), tree AS (SELECT x.LppParentPayeeId, x.LppPayeeId FROM LPayeeParents x INNER JOIN parent ON x.LppParentPayeeId = parent.LppParentPayeeId UNION ALL SELECT y.LppParentPayeeId, y.LppPayeeId  FROM LPayeeParents y INNER JOIN tree t ON y.LppParentPayeeId = t.LppPayeeId ) SELECT parent.LpPayeeCode   FROM tree t   inner join LPayees parent on t.LppParentPayeeId=parent.Id where parent.WFStatus='Completed' union select child.LpPayeeCode FROM tree t inner join LPayees child on t.LppPayeeId=child.Id where child.WFStatus='Completed'";
                    //var ChildPayeeList = db.Database.SqlQuery<PayeeResultViewModel>(ChildPayeeQuery, PayeeId).ToList();
                    //foreach (var ChildPayee in ChildPayeeList)
                    //{
                    //    ChildPayeeStr += "'" + ChildPayee.LpPayeeCode + "',";
                    //}
                }
            }
            PayeeCodeList = PayeeCodeList.TrimEnd(',');//trim comma , from end as there would be extra comma ,
            var CommPeriodList = "";
            //Split the CommissionPeriodIdList and store in array
            //This period code is defined to get the database details from where db connection will be made
            //The column Periodcode is null for timebeing but this will be not null in future
            int PeriodCode = 0;
            string CommissionPeriod = "";
            string[] CommPeriodsList = CommissionPeriodIdList.Split(',');
            for (int j = 0; j < CommPeriodsList.Length; j = j + 1)
            {
                //find out the PayeeCode for each PayeeId
                var CommPeriodId = Convert.ToInt32(CommPeriodsList[j]);
                var CommPeriod = db.LCommissionPeriods.Where(p => p.Id == CommPeriodId).FirstOrDefault();
                if (CommPeriod.LcpPeriodCode.HasValue)
                    PeriodCode = CommPeriod.LcpPeriodCode.Value;
                CommissionPeriod = CommPeriod.LcpPeriodName;//db.LCommissionPeriods.Where(p => p.Id == CommPeriodId).FirstOrDefault().LcpPeriodName;
                CommPeriodList += "'" + CommissionPeriod + "',";
            }
            CommPeriodList = CommPeriodList.TrimEnd(',');

            //Added Portfolio maching for getting the Batch Number in  R2.4
            var BatchQry = string.Empty;
            if (UserRole == "Payee" || UserRole == "Channel Manager")
            {
                // NOTE: Payee and CM can see Pay data across all channel/schemes as long as the Pay has the PayeeCode from their dropdown (direct match or child payee)
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'Pay' AND WFStatus in ('" + WFStatus + "') ";
            }
            else
            {
                if (string.IsNullOrEmpty(model.PortfolioList))
                {
                    /*If none is selected, 
                     Add user portfolios to portfolio matching during batch selection during the query to populate Pay grid for that period
                    */
                    var userData = db.LUsers.Where(x => x.LuUserId == LoggedInUserId).FirstOrDefault();
                    model.PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == userData.Id && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
                }
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Pay grid for that period*/
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (" + model.PortfolioList + ") and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'Pay' AND WFStatus in ('" + WFStatus + "') ";
            }
            var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
            string BatchNumberStr = "";
            foreach (var BatchNumber in BatchNumberList)
            {
                BatchNumberStr += BatchNumber + ",";
            }
            BatchNumberStr = BatchNumberStr.TrimEnd(',');
            if (BatchNumberStr.Equals(""))
            {
                BatchNumberStr = "''";
            }
            //END
            //Commented getting the Batch Number in  R2.4
            ////Getting list of Batch Nos and concatenating into string with ','
            //var BatchQry = "select lb.LbBatchNumber from LBatches lb where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'Pay' AND WFStatus in ('" + WFStatus + "')";
            ////var BatchQry = "select lb.LbBatchNumber from LBatches lb where lb.LbCommissionPeriod in ('" + CommissionPeriod + "') AND lb.LbBatchType = 'Pay' AND WFStatus in ('" + WFStatus + "')";
            //var BatchNumberList = db.Database.SqlQuery<int>(BatchQry);
            //string BatchNumberStr = "";
            //foreach (var BatchNumber in BatchNumberList)
            //{
            //    BatchNumberStr += BatchNumber + ",";
            //}
            //BatchNumberStr = BatchNumberStr.TrimEnd(',');
            //if (BatchNumberStr.Equals(""))
            //{
            //    BatchNumberStr = "''";
            //}
            //END
            //Depending upon Group Criteria provided, concatenate the Column into Group
            string PlaceHolder = "";
            if (Payeechecked)
            {
                PlaceHolder += ",C.XPayeeCode";
            }
            //SS commented as it wiil be replaced with XPay.XCommissionPeriod in future release
            //if (CommPeriodchecked)
            //{
            //    PlaceHolder += ",B.LbCommissionPeriod";
            //}
            if (BatchNo)
            {
                PlaceHolder += ",C.XBatchNumber";
            }
            if (PrimaryChannelchecked)
            {
                PlaceHolder += ",C.XPrimaryChannel";
            }
            if (CommyTypechecked)
            {
                PlaceHolder += ",C.XCommType";
            }
            
            if (PlaceHolder.StartsWith(","))
            {
                PlaceHolder = PlaceHolder.Substring(1);
            }
            //Placeholders are used and manipulated for use in SQL query
            var PlaceHolder2 = PlaceHolder;
            if (!PlaceHolder.Equals(""))
            {
                PlaceHolder = " group by " + PlaceHolder;
            }
            if (!PlaceHolder2.Equals(""))
            {
                PlaceHolder2 = "," + PlaceHolder2;
            }

            //var QRY = "select sum(C.LcCommAmtExTax) LcCommAmtExTax " + PlaceHolder2 + " from lPay C join LBatches B on C.LcSOSBatchNumber = B.LbBatchNumber where C.LcPayee in ("
            var QRY = "select sum(C.XPayAmtExTax) as XPayAmtExTax " + PlaceHolder2 + " from {Schema}.XPay C  where C.XPayeeCode in ("
                + ChildPayeeStr + PayeeCodeList + ") and C.XBatchNumber in (" + BatchNumberStr + ") and C.XPeriodCode =" + PeriodCode + " " + PlaceHolder;//Added Period Code in Where Clause in order to use the partition on XPay table
                                                                                                                                                          // db.Database.CommandTimeout = 180;//SS is setting the command timeout to 3 minas this query is taking toolong to execute. Will removethe lline onceissue is resolved
            var result = Globals.GetQueryResultFromOpcoDB(CompanyId, QRY, CommissionPeriod);//db.Database.SqlQuery<LPayViewModel>(QRY).ToList();
            return Ok(result);
        }


        //Pay  Review
        [HttpPost]
        public IHttpActionResult GetPayByPayeeCommissionPeriodCompanyId(PayeePayViewModel model, int CompanyId, string CommissionPeriodIdList, string Status, int PageSize, int PageNumber, string sortdatafield, string sortorder, string FilterQuery,string UserName, string Workflow, string LoggedInRoleName, int LoggedinLUserId,string LoggedInUserId)
        {
            //in case of Prelim Status, InProgress status records also need to be included.
            var newStatus = "'" + Status + "'";
            var PayeeCodeList = "";
            string ChildPayeeStr = "";
            if (string.IsNullOrEmpty(model.PayeeList))
            {
                //Section to get Payee Data from Stored Procedure starts Here
                var Query = "Exec SPGetPayeeData @UserRole,@LoggedInUserId,@CompanyId,@PortfolioList,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,null";
                SqlCommand cmd = new SqlCommand(Query);
                cmd.Parameters.AddWithValue("@PortfolioList", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@UserRole", LoggedInRoleName);
                cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@sortdatafield", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@sortorder", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@pagesize", 99999);
                cmd.Parameters.AddWithValue("@pagenum", 0);
                cmd.Parameters.AddWithValue("@FilterQuery", (object)System.DBNull.Value);
                DataSet ds = Globals.GetData(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                    PayeeCodeList = string.Join(",", ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LpPayeeCode")).ToArray());
                //Added By sachin for Bug reprted on 2.1 - Error while accessing DownloadPayeePayFile method from Lpay Controller.
                string[] newData = PayeeCodeList.Split(',');
                PayeeCodeList = "";
                for (int j = 0; j < newData.Length; j = j + 1)
                {
                    PayeeCodeList += "'" + newData[j] + "',";
                }
            }
            else
            {
                //Split the PayeeIdList and store in array
                string[] Payees = model.PayeeList.Split(',');
                for (int j = 0; j < Payees.Length; j = j + 1)
                {
                    //find out the PayeeCode for each PayeeId
                    var PayeeId = Convert.ToInt32(Payees[j]);
                    var PayeeCode = db.LPayees.Where(p => p.Id == PayeeId).FirstOrDefault().LpPayeeCode;
                    PayeeCodeList += "'" + PayeeCode + "',";

                    //SS Commented Children Query on 4Feb2018
                    //Find out the child if any for the corresponding PayeeId
                    //var ChildPayeeQuery = "WITH parent AS (select LppParentPayeeId from LPayeeParents where LppParentPayeeId= {0} ), tree AS (SELECT x.LppParentPayeeId, x.LppPayeeId FROM LPayeeParents x INNER JOIN parent ON x.LppParentPayeeId = parent.LppParentPayeeId UNION ALL SELECT y.LppParentPayeeId, y.LppPayeeId  FROM LPayeeParents y INNER JOIN tree t ON y.LppParentPayeeId = t.LppPayeeId ) SELECT parent.LpPayeeCode   FROM tree t   inner join LPayees parent on t.LppParentPayeeId=parent.Id where parent.WFStatus='Completed' union select child.LpPayeeCode FROM tree t inner join LPayees child on t.LppPayeeId=child.Id where child.WFStatus='Completed'";
                    //var ChildPayeeList = db.Database.SqlQuery<PayeeResultViewModel>(ChildPayeeQuery, PayeeId).ToList();
                    //foreach (var ChildPayee in ChildPayeeList)
                    //{
                    //    ChildPayeeStr += "'" + ChildPayee.LpPayeeCode + "',";
                    //}
                }
            }
            PayeeCodeList = PayeeCodeList.TrimEnd(',');//trim comma , from end as there would be extra comma 
            var CommPeriodList = "";
            var PeriodCode = 0;
            var CommissionPeriod = "";
            //Split the CommissionPeriodIdList and store in array
            string[] CommPeriodsList = CommissionPeriodIdList.Split(',');
            for (int j = 0; j < CommPeriodsList.Length; j = j + 1)
            {
                //find out the PayeeCode for each PayeeId
                var CommPeriodId = Convert.ToInt32(CommPeriodsList[j]);
                var CommPeriod = db.LCommissionPeriods.Where(p => p.Id == CommPeriodId).FirstOrDefault();
                CommissionPeriod = CommPeriod.LcpPeriodName;
                if (CommPeriod.LcpPeriodCode.HasValue)
                    PeriodCode = CommPeriod.LcpPeriodCode.Value;
                CommPeriodList += "'" + CommissionPeriod + "',";
            }
            CommPeriodList = CommPeriodList.TrimEnd(',');
            var BatchQry = string.Empty;
            if (LoggedInRoleName == "Payee" || LoggedInRoleName == "Channel Manager")
            {
                // NOTE: Payee and CM can see Pay data across all channel/schemes as long as the Pay has the PayeeCode from their dropdown (direct match or child payee)
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'Pay' AND WFStatus in (" + newStatus + ") ";
            }
            else
            {
                if (string.IsNullOrEmpty(model.PortfolioList))
                {
                    /*If none is selected, 
                     Add user portfolios to portfolio matching during batch selection during the query to populate Pay grid for that period
                    */
                    model.PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == LoggedinLUserId && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
                }
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Pay grid for that period*/
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (" + model.PortfolioList + ") and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'Pay' AND WFStatus in (" + newStatus + ") ";
            }
            var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
            string BatchNumberStr = "";
            foreach (var BatchNumber in BatchNumberList)
            {
                BatchNumberStr += BatchNumber + ",";
            }
            BatchNumberStr = BatchNumberStr.TrimEnd(',');
            if (BatchNumberStr.Equals(""))
            {
                BatchNumberStr = "''";
            }
            //As per JS directions we will save column Name in LCompanySpecificColumnsas ColumnName~DataType
            var CompanySpecificColumnsList = db.LCompanySpecificColumns.Where(p => p.LcscCompanyId == CompanyId).Where(p => p.LcscTableName == "XPay").Select(p => p.LcscColumnName).ToList();
            for (var i = 0; i < CompanySpecificColumnsList.Count(); i++)
            {
                CompanySpecificColumnsList[i] = CompanySpecificColumnsList.ElementAt(i);
            }

            Boolean CanRaiseClaims = false;
            string WheatherAllowedOrNot = string.Empty;
            if (LoggedInRoleName == "Payee")
            {

                CanRaiseClaims = db.LPayees.Where(x => x.LpUserId == LoggedInUserId).Select(x => x.LpCanRaiseClaims).FirstOrDefault();
                if (CanRaiseClaims == true)
                {
                    RWorkFlow RW = db.RWorkFlows.Where(x => x.RwfName == "Claims").FirstOrDefault();
                    var CompanyCodeData = db.GCompanies.Where(x => x.Id == CompanyId).Select(x => x.GcCode).FirstOrDefault();
                    var roleid = db.AspNetRoles.Where(x => x.CompanyCode == CompanyCodeData && x.Name == LoggedInRoleName).Select(x => x.Id).FirstOrDefault();
                    WheatherAllowedOrNot = db.Database.SqlQuery<string>("select dbo.FnGetCreateClaimPermission({0},{1},{2})", roleid, RW.Id, CompanyId).FirstOrDefault(); //Sachin
                                                                                                                                                                          // var data = db.LWorkFlowConfigs.Where(x => x.LwfcRoleId == roleid && x.LwfcCompanyId == CompanyId & x.LwfcWorkFlowId== RW.Id).Select(x => x.LwfcCanCreate).FirstOrDefault();
                    WheatherAllowedOrNot = "'" + WheatherAllowedOrNot + "'" + " as ClaimPermission";
                }
                else
                {
                    // var data = db.LWorkFlowConfigs.Where(x => x.LwfcRoleId == roleid && x.LwfcCompanyId == CompanyId & x.LwfcWorkFlowId== RW.Id).Select(x => x.LwfcCanCreate).FirstOrDefault();
                    WheatherAllowedOrNot = "'False' as ClaimPermission";
                }

            }
            else
            {
                RWorkFlow RW = db.RWorkFlows.Where(x => x.RwfName == "Claims").FirstOrDefault();
                var CompanyCodeData = db.GCompanies.Where(x => x.Id == CompanyId).Select(x => x.GcCode).FirstOrDefault();
                var roleid = db.AspNetRoles.Where(x => x.CompanyCode == CompanyCodeData && x.Name == LoggedInRoleName).Select(x => x.Id).FirstOrDefault();
                WheatherAllowedOrNot = db.Database.SqlQuery<string>("select dbo.FnGetCreateClaimPermission({0},{1},{2})", roleid, RW.Id, CompanyId).FirstOrDefault(); //Sachin
                                                                                                                                                                      // var data = db.LWorkFlowConfigs.Where(x => x.LwfcRoleId == roleid && x.LwfcCompanyId == CompanyId & x.LwfcWorkFlowId== RW.Id).Select(x => x.LwfcCanCreate).FirstOrDefault();
                WheatherAllowedOrNot = "'" + WheatherAllowedOrNot + "'" + " as ClaimPermission";

            }

            // string ColumnList = " dbo.FnGetUserName(LcAcceptedById) as LcAcceptedById,LcIsPayeeAccepted,LcPayeeAttachmentId,Id, LcOpCoCode, LcSource, LcAdjustmenCode, LcSOSBatchNumber, LcAlteryxTransactionNumber, LcPrimaryChannel, LcPayee, LcParentPayee, LcOrderDate, LcConnectionDate, LcTerminationDate, LcSubscriberNumber, LcBAN, LcActivityType, LcPlanDescrition, LcProductCode, LcUpgradeCode, LcIMEI, LcDevieCode, LcDeviceType, LcCommType, LcContractDuration, LcContractId, LcCommAmtExTax, LcTax, LcCommAmtIncTax, LcComments";
            string ColumnList = String.Join(",", CompanySpecificColumnsList);
            var qry = "Select *," + WheatherAllowedOrNot + " From (select " + ColumnList + ",ROW_NUMBER() OVER (ORDER BY XAlteryxTransactionNumber) as row from {Schema}.XPay where XPayeeCode in("
                          + ChildPayeeStr + PayeeCodeList + ") AND  XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ")) a";
            //apply paging
            qry += "  Where row > " + PageNumber * PageSize + " And row <= " + (PageNumber + 1) * PageSize;
            //Apply filterations
            qry = qry + FilterQuery;
            if (!string.IsNullOrEmpty(sortorder))//code for server side filtering
            {
                if (sortorder == "asc")
                {
                    qry += " order by " + sortdatafield;
                }
                else
                {
                    qry += " order by " + sortdatafield + " desc";
                }
            }
            var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, qry, CommissionPeriod);//db.Database.SqlQuery<LPayViewModel>(qry).ToList();
            return Ok(xx);
        }

        //method to download Pay  Review Grid
        [HttpGet]
        public IHttpActionResult DownloadPayByPayeeCommissionPeriodCompanyId(int LoggedinLUserId, string LoggedInRoleName, string CompanyCode, string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, string PortfolioList, string UserName, string Workflow,string LoggedInUserId)
        {
            //in case of Prelim Status, InProgress status records also need to be included.
            var newStatus = "'" + Status + "'";
            var PayeeCodeList = "";
            string ChildPayeeStr = "";
            if (string.IsNullOrEmpty(PayeeIdList))
            {
                //Section to get Payee Data from Stored Procedure starts Here
                var Query = "Exec SPGetPayeeData @UserRole,@LoggedInUserId,@CompanyId,@PortfolioList,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,null";
                SqlCommand cmd = new SqlCommand(Query);
                cmd.Parameters.AddWithValue("@PortfolioList", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@UserRole", LoggedInRoleName);
                cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@sortdatafield", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@sortorder", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@pagesize", 99999);
                cmd.Parameters.AddWithValue("@pagenum", 0);
                cmd.Parameters.AddWithValue("@FilterQuery", (object)System.DBNull.Value);
                DataSet ds = Globals.GetData(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                    PayeeCodeList = string.Join(",", ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LpPayeeCode")).ToArray());
                //Added By sachin for Bug reprted on 2.1 - Error while accessing DownloadPayeePayFile method from Lpay Controller.
                string[] newData = PayeeCodeList.Split(',');
                PayeeCodeList = "";
                for (int j = 0; j < newData.Length; j = j + 1)
                {
                    PayeeCodeList += "'" + newData[j] + "',";
                }
                //End
                }
            else
            {
                //Split the PayeeIdList and store in array
                string[] Payees = PayeeIdList.Split(',');
                for (int j = 0; j < Payees.Length; j = j + 1)
                {
                    //find out the PayeeCode for each PayeeId
                    var PayeeId = Convert.ToInt32(Payees[j]);
                    var PayeeCode = db.LPayees.Where(p => p.Id == PayeeId).FirstOrDefault().LpPayeeCode;
                    PayeeCodeList += "'" + PayeeCode + "',";

                    //SS Commented Children Query on 4Feb2018
                    //Find out the child if any for the corresponding PayeeId
                    //var ChildPayeeQuery = "WITH parent AS (select LppParentPayeeId from LPayeeParents where LppParentPayeeId= {0} ), tree AS (SELECT x.LppParentPayeeId, x.LppPayeeId FROM LPayeeParents x INNER JOIN parent ON x.LppParentPayeeId = parent.LppParentPayeeId UNION ALL SELECT y.LppParentPayeeId, y.LppPayeeId  FROM LPayeeParents y INNER JOIN tree t ON y.LppParentPayeeId = t.LppPayeeId ) SELECT parent.LpPayeeCode   FROM tree t   inner join LPayees parent on t.LppParentPayeeId=parent.Id where parent.WFStatus='Completed' union select child.LpPayeeCode FROM tree t inner join LPayees child on t.LppPayeeId=child.Id where child.WFStatus='Completed'";
                    //var ChildPayeeList = db.Database.SqlQuery<PayeeResultViewModel>(ChildPayeeQuery, PayeeId).ToList();
                    //foreach (var ChildPayee in ChildPayeeList)
                    //{
                    //    ChildPayeeStr += "'" + ChildPayee.LpPayeeCode + "',";
                    //}
                }
            }
            PayeeCodeList = PayeeCodeList.TrimEnd(',');//trim comma , from end as there would be extra comma ,
            var PeriodCode = 0;
            var CommissionPeriod = "";
            var CommPeriodList = "";
            //Split the CommissionPeriodIdList and store in array
            string[] CommPeriodsList = CommissionPeriodIdList.Split(',');
            for (int j = 0; j < CommPeriodsList.Length; j = j + 1)
            {
                //find out the PayeeCode for each PayeeId
                var CommPeriodId = Convert.ToInt32(CommPeriodsList[j]);
                var CommPeriod = db.LCommissionPeriods.Where(p => p.Id == CommPeriodId).FirstOrDefault();
                CommissionPeriod = CommPeriod.LcpPeriodName;
                if (CommPeriod.LcpPeriodCode.HasValue)
                    PeriodCode = CommPeriod.LcpPeriodCode.Value;
                CommPeriodList += "'" + CommissionPeriod + "',";
            }
            CommPeriodList = CommPeriodList.TrimEnd(',');

            var BatchQry = string.Empty;
            if (LoggedInRoleName == "Payee" || LoggedInRoleName == "Channel Manager")
            {
                // NOTE: Payee and CM can see Pay data across all channel/schemes as long as the Pay has the PayeeCode from their dropdown (direct match or child payee)
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'Pay' AND WFStatus in (" + newStatus + ") ";
            }
            else
            {
                if (string.IsNullOrEmpty(PortfolioList))
                {
                    /*If none is selected, 
                     Add user portfolios to portfolio matching during batch selection during the query to populate Pay grid for that period
                    */
                    PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == LoggedinLUserId && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
                }
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Pay grid for that period*/
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (" + PortfolioList + ") and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'Pay' AND WFStatus in (" + newStatus + ") ";
            }
            var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
            string BatchNumberStr = "";
            foreach (var BatchNumber in BatchNumberList)
            {
                BatchNumberStr += BatchNumber + ",";
            }
            BatchNumberStr = BatchNumberStr.TrimEnd(',');
            if (BatchNumberStr.Equals(""))
            {
                BatchNumberStr = "''";
            }

            //var qry = "select LcOpCoCode as OpCoCode,LcSource as Source,LcAdjustmenCode as AdjustmenCode,LcSOSBatchNumber as SOSBatchNumber,LcAlteryxTransactionNumber as AlteryxTransactionNumber,LcPrimaryChannel as PrimaryChannel,LcPayee as Payee,LcParentPayee as ParentPayee,LcOrderDate as OrderDate,LcConnectionDate as ConnectionDate,LcTerminationDate as TerminationDate,LcSubscriberNumber as SubscriberNumber,LcBAN as BAN,LcActivityType as ActivityType,LcPlanDescrition as PlanDescrition,LcProductCode as ProductCode,LcUpgradeCode as UpgradeCode,LcIMEI as IMEI,LcDevieCode as DevieCode,LcDeviceType as DeviceType,LcCommType as CommType,LcContractDuration as ContractDuration,LcContractId as Contract,LcCommAmtExTax as CommAmtExTax,LcTax as Tax,LcCommAmtIncTax as CommAmtIncTax,LcComments as Comments,LcIsPayeeAccepted as IsPayeeAccepted,LcAcceptedDateTime as AcceptedDateTime,ROW_NUMBER() OVER (ORDER BY Id desc) as row from LPay where LcPayee in("
            //              + ChildPayeeStr + PayeeCodeList + ") AND LcSOSBatchNumber in (" + BatchNumberStr + ")";
            var FileName = "ExportPayeePayFile_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".zip";
            //As per JS directions we will save column Name in LCompanySpecificColumnsas ColumnName~DataType
            var CompanySpecificColumnsList = db.LCompanySpecificColumns.Where(p => p.LcscCompanyId == CompanyId).Where(p => p.LcscTableName == "XPay").Select(p => new CompanySpecificColumnViewModel { ColumnName = p.LcscColumnName, LcscLabel = p.LcscLabel }).ToList();
            for (var i = 0; i < CompanySpecificColumnsList.Count(); i++)
            {
                CompanySpecificColumnsList[i].LcscColumnName = CompanySpecificColumnsList.ElementAt(i).ColumnName + " as [" + ((string.IsNullOrEmpty(CompanySpecificColumnsList.ElementAt(i).LcscLabel)) ? CompanySpecificColumnsList.ElementAt(i).ColumnName.Replace("X", "") : CompanySpecificColumnsList.ElementAt(i).LcscLabel) + "]";
            }
            string ColumnList = String.Join(",", CompanySpecificColumnsList.Select(p => p.LcscColumnName));
            string Qry = "select " + ColumnList + " from {Schema}.XPay where  XPayeeCode in (" + ChildPayeeStr + PayeeCodeList + ") and XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ")";

            //method to get query result and save it as an excel file in bucket
            var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, CommissionPeriod);//db.Database.SqlQuery<LPayViewModel>(Qry).ToList();
            Globals.ExportZipFromDataTable(null, CompanyCode, UserName, FileName, xx);
            return Ok(FileName);
        }


        //get Counts for the Pay  Review Grid--
        [HttpPost]
        public IHttpActionResult GetPayByPayeeCommissionPeriodCompanyIdCounts(PayeeCalcViewModel model, int CompanyId, string CommissionPeriodIdList, string Status, string UserName, string Workflow, string LoggedInRoleName, int LoggedinLUserId,string LoggedInUserId)
        {
            //in case of Prelim Status, InProgress status records also need to be included.
            var newStatus = "'" + Status + "'";

            var PayeeCodeList = "";
            string ChildPayeeStr = "";
            if (string.IsNullOrEmpty(model.PayeeList))
            {
                //Section to get Payee Data from Stored Procedure starts Here
                var Query = "Exec SPGetPayeeData @UserRole,@LoggedInUserId,@CompanyId,@PortfolioList,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,null";
                SqlCommand cmd = new SqlCommand(Query);
                cmd.Parameters.AddWithValue("@PortfolioList", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@UserRole", LoggedInRoleName);
                cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@sortdatafield", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@sortorder", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@pagesize", 99999);
                cmd.Parameters.AddWithValue("@pagenum", 0);
                cmd.Parameters.AddWithValue("@FilterQuery", (object)System.DBNull.Value);
                DataSet ds = Globals.GetData(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                    PayeeCodeList = string.Join(",", ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LpPayeeCode")).ToArray());
                //Added By sachin for Bug reprted on 2.1 - Error while accessing DownloadPayeePayFile method from Lpay Controller.
                string[] newData = PayeeCodeList.Split(',');
                PayeeCodeList = "";
                for (int j = 0; j < newData.Length; j = j + 1)
                {
                    PayeeCodeList += "'" + newData[j] + "',";
                }
            }
            else
            {
                //Split the PayeeIdList and store in array
                string[] Payees = model.PayeeList.Split(',');
                for (int j = 0; j < Payees.Length; j = j + 1)
                {
                    //find out the PayeeCode for each PayeeId
                    var PayeeId = Convert.ToInt32(Payees[j]);
                    var PayeeCode = db.LPayees.Where(p => p.Id == PayeeId).FirstOrDefault().LpPayeeCode;
                    PayeeCodeList += "'" + PayeeCode + "',";

                }
            }
            PayeeCodeList = PayeeCodeList.TrimEnd(',');//trim comma , from end as there would be extra comma ,

            var CommPeriodList = "";
            int PeriodCode = 0;
            string CommissionPeriod = "";
            //Split the CommissionPeriodIdList and store in array
            string[] CommPeriodsList = CommissionPeriodIdList.Split(',');
            for (int j = 0; j < CommPeriodsList.Length; j = j + 1)
            {
                //find out the PayeeCode for each PayeeId
                var CommPeriodId = Convert.ToInt32(CommPeriodsList[j]);
                var CommPeriod = db.LCommissionPeriods.Where(p => p.Id == CommPeriodId).FirstOrDefault();
                CommissionPeriod = CommPeriod.LcpPeriodName;
                if (CommPeriod.LcpPeriodCode.HasValue)
                    PeriodCode = CommPeriod.LcpPeriodCode.Value;
                CommPeriodList += "'" + CommissionPeriod + "',";
            }
            CommPeriodList = CommPeriodList.TrimEnd(',');
            var BatchQry = string.Empty;
            if (LoggedInRoleName == "Payee" || LoggedInRoleName == "Channel Manager")
            {
                // NOTE: Payee and CM can see Pay data across all channel/schemes as long as the Pay has the PayeeCode from their dropdown (direct match or child payee)
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'Pay' AND WFStatus in (" + newStatus + ") and WFCompanyId="+CompanyId;
            }
            else
            {
                if (string.IsNullOrEmpty(model.PortfolioList))
                {
                    /*If none is selected, 
                     Add user portfolios to portfolio matching during batch selection during the query to populate Pay grid for that period
                    */
                    model.PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == LoggedinLUserId && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
                }
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Pay grid for that period*/
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (" + model.PortfolioList + ") and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'Pay' AND WFStatus in (" + newStatus + ") ";
            }
            var BatchNumberList = db.Database.SqlQuery<int>(BatchQry);
            string BatchNumberStr = "";
            foreach (var BatchNumber in BatchNumberList)
            {
                BatchNumberStr += BatchNumber + ",";
            }
            BatchNumberStr = BatchNumberStr.TrimEnd(',');
            if (BatchNumberStr.Equals(""))
            {
                BatchNumberStr = "''";
            }
            var qry = "select count(*) as RowCounts from {Schema}.XPay where XPayeeCode in("
                       + ChildPayeeStr + PayeeCodeList + ") AND  XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ")";
            var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, qry, CommissionPeriod);
            // int xx = db.Database.SqlQuery<int>(qry).FirstOrDefault();
            return Ok(xx.Rows[0].Field<int>("RowCounts"));
        }


       

        public IHttpActionResult DownloadLPayGrid(int TransactionId,string CompanyCode,string LoggedInUserName)
        {
            var FilesPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName+"/forzip";
            if (System.IO.Directory.Exists(FilesPath))
            {
                try
                {
                    string PayFileName = "ExportPayFile_*.*";
                    var PayFileList = Directory.GetFiles(FilesPath, PayFileName, SearchOption.AllDirectories).ToList();
                    foreach (var PayFile in PayFileList)
                    {
                        if (System.IO.File.Exists(PayFile))//delete already existing file
                            System.IO.File.Delete(PayFile);
                    }
                }
                catch
                {
                    //do nothing
                }
            }
            //Get batch Number
            var BatchDetails = db.LBatches.Find(TransactionId);
            //Column List for LPay
            string ColumnList = "LpOpCoCode, LpSOSBatchNumber as SOSBatchNumber, LpAlteryxTransactionNumber as AlteryxTransactionNumber, LpPrimaryChannel as PrimaryChannel, LpPayee as Payee, LpParentPayee as ParentPayee, LpOrderDate as OrderDate, LpConnectionDate as ConnectionDate, LpTerminationDate as TerminationDate, LpSubscriberNumber as SubscriberNumber, LpBAN as BAN, LpActivityType as ActivityType, LpPlanDescrition as PlanDescrition, LpProductCode as ProductCode, LpUpgradeCode as UpgradeCode, LpIMEI as IMEI, LpDevieCode as DevieCode, LpDeviceType as DeviceType, LpCommType as CommType, LpContractDuration as ContractDuration, LpContractId as ContractId, LpCommAmtExTax as CommAmtExTax, LpTax as Tax, LpCommAmtIncTax as CommAmtIncTax, LpComments as Comments";
            //Using the column list obtained above, and other parameters passed in the method create a SQL query to fatch the data from database
            string Qry = "Select * from (Select " + ColumnList + ", ";
            Qry = Qry + "ROW_NUMBER() OVER (ORDER BY LpAlteryxTransactionNumber) as row FROM LPay ";
            Qry = Qry + " Where LpSOSBatchNumber = " +BatchDetails.LbBatchNumber + ") a ";

            var FileName = "ExportPayFile_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".zip";
            //method to get query result and save it as an excel file in S drive
            Globals.ExportZipFromDataTable(Qry, CompanyCode, LoggedInUserName, FileName,null);

            //Execute the query and return the result 
            // var xx = db.Database.SqlQuery<DownloadLPayViewModel>(Qry).ToList();

            ////Create a csv in S drive for the Payulations
            //var CfileLocation = ConfigurationManager.AppSettings["PayDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName + "/ExportPayFile.csv";
            //var FilesPath = ConfigurationManager.AppSettings["PayDocumentPath"];
            //if (System.IO.File.Exists(CfileLocation))//delete already existing file
            //    System.IO.File.Delete(CfileLocation);

            //if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["PayDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName))//create directory if not present
            //{
            //    System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["PayDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName);
            //}
            //using (var CTextWriter = new StreamWriter(CfileLocation))
            //using (var Csv = new CsvWriter(CTextWriter))
            //{
            //    Csv.WriteRecords(xx);
            //    //The below lines of code converts the data returned from api to a datatable
            //    //var tb = new DataTable(typeof(DownloadLPayViewModel).Name);

            //    //PropertyInfo[] props = typeof(DownloadLPayViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //    //foreach (var prop in props)
            //    //{
            //    //    // var displayName=PayeeModel.GetDisplayName()

            //    //    var PropName = prop.Name.Replace("Lp", "");//make heading of column without suffix
            //    //    tb.Columns.Add(PropName);

            //    //}

            //    //foreach (var item in xx)
            //    //{
            //    //    var values = new object[props.Length];
            //    //    for (var i = 0; i < props.Length; i++)
            //    //    {
            //    //        //if (!props[i].Name.Equals("Id"))
            //    //        //{
            //    //        values[i] = props[i].GetValue(item, null);
            //    //        //}
            //    //    }

            //    //    tb.Rows.Add(values);
            //    //}


            //    //foreach (DataColumn column in tb.Columns)
            //    //{
            //    //    Csv.WriteField(column.ColumnName);
            //    //}
            //    //Csv.NextRecord();

            //    //foreach (DataRow row in tb.Rows)
            //    //{
            //    //    for (var i = 0; i < tb.Columns.Count; i++)
            //    //    {
            //    //        Csv.WriteField(row[i]);
            //    //    }
            //    //    Csv.NextRecord();
            //    //}
            //}


            return Ok(FileName);
        }
        //get data for pay grid
        public IHttpActionResult GetLPayForGrid(int SOSBatchNumber, int PageNumber, int PageSize, string UserName, string Workflow, string sortdatafield, string sortorder, string FilterQuery, int CompanyId)
        {
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            var PeriodCode = 0;
            if (BatchDetails != null)
            {
                if (BatchDetails.LbPeriodCode.HasValue)
                    PeriodCode = BatchDetails.LbPeriodCode.Value;
                var SortQuery = "";
                if (!string.IsNullOrEmpty(sortdatafield))
                {
                    SortQuery = " order by " + sortdatafield + " " + sortorder;
                }
                else
                {
                    SortQuery = " ORDER BY XAlteryxTransactionNumber ";
                }
                //get company specific columns
                var CompanySpecificColumnsList = db.LCompanySpecificColumns.Where(p => p.LcscCompanyId == CompanyId).Where(p => p.LcscTableName == "XPay").Select(p => p.LcscColumnName).ToList();
                for (var i = 0; i < CompanySpecificColumnsList.Count(); i++)
                {
                    CompanySpecificColumnsList[i] = CompanySpecificColumnsList.ElementAt(i);
                }
                // string ColumnList = " dbo.FnGetUserName(LcAcceptedById) as LcAcceptedById,LcIsPayeeAccepted,LcPayeeAttachmentId,Id, LcOpCoCode, LcSource, LcAdjustmenCode, LcSOSBatchNumber, LcAlteryxTransactionNumber, LcPrimaryChannel, LcPayee, LcParentPayee, LcOrderDate, LcConnectionDate, LcTerminationDate, LcSubscriberNumber, LcBAN, LcActivityType, LcPlanDescrition, LcProductCode, LcUpgradeCode, LcIMEI, LcDevieCode, LcDeviceType, LcCommType, LcContractDuration, LcContractId, LcCommAmtExTax, LcTax, LcCommAmtIncTax, LcComments";
                string ColumnList = String.Join(",", CompanySpecificColumnsList);//" XBatchNumber,XAlteryxTransactionNumber,[XPayee],XParentPayee,[XSubscriberNumber],[XBAN],[XActivityType],XIMEI";
                                                                                 //Using the column list obtained above, and other parameters passed in the method create a SQL query to fatch the data from database
                string Qry = "Select * from (Select " + ColumnList + ", ";
                Qry = Qry + "ROW_NUMBER() OVER (" + SortQuery + ") as row FROM {Schema}.XPay ";
                Qry = Qry + " Where XBatchNumber = " + SOSBatchNumber + " and XPeriodCode=" + PeriodCode + ") a ";
                Qry = Qry + " Where row > " + PageNumber * PageSize + " And row <= " + (PageNumber + 1) * PageSize;
                if (!string.IsNullOrEmpty(FilterQuery))
                    Qry += FilterQuery;
                if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
                {
                    Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LPay", "GetLPayForGrid", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
                }
                else
                {
                    //Execute the query and return the result 
                    var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, BatchDetails.LbCommissionPeriod);//db.Database.SqlQuery<LPayViewModel>(Qry).ToList();
                    return Ok(xx);
                }
            }
            return Ok();
        }

        public IHttpActionResult GetLPayCounts(int SOSBatchNumber, string UserName, string Workflow, int CompanyId)
        {
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            var PeriodCode = 0;
            if (BatchDetails != null)
            {
                if (BatchDetails.LbPeriodCode.HasValue)
                    PeriodCode = BatchDetails.LbPeriodCode.Value;
                string Qry = "Select Count(*) as RowCounts from {Schema}.XPay Where XBatchNumber = " + SOSBatchNumber + " and XPeriodCode=" + PeriodCode;

                if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
                {
                    Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LPay", "GetLPayCounts", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
                }
                else
                {
                    var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, BatchDetails.LbCommissionPeriod);
                    return Ok(xx);
                }
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    //internal class CsvWriter : IDisposable
    //{
    //    private StreamWriter cTextWriter;

    //    public CsvWriter(StreamWriter cTextWriter)
    //    {
    //        this.cTextWriter = cTextWriter;
    //    }
    //}
}
