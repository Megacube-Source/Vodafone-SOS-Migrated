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
    public class LWorkFlowActionParametersController : ApiController
    {

        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LWorkFlowActionParameterss/5
        [ResponseType(typeof(LWorkFlowActionParameter))]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var xx = db.LWorkFlowActionParameters.Where(p => p.Id == id).
                Select(p => new
                {
                    p.Id,
                    p.ParameterName,
                    p.ParameterValue,
                    p.ParameterValueType,
                    p.WFActionItemId
                }).FirstOrDefault();
            if (xx == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW ACTION PARAMETER")));
            }
            return Ok(xx);
        }

        // PUT: api/LWorkFlowActionParameterss/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLWorkFlowActionParameters(int id, LWorkFlowActionParameter LWorkFlowActionParameters)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "WORKFLOW ACTION PARAMETER")));
            }

            if (id != LWorkFlowActionParameters.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "WORKFLOW ACTION PARAMETER")));
            }
            try
            {
                db.Entry(LWorkFlowActionParameters).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LWorkFlowActionParametersExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW ACTION PARAMETER")));
                }
                else
                {
                    // throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                    throw ex;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LWorkFlowActionParameterss
        [ResponseType(typeof(LWorkFlowActionParameter))]
        public async Task<IHttpActionResult> PostLWorkFlowActionParameters(LWorkFlowActionParameter LWorkFlowActionParameters)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "WORKFLOW ACTION PARAMETER")));
            }
            try
            {
                //As soon as WORKFLOW GRID COLUMN is created a database trigger will automatically Create a sequence for this WORKFLOW GRID COLUMN which will provide sequence of Claim Ids 
                //while saving Lclaims data and will also add a row in GKeyValues for this sequence Name.
                db.LWorkFlowActionParameters.Add(LWorkFlowActionParameters);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                throw ex;
            }
            return CreatedAtRoute("DefaultApi", new { id = LWorkFlowActionParameters.Id }, LWorkFlowActionParameters);
        }

        // DELETE: api/LWorkFlowActionParameterss/5
        [ResponseType(typeof(LWorkFlowActionParameter))]
        public async Task<IHttpActionResult> DeleteLWorkFlowActionParameters(int id)
        {
            LWorkFlowActionParameter LWorkFlowActionParameters = await db.LWorkFlowActionParameters.FindAsync(id);
            if (LWorkFlowActionParameters == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW ACTION PARAMETER")));
            }
            try
            {
                db.LWorkFlowActionParameters.Remove(LWorkFlowActionParameters);
                await db.SaveChangesAsync();
                return Ok(LWorkFlowActionParameters);
            }
            catch (Exception ex)
            {
                // throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                throw ex;
            }
        }


        private bool LWorkFlowActionParametersExists(int id)
        {
            return db.LWorkFlowActionParameters.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            //Something else failed return original error message as retrieved from database
            return SqEx.Message;
        }

        //method added by SG for getting GridColumn details by configId passed
        public IHttpActionResult GetByActionItemId(int ActionItemId)
        {
            var xx = (from param in db.LWorkFlowActionParameters.Where(param => param.WFActionItemId == ActionItemId)
                     join actions in db.LWorkFlowActionItems on param.WFActionItemId equals actions.Id
                    select new {
                        param.Id,
                        param.ParameterName,
                        param.ParameterValue,
                        param.ParameterValueType,
                        param.WFActionItemId,
                        ActionName = actions.LwfaiActionItemName
                });
            return Ok(xx);
        }






    }
}