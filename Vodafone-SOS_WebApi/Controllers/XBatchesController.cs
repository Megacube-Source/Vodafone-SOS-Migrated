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
    public class XBatchesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        [ResponseType(typeof(XBatch))]
        public async Task<IHttpActionResult> GetXBatchByBatchNumber(int BatchNumber)
        {
            var XBatch = db.XBatches.Where(p => p.XBatchNumber == BatchNumber).Select(aa => new
            {
                aa.Id,
                aa.XCompanyCode,
                aa.XUpdatedBy,
                aa.XBatchNumber,
                aa.XBatchType,
                aa.XRawDataType,
                aa.XStatus,
                aa.XUploadStartDateTime,
                aa.XUploadFinishDateTime,
                aa.XRecordCount,
                aa.XAlteryxBatchNumber,
                aa.XComments,
                aa.XCommissionPeriod
            }).FirstOrDefault();
            if (XBatch == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "XBatch")));
            }
            return Ok(XBatch);
        }

        // PUT: api/XBatches/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutXBatch(int Id, XBatch XBatch)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "XBatch")));
            }

            if (Id != XBatch.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "XBatch")));
            }
            try
            {
                db.Entry(XBatch).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!XBatchExists(Id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "XBatch")));
                }
                else
                {
                   // Globals.SendEmail("ssharma@megacube.com.au",null, "Vodafone-SOS WebApi", ex.Message + " " + ex.StackTrace);
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/XBatches/5
        [ResponseType(typeof(XBatch))]
        public async Task<IHttpActionResult> DeleteXBatch(int id)
        {
            XBatch XBatch = await db.XBatches.FindAsync(id);
            if (XBatch == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "XBatch")));
            }
            try
            {
                db.XBatches.Remove(XBatch);
                await db.SaveChangesAsync();
                return Ok(XBatch);
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

        private bool XBatchExists(int id)
        {
            return db.XBatches.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("TrPopulateLRawDataTable", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "XBATCHES", "DATABASE OBJECTS"));

            //Depending upon the constraint failed return appropriate error message
            //if (SqEx.Message.IndexOf("UQ_LBatches_LbBatchNumber_LbCompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return ("Can not insert duplicate Batch");
            //else if (SqEx.Message.IndexOf("FK_LBatches_LBatches_LbParentBatchId", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Batch", "Child Batch"));
            //else if (SqEx.Message.IndexOf("FK_LBatches_LRawData", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Batch", "RAW DATA"));
            //else
            //    //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}