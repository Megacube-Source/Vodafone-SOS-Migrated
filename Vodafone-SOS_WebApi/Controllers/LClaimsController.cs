using System;
using System.Collections.Generic;
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
using System.Data.Entity.Validation;
using System.Globalization;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using System.Data.OleDb;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Threading;
using Ionic.Zip;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LClaimsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();


        //public IHttpActionResult GetTestAudit()
        //{
        //    //Add Entry in Audit Log
        //    Globals.ExecuteSPAuditLog("Claims", "Audit", null, "Create",
        //           "Create", "f573c24c-075a-4ede-b872-bd3abf12177f", DateTime.UtcNow, "Saved", "Saved",
        //          "LClaims", 0, Convert.ToString("Test"), 3, 3, string.Empty,"28");
        //    return Ok();
        //}

        //[HttpPost]
        private Boolean blnCheckTemplateColumns(DataTable dt)
        {
            if (!dt.Columns.Contains("LcClaimId")) return false;
            if (!dt.Columns.Contains("LcReasonNonAutoPayment")) return false;
            if (!dt.Columns.Contains("LcRejectionReasonId")) return false;
            if (!dt.Columns.Contains("WFComments")) return false;
            if (!dt.Columns.Contains("LcActivityTypeId")) return false;
            if (!dt.Columns.Contains("LcAllocationDate")) return false;
            if (!dt.Columns.Contains("LcCommissionTypeId")) return false;
            if (!dt.Columns.Contains("LcConnectionDate")) return false;
            if (!dt.Columns.Contains("LcExpectedCommissionAmount")) return false;
            if (!dt.Columns.Contains("LcIMEI")) return false;
            if (!dt.Columns.Contains("LcLastReclaimDate")) return false;
            if (!dt.Columns.Contains("LcMSISDN")) return false;
            if (!dt.Columns.Contains("LcOrderDate")) return false;
            if (!dt.Columns.Contains("LcOrderNumber")) return false;
            if (!dt.Columns.Contains("LcPayeeId")) return false;
            if (!dt.Columns.Contains("LcPaymentAmount")) return false;
            //if (!dt.Columns.Contains("LcPaymentBatchNumber")) return false;
            if (!dt.Columns.Contains("LcPaymentCommissionTypeId")) return false;
            if (!dt.Columns.Contains("LcProductCodeId")) return false;
            if (!dt.Columns.Contains("A01")) return false;
            if (!dt.Columns.Contains("A02")) return false;
            if (!dt.Columns.Contains("A03")) return false;
            if (!dt.Columns.Contains("A04")) return false;
            if (!dt.Columns.Contains("A05")) return false;
            if (!dt.Columns.Contains("A06")) return false;
            if (!dt.Columns.Contains("A07")) return false;
            if (!dt.Columns.Contains("A08")) return false;
            if (!dt.Columns.Contains("A09")) return false;
            if (!dt.Columns.Contains("A10")) return false;
            if (!dt.Columns.Contains("LcAlreadyPaidAmount")) return false;
            if (!dt.Columns.Contains("LcAlreadyPaidDate")) return false;
            if (!dt.Columns.Contains("LcAlreadyPaidDealer")) return false;
            if (!dt.Columns.Contains("LcBAN")) return false;
            if (!dt.Columns.Contains("LcBrandId")) return false;
            //if (!dt.Columns.Contains("LcClaimBatchNumber")) return false;
            if (!dt.Columns.Contains("LcClawbackAmount")) return false;
            if (!dt.Columns.Contains("LcClawbackPayeeCode")) return false;
            if (!dt.Columns.Contains("LcCustomerName")) return false;
            if (!dt.Columns.Contains("LcDeviceTypeId")) return false;
            
            return true;
        }
        //public IHttpActionResult ValidateUploadLClaims(List<LClaimDecryptedViewModel> LClaim,string FileName, string LoggedInRoleId, string UserName, string Workflow, int iCompanyId)
        //{
        //    //string strConnectionString = "";
        //    //strConnectionString = CheckAvailableConnections(FileName);

        //    #region NewCode
        //    try
        //    {
        //        DataSet dsErrors = new DataSet();
        //        #region R2.1.3 changes
        //        string excelConnectionString = string.Empty;
        //        #region SOS Approach
        //        //try
        //        //{
        //        //    excelConnectionString = string.Format(System.Configuration.ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"], FileName);
        //        //}
        //        //catch (Exception ex)
        //        //{
        //        //    CreateDebugEntry(ex.ToString());
        //        //}
        //        //OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
        //        //excelConnection.Open();
        //        //DataTable dt = new DataTable();
        //        //dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //        //if (dt == null)
        //        //{
        //        //    CreateDebugEntry("nothing found to import");
        //        //    return null;
        //        //}
        //        //string[] excelSheets = new string[dt.Rows.Count];
        //        //int t = 0;
        //        ////excel data is saved in temporary file here
        //        //foreach (DataRow row in dt.Rows)
        //        //{
        //        //    excelSheets[t] = row["TABLE_NAME"].ToString();
        //        //    t++;
        //        //}
        //        //OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
        //        //DataSet ds = new DataSet();
        //        //DataTable dtData = new DataTable();
        //        //CreateDebugEntry("trying to load sheet");
        //        //string query = string.Format("Select * from [{0}]", excelSheets[0]);
        //        #endregion
        //        #region RELY Approach
        //        //CreateDebugEntry("Start reading file");
        //        var CompanyDetails = db.GCompanies.Where(p => p.Id == iCompanyId).FirstOrDefault();
        //        string S3BucketRootFolder = ConfigurationManager.AppSettings["SOSBucketRootFolder"];
        //        string S3TargetPath = S3BucketRootFolder + CompanyDetails.GcCode+"/upload/claims/" + FileName;
        //        //CreateDebugEntry("Connecting to S3");
        //        //var bytedata = Globals.DownloadFromS3RKTest(  S3TargetPath);
        //        var bytedata = Globals.DownloadFromS3(S3TargetPath,"");
        //        //if (System.IO.File.Exists(FileName))
        //        //{
        //        //    System.IO.File.Delete(FileName);
        //        //}

        //        SqlDataAdapter adapter = new SqlDataAdapter();
        //        DataSet ds2 = new DataSet();
        //        DataTable dtdata = null;

        //        try
        //        {

        //            OleDbConnection con = null;
        //            try
        //            {
        //                string fileExtension = System.IO.Path.GetExtension(FileName);
        //                string name = System.IO.Path.GetFileNameWithoutExtension(FileName);
        //                string FileName_New = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss")+"_UPLOAD" + fileExtension;

        //                string path = ConfigurationManager.AppSettings["TempDocumentPath"];
        //                string fullpath = path + "\\" + FileName_New;
        //                System.IO.File.WriteAllBytes(fullpath, bytedata); // Save File
        //                //CreateDebugEntry("File saved from byte to excel.");
        //                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
        //                con = new System.Data.OleDb.OleDbConnection(connectionString);
        //                con.Open();
        //                OleDbDataAdapter cmd2 = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [Claims$]", con);
        //                cmd2.Fill(ds2);
        //                dtdata = ds2.Tables[0];
        //                con.Close();
        //                if (!blnCheckTemplateColumns(dtdata))
        //                {
        //                    DataTable dtE = new DataTable();
        //                    dtE.Columns.Add("ExceptionMessage");
        //                    dtE.Rows.Add("Invalid Template File, Please download fresh template and use the same for uploading claims.");
        //                    return Ok(dtE);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                DataTable dtE = new DataTable();
        //                dtE.Columns.Add("ExceptionMessage");
        //                dtE.Rows.Add(ex.ToString());
        //                return Ok(dtE);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            var models = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "UploadClaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
        //            db.GErrorLogs.Add(models);
        //            db.SaveChanges();
        //        }
        //        #endregion
        //        db.Database.ExecuteSqlCommand("delete from TClaimsUpload where WFRequester = '" + UserName.ToString() + "' and WFCompanyId = '" + iCompanyId.ToString() + "'");

        //        dtdata.Rows.RemoveAt(0);//Removing the 1st row of data table as it contains the column alias.
        //        #region Prepare data for bulk insert
        //        System.Data.DataColumn newColumn1 = new System.Data.DataColumn("WFRequesterRoleId", typeof(System.String));
        //        newColumn1.DefaultValue = LoggedInRoleId.ToString();
        //        dtdata.Columns.Add(newColumn1);
        //        System.Data.DataColumn newColumn2 = new System.Data.DataColumn("WFCompanyID", typeof(System.String));
        //        newColumn2.DefaultValue = iCompanyId.ToString();
        //        dtdata.Columns.Add(newColumn2);
        //        System.Data.DataColumn newColumn3 = new System.Data.DataColumn("WFRequester", typeof(System.String));
        //        newColumn3.DefaultValue = UserName;
        //        dtdata.Columns.Add(newColumn3);
        //        System.Data.SqlClient.SqlBulkCopy sqlBulk = new SqlBulkCopy(db.Database.Connection.ConnectionString);

        //        sqlBulk.ColumnMappings.Add("WFRequesterRoleId", "WFRequesterRoleId");
        //        sqlBulk.ColumnMappings.Add("WFCompanyID", "WFCompanyID");
        //        sqlBulk.ColumnMappings.Add("WFRequester", "WFRequester");
        //        sqlBulk.ColumnMappings.Add("LcActivityTypeId", "LcActivityTypeId");
        //        sqlBulk.ColumnMappings.Add("LcAlreadyPaidDate", "LcAlreadyPaidDate");
        //        sqlBulk.ColumnMappings.Add("LcAlreadyPaidDealer", "LcAlreadyPaidDealer");
        //        sqlBulk.ColumnMappings.Add("LcAlreadyPaidAmount", "LcAlreadyPaidAmount");
        //        sqlBulk.ColumnMappings.Add("LcAllocationDate", "LcAllocationDate");
        //        sqlBulk.ColumnMappings.Add("LcBAN", "LcBAN");
        //        sqlBulk.ColumnMappings.Add("LcBrandId", "LcBrandId");
        //        sqlBulk.ColumnMappings.Add("LcCommissionTypeId", "LcCommissionTypeId");
        //        sqlBulk.ColumnMappings.Add("LcConnectionDate", "LcConnectionDate");
        //        //sqlBulk.ColumnMappings.Add("LcCreatedById", "LcCreatedById");
        //        sqlBulk.ColumnMappings.Add("LcCustomerName", "LcCustomerName");
        //        //sqlBulk.ColumnMappings.Add("LcClaimBatchNumber", "LcClaimBatchNumber");
        //        sqlBulk.ColumnMappings.Add("LcClawbackPayeeCode", "LcClawbackPayeeCode");
        //        sqlBulk.ColumnMappings.Add("LcClawbackAmount", "LcClawbackAmount");
        //        sqlBulk.ColumnMappings.Add("LcDeviceTypeId", "LcDeviceTypeId");
        //        sqlBulk.ColumnMappings.Add("LcExpectedCommissionAmount", "LcExpectedCommissionAmount");
        //        sqlBulk.ColumnMappings.Add("LcIMEI", "LcIMEI");
        //        sqlBulk.ColumnMappings.Add("LcLastReclaimDate", "LcLastReclaimDate");
        //        sqlBulk.ColumnMappings.Add("LcMSISDN", "LcMSISDN");
        //        sqlBulk.ColumnMappings.Add("LcOrderDate", "LcOrderDate");
        //        sqlBulk.ColumnMappings.Add("LcOrderNumber", "LcOrderNumber");
        //        sqlBulk.ColumnMappings.Add("LcPaymentAmount", "LcPaymentAmount");
        //        //sqlBulk.ColumnMappings.Add("LcPaymentBatchNumber", "LcPaymentBatchNumber");
        //        sqlBulk.ColumnMappings.Add("LcPaymentCommissionTypeId", "LcPaymentCommissionTypeId");
        //        sqlBulk.ColumnMappings.Add("LcProductCodeId", "LcProductCodeId");
        //        sqlBulk.ColumnMappings.Add("LcReasonNonAutoPayment", "LcReasonNonAutoPayment");
        //        sqlBulk.ColumnMappings.Add("LcPayeeId", "LcPayeeId");
        //        sqlBulk.ColumnMappings.Add("LcRejectionReasonId", "LcRejectionReasonId");

        //        sqlBulk.ColumnMappings.Add("A01", "A01");
        //        sqlBulk.ColumnMappings.Add("A02", "A02");
        //        sqlBulk.ColumnMappings.Add("A03", "A03");
        //        sqlBulk.ColumnMappings.Add("A04", "A04");
        //        sqlBulk.ColumnMappings.Add("A05", "A05");
        //        sqlBulk.ColumnMappings.Add("A06", "A06");
        //        sqlBulk.ColumnMappings.Add("A07", "A07");
        //        sqlBulk.ColumnMappings.Add("A08", "A08");
        //        sqlBulk.ColumnMappings.Add("A09", "A09");
        //        sqlBulk.ColumnMappings.Add("A10", "A10");

        //        sqlBulk.ColumnMappings.Add("LcClaimId", "LcClaimId");
        //        sqlBulk.ColumnMappings.Add("WFComments", "WFComments");
        //        sqlBulk.ColumnMappings.Add("LcCommissionPeriod", "LcCommissionPeriod");
        //        sqlBulk.ColumnMappings.Add("LcParentPayeeId", "LcParentPayeeId");
        //        sqlBulk.DestinationTableName = "TClaimsUpload";
        //        #endregion
        //        try
        //        {
        //            sqlBulk.WriteToServer(dtdata);

        //        }
        //        catch (Exception ex)
        //        {
        //            var models = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "ValidateUploadLClaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
        //            db.GErrorLogs.Add(models);
        //            db.SaveChanges();
        //            DataTable dtE = new DataTable();
        //            dtE.Columns.Add("ExceptionMessage");
        //            return Ok(dtE);
        //        }

        //        //excelConnection1.Dispose();
        //        //excelConnection.Dispose();
        //        if (System.IO.File.Exists(FileName))
        //            System.IO.File.Delete(FileName);
        //        var Query = "Exec dbo.USPValidateAndInsertClaimsUploadData @UserID,@UserRoleID,@CompanyID,@UploadData";
        //        SqlCommand cmd = new SqlCommand(Query);
        //        cmd.Parameters.AddWithValue("@UserID", UserName);
        //        cmd.Parameters.AddWithValue("@UserRoleID", LoggedInRoleId.ToString());
        //        cmd.Parameters.AddWithValue("@CompanyID", iCompanyId.ToString());
        //        cmd.Parameters.AddWithValue("@UploadData", 0);
        //        dsErrors = GetData(cmd);
        //        if(dsErrors.Tables.Count>0)
        //        {
        //            if (dsErrors.Tables[0].Rows.Count > 0)
        //            {
        //                return Ok(dsErrors.Tables[0]);
        //            }
        //            else
        //            {
        //                return Ok();
        //            }
        //        }
        //        else
        //        {
        //            return Ok();
        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        var models = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "UploadClaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
        //        db.GErrorLogs.Add(models);
        //        db.SaveChanges();
        //        DataTable dtE = new DataTable();
        //        dtE.Columns.Add("ExceptionMessage");
        //        return Ok(dtE);
        //    }
        //    return Ok();
        //    #endregion
        //    #region OldCode
        //    //try
        //    //{
        //    //    using (var transaction2 = db.Database.BeginTransaction())
        //    //    {
        //    //        var RawQuery = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo.SQ_BatchNumber");
        //    //        var Task = RawQuery.SingleAsync();
        //    //        var BatchNumber = Task.Result;
        //    //        Task.Dispose();
        //    //        //var BatchStatusId =  db.RStatuses.Where(p => p.RsStatus == "ClaimBatchAccepted").Where(p => p.RStatusOwner.RsoStatusOwner == "Batch").FirstOrDefault().Id;
        //    //        //var ClaimStatusId = db.RStatuses.Where(p => p.RsStatus == "Accepted").Where(p => p.RStatusOwner.RsoStatusOwner == "Claim").FirstOrDefault().Id;
        //    //        //var BatchModel = new LBatch { LbBatchNumber = BatchNumber, LbBatchType = "Claims", LbCompanyId = LClaim.FirstOrDefault().LcCompanyId, LbStatus = "ClaimBatchAccepted", LbUpdatedBy = LClaim.FirstOrDefault().LcCreatedById, LbUploadStartDateTime = DateTime.UtcNow };
        //    //        //db.LBatches.Add(BatchModel);
        //    //        //db.SaveChanges();
        //    //        //var batchFiles = new LBatchFile { LbfBatchId = BatchModel.Id, LbfFileName = FileName, LbfFileTimeStamp = DateTime.UtcNow };
        //    //        //db.LBatchFiles.Add(batchFiles);
        //    //        //db.SaveChanges();
        //    //        //var BatchId = BatchModel.Id;
        //    //        //Note: Calculate Ordinal based on the Current Role who has created Manual Adjustment based on RoleId and Opco and WorkflowName
        //    //        var CompanyId = LClaim.FirstOrDefault().LcCompanyId;
        //    //        var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault().LwfcOrdinalNumber;
        //    //        var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();


        //    //        foreach (var claim in LClaim)
        //    //        {
        //    //            try
        //    //            {
        //    //                var Claim = db.GKeyValues.Where(p => p.GkvCompanyId == claim.LcCompanyId).Where(p => p.GkvKey == "ClaimIdSequenceName").FirstOrDefault();
        //    //                var RawQuery1 = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo." + Claim.GkvValue);
        //    //                var Task1 = RawQuery1.SingleAsync();
        //    //                //var model = new LClaim
        //    //                //{
        //    //                //    A01 = claim.A01,
        //    //                //    A02 = claim.A02,
        //    //                //    A03 = claim.A03,
        //    //                //    A04 = claim.A04,
        //    //                //    A05 = claim.A05,
        //    //                //    A06 = claim.A06,
        //    //                //    A07 = claim.A07,
        //    //                //    A08 = claim.A08,
        //    //                //    A09 = claim.A09,
        //    //                //    A10 = claim.A10,
        //    //                //    LcActivityTypeId = claim.LcActivityTypeId,
        //    //                //    LcAllocatedById = claim.LcAllocatedById,
        //    //                //    LcAllocatedToId = claim.LcAllocatedToId,
        //    //                //    LcAllocationDate = claim.LcAllocationDate,
        //    //                //    LcAlreadyPaidAmount = claim.LcAlreadyPaidAmount,
        //    //                //    LcAlreadyPaidDate = claim.LcAlreadyPaidDate,
        //    //                //    LcAlreadyPaidDealer = claim.LcAlreadyPaidDealer,
        //    //                //    LcApprovalDate = claim.LcApprovalDate,
        //    //                //    LcApprovedById = claim.LcApprovedById,
        //    //                //    LcBAN = claim.LcBAN,
        //    //                //    LcBrandId = claim.LcBrandId,
        //    //                //    LcClaimBatchNumber = BatchNumber,
        //    //                //    LcClaimId =  Task1.Result,
        //    //                //    LcClawbackAmount = claim.LcClawbackAmount,
        //    //                //    LcClawbackPayeeCode = claim.LcClawbackPayeeCode,
        //    //                //    LcCommissionTypeId = claim.LcCommissionTypeId,
        //    //                //    LcCompanyId = claim.LcCompanyId,
        //    //                //    LcConnectionDate = claim.LcConnectionDate,
        //    //                //    LcCreatedById = claim.LcCreatedById,
        //    //                //    LcCreatedDateTime = claim.LcCreatedDateTime,
        //    //                //    LcCustomerName = claim.LcCustomerName,
        //    //                //    LcDeviceTypeId = claim.LcDeviceTypeId,
        //    //                //    LcDuplicateClaimDetails = claim.LcDuplicateClaimDetails,
        //    //                //    LcExpectedCommissionAmount = claim.LcExpectedCommissionAmount,
        //    //                //    LcIMEI = claim.LcIMEI,
        //    //                //    LcIsDuplicateClaim = claim.LcIsDuplicateClaim,
        //    //                //    LcIsReclaim = claim.LcIsReclaim,
        //    //                //    LcLastReclaimDate = claim.LcLastReclaimDate,
        //    //                //    LcMSISDN = claim.LcMSISDN,
        //    //                //    LcOrderDate = claim.LcOrderDate,
        //    //                //    LcOrderNumber = claim.LcOrderNumber,
        //    //                //    LcPayeeId = claim.LcPayeeId,
        //    //                //    LcPaymentAmount = claim.LcPaymentAmount,
        //    //                //    LcPaymentBatchNumber = 0,
        //    //                //    LcPaymentCommissionTypeId = claim.LcPaymentCommissionTypeId,
        //    //                //    LcPaymentDate = claim.LcPaymentDate,
        //    //                //    LcProductCodeId = claim.LcProductCodeId,
        //    //                //    LcReasonNonAutoPayment = claim.LcReasonNonAutoPayment,
        //    //                //    LcRejectedById = claim.LcRejectedById,
        //    //                //    LcRejectionDate = claim.LcRejectionDate,
        //    //                //    LcRejectionReasonId = claim.LcRejectionReasonId,
        //    //                //    LcSentForApprovalById = claim.LcSentForApprovalById,
        //    //                //    LcSentForApprovalDate = claim.LcSentForApprovalDate,
        //    //                //    WFStatus = "Saved",
        //    //                //    WFOrdinal = Ordinal,
        //    //                //    WFType = "LClaims",
        //    //                //    WFRequesterId = claim.WFRequesterId,
        //    //                //    WFRequesterRoleId = LoggedInRoleId,
        //    //                //    WFCompanyId = claim.LcCompanyId,

        //    //                //    LcWithdrawnById = claim.LcWithdrawnById,
        //    //                //    LcWithdrawnDate = claim.LcWithdrawnDate,

        //    //                //};
        //    //                //Task1.Dispose();
        //    //                //db.LClaims.Add(model);
        //    //                //db.SaveChanges();

        //    //                //Encryption changes
        //    //                var model = new LClaimDecryptedViewModel
        //    //                {
        //    //                    A01 = claim.A01,
        //    //                    A02 = claim.A02,
        //    //                    A03 = claim.A03,
        //    //                    A04 = claim.A04,
        //    //                    A05 = claim.A05,
        //    //                    A06 = claim.A06,
        //    //                    A07 = claim.A07,
        //    //                    A08 = claim.A08,
        //    //                    A09 = claim.A09,
        //    //                    A10 = claim.A10,
        //    //                    LcActivityTypeId = claim.LcActivityTypeId,
        //    //                    LcAllocatedById = claim.LcAllocatedById,
        //    //                    LcAllocatedToId = claim.LcAllocatedToId,
        //    //                    LcAllocationDate = claim.LcAllocationDate,
        //    //                    LcAlreadyPaidAmount = claim.LcAlreadyPaidAmount,
        //    //                    LcAlreadyPaidDate = claim.LcAlreadyPaidDate,
        //    //                    LcAlreadyPaidDealer = claim.LcAlreadyPaidDealer,
        //    //                    LcApprovalDate = claim.LcApprovalDate,
        //    //                    LcApprovedById = claim.LcApprovedById,
        //    //                    LcBAN = claim.LcBAN,
        //    //                    LcBrandId = claim.LcBrandId,
        //    //                    LcClaimBatchNumber = BatchNumber,
        //    //                    LcClaimId = Task1.Result,
        //    //                    LcClawbackAmount = claim.LcClawbackAmount,
        //    //                    LcClawbackPayeeCode = claim.LcClawbackPayeeCode,
        //    //                    LcCommissionTypeId = claim.LcCommissionTypeId,
        //    //                    LcCompanyId = claim.LcCompanyId,
        //    //                    LcConnectionDate = claim.LcConnectionDate,
        //    //                    LcCreatedById = claim.LcCreatedById,
        //    //                    LcCreatedDateTime = claim.LcCreatedDateTime,
        //    //                    LcCustomerName = claim.LcCustomerName,
        //    //                    LcDeviceTypeId = claim.LcDeviceTypeId,
        //    //                    LcDuplicateClaimDetails = claim.LcDuplicateClaimDetails,
        //    //                    LcExpectedCommissionAmount = claim.LcExpectedCommissionAmount,
        //    //                    LcIMEI = claim.LcIMEI,
        //    //                    LcIsDuplicateClaim = claim.LcIsDuplicateClaim,
        //    //                    LcIsReclaim = claim.LcIsReclaim,
        //    //                    LcLastReclaimDate = claim.LcLastReclaimDate,
        //    //                    LcMSISDN = claim.LcMSISDN,
        //    //                    LcOrderDate = claim.LcOrderDate,
        //    //                    LcOrderNumber = claim.LcOrderNumber,
        //    //                    LcPayeeId = claim.LcPayeeId,
        //    //                    LcPaymentAmount = claim.LcPaymentAmount,
        //    //                    LcPaymentBatchNumber = 0,
        //    //                    LcPaymentCommissionTypeId = claim.LcPaymentCommissionTypeId,
        //    //                    LcPaymentDate = claim.LcPaymentDate,
        //    //                    LcProductCodeId = claim.LcProductCodeId,
        //    //                    LcReasonNonAutoPayment = claim.LcReasonNonAutoPayment,
        //    //                    LcRejectedById = claim.LcRejectedById,
        //    //                    LcRejectionDate = claim.LcRejectionDate,
        //    //                    LcRejectionReasonId = claim.LcRejectionReasonId,
        //    //                    LcSentForApprovalById = claim.LcSentForApprovalById,
        //    //                    LcSentForApprovalDate = claim.LcSentForApprovalDate,
        //    //                    WFStatus = "Saved",
        //    //                    WFOrdinal = Ordinal,
        //    //                    WFType = "LClaims",
        //    //                    WFRequesterId = claim.WFRequesterId,
        //    //                    WFRequesterRoleId = LoggedInRoleId,
        //    //                    WFCompanyId = claim.LcCompanyId,
        //    //                    WFCurrentOwnerId=claim.LcCreatedById,
        //    //                    LcWithdrawnById = claim.LcWithdrawnById,
        //    //                    LcWithdrawnDate = claim.LcWithdrawnDate,

        //    //                };
        //    //                //Call SP to save clams to DB
        //    //                var Id = Globals.ExecutePutOrPostLClaim(model, "Create");

        //    //                //Update Model Id if it is not null
        //    //                if (Id.HasValue)
        //    //                {
        //    //                    model.Id = Id.Value;
        //    //                    //03 July 2018 Now we are using SPUpdateActionStatus for Auto Approval of Claims Upload
        //    //                    var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
        //    //                    string SPresult = Globals.ExecuteSPUpdateActionStatus("Approve", "Claims", Convert.ToString(model.Id), CompanyId, model.LcCreatedById, "Empty", LoggedInRoleId, ProjectEnviournment, model.LcCreatedById);
        //    //                    var Payee = db.LPayees.Where(p => p.LpCompanyId == CompanyId).Where(p => p.Id == model.LcPayeeId).FirstOrDefault();
        //    //                    if (Payee != null)
        //    //                    {
        //    //                        var MEntityPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityType.Equals("LPayees", StringComparison.OrdinalIgnoreCase)).Where(p => p.MepEntityId == Payee.Id).ToList();
        //    //                        foreach (var PF in MEntityPortfolios)
        //    //                        {
        //    //                            var MEP = new MEntityPortfolio { MepPortfolioId = PF.MepPortfolioId, MepEntityId = model.Id, MepEntityType = "LClaims" };
        //    //                            db.MEntityPortfolios.Add(MEP);
        //    //                            db.SaveChanges();
        //    //                        }
        //    //                    }

        //    //                    //Old Approval Code Commented now
        //    //                    ////Skip functionality
        //    //                    //skipFunction:
        //    //                    //Ordinal++;
        //    //                    //Boolean blnSkip = false;
        //    //                    //var skipDetail = db.LWorkFlowConfigs.Where(p => p.LwfcWorkFlowId == WFDetails.Id).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcOrdinalNumber == Ordinal).FirstOrDefault().LwfcSkip;
        //    //                    //blnSkip = Convert.ToBoolean(skipDetail);

        //    //                    //if (blnSkip)
        //    //                    //{
        //    //                    //    blnSkip = false;
        //    //                    //    try
        //    //                    //    {
        //    //                    //        //Placed this piece of code in try block as it throws exception when the SkipFunctionName is null
        //    //                    //        string skipFunctionName = Convert.ToString(db.LWorkFlowConfigs.Where(p => p.LwfcWorkFlowId == WFDetails.Id).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcOrdinalNumber == Ordinal).FirstOrDefault().LwfcSkipFunctionName);
        //    //                    //        if (!string.IsNullOrEmpty(skipFunctionName.ToString()))
        //    //                    //        {
        //    //                    //            blnSkip = db.Database.SqlQuery<bool>("select dbo." + skipFunctionName.ToString() + "()").FirstOrDefault<bool>();
        //    //                    //        }
        //    //                    //        if (blnSkip)
        //    //                    //        {
        //    //                    //            goto skipFunction;
        //    //                    //        }
        //    //                    //    }
        //    //                    //    catch (Exception ex)
        //    //                    //    { }

        //    //                    //}

        //    //                    ////Approval/Sending to next ordinal
        //    //                    //db.Database.ExecuteSqlCommand("update LClaims set WFOrdinal = " + (Ordinal) + ", WFCurrentOwnerId = NULL where id = " + model.Id + "");
        //    //                    ////NoMatchFound
        //    //                    //var RawQuery2 = db.Database.SqlQuery<string>("select dbo.FNGetUserForAllocation('LClaims','LClaims'," + model.Id.ToString() + "," + (Ordinal).ToString() + "," + CompanyId + ")");
        //    //                    //var Task2 = RawQuery2.SingleAsync();
        //    //                    //var NewOwner = Task2.Result;
        //    //                    //if (NewOwner.ToString() != "NoMatchFound")
        //    //                    //{

        //    //                    //    string strActingAs = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcWorkFlowId == WFDetails.Id).Where(p => p.LwfcOrdinalNumber == Ordinal).FirstOrDefault().LwfcActingAs;
        //    //                    //    if (strActingAs == "Analyst")
        //    //                    //    {
        //    //                    //        db.Database.ExecuteSqlCommand("update LClaims set  WFCurrentOwnerId = '" + NewOwner.ToString() + "', WFAnalystId = '" + NewOwner + "' where id = " + model.Id + "");
        //    //                    //    }
        //    //                    //    else if (strActingAs == "Manager")
        //    //                    //    {
        //    //                    //        db.Database.ExecuteSqlCommand("update LClaims set  WFCurrentOwnerId = '" + NewOwner.ToString() + "', WFManagerId = '" + NewOwner + "' where id = " + model.Id + "");
        //    //                    //    }
        //    //                    //    else
        //    //                    //    {
        //    //                    //        db.Database.ExecuteSqlCommand("update LClaims set  WFCurrentOwnerId = '" + NewOwner.ToString() + "' where id = " + model.Id + "");
        //    //                    //    }
        //    //                    //    //db.Database.ExecuteSqlCommand("update LClaims set  WFCurrentOwnerId = '" + NewOwner.ToString() + "', WFManagerId = '" + strManagerID+ "', WFAnalystId = '" + strAnalystID + "' where id = " + model.Id + "");
        //    //                    //}
        //    //                }
        //    //            }
        //    //            catch (Exception ex)
        //    //            {
        //    //                iExeceptionsCount++;
        //    //                strExceptionMessage += ex.StackTrace.ToString();
        //    //            }
        //    //        }
        //    //        if (iExeceptionsCount == 0)
        //    //        {
        //    //            transaction2.Commit();
        //    //        }
        //    //        else
        //    //        {
        //    //            transaction2.Rollback();
        //    //        }
        //    //        #endregion
        //    //        #region OLD Code Commented
        //    //        /*
        //    //        //var RawQuery = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo.SQ_BatchNumber");
        //    //        //var Task = RawQuery.SingleAsync();
        //    //        //var BatchNumber = Task.Result;
        //    //        //Task.Dispose();

        //    //        //steps to add multiple records under a transaction
        //    //        using (var transaction = db.Database.BeginTransaction())
        //    //    {
        //    //        string ErrorMessage = null;
        //    //        int ExceptionCount = 0;
        //    //        //try
        //    //        //{
        //    //            var RD = new Random();
        //    //            #region OldCode
        //    //            ////var BatchStatusId = db.RStatuses.Where(p => p.RsStatus == "ClaimBatchAccepted").Where(p => p.RStatusOwner.RsoStatusOwner == "Batch").FirstOrDefault().Id;
        //    //            ////var ClaimStatusId = db.RStatuses.Where(p => p.RsStatus == "Accepted").Where(p => p.RStatusOwner.RsoStatusOwner == "Claim").FirstOrDefault().Id;
        //    //            //var BatchModel = new LBatch { LbBatchNumber = BatchNumber, LbBatchType = "Claims", LbCompanyId = LClaim.FirstOrDefault().LcCompanyId, LbStatus = "ClaimBatchAccepted", LbUpdatedBy = LClaim.FirstOrDefault().LcCreatedById, LbUploadStartDateTime = DateTime.UtcNow };
        //    //            //db.LBatches.Add(BatchModel);
        //    //            //db.SaveChanges();
        //    //            //var batchFiles = new LBatchFile { LbfBatchId = BatchModel.Id, LbfFileName = FileName, LbfFileTimeStamp =DateTime.UtcNow };
        //    //            //db.LBatchFiles.Add(batchFiles);
        //    //            //db.SaveChanges();
        //    //            //var BatchId = BatchModel.Id;
        //    //            ////Note: Calculate Ordinal based on the Current Role who has created Manual Adjustment based on RoleId and Opco and WorkflowName
        //    //            //RK21062017var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LClaim.FirstOrDefault().LcCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == WorkflowName).FirstOrDefault().LwfcOrdinalNumber;
        //    //            //var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkflowName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        //    //            #endregion
        //    //            foreach (var claim in LClaim)
        //    //            {
        //    //                //try
        //    //                //{
        //    //                    //Selectsequence name to get claimId
        //    //                    //////var Claim = db.GKeyValues.Where(p => p.GkvCompanyId == claim.LcCompanyId).Where(p => p.GkvKey == "ClaimIdSequenceName").FirstOrDefault();
        //    //                    //////var RawQuery1 = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo." + Claim.GkvValue);
        //    //                    //////var Task1 = RawQuery1.SingleAsync();
        //    //                    //.LcClaimId = 57;// Task1.Result;
        //    //                    //Task.Dispose();
        //    //                    //claim.LcStatus = "Accepted";
        //    //                    //claim.WFStatus = "Saved";
        //    //                    //claim.WFOrdinal = 1;
        //    //                    //claim.WFType = "Claims";// WFDetails.RwfWFType;
        //    //                    //claim.LcClaimBatchNumber = 0;//RK BatchNumber;
        //    //                    //claim.WFRequesterRoleId = LoggedInRoleId;
        //    //                    //db.LClaims.Add(claim);
        //    //                    var model = new LClaim
        //    //                    {
        //    //                        A01 = claim.A01,
        //    //                        A02 = claim.A02,
        //    //                        A03 = claim.A03,
        //    //                        A04 = claim.A04,
        //    //                        A05 = claim.A05,
        //    //                        A06 = claim.A06,
        //    //                        A07 = claim.A07,
        //    //                        A08 = claim.A08,
        //    //                        A09 = claim.A09,
        //    //                        A10 = claim.A10,
        //    //                        LcActivityTypeId = claim.LcActivityTypeId,
        //    //                        LcAllocatedById = claim.LcAllocatedById,
        //    //                        LcAllocatedToId = claim.LcAllocatedToId,
        //    //                        LcAllocationDate = claim.LcAllocationDate,
        //    //                        LcAlreadyPaidAmount = claim.LcAlreadyPaidAmount,
        //    //                        LcAlreadyPaidDate = claim.LcAlreadyPaidDate,
        //    //                        LcAlreadyPaidDealer = claim.LcAlreadyPaidDealer,
        //    //                        LcApprovalDate = claim.LcApprovalDate,
        //    //                        LcApprovedById = claim.LcApprovedById,
        //    //                        LcBAN = claim.LcBAN,
        //    //                        LcBrandId = claim.LcBrandId,
        //    //                        LcClaimBatchNumber = 0,
        //    //                        LcClaimId = 111,// Task1.Result,
        //    //                        LcClawbackAmount = claim.LcClawbackAmount,
        //    //                        LcClawbackPayeeCode = claim.LcClawbackPayeeCode,
        //    //                        LcCommissionTypeId = claim.LcCommissionTypeId,
        //    //                        LcCompanyId = claim.LcCompanyId,
        //    //                        LcConnectionDate = claim.LcConnectionDate,
        //    //                        LcCreatedById = claim.LcCreatedById,
        //    //                        LcCreatedDateTime = claim.LcCreatedDateTime,
        //    //                        LcCustomerName = claim.LcCustomerName,
        //    //                        LcDeviceTypeId = claim.LcDeviceTypeId,
        //    //                        LcDuplicateClaimDetails = claim.LcDuplicateClaimDetails,
        //    //                        LcExpectedCommissionAmount = claim.LcExpectedCommissionAmount,
        //    //                        LcIMEI = claim.LcIMEI,
        //    //                        LcIsDuplicateClaim = claim.LcIsDuplicateClaim,
        //    //                        LcIsReclaim = claim.LcIsReclaim,
        //    //                        LcLastReclaimDate = claim.LcLastReclaimDate,
        //    //                        LcMSISDN = claim.LcMSISDN,
        //    //                        LcOrderDate = claim.LcOrderDate,
        //    //                        LcOrderNumber = claim.LcOrderNumber,
        //    //                        LcPayeeId = claim.LcPayeeId,
        //    //                        LcPaymentAmount = claim.LcPaymentAmount,
        //    //                        LcPaymentBatchNumber = 0,
        //    //                        LcPaymentCommissionTypeId = claim.LcPaymentCommissionTypeId,
        //    //                        LcPaymentDate = claim.LcPaymentDate,
        //    //                        LcProductCodeId = claim.LcProductCodeId,
        //    //                        LcReasonNonAutoPayment = claim.LcReasonNonAutoPayment,
        //    //                        LcRejectedById = claim.LcRejectedById,
        //    //                        LcRejectionDate = claim.LcRejectionDate,
        //    //                        LcRejectionReasonId = claim.LcRejectionReasonId,
        //    //                        LcSentForApprovalById = claim.LcSentForApprovalById,
        //    //                        LcSentForApprovalDate = claim.LcSentForApprovalDate,
        //    //                        LcStatus = "Accedpted",
        //    //                        WFStatus = "Saved",
        //    //                        WFOrdinal = 1,
        //    //                        WFType = "Claims",
        //    //                        WFRequesterRoleId = LoggedInRoleId,
        //    //                        WFCompanyId = claim.LcCompanyId,

        //    //                        LcWithdrawnById = claim.LcWithdrawnById,
        //    //                        LcWithdrawnDate = claim.LcWithdrawnDate,

        //    //                    };

        //    //                    db.LClaims.Add(model);
        //    //                    db.SaveChanges();
        //    //                    //Task1.Dispose();
        //    //             }
        //    //        //catch (DbEntityValidationException e)
        //    //        //{
        //    //        //    //string ErrorMessages = null;
        //    //        //    //foreach (var eve in e.EntityValidationErrors)
        //    //        //    //{
        //    //        //    //    ErrorMessages += string.Format("Entity of type {0} in state {1} has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //    //        //    //    foreach (var ve in eve.ValidationErrors)
        //    //        //    //    {
        //    //        //    //        ErrorMessages += string.Format("- Property: {0}, Error: {1}", ve.PropertyName, ve.ErrorMessage);
        //    //        //    //    }
        //    //        //    //}
        //    //        //    //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage));
        //    //        //}
        //    //        //catch (Exception ex)
        //    //        //{
        //    //        //    //var Body = "<table border='1'><tr><td>Application Name</td><td>" + "SOS" + "</td></tr><tr><td>Controller</td><td>" + s[2] + "</td></tr><tr><td>Method Name</td><td>" + s[3] + "</td></tr><tr><td>Exception Date/Time(Utc)</td><td>" + DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td>" + "" + "</td></tr><tr><td>Stack Trace</td><td>" + exceptionMessage + "</td></tr></table>";
        //    //        //    Globals.SendEmail("rkumar@megacube.com.au", "vgupta@megacube.com.au", "Claim upload error", ex.StackTrace.ToString(), "QA");//Need to disscuss from Where CompanyCode will be derived

        //    //        //    //var model = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "Uploadclaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.StackTrace.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
        //    //        //    //db.GErrorLogs.Add(model);
        //    //        //    //db.SaveChanges();
        //    //        //    //transaction.Commit();
        //    //        //    ErrorMessage += ex.Message;
        //    //        //    ExceptionCount  += 1;
        //    //        //    continue;
        //    //        //}
        //    //        transaction.Commit();
        //    //    }
        //    //            //db.SaveChanges();

        //    //    //if (ExceptionCount == 0)//data is not saved if a single exception is found
        //    //    //{
        //    //    //    transaction.Commit();
        //    //    //}
        //    //    //else
        //    //    //{
        //    //    //    return Ok(LClaim); //Faulty records are resent//throw new Exception();//This exception will again be caught in the outer catch  block
        //    //    //    //transaction.Commit();
        //    //    //}
        //    //    return Ok();

        //    //}

        //    //        //catch (SqlException ex)
        //    //        //{
        //    //        //    //transaction.Rollback();
        //    //        //    //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
        //    //        //}
        //    //        //catch (Exception ex)
        //    //        //{
        //    //        //    //transaction.Rollback();
        //    //        //    //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex));
        //    //        //}
        //    //        //return Ok(ErrorMessage);

        //    ////    }

        //    ////}
        //    //*/
        //    //        #endregion
        //    //    }
        //    //    if (iExeceptionsCount > 0)
        //    //    {
        //    //        var models = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "UploadClaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = strExceptionMessage, GelSourceProject = "[Vodafone-SOS WebApi]" };
        //    //        db.GErrorLogs.Add(models);
        //    //        db.SaveChanges();

        //    //    }
        //    //}
        //    //catch (Exception ex2)
        //    //{
        //    //    var modelf = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "UploadClaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex2.StackTrace.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
        //    //    db.GErrorLogs.Add(modelf);
        //    //    db.SaveChanges();

        //    //}
        //    #endregion

        //}
        [HttpPost]
        public IHttpActionResult ValidateUploadLClaims(List<LClaimDecryptedViewModel> LClaim, string FileName, string LoggedInRoleId, string UserName, string Workflow, int iCompanyId)
        {
            //string strConnectionString = "";
            //strConnectionString = CheckAvailableConnections(FileName);

            #region NewCode
            try
            {
                DataSet dsErrors = new DataSet();
                #region R2.1.3 changes
                string excelConnectionString = string.Empty;
                #region SOS Approach
                //try
                //{
                //    excelConnectionString = string.Format(System.Configuration.ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"], FileName);
                //}
                //catch (Exception ex)
                //{
                //    CreateDebugEntry(ex.ToString());
                //}
                //OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                //excelConnection.Open();
                //DataTable dt = new DataTable();
                //dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                //if (dt == null)
                //{
                //    CreateDebugEntry("nothing found to import");
                //    return null;
                //}
                //string[] excelSheets = new string[dt.Rows.Count];
                //int t = 0;
                ////excel data is saved in temporary file here
                //foreach (DataRow row in dt.Rows)
                //{
                //    excelSheets[t] = row["TABLE_NAME"].ToString();
                //    t++;
                //}
                //OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                //DataSet ds = new DataSet();
                //DataTable dtData = new DataTable();
                //CreateDebugEntry("trying to load sheet");
                //string query = string.Format("Select * from [{0}]", excelSheets[0]);
                #endregion
                #region RELY Approach
                //CreateDebugEntry("Start reading file");
                var CompanyDetails = db.GCompanies.Where(p => p.Id == iCompanyId).FirstOrDefault();
                string S3BucketRootFolder = ConfigurationManager.AppSettings["SOSBucketRootFolder"];
                string S3TargetPath = S3BucketRootFolder + CompanyDetails.GcCode + "/upload/claims/" + FileName;
                //CreateDebugEntry("Connecting to S3");
                //var bytedata = Globals.DownloadFromS3RKTest(  S3TargetPath);
                var bytedata = Globals.DownloadFromS3(S3TargetPath, "");
                //if (System.IO.File.Exists(FileName))
                //{
                //    System.IO.File.Delete(FileName);
                //}

                SqlDataAdapter adapter = new SqlDataAdapter();
                DataSet ds2 = new DataSet();
                DataTable dtdata = null;

                try
                {

                    OleDbConnection con = null;
                    try
                    {
                        string fileExtension = System.IO.Path.GetExtension(FileName);
                        string name = System.IO.Path.GetFileNameWithoutExtension(FileName);
                        string FileName_New = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + "_UPLOAD" + fileExtension;

                        string path = ConfigurationManager.AppSettings["TempDocumentPath"];
                        string fullpath = path + "\\" + FileName_New;
                        System.IO.File.WriteAllBytes(fullpath, bytedata); // Save File
                        //CreateDebugEntry("File saved from byte to excel.");
                        //string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
                        //SG - 2020/19/02- OLEDB connectionstring will be read from web.config file
                        string connectionString = ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"].Replace("{0}", fullpath);

                        con = new System.Data.OleDb.OleDbConnection(connectionString);
                        con.Open();
                        OleDbDataAdapter cmd2 = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [Claims$]", con);
                        cmd2.Fill(ds2);
                        dtdata = ds2.Tables[0];
                        con.Close();
                        if (!blnCheckTemplateColumns(dtdata))
                        {
                            DataTable dtE = new DataTable();
                            dtE.Columns.Add("ExceptionMessage");
                            dtE.Rows.Add("Invalid Template File, Please download fresh template and use the same for uploading claims.");
                            return Ok(dtE);
                        }

                        Boolean IsColumnMatched = false;
                        var yy = db.Database.SqlQuery<CompanySpecificColumnViewModel>("select LCSC.LcscIsMandatory,Lcsc.LcscIsReportParameter,Lcsc.LcscDataType,Lcsc.LcscReportParameterOrdinal,Lcsc.LcscDropDownId, LCSC.LcscOrdinalPosition, LCSC.LcscColumnName,LCSC.LcscDisplayOnForm,LCSC.Id,LCSC.LcscLabel,LCSC.LcscTooltip from LCompanySpecificColumns LCSC where LCSC.LcscTableName='LClaims' and  LCSC.LcscCompanyId={0}", iCompanyId).ToList();
                        for (int i = 0; i <= dtdata.Rows[0].ItemArray.Length - 1; i++)
                        {
                            IsColumnMatched = false;
                            if (dtdata.Rows[0].ItemArray[i] != null && Convert.ToString(dtdata.Rows[0].ItemArray[i]) != "" && Convert.ToString(dtdata.Rows[0].ItemArray[i]) != "Claim Id" && Convert.ToString(dtdata.Rows[0].ItemArray[i]) != "Comments")
                            {
                                string data = Convert.ToString(dtdata.Rows[0].ItemArray[i]);
                                data = data.Replace(@"*", string.Empty).Trim();


                                foreach (CompanySpecificColumnViewModel obj in yy)
                                {
                                    if (obj.LcscDisplayOnForm == true)
                                    {
                                        if (obj.LcscColumnName == Convert.ToString(data) || obj.LcscLabel == Convert.ToString(data))
                                        {
                                            IsColumnMatched = true;
                                            break;
                                        }

                                    }
                                }

                                if (IsColumnMatched == false)
                                {
                                    DataTable dtE = new DataTable();
                                    dtE.Columns.Add("ExceptionMessage");
                                    dtE.Rows.Add("Template Coulumn name has been changed, Please download the latest template file to proceed further.");
                                    return Ok(dtE);
                                }

                            }
                        }
                        if (dtdata.Rows.Count == 1)
                        {
                            DataTable dtE = new DataTable();
                            dtE.Columns.Add("ExceptionMessage");
                            dtE.Rows.Add("No Data in Template File, Please add the data in the template file.");
                            return Ok(dtE);
                        }
                    }

                    catch (Exception ex)
                    {
                        DataTable dtE = new DataTable();
                        dtE.Columns.Add("ExceptionMessage");
                        dtE.Rows.Add(ex.ToString());
                        return Ok(dtE);
                    }
                }
                catch (Exception ex)
                {
                    var models = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "UploadClaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
                    db.GErrorLogs.Add(models);
                    db.SaveChanges();
                }
                #endregion
                db.Database.ExecuteSqlCommand("delete from TClaimsUpload where WFRequester = '" + UserName.ToString() + "' and WFCompanyId = '" + iCompanyId.ToString() + "'");

                dtdata.Rows.RemoveAt(0);//Removing the 1st row of data table as it contains the column alias.
                #region Prepare data for bulk insert
                System.Data.DataColumn newColumn1 = new System.Data.DataColumn("WFRequesterRoleId", typeof(System.String));
                newColumn1.DefaultValue = LoggedInRoleId.ToString();
                dtdata.Columns.Add(newColumn1);
                System.Data.DataColumn newColumn2 = new System.Data.DataColumn("WFCompanyID", typeof(System.String));
                newColumn2.DefaultValue = iCompanyId.ToString();
                dtdata.Columns.Add(newColumn2);
                System.Data.DataColumn newColumn3 = new System.Data.DataColumn("WFRequester", typeof(System.String));
                newColumn3.DefaultValue = UserName;
                dtdata.Columns.Add(newColumn3);
                System.Data.SqlClient.SqlBulkCopy sqlBulk = new SqlBulkCopy(db.Database.Connection.ConnectionString);

                sqlBulk.ColumnMappings.Add("WFRequesterRoleId", "WFRequesterRoleId");
                sqlBulk.ColumnMappings.Add("WFCompanyID", "WFCompanyID");
                sqlBulk.ColumnMappings.Add("WFRequester", "WFRequester");
                sqlBulk.ColumnMappings.Add("LcActivityTypeId", "LcActivityTypeId");
                sqlBulk.ColumnMappings.Add("LcAlreadyPaidDate", "LcAlreadyPaidDate");
                sqlBulk.ColumnMappings.Add("LcAlreadyPaidDealer", "LcAlreadyPaidDealer");
                sqlBulk.ColumnMappings.Add("LcAlreadyPaidAmount", "LcAlreadyPaidAmount");
                sqlBulk.ColumnMappings.Add("LcAllocationDate", "LcAllocationDate");
                sqlBulk.ColumnMappings.Add("LcBAN", "LcBAN");
                sqlBulk.ColumnMappings.Add("LcBrandId", "LcBrandId");
                sqlBulk.ColumnMappings.Add("LcCommissionTypeId", "LcCommissionTypeId");
                sqlBulk.ColumnMappings.Add("LcConnectionDate", "LcConnectionDate");
                //sqlBulk.ColumnMappings.Add("LcCreatedById", "LcCreatedById");
                sqlBulk.ColumnMappings.Add("LcCustomerName", "LcCustomerName");
                //sqlBulk.ColumnMappings.Add("LcClaimBatchNumber", "LcClaimBatchNumber");
                sqlBulk.ColumnMappings.Add("LcClawbackPayeeCode", "LcClawbackPayeeCode");
                sqlBulk.ColumnMappings.Add("LcClawbackAmount", "LcClawbackAmount");
                sqlBulk.ColumnMappings.Add("LcDeviceTypeId", "LcDeviceTypeId");
                sqlBulk.ColumnMappings.Add("LcExpectedCommissionAmount", "LcExpectedCommissionAmount");
                sqlBulk.ColumnMappings.Add("LcIMEI", "LcIMEI");
                sqlBulk.ColumnMappings.Add("LcLastReclaimDate", "LcLastReclaimDate");
                sqlBulk.ColumnMappings.Add("LcMSISDN", "LcMSISDN");
                sqlBulk.ColumnMappings.Add("LcOrderDate", "LcOrderDate");
                sqlBulk.ColumnMappings.Add("LcOrderNumber", "LcOrderNumber");
                sqlBulk.ColumnMappings.Add("LcPaymentAmount", "LcPaymentAmount");
                //sqlBulk.ColumnMappings.Add("LcPaymentBatchNumber", "LcPaymentBatchNumber");
                sqlBulk.ColumnMappings.Add("LcPaymentCommissionTypeId", "LcPaymentCommissionTypeId");
                sqlBulk.ColumnMappings.Add("LcProductCodeId", "LcProductCodeId");
                sqlBulk.ColumnMappings.Add("LcReasonNonAutoPayment", "LcReasonNonAutoPayment");
                sqlBulk.ColumnMappings.Add("LcPayeeId", "LcPayeeId");
                sqlBulk.ColumnMappings.Add("LcRejectionReasonId", "LcRejectionReasonId");

                sqlBulk.ColumnMappings.Add("A01", "A01");
                sqlBulk.ColumnMappings.Add("A02", "A02");
                sqlBulk.ColumnMappings.Add("A03", "A03");
                sqlBulk.ColumnMappings.Add("A04", "A04");
                sqlBulk.ColumnMappings.Add("A05", "A05");
                sqlBulk.ColumnMappings.Add("A06", "A06");
                sqlBulk.ColumnMappings.Add("A07", "A07");
                sqlBulk.ColumnMappings.Add("A08", "A08");
                sqlBulk.ColumnMappings.Add("A09", "A09");
                sqlBulk.ColumnMappings.Add("A10", "A10");

                sqlBulk.ColumnMappings.Add("LcClaimId", "LcClaimId");
                sqlBulk.ColumnMappings.Add("WFComments", "WFComments");
                sqlBulk.ColumnMappings.Add("LcCommissionPeriod", "LcCommissionPeriod");
                sqlBulk.ColumnMappings.Add("LcParentPayeeId", "LcParentPayeeId");
                sqlBulk.DestinationTableName = "TClaimsUpload";
                #endregion
                try
                {
                    sqlBulk.WriteToServer(dtdata);

                }
                catch (Exception ex)
                {
                    var models = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "ValidateUploadLClaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
                    db.GErrorLogs.Add(models);
                    db.SaveChanges();
                    DataTable dtE = new DataTable();
                    dtE.Columns.Add("ExceptionMessage");
                    return Ok(dtE);
                }

                //excelConnection1.Dispose();
                //excelConnection.Dispose();
                if (System.IO.File.Exists(FileName))
                    System.IO.File.Delete(FileName);
                var Query = "Exec dbo.USPValidateAndInsertClaimsUploadData @UserID,@UserRoleID,@CompanyID,@UploadData";
                SqlCommand cmd = new SqlCommand(Query);
                cmd.Parameters.AddWithValue("@UserID", UserName);
                cmd.Parameters.AddWithValue("@UserRoleID", LoggedInRoleId.ToString());
                cmd.Parameters.AddWithValue("@CompanyID", iCompanyId.ToString());
                cmd.Parameters.AddWithValue("@UploadData", 0);
                dsErrors = GetData(cmd);
                if (dsErrors.Tables.Count > 0)
                {
                    if (dsErrors.Tables[0].Rows.Count > 0)
                    {
                        return Ok(dsErrors.Tables[0]);
                    }
                    else
                    {
                        return Ok();
                    }
                }
                else
                {
                    return Ok();
                }
                #endregion
            }
            catch (Exception ex)
            {
                var models = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "UploadClaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
                db.GErrorLogs.Add(models);
                db.SaveChanges();
                DataTable dtE = new DataTable();
                dtE.Columns.Add("ExceptionMessage");
                return Ok(dtE);
            }
            return Ok();
            #endregion
            #region OldCode
            //try
            //{
            //    using (var transaction2 = db.Database.BeginTransaction())
            //    {
            //        var RawQuery = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo.SQ_BatchNumber");
            //        var Task = RawQuery.SingleAsync();
            //        var BatchNumber = Task.Result;
            //        Task.Dispose();
            //        //var BatchStatusId =  db.RStatuses.Where(p => p.RsStatus == "ClaimBatchAccepted").Where(p => p.RStatusOwner.RsoStatusOwner == "Batch").FirstOrDefault().Id;
            //        //var ClaimStatusId = db.RStatuses.Where(p => p.RsStatus == "Accepted").Where(p => p.RStatusOwner.RsoStatusOwner == "Claim").FirstOrDefault().Id;
            //        //var BatchModel = new LBatch { LbBatchNumber = BatchNumber, LbBatchType = "Claims", LbCompanyId = LClaim.FirstOrDefault().LcCompanyId, LbStatus = "ClaimBatchAccepted", LbUpdatedBy = LClaim.FirstOrDefault().LcCreatedById, LbUploadStartDateTime = DateTime.UtcNow };
            //        //db.LBatches.Add(BatchModel);
            //        //db.SaveChanges();
            //        //var batchFiles = new LBatchFile { LbfBatchId = BatchModel.Id, LbfFileName = FileName, LbfFileTimeStamp = DateTime.UtcNow };
            //        //db.LBatchFiles.Add(batchFiles);
            //        //db.SaveChanges();
            //        //var BatchId = BatchModel.Id;
            //        //Note: Calculate Ordinal based on the Current Role who has created Manual Adjustment based on RoleId and Opco and WorkflowName
            //        var CompanyId = LClaim.FirstOrDefault().LcCompanyId;
            //        var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault().LwfcOrdinalNumber;
            //        var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();


            //        foreach (var claim in LClaim)
            //        {
            //            try
            //            {
            //                var Claim = db.GKeyValues.Where(p => p.GkvCompanyId == claim.LcCompanyId).Where(p => p.GkvKey == "ClaimIdSequenceName").FirstOrDefault();
            //                var RawQuery1 = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo." + Claim.GkvValue);
            //                var Task1 = RawQuery1.SingleAsync();
            //                //var model = new LClaim
            //                //{
            //                //    A01 = claim.A01,
            //                //    A02 = claim.A02,
            //                //    A03 = claim.A03,
            //                //    A04 = claim.A04,
            //                //    A05 = claim.A05,
            //                //    A06 = claim.A06,
            //                //    A07 = claim.A07,
            //                //    A08 = claim.A08,
            //                //    A09 = claim.A09,
            //                //    A10 = claim.A10,
            //                //    LcActivityTypeId = claim.LcActivityTypeId,
            //                //    LcAllocatedById = claim.LcAllocatedById,
            //                //    LcAllocatedToId = claim.LcAllocatedToId,
            //                //    LcAllocationDate = claim.LcAllocationDate,
            //                //    LcAlreadyPaidAmount = claim.LcAlreadyPaidAmount,
            //                //    LcAlreadyPaidDate = claim.LcAlreadyPaidDate,
            //                //    LcAlreadyPaidDealer = claim.LcAlreadyPaidDealer,
            //                //    LcApprovalDate = claim.LcApprovalDate,
            //                //    LcApprovedById = claim.LcApprovedById,
            //                //    LcBAN = claim.LcBAN,
            //                //    LcBrandId = claim.LcBrandId,
            //                //    LcClaimBatchNumber = BatchNumber,
            //                //    LcClaimId =  Task1.Result,
            //                //    LcClawbackAmount = claim.LcClawbackAmount,
            //                //    LcClawbackPayeeCode = claim.LcClawbackPayeeCode,
            //                //    LcCommissionTypeId = claim.LcCommissionTypeId,
            //                //    LcCompanyId = claim.LcCompanyId,
            //                //    LcConnectionDate = claim.LcConnectionDate,
            //                //    LcCreatedById = claim.LcCreatedById,
            //                //    LcCreatedDateTime = claim.LcCreatedDateTime,
            //                //    LcCustomerName = claim.LcCustomerName,
            //                //    LcDeviceTypeId = claim.LcDeviceTypeId,
            //                //    LcDuplicateClaimDetails = claim.LcDuplicateClaimDetails,
            //                //    LcExpectedCommissionAmount = claim.LcExpectedCommissionAmount,
            //                //    LcIMEI = claim.LcIMEI,
            //                //    LcIsDuplicateClaim = claim.LcIsDuplicateClaim,
            //                //    LcIsReclaim = claim.LcIsReclaim,
            //                //    LcLastReclaimDate = claim.LcLastReclaimDate,
            //                //    LcMSISDN = claim.LcMSISDN,
            //                //    LcOrderDate = claim.LcOrderDate,
            //                //    LcOrderNumber = claim.LcOrderNumber,
            //                //    LcPayeeId = claim.LcPayeeId,
            //                //    LcPaymentAmount = claim.LcPaymentAmount,
            //                //    LcPaymentBatchNumber = 0,
            //                //    LcPaymentCommissionTypeId = claim.LcPaymentCommissionTypeId,
            //                //    LcPaymentDate = claim.LcPaymentDate,
            //                //    LcProductCodeId = claim.LcProductCodeId,
            //                //    LcReasonNonAutoPayment = claim.LcReasonNonAutoPayment,
            //                //    LcRejectedById = claim.LcRejectedById,
            //                //    LcRejectionDate = claim.LcRejectionDate,
            //                //    LcRejectionReasonId = claim.LcRejectionReasonId,
            //                //    LcSentForApprovalById = claim.LcSentForApprovalById,
            //                //    LcSentForApprovalDate = claim.LcSentForApprovalDate,
            //                //    WFStatus = "Saved",
            //                //    WFOrdinal = Ordinal,
            //                //    WFType = "LClaims",
            //                //    WFRequesterId = claim.WFRequesterId,
            //                //    WFRequesterRoleId = LoggedInRoleId,
            //                //    WFCompanyId = claim.LcCompanyId,

            //                //    LcWithdrawnById = claim.LcWithdrawnById,
            //                //    LcWithdrawnDate = claim.LcWithdrawnDate,

            //                //};
            //                //Task1.Dispose();
            //                //db.LClaims.Add(model);
            //                //db.SaveChanges();

            //                //Encryption changes
            //                var model = new LClaimDecryptedViewModel
            //                {
            //                    A01 = claim.A01,
            //                    A02 = claim.A02,
            //                    A03 = claim.A03,
            //                    A04 = claim.A04,
            //                    A05 = claim.A05,
            //                    A06 = claim.A06,
            //                    A07 = claim.A07,
            //                    A08 = claim.A08,
            //                    A09 = claim.A09,
            //                    A10 = claim.A10,
            //                    LcActivityTypeId = claim.LcActivityTypeId,
            //                    LcAllocatedById = claim.LcAllocatedById,
            //                    LcAllocatedToId = claim.LcAllocatedToId,
            //                    LcAllocationDate = claim.LcAllocationDate,
            //                    LcAlreadyPaidAmount = claim.LcAlreadyPaidAmount,
            //                    LcAlreadyPaidDate = claim.LcAlreadyPaidDate,
            //                    LcAlreadyPaidDealer = claim.LcAlreadyPaidDealer,
            //                    LcApprovalDate = claim.LcApprovalDate,
            //                    LcApprovedById = claim.LcApprovedById,
            //                    LcBAN = claim.LcBAN,
            //                    LcBrandId = claim.LcBrandId,
            //                    LcClaimBatchNumber = BatchNumber,
            //                    LcClaimId = Task1.Result,
            //                    LcClawbackAmount = claim.LcClawbackAmount,
            //                    LcClawbackPayeeCode = claim.LcClawbackPayeeCode,
            //                    LcCommissionTypeId = claim.LcCommissionTypeId,
            //                    LcCompanyId = claim.LcCompanyId,
            //                    LcConnectionDate = claim.LcConnectionDate,
            //                    LcCreatedById = claim.LcCreatedById,
            //                    LcCreatedDateTime = claim.LcCreatedDateTime,
            //                    LcCustomerName = claim.LcCustomerName,
            //                    LcDeviceTypeId = claim.LcDeviceTypeId,
            //                    LcDuplicateClaimDetails = claim.LcDuplicateClaimDetails,
            //                    LcExpectedCommissionAmount = claim.LcExpectedCommissionAmount,
            //                    LcIMEI = claim.LcIMEI,
            //                    LcIsDuplicateClaim = claim.LcIsDuplicateClaim,
            //                    LcIsReclaim = claim.LcIsReclaim,
            //                    LcLastReclaimDate = claim.LcLastReclaimDate,
            //                    LcMSISDN = claim.LcMSISDN,
            //                    LcOrderDate = claim.LcOrderDate,
            //                    LcOrderNumber = claim.LcOrderNumber,
            //                    LcPayeeId = claim.LcPayeeId,
            //                    LcPaymentAmount = claim.LcPaymentAmount,
            //                    LcPaymentBatchNumber = 0,
            //                    LcPaymentCommissionTypeId = claim.LcPaymentCommissionTypeId,
            //                    LcPaymentDate = claim.LcPaymentDate,
            //                    LcProductCodeId = claim.LcProductCodeId,
            //                    LcReasonNonAutoPayment = claim.LcReasonNonAutoPayment,
            //                    LcRejectedById = claim.LcRejectedById,
            //                    LcRejectionDate = claim.LcRejectionDate,
            //                    LcRejectionReasonId = claim.LcRejectionReasonId,
            //                    LcSentForApprovalById = claim.LcSentForApprovalById,
            //                    LcSentForApprovalDate = claim.LcSentForApprovalDate,
            //                    WFStatus = "Saved",
            //                    WFOrdinal = Ordinal,
            //                    WFType = "LClaims",
            //                    WFRequesterId = claim.WFRequesterId,
            //                    WFRequesterRoleId = LoggedInRoleId,
            //                    WFCompanyId = claim.LcCompanyId,
            //                    WFCurrentOwnerId=claim.LcCreatedById,
            //                    LcWithdrawnById = claim.LcWithdrawnById,
            //                    LcWithdrawnDate = claim.LcWithdrawnDate,

            //                };
            //                //Call SP to save clams to DB
            //                var Id = Globals.ExecutePutOrPostLClaim(model, "Create");

            //                //Update Model Id if it is not null
            //                if (Id.HasValue)
            //                {
            //                    model.Id = Id.Value;
            //                    //03 July 2018 Now we are using SPUpdateActionStatus for Auto Approval of Claims Upload
            //                    var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
            //                    string SPresult = Globals.ExecuteSPUpdateActionStatus("Approve", "Claims", Convert.ToString(model.Id), CompanyId, model.LcCreatedById, "Empty", LoggedInRoleId, ProjectEnviournment, model.LcCreatedById);
            //                    var Payee = db.LPayees.Where(p => p.LpCompanyId == CompanyId).Where(p => p.Id == model.LcPayeeId).FirstOrDefault();
            //                    if (Payee != null)
            //                    {
            //                        var MEntityPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityType.Equals("LPayees", StringComparison.OrdinalIgnoreCase)).Where(p => p.MepEntityId == Payee.Id).ToList();
            //                        foreach (var PF in MEntityPortfolios)
            //                        {
            //                            var MEP = new MEntityPortfolio { MepPortfolioId = PF.MepPortfolioId, MepEntityId = model.Id, MepEntityType = "LClaims" };
            //                            db.MEntityPortfolios.Add(MEP);
            //                            db.SaveChanges();
            //                        }
            //                    }

            //                    //Old Approval Code Commented now
            //                    ////Skip functionality
            //                    //skipFunction:
            //                    //Ordinal++;
            //                    //Boolean blnSkip = false;
            //                    //var skipDetail = db.LWorkFlowConfigs.Where(p => p.LwfcWorkFlowId == WFDetails.Id).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcOrdinalNumber == Ordinal).FirstOrDefault().LwfcSkip;
            //                    //blnSkip = Convert.ToBoolean(skipDetail);

            //                    //if (blnSkip)
            //                    //{
            //                    //    blnSkip = false;
            //                    //    try
            //                    //    {
            //                    //        //Placed this piece of code in try block as it throws exception when the SkipFunctionName is null
            //                    //        string skipFunctionName = Convert.ToString(db.LWorkFlowConfigs.Where(p => p.LwfcWorkFlowId == WFDetails.Id).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcOrdinalNumber == Ordinal).FirstOrDefault().LwfcSkipFunctionName);
            //                    //        if (!string.IsNullOrEmpty(skipFunctionName.ToString()))
            //                    //        {
            //                    //            blnSkip = db.Database.SqlQuery<bool>("select dbo." + skipFunctionName.ToString() + "()").FirstOrDefault<bool>();
            //                    //        }
            //                    //        if (blnSkip)
            //                    //        {
            //                    //            goto skipFunction;
            //                    //        }
            //                    //    }
            //                    //    catch (Exception ex)
            //                    //    { }

            //                    //}

            //                    ////Approval/Sending to next ordinal
            //                    //db.Database.ExecuteSqlCommand("update LClaims set WFOrdinal = " + (Ordinal) + ", WFCurrentOwnerId = NULL where id = " + model.Id + "");
            //                    ////NoMatchFound
            //                    //var RawQuery2 = db.Database.SqlQuery<string>("select dbo.FNGetUserForAllocation('LClaims','LClaims'," + model.Id.ToString() + "," + (Ordinal).ToString() + "," + CompanyId + ")");
            //                    //var Task2 = RawQuery2.SingleAsync();
            //                    //var NewOwner = Task2.Result;
            //                    //if (NewOwner.ToString() != "NoMatchFound")
            //                    //{

            //                    //    string strActingAs = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcWorkFlowId == WFDetails.Id).Where(p => p.LwfcOrdinalNumber == Ordinal).FirstOrDefault().LwfcActingAs;
            //                    //    if (strActingAs == "Analyst")
            //                    //    {
            //                    //        db.Database.ExecuteSqlCommand("update LClaims set  WFCurrentOwnerId = '" + NewOwner.ToString() + "', WFAnalystId = '" + NewOwner + "' where id = " + model.Id + "");
            //                    //    }
            //                    //    else if (strActingAs == "Manager")
            //                    //    {
            //                    //        db.Database.ExecuteSqlCommand("update LClaims set  WFCurrentOwnerId = '" + NewOwner.ToString() + "', WFManagerId = '" + NewOwner + "' where id = " + model.Id + "");
            //                    //    }
            //                    //    else
            //                    //    {
            //                    //        db.Database.ExecuteSqlCommand("update LClaims set  WFCurrentOwnerId = '" + NewOwner.ToString() + "' where id = " + model.Id + "");
            //                    //    }
            //                    //    //db.Database.ExecuteSqlCommand("update LClaims set  WFCurrentOwnerId = '" + NewOwner.ToString() + "', WFManagerId = '" + strManagerID+ "', WFAnalystId = '" + strAnalystID + "' where id = " + model.Id + "");
            //                    //}
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                iExeceptionsCount++;
            //                strExceptionMessage += ex.StackTrace.ToString();
            //            }
            //        }
            //        if (iExeceptionsCount == 0)
            //        {
            //            transaction2.Commit();
            //        }
            //        else
            //        {
            //            transaction2.Rollback();
            //        }
            //        #endregion
            //        #region OLD Code Commented
            //        /*
            //        //var RawQuery = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo.SQ_BatchNumber");
            //        //var Task = RawQuery.SingleAsync();
            //        //var BatchNumber = Task.Result;
            //        //Task.Dispose();

            //        //steps to add multiple records under a transaction
            //        using (var transaction = db.Database.BeginTransaction())
            //    {
            //        string ErrorMessage = null;
            //        int ExceptionCount = 0;
            //        //try
            //        //{
            //            var RD = new Random();
            //            #region OldCode
            //            ////var BatchStatusId = db.RStatuses.Where(p => p.RsStatus == "ClaimBatchAccepted").Where(p => p.RStatusOwner.RsoStatusOwner == "Batch").FirstOrDefault().Id;
            //            ////var ClaimStatusId = db.RStatuses.Where(p => p.RsStatus == "Accepted").Where(p => p.RStatusOwner.RsoStatusOwner == "Claim").FirstOrDefault().Id;
            //            //var BatchModel = new LBatch { LbBatchNumber = BatchNumber, LbBatchType = "Claims", LbCompanyId = LClaim.FirstOrDefault().LcCompanyId, LbStatus = "ClaimBatchAccepted", LbUpdatedBy = LClaim.FirstOrDefault().LcCreatedById, LbUploadStartDateTime = DateTime.UtcNow };
            //            //db.LBatches.Add(BatchModel);
            //            //db.SaveChanges();
            //            //var batchFiles = new LBatchFile { LbfBatchId = BatchModel.Id, LbfFileName = FileName, LbfFileTimeStamp =DateTime.UtcNow };
            //            //db.LBatchFiles.Add(batchFiles);
            //            //db.SaveChanges();
            //            //var BatchId = BatchModel.Id;
            //            ////Note: Calculate Ordinal based on the Current Role who has created Manual Adjustment based on RoleId and Opco and WorkflowName
            //            //RK21062017var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LClaim.FirstOrDefault().LcCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == WorkflowName).FirstOrDefault().LwfcOrdinalNumber;
            //            //var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkflowName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            //            #endregion
            //            foreach (var claim in LClaim)
            //            {
            //                //try
            //                //{
            //                    //Selectsequence name to get claimId
            //                    //////var Claim = db.GKeyValues.Where(p => p.GkvCompanyId == claim.LcCompanyId).Where(p => p.GkvKey == "ClaimIdSequenceName").FirstOrDefault();
            //                    //////var RawQuery1 = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo." + Claim.GkvValue);
            //                    //////var Task1 = RawQuery1.SingleAsync();
            //                    //.LcClaimId = 57;// Task1.Result;
            //                    //Task.Dispose();
            //                    //claim.LcStatus = "Accepted";
            //                    //claim.WFStatus = "Saved";
            //                    //claim.WFOrdinal = 1;
            //                    //claim.WFType = "Claims";// WFDetails.RwfWFType;
            //                    //claim.LcClaimBatchNumber = 0;//RK BatchNumber;
            //                    //claim.WFRequesterRoleId = LoggedInRoleId;
            //                    //db.LClaims.Add(claim);
            //                    var model = new LClaim
            //                    {
            //                        A01 = claim.A01,
            //                        A02 = claim.A02,
            //                        A03 = claim.A03,
            //                        A04 = claim.A04,
            //                        A05 = claim.A05,
            //                        A06 = claim.A06,
            //                        A07 = claim.A07,
            //                        A08 = claim.A08,
            //                        A09 = claim.A09,
            //                        A10 = claim.A10,
            //                        LcActivityTypeId = claim.LcActivityTypeId,
            //                        LcAllocatedById = claim.LcAllocatedById,
            //                        LcAllocatedToId = claim.LcAllocatedToId,
            //                        LcAllocationDate = claim.LcAllocationDate,
            //                        LcAlreadyPaidAmount = claim.LcAlreadyPaidAmount,
            //                        LcAlreadyPaidDate = claim.LcAlreadyPaidDate,
            //                        LcAlreadyPaidDealer = claim.LcAlreadyPaidDealer,
            //                        LcApprovalDate = claim.LcApprovalDate,
            //                        LcApprovedById = claim.LcApprovedById,
            //                        LcBAN = claim.LcBAN,
            //                        LcBrandId = claim.LcBrandId,
            //                        LcClaimBatchNumber = 0,
            //                        LcClaimId = 111,// Task1.Result,
            //                        LcClawbackAmount = claim.LcClawbackAmount,
            //                        LcClawbackPayeeCode = claim.LcClawbackPayeeCode,
            //                        LcCommissionTypeId = claim.LcCommissionTypeId,
            //                        LcCompanyId = claim.LcCompanyId,
            //                        LcConnectionDate = claim.LcConnectionDate,
            //                        LcCreatedById = claim.LcCreatedById,
            //                        LcCreatedDateTime = claim.LcCreatedDateTime,
            //                        LcCustomerName = claim.LcCustomerName,
            //                        LcDeviceTypeId = claim.LcDeviceTypeId,
            //                        LcDuplicateClaimDetails = claim.LcDuplicateClaimDetails,
            //                        LcExpectedCommissionAmount = claim.LcExpectedCommissionAmount,
            //                        LcIMEI = claim.LcIMEI,
            //                        LcIsDuplicateClaim = claim.LcIsDuplicateClaim,
            //                        LcIsReclaim = claim.LcIsReclaim,
            //                        LcLastReclaimDate = claim.LcLastReclaimDate,
            //                        LcMSISDN = claim.LcMSISDN,
            //                        LcOrderDate = claim.LcOrderDate,
            //                        LcOrderNumber = claim.LcOrderNumber,
            //                        LcPayeeId = claim.LcPayeeId,
            //                        LcPaymentAmount = claim.LcPaymentAmount,
            //                        LcPaymentBatchNumber = 0,
            //                        LcPaymentCommissionTypeId = claim.LcPaymentCommissionTypeId,
            //                        LcPaymentDate = claim.LcPaymentDate,
            //                        LcProductCodeId = claim.LcProductCodeId,
            //                        LcReasonNonAutoPayment = claim.LcReasonNonAutoPayment,
            //                        LcRejectedById = claim.LcRejectedById,
            //                        LcRejectionDate = claim.LcRejectionDate,
            //                        LcRejectionReasonId = claim.LcRejectionReasonId,
            //                        LcSentForApprovalById = claim.LcSentForApprovalById,
            //                        LcSentForApprovalDate = claim.LcSentForApprovalDate,
            //                        LcStatus = "Accedpted",
            //                        WFStatus = "Saved",
            //                        WFOrdinal = 1,
            //                        WFType = "Claims",
            //                        WFRequesterRoleId = LoggedInRoleId,
            //                        WFCompanyId = claim.LcCompanyId,

            //                        LcWithdrawnById = claim.LcWithdrawnById,
            //                        LcWithdrawnDate = claim.LcWithdrawnDate,

            //                    };

            //                    db.LClaims.Add(model);
            //                    db.SaveChanges();
            //                    //Task1.Dispose();
            //             }
            //        //catch (DbEntityValidationException e)
            //        //{
            //        //    //string ErrorMessages = null;
            //        //    //foreach (var eve in e.EntityValidationErrors)
            //        //    //{
            //        //    //    ErrorMessages += string.Format("Entity of type {0} in state {1} has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        //    //    foreach (var ve in eve.ValidationErrors)
            //        //    //    {
            //        //    //        ErrorMessages += string.Format("- Property: {0}, Error: {1}", ve.PropertyName, ve.ErrorMessage);
            //        //    //    }
            //        //    //}
            //        //    //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage));
            //        //}
            //        //catch (Exception ex)
            //        //{
            //        //    //var Body = "<table border='1'><tr><td>Application Name</td><td>" + "SOS" + "</td></tr><tr><td>Controller</td><td>" + s[2] + "</td></tr><tr><td>Method Name</td><td>" + s[3] + "</td></tr><tr><td>Exception Date/Time(Utc)</td><td>" + DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td>" + "" + "</td></tr><tr><td>Stack Trace</td><td>" + exceptionMessage + "</td></tr></table>";
            //        //    Globals.SendEmail("rkumar@megacube.com.au", "vgupta@megacube.com.au", "Claim upload error", ex.StackTrace.ToString(), "QA");//Need to disscuss from Where CompanyCode will be derived

            //        //    //var model = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "Uploadclaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.StackTrace.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
            //        //    //db.GErrorLogs.Add(model);
            //        //    //db.SaveChanges();
            //        //    //transaction.Commit();
            //        //    ErrorMessage += ex.Message;
            //        //    ExceptionCount  += 1;
            //        //    continue;
            //        //}
            //        transaction.Commit();
            //    }
            //            //db.SaveChanges();

            //    //if (ExceptionCount == 0)//data is not saved if a single exception is found
            //    //{
            //    //    transaction.Commit();
            //    //}
            //    //else
            //    //{
            //    //    return Ok(LClaim); //Faulty records are resent//throw new Exception();//This exception will again be caught in the outer catch  block
            //    //    //transaction.Commit();
            //    //}
            //    return Ok();

            //}

            //        //catch (SqlException ex)
            //        //{
            //        //    //transaction.Rollback();
            //        //    //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            //        //}
            //        //catch (Exception ex)
            //        //{
            //        //    //transaction.Rollback();
            //        //    //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex));
            //        //}
            //        //return Ok(ErrorMessage);

            ////    }

            ////}
            //*/
            //        #endregion
            //    }
            //    if (iExeceptionsCount > 0)
            //    {
            //        var models = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "UploadClaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = strExceptionMessage, GelSourceProject = "[Vodafone-SOS WebApi]" };
            //        db.GErrorLogs.Add(models);
            //        db.SaveChanges();

            //    }
            //}
            //catch (Exception ex2)
            //{
            //    var modelf = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "UploadClaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex2.StackTrace.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
            //    db.GErrorLogs.Add(modelf);
            //    db.SaveChanges();

            //}
            #endregion

        }


        [HttpPost]
        public IHttpActionResult UploadClaims(string LoggedInRoleId, string UserName, string Workflow, int iCompanyId)
        {
            var Query = "Exec dbo.USPValidateAndInsertClaimsUploadData @UserID,@UserRoleID,@CompanyID,@UploadData";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@UserID", UserName);
            cmd.Parameters.AddWithValue("@UserRoleID", LoggedInRoleId.ToString());
            cmd.Parameters.AddWithValue("@CompanyID", iCompanyId.ToString());
            cmd.Parameters.AddWithValue("@UploadData", "1");
            DataSet dsErrors = GetData(cmd);
            return Ok();
        }
        private string CheckAvailableConnections(string FileName)
        {
            string strConnStr = "";
            
            try
            {
                DataTable dtconnction = new DataTable();
                System.Data.OleDb.OleDbConnection excelConnection;
                System.Data.OleDb.OleDbDataAdapter MyCommand;
                OleDbEnumerator enumerator = new OleDbEnumerator();
                DataTable table = enumerator.GetElements();
                string sRowsfilter = String.Format("SOURCES_NAME LIKE '*{0}*'", "Microsoft.ACE.OLEDB");
                System.Data.DataRow[] rows = table.Select(sRowsfilter, "SOURCES_NAME");
                dtconnction = table.Clone();
                foreach (DataRow dr in rows)
                    dtconnction.ImportRow(dr);
                excelConnection = new System.Data.OleDb.OleDbConnection(@"Provider=" + dtconnction.Rows[0][0].ToString() + ";Data Source=" + FileName + ";Extended Properties=Excel " + dtconnction.Rows[0][0].ToString().Replace("Microsoft.ACE.OLEDB.", "") + ";");
                strConnStr = excelConnection.ConnectionString;
                
            }
            catch (Exception ex)
            {
            }
            return strConnStr;


        }
        public static DataSet GetData(SqlCommand cmd)
        {
            DataSet ds = new DataSet();
            String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(strConnString);
            SqlDataAdapter sda = new SqlDataAdapter();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            try
            {
                con.Open();
                sda.SelectCommand = cmd;
                sda.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                sda.Dispose();
                con.Dispose();
            }
        }
        //
        // GET: api/LClaims Get Claims List to be exported as csv or displayed as grid between the start date and end date and for the payeeId passed as parameter
        [HttpGet]
        public IHttpActionResult GetLClaimsForReports(string PayeeId,string StartDate,string EndDate,int CompanyId, //int pagesize, int pagenum, string sortdatafield, string sortorder,string FilterQuery, string UserName, string Workflow,
            string A01Filter, string A01, string A02Filter, string A02, string A03Filter, string A03, string A04Filter, string A04, string A05Filter, string A05,
            string A06Filter, string A06, string A07Filter, string A07, string A08Filter, string A08, string A09Filter, string A09, string A10Filter, string A10,
            string AllocationDateFilter, string AllocationDateFrom,string AllocationDateTo, 
            string AlreadyPaidDateFilter, string AlreadyPaidDateFrom, string AlreadyPaidDateTo, string ConnectionDateFilter,
            string ConnectionDateFrom, string ConnectionDateTo, string LastReclaimDateFilter, string LastReclaimDateFrom,
            string LastReclaimDateTo, string OrderDateFilter, string OrderDateFrom, string OrderDateTo, string AlreadyPaidAmountFilter,
            string AlreadyPaidAmountFrom, string AlreadyPaidAmountTo, string ClawbackAmountFilter, string ClawbackAmountFrom, string ClawbackAmountTo,
            string ExpectedCommissionAmountFilter, string ExpectedCommissionAmountFrom, string ExpectedCommissionAmountTo, string PaymentAmountFilter,
            string PaymentAmountFrom, string PaymentAmountTo,
            string BANFilter, string BAN, string CustomerNameFilter, string CustomerName,
            string IMEIFilter, string IMEI, string MSISDNFilter, string MSISDN, string OrderNumberFilter, string OrderNumber, string PaymentBatchNumberFilter,
            string PaymentBatchNumber, string ReasonNonAutoPaymentFilter, string ReasonNonAutoPayment, string ClaimBatchNumberFilter, string ClaimBatchNumber,
            string ClawbackPayeeCodeFilter, string ClawbackPayeeCode, string BrandIds, string CommissionTypeIds, string DeviceTypeIds, string PaymentCommissionTypeIds,
            string ProductCodeIds, string StatusFilter, string Status, string CreatedByIds, string ActivityTypeIds, string AlreadyPaidDealer,string RejectionReasonIds
            )
        {
            //These vaiables are declared to convert start date and end date to datetime as in linq datetime objects are required for comparison
            //var StartDateTime = DateTime.ParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //var EndDateTime = DateTime.ParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var userlist = db.AspNetUsers.Where(p => p.GcCompanyId == CompanyId).Select(p => new { p.Id, p.UserName });
            //if(string.IsNullOrEmpty(FilterQuery))
            //{
            //    FilterQuery = string.Empty;
            //}

            //var RawQuery = db.Database.SqlQuery<ClaimsReportViewModel>("exec spGetClaimsReportData '" + PayeeId + "', '" + StartDate + "' , '" + EndDate + "' , '" + CompanyId.ToString() + "' , '" + A01Filter + "' , '" + A01 + "' , '" + A02Filter + "' , '" + A02 + "' , '" + A03Filter + "' , '" + A03 + "' , '" + A04Filter + "' , '" + A04 + "' , '" + A05Filter + "' , '" + A05 + "' , '" + A06Filter + "' , '" + A06 + "' , '" + A07Filter + "' , '" + A07 + "' , '" + A08Filter + "' , '" + A08 + "' , '" + A09Filter + "' , '" + A09 + "' , '" + A10Filter + "' , '" + A10 + "' ,'" + AllocationDateFilter + "' , '" + AllocationDateFrom + "' ,'" + AllocationDateTo + "' ,'" + AlreadyPaidDateFilter + "' ,'" + AlreadyPaidDateFrom + "' ,'" + AlreadyPaidDateTo + "' ,'" + ConnectionDateFilter + "' ,'" + ConnectionDateFrom + "' ,'" + ConnectionDateTo + "' ,'" + LastReclaimDateFilter + "' ,'" + LastReclaimDateFrom + "' ,'" + LastReclaimDateTo + "' ,'" + OrderDateFilter + "' ,'" + OrderDateFrom + "' ,'" + OrderDateTo + "' ,'" + AlreadyPaidAmountFilter + "' ,'" + AlreadyPaidAmountFrom + "' ,'" + AlreadyPaidAmountTo + "' ,'" + ClawbackAmountFilter + "' ,'" + ClawbackAmountFrom + "' ,'" + ClawbackAmountTo + "' ,'" + ExpectedCommissionAmountFilter + "' ,'" + ExpectedCommissionAmountFrom + "' ,'" + ExpectedCommissionAmountTo + "' ,'" + PaymentAmountFilter + "' ,'" + PaymentAmountFrom + "' ,'" + PaymentAmountTo + "' ");//,'" + BANFilter + "' ,'" + BAN + "' ,'" + CustomerNameFilter + "' ,'" + CustomerName + "' ,'" + IMEIFilter + "' ,'" + IMEI + "' ,'" + MSISDNFilter + "' ,'" + MSISDN + "' ,'" + OrderNumberFilter + "' ,'" + OrderNumber + "' ,'" + PaymentBatchNumberFilter + "' ,'" + PaymentBatchNumber + "' ,'" + ReasonNonAutoPaymentFilter + "' ,'" + ReasonNonAutoPayment + "' ,'" + ClaimBatchNumberFilter + "' ,'" + ClaimBatchNumber + "' ,'" + ClawbackPayeeCodeFilter + "' ,'" + ClawbackPayeeCode + "' ,'" + BrandIds + "' ,'" + CommissionTypeIds + "' ,'" + DeviceTypeIds + "' ,'" + PaymentCommissionTypeIds + "' ,'" + ProductCodeIds + "' ,'" + StatusFilter + "' ,'" + Status + "' ,'" + CreatedByIds + "' ,'" + ActivityTypeIds + "' ,'" + AlreadyPaidDealer + "' ");
            var RawQuery = db.Database.SqlQuery<ClaimsReportViewModel>("exec spGetClaimsReportData '" + PayeeId + "', '" + StartDate + "' , '" + EndDate + "' , '" + CompanyId.ToString() + "' , '" + A01Filter + "' , '" + A01 + "' , '" + A02Filter + "' , '" + A02 + "' , '" + A03Filter + "' , '" + A03 + "' , '" + A04Filter + "' , '" + A04 + "' , '" + A05Filter + "' , '" + A05 + "' , '" + A06Filter + "' , '" + A06 + "' , '" + A07Filter + "' , '" + A07 + "' , '" + A08Filter + "' , '" + A08 + "' , '" + A09Filter + "' , '" + A09 + "' , '" + A10Filter + "' , '" + A10 + "' ,'" + AllocationDateFilter + "' , '" + AllocationDateFrom + "' ,'" + AllocationDateTo + "' ,'" + AlreadyPaidDateFilter + "' ,'" + AlreadyPaidDateFrom + "' ,'" + AlreadyPaidDateTo + "' ,'" + ConnectionDateFilter + "' ,'" + ConnectionDateFrom + "' ,'" + ConnectionDateTo + "' ,'" + LastReclaimDateFilter + "' ,'" + LastReclaimDateFrom + "' ,'" + LastReclaimDateTo + "' ,'" + OrderDateFilter + "' ,'" + OrderDateFrom + "' ,'" + OrderDateTo + "' ,'" + AlreadyPaidAmountFilter + "' ,'" + AlreadyPaidAmountFrom + "' ,'" + AlreadyPaidAmountTo + "' ,'" + ClawbackAmountFilter + "' ,'" + ClawbackAmountFrom + "' ,'" + ClawbackAmountTo + "' ,'" + ExpectedCommissionAmountFilter + "' ,'" + ExpectedCommissionAmountFrom + "' ,'" + ExpectedCommissionAmountTo + "' ,'" + PaymentAmountFilter + "' ,'" + PaymentAmountFrom + "' ,'" + PaymentAmountTo + "' ,'" + BANFilter + "' ,'" + BAN + "' ,'" + CustomerNameFilter + "' ,'" + CustomerName + "' ,'" + IMEIFilter + "' ,'" + IMEI + "' ,'" + MSISDNFilter + "' ,'" + MSISDN + "' ,'" + OrderNumberFilter + "' ,'" + OrderNumber + "' ,'" + PaymentBatchNumberFilter + "' ,'" + PaymentBatchNumber + "' ,'" + ReasonNonAutoPaymentFilter + "' ,'" + ReasonNonAutoPayment + "' ,'" + ClaimBatchNumberFilter + "' ,'" + ClaimBatchNumber + "' ,'" + ClawbackPayeeCodeFilter + "' ,'" + ClawbackPayeeCode + "' ,'" + BrandIds + "' ,'" + CommissionTypeIds + "' ,'" + DeviceTypeIds + "' ,'" + PaymentCommissionTypeIds + "' ,'" + ProductCodeIds + "' ,'" + StatusFilter + "' ,'" + Status + "' ,'" + CreatedByIds + "' ,'" + ActivityTypeIds + "' ,'" + AlreadyPaidDealer + "', '" + RejectionReasonIds+"'");




            //var Task = RawQuery.ToList();
            var Task = RawQuery.ToList();
            var ListData = Task.ToList();

            return Ok(ListData);
            
            /////Commented by RK, added the stored procedure
            ////////////changes by RG to add column in claimGrid
            //////////string Qry = "select * From (select LcStatus as Status ,A01 ,A02 ,A03 ,A04 ,A05 ,A06 ,A07 ,A08 ,A09 ,A10,LcAlreadyPaidDate as AlreadyPaidDate,LcAlreadyPaidDealer as AlreadyPaidDealer,LcBAN as BAN,LcBrandId as BrandId,LcClaimBatchNumber as ClaimBatchNumber,LcClawbackAmount as ClawbackAmount,LcClawbackPayeeCode as ClawbackPayeeCode,LcCommissionTypeId as CommissionTypeId,LcConnectionDate as ConnectionDate,dbo.FnGetUserName(LcCreatedById) as CreatedById,LcCustomerName as CustomerName,LcDeviceTypeId as DeviceTypeId,LcExpectedCommissionAmount as ExpectedCommissionAmount,LcIMEI as IMEI,LcLastReclaimDate as LastReclaimDate,LcMSISDN as MSISDN,LcOrderDate as OrderDate,LcOrderNumber as OrderNumber,LcPayeeId as PayeeId,LcPaymentAmount as PaymentAmount,LcPaymentBatchNumber as PaymentBatchNumber,LcPaymentCommissionTypeId as PaymentCommissionTypeId,LcReasonNonAutoPayment as ReasonNonAutoPayment,LcActivityTypeId as ActivityTypeId,LcAllocationDate as AllocationDate,LcAlreadyPaidAmount as AlreadyPaidAmount,WFStatus, ROW_NUMBER() OVER (ORDER BY LcCreatedDateTime) as row  from LClaims where LcPayeeId in (SELECT CAST(Item AS INTEGER) FROM dbo.SplitString( '" + PayeeId+"', ',')) and LcCreatedDateTime BETWEEN '" + StartDate+"' AND '"+EndDate+"' ) a ";
            //////////Qry = Qry + " Where row > "+ pagenum * pagesize + " And row <= "+ (pagenum + 1) * pagesize;
            ////////// Qry = Qry +" "+ FilterQuery;

            ////////////SS :Roll back the parameterized sql query asit is throwing error. Will replace once resolution found
            //////////if (!string.IsNullOrEmpty(sortorder))//code for server side filtering
            //////////{
            //////////    if (sortorder == "asc")
            //////////    {
            //////////        Qry += " order by "+ sortdatafield;
            //////////    }
            //////////    else
            //////////    {
            //////////        Qry += " order by "+ sortdatafield + " desc";
            //////////    }
            //////////    //, PayeeId, StartDate, EndDate, pagenum * pagesize, (pagenum + 1) * pagesize, FilterQuery,sortdatafield
            //////////    var xx = db.Database.SqlQuery<ClaimsReportViewModel>(Qry).ToList();
            //////////    return Ok(xx);
            //////////}
            //////////else
            //////////{   
            //////////    var xx = db.Database.SqlQuery<ClaimsReportViewModel>(Qry).ToList();
            //////////    return Ok(xx);
            //////////}

        }

        // GET: api/LClaims Get Claims List to be exported as csv or displayed as grid between the start date and end date and for the payeeId passed as parameter
        [HttpGet]
        public IHttpActionResult DownloadLClaimsForReports(string PayeeId, string StartDate, string EndDate, int CompanyId, //int pagesize, int pagenum, string sortdatafield, string sortorder,string FilterQuery, string UserName, string Workflow,
            string A01Filter, string A01, string A02Filter, string A02, string A03Filter, string A03, string A04Filter, string A04, string A05Filter, string A05,
            string A06Filter, string A06, string A07Filter, string A07, string A08Filter, string A08, string A09Filter, string A09, string A10Filter, string A10,
            string AllocationDateFilter, string AllocationDateFrom, string AllocationDateTo,
            string AlreadyPaidDateFilter, string AlreadyPaidDateFrom, string AlreadyPaidDateTo, string ConnectionDateFilter,
            string ConnectionDateFrom, string ConnectionDateTo, string LastReclaimDateFilter, string LastReclaimDateFrom,
            string LastReclaimDateTo, string OrderDateFilter, string OrderDateFrom, string OrderDateTo, string AlreadyPaidAmountFilter,
            string AlreadyPaidAmountFrom, string AlreadyPaidAmountTo, string ClawbackAmountFilter, string ClawbackAmountFrom, string ClawbackAmountTo,
            string ExpectedCommissionAmountFilter, string ExpectedCommissionAmountFrom, string ExpectedCommissionAmountTo, string PaymentAmountFilter,
            string PaymentAmountFrom, string PaymentAmountTo,
            string BANFilter, string BAN, string CustomerNameFilter, string CustomerName,
            string IMEIFilter, string IMEI, string MSISDNFilter, string MSISDN, string OrderNumberFilter, string OrderNumber, string PaymentBatchNumberFilter,
            string PaymentBatchNumber, string ReasonNonAutoPaymentFilter, string ReasonNonAutoPayment, string ClaimBatchNumberFilter, string ClaimBatchNumber,
            string ClawbackPayeeCodeFilter, string ClawbackPayeeCode, string BrandIds, string CommissionTypeIds, string DeviceTypeIds, string PaymentCommissionTypeIds,
            string ProductCodeIds, string StatusFilter, string Status, string CreatedByIds, string ActivityTypeIds, string AlreadyPaidDealer,string UserName,string RejectionReasonIds)
        {
            #region OldMethod
            ////old method DownloadLClaimsForReports(string PayeeId, string StartDate, string EndDate, int CompanyId, string UserName, string Workflow)
            ////These vaiables are declared to convert start date and end date to datetime as in linq datetime objects are required for comparison
            ////var StartDateTime = DateTime.ParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            ////var EndDateTime = DateTime.ParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //var userlist = db.AspNetUsers.Where(p => p.GcCompanyId == CompanyId).Select(p => new { p.Id, p.UserName });
            //var Company = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
            //var CompanySpecificColumns = db.LCompanySpecificColumns.Where(p => p.LcscTableName == "LClaims").Where(p => p.LcscCompanyId == CompanyId).Where(p => p.LcscDisplayOnForm == true).ToList();
            //var ColList = "";
            //foreach(var CompanySpecificColumn in CompanySpecificColumns)
            //{
            //    if (CompanySpecificColumn.LcscColumnName == "A01" || CompanySpecificColumn.LcscColumnName == "A02" || CompanySpecificColumn.LcscColumnName == "A03" || CompanySpecificColumn.LcscColumnName == "A04" || CompanySpecificColumn.LcscColumnName == "A05" || CompanySpecificColumn.LcscColumnName == "A06" || CompanySpecificColumn.LcscColumnName == "A07" || CompanySpecificColumn.LcscColumnName == "A08" || CompanySpecificColumn.LcscColumnName == "A09" || CompanySpecificColumn.LcscColumnName == "A10")
            //    {
            //        ColList += CompanySpecificColumn.LcscColumnName + " as [" + (string.IsNullOrEmpty(CompanySpecificColumn.LcscLabel) ? CompanySpecificColumn.LcscColumnName : CompanySpecificColumn.LcscLabel) + "],";
            //    }
            //    else if (CompanySpecificColumn.LcscColumnName == "CreatedById")
            //    {
            //        ColList += " dbo.FnGetUserName(Lc"+CompanySpecificColumn.LcscColumnName + ") as [" + (string.IsNullOrEmpty(CompanySpecificColumn.LcscLabel) ? CompanySpecificColumn.LcscColumnName : CompanySpecificColumn.LcscLabel) + "],";
            //    }
            //    else
            //    {
            //        ColList += "Lc"+CompanySpecificColumn.LcscColumnName + " as [" + (string.IsNullOrEmpty(CompanySpecificColumn.LcscLabel) ? CompanySpecificColumn.LcscColumnName : CompanySpecificColumn.LcscLabel) + "],";
            //    }
            //}
            ////changes by RG to add column in claimGrid
            //string Qry = "select * From (select "+ColList+" ROW_NUMBER() OVER (ORDER BY LcCreatedDateTime) as row  from LClaims where LcPayeeId in (SELECT CAST(Item AS INTEGER) FROM dbo.SplitString( '" + PayeeId + "', ',')) and LcCreatedDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' ) a ";
            //var FileName = "ExportClaimsReport_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".zip";
            //Globals.ExportZipFromDataTable(Qry, Company.GcCode, UserName, FileName,null);
            //return Ok(FileName);
            #endregion
            var Company = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
            var CompanySpecificColumnsList = db.Database.SqlQuery<CompanySpecificColumnViewModel>("select LCSC.LcscIsMandatory,Lcsc.LcscIsReportParameter,Lcsc.LcscDataType,Lcsc.LcscReportParameterOrdinal,Lcsc.LcscDropDownId, LCSC.LcscOrdinalPosition, LCSC.LcscColumnName,LCSC.LcscDisplayOnForm,LCSC.Id,LCSC.LcscLabel from LCompanySpecificColumns LCSC where LCSC.LcscTableName='LClaims' and  LCSC.LcscCompanyId={0} and LCSC.LcscDisplayOnForm = 1", CompanyId).ToList();
            for (var i = 0; i < CompanySpecificColumnsList.Count(); i++)
            {
            //    if (CompanySpecificColumnsList.ElementAt(i).ColumnName == "LcMSISDN" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "LcCustomerName" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "LcCustomerName" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "LcIMEI" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "LcOrderNumber" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "A01" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "A02" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "A03" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "A04" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "A05" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "A06" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "A07" || CompanySpecificColumnsList.ElementAt(i).ColumnName == "A10" )
            //    {
            //        CompanySpecificColumnsList[i].LcscColumnName = "CONVERT(nvarchar(max), DecryptByKey(" + CompanySpecificColumnsList.ElementAt(i).ColumnName + ")) as [" + ((string.IsNullOrEmpty(CompanySpecificColumnsList.ElementAt(i).LcscLabel)) ? CompanySpecificColumnsList.ElementAt(i).ColumnName.Replace("X", "") : CompanySpecificColumnsList.ElementAt(i).LcscLabel) + "]";
            //    }
            //    else
            //    {
            //        CompanySpecificColumnsList[i].LcscColumnName = CompanySpecificColumnsList.ElementAt(i).ColumnName + " as [" + ((string.IsNullOrEmpty(CompanySpecificColumnsList.ElementAt(i).LcscLabel)) ? CompanySpecificColumnsList.ElementAt(i).ColumnName.Replace("X", "") : CompanySpecificColumnsList.ElementAt(i).LcscLabel) + "]";
            //    }
            }
            //string ColumnList = String.Join(",", CompanySpecificColumnsList.Select(p => p.LcscColumnName));
            //string Qry = "select " + ColumnList + " from {Schema}.XCalc where XBatchNumber=" + SOSBatchNumber + " and XPeriodCode=" + PeriodCode + FilterQuery;

            string Qry = "exec spGetClaimsReportData '" + PayeeId + "', '" + StartDate + "' , '" + EndDate + "' , '" + CompanyId.ToString() + "' , '" + A01Filter + "' , '" + A01 + "' , '" + A02Filter + "' , '" + A02 + "' , '" + A03Filter + "' , '" + A03 + "' , '" + A04Filter + "' , '" + A04 + "' , '" + A05Filter + "' , '" + A05 + "' , '" + A06Filter + "' , '" + A06 + "' , '" + A07Filter + "' , '" + A07 + "' , '" + A08Filter + "' , '" + A08 + "' , '" + A09Filter + "' , '" + A09 + "' , '" + A10Filter + "' , '" + A10 + "' ,'" + AllocationDateFilter + "' , '" + AllocationDateFrom + "' ,'" + AllocationDateTo + "' ,'" + AlreadyPaidDateFilter + "' ,'" + AlreadyPaidDateFrom + "' ,'" + AlreadyPaidDateTo + "' ,'" + ConnectionDateFilter + "' ,'" + ConnectionDateFrom + "' ,'" + ConnectionDateTo + "' ,'" + LastReclaimDateFilter + "' ,'" + LastReclaimDateFrom + "' ,'" + LastReclaimDateTo + "' ,'" + OrderDateFilter + "' ,'" + OrderDateFrom + "' ,'" + OrderDateTo + "' ,'" + AlreadyPaidAmountFilter + "' ,'" + AlreadyPaidAmountFrom + "' ,'" + AlreadyPaidAmountTo + "' ,'" + ClawbackAmountFilter + "' ,'" + ClawbackAmountFrom + "' ,'" + ClawbackAmountTo + "' ,'" + ExpectedCommissionAmountFilter + "' ,'" + ExpectedCommissionAmountFrom + "' ,'" + ExpectedCommissionAmountTo + "' ,'" + PaymentAmountFilter + "' ,'" + PaymentAmountFrom + "' ,'" + PaymentAmountTo + "' ,'" + BANFilter + "' ,'" + BAN + "' ,'" + CustomerNameFilter + "' ,'" + CustomerName + "' ,'" + IMEIFilter + "' ,'" + IMEI + "' ,'" + MSISDNFilter + "' ,'" + MSISDN + "' ,'" + OrderNumberFilter + "' ,'" + OrderNumber + "' ,'" + PaymentBatchNumberFilter + "' ,'" + PaymentBatchNumber + "' ,'" + ReasonNonAutoPaymentFilter + "' ,'" + ReasonNonAutoPayment + "' ,'" + ClaimBatchNumberFilter + "' ,'" + ClaimBatchNumber + "' ,'" + ClawbackPayeeCodeFilter + "' ,'" + ClawbackPayeeCode + "' ,'" + BrandIds + "' ,'" + CommissionTypeIds + "' ,'" + DeviceTypeIds + "' ,'" + PaymentCommissionTypeIds + "' ,'" + ProductCodeIds + "' ,'" + StatusFilter + "' ,'" + Status + "' ,'" + CreatedByIds + "' ,'" + ActivityTypeIds + "' ,'" + AlreadyPaidDealer + "', '" + RejectionReasonIds + "'";
            var FileName = "ExportClaimsReport_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".zip";
            var FileNamewithoutextention = "ExportClaimsReport_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") ;
           // Globals.ExportZipFromDataTable(Qry, Company.GcCode, UserName, FileName,null);
            DataTable xx = Globals.GetDdatainDataTable(Qry);
            Boolean isFound = false;
            for (int j = 0; j < xx.Columns.Count; j++)
            {
                isFound = false;
                for (var i = 0; i < CompanySpecificColumnsList.Count(); i++)
                {
                    if (CompanySpecificColumnsList.ElementAt(i).LcscColumnName == xx.Columns[j].ToString())
                    {
                        if (CompanySpecificColumnsList.ElementAt(i).LcscLabel != null && CompanySpecificColumnsList.ElementAt(i).LcscLabel != "")
                        {
                           // dt1.Columns.Add(CompanySpecificColumnsList.ElementAt(i).LcscLabel);
                            xx.Columns[j].ColumnName = CompanySpecificColumnsList.ElementAt(i).LcscLabel;
                        }
                        //else
                        //{
                        //    dt1.Columns.Add(CompanySpecificColumnsList.ElementAt(i).LcscColumnName);
                        //}
                        isFound = true;
                        break;
                    }

                    if(xx.Columns[j].ToString() == "Reclaim" || xx.Columns[j].ToString() == "ClaimNumber" || xx.Columns[j].ToString() == "Payeefirstname" || xx.Columns[j].ToString() == "xx.Columns[j].ToString()" || xx.Columns[j].ToString() == "CreatedBy" || xx.Columns[j].ToString() == "CurrentOwner" || xx.Columns[j].ToString() == "CreatedDateTime" || xx.Columns[j].ToString() == "Status")
                    {
                        isFound = true;
                        break;
                    }
                    
                }

                if(isFound == false)
                {
                    xx.Columns.Remove(xx.Columns[j].ToString());
                    j = j - 1;
                }
                
            }

            Globals.CreateExcelFromDataTable(xx, FileName, "", Company.GcCode, UserName, FileNamewithoutextention);
            return Ok(FileName);
        }


        //method added by Ritu Apply all filters and Get Counts of Grid Data and return to webApp
        public IHttpActionResult GetLClaimsForReportCounts(string PayeeId, string StartDate, string EndDate, int CompanyId, string UserName, string Workflow)
        {
            //var StartDateTime = DateTime.ParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //var EndDateTime = DateTime.ParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var userlist = db.AspNetUsers.Where(p => p.GcCompanyId == CompanyId).Select(p => new { p.Id, p.UserName });
            
            string Query = "select count(*) from LClaims where LcPayeeId in (SELECT CAST(Item AS INTEGER) FROM dbo.SplitString('"+PayeeId+"', ',')) and LcCreatedDateTime BETWEEN '"+StartDate+"' AND '"+EndDate+"' ";
            int xx = db.Database.SqlQuery<int>(Query).FirstOrDefault();
            return Ok(xx);
        }

        //method to return columns of lClaims to be displayed in company specific columns
        public IHttpActionResult GetCompanySpecificColumns(string UserName, string Workflow)
        {
            var xx = db.Database.SqlQuery<CompanySpecificColumnViewModel>("select Replace(COLUMN_NAME ,'Lc','') as ColumnName,IS_NULLABLE as IsNullable from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='LClaims'and Column_Name not in ('Id','LcCompanyId','LcCreatedBy','LcUpdatedById','LcStatusId','LcAllocatedToId','LcAllocatedById','LcPaymentBatchId','LcApprovedById','LcSentForApprovalById','LcWithdrawnById','LcRejectedById','LcClaimId','LcCommentsExternal','LcIsDuplicateClaim','LcDuplicateClaimDetails','LcCommentsInternal','LcIsReclaim','LcSentForApprovalDate','LcApprovalDate','LcPaymentDate','LcWithdrawnDate','LcRejectionDate','WFRequesterId','WFAnalystId','WFManagerId','WFOrdinal','WFCurrentOwnerId','WFStatus','WFType','WFRequesterRoleId','WFCompanyId')");
            return Ok(xx);
        }

        //public IHttpActionResult GetLClaimsByStatusNameCompanyId(string StatusName, int CompanyId, string UserName, string Workflow)
        //{
        //    var userlist = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LClaims.Where(p => p.LcStatus == StatusName).Where(p => p.LcCompanyId == CompanyId).Include(p => p.RActivityType).Include(p => p.RBrand).Include(p => p.RCommissionType).Include(p => p.RDeviceType)
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.LcClaimId,
        //                  aa.LcConnectionDate,
        //                  aa.LcCustomerName,
        //                  aa.LcExpectedCommissionAmount,
        //                  aa.LcOrderDate,
        //                  aa.LcPayeeId,
        //                  LcSentForApprovalById = (aa.LcSentForApprovalById != null) ? userlist.Where(p => p.Id == aa.LcSentForApprovalById).FirstOrDefault().UserName : "",
        //                  aa.LcSentForApprovalDate,
        //                  aa.LcStatus,
        //                  LcWithdrawnById = (aa.LcWithdrawnById != null) ? userlist.Where(p => p.Id == aa.LcWithdrawnById).FirstOrDefault().UserName : "",
        //                  aa.LcWithdrawnDate,
        //                  aa.LcPaymentBatchNumber,
        //                  aa.LcPaymentDate,
        //                  aa.LcRejectionDate,
        //                  aa.LcIsReclaim,
        //                  aa.LcLastReclaimDate,
        //                  aa.LcPaymentAmount,
        //                  aa.LcApprovalDate,
        //                  LcApprovedById = (aa.LcApprovedById != null) ? userlist.Where(p => p.Id == aa.LcApprovedById).FirstOrDefault().UserName : "",
        //                  aa.LcClawbackPayeeCode,
        //                  aa.LcClawbackAmount,
        //                  LcAllocatedById = (aa.LcAllocatedById != null) ? userlist.Where(p => p.Id == aa.LcAllocatedById).FirstOrDefault().UserName : "",
        //                  LcAllocatedToId = (aa.LcAllocatedToId != null) ? userlist.Where(p => p.Id == aa.LcAllocatedToId).FirstOrDefault().UserName : "",
        //                  aa.LcAllocationDate,
        //                  aa.LcAlreadyPaidAmount,
        //                  aa.LcAlreadyPaidDate
        //              }).OrderByDescending(p => p.LcIsReclaim);//.OrderByDescending(p=>p.LcOrderDate);

        //    //if (StatusName.Equals("Withdrawn")&&xx.Count()>0)
        //    //{
        //    //    var CompanyId=xx.FirstOrDefault().LcCompanyId;
        //    //    var KeyValue = db.GKeyValues.Where(p => p.GkvCompanyId == CompanyId).Where(p => p.GkvKey == "ClaimHistoryDays").FirstOrDefault();
        //    //    if(KeyValue!=null)
        //    //    {
        //    //        var Double = Convert.ToDouble(KeyValue.GkvValue);
        //    //        var Dates=DateTime.UtcNow.AddDays(-Double);
        //    //       var result = xx.Where(p => p.LcWithdrawnDate.Value <Dates );
        //    //       return Ok(result);
        //    //    }
        //    //}
        //    //if ( StatusName.Equals("Rejected")&&xx.Count()>0)
        //    //{
        //    //    var CompanyId = xx.FirstOrDefault().LcCompanyId;
        //    //    var KeyValue = db.GKeyValues.Where(p => p.GkvCompanyId ==CompanyId).Where(p => p.GkvKey == "ClaimHistoryDays").FirstOrDefault();
        //    //    if (KeyValue != null)
        //    //    {
        //    //        var Double = Convert.ToDouble(KeyValue.GkvValue);
        //    //        var Dates = DateTime.UtcNow.AddDays(-Double);
        //    //        var result = xx.Where(p => p.LcRejectionDate.Value< Dates);
        //    //        return Ok(result);
        //    //    }
        //    //}
        //    return Ok(xx);
        //}

        //[HttpGet]
        //public IHttpActionResult DownloadLClaimsByStatusNameCompanyIdCreatedById(string StatusName, int CompanyId, string CreatedByUserId)
        //{
        //    var userlist = db.AspNetUsers.Where(p=>p.GcCompanyId==CompanyId).Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LClaims.Where(p => p.LcStatus == StatusName).Where(p => p.LcCompanyId == CompanyId).Where(p => p.LcCreatedById == CreatedByUserId).Include(p => p.RActivityType).Include(p => p.RBrand).Include(p => p.RCommissionType).Include(p => p.RDeviceType)
        //              select new
        //              {
        //                  Status=aa.LcStatus,
        //                  aa.A01,
        //                  aa.A02,
        //                  aa.A03,
        //                  aa.A04,
        //                  aa.A05,
        //                  aa.A06,
        //                  aa.A07,
        //                  aa.A08,
        //                  aa.A09,
        //                  aa.A10,
        //                  ConnectionDate=aa.LcConnectionDate,
        //                  CustomerName= aa.LcCustomerName,
        //                  ExpectedCommissionAmount= aa.LcExpectedCommissionAmount,
        //                  OrderDate=aa.LcOrderDate,
        //                  PaymentBatchNumber= aa.LcPaymentBatchNumber,
        //                  PaymentDate= aa.LcPaymentDate,
        //                  LastReclaimDate= aa.LcLastReclaimDate,
        //                  PaymentAmount =aa.LcPaymentAmount,
        //                  ClawbackPayeeCode=aa.LcClawbackPayeeCode,
        //                  ClawbackAmount= aa.LcClawbackAmount,
        //                  AllocationDate=aa.LcAllocationDate,
        //                  AlreadyPaidAmount=aa.LcAlreadyPaidAmount,
        //                  AlreadyPaidDate= aa.LcAlreadyPaidDate,
        //                  Ban=aa.LcBAN,IMEI=aa.LcIMEI,CreatedBy= (aa.LcCreatedById != null) ? userlist.Where(p => p.Id == aa.LcCreatedById).FirstOrDefault().UserName : ""
        //                  ,DeviceTypeId=aa.RDeviceType.RdtName,
        //                  CommissionTypeId=aa.RCommissionType.RctName,
        //                  ActivityTypeId=aa.RActivityType.RatName,
        //                  MSISDN=aa.LcMSISDN
        //              }).OrderByDescending(p => p.CustomerName);//.OrderByDescending(p => p.LcOrderDate);


        //    return Ok(xx);
        //}

        //[HttpGet]
        //public IHttpActionResult DownloadLClaimsByStatusNameCompanyId(string StatusName,int CompanyId)
        //{
        //    var userlist = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LClaims.Where(p => p.LcStatus == StatusName).Where(p => p.LcCompanyId == CompanyId).Include(p => p.RActivityType).Include(p => p.RBrand).Include(p => p.RCommissionType).Include(p => p.RDeviceType)
        //              select new
        //              {
        //                  Ststus = aa.LcStatus,
        //                  aa.A01,
        //                  aa.A02,
        //                  aa.A03,
        //                  aa.A04,
        //                  aa.A05,
        //                  aa.A06,
        //                  aa.A07,
        //                  aa.A08,
        //                  aa.A09,
        //                  aa.A10,
        //                  ConnectionDate = aa.LcConnectionDate,
        //                  CustomerName = aa.LcCustomerName,
        //                  ExpectedCommissionAmount = aa.LcExpectedCommissionAmount,
        //                  OrderDate = aa.LcOrderDate,
        //                  PaymentBatchNumber = aa.LcPaymentBatchNumber,
        //                  PaymentDate = aa.LcPaymentDate,
        //                  LastReclaimDate = aa.LcLastReclaimDate,
        //                  PaymentAmount = aa.LcPaymentAmount,
        //                  ClawbackPayeeCode = aa.LcClawbackPayeeCode,
        //                  ClawbackAmount = aa.LcClawbackAmount,
        //                  AllocationDate = aa.LcAllocationDate,
        //                  AlreadyPaidAmount = aa.LcAlreadyPaidAmount,
        //                  AlreadyPaidDate = aa.LcAlreadyPaidDate,
        //                  Ban = aa.LcBAN,
        //                  IMEI = aa.LcIMEI,
        //                  CreatedBy = (aa.LcCreatedById != null) ? userlist.Where(p => p.Id == aa.LcCreatedById).FirstOrDefault().UserName : ""
        //                  ,
        //                  DeviceTypeId = aa.RDeviceType.RdtName,
        //                  CommissionTypeId = aa.RCommissionType.RctName,
        //                  ActivityTypeId = aa.RActivityType.RatName,
        //                  MSISDN = aa.LcMSISDN
        //              });//.OrderByDescending(p=>p.LcOrderDate);
          
        //    //if (StatusName.Equals("Withdrawn")&&xx.Count()>0)
        //    //{
        //    //    var CompanyId=xx.FirstOrDefault().LcCompanyId;
        //    //    var KeyValue = db.GKeyValues.Where(p => p.GkvCompanyId == CompanyId).Where(p => p.GkvKey == "ClaimHistoryDays").FirstOrDefault();
        //    //    if(KeyValue!=null)
        //    //    {
        //    //        var Double = Convert.ToDouble(KeyValue.GkvValue);
        //    //        var Dates=DateTime.UtcNow.AddDays(-Double);
        //    //       var result = xx.Where(p => p.LcWithdrawnDate.Value <Dates );
        //    //       return Ok(result);
        //    //    }
        //    //}
        //    //if ( StatusName.Equals("Rejected")&&xx.Count()>0)
        //    //{
        //    //    var CompanyId = xx.FirstOrDefault().LcCompanyId;
        //    //    var KeyValue = db.GKeyValues.Where(p => p.GkvCompanyId ==CompanyId).Where(p => p.GkvKey == "ClaimHistoryDays").FirstOrDefault();
        //    //    if (KeyValue != null)
        //    //    {
        //    //        var Double = Convert.ToDouble(KeyValue.GkvValue);
        //    //        var Dates = DateTime.UtcNow.AddDays(-Double);
        //    //        var result = xx.Where(p => p.LcRejectionDate.Value< Dates);
        //    //        return Ok(result);
        //    //    }
        //    //}
        //    return Ok(xx);
        //}

        //public IHttpActionResult GetLClaimsByStatusNameCompanyIdCreatedById(string StatusName, int CompanyId,string CreatedByUserId)
        //{
        //    var userlist = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LClaims.Where(p => p.LcStatus == StatusName).Where(p => p.LcCompanyId == CompanyId).Where(p=>p.LcCreatedById==CreatedByUserId).Include(p => p.RActivityType).Include(p => p.RBrand).Include(p => p.RCommissionType).Include(p => p.RDeviceType)
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.LcClaimId,
        //                  aa.LcConnectionDate,
        //                  aa.LcCustomerName,
        //                  aa.LcExpectedCommissionAmount,
        //                  aa.LcOrderDate,
        //                  aa.LcPayeeId,
        //                  LcSentForApprovalById = (aa.LcSentForApprovalById != null) ? userlist.Where(p => p.Id == aa.LcSentForApprovalById).FirstOrDefault().UserName : "",
        //                  aa.LcSentForApprovalDate,
        //                  aa.LcStatus,
        //                  LcWithdrawnById = (aa.LcWithdrawnById != null) ? userlist.Where(p => p.Id == aa.LcWithdrawnById).FirstOrDefault().UserName : "",
        //                  aa.LcWithdrawnDate,
        //                  aa.LcPaymentBatchNumber,
        //                  aa.LcPaymentDate,
        //                  aa.LcRejectionDate,
        //                  aa.LcIsReclaim,
        //                  aa.LcLastReclaimDate,
        //                  aa.LcPaymentAmount,
        //                  aa.LcApprovalDate,
        //                  LcApprovedById = (aa.LcApprovedById != null) ? userlist.Where(p => p.Id == aa.LcApprovedById).FirstOrDefault().UserName : "",
        //                  aa.LcClawbackPayeeCode,
        //                  aa.LcClawbackAmount,
        //                  LcAllocatedById = (aa.LcAllocatedById != null) ? userlist.Where(p => p.Id == aa.LcAllocatedById).FirstOrDefault().UserName : "",
        //                  LcAllocatedToId = (aa.LcAllocatedToId != null) ? userlist.Where(p => p.Id == aa.LcAllocatedToId).FirstOrDefault().UserName : "",
        //                  aa.LcAllocationDate,
        //                  aa.LcAlreadyPaidAmount,
        //                  aa.LcAlreadyPaidDate
        //              }).OrderByDescending(p => p.LcIsReclaim);//.OrderByDescending(p => p.LcOrderDate);

         
        //    return Ok(xx);
        //}

        //public IHttpActionResult GetLClaimsByStatusCompanyIdPayeeUserIdForGrid(string StatusName, int CompanyId, string CreatedByUserId, string UserName, string Workflow)
        //{
        //    //The above lines are used to get userId of all children of that payee by using userid of payee passed 
        //    var PayeeId = db.LPayees.Where(p => p.LpUserId == CreatedByUserId).FirstOrDefault().Id;
        //    //To Do get payee hierarchy list from sql function function 
        //    var ChildrenPayeeList = db.LPayeeParents.Where(p => p.LppParentPayeeId == PayeeId).Where(p => p.LppEffectiveEndDate == null).Include(p => p.LPayee).Select(p => p.LPayee.LpUserId);

        //    var userlist = db.AspNetUsers.Where(p => p.GcCompanyId == CompanyId).Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LClaims.Where(p => p.LcStatus == StatusName).Where(p => p.LcCompanyId == CompanyId).Where(p => ChildrenPayeeList.Contains(p.LcCreatedById) == true || p.LcCreatedById == CreatedByUserId).Include(p => p.RActivityType).Include(p => p.RBrand).Include(p => p.RCommissionType).Include(p => p.RDeviceType)
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.LcClaimId,
        //                  aa.LcConnectionDate,
        //                  aa.LcOrderDate,
        //                  aa.LcOrderNumber,
        //                  aa.LcPayeeId,
        //                  SentForApprovalBy = (aa.LcSentForApprovalById != null) ? userlist.Where(p => p.Id == aa.LcSentForApprovalById).FirstOrDefault().UserName : "",
        //                  aa.LcSentForApprovalDate,
        //                  aa.LcPaymentDate,
        //                  aa.LcCompanyId,
        //                  aa.LcLastReclaimDate,
        //                  aa.LcPaymentAmount,
        //                  aa.LcApprovalDate,aa.LcIsReclaim,
        //                  ApprovedBy = (aa.LcApprovedById != null) ? userlist.Where(p => p.Id == aa.LcApprovedById).FirstOrDefault().UserName : "",
        //                  AllocatedBy = (aa.LcAllocatedById != null) ? userlist.Where(p => p.Id == aa.LcAllocatedById).FirstOrDefault().UserName : "",
        //                  AllocatedTo = (aa.LcAllocatedToId != null) ? userlist.Where(p => p.Id == aa.LcAllocatedToId).FirstOrDefault().UserName : "",
        //              }).OrderByDescending(p => p.LcIsReclaim);//.OrderByDescending(p => p.LcOrderDate);


        //    return Ok(xx);
        //}

        //This method returns a list of claims entered by that payeecode and its children
        //public IHttpActionResult GetLClaimsByStatusCompanyIdPayeeUserId(string StatusName, int CompanyId, string CreatedByUserId, string UserName, string Workflow)
        //{
        //    //The above lines are used to get userId of all children of that payee by using userid of payee passed 
        //    var PayeeId=db.LPayees.Where(p=>p.LpUserId==CreatedByUserId).FirstOrDefault().Id;
        //    //To Do get payee hierarchy list from sql function function 
        //    var ChildrenPayeeList = db.LPayeeParents.Where(p => p.LppParentPayeeId == PayeeId).Where(p=>p.LppEffectiveEndDate==null).Include(p => p.LPayee).Select(p => p.LPayee.LpUserId);

        //    var userlist = db.AspNetUsers.Where(p=>p.GcCompanyId==CompanyId).Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LClaims.Where(p => p.LcStatus == StatusName).Where(p => p.LcCompanyId == CompanyId).Where(p => ChildrenPayeeList.Contains(p.LcCreatedById)==true||p.LcCreatedById==CreatedByUserId).Include(p => p.RActivityType).Include(p => p.RBrand).Include(p => p.RCommissionType).Include(p => p.RDeviceType)
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.LcClaimId,
        //                  aa.LcConnectionDate,
        //                  aa.LcCustomerName,
        //                  aa.LcExpectedCommissionAmount,
        //                  aa.LcOrderDate,
        //                  aa.LcPayeeId,
        //                  LcSentForApprovalById = (aa.LcSentForApprovalById != null) ? userlist.Where(p => p.Id == aa.LcSentForApprovalById).FirstOrDefault().UserName : "",
        //                  aa.LcSentForApprovalDate,
        //                  aa.LcStatus,
        //                  LcWithdrawnById = (aa.LcWithdrawnById != null) ? userlist.Where(p => p.Id == aa.LcWithdrawnById).FirstOrDefault().UserName : "",
        //                  aa.LcWithdrawnDate,
        //                  aa.LcPaymentBatchNumber,
        //                  aa.LcPaymentDate,
        //                  aa.LcRejectionDate,
        //                  aa.LcIsReclaim,
        //                  aa.LcLastReclaimDate,
        //                  aa.LcPaymentAmount,
        //                  aa.LcApprovalDate,
        //                  LcApprovedById = (aa.LcApprovedById != null) ? userlist.Where(p => p.Id == aa.LcApprovedById).FirstOrDefault().UserName : "",
        //                  aa.LcClawbackPayeeCode,
        //                  aa.LcClawbackAmount,
        //                  LcAllocatedById = (aa.LcAllocatedById != null) ? userlist.Where(p => p.Id == aa.LcAllocatedById).FirstOrDefault().UserName : "",
        //                  LcAllocatedToId = (aa.LcAllocatedToId != null) ? userlist.Where(p => p.Id == aa.LcAllocatedToId).FirstOrDefault().UserName : "",
        //                  aa.LcAllocationDate,
        //                  aa.LcAlreadyPaidAmount,
        //                  aa.LcAlreadyPaidDate
        //              }).OrderByDescending(p => p.LcIsReclaim); ;


        //    return Ok(xx);
        //}

        //This method return list of claims as per status,CompanyId and AllocatedToUserId

        //public IHttpActionResult GetLClaimsByStatusCompanyIdAllocatedToId(string StatusName, int CompanyId,string AllocatedToUserId, string UserName, string Workflow)
        //{
        //    var userlist = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LClaims.Where(p => p.LcStatus == StatusName).Where(p => p.LcCompanyId == CompanyId).Where(p=>p.LcAllocatedToId==AllocatedToUserId).Include(p => p.RActivityType).Include(p => p.RBrand).Include(p => p.RCommissionType).Include(p => p.RDeviceType)
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.LcClaimId,
        //                  aa.LcConnectionDate,
        //                  aa.LcCustomerName,
        //                  aa.LcExpectedCommissionAmount,
        //                  aa.LcOrderDate,
        //                  aa.LcPayeeId,
        //                  LcSentForApprovalById = (aa.LcSentForApprovalById != null) ? userlist.Where(p => p.Id == aa.LcSentForApprovalById).FirstOrDefault().UserName : "",
        //                  aa.LcSentForApprovalDate,
        //                  aa.LcStatus,
        //                  LcWithdrawnById = (aa.LcWithdrawnById != null) ? userlist.Where(p => p.Id == aa.LcWithdrawnById).FirstOrDefault().UserName : "",
        //                  aa.LcWithdrawnDate,
        //                  aa.LcPaymentBatchNumber,
        //                  aa.LcPaymentDate,
        //                  aa.LcRejectionDate,
        //                  aa.LcIsReclaim,
        //                  aa.LcLastReclaimDate,
        //                  aa.LcPaymentAmount,
        //                  aa.LcApprovalDate,
        //                  LcApprovedById = (aa.LcApprovedById != null) ? userlist.Where(p => p.Id == aa.LcApprovedById).FirstOrDefault().UserName : "",
        //                  aa.LcClawbackPayeeCode,
        //                  aa.LcClawbackAmount,
        //                  LcAllocatedById = (aa.LcAllocatedById != null) ? userlist.Where(p => p.Id == aa.LcAllocatedById).FirstOrDefault().UserName : "",
        //                  LcAllocatedToId = (aa.LcAllocatedToId != null) ? userlist.Where(p => p.Id == aa.LcAllocatedToId).FirstOrDefault().UserName : "",
        //                  aa.LcAllocationDate,
        //                  aa.LcAlreadyPaidAmount,
        //                  aa.LcAlreadyPaidDate
        //              }).OrderByDescending(p => p.LcIsReclaim);

           
        //    return Ok(xx);
        //}


        // GET: api/LClaims/5
        [ResponseType(typeof(LClaim))]
        public async Task<IHttpActionResult> GetLClaim(int id, string UserName, string Workflow)
        {
            //var xx = (from aa in db.LClaims.Where(p=>p.Id==id).Include(p => p.RActivityType).Include(p => p.RBrand).Include(p => p.RCommissionType).Include(p => p.RDeviceType)
            //          join bb in db.LPayees on aa.LcPayeeId equals bb.Id
            //          select new
            //          {
            //              aa.WFComments,aa.WFCompanyId,
            //              aa.WFAnalystId,aa.WFCurrentOwnerId,aa.WFManagerId,aa.WFOrdinal,aa.WFRequesterId,aa.WFRequesterRoleId,aa.WFStatus,aa.WFType,
            //              PayeeId=bb.Id,
            //              aa.Id,
            //              aa.LcActivityTypeId,
            //              aa.LcBAN,
            //              aa.LcBrandId,
            //              aa.LcClaimId,
            //              aa.LcCommissionTypeId,
            //              aa.LcConnectionDate,
            //              aa.LcCustomerName,
            //              aa.LcDeviceTypeId,
            //              aa.LcExpectedCommissionAmount,
            //              aa.LcIMEI,
            //              aa.LcMSISDN,
            //              aa.LcOrderDate,
            //              aa.LcOrderNumber,
            //              aa.LcPayeeId,
            //              aa.A01,aa.A02,aa.A03,aa.A04,aa.A05,aa.A06,aa.A07,aa.A08,aa.A09,aa.A10,aa.LcClaimBatchNumber,aa.LcPaymentBatchNumber,
            //              aa.LcProductCodeId,aa.LcRejectionReasonId,aa.LcSentForApprovalById,aa.LcSentForApprovalDate,aa.LcWithdrawnById,aa.LcWithdrawnDate,
            //              aa.RActivityType.RatName,aa.LcPaymentCommissionTypeId,aa.LcPaymentDate,aa.LcReasonNonAutoPayment,aa.LcRejectedById,aa.LcRejectionDate,
            //              aa.RBrand.RbName,aa.LcCompanyId,aa.LcCreatedById,aa.LcIsDuplicateClaim,aa.LcIsReclaim,aa.LcLastReclaimDate,aa.LcPaymentAmount,
            //              aa.RCommissionType.RctName,aa.LcAlreadyPaidDealer,aa.LcApprovalDate,aa.LcApprovedById,aa.LcClawbackPayeeCode,aa.LcClawbackAmount,
            //              aa.RDeviceType.RdtName,aa.LcDuplicateClaimDetails,aa.LcAllocatedById,aa.LcAllocatedToId,aa.LcAllocationDate,aa.LcAlreadyPaidAmount,aa.LcAlreadyPaidDate
            // }).FirstOrDefault();
            //Get the decrypted data from database
            var xx = Globals.ExecuteSPGetFilteredClaimsResults(id, null, null, null, null, null, null, null, null);
            if (xx == null)
            {
                //return NotFound();
                //CLAIM to be displayed could not be found. Send appropriate response to the request.
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "CLAIM")));
            }
            return Ok(xx);
        }

        // PUT: api/LClaims/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLClaim(int id, LClaimDecryptedViewModel LClaim,string AtachedFiles,string AttachmentPath, string UserName, string Workflow)
        {

            if (id != LClaim.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "CLAIM")));
            }
            try
            {
                var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                //db.Entry(LClaim).State = EntityState.Modified;
                //await db.SaveChangesAsync();
                //Call SP to update clams to DB
                Globals.ExecutePutOrPostLClaim(LClaim, "Edit");

                //SG R2.8 Commented the the WFComments from the SP for Edit Only . Becuase now its is handled in c# as greek charcker is converted to ?? here and we are now able to use 'N' infornt of @WfComments here.
                var claimEdit = db.LClaims.Where(x => x.Id == LClaim.Id).FirstOrDefault();
                if(claimEdit != null)
                {
                    claimEdit.WFComments = LClaim.WFComments;
                    db.Entry(claimEdit).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                //Delete old values from MEntityPortfolios R2.6
                //R2.6.1 RK 26022019 Commented below code as there is not need to update portfolios while updating claims
                //var MEntityPortfoliosList = db.MEntityPortfolios.Where(p => p.MepEntityId == LClaim.Id & p.MepEntityType == "LClaims").ToList();
                //foreach (var data in MEntityPortfoliosList)
                //{
                //    db.MEntityPortfolios.Remove(data);
                //    db.SaveChanges();
                //}
                //Addning the new one R2.6
                var PayeeDetails = db.LPayees.Find(LClaim.LcPayeeId);
                var Payee = db.LPayees.Where(p => p.LpCompanyId == LClaim.LcCompanyId).Where(p => p.LpPayeeCode == PayeeDetails.LpPayeeCode).FirstOrDefault();
                if (Payee != null)
                {
                    #region R2.6.1 RK 26022019 Commented below code as there is not need to update portfolios while updating claims
                    //string[] values = LClaim.RChannel.Split(',');
                    //    #region Already commented before R2.6.1
                    //    //foreach (string NewChannelID in values)
                    //    //{
                    //    //    int ChannelID = Convert.ToInt32(NewChannelID);
                    //    //    var Portfolios = db.LPortfolios.Where(p => p.LpChannelId == ChannelID & p.LpCompanyId == LClaim.LcCompanyId).FirstOrDefault();
                    //    //    var MEntityPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityId == LClaim.Id & p.MepEntityType == "LClaims" & p.MepPortfolioId == Portfolios.Id).ToList();
                    //    //    if (MEntityPortfolios.Count == 0)
                    //    //    {
                    //    //        var MEP = new MEntityPortfolio { MepPortfolioId = Portfolios.Id, MepEntityId = LClaim.Id, MepEntityType = "LClaims" };
                    //    //        db.MEntityPortfolios.Add(MEP);
                    //    //        db.SaveChanges();
                    //    //    }
                    //    //}
                    //    #endregion
                    //    foreach (string NewChannelID in values)
                    //{
                    //    int ChannelID = Convert.ToInt32(NewChannelID);
                    //    var PortfoliosLst = db.LPortfolios.Where(p => p.LpChannelId == ChannelID & p.LpCompanyId == LClaim.LcCompanyId).ToList();
                    //    foreach (var Portfolios in PortfoliosLst)
                    //    {
                    //        var checkPayeePortfolioMapping = db.MEntityPortfolios.Where(p => p.MepEntityType == "LPayees" & p.MepPortfolioId == Portfolios.Id & p.MepEntityId == Payee.Id).FirstOrDefault();
                    //        if (checkPayeePortfolioMapping != null)
                    //        {
                    //            var MEntityPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityId == LClaim.Id & p.MepEntityType == "LClaims" & p.MepPortfolioId == Portfolios.Id).ToList();
                    //            if (MEntityPortfolios.Count == 0)
                    //            {
                    //                var MEP = new MEntityPortfolio { MepPortfolioId = Portfolios.Id, MepEntityId = LClaim.Id, MepEntityType = "LClaims" };
                    //                db.MEntityPortfolios.Add(MEP);
                    //                db.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                }
                //add row in LUploaded files if attachments are added while updating claims
                if (!string.IsNullOrEmpty(AtachedFiles)&&AtachedFiles!="null")
                {
                    var FilesArray = AtachedFiles.Split(',').ToList();
                    foreach (var file in FilesArray)
                    {
                        var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = LClaim.LcCreatedById, LsdUpdatedById=LClaim.LcCreatedById, LsdFileName = file, LsdFilePath = AttachmentPath, LsdEntityType = "LClaims", LsdEntityId = LClaim.Id, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedDateTime = DateTime.UtcNow };
                        db.LSupportingDocuments.Add(LSupportingDocuments);
                        db.SaveChanges();
                    }
                }



                //Add Entry in Audit Log
                // R2.7 SG 27012019 Add the below mention code for adding the correct Enityname in LAudit Table
                string WorkFlowBaseTableName = db.RWorkFlows.Where(x => x.RwfName == Workflow).Select(x => x.RwfBaseTableName).FirstOrDefault();
                string name = string.Empty;
                var EntityNameDetails = db.Database.SqlQuery<string>("Exec [dbo].[SPGetIdentifier] {0},{1},{2},{3}", Workflow, LClaim.Id, WorkFlowBaseTableName, name).FirstOrDefault();


                Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Edit",
                       "Update", LClaim.LcCreatedById, DateTime.UtcNow, LClaim.WFStatus, LClaim.WFStatus,
                      WFDetails.RwfBaseTableName, LClaim.Id, Convert.ToString(EntityNameDetails), WFDetails.Id, LClaim.WFCompanyId, string.Empty, LClaim.WFRequesterRoleId,null);
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

        // POST: api/LClaims//
        [ResponseType(typeof(LClaim))]
        public async Task<IHttpActionResult> PostLClaim(LClaimDecryptedViewModel LClaim,string AtachedFiles,string LoggedInRoleId,string AttachmentPath, string UserName, string Workflow)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                //Note: Calculate Ordinal based on the Current Role who has created Manual Adjustment based on RoleId and Opco and WorkflowName
                var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LClaim.LcCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault().LwfcOrdinalNumber;
                var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                var ConfigData = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LClaim.LcCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault();
                try
                {
                    // db.Database.CommandTimeout = 180;//SS setting this to 3 min as this method has a long run query
                    //first check for the duplicate claim and send message to the user
                    /*Duplicate claim code commented. Will be uncommented after tuning . Query taking too long*/
                    // string ActivityType = null;
                    // if (LClaim.LcActivityTypeId.HasValue)
                    // {
                    //   ActivityType  = db.RActivityTypes.Where(p => p.Id == LClaim.LcActivityTypeId).FirstOrDefault().RatName;
                    // }
                    // string CommissionType = null;
                    // if (LClaim.LcCommissionTypeId.HasValue)
                    // {
                    //     CommissionType = db.RCommissionTypes.Where(p => p.Id == LClaim.LcCommissionTypeId).FirstOrDefault().RctName;
                    // }
                    // var PayeeDetails = db.LPayees.Find(LClaim.LcPayeeId);
                    // string ProductCode = null;
                    // if (LClaim.LcProductCodeId.HasValue)
                    // {
                    //     ProductCode = db.RProductCodes.Where(p => p.Id == LClaim.LcProductCodeId).Select(p => p.RpcProductCode).FirstOrDefault();
                    // }
                    // Nullable<DateTime> OrderDate = null;
                    // var ExisitingCalculations =new List<LCalc>();
                    // //var TwoYearsAgoDate = DateTime.UtcNow.AddYears(-2);
                    // //## Condition to check  if Calc is not more than 2 years before has been parked by JS
                    // if (LClaim.LcOrderDate.HasValue)
                    // {
                    //     OrderDate = LClaim.LcOrderDate.Value.Date;
                    //     ExisitingCalculations = db.LCalcs.Where(p => p.LcPayee ==PayeeDetails.LpPayeeCode ).Where(p => p.LcActivityType == ActivityType).Where(p => p.LcCommType == CommissionType).Where(p=>p.LcProductCode==ProductCode).Where(p => p.LcOrderDate==OrderDate).ToList();
                    // }
                    // else
                    // {
                    //     ExisitingCalculations = db.LCalcs.Where(p => p.LcPayee == PayeeDetails.LpPayeeCode).Where(p => p.LcActivityType == ActivityType).Where(p => p.LcCommType == CommissionType).Where(p => p.LcProductCode ==ProductCode).ToList();
                    // }
                    ////check if existing calculation exist with same details
                    // if(ExisitingCalculations.Count()>0&&LClaim.LcIsDuplicateClaim==false)
                    // {
                    //     throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "DuplicateClaim"));
                    // }
                    /*Duplicate claim section ends here*/

                    var PayeeDetails = db.LPayees.Find(LClaim.LcPayeeId);
                    //Selectsequence name to get claimId
                    //var Claim = db.GKeyValues.Where(p => p.GkvCompanyId == LClaim.LcCompanyId).Where(p => p.GkvKey == "ClaimIdSequenceName").FirstOrDefault();
                    //var RawQuery = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo." + Claim.GkvValue);
                    //var Task = RawQuery.SingleAsync();
                    LClaim.LcClaimId = 0;//This will get generated in SQL SP.
                    //Task.Dispose();

                    //if (!ModelState.IsValid) commented by shubham as It was showing validation error but claim got created without giving any validation error
                    //{
                    //    //return BadRequest(ModelState);
                    //    var Errors = this.ModelState.Keys.SelectMany(key => this.ModelState["key"].Errors);
                    //    //Globals.SendEmail("shubhamshrm97@yahoo.com",null, "Vodafone-SOS WebApi", Errors.ToString());
                    //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "CLAIM")));
                    //}
                    LClaim.WFAnalystId = (ConfigData.LwfcActingAs == "Analyst") ? LClaim.WFRequesterId : null;
                    LClaim.WFType = WFDetails.RwfWFType;
                    LClaim.WFStatus = "Saved";
                    LClaim.WFOrdinal = Ordinal;
                    LClaim.LcCreatedByForm = true;//default true in this method as called from app
                                                  //db.LClaims.Add(LClaim);
                                                  //await db.SaveChangesAsync();
                   //Call SP to save clams to DB
                    var Id=Globals.ExecutePutOrPostLClaim(LClaim,"Create");
                    if (Id.HasValue)
                    {
                        //Test By Sachin 
                        LClaim.Id = Id.Value;
                        //Add portfolios to Claims same as associated with the Payee Whose claim has been raised
                        var Payee = db.LPayees.Where(p => p.LpCompanyId == LClaim.LcCompanyId).Where(p => p.LpPayeeCode == PayeeDetails.LpPayeeCode).FirstOrDefault();
                        if (Payee != null)
                        {
                            //var MEntityPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityType.Equals("LPayees", StringComparison.OrdinalIgnoreCase)).Where(p => p.MepEntityId == Payee.Id).ToList();
                            //foreach (var PF in MEntityPortfolios)
                            //{
                            //    var MEP = new MEntityPortfolio { MepPortfolioId = PF.MepPortfolioId, MepEntityId = LClaim.Id, MepEntityType = "LClaims" };
                            //    db.MEntityPortfolios.Add(MEP);
                            //    db.SaveChanges();
                            //}

                            string[] values = LClaim.RChannel.Split(',');
                            //foreach (string NewChannelID in values)
                            //{
                            //    int ChannelID = Convert.ToInt32(NewChannelID);
                            //    var Portfolios = db.LPortfolios.Where(p => p.LpChannelId == ChannelID & p.LpCompanyId == LClaim.LcCompanyId).FirstOrDefault();
                            //    var MEntityPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityId == LClaim.Id & p.MepEntityType == "LClaims" & p.MepPortfolioId == Portfolios.Id).ToList();
                            //    if (MEntityPortfolios.Count == 0) {
                            //        var MEP = new MEntityPortfolio { MepPortfolioId = Portfolios.Id, MepEntityId = LClaim.Id, MepEntityType = "LClaims" };
                            //        db.MEntityPortfolios.Add(MEP);
                            //        db.SaveChanges();
                            //    }
                            //}
                            foreach (string NewChannelID in values)
                           {                                int ChannelID = Convert.ToInt32(NewChannelID);
                                var PortfoliosLst = db.LPortfolios.Where(p => p.LpChannelId == ChannelID & p.LpCompanyId == LClaim.LcCompanyId).ToList();
                                foreach (var Portfolios in PortfoliosLst) {
                                    var checkPayeePortfolioMapping = db.MEntityPortfolios.Where(p => p.MepEntityType == "LPayees" & p.MepPortfolioId == Portfolios.Id & p.MepEntityId == Payee.Id).FirstOrDefault();
                                    if (checkPayeePortfolioMapping != null)
                                    {
                                        var MEntityPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityId == LClaim.Id & p.MepEntityType == "LClaims" & p.MepPortfolioId == Portfolios.Id).ToList();
                                        if (MEntityPortfolios.Count == 0)
                                        {
                                            var MEP = new MEntityPortfolio { MepPortfolioId = Portfolios.Id, MepEntityId = LClaim.Id, MepEntityType = "LClaims" };
                                            db.MEntityPortfolios.Add(MEP);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }

                        //add row in LSupportingDocuments if attachments are added while adding Claim
                        if (!string.IsNullOrEmpty(AtachedFiles) && AtachedFiles != "null")
                        {
                            var FilesArray = AtachedFiles.Split(',').ToList();
                            foreach (var file in FilesArray)
                            {
                                var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = LClaim.LcCreatedById, LsdUpdatedById = LClaim.LcCreatedById, LsdFileName = file, LsdFilePath = AttachmentPath, LsdEntityType = "LClaims", LsdEntityId = LClaim.Id, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedDateTime = DateTime.UtcNow };
                                db.LSupportingDocuments.Add(LSupportingDocuments);
                                db.SaveChanges();
                            }
                        }

                        //Add Entry in Audit Log
                        //Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                        //       "Create", LClaim.LcCreatedById, DateTime.UtcNow, LClaim.WFStatus, LClaim.WFStatus,
                        //      WFDetails.RwfBaseTableName, LClaim.Id, Convert.ToString(LClaim.LcClaimId), WFDetails.Id, LClaim.WFCompanyId, string.Empty, LClaim.WFRequesterRoleId,null);

                        // R2.7 SG 27012019 Add the below mention code for adding the correct Enityname in LAudit Table

                        string WorkFlowBaseTableName = db.RWorkFlows.Where(x => x.RwfName == Workflow).Select(x => x.RwfBaseTableName).FirstOrDefault();
                        string name = string.Empty;
                        var EntityNameDetails = db.Database.SqlQuery<string>("Exec [dbo].[SPGetIdentifier] {0},{1},{2},{3}", Workflow, LClaim.Id, WorkFlowBaseTableName, name).FirstOrDefault();
                        
                        Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                               "Create", LClaim.LcCreatedById, DateTime.UtcNow, LClaim.WFStatus, LClaim.WFStatus,
                              WFDetails.RwfBaseTableName, LClaim.Id, Convert.ToString(EntityNameDetails), WFDetails.Id, LClaim.WFCompanyId, string.Empty, LClaim.WFRequesterRoleId, null);


                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, "Claim could not be added"));//type 2 error
                    }
                }
                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
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
                transaction.Commit();
            }
            return Ok(LClaim.Id);
            //return CreatedAtRoute("DefaultApi", new { id = LClaim.Id }, LClaim);
        }

        // DELETE: api/LClaims/5
        [ResponseType(typeof(LClaim))]
        public async Task<IHttpActionResult> DeleteLClaim(int id, string UserName, string Workflow)
        {
            LClaim LClaim = await db.LClaims.FindAsync(id);
            if (LClaim == null)
            {
                //return NotFound();
                //CITY/POST CODE could not be found. Send appropriate response to the request.
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "CLAIM")));
            }
            try
            {
                db.LClaims.Remove(LClaim);
                await db.SaveChangesAsync();
                return Ok(LClaim);
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

        private bool LClaimExists(int id)
        {
            return db.LClaims.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("SpUpdateClaimsData", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "CLAIM", "DATABASE OBJECTS"));
            else if (SqEx.Message.IndexOf("UQ_LClaims_ClaimId_CompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "CLAIM"));

            else
            {
                //callGlobals.ExecuteSPLogError SP here and log SQL SqEx.Message
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
        }

        //RK Created this method to get all roles assigned for the said user
        //This will accept userid (LuUserID from LUsers) and company id as input parameter
        //and it will return all roles assigned (ID and role Name)
        
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public List<AspnetRoleViewModel> GetUserRoles(string strUserID, int CompanyID)
        {

            var roles = (List<string>)UserManager.GetRolesAsync(strUserID).Result;
            //Get details of opco
            var Company = db.GCompanies.Find(CompanyID);
            //get list of roles(Name and Id ) from Aspnet roles which are assigned to user
            var AspRolesList = (List<AspnetRoleViewModel>)db.AspNetRoles.Where(p => roles.Contains(p.Name)).Where(p => p.CompanyCode == Company.GcCode).Select(p => new AspnetRoleViewModel { Id = p.Id, Name = p.Name }).ToList();

            return AspRolesList;
        }
        [ResponseType(typeof(string))]
        public string GetMyClaimsReportData(string UserID, int CompanyID, string CompanyCode, string LoggedInUserName)
        {
            //columnnames must not be any of following: 'WFComments','CreatedByForm','CreatedById','CreatedDateTime','ParameterCarrier','Status','WFCompanyId')
            //WFStatus is being placed in the file as hard coded
            DataTable tb = new DataTable();
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand("USPGetMyClaimsReportData", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Claims");
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < tb.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = tb.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }

            //loops through data  
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < tb.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = tb.Columns[j].ToString();
                    cell.SetCellValue(tb.Rows[i][columnName].ToString());
                }
            }
            IRow dRow = sheet1.GetRow(0);
            sheet1.RemoveRow(dRow);
            sheet1.ShiftRows(0, sheet1.LastRowNum, -1);
            dRow = sheet1.GetRow(0);
            dRow.ZeroHeight = true;
            //Create OPCO Folder
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode);
            }
            //Create UserName Folder
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            }
            //Delete existing file
            if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName +  "/" + "MyClaimsReport.xlsx"))
            {
                System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/" + "MyClaimsReport.xlsx");
            }
            using (ZipFile zip = new ZipFile())
            {
                FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/", "MyClaimsReport.xlsx"), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                workbook.Write(xfile);
                //tb.TableName = "Tmp";
                //zip.AddEntry("Temp.xml", (name, stream) => tb.WriteXml(stream));
                //zip.AddFile(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/", "MyClaimsReport.xlsx"), "");
                //zip.Save(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/", "MyClaims.zip"));
                //System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/",  "MyClaimsReport.xlsx"));
                xfile.Close();
            }
            //FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/", "MyClaimsReport.xlsx"), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            //workbook.Write(xfile);
            //xfile.Close();
            //Thread.Sleep(1000);
            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            di.Refresh();
            return Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/", "MyClaimsReport.xlsx");
        }
        [ResponseType(typeof(string))]
        public string GetMyClaimsAuditData(string UserID, int CompanyID, string CompanyCode, string LoggedInUserName)
        {
            DataTable tb = new DataTable();
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand("USPGetMyClaimsAuditData", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("ClaimsAudit");
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < tb.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = tb.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }

            //loops through data  
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < tb.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = tb.Columns[j].ToString();
                    cell.SetCellValue(tb.Rows[i][columnName].ToString());
                }
            }
            //IRow dRow = sheet1.GetRow(0);
            //sheet1.RemoveRow(dRow);
            //sheet1.ShiftRows(0, sheet1.LastRowNum, -1);
            //dRow = sheet1.GetRow(0);
            //dRow.ZeroHeight = true;
            //Create OPCO Folder
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode);
            }
            //Create UserName Folder
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            }
            //Delete existing file
            if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/" + "MyClaimsAuditReport.xlsx"))
            {
                System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/" + "MyClaimsAuditReport.xlsx");
            }
            using (ZipFile zip = new ZipFile())
            {
                FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/", "MyClaimsAuditReport.xlsx"), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                workbook.Write(xfile);
                xfile.Close();
            }
            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            di.Refresh();
            return Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/", "MyClaimsAuditReport.xlsx");
        }

        [HttpGet]
        public IHttpActionResult GetPayeesChannels(string PayeeID, int CompanyId)
        {
            var RawQuery = db.Database.SqlQuery<RChannel>("exec SPGetChannelForPayees '" + PayeeID + "','"+ CompanyId + "'");

            var Task = RawQuery.ToList();
            var ListData = Task.ToList();
            return Ok(ListData);

        }

        [HttpGet]
        public IHttpActionResult GetByRChannelByTransactionID(string TransactionID)
        {
            var RawQuery = db.Database.SqlQuery<RChannel>("exec SPGetChannelByTransactionID '" + TransactionID + "'");
            var Task = RawQuery.ToList();
            var ListData = Task.ToList();
            return Ok(ListData);
        }

        [HttpGet]
        public IHttpActionResult GetNextUserDetails(string Workflow, int CompanyID, string LoggedInUserId, string LoggedinRoleID)
        {
            DataTable tb = new DataTable();
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand("USPGetUsersForNextSteps", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@WorkFlowName", Workflow);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyID);
            cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
            cmd.Parameters.AddWithValue("@CurrentRoleId", LoggedinRoleID);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();

            return Ok(tb);
        }



    }
}
