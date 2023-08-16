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
    public class LRawDataTablesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        public IHttpActionResult DeRegisterTables(int RawDataTableId)
        {
            try
            {
                var CompanySpecificRawDataMapping = db.LCompanySpecificRawDataColumns.Where(p => p.LcsrdcRawDataTableId == RawDataTableId).ToList();
                db.LCompanySpecificRawDataColumns.RemoveRange(CompanySpecificRawDataMapping);
                db.SaveChanges();
                //In R1.6 we disabled the option of deleting RawData Table
                //var RawDataTable = db.LRawDataTables.Where(p => p.Id == RawDataTableId).FirstOrDefault();
                //db.LRawDataTables.Remove(RawDataTable);
                //db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                {
                    //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry. This may or may not be logged depending upon whether it is a known exception or not (Unknown will be logged).
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
                else
                {
                    //Unknown Scenario Log it into Error Log.This will be caught in Filter Config and will be logged in Error Log.
                    throw ex;
                }
            }
        }

        public IHttpActionResult GetTableColumnsByRawDataTableName(string CompanyCode,string RawDataTableName)
        {
            //var xx = db.Database.SqlQuery<GetTableColumnByTableName>("select 'true' as DisplayOnForm,Replace(ISC.COLUMN_NAME,'X','')as ColumnLabel, ( ISC.COLUMN_NAME +' ('+ISC.DATA_TYPE+' '+Isnull(Convert(Nvarchar(50),ISC.CHARACTER_MAXIMUM_LENGTH),'')+'  '+case  ISC.IS_NULLABLE when 'YES' then 'NULL' else 'NOT NULL' end +')')  as ColumnName,ISC.IS_NULLABLE as IsNullable,ISC.DATA_TYPE as DataType from INFORMATION_SCHEMA.COLUMNS ISC where  ISC.TABLE_SCHEMA='XSchema" + CompanyCode+"' and ISC.TABLE_NAME='"+RawDataTableName+"' and ISC.COLUMN_NAME not in ('XBatchNumber','XStatus','XErrorMessage','XResolution','XCreatedBy','XCreateDateTime','XUpdatedBy','XUpdateDateTime','XTransactionNumber') ").ToList();
            //return Ok(xx);
            var Companydetails = db.GCompanies.Where(p => p.GcCode == CompanyCode).FirstOrDefault();
            var CommissionPeriod = db.LCommissionPeriods.Where(p => p.LcpCompanyId == Companydetails.Id).Where(p => p.LcpDatabaseId.HasValue).FirstOrDefault();
            //var Query = "select 0 as Id,'true' as DisplayOnForm,( ISC.COLUMN_NAME +' ('+ISC.DATA_TYPE+' '+Isnull(Convert(Nvarchar(50),ISC.CHARACTER_MAXIMUM_LENGTH),'')+'  '+case  ISC.IS_NULLABLE when 'YES' then 'NULL' else 'NOT NULL' end +')')  as ColumnName,ISC.Column_Name as ColumnLabel,ISC.IS_NULLABLE as IsNullable,case ISC.DATA_TYPE when 'numeric' then 'decimal' when 'float' then 'decimal' else ISC.DATA_TYPE end as DataType from INFORMATION_SCHEMA.COLUMNS ISC where  ISC.TABLE_SCHEMA='{Schema}' and ISC.TABLE_NAME='" + RawDataTableName + "' order by isc.COLUMN_NAME";
            //RK R2.3 10 Oct 2018, Trimmed the column label to 50 character only as it create issue while save the mapping if the column label length increases from 50 characters
            var Query = "select 0 as Id,'true' as DisplayOnForm,( ISC.COLUMN_NAME +' ('+ISC.DATA_TYPE+' '+Isnull(Convert(Nvarchar(50),ISC.CHARACTER_MAXIMUM_LENGTH),'')+'  '+case  ISC.IS_NULLABLE when 'YES' then 'NULL' else 'NOT NULL' end +')')  as ColumnName,SUBSTRING(ISC.Column_Name,0,51) as ColumnLabel,ISC.IS_NULLABLE as IsNullable,case ISC.DATA_TYPE when 'numeric' then 'decimal' when 'float' then 'decimal' else ISC.DATA_TYPE end as DataType from INFORMATION_SCHEMA.COLUMNS ISC where  ISC.TABLE_SCHEMA='{Schema}' and ISC.TABLE_NAME='" + RawDataTableName + "' order by isc.COLUMN_NAME";
            var xx = Globals.GetQueryResultFromOpcoDB(Companydetails.Id, Query, CommissionPeriod.LcpPeriodName);
            return Ok(xx);
        }

        //Get the tables list from XSchema for the current opco
        public IHttpActionResult GetXTablesList(string CompanyCode)
        {
            var CompanyDetails = db.GCompanies.Where(p => p.GcCode == CompanyCode).FirstOrDefault();
            //Select Top 1 Database ID from LCommissionPeriods where getdate() between EffectiveStartDate and EffectiveEndDate and CompanyId = <current company>
            var CurrentDate = DateTime.UtcNow.Date;
            var TomorrowDate = DateTime.UtcNow.AddDays(1).Date;
            var CommPeriod=db.LCommissionPeriods.Where(p=>p.LcpCompanyId==CompanyDetails.Id).Where(p=>CurrentDate>=((p.LcpEffectiveStartDate.HasValue)?p.LcpEffectiveStartDate.Value:CurrentDate)&&CurrentDate<((p.LcpEffectiveEndDate.HasValue)?p.LcpEffectiveEndDate.Value:TomorrowDate)).FirstOrDefault();
            //var ListOfMappedRawDataTables = db.LCompanySpecificRawDataColumns.Select(p => p.LcsrdcRawDataTableId).Distinct().ToList();
            if(CommPeriod==null)
            {
               Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "LRawDataTables", "GetXTablesList","Commission Period does not exist for Opco :-"+CompanyCode, "", "Warning", "", "", "L2 Admin", null, null, "New");
                return Ok();
            }
            var Query = "select 0 as RawDataTableId,0 as IsRawDataTableMapped,TABLE_NAME from INFORMATION_SCHEMA.TABLES ISC where TABLE_SCHEMA='{Schema}' and TABLE_TYPE='BASE TABLE' and TABLE_NAME not in ('XCalc','XPay','XBatches','XBatchPortfolios')";
            var xx = Globals.GetQueryResultFromOpcoDB(CompanyDetails.Id,Query,CommPeriod.LcpPeriodName);
            foreach (DataRow RawDataTable in xx.Rows)
            {
                var tableName = RawDataTable.Field<string>("TABLE_NAME");
                var LRawdataTable = db.LRawDataTables.Where(p => p.LrdtName == tableName).FirstOrDefault();
                if (LRawdataTable != null)
                {
                if (db.LCompanySpecificRawDataColumns.Where(p=>p.LcsrdcRawDataTableId==LRawdataTable.Id).Count()>0)
                {
                    RawDataTable["IsRawDataTableMapped"] = true;
                }
                else
                {
                    RawDataTable["IsRawDataTableMapped"] = false;
                }
                    RawDataTable["RawDataTableId"] = LRawdataTable.Id;
                }
            }
            return Ok(xx);
        }

        // GET: api/LRawDataTables
        public IHttpActionResult GetLRawDataTables()
        {
            var xx = (db.LRawDataTables.Include(c => c.GCompany).Select(aa=> new { aa.Id, aa.LrdtDescription, aa.LrdtName })).OrderBy(p => p.LrdtName).ToList();
            return Ok(xx);
        }

        //Method to get company specific data for dropdown in WebApplication
        public IHttpActionResult GetLRawDataTablesDropdownData(int CompanyId)
        {
            var data = db.LRawDataTables.Where(p => p.LrdtCompanyId == CompanyId).Select(x => new { x.LrdtName, x.Id }).OrderBy(p => p.LrdtName).AsEnumerable();
            return Ok(data);
        }

        // GET: api/LRawDataTables?CompanyId=5
        public IHttpActionResult GetLRawDataTablesByCompanyId(int CompanyId)
        {
            var xx = (from aa in db.LRawDataTables.Where(p => p.LrdtCompanyId == CompanyId).Include(c => c.GCompany)
                      select new {aa.Id,aa.LrdtCompanyId,aa.LrdtDescription,aa.LrdtName }).OrderBy(p => p.LrdtName);
            return Ok(xx);
        }

        // GET: api/LRawDataTables/5
        [ResponseType(typeof(LRawDataTable))]
        public async Task<IHttpActionResult> GetLRawDataTable(int id)
        {
            var LRawDataTable = db.LRawDataTables.Where(p => p.Id == id).Include(c => c.GCompany).FirstOrDefault();
            if (LRawDataTable == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RAW DATA TABLE")));
            }
            return Ok(LRawDataTable);
        }

        // PUT: api/LRawDataTables/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLRawDataTable(int id, LRawDataTable LRawDataTable)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RAW DATA TABLE")));
            }

            if (id != LRawDataTable.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "RAW DATA TABLE")));
            }
            try
            {
                db.Entry(LRawDataTable).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LRawDataTableExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RAW DATA TABLE")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LRawDataTables
        [ResponseType(typeof(LRawDataTable))]
        public async Task<IHttpActionResult> PostLRawDataTable(LRawDataTable LRawDataTable)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "RAW DATA TABLE")));
            }
            try
            {
                db.LRawDataTables.Add(LRawDataTable);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { id = LRawDataTable.Id }, LRawDataTable);
        }

        // DELETE: api/LRawDataTables/5
        [ResponseType(typeof(LRawDataTable))]
        public async Task<IHttpActionResult> DeleteLRawDataTable(int id)
        {
            LRawDataTable LRawDataTable = await db.LRawDataTables.FindAsync(id);
            if (LRawDataTable == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "RAW DATA TABLE")));
            }
            try
            {
                db.LRawDataTables.Remove(LRawDataTable);
                await db.SaveChangesAsync();
                return Ok(LRawDataTable);
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

        private bool LRawDataTableExists(int id)
        {
            return db.LRawDataTables.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("UQ_LRawDataTypes_CompanyId_Name", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "RAW DATA TYPES"));
            else if (SqEx.Message.IndexOf("FK_LRawDataTypes_LBatches", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "RAW DATA TYPES", "BATCHES"));
            else if (SqEx.Message.IndexOf("FK_LRawDataTables_LCompanySpecificRawDataColumns", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "RAW DATA TYPES", "INTEGRATION COLUMN MAPPING"));
            else
            return SqEx.Message;
        }
    }
}
