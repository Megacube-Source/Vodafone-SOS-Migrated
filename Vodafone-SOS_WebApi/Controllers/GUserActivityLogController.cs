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
using System.Configuration;

namespace Vodafone_SOS_WebApi.Controllers
{
    public class GUserActivityLogController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        // POST: api/GUserActivityLog
        [ResponseType(typeof(GUserActivityLog))]
        public async Task<IHttpActionResult> PostGUserActivityLog(GUserActivityLog GUserActivityLog)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "USERACTIVITYLOG")));
            }
            try
            {
                
                db.GUserActivityLogs.Add(GUserActivityLog);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,ex.StackTrace.ToString()));
            }
            return CreatedAtRoute("DefaultApi", new { id = GUserActivityLog.Id }, GUserActivityLog);
        }


        public IHttpActionResult GetUserActivityLogCounts(string EmailId)
        {
            var tb = new DataTable();
            string Query = "Exec [spGetUserActivityLogCounts] @EmailId";

            ////using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);

            cmd.Parameters.AddWithValue("@EmailId", EmailId);
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
        public async Task<IHttpActionResult> GetUserActivityLog(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery, string EmailId)
        {

            var tb = new DataTable();

            string Query = "Exec [spGetUserActivityLog] @pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery,@EmailId";

            ////using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);

            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            //cmd.Parameters.AddWithValue("@FilterQuery", (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@EmailId", EmailId);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            //tb.Columns.Remove(tb.Columns["row"]);
            sda.Fill(tb);
            tb.Columns.Remove("datacount");
            conn.Close();
            // return Ok(ds);
            return Ok(tb);


        }

    }
}