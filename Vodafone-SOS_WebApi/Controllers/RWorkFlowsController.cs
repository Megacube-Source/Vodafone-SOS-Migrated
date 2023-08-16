//Code Review Convert SQL to LINQ completed
//There are many places where excdeption is caught and thrown as it is it's wrong practice. Need to review and fix

using System;
//using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Web;
using System.Web.Http;
using System.Data;
using System.Data.Entity;
using System.Web.Http.Description;
//using System.Web.Mvc;
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class RWorkFlowsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        //This method will return Change Request Workflow Name corresponding to the Workflow Name passed as parameter
        public IHttpActionResult GetCRWorkflowname(string WorkflowName)
        {
            var Workflow = db.RWorkFlows.Where(p => p.RwfName == WorkflowName).FirstOrDefault();
            if(Workflow==null)
            {
                return Ok();
            }
            else
            {
                return Ok(Workflow.RwfCRWFName);
            }
        }

        public IHttpActionResult PostRWorkFlow(RWorkFlow RWorkFlow)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE")));
            }
            try
            {
                string tbl = RWorkFlow.RwfBaseTableName;
                int result = db.Database.SqlQuery<int>("select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME= @P0", tbl).FirstOrDefault<int>();
                if (result > 0)
                {
                    db.RWorkFlows.Add(RWorkFlow);
                    db.SaveChanges();
                }
               else if(result == 0)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "BASETABLE")));
                }
            }
            catch (Exception ex)
            {
                throw ex;
              //  throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return Ok();
        }

        // PUT: api/RWorkFlow/5

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRWorkFlow(int id, RWorkFlow RW)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "WORKFLOW")));
            }

            if (id != RW.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "WORKFLOW")));
            }
            try
            {
                string tbl = RW.RwfBaseTableName;
                int result = db.Database.SqlQuery<int>("select count(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME= @P0", tbl).FirstOrDefault<int>();
                if (result > 0)
                {
                    db.Entry(RW).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "BASETABLE")));
                }
               
            }
            catch (Exception ex)
            {
                /*  if (!RWorkFlowExists(id))
                  {
                      throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW")));
                  }
                  else
                  {
                      throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                  }*/
                throw ex;
            }
            return StatusCode(HttpStatusCode.NoContent);
        }




        private bool RWorkFlowExists(int id)
        {

            return db.RWorkFlows.Count(e => e.Id == id) > 0;
        }
        // GET: api/RWorkFlow/5

        public IHttpActionResult GetRWorkFlowById(int id)
        {
            var RW = db.RWorkFlows.Where(p => p.Id == id).
                Select(x => new { x.Id, x.RwfName, x.RwfBaseTableName, x.RwfUILabel, x.RwfCRAllowed,x.RwfCRWFName,x.RwfWFType}).FirstOrDefault();
            if (RW == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WorkFlow")));
            }
            return Ok(RW);
        }

        public IHttpActionResult GetRWorkFlows()
        {
            var xx = db.RWorkFlows.
                Select(x => new { x.Id, x.RwfName, x.RwfBaseTableName, x.RwfUILabel, x.RwfCRAllowed, x.RwfCRWFName, x.RwfWFType }).OrderBy(x => x.RwfName);
            return Ok(xx);
            
        }

        // DELETE: api/RWorkFlows/5

        public IHttpActionResult DeleteRWorkFlow(int id)
        {
            RWorkFlow RWF = db.RWorkFlows.Find(id);
            if (RWF == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WorkFlow")));
            }
            try
            {
                db.RWorkFlows.Remove(RWF);
                 db.SaveChanges();
                return Ok(RWF);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
        }

        //public IHttpActionResult GetWFSummaryCounts()
        //{
        //    string Qry = "SELECT LaSOSProcessName, count(*),GcCode from LAudit La inner join GCompanies gc on gc.Id = La.LaCompanyId  Where LaAction = 'Create' AND LaActioDateTime >= DATEADD(dd, -30, GETDATE()) group by LaSOSProcessName, GcCode";

        //    //string Qry = "SELECT Id,GkvCompanyId,GkvKey,GkvValue,GkvDescription FROM GKeyValues";
        //    var xx = db.Database.SqlQuery<LAudit>(Qry).Count();
        //    return Ok(xx);
        //}

        [HttpGet]
        public async Task<IHttpActionResult> GetCompletedListcolumnlist(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {

            var tb = new DataTable();

            string Query = "Exec [spGetCompletedItems] @pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery";

            ////using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            // return Ok(ds);
            tb.Columns.Remove(tb.Columns["datacount"]);

            string[] columnNames = tb.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            

            return Ok(columnNames);
        }

        public IHttpActionResult GetCompletedItems(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {
            var tb = new DataTable();

            string Query = "Exec [spGetCompletedItems] @pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery";

            ////using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);

            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            // return Ok(ds);
            tb.Columns.Remove(tb.Columns["datacount"]);


            return Ok(tb);
        }

        public IHttpActionResult GetCountsForCompletedItems()
        {
            var tb = new DataTable();
            var Query = "Exec [spGetCompletedItemsCounts]";
            ////using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);

            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            //tb.Columns.Remove(tb.Columns["row"]);
            sda.Fill(tb);
            var count = tb.Rows.Count;
            //tb.Columns.Remove("row");
            conn.Close();
            // return Ok(ds);
            return Ok(count);
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("RwfName", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "WORKFLOW NAME"));
            else if (SqEx.Message.IndexOf("FK_RWorkFlows_LWorkFlowConfig", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "WORKFLOW", "WORKFLOW CONFIGURATION"));
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}