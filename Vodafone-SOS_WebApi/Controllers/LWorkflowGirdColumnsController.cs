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
    public class LWorkflowGridColumnsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LWorkflowGridColumns
        public IHttpActionResult GetLWorkflowGridColumns()
        {
            var xx = (from aa in db.LWorkflowGridColumns
                      select new { aa.Id, aa.LwfgcAscDesc,aa.LwfgcColumnName,aa.LwfgcOrderByOrdinal,aa.LwfgcShouldBeVisible,aa.LwfgcUILabel,aa.LwfgcWfConfigId }).OrderBy(p => p.LwfgcOrderByOrdinal);
            return Ok(xx);
        }
        //Used to load Tabs in Workflow Grid
        public IHttpActionResult GetTabsByWorkflowId(int CompanyId,string Workflow)
        {
            var RoleList = db.LWorkFlowConfigs.Where(p => p.RWorkFlow.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).Where(p => p.LwfcCompanyId == CompanyId).Where(p=>p.LwfcOrdinalNumber>0).Select(p=>new { p.AspNetRole.Name,p.Id ,p.LwfcOrdinalNumber}).OrderBy(p=>p.LwfcOrdinalNumber);
            return Ok(RoleList);
        }

        public IHttpActionResult GetLWorkflowGridColumnsByWorkFlowId(string WorkFlow, int CompanyId,string LoggedInRoleId)
        {
            var WorkFlowId = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkFlow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Id;
            // var xx = db.Database.SqlQuery<GenericGridViewModel>("select ISC.DATA_TYPE as DataType, AR.Name as UserRole,WC.LwfcBanner, C.LwfgcOrdinal ,WC.LwfcCanCreate,WC.LwfcOrdinalNumber, WF.RwfBaseTableName,WF.RwfName,WF.RwfCRAllowed,WF.RwfUILabel,C.LwfgcColumnName, C.Id,C.LwfgcAscDesc,C.LwfgcOrderByOrdinal,C.LwfgcShouldBeVisible,C.LwfgcUILabel ,C.LwfgcWfConfigId from LWorkflowGridColumns C join LWorkFlowConfig WC on C.LwfgcWfConfigId=WC.Id inner join RWorkFlows WF on (WC.LwfcWorkFlowId=WF.Id and LwfcRoleId=" + LoggedInRoleId+") inner join AspNetRoles AR on WC.LwfcRoleId=AR.Id inner join INFORMATION_SCHEMA.COLUMNS ISC on (WF.RwfBaseTableName=ISC.Table_Name and C.LwfgcColumnName=ISC.COLUMN_NAME) Where WC.LwfcWorkFlowId=" + WorkFlowId+" and WC.LwfcCompanyId="+CompanyId+ " order by WC.LwfcOrdinalNumber,C.LwfgcOrdinal").ToList();
            //Avoiding SQL injection
            var xx = db.Database.SqlQuery<GenericGridViewModel>("select C.LwfgcJoinTable,C.LwfgcJoinTableColumn, C.LwfgcBaseTableJoinColumn, ISC.DATA_TYPE as DataType, AR.Name as UserRole,WC.LwfcBanner, C.LwfgcOrdinal ,WC.LwfcCanCreate,WC.LwfcOrdinalNumber, WF.RwfBaseTableName,WF.RwfName,WF.RwfCRAllowed,WF.RwfUILabel,C.LwfgcColumnName, C.Id,C.LwfgcAscDesc,C.LwfgcOrderByOrdinal,C.LwfgcShouldBeVisible,C.LwfgcUILabel ,C.LwfgcWfConfigId from LWorkflowGridColumns C join LWorkFlowConfig WC on C.LwfgcWfConfigId = WC.Id inner join RWorkFlows WF on(WC.LwfcWorkFlowId = WF.Id and LwfcRoleId ={0}) inner join AspNetRoles AR on WC.LwfcRoleId = AR.Id inner join INFORMATION_SCHEMA.COLUMNS ISC on(ISC.Table_Name in(WF.RwfBaseTableName,C.LwfgcJoinTable) and C.LwfgcColumnName = ISC.COLUMN_NAME) Where WC.LwfcWorkFlowId = {1} and WC.LwfcCompanyId = {2}  order by WC.LwfcOrdinalNumber,C.LwfgcOrdinal", LoggedInRoleId, WorkFlowId, CompanyId).ToList();
            //var xx = (from aa in db.LWorkflowGridColumns.Where(p=>p.LWorkFlowConfig.LwfcWorkFlowId==WorkFlowId).Include(p=>p.LWorkFlowConfig.RWorkFlow)
            //          select new { aa.Id, aa.LwfgcAscDesc, aa.LwfgcColumnName, aa.LwfgcOrderByOrdinal, aa.LwfgcShouldBeVisible, aa.LwfgcUILabel, aa.LwfgcWfConfigId }).OrderBy(p => p.LwfgcOrderByOrdinal);

            //The following columns used in switch case statement are converted from their Ids to Names so as to display then in Generic Grid
            for (var i = 0; i < xx.Count(); i++)
            {
                if (!string.IsNullOrEmpty(xx[i].LwfgcJoinTable))
                {
                    //DataType = "nvarchar";
                    //This modification in column name assures unique column nams while displaying them on grid
                    xx[i].LwfgcColumnName =  xx[i].LwfgcJoinTable + xx[i].LwfgcOrdinal + "." + xx[i].LwfgcColumnName + "";
                }
                //    switch (xx[i].LwfgcColumnName)
                //    {
                //        case "WFRequesterId":
                //            xx[i].LwfgcColumnName= "WFRequesterName";
                //            break;
                //        case "WFAnalystId":
                //            xx[i].LwfgcColumnName = "WFAnalystName";
                //            break;
                //        case "WFManagerId":
                //            xx[i].LwfgcColumnName = "WFManagerName";
                //            break;
                //        case "WFCurrentOwnerId":
                //            xx[i].LwfgcColumnName = "WFCurrentOwnerName";
                //            break;
                //    }
                //    switch (xx[i].DataType)
                //    {
                //        case "varchar":
                //        case "nvarchar":
                //            xx[i].DataType="string";
                //            break;
                //        case "date":
                //        case "datetime":
                //            xx[i].DataType = "date";
                //            break;
                //        case "bit":
                //            xx[i].DataType = "bool";
                //            break;
                //        case "bigint":
                //            xx[i].DataType = "int";
                //            break;
                //        case "decimal":
                //        case "float":
                //        case "numeric":
                //            xx[i].DataType = "float";
                //            break;


                //    }
            }
            return Ok(xx);
        }
        public IHttpActionResult GetColumnNameByWFId(string WFId)
        {
            //Avoiding SQL Injection
            //var xx = db.Database.SqlQuery<InformationSchemaViewModel>("select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = (select RwfBaseTableName from RWorkFlows where Id =" + WFId +")");
            var xx = db.Database.SqlQuery<InformationSchemaViewModel>("select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = (select RwfBaseTableName from RWorkFlows where Id = {0} )", WFId);
            return Ok(xx);
        }

        // GET: api/LWorkflowGridColumns/5
        [ResponseType(typeof(LWorkflowGridColumn))]
        public async Task<IHttpActionResult> GetLWorkflowGridColumn(int id)
        {
            var LWorkflowGridColumn = db.LWorkflowGridColumns.Where(p => p.Id == id).Select(aa => new { aa.Id, aa.LwfgcOrdinal,aa.LwfgcAscDesc, aa.LwfgcColumnName, aa.LwfgcOrderByOrdinal, aa.LwfgcShouldBeVisible, aa.LwfgcUILabel, aa.LwfgcWfConfigId }).FirstOrDefault();
            if (LWorkflowGridColumn == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW GRID COLUMN")));
            }
            return Ok(LWorkflowGridColumn);
        }

        // PUT: api/LWorkflowGridColumns/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLWorkflowGridColumn(int id, LWorkflowGridColumn LWorkflowGridColumn)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "WORKFLOW GRID COLUMN")));
            }

            if (id != LWorkflowGridColumn.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "WORKFLOW GRID COLUMN")));
            }
            try
            {
                db.Entry(LWorkflowGridColumn).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LWorkflowGridColumnExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW GRID COLUMN")));
                }
                else
                {
                    //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                    throw ex;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LWorkflowGridColumns
        [ResponseType(typeof(LWorkflowGridColumn))]
        public async Task<IHttpActionResult> PostLWorkflowGridColumn(LWorkflowGridColumn LWorkflowGridColumn)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "WORKFLOW GRID COLUMN")));
            }
            try
            {
                //As soon as WORKFLOW GRID COLUMN is created a database trigger will automatically Create a sequence for this WORKFLOW GRID COLUMN which will provide sequence of Claim Ids 
                //while saving Lclaims data and will also add a row in GKeyValues for this sequence Name.
                db.LWorkflowGridColumns.Add(LWorkflowGridColumn);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = LWorkflowGridColumn.Id }, LWorkflowGridColumn);
        }

        // DELETE: api/LWorkflowGridColumns/5
        [ResponseType(typeof(LWorkflowGridColumn))]
        public async Task<IHttpActionResult> DeleteLWorkflowGridColumn(int id)
        {
            LWorkflowGridColumn LWorkflowGridColumn = await db.LWorkflowGridColumns.FindAsync(id);
            if (LWorkflowGridColumn == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW GRID COLUMN")));
            }
            try
            {
                db.LWorkflowGridColumns.Remove(LWorkflowGridColumn);
                await db.SaveChangesAsync();
                return Ok(LWorkflowGridColumn);
            }
            catch (Exception ex)
            {
                //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                throw ex;
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

        private bool LWorkflowGridColumnExists(int id)
        {
            return db.LWorkflowGridColumns.Count(e => e.Id == id) > 0;
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
        public IHttpActionResult GetWorkFlowGridColumnsByConfigId(int ConfigId,int RoleId)
        {
            var xx = (from aa in db.LWorkflowGridColumns.Where(p => p.LwfgcWfConfigId == ConfigId)
                      join bb in db.LWorkFlowConfigs on aa.LwfgcWfConfigId equals bb.Id
                      join cc in db.AspNetRoles on bb.LwfcRoleId equals cc.Id
                      select new {
                          aa.Id,
                          aa.LwfgcColumnName,
                          aa.LwfgcOrderByOrdinal,
                          aa.LwfgcOrdinal,
                          aa.LwfgcShouldBeVisible,
                          aa.LwfgcAscDesc,
                          aa.LwfgcUILabel,
                          aa.LwfgcWfConfigId,
                          aa.LwfgcFunctionName,
                          ActingAs = bb.LwfcActingAs,
                          Role = cc.Name,
                          RoleId = bb.LwfcRoleId
                      }).OrderBy(p => p.LwfgcOrdinal);
            
            return Ok(xx);
        }

    }
}
