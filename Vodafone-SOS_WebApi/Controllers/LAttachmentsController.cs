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
    public class LAttachmentsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LAttachments
        public IHttpActionResult GetLAttachments(string EntityType, int EntityId)
        {
                var xx = (from aa in db.LAttachments.Where(p => p.LaEntityId == EntityId).Where(p => p.LaEntityType == EntityType)
                          select new { aa.Id, aa.LaFileName, aa.LaFilePath, aa.LaType }).OrderBy(p => p.LaFileName);
                return Ok(xx);
        }

        public IHttpActionResult GetLAttachmentsForPayeeID(string EntityType, int EntityId, string PayeeEmail)
        {
            List<LAttachment> objlst = new List<LAttachment>();
            var LPayeeID = db.LPayees.Where(x => x.LpEmail == PayeeEmail).Select(x => x.Id).FirstOrDefault();
            List<string> AttachmentIDs = db.MDocumentSetsPayees.Where(x => x.MdspPayeeId == LPayeeID & x.MdspDocumentSetId == EntityId).Select(x => x.AttachmentId).ToList();
            if (AttachmentIDs == null || AttachmentIDs.Count == 0 || AttachmentIDs[0] == null)
            {
                var xx = (from aa in db.LAttachments.Where(p => p.LaEntityId == EntityId).Where(p => p.LaEntityType == EntityType)
                          select new { aa.Id, aa.LaFileName, aa.LaFilePath, aa.LaType }).OrderBy(p => p.LaFileName);
                return Ok(xx);
            }
            else
            {


                for (int i = 0; i < AttachmentIDs.Count; i++)
                {
                    int id = Convert.ToInt32(AttachmentIDs[i]);
                    LAttachment data = db.LAttachments.Where(x => x.Id == id).FirstOrDefault();
                    LAttachment objdata = new LAttachment();
                    objdata.Id = data.Id;
                    objdata.LaFileName = data.LaFileName;
                    objdata.LaFilePath = data.LaFilePath;
                    objdata.LaType = data.LaType;
                    objlst.Add(objdata);
                }

                return Ok(objlst);
            }
            //var matches = (from aa in  db.LAttachments
            //              .Where(x => AttachmentIDs.Contains(x.LaEntityType))
            //              select new { aa.Id, aa.LaFileName, aa.LaFilePath, aa.LaType }).OrderBy(p => p.LaFileName);


        }

        // GET: api/LAttachments?CompanyId=5
        //public IHttpActionResult GetLAttachmentsByUploadedFileId(int UploadedFileId)
        //{
        //    var xx = (from aa in db.LAttachments.Where(p => p.LaUploadFileId==UploadedFileId).Include(c => c.LUploadedFile)
        //              select new { aa.LaUploadFileId,aa.LUploadedFile.LufUploadType, aa.Id,aa.LaFileName }).OrderBy(p => p.LaFileName);
        //    return Ok(xx);
        //}
        ////public IHttpActionResult GetLAttachmentsByUserId(int UserId)
        ////{
        ////    var xx = (from aa in db.LAttachments.Where(p =>p.LUploadedFile.LufUserId==UserId).Include(c => c.LUploadedFile.LUser)
        ////              select new { aa.LaUploadFileId, aa.LUploadedFile.LufUploadType, aa.Id, aa.LaFileName }).OrderBy(p => p.LaFileName);
        ////    return Ok(xx);
        ////}

        ////public IHttpActionResult GetLAttachmentsByPayeeId(int PayeeId)
        ////{
        ////    var xx = (from aa in db.LAttachments.Where(p => p.LUploadedFile.LufPayeeId == PayeeId).Include(c => c.LUploadedFile.LPayee)
        ////              select new { aa.LaUploadFileId, aa.LUploadedFile.LufUploadType, aa.Id, aa.LaFileName }).OrderBy(p => p.LaFileName);
        ////    return Ok(xx);
        ////}

        //public IHttpActionResult GetLAttachmentsByClaimId(int ClaimId)
        //{
        //    var xx = (from aa in db.LAttachments.Where(p => p.LUploadedFile.LufClaimId==ClaimId).Include(c => c.LUploadedFile.LClaim)
        //              select new { aa.LaUploadFileId, aa.LUploadedFile.LufUploadType, aa.Id, aa.LaFileName }).OrderBy(p => p.LaFileName);
        //    return Ok(xx);
        //}

        // GET: api/LAttachments/5
        [ResponseType(typeof(LAttachment))]
        public async Task<IHttpActionResult> GetLAttachment(int id)
        {
            var LAttachment = db.LAttachments.Where(p => p.Id == id).Select(x => new { x.Id, x.LaFileName,x.LaFilePath,x.LaType }).FirstOrDefault();
            if (LAttachment == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ATTACHMENT")));
            }
            return Ok(LAttachment);
        }

        // PUT: api/LAttachments/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLAttachment(int id, LAttachment LAttachment)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ATTACHMENT")));
            }

            if (id != LAttachment.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ATTACHMENT")));
            }
            try
            {
                db.Entry(LAttachment).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LAttachmentExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ATTACHMENT")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LAttachments
        [ResponseType(typeof(LAttachment))]
        public async Task<IHttpActionResult> PostLAttachment(LAttachment LAttachment)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ATTACHMENT")));
            }
            try
            {
                db.LAttachments.Add(LAttachment);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = LAttachment.Id }, LAttachment);
        }

        // DELETE: api/LAttachments/5
        [ResponseType(typeof(LAttachment))]
        public async Task<IHttpActionResult> DeleteLAttachment(int id)
        {
            LAttachment LAttachment = await db.LAttachments.FindAsync(id);
            if (LAttachment == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ATTACHMENT")));
            }
            try
            {
                db.LAttachments.Remove(LAttachment);
                await db.SaveChangesAsync();
                return Ok(LAttachment);
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

        private bool LAttachmentExists(int id)
        {
            return db.LAttachments.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("VUploadedFiles", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "ATTATCHMENT", "VIEW(S)"));
            else
            //Something else failed return original error message as retrieved from database
            return SqEx.Message;
        }
    }
}
