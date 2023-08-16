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
using System.Configuration;

namespace Vodafone_SOS_WebApi.Controllers
{
    public class ReportsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        // GET: Reports
        
        [ResponseType(typeof(ReportsViewModel))]
        public async Task<IHttpActionResult> GetXReportsTreeStructure(string strTreeType,string strUserEmail,string strUserRole,string strOpCoCode, string strCommissionPeriod, string FilterQuery, string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            var tb = new DataTable();
            string Query = "Exec [dbo].[spGetUserReportTreeStrucure] @TreeType ,@LoggedInUserEmail , @UserRole ,@OpCo , @CommissionPeriod ,@FilterQuery ,@sortdatafield ,@sortorder ,@pagesize,@pagenum";
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@TreeType", strTreeType);
            cmd.Parameters.AddWithValue("@LoggedInUserEmail", string.IsNullOrEmpty(strUserEmail) ? (object)System.DBNull.Value : (object)strUserEmail);
            cmd.Parameters.AddWithValue("@UserRole", string.IsNullOrEmpty(strUserRole) ? (object)System.DBNull.Value : (object)strUserRole);
            cmd.Parameters.AddWithValue("@OpCo", string.IsNullOrEmpty(strOpCoCode) ? (object)System.DBNull.Value : (object)strOpCoCode);
            cmd.Parameters.AddWithValue("CommissionPeriod", string.IsNullOrEmpty(strCommissionPeriod) ? (object)System.DBNull.Value : (object)strCommissionPeriod); 
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? "" : (object)sortorder);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : (object)sortdatafield);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : (object)FilterQuery);

            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();


           // var RawQuery = db.Database.SqlQuery<ReportsViewModel>("exec spGetUserReportTreeStrucure '"+ strTreeType + "','" +strUserEmail+"','" + strUserRole + "','" + strOpCoCode + "','" + strCommissionPeriod + "' ,'" + FilterQuery + "','" + sortdatafield + "','" + sortorder + "','" + pagesize + "','" + pagenum + "'" );

            //var Task = RawQuery.ToList();
            //var ListData = Task.ToList();
            //return Ok(ListData);
            return Ok(tb);

        }

        //[ResponseType(typeof(ReportsViewModel))]
        //public async Task<IHttpActionResult> GetXReportsTreeStructure(string strTreeType, string strUserEmail, string strUserRole, string strOpCoCode, string strCommissionPeriod, string FilterQuery, string sortdatafield, string sortorder, int pagesize, int pagenum)
        //{
        //    var RawQuery = db.Database.SqlQuery<ReportsViewModel>("exec spGetUserReportTreeStrucure '" + strTreeType + "','" + strUserEmail + "','" + strUserRole + "','" + strOpCoCode + "','" + strCommissionPeriod + "' ,'" + FilterQuery + "','" + sortdatafield + "','" + sortorder + "','" + pagesize + "','" + pagenum + "'");

        //    var Task = RawQuery.ToList();
        //    var ListData = Task.ToList();
        //    return Ok(ListData);

        //}

        //public IHttpActionResult GetXReportsTreeStructure()
        //{
        //    //var XReportTree = db.x
        //    var PayeeDetails = db.LPayees.Where(p => p.Id == PayeeId).Select(p => new { p.LpFileNames, p.LpUserFriendlyFileNames, p.Id, p.LpPayeeCode }).FirstOrDefault();
        //    //return Ok(PayeeDetails);
        //    return Ok(PayeeDetails);
        //}
        
    }
}