//Code review and fix SQL to LINQ completed for necessary methods. Didn't touch methods that are not pron for SQL injection.

using System;
//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Threading.Tasks;
using System.Web.Http;
//using System.Web.Http.Description;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
using System.Reflection.Emit; //For creation of dynamic viewmodel
using System.Reflection; //For creation of dynamic viewmodel
using System.Configuration;
//using CsvHelper;
using System.IO;
using System.Text;
using System.Collections.Generic;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LRawDataController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        [HttpGet]
        public IHttpActionResult DownloadLRawDataFile(int SOSBatchNumber,int RawDataTableId,int CompanyId,string LoggedInUserName)
        {
            var Company = db.GCompanies.Find(CompanyId);
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            //Create a csv in S drive for the Calculations
            //var CfileLocation = ConfigurationManager.AppSettings["CalcDocumentPath"] + "/" + Company.GcCode +"/"+ LoggedInUserName + "/ExportRawDataFile.csv";
            var FilesPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + Company.GcCode + "/" + LoggedInUserName+"/forzip";
            string RawDataFileName = "ExportRawDataFile_*.*";
           // string RawDataFileName = "ExportRawDataFile_"+BatchDetails.LbCommissionPeriod + "_" + BatchDetails.LbBatchName+ "*.*";
            var RawDataTable = db.LRawDataTables.Where(p => p.Id == RawDataTableId).FirstOrDefault();
            if (System.IO.Directory.Exists(FilesPath))
            {
                try
                {
                    var RawDataFileList = Directory.GetFiles(FilesPath, RawDataFileName, SearchOption.AllDirectories).ToList();
                    foreach (var RawDataFile in RawDataFileList)
                    {
                        if (System.IO.File.Exists(RawDataFile))//delete already existing file
                            System.IO.File.Delete(RawDataFile);
                    }
                }
                catch
                {
                    //do nothing if there is error in searching or deleting files in cloudberry
                }
            }

            //Create dynamic type so that the same can be used to get output of the query (Qry) in the segment following to this section//
           // TypeBuilder builder = CreateTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");

            //Get a list of labels defined in LCompanySpecificRawDataColumns table and add each of them as a property in the newly created TypeBuilder
            var ColListForNewType = db.LCompanySpecificRawDataColumns.Where(p => p.LcsrdcRawDataTableId == RawDataTableId).Select(p => new {p.LcsrdcDisplayLabel, p.LcsrdcDataType, p.LcsrdcOrdinalPosition }).OrderBy(p => p.LcsrdcOrdinalPosition).ToList();
            //var ColumnList = db.Database.SqlQuery<string>(ColumnQuery).ToList();
           if(ColListForNewType.Count()==0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No Raw Data column mapping found. Please contact L2 Support to fix"));
            }
            //when there are no records, just download empty file
            //If No record found in RawData
            /* try
            {
                var CountQuery = "SELECT COUNT(*) as RecordCount FROM {Schema}." + RawDataTable.LrdtName + " WHERE XBatchNumber =" + SOSBatchNumber + " and XPeriodCode=" + BatchDetails.LbPeriodCode;
                var RecordCount = Globals.GetQueryResultFromOpcoDB(CompanyId, CountQuery, BatchDetails.LbCommissionPeriod);// db.Database.SqlQuery<int>("SELECT COUNT(*) FROM LRawData WHERE LrdSOSBatchNumber = {0}", SOSBatchNumber).FirstOrDefault();

                if (RecordCount.Rows[0].Field<int>("RecordCount") == 0)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No records available for this batch !!"));
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Table is missing for this batch !!"));
            }
            */
            //"For XML path('')" in this query is used to convert sql query result set into a single string.
            string ColumnQuerySingleRow = "select stuff((select '[' + [LcsrdcXColumnName] + '] AS [' +  [LcsrdcDisplayLabel] +'], ' FROM LCompanySpecificRawDataColumns WHERE LcsrdcRawDataTableId = " + RawDataTableId + " Order by LcsrdcOrdinalPosition FOR XML PATH('') ),1,0,'')";
            var ColumnListSingleRow = db.Database.SqlQuery<string>(ColumnQuerySingleRow).ToList().ElementAt(0).Trim() ;
            string Qry = "";
            if (RawDataTable != null)
            {
                //Using the column list obtained above, and other parameters passed in the method create a SQL query to fatch the data from database
                Qry = "Select  " + ColumnListSingleRow.Remove(ColumnListSingleRow.Length - 1) + "  ";
                Qry = Qry + " FROM  {Schema}."+RawDataTable.LrdtName;
                Qry = Qry + " Where XBatchNumber = " + SOSBatchNumber + " and XPeriodCode=" + BatchDetails.LbPeriodCode; 
            }
            //var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            // var result = db.Database.SqlQuery(resultType,Qry);
            //method to get query result and save it as an excel file in S drive
           // var FileName = "ExportRawDataFile_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss")+ ".zip";
           // var FileNamewithoutextention = "ExportRawDataFile_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") ;
            var FileName = "ExportRawDataFile_"+ BatchDetails .LbCommissionPeriod+ "_" + BatchDetails .LbBatchName+"_"+ DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".zip";
            var FileNamewithoutextention = "ExportRawDataFile_"+ BatchDetails.LbCommissionPeriod + "_" + BatchDetails.LbBatchName + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss");
            //Added in R2.8 remove the special character from file.
            FileName = FileName.Replace(":", "");
            FileNamewithoutextention = FileNamewithoutextention.Replace(":", "");
            //End
            if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
            {
               Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LRawData", "DownloadLRawDataFile", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
            }
            else
            {
                var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, BatchDetails.LbCommissionPeriod);
                Globals.CreateExcelFromDataTable(xx, FileName, "", Company.GcCode, LoggedInUserName, FileNamewithoutextention);
               // Globals.ExportZipFromDataTable(null, Company.GcCode, LoggedInUserName, FileName, xx);
            }
            //Add the result obtained from a query into a csv file

            //if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["CalcDocumentPath"] + "/" + Company.GcCode + "/" + LoggedInUserName))//create directory if not present
            //    {
            //        System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["CalcDocumentPath"] + "/" + Company.GcCode + "/" + LoggedInUserName);
            //    }
            //    using (var CTextWriter = new StreamWriter(CfileLocation))
            //    using (var Csv = new CsvWriter(CTextWriter))
            //    {

            //    //foreach (DataColumn column in tb.Columns)
            //    //{
            //    //   // var Col = RemoveSpecialCharacters(column.ColumnName);
            //    //    Csv.WriteField(column.ColumnName);
            //    //}
            //    //Csv.WriteRecords(tb.Columns);
            //    //Csv.NextRecord();
            //    Csv.Configuration.Encoding = Encoding.UTF8;
            //    Csv.WriteRecords(result);
            //        //foreach (DataRow row in tb.Rows)
            //        //{
            //        //    for (var i = 0; i < tb.Columns.Count; i++)
            //        //    {
            //        //       // var rows =RemoveSpecialCharacters(row[i] as string);
            //        //        Csv.WriteField(row[i]);
            //        //    }
            //        //    Csv.NextRecord();
            //        //}
            //    }
            return Ok(FileName);
        }

        public IHttpActionResult GetRawDataCounts(int CompanyId, int SOSBatchNumber)
        {
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            var RawDataTable = db.LRawDataTables.Where(p => p.Id == BatchDetails.LbRawDataTableId).FirstOrDefault();
            var ColListForNewType = db.LCompanySpecificRawDataColumns.Where(p => p.LcsrdcRawDataTableId == RawDataTable.Id).Select(p => p.LcsrdcXColumnName).ToList(); 
            //Adding a check such that if any column mapped in companyspecific rawdata columns is not present in rawdatatable then we will throw an error to user screen.
            var RawDataColumnsQuery = "select COLUMN_NAME as ColName from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" + RawDataTable.LrdtName + "' and TABLE_SCHEMA='{Schema}'";
            var RawDataColumns = Globals.GetQueryResultFromOpcoDB(CompanyId, RawDataColumnsQuery, BatchDetails.LbCommissionPeriod);
            //SS14July2018:- if any column mismatch is found in column mapping and rawdata columns throw error
            //var MatchedColumns = RawDataColumns.AsEnumerable();
            //var zz= MatchedColumns.Where(p => ColListForNewType.Contains(p.Field<string>("ColName"),StringComparer.OrdinalIgnoreCase)).Count();
            if (ColListForNewType.Count() == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No Raw Data column mapping found. Please contact L2 Support to fix"));
            }
            string UnMatchedColumns="";
            foreach(var column in ColListForNewType)
            {
                var result = from r in RawDataColumns.AsEnumerable()
                             where r.Field<string>("ColName") == column
                             select r;
                if (result.Count() == 0)
                    UnMatchedColumns += column + ",";
            }

            if (!string.IsNullOrEmpty(UnMatchedColumns)||!ColListForNewType.Contains("XBatchNumber") || !ColListForNewType.Contains("XPeriodCode") || !ColListForNewType.Contains("XStatus"))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "RawData columns does not match the mapping or the RawData table does not exist in Comm DB. Please contact L2 Support to fix"));
            }
            
            //if (yy.Count() > 0)
            //{
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No Raw Data column mapping found for (" + string.Join(",", yy) + "). Please contact L2 Support to fix"));
            //}
            //Get the status wise raw data counts from database
            string Qry = "Select isnull(XStatus,'NA') AS Status, Count(*) as RowCounts from {Schema}."+RawDataTable.LrdtName+" Where XBatchNumber = " + SOSBatchNumber + " and XPeriodCode=" + BatchDetails.LbPeriodCode + " Group by XStatus";
            Qry = Qry + " Order By XStatus";

            //Execute the query and return the result 
            //var xx = db.Database.SqlQuery<LRawDataRowCountsViewModel>(Qry).ToList();
            if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
            {
               Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LRawData", "GetRawDataCounts", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
            }
            else
            {
                var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, BatchDetails.LbCommissionPeriod);
                return Ok(xx);
            }
            return Ok();
        }

        public IHttpActionResult GetLRawDataForGrid(int CompanyId, int SOSBatchNumber, int PageNumber, int PageSize, int RawDataTableId, string Status, string FilterQuery)
        {
            //Create dynamic type so that the same can be used to get output of the query (Qry) in the segment following to this section
            TypeBuilder builder = CreateTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            //Get a list of labels defined in LCompanySpecificRawDataColumns table and add each of them as a property in the newly created TypeBuilder
            var ColListForNewType = db.LCompanySpecificRawDataColumns.Where(p => p.LcsrdcRawDataTableId == RawDataTableId).Select(p => new {  p.LcsrdcDataType, p.LcsrdcOrdinalPosition,p.LcsrdcXColumnName }).OrderBy(p => p.LcsrdcOrdinalPosition).ToList();
            //var ColumnList = db.Database.SqlQuery<string>(ColumnQuery).ToList();
            if (ColListForNewType.Count() == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No Raw Data column mapping found. Please contact L2 Support to fix"));
            }
            //Add Obivious columns (these columns are not available in the ColListForNewType)
           // CreateAutoImplementedProperty(builder, "Id", typeof(int));
          //  CreateAutoImplementedProperty(builder, "LrdExclusionComments", typeof(string));


            //Add dynamic columns by looping through ColListForNewType and getting column details from there
            foreach (var item in ColListForNewType)
            {
                //switch (item.LcsrdcDataType)
                //{
                //    case "varchar":
                //    case "nvarchar":
                //        CreateAutoImplementedProperty(builder, item.LcsrdcLColumnName.ToString(), typeof(string));
                //        break;
                //    case "int":
                //        CreateAutoImplementedProperty(builder, item.LcsrdcLColumnName.ToString(), typeof(int?));
                //        break;
                //    case "date":
                //        CreateAutoImplementedProperty(builder, item.LcsrdcLColumnName.ToString(), typeof(DateTime?));
                //        break;
                //    case "datetime":
                //        CreateAutoImplementedProperty(builder, item.LcsrdcLColumnName.ToString(), typeof(DateTime?));
                //        break;
                //    case "bit":
                //        CreateAutoImplementedProperty(builder, item.LcsrdcLColumnName.ToString(), typeof(bool?));
                //        break;
                //    case "bigint":
                //        CreateAutoImplementedProperty(builder, item.LcsrdcLColumnName.ToString(), typeof(Int64?));
                //        break;
                //    case "decimal":
                //        CreateAutoImplementedProperty(builder, item.LcsrdcLColumnName.ToString(), typeof(decimal?));
                //        break;
                //    case "float":
                //        CreateAutoImplementedProperty(builder, item.LcsrdcLColumnName.ToString(), typeof(double?));
                //        break;
                //    case "numeric":
                //        CreateAutoImplementedProperty(builder, item.LcsrdcLColumnName.ToString(), typeof(double?));
                //        break;
                //}
            }

            Type resultType = builder.CreateType();
            var RawDataTable = db.LRawDataTables.Find(RawDataTableId);
            //"For XML path('')" in this query is used to convert sql query result set into a single string.
            string ColumnQuerySingleRow = "select stuff((select '[' + [LcsrdcXColumnName] + '] AS [' +  [LcsrdcXColumnName] +'], ' FROM LCompanySpecificRawDataColumns WHERE LcsrdcRawDataTableId = " + RawDataTableId + " Order by LcsrdcOrdinalPosition FOR XML PATH('') ),1,0,'')";
            var ColumnListSingleRow = db.Database.SqlQuery<string>(ColumnQuerySingleRow).ToList();

            ////Using the column list obtained above, and other parameters passed in the method create a SQL query to fatch the data from database
            //string Qry = "Select top 5 Id, " + ColumnListSingleRow.ElementAt(0).ToString() + " LrdCompanyId ";
            //Qry = Qry + "From LRawData Where LrdCompanyId = " + CompanyId + " And LrdStatus = '" + Status + "' And LrdSOSBatchNumber = " + SOSBatchNumber + " ";
            //Qry = Qry + "Order By LrdAlteryxTransactionNumber";

            //Using the column list obtained above, and other parameters passed in the method create a SQL query to fatch the data from database
            string Qry = "Select * From (Select  " + ColumnListSingleRow.ElementAt(0).ToString() + " ";
            Qry = Qry + "ROW_NUMBER() OVER (ORDER BY XTransactionNumber) as row FROM {Schema}."+RawDataTable.LrdtName;
            if (Status == "NA")
            {
                Qry = Qry + " Where  (XStatus = '" + Status + "' or XStatus is null ) "+ FilterQuery;
            }
            else
            {
                Qry = Qry + " Where  XStatus = '" + Status + "' " + FilterQuery;
            }
            Qry = Qry + "And XBatchNumber = " + SOSBatchNumber + " and XPeriodCode=" + BatchDetails.LbPeriodCode + ") a ";
            Qry = Qry + " Where row > " + PageNumber * PageSize + " And row <= " + (PageNumber + 1) * PageSize;
            Qry = Qry.Replace("XStatus,", "isnull(XStatus,'NA')");//Converting XStatus=null to NA because in the rawdata grid the data tab shows all records where status=NA

            if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
            {
               Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LRawData", "GetLRawDataForGrid", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
            }
            else
            {
                var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, BatchDetails.LbCommissionPeriod);
                //Execute the query and return the result 
                //dynamic xx = db.Database.SqlQuery(resultType,Qry);
                return Ok(xx);
            }
            return Ok();
        }

        ////method to get raw data from opco wise db got from LDatabases
        //public IHttpActionResult GetRawDataCountsForOpcoDB(int CompanyId, int SOSBatchNumber)
        //{
        //    var CompanyDetails = db.GCompanies.Find(CompanyId);
        //    var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
        //    var RawDataDetails = db.LRawDataTables.Find(BatchDetails.LbRawDataTableId);

        //    //Get the details of the database which is to be queried to obtain RawData for the given batch number
        //    var DatabaseDetails = db.LDatabases.Where(p => p.CompanyId == CompanyId).Where(p => p.EffectiveStartDate <= DateTime.UtcNow && p.EffectiveEndDate > DateTime.UtcNow).FirstOrDefault();
        //    string ConnectionString = "data source={HostName};initial catalog={DataBaseName};persist security info=True;user id={DatabaseUser};password={Password};MultipleActiveResultSets=True;Connect Timeout=60";
        //    //Replace the credentials required to frame connection string from LDatabases
        //    if (DatabaseDetails != null)
        //    {
        //        ConnectionString = ConnectionString.Replace("{HostName}", DatabaseDetails.HostName);
        //        ConnectionString = ConnectionString.Replace("{DataBaseName}", DatabaseDetails.DatabaseName);
        //        ConnectionString = ConnectionString.Replace("{DatabaseUser}", DatabaseDetails.LoginId);
        //        ConnectionString = ConnectionString.Replace("{Password}", DatabaseDetails.Password);
        //    }

        //    var RawDataTableNameWithSchema = DatabaseDetails.SchemaName + "." + RawDataDetails.LrdtName;
        //    //Get the status wise raw data counts from database
        //    string Qry = "Select XStatus AS Status, Count(*) as RowCounts from "+RawDataTableNameWithSchema+" Where XBatchNumber = " + SOSBatchNumber + " Group by XStatus";
        //    Qry = Qry + " Order By XStatus";


        //    SqlConnection cn = new SqlConnection();
        //    cn.ConnectionString = ConnectionString;//"data source=euitdaards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=RawData;persist security info=True;user id=euitdaadbmuser;password=KewReN8=w&Mabu;MultipleActiveResultSets=True;Connect Timeout=60";
        //    //SqlCommand cmd = new SqlCommand(Qry, cn);
        //    DataTable dt = new DataTable { TableName = "MyTableName" };
        //    SqlCommand cmd = new SqlCommand(Qry, cn);
        //    cn.Open();
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    da.Fill(dt);
        //    cn.Close();
        //    var RawDataCountsList = new List<LRawDataRowCountsViewModel>();
        //    //Execute the query and return the result 
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        var xx = new LRawDataRowCountsViewModel { Status = dr.Field<string>("Status"), RowCounts = dr.Field<int>("RowCounts") };
        //        RawDataCountsList.Add(xx);
        //    }
        //    //These lines are added by Shubham to remove error generated in view in case of any of the three types are not found in database.
        //    //will replace once better method is found
        //    if (RawDataCountsList.Where(p=>p.Status== "NA").Count()==0)
        //    {
        //        RawDataCountsList.Add(new LRawDataRowCountsViewModel { Status = "NA", RowCounts = 0 });
        //    }
        //    if (RawDataCountsList.Where(p => p.Status == "Exclusion").Count() == 0)
        //    {
        //        RawDataCountsList.Add(new LRawDataRowCountsViewModel { Status = "Exclusion", RowCounts = 0 });
        //    }
        //    if (RawDataCountsList.Where(p => p.Status == "Error").Count() == 0)
        //    {
        //        RawDataCountsList.Add(new LRawDataRowCountsViewModel { Status = "Error", RowCounts = 0 });
        //    }
        //    return Ok(RawDataCountsList);
        //}

        //public IHttpActionResult GetRawDataFromOpCoDBForGrid(int CompanyId, int SOSBatchNumber, int PageNumber, int PageSize, int RawDataTableId, string Status)
        //{
        //    //Create dynamic type so that the same can be used to get output of the query (Qry) in the segment following to this section
        //    TypeBuilder builder = CreateTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");

        //    //Get a list of labels defined in LCompanySpecificRawDataColumns table and add each of them as a property in the newly created TypeBuilder
        //    //var ColListForNewType = db.LCompanySpecificRawDataColumns.Where(p => p.LcsrdcRawDataTableId == RawDataTableId).Select(p => new { p.LcsrdcLColumnName, p.LcsrdcDataType, p.LcsrdcOrdinalPosition }).OrderBy(p => p.LcsrdcOrdinalPosition).ToList();
            
        //    //Above one line is original for fatching column names of LRawDataTable, now since we want to gatch data from OpCo db hence instead of L column we will fetch X column
        //    var ColListForNewType = db.LCompanySpecificRawDataColumns.Where(p => p.LcsrdcRawDataTableId == RawDataTableId).Select(p => new { p.LcsrdcXColumnName, p.LcsrdcDataType, p.LcsrdcOrdinalPosition }).OrderBy(p => p.LcsrdcOrdinalPosition).ToList();
        //    var CompanyDetails = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
        //    //var ColumnList = db.Database.SqlQuery<string>(ColumnQuery).ToList();
        //    if (ColListForNewType.Count() == 0)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No Raw Data column mapping found. Please contact L2 Support to fix"));
        //    }
        //    //Add Obivious columns (these columns are not available in the ColListForNewType)
        //    CreateAutoImplementedProperty(builder, "Id", typeof(int));
        //    CreateAutoImplementedProperty(builder, "LrdExclusionComments", typeof(string));


        //    //Add dynamic columns by looping through ColListForNewType and getting column details from there
        //    foreach (var item in ColListForNewType)
        //    {
        //        switch (item.LcsrdcDataType)
        //        {
        //            case "varchar":
        //            case "nvarchar":
        //                CreateAutoImplementedProperty(builder, item.LcsrdcXColumnName.ToString(), typeof(string));
        //                break;
        //            case "int":
        //                CreateAutoImplementedProperty(builder, item.LcsrdcXColumnName.ToString(), typeof(int?));
        //                break;
        //            case "date":
        //                CreateAutoImplementedProperty(builder, item.LcsrdcXColumnName.ToString(), typeof(DateTime?));
        //                break;
        //            case "datetime":
        //                CreateAutoImplementedProperty(builder, item.LcsrdcXColumnName.ToString(), typeof(DateTime?));
        //                break;
        //            case "bit":
        //                CreateAutoImplementedProperty(builder, item.LcsrdcXColumnName.ToString(), typeof(bool?));
        //                break;
        //            case "bigint":
        //                CreateAutoImplementedProperty(builder, item.LcsrdcXColumnName.ToString(), typeof(Int64?));
        //                break;
        //            case "decimal":
        //                CreateAutoImplementedProperty(builder, item.LcsrdcXColumnName.ToString(), typeof(decimal?));
        //                break;
        //            case "float":
        //                CreateAutoImplementedProperty(builder, item.LcsrdcXColumnName.ToString(), typeof(double?));
        //                break;
        //            case "numeric":
        //                CreateAutoImplementedProperty(builder, item.LcsrdcXColumnName.ToString(), typeof(double?));
        //                break;
        //        }
        //    }

        //    Type resultType = builder.CreateType();

        //    //"For XML path('')" in this query is used to convert sql query result set into a single string.
        //    string ColumnQuerySingleRow = "select stuff((select '[' + [LcsrdcXColumnName] + '] AS [' +  [LcsrdcXColumnName] +'], ' FROM LCompanySpecificRawDataColumns WHERE LcsrdcRawDataTableId = " + RawDataTableId + " Order by LcsrdcOrdinalPosition FOR XML PATH('') ),1,0,'')";
        //    var ColumnListSingleRow = db.Database.SqlQuery<string>(ColumnQuerySingleRow).ToList();

        //    //Identify the name of raw data table (Code here after POC) For the thime being table name is hardcoded in below query
        //    //var RawDataDbDetails = db.LCompanySpecificRawDataColumns.Where(p => p.LcsrdcRawDataTableId == RawDataTableId).Select(p => new { p.LcsrdcXColumnName, p.LcsrdcDataType, p.LcsrdcOrdinalPosition }).OrderBy(p => p.LcsrdcOrdinalPosition).ToList();
        //    var RawDataDetails = db.LRawDataTables.Find(RawDataTableId);
        //    var RawDataTableNameWithSchema = "XSchema" + CompanyDetails.GcCode+"."+RawDataDetails.LrdtName;

        //    String Qry = "SELECT " + ColumnListSingleRow.ElementAt(0).ToString() + "[MSISDN] FROM "+RawDataTableNameWithSchema;
        //    Qry = Qry + " Where XBatchNumber = " + SOSBatchNumber + " AND XTransactionNumber BETWEEN 1 AND 1000";

        //    //Get the details of the database which is to be queried to obtain RawData for the given batch number
        //    var DatabaseDetails = db.LDatabases.Where(p => p.CompanyId == CompanyId).Where(p => p.EffectiveStartDate <= DateTime.UtcNow && p.EffectiveEndDate > DateTime.UtcNow).FirstOrDefault();
        //    string ConnectionString = "data source={HostName};initial catalog={DataBaseName};persist security info=True;user id={DatabaseUser};password={Password};MultipleActiveResultSets=True;Connect Timeout=60";
        //    //Replace the credentials required to frame connection string from LDatabases
        //    if (DatabaseDetails != null)
        //    {
        //      ConnectionString=  ConnectionString.Replace("{HostName}", DatabaseDetails.HostName);
        //        ConnectionString = ConnectionString.Replace("{DataBaseName}", DatabaseDetails.DatabaseName);
        //        ConnectionString = ConnectionString.Replace("{DatabaseUser}", DatabaseDetails.LoginId);
        //        ConnectionString = ConnectionString.Replace("{Password}", DatabaseDetails.Password);
        //    }

        //        SqlConnection cn = new SqlConnection();
        //    cn.ConnectionString = ConnectionString;//"data source=euitdaards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=RawData;persist security info=True;user id=euitdaadbmuser;password=KewReN8=w&Mabu;MultipleActiveResultSets=True;Connect Timeout=60";
        //    //SqlCommand cmd = new SqlCommand(Qry, cn);
        //    DataTable dt = new DataTable { TableName = "MyTableName" };
        //    SqlCommand cmd = new SqlCommand(Qry, cn);
        //    cn.Open();
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    da.Fill(dt);
        //    cn.Close();


        //    //Execute the query and return the result 
        //    //dynamic xx = db.Database.SqlQuery(resultType, Qry);
        //    dynamic xx = dt;
        //    return Ok(xx);
        //}


        //Update Raw Data Status
        public IHttpActionResult UpdateRawDataStatus(int Id,string Comments,string Status,int CompanyId,int RawDataTableId,int SOSBatchNumber)
        {
            // var RawData = db.LRawDatas.Find(Id);
            //Below if statement for for inserting NULL is not a preferred way of doing it. Actually the comment should come
            //as null from calling method, but due to time constraint coded this workaround. Need to fix it lateron. 
            //if (Comments.IndexOf("{Comments}", StringComparison.OrdinalIgnoreCase) >= 0)
            //    RawData.LrdExclusionComments = null;
            //else
            //    RawData.LrdExclusionComments = Comments;
            //RawData.LrdStatus = Status;
            //db.Entry(RawData).State = EntityState.Modified;
            //db.SaveChanges();
            var RawDataTable = db.LRawDataTables.Where(p => p.Id == RawDataTableId).FirstOrDefault();
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
           
            if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
            {
               Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LRawData", "UpdateRawDataStatus", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
            }
            else
            {
                var Query = "";
                if (string.IsNullOrEmpty(Status))
                {
                    Query = " update {Schema}." + RawDataTable.LrdtName + " set XStatus=NULL,XExclusionComments='" + Comments + "' where XTransactionNumber=" + Id+" and XBatchNumber="+BatchDetails.LbBatchNumber;

                }
                else
                {
                    Query = " update {Schema}." + RawDataTable.LrdtName + " set XStatus='" + Status + "',XExclusionComments='" + Comments + "' where XTransactionNumber=" + Id + " and XBatchNumber=" + BatchDetails.LbBatchNumber;
                }
                Globals.RunUpdateQueryInOpcoDB(CompanyId, Query, BatchDetails.LbCommissionPeriod);
            }
            return Ok();
        }

        // GET: api/LRawDatas?CompanyId=5
        //making changes in data sent to reduce time to load and take 100 records at a time shubham. method to get excluded raw data
        ////A stands for Analyst Excluded the data from Raw data list
        //public IHttpActionResult GetLRawExcludedDatasByCompanyId(int CompanyId, int BatchNumber, int PageNumber)
        //{
        //    var rowCount = PageNumber * 100; ;
        //    var yy = db.LRawDatas.Where(p => p.LrdCompanyId == CompanyId).Where(p => p.LrdStatus == "Exclusion").Where(p => p.LrdSOSBatchNumber == BatchNumber);
        //    var xx = (from aa in yy
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.LrdCompanyId,
        //                  aa.LrdBrandName,
        //                  aa.LrdActivityTypeName,
        //                  aa.LrdCommissionTypeName,
        //                  aa.LrdDeviceTypeName,
        //                  aa.LrdPayeeCode,
        //                  aa.LrdConnectionDate,
        //                  aa.LrdOrderDate,
        //                  aa.LrdMSISDN,
        //                  aa.LrdBAN,
        //                  aa.LrdOrderNumber,
        //                  aa.LrdCustomerName,
        //                  aa.LrdProductCode,
        //                  aa.LrdCommissionAmount,
        //                  aa.LrdPayeeComment,
        //                  aa.LrdIMEI,
        //                  aa.GCompany.GcCompanyName,
        //                  aa.LrdStatus,
        //                  aa.LrdExclusionComments,
        //                  Count = yy.Count(),
        //                  aa.LrdSOSBatchNumber
        //              }).OrderBy(p => p.LrdSOSBatchNumber);
        //    var LstItems = xx.Skip(Math.Max(rowCount - 100, xx.Count() - rowCount)).Take(100);
        //    return Ok(LstItems);
        //}
        
        //// GET: api/LRawDatas?CompanyId=5
        ////making changes in data sent to reduce time to load and take 100 records at a time shubham. method to get excluded raw data
        ////E stands for data with errors from Raw data list
        //public IHttpActionResult GetLRawErrorDatasByCompanyId(int CompanyId, int BatchNumber, int PageNumber)
        //{
        //    var rowCount = PageNumber * 100; ;
        //    var yy = db.LRawDatas.Where(p => p.LrdCompanyId == CompanyId).Where(p => p.LrdStatus == "Error").Where(p => p.LrdSOSBatchNumber == BatchNumber);
        //    var xx = (from aa in yy
        //              select new
        //                  {
        //                      aa.Id,
        //                      aa.LrdCompanyId,
        //                      aa.LrdBrandName,
        //                      aa.LrdActivityTypeName,
        //                      aa.LrdCommissionTypeName,
        //                      aa.LrdDeviceTypeName,
        //                      aa.LrdPayeeCode,
        //                      aa.LrdConnectionDate,
        //                      aa.LrdOrderDate,
        //                      aa.LrdMSISDN,
        //                      aa.LrdBAN,
        //                      aa.LrdOrderNumber,
        //                      aa.LrdCustomerName,
        //                      aa.LrdProductCode,
        //                      aa.LrdCommissionAmount,
        //                      aa.LrdPayeeComment,
        //                      aa.LrdIMEI,
        //                      aa.GCompany.GcCompanyName,
        //                      aa.LrdStatus,
        //                      aa.LrdSOSBatchNumber,
        //                      aa.LrdExclusionComments,
        //                      Count = yy.Count()
        //                  }).OrderBy(p => p.LrdSOSBatchNumber);
        //    var LstItems = xx.Skip(Math.Max(rowCount - 100, xx.Count() - rowCount)).Take(100);
        //    return Ok(LstItems);
        //}

       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LRawDataExists(int id)
        {
            return true;//db.LRawDatas.Count(e => e.Id == id) > 0; Raw Data table removed from SOS
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            return SqEx.Message;
        }

        private TypeBuilder CreateTypeBuilder(string assemblyName, string moduleName, string typeName)
        {
            TypeBuilder typeBuilder = AppDomain
                .CurrentDomain
                .DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName)
                .DefineType(typeName, TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }

        private void CreateAutoImplementedProperty(TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(string.Concat(PrivateFieldPrefix, propertyName), propertyType, FieldAttributes.Private);

            // Generate the property
            PropertyBuilder propertyBuilder = builder.DefineProperty(propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);

            // Property getter and setter attributes.
            MethodAttributes propertyMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(string.Concat(GetterPrefix, propertyName), propertyMethodAttributes, propertyType, Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(string.Concat(SetterPrefix, propertyName), propertyMethodAttributes, null, new Type[] { propertyType });

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            ILGenerator setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }

        [HttpGet]
        public IHttpActionResult DownloadLRawDataFileStatusWise(int SOSBatchNumber, int RawDataTableId, int CompanyId, string LoggedInUserName, string Status, string FilterQuery)
        {
            var Company = db.GCompanies.Find(CompanyId);
            var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            //Create a csv in S drive for the Calculations
            //var CfileLocation = ConfigurationManager.AppSettings["CalcDocumentPath"] + "/" + Company.GcCode +"/"+ LoggedInUserName + "/ExportRawDataFile.csv";
            var FilesPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + Company.GcCode + "/" + LoggedInUserName + "/forzip";
            string RawDataFileName = "ExportRawDataFile_*.*";
            var RawDataTable = db.LRawDataTables.Where(p => p.Id == RawDataTableId).FirstOrDefault();
            if (System.IO.Directory.Exists(FilesPath))
            {
                try
                {
                    var RawDataFileList = Directory.GetFiles(FilesPath, RawDataFileName, SearchOption.AllDirectories).ToList();
                    foreach (var RawDataFile in RawDataFileList)
                    {
                        if (System.IO.File.Exists(RawDataFile))//delete already existing file
                            System.IO.File.Delete(RawDataFile);
                    }
                }
                catch
                {
                    //do nothing if there is error in searching or deleting files in cloudberry
                }
            }

            //Create dynamic type so that the same can be used to get output of the query (Qry) in the segment following to this section//
            // TypeBuilder builder = CreateTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");

            //Get a list of labels defined in LCompanySpecificRawDataColumns table and add each of them as a property in the newly created TypeBuilder
            var ColListForNewType = db.LCompanySpecificRawDataColumns.Where(p => p.LcsrdcRawDataTableId == RawDataTableId).Select(p => new { p.LcsrdcDisplayLabel, p.LcsrdcDataType, p.LcsrdcOrdinalPosition }).OrderBy(p => p.LcsrdcOrdinalPosition).ToList();
            //var ColumnList = db.Database.SqlQuery<string>(ColumnQuery).ToList();
            if (ColListForNewType.Count() == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No Raw Data column mapping found. Please contact L2 Support to fix"));
            }
            //If No record found in RawData
            var CountQuery = "SELECT COUNT(*) as RecordCount FROM {Schema}." + RawDataTable.LrdtName + " WHERE XBatchNumber =" + SOSBatchNumber + " and XPeriodCode=" + BatchDetails.LbPeriodCode;
            var RecordCount = Globals.GetQueryResultFromOpcoDB(CompanyId, CountQuery, BatchDetails.LbCommissionPeriod);// db.Database.SqlQuery<int>("SELECT COUNT(*) FROM LRawData WHERE LrdSOSBatchNumber = {0}", SOSBatchNumber).FirstOrDefault();
            if (RecordCount.Rows[0].Field<int>("RecordCount") == 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No records available for this batch !!"));
            }
            //Add dynamic columns by looping through ColListForNewType and getting column details from there
            //foreach (var item in ColListForNewType)
            //{
            //    switch (item.LcsrdcDataType)
            //    {
            //        case "varchar":
            //        case "nvarchar":
            //            CreateAutoImplementedProperty(builder, item.LcsrdcDisplayLabel.ToString(), typeof(string));
            //            break;
            //        case "int":
            //            CreateAutoImplementedProperty(builder, item.LcsrdcDisplayLabel.ToString(), typeof(int?));
            //            break;
            //        case "date":
            //            CreateAutoImplementedProperty(builder, item.LcsrdcDisplayLabel.ToString(), typeof(DateTime?));
            //            break;
            //        case "datetime":
            //            CreateAutoImplementedProperty(builder, item.LcsrdcDisplayLabel.ToString(), typeof(DateTime?));
            //            break;
            //        case "bit":
            //            CreateAutoImplementedProperty(builder, item.LcsrdcDisplayLabel.ToString(), typeof(bool?));
            //            break;
            //        case "bigint":
            //            CreateAutoImplementedProperty(builder, item.LcsrdcDisplayLabel.ToString(), typeof(Int64?));
            //            break;
            //        case "decimal":
            //            CreateAutoImplementedProperty(builder, item.LcsrdcDisplayLabel.ToString(), typeof(decimal?));
            //            break;
            //        case "float":
            //            CreateAutoImplementedProperty(builder, item.LcsrdcDisplayLabel.ToString(), typeof(double?));
            //            break;
            //        case "numeric":
            //            CreateAutoImplementedProperty(builder, item.LcsrdcDisplayLabel.ToString(), typeof(double?));
            //            break;
            //    }
            //}

            // Type resultType = builder.CreateType();

            //"For XML path('')" in this query is used to convert sql query result set into a single string.
            string ColumnQuerySingleRow = "select stuff((select '[' + [LcsrdcXColumnName] + '] AS [' +  [LcsrdcDisplayLabel] +'], ' FROM LCompanySpecificRawDataColumns WHERE LcsrdcRawDataTableId = " + RawDataTableId + " Order by LcsrdcOrdinalPosition FOR XML PATH('') ),1,0,'')";
            var ColumnListSingleRow = db.Database.SqlQuery<string>(ColumnQuerySingleRow).ToList().ElementAt(0).Trim();
            string Qry = "";
            if (RawDataTable != null) 
            {
                //Using the column list obtained above, and other parameters passed in the method create a SQL query to fatch the data from database
                Qry = "Select  " + ColumnListSingleRow.Remove(ColumnListSingleRow.Length - 1) + "  ";
                Qry = Qry + " FROM  {Schema}." + RawDataTable.LrdtName;
                Qry = Qry + " Where XBatchNumber = " + SOSBatchNumber + " and XPeriodCode=" + BatchDetails.LbPeriodCode;
            }
            if (Status == "NA")
            {
                Qry = Qry + "AND (XStatus = '" + Status + "' or XStatus is null ) " + FilterQuery;
            }
            else
            {
                Qry = Qry + " AND XStatus = '" + Status + "' " + FilterQuery;
            }

            
            //var BatchDetails = db.LBatches.Where(p => p.LbBatchNumber == SOSBatchNumber).FirstOrDefault();
            // var result = db.Database.SqlQuery(resultType,Qry);
            //method to get query result and save it as an excel file in S drive
            var FileName = "ExportRawDataFile_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".zip";
            var FileNamewithoutextention = "ExportRawDataFile_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss");
            if (string.IsNullOrEmpty(BatchDetails.LbCommissionPeriod))
            {
                Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LRawData", "DownloadLRawDataFile", "Commission Period does not exist for the batch with Batch# - " + BatchDetails.LbBatchNumber, "", "Warning", "", "", "L2 Admin", null, null, "New");
            }
            else
            {
                var xx = Globals.GetQueryResultFromOpcoDB(CompanyId, Qry, BatchDetails.LbCommissionPeriod);
                 Globals.CreateExcelFromDataTable(xx, FileName, "",Company.GcCode, LoggedInUserName,FileNamewithoutextention);
                //Globals.ExportZipFromDataTable(null, Company.GcCode, LoggedInUserName, FileName, xx);
            }
            //Add the result obtained from a query into a csv file

            //if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["CalcDocumentPath"] + "/" + Company.GcCode + "/" + LoggedInUserName))//create directory if not present
            //    {
            //        System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["CalcDocumentPath"] + "/" + Company.GcCode + "/" + LoggedInUserName);
            //    }
            //    using (var CTextWriter = new StreamWriter(CfileLocation))
            //    using (var Csv = new CsvWriter(CTextWriter))
            //    {

            //    //foreach (DataColumn column in tb.Columns)
            //    //{
            //    //   // var Col = RemoveSpecialCharacters(column.ColumnName);
            //    //    Csv.WriteField(column.ColumnName);
            //    //}
            //    //Csv.WriteRecords(tb.Columns);
            //    //Csv.NextRecord();
            //    Csv.Configuration.Encoding = Encoding.UTF8;
            //    Csv.WriteRecords(result);
            //        //foreach (DataRow row in tb.Rows)
            //        //{
            //        //    for (var i = 0; i < tb.Columns.Count; i++)
            //        //    {
            //        //       // var rows =RemoveSpecialCharacters(row[i] as string);
            //        //        Csv.WriteField(row[i]);
            //        //    }
            //        //    Csv.NextRecord();
            //        //}
            //    }
            return Ok(FileName);
        }
    }
}
