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
    public class LBatchFilesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LBatchFiles
        public IHttpActionResult GetLBatchFilesByBatchId(int BatchId)
        {
            var xx = (from aa in db.LBatchFiles.Where(p=>p.LbfBatchId==BatchId)
                      select new { aa.Id, aa.LBatch.LbBatchNumber,aa.LbfBatchId,aa.LbfFileName,aa.LbfFileTimeStamp }).OrderBy(p => p.LbBatchNumber);
            return Ok(xx);
        }

        // GET: api/LBatchFiles/5
        [ResponseType(typeof(LBatchFile))]
        public async Task<IHttpActionResult> GetLBatchFile(int id)
        {
            var LBatchFile = db.LBatchFiles.Where(p => p.Id == id).FirstOrDefault();
            if (LBatchFile == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "BATCH FILES")));
            }
            return Ok(LBatchFile);
        }

        // PUT: api/LBatchFiles/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLBatchFile(int id, LBatchFile LBatchFile)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "BATCH FILES")));
            }

            if (id != LBatchFile.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "BATCH FILES")));
            }
            try
            {
                db.Entry(LBatchFile).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LBatchFileExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "BATCH FILES")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LBatchFiles
        [ResponseType(typeof(LBatchFile))]
        public async Task<IHttpActionResult> PostLBatchFile(LBatchFile LBatchFile)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "BATCH FILES")));
            }
            try
            {
                //As soon as BATCH FILES is created a database trigger will automatically Create a sequence for this BATCH FILES which will provide sequence of Claim Ids 
                //while saving Lclaims data and will also add a row in GKeyValues for this sequence Name.
                db.LBatchFiles.Add(LBatchFile);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = LBatchFile.Id }, LBatchFile);
        }

        // DELETE: api/LBatchFiles/5
        [ResponseType(typeof(LBatchFile))]
        public async Task<IHttpActionResult> DeleteLBatchFile(int id)
        {
            LBatchFile LBatchFile = await db.LBatchFiles.FindAsync(id);
            if (LBatchFile == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "BATCH FILES")));
            }
            try
            {
                db.LBatchFiles.Remove(LBatchFile);
                await db.SaveChangesAsync();
                return Ok(LBatchFile);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
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

        private bool LBatchFileExists(int id)
        {
            return db.LBatchFiles.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
