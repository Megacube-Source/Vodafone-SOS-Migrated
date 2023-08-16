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
using System.Data.Entity.Validation;
using System.Globalization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using System.Data.Entity.Core.Objects;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LAuditController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        // GET: api/LAudit Get Audit List to be exported as excel or displayed as grid between the start date and end date and for the entity passed as parameter
        [HttpGet]
        public IHttpActionResult GetLAuditForReports(string Entity, string StartDate, string EndDate, int CompanyId, int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery,string UserName, string Workflow)
        {
            //These vaiables are declared to convert start date and end date to datetime as in linq datetime objects are required for comparison
            //var StartDateTime = DateTime.ParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
           // var EndDateTime = DateTime.ParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var Company = db.GCompanies.Find(CompanyId);
            string Qry = "";
            if (Entity.Equals("PayeesCR",StringComparison.OrdinalIgnoreCase) || Entity.Equals("UsersCR",StringComparison.OrdinalIgnoreCase))
            {
                if (Company.GcCode.Equals("99"))//Group Auditor will see all opco records
                {
                    Qry = "select * From (select LcrOldValue,LcrNewValue,LcrColumnLabel,LcrColumnName,LaSOSProcessName,LaL3ProcessName,LaControlCode,LaControlDescription,LaAction,dbo.FnGetUserName(LaActionedById) as LaActionedById,LaActioDateTime,LaOldStatus,LaNewStatus,LaEntityType,LaEntityId,LaEntityName,(select Name from AspnetRoles where Id=LaRoleId)as LaRoleId, ROW_NUMBER() OVER (ORDER BY LaActioDateTime desc) as row,(select GcCompanyName from GCompanies where Id=LaCompanyId) as GcCompanyName , LP.LpPayeeCode as PayeeCode  from LAudit A inner join LChangeRequests CR on A.LaEntityId=CR.Id left join LPayees LP on CR.LcrRowId=LP.Id where  LaSOSProcessName='" + Entity+ "' and convert(date,LaActioDateTime) >= convert(date,'" + StartDate+ "') AND convert(date,LaActioDateTime) <= convert(date,'" + EndDate+"') ) a ";
                }
                else
                {
                    Qry = "select * From (select LcrOldValue,LcrNewValue,LcrColumnLabel,LcrColumnName,LaSOSProcessName,LaL3ProcessName,LaControlCode,LaControlDescription,LaAction,dbo.FnGetUserName(LaActionedById) as LaActionedById,LaActioDateTime,LaOldStatus,LaNewStatus,LaEntityType,LaEntityId,LaEntityName,(select Name from AspnetRoles where Id=LaRoleId)as LaRoleId, ROW_NUMBER() OVER (ORDER BY LaActioDateTime desc) as row,(select GcCompanyName from GCompanies where Id=LaCompanyId) as GcCompanyName , LP.LpPayeeCode as PayeeCode  from LAudit A inner join LChangeRequests CR on A.LaEntityId=CR.Id left join LPayees LP on CR.LcrRowId=LP.Id where LaCompanyId=" + CompanyId + " and LaSOSProcessName='" + Entity + "' and convert(date,LaActioDateTime) >= convert(date,'" + StartDate + "') AND   convert(date,LaActioDateTime) <= convert(date,'" + EndDate + "') ) a ";
                }
            }
            else
            {
                if (Company.GcCode.Equals("99"))//Group Auditor will see all opco records
                {
                    Qry = "select * From (select LaSOSProcessName,LaL3ProcessName,LaControlCode,LaControlDescription,LaAction,dbo.FnGetUserName(LaActionedById) as LaActionedById,LaActioDateTime,LaOldStatus,LaNewStatus,LaEntityType,LaEntityId,LaEntityName,(select Name from AspnetRoles where Id=LaRoleId)as LaRoleId, ROW_NUMBER() OVER (ORDER BY LaActioDateTime desc) as row,(select GcCompanyName from GCompanies where Id=LaCompanyId) as GcCompanyName ,ISNULL(laPeriod, '') as laPeriod , LP.LpPayeeCode as PayeeCode from LAudit A left join LPayees LP on A.LaEntityId=LP.Id where LaSOSProcessName='" + Entity + "'  and convert(date,LaActioDateTime) >= convert(date,'" + StartDate + "') AND   convert(date,LaActioDateTime) <= convert(date,'" + EndDate + "')) a ";
                }
                else
                {
                    Qry = "select * From (select LaSOSProcessName,LaL3ProcessName,LaControlCode,LaControlDescription,LaAction,dbo.FnGetUserName(LaActionedById) as LaActionedById,LaActioDateTime,LaOldStatus,LaNewStatus,LaEntityType,LaEntityId,LaEntityName,(select Name from AspnetRoles where Id=LaRoleId)as LaRoleId, ROW_NUMBER() OVER (ORDER BY LaActioDateTime desc) as row,(select GcCompanyName from GCompanies where Id=LaCompanyId) as GcCompanyName , ISNULL(laPeriod, '') as laPeriod , LP.LpPayeeCode as PayeeCode  from LAudit A left join LPayees LP on A.LaEntityId=LP.Id where LaCompanyId=" + CompanyId + " and LaSOSProcessName='" + Entity + "' and convert(date,LaActioDateTime) >= convert(date,'" + StartDate + "') AND   convert(date,LaActioDateTime) <= convert(date,'" + EndDate + "') ) a ";
                }
            }
            Qry = Qry + " Where row > " + pagenum * pagesize + " And row <= " + (pagenum + 1) * pagesize;
            Qry = Qry + FilterQuery;
            if (!string.IsNullOrEmpty(sortorder))//code for server side filtering
            {
                if (sortorder == "asc")
                {
                    Qry += " order by " + sortdatafield;
                }
                else
                {
                    Qry += " order by " + sortdatafield + " desc";
                }
            }
            var xx = db.Database.SqlQuery<GetLAuditForReports>(Qry).ToList();

            return Ok(xx);
        }

        // GET: api/LAudit Get Audit List to be exported as excel or displayed as grid between the start date and end date and for the entity passed as parameter
        [HttpGet]
        public IHttpActionResult DownloadLAuditForReports(string Entity, string StartDate, string EndDate, int CompanyId, string UserName, string Workflow)
        {
            //These vaiables are declared to convert start date and end date to datetime as in linq datetime objects are required for comparison
            //var StartDateTime = DateTime.ParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //var EndDateTime = DateTime.ParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var Company = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
            string Qry = "";
            if (Entity.Equals("PayeesCR", StringComparison.OrdinalIgnoreCase) || Entity.Equals("UsersCR", StringComparison.OrdinalIgnoreCase))
            {
                if (Company.GcCode.Equals("99"))//Group Auditor will see all opco records
                {
                    Qry = "select * From (select LcrOldValue,LcrNewValue,LcrColumnLabel,LcrColumnName,LaSOSProcessName,LaL3ProcessName,LaControlCode,LaControlDescription,LaAction,dbo.FnGetUserName(LaActionedById) as LaActionedById,LaActioDateTime,LaOldStatus,LaNewStatus,LaEntityType,LaEntityId,LaEntityName,(select Name from AspnetRoles where Id=LaRoleId)as LaRoleId, ROW_NUMBER() OVER (ORDER BY LaActioDateTime desc) as row,(select GcCompanyName from GCompanies where Id=LaCompanyId) as GcCompanyName  from LAudit A inner join LChangeRequests CR on A.LaEntityId=CR.Id where  LaSOSProcessName='" + Entity + "' and convert(date,LaActioDateTime) >= convert(date,'" + StartDate + "') AND   convert(date,LaActioDateTime) <= convert(date,'" + EndDate + "') ) a ";
                }
                else
                {
                    Qry = "select * From (select LcrOldValue,LcrNewValue,LcrColumnLabel,LcrColumnName,LaSOSProcessName,LaL3ProcessName,LaControlCode,LaControlDescription,LaAction,dbo.FnGetUserName(LaActionedById) as LaActionedById,LaActioDateTime,LaOldStatus,LaNewStatus,LaEntityType,LaEntityId,LaEntityName,(select Name from AspnetRoles where Id=LaRoleId)as LaRoleId, ROW_NUMBER() OVER (ORDER BY LaActioDateTime desc) as row,(select GcCompanyName from GCompanies where Id=LaCompanyId) as GcCompanyName  from LAudit A inner join LChangeRequests CR on A.LaEntityId=CR.Id where LaCompanyId=" + CompanyId + " and LaSOSProcessName='" + Entity + "' and convert(date,LaActioDateTime) >= convert(date,'" + StartDate + "') AND   convert(date,LaActioDateTime) <= convert(date,'" + EndDate + "') ) a ";
                }
            }
            else
            {
                if (Company.GcCode.Equals("99"))//Group Auditor will see all opco records
                {
                    Qry = "select LaSOSProcessName as [SOS Process Name] ,LaL3ProcessName as [L3 Process Name],LaControlCode as [Control Code],LaControlDescription as[Control Description],LaAction as Action,dbo.FnGetUserName(LaActionedById) as [Actioned By Id],LaActioDateTime as ActioDateTime,LaOldStatus as [Old Status],LaNewStatus as [New Status],LaEntityType as [Entity Type],LaEntityId as [Entity Id],LaEntityName as [Entity Name],(select Name from AspnetRoles where Id=LaRoleId)as RoleId,(select GcCompanyName from GCompanies where Id=LaCompanyId) as [Company Name] ,ISNULL(laPeriod, '') as Period from LAudit where  LaSOSProcessName='" + Entity + "' and convert(date,LaActioDateTime) >= convert(date,'" + StartDate + "') AND   convert(date,LaActioDateTime) <= convert(date,'" + EndDate + "') ";
                }
                else
                {
                    Qry = "select LaSOSProcessName as [SOS Process Name],LaL3ProcessName as [L3 Process Name],LaControlCode as [Control Code],LaControlDescription as [Control Description],LaAction as Action,dbo.FnGetUserName(LaActionedById) as [Actioned By Id],LaActioDateTime as ActioDateTime,LaOldStatus as [Old Status],LaNewStatus as [New Status],LaEntityType as [Entity Type],LaEntityId as [Entity Id],LaEntityName as [Entity Name],(select Name from AspnetRoles where Id=LaRoleId)as RoleId,(select GcCompanyName from GCompanies where Id=LaCompanyId) as [Company Name],ISNULL(laPeriod, '') as Period  from LAudit where LaCompanyId=" + CompanyId + " and LaSOSProcessName='" + Entity + "' and convert(date,LaActioDateTime) >= convert(date,'" + StartDate + "') AND   convert(date,LaActioDateTime) <= convert(date,'" + EndDate + "') ";
                }
            }
            var FileName = "ExportAuditReport_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".zip";
            Globals.ExportZipFromDataTable(Qry, Company.GcCode, UserName, FileName,null);

            return Ok(FileName);
        }


        
        public IHttpActionResult GetLAuditForReportCounts(string Entity, string StartDate, string EndDate, int CompanyId, string UserName, string Workflow)
        {
            //var StartDateTime = DateTime.ParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //var EndDateTime = DateTime.ParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var userlist = db.AspNetUsers.Where(p => p.GcCompanyId == CompanyId).Select(p => new { p.Id, p.UserName });
            var Company = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
            string Qry = "";
            if (Company.GcCode.Equals("99"))//Group Auditor will see all opco records
            {
                Qry = "select count(*) from LAudit where  LaSOSProcessName='" + Entity + "' and convert(date,LaActioDateTime) >= convert(date,'" + StartDate + "') AND   convert(date,LaActioDateTime) <= convert(date,'" + EndDate + "') ";
            }
            else
            {
                Qry = "select count(*) from LAudit where LaCompanyId=" + CompanyId + " and LaSOSProcessName='" + Entity + "' and convert(date,LaActioDateTime) >= convert(date,'" + StartDate + "') AND   convert(date,LaActioDateTime) <= convert(date,'" + EndDate + "') ";
            }
                int xx = db.Database.SqlQuery<int>(Qry).FirstOrDefault();
            return Ok(xx);
        }

        [ResponseType(typeof(GetLAuditForReports))]
        public IHttpActionResult GetNewItems(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            
            string Qry = string.Empty;

            if (FilterQuery == null)
            {
                Qry = "SELECT * FROM (SELECT la.LaSOSProcessName,GcCode, count(*) as counts,ROW_NUMBER() OVER(ORDER BY la.LaSOSProcessName)AS RowNum FROM LAudit la INNER JOIN GCompanies gc on gc.Id = la.LaCompanyId Where LaAction = 'Create' AND LaActioDateTime >= DATEADD(dd, -30, GETDATE()) Group by LaSOSProcessName,GcCode) AS RowConstrainedResult WHERE RowNum >= @P1 AND RowNum <= @P2 ORDER BY RowNum desc";

            }
            else
            {
                Qry = "SELECT * FROM (SELECT la.LaSOSProcessName,GcCode, count(*) as counts,ROW_NUMBER() OVER(ORDER BY la.LaSOSProcessName)AS RowNum FROM LAudit la INNER JOIN GCompanies gc on gc.Id = la.LaCompanyId Where LaAction = 'Create' AND LaActioDateTime >= DATEADD(dd, -30, GETDATE()) Group by LaSOSProcessName,GcCode) AS RowConstrainedResult WHERE RowNum >= @P1 AND RowNum <= @P2" + FilterQuery + "ORDER BY RowNum desc";
            }
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            //parameterList.Add(new SqlParameter("@P3", qq));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<GetLAuditForReports>(Qry, parameters).ToList();
            return Ok(xx);

        }

        //public IHttpActionResult GetCountsForNewItems()
        //{

        //    string Qry = "SELECT LaSOSProcessName,GcCode, count(*) as counts from LAudit la inner join GCompanies gc on gc.Id = la.LaCompanyId Where LaAction = 'Create' AND LaActioDateTime >= DATEADD(dd, -30, GETDATE()) Group by LaSOSProcessName,GcCode";
        //    var xx = db.Database.SqlQuery<GetLAuditForReports>(Qry).Count();
        //    return Ok(xx);
        //}

        public IHttpActionResult GetNIForChart()
        {
            string Qry = "SELECT rw.Id as workflowId ,la.LaSOSProcessName as LaSOSProcessName,GcCode, count(*) as counts FROM LAudit la INNER JOIN GCompanies gc on gc.Id = la.LaCompanyId Inner Join RWorkFlows rw on rw.Id = la.LaWorkflowId Where LaAction = 'Create' AND LaActioDateTime >= DATEADD(dd, -30, GETDATE()) AND la.LaCompanyId !=1 Group by LaSOSProcessName,GcCode,rw.Id";
            var xx = db.Database.SqlQuery<GetLAuditForReports>(Qry).ToList();
            return Ok(xx);
        }


        public async Task<IHttpActionResult> GetDataForNewItemColumns(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery,int Intervalid)
        {
            var tb = new DataTable();

            string Query = "Exec [spGetNewItems] @pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery,@Intervalid";

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
            cmd.Parameters.AddWithValue("@Intervalid", Intervalid);

            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            //tb.Columns.Remove(tb.Columns["row"]);
            sda.Fill(tb);
            tb.Columns.Remove("datacount");
            conn.Close();
            // return Ok(ds);
            return Ok(tb);
        }

        public async Task<IHttpActionResult> GetCountsForNewItems()
        {
            var tb = new DataTable();

            string Query = "Exec [spGetNewItemsCounts]";

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

        [HttpGet]
        public async Task<IHttpActionResult> GetNewItemscolumnlist(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, int Intervalid)
        {
            
           
            var tb = new DataTable();
            
            string Query = "Exec [spGetNewItems] @pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery,@Intervalid";
          
            ////using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            cmd.Parameters.AddWithValue("@Intervalid", Intervalid);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            // return Ok(ds);
            tb.Columns.Remove(tb.Columns["datacount"]);




            string[] columnNames = tb.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            // var columnlist = (columnNames);
            // var column = ds.Tables[0].Columns.RemoveAt(14);
            //The Ado.Net code ends here

            // var q = from r in columnNames where r != "row" select r;

            return Ok(columnNames); 
        }

    }
}
