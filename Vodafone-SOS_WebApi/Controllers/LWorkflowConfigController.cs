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
//    public class LWorkFlowConfigsController : ApiController
//    {
//        private VodafoneSOSLiteEntities db = new VodafoneSOSLiteEntities();
//        //This method is commented as we will not send data without companyId filter from api
//        // GET: api/LWorkFlowConfigs
//        public IHttpActionResult GetLWorkFlowConfigs()
//        {
//            var xx = (from aa in db.LWorkFlowConfigs.Include(c => c.GCompany)
//                      select new { aa.Id, aa.RbName, aa.RbDescription, aa.RbCompanyId, aa.GCompany.GcCompanyName }).OrderBy(p => p.RbName);
//            return Ok(xx);
//        }



//        // GET: api/LWorkFlowConfigs?CompanyId=5
//        public IHttpActionResult GetLWorkFlowConfigsByWorkFlowId(int WorkFlowId)
//        {
//            var xx = (from aa in db.LWorkFlowConfigs.Where(p => p.RbCompanyId == CompanyId).Include(c => c.GCompany)
//                      select new { aa.RbName, aa.RbDescription, aa.RbCompanyId, aa.GCompany.GcCompanyName, aa.Id }).OrderBy(p => p.RbName);
//            return Ok(xx);
//        }

//        // GET: api/LWorkFlowConfigs/5
//        [ResponseType(typeof(RBrand))]
//        public async Task<IHttpActionResult> GetRBrand(int id)
//        {
//            var RBrand = db.LWorkFlowConfigs.Where(p => p.Id == id).Include(c => c.GCompany).Select(x => new { x.Id, x.RbCompanyId, x.RbName, x.RbDescription, x.GCompany.GcCompanyName }).FirstOrDefault();
//            if (RBrand == null)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "BRAND")));
//            }
//            return Ok(RBrand);
//        }

//        // PUT: api/LWorkFlowConfigs/5
//        [ResponseType(typeof(void))]
//        public async Task<IHttpActionResult> PutRBrand(int id, RBrand RBrand)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "BRAND")));
//            }

//            if (id != RBrand.Id)
//            {
//                //return BadRequest();
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "BRAND")));
//            }
//            try
//            {
//                db.Entry(RBrand).State = EntityState.Modified;
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                if (!RBrandExists(id))
//                {
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "BRAND")));
//                }
//                else
//                {
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//                }
//            }
//            return StatusCode(HttpStatusCode.NoContent);
//        }

//        // POST: api/LWorkFlowConfigs
//        [ResponseType(typeof(RBrand))]
//        public async Task<IHttpActionResult> PostRBrand(RBrand RBrand)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "BRAND")));
//            }
//            try
//            {
//                db.LWorkFlowConfigs.Add(RBrand);
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//            }
//            return CreatedAtRoute("DefaultApi", new { id = RBrand.Id }, RBrand);
//        }

//        // DELETE: api/LWorkFlowConfigs/5
//        [ResponseType(typeof(RBrand))]
//        public async Task<IHttpActionResult> DeleteRBrand(int id)
//        {
//            RBrand RBrand = await db.LWorkFlowConfigs.FindAsync(id);
//            if (RBrand == null)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "BRAND")));
//            }
//            try
//            {
//                db.LWorkFlowConfigs.Remove(RBrand);
//                await db.SaveChangesAsync();
//                return Ok(RBrand);
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

//        private bool RBrandExists(int id)
//        {
//            return db.LWorkFlowConfigs.Count(e => e.Id == id) > 0;
//        }

//        private string GetCustomizedErrorMessage(Exception ex)
//        {
//            //Convert the exception to SqlException to get the error message returned by database.
//            var SqEx = ex.GetBaseException() as SqlException;

//            //Depending upon the constraint failed return appropriate error message
//            if (SqEx.Message.IndexOf("UQ_RBrads_RbName_RbCompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
//                return ("Can not insert duplicate BRAND NAME");
//            else if (SqEx.Message.IndexOf("FK_LWorkFlowConfigs_LRawData", StringComparison.OrdinalIgnoreCase) >= 0)
//                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "BRAND", "RAW DATA"));
//            else
//                //Something else failed return original error message as retrieved from database
//                return SqEx.Message;
//        }
//    }
//}



using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LWorkflowConfigController : System.Web.Http.ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();


        //check for can create in Workflow for logged In Role
        [HttpGet]
        public IHttpActionResult GetCheckCanCreate(string WorkflowName,string LoggedInRoleId,int CompanyId)
        {
            var ConfigData = db.LWorkFlowConfigs.Where(p => p.RWorkFlow.RwfName == WorkflowName).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).FirstOrDefault();
            if(ConfigData!=null)
            {
                return Ok(ConfigData.LwfcCanCreate);
            }
            return Ok(false);
        }
        // GET: api/GetByWFId
        public IHttpActionResult GetByWFId(int CompanyId, int WorkflowId)
        {
            var xx = (from aa in db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == CompanyId && p.LwfcWorkFlowId == WorkflowId )
                      join ar in db.AspNetRoles on aa.LwfcRoleId equals ar.Id
                      select new {
                          aa.Id,
                          aa.LwfcActingAs,
                          aa.LwfcCanCreate,
                          aa.LwfcDoNotNotify,
                          aa.LwfcDescription,
                          aa.LwfcOrdinalNumber,
                          aa.LwfcWorkFlowId,
                          aa.LwfcRoleId,
                          aa.LwfcCompanyId,
                          aa.LwfcSkip,
                          aa.LwfcSkipFunctionName,
                          ar.Name}).OrderBy(p => p.LwfcOrdinalNumber);
            return Ok(xx);
        }

        public IHttpActionResult GetById(int id)
        {
            var xx = (from aa in db.LWorkFlowConfigs.Where( p => p.Id == id)
                      join ar in db.AspNetRoles on aa.LwfcRoleId equals ar.Id
                      select new
                      {
                          aa.Id,
                          aa.LwfcActingAs,
                          aa.LwfcCanCreate,
                          aa.LwfcDoNotNotify,
                          aa.LwfcDescription,
                          aa.LwfcOrdinalNumber,
                          aa.LwfcWorkFlowId,
                          aa.LwfcRoleId,
                          aa.LwfcCompanyId,
                          aa.LwfcSkip,
                          aa.LwfcSkipFunctionName,
                          ar.Name
                      }).FirstOrDefault();
            if (xx == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW ACTIONITEM")));
            }
            return Ok(xx);
        }
        private bool LWorkFlowConfigExists(int id)
        {

            return db.LWorkFlowConfigs.Count(e => e.Id == id) > 0;
        }

        // DELETE: api/RWorkFlows/5

        public IHttpActionResult DeleteLWorkflowConfig(int id)
        {
            LWorkFlowConfig RWF = db.LWorkFlowConfigs.Find(id);
            if (RWF == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WorkFlowConfig")));
            }
            try
            {
                db.LWorkFlowConfigs.Remove(RWF);
                db.SaveChanges();
                return Ok(RWF);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // POST: api/LWorkFlowConfig
        [ResponseType(typeof(LWorkFlowConfig))]
        public async Task<IHttpActionResult> PostLWorkFlowConfig(LWorkFlowConfig LWorkFlowConfig)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "LWorkFlowConfig")));
            }
            try
            {
                db.LWorkFlowConfigs.Add(LWorkFlowConfig);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
                // throw ex;
            }
            return CreatedAtRoute("DefaultApi", new { id = LWorkFlowConfig.Id }, LWorkFlowConfig);
        }



        // PUT: api/LWorkFlowConfig/5

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLWorkFlowConfig(int id, LWorkFlowConfig LWFC)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "WORKFLOW CONFIG")));
            }

            if (id != LWFC.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "WORKFLOW CONFIG")));
            }
            try
            {
              
                    db.Entry(LWFC).State = EntityState.Modified;
                    await db.SaveChangesAsync();
              
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        public IHttpActionResult GetRolesByCompanyId(int CompanyId)
        {
            //select * from AspNetRoles ar inner join GCompanies gc on ar.CompanyCode=gc.GcCode where gc.Id=6

            var xx = (from ar in db.AspNetRoles
                      join gc in db.GCompanies.Where(p => p.Id == CompanyId)
                      on ar.CompanyCode equals gc.GcCode
                      select new
                      {
                          RoleName = ar.Name,
                          Id = ar.Id
                      });

            return Ok(xx);
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("FnAllocateCommissionBatch", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "WORKFLOW CONFIG", "DATABASE OBJECTS"));
            else if (SqEx.Message.IndexOf("FK_LWorkFlowConfig_LWorkFlowActionItems", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "WORKFLOW CONFIG", "WORKFLOW ACTION ITEM"));
            //else if (SqEx.Message.IndexOf("FK_RWorkFlows_LWorkFlowConfig", StringComparison.OrdinalIgnoreCase) >= 0)
            //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "WORKFLOW CONFIG", "WORKFLOW"));
            else if (SqEx.Message.IndexOf("FK_LWorkFlowConfiguration_LWorkFlowGridColumns", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "WORKFLOW CONFIG", "WORKFLOW GRID COLUMN"));
            
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;

         }

    }
}
