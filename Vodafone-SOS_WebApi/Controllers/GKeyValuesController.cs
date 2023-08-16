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

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class GKeyValuesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/GKeyValues
        public IHttpActionResult GetGKeyValues()
        {
            var xx = (from aa in db.GKeyValues.Include(c => c.GCompany)
                      select new { aa.Id, aa.GkvKey, aa.GkvValue, aa.GkvCompanyId, aa.GCompany.GcCompanyName }).OrderBy(p => p.GkvKey);
            return Ok(xx);
        }

        // GET: api/GKeyValues
        public IHttpActionResult GetGKeyValuesByName(string KeyName, int CompanyId)
        {
            var xx = (from aa in db.GKeyValues.Include(c => c.GCompany).Where(p => p.GkvCompanyId == CompanyId).Where(p => p.GkvKey == KeyName)
                      select new { aa.Id, aa.GkvKey, aa.GkvValue, aa.GkvCompanyId, aa.GCompany.GcCompanyName }).FirstOrDefault();
            //If Key isnot defined in opco get it from Group 99
            if (xx == null)
            {
                xx = (from aa in db.GKeyValues.Include(c => c.GCompany).Where(p => p.GCompany.GcCode == "99").Where(p => p.GkvKey == KeyName)
                      select new { aa.Id, aa.GkvKey, aa.GkvValue, aa.GkvCompanyId, aa.GCompany.GcCompanyName }).FirstOrDefault();
            }
            return Ok(xx);
        }

        // GET: api/GKeyValues?CompanyId=5
        public IHttpActionResult GetGKeyValuesByCompanyId(int CompanyId)
        {
            var xx = (from aa in db.GKeyValues.Where(p => p.GkvCompanyId == CompanyId).Include(c => c.GCompany)
                      select new { aa.GkvKey, aa.GkvValue, aa.GkvCompanyId, aa.GCompany.GcCompanyName, aa.Id }).OrderBy(p => p.GkvKey);
            return Ok(xx);
        }

       //method used to get the detail of selected config for updation
        [ResponseType(typeof(GKeyValue))]
        public async Task<IHttpActionResult> GetGKeyValue(int Id)
        {
            var GKeyValue = db.GKeyValues.Where(p => p.Id == Id).Include(c => c.GCompany).Select(x => new { x.Id, x.GkvCompanyId, x.GkvKey, x.GkvValue, x.GCompany.GcCompanyName,x.GkvDescription }).FirstOrDefault();
            if (GKeyValue == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "KeyValue")));
            }
            return Ok(GKeyValue);
        }

        //method to save the updated config data
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGKeyValue(int Id, GKeyValue GKeyValue)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "KeyValue")));
            }

            if (Id != GKeyValue.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "KeyValue")));
            }
            try
            {
                db.Entry(GKeyValue).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!GKeyValueExists(Id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "KeyValue")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        //method to save the created config in db
        [ResponseType(typeof(GKeyValue))]
        public async Task<IHttpActionResult> PostGKeyValue(GKeyValue GKeyValue)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "KeyValue")));
            }
            try
            {
                db.GKeyValues.Add(GKeyValue);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { Id = GKeyValue.Id }, GKeyValue);
        }

        //method used to delete the selected config for deletion 
        [ResponseType(typeof(GKeyValue))]
        public async Task<IHttpActionResult> DeleteGKeyVAlue(int id)
        {  
            GKeyValue GKeyValue = await db.GKeyValues.FindAsync(id);
            if (GKeyValue == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "KeyValue")));
            }
            try
            {
                db.GKeyValues.Remove(GKeyValue);
                await db.SaveChangesAsync();
               
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return Ok(GKeyValue);
        }

        
        /// Method to get the Key Values from the GKeyValues for L2Admin Page
        [ResponseType(typeof(GKeyValues))]
        public IHttpActionResult GetGKeyValueForConfiguration(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            var SortQuery = "";
            if (!string.IsNullOrEmpty(sortdatafield))
            {
                SortQuery = " order by " + sortdatafield + " " + sortorder;
            }
            else
            {
                SortQuery = " ORDER BY GkvKey desc";
            }
            string Qry = string.Empty;
            if (FilterQuery == null)
            {
                Qry = "SELECT * FROM(SELECT *,ROW_NUMBER()  OVER (" + SortQuery + ") as datacount FROM (SELECT g.Id,gc.GcCode,GkvKey,GkvValue,GkvDescription FROM GKeyValues g" +
                      " INNER JOIN GCompanies gc on gc.Id = g.GkvCompanyId)A)B WHERE B.datacount > @P1 AND B.datacount <= @P2 ";
            }
            else
            {
                FilterQuery = "WHERE 1=1 " + FilterQuery;
                Qry = "SELECT * FROM(SELECT *,ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM (SELECT g.Id,gc.GcCode,GkvKey,GkvValue,GkvDescription FROM GKeyValues g" +
                     " INNER JOIN GCompanies gc on gc.Id = g.GkvCompanyId " + FilterQuery + " )A)B WHERE B.datacount > @P1 AND B.datacount <= @P2 ";
            }
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<GKeyValues>(Qry, parameters).ToList();
            return Ok(xx);
        }

        
        /// Method to get the counts for Key Values from the GKeyValues for L2Admin Page       
        /// <returns></returns>
        public IHttpActionResult GetGKeyValueCountForConfiguration()
        {
            string Qry = "SELECT g.Id,g.GkvCompanyId,gc.GcCode,GkvKey,GkvValue,GkvDescription FROM GKeyValues g INNER JOIN GCompanies gc on gc.Id = g.GkvCompanyId";
            var xx = db.Database.SqlQuery<GKeyValue>(Qry).Count();
            return Ok(xx);           
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


       

        private bool GKeyValueExists(int Id)
        {
            return db.GKeyValues.Count(e => e.Id == Id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("UQ_GKeyValues_CompanyId_Key", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "KEY VALUES", "COMPANY"));
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
