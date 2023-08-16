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
using System.Data.Entity.Validation;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data.SqlClient;
using System.IO.Compression;
using Ionic.Zip;
using NPOI.POIFS.FileSystem;
using System.Collections.Generic;
using System.ComponentModel;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Encryption;
using Amazon.S3.IO;
using ClosedXML.Excel;

namespace Vodafone_SOS_WebApi.Utilities
{

    /***************************************
     * This class is created to serve the purpose of maintaining global objects
     ***************************************/
    public static class Globals
    {
        private static SOSEDMV10Entities db = new SOSEDMV10Entities();
        private static readonly IAuthenticationManager authenticationManager;
        public static string NotFoundErrorMessage { get { return "The {0} you are looking for could not be found. It may already have been deleted."; } }
        public static string BadRequestErrorMessage { get { return "The data passed for {0} the {1} is not valid. Please check the values and try again."; } }
        public static string CanNotUpdateDeleteErrorMessage { get { return "Can not Update/Delete {0} because there are  {1} associated with it."; } }
        public static string CannotInsertDuplicateErrorMessage { get { return "Can not Insert Duplicate {0}."; } }

        /*The below section contains methods required for error handling which were referred from Rely*/
        public static string SomethingElseFailedInDBErrorMessage { get { return "Oops! Something went wrong(DB). The issue has been reported and will be resolved soon."; } }

        public static string SOSAWSAccessKey = GetValue("sos_accesskey");
        public static string SOSAWSSecretKey = GetValue("sos_secretkey");
        public static string A2SS3AccessKey = GetValue("int_accesskey");
        public static string A2SS3SecretKey = GetValue("int_secretkey");

        public static string GetValue(string Key)
        {
            string KeyValue = (from aa in db.GKeyValues.Where(aa => aa.GkvKey == Key)
                               select aa.GkvValue).FirstOrDefault();           
                return KeyValue;
        }

        //new HTTP Status codes in accordance with their types has been defined.
        public enum ExceptionType
        {
            /*An exception/error condition in the application which is unhandled in the system and the system don’t have any predefined mechanism to recover*/
            Type1 = 551,
            /*- A server-side validation failure, such as unique key constraint failure while trying inserting data in a unique column*/
            Type2 = 552,
            /*In some cases, we will need to display a popup message to the user (maybe with some relevant information about the process in which error occurred) and then redirect user to another page*/
            Type3 = 553,
            /*When some validation fails, user can be presented with a popup message describing the failed validation, and then keep user on the same page*/
            Type4 = 554
        };
        private static int DeleteAllKeys(IAmazonS3 client, string bucketName, List<string> allKeys)
        {
            var multiObjectDeleteRequest = new DeleteObjectsRequest();
            multiObjectDeleteRequest.BucketName = bucketName;
            foreach (var key in allKeys)
                multiObjectDeleteRequest.AddKey(key, null); // version ID is null
            try
            {
                var response = client.DeleteObjects(multiObjectDeleteRequest);
                return response.DeletedObjects.Count;
            }
            catch (DeleteObjectsException e)
            {
                // throw exception.
            }
            return 0;
        }
        private static List<string> GetFolderContents(IAmazonS3 client, string bucketName, string folderPath)
        {
            var r = new List<string>();
            try
            {
                ListObjectsRequest request = new ListObjectsRequest
                {
                    BucketName = bucketName,
                    Prefix = folderPath,
                    MaxKeys = 100
                };
                do
                {
                    ListObjectsResponse response = client.ListObjects(request);

                    foreach (S3Object entry in response.S3Objects)
                        r.Add(entry.Key);

                    // If response is truncated, set the marker to get the next 
                    // set of keys.
                    if (response.IsTruncated)
                        request.Marker = response.NextMarker;
                    else
                        request = null;
                } while (request != null);

            }
            catch (Exception ex)
            {
                // throw error
            }
            return r;
        }

        public static void DeleteMultipleFilesFromS3(string path)
        {
            string _awsAccessKey = SOSAWSAccessKey; // ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey; // ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];
            try
            {
                using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
                {
                    var allKeys = GetFolderContents(client, _bucketName, path);
                    allKeys.Remove(path);//we donot want to delete forzip folder
                    if (allKeys.Count() > 0)
                        DeleteAllKeys(client, _bucketName, allKeys);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public static void ExecuteSPLogError(string sourceProject, string controller, string method, string stackTrace, string userName, string errorType, string errorDescription, string resolution, string errorOwner, string fieldName, Nullable<int> sOSBatchNumber, string status)
        {
            var Query = "EXECUTE SPLogError '" + sourceProject + "','" + controller + "','" + method + "','" + stackTrace + "','" + userName + "','" + errorType + "','" + errorDescription + "','','L2',NULL,Null,'New'";
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            conn.Open();
            cmd.ExecuteScalarAsync();
            conn.Close();
        }

        public static void ExecuteSPLogEmail(string recipientList, string cCList, string bCCList, string replyToList, string subject, string body, Nullable<bool> isHTML, string emailType, string priority, string attachmentList, string status, string comments, string createdById, string updatedById, string senderAccountName, string BucketName, string AccessKey, string SecretKey)
        {
            if (string.IsNullOrEmpty(createdById) || string.IsNullOrEmpty(createdById))
            {
                var SystemUser = ConfigurationManager.AppSettings["SystemUser"];
                var SystemUserId = db.AspNetUsers.Where(p => p.Email == SystemUser).Select(p => p.Id).FirstOrDefault();
                createdById = SystemUserId;
                updatedById = SystemUserId;
            }
            var Query = "Exec dbo.SpLogEmail @ReceiverEmail,@cCList,@bCCList,@replyToList,@EmailSubject,@EmailBody,@isHTML,@emailType,@priority,@attachmentList,@status,@Comments,@createdById,@updatedById,@senderAccountName,@BucketName,@AccessKey,@SecretKey";
            //string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query);//conn
            cmd.Parameters.AddWithValue("@ReceiverEmail", recipientList);
            cmd.Parameters.AddWithValue("@cCList", string.IsNullOrEmpty(cCList) ? (object)System.DBNull.Value : (object)cCList);
            cmd.Parameters.AddWithValue("@bCCList", string.IsNullOrEmpty(bCCList) ? (object)System.DBNull.Value : (object)bCCList);
            cmd.Parameters.AddWithValue("@replyToList", string.IsNullOrEmpty(replyToList) ? (object)System.DBNull.Value : (object)replyToList);
            cmd.Parameters.AddWithValue("@EmailSubject", subject);
            cmd.Parameters.AddWithValue("@EmailBody", body);
            cmd.Parameters.AddWithValue("@isHTML", (isHTML == true) ? 1 : 0);
            cmd.Parameters.AddWithValue("@emailType", emailType);
            cmd.Parameters.AddWithValue("@priority", priority);
            cmd.Parameters.AddWithValue("@attachmentList", string.IsNullOrEmpty(attachmentList) ? (object)System.DBNull.Value : (object)attachmentList);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@Comments", string.IsNullOrEmpty(comments) ? (object)System.DBNull.Value : (object)comments);
            cmd.Parameters.AddWithValue("@createdById", string.IsNullOrEmpty(createdById) ? (object)System.DBNull.Value : (object)createdById);
            cmd.Parameters.AddWithValue("@updatedById", string.IsNullOrEmpty(updatedById) ? (object)System.DBNull.Value : (object)updatedById);
            cmd.Parameters.AddWithValue("@senderAccountName", senderAccountName);
            //Remove This default Bucket,AccessKey and Secret kety with DBNull when these columns are made nullable
            cmd.Parameters.AddWithValue("@BucketName", string.IsNullOrEmpty(BucketName) ? (object)"SOSS3Bucketname" : (object)BucketName);
            cmd.Parameters.AddWithValue("@AccessKey", string.IsNullOrEmpty(AccessKey) ? (object)"SOSS3AccessKey" : (object)AccessKey);
            cmd.Parameters.AddWithValue("@SecretKey", string.IsNullOrEmpty(SecretKey) ? (object)"SOSS3SecretKey" : (object)SecretKey);
            //conn.Open();
            //cmd.ExecuteScalarAsync();
            //conn.Close();
            DataSet dt = GetData(cmd);
        }
        public static string ExportDataSetToExcel(DataSet ds, string Path, string Filename, string ContentType, string dateformat)
        {
            try
            {
                string CompletePath = Path + "/" + Filename;
                ContentType = (ContentType.Replace(" ", "")).ToLower();
                string OutputMsg = "";

                if (string.IsNullOrEmpty(Path))
                {
                    OutputMsg = "Path not defined";
                    return OutputMsg;
                }
                else
                {
                    //check the existence of path, if do not exist then create it
                    if (!Directory.Exists(Path))
                    {
                        Directory.CreateDirectory(Path);
                    }
                }
                if (string.IsNullOrEmpty(Filename))
                {
                    OutputMsg = "Filename is Mandatory";
                    return OutputMsg;
                }
                else
                {

                    IWorkbook workbook = new XSSFWorkbook();

                    ICellStyle _TextCellStyle = workbook.CreateCellStyle();
                    _TextCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("@");

                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        ISheet sheet1 = null;
                        sheet1 = workbook.CreateSheet(ds.Tables[k].TableName);
                        //Creating first row for Columns
                        IRow row1 = sheet1.CreateRow(0);
                        DataTable dt = ds.Tables[k];
                        //Setting Column names in first row of excel
                        for (int j = 0; j < ds.Tables[k].Columns.Count; j++)
                        {
                            ICell cell = row1.CreateCell(j);
                            //cell.CellStyle = _TextCellStyle;
                            string columnName = dt.Columns[j].ToString();
                            cell.SetCellValue(columnName);

                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var value = "";
                            IRow row = sheet1.CreateRow(i + 1);
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                ICell cell = row.CreateCell(j);
                                value = dt.Rows[i][j].ToString();
                                //getting datatype for datacolumn. If it is datatime, convert its value to contain only date in dd.mm.yyyy format.
                                var type = dt.Columns[j].DataType.Name.ToString();
                                if (type == "DateTime")
                                {
                                    if (!String.IsNullOrEmpty(value))
                                        value = DateTime.Parse(value).ToString(dateformat);//parameter should passed
                                }
                                cell.SetCellValue(value);
                            }
                        }
                    }
                    FileStream xfile = new FileStream(CompletePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);

                    workbook.Write(xfile);
                    xfile.Close();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public static string ExecuteSPUpdateActionStatus(string ActionName, string WorkFlowName, string TransactionId, int CompanyId, string LoggedInUserId, string Comments, string CurrentRoleId, string ProjectEnviournment, string AssigneUserId)
        {
            var Query = "Exec dbo.SPUpdateActionStatus @Action,@WorkflowName,@TransactionId,@CompanyId,@LoggedInUserId,@Comments,@CurrentRoleId,@ProjectEnviournment,@AssigneUserId";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@Action", string.IsNullOrEmpty(ActionName) ? (object)System.DBNull.Value : (object)ActionName);
            cmd.Parameters.AddWithValue("@WorkflowName", WorkFlowName);
            cmd.Parameters.AddWithValue("@TransactionId", TransactionId);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
            cmd.Parameters.AddWithValue("@Comments", string.IsNullOrEmpty(Comments) ? (object)System.DBNull.Value : (object)Comments);
            cmd.Parameters.AddWithValue("@CurrentRoleId", CurrentRoleId);
            cmd.Parameters.AddWithValue("@ProjectEnviournment", ProjectEnviournment);
            cmd.Parameters.AddWithValue("@AssigneUserId", string.IsNullOrEmpty(AssigneUserId) ? (object)System.DBNull.Value : (object)AssigneUserId);
            DataSet ds = GetData(cmd);
            //if (ds.Tables.Count == 0)
            //    return null;
            //else if (ds.Tables.Count < 2)//This condition checks if DataSet has more than one row which is used for validation messages
            //    return null;
            //check for validation message in all datatables
            string Validationmessage = null;
            foreach (DataTable dt in ds.Tables)
            {
                string result = Convert.ToString(dt.Rows[0][0]);
                if (result.Contains("Validation Message"))
                {
                    Validationmessage = result;
                    break;
                }
            }
            return Validationmessage;
        }

        public static DataTable ExecuteSPGetFilteredClaimsResults(int? Id, DateTime? OrderDate, int? CompanyId, decimal? ExpectedCommissionAmount, string WFStatus, int? PayeeId, string MSISDN, DateTime? CreatedDateTime, int? WFOrdinal)
        {
            var Query = "Exec [dbo].[SPGetFilteredClaimsResults] @Id,@OrderDate,@CompanyId,@ExpectedCommissionAmount,@WFStatus,@PayeeId,@MSISDN,@CreatedDateTime,@WFOrdinal";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@Id", (!Id.HasValue) ? (object)System.DBNull.Value : (object)Id);
            cmd.Parameters.AddWithValue("@OrderDate", !OrderDate.HasValue ? (object)System.DBNull.Value : (object)OrderDate);
            cmd.Parameters.AddWithValue("@CompanyId", !CompanyId.HasValue ? (object)System.DBNull.Value : (object)CompanyId);
            cmd.Parameters.AddWithValue("@ExpectedCommissionAmount", !CompanyId.HasValue ? (object)System.DBNull.Value : (object)CompanyId);
            cmd.Parameters.AddWithValue("@LoggedInUserId", !CompanyId.HasValue ? (object)System.DBNull.Value : (object)CompanyId);
            cmd.Parameters.AddWithValue("@WFStatus", string.IsNullOrEmpty(WFStatus) ? (object)System.DBNull.Value : (object)WFStatus);
            cmd.Parameters.AddWithValue("@PayeeId", !PayeeId.HasValue ? (object)System.DBNull.Value : (object)PayeeId);
            cmd.Parameters.AddWithValue("@CreatedDateTime", !CreatedDateTime.HasValue ? (object)System.DBNull.Value : (object)CreatedDateTime);
            cmd.Parameters.AddWithValue("@WFOrdinal", !WFOrdinal.HasValue ? (object)System.DBNull.Value : (object)WFOrdinal);
            cmd.Parameters.AddWithValue("@MSISDN", string.IsNullOrEmpty(MSISDN) ? (object)System.DBNull.Value : (object)MSISDN);
            DataSet ds = GetData(cmd);
            if (ds.Tables.Count == 0)
                return null;
            return ds.Tables[0];
        }

        public static int? ExecutePostLPayee(LPayeeDecryptedViewModel LPayee)
        {

            //Section to get Payee Data from Stored Procedure starts Here
            var Query = "Exec [dbo].[USPPostLPayee]  @LpUserId,@LpCompanyId,@LpChannelId,@LpSubChannelId,@LpCreatedById,@LpUpdatedById,@LpPrimaryChannel,@LpPayeeCode,@LpEmail,@LpEffectiveStartDate,@LpEffectiveEndDate,@LpFileNames,@LpUserFriendlyFileNames,@LpBusinessUnit,@LpCanRaiseClaims,@LpChannelManager,@LpCreatedDateTime,@LpUpdatedDateTime,@LpTIN,@LpDistributionChannel,@LpPosition,@LpBatchNumber,@LpCreateLogin,@WFRequesterId,@WFAnalystId,@WFOrdinal,@WFManagerId,@WFCurrentOwnerId,@WFStatus,@WFType,@WFRequesterRoleId,@WFCompanyId,@WFComments,@LpFinOpsRoles,@ParameterCarrier,@LpCreatedByForm,@LpFirstName,@LpLastName,@LpPhone,@LpAddress,@LpTradingName,@A01,@A02,@A03,@A04,@A05,@A06,@A07,@A08,@A09,@A10,@LpBlockNotification,@Id,@Action";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@LpUserId", string.IsNullOrEmpty(LPayee.LpUserId) ? (object)System.DBNull.Value : (object)LPayee.LpUserId);
            cmd.Parameters.AddWithValue("@LpCompanyId", LPayee.LpCompanyId == 0 ? (object)System.DBNull.Value : (object)LPayee.LpCompanyId);
            cmd.Parameters.AddWithValue("@LpChannelId", (!LPayee.LpChannelId.HasValue) ? (object)System.DBNull.Value : (object)LPayee.LpChannelId.Value);
            cmd.Parameters.AddWithValue("@LpSubChannelId", (!LPayee.LpSubChannelId.HasValue) ? (object)System.DBNull.Value : (object)LPayee.LpSubChannelId.Value);
            cmd.Parameters.AddWithValue("@LpCreatedById", string.IsNullOrEmpty(LPayee.LpCreatedById) ? (object)System.DBNull.Value : (object)LPayee.LpCreatedById);
            cmd.Parameters.AddWithValue("@LpUpdatedById", string.IsNullOrEmpty(LPayee.LpUpdatedById) ? (object)System.DBNull.Value : (object)LPayee.LpUpdatedById);
            cmd.Parameters.AddWithValue("@LpPrimaryChannel", string.IsNullOrEmpty(LPayee.LpPrimaryChannel) ? (object)System.DBNull.Value : (object)LPayee.LpPrimaryChannel);
            cmd.Parameters.AddWithValue("@LpPayeeCode", string.IsNullOrEmpty(LPayee.LpPayeeCode) ? (object)System.DBNull.Value : (object)LPayee.LpPayeeCode);
            cmd.Parameters.AddWithValue("@LpEmail", string.IsNullOrEmpty(LPayee.LpEmail) ? (object)System.DBNull.Value : (object)LPayee.LpEmail);
            cmd.Parameters.AddWithValue("@LpEffectiveStartDate", (LPayee.LpEffectiveStartDate == null) ? (object)System.DBNull.Value : (object)LPayee.LpEffectiveStartDate.AddHours(6));//To handle DB Offset 6 hours added
            cmd.Parameters.AddWithValue("@LpEffectiveEndDate", (LPayee.LpEffectiveEndDate == null) ? (object)System.DBNull.Value : (object)LPayee.LpEffectiveEndDate);
            cmd.Parameters.AddWithValue("@LpFileNames", string.IsNullOrEmpty(LPayee.LpFileNames) ? (object)System.DBNull.Value : (object)LPayee.LpFileNames);
            cmd.Parameters.AddWithValue("@LpUserFriendlyFileNames", string.IsNullOrEmpty(LPayee.LpUserFriendlyFileNames) ? (object)System.DBNull.Value : (object)LPayee.LpUserFriendlyFileNames);
            cmd.Parameters.AddWithValue("@LpBusinessUnit", string.IsNullOrEmpty(LPayee.LpBusinessUnit) ? (object)System.DBNull.Value : (object)LPayee.LpBusinessUnit);
            cmd.Parameters.AddWithValue("@LpCanRaiseClaims", LPayee.LpCanRaiseClaims);
            cmd.Parameters.AddWithValue("@LpChannelManager", string.IsNullOrEmpty(LPayee.LpChannelManager) ? (object)System.DBNull.Value : (object)LPayee.LpChannelManager);
            cmd.Parameters.AddWithValue("@LpCreatedDateTime", (LPayee.LpCreatedDateTime == null) ? (object)System.DBNull.Value : (object)LPayee.LpCreatedDateTime);
            cmd.Parameters.AddWithValue("@LpUpdatedDateTime", (LPayee.LpUpdatedDateTime == null) ? (object)System.DBNull.Value : (object)LPayee.LpUpdatedDateTime);
            cmd.Parameters.AddWithValue("@LpTIN", string.IsNullOrEmpty(LPayee.LpTIN) ? (object)System.DBNull.Value : (object)LPayee.LpTIN);
            cmd.Parameters.AddWithValue("@LpDistributionChannel", string.IsNullOrEmpty(LPayee.LpDistributionChannel) ? (object)System.DBNull.Value : (object)LPayee.LpDistributionChannel);
            cmd.Parameters.AddWithValue("@LpPosition", string.IsNullOrEmpty(LPayee.LpPosition) ? (object)System.DBNull.Value : (object)LPayee.LpPosition);
            cmd.Parameters.AddWithValue("@LpBatchNumber", (LPayee.LpBatchNumber == null) ? (object)System.DBNull.Value : (object)LPayee.LpBatchNumber);
            cmd.Parameters.AddWithValue("@LpCreateLogin", LPayee.LpCreateLogin);
            cmd.Parameters.AddWithValue("@WFRequesterId", string.IsNullOrEmpty(LPayee.WFRequesterId) ? (object)System.DBNull.Value : (object)LPayee.WFRequesterId);
            cmd.Parameters.AddWithValue("@WFAnalystId", string.IsNullOrEmpty(LPayee.WFAnalystId) ? (object)System.DBNull.Value : (object)LPayee.WFAnalystId);
            cmd.Parameters.AddWithValue("@WFOrdinal", LPayee.WFOrdinal);
            cmd.Parameters.AddWithValue("@WFManagerId", string.IsNullOrEmpty(LPayee.WFManagerId) ? (object)System.DBNull.Value : (object)LPayee.WFManagerId);
            cmd.Parameters.AddWithValue("@WFCurrentOwnerId", string.IsNullOrEmpty(LPayee.WFCurrentOwnerId) ? (object)System.DBNull.Value : (object)LPayee.WFCurrentOwnerId);
            cmd.Parameters.AddWithValue("@WFStatus", string.IsNullOrEmpty(LPayee.WFStatus) ? (object)System.DBNull.Value : (object)LPayee.WFStatus);
            cmd.Parameters.AddWithValue("@WFType", string.IsNullOrEmpty(LPayee.WFType) ? (object)System.DBNull.Value : (object)LPayee.WFType);
            cmd.Parameters.AddWithValue("@WFRequesterRoleId", string.IsNullOrEmpty(LPayee.WFRequesterRoleId) ? (object)System.DBNull.Value : (object)LPayee.WFRequesterRoleId);
            cmd.Parameters.AddWithValue("@WFCompanyId", LPayee.WFCompanyId);
            cmd.Parameters.AddWithValue("@WFComments", string.IsNullOrEmpty(LPayee.WFComments) ? (object)System.DBNull.Value : (object)LPayee.WFComments);
            cmd.Parameters.AddWithValue("@LpFinOpsRoles", string.IsNullOrEmpty(LPayee.LpFinOpsRoles) ? (object)System.DBNull.Value : (object)LPayee.LpFinOpsRoles);
            cmd.Parameters.AddWithValue("@ParameterCarrier", string.IsNullOrEmpty(LPayee.ParameterCarrier) ? (object)System.DBNull.Value : (object)LPayee.ParameterCarrier);
            cmd.Parameters.AddWithValue("@LpCreatedByForm", LPayee.LpCreatedByForm);
            cmd.Parameters.AddWithValue("@LpFirstName", string.IsNullOrEmpty(LPayee.LpFirstName) ? (object)System.DBNull.Value : (object)LPayee.LpFirstName);
            cmd.Parameters.AddWithValue("@LpLastName", string.IsNullOrEmpty(LPayee.LpLastName) ? (object)System.DBNull.Value : (object)LPayee.LpLastName);
            cmd.Parameters.AddWithValue("@LpPhone", string.IsNullOrEmpty(LPayee.LpPhone) ? (object)System.DBNull.Value : (object)LPayee.LpPhone);
            cmd.Parameters.AddWithValue("@LpAddress", string.IsNullOrEmpty(LPayee.LpAddress) ? (object)System.DBNull.Value : (object)LPayee.LpAddress);
            cmd.Parameters.AddWithValue("@LpTradingName", string.IsNullOrEmpty(LPayee.LpTradingName) ? (object)System.DBNull.Value : (object)LPayee.LpTradingName);
            cmd.Parameters.AddWithValue("@A01", string.IsNullOrEmpty(LPayee.A01) ? (object)System.DBNull.Value : (object)LPayee.A01);
            cmd.Parameters.AddWithValue("@A02", string.IsNullOrEmpty(LPayee.A02) ? (object)System.DBNull.Value : (object)LPayee.A02);
            cmd.Parameters.AddWithValue("@A03", string.IsNullOrEmpty(LPayee.A03) ? (object)System.DBNull.Value : (object)LPayee.A03);
            cmd.Parameters.AddWithValue("@A04", string.IsNullOrEmpty(LPayee.A04) ? (object)System.DBNull.Value : (object)LPayee.A04);
            cmd.Parameters.AddWithValue("@A05", string.IsNullOrEmpty(LPayee.A05) ? (object)System.DBNull.Value : (object)LPayee.A05);
            cmd.Parameters.AddWithValue("@A06", string.IsNullOrEmpty(LPayee.A06) ? (object)System.DBNull.Value : (object)LPayee.A06);
            cmd.Parameters.AddWithValue("@A07", string.IsNullOrEmpty(LPayee.A07) ? (object)System.DBNull.Value : (object)LPayee.A07);
            cmd.Parameters.AddWithValue("@A08", string.IsNullOrEmpty(LPayee.A08) ? (object)System.DBNull.Value : (object)LPayee.A08);
            cmd.Parameters.AddWithValue("@A09", string.IsNullOrEmpty(LPayee.A09) ? (object)System.DBNull.Value : (object)LPayee.A09);
            cmd.Parameters.AddWithValue("@A10", string.IsNullOrEmpty(LPayee.A10) ? (object)System.DBNull.Value : (object)LPayee.A10);
            cmd.Parameters.AddWithValue("@LpBlockNotification", LPayee.LpBlockNotification);
            cmd.Parameters.AddWithValue("@Id", LPayee.Id);
            cmd.Parameters.AddWithValue("@Action", "Create");
            DataSet ds = GetData(cmd);
            if (ds.Tables.Count == 0)
                return null;
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        public static int? ExecutePutOrPostLClaim(LClaimDecryptedViewModel LClaim, string Action)
        {

            //Section to get Payee Data from Stored Procedure starts Here
            var Query = "Exec [dbo].[USPPostLClaims]  @LcCompanyId,@LcCreatedById,@LcBrandId,@LcActivityTypeId,@LcCommissionTypeId,@LcAllocatedToId,@LcAllocatedById,@LcRejectionReasonId,@LcPaymentCommissionTypeId,@LcApprovedById,@LcSentForApprovalById,@LcWithdrawnById,@LcRejectedById,@LcDeviceTypeId,@LcClaimId,@LcConnectionDate,@LcOrderDate,@LcProductCodeId,@LcExpectedCommissionAmount,@LcIsDuplicateClaim,@LcDuplicateClaimDetails,@LcAllocationDate,@LcIsReclaim,@LcAlreadyPaidAmount,@LcAlreadyPaidDate ,@LcAlreadyPaidDealer,@LcReasonNonAutoPayment,@LcClawbackPayeeCode,@LcClawbackAmount,@LcSentForApprovalDate,@LcApprovalDate,@LcPaymentDate,@LcLastReclaimDate,@LcWithdrawnDate,@LcRejectionDate,@LcPaymentAmount,@LcPaymentBatchNumber,@LcClaimBatchNumber ,@LcCreatedDateTime,@WFRequesterId,@WFAnalystId,@WFOrdinal,@WFManagerId,@WFCurrentOwnerId,@WFStatus,@WFType,@WFRequesterRoleId,@WFCompanyId,@WFComments,@ParameterCarrier,@LcCreatedByForm,@LcMSISDN,@LcCustomerName,@LcBAN,@LcIMEI,@LcOrderNumber,@A01,@A02,@A03,@A04,@A05,@A06,@A07,@A08,@A09,@A10,@Id,@LcPayeeId,@Action,@LcCommissionPeriod,@LcParentPayeeId";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@LcCompanyId", (LClaim.LcCompanyId));
            cmd.Parameters.AddWithValue("@LcCreatedById", string.IsNullOrEmpty(LClaim.LcCreatedById) ? (object)System.DBNull.Value : (object)LClaim.LcCreatedById);
            cmd.Parameters.AddWithValue("@LcBrandId", (!LClaim.LcBrandId.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcBrandId.Value);
            cmd.Parameters.AddWithValue("@LcActivityTypeId", (!LClaim.LcActivityTypeId.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcActivityTypeId.Value);
            cmd.Parameters.AddWithValue("@LcCommissionTypeId", (!LClaim.LcCommissionTypeId.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcCommissionTypeId);
            cmd.Parameters.AddWithValue("@LcAllocatedToId", string.IsNullOrEmpty(LClaim.LcAllocatedToId) ? (object)System.DBNull.Value : (object)LClaim.LcAllocatedToId);
            cmd.Parameters.AddWithValue("@LcAllocatedById", string.IsNullOrEmpty(LClaim.LcAllocatedById) ? (object)System.DBNull.Value : (object)LClaim.LcAllocatedById);
            cmd.Parameters.AddWithValue("@LcRejectionReasonId", (!LClaim.LcRejectionReasonId.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcRejectionReasonId);
            cmd.Parameters.AddWithValue("@LcPaymentCommissionTypeId", (!LClaim.LcPaymentCommissionTypeId.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcPaymentCommissionTypeId);
            cmd.Parameters.AddWithValue("@LcApprovedById", string.IsNullOrEmpty(LClaim.LcApprovedById) ? (object)System.DBNull.Value : (object)LClaim.LcApprovedById);
            cmd.Parameters.AddWithValue("@LcSentForApprovalById", string.IsNullOrEmpty(LClaim.LcSentForApprovalById) ? (object)System.DBNull.Value : (object)LClaim.LcSentForApprovalById);
            cmd.Parameters.AddWithValue("@LcWithdrawnById", string.IsNullOrEmpty(LClaim.LcWithdrawnById) ? (object)System.DBNull.Value : (object)LClaim.LcWithdrawnById);
            cmd.Parameters.AddWithValue("@LcRejectedById", string.IsNullOrEmpty(LClaim.LcRejectedById) ? (object)System.DBNull.Value : (object)LClaim.LcRejectedById);
            cmd.Parameters.AddWithValue("@LcDeviceTypeId", (!LClaim.LcDeviceTypeId.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcDeviceTypeId);
            cmd.Parameters.AddWithValue("@LcClaimId", LClaim.LcClaimId);
            cmd.Parameters.AddWithValue("@LcConnectionDate", (LClaim.LcConnectionDate == null) ? (object)System.DBNull.Value : (object)LClaim.LcConnectionDate);
            cmd.Parameters.AddWithValue("@LcOrderDate", (LClaim.LcOrderDate == null) ? (object)System.DBNull.Value : (object)LClaim.LcOrderDate);
            cmd.Parameters.AddWithValue("@LcProductCodeId", (!LClaim.LcProductCodeId.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcProductCodeId);
            cmd.Parameters.AddWithValue("@LcExpectedCommissionAmount", LClaim.LcExpectedCommissionAmount);
            cmd.Parameters.AddWithValue("@LcIsDuplicateClaim", (LClaim.LcIsDuplicateClaim));
            cmd.Parameters.AddWithValue("@LcDuplicateClaimDetails", string.IsNullOrEmpty(LClaim.LcDuplicateClaimDetails) ? (object)System.DBNull.Value : (object)LClaim.LcDuplicateClaimDetails);
            cmd.Parameters.AddWithValue("@LcAllocationDate", (LClaim.LcAllocationDate == null) ? (object)System.DBNull.Value : (object)LClaim.LcAllocationDate);
            cmd.Parameters.AddWithValue("@LcIsReclaim", LClaim.LcIsReclaim);
            cmd.Parameters.AddWithValue("@LcAlreadyPaidAmount", (!LClaim.LcAlreadyPaidAmount.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcAlreadyPaidAmount);
            cmd.Parameters.AddWithValue("@WFRequesterId", string.IsNullOrEmpty(LClaim.WFRequesterId) ? (object)System.DBNull.Value : (object)LClaim.WFRequesterId);
            cmd.Parameters.AddWithValue("@WFAnalystId", string.IsNullOrEmpty(LClaim.WFAnalystId) ? (object)System.DBNull.Value : (object)LClaim.WFAnalystId);
            cmd.Parameters.AddWithValue("@WFOrdinal", LClaim.WFOrdinal);
            cmd.Parameters.AddWithValue("@WFManagerId", string.IsNullOrEmpty(LClaim.WFManagerId) ? (object)System.DBNull.Value : (object)LClaim.WFManagerId);
            cmd.Parameters.AddWithValue("@WFCurrentOwnerId", string.IsNullOrEmpty(LClaim.WFCurrentOwnerId) ? (object)System.DBNull.Value : (object)LClaim.WFCurrentOwnerId);
            cmd.Parameters.AddWithValue("@WFStatus", string.IsNullOrEmpty(LClaim.WFStatus) ? (object)System.DBNull.Value : (object)LClaim.WFStatus);
            cmd.Parameters.AddWithValue("@WFType", string.IsNullOrEmpty(LClaim.WFType) ? (object)System.DBNull.Value : (object)LClaim.WFType);
            cmd.Parameters.AddWithValue("@WFRequesterRoleId", string.IsNullOrEmpty(LClaim.WFRequesterRoleId) ? (object)System.DBNull.Value : (object)LClaim.WFRequesterRoleId);
            cmd.Parameters.AddWithValue("@WFCompanyId", LClaim.WFCompanyId);
            cmd.Parameters.AddWithValue("@WFComments", string.IsNullOrEmpty(LClaim.WFComments) ? (object)System.DBNull.Value : (object)LClaim.WFComments);
            cmd.Parameters.AddWithValue("@LcAlreadyPaidDate", (LClaim.LcAlreadyPaidDate == null) ? (object)System.DBNull.Value : (object)LClaim.LcAlreadyPaidDate);
            cmd.Parameters.AddWithValue("@ParameterCarrier", string.IsNullOrEmpty(LClaim.ParameterCarrier) ? (object)System.DBNull.Value : (object)LClaim.ParameterCarrier);
            cmd.Parameters.AddWithValue("@LcCreatedByForm", LClaim.LcCreatedByForm);
            cmd.Parameters.AddWithValue("@LcAlreadyPaidDealer", string.IsNullOrEmpty(LClaim.LcAlreadyPaidDealer) ? (object)System.DBNull.Value : (object)LClaim.LcAlreadyPaidDealer);
            cmd.Parameters.AddWithValue("@LcReasonNonAutoPayment", string.IsNullOrEmpty(LClaim.LcReasonNonAutoPayment) ? (object)System.DBNull.Value : (object)LClaim.LcReasonNonAutoPayment);
            cmd.Parameters.AddWithValue("@LcClawbackPayeeCode", string.IsNullOrEmpty(LClaim.LcClawbackPayeeCode) ? (object)System.DBNull.Value : (object)LClaim.LcClawbackPayeeCode);
            cmd.Parameters.AddWithValue("@LcClawbackAmount", (!LClaim.LcClawbackAmount.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcClawbackAmount);
            cmd.Parameters.AddWithValue("@LcSentForApprovalDate", (!LClaim.LcSentForApprovalDate.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcSentForApprovalDate);
            cmd.Parameters.AddWithValue("@LcApprovalDate", (!LClaim.LcApprovalDate.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcApprovalDate.Value);
            cmd.Parameters.AddWithValue("@LcPaymentDate", (!LClaim.LcPaymentDate.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcPaymentDate.Value);
            cmd.Parameters.AddWithValue("@LcLastReclaimDate", (!LClaim.LcLastReclaimDate.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcLastReclaimDate);
            cmd.Parameters.AddWithValue("@LcWithdrawnDate", (!LClaim.LcWithdrawnDate.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcWithdrawnDate);
            cmd.Parameters.AddWithValue("@LcPaymentAmount", (!LClaim.LcPaymentAmount.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcPaymentAmount);
            cmd.Parameters.AddWithValue("@LcRejectionDate", (!LClaim.LcRejectionDate.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcRejectionDate);
            cmd.Parameters.AddWithValue("@LcPaymentBatchNumber", (!LClaim.LcPaymentBatchNumber.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcPaymentBatchNumber);
            cmd.Parameters.AddWithValue("@LcClaimBatchNumber", (!LClaim.LcClaimBatchNumber.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcClaimBatchNumber);
            cmd.Parameters.AddWithValue("@LcCreatedDateTime", (!LClaim.LcCreatedDateTime.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcCreatedDateTime);
            cmd.Parameters.AddWithValue("@LcMSISDN", string.IsNullOrEmpty(LClaim.LcMSISDN) ? (object)System.DBNull.Value : (object)LClaim.LcMSISDN);
            cmd.Parameters.AddWithValue("@LcCustomerName", string.IsNullOrEmpty(LClaim.LcCustomerName) ? (object)System.DBNull.Value : (object)LClaim.LcCustomerName);
            cmd.Parameters.AddWithValue("@LcBAN", string.IsNullOrEmpty(LClaim.LcBAN) ? (object)System.DBNull.Value : (object)LClaim.LcBAN);
            cmd.Parameters.AddWithValue("@LcIMEI", string.IsNullOrEmpty(LClaim.LcIMEI) ? (object)System.DBNull.Value : (object)LClaim.LcIMEI);
            cmd.Parameters.AddWithValue("@LcOrderNumber", string.IsNullOrEmpty(LClaim.LcOrderNumber) ? (object)System.DBNull.Value : (object)LClaim.LcOrderNumber);
            cmd.Parameters.AddWithValue("@A01", string.IsNullOrEmpty(LClaim.A01) ? (object)System.DBNull.Value : (object)LClaim.A01);
            cmd.Parameters.AddWithValue("@A02", string.IsNullOrEmpty(LClaim.A02) ? (object)System.DBNull.Value : (object)LClaim.A02);
            cmd.Parameters.AddWithValue("@A03", string.IsNullOrEmpty(LClaim.A03) ? (object)System.DBNull.Value : (object)LClaim.A03);
            cmd.Parameters.AddWithValue("@A04", string.IsNullOrEmpty(LClaim.A04) ? (object)System.DBNull.Value : (object)LClaim.A04);
            cmd.Parameters.AddWithValue("@A05", string.IsNullOrEmpty(LClaim.A05) ? (object)System.DBNull.Value : (object)LClaim.A05);
            cmd.Parameters.AddWithValue("@A06", string.IsNullOrEmpty(LClaim.A06) ? (object)System.DBNull.Value : (object)LClaim.A06);
            cmd.Parameters.AddWithValue("@A07", string.IsNullOrEmpty(LClaim.A07) ? (object)System.DBNull.Value : (object)LClaim.A07);
            cmd.Parameters.AddWithValue("@A08", string.IsNullOrEmpty(LClaim.A08) ? (object)System.DBNull.Value : (object)LClaim.A08);
            cmd.Parameters.AddWithValue("@A09", string.IsNullOrEmpty(LClaim.A09) ? (object)System.DBNull.Value : (object)LClaim.A09);
            cmd.Parameters.AddWithValue("@A10", string.IsNullOrEmpty(LClaim.A10) ? (object)System.DBNull.Value : (object)LClaim.A10);
            cmd.Parameters.AddWithValue("@Id", LClaim.Id);
            cmd.Parameters.AddWithValue("@LcPayeeId", LClaim.LcPayeeId);
            cmd.Parameters.AddWithValue("@Action", Action);
            cmd.Parameters.AddWithValue("@LcCommissionPeriod", string.IsNullOrEmpty(LClaim.LcCommissionPeriod) ? (object)System.DBNull.Value : (object)LClaim.LcCommissionPeriod);
            cmd.Parameters.AddWithValue("@LcParentPayeeId", (!LClaim.LcParentPayeeId.HasValue) ? (object)System.DBNull.Value : (object)LClaim.LcParentPayeeId);
            DataSet ds = GetData(cmd);
            if (ds.Tables.Count == 0)
                return null;
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        public static void ExecutePutLPayee(LPayeeDecryptedViewModel LPayee)
        {

            //Section to get Payee Data from Stored Procedure starts Here
            var Query = "Exec [dbo].[USPPostLPayee]  @LpUserId,@LpCompanyId,@LpChannelId,@LpSubChannelId,@LpCreatedById,@LpUpdatedById,@LpPrimaryChannel,@LpPayeeCode,@LpEmail,@LpEffectiveStartDate,@LpEffectiveEndDate,@LpFileNames,@LpUserFriendlyFileNames,@LpBusinessUnit,@LpCanRaiseClaims,@LpChannelManager,@LpCreatedDateTime,@LpUpdatedDateTime,@LpTIN,@LpDistributionChannel,@LpPosition,@LpBatchNumber,@LpCreateLogin,@WFRequesterId,@WFAnalystId,@WFOrdinal,@WFManagerId,@WFCurrentOwnerId,@WFStatus,@WFType,@WFRequesterRoleId,@WFCompanyId,@WFComments,@LpFinOpsRoles,@ParameterCarrier,@LpCreatedByForm,@LpFirstName,@LpLastName,@LpPhone,@LpAddress,@LpTradingName,@A01,@A02,@A03,@A04,@A05,@A06,@A07,@A08,@A09,@A10,@LpBlockNotification,@Id,@Action";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@LpUserId", string.IsNullOrEmpty(LPayee.LpUserId) ? (object)System.DBNull.Value : (object)LPayee.LpUserId);
            cmd.Parameters.AddWithValue("@LpCompanyId", LPayee.LpCompanyId == 0 ? (object)System.DBNull.Value : (object)LPayee.LpCompanyId);
            cmd.Parameters.AddWithValue("@LpChannelId", (!LPayee.LpChannelId.HasValue) ? (object)System.DBNull.Value : (object)LPayee.LpChannelId.Value);
            cmd.Parameters.AddWithValue("@LpSubChannelId", (!LPayee.LpSubChannelId.HasValue) ? (object)System.DBNull.Value : (object)LPayee.LpSubChannelId.Value);
            cmd.Parameters.AddWithValue("@LpCreatedById", string.IsNullOrEmpty(LPayee.LpCreatedById) ? (object)System.DBNull.Value : (object)LPayee.LpCreatedById);
            cmd.Parameters.AddWithValue("@LpUpdatedById", string.IsNullOrEmpty(LPayee.LpUpdatedById) ? (object)System.DBNull.Value : (object)LPayee.LpUpdatedById);
            cmd.Parameters.AddWithValue("@LpPrimaryChannel", string.IsNullOrEmpty(LPayee.LpPrimaryChannel) ? (object)System.DBNull.Value : (object)LPayee.LpPrimaryChannel);
            cmd.Parameters.AddWithValue("@LpPayeeCode", string.IsNullOrEmpty(LPayee.LpPayeeCode) ? (object)System.DBNull.Value : (object)LPayee.LpPayeeCode);
            cmd.Parameters.AddWithValue("@LpEmail", string.IsNullOrEmpty(LPayee.LpEmail) ? (object)System.DBNull.Value : (object)LPayee.LpEmail);
            cmd.Parameters.AddWithValue("@LpEffectiveStartDate", (LPayee.LpEffectiveStartDate == null) ? (object)System.DBNull.Value : (object)LPayee.LpEffectiveStartDate.AddHours(6));
            cmd.Parameters.AddWithValue("@LpEffectiveEndDate", (LPayee.LpEffectiveEndDate == null) ? (object)System.DBNull.Value : (object)LPayee.LpEffectiveEndDate);
            cmd.Parameters.AddWithValue("@LpFileNames", string.IsNullOrEmpty(LPayee.LpFileNames) ? (object)System.DBNull.Value : (object)LPayee.LpFileNames);
            cmd.Parameters.AddWithValue("@LpUserFriendlyFileNames", string.IsNullOrEmpty(LPayee.LpUserFriendlyFileNames) ? (object)System.DBNull.Value : (object)LPayee.LpUserFriendlyFileNames);
            cmd.Parameters.AddWithValue("@LpBusinessUnit", string.IsNullOrEmpty(LPayee.LpBusinessUnit) ? (object)System.DBNull.Value : (object)LPayee.LpBusinessUnit);
            cmd.Parameters.AddWithValue("@LpCanRaiseClaims", LPayee.LpCanRaiseClaims);
            cmd.Parameters.AddWithValue("@LpChannelManager", string.IsNullOrEmpty(LPayee.LpChannelManager) ? (object)System.DBNull.Value : (object)LPayee.LpChannelManager);
            cmd.Parameters.AddWithValue("@LpCreatedDateTime", (LPayee.LpCreatedDateTime == null) ? (object)System.DBNull.Value : (object)LPayee.LpCreatedDateTime);
            cmd.Parameters.AddWithValue("@LpUpdatedDateTime", (LPayee.LpUpdatedDateTime == null) ? (object)System.DBNull.Value : (object)LPayee.LpUpdatedDateTime);
            cmd.Parameters.AddWithValue("@LpTIN", string.IsNullOrEmpty(LPayee.LpTIN) ? (object)System.DBNull.Value : (object)LPayee.LpTIN);
            cmd.Parameters.AddWithValue("@LpDistributionChannel", string.IsNullOrEmpty(LPayee.LpDistributionChannel) ? (object)System.DBNull.Value : (object)LPayee.LpDistributionChannel);
            cmd.Parameters.AddWithValue("@LpPosition", string.IsNullOrEmpty(LPayee.LpPosition) ? (object)System.DBNull.Value : (object)LPayee.LpPosition);
            cmd.Parameters.AddWithValue("@LpBatchNumber", (LPayee.LpBatchNumber == null) ? (object)System.DBNull.Value : (object)LPayee.LpBatchNumber);
            cmd.Parameters.AddWithValue("@LpCreateLogin", LPayee.LpCreateLogin);
            cmd.Parameters.AddWithValue("@WFRequesterId", string.IsNullOrEmpty(LPayee.WFRequesterId) ? (object)System.DBNull.Value : (object)LPayee.WFRequesterId);
            cmd.Parameters.AddWithValue("@WFAnalystId", string.IsNullOrEmpty(LPayee.WFAnalystId) ? (object)System.DBNull.Value : (object)LPayee.WFAnalystId);
            cmd.Parameters.AddWithValue("@WFOrdinal", LPayee.WFOrdinal);
            cmd.Parameters.AddWithValue("@WFManagerId", string.IsNullOrEmpty(LPayee.WFManagerId) ? (object)System.DBNull.Value : (object)LPayee.WFManagerId);
            cmd.Parameters.AddWithValue("@WFCurrentOwnerId", string.IsNullOrEmpty(LPayee.WFCurrentOwnerId) ? (object)System.DBNull.Value : (object)LPayee.WFCurrentOwnerId);
            cmd.Parameters.AddWithValue("@WFStatus", string.IsNullOrEmpty(LPayee.WFStatus) ? (object)System.DBNull.Value : (object)LPayee.WFStatus);
            cmd.Parameters.AddWithValue("@WFType", string.IsNullOrEmpty(LPayee.WFType) ? (object)System.DBNull.Value : (object)LPayee.WFType);
            cmd.Parameters.AddWithValue("@WFRequesterRoleId", string.IsNullOrEmpty(LPayee.WFRequesterRoleId) ? (object)System.DBNull.Value : (object)LPayee.WFRequesterRoleId);
            cmd.Parameters.AddWithValue("@WFCompanyId", LPayee.WFCompanyId);
            cmd.Parameters.AddWithValue("@WFComments", string.IsNullOrEmpty(LPayee.WFComments) ? (object)System.DBNull.Value : (object)LPayee.WFComments);
            cmd.Parameters.AddWithValue("@LpFinOpsRoles", string.IsNullOrEmpty(LPayee.LpFinOpsRoles) ? (object)System.DBNull.Value : (object)LPayee.LpFinOpsRoles);
            cmd.Parameters.AddWithValue("@ParameterCarrier", string.IsNullOrEmpty(LPayee.ParameterCarrier) ? (object)System.DBNull.Value : (object)LPayee.ParameterCarrier);
            cmd.Parameters.AddWithValue("@LpCreatedByForm", LPayee.LpCreatedByForm);
            cmd.Parameters.AddWithValue("@LpFirstName", string.IsNullOrEmpty(LPayee.LpFirstName) ? (object)System.DBNull.Value : (object)LPayee.LpFirstName);
            cmd.Parameters.AddWithValue("@LpLastName", string.IsNullOrEmpty(LPayee.LpLastName) ? (object)System.DBNull.Value : (object)LPayee.LpLastName);
            cmd.Parameters.AddWithValue("@LpPhone", string.IsNullOrEmpty(LPayee.LpPhone) ? (object)System.DBNull.Value : (object)LPayee.LpPhone);
            cmd.Parameters.AddWithValue("@LpAddress", string.IsNullOrEmpty(LPayee.LpAddress) ? (object)System.DBNull.Value : (object)LPayee.LpAddress);
            cmd.Parameters.AddWithValue("@LpTradingName", string.IsNullOrEmpty(LPayee.LpTradingName) ? (object)System.DBNull.Value : (object)LPayee.LpTradingName);
            cmd.Parameters.AddWithValue("@A01", string.IsNullOrEmpty(LPayee.A01) ? (object)System.DBNull.Value : (object)LPayee.A01);
            cmd.Parameters.AddWithValue("@A02", string.IsNullOrEmpty(LPayee.A02) ? (object)System.DBNull.Value : (object)LPayee.A02);
            cmd.Parameters.AddWithValue("@A03", string.IsNullOrEmpty(LPayee.A03) ? (object)System.DBNull.Value : (object)LPayee.A03);
            cmd.Parameters.AddWithValue("@A04", string.IsNullOrEmpty(LPayee.A04) ? (object)System.DBNull.Value : (object)LPayee.A04);
            cmd.Parameters.AddWithValue("@A05", string.IsNullOrEmpty(LPayee.A05) ? (object)System.DBNull.Value : (object)LPayee.A05);
            cmd.Parameters.AddWithValue("@A06", string.IsNullOrEmpty(LPayee.A06) ? (object)System.DBNull.Value : (object)LPayee.A06);
            cmd.Parameters.AddWithValue("@A07", string.IsNullOrEmpty(LPayee.A07) ? (object)System.DBNull.Value : (object)LPayee.A07);
            cmd.Parameters.AddWithValue("@A08", string.IsNullOrEmpty(LPayee.A08) ? (object)System.DBNull.Value : (object)LPayee.A08);
            cmd.Parameters.AddWithValue("@A09", string.IsNullOrEmpty(LPayee.A09) ? (object)System.DBNull.Value : (object)LPayee.A09);
            cmd.Parameters.AddWithValue("@A10", string.IsNullOrEmpty(LPayee.A10) ? (object)System.DBNull.Value : (object)LPayee.A10);
            cmd.Parameters.AddWithValue("@LpBlockNotification", LPayee.LpCreatedByForm);
            cmd.Parameters.AddWithValue("@Id", LPayee.Id);
            cmd.Parameters.AddWithValue("@Action", "Edit");
            DataSet ds = GetData(cmd);
        }


        //method to Select payees based on filters
        public static DataTable ExecuteSPGetFilteredPayeeResults(int? Id, string PrimaryChannel, int? CompanyId, int? ParentId, string WFStatus, string PayeeCode, string Email, DateTime? EffectiveStartDate, DateTime? EffectiveEndDate, int? ChannelId, int? WFOrdinal, string UserId)
        {
            var Query = "Exec [dbo].[SPGetFilteredPayeeResults] @Id,@PrimaryChannel,@CompanyId,@ParentId,@WFStatus,@PayeeCode,@Email,@EffectiveStartDate,@EffectiveEndDate,@ChannelId,@WFOrdinal,@UserId";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@Id", (!Id.HasValue) ? (object)System.DBNull.Value : (object)Id.Value);
            cmd.Parameters.AddWithValue("@PrimaryChannel", string.IsNullOrEmpty(PrimaryChannel) ? (object)System.DBNull.Value : (object)PrimaryChannel);
            cmd.Parameters.AddWithValue("@CompanyId", (!CompanyId.HasValue) ? (object)System.DBNull.Value : (object)CompanyId.Value);
            cmd.Parameters.AddWithValue("@ParentId", (!ParentId.HasValue) ? (object)System.DBNull.Value : (object)ParentId.Value);
            cmd.Parameters.AddWithValue("@WFStatus", string.IsNullOrEmpty(WFStatus) ? (object)System.DBNull.Value : (object)WFStatus);
            cmd.Parameters.AddWithValue("@PayeeCode", string.IsNullOrEmpty(PayeeCode) ? (object)System.DBNull.Value : (object)PayeeCode);
            cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? (object)System.DBNull.Value : (object)Email);
            cmd.Parameters.AddWithValue("@EffectiveStartDate", (!EffectiveStartDate.HasValue) ? (object)System.DBNull.Value : (object)EffectiveStartDate.Value);
            cmd.Parameters.AddWithValue("@EffectiveEndDate", (!EffectiveEndDate.HasValue) ? (object)System.DBNull.Value : (object)EffectiveEndDate.Value);
            cmd.Parameters.AddWithValue("@ChannelId", (!ChannelId.HasValue) ? (object)System.DBNull.Value : (object)ChannelId.Value);
            cmd.Parameters.AddWithValue("@WFOrdinal", (!WFOrdinal.HasValue) ? (object)System.DBNull.Value : (object)WFOrdinal.Value);
            cmd.Parameters.AddWithValue("@UserId", string.IsNullOrEmpty(UserId) ? (object)System.DBNull.Value : (object)UserId);
            DataSet ds = GetData(cmd);
            if (ds.Tables.Count == 0)
                return null;
            var result = ds.Tables[0];
            return result;
        }

        //Method to add entry inAudit Log
        public static string ExecuteSPAuditLog(string WorkFlowName, string ControlCode, string ControlDecription, string Action, string ActionType, string ActionedById, DateTime ActionDateTime, string OldStatus, string NewStatus, string EntityType, int EntityId, string EntityName, int WorkFlowId, int CompanyId, string Comments, string RoleId, string LaPeriod)
        {
            var Query = "Exec  dbo.SpLogAudit @WorkFlow, @WorkFlow,@ControlCode,@ControlDecription,@Action,@ActionType,@ActionedById,@ActionDateTime,@OldStatus,@NewStatus,@EntityType,@EntityId,@EntityName, @WorkFlowId,@CompanyId, @Comments,@RoleId,@LaPeriod ";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@WorkFlow", string.IsNullOrEmpty(WorkFlowName) ? (object)System.DBNull.Value : (object)WorkFlowName);
            cmd.Parameters.AddWithValue("@ControlCode", ControlCode);
            cmd.Parameters.AddWithValue("@ControlDecription", string.IsNullOrEmpty(ControlDecription) ? (object)System.DBNull.Value : (object)ControlDecription);
            cmd.Parameters.AddWithValue("@Action", Action);
            cmd.Parameters.AddWithValue("@ActionType", ActionType);
            cmd.Parameters.AddWithValue("@ActionedById", string.IsNullOrEmpty(ActionedById) ? (object)System.DBNull.Value : (object)ActionedById);
            cmd.Parameters.AddWithValue("@ActionDateTime", ActionDateTime);
            cmd.Parameters.AddWithValue("@OldStatus", OldStatus);
            cmd.Parameters.AddWithValue("@NewStatus", NewStatus);
            cmd.Parameters.AddWithValue("@EntityType", string.IsNullOrEmpty(EntityType) ? (object)System.DBNull.Value : (object)EntityType);
            cmd.Parameters.AddWithValue("@EntityId", EntityId);
            cmd.Parameters.AddWithValue("@EntityName", string.IsNullOrEmpty(EntityName) ? (object)System.DBNull.Value : (object)EntityName);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@WorkFlowId", WorkFlowId);
            cmd.Parameters.AddWithValue("@Comments", string.IsNullOrEmpty(Comments) ? (object)System.DBNull.Value : (object)Comments);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@LaPeriod", string.IsNullOrEmpty(LaPeriod) ? (object)System.DBNull.Value : (object)LaPeriod);

            DataSet ds = GetData(cmd);
            if (ds.Tables.Count == 0)
                return null;
            string result = Convert.ToString(ds.Tables[0].Rows[0][0]);
            return result;
        }
        public static void SaveSPPayeeDoucmentData(int CompanyId, string LoggedInRoleId, string WorkFlowName, string CommissionPeriod, string CreatedById, string Description, string DocumentList, string Name, string PayeeList, bool SendEmail, string UpdatedById, DateTime UpdatedDateTime, string Comments, string CurrentOwnerId, string ManagerId, string RequesterRoleId, string RequesterId, string AttachedFiles, string Status, string Type, string AttachedFilePath, string ParameterCarrier, string SupportingDocumentFiles, string AnalystId, int Ordinal, string SupportingDocumentFilePath, string LoggedInUserId, string CallType, string PayeeListCarrier, out string ValidationMessageFinal, out string FinalID)
        {
            String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(strConnString);
            con.Open();
            ValidationMessageFinal = "";
            FinalID = "0";
            // var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
            var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
            //SqlDataAdapter sda = new SqlDataAdapter();

            //var Query = "Exec  [dbo].[USPInsertPayeeDocument] @CompanyId, @LoggedInRoleId, @WorkFlowName , @CommissionPeriod, @CreatedById , @Description, @DocumentList , @Name ,@PayeeList , @SendEmail, @UpdatedById , @Comments , @CurrentOwnerId, @ManagerId , @RequesterRoleId , @RequesterId , @AttachedFiles , @Status, @Type, @AttachedFilePath , @ParameterCarrier, @SupportingDocumentFiles, @AnalystId, @Ordinal, @SupportingDocumentFilePath, @CallType, @PayeeListCarrier, @ValodationMessageFinal, @FinalID";

            var Query = "Exec  [dbo].[USPInsertPayeeDocument] @CompanyId, @LoggedInRoleId, @WorkFlowName,@CommissionPeriod, @CreatedById , @Description,@DocumentList , @Name ,@PayeeList , @SendEmail, @UpdatedById , @Comments,@CurrentOwnerId, @ManagerId , @RequesterRoleId , @RequesterId , @AttachedFiles , @Status, @Type,@AttachedFilePath , @ParameterCarrier, @SupportingDocumentFiles,@AnalystId ,@Ordinal, @SupportingDocumentFilePath,@CallType,@PayeeListCarrier,@ProjectEnviournment";
            SqlCommand cmd = new SqlCommand(Query, con);
            // SqlCommand cmd = new SqlCommand(Query);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Connection = con;

            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@LoggedInRoleId", LoggedInRoleId);
            cmd.Parameters.AddWithValue("@WorkFlowName", WorkFlowName);
            cmd.Parameters.AddWithValue("@CommissionPeriod", CommissionPeriod);
            cmd.Parameters.AddWithValue("@CreatedById", CreatedById);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@DocumentList", DocumentList);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@PayeeList", string.IsNullOrEmpty(PayeeList) ? (object)System.DBNull.Value : (object)PayeeList);
            cmd.Parameters.AddWithValue("@SendEmail", SendEmail);
            cmd.Parameters.AddWithValue("@UpdatedById", UpdatedById);
            cmd.Parameters.AddWithValue("@Comments", Comments == null ? (object)System.DBNull.Value : Comments);
            cmd.Parameters.AddWithValue("@CurrentOwnerId", CurrentOwnerId);
            cmd.Parameters.AddWithValue("@ManagerId", string.IsNullOrEmpty(ManagerId) ? (object)System.DBNull.Value : (object)ManagerId);
            cmd.Parameters.AddWithValue("@RequesterRoleId", RequesterRoleId);
            cmd.Parameters.AddWithValue("@RequesterId", RequesterId);
            cmd.Parameters.AddWithValue("@AttachedFiles", AttachedFiles);
            cmd.Parameters.AddWithValue("@Status", Status);
            cmd.Parameters.AddWithValue("@Type", Type);
            cmd.Parameters.AddWithValue("@AttachedFilePath", AttachedFilePath);
            cmd.Parameters.AddWithValue("@ParameterCarrier", ParameterCarrier);
            cmd.Parameters.AddWithValue("@SupportingDocumentFiles", SupportingDocumentFiles);
            cmd.Parameters.AddWithValue("@AnalystId", string.IsNullOrEmpty(AnalystId) ? (object)System.DBNull.Value : (object)AnalystId);
            cmd.Parameters.AddWithValue("@Ordinal", Ordinal);
            // cmd.Parameters.AddWithValue("@SupportingDocumentFilePath", SupportingDocumentFilePath);
            cmd.Parameters.AddWithValue("@SupportingDocumentFilePath", string.IsNullOrEmpty(SupportingDocumentFilePath) ? (object)System.DBNull.Value : (object)SupportingDocumentFilePath);
            cmd.Parameters.AddWithValue("@CallType", CallType);
            //cmd.Parameters.AddWithValue("@PayeeListCarrier", PayeeListCarrier);
            cmd.Parameters.AddWithValue("@PayeeListCarrier", string.IsNullOrEmpty(PayeeListCarrier) ? (object)System.DBNull.Value : (object)PayeeListCarrier);
            cmd.Parameters.AddWithValue("@ProjectEnviournment", ProjectEnviournment);
            //cmd.Parameters.AddWithValue("@ValodationMessageFinal", "");
            //cmd.Parameters.AddWithValue("@FinalID", 1);




            var tb = new DataTable();

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);



            if (tb.Rows.Count == 1)
            {
                FinalID = Convert.ToString(tb.Rows[0][0]);
                ValidationMessageFinal = Convert.ToString(tb.Rows[0][1]);

            }

            con.Close();

        }

        public static int intHandleDBNull(object i)
        {
            if (i == System.DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(i);
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
        
        //This is a generalised method to run query in Opco DB and return result in datatable//
        public static DataTable GetQueryResultFromOpcoDB(int CompanyId, string Query, string CommissionPeriod)
        {
            //Get the details of the database which is to be queried to obtain record 
            var DatabaseDetails = db.Database.SqlQuery<SPDBContextEncryptedViewModel>("Exec [dbo].[SPGetDBContext] {0},{1}", CommissionPeriod, CompanyId).FirstOrDefault();//db.FNGetDBContext(CommissionPeriod, CompanyId).FirstOrDefault();
            //RK 21022018 - Added text " Column Encryption Setting = Enabled" as a part of encryption implementation
            string ConnectionString = "data source={HostName};  Column Encryption Setting = Enabled; initial catalog={DataBaseName};persist security info=True;user id={DatabaseUser};password={Password};MultipleActiveResultSets=True;Connect Timeout=60";
            //Replace the credentials required to frame connection string from LDatabases
            if (DatabaseDetails != null)
            {
                ConnectionString = ConnectionString.Replace("{HostName}", DatabaseDetails.HostName);
                ConnectionString = ConnectionString.Replace("{DataBaseName}", DatabaseDetails.DatabaseName);
                ConnectionString = ConnectionString.Replace("{DatabaseUser}", DatabaseDetails.LoginId);
                ConnectionString = ConnectionString.Replace("{Password}", DatabaseDetails.Password);
                Query = Query.Replace("{Schema}", DatabaseDetails.SchemaName);
            }

            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConnectionString;
            //SqlCommand cmd = new SqlCommand(Qry, cn);
            DataTable dt = new DataTable { TableName = "MyTableName" };
            SqlCommand cmd = new SqlCommand(Query, cn);
            cn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            cn.Close();
            return dt;
        }

        //This is a generalised method to run query in Opco DB and return result in datatable
        public static void RunUpdateQueryInOpcoDB(int CompanyId, string Query, string CommissionPeriod)
        {
            //Get the details of the database which is to be queried to obtain record 
            //var DatabaseDetails = db.FNGetDBContext(CommissionPeriod, CompanyId).FirstOrDefault();
            //RK 30052018- Commented above statement and added the below one to get the password in decrypted format
            var DatabaseDetails = db.Database.SqlQuery<SPDBContextEncryptedViewModel>("Exec [dbo].[SPGetDBContext] {0},{1}", CommissionPeriod, CompanyId).FirstOrDefault();//db.FNGetDBContext(CommissionPeriod, CompanyId).FirstOrDefault();
            string ConnectionString = "data source={HostName};initial catalog={DataBaseName};persist security info=True;user id={DatabaseUser};password={Password};MultipleActiveResultSets=True;Connect Timeout=60";
            //Replace the credentials required to frame connection string from LDatabases
            if (DatabaseDetails != null)
            {
                ConnectionString = ConnectionString.Replace("{HostName}", DatabaseDetails.HostName);
                ConnectionString = ConnectionString.Replace("{DataBaseName}", DatabaseDetails.DatabaseName);
                ConnectionString = ConnectionString.Replace("{DatabaseUser}", DatabaseDetails.LoginId);
                ConnectionString = ConnectionString.Replace("{Password}", DatabaseDetails.Password);
                Query = Query.Replace("{Schema}", DatabaseDetails.SchemaName);
            }

            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConnectionString;
            SqlCommand cmd = new SqlCommand(Query, cn);
            cn.Open();
            cmd.ExecuteNonQuery();
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //da.Fill(dt);
            cn.Close();
        }

        //This method will return the ErrorMessage string when called for DbEntityValidationException
        public static string GetEntityValidationErrorMessage(DbEntityValidationException e)
        {
            string ErrorMessage = "";
            foreach (var eve in e.EntityValidationErrors)
            {
                ErrorMessage += string.Format("Entity of type {0} in state {1} has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    ErrorMessage += string.Format("- Property: {0}, Error: {1}", ve.PropertyName, ve.ErrorMessage);
                }
            }
            return ErrorMessage;
        }

        //public static void LogError(string ExceptionMessage,string ErrorDescription)
        //{
        //   //store Username and workflowname in session variable 
        //   Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "controller", "Action", ExceptionMessage, "username", "Type2", "desc", "resolution", "L2Admin", "field", 0,"New");//Status is new on entry to error log
        //}

        /*Section ends here*/

        //commented Copy of Send Email With Sender as Notifier@megacube.com.au as sender
        //public static bool SendEmail(string ToAddress, string CcAddress, string EmailSubject, string EmailBody, string CompanyCode)//
        //{
        //    try
        //    {
        //        SmtpClient Client = new SmtpClient("mail.megacube.com.au");
        //        Client.Credentials = new NetworkCredential("notifier@megacube.com.au", "notifier#123");
        //        Client.Port = 25;
        //        MailMessage message = new MailMessage();
        //        message.From = new MailAddress("notifier@megacube.com.au");

        //        //Send Email to Different Email Id as per Project Enviournment
        //        switch (ConfigurationManager.AppSettings["ProjectEnviournment"])
        //        {
        //            case "Dev":
        //            case "Test":
        //                //var CompanyCode = db.GCompanies.Find(CompanyId).GcCode.ToLower();
        //                var TestEmailBox = CompanyCode + ConfigurationManager.AppSettings["TestEmail"];
        //                message.To.Add(TestEmailBox);
        //                break;
        //            case "Prod":
        //                message.To.Add(ToAddress);
        //                if (!string.IsNullOrEmpty(CcAddress))
        //                {
        //                    message.CC.Add(CcAddress);
        //                }
        //                break;
        //        }

        //        message.Body = EmailBody;
        //        message.Subject = EmailSubject;
        //        message.IsBodyHtml = true;
        //        Client.Send(message);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public static bool SendEmail(string ToAddress, string CcAddress, string EmailSubject, string EmailBody, string CompanyCode)//
        {
            try
            {
                //SmtpClient Client = new SmtpClient("email-smtp.eu-west-1.amazonaws.com");
                //Client.Credentials = new NetworkCredential("AKIAJKZMZQBQQS6AZ4PQ", "AvssA5Icped8SGdmHrNXqXw9LTH9Q28DleJO+m6mBK6l");
                //Client.Port = 587;
                //MailMessage message = new MailMessage();
                //message.From = new MailAddress("sos@vodafonelite.com");

                SmtpClient Client = new SmtpClient("mail.megacube.com.au");
                Client.Credentials = new NetworkCredential("notifier@megacube.com.au", "notifier#123");
                Client.Port = 25;
                MailMessage message = new MailMessage();
                message.From = new MailAddress("notifier@megacube.com.au");

                //Get receiver Name based on Project Env
                //var ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ConfigurationManager.AppSettings["ProjectEnviournment"], ToAddress).FirstOrDefault();
                //RK R2.3 - added to get project environment from db
                var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
                var ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, ToAddress).FirstOrDefault();
                message.To.Add(ReceiverEmail);
                //Send Email to Different Email Id as per Project Enviournment
                //switch (ConfigurationManager.AppSettings["ProjectEnviournment"])
                //{
                //    case "Dev":
                //    case "Test":
                //        //var CompanyCode = db.GCompanies.Find(CompanyId).GcCode.ToLower();
                //        var TestEmailBox = CompanyCode + ConfigurationManager.AppSettings["TestEmail"];
                //        message.To.Add(TestEmailBox);
                //        break;
                //    case "Prod":
                //        message.To.Add(ToAddress);
                //        if (!string.IsNullOrEmpty(CcAddress))
                //        {
                //            message.CC.Add(CcAddress);
                //        }
                //        break;
                //}

                message.Body = EmailBody;
                message.Subject = EmailSubject;
                message.IsBodyHtml = true;
                Client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendEmailSES(string ToAddress, string CcAddress, string EmailSubject, string EmailBody, string CompanyCode)//
        {
            try
            {
                const String FROM = "lite_support@megacube.com.au";   // Replace with your "From" address. This address must be verified.

                // Supply your SMTP credentials below. Note that your SMTP credentials are different from your AWS credentials.
                const String SMTP_USERNAME = "YOUR_SMTP_USERNAME";  // Replace with your SMTP username. 
                const String SMTP_PASSWORD = "YOUR_SMTP_PASSWORD";  // Replace with your SMTP password.

                // Amazon SES SMTP host name. This example uses the US West (Oregon) region.
                const String HOST = "email-smtp.us-west-2.amazonaws.com";

                // The port you will connect to on the Amazon SES SMTP endpoint. We are choosing port 587 because we will use
                // STARTTLS to encrypt the connection.
                const int PORT = 587;

                // Create an SMTP client with the specified host name and port.
                using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(HOST, PORT))
                {
                    // Create a network credential with your SMTP user name and password.
                    client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                    // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
                    // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
                    client.EnableSsl = true;

                    //Send Email to Different Email Id as per Project Enviournment
                    //RK R2.3 - added to get project environment from db
                    var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
                    switch (ProjectEnviournment)
                    {
                        case "Dev":
                        case "Test":
                            //var CompanyCode = db.GCompanies.Find(CompanyId).GcCode.ToLower();
                            ToAddress = CompanyCode + ConfigurationManager.AppSettings["TestEmail"];
                            CcAddress = null;
                            break;
                        case "Prod":
                            //Do Nothing
                            break;
                    }

                    // Send the email. 
                    client.Send(FROM, ToAddress, EmailSubject, EmailBody);
                }


                return true;
            }
            catch
            {
                return false;
            }
        }

        //the below method is defined to store the error details in Gerror logs and send email of the same
        //static VodafoneSOSLiteEntities db = new VodafoneSOSLiteEntities();
        //public static bool LogApiError(string Controller,string Method,string ErrorMessage)
        //{
        //    var GErrorLog = new GErrorLog {GelErrorDateTime=DateTime.UtcNow,GelSourceProject="Sos WebApi",GelController=Controller ,GelMethod=Method,GelStackTrace=ErrorMessage};
        //    db.GErrorLogs.Add(GErrorLog);
        //    db.SaveChanges();
        //    //Send mail to  l2 admin
        //    var Subject = ConfigurationManager.AppSettings["ExceptionEmailSubject"];
        //    string Body;
        //    var UserName = (System.Web.HttpContext.Current.Session["UserName"] != null) ? System.Web.HttpContext.Current.Session["UserName"].ToString() : "";
        //    Body = "<table border='1'><tr><td>Application Name</td><td>" + ConfigurationManager.AppSettings["ExceptionEmailProjectName"] + "</td></tr><tr><td>Controller</td><td>" + Controller + "</td></tr><tr><td>Method Name</td><td>" + Method + "</td></tr><tr><td>Exception Date/Time(Utc)</td><td>" + DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td> </td></tr><tr><td>Stack Trace</td><td>" +ErrorMessage + "</td></tr></table>";

        //    SendEmail(ConfigurationManager.AppSettings["ExceptionEmailTo"], ConfigurationManager.AppSettings["ExceptionEmailCc"], Subject, Body);
        //    return true;
        //}

        //check user exist or not
        public static bool UserExist(string userName)
        {
            using (PrincipalContext pc = getPrincipalContext())
            {
                using (var foundUser = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, userName))
                {
                    return foundUser != null;
                }
            }
        }
        public static AuthenticationResult AddUserToGroup(UserPrincipal oUserPrincipal, string sGroupName)
        {
            try
            {
                GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
                if (oUserPrincipal != null && oGroupPrincipal != null)
                {
                    oGroupPrincipal.Members.Add(oUserPrincipal);
                    oGroupPrincipal.Save();
                }
                return new AuthenticationResult();
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }

        public static AuthenticationResult CreateUser(string Email, string ProjectEnviournment, int? CompanyId, string Identifier, string LoggedInUserId, string Comments, string Password)
        {
            ADUserViewModel model = new ADUserViewModel();
            model.Email = Email;
            PrincipalContext pc = getPrincipalContext();
            UserPrincipal userPrincipal = new UserPrincipal(pc);
            string user = model.Email;
            if (string.IsNullOrEmpty(Password))
            {
                //Add Password as per project enviournment
                switch (ProjectEnviournment)
                {
                    case "Prod":
                        RandomPassword pwd = new RandomPassword();
                        string randompwd = pwd.Generate();
                        model.Password = randompwd;//ConfigurationManager.AppSettings["DefaultUserPassword"];
                        break;
                    default:
                        model.Password = ConfigurationManager.AppSettings["DefaultUserPassword"];
                        break;
                }
            }
            else
            {
                model.Password = Password;
            }
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, user);
            string EUG = "SOS " + ProjectEnviournment + " Users";
            //If User already exists, and member of EUG, raise error, User exists else, add to EUG
            //If User does not exists, Create new User and add it to EUG(Env User Group)
            if (up == null)
            {
                try
                {
                    if (user.Length > 20)
                    {
                        string substr = user.Substring(0, 20);
                        char[] charsToTrim = { '@', '.' };
                        substr = substr.TrimEnd(charsToTrim);
                        userPrincipal.SamAccountName = substr;
                    }
                    else
                    {
                        userPrincipal.SamAccountName = user;
                    }
                    userPrincipal.UserPrincipalName = user;
                    userPrincipal.SetPassword(model.Password);
                    userPrincipal.PasswordNeverExpires = true;//SS changed this method24-72017.ExpirePasswordNow();//using the property as user need to reset password on first logon
                    userPrincipal.Enabled = true;
                    userPrincipal.Save();
                    //Add User to Group
                    var result = AddUserToGroup(userPrincipal, EUG);
                    //Get receiver Name based on Project Env
                    if (!string.IsNullOrEmpty(Identifier) && CompanyId.HasValue)
                    {
                        var ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, model.Email).FirstOrDefault();
                        var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeUser").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                        if (EmailTemplate != null)
                        {
                            var EmailBody = EmailTemplate.LetEmailBody;
                            EmailBody = EmailBody.Replace("###EmailAddress###", model.Email);
                            var EmailSubject = EmailTemplate.LetEmailSubject;
                            Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody + "<br>" + Comments, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", "", "", "");
                        }
                        Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, "Vodafone LITE", "Hi ,<br>Your Vodafone LITE Password is " + model.Password, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                    }
                    if (!result.IsSuccess)
                    {
                        //User could  not be added to EUG
                        return new AuthenticationResult(result.ErrorMessage);
                    }
                    return new AuthenticationResult();
                }
                catch (Exception ex)
                {
                    return new AuthenticationResult(ex.Message);
                }
            }
            else
            {
                //chcek user is Member of EUG
                if (IsUserMemberOfEUG(up, EUG))
                {
                    return new AuthenticationResult("User Already exists.");
                }
                else
                {//Add User to Group 
                    var result = AddUserToGroup(up, EUG);
                    if (!result.IsSuccess)
                    {
                        //User could  not be added to EUG
                        return new AuthenticationResult(result.ErrorMessage);
                    }
                    //and reset pwd if PROD
                    if (ProjectEnviournment.Equals("PROD", StringComparison.OrdinalIgnoreCase))
                    {
                        model.NewPassword = model.Password; ;
                        SetUserPassword(model);
                        //Get receiver Name based on Project Env
                        if (!string.IsNullOrEmpty(Identifier) && CompanyId.HasValue)
                        {
                            var ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, model.Email).FirstOrDefault();
                            var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeUser").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                            if (EmailTemplate != null)
                            {
                                var EmailBody = EmailTemplate.LetEmailBody;
                                EmailBody = EmailBody.Replace("###EmailAddress###", model.Email);
                                var EmailSubject = EmailTemplate.LetEmailSubject;
                                Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody + "<br>" + Comments, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", "", "", "");
                            }
                            Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, "Vodafone LITE", "Hi ,<br>Your Vodafone LITE Password is " + model.Password, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                        }
                    }
                    return new AuthenticationResult("added to EUG");
                }
            }

        }

        //added by SG
        //CreateUser method is a utility to create AD user
        //SS is using this method to create random password and create user both as this same logic is repeated multiple times in code.
        public static AuthenticationResult CreateUser_old(string Email, string ProjectEnviournment, int? CompanyId, string Identifier, string LoggedInUserId, string Comments, string Password)
        {
            ADUserViewModel ADmodel = new ADUserViewModel();
            ADmodel.Email = Email;
            if (string.IsNullOrEmpty(Password))
            {
                //Add Password as per project enviournment
                switch (ProjectEnviournment)
                {
                    case "Prod":
                        RandomPassword pwd = new RandomPassword();
                        string randompwd = pwd.Generate();
                        ADmodel.Password = randompwd;//ConfigurationManager.AppSettings["DefaultUserPassword"];
                        break;
                    default:
                        ADmodel.Password = ConfigurationManager.AppSettings["DefaultUserPassword"];
                        break;
                }
            }
            else
            {
                ADmodel.Password = Password;
            }
            PrincipalContext pc = getPrincipalContext();
            UserPrincipal userPrincipal = new UserPrincipal(pc);
            string user = ADmodel.Email;
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, user);
            /*If the AD account already exist (say it has created from another env, AD being common), then act as per Env
             If Env = Prod: Drop AD account and re-create and Create “Welcome Email” entries in Email Bucket
             Else (Non-prod): Do nothing (leave the AD account as-is and do not send email)
             */
            bool CreateADAccount = false;
            if (up == null)//that means this user not found in AD
            {
                CreateADAccount = true;
            }
            else
            {
                if (ProjectEnviournment.Equals("Prod", StringComparison.OrdinalIgnoreCase))
                {
                    DeleteUser(ADmodel);
                    CreateADAccount = true;
                }
            }

            if (CreateADAccount)
            {
                try
                {
                    if (user.Length > 20)
                    {
                        string substr = user.Substring(0, 20);
                        char[] charsToTrim = { '@', '.' };
                        substr = substr.TrimEnd(charsToTrim);
                        userPrincipal.SamAccountName = substr;
                    }
                    else
                    {
                        userPrincipal.SamAccountName = user;
                    }
                    userPrincipal.UserPrincipalName = user;
                    userPrincipal.SetPassword(ADmodel.Password);
                    userPrincipal.PasswordNeverExpires = true;//SS changed this method24-72017.ExpirePasswordNow();//using the property as user need to reset password on first logon
                    userPrincipal.Enabled = true;
                    //userPrincipal.LastPasswordSet = null;
                    userPrincipal.Save();
                    //need to send pwd in separate mail
                    //send Email to give id and password
                    //Get receiver Name based on Project Env
                    if (!string.IsNullOrEmpty(Identifier) && CompanyId.HasValue)
                    {
                        var ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, ADmodel.Email).FirstOrDefault();
                        var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeUser").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                        if (EmailTemplate != null)
                        {
                            var EmailBody = EmailTemplate.LetEmailBody;
                            EmailBody = EmailBody.Replace("###EmailAddress###", ADmodel.Email);
                            var EmailSubject = EmailTemplate.LetEmailSubject;
                            Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody + "<br>" + Comments, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", "", "", "");
                        }
                        Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, "Vodafone LITE", "Hi ,<br>Your Vodafone LITE Password is " + ADmodel.Password, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                    }
                    return new AuthenticationResult();
                }
                catch (Exception ex)
                {
                    return new AuthenticationResult(ex.Message);
                }
            }
            else
            {
                return new AuthenticationResult("User Already exists.");
            }

        }


        //Added by SG - used to change the AD users password
        public static AuthenticationResult ChangeMyPassword(ADUserViewModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                //userPrincipal.ExpirePasswordNow();/*Expiring password so that AD does not apply the minimum day policy while changing password*/
                //userPrincipal.SetPassword(model.NewPassword);
                userPrincipal.ChangePassword(model.Password, model.NewPassword);
                userPrincipal.PasswordNeverExpires = true;
                userPrincipal.Save();
                //password changed successfully
                return new AuthenticationResult();
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }

        //Set password for user
        public static AuthenticationResult SetUserPassword(ADUserViewModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                userPrincipal.SetPassword(model.NewPassword);
                userPrincipal.PasswordNeverExpires = true;
                userPrincipal.Save();
                //password set successfully
                return new AuthenticationResult();
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }

        //To check AD account 
        public static Boolean CheckADAccount(ADUserViewModel model)
        {
            Boolean accountExits = true;
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                if (userPrincipal == null)
                {
                    accountExits = false;
                }
                else
                {
                    accountExits = true;
                }
            }
            catch (Exception ex)
            {
                accountExits = false;
            }
            return accountExits;
        }

        //Update userDetails
        public static AuthenticationResult UpdateUserDetails(ADUserViewModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                // userPrincipal.Enabled = model.Status;
                userPrincipal.SamAccountName = model.Email.Substring(0, 20);//SamAccounNamtName accepts only 20 chars
                userPrincipal.EmailAddress = model.Email;
                userPrincipal.Save();
                return new AuthenticationResult();
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }

        public static AuthenticationResult ActivateDeactivateUser(ADUserViewModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                if (userPrincipal != null)
                {
                    userPrincipal.Enabled = model.Status;
                    userPrincipal.Save();
                }
                else
                {
                    return new AuthenticationResult("user not found");
                }
                return new AuthenticationResult();
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }

        //public static bool LockAccount(ADUserViewModel model)
        //{
        //    try
        //    {
        //        PrincipalContext pc = getPrincipalContext();
        //        UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);

        //        if (!userPrincipal.IsAccountLockedOut())
        //        {

        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //Unlock an Account
        public static AuthenticationResult UnlockAccount(ADUserViewModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                if (userPrincipal.IsAccountLockedOut())
                {
                    userPrincipal.UnlockAccount();
                }
                return new AuthenticationResult();
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }

        //Delete an existing user 
        public static AuthenticationResult DeleteUser(ADUserViewModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                if (userPrincipal != null)
                {
                    userPrincipal.Delete();//deleted successfully
                    //Also Delete entry from LPasswordHistory for this User so that user can be redirected to reset password when this AD account is created again in future.
                    var AspnetUserDetail = db.AspNetUsers.Where(p => p.Email == model.Email).FirstOrDefault();
                    var companycode = db.GCompanies.Where(x => x.Id == AspnetUserDetail.GcCompanyId).FirstOrDefault();
                    var PasswordHistory = db.LPasswordHistories.Where(p => p.UserId == AspnetUserDetail.Id);
                    db.LPasswordHistories.RemoveRange(PasswordHistory);
                    //Also REmove the security question answer
                    var SecurityQuestionsData = db.MAspnetUsersGSecurityQuestions.Where(x => x.MAuqsqUserId == AspnetUserDetail.Id);
                    db.MAspnetUsersGSecurityQuestions.RemoveRange(SecurityQuestionsData);
                    db.SaveChanges();

                    db.Database.ExecuteSqlCommand("Update  XSchema" + companycode.GcCode.ToUpper() + ".XReportUsers set  XUserEmailID = XUserEmailID + 'X' where XUserEmailID='" + AspnetUserDetail.Email + "'");

                }
                else
                {
                    return new AuthenticationResult("user not found");
                }
                return new AuthenticationResult();

            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }

        //**************added on 21 Feb 2019**********************************************************************
        private static PrincipalContext getPrincipalContextForUserGroup()
        {
            string AdminUserName = ConfigurationManager.AppSettings["ADUserName"];
            string AdminUserPassword = ConfigurationManager.AppSettings["ADUserPassword"];
            string stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string OUstring = ConfigurationManager.AppSettings["ActiveDirectoryOUForUserGroup"];
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, stringDomainName, OUstring, AdminUserName, AdminUserPassword);
            return ctx;
        }

        public static GroupPrincipal GetGroup(string sGroupName)
        {
            PrincipalContext oPrincipalContext = getPrincipalContextForUserGroup();

            GroupPrincipal oGroupPrincipal =
            GroupPrincipal.FindByIdentity(oPrincipalContext, IdentityType.Name, sGroupName);
            return oGroupPrincipal;
        }
        public static bool IsUserMemberOfEUG(UserPrincipal oUserPrincipal, string sGroupName)
        {
            GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
            if (oUserPrincipal != null && oGroupPrincipal != null)
            {
                if (oGroupPrincipal.Members.Contains(oUserPrincipal))
                    return true;
                else
                    return false;
            }
            return false;
        }

        //Validate the user credentials for SignIn
        public static AuthenticationResult SignIn(ADUserViewModel model,string ProjectEnviournment)
        {
            PrincipalContext principalContext = getPrincipalContext();
            bool isAuthenticated = false;
            UserPrincipal userPrincipal = null;

            string EUG = "SOS " + ProjectEnviournment + " Users";
            bool isMember = false;
            try
            {
                isAuthenticated = principalContext.ValidateCredentials(model.Email, model.Password, ContextOptions.Negotiate);

                userPrincipal = UserPrincipal.FindByIdentity(principalContext, model.Email);
                if (userPrincipal.LastPasswordSet == null && userPrincipal.PasswordNeverExpires == false) //Must reset Password
                {
                    return new AuthenticationResult("Must reset Password");
                }
                //Check user is member of EUG
                if (IsUserMemberOfEUG(userPrincipal, EUG))
                {
                    isMember = true;
                }
            }
            catch (Exception)
            {
                //isAuthenticated = false;
                //userPrincipal = null;
            }
            if (userPrincipal == null || !isMember)
            {
                return new AuthenticationResult("AD Account does not exist");
            }
            if (!isAuthenticated)
            {
                return new AuthenticationResult("Username or Password is incorrect");
            }
            
            return new AuthenticationResult();
        }

        //This method returns the Principal Context using Admin user and password.
        private static PrincipalContext getPrincipalContext()
        {
            string AdminUserName = ConfigurationManager.AppSettings["ADUserName"];
            string AdminUserPassword = ConfigurationManager.AppSettings["ADUserPassword"];
            string stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string OUstring = ConfigurationManager.AppSettings["ActiveDirectoryOU"];
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, stringDomainName, OUstring, AdminUserName, AdminUserPassword);
            return ctx;
        }

        public static void ExportZipFromDataTable(string Query, string CompanyCode, string LoggedInUserName, string FileName, DataTable tb)
        {
            //The below lines of code converts the data returned from api to a datatable
            //var tb = new DataTable();
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            if (tb == null)
            {
                tb = new DataTable();
                string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection conn = new SqlConnection(ConnectionString);
                SqlCommand cmd = new SqlCommand(Query, conn);
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(tb);
                conn.Close();
                //The Ado.Net code ends here
            }

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < tb.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = tb.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
                //GC.Collect();//RK11102018 commented to improve performance
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
            //Create UserName Folder
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");
            }
            //Delete existing file
            if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip" + "/" + FileName))
            {
                System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip" + "/" + FileName);
            }
            //Get fileName without datetimestamp
            var FileNameArray = FileName.Split('_');
            var FileNameInsideZip = FileNameArray[0];
            using (ZipFile zip = new ZipFile())
            {
                FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip", FileNameInsideZip + ".xlsx"), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                workbook.Write(xfile);
                //tb.TableName = "Tmp";
                //zip.AddEntry("Temp.xml", (name, stream) => tb.WriteXml(stream));
                zip.AddFile(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip", FileNameInsideZip + ".xlsx"), "");
                zip.Save(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip", FileName));
                System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip", FileNameInsideZip + ".xlsx"));
                xfile.Close();
            }
        }
        public static void DeleteFileFromS3(string FileName, string CompanyCode, string UserName)
        {

            string _awsAccessKey = SOSAWSAccessKey; // ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey; // ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new DeleteObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = string.Format("{0}/{1}/{2}/{3}", ConfigurationManager.AppSettings["SOSBucketFolder"], CompanyCode, UserName, FileName),
                };

                client.DeleteObject(request);
            }


        }


        public static void CreateFolderInS3(string UserName, string CompanyCode)
        {

            string _awsAccessKey = SOSAWSAccessKey; // ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey; // ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    CannedACL = S3CannedACL.Private,
                    Key = string.Format("{0}/{1}/{2}/{3}", ConfigurationManager.AppSettings["SOSBucketFolder"], CompanyCode, UserName, ""),
                    ContentBody = UserName
                };
                client.PutObject(request);
            }


        }
        public static bool FileExistsInS3(string FileName, string UserName, string CompanyCode)
        {
            string _awsAccessKey = SOSAWSAccessKey; // ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey;// ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                 S3FileInfo s3FileInfo = new Amazon.S3.IO.S3FileInfo(client, _bucketName, string.Format("{0}/{1}/{2}/{3}", ConfigurationManager.AppSettings["SOSBucketFolder"], CompanyCode, UserName, FileName));
                 return s3FileInfo.Exists;
            }

        }

        public static bool FolderExistsInS3(string UserName, string CompanyCode)
        {
            string _awsAccessKey = SOSAWSAccessKey; // ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey; // ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                S3FileInfo s3FileInfo = new Amazon.S3.IO.S3FileInfo(client, _bucketName, string.Format("{0}/{1}/{2}/{3}", ConfigurationManager.AppSettings["SOSBucketFolder"], CompanyCode, UserName, ""));
                return s3FileInfo.Exists;
            }


        }

        public static void CreateFolderInA2S(string FolderPath, string FolderName)
        {

            string _awsAccessKey = A2SS3AccessKey; // ConfigurationManager.AppSettings["A2SS3AccessKey"];
            string _awsSecretKey = A2SS3SecretKey; // ConfigurationManager.AppSettings["A2SS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["A2SS3Bucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    CannedACL = S3CannedACL.Private,
                    Key = string.Format("{0}/{1}", FolderPath, ""),
                    ContentBody = FolderName
                };
                client.PutObject(request);
            }


        }

        public static bool FolderExistsInA2S(string FolderPath)
        {
            string _awsAccessKey = A2SS3AccessKey; // ConfigurationManager.AppSettings["A2SS3AccessKey"];
            string _awsSecretKey = A2SS3SecretKey; // ConfigurationManager.AppSettings["A2SS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["A2SS3Bucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                S3FileInfo s3FileInfo = new Amazon.S3.IO.S3FileInfo(client, _bucketName, string.Format("{0}/{1}", FolderPath, ""));
                return s3FileInfo.Exists;

            }


        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        //section to download file from S3 drive drectly 
        public static byte[] DownloadFromS3(string FileName, string FilePath)
        {
            string _awsAccessKey = SOSAWSAccessKey; // ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey; // ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];
            byte[] FileData;
            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = string.Format("{0}{1}", FilePath, FileName),
                };

                using (GetObjectResponse response = client.GetObject(request))
                {
                    FileData = ReadFully(response.ResponseStream);//response.WriteResponseStreamToFile(dest);
                }
            }
            return FileData;
        }

        public static bool UploadToA2S(Stream stream, string FileName, string FilePath)
        {
            string _awsAccessKey = A2SS3AccessKey; // ConfigurationManager.AppSettings["A2SS3AccessKey"];
            string _awsSecretKey = A2SS3SecretKey; // ConfigurationManager.AppSettings["A2SS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["A2SS3Bucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    CannedACL = S3CannedACL.Private,//PERMISSION TO FILE PUBLIC ACCESIBLE
                    Key = string.Format("{0}/{1}", FilePath, FileName),
                    InputStream = stream//SEND THE FILE STREAM
                };
                client.PutObject(request);
            }
            return true;
        }

        public static bool UploadToS3(Stream stream, string FileName, string UserName, string CompanyCode)
        {
            string _awsAccessKey = SOSAWSAccessKey; // ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey; // ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];
            //try
            //{
            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    CannedACL = S3CannedACL.Private,//PERMISSION TO FILE PUBLIC ACCESIBLE
                    Key = string.Format("{0}/{1}/{2}/{3}", ConfigurationManager.AppSettings["SOSBucketFolder"], CompanyCode, UserName, FileName),
                    InputStream = stream//SEND THE FILE STREAM
                };

                client.PutObject(request);
            }
            return true;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
        }

        public static string GetPayeeName(int PayeeID, Boolean AddPayeeCode)
        {
            string strReturnVal = "";
            var Query = "Exec  dbo.USPGetPayeeName @PayeeID, @AddPayeeCode";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@PayeeID", PayeeID);
            cmd.Parameters.AddWithValue("@AddPayeeCode", AddPayeeCode);
            DataSet ds = GetData(cmd);
            if (ds.Tables.Count == 0)
                return null;
            strReturnVal = Convert.ToString(ds.Tables[0].Rows[0][0]);
            return strReturnVal;
        }
        public static string GetPayeeFirstOrLast(int PayeeID, string Type)
        {
            string strReturnVal = "";
            var Query = "Exec  dbo.USPGetPayeeFirstOrLastName @PayeeID, @Type";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@PayeeID", PayeeID);
            cmd.Parameters.AddWithValue("@type", Type);
            DataSet ds = GetData(cmd);
            if (ds.Tables.Count == 0)
                return null;
            strReturnVal = Convert.ToString(ds.Tables[0].Rows[0][0]);
            return strReturnVal;
        }

        public static string getBannerValue()
        {
            try
            {
                string KeyValue = (from aa in db.GKeyValues.Where(aa => aa.GkvKey == "SOSBannerText")
                                   select aa.GkvValue).FirstOrDefault();
                return KeyValue;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public static DataTable GetParentByPayeeId(int PayeeID)
        {
            var Query = "Exec  dbo.[SPGetParentByPayeeId] @PayeeID";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@PayeeID", PayeeID);
            DataSet ds = GetData(cmd);
            if (ds.Tables.Count == 0)
                return null;
            return ds.Tables[0];
        }


        //method to only export to excel without zip
        public static string ExportToExcel(DataTable dt, string TempPath, string Filename)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet1");
            IRow row1 = sheet1.CreateRow(0);
            // ICellStyle _TextCellStyle = workbook.CreateCellStyle();
            // _TextCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("text");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
                // GC.Collect();
            }
            //loops through data  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var value = "";
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    value = dt.Rows[i][columnName].ToString();
                    //getting datatype for dataclumn. If it is datatime, convert its value to contain only date in MM/dd/yyyy format.

                    var type = dt.Columns[j].DataType.Name.ToString();
                    if (type == "DateTime")
                    {
                        if (!String.IsNullOrEmpty(value))
                            value = DateTime.Parse(value).ToString("dd/MM/yyyy");
                    }
                    cell.SetCellValue(value);
                    // GC.Collect();
                }

            }
            string FilePath = TempPath + Filename; //ConfigurationManager.AppSettings["TempUploadFolderName"] + Filename;
                                                   //obook.SaveAs(FilePath);

            FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            return ("Success");

        }

        public static void CreateExcelFromDataTable(DataTable DT, string FileName, string strWorksheetName, string CompanyCode, string LoggedInUserName, string FileNamewithoutextention)
        {
            // DT = DT.Rows.Cast<System.Data.DataRow>().Take(500000).CopyToDataTable();
            //DT = DT.DT1
            try
            {


                if (strWorksheetName == "") strWorksheetName = "Sheet1";
                var workbook = new XLWorkbook();
                workbook.Worksheets.Add(DT, strWorksheetName);
                var ws = workbook.Worksheet(1);
                //Create UserName Folder
                if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))//create directory if not present
                {
                    System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");
                }
                //Delete existing file
                if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip" + "/" + FileName))
                {
                    System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip" + "/" + FileName);
                }
                using (ZipFile zip = new ZipFile())
                {
                    FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip", FileNamewithoutextention + ".xlsx"), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                    workbook.SaveAs(xfile);
                    xfile.Close();
                    //tb.TableName = "Tmp";
                    //zip.AddEntry("Temp.xml", (name, stream) => tb.WriteXml(stream));
                    zip.AddFile(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip", FileNamewithoutextention + ".xlsx"), "");
                    zip.Save(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip", FileName));
                    System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip", FileNamewithoutextention + ".xlsx"));

                }
                //workbook.SaveAs(FileName);

            }
            catch (Exception ex)
            { }
        }

        public static DataTable GetDdatainDataTable(string Query)
        {
            DataTable tb = new DataTable();

            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            //The Ado.Net code ends here
            return tb;
        }

        public static Boolean GetSudmitableorNot(int Transactionid, string workflow, string role, int companyID)
        {
            Boolean result = false;
            string CompanyCode = string.Empty;
            string RoleId = string.Empty; ;
            int? Ordinal = 0;
            int lwcOrdinalNumber = 0;
            int workflowid = 0;
            workflowid = db.RWorkFlows.Where(x => x.RwfName == workflow).Select(x => x.Id).FirstOrDefault();
            CompanyCode = db.GCompanies.Where(x => x.Id == companyID).Select(x => x.GcCode).FirstOrDefault();
            RoleId = Convert.ToString(db.AspNetRoles.Where(x => x.Name == role && x.CompanyCode == CompanyCode).Select(x => x.Id).FirstOrDefault());
            lwcOrdinalNumber = db.LWorkFlowConfigs.Where(x => x.LwfcRoleId == RoleId && x.LwfcWorkFlowId == workflowid).Select(x => x.LwfcOrdinalNumber).FirstOrDefault();

            if (workflow == "Claims")
            {
                Ordinal = db.LClaims.Where(x => x.Id == Transactionid).Select(x => x.WFOrdinal).FirstOrDefault();
                if (lwcOrdinalNumber == Ordinal)
                    result = true;

            }
            else if (workflow == "Users")
            {
                Ordinal = db.LUsers.Where(x => x.Id == Transactionid).Select(x => x.WFOrdinal).FirstOrDefault();
                result = true;
            }
            else if (workflow == "Payees")
            {
                Ordinal = db.LPayees.Where(x => x.Id == Transactionid).Select(x => x.WFOrdinal).FirstOrDefault();
                result = true;
            }
            return result;
        }

        public static void ExecuteSPLogUserPreference(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string Config, string SessionId)
        {
            String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(strConnString);
            SqlDataAdapter sda = new SqlDataAdapter();

            var Query = "Exec dbo.SpLogUserPreference @UserId,@EntityName,@EntityItem,@ConfigType,@WFConfigId,@Config,@SessionId";
            //string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query);//conn
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.AddWithValue("@EntityName", string.IsNullOrEmpty(EntityName) ? (object)System.DBNull.Value : (object)EntityName);
            cmd.Parameters.AddWithValue("@EntityItem", string.IsNullOrEmpty(EntityItem) ? (object)System.DBNull.Value : (object)EntityItem);
            cmd.Parameters.AddWithValue("@ConfigType", string.IsNullOrEmpty(ConfigType) ? (object)System.DBNull.Value : (object)ConfigType);
            cmd.Parameters.AddWithValue("@WFConfigId", string.IsNullOrEmpty(WFConfigId) ? (object)System.DBNull.Value : (object)WFConfigId);
            cmd.Parameters.AddWithValue("@Config", string.IsNullOrEmpty(Config) ? (object)System.DBNull.Value : (object)Config);
            cmd.Parameters.AddWithValue("@SessionId", string.IsNullOrEmpty(SessionId) ? (object)System.DBNull.Value : (object)SessionId);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            //DataSet dt = GetData(cmd);
        }

        public static DataTable GetUserPreferenceData(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string SessionId)
        {
            //var RawQuery = db.Database.SqlQuery<RChannel>("exec SPGetUserPreferenceData '" + UserId + "','"+ EntityName + "','"+ EntityItem + "', '" + ConfigType + "', '" + WFConfigId + "', '" + SessionId + "'");
            //var Task = RawQuery.ToList();
            //var ListData = Task.ToList();
            //return ListData;

            var Query = "Exec  dbo.[SPGetUserPreferenceData] @UserId,@EntityName,@EntityItem,@ConfigType,@WFConfigId,@SessionId";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.AddWithValue("@EntityName", string.IsNullOrEmpty(EntityName) ? (object)System.DBNull.Value : (object)EntityName);
            cmd.Parameters.AddWithValue("@EntityItem", string.IsNullOrEmpty(EntityItem) ? (object)System.DBNull.Value : (object)EntityItem);
            cmd.Parameters.AddWithValue("@ConfigType", string.IsNullOrEmpty(ConfigType) ? (object)System.DBNull.Value : (object)ConfigType);
            cmd.Parameters.AddWithValue("@WFConfigId", string.IsNullOrEmpty(WFConfigId) ? (object)System.DBNull.Value : (object)WFConfigId);
            cmd.Parameters.AddWithValue("@SessionId", string.IsNullOrEmpty(SessionId) ? (object)System.DBNull.Value : (object)SessionId);
            DataSet ds = GetData(cmd);
            if (ds.Tables.Count == 0)
                return null;
            return ds.Tables[0];
        }

        public static void DeleteUserPreference(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string SessionId)
        {
            String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(strConnString);
            SqlDataAdapter sda = new SqlDataAdapter();

            var Query = "Exec dbo.SpDeleteUserPreference @UserId,@EntityName,@EntityItem,@ConfigType,@WFConfigId,@SessionId";
            //string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query);//conn
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.Parameters.AddWithValue("@EntityName", string.IsNullOrEmpty(EntityName) ? (object)System.DBNull.Value : (object)EntityName);
            cmd.Parameters.AddWithValue("@EntityItem", string.IsNullOrEmpty(EntityItem) ? (object)System.DBNull.Value : (object)EntityItem);
            cmd.Parameters.AddWithValue("@ConfigType", string.IsNullOrEmpty(ConfigType) ? (object)System.DBNull.Value : (object)ConfigType);
            cmd.Parameters.AddWithValue("@WFConfigId", string.IsNullOrEmpty(WFConfigId) ? (object)System.DBNull.Value : (object)WFConfigId);
            cmd.Parameters.AddWithValue("@SessionId", string.IsNullOrEmpty(SessionId) ? (object)System.DBNull.Value : (object)SessionId);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            //DataSet dt = GetData(cmd);
        }

        public static void ExportSearchEngineFromDataTable(string Query, string CompanyCode, string LoggedInUserName, string FileName, DataTable tb)
        {
            //The below lines of code converts the data returned from api to a datatable
            //var tb = new DataTable();
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            if (tb == null)
            {
                tb = new DataTable();
                string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection conn = new SqlConnection(ConnectionString);
                SqlCommand cmd = new SqlCommand(Query, conn);
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(tb);
                conn.Close();
                //The Ado.Net code ends here
            }

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < tb.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = tb.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
                //GC.Collect();//RK11102018 commented to improve performance
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
            //Create UserName Folder
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/SearchEngine"))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/SearchEngine");
            }
            //Delete existing file
            if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/SearchEngine" + "/" + FileName))
            {
                System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/SearchEngine" + "/" + FileName);
            }
            //Get fileName without datetimestamp
            var FileNameArray = FileName.Split('_');
            var FileNameInsideZip = FileNameArray[0];
            // using (ZipFile zip = new ZipFile())
            //{
            FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/SearchEngine", FileNameInsideZip + ".xlsx"), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            //tb.TableName = "Tmp";
            //zip.AddEntry("Temp.xml", (name, stream) => tb.WriteXml(stream));
            xfile.Close();
        }

        public static void ExportExceptionFromDataTable(string Query, string CompanyCode, string LoggedInUserName, string FileName, DataTable tb)
        {
            //The below lines of code converts the data returned from api to a datatable
            //var tb = new DataTable();
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            if (tb == null)
            {
                tb = new DataTable();
                string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                SqlConnection conn = new SqlConnection(ConnectionString);
                SqlCommand cmd = new SqlCommand(Query, conn);
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(tb);
                conn.Close();
                //The Ado.Net code ends here
            }

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < tb.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = tb.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
                //GC.Collect();//RK11102018 commented to improve performance
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
            //Create UserName Folder
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/Exception"))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/Exception");
            }
            //Delete existing file
            if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/Exception" + "/" + FileName))
            {
                System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/Exception" + "/" + FileName);
            }
            //Get fileName without datetimestamp
            var FileNameArray = FileName.Split('_');
            var FileNameInsideZip = FileNameArray[0];
            // using (ZipFile zip = new ZipFile())
            //{
            FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/Exception", FileNameInsideZip + ".xlsx"), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            //tb.TableName = "Tmp";
            //zip.AddEntry("Temp.xml", (name, stream) => tb.WriteXml(stream));
            xfile.Close();
        }

        public static void GiveAllPortfolios(string EmailID,string EntityType, int RoleId)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();

            try
            {

                SqlCommand command = new SqlCommand("USP_GiveAllPortfolios", cn);
                command.Parameters.AddWithValue("@EmailID", EmailID);//sending null parameter
                command.Parameters.AddWithValue("@EntityType", EntityType);//sending null parameter
                command.Parameters.AddWithValue("@Role", RoleId);//sending null parameter
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();

                cn.Close();
            }
            catch (Exception ex)
            {              

            }
        }



    }



}
public class AuthenticationResult
{
    public AuthenticationResult(string errorMessage = null)
    {
        ErrorMessage = errorMessage;
    }

    public String ErrorMessage { get; private set; }
    public Boolean IsSuccess => String.IsNullOrEmpty(ErrorMessage);
}



public class RandomPassword
{
    // Define default min and max password lengths.
    private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
    private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

    // Define supported password characters divided into groups.
    // You can add (or remove) characters to (from) these groups.
    private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnpqrstwxyz";
    private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
    private static string PASSWORD_CHARS_NUMERIC = "23456789";
    //private static string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";
    public string Generate()
    {
        return Generate(DEFAULT_MIN_PASSWORD_LENGTH,
                        DEFAULT_MAX_PASSWORD_LENGTH);
    }
    public string Generate(int length)
    {
        return Generate(length, length);
    }
    public string Generate(int minLength,
                                  int maxLength)
    {
        // Make sure that input parameters are valid.
        if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
            return null;

        // Create a local array containing supported password characters
        // grouped by types. You can remove character groups from this
        // array, but doing so will weaken the password strength.
        char[][] charGroups = new char[][]
        {
            PASSWORD_CHARS_LCASE.ToCharArray(),
            PASSWORD_CHARS_UCASE.ToCharArray(),
            PASSWORD_CHARS_NUMERIC.ToCharArray(),
            //PASSWORD_CHARS_SPECIAL.ToCharArray()
        };

        // Use this array to track the number of unused characters in each
        // character group.
        int[] charsLeftInGroup = new int[charGroups.Length];

        // Initially, all characters in each group are not used.
        for (int i = 0; i < charsLeftInGroup.Length; i++)
            charsLeftInGroup[i] = charGroups[i].Length;

        // Use this array to track (iterate through) unused character groups.
        int[] leftGroupsOrder = new int[charGroups.Length];

        // Initially, all character groups are not used.
        for (int i = 0; i < leftGroupsOrder.Length; i++)
            leftGroupsOrder[i] = i;

        // Because we cannot use the default randomizer, which is based on the
        // current time (it will produce the same "random" number within a
        // second), we will use a random number generator to seed the
        // randomizer.

        // Use a 4-byte array to fill it with random bytes and convert it then
        // to an integer value.
        byte[] randomBytes = new byte[4];

        // Generate 4 random bytes.
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        rng.GetBytes(randomBytes);

        // Convert 4 bytes into a 32-bit integer value.
        int seed = BitConverter.ToInt32(randomBytes, 0);

        // Now, this is real randomization.
        Random random = new Random(seed);

        // This array will hold password characters.
        char[] password = null;

        // Allocate appropriate memory for the password.
        if (minLength < maxLength)
            password = new char[random.Next(minLength, maxLength + 1)];
        else
            password = new char[minLength];

        // Index of the next character to be added to password.
        int nextCharIdx;

        // Index of the next character group to be processed.
        int nextGroupIdx;

        // Index which will be used to track not processed character groups.
        int nextLeftGroupsOrderIdx;

        // Index of the last non-processed character in a group.
        int lastCharIdx;

        // Index of the last non-processed group.
        int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

        // Generate password characters one at a time.
        for (int i = 0; i < password.Length; i++)
        {
            // If only one character group remained unprocessed, process it;
            // otherwise, pick a random character group from the unprocessed
            // group list. To allow a special character to appear in the
            // first position, increment the second parameter of the Next
            // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
            if (lastLeftGroupsOrderIdx == 0)
                nextLeftGroupsOrderIdx = 0;
            else
                nextLeftGroupsOrderIdx = random.Next(0,
                                                     lastLeftGroupsOrderIdx);

            // Get the actual index of the character group, from which we will
            // pick the next character.
            nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

            // Get the index of the last unprocessed characters in this group.
            lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

            // If only one unprocessed character is left, pick it; otherwise,
            // get a random character from the unused character list.
            if (lastCharIdx == 0)
                nextCharIdx = 0;
            else
                nextCharIdx = random.Next(0, lastCharIdx + 1);

            // Add this character to the password.
            password[i] = charGroups[nextGroupIdx][nextCharIdx];

            // If we processed the last character in this group, start over.
            if (lastCharIdx == 0)
                charsLeftInGroup[nextGroupIdx] =
                                          charGroups[nextGroupIdx].Length;
            // There are more unprocessed characters left.
            else
            {
                // Swap processed character with the last unprocessed character
                // so that we don't pick it until we process all characters in
                // this group.
                if (lastCharIdx != nextCharIdx)
                {
                    char temp = charGroups[nextGroupIdx][lastCharIdx];
                    charGroups[nextGroupIdx][lastCharIdx] =
                                charGroups[nextGroupIdx][nextCharIdx];
                    charGroups[nextGroupIdx][nextCharIdx] = temp;
                }
                // Decrement the number of unprocessed characters in
                // this group.
                charsLeftInGroup[nextGroupIdx]--;
            }

            // If we processed the last group, start all over.
            if (lastLeftGroupsOrderIdx == 0)
                lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
            // There are more unprocessed groups left.
            else
            {
                // Swap processed group with the last unprocessed group
                // so that we don't pick it until we process all groups.
                if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                {
                    int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                    leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                leftGroupsOrder[nextLeftGroupsOrderIdx];
                    leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                }
                // Decrement the number of unprocessed groups.
                lastLeftGroupsOrderIdx--;
            }
        }

        // Convert password characters into a string and return the result.
        //SS Adding # at the end
        String Password = new string(password);
        //return Password+"#";
        return Password;
    }
}