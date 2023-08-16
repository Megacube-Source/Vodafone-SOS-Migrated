//commented raw data controller because of number of changes made in backened database by vikas
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
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using Ionic.Zip;
using System.Data.Entity.Validation;
using System.ComponentModel;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LCalcController : ApiController
    {
        //To avaod multiple loops we are using the same loop of calc to update Batch and maintaining a list of batches already modified
        List<int> BatchesAlreadyModified = new List<int>();
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        //methos to display sum of LCalc Commision amount for the Payee
        public IHttpActionResult GetPayeeCalculationsGraph(int CompanyId, string PayeeUserId, int CommissionPeriodCount, string UserName, string Workflow)
        {
            string PayeeCode = db.LPayees.Where(p => p.LpUserId == PayeeUserId).Select(p => p.LpPayeeCode).FirstOrDefault();
            var CommPeriods = db.LCommissionPeriods.Where(p => p.LcpCompanyId == CompanyId).OrderByDescending(p => p.LcpCreatedDateTime).Select(p => p.LcpPeriodName).Take(CommissionPeriodCount).ToList();
            var PayeeCalcModelList = new List<PayeeCalcChartViewModel>();
            var RowNo = 1;
            foreach (var CommPeriod in CommPeriods)
            {
                var PayeeCalcModel = new PayeeCalcChartViewModel();
                PayeeCalcModel.RowNumber = RowNo;
                RowNo += 1;
                PayeeCalcModel.CommissionPeriod = (CommPeriod.Length > 10) ? CommPeriod.Substring(0, 10) : CommPeriod;
                //select SUM(LcCommAmtExTax) from lcalc where LPayees = 'JS123'-- login Payee and its children and LcSOSBatchNumber in (select LbBatchNumber from LBatches where LbCommissionPeriod = '2017_03' AND LbBatchType = 'CALC' AND WFStatus = 'Prelim')
                PayeeCalcModel.PrelimCount = db.Database.SqlQuery<decimal>("WITH MyCTE AS ( SELECT A.Id, A.LpPayeeCode FROM LPayees A WHERE A.LpCompanyId=" + CompanyId + " and A.WFStatus='Completed' and LpPayeeCode='" + PayeeCode + "' UNION ALL SELECT B.Id, B.LpPayeeCode FROM LPayees B inner join LPayeeParents PP on B.Id=PP.LppPayeeId INNER JOIN MyCTE C ON PP.LppParentPayeeId = C.Id WHERE B.LpCompanyId=" + CompanyId + " and GETDATE() between PP.LppEffectiveStartDate and isnull(pp.LppEffectiveEndDate,DateAdd(day,30,GETDATE())) and B.WFStatus='Completed' ) select isnull(SUM(LcCommAmtExTax),0) from lcalc where LcPayee  in (SELECT LpPayeeCode FROM MyCTE) and LcSOSBatchNumber in (select LbBatchNumber from LBatches where LbCommissionPeriod in ('" + CommPeriod + "')  AND LbBatchType = 'CALC' AND WFStatus = 'Prelim')").FirstOrDefault();
                //--Top part of bar(Completed)
                //select SUM(LcCommAmtExTax) from lcalc where LPayees = 'JS123'-- login Payee and its children
                //and LcSOSBatchNumber in (select LbBatchNumber from LBatches where LbCommissionPeriod = '2017_03' AND LbBatchType = 'CALC' AND WFStatus = 'Completed')
                PayeeCalcModel.CompletedCount = db.Database.SqlQuery<decimal>("WITH MyCTE AS ( SELECT A.Id, A.LpPayeeCode FROM LPayees A WHERE A.LpCompanyId=" + CompanyId + " and A.WFStatus='Completed' and LpPayeeCode='" + PayeeCode + "' UNION ALL SELECT B.Id, B.LpPayeeCode FROM LPayees B inner join LPayeeParents PP on B.Id=PP.LppPayeeId INNER JOIN MyCTE C ON PP.LppParentPayeeId = C.Id WHERE B.LpCompanyId=" + CompanyId + " and GETDATE() between PP.LppEffectiveStartDate and isnull(pp.LppEffectiveEndDate,DateAdd(day,30,GETDATE())) and B.WFStatus='Completed' ) select isnull(SUM(LcCommAmtExTax),0) from lcalc where LcPayee  in (SELECT LpPayeeCode FROM MyCTE) and LcSOSBatchNumber in (select LbBatchNumber from LBatches where LbCommissionPeriod in ('" + CommPeriod + "')  AND LbBatchType = 'CALC' AND WFStatus = 'Completed')").FirstOrDefault();
                PayeeCalcModelList.Add(PayeeCalcModel);
            }
            return Ok(PayeeCalcModelList);
        }

        //methos to display sum of LCalc Commision amount for the Payee
        public IHttpActionResult GetPayeeCalculationsGraphByPayeeId(int CompanyId, int PayeeId, int CommissionPeriodCount, string UserName, string Workflow)
        {
            string PayeeCode = db.LPayees.Where(p => p.Id == PayeeId).Select(p => p.LpPayeeCode).FirstOrDefault();
            var CommPeriods = db.LCommissionPeriods.Where(p => p.LcpCompanyId == CompanyId).OrderByDescending(p => p.LcpCreatedDateTime).Select(p => p.LcpPeriodName).Take(CommissionPeriodCount).ToList();
            var PayeeCalcModelList = new List<PayeeCalcChartViewModel>();
            var RowNo = 1;
            foreach (var CommPeriod in CommPeriods)
            {
                var PayeeCalcModel = new PayeeCalcChartViewModel();
                PayeeCalcModel.RowNumber = RowNo;
                RowNo += 1;
                PayeeCalcModel.CommissionPeriod = (CommPeriod.Length > 10) ? CommPeriod.Substring(0, 10) : CommPeriod;
                //select SUM(LcCommAmtExTax) from lcalc where LPayees = 'JS123'-- login Payee and its children and LcSOSBatchNumber in (select LbBatchNumber from LBatches where LbCommissionPeriod = '2017_03' AND LbBatchType = 'CALC' AND WFStatus = 'Prelim')
                PayeeCalcModel.PrelimCount = db.Database.SqlQuery<decimal>("WITH MyCTE AS ( SELECT A.Id, A.LpPayeeCode FROM LPayees A WHERE A.LpCompanyId=" + CompanyId + " and A.WFStatus='Completed' and LpPayeeCode='" + PayeeCode + "' UNION ALL SELECT B.Id, B.LpPayeeCode FROM LPayees B inner join LPayeeParents PP on B.Id=PP.LppPayeeId INNER JOIN MyCTE C ON PP.LppParentPayeeId = C.Id WHERE B.LpCompanyId=" + CompanyId + " and GETDATE() between PP.LppEffectiveStartDate and isnull(pp.LppEffectiveEndDate,DateAdd(day,30,GETDATE())) and B.WFStatus='Completed' ) select isnull(SUM(LcCommAmtExTax),0) from lcalc where LcPayee  in (SELECT LpPayeeCode FROM MyCTE) and LcSOSBatchNumber in (select LbBatchNumber from LBatches where LbCommissionPeriod in ('" + CommPeriod + "')  AND LbBatchType = 'CALC' AND WFStatus = 'Prelim')").FirstOrDefault();
                //--Top part of bar(Completed)
                //select SUM(LcCommAmtExTax) from lcalc where LPayees = 'JS123'-- login Payee and its children
                //and LcSOSBatchNumber in (select LbBatchNumber from LBatches where LbCommissionPeriod = '2017_03' AND LbBatchType = 'CALC' AND WFStatus = 'Completed')
                PayeeCalcModel.CompletedCount = db.Database.SqlQuery<decimal>("WITH MyCTE AS ( SELECT A.Id, A.LpPayeeCode FROM LPayees A WHERE A.LpCompanyId=" + CompanyId + " and A.WFStatus='Completed' and LpPayeeCode='" + PayeeCode + "' UNION ALL SELECT B.Id, B.LpPayeeCode FROM LPayees B inner join LPayeeParents PP on B.Id=PP.LppPayeeId INNER JOIN MyCTE C ON PP.LppParentPayeeId = C.Id WHERE B.LpCompanyId=" + CompanyId + " and GETDATE() between PP.LppEffectiveStartDate and isnull(pp.LppEffectiveEndDate,DateAdd(day,30,GETDATE())) and B.WFStatus='Completed' ) select isnull(SUM(LcCommAmtExTax),0) from lcalc where LcPayee  in (SELECT LpPayeeCode FROM MyCTE) and LcSOSBatchNumber in (select LbBatchNumber from LBatches where LbCommissionPeriod in ('" + CommPeriod + "')  AND LbBatchType = 'CALC' AND WFStatus = 'Completed')").FirstOrDefault();
                PayeeCalcModelList.Add(PayeeCalcModel);
            }
            return Ok(PayeeCalcModelList);
        }

        //this method is used to calculate aggregarted data for Payee depending on the combinations used.

        [HttpPost]
        public IHttpActionResult GetLCalcSummary(PayeeCalcViewModel model, int CompanyId, string CommissionPeriodIdList, bool BatchNo, bool BatchName, bool Source, bool CommType, bool CommPeriodchecked, bool Payeechecked, String Status, string UserName, string Workflow, string LoggedInUserId, string UserRole, bool MSISDN, int PageSize, int PageNumber, string sortdatafield, string sortorder, string FilterQuery)
        {
            var WFStatus = "Prelim','InProgress','Completed";
            string BatchNumberStr = "";
            var StatusForGrouping = "";
            if (Status == null || Status.Equals("") || Status.ToUpper().Equals("ALL"))
            {
                StatusForGrouping = "'" + WFStatus + "'";
            }
            else if (Status.Equals("Prelim"))
            {
                /*Calc Review Prelim tab. Currently we pick batches which are Prelim AND InProgress. 
            * It should pick only ‘Prelim’. Remove InProgress. See if we have put this in where clause at other places too (like summary tab etc)*/
                StatusForGrouping = "'Prelim'";
                WFStatus = "'Prelim'";
            }
            else if (Status.Equals("Approved"))
            {
                WFStatus = "'Completed','Paid'";
                StatusForGrouping = "'Completed','Paid'";
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
                {
                    var PayeeCodesList = ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LpPayeeCode")).ToList();
                    foreach (var payeecode in PayeeCodesList)
                    {
                        PayeeCodeList += "'" + payeecode + "',";
                    }
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

            //var PayeeCode = db.LPayees.Where(p => p.Id == PayeeId).FirstOrDefault().LpPayeeCode;
            ////get CommissionPeriod from Name
            //var CommissionPeriod = db.LCommissionPeriods.Where(p => p.Id == CommissionPeriodId).FirstOrDefault().LcpPeriodName;
            ////Get list of child Payees
            //var Query = "WITH parent AS (select LppParentPayeeId from LPayeeParents where LppParentPayeeId= {0} ), tree AS (SELECT x.LppParentPayeeId, x.LppPayeeId FROM LPayeeParents x INNER JOIN parent ON x.LppParentPayeeId = parent.LppParentPayeeId UNION ALL SELECT y.LppParentPayeeId, y.LppPayeeId  FROM LPayeeParents y INNER JOIN tree t ON y.LppParentPayeeId = t.LppPayeeId ) SELECT parent.LpPayeeCode   FROM tree t   inner join LPayees parent on t.LppParentPayeeId=parent.Id where parent.WFStatus='Completed' union select child.LpPayeeCode FROM tree t inner join LPayees child on t.LppPayeeId=child.Id where child.WFStatus='Completed'";
            //var ChildPayeeList = db.Database.SqlQuery<PayeeResultViewModel>(Query, PayeeId).ToList();
            //string ChildPayeeStr = "";
            //foreach (var ChildPayee in ChildPayeeList)
            //{
            //    ChildPayeeStr += "'" + ChildPayee.LpPayeeCode + "',";
            //}

            //Getting list of Batch Nos and concatenating into string with ','
            if (UserRole == "Payee" || UserRole == "Channel Manager")
            {
                if (CommPeriodList.Equals(""))
                {
                    CommPeriodList = "''";
                }
                var BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in (" + WFStatus + ") and WFCompanyId=" + CompanyId;
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.PortfolioList))
                {
                    /*If none is selected, 
                     Add user portfolios to portfolio matching during batch selection during the query to populate Calc grid for that period
                    */
                    var lusers = db.LUsers.Where(x => x.LuUserId == LoggedInUserId).FirstOrDefault();
                    model.PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == lusers.Id && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
                }
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Calc grid for that period*/
                if (model.PortfolioList.Equals(""))
                {
                    model.PortfolioList = "''";
                }
                if (CommPeriodList.Equals(""))
                {
                    CommPeriodList = "''";
                }
                var BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (select Id from LPortfolios where LpChannelId in (select LpChannelId from LPortfolios where Id in (" + model.PortfolioList + ") )) and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in (" + WFStatus + ") ";
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }









            ////var BatchQry = "select lb.LbBatchNumber from LBatches lb where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('" + WFStatus + "') and WFCompanyId="+CompanyId;
            //var BatchQry = "select lb.LbBatchNumber from LBatches lb where lb.LbCommissionPeriod in ('" + CommissionPeriod + "') AND lb.LbBatchType = 'CALC' AND WFStatus in ('" + WFStatus + "')";
            ////var BatchNumberList = db.Database.SqlQuery<int>(BatchQry);
            //// string BatchNumberStr = "";
            ////foreach (var BatchNumber in BatchNumberList)
            ////{
            ////    BatchNumberStr += BatchNumber + ",";
            ////}
            ////BatchNumberStr = BatchNumberStr.TrimEnd(',');
            ////if (BatchNumberStr.Equals(""))
            ////{
            ////    BatchNumberStr = "''";
            ////}
            //Depending upon Group Criteria provided, concatenate the Column into Group
            string PlaceHolder = "";
            if (Payeechecked)
            {
                PlaceHolder += ",C.XPayee";
            }
            //SS commented as it wiil be replaced with XCalc.XCommissionPeriod in future release
            //if (CommPeriodchecked)
            //{
            //    PlaceHolder += ",B.LbCommissionPeriod";
            //}
            if (BatchNo)
            {
                PlaceHolder += ",C.XBatchNumber";
            }
            if (Source)
            {
                PlaceHolder += ",C.XSource";
            }
            if (CommType)
            {
                PlaceHolder += ",C.XCommType";
            }
            if (MSISDN)
            {
                PlaceHolder += ",C.XSubscriberNumber";
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
            //if (MSISDN)
            //{
            //    PlaceHolder += ",C.XSubscriberNumber";
            //}
            //SS commented the below query as it was taking 1.5 min to execute with the join. Hence the join has been removed as for now.In the future Commission Period grouping will be added from XCalc
            //var QRY = "select sum(C.LcCommAmtExTax) LcCommAmtExTax " + PlaceHolder2 + " from lcalc C join LBatches B on C.LcSOSBatchNumber = B.LbBatchNumber where C.LcPayee in ("


            string newPlaceHolder = PlaceHolder2.TrimStart(',');

            var QRY = "select * from (select sum(C.XCommAmtExTax) as XCommAmtExTax " + PlaceHolder2 + ",ROW_NUMBER() OVER (ORDER BY " + newPlaceHolder + "  desc) as row from {Schema}.XCalc C  where C.XPayee in ("
                + ChildPayeeStr + PayeeCodeList + ") and C.XBatchNumber in (" + BatchNumberStr + ") and C.XPeriodCode =" + PeriodCode + " " + FilterQuery + PlaceHolder + " )  a Where row > " + PageNumber * PageSize + " And row <= " + (PageNumber + 1) * PageSize;//Added Period Code in Where Clause in order to use the partition on XCalc table
                                                                                                                                                                                                                                                                      // db.Database.CommandTimeout = 180;//SS is setting the command timeout to 3 minas this query is taking toolong to execute. Will removethe lline onceissue is resolved
            var result = Globals.GetQueryResultFromOpcoDB(CompanyId, QRY, CommissionPeriod);//db.Database.SqlQuery<LCalcViewModel>(QRY).ToList();
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GetLCalcSummaryCount(PayeeCalcViewModel model, int CompanyId, string CommissionPeriodIdList, bool BatchNo, bool BatchName, bool Source, bool CommType, bool CommPeriodchecked, bool Payeechecked, String Status, string UserName, string Workflow, string LoggedInUserId, string UserRole, bool MSISDN)
        {
            var WFStatus = "Prelim','InProgress','Completed";
            string BatchNumberStr = "";
            var StatusForGrouping = "";
            if (Status == null || Status.Equals("") || Status.ToUpper().Equals("ALL"))
            {
                StatusForGrouping = "'" + WFStatus + "'";
            }
            else if (Status.Equals("Prelim"))
            {
                /*Calc Review Prelim tab. Currently we pick batches which are Prelim AND InProgress. 
            * It should pick only ‘Prelim’. Remove InProgress. See if we have put this in where clause at other places too (like summary tab etc)*/
                StatusForGrouping = "'Prelim'";
                WFStatus = "'Prelim'";
            }
            else if (Status.Equals("Approved"))
            {
                WFStatus = "'Completed','Paid'";
                StatusForGrouping = "'Completed','Paid'";
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
                {
                    var PayeeCodesList = ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LpPayeeCode")).ToList();
                    foreach (var payeecode in PayeeCodesList)
                    {
                        PayeeCodeList += "'" + payeecode + "',";
                    }
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

            //var PayeeCode = db.LPayees.Where(p => p.Id == PayeeId).FirstOrDefault().LpPayeeCode;
            ////get CommissionPeriod from Name
            //var CommissionPeriod = db.LCommissionPeriods.Where(p => p.Id == CommissionPeriodId).FirstOrDefault().LcpPeriodName;
            ////Get list of child Payees
            //var Query = "WITH parent AS (select LppParentPayeeId from LPayeeParents where LppParentPayeeId= {0} ), tree AS (SELECT x.LppParentPayeeId, x.LppPayeeId FROM LPayeeParents x INNER JOIN parent ON x.LppParentPayeeId = parent.LppParentPayeeId UNION ALL SELECT y.LppParentPayeeId, y.LppPayeeId  FROM LPayeeParents y INNER JOIN tree t ON y.LppParentPayeeId = t.LppPayeeId ) SELECT parent.LpPayeeCode   FROM tree t   inner join LPayees parent on t.LppParentPayeeId=parent.Id where parent.WFStatus='Completed' union select child.LpPayeeCode FROM tree t inner join LPayees child on t.LppPayeeId=child.Id where child.WFStatus='Completed'";
            //var ChildPayeeList = db.Database.SqlQuery<PayeeResultViewModel>(Query, PayeeId).ToList();
            //string ChildPayeeStr = "";
            //foreach (var ChildPayee in ChildPayeeList)
            //{
            //    ChildPayeeStr += "'" + ChildPayee.LpPayeeCode + "',";
            //}

            //Getting list of Batch Nos and concatenating into string with ','
            if (UserRole == "Payee" || UserRole == "Channel Manager")
            {
                if (CommPeriodList.Equals(""))
                {
                    CommPeriodList = "''";
                }
                var BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in (" + WFStatus + ") and WFCompanyId=" + CompanyId;
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.PortfolioList))
                {
                    /*If none is selected, 
                     Add user portfolios to portfolio matching during batch selection during the query to populate Calc grid for that period
                    */
                    var lusers = db.LUsers.Where(x => x.LuUserId == LoggedInUserId).FirstOrDefault();
                    model.PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == lusers.Id && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
                }
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Calc grid for that period*/
                if (model.PortfolioList.Equals(""))
                {
                    model.PortfolioList = "''";
                }
                if (CommPeriodList.Equals(""))
                {
                    CommPeriodList = "''";
                }
                var BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (select Id from LPortfolios where LpChannelId in (select LpChannelId from LPortfolios where Id in (" + model.PortfolioList + ") )) and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in (" + WFStatus + ") ";
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }

            ////var BatchQry = "select lb.LbBatchNumber from LBatches lb where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('" + WFStatus + "') and WFCompanyId="+CompanyId;
            //var BatchQry = "select lb.LbBatchNumber from LBatches lb where lb.LbCommissionPeriod in ('" + CommissionPeriod + "') AND lb.LbBatchType = 'CALC' AND WFStatus in ('" + WFStatus + "')";
            ////var BatchNumberList = db.Database.SqlQuery<int>(BatchQry);
            //// string BatchNumberStr = "";
            ////foreach (var BatchNumber in BatchNumberList)
            ////{
            ////    BatchNumberStr += BatchNumber + ",";
            ////}
            ////BatchNumberStr = BatchNumberStr.TrimEnd(',');
            ////if (BatchNumberStr.Equals(""))
            ////{
            ////    BatchNumberStr = "''";
            ////}
            //Depending upon Group Criteria provided, concatenate the Column into Group
            string PlaceHolder = "";
            if (Payeechecked)
            {
                PlaceHolder += ",C.XPayee";
            }
            //SS commented as it wiil be replaced with XCalc.XCommissionPeriod in future release
            //if (CommPeriodchecked)
            //{
            //    PlaceHolder += ",B.LbCommissionPeriod";
            //}
            if (BatchNo)
            {
                PlaceHolder += ",C.XBatchNumber";
            }
            if (Source)
            {
                PlaceHolder += ",C.XSource";
            }
            if (CommType)
            {
                PlaceHolder += ",C.XCommType";
            }
            if (MSISDN)
            {
                PlaceHolder += ",C.XSubscriberNumber";
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
            //if (MSISDN)
            //{
            //    PlaceHolder += ",C.XSubscriberNumber";
            //}
            //SS commented the below query as it was taking 1.5 min to execute with the join. Hence the join has been removed as for now.In the future Commission Period grouping will be added from XCalc
            //var QRY = "select sum(C.LcCommAmtExTax) LcCommAmtExTax " + PlaceHolder2 + " from lcalc C join LBatches B on C.LcSOSBatchNumber = B.LbBatchNumber where C.LcPayee in ("


            string newPlaceHolder = PlaceHolder2.TrimStart(',');

            var QRY = "select sum(C.XCommAmtExTax) as XCommAmtExTax " + PlaceHolder2 + ",ROW_NUMBER() OVER (ORDER BY " + newPlaceHolder + "  desc) as row from {Schema}.XCalc C  where C.XPayee in ("
                + ChildPayeeStr + PayeeCodeList + ") and C.XBatchNumber in (" + BatchNumberStr + ") and C.XPeriodCode =" + PeriodCode + " " + PlaceHolder;//Added Period Code in Where Clause in order to use the partition on XCalc table
                                                                                                                                                          // db.Database.CommandTimeout = 180;//SS is setting the command timeout to 3 minas this query is taking toolong to execute. Will removethe lline onceissue is resolved
            var result = Globals.GetQueryResultFromOpcoDB(CompanyId, QRY, CommissionPeriod);//db.Database.SqlQuery<LCalcViewModel>(QRY).ToList();
            return Ok(result.Rows.Count);

            //var qry = "select count(*) as RowCounts from {Schema}.XCalc where XPayee in("
            //         + ChildPayeeStr + PayeeCodeList + ") AND  XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ")";
            //var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, qry, CommissionPeriod);
            // int xx = db.Database.SqlQuery<int>(qry).FirstOrDefault();
            // return Ok(result.Rows[0].Field<int>("RowCounts"));
        }


        //Payee Calc Review Screen - Prelim and Approved Tabs
        //method added by SG for PayeeCalc
        [HttpPost]
        public IHttpActionResult GetLCalcByPayeeCommissionPeriodCompanyId(PayeeCalcViewModel model, int CompanyId, string CommissionPeriodIdList, string Status, int PageSize, int PageNumber, string sortdatafield, string sortorder, string FilterQuery, string UserName, string Workflow, string LoggedInRoleName, int LoggedinLUserId, string LoggedInUserId)
        {
            //in case of Prelim Status, InProgress status records also need to be included.
            var newStatus = Status;
            /*Calc Review Prelim tab. Currently we pick batches which are Prelim AND InProgress. 
             * It should pick only ‘Prelim’. Remove InProgress. See if we have put this in where clause at other places too (like summary tab etc)*/
            //    if (Status.Equals("Prelim"))
            //    {
            //        newStatus = "'"+Status + "','InProgress'";
            //    }
            //else
            //{
            if (newStatus == "Completed")
            {
                newStatus = "'Completed','Paid'";
            }
            else
            {
                newStatus = "'" + newStatus + "'";
            }
            // }
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
                {
                    var PayeeCodesList = ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LpPayeeCode")).ToList();
                    foreach (var payeecode in PayeeCodesList)
                    {
                        PayeeCodeList += "'" + payeecode + "',";
                    }
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

            //commenting as new code for multi Payee is there.
            //var PayeeCode = db.LPayees.Where(p => p.Id == PayeeId).FirstOrDefault().LpPayeeCode;
            //    //get CommissionPeriod from Name
            //    var CommissionPeriod = db.LCommissionPeriods.Where(p => p.Id == CommissionPeriodId).FirstOrDefault().LcpPeriodName;
            //    //Get list of child Payees
            //    var Query = "WITH parent AS (select LppParentPayeeId from LPayeeParents where LppParentPayeeId={0}), tree AS (SELECT x.LppParentPayeeId, x.LppPayeeId FROM LPayeeParents x INNER JOIN parent ON x.LppParentPayeeId = parent.LppParentPayeeId UNION ALL SELECT y.LppParentPayeeId, y.LppPayeeId  FROM LPayeeParents y INNER JOIN tree t ON y.LppParentPayeeId = t.LppPayeeId ) SELECT parent.LpPayeeCode   FROM tree t   inner join LPayees parent on t.LppParentPayeeId=parent.Id where parent.WFStatus='Completed' union select child.LpPayeeCode FROM tree t inner join LPayees child on t.LppPayeeId=child.Id where child.WFStatus='Completed'";
            //    var ChildPayeeList = db.Database.SqlQuery<PayeeResultViewModel>(Query, PayeeId).ToList();
            //    string ChildPayeeStr = "";
            //    foreach(var ChildPayee in ChildPayeeList)
            //    {
            //        ChildPayeeStr += "'"+ ChildPayee.LpPayeeCode + "',";
            //    }

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
            //Getting list of Batch Nos
            var BatchQry = string.Empty;
            DataTable BatchNumberDT = new DataTable();
            string BatchNumberStr = "";
            if (LoggedInRoleName == "Payee" || LoggedInRoleName == "Channel Manager")
            {
                // NOTE: Payee and CM can see Calc data across all channel/schemes as long as the Calc has the PayeeCode from their dropdown (direct match or child payee)
                //BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') ";
                //QUERY Corrected as per logic
                //BatchQry = "select distinct XBatchNumber from {Schema}.XCalc where XPayee in("
                //         + ChildPayeeStr + PayeeCodeList + ")  and XPeriodCode=" + PeriodCode;
                //BatchNumberDT = Globals.GetQueryResultFromOpcoDB(CompanyId, BatchQry, CommissionPeriod);
                //foreach (DataRow BatchNumber in BatchNumberDT.Rows)
                //{
                //    BatchNumberStr += BatchNumber + ",";
                //}
                //BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (CommPeriodList.Equals(""))
                {
                    CommPeriodList = "''";
                }
                //As XSchemaGR is taking toolong to respond using LBatches here And will use XSchem<opco> only in AcceptAll and AttachAll
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in (" + newStatus + ") and WFCompanyId=" + CompanyId;
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.PortfolioList))
                {
                    /*If none is selected, 
                     Add user portfolios to portfolio matching during batch selection during the query to populate Calc grid for that period
                    */
                    model.PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == LoggedinLUserId && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
                }
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Calc grid for that period*/
                if (model.PortfolioList.Equals(""))
                {
                    model.PortfolioList = "''";
                }
                if (CommPeriodList.Equals(""))
                {
                    CommPeriodList = "''";
                }
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (select Id from LPortfolios where LpChannelId in (select LpChannelId from LPortfolios where Id in (" + model.PortfolioList + ") )) and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in (" + newStatus + ") ";
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }

            //As per JS directions we will save column Name in LCompanySpecificColumnsas ColumnName~DataType
            var CompanySpecificColumnsList = db.LCompanySpecificColumns.Where(p => p.LcscCompanyId == CompanyId).Where(p => p.LcscTableName == "XCalc").Select(p => p.LcscColumnName).ToList();
            for (var i = 0; i < CompanySpecificColumnsList.Count(); i++)
            {
                CompanySpecificColumnsList[i] = CompanySpecificColumnsList.ElementAt(i);
            }
            // string ColumnList = " dbo.FnGetUserName(LcAcceptedById) as LcAcceptedById,LcIsPayeeAccepted,LcPayeeAttachmentId,Id, LcOpCoCode, LcSource, LcAdjustmenCode, LcSOSBatchNumber, LcAlteryxTransactionNumber, LcPrimaryChannel, LcPayee, LcParentPayee, LcOrderDate, LcConnectionDate, LcTerminationDate, LcSubscriberNumber, LcBAN, LcActivityType, LcPlanDescrition, LcProductCode, LcUpgradeCode, LcIMEI, LcDevieCode, LcDeviceType, LcCommType, LcContractDuration, LcContractId, LcCommAmtExTax, LcTax, LcCommAmtIncTax, LcComments";
            if (PayeeCodeList.Equals(""))
            {
                PayeeCodeList = "''";
            }
            string SortQry = "";
            if (!string.IsNullOrEmpty(sortorder))//code for server side filtering
            {
                if (sortorder == "asc")
                {
                    SortQry = " order by " + sortdatafield + " ";
                }
                else
                {
                    SortQry = " order by " + sortdatafield + " desc ";
                }
            }
            else
            {
                SortQry = " ORDER BY XAlteryxTransactionNumber ";
            }
            if (BatchNumberStr.Equals(""))
            {
                BatchNumberStr = "''";
            }
            Boolean CanRaiseClaims = false;
            string WheatherAllowedOrNot = string.Empty;
            if (LoggedInRoleName == "Payee")
            {
                //SG100620202 - WFSTatus check added
                CanRaiseClaims = db.LPayees.Where(x => x.LpUserId == LoggedInUserId).Where(x=>x.WFStatus=="Completed").Select(x => x.LpCanRaiseClaims).FirstOrDefault();
                if(CanRaiseClaims == true)
                {
                    RWorkFlow RW = db.RWorkFlows.Where(x => x.RwfName == "Claims").FirstOrDefault();
                    var CompanyCodeData = db.GCompanies.Where(x => x.Id == CompanyId).Select(x => x.GcCode).FirstOrDefault();
                    var roleid = db.AspNetRoles.Where(x => x.CompanyCode == CompanyCodeData && x.Name == LoggedInRoleName).Select(x => x.Id).FirstOrDefault();
                    WheatherAllowedOrNot = db.Database.SqlQuery<string>("select dbo.FnGetCreateClaimPermission({0},{1},{2})", roleid, RW.Id, CompanyId).FirstOrDefault(); //Sachin
                                                                                                                                                                          // var data = db.LWorkFlowConfigs.Where(x => x.LwfcRoleId == roleid && x.LwfcCompanyId == CompanyId & x.LwfcWorkFlowId== RW.Id).Select(x => x.LwfcCanCreate).FirstOrDefault();
                    WheatherAllowedOrNot = "'" + WheatherAllowedOrNot + "'" + " as ClaimPermission";
                }else
                {
                                                                                                                                                     // var data = db.LWorkFlowConfigs.Where(x => x.LwfcRoleId == roleid && x.LwfcCompanyId == CompanyId & x.LwfcWorkFlowId== RW.Id).Select(x => x.LwfcCanCreate).FirstOrDefault();
                    WheatherAllowedOrNot = "'False' as ClaimPermission";
                }

            }else
            {
                RWorkFlow RW = db.RWorkFlows.Where(x => x.RwfName == "Claims").FirstOrDefault();
                var CompanyCodeData = db.GCompanies.Where(x => x.Id == CompanyId).Select(x => x.GcCode).FirstOrDefault();
                var roleid = db.AspNetRoles.Where(x => x.CompanyCode == CompanyCodeData && x.Name == LoggedInRoleName).Select(x => x.Id).FirstOrDefault();
                WheatherAllowedOrNot = db.Database.SqlQuery<string>("select dbo.FnGetCreateClaimPermission({0},{1},{2})", roleid, RW.Id, CompanyId).FirstOrDefault(); //Sachin
                                                                                                                                                                      // var data = db.LWorkFlowConfigs.Where(x => x.LwfcRoleId == roleid && x.LwfcCompanyId == CompanyId & x.LwfcWorkFlowId== RW.Id).Select(x => x.LwfcCanCreate).FirstOrDefault();
                WheatherAllowedOrNot = "'" + WheatherAllowedOrNot + "'" + " as ClaimPermission";

            }




            string ColumnList = String.Join(",", CompanySpecificColumnsList);
            var qry = "Select *," + WheatherAllowedOrNot + " From (select " + ColumnList + ",ROW_NUMBER() OVER (" + SortQry + ") as row from {Schema}.XCalc where XPayee in("
                          + ChildPayeeStr + PayeeCodeList + ") AND  XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ") ";
            //Apply filterations
            qry = qry + FilterQuery + " ) a";

            //apply paging
            qry += "  Where row > " + PageNumber * PageSize + " And row <= " + (PageNumber + 1) * PageSize;

            var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, qry, CommissionPeriod);//db.Database.SqlQuery<LCalcViewModel>(qry).ToList();
            return Ok(xx);

        }

        //method to download PayeeCalc Grid
        [HttpGet]
        public IHttpActionResult DownloadLCalcByPayeeCommissionPeriodCompanyId(int LoggedinLUserId, string LoggedInRoleName, string CompanyCode, string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, string PortfolioList, string UserName, string Workflow, string LoggedInUserId)
        {
            /*Calc Review Prelim tab. Currently we pick batches which are Prelim AND InProgress. 
            * It should pick only ‘Prelim’. Remove InProgress. See if we have put this in where clause at other places too (like summary tab etc)*/
            //in case of Prelim Status, InProgress status records also need to be included.
            var newStatus = Status;
            //if (Status.Equals("Prelim"))
            //{
            //    newStatus = "'" + Status + "','InProgress'";
            //}
            //else
            //{
            newStatus = "'" + newStatus + "'";
            if (Status.Equals("Completed"))
            {
                newStatus = "'" + Status + "','Paid'";
            }
            //newStatus = "'" + newStatus + "'";
            //}
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
                {
                    var PayeeCodesList = ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LpPayeeCode")).ToList();
                    foreach (var payeecode in PayeeCodesList)
                    {
                        PayeeCodeList += "'" + payeecode + "',";
                    }
                }
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

            //Getting list of Batch Nos
            var BatchQry = string.Empty;
            DataTable BatchNumberDT = new DataTable();
            string BatchNumberStr = "";
            if (LoggedInRoleName == "Payee" || LoggedInRoleName == "Channel Manager")
            {
                // NOTE: Payee and CM can see Calc data across all channel/schemes as long as the Calc has the PayeeCode from their dropdown (direct match or child payee)
                //BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') ";
                //QUERY Corrected as per logic
                //BatchQry = "select distinct XBatchNumber from {Schema}.XCalc where XPayee in("
                //         + ChildPayeeStr + PayeeCodeList + ")  and XPeriodCode=" + PeriodCode;
                //BatchNumberDT = Globals.GetQueryResultFromOpcoDB(CompanyId, BatchQry, CommissionPeriod);
                //foreach (DataRow BatchNumber in BatchNumberDT.Rows)
                //{
                //    BatchNumberStr += BatchNumber + ",";
                //}
                //BatchNumberStr = BatchNumberStr.TrimEnd(',');
                //if (BatchNumberStr.Equals(""))
                //{
                //    BatchNumberStr = "''";
                //}
                //As XSchemaGR is taking toolong to respond using LBatches here And will use XSchem<opco> only in AcceptAll and AttachAll
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') and WFCompanyId=" + CompanyId;
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(PortfolioList))
                {
                    /*If none is selected, 
                     Add user portfolios to portfolio matching during batch selection during the query to populate Calc grid for that period
                    */
                    PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == LoggedinLUserId && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
                }
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Calc grid for that period*/
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (select Id from LPortfolios where LpChannelId in (select LpChannelId from LPortfolios where Id in (" + PortfolioList + ") )) and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in (" + newStatus + ") ";
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }

            //var qry = "select LcOpCoCode as OpCoCode,LcSource as Source,LcAdjustmenCode as AdjustmenCode,LcSOSBatchNumber as SOSBatchNumber,LcAlteryxTransactionNumber as AlteryxTransactionNumber,LcPrimaryChannel as PrimaryChannel,LcPayee as Payee,LcParentPayee as ParentPayee,LcOrderDate as OrderDate,LcConnectionDate as ConnectionDate,LcTerminationDate as TerminationDate,LcSubscriberNumber as SubscriberNumber,LcBAN as BAN,LcActivityType as ActivityType,LcPlanDescrition as PlanDescrition,LcProductCode as ProductCode,LcUpgradeCode as UpgradeCode,LcIMEI as IMEI,LcDevieCode as DevieCode,LcDeviceType as DeviceType,LcCommType as CommType,LcContractDuration as ContractDuration,LcContractId as Contract,LcCommAmtExTax as CommAmtExTax,LcTax as Tax,LcCommAmtIncTax as CommAmtIncTax,LcComments as Comments,LcIsPayeeAccepted as IsPayeeAccepted,LcAcceptedDateTime as AcceptedDateTime,ROW_NUMBER() OVER (ORDER BY Id desc) as row from LCalc where LcPayee in("
            //              + ChildPayeeStr + PayeeCodeList + ") AND LcSOSBatchNumber in (" + BatchNumberStr + ")";

            var FileName = "ExportPayeeCalcFile.zip";

            //As per JS directions we will save column Name in LCompanySpecificColumnsas ColumnName~DataType
            var CompanySpecificColumnsList = db.LCompanySpecificColumns.Where(p => p.LcscCompanyId == CompanyId).Where(p => p.LcscTableName == "XCalc").Select(p => new CompanySpecificColumnViewModel { ColumnName = p.LcscColumnName, LcscLabel = p.LcscLabel }).ToList();
            for (var i = 0; i < CompanySpecificColumnsList.Count(); i++)
            {
                CompanySpecificColumnsList[i].LcscColumnName = CompanySpecificColumnsList.ElementAt(i).ColumnName + " as [" + ((string.IsNullOrEmpty(CompanySpecificColumnsList.ElementAt(i).LcscLabel)) ? CompanySpecificColumnsList.ElementAt(i).ColumnName.Replace("X", "") : CompanySpecificColumnsList.ElementAt(i).LcscLabel) + "]";
            }
            string ColumnList = String.Join(",", CompanySpecificColumnsList.Select(p => p.LcscColumnName));
            string Qry = "select " + ColumnList + " from {Schema}.XCalc where  XPayee in (" + ChildPayeeStr + PayeeCodeList + ") and XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ")";

            //method to get query result and save it as an excel file in bucket
            var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, CommissionPeriod);//db.Database.SqlQuery<LCalcViewModel>(Qry).ToList();

            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            Globals.ExportZipFromDataTable(null, CompanyCode, UserName, FileName, xx);
            return Ok(FileName);
        }


        //get Counts for the PayeeCalc Grid--Prelim and Approved//
        [HttpPost]
        public IHttpActionResult GetLCalcByPayeeCommissionPeriodCompanyIdCounts(PayeeCalcViewModel model, int CompanyId, string CommissionPeriodIdList, string Status, string UserName, string Workflow, string LoggedInRoleName, int LoggedinLUserId, string LoggedInUserId)
        {
            /*Calc Review Prelim tab. Currently we pick batches which are Prelim AND InProgress. 
            * It should pick only ‘Prelim’. Remove InProgress. See if we have put this in where clause at other places too (like summary tab etc)*/
            //in case of Prelim Status, InProgress status records also need to be included.
            var newStatus = Status;
            //if (Status.Equals("Prelim"))
            //{
            //    newStatus ="'"+ Status + "','InProgress'";
            //}
            //else
            //{
            // newStatus = "'" + newStatus + "'"; ????
            if (newStatus == "Completed")
            {
                newStatus = "'Completed','Paid'";
            }
            else
            {
                newStatus = "'" + newStatus + "'";
            }
            // }
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
                {
                    var PayeeCodesList = ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LpPayeeCode")).ToList();
                    foreach (var payeecode in PayeeCodesList)
                    {
                        PayeeCodeList += "'" + payeecode + "',";
                    }
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
            //Getting list of Batch Nos
            var BatchQry = string.Empty;
            DataTable BatchNumberDT = new DataTable();
            string BatchNumberStr = "";
            if (LoggedInRoleName == "Payee" || LoggedInRoleName == "Channel Manager")
            {
                // NOTE: Payee and CM can see Calc data across all channel/schemes as long as the Calc has the PayeeCode from their dropdown (direct match or child payee)
                //BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') ";
                //QUERY Corrected as per logic
                //BatchQry = "select distinct XBatchNumber from {Schema}.XCalc where XPayee in("
                //         + ChildPayeeStr + PayeeCodeList + ")  and XPeriodCode=" + PeriodCode;
                //BatchNumberDT = Globals.GetQueryResultFromOpcoDB(CompanyId, BatchQry, CommissionPeriod);
                //foreach (DataRow BatchNumber in BatchNumberDT.Rows)
                //{
                //    BatchNumberStr += BatchNumber + ",";
                //}
                //BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (CommPeriodList.Equals(""))
                {
                    CommPeriodList = "''";
                }
                //As XSchemaGR is taking toolong to respond using LBatches here And will use XSchem<opco> only in AcceptAll and AttachAll
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in (" + newStatus + ") and WFCompanyId=" + CompanyId;
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.PortfolioList))
                {
                    /*If none is selected, 
                     Add user portfolios to portfolio matching during batch selection during the query to populate Calc grid for that period
                    */
                    model.PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == LoggedinLUserId && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
                }
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Calc grid for that period*/
                if (model.PortfolioList.Equals(""))
                {
                    model.PortfolioList = "''";
                }
                if (CommPeriodList.Equals(""))
                {
                    CommPeriodList = "''";
                }
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (select Id from LPortfolios where LpChannelId in (select LpChannelId from LPortfolios where Id in (" + model.PortfolioList + ") )) and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in (" + newStatus + ") ";
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }


            //var qry = "select count(*) from LCalc where LcPayee in("
            //           + PayeeStr + "'" + PayeeCode + "') AND LcSOSBatchNumber in (select lb.LbBatchNumber from LBatches lb where lb.LbCommissionPeriod = '"
            //           + CommissionPeriod + "' AND lb.LbBatchType = 'CALC' AND WFStatus = '')";
            // var qry = "select count(*) from LCalc where LcPayee in("
            //            + ChildPayeeStr + PayeeCodeList + ") AND LcSOSBatchNumber in (select lb.LbBatchNumber from LBatches lb where lb.LbCommissionPeriod in ("
            //           + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus = '{0}')";
            if (BatchNumberStr.Equals(""))
            {
                BatchNumberStr = "''";
            }
            var qry = "select count(*) as RowCounts from {Schema}.XCalc where XPayee in("
                       + ChildPayeeStr + PayeeCodeList + ") AND  XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ")";
            var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, qry, CommissionPeriod);
            // int xx = db.Database.SqlQuery<int>(qry).FirstOrDefault();
            return Ok(xx.Rows[0].Field<int>("RowCounts"));
        }



        //Download the data of zoom grid in LCalc
        [HttpGet]
        public IHttpActionResult DownloadLCalc(int SOSBatchNumber, string CompanyCode, string UserName, string Workflow, int CompanyId, string FilterQuery)
        {
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            var PeriodCode = 0;
            if (BatchDetails.LbPeriodCode.HasValue)
                PeriodCode = BatchDetails.LbPeriodCode.Value;
            var FilesPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserName + "/forzip";
             string CalcFileName = "ExportCalcFile_*.*";
           // string CalcFileName = BatchDetails.LbCommissionPeriod + "_" + BatchDetails.LbBatchName+"*.*";
            if (System.IO.Directory.Exists(FilesPath))
            {
                try
                {
                    var CalcFileList = Directory.GetFiles(FilesPath, CalcFileName, SearchOption.AllDirectories);
                    foreach (var CalcFile in CalcFileList)
                    {
                        if (System.IO.File.Exists(CalcFile))//delete already existing file
                            System.IO.File.Delete(CalcFile);
                    }
                }
                catch
                { //SS:-do nothing if there is an issue in searching or deletion of files
                }
            }
            //var Qry = "Select LcOpCoCode as OpCoCode, LcSource as Source, LcAdjustmenCode as AdjustmenCode, LcSOSBatchNumber as SOSBatchNumber, LcAlteryxTransactionNumber as AlteryxTransactionNumber,LcPrimaryChannel as PrimaryChannel, LcPayee as Payee, LcParentPayee as ParentPayee, LcOrderDate as OrderDate, LcConnectionDate as ConnectionDate, LcTerminationDate as TerminationDate, LcSubscriberNumber as SubscriberNumber, LcBAN as BAN, LcActivityType as ActivityType, LcPlanDescrition as PlanDescrition, LcProductCode as ProductCode, LcUpgradeCode as UpgradeCode, LcIMEI as IMEI, LcDevieCode as DevieCode, LcDeviceType as DeviceType, LcCommType as CommType, LcContractDuration as ContractDuration, LcContractId as ContractId, LcCommAmtExTax as CommAmtExTax, LcTax as Tax, LcCommAmtIncTax as CommAmtIncTax, LcComments as Comments, ROW_NUMBER() OVER (ORDER BY LcAlteryxTransactionNumber) as row FROM LCalc where LcSOSBatchNumber=" + SOSBatchNumber;
            //As per JS directions we will save column Name in LCompanySpecificColumnsas ColumnName~DataType
            var CompanySpecificColumnsList = db.LCompanySpecificColumns.Where(p => p.LcscCompanyId == CompanyId).Where(p => p.LcscTableName == "XCalc" & p.LcscDisplayOnForm == true).Select(p => new CompanySpecificColumnViewModel { ColumnName = p.LcscColumnName, LcscLabel = p.LcscLabel }).ToList();
            for (var i = 0; i < CompanySpecificColumnsList.Count(); i++)
            {
                CompanySpecificColumnsList[i].LcscColumnName = CompanySpecificColumnsList.ElementAt(i).ColumnName + " as [" + ((string.IsNullOrEmpty(CompanySpecificColumnsList.ElementAt(i).LcscLabel)) ? CompanySpecificColumnsList.ElementAt(i).ColumnName.Replace("X", "") : CompanySpecificColumnsList.ElementAt(i).LcscLabel) + "]";
            }
            string ColumnList = String.Join(",", CompanySpecificColumnsList.Select(p => p.LcscColumnName));
            if (ColumnList != "")
            {
                string Qry = "select " + ColumnList + " from {Schema}.XCalc where XBatchNumber=" + SOSBatchNumber + " and XPeriodCode=" + PeriodCode + FilterQuery;

                //method to get query result and save it as an excel file in S drive
                //var FileName = "ExportCalcFile_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".zip";
                //var FileNamewithoutextention = "ExportCalcFile_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss");

                var FileName = "ExportCalcFile_"+BatchDetails.LbCommissionPeriod + "_" + BatchDetails.LbBatchName +"_"+DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".zip";
                var FileNamewithoutextention = "ExportCalcFile_" + BatchDetails.LbCommissionPeriod + "_" + BatchDetails.LbBatchName + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss");

                if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
                {
                    Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LCalc", "DownloadLCalc", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
                }
                else
                {
                    var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, BatchDetails.LbCommissionPeriod);//db.Database.SqlQuery<LCalcViewModel>(Qry).ToList();
                    Globals.CreateExcelFromDataTable(xx, FileName, "", CompanyCode, UserName, FileNamewithoutextention);
                    // Globals.ExportZipFromDataTable(null, CompanyCode, UserName, FileName, xx);
                }

                return Ok(FileName);
            }
            else
            {
                return Ok("No Mapping columns found");
            }
        }
        [HttpGet]
        public IHttpActionResult DownloadLCalcData(int SOSBatchNumber, string CompanyCode, string UserName, string Workflow, int CompanyId, string FilterQuery)
        {
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            var PeriodCode = 0;
            if(BatchDetails != null) {
                if (BatchDetails.LbPeriodCode.HasValue)
                    PeriodCode = BatchDetails.LbPeriodCode.Value;
            }
            var FilesPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserName + "/forzip";
            string CalcFileName = "ExportCalcFile_*.*";
            if (System.IO.Directory.Exists(FilesPath))
            {
                try
                {
                    var CalcFileList = Directory.GetFiles(FilesPath, CalcFileName, SearchOption.AllDirectories);
                    foreach (var CalcFile in CalcFileList)
                    {
                        if (System.IO.File.Exists(CalcFile))//delete already existing file
                            System.IO.File.Delete(CalcFile);
                    }
                }
                catch
                { //SS:-do nothing if there is an issue in searching or deletion of files
                }
            }
            //var Qry = "Select LcOpCoCode as OpCoCode, LcSource as Source, LcAdjustmenCode as AdjustmenCode, LcSOSBatchNumber as SOSBatchNumber, LcAlteryxTransactionNumber as AlteryxTransactionNumber,LcPrimaryChannel as PrimaryChannel, LcPayee as Payee, LcParentPayee as ParentPayee, LcOrderDate as OrderDate, LcConnectionDate as ConnectionDate, LcTerminationDate as TerminationDate, LcSubscriberNumber as SubscriberNumber, LcBAN as BAN, LcActivityType as ActivityType, LcPlanDescrition as PlanDescrition, LcProductCode as ProductCode, LcUpgradeCode as UpgradeCode, LcIMEI as IMEI, LcDevieCode as DevieCode, LcDeviceType as DeviceType, LcCommType as CommType, LcContractDuration as ContractDuration, LcContractId as ContractId, LcCommAmtExTax as CommAmtExTax, LcTax as Tax, LcCommAmtIncTax as CommAmtIncTax, LcComments as Comments, ROW_NUMBER() OVER (ORDER BY LcAlteryxTransactionNumber) as row FROM LCalc where LcSOSBatchNumber=" + SOSBatchNumber;
            //As per JS directions we will save column Name in LCompanySpecificColumnsas ColumnName~DataType
            var CompanySpecificColumnsList = db.LCompanySpecificColumns.Where(p => p.LcscCompanyId == CompanyId).Where(p => p.LcscTableName == "XCalc" & p.LcscDisplayOnForm == true).Select(p => new CompanySpecificColumnViewModel { ColumnName = p.LcscColumnName, LcscLabel = p.LcscLabel }).ToList();
            for (var i = 0; i < CompanySpecificColumnsList.Count(); i++)
            {
                CompanySpecificColumnsList[i].LcscColumnName = CompanySpecificColumnsList.ElementAt(i).ColumnName + " as [" + ((string.IsNullOrEmpty(CompanySpecificColumnsList.ElementAt(i).LcscLabel)) ? CompanySpecificColumnsList.ElementAt(i).ColumnName.Replace("X", "") : CompanySpecificColumnsList.ElementAt(i).LcscLabel) + "]";
            }
            string ColumnList = String.Join(",", CompanySpecificColumnsList.Select(p => p.LcscColumnName));
            string Qry = "select " + ColumnList + " from {Schema}.XCalc where XBatchNumber=" + SOSBatchNumber + " and XPeriodCode=" + PeriodCode + FilterQuery;

            //method to get query result and save it as an excel file in S drive
            var FileName = "ExportCalcFile_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".zip";
            var FileNamewithoutextention = "ExportCalcFile_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss");
            if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
            {
                Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LCalc", "DownloadLCalc", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
            }
            else
            {
                var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, BatchDetails.LbCommissionPeriod);//db.Database.SqlQuery<LCalcViewModel>(Qry).ToList();

                Globals.CreateExcelFromDataTable(xx, FileName, "", CompanyCode, UserName, FileNamewithoutextention);
                //Globals.ExportZipFromDataTable(null, CompanyCode, UserName, FileName, xx);
            }

            //var CfileLocation = ConfigurationManager.AppSettings["CalcDocumentPath"] + "/"+CompanyCode + "/" + LoggedInUserName + "/ExportCalcFile.csv";
            //var FilesPath = ConfigurationManager.AppSettings["CalcDocumentPath"];
            //if (System.IO.File.Exists(CfileLocation))//delete already existing file
            //    System.IO.File.Delete(CfileLocation);

            //if (xx != null)
            //{


            //if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["CalcDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName))//create directory if not present
            //{
            //    System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["CalcDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName);
            //}
            //using (var CTextWriter = new StreamWriter(CfileLocation))
            //using (var Csv = new CsvWriter(CTextWriter))
            //{
            //    Csv.WriteRecords(xx);
            //    //The below lines of code converts the data returned from api to a datatable
            //    //    var tb = new DataTable(typeof(DownloadLCalcViewModel).Name);

            //    //    PropertyInfo[] props = typeof(DownloadLCalcViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //    //    foreach (var prop in props)
            //    //    {
            //    //        // var displayName=PayeeModel.GetDisplayName()

            //    //        var PropName = prop.Name.Replace("Lc", "");//make heading of column without suffix
            //    //        tb.Columns.Add(PropName);

            //    //    }

            //    //    foreach (var item in xx)
            //    //    {
            //    //        var values = new object[props.Length];
            //    //        for (var i = 0; i < props.Length; i++)
            //    //        {
            //    //            //if (!props[i].Name.Equals("Id"))
            //    //            //{
            //    //            values[i] = props[i].GetValue(item, null);
            //    //            //}
            //    //        }

            //    //        tb.Rows.Add(values);
            //    //    }


            //    //    foreach (DataColumn column in tb.Columns)
            //    //    {
            //    //        Csv.WriteField(column.ColumnName);
            //    //    }
            //    //    Csv.NextRecord();

            //    //    foreach (DataRow row in tb.Rows)
            //    //    {
            //    //        for (var i = 0; i < tb.Columns.Count; i++)
            //    //        {
            //    //            Csv.WriteField(row[i]);
            //    //        }
            //    //        Csv.NextRecord();
            //    //    }
            //    }
            //}
            return Ok(FileName);
        }

        //to get counts in calc reports
        //This method is using ADO.NET 
        public IHttpActionResult GetLCalForReportCounts(string Source, string ProductCode, string CommissionType, string ActivityType, int PageNumber, int PageSize, string CompanyCode, string PrimaryChannel, string PayeeList, string MinIMEI, string MaxIMEI, string MinBAN, string MaxBAN, string MinContractDuration, string MaxContractDuration, string MinCommissionAmount, string MaxCommissionAmount, string MinOrderDate, string MaxOrderDate, string MinConnectionDate, string MaxConnectionDate, string MinTerminationDate, string MaxTerminationDate, string UserName, string Workflow, string MinSubscriberNumber, string MaxSubscriberNumber, string CommissionPeriod)//(GetLCalcForReports LCalcReports)//
        {
            try
            {
                //using (var ctx = new SOSEDMV10Entities())
                //{
                //Nullable<DateTime> FromOrderDate = DateTime.ParseExact(MinOrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> ToOrderDate = DateTime.ParseExact(MaxOrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> FromConnectionDate = DateTime.ParseExact(MinConnectionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> ToConnectionDate = DateTime.ParseExact(MaxConnectionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> FromTerminationDate = DateTime.ParseExact(MinTerminationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> ToTerminationDate = DateTime.ParseExact(MaxTerminationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                // string CurrentDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");

                //var LCalcReports = new GetLCalcForReports();
                //    LCalcReports.PageNumber = PageNumber;
                //    LCalcReports.PageSize = PageSize;
                //    LCalcReports.CompanyCode = CompanyCode;
                //    LCalcReports.PrimaryChannel = string.IsNullOrEmpty(PrimaryChannel) ? (object)System.DBNull.Value : (object)PrimaryChannel;
                //    LCalcReports.PayeeList = string.IsNullOrEmpty(PayeeList) ? System.DBNull.Value.ToString() : PayeeList;
                //    LCalcReports.MinIMEI = string.IsNullOrEmpty(MinIMEI) ? System.DBNull.Value.ToString() : MinIMEI;
                //    LCalcReports.MaxIMEI = string.IsNullOrEmpty(MaxIMEI) ? System.DBNull.Value.ToString() : MaxIMEI;
                //    LCalcReports.MinBAN = string.IsNullOrEmpty(MinBAN) ? System.DBNull.Value.ToString() : MinBAN;
                //    LCalcReports.MaxBAN = string.IsNullOrEmpty(MaxBAN) ? System.DBNull.Value.ToString() : MaxBAN;
                //    LCalcReports.MinSubscriberNumber = string.IsNullOrEmpty(MinSubscriberNumber) ? System.DBNull.Value.ToString() : MinSubscriberNumber;
                //    LCalcReports.MaxSubscriberNumber = string.IsNullOrEmpty(MaxSubscriberNumber) ? System.DBNull.Value.ToString() : MaxSubscriberNumber;
                //    LCalcReports.MinContractDuration = string.IsNullOrEmpty(MinContractDuration) ? System.DBNull.Value.ToString() : MinContractDuration;
                //    LCalcReports.MaxContractDuration = string.IsNullOrEmpty(MaxContractDuration) ? System.DBNull.Value.ToString() : MaxContractDuration;
                //    LCalcReports.MinCommissionAmount = string.IsNullOrEmpty(MinCommissionAmount) ? System.DBNull.Value.ToString() : MinCommissionAmount;
                //    LCalcReports.MaxCommissionAmount = string.IsNullOrEmpty(MaxCommissionAmount) ? System.DBNull.Value.ToString() : MaxCommissionAmount;
                //LCalcReports.FromOrderDate = (MinOrderDate == CurrentDate) ? System.DBNull.Value.ToString() : MinOrderDate;
                //LCalcReports.ToOrderDate = (MaxOrderDate == CurrentDate) ? System.DBNull.Value.ToString() : MaxOrderDate;
                //LCalcReports.FromConnectionDate = (MinConnectionDate == CurrentDate) ? System.DBNull.Value.ToString() : MinConnectionDate;
                //LCalcReports.ToConnectionDate = (MaxConnectionDate == CurrentDate) ? System.DBNull.Value.ToString() : MaxConnectionDate;
                //LCalcReports.FromTerminationDate = (MinTerminationDate == CurrentDate) ? System.DBNull.Value.ToString() : MinTerminationDate;
                //LCalcReports.ToTerminationDate = (MaxTerminationDate == CurrentDate) ? System.DBNull.Value.ToString() : MaxTerminationDate;
                //LCalcReports.ActivityType = string.IsNullOrEmpty(ActivityType) ? System.DBNull.Value.ToString() : ActivityType;
                //    LCalcReports.ProductCode = string.IsNullOrEmpty(ProductCode) ? System.DBNull.Value.ToString() : ProductCode;
                //    LCalcReports.CommissionType = string.IsNullOrEmpty(CommissionType) ? System.DBNull.Value.ToString() : CommissionType;

                //var xx = ctx.GetCalcReport(LCalcReports.Source, LCalcReports.PrimaryChannel, LCalcReports.PageSize, LCalcReports.PageNumber, LCalcReports.PayeeList, 
                //    LCalcReports.MinSubscriberNumber, LCalcReports.MaxSubscriberNumber, LCalcReports.MinBAN, LCalcReports.MaxBAN,
                //    LCalcReports.MinIMEI, LCalcReports.MaxIMEI, LCalcReports.ActivityType, LCalcReports.ProductCode, LCalcReports.CommissionType, 
                //    LCalcReports.MinContractDuration, LCalcReports.MaxContractDuration,
                //      LCalcReports.MinCommissionAmount, LCalcReports.MaxCommissionAmount, LCalcReports.CompanyCode,
                //     LCalcReports.FromOrderDate, LCalcReports.ToOrderDate, LCalcReports.FromConnectionDate, LCalcReports.ToConnectionDate,
                //     LCalcReports.FromTerminationDate, LCalcReports.ToTerminationDate,CommissionPeriod
                //    ).ToList();

                //ADO .Net Part starts here as we were not able to include it in EF
                string strQuery = "Exec GetCalcReport @Source, @PrimaryChannel, @PageSize, @PageNumber, @PayeeList,@MinSubscriberNumber, @MaxSubscriberNumber,@MinBAN, @MaxBAN,@MinIMEI, @MaxIMEI, @ActivityType, @ProductCode, @CommissionType,@MinContractDuration,@MaxContractDuration,@MinCommissionAmount, @MaxCommissionAmount, @CompanyCode,@MinOrderDate, @MaxOrderDate, @MinConnectionDate, @MaxConnectionDate, @MinTerminationDate, @MaxTerminationDate,@CommissionPeriod ";
                SqlCommand cmd = new SqlCommand(strQuery);
                cmd.Parameters.AddWithValue("@Source", string.IsNullOrEmpty(Source) ? (object)System.DBNull.Value : (object)Source);
                cmd.Parameters.AddWithValue("@PrimaryChannel", string.IsNullOrEmpty(PrimaryChannel) ? (object)System.DBNull.Value : (object)PrimaryChannel);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                cmd.Parameters.AddWithValue("@PayeeList", string.IsNullOrEmpty(PayeeList) ? (object)System.DBNull.Value : (object)PayeeList);
                cmd.Parameters.AddWithValue("@MinSubscriberNumber", string.IsNullOrEmpty(MinSubscriberNumber) ? (object)System.DBNull.Value : (object)MinSubscriberNumber);
                cmd.Parameters.AddWithValue("@MaxSubscriberNumber", string.IsNullOrEmpty(MaxSubscriberNumber) ? (object)System.DBNull.Value : (object)MaxSubscriberNumber);
                cmd.Parameters.AddWithValue("@MinBAN", string.IsNullOrEmpty(MinBAN) ? (object)System.DBNull.Value : (object)MinBAN);
                cmd.Parameters.AddWithValue("@MaxBAN", string.IsNullOrEmpty(MaxBAN) ? (object)System.DBNull.Value : (object)MaxBAN);
                cmd.Parameters.AddWithValue("@MinIMEI", string.IsNullOrEmpty(MinIMEI) ? (object)System.DBNull.Value : (object)MinIMEI);
                cmd.Parameters.AddWithValue("@MaxIMEI", string.IsNullOrEmpty(MaxIMEI) ? (object)System.DBNull.Value : (object)MaxIMEI);
                cmd.Parameters.AddWithValue("@ActivityType", string.IsNullOrEmpty(ActivityType) ? (object)System.DBNull.Value : (object)ActivityType);
                cmd.Parameters.AddWithValue("@ProductCode", string.IsNullOrEmpty(ProductCode) ? (object)System.DBNull.Value : (object)ProductCode);
                cmd.Parameters.AddWithValue("@CommissionType", string.IsNullOrEmpty(CommissionType) ? (object)System.DBNull.Value : (object)CommissionType);
                cmd.Parameters.AddWithValue("@MinContractDuration", string.IsNullOrEmpty(MinContractDuration) ? (object)System.DBNull.Value : (object)MinContractDuration);
                cmd.Parameters.AddWithValue("@MaxContractDuration", string.IsNullOrEmpty(MaxContractDuration) ? (object)System.DBNull.Value : (object)MaxContractDuration);
                cmd.Parameters.AddWithValue("@MinCommissionAmount", string.IsNullOrEmpty(MinCommissionAmount) ? (object)System.DBNull.Value : (object)MinCommissionAmount);
                cmd.Parameters.AddWithValue("@MaxCommissionAmount", string.IsNullOrEmpty(MaxCommissionAmount) ? (object)System.DBNull.Value : (object)MaxCommissionAmount);
                cmd.Parameters.AddWithValue("@CompanyCode", string.IsNullOrEmpty(CompanyCode) ? (object)System.DBNull.Value : (object)CompanyCode);
                cmd.Parameters.AddWithValue("@MinOrderDate", string.IsNullOrEmpty(MinOrderDate) ? (object)System.DBNull.Value : (object)MinOrderDate);
                cmd.Parameters.AddWithValue("@MaxOrderDate", string.IsNullOrEmpty(MaxOrderDate) ? (object)System.DBNull.Value : (object)MaxOrderDate);
                cmd.Parameters.AddWithValue("@MinConnectionDate", string.IsNullOrEmpty(MinConnectionDate) ? (object)System.DBNull.Value : (object)MinConnectionDate);
                cmd.Parameters.AddWithValue("@MaxConnectionDate", string.IsNullOrEmpty(MaxConnectionDate) ? (object)System.DBNull.Value : (object)MaxConnectionDate);
                cmd.Parameters.AddWithValue("@MinTerminationDate", string.IsNullOrEmpty(MinTerminationDate) ? (object)System.DBNull.Value : (object)MinTerminationDate);
                cmd.Parameters.AddWithValue("@MaxTerminationDate", string.IsNullOrEmpty(MaxTerminationDate) ? (object)System.DBNull.Value : (object)MaxTerminationDate);
                cmd.Parameters.AddWithValue("@CommissionPeriod", string.IsNullOrEmpty(CommissionPeriod) ? (object)System.DBNull.Value : (object)CommissionPeriod);
                DataSet ds = Globals.GetData(cmd);
                //ctx.Dispose();
                return Ok(ds.Tables[0].Rows.Count);
                // }

            }
            catch (Exception ex)
            {
                throw ex;
                // throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex));
            }

        }

        //this method  will  get rows from LCalc to be displayed in Reports
        public IHttpActionResult GetLCalForReports(string Source, string ProductCode, string CommissionType, string ActivityType, int PageNumber, int PageSize, string CompanyCode, string PrimaryChannel, string PayeeList, string MinIMEI, string MaxIMEI, string MinBAN, string MaxBAN, string MinContractDuration, string MaxContractDuration, string MinCommissionAmount, string MaxCommissionAmount, string MinOrderDate, string MaxOrderDate, string MinConnectionDate, string MaxConnectionDate, string MinTerminationDate, string MaxTerminationDate, string UserName, string Workflow, string MinSubscriberNumber, string MaxSubscriberNumber, string CommissionPeriod)//(GetLCalcForReports LCalcReports)//
        {
            try
            {
                //using (var ctx = new SOSEDMV10Entities())
                //{
                //Nullable<DateTime> FromOrderDate = DateTime.ParseExact(MinOrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> ToOrderDate = DateTime.ParseExact(MaxOrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> FromConnectionDate = DateTime.ParseExact(MinConnectionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> ToConnectionDate = DateTime.ParseExact(MaxConnectionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> FromTerminationDate = DateTime.ParseExact(MinTerminationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> ToTerminationDate = DateTime.ParseExact(MaxTerminationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> CurrentDate = DateTime.UtcNow.Date;

                //string CurrentDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");

                //var LCalcReports = new GetLCalcForReports();
                //LCalcReports.PageNumber = PageNumber;
                //LCalcReports.PageSize = PageSize;
                //LCalcReports.CompanyCode = CompanyCode;
                //LCalcReports.PrimaryChannel = string.IsNullOrEmpty(PrimaryChannel) ? System.DBNull.Value.ToString() : PrimaryChannel;
                //LCalcReports.PayeeList = string.IsNullOrEmpty(PayeeList) ? System.DBNull.Value.ToString() : PayeeList;
                //LCalcReports.MinIMEI = string.IsNullOrEmpty(MinIMEI) ? System.DBNull.Value.ToString() : MinIMEI;
                //LCalcReports.MaxIMEI = string.IsNullOrEmpty(MaxIMEI) ? System.DBNull.Value.ToString() : MaxIMEI;
                //LCalcReports.MinBAN = string.IsNullOrEmpty(MinBAN) ? System.DBNull.Value.ToString() : MinBAN;
                //LCalcReports.MaxBAN = string.IsNullOrEmpty(MaxBAN) ? System.DBNull.Value.ToString() : MaxBAN;
                //LCalcReports.MinSubscriberNumber = string.IsNullOrEmpty(MinSubscriberNumber) ? System.DBNull.Value.ToString() : MinSubscriberNumber;
                //LCalcReports.MaxSubscriberNumber = string.IsNullOrEmpty(MaxSubscriberNumber) ? System.DBNull.Value.ToString() : MaxSubscriberNumber;
                //LCalcReports.MinContractDuration = (MinContractDuration.Equals(0)) ? System.DBNull.Value.ToString() : MinContractDuration;
                //LCalcReports.MaxContractDuration = (MaxContractDuration.Equals(0)) ? System.DBNull.Value.ToString() : MaxContractDuration;
                //LCalcReports.MinCommissionAmount = (MinCommissionAmount.Equals(0)) ? System.DBNull.Value.ToString() : MinCommissionAmount;
                //LCalcReports.MaxCommissionAmount = (MaxCommissionAmount.Equals(0)) ? System.DBNull.Value.ToString() : MaxCommissionAmount;
                //LCalcReports.FromOrderDate = (MinOrderDate == CurrentDate) ? System.DBNull.Value.ToString() : MinOrderDate;
                //LCalcReports.ToOrderDate = (MaxOrderDate == CurrentDate) ? System.DBNull.Value.ToString() : MaxOrderDate;
                //LCalcReports.FromConnectionDate = (MinConnectionDate == CurrentDate) ? System.DBNull.Value.ToString() : MinConnectionDate;
                //LCalcReports.ToConnectionDate = (MaxConnectionDate == CurrentDate) ? System.DBNull.Value.ToString() : MaxConnectionDate;
                //LCalcReports.FromTerminationDate = (MinTerminationDate == CurrentDate) ? System.DBNull.Value.ToString() : MinTerminationDate;
                //LCalcReports.ToTerminationDate = (MaxTerminationDate == CurrentDate) ? System.DBNull.Value.ToString() : MaxTerminationDate;
                //LCalcReports.ActivityType = string.IsNullOrEmpty(ActivityType) ? System.DBNull.Value.ToString() : ActivityType;
                //LCalcReports.ProductCode = string.IsNullOrEmpty(ProductCode) ? System.DBNull.Value.ToString() : ProductCode;
                //LCalcReports.CommissionType = string.IsNullOrEmpty(CommissionType) ? System.DBNull.Value.ToString() : CommissionType;
                //var xx = ctx.GetCalcReport(LCalcReports.Source, LCalcReports.PrimaryChannel, LCalcReports.PageSize, LCalcReports.PageNumber, LCalcReports.PayeeList, LCalcReports.MinSubscriberNumber, LCalcReports.MaxSubscriberNumber, LCalcReports.MinBAN, LCalcReports.MaxBAN,
                //    LCalcReports.MinIMEI, LCalcReports.MaxIMEI, LCalcReports.ActivityType, LCalcReports.ProductCode, LCalcReports.CommissionType, LCalcReports.MinContractDuration, LCalcReports.MaxContractDuration,
                //      LCalcReports.MinCommissionAmount, LCalcReports.MaxCommissionAmount, LCalcReports.CompanyCode,
                //     LCalcReports.FromOrderDate, LCalcReports.ToOrderDate, LCalcReports.FromConnectionDate, LCalcReports.ToConnectionDate, LCalcReports.FromTerminationDate, LCalcReports.ToTerminationDate
                //   ,CommissionPeriod ).ToList();
                //ctx.Dispose();
                //ADO .Net Part starts here
                string strQuery = "Exec GetCalcReport @Source, @PrimaryChannel, @PageSize, @PageNumber, @PayeeList,@MinSubscriberNumber, @MaxSubscriberNumber,@MinBAN, @MaxBAN,@MinIMEI, @MaxIMEI, @ActivityType, @ProductCode, @CommissionType,@MinContractDuration,@MaxContractDuration,@MinCommissionAmount, @MaxCommissionAmount, @CompanyCode,@MinOrderDate, @MaxOrderDate, @MinConnectionDate, @MaxConnectionDate, @MinTerminationDate, @MaxTerminationDate,@CommissionPeriod ";
                SqlCommand cmd = new SqlCommand(strQuery);
                cmd.Parameters.AddWithValue("@Source", string.IsNullOrEmpty(Source) ? (object)System.DBNull.Value : (object)Source);
                cmd.Parameters.AddWithValue("@PrimaryChannel", string.IsNullOrEmpty(PrimaryChannel) ? (object)System.DBNull.Value : (object)PrimaryChannel);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                cmd.Parameters.AddWithValue("@PayeeList", string.IsNullOrEmpty(PayeeList) ? (object)System.DBNull.Value : (object)PayeeList);
                cmd.Parameters.AddWithValue("@MinSubscriberNumber", string.IsNullOrEmpty(MinSubscriberNumber) ? (object)System.DBNull.Value : (object)MinSubscriberNumber);
                cmd.Parameters.AddWithValue("@MaxSubscriberNumber", string.IsNullOrEmpty(MaxSubscriberNumber) ? (object)System.DBNull.Value : (object)MaxSubscriberNumber);
                cmd.Parameters.AddWithValue("@MinBAN", string.IsNullOrEmpty(MinBAN) ? (object)System.DBNull.Value : (object)MinBAN);
                cmd.Parameters.AddWithValue("@MaxBAN", string.IsNullOrEmpty(MaxBAN) ? (object)System.DBNull.Value : (object)MaxBAN);
                cmd.Parameters.AddWithValue("@MinIMEI", string.IsNullOrEmpty(MinIMEI) ? (object)System.DBNull.Value : (object)MinIMEI);
                cmd.Parameters.AddWithValue("@MaxIMEI", string.IsNullOrEmpty(MaxIMEI) ? (object)System.DBNull.Value : (object)MaxIMEI);
                cmd.Parameters.AddWithValue("@ActivityType", string.IsNullOrEmpty(ActivityType) ? (object)System.DBNull.Value : (object)ActivityType);
                cmd.Parameters.AddWithValue("@ProductCode", string.IsNullOrEmpty(ProductCode) ? (object)System.DBNull.Value : (object)ProductCode);
                cmd.Parameters.AddWithValue("@CommissionType", string.IsNullOrEmpty(CommissionType) ? (object)System.DBNull.Value : (object)CommissionType);
                cmd.Parameters.AddWithValue("@MinContractDuration", string.IsNullOrEmpty(MinContractDuration) ? (object)System.DBNull.Value : (object)MinContractDuration);
                cmd.Parameters.AddWithValue("@MaxContractDuration", string.IsNullOrEmpty(MaxContractDuration) ? (object)System.DBNull.Value : (object)MaxContractDuration);
                cmd.Parameters.AddWithValue("@MinCommissionAmount", string.IsNullOrEmpty(MinCommissionAmount) ? (object)System.DBNull.Value : (object)MinCommissionAmount);
                cmd.Parameters.AddWithValue("@MaxCommissionAmount", string.IsNullOrEmpty(MaxCommissionAmount) ? (object)System.DBNull.Value : (object)MaxCommissionAmount);
                cmd.Parameters.AddWithValue("@CompanyCode", string.IsNullOrEmpty(CompanyCode) ? (object)System.DBNull.Value : (object)CompanyCode);
                cmd.Parameters.AddWithValue("@MinOrderDate", string.IsNullOrEmpty(MinOrderDate) ? (object)System.DBNull.Value : (object)MinOrderDate);
                cmd.Parameters.AddWithValue("@MaxOrderDate", string.IsNullOrEmpty(MaxOrderDate) ? (object)System.DBNull.Value : (object)MaxOrderDate);
                cmd.Parameters.AddWithValue("@MinConnectionDate", string.IsNullOrEmpty(MinConnectionDate) ? (object)System.DBNull.Value : (object)MinConnectionDate);
                cmd.Parameters.AddWithValue("@MaxConnectionDate", string.IsNullOrEmpty(MaxConnectionDate) ? (object)System.DBNull.Value : (object)MaxConnectionDate);
                cmd.Parameters.AddWithValue("@MinTerminationDate", string.IsNullOrEmpty(MinTerminationDate) ? (object)System.DBNull.Value : (object)MinTerminationDate);
                cmd.Parameters.AddWithValue("@MaxTerminationDate", string.IsNullOrEmpty(MaxTerminationDate) ? (object)System.DBNull.Value : (object)MaxTerminationDate);
                cmd.Parameters.AddWithValue("@CommissionPeriod", string.IsNullOrEmpty(CommissionPeriod) ? (object)System.DBNull.Value : (object)CommissionPeriod);
                DataSet ds = Globals.GetData(cmd);

                return Ok(ds.Tables[0]);
                //  }

            }
            catch (Exception ex)
            {
                throw ex;
                // throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex));
            }

        }

        [HttpGet]
        public IHttpActionResult DownloadLCalForReports(string Source, string ProductCode, string CommissionType, string ActivityType, int PageNumber, int PageSize, string CompanyCode, string PrimaryChannel, string PayeeList, string MinIMEI, string MaxIMEI, string MinBAN, string MaxBAN, string MinContractDuration, string MaxContractDuration, string MinCommissionAmount, string MaxCommissionAmount, string MinOrderDate, string MaxOrderDate, string MinConnectionDate, string MaxConnectionDate, string MinTerminationDate, string MaxTerminationDate, string UserName, string Workflow, string MinSubscriberNumber, string MaxSubscriberNumber, string CommissionPeriod)//(GetLCalcForReports LCalcReports)//
        {
            try
            {
                var FilesPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserName + "/forzip";
                if (System.IO.Directory.Exists(FilesPath))
                {
                    string CalcFileName = "ExportCalcReport_*.*";
                    try
                    {
                        var CalcFileList = Directory.GetFiles(FilesPath, CalcFileName, SearchOption.AllDirectories).ToList();
                        foreach (var CalcFile in CalcFileList)
                        {
                            if (System.IO.File.Exists(CalcFile))//delete already existing file
                                System.IO.File.Delete(CalcFile);
                        }
                    }
                    catch
                    { //do nothing 
                    }
                }

                //Nullable<DateTime> FromOrderDate = DateTime.ParseExact(MinOrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> ToOrderDate = DateTime.ParseExact(MaxOrderDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> FromConnectionDate = DateTime.ParseExact(MinConnectionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> ToConnectionDate = DateTime.ParseExact(MaxConnectionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> FromTerminationDate = DateTime.ParseExact(MinTerminationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> ToTerminationDate = DateTime.ParseExact(MaxTerminationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //Nullable<DateTime> CurrentDate = DateTime.UtcNow.Date;

                //string CurrentDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");

                //var LCalcReports = new GetLCalcForReports();
                //LCalcReports.PageNumber = PageNumber;
                //LCalcReports.PageSize = PageSize;
                //LCalcReports.CompanyCode = CompanyCode;
                //LCalcReports.PrimaryChannel = string.IsNullOrEmpty(PrimaryChannel) ? System.DBNull.Value.ToString() : PrimaryChannel;
                //LCalcReports.PayeeList = string.IsNullOrEmpty(PayeeList) ? System.DBNull.Value.ToString() : PayeeList;
                //LCalcReports.MinIMEI = string.IsNullOrEmpty(MinIMEI) ? System.DBNull.Value.ToString() : MinIMEI;
                //LCalcReports.MaxIMEI = string.IsNullOrEmpty(MaxIMEI) ? System.DBNull.Value.ToString() : MaxIMEI;
                //LCalcReports.MinBAN = string.IsNullOrEmpty(MinBAN) ? System.DBNull.Value.ToString() : MinBAN;
                //LCalcReports.MaxBAN = string.IsNullOrEmpty(MaxBAN) ? System.DBNull.Value.ToString() : MaxBAN;
                //LCalcReports.MinSubscriberNumber = string.IsNullOrEmpty(MinSubscriberNumber) ? System.DBNull.Value.ToString() : MinSubscriberNumber;
                //LCalcReports.MaxSubscriberNumber = string.IsNullOrEmpty(MaxSubscriberNumber) ? System.DBNull.Value.ToString() : MaxSubscriberNumber;
                //LCalcReports.MinContractDuration = (MinContractDuration.Equals(0)) ? System.DBNull.Value.ToString() : MinContractDuration;
                //LCalcReports.MaxContractDuration = (MaxContractDuration.Equals(0)) ? System.DBNull.Value.ToString() : MaxContractDuration;
                //LCalcReports.MinCommissionAmount = (MinCommissionAmount.Equals(0)) ? System.DBNull.Value.ToString() : MinCommissionAmount;
                //LCalcReports.MaxCommissionAmount = (MaxCommissionAmount.Equals(0)) ? System.DBNull.Value.ToString() : MaxCommissionAmount;
                //LCalcReports.FromOrderDate = (MinOrderDate == CurrentDate) ? System.DBNull.Value.ToString() : MinOrderDate;
                //LCalcReports.ToOrderDate = (MaxOrderDate == CurrentDate) ? System.DBNull.Value.ToString() : MaxOrderDate;
                //LCalcReports.FromConnectionDate = (MinConnectionDate == CurrentDate) ? System.DBNull.Value.ToString() : MinConnectionDate;
                //LCalcReports.ToConnectionDate = (MaxConnectionDate == CurrentDate) ? System.DBNull.Value.ToString() : MaxConnectionDate;
                //LCalcReports.FromTerminationDate = (MinTerminationDate == CurrentDate) ? System.DBNull.Value.ToString() : MinTerminationDate;
                //LCalcReports.ToTerminationDate = (MaxTerminationDate == CurrentDate) ? System.DBNull.Value.ToString() : MaxTerminationDate;
                //LCalcReports.ActivityType = string.IsNullOrEmpty(ActivityType) ? System.DBNull.Value.ToString() : ActivityType;
                //LCalcReports.ProductCode = string.IsNullOrEmpty(ProductCode) ? System.DBNull.Value.ToString() : ProductCode;
                //LCalcReports.CommissionType = string.IsNullOrEmpty(CommissionType) ? System.DBNull.Value.ToString() : CommissionType;
                //var xx = db.GetCalcReport(LCalcReports.Source, LCalcReports.PrimaryChannel, LCalcReports.PageSize, LCalcReports.PageNumber, LCalcReports.PayeeList, LCalcReports.MinSubscriberNumber, LCalcReports.MaxSubscriberNumber, LCalcReports.MinBAN, LCalcReports.MaxBAN,
                //    LCalcReports.MinIMEI, LCalcReports.MaxIMEI, LCalcReports.ActivityType, LCalcReports.ProductCode, LCalcReports.CommissionType, LCalcReports.MinContractDuration, LCalcReports.MaxContractDuration,
                //      LCalcReports.MinCommissionAmount, LCalcReports.MaxCommissionAmount, LCalcReports.CompanyCode,
                //     LCalcReports.FromOrderDate, LCalcReports.ToOrderDate, LCalcReports.FromConnectionDate, LCalcReports.ToConnectionDate, LCalcReports.FromTerminationDate, LCalcReports.ToTerminationDate
                //    ,CommissionPeriod).ToList();

                //Create a excel in S drive for the Calculations
                var FileName = "ExportCalcReport_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".zip";
                var CfileLocation = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserName + "/forzip/" + FileName;

                //var FilesPath = ConfigurationManager.AppSettings["CalcDocumentPath"];
                if (System.IO.File.Exists(CfileLocation))
                    System.IO.File.Delete(CfileLocation);
                //ADO .Net Part starts here
                string strQuery = "Exec GetCalcReport @Source, @PrimaryChannel, @PageSize, @PageNumber, @PayeeList,@MinSubscriberNumber, @MaxSubscriberNumber,@MinBAN, @MaxBAN,@MinIMEI, @MaxIMEI, @ActivityType, @ProductCode, @CommissionType,@MinContractDuration,@MaxContractDuration,@MinCommissionAmount, @MaxCommissionAmount, @CompanyCode,@MinOrderDate, @MaxOrderDate, @MinConnectionDate, @MaxConnectionDate, @MinTerminationDate, @MaxTerminationDate,@CommissionPeriod ";
                SqlCommand cmd = new SqlCommand(strQuery);
                cmd.Parameters.AddWithValue("@Source", string.IsNullOrEmpty(Source) ? (object)System.DBNull.Value : (object)Source);
                cmd.Parameters.AddWithValue("@PrimaryChannel", string.IsNullOrEmpty(PrimaryChannel) ? (object)System.DBNull.Value : (object)PrimaryChannel);
                cmd.Parameters.AddWithValue("@PageSize", PageSize);
                cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                cmd.Parameters.AddWithValue("@PayeeList", string.IsNullOrEmpty(PayeeList) ? (object)System.DBNull.Value : (object)PayeeList);
                cmd.Parameters.AddWithValue("@MinSubscriberNumber", string.IsNullOrEmpty(MinSubscriberNumber) ? (object)System.DBNull.Value : (object)MinSubscriberNumber);
                cmd.Parameters.AddWithValue("@MaxSubscriberNumber", string.IsNullOrEmpty(MaxSubscriberNumber) ? (object)System.DBNull.Value : (object)MaxSubscriberNumber);
                cmd.Parameters.AddWithValue("@MinBAN", string.IsNullOrEmpty(MinBAN) ? (object)System.DBNull.Value : (object)MinBAN);
                cmd.Parameters.AddWithValue("@MaxBAN", string.IsNullOrEmpty(MaxBAN) ? (object)System.DBNull.Value : (object)MaxBAN);
                cmd.Parameters.AddWithValue("@MinIMEI", string.IsNullOrEmpty(MinIMEI) ? (object)System.DBNull.Value : (object)MinIMEI);
                cmd.Parameters.AddWithValue("@MaxIMEI", string.IsNullOrEmpty(MaxIMEI) ? (object)System.DBNull.Value : (object)MaxIMEI);
                cmd.Parameters.AddWithValue("@ActivityType", string.IsNullOrEmpty(ActivityType) ? (object)System.DBNull.Value : (object)ActivityType);
                cmd.Parameters.AddWithValue("@ProductCode", string.IsNullOrEmpty(ProductCode) ? (object)System.DBNull.Value : (object)ProductCode);
                cmd.Parameters.AddWithValue("@CommissionType", string.IsNullOrEmpty(CommissionType) ? (object)System.DBNull.Value : (object)CommissionType);
                cmd.Parameters.AddWithValue("@MinContractDuration", string.IsNullOrEmpty(MinContractDuration) ? (object)System.DBNull.Value : (object)MinContractDuration);
                cmd.Parameters.AddWithValue("@MaxContractDuration", string.IsNullOrEmpty(MaxContractDuration) ? (object)System.DBNull.Value : (object)MaxContractDuration);
                cmd.Parameters.AddWithValue("@MinCommissionAmount", string.IsNullOrEmpty(MinCommissionAmount) ? (object)System.DBNull.Value : (object)MinCommissionAmount);
                cmd.Parameters.AddWithValue("@MaxCommissionAmount", string.IsNullOrEmpty(MaxCommissionAmount) ? (object)System.DBNull.Value : (object)MaxCommissionAmount);
                cmd.Parameters.AddWithValue("@CompanyCode", string.IsNullOrEmpty(CompanyCode) ? (object)System.DBNull.Value : (object)CompanyCode);
                cmd.Parameters.AddWithValue("@MinOrderDate", string.IsNullOrEmpty(MinOrderDate) ? (object)System.DBNull.Value : (object)MinOrderDate);
                cmd.Parameters.AddWithValue("@MaxOrderDate", string.IsNullOrEmpty(MaxOrderDate) ? (object)System.DBNull.Value : (object)MaxOrderDate);
                cmd.Parameters.AddWithValue("@MinConnectionDate", string.IsNullOrEmpty(MinConnectionDate) ? (object)System.DBNull.Value : (object)MinConnectionDate);
                cmd.Parameters.AddWithValue("@MaxConnectionDate", string.IsNullOrEmpty(MaxConnectionDate) ? (object)System.DBNull.Value : (object)MaxConnectionDate);
                cmd.Parameters.AddWithValue("@MinTerminationDate", string.IsNullOrEmpty(MinTerminationDate) ? (object)System.DBNull.Value : (object)MinTerminationDate);
                cmd.Parameters.AddWithValue("@MaxTerminationDate", string.IsNullOrEmpty(MaxTerminationDate) ? (object)System.DBNull.Value : (object)MaxTerminationDate);
                cmd.Parameters.AddWithValue("@CommissionPeriod", string.IsNullOrEmpty(CommissionPeriod) ? (object)System.DBNull.Value : (object)CommissionPeriod);
                DataSet ds = Globals.GetData(cmd);

                Globals.ExportZipFromDataTable(null, CompanyCode, UserName, FileName, ds.Tables[0]);

                //IWorkbook workbook = new XSSFWorkbook();
                //ISheet sheet1 = workbook.CreateSheet("Sheet 1");
                //IRow row1 = sheet1.CreateRow(0);
                //var tb =new  DataTable((typeof(GetCalcReport_Result)).Name);
                //PropertyInfo[] props = typeof(GetCalcReport_Result).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                //for(var j= 0;j<props.Length;j++)
                //{
                //    // var displayName=PayeeModel.GetDisplayName()
                //    var PropName = props[j].Name.Replace("Lc", "");//make heading of column without suffix
                //                                               //if (PropName != "Id")
                //                                               //{
                //    ICell cell = row1.CreateCell(j);
                //    string columnName = PropName;//tb.Columns[j].ToString();
                //    cell.SetCellValue(columnName);
                //    //method to size column width and GC is used to avoid error System.argument exception

                //    GC.Collect();
                //   // tb.Columns.Add(PropName);
                //    //  }

                //}

                //for(int i = 0; i <xx.Count() ; i++)
                //{
                //    IRow row = sheet1.CreateRow(i + 1);
                //    //var values = new object[props.Length];
                //    for (var j = 0; j < props.Length; j++)
                //    {
                //        try
                //        {
                //            var value = props[j].GetValue(xx, null);
                //            ICell cell = row.CreateCell(j);
                //            if (value == null)
                //            {
                //                cell.SetCellValue("");
                //            }
                //            else
                //            {
                //                string val = "";
                //                try
                //                {
                //                    val = value.ToString();
                //                }
                //                catch { }
                //                cell.SetCellValue(val);
                //            }
                //        }
                //        catch
                //        {
                //           //do nothing
                //        }

                //    }

                //   // tb.Rows.Add(values);
                //}

                //for (int j = 0; j < tb.Columns.Count; j++)
                //{
                //    ICell cell = row1.CreateCell(j);
                //    string columnName = tb.Columns[j].ToString();
                //    cell.SetCellValue(columnName);
                //    //method to size column width and GC is used to avoid error System.argument exception

                //    GC.Collect();
                //}

                //////loops through data  
                //for (int k = 0; k < tb.Rows.Count; k++)
                //{
                //    IRow row = sheet1.CreateRow(k + 1);
                //    for (int m = 0; m < tb.Columns.Count; m++)
                //    {

                //        ICell cell = row.CreateCell(m);
                //        String columnName = tb.Columns[m].ToString();
                //        cell.SetCellValue(tb.Rows[k][columnName].ToString());
                //    }
                //}

                //if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyCode + "/" + UserName))//create directory if not present
                //{
                //    System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyCode + "/" + UserName);
                //}

                //using (ZipFile zip = new ZipFile())
                //{
                //    FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyCode + "/" + UserName, "ExportData.xlsx"), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                //    workbook.Write(xfile);

                //    //tb.TableName = "Tmp";
                //    //zip.AddEntry("Temp.xml", (name, stream) => tb.WriteXml(stream));
                //    zip.AddFile(Path.Combine(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyCode + "/" + UserName, "ExportData.xlsx"),"");
                //    zip.Save(Path.Combine(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyCode + "/" + UserName, FileName));
                //    System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyCode + "/" + UserName, "ExportData.xlsx"));
                //    xfile.Close();
                //}
                //FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyCode + "/" + LoggedInUserName, FileName), FileMode.Create, System.IO.FileAccess.Write);
                //workbook.Write(xfile);

                //xfile.Close();

                return Ok(FileName);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex));
            }

        }

        public IHttpActionResult GetLCalcCounts(int SOSBatchNumber, string UserName, string Workflow, int CompanyId)
        {
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            var PeriodCode = 0;
            if (BatchDetails.LbPeriodCode.HasValue)
                PeriodCode = BatchDetails.LbPeriodCode.Value;
            string Qry = "Select Count(*) as RowCounts from {Schema}.XCalc Where XBatchNumber = " + SOSBatchNumber + " and XPeriodCode=" + PeriodCode;

            if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
            {
                Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LCalc", "GetLCalcCounts", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
            }
            else
            {
                var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, BatchDetails.LbCommissionPeriod);//db.Database.SqlQuery<LCalcViewModel>(Qry).ToList();
                                                                                                           //Execute the query and return the result 
                                                                                                           //var xx = db.Database.SqlQuery<LCalcRowCountsViewModel>(Qry).ToList();
                return Ok(xx);
            }
            return Ok();
        }


        public IHttpActionResult GetLCalcForGrid(int SOSBatchNumber, int PageNumber, int PageSize, string UserName, string Workflow, string sortdatafield, string sortorder, string FilterQuery, int CompanyId)
        {
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            var PeriodCode = 0;
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
            var CompanySpecificColumnsList = db.LCompanySpecificColumns.Where(p => p.LcscCompanyId == CompanyId).Where(p => p.LcscTableName == "XCalc").Select(p => p.LcscColumnName).ToList();
            for (var i = 0; i < CompanySpecificColumnsList.Count(); i++)
            {
                CompanySpecificColumnsList[i] = CompanySpecificColumnsList.ElementAt(i);
            }
            // string ColumnList = " dbo.FnGetUserName(LcAcceptedById) as LcAcceptedById,LcIsPayeeAccepted,LcPayeeAttachmentId,Id, LcOpCoCode, LcSource, LcAdjustmenCode, LcSOSBatchNumber, LcAlteryxTransactionNumber, LcPrimaryChannel, LcPayee, LcParentPayee, LcOrderDate, LcConnectionDate, LcTerminationDate, LcSubscriberNumber, LcBAN, LcActivityType, LcPlanDescrition, LcProductCode, LcUpgradeCode, LcIMEI, LcDevieCode, LcDeviceType, LcCommType, LcContractDuration, LcContractId, LcCommAmtExTax, LcTax, LcCommAmtIncTax, LcComments";
            string ColumnList = String.Join(",", CompanySpecificColumnsList);//" XBatchNumber,XAlteryxTransactionNumber,[XPayee],XParentPayee,[XSubscriberNumber],[XBAN],[XActivityType],XIMEI";
            //Using the column list obtained above, and other parameters passed in the method create a SQL query to fatch the data from database
            string Qry = "Select * from (Select " + ColumnList + ", ";
            Qry = Qry + "ROW_NUMBER() OVER (" + SortQuery + ") as row FROM {Schema}.XCalc ";
            if (!string.IsNullOrEmpty(FilterQuery))
            {
                Qry = Qry + " Where XBatchNumber = " + SOSBatchNumber + " and XPeriodCode=" + PeriodCode + FilterQuery + ") a ";
            }
            else
            {
                Qry = Qry + " Where XBatchNumber = " + SOSBatchNumber + " and XPeriodCode=" + PeriodCode + ") a ";
            }
            Qry = Qry + " Where row > " + PageNumber * PageSize + " And row <= " + (PageNumber + 1) * PageSize;

            if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
            {
                Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LCalc", "GetLCalcForGrid", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
            }
            else
            {
                //Execute the query and return the result 
                var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, BatchDetails.LbCommissionPeriod);//db.Database.SqlQuery<LCalcViewModel>(Qry).ToList();
                return Ok(xx);
            }
            return Ok();
        }


        //This method updates the Attament and Acceptance of Selected Payee records.
        [HttpGet]
        public IHttpActionResult UpdateAcceptance(string SelectedData, string AcceptedBy, DateTime AcceptedAt, string UserName, string Workflow, int CompanyId)
        {
            string[] s = SelectedData.Split(',');
            for (int j = 0; j < s.Length; j = j + 2)
            {
                var Id = Convert.ToInt32(s[j]);
                int XBatchNumber = Convert.ToInt32(s[j + 1]);
                //LCalc LCalc = db.LCalcs.Find(Id);
                //LCalc.LcIsPayeeAccepted = true;
                //LCalc.LcAcceptedDateTime = AcceptedAt;
                //LCalc.LcAcceptedById = AcceptedBy;
                try
                {
                    var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == XBatchNumber).FirstOrDefault();
                    var PeriodCode = 0;
                    if (BatchDetails.LbPeriodCode.HasValue)
                        PeriodCode = BatchDetails.LbPeriodCode.Value;
                    //db.Entry(LCalc).State = EntityState.Modified;
                    //db.SaveChanges();
                    var todayDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
                    string Query = "update {Schema}.XCalc set AcceptedBy='" + UserName + "',IsPayeeAccepted=1,AcceptedDateTime='" + todayDate + "' where XAlteryxTransactionNumber=" + Id + " and XBatchNumber=" + XBatchNumber + " and XPeriodCode=" + PeriodCode;
                    if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
                    {
                        Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LCalc", "UpdateAcceptance", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
                    }
                    else
                    {
                        Globals.RunUpdateQueryInOpcoDB(CompanyId, Query, BatchDetails.LbCommissionPeriod);
                        //var BatchData = Globals.GetQueryResultFromOpcoDB(CompanyId, "select XBatchNumber from {Schema}.XCalc where XAlteryxTransactionNumber=" + Id);
                        //  if (BatchData.Rows.Count > 0)
                        UpdateBatchesAndSendEmail(XBatchNumber, UserName, "AcceptNotifyRA", AcceptedBy, string.Empty, "Accepted By ");
                    }
                }
                catch (DbEntityValidationException dbex)
                {
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    {
                        //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                    }
                    else
                    {
                        throw ex;//This exception will be handled in FilterConfig's CustomHandler
                    }
                }
            }
            return Ok();

        }

        //This method updates the Attament and Acceptance of all Payee records.//
        [HttpPost]
        public IHttpActionResult UpdatePayeeAttachmentForAll(PayeeCalcViewModel model, string LoggedInRoleName, int LoggedinLUserId, string CommissionPeriodIdList, int AttachmentId, string UserName, string Workflow, string LoggedInUserId, int CompanyId)
        {

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
                {
                    var PayeeCodesList = ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LpPayeeCode")).ToList();
                    foreach (var payeecode in PayeeCodesList)
                    {
                        PayeeCodeList += "'" + payeecode + "',";
                    }
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

            // //    //Getting list of Batch Nos
            // var BatchQry = string.Empty;
            // if (LoggedInRoleName == "Payee" || LoggedInRoleName == "Channel Manager")
            // {
            //     // NOTE: Payee and CM can see Calc data across all channel/schemes as long as the Calc has the PayeeCode from their dropdown (direct match or child payee)
            //     BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') ";
            // }
            // else
            // {
            //     if (string.IsNullOrEmpty(model.PortfolioList))
            //     {
            //         /*If none is selected, 
            //          Add user portfolios to portfolio matching during batch selection during the query to populate Calc grid for that period
            //         */
            //         model.PortfolioList= string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == LoggedinLUserId && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
            //     }
            //     /*If any channel is selected
            //Add that portfolio to portfolio matching during batch selection during the query 
            //to populate Calc grid for that period*/
            //     BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (select Id from LPortfolios where LpChannelId in (select LpChannelId from LPortfolios where Id in (" + model.PortfolioList + ") )) and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') ";
            // }
            // var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
            // string BatchNumberStr = "";
            // foreach (var BatchNumber in BatchNumberList)
            // {
            //     BatchNumberStr += BatchNumber + ",";
            //     if (BatchNumber != 0)
            //         UpdateBatchesAndSendEmail(BatchNumber, UserName, "AttachNotifyRA", LoggedInUserId, string.Empty, "Attached By ");
            // }
            // BatchNumberStr = BatchNumberStr.TrimEnd(',');
            // if (BatchNumberStr.Equals(""))
            // {
            //     BatchNumberStr = "''";
            // }

            //Getting list of Batch Nos
            var BatchQry = string.Empty;
            DataTable BatchNumberDT = new DataTable();
            string BatchNumberStr = "";
            if (LoggedInRoleName == "Payee" || LoggedInRoleName == "Channel Manager")
            {
                // NOTE: Payee and CM can see Calc data across all channel/schemes as long as the Calc has the PayeeCode from their dropdown (direct match or child payee)
                //BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') ";
                //QUERY Corrected as per logic
                //As XSchemaGR is taking toolong to respond using LBatches here And will use XSchem<opco> only in AcceptAll and AttachAll
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') and WFCompanyId=" + CompanyId;
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
                BatchQry = "select distinct XBatchNumber from {Schema}.XCalc where XPayee in("
                         + ChildPayeeStr + PayeeCodeList + ")  and XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ")";
                BatchNumberDT = Globals.GetQueryResultFromOpcoDB(CompanyId, BatchQry, CommissionPeriod);
                BatchNumberStr = "";
                foreach (DataRow BatchNumber in BatchNumberDT.Rows)
                {
                    BatchNumberStr += BatchNumber.Field<int>("XBatchNumber") + ",";
                    if (BatchNumber.Field<int>("XBatchNumber") != 0)
                        UpdateBatchesAndSendEmail(BatchNumber.Field<int>("XBatchNumber"), UserName, "AttachNotifyRA", LoggedInUserId, string.Empty, "Accepted By ");
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.PortfolioList))
                {
                    /*If none is selected, 
                     Add user portfolios to portfolio matching during batch selection during the query to populate Calc grid for that period
                    */
                    model.PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == LoggedinLUserId && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
                }
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Calc grid for that period*/
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (select Id from LPortfolios where LpChannelId in (select LpChannelId from LPortfolios where Id in (" + model.PortfolioList + ") )) and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') ";
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                    if (BatchNumber != 0)
                        UpdateBatchesAndSendEmail(BatchNumber, UserName, "AttachNotifyRA", UserName, LoggedInUserId, "Accepted By ");
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }



            var qry = "Update {Schema}.XCalc set PayeeAttachmentId=" + AttachmentId + " where XPayee in("
                          + ChildPayeeStr + PayeeCodeList + ")  and XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ")";
            if (string.IsNullOrEmpty(CommissionPeriod))
            {
                Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LCalc", "UpdatePayeeAttachmentForAll", "Commission Period does not exist", "", "Warning", "", "", "L2 Admin", null, null, "New");
            }
            else
            {
                Globals.RunUpdateQueryInOpcoDB(CompanyId, qry, CommissionPeriod);//db.Database.ExecuteSqlCommand(qry,AttachmentId);
            }
            return Ok();

        }

        //This method updates the Attament and Acceptance of all Payee records.
        [HttpPost]
        public IHttpActionResult UpdatePayeeAcceptanceStatusForAll(PayeeCalcViewModel model, int LoggedinLUserId, string LoggedInRoleName, int CompanyId, string CommissionPeriodIdList, string AcceptedBy, DateTime AcceptedAt, string UserName, string Workflow, string LoggedInUserId)
        {
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
                {
                    var PayeeCodesList = ds.Tables[0].AsEnumerable().Select(row => row.Field<string>("LpPayeeCode")).ToList();
                    foreach (var payeecode in PayeeCodesList)
                    {
                        PayeeCodeList += "'" + payeecode + "',";
                    }
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
            string[] CommPeriodsList = CommissionPeriodIdList.Split(',');
            var PeriodCode = 0;
            var CommissionPeriod = "";
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

            //Getting list of Batch Nos
            var BatchQry = string.Empty;
            DataTable BatchNumberDT = new DataTable();
            string BatchNumberStr = "";
            if (LoggedInRoleName == "Payee" || LoggedInRoleName == "Channel Manager")
            {
                // NOTE: Payee and CM can see Calc data across all channel/schemes as long as the Calc has the PayeeCode from their dropdown (direct match or child payee)
                //BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') ";
                //QUERY Corrected as per logic
                //As XSchemaGR is taking toolong to respond using LBatches here And will use XSchem<opco> only in AcceptAll and AttachAll
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') and WFCompanyId=" + CompanyId;
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
                BatchQry = "select distinct XBatchNumber from {Schema}.XCalc where XPayee in("
                         + ChildPayeeStr + PayeeCodeList + ")  and XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ")";
                BatchNumberDT = Globals.GetQueryResultFromOpcoDB(CompanyId, BatchQry, CommissionPeriod);
                BatchNumberStr = "";
                foreach (DataRow BatchNumber in BatchNumberDT.Rows)
                {
                    BatchNumberStr += BatchNumber.Field<int>("XBatchNumber") + ",";
                    if (BatchNumber.Field<int>("XBatchNumber") != 0)
                        UpdateBatchesAndSendEmail(BatchNumber.Field<int>("XBatchNumber"), UserName, "AcceptNotifyRA", AcceptedBy, string.Empty, "Accepted By ");
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(model.PortfolioList))
                {
                    /*If none is selected, 
                     Add user portfolios to portfolio matching during batch selection during the query to populate Calc grid for that period
                    */
                    model.PortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == LoggedinLUserId && p.MepEntityType == "LUsers").Select(p => p.MepPortfolioId));
                }
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Calc grid for that period*/
                BatchQry = "select distinct lb.LbBatchNumber from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (select Id from LPortfolios where LpChannelId in (select LpChannelId from LPortfolios where Id in (" + model.PortfolioList + ") )) and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') ";
                var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                foreach (var BatchNumber in BatchNumberList)
                {
                    BatchNumberStr += BatchNumber + ",";
                    if (BatchNumber != 0)
                        UpdateBatchesAndSendEmail(BatchNumber, UserName, "AcceptNotifyRA", AcceptedBy, string.Empty, "Accepted By ");
                }
                BatchNumberStr = BatchNumberStr.TrimEnd(',');
                if (BatchNumberStr.Equals(""))
                {
                    BatchNumberStr = "''";
                }
            }


            var qry = "Update {Schema}.XCalc set IsPayeeAccepted=1,AcceptedDateTime='" + AcceptedAt + "',AcceptedBy='" + UserName + "' where XPayee in("
                          + ChildPayeeStr + PayeeCodeList + ")  and XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ")";
            if (string.IsNullOrEmpty(CommissionPeriod))
            {
                Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LCalc", "UpdatePayeeAcceptanceForAll", "Commission Period does not exist ", "", "Warning", "", "", "L2 Admin", null, null, "New");
            }
            else
            {
                Globals.RunUpdateQueryInOpcoDB(CompanyId, qry, CommissionPeriod);//db.Database.ExecuteSqlCommand(qry, AcceptedAt,AcceptedBy);
            }
            return Ok();

        }

        //This method updates the Attament and Acceptance of Selected Payee records.
        [HttpGet]
        public IHttpActionResult UpdatePayeeAttachment(string SelectedData, int AttachmentId, string UserName, string Workflow, string LoggedInUserId, int CompanyId)
        {
            string[] s = SelectedData.Split(',');
            for (int j = 0; j < s.Length; j = j + 2)
            {
                var Id = Convert.ToInt32(s[j]);
                var XBatchNumber = Convert.ToInt32(s[j + 1]);
                //LCalc LCalc = db.LCalcs.Find(Id);
                //if (LCalc == null)
                //{
                //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "CALC")));
                //}
                //LCalc.LcPayeeAttachmentId = AttachmentId;
                try
                {
                    var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == XBatchNumber).FirstOrDefault();
                    var PeriodCode = 0;
                    if (BatchDetails.LbPeriodCode.HasValue)
                        PeriodCode = BatchDetails.LbPeriodCode.Value;
                    //db.Entry(LCalc).State = EntityState.Modified;
                    //db.SaveChanges();

                    if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
                    {
                        Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LCalc", "UpdatePayeeAttachment", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
                    }
                    else
                    {
                        string Query = "update {Schema}.XCalc set PayeeAttachmentId=" + AttachmentId + " where XAlteryxTransactionNumber=" + Id + " and XBatchNumber=" + XBatchNumber + " and XPeriodCode=" + PeriodCode;
                        Globals.RunUpdateQueryInOpcoDB(CompanyId, Query, BatchDetails.LbCommissionPeriod);
                        // var BatchData = Globals.GetQueryResultFromOpcoDB(CompanyId, "select XBatchNumber from {Schema}.XCalc where XAlteryxTransactionNumber=" + Id);
                        //if (BatchData.Rows.Count > 0)
                        UpdateBatchesAndSendEmail(XBatchNumber, UserName, "AttachNotifyRA", LoggedInUserId, string.Empty, "Attached By ");
                    }
                }
                catch (Exception ex)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
                }
            }
            return Ok();

        }

        //This method updates the comments Column value for the selected Payee records.//
        [HttpGet]
        public IHttpActionResult UpdateComments(string SelectedData, string Comments, string UserName, string Workflow, string LoggedInUserId, int CompanyId)
        {
            string[] s = SelectedData.Split(',');
            for (int j = 0; j < s.Length; j = j + 2)
            {
                var Id = Convert.ToInt32(s[j]);
                var XBatchNumber = Convert.ToInt32(s[j + 1]);
                //LCalc LCalc = db.LCalcs.Find(Id);
                //if (LCalc == null)
                //{
                //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "CALC")));
                //}
                //LCalc.LcComments = Comments;
                try
                {
                    var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == XBatchNumber).FirstOrDefault();
                    var PeriodCode = 0;
                    if (BatchDetails.LbPeriodCode.HasValue)
                        PeriodCode = BatchDetails.LbPeriodCode.Value;
                    //db.Entry(LCalc).State = EntityState.Modified;
                    //db.SaveChanges();
                    if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
                    {
                        Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LCalc", "UpdateComments", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
                    }
                    else
                    {
                        string Query = "update {Schema}.XCalc set SalesComments='" + Comments + "' where XAlteryxTransactionNumber=" + Id + " and XBatchNumber=" + XBatchNumber + " and XPeriodCode=" + PeriodCode;
                        Globals.RunUpdateQueryInOpcoDB(CompanyId, Query, BatchDetails.LbCommissionPeriod);
                        // var BatchData = Globals.GetQueryResultFromOpcoDB(CompanyId, "select XBatchNumber from {Schema}.XCalc where XAlteryxTransactionNumber=" + Id);
                        // if (BatchData.Rows.Count > 0)
                        UpdateBatchesAndSendEmail(XBatchNumber, UserName, "LineCommentsNotifyRA", LoggedInUserId, string.Empty, "Line level comments By ");
                    }
                }
                catch (Exception ex)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
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

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //call Globals.ExecuteSPLogError SP here and log SQL SqEx.Message
            //Add complete Url in description
            var UserName = "";//System.Web.HttpContext.Current.Session["UserName"] as string;
            string UrlString = Convert.ToString(Request.RequestUri.AbsolutePath);
            var ErrorDesc = "";
            var Desc = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
            if (Desc.Count() > 0)
                ErrorDesc = string.Join(",", Desc);
            //This array will provide controller name at 2nd and action name at 3 rd index position
            string[] s = Request.RequestUri.AbsolutePath.Split('/');
            Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
            //Globals.LogError(SqEx.Message, ErrorDesc);
            return Globals.SomethingElseFailedInDBErrorMessage;
        }

        //Method to handle notifications part in Payee calc

        private void UpdateBatchesAndSendEmail(int BatchNumber, string UserName, string TemplateName, string LoggedInUserId, string Comments, String ActionPrefix)
        {
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == BatchNumber).FirstOrDefault();
            if (BatchDetails != null)
            {
                //The batch must not be modified i.e no email added and lbatches updated yet
                if (BatchesAlreadyModified.Contains(BatchDetails.LbBatchNumber) == false)
                {

                    var Receiver =
                  //update LBatches record
                  BatchDetails.A01 = ActionPrefix + UserName;
                    db.Entry(BatchDetails).State = EntityState.Modified;
                    db.SaveChanges();
                    var CurrentOwner = db.AspNetUsers.Where(p => p.Id == BatchDetails.WFCurrentOwnerId).FirstOrDefault();
                    //var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
                    var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
                    var ReceiverEmail = "";
                    switch (ProjectEnviournment)
                    {
                        case "Prod":
                            ReceiverEmail = CurrentOwner.Email;
                            break;
                        default:
                            ReceiverEmail = ConfigurationManager.AppSettings["TestEmail"];
                            break;
                    }
                    //Add entry in email bucket
                    var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == TemplateName).Where(p => p.LetCompanyId == BatchDetails.LbCompanyId).FirstOrDefault();
                    if (EmailTemplate != null && CurrentOwner != null)
                    {
                        var EmailBody = EmailTemplate.LetEmailBody;
                        EmailBody = EmailBody.Replace("###BatchName###", BatchDetails.LbBatchName == null ? string.Empty : BatchDetails.LbBatchName);
                        EmailBody = EmailBody.Replace("###Batch Name###", BatchDetails.LbBatchName == null ? string.Empty : BatchDetails.LbBatchName);
                        EmailBody = EmailBody.Replace("###UserEmailId of the user who accepted###", UserName);
                        EmailBody = EmailBody.Replace("###UserEmailId of the user who added comments###", UserName);
                        EmailBody = EmailBody.Replace("###UserEmailId of the user who attached###", UserName);
                        EmailBody = EmailBody.Replace("###BatchNumber###", BatchDetails.LbBatchNumber.ToString());
                        var EmailSubject = EmailTemplate.LetEmailSubject;
                        EmailSubject = EmailSubject.Replace("###Batch Name###", BatchDetails.LbBatchName == null ? string.Empty : BatchDetails.LbBatchName);
                        string LoggesinUserIDEnc = db.AspNetUsers.Where(x => x.Email == LoggedInUserId).Select(x => x.Id).FirstOrDefault();
                        Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody + "<br>" + Comments, true, "Notifier", "Normal", null, "InQueue", null, LoggesinUserIDEnc, LoggesinUserIDEnc, "Test Vodafone Lite SES", "", "", "");
                    }
                    BatchesAlreadyModified.Add(BatchDetails.LbBatchNumber);
                }
            }
        }
        //Method to convert Model object of calc report to a datatable
        //public static DataTable ConvertModelToDatatable(List<GetCalcReport_Result> data)
        //{
        //    PropertyDescriptorCollection props =
        //        TypeDescriptor.GetProperties(typeof(GetCalcReport_Result));
        //    DataTable table = new DataTable();
        //    for (int i = 0; i < props.Count; i++)
        //    {
        //        PropertyDescriptor prop = props[i];
        //        var ColumnName = (prop.Name as string).Replace("Lc","");
        //        table.Columns.Add(ColumnName, typeof(String));
        //    }
        //    object[] values = new object[props.Count];
        //    foreach (GetCalcReport_Result item in data)
        //    {
        //        for (int i = 0; i < values.Length; i++)
        //        {
        //            values[i] = props[i].GetValue(item);
        //        }
        //        table.Rows.Add(values);
        //    }
        //    return table;
        //}

        [HttpGet]
        public IHttpActionResult CheckCompanySpecificMappedorNot(string Type, int CompanyId)
        {
            var CompanySpecificColumnsList = db.LCompanySpecificColumns.Where(p => p.LcscCompanyId == CompanyId).Where(p => p.LcscTableName == Type & p.LcscDisplayOnForm == true).Select(p => new CompanySpecificColumnViewModel { ColumnName = p.LcscColumnName, LcscLabel = p.LcscLabel }).ToList();
            for (var i = 0; i < CompanySpecificColumnsList.Count(); i++)
            {
                CompanySpecificColumnsList[i].LcscColumnName = CompanySpecificColumnsList.ElementAt(i).ColumnName + " as [" + ((string.IsNullOrEmpty(CompanySpecificColumnsList.ElementAt(i).LcscLabel)) ? CompanySpecificColumnsList.ElementAt(i).ColumnName.Replace("X", "") : CompanySpecificColumnsList.ElementAt(i).LcscLabel) + "]";
            }
            string ColumnList = String.Join(",", CompanySpecificColumnsList.Select(p => p.LcscColumnName));

            return Ok(ColumnList);

        }

    }
}
