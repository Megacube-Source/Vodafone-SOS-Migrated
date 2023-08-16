using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
using System.Collections;
using System.Configuration;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LReadyToPayController : ApiController
    {
        // GET: XReadyToPay

        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        public object SqlMethods { get; private set; }

        //get list of payee documents for the batch in Pay
        [HttpGet]
        public IHttpActionResult GetXPayeeDocuments(string CompanyCode, int BatchNumber)
        {
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == BatchNumber).FirstOrDefault();
            if (BatchDetails != null)
            {
                var PayeeDocQuery = "select XFileName,XFileLocation,XPayeeCode from [XSchema" + CompanyCode + "].XPayeeDocuments where XBatchNumber={0}";
                var XPayeeDetails = db.Database.SqlQuery<XPayeeDocumentsViewModel>(PayeeDocQuery, BatchDetails.LbBatchNumber).ToList();
                return Ok(XPayeeDetails);
            }
            return Ok();
        }
        //This method will return Payee List based on Portfolio matching 
        [HttpGet]
        public IHttpActionResult GetPayeeDocumentpaging(string CompanyCode, int BatchNumber, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, string PayeeCode)
        {
            //Section to get Payee Data from Stored Procedure starts Here
            //SPGetPayeeData
            var Query = "Exec SPGetPayeeDocumentData  @companycode,@PaymentBatchNO,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,@PayeeCode";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@companycode", CompanyCode);
            cmd.Parameters.AddWithValue("@PaymentBatchNO", BatchNumber);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            cmd.Parameters.AddWithValue("@PayeeCode", string.IsNullOrEmpty(PayeeCode) ? (object)System.DBNull.Value : PayeeCode);
            DataSet ds = Globals.GetData(cmd);
            return Ok(ds.Tables[0]);
        }



        public IHttpActionResult GetCalcAndManualAdjBatches(string strType, int iCompanyId)
        {
            if (strType != "Claims")
            {
                var xx = (from aa in db.LBatches
                          where
                           aa.WFStatus == "Completed" && aa.WFType == strType && aa.WFCompanyId == iCompanyId
                          select new
                          {
                              aa.Id,
                              aa.LbBatchNumber,
                              aa.LbBatchName,
                              aa.LbCommissionPeriod
                          }).OrderBy(p => p.LbBatchNumber);
                return Ok(xx);
            }
            else
            {
                var xx = (from C in db.LClaims
                          join P in db.LPayees on C.LcPayeeId equals P.Id
                          join R in db.RRejectionReasons on C.LcRejectionReasonId equals R.Id into PS
                          from R in PS.DefaultIfEmpty()
                          let PayeeName = P.LpFirstName + " " + P.LpLastName + " (" + P.LpPayeeCode + ")"
                          where
                           C.WFStatus == "Completed" && C.WFCompanyId == iCompanyId
                          select new
                          {
                              C.Id,
                              C.LcClaimId,
                              C.LcPayeeId,
                              PayeeName,
                              C.LcPaymentAmount,
                              R.RrrReason
                          }).OrderBy(p => p.LcClaimId);
                return Ok(xx);
            }
        }

        [ResponseType(typeof(LReadyToPayViewModel))]

        //public async Task<IHttpActionResult> GetData(int iRTPID, int iCompanyID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strAction, string strPayBatchName, string strBatchCommPeriod, string strRTPData, string strCreatedBy, string strUpdatedBy, string strPortfolios)
        //{
        //    var RawQuery = db.Database.SqlQuery<LReadyToPayViewModel>("exec spReadyToPayData " + iRTPID.ToString() + "," + iCompanyID.ToString() + ",'" + blnIsBatchList + "','" + strType + "','" + strRTPStatus + "','" + strAction + "','" + strPayBatchName + "','" + strBatchCommPeriod + "','" + strRTPData + "','" + strCreatedBy + "','" + strUpdatedBy + "','" + strPortfolios + "','',0,0,null,null");
        //    var Task = RawQuery.ToList();
        //    var ListData = Task.ToList();
        //    return Ok(ListData);
        //}

        //GetDataCounts
        // public async Task<IHttpActionResult> GetDataCounts(int iRTPID, int iCompanyID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strAction, string strPayBatchName, string strBatchCommPeriod, string strRTPData, string strCreatedBy, string strUpdatedBy, string strPortfolios)
        [HttpPost]
        public async Task<IHttpActionResult> GetDataCounts(LReadyToPayCount objLReadyToPayCount)
        {
            int iRTPID = objLReadyToPayCount.iRTPID;
            int iCompanyID = objLReadyToPayCount.iCompanyID;
            Boolean blnIsBatchList = objLReadyToPayCount.blnIsBatchList;
            string strType = objLReadyToPayCount.strType;
            string strRTPStatus = objLReadyToPayCount.strRTPStatus;
            string strAction = objLReadyToPayCount.strAction;
            string strPayBatchName = objLReadyToPayCount.strPayBatchName;
            string strBatchCommPeriod = objLReadyToPayCount.strBatchCommPeriod;
            string strRTPData = objLReadyToPayCount.strRTPData;
            string strCreatedBy = objLReadyToPayCount.strCreatedBy;
            string strUpdatedBy = objLReadyToPayCount.strUpdatedBy;
            string strPortfolios = objLReadyToPayCount.strPortfolios;
            var RawQuery = db.Database.SqlQuery<LReadyToPayViewModel>("exec spReadyToPayDataCounts " + iRTPID.ToString() + "," + iCompanyID.ToString() + ",'" + blnIsBatchList + "','" + strType + "','" + strRTPStatus + "','" + strAction + "','" + strPayBatchName + "','" + strBatchCommPeriod + "','" + strRTPData + "','" + strCreatedBy + "','" + strUpdatedBy + "','" + strPortfolios + "','',0,0,null,null");
            var Task = RawQuery.ToList();
            var ListData = Task.ToList();
            var count = ListData.Count();
            return Ok(count);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetData(LReadyToPayCount objLReadyToPayCount)
        {
            //int iRTPID, int iCompanyID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strAction, string strPayBatchName, 
            //string strBatchCommPeriod, string strRTPData, string strCreatedBy, string strUpdatedBy, string strPortfolios, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery
            //var RawQuery = db.Database.SqlQuery<LReadyToPayViewModel>("exec spReadyToPayData " + iRTPID.ToString() + "," + iCompanyID.ToString() + ",'" + blnIsBatchList + "','" + strType + "','" + strRTPStatus + "','" + strAction + "','" + strPayBatchName + "','" + strBatchCommPeriod + "','" + strRTPData + "','" + strCreatedBy + "','" + strUpdatedBy + "','" + strPortfolios + "','',0,0,null,null, '" + sortdatafield + "','" + sortorder + "'," + pagesize + "," + pagenum + ",'" + FilterQuery + "'");
            //var Task = RawQuery.ToList();
            //var ListData = Task.ToList();
            //return Ok(ListData);
            int iRTPID = objLReadyToPayCount.iRTPID;
            int iCompanyID = objLReadyToPayCount.iCompanyID;
            Boolean blnIsBatchList = objLReadyToPayCount.blnIsBatchList;
            string strType = objLReadyToPayCount.strType;
            string strRTPStatus = objLReadyToPayCount.strRTPStatus;
            string strAction = objLReadyToPayCount.strAction;
            string strPayBatchName = objLReadyToPayCount.strPayBatchName;
            string strBatchCommPeriod = objLReadyToPayCount.strBatchCommPeriod;
            string strRTPData = objLReadyToPayCount.strRTPData;
            string strCreatedBy = objLReadyToPayCount.strCreatedBy;
            string strUpdatedBy = objLReadyToPayCount.strUpdatedBy;
            string strPortfolios = objLReadyToPayCount.strPortfolios;
            string sortdatafield = objLReadyToPayCount.sortdatafield;
            string sortorder = objLReadyToPayCount.sortdatafield;
            int pagesize = objLReadyToPayCount.pagesize;
            int pagenum = objLReadyToPayCount.pagenum;
            string FilterQuery = objLReadyToPayCount.FilterQuery;




            string PayPublishEmailIds = null;

            var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
            var tb = new DataTable();
            var Query = "exec spReadyToPayData @iRTPID,@iCompanyID,@blnIsBatchList,@strType,@strRTPStatus,@strAction,@strPayBatchName,@strBatchCommPeriod,@strRTPData,@strCreatedBy,@strUpdatedBy,@strPortfolios,@UserRole,@SendEmail,@SendPayeeDocument,@ProjectEnviournment,@PayPublishEmailIds,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,@IsClaimChanged ,@IsCalChanged ,@isMAChanged ";
            ////using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@iRTPID", iRTPID.ToString());
            cmd.Parameters.AddWithValue("@iCompanyID", iCompanyID.ToString());
            cmd.Parameters.AddWithValue("@blnIsBatchList", blnIsBatchList);
            cmd.Parameters.AddWithValue("@strType", string.IsNullOrEmpty(strType) ? (object)System.DBNull.Value : strType);
            cmd.Parameters.AddWithValue("@strRTPStatus", string.IsNullOrEmpty(strRTPStatus) ? (object)System.DBNull.Value : strRTPStatus);
            cmd.Parameters.AddWithValue("@strAction", string.IsNullOrEmpty(strAction) ? (object)System.DBNull.Value : strAction);
            cmd.Parameters.AddWithValue("@strPayBatchName", string.IsNullOrEmpty(strPayBatchName) ? (object)System.DBNull.Value : strPayBatchName);
            cmd.Parameters.AddWithValue("@strBatchCommPeriod", string.IsNullOrEmpty(strBatchCommPeriod) ? (object)System.DBNull.Value : strBatchCommPeriod);
            cmd.Parameters.AddWithValue("@strRTPData", string.IsNullOrEmpty(strRTPData) ? (object)System.DBNull.Value : strRTPData);
            cmd.Parameters.AddWithValue("@strCreatedBy", string.IsNullOrEmpty(strCreatedBy) ? (object)System.DBNull.Value : strCreatedBy);
            cmd.Parameters.AddWithValue("@strUpdatedBy", string.IsNullOrEmpty(strUpdatedBy) ? (object)System.DBNull.Value : strUpdatedBy);
            cmd.Parameters.AddWithValue("@strPortfolios", string.IsNullOrEmpty(strPortfolios) ? (object)System.DBNull.Value : strPortfolios);
            cmd.Parameters.AddWithValue("@UserRole", "");
            cmd.Parameters.AddWithValue("@SendEmail", 0);
            cmd.Parameters.AddWithValue("@SendPayeeDocument", 0);
            cmd.Parameters.AddWithValue("@ProjectEnviournment", ProjectEnviournment);
            cmd.Parameters.AddWithValue("@PayPublishEmailIds", "");
            //cmd.Parameters.AddWithValue("@PayPublishEmailIds", string.IsNullOrEmpty(PayPublishEmailIds) ? (object)System.DBNull.Value : PayPublishEmailIds);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            cmd.Parameters.AddWithValue("@IsClaimChanged", false);
            cmd.Parameters.AddWithValue("@IsCalChanged", false);
            cmd.Parameters.AddWithValue("@isMAChanged", false);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);

            //tb.Columns.Remove(tb.Columns["row"]);
            sda.Fill(tb);
            //tb.Columns.Remove("row");
            conn.Close();
            // return Ok(ds);
            return Ok(tb);

        }




        //[HttpGet] RK 11Feb2018, commented [HttpGet] as the method is supposed to perform post as well as get
        //[HttpGet]
        [HttpPost]
        public async Task<IHttpActionResult> SaveUpdateRTPData(LReadyToPaySaveModel objSaveModelData)
        {

            int iRTPID = objSaveModelData.iRTPID;
            int iCompanyID = objSaveModelData.iCompanyID;
            Boolean blnIsBatchList = objSaveModelData.blnIsBatchList;
            string strType = objSaveModelData.strType;
            string strRTPStatus = objSaveModelData.strRTPStatus;
            string strAction = objSaveModelData.strAction;
            string strPayBatchName = objSaveModelData.strPayBatchName;
            string strBatchCommPeriod = objSaveModelData.strBatchCommPeriod;
            string strRTPData = objSaveModelData.strRTPData;
            string strCreatedBy = objSaveModelData.strCreatedBy;
            string strUpdatedBy = objSaveModelData.strUpdatedBy;
            string strPortfolios = objSaveModelData.strPortfolios;
            bool EmailDocuments = objSaveModelData.EmailDocuments;
            bool SendPayeeDocuments = objSaveModelData.SendPayeeDocuments;
            string PayPublishEmailIds = objSaveModelData.PayPublishEmailIds;
            string UserRole = objSaveModelData.UserRole;
            Boolean IsClaimChanged = objSaveModelData.IsClaimChanged;
            Boolean IsCalChanged = objSaveModelData.IsCalChanged;
            Boolean isMAChanged = objSaveModelData.isMAChanged;

            string sortdatafield = null;
            string sortorder = null;
            int pagesize = 200;
            int pagenum = 0;
            string FilterQuery = null;

            var db = new SOSEDMV10Entities();
            int SendEmail = 0;
            int SendPayeeDocument = 0;
            if (EmailDocuments)
                SendEmail = 1;
            if (SendPayeeDocuments)
                SendPayeeDocument = 1;
            //var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
            var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
            // db.Database.ExecuteSqlCommand("exec spReadyToPayData " + iRTPID.ToString() + "," + iCompanyID.ToString() + ",'" + blnIsBatchList + "','" + strType + "','" + strRTPStatus + "','" + strAction + "','" + strPayBatchName + "','" + strBatchCommPeriod + "','" + strRTPData + "','" + strCreatedBy + "','" + strUpdatedBy + "','" + strPortfolios + "','"+UserRole+"'," + SendEmail+","+SendPayeeDocument+",'"+PayPublishEmailIds+"','"+ProjectEnviournment+"'");
            var Query = "exec spReadyToPayData @iRTPID,@iCompanyID,@blnIsBatchList,@strType,@strRTPStatus,@strAction,@strPayBatchName,@strBatchCommPeriod,@strRTPData,@strCreatedBy,@strUpdatedBy,@strPortfolios,@UserRole,@SendEmail,@SendPayeeDocument,@PayPublishEmailIds,@ProjectEnviournment,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,@IsClaimChanged ,@IsCalChanged ,@isMAChanged ";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@iRTPID", iRTPID.ToString());
            cmd.Parameters.AddWithValue("@iCompanyID", iCompanyID.ToString());
            cmd.Parameters.AddWithValue("@blnIsBatchList", blnIsBatchList);
            cmd.Parameters.AddWithValue("@strType", string.IsNullOrEmpty(strType) ? (object)System.DBNull.Value : strType);
            cmd.Parameters.AddWithValue("@strRTPStatus", string.IsNullOrEmpty(strRTPStatus) ? (object)System.DBNull.Value : strRTPStatus);
            cmd.Parameters.AddWithValue("@strAction", strAction);
            cmd.Parameters.AddWithValue("@strPayBatchName", string.IsNullOrEmpty(strPayBatchName) ? (object)System.DBNull.Value : strPayBatchName);
            cmd.Parameters.AddWithValue("@strBatchCommPeriod", string.IsNullOrEmpty(strBatchCommPeriod) ? (object)System.DBNull.Value : strBatchCommPeriod);
            cmd.Parameters.AddWithValue("@strRTPData", string.IsNullOrEmpty(strRTPData) ? (object)System.DBNull.Value : strRTPData);
            cmd.Parameters.AddWithValue("@strCreatedBy", strCreatedBy);
            cmd.Parameters.AddWithValue("@strUpdatedBy", strUpdatedBy);
            cmd.Parameters.AddWithValue("@strPortfolios", string.IsNullOrEmpty(strPortfolios) ? (object)System.DBNull.Value : strPortfolios);
            cmd.Parameters.AddWithValue("@UserRole", UserRole);
            cmd.Parameters.AddWithValue("@SendEmail", SendEmail);
            cmd.Parameters.AddWithValue("@SendPayeeDocument", SendPayeeDocument);
            cmd.Parameters.AddWithValue("@ProjectEnviournment", ProjectEnviournment);
            cmd.Parameters.AddWithValue("@PayPublishEmailIds", string.IsNullOrEmpty(PayPublishEmailIds) ? (object)System.DBNull.Value : PayPublishEmailIds);

            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            cmd.Parameters.AddWithValue("@IsClaimChanged", IsClaimChanged);
            cmd.Parameters.AddWithValue("@IsCalChanged", IsCalChanged);
            cmd.Parameters.AddWithValue("@isMAChanged", isMAChanged);

            DataSet ds = Globals.GetData(cmd);
            if (ds.Tables.Count == 0)
                return null;
            string result = Convert.ToString(ds.Tables[0].Rows[0][0]);
            if (result.Contains("Validation Message"))
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, result));//type 2 error
            }
            if (strAction == "Publish")
            {
                var CompanyDetails = db.GCompanies.Where(p => p.Id == iCompanyID).FirstOrDefault();
                var BatchDetails = db.LReadyToPays.Where(p => p.Id == iRTPID).FirstOrDefault();
                var Role = db.AspNetRoles.Where(p => p.Name == UserRole).Where(p => p.CompanyCode == CompanyDetails.GcCode).FirstOrDefault();
                var WorkFlow = db.RWorkFlows.Where(p => p.RwfName == "PaymentCycles").FirstOrDefault();
                //Add Entry in Audit Log
                Globals.ExecuteSPAuditLog("PaymentCycle", "Audit", null, strAction,
                      strAction, strCreatedBy, DateTime.UtcNow, strRTPStatus, strRTPStatus,
                     "LReadyToPay", iRTPID, BatchDetails.PaymentBatchName + " (" + BatchDetails.PaymentBatchNo + ")", WorkFlow.Id, iCompanyID, string.Empty, Role.Id, null);

            }
            //var RawQuery = db.Database.SqlQuery("exec spReadyToPayData " + iRTPID.ToString() + "," + iCompanyID.ToString() + ",'" + blnIsBatchList + "','" + strType + "','" + strRTPStatus + "','" + strAction + "','" + strPayBatchName + "','" + strBatchCommPeriod + "','" + strRTPData + "','" + strCreatedBy + "','" + strUpdatedBy + "'");
            //var Task = RawQuery.ToList();
            //var ListData = Task.ToList();
            //return Ok(ListData);
            return Ok();
        }
        [ResponseType(typeof(LReadyToPay))]
        public async Task<IHttpActionResult> GetRTPByID(int Id)
        {

            var LReadyToPay = db.LReadyToPays.Where(p => p.Id == Id).Select(x => new { x.Id, x.PaymentBatchName, x.PaymentBatchNo, x.PeriodName, x.Status, x.UpdatedByID, x.UpdatedDateTime, x.CreatedByID, x.CreatedDateTime }).FirstOrDefault();
            if (LReadyToPay == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LReadyToPay")));
            }
            return Ok(LReadyToPay);
        }

        public async Task<IHttpActionResult> GetCountPaymentDocument(int Id, string CompanyCode)
        {
            var LReadyToPay = db.LReadyToPays.Where(p => p.Id == Id).Select(x => new { x.Id, x.PaymentBatchName, x.PaymentBatchNo, x.PeriodName, x.Status, x.UpdatedByID, x.UpdatedDateTime, x.CreatedByID, x.CreatedDateTime }).FirstOrDefault();
            if (LReadyToPay != null)
            {
                int CurrentOrdinal = db.Database.SqlQuery<int>("select count(XBatchNumber) from  [XSchema" + CompanyCode + "].XPayeeDocuments where XBatchNumber=" + LReadyToPay.PaymentBatchNo).FirstOrDefault<int>();
                return Ok(CurrentOrdinal);
            }
            else
            {
                return Ok(0);
            }
        }

        public IHttpActionResult GetNewRTPByCompanyID(int CompanyId, string strUserID)
        {
            //added parameters as SP was updated
            var RawQuery = db.Database.SqlQuery<LReadyToPayViewModel>("exec spReadyToPayData 0," + CompanyId.ToString() + ",'True','','submitted,draft','','','','','" + strUserID + "','','','role',0,0,null,null,null,null,20,0,null,false,false,false");
            var Task = RawQuery.ToList();
            var ListData = Task.ToList();
            return Ok(ListData);
        }
        public IHttpActionResult GetCancelledRTPByCompanyID(int CompanyId, string strUserID)
        {

            var RawQuery = db.Database.SqlQuery<LReadyToPayViewModel>("exec spReadyToPayData 0," + CompanyId.ToString() + ",'True','','cancelled,systemcancelled,payrejected','','','','','" + strUserID + "','','','role',0,0,null,null,null,null,20,0,null,false,false,false");
            var Task = RawQuery.ToList();
            var ListData = Task.ToList();
            return Ok(ListData);
        }
        public IHttpActionResult GetPayGeneratedRTPByCompanyID(int CompanyId, string strUserID)
        {

            var RawQuery = db.Database.SqlQuery<LReadyToPayViewModel>("exec spReadyToPayData 0," + CompanyId.ToString() + ",'True','','paygenerated','','','','','" + strUserID + "','','','role',0,0,null,null,null,null,20,0,null,false,false,false");
            var Task = RawQuery.ToList();
            var ListData = Task.ToList();
            return Ok(ListData);
        }
        public IHttpActionResult GetPublishedRTPByCompanyID(int CompanyId, string strUserID)
        {
            var RawQuery = db.Database.SqlQuery<LReadyToPayViewModel>("exec spReadyToPayData 0," + CompanyId.ToString() + ",'True','','PayPublished','','','','','" + strUserID + "','','','role',0,0,null,null,null,null,20,0,null,false,false,false");
            var Task = RawQuery.ToList();
            var ListData = Task.ToList();
            return Ok(ListData);
        }
        public IHttpActionResult GetRTPPortfolios(int RTPId, int CompanyId, string strAction, string strUserID, string Role)
        {
            var RawQuery = db.Database.SqlQuery<LPortfolioViewModel>("exec spReadyToPayData " + RTPId + "," + CompanyId.ToString() + ",'False','Portfolio','','" + strAction + "','','','','" + strUserID + "','','','" + Role + "',0,0,null,null,null,null,20,0,null,false,false,false");
            var Task = RawQuery.ToList();
            var ListData = Task.ToList();
            return Ok(ListData);

        }

        [HttpPost]
        public async Task<IHttpActionResult> GetClaimValidate(LReadyToPayCount objLReadyToPayCount)
        {

            int iRTPID = objLReadyToPayCount.iRTPID;
            int iCompanyID = objLReadyToPayCount.iCompanyID;
            Boolean blnIsBatchList = objLReadyToPayCount.blnIsBatchList;
            string strType = objLReadyToPayCount.strType;
            string strRTPStatus = objLReadyToPayCount.strRTPStatus;
            string strAction = objLReadyToPayCount.strAction;
            string strPayBatchName = objLReadyToPayCount.strPayBatchName;
            string strBatchCommPeriod = objLReadyToPayCount.strBatchCommPeriod;
            string strRTPData = objLReadyToPayCount.strRTPData;
            string strCreatedBy = objLReadyToPayCount.strCreatedBy;
            string strUpdatedBy = objLReadyToPayCount.strUpdatedBy;
            string strPortfolios = objLReadyToPayCount.strPortfolios;
            string sortdatafield = objLReadyToPayCount.sortdatafield;
            string sortorder = objLReadyToPayCount.sortdatafield;
            int pagesize = objLReadyToPayCount.pagesize;
            int pagenum = objLReadyToPayCount.pagenum;
            string FilterQuery = objLReadyToPayCount.FilterQuery;

            var tb = new DataTable();
            var Query = "exec spValidateClaimsData @iCompanyID,@strType,@strRTPData,@strPortfolios";
            ////using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@iCompanyID", iCompanyID.ToString());
            cmd.Parameters.AddWithValue("@strType", string.IsNullOrEmpty(strType) ? (object)System.DBNull.Value : strType);
            cmd.Parameters.AddWithValue("@strRTPData", string.IsNullOrEmpty(strRTPStatus) ? (object)System.DBNull.Value : strRTPStatus);
            cmd.Parameters.AddWithValue("@strPortfolios", string.IsNullOrEmpty(strPortfolios) ? (object)System.DBNull.Value : strPortfolios);


            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);

            //tb.Columns.Remove(tb.Columns["row"]);
            sda.Fill(tb);
            //tb.Columns.Remove("row");
            conn.Close();
            // return Ok(ds);
            return Ok(tb);

        }

    }
}