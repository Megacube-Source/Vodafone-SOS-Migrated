//Code review and fix SQL to LINQ completed for necessary methods. Didn't touch methods that are not pron for SQL injection.

using System;
//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
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
    public class LCompanySpecificRawDataColumnsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LCompanySpecificRawDataColumns
        //public IHttpActionResult GetLCompanySpecificRawDataColumns()
        //{
        //    var xx = (from aa in db.LCompanySpecificRawDataColumns.Include(c => c.LRawDataTable)
        //              select new { aa.Id, aa.LcsrdcDataType, aa.LcsrdcDisplayLabel, aa.LcsrdcIsDisplayable, aa.LcsrdcLColumnName, aa.LcsrdcOrdinalPosition, aa.LcsrdcRawDataTableId, aa.LcsrdcXColumnName, aa.LRawDataTable.LrdtName }).OrderBy(p => p.LcsrdcOrdinalPosition);
        //    return Ok(xx);
        //}

        //public IHttpActionResult DeleteAllPayeeColumns()
        //{
        //    db.Database.ExecuteSqlCommand("delete from LCompanySpecificRawDataColumns where LcscTableName='LPayees'");
        //    return Ok();
        //}

        //This method loads the raw data table column list to be displayed in grid
        public IHttpActionResult GetRawDataColumnsForGridByRawDataTableId(int RawDataTableId)
        {
            var RawDataColumns = (from aa in db.LCompanySpecificRawDataColumns
                                  .Where(p => p.LcsrdcRawDataTableId == RawDataTableId)
                                  .Where(p => p.LcsrdcIsDisplayable == true)
                                  select new
                                  {
                                      aa.LcsrdcXColumnName,
                                      aa.LcsrdcIsDisplayable,
                                      aa.LcsrdcDisplayLabel,
                                      LcsrdcDataType =  aa.LcsrdcDataType.Equals("varchar") ? "string" :
                                                        aa.LcsrdcDataType.Equals("nvarchar") ? "string" :
                                                        aa.LcsrdcDataType.Equals("int") ? "int" :
                                                        aa.LcsrdcDataType.Equals("date") ? "date" :
                                                        aa.LcsrdcDataType.Equals("datetime") ? "date" :
                                                        aa.LcsrdcDataType.Equals("bit") ? "bool" :
                                                        aa.LcsrdcDataType.Equals("numeric") ? "double" :
                                                        aa.LcsrdcDataType.Equals("decimal") ? "double" :
                                                        aa.LcsrdcDataType.Equals("float") ? "double" :
                                                        aa.LcsrdcDataType.Equals("bigint") ? "int" : "string",
                                      aa.LcsrdcOrdinalPosition}).OrderBy(p => p.LcsrdcOrdinalPosition);

            //RawDataColumns.Insert(0,  new { LcsrdcLColumnName = "AlteryxTransactionNumber", LcsrdcIsDisplayable = true, LcsrdcDisplayLabel = "Transaction Number", LcsrdcDataType = "int", LcsrdcOrdinalPosition = 0});
            return Ok(RawDataColumns);
        }

        // GET: This method returns list of columns of LRawData table
        public IHttpActionResult GetLCompanySpecificRawDataColumnNames()
        {
            var yy = db.Database.SqlQuery<CompanySpecificColumnViewModel>("SELECT(COLUMN_NAME + ' (' + DATA_TYPE+ ' ' + Isnull(Convert(Nvarchar(50), CHARACTER_MAXIMUM_LENGTH),'') + '  ' + CASE IS_NULLABLE WHEN 'YES' THEN 'NULL' ELSE 'NOT NULL' END + ')')  AS ColumnName, IS_NULLABLE AS IsNullable, DATA_TYPE AS DataType FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LRawData' ORDER BY COLUMN_NAME ");
            return Ok(yy);
        }

        //call stored procedure to get columns list from XSchema tables
        //This stored procedure will return Column Name and Type of XSchema table
        public IHttpActionResult GetCompanySpecificColumnsofXSchema(int RawDataTableId)
        {
            //issue parked by ss will disscuss with js 12 dec 2017
            var RawDataTableDetails = db.LRawDataTables.Where(p => p.Id == RawDataTableId).FirstOrDefault();
            //  var xx = db.SpGetColumnNamesForXSchemaTable(RawDataTableId).AsEnumerable();
            //var Query = "select  ( LcsrdcXColumnName +' ('+LcsrdcDataType+'  NULL)')  as ColumnName,REPLACE(LcsrdcDisplayLabel,'X','') as ColumnLabel,'YES' as IsNullable,case LcsrdcDataType when 'numeric' then 'decimal' when 'float' then 'decimal' else LcsrdcDataType  end as DataType from LCompanySpecificRawDataColumns where  LcsrdcRawDataTableId="+RawDataTableId+" order by ColumnName";
            //var xx = db.Database.SqlQuery<get>(Query);
            var CompanySpecificRawDataCol = db.LCompanySpecificRawDataColumns.Where(p => p.LcsrdcRawDataTableId == RawDataTableId).Select(p=>new { ColumnName=p.LcsrdcXColumnName + " (" + p.LcsrdcDataType + " NULL" +")", ColumnLabel=p.LcsrdcDisplayLabel,DataType=p.LcsrdcDataType,p.Id, DisplayOnForm=p.LcsrdcIsDisplayable,Ordinal=p.LcsrdcOrdinalPosition,p.LcsrdcRawDataTableId }).OrderBy(p=>p.Ordinal).ToList();
            if(CompanySpecificRawDataCol.Count()>0)
            {
                return Ok(CompanySpecificRawDataCol);
            }
            var CommissionPeriod = db.LCommissionPeriods.Where(p => p.LcpCompanyId == RawDataTableDetails.LrdtCompanyId).Where(p => p.LcpDatabaseId.HasValue).FirstOrDefault();
           var Query = "select  0 as Ordinal,0 as Id,'true' as DisplayOnForm,( ISC.COLUMN_NAME +' ('+ISC.DATA_TYPE+' '+Isnull(Convert(Nvarchar(50),ISC.CHARACTER_MAXIMUM_LENGTH),'')+'  '+case  ISC.IS_NULLABLE when 'YES' then 'NULL' else 'NOT NULL' end +')')  as ColumnName,ISC.Column_Name as ColumnLabel,ISC.IS_NULLABLE as IsNullable,case ISC.DATA_TYPE when 'numeric' then 'decimal' when 'float' then 'decimal' else ISC.DATA_TYPE end as DataType from INFORMATION_SCHEMA.COLUMNS ISC where  ISC.TABLE_SCHEMA='{Schema}' and ISC.TABLE_NAME='" + RawDataTableDetails.LrdtName+"' order by isc.COLUMN_NAME";
            var xx=Globals.GetQueryResultFromOpcoDB(RawDataTableDetails.LrdtCompanyId,Query,CommissionPeriod.LcpPeriodName);
            return Ok(xx);
        }

        //Delete all columns of that raw data table
        public IHttpActionResult DeleteAllRowsByRawDataTableId(int RawDataTableId)
        {
            var Query = db.Database.ExecuteSqlCommand("DELETE FROM LCompanySpecificRawDataColumns WHERE LcsrdcRawDataTableId = @0", RawDataTableId);
            return Ok();
        }

        // GET: api/LCompanySpecificRawDataColumns/5
        [ResponseType(typeof(LCompanySpecificRawDataColumn))]
        public async Task<IHttpActionResult> GetCompanySpecificColumn(int Id)
        {
            var CompanySpecificColumn = db.LCompanySpecificRawDataColumns.Where(p => p.Id == Id).Include(c => c.LRawDataTable.GCompany).Select(x => new { x.Id, x.LcsrdcDataType, x.LcsrdcDisplayLabel, x.LcsrdcIsDisplayable, x.LcsrdcOrdinalPosition, x.LcsrdcRawDataTableId, x.LcsrdcXColumnName }).FirstOrDefault();
            if (CompanySpecificColumn == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "COMPANY")));
            }
            return Ok(CompanySpecificColumn);
        }

        // PUT: api/LCompanySpecificRawDataColumns/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCompanySpecificColumn(int Id, LCompanySpecificRawDataColumn CompanySpecificColumn)
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

        // POST: api/LCompanySpecificRawDataColumns
        [ResponseType(typeof(LCompanySpecificRawDataColumn))]
        public async Task<IHttpActionResult> PostRawDataColumnsGridData(GridDataViewModel GriDDataModel, int RawDataTableId, string RawDataTableName, int CompanyId)
        {
            try
            {
                var ExceptionMessage = "";
                var CompanySpecificRawData = GriDDataModel.GridData.Split(',');
                for (var i = 0; i < CompanySpecificRawData.Length; i = i + 6)
                {
                    try
                    {
                        //check if RawDataTableId is not zero otherwise add a new row in LRawDataTables
                        if (RawDataTableId==0)
                        {
                            var RawDataTable = new LRawDataTable { LrdtCompanyId = CompanyId, LrdtName = RawDataTableName };
                            db.LRawDataTables.Add(RawDataTable);
                            db.SaveChanges();
                            RawDataTableId = RawDataTable.Id;
                        }

                        var CompanySpecificRawDataModel = new LCompanySpecificRawDataColumn();
                        var XColumn = CompanySpecificRawData[i];
                        var XColumnName = XColumn.Split('(');
                        CompanySpecificRawDataModel.LcsrdcXColumnName = XColumnName.ElementAt(0).Trim();
                        var RawColumnName = CompanySpecificRawData[i].Split('(');
                       // CompanySpecificRawDataModel.LcsrdcLColumnName = (string.IsNullOrEmpty(CompanySpecificRawData[i])) ? null : RawColumnName.ElementAt(0).Trim();
                        CompanySpecificRawDataModel.LcsrdcDisplayLabel = (string.IsNullOrEmpty(CompanySpecificRawData[i+1])) ? null : CompanySpecificRawData[i + 1];
                        CompanySpecificRawDataModel.LcsrdcIsDisplayable = (string.IsNullOrEmpty(CompanySpecificRawData[i + 2])) ? false : Convert.ToBoolean(CompanySpecificRawData[i + 2]);
                        CompanySpecificRawDataModel.LcsrdcDataType = CompanySpecificRawData[i + 3];
                        CompanySpecificRawDataModel.LcsrdcOrdinalPosition = Convert.ToInt32(CompanySpecificRawData[i + 5]);
                        CompanySpecificRawDataModel.LcsrdcRawDataTableId = RawDataTableId;
                        CompanySpecificRawDataModel.Id = Convert.ToInt32(CompanySpecificRawData[i + 4]);

                        if (CompanySpecificRawDataModel.Id == 0)
                        {
                            db.LCompanySpecificRawDataColumns.Add(CompanySpecificRawDataModel);
                        }
                        else
                        {
                            db.Entry(CompanySpecificRawDataModel).State = EntityState.Modified;
                        }

                    }
                    catch (Exception ex)
                    {
                        ExceptionMessage = ExceptionMessage + ex.Message + " at Row Number :" + (i / 6 + 1) + Environment.NewLine;
                        continue;
                    }
                }
                await db.SaveChangesAsync();
                return Ok(ExceptionMessage);
            }
            catch (Exception ex)
            {
                //  Globals.SendEmail("ssharma@megacube.com.au", "Mapping Raw Data", ex.Message + " " + ex.InnerException.Message);
                //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                throw (ex); //temporarily throwing error as is for testing
            }
        }

        // POST: api/LCompanySpecificRawDataColumns
        [ResponseType(typeof(LCompanySpecificRawDataColumn))]
        public async Task<IHttpActionResult> PostCompanySpecificColumn(LCompanySpecificRawDataColumn CompanySpecificColumn)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "COLUMN")));
            }
            try
            {
                db.LCompanySpecificRawDataColumns.Add(CompanySpecificColumn);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return CreatedAtRoute("DefaultApi", new { Id = CompanySpecificColumn.Id }, CompanySpecificColumn);
        }

        // DELETE: api/LCompanySpecificRawDataColumns/5
        [ResponseType(typeof(LCompanySpecificRawDataColumn))]
        public async Task<IHttpActionResult> DeleteCompanySpecificColumn(int Id)
        {
            LCompanySpecificRawDataColumn CompanySpecificColumn = await db.LCompanySpecificRawDataColumns.FindAsync(Id);
            if (CompanySpecificColumn == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "COLUMN")));
            }
            try
            {
                db.LCompanySpecificRawDataColumns.Remove(CompanySpecificColumn);
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
            return db.LCompanySpecificRawDataColumns.Count(e => e.Id == Id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("SpGetColumnNamesForXSchemaTable", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "RAW DATA COLUMN", "DATABASE OBJECTS"));
            else if (SqEx.Message.IndexOf("FnGenerateQueryForLRawData", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "RAW DATA COLUMN", "DATABASE OBJECTS"));
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
