//Code review and fix SQL to LINQ completed for necessary methods. Didn't touch methods that are not pron for SQL injection.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class GErrorLogsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/GErrorLogs
        public IHttpActionResult GetGErrorLogs()
        {
            var xx = (from aa in db.GErrorLogs
                      select aa).OrderByDescending(p => p.GelErrorDateTime);
            return Ok(xx);
        }

        // GET: api/GErrorLogs/5
        [ResponseType(typeof(GErrorLog))]
        public async Task<IHttpActionResult> GetGErrorLog(int id)
        {
            var GErrorLog = db.GErrorLogs.Where(p => p.Id == id).FirstOrDefault();
            if (GErrorLog == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ERROR LOG")));
            }
            return Ok(GErrorLog);
        }

        // POST: api/GErrorLogs
        [ResponseType(typeof(GErrorLog))]
        public async Task<IHttpActionResult> PostGErrorLog(GErrorLog GErrorLog)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ERROR LOG")));
            }
            try
            {
                db.GErrorLogs.Add(GErrorLog);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = GErrorLog.Id }, GErrorLog);
        }

        //Method added by Ritu to load data into grid from db
        //GET:api/ErrorLogs
        public IHttpActionResult GetGErrorlogGridData(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            var SortQuery = "";
            if (!string.IsNullOrEmpty(sortdatafield))
            {
                SortQuery = " order by " + sortdatafield + " " + sortorder;
            }
            else
            {
                SortQuery = " ORDER BY GelErrorDateTime desc";
            }
            string Qry = string.Empty;
            if (FilterQuery == null)
            {
               
                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as row " +
                      "FROM(Select Id, GelErrorDateTime, GelSourceProject, GelController, GelMethod, GelStackTrace, GelUserName," +
                      "GelErrorType,GelErrorDescription,GelResolution,GelErrorOwner,GelFieldName,GelSOSBatchNumber from GErrorLogs where GelErrorDateTime >= dateadd(dd, -30, getdate()))A " +
                      " )B WHERE B.row > @P1 AND B.row <= @P2";
            }
            else
            {
               
                FilterQuery = "WHERE 1=1" + FilterQuery;
                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as row " +
                      "FROM(Select Id, GelErrorDateTime, GelSourceProject, GelController, GelMethod, GelStackTrace, GelUserName," +
                      "GelErrorType,GelErrorDescription,GelResolution,GelErrorOwner,GelFieldName,GelSOSBatchNumber from GErrorLogs where GelErrorDateTime >= dateadd(dd, -30, getdate()))A " +
                      FilterQuery + " )B WHERE B.row > @P1 AND B.row <= @P2";

            }            
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            //parameterList.Add(new SqlParameter("@P3", qq));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<GErrorLogViewModel>(Qry, parameters).ToList();
            return Ok(xx);
        }

        //Method Added by RG To count Error into GErrorLog
        public IHttpActionResult GetErrorLogCount()
        {
            //string Query = "select count(*) from GErrorLogs";
            //int xx = db.Database.SqlQuery<int>(Query).FirstOrDefault();           
            //SQL query converted to LINQ by VG
            //var xx = db.GErrorLogs.Count();
            //return Ok(xx);

            //string Qry = "select GelErrorDateTime, GelController, GelMethod, GelStackTrace, GelUserName, GelErrorDescription from GErrorLogs where GelErrorDateTime >= dateadd(dd, -90, getdate()) order by GelErrorDateTime desc";// Order by @P3";


            //
            string Qry = "SELECT GelErrorDateTime, GelSourceProject, GelController, GelMethod, GelStackTrace, GelUserName,GelErrorType, GelErrorDescription, GelResolution, GelErrorOwner, GelFieldName, GelSOSBatchNumber from GErrorLogs  where GelErrorDateTime >= dateadd(dd, -30, getdate()) order by GelErrorDateTime desc";
            var xx = db.Database.SqlQuery<GErrorLogViewModel>(Qry).Count();
            return Ok(xx);


        }

        /// <summary>
        /// Created by Rakhi Singh
        /// Method to get the total counts of Exception records for Summary tab on L2Admin Page
        /// </summary>  
        public IHttpActionResult GetExceptionSummaryCounts()
        {
            string Qry = "SELECT GelController,GelMethod,count(GelMethod) as counts from GErrorLogs where GelErrorDateTime >= dateadd(dd, -30, getdate()) group by GelController, GelMethod";// Order by @P3";
            var xx = db.Database.SqlQuery<GErrorLogViewModel>(Qry).Count();
            return Ok(xx);
        }

        /// <summary>
        ///  Created by Rakhi Singh
        ///  Method to get the data for summary tab of exception on L2Admin Page
        /// </summary>
        public IHttpActionResult GetExceptionSummary(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {            
            var SortQuery = "";
            if (!string.IsNullOrEmpty(sortdatafield))
            {
                SortQuery = " order by " + sortdatafield + " " + sortorder;
            }
            else
            {
                SortQuery = " ORDER BY count(GelMethod) desc";
            }

            string Qry = string.Empty;
            if (FilterQuery == null)
            {
                Qry = "SELECT * FROM (SELECT GelController,GelMethod,count(GelMethod) as counts,ROW_NUMBER() OVER (" + SortQuery + ") " +
                      "as datacount FROM (SELECT * from GErrorLogs where GelErrorDateTime >= dateadd(dd, -30, getdate()))a "+
                      "group by GelController, GelMethod)b  where b.datacount > @P1 and b.datacount <= @P2 and b.counts<>0";// Order by @P3";
            }
            else
            {
                FilterQuery = "WHERE 1=1" + FilterQuery;
                Qry = "SELECT * FROM (SELECT GelController,GelMethod,count(GelMethod) as counts,ROW_NUMBER() OVER (" + SortQuery + ") " +
                     "as datacount FROM(SELECT * from GErrorLogs where GelErrorDateTime >= dateadd(dd, -30, getdate()))a " +
                     FilterQuery + " group by GelController, GelMethod)b  where b.datacount > @P1 and b.datacount <= @P2 and b.counts<>0";// Order by @P3";
            }

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<GErrorLogViewModel>(Qry, parameters).ToList();

            return Ok(xx);
        }

        /// <summary>
        /// Created by Rakhi Singh
        /// Method to show the data on chart for Exception
        /// </summary>
        //public IHttpActionResult GetExceptionChart()
        //{
           
        //    string Qry = string.Empty;
        //    Qry = "select GelController,GelMethod,count(GelMethod) as counts from GErrorLogs where GelErrorDateTime >= dateadd(dd, -30, getdate()) group by GelController,GelMethod";// Order by @P3";
        //    var xx = db.Database.SqlQuery<GErrorLogViewModel>(Qry).ToList();

        //    return Ok(xx);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GErrorLogExists(int id)
        {
            return db.GErrorLogs.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("VErrorLog", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ERROR", "VIEW(S)"));
            else if (SqEx.Message.IndexOf("SpErrorLogs", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ERROR", "DATABASE OBJECTS(S)"));
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }

        public IHttpActionResult DownloadExceptionFile(int pagesize, int pagenum,string FilterQuery, string CompanyCode, string LoggedinUserName)
        {
            var tb = new DataTable();
            var SortQuery = " ORDER BY GelErrorDateTime desc";
            
            string Qry = string.Empty;
            if (FilterQuery == null)
            {

                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as row " +
                      "FROM(Select Id, GelErrorDateTime, GelSourceProject, GelController, GelMethod, GelStackTrace, GelUserName," +
                      "GelErrorType,GelErrorDescription,GelResolution,GelErrorOwner,GelFieldName,GelSOSBatchNumber from GErrorLogs where GelErrorDateTime >= dateadd(dd, -30, getdate()))A " +
                      " )B WHERE B.row > @P1 AND B.row <= @P2";
            }
            else
            {

                FilterQuery = "WHERE 1=1" + FilterQuery;
                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as row " +
                      "FROM(Select Id, GelErrorDateTime, GelSourceProject, GelController, GelMethod, GelStackTrace, GelUserName," +
                      "GelErrorType,GelErrorDescription,GelResolution,GelErrorOwner,GelFieldName,GelSOSBatchNumber from GErrorLogs where GelErrorDateTime >= dateadd(dd, -30, getdate()))A " +
                      FilterQuery + " )B WHERE B.row > @P1 AND B.row <= @P2";

            }
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Qry, conn);
            cmd.Parameters.AddWithValue("@P1", pagenum);
            cmd.Parameters.AddWithValue("@P2", pagesize);


            //SqlParameter param;

            //param = cmd.Parameters.Add("@P1", SqlDbType.Int);

            //param.Value = pagenum;



            //param = cmd.Parameters.Add("@P2", SqlDbType.Int);

            //param.Value = pagesize;



            // Execute the command.

            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();

            //List<SqlParameter> parameterList = new List<SqlParameter>();
            //parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            //parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            ////parameterList.Add(new SqlParameter("@P3", qq));
            //SqlParameter[] parameters = parameterList.ToArray();
            //tb = db.Database.SqlQuery(Qry, parameters);
            var FileName = "ExportException";
            
            Globals.ExportExceptionFromDataTable(null, CompanyCode, LoggedinUserName, FileName, tb);

            return Ok(FileName + ".xlsx");

        }

    }
}
