//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Web.Http;
//using System.Web.Http.Description;
//using System.Data.SqlClient; //being used in catch statement for identifying exception only.
//using Vodafone_SOS_WebApi.Models;
//using Vodafone_SOS_WebApi.Utilities;

//namespace Vodafone_SOS_WebApi.Controllers
//{
//    public class LRawDataColumnsController : ApiController
//    {
//        private VodafoneSOSLiteEntities db = new VodafoneSOSLiteEntities();

//        // GET: api/LRawDataColumns
//        public IHttpActionResult GetLRawDataColumns()
//        {
//            var xx = (from aa in db.LRawDataColumns
//                      select aa);
//            return Ok(xx);
//        }

//        public IHttpActionResult GetLRawDataColumnsByRawDataTypeId(int RawDataTypeId)
//        {
//            var xx = (from aa in db.LRawDataColumns
//                      select aa);
//            return Ok(xx);
//        }

//        // GET: api/LRawDataColumns/5
//        [ResponseType(typeof(LRawDataColumn))]
//        public async Task<IHttpActionResult> GetLRawDataColumn(int id)
//        {
//            var LRawDataColumn = db.LRawDataColumns.Where(p => p.Id == id).FirstOrDefault();
//            if (LRawDataColumn == null)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RAW DATA COLUMN")));
//            }
//            return Ok(LRawDataColumn);
//        }

//        // PUT: api/LRawDataColumns/5
//        [ResponseType(typeof(void))]
//        public async Task<IHttpActionResult> PutLRawDataColumn(int id, LRawDataColumn LRawDataColumn)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RAW DATA COLUMN")));
//            }

//            if (id != LRawDataColumn.Id)
//            {
//                //return BadRequest();
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RAW DATA COLUMN")));
//            }
//            try
//            {
//                db.Entry(LRawDataColumn).State = EntityState.Modified;
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                if (!LRawDataColumnExists(id))
//                {
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RAW DATA COLUMN")));
//                }
//                else
//                {
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//                }
//            }
//            return StatusCode(HttpStatusCode.NoContent);
//        }

//        // POST: api/LRawDataColumns
//        [ResponseType(typeof(LRawDataColumn))]
//        public async Task<IHttpActionResult> PostLRawDataColumn(LRawDataColumn LRawDataColumn)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "RAW DATA COLUMN")));
//            }
//            try
//            {
//                //As soon as RAW DATA COLUMN is created a database trigger will automatically Create a sequence for this RAW DATA COLUMN which will provide sequence of Claim Ids 
//                //while saving Lclaims data and will also add a row in GKeyValues for this sequence Name.
//                db.LRawDataColumns.Add(LRawDataColumn);
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//            }
//            return CreatedAtRoute("DefaultApi", new { id = LRawDataColumn.Id }, LRawDataColumn);
//        }

//        // DELETE: api/LRawDataColumns/5
//        [ResponseType(typeof(LRawDataColumn))]
//        public async Task<IHttpActionResult> DeleteLRawDataColumn(int id)
//        {
//            LRawDataColumn LRawDataColumn = await db.LRawDataColumns.FindAsync(id);
//            if (LRawDataColumn == null)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RAW DATA COLUMN")));
//            }
//            try
//            {
//                db.LRawDataColumns.Remove(LRawDataColumn);
//                await db.SaveChangesAsync();
//                return Ok(LRawDataColumn);
//            }
//            catch (Exception ex)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//            }
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        private bool LRawDataColumnExists(int id)
//        {
//            return db.LRawDataColumns.Count(e => e.Id == id) > 0;
//        }

//        private string GetCustomizedErrorMessage(Exception ex)
//        {
//            //Convert the exception to SqlException to get the error message returned by database.
//            var SqEx = ex.GetBaseException() as SqlException;
//                //Something else failed return original error message as retrieved from database
//                return SqEx.Message;
//        }
//    }
//}
