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
    public class LWorkFlowActionItemController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LWorkFlowActionItems/5
        [ResponseType(typeof(LWorkFlowActionItem))]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var xx = db.LWorkFlowActionItems.Where(p => p.Id == id).
                Select(p => new
                {
                    p.Id,
                    p.LwfaiActionItemName,
                    p.LwfaiUILabel,
                    p.LwfaiActionDescription,
                    p.LwfaiLoginWFConfigId,
                    p.LwfaiIsButtonOnWfGrid,
                    p.LwfaiIsButtonOnForm,
                    p.LwfaiActionURL,
                    p.LwfaiShowInTabWFConfigId,
                    p.LwfaiOrdinal
                }).FirstOrDefault();
            if (xx == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW ACTIONITEM")));
            }
            return Ok(xx);
        }

        // PUT: api/LWorkFlowActionItems/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLWorkFlowActionItem(int id, LWorkFlowActionItem LWorkFlowActionItem)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "WORKFLOW ACTION ITEM")));
            }

            if (id != LWorkFlowActionItem.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "WORKFLOW ACTION ITEM")));
            }
            try
            {
                db.Entry(LWorkFlowActionItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LWorkFlowActionItemExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW ACTION ITEM")));
                }
                else
                {
                    // throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                    throw ex;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LWorkFlowActionItems
        [ResponseType(typeof(LWorkFlowActionItem))]
        public async Task<IHttpActionResult> PostLWorkFlowActionItem(LWorkFlowActionItem LWorkFlowActionItem)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "WORKFLOW ACTION ITEM")));
            }
            try
            {
               
                db.LWorkFlowActionItems.Add(LWorkFlowActionItem);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = LWorkFlowActionItem.Id }, LWorkFlowActionItem);
        }

        // DELETE: api/LWorkFlowActionItems/5
        [ResponseType(typeof(LWorkFlowActionItem))]
        public async Task<IHttpActionResult> DeleteLWorkFlowActionItem(int id)
        {
            LWorkFlowActionItem LWorkFlowActionItem = await db.LWorkFlowActionItems.FindAsync(id);
            if (LWorkFlowActionItem == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW ACTION ITEM")));
            }
            try
            {
                db.LWorkFlowActionItems.Remove(LWorkFlowActionItem);
                await db.SaveChangesAsync();
                return Ok(LWorkFlowActionItem);
            }
            catch (Exception ex)
            {
                // throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                throw ex;
            }
        }


        private bool LWorkFlowActionItemExists(int id)
        {
            return db.LWorkFlowActionItems.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("FK_LWorkFlowActionItems_LWorkFlowActionParameters", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "WORKFLOW ACTION ITEM", "WORKFLOW ACTION PARAMETER"));
             //Depending upon the constraint failed return appropriate error message
             //Something else failed return original error message as retrieved from database
             else
            return SqEx.Message;
        }

      
        public IHttpActionResult GetByConfigId(int Configid,int RoleId)
        {
            /*select  distinct actions.LwfaiUILabel, actions.Id,actions.LwfaiActionItemName,actions.LwfaiUILabel,actions.LwfaiActionDescription,actions.LwfaiLoginWFConfigId,actions.LwfaiShowInTabWFConfigId,actions.LwfaiActionURL
,roles.Name as LoginConfigId,config.LwfcActingAs
, roles1.Name as ShowInTabConfig
 from LWorkFlowActionItems actions
 inner join  LWorkFlowConfig config on actions.LwfaiLoginWFConfigId= config.Id
 inner join AspNetRoles roles on  config.LwfcRoleId = roles.Id
 inner join LWorkFlowConfig config1 on actions.LwfaiShowInTabWFConfigId = config1.Id
 inner join AspNetRoles roles1 on config1.LwfcRoleId  = roles1.Id
 where actions.LwfaiLoginWFConfigId=11*/

            var xx = (from actions in db.LWorkFlowActionItems.Where(p => p.LwfaiLoginWFConfigId == Configid)
                      join config in db.LWorkFlowConfigs on actions.LwfaiLoginWFConfigId equals config.Id
                      join roles in db.AspNetRoles on config.LwfcRoleId equals roles.Id
                      join config1 in db.LWorkFlowConfigs on actions.LwfaiShowInTabWFConfigId equals config1.Id
                      join roles1 in db.AspNetRoles on config1.LwfcRoleId equals roles1.Id
                      select new {
                          actions.Id,
                          actions.LwfaiActionItemName,
                          actions.LwfaiUILabel,
                          actions.LwfaiActionDescription,
                          actions.LwfaiLoginWFConfigId,
                          actions.LwfaiIsButtonOnWfGrid,
                          actions.LwfaiIsButtonOnForm,
                          actions.LwfaiActionURL,
                          actions.LwfaiShowInTabWFConfigId,
                          actions.LwfaiOrdinal,
                          ActingAs = config.LwfcActingAs,
                          Role = roles.Name,
                          RoleId = config.LwfcRoleId,
                          LoginConfigName = roles.Name  ,
                          ShowInTabConfigName = roles1.Name
                      }).OrderBy(p=>p.LoginConfigName).ThenBy(p=>p.LwfaiOrdinal);



           /* var RoleList = db.LWorkFlowActionItems.Where(p => p.LwfaiLoginWFConfigId == Configid).
                Select(p => new {
                    p.Id,
                    p.LwfaiActionItemName,
                    p.LwfaiUILabel,
                    p.LwfaiActionDescription,
                    p.LwfaiLoginWFConfigId,
                    p.LwfaiIsButtonOnWfGrid,
                    p.LwfaiIsButtonOnForm,
                    p.LwfaiActionURL,
                    p.LwfaiShowInTabWFConfigId,
                    p.LwfaiOrdinal
                });*/
            return Ok(xx);
        }




    }
}