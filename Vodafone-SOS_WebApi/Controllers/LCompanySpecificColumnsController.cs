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
    public class LCompanySpecificColumnsController : ApiController
    {
            private SOSEDMV10Entities db = new SOSEDMV10Entities();

            // GET: api/LCompanySpecificColumns
            //public IHttpActionResult GetLCompanySpecificColumns()
            //{
            //    var xx = (from aa in db.LCompanySpecificColumns.Include(c => c.GCompany)
            //              select new { aa.Id, aa.LcscTableName, aa.LcscColumnName, aa.LcscLabel, aa.LcscCompanyId, aa.GCompany.GcCompanyName,aa.LcscDisplayOnForm }).OrderBy(p => p.LcscLabel);
            //    return Ok(xx);
            //}

        public IHttpActionResult DeleteAllColumns(string FormType,int CompanyId)
            {
                if (FormType == "Payee")
                {
                    db.Database.ExecuteSqlCommand("delete from LCompanySpecificColumns where LcscTableName='LPayees' and LcscCompanyId={0}",CompanyId);
                }
            else if(FormType=="Claims")
            {
                db.Database.ExecuteSqlCommand("delete from LCompanySpecificColumns where LcscTableName='LClaims' and  LcscCompanyId={0}",CompanyId);
            }
            else if (FormType == "Calculations")
            {
                db.Database.ExecuteSqlCommand("delete from LCompanySpecificColumns where LcscTableName='XCalc' and  LcscCompanyId={0}", CompanyId);
            }
            else if (FormType.Equals("Pay"))
            {
                db.Database.ExecuteSqlCommand("delete from LCompanySpecificColumns where LcscTableName='XPay' and  LcscCompanyId={0}", CompanyId);
            }
            return Ok();
            }

        public IHttpActionResult GetColumnsForDownloadTemplateForClaims(int CompanyId)
        {
            var xx = db.Database.SqlQuery<CompanySpecificColumnViewModel>("select distinct LCSC1.LcscColumnName,LCSC1.LcscDisplayOnForms,IsNull(LCSC1.LcscLabel,LCSC2.LcscLabel) as LcscLabel  from LCompanySpecificColumns LCSC1 inner join LCompanySpecificColumns LCSC2 on LCSC1.LcscColumnName=LCSC2.LcscColumnName where LCSC1.LcscTableName='LClaims' and LCSC2.LcscCompanyId=(select Id from GCompanies where GcCode='99') and LCSC1.LcscDisplayOnForms=1 and LCSC1.LcscCompanyId={0}" , CompanyId).ToList();
            return Ok(xx);
        }

        ////get claims company specified columns
        //public IHttpActionResult GetLCompanySpecificRawDataColumnsForClaimsByCompanyId(int CompanyId)
        //{
        //    var yy = db.Database.SqlQuery<CompanySpecificColumnViewModel>("select Replace(COLUMN_NAME ,'Lc','') as ColumnName,IS_NULLABLE as IsNullable from INFORMATION_SCHEMA.COLUMNS ISC join LCompanySpecificRawDataColumns LCSC on Replace(ISC.COLUMN_NAME ,'Lc','')=LCSC.LcscColumnNamewhere LCSC.LcscTableName='LClaims' and ISC.TABLE_NAME='LClaims'and ISC.Column_Name not in ('Id','LcCompanyId','LcCreatedBy','LcUpdatedById','LcStatusId','LcAllocatedToId','LcAllocatedById','LcRejectionReasonId','LcPaymentBatchId','LcApprovedById','LcSentForApprovalById','LcWithdrawnById','LcRejectedById','LcClaimId','LcCommentsExternal')");
        //    return Ok(yy);
        //}
            // GET: api/LCompanySpecificColumns?CompanyId=5 this method gets the column specified for payees table
            public IHttpActionResult GetPayeeLCompanySpecificColumnsByCompanyId(int CompanyId)
            {
                var yy = db.Database.SqlQuery<CompanySpecificColumnViewModel>("select LCSC.LcscIsMandatory,Lcsc.LcscDataType,Lcsc.LcscIsReportParameter,Lcsc.LcscReportParameterOrdinal,Lcsc.LcscDropDownId, LCSC.LcscOrdinalPosition,LCSC.LcscColumnName,LCSC.LcscDisplayOnForm,LCSC.Id,LCSC.LcscLabel,LCSC.LcscTooltip from  LCompanySpecificColumns LCSC where LCSC.LcscTableName='LPayees' and LCSC.LcscCompanyId={0}", CompanyId).ToList();
                return Ok(yy);
            }
            //get claims company specified columns
            public IHttpActionResult GetLCompanySpecificColumnsForClaimsByCompanyId(int CompanyId)
            {
                var yy = db.Database.SqlQuery<CompanySpecificColumnViewModel>("select LCSC.LcscIsMandatory,Lcsc.LcscIsReportParameter,Lcsc.LcscDataType,Lcsc.LcscReportParameterOrdinal,Lcsc.LcscDropDownId, LCSC.LcscOrdinalPosition, LCSC.LcscColumnName,LCSC.LcscDisplayOnForm,LCSC.Id,LCSC.LcscLabel,LCSC.LcscTooltip from LCompanySpecificColumns LCSC where LCSC.LcscTableName='LClaims' and  LCSC.LcscCompanyId={0}", CompanyId ).ToList();
                return Ok(yy);
            }

            public IHttpActionResult GetPayeeLCompanySpecificColumnsByCompanyIdForGrid(int CompanyId)
            {
                    //RK 14122018 this method will return all the columns from lpayees when configuration is not saved for any company
                    //Any exclusion will be mentioned in the not in clause of query
                    //RK 14122018: added LpBlockNotifiation to the exclusion list
                var yy = db.Database.SqlQuery<CompanySpecificColumnViewModel>("select (select LdName from LDropDowns where Id=Lcsc.LcscDropDownId) as LdName,Lcsc.LcscDataType, LCSC.LcscIsMandatory,Lcsc.LcscDropDownId, LCSC.LcscOrdinalPosition, Replace(ISC.COLUMN_NAME ,'Lp','') as ColumnName,ISC.IS_NULLABLE as IsNullable,LCSC.LcscColumnName,LCSC.LcscDisplayOnForm,LCSC.Id,LCSC.LcscTooltip,LCSC.LcscLabel from INFORMATION_SCHEMA.COLUMNS ISC inner join LCompanySpecificColumns LCSC on Replace(ISC.COLUMN_NAME ,'Lp','')=LCSC.LcscColumnName where LCSC.LcscTableName='LPayees' and LCSC.LcscCompanyId={0} and  ISC.TABLE_NAME='LPayees'and ISC.Column_Name not in ('Id','LpUserId','LpUserId','LpCompanyId','LpStatusId','LpCreatedById','LpUpdatedById','LpBatchId','LpComments','LpFileNames','LpUserFriendlyFileNames','LpCreatedDateTime','LpUpdatedDateTime','WFRequesterId','WFAnalystId','WFManagerId','WFOrdinal','WFCurrentOwnerId','WFStatus','WFType','WFRequesterRoleId','WFCompanyId','WFComments','LpBusinessUnit','LpSubChannelId','LpChannelId','LpPrimaryChannel','LpCreatedByForm','ParameterCarrier','LpBlockNotification') order by LCSC.LcscDisplayOnForm desc, LcscOrdinalPosition", CompanyId).ToList();
                return Ok(yy);
            }
            //get claims company specified columns
            public IHttpActionResult GetLCompanySpecificColumnsForClaimsByCompanyIdGrid(int CompanyId)
            {
                var yy = db.Database.SqlQuery<CompanySpecificColumnViewModel>("select (select LdName from LDropDowns where Id=Lcsc.LcscDropDownId) as LdName,Lcsc.LcscDataType,Lcsc.LcscIsReportParameter,Lcsc.LcscReportParameterOrdinal, LCSC.LcscIsMandatory,Lcsc.LcscDropDownId, LCSC.LcscOrdinalPosition, LCSC.LcscColumnName,LCSC.LcscDisplayOnForm,LCSC.Id,LCSC.LcscLabel,LCSC.LcscTooltip, ISC.IS_NULLABLE as IsNullable from INFORMATION_SCHEMA.COLUMNS ISC inner join LCompanySpecificColumns LCSC on Replace(ISC.COLUMN_NAME ,'Lc','')=LCSC.LcscColumnName where LCSC.LcscTableName='LClaims' and  LCSC.LcscCompanyId={0} and ISC.TABLE_NAME='LClaims'and ISC.Column_Name not in ('Id','LcCompanyId','LcCreatedBy','LcUpdatedById','WFRequesterId','WFAnalystId','WFManagerId','WFOrdinal','WFCurrentOwnerId','WFStatus','WFType','WFRequesterRoleId','WFCompanyId','LcStatusId','LcAllocatedToId','LcAllocatedById','LcPaymentBatchId','LcApprovedById','LcSentForApprovalById','LcWithdrawnById','LcRejectedById','LcClaimId','LcCommentsExternal','LcIsDuplicateClaim','LcDuplicateClaimDetails','LcCommentsInternal','LcIsReclaim','LcSentForApprovalDate','LcApprovalDate','LcPaymentDate','LcWithdrawnDate','LcRejectionDate','CreatedByForm','CreatedDateTime','ParameterCarrier','WFComments') order by LCSC.LcscDisplayOnForm desc, LcscOrdinalPosition", CompanyId).ToList();
                return Ok(yy);
            }
        //get calculation company specific columns
        public IHttpActionResult GetLCompanySpecificColumnsForCalculationsByCompanyIdGrid(int CompanyId)
        {
            var CompanyDetails = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
            var SqlQuery = "select (select LdName from LDropDowns where Id=Lcsc.LcscDropDownId) as LdName,Lcsc.LcscDataType,Lcsc.LcscIsReportParameter,Lcsc.LcscReportParameterOrdinal, LCSC.LcscIsMandatory,Lcsc.LcscDropDownId, LCSC.LcscOrdinalPosition, LCSC.LcscColumnName as ColumnName,LCSC.LcscDisplayOnForm,LCSC.Id,LCSC.LcscLabel from LCompanySpecificColumns LCSC  where LCSC.LcscTableName='XCalc'  and LCSC.LcscCompanyId={0}  order by LCSC.LcscDisplayOnForm desc, LcscOrdinalPosition";
            var yy = db.Database.SqlQuery<CompanySpecificColumnViewModel>(SqlQuery,CompanyId);
            //JS directed to get Max Period code where databaseid is not null in any opco
            var CommissionPeriod = db.LCommissionPeriods.Where(p => p.LcpCompanyId == CompanyId).Where(p => p.LcpDatabaseId.HasValue).Where(p=>p.LcpPeriodCode.HasValue).OrderByDescending(p => p.LcpPeriodCode).FirstOrDefault();
            if(yy.Count()==0)
            {
                SqlQuery= "select ISC.Column_Name as ColumnName,ISC.IS_NULLABLE as IsNullable ,ISC.DATA_TYPE as DataType from INFORMATION_SCHEMA.COLUMNS ISC where ISC.TABLE_NAME='XCalc'and ISC.TABLE_SCHEMA='{Schema}'";
                var CompanySpecificColumns = Globals.GetQueryResultFromOpcoDB(CompanyId,SqlQuery,CommissionPeriod.LcpPeriodName);
                return Ok(CompanySpecificColumns);
            }
            return Ok(yy);
        }


        [HttpGet]
        public IHttpActionResult GetBannerTextValue(int CompanyId, string formName)
        {
            //string Value = (from aa in db.LCompanySpecificForms.Where(aa => aa.CompanyID == CompanyId).Where(p => p.FormName == formName)
            //                select aa);
            //return Value;           

            var xx = (from aa in db.LCompanySpecificForms.Where(aa => aa.CompanyID == CompanyId).Where(p => p.FormName == formName)
                      select new { aa.BannerText, aa.Id }).FirstOrDefault();
            return Ok(xx);
        }


        //[HttpGet]
        //public IHttpActionResult GetBannerDetailById(int id)
        //{
                   

        //    var xx = (from aa in db.LCompanySpecificForms.Where(aa => aa.Id == id)
        //              select aa).FirstOrDefault();
        //    return Ok(xx);
        //}

        //method used to get the detail of selected config for updation
        [ResponseType(typeof(LCompanySpecificForm))]
        public async Task<IHttpActionResult> GetBannerDetailById(int Id)
        {
            var data = db.LCompanySpecificForms.Where(p => p.Id == Id).Select(x => new { x.Id, x.BannerText,x.CompanyID,x.FormName}).FirstOrDefault();
            if (data == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "data")));
            }
            return Ok(data);
        }


        //method to save the updated config data
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(int Id, LCompanySpecificForm lcs)  /*int Id, LCompanySpecificForm lcs*/
        {
            //LCompanySpecificForm lcs = new LCompanySpecificForm();
            //lcs.Id = 43;
            //lcs.BannerText = "RS_23";
            //    lcs.CompanyID = 5;
            //lcs.FormName = "Claims";

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "LCompanySpecific")));
            }

            if (Id != lcs.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "LCompanySpecific")));
            }
            try
            {
                db.Entry(lcs).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LCompanySpecificFormsExists(Id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "LCompanySpecific")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        private bool LCompanySpecificFormsExists(int Id)
        {
            return db.LCompanySpecificForms.Count(e => e.Id == Id) > 0;
        }
        //get pay company specific columns
        public IHttpActionResult GetLCompanySpecificColumnsForPayByCompanyIdGrid(int CompanyId)
        {
            var CompanyDetails = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
            var SqlQuery = "select (select LdName from LDropDowns where Id=Lcsc.LcscDropDownId) as LdName,Lcsc.LcscDataType,Lcsc.LcscIsReportParameter,Lcsc.LcscReportParameterOrdinal, LCSC.LcscIsMandatory,Lcsc.LcscDropDownId, LCSC.LcscOrdinalPosition, LCSC.LcscColumnName as ColumnName,LCSC.LcscDisplayOnForm,LCSC.Id,LCSC.LcscLabel from LCompanySpecificColumns LCSC  where LCSC.LcscTableName='XPay'  and LCSC.LcscCompanyId={0}  order by LCSC.LcscDisplayOnForm desc, LcscOrdinalPosition";
            var yy = db.Database.SqlQuery<CompanySpecificColumnViewModel>(SqlQuery, CompanyId);
            //JS directed to get Max Period code where databaseid is not null in any opco
            var CommissionPeriod = db.LCommissionPeriods.Where(p => p.LcpCompanyId == CompanyId).Where(p => p.LcpDatabaseId.HasValue).Where(p => p.LcpPeriodCode.HasValue).OrderByDescending(p => p.LcpPeriodCode).FirstOrDefault();
            if (yy.Count() == 0)
            {
                SqlQuery = "select ISC.Column_Name as ColumnName,ISC.IS_NULLABLE as IsNullable ,ISC.DATA_TYPE as DataType from INFORMATION_SCHEMA.COLUMNS ISC where ISC.TABLE_NAME='XPay'and ISC.TABLE_SCHEMA='{Schema}'";
                var CompanySpecificColumns = Globals.GetQueryResultFromOpcoDB(CompanyId, SqlQuery, CommissionPeriod.LcpPeriodName);
                return Ok(CompanySpecificColumns);
            }
            return Ok(yy);
        }


        // GET: api/LCompanySpecificColumns/5
        [ResponseType(typeof(LCompanySpecificColumn))]
            public async Task<IHttpActionResult> GetCompanySpecificColumn(int Id)
            {
                var CompanySpecificColumn = db.LCompanySpecificColumns.Where(p => p.Id == Id).Include(c => c.GCompany).Select(x => new { x.LcscDisplayOnForm,x.Id, x.LcscCompanyId, x.LcscTableName, x.LcscColumnName, x.LcscLabel, x.GCompany.GcCompanyName }).FirstOrDefault();
                if (CompanySpecificColumn == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "COMPANY")));
                }
                return Ok(CompanySpecificColumn);
            }

            // PUT: api/LCompanySpecificColumns/5
            [ResponseType(typeof(void))]
            public async Task<IHttpActionResult> PutCompanySpecificColumn(int Id, LCompanySpecificColumn CompanySpecificColumn)
            {
                if (!ModelState.IsValid)
                {
                    //return BadRequest(ModelState);
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "COMPANY")));
                }

                if (Id != CompanySpecificColumn.Id)
                {
                    //return BadRequest();
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "COLUMN")));
                }
                try
                {
                    db.Entry(CompanySpecificColumn).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (!CompanySpecificColumnExists(Id))
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "COLUMN")));
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                    }
                }
                return StatusCode(HttpStatusCode.NoContent);
            }

            // POST: api/LCompanySpecificColumns
            [ResponseType(typeof(LCompanySpecificColumn))]
            public async Task<IHttpActionResult> PostCompanySpecificColumn(LCompanySpecificColumn CompanySpecificColumn)
            {
                if (!ModelState.IsValid)
                {
                    //return BadRequest(ModelState);
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "COLUMN")));
                }
                try
                {
                    db.LCompanySpecificColumns.Add(CompanySpecificColumn);
                
                await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
                return CreatedAtRoute("DefaultApi", new { Id = CompanySpecificColumn.Id }, CompanySpecificColumn);
            }

        // POST: api/LCompanySpecificColumns
        [ResponseType(typeof(LCompanySpecificForm))]
        public async Task<IHttpActionResult> PostCompanySpecificForm(LCompanySpecificForm CompanySpecificForm)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "COLUMN")));
            }
            try
            {
                db.LCompanySpecificForms.Add(CompanySpecificForm);

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { Id = CompanySpecificForm.Id }, CompanySpecificForm);
        }

        // DELETE: api/LCompanySpecificColumns/5
        [ResponseType(typeof(LCompanySpecificColumn))]
            public async Task<IHttpActionResult> DeleteCompanySpecificColumn(int Id)
            {
                LCompanySpecificColumn CompanySpecificColumn = await db.LCompanySpecificColumns.FindAsync(Id);
                if (CompanySpecificColumn == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "COLUMN")));
                }
                try
                {
                    db.LCompanySpecificColumns.Remove(CompanySpecificColumn);
                    await db.SaveChangesAsync();
                    return Ok(CompanySpecificColumn);
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

            private bool CompanySpecificColumnExists(int Id)
            {
                return db.LCompanySpecificColumns.Count(e => e.Id == Id) > 0;
            }

            private string GetCustomizedErrorMessage(Exception ex)
            {
                //Convert the exception to SqlException to get the error message returned by database.
                var SqEx = ex.GetBaseException() as SqlException;

                ////Depending upon the constraint failed return appropriate error message
                //if (SqEx.Message.IndexOf("UQ_RBrads_RbBrandName_RbCompanyId", StringComparison.OrdinalIgnoreCase) >= 0) 
                //    return ("Can not insert duplicate BRAND NAME");
                //else if (SqEx.Message.IndexOf("FK_LCompanySpecificColumns_LRawData", StringComparison.OrdinalIgnoreCase) >= 0)
                //    return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "BRAND", "RAW DATA"));
                //else
                    //Something else failed return original error message as retrieved from database
                    return SqEx.Message;
            }
        }
}
