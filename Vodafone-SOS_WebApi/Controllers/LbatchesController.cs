using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using System.Collections.Generic;
using System.Configuration;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LbatchesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        //This method is used to Load Batch List on the Dropdown displayed in Payee Calc Review and Pay Calc Review as per Commission Period Selected
        public IHttpActionResult GetByCommPeriodIdList(string CommissionPeriodIdList,int CompanyId, string UserName, string Workflow,string PortfolioList,string LoggedInRoleId,string LoggedInUserId,string LoggedInRoleName)
        {
            var WFStatus = "'Prelim'";
            var CommPeriodList = "";
            int PeriodCode = 0;
            string CommissionPeriod = "";
            var substr = "";

            var PayeeCodeList = "";
            string ChildPayeeStr = "";
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
            PayeeCodeList = PayeeCodeList.TrimEnd(',');//trim comma , from end as there would be extra comma ,

            if (CommissionPeriodIdList == null || CommissionPeriodIdList.Equals(""))
            {
                CommPeriodList = "";
            }
            else
            {
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
                 substr = " AND lb.LbCommissionPeriod in (" + CommPeriodList + ")";
            }
            //LoggedIn User Portfolios
            var LoggedInUser = db.AspNetUsers.Where(p => p.Email == UserName).FirstOrDefault();
            string UserPortfolioList = null;
            //Get list of portfolios associated with user
            var LUserRecord = db.LUsers.Where(p => p.LuUserId == LoggedInUser.Id).FirstOrDefault();
            if (LUserRecord == null)//User must be a Payee if record not present in LUsers
            {//--SG100620202 - WFSTatus check added
                var PayeeRecord = db.LPayees.Where(p => p.LpUserId == LoggedInUser.Id).Where(p=>p.WFStatus == "Completed").FirstOrDefault();
                UserPortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityType == "LPayees").Where(p => p.MepEntityId == PayeeRecord.Id).Where(p=>p.MepRoleId==LoggedInRoleId).Select(p => p.MepPortfolioId).ToList());
            }
            else
            {
                UserPortfolioList = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityType == "LUsers").Where(p => p.MepEntityId == LUserRecord.Id).Where(p => p.MepRoleId == LoggedInRoleId).Select(p => p.MepPortfolioId).ToList());
            }



            //Will replace * with column names as it is not working with selected columns as of now
            //var BatchQry = "select * from LBatches lb where lb.LbBatchType = 'CALC' And LbCompanyId=" + CompanyId+" AND WFStatus in (" + WFStatus + ") and Id in (select MepEntityId from MEntityPortfolios where MepEntityType='LBatches' and MepPortfolioId in (SELECT CAST(Item AS INTEGER) FROM dbo.SplitString( '" + UserPortfolioList + "', ',')))" + substr;
            // var BatchQry = "select lb.LbBatchNumber from LBatches lb where lb.LbCommissionPeriod in ('" + CommissionPeriod + "') AND lb.LbBatchType = 'CALC' AND WFStatus in ('" + WFStatus + "')";
            // var BatchNumberList = db.Database.SqlQuery<LBatch>(BatchQry).ToList();
            //NEW Logic
            //Getting list of Batch Nos
            var BatchQry = string.Empty;
            DataTable BatchNumberDT = new DataTable();
            if (LoggedInRoleName == "Payee" || LoggedInRoleName == "Channel Manager")
            {
                // NOTE: Payee and CM can see Calc data across all channel/schemes as long as the Calc has the PayeeCode from their dropdown (direct match or child payee)
                //BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') ";
                //QUERY Corrected as per logic
                //As XSchemaGR is taking toolong to respond using LBatches here And will use XSchem<opco> only in AcceptAll and AttachAll
                //First Get the Batch list for the Opco
                if (CommPeriodList != "")
                {
                    BatchQry = "select distinct lb.LbBatchNumber from LBatches lb  where lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') and WFCompanyId=" + CompanyId;
                    var BatchNumberList = db.Database.SqlQuery<int>(BatchQry).ToList();
                    var BatchNumberStr = "";
                    if (BatchNumberList.Count() > 0)
                    {
                        foreach (var BatchNumber in BatchNumberList)
                    {
                        BatchNumberStr += BatchNumber + ",";
                    }
                    BatchNumberStr = BatchNumberStr.TrimEnd(',');
                    if (BatchNumberStr.Equals(""))
                    {
                        BatchNumberStr = "''";
                    }
                    //Now shortlist that Batch List to the batches that belong to the selected payees only.
                    if(ChildPayeeStr + PayeeCodeList == "")//when there is no payee which has this channel manager assigned
                    {
                        BatchQry = "select distinct XBatchNumber from {Schema}.XCalc where  1=2";
                    }
                    else
                    {
                        BatchQry = "select distinct XBatchNumber from {Schema}.XCalc where XPayee in("
                             + ChildPayeeStr + PayeeCodeList + ")  and XPeriodCode=" + PeriodCode + " and XBatchNumber in (" + BatchNumberStr + ")";
                    }
                    
                    BatchNumberDT = Globals.GetQueryResultFromOpcoDB(CompanyId, BatchQry, CommissionPeriod);
                    BatchNumberStr = "";
                    foreach (DataRow BatchNumber in BatchNumberDT.Rows)
                    {
                        BatchNumberStr += BatchNumber.Field<int>("XBatchNumber") + ",";
                    }
                    BatchNumberStr = BatchNumberStr.TrimEnd(',');
                    //Get Othere details like BatchName etc from LBatches for the shortlisted batches(Needed for Batch level Dropdown).
                    //The following condition is to prevent the query from failing while execution where it says incorrect syntax near')' as there is no batch number for "IN condition.

                    if (BatchNumberStr != "")
                    {
                        BatchQry = "select * from LBatches where LbBatchNumber in (" + BatchNumberStr + ")";
                    }
                    else //For that period, if no batches where found
                    {

                        BatchQry = "select * from LBatches where 1=2";
                    }
                        var BatchList = db.Database.SqlQuery<LBatch>(BatchQry).ToList();
                        return Ok(BatchList);
                    }
                }
                //else //If commission period list is null (i,e; user has just landed on the page and not selected any period yet) then do not consider period as it will give a syntax error otherwise. Since even on initial landing on the page, this method is being called.
                //{
                //    BatchQry = "select * from LBatches lb  where 1=2";
                //    //lb.LbBatchType = 'CALC' AND WFStatus in ('Prelim') and WFCompanyId=" + CompanyId
                //}

                return Ok(new List<LBatch>());
            }
            else //If role is any other than Payee/Channel Manager
            {
                /*If any channel is selected
           Add that portfolio to portfolio matching during batch selection during the query 
           to populate Calc grid for that period*/
                if (CommPeriodList !="")
                {
                    BatchQry = "select lb.* from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (select Id from LPortfolios where LpChannelId in (select LpChannelId from LPortfolios where Id in (" + UserPortfolioList + ") )) and  lb.LbCommissionPeriod in (" + CommPeriodList + ") AND lb.LbBatchType = 'CALC' AND WFStatus in (" + WFStatus + ") ";
                }
                else //If commission period list is null (i,e; user has just landed on the page and not selected any period yet) then do not consider period as it will give a syntax error otherwise. Since even on initial landing on the page, this method is being called.
                {
                    //RK removed the query condition --and  lb.LbCommissionPeriod in (" + CommPeriodList + ")
                    BatchQry = "select lb.* from LBatches lb inner join MEntityPortfolios MEP on MEP.MepEntityId=lb.Id  where MepEntityType='LBatches' and MepPortfolioId in (select Id from LPortfolios where LpChannelId in (select LpChannelId from LPortfolios where Id in (" + UserPortfolioList + ") ))  AND lb.LbBatchType = 'CALC' AND WFStatus in (" + WFStatus + ") AND 1=2 ";
                }
                var BatchNumberList = db.Database.SqlQuery<LBatch>(BatchQry).ToList();
                return Ok(BatchNumberList);
            }

           
        }

        

            // GET: api/LBatches
        //    public IHttpActionResult GetLBatches()
        //{
        //    var xx = (from aa in db.LBatches
        //              select new {
        //                  aa.Id,
        //                  aa.LbBatchNumber,
        //                  aa.LbBatchType,
        //                  aa.LbRecordCount,
        //                  aa.LbUploadStartDateTime,
        //                  aa.LbUploadFinishDateTime, 
        //                  aa.LbAlteryxBatchNumber,
        //                  aa.LbStatus,
        //                  aa.LbRawDataTableId}).OrderByDescending(p=>p.LbBatchNumber);
        //    return Ok(xx);
        //}

        // GET: api/LBatches?CompanyId=5
        public IHttpActionResult GetLBatchesByCompanyId(int CompanyId, string UserName, string Workflow)
        {
            var xx = (from aa in db.LBatches.Where(p=>p.LbCompanyId==CompanyId).Include(c => c.GCompany)
                      select new {
                          aa.Id,
                          aa.LbBatchNumber,
                          aa.LbBatchType,
                          aa.LbRecordCount,
                          aa.LbUploadStartDateTime,
                          aa.LbUploadFinishDateTime,
                          aa.LbAlteryxBatchNumber,
                          aa.GCompany.GcCompanyName,
                          aa.LbRawDataTableId }).OrderByDescending(p => p.LbBatchNumber);
            return Ok(xx); 
        }


        /*** Probably below method is no longer required hence commented by VG on 23Jan2017 ***/
        //// GET: api/LBatches?CompanyId=5
        //public IHttpActionResult GetLBatchesByCompanyIdByStatusId(int CompanyId, int StatusId)
        //{
        //    var xx = (from aa in db.LBatches.Where(p => p.LbCompanyId == CompanyId && p.LbStatusId == StatusId).Include(c => c.GCompany).Include(c => c.RStatus).Include(c => c.LRawDataTable)
        //              select new {
        //                  aa.Id,
        //                  aa.LRawDataTable.LrdtName,
        //                  aa.LbBatchNumber,
        //                  aa.LbBatchType,
        //                  aa.LbRecordCount,
        //                  aa.LbUploadStartDateTime,
        //                  aa.LbUploadFinishDateTime,
        //                  aa.LbAlteryxBatchNumber, 
        //                  aa.GCompany.GcCompanyName,
        //                  aa.RStatus.RsStatus }).OrderBy(p => p.LrdtName).OrderBy(p => p.LbUploadStartDateTime);
        //    return Ok(xx);
        //}

        // GET: api/LBatches?CompanyId=5
        public IHttpActionResult GetLBatchesByCompanyIdBatchTypeBatchStatus(int CompanyId, string BatchType, string BatchStatus, string AspnetUserid, bool IsManager, string UserName, string Workflow)
        {
            //Frame the common part of the query which is applicable to Manager as well as ReportingAnalyst
            string Qry = "";
            if (BatchType.Equals("RawData"))
            {
                Qry = "select B.Id, B.LbRawDataTableId AS LbRawDataTableId , B.LbCommissionPeriod, REPLACE (T.LrdtName, 'XRawData_', '') As LrdtName, B.LbBatchNumber, B.LbRecordCount, B.LbUploadStartDateTime ";
                Qry = Qry + "from LBatches B inner join LRawDataTables T on B.LbRawDataTableId = T.Id ";//if(BatchType==RawData) ? if(Batchtype==Calc)then remove the join
            }
            else if(BatchType.Equals("Calc") || BatchType.Equals("Pay"))
            {
                Qry = "select B.Id, B.LbRawDataTableId AS LbRawDataTableId , B.LbCommissionPeriod, B.LbBatchNumber, B.LbRecordCount, B.LbUploadStartDateTime ";
                Qry = Qry + "from LBatches B  "; //if(Batchtype==Calc or Pay)then remove the join
            }

            Qry = Qry + "where B.LbCompanyId = " + CompanyId + " AND B.LbBatchType = '" + BatchType + "' AND B.LbStatus = '" + BatchStatus + "' ";

            if (IsManager)  //If Manager then retrieve all the records from Lbatches table where LbAllocatedToId is of those persons who reports to this manager (Id passed in AspnetUserId parameter).
                Qry = Qry + "and LbAllocatedToId in (select LuUserid from LUsers where LuReportsToId = '" + AspnetUserid + "') ";
            else //If not manager (i.e. If Reporting Analyst) then retrieve all the records from LBatches table where LbAllocatedToId is equal to the Id passed in AspnetUserId parameter.
                Qry = Qry + "and LbAllocatedToId =  '" + AspnetUserid + "' ";

            //Add an Order By clause to the query
            if (BatchType.Equals("RawData"))
            {
                Qry = Qry + "Order by B.LbCommissionPeriod, T.LrdtName, B.LbUploadStartDateTime";
            }
            else if (BatchType.Equals("Calc"))
            {
                Qry = Qry + "Order by B.LbCommissionPeriod, B.LbUploadStartDateTime";
            }
                var xx = db.Database.SqlQuery<LBatchViewModelForAnalystGrid>(Qry);
            return Ok(xx);
        }


        // GET: api/LBatches?CompanyId=5
        public IHttpActionResult GetLBatchesByCompanyIdBatchTypeBatchStatusPeriodStatus(int CompanyId, string BatchType, string BatchStatus, string PeriodStatus, string AspnetUserid, bool IsManager, string UserName, string Workflow)
        {
            //Frame the common part of the query which is applicable to Manager as well as ReportingAnalyst
            string Qry = "select B.Id, B.LbRawDataTableId AS LbRawDataTableId, B.LbCommissionPeriod, REPLACE (T.LrdtName, 'XRawData_', '') As LrdtName, B.LbBatchNumber, B.LbRecordCount, B.LbUploadStartDateTime ";
            //Qry = Qry + "from LBatches B inner join LRawDataTables T on B.LbRawDataTableId = T.Id ";
            Qry = Qry + "from LBatches B left outer join LRawDataTables T on B.LbRawDataTableId = T.Id ";
            Qry = Qry + "where B.LbCompanyId = " + CompanyId + " AND B.LbBatchType = '" + BatchType + "' AND B.LbStatus = '" + BatchStatus + "' ";
            Qry = Qry + "and LbCommissionPeriod in 	(select [LcpPeriodName] from LCommissionPeriods where LcpStatus in('" + PeriodStatus + "')) ";

            if (IsManager)  //If Manager then retrieve all the records from Lbatches table where LbAllocatedToId is of those persons who reports to this manager (Id passed in AspnetUserId parameter).
                Qry = Qry + "and LbAllocatedToId in (select LuUserid from LUsers where LuReportsToId = '" + AspnetUserid + "') ";
            else //If not manager (i.e. If Reporting Analyst) then retrieve all the records from LBatches table where LbAllocatedToId is equal to the Id passed in AspnetUserId parameter.
                Qry = Qry + "and LbAllocatedToId =  '" + AspnetUserid + "' ";

            //Add an Order By clause to the query
            Qry = Qry + "Order by B.LbCommissionPeriod, T.LrdtName, B.LbUploadStartDateTime";
            var xx = db.Database.SqlQuery<LBatchViewModelForAnalystGrid>(Qry);
            return Ok(xx);
        }

        public IHttpActionResult GetCalcBatchGridForPayee(int CompanyId, string BatchType, string BatchStatus, string PeriodStatus, string AspnetUserid, string UserName, string Workflow)
        {
            //Frame the common part of the query which is applicable to Manager as well as ReportingAnalyst
            string Qry = "select B.Id, B.LbRawDataTableId AS LbRawDataTableId, B.LbCommissionPeriod, REPLACE (T.LrdtName, 'XRawData_', '') As LrdtName, B.LbBatchNumber, B.LbRecordCount, B.LbUploadStartDateTime ";
            //Qry = Qry + "from LBatches B inner join LRawDataTables T on B.LbRawDataTableId = T.Id ";
            Qry = Qry + "from LBatches B left outer join LRawDataTables T on B.LbRawDataTableId = T.Id ";
            Qry = Qry + "where B.LbCompanyId = " + CompanyId + " AND B.LbBatchType = '" + BatchType + "' AND B.LbStatus = '" + BatchStatus + "' ";
            Qry = Qry + "and LbCommissionPeriod in 	(select [LcpPeriodName] from LCommissionPeriods where LcpStatus in(" + PeriodStatus + ")) ";

            //Qry = Qry + "and LbAllocatedToId =  '" + AspnetUserid + "' ";

            //Add an Order By clause to the query
            Qry = Qry + "Order by B.LbCommissionPeriod, T.LrdtName, B.LbUploadStartDateTime";
            var xx = db.Database.SqlQuery<LBatchViewModelForAnalystGrid>(Qry);
            return Ok(xx);
        }
        
        [HttpGet]
        public IHttpActionResult GetByUserForPayeeUploadGrid(int CompanyId, string AspnetUserid)
        {
            string Qry = "select Case lb.LbStatus WHEN 'ValidationFailed' Then 0 else 1 END as IsImport,lb.Id,lb.LbStatus,lb.LbBatchNumber,lb.LbRecordCount,lb.LbUploadStartDateTime,lbf.LbfFileName from LBatches lb " +
                " join LBatchFiles lbf on  lb.id = lbf.LbfBatchId where lb.WFType='LPayees' and lb.LbCompanyId = {0} and lb.WFRequesterId={1} and lb.LbStatus <> 'Deleted' order by lb.Id desc";
            var xx = db.Database.SqlQuery<LBatchViewModelForPayeeGrid>(Qry,CompanyId,AspnetUserid).ToList();
            return Ok(xx);
        }
        [HttpGet]
        public IHttpActionResult GetById(int CompanyId, int Id)
        {
            string Qry = "select lb.Id,lb.LbStatus,lb.LbBatchNumber,lb.LbRecordCount,lb.LbUploadStartDateTime,lbf.LbfFileName from LBatches lb " +
                " join LBatchFiles lbf on  lb.id = lbf.LbfBatchId where lb.WFType='LPayees' and lb.LbCompanyId = {0} and lb.Id={1}";
            var xx = db.Database.SqlQuery<LBatchViewModelForPayeeGrid>(Qry, CompanyId, Id).FirstOrDefault();
            return Ok(xx);
        }

        // GET: api/LBatches/5
        [ResponseType(typeof(LBatch))]
        public async Task<IHttpActionResult> GetLBatch(int id, string UserName, string Workflow)
        {
            var LBatch = db.LBatches.Where(p => p.Id == id).Include(c => c.GCompany).Select(aa => new 
            { 
                aa.Id, aa.LbBatchName,
                aa.LbBatchNumber, 
                aa.LbBatchType, 
                aa.LbRecordCount, 
                aa.LbUploadStartDateTime, 
                aa.LbUploadFinishDateTime, 
                aa.LbAlteryxBatchNumber, 
                aa.GCompany.GcCompanyName, 
                aa.LbStatus,
                aa.LbCompanyId,
                aa.LbParentBatchId,
                aa.LbUpdatedBy,
                aa.LbComments,
                aa.LbRawDataTableId,
                aa.LbCommissionPeriod
                ,aa.WFStatus,aa.WFComments,
                LrdtName=(aa.LRawDataTable!=null)?aa.LRawDataTable.LrdtName:""
            }).FirstOrDefault();
            if (LBatch == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Batch")));
            }
            return Ok(LBatch);
        }
        [ResponseType(typeof(LBatch))]
        public async Task<IHttpActionResult> GetLBatchByBatchNumber(int BatchNumber, string UserName, string Workflow)
        {
            var LBatch = db.LBatches.Where(p => p.LbBatchNumber==BatchNumber).Include(c => c.GCompany).Select(aa => new
            {
                aa.Id,
                aa.LbBatchNumber,
                aa.LbBatchType,
                aa.LbRecordCount,
                aa.LbUploadStartDateTime,
                aa.LbUploadFinishDateTime,
                aa.LbAlteryxBatchNumber,
                aa.GCompany.GcCompanyName,
                aa.LbStatus,
                aa.LbCompanyId,
                aa.LbParentBatchId,
                aa.LbUpdatedBy,
                aa.LbComments,
                aa.LbRawDataTableId,
                aa.LbCommissionPeriod,
               // aa.LbAllocatedToId,
                aa.WFComments
            }).FirstOrDefault();
            if (LBatch == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Batch")));
            }
            return Ok(LBatch);
        }

        //method to update status 
        [HttpPut]
        public async Task<IHttpActionResult> UpdateBatchStatus(int SOSBatchNumber, string BatchLevelComments, string NewStatus, string UserName, string Workflow)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //Update LBatch 
                    var SosBatch = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
                    SosBatch.LbStatus = NewStatus;
                    if(!string.IsNullOrEmpty(BatchLevelComments))
                    {
                        SosBatch.LbComments = BatchLevelComments + Environment.NewLine + SosBatch.LbComments;
                    }
                    db.Entry(SosBatch).State = EntityState.Modified;
                   await db.SaveChangesAsync();

                    //update XBatches table so that Alteryx can monitor the progress. No need to update comments column as they are internal to SOS 
                    var XBatch = db.XBatches.Where(p=>p.XBatchNumber==SOSBatchNumber).FirstOrDefault();
                    XBatch.XStatus= NewStatus;
                    db.Entry(XBatch).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    transaction.Commit();
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

        //method to update BatchLevelComments 
        [HttpPut]
        public async Task<IHttpActionResult> UpdateBatchLevelComments(int SOSBatchNumber, string BatchLevelComments,  string UserName, string Workflow,string LoggedInUserId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //Update LBatch 
                    var SosBatch = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
                    if (!string.IsNullOrEmpty(BatchLevelComments))
                    {
                        SosBatch.WFComments = "[" + UserName + "] [" + DateTime.UtcNow + "] " + BatchLevelComments + Environment.NewLine + SosBatch.WFComments;
                        SosBatch.A01 = "Batch level comments by " + UserName;
                    }
                    db.Entry(SosBatch).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    if (SosBatch != null)
                    {
                            var CurrentOwner = db.AspNetUsers.Where(p => p.Id == SosBatch.WFCurrentOwnerId).FirstOrDefault();
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
                            var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "BatchCommentsNotifyRA").Where(p => p.LetCompanyId == SosBatch.LbCompanyId).FirstOrDefault();
                            if (EmailTemplate != null && CurrentOwner != null)
                            {
                                var EmailBody = EmailTemplate.LetEmailBody;
                                EmailBody = EmailBody.Replace("###BatchName###", SosBatch.LbBatchName == null ? string.Empty : SosBatch.LbBatchName);
                                EmailBody = EmailBody.Replace("###UserEmailId of the user who added comments###", UserName);
                                EmailBody = EmailBody.Replace("###BatchNumber###", SosBatch.LbBatchNumber.ToString());
                                var EmailSubject = EmailTemplate.LetEmailSubject;
                                EmailSubject = EmailSubject.Replace("###BatchName###", SosBatch.LbBatchName == null ? string.Empty : SosBatch.LbBatchName);
                                Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody+"<br>"+ SosBatch.WFComments, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES","","","");
                            }
                    }
                    transaction.Commit();
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


        // PUT: api/LBatches/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLBatch(int Id, LBatch LBatch, string UserName, string Workflow)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Batch")));
            }

            if (Id != LBatch.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "Batch")));
            }
            try
            {
                db.Entry(LBatch).State = EntityState.Modified;
                await db.SaveChangesAsync();
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
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LBatches
        [ResponseType(typeof(LBatch))]
        public async Task<IHttpActionResult> PostLBatch(LBatch LBatch, string UserName, string Workflow)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Batch")));
            }
            try
            {
                db.LBatches.Add(LBatch);
                await db.SaveChangesAsync();
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
            return CreatedAtRoute("DefaultApi", new { id = LBatch.Id }, LBatch);
        }

        // DELETE: api/LBatches/5
        [ResponseType(typeof(LBatch))]
        public async Task<IHttpActionResult> DeleteLBatch(int id, string UserName, string Workflow)
        {
            LBatch LBatch = await db.LBatches.FindAsync(id);
            if (LBatch == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Batch")));
            }
            try
            {
                db.LBatches.Remove(LBatch);
                await db.SaveChangesAsync();
                return Ok(LBatch);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LBatchExists(int id)
        {
            return db.LBatches.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            ////Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("FNAllocateCommisionBatch", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "BATCH", "DATABASE OBJECTS"));
            else if (SqEx.Message.IndexOf("VBatches", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "BATCH", "VIEW"));
            else if (SqEx.Message.IndexOf("VPayees", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "BATCH", "VIEW(S)"));
            else if (SqEx.Message.IndexOf("FK_LBatches_LBatchFiles", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "BATCH", "BATCH FILES"));


            ////Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("UQ_LBatches_LbBatchNumber_LbCompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return ("Can not insert duplicate Batch");
            //else if (SqEx.Message.IndexOf("FK_LBatches_LBatches_LbParentBatchId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Batch", "Child Batch"));
            //else if (SqEx.Message.IndexOf("FK_LBatches_LRawData", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Batch", "RAW DATA"));
            //else
            //    //Something else failed return original error message as retrieved from database
            //    return SqEx.Message;
            else
            {
                //Something else failed return original error message as retrieved from database
                //callGlobals.ExecuteSPLogError SP here and log SQL SqEx.Message
                //Add complete Url in description
                var UserName = "";//System.Web.HttpContext.Current.Session["UserName"] as string;
                string UrlString = Convert.ToString(Request.RequestUri.AbsolutePath);
                var ErrorDesc = "";
                var Desc = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                if (Desc.Count() > 0)
                    ErrorDesc = string.Join(",", Desc);
                string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3 rd index position
               Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
                //Globals.LogError(SqEx.Message, ErrorDesc);
                return Globals.SomethingElseFailedInDBErrorMessage;
            }
        }
    }
}
