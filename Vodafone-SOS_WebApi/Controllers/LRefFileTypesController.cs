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
using System.Data.Entity.Validation;
using System.Web.Script.Serialization;
using System.Configuration;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LRefFileTypesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // GET: api/LRefFileTypes
        public IHttpActionResult GetLRefFileTypes()
        {
            var xx = (from aa in db.LRefFileTypes.Include(c => c.GCompany)
                      select new { aa.Id, aa.LrftCompanyId,aa.LrftCreatedById,aa.LrftCreatedDateTime,aa.LrftDescription,aa.LrftName
                          ,aa.LrftUpdatedById,aa.LrftUpdatedDateTime, aa.GCompany.GcCompanyName }).OrderBy(p => p.LrftName);
            return Ok(xx);
        }

        //Method to get company specific data for dropdown in WebApplication
        public IHttpActionResult GetLRefFileTypesDropdownData(int CompanyId)
        {
            var data = db.LRefFileTypes.Where(p => p.LrftCompanyId == CompanyId).Select(x => new { x.LrftName, x.Id }).OrderBy(p => p.LrftName).AsEnumerable();
            return Ok(data);
        }

        // GET: api/LRefFileTypes?CompanyId=5
        public IHttpActionResult GetLRefFileTypesByCompanyId(int CompanyId)
        {
            var xx = (from aa in db.LRefFileTypes.Where(p => p.LrftCompanyId == CompanyId).Include(c => c.GCompany)
                      select new
                      {
                          aa.Id,aa.LrftCompanyId,aa.LrftCreatedById,aa.LrftCreatedDateTime,aa.LrftDescription,
                          aa.LrftName,aa.LrftUpdatedById,aa.LrftUpdatedDateTime,
                          aa.GCompany.GcCompanyName
                      }).OrderBy(p => p.LrftName);
            return Ok(xx);
        }


       
        [HttpGet]
        public IHttpActionResult DownloadRefFileTypes(int CompanyId, string UserName)
        {
            var CompanyDetails = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
            //The below lines of code converts the data returned from api to a datatable
            var tb = new DataTable();
            string Query = "Exec [dbo].[SpDownloadRefFileTypes] @CompanyId";
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);          
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            var FileName = "RefFileTypes" + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".xlsx";                       
            var TempPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyDetails.GcCode + "/" + UserName + "/";

            if (tb.Columns.Count > 0)
            {
                tb.Columns["LrftName"].ColumnName = "Reference Type";
                tb.Columns["LrftDescription"].ColumnName = "Description";
                tb.Columns["RcPrimaryChannel"].ColumnName = "PrimaryChannel";
                tb.Columns["LpBusinessUnit"].ColumnName = "BusinessUnit";
                tb.Columns["RcName"].ColumnName = "Channel";
            }

            Globals.ExportToExcel(tb, TempPath, FileName); //method to create excel file of data return from storedproc in the form of tb
            return Ok(FileName);
        }

        /*note on the RefFile Type create/Edit Page. Note: if no Portfolio is selected, then RefFile Type will be visible to ALL users in the dropdown on RefData Upload Page.*/
        public IHttpActionResult GetByPortfolioMatching(int CompanyId, string LoggedInUserId, string LoggedInRoleId)
        {
            var LUserDetails = db.LUsers.Where(p => p.LuUserId == LoggedInUserId).FirstOrDefault();
            if (LUserDetails != null)
            {
                var dataTable = new DataTable();
                string connString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                string query = "select distinct LRFT.LrftName,LRFT.Id,LRFT.LrftCompanyId from LRefFileTypes LRFT left outer join MEntityPortfolios MEP on (LRFT.Id=MEP.MepEntityId and MEp.MepEntityType='LRefFileTypes') where LRFT.LrftCompanyId=@CompanyId and ((([dbo].[FnHasPortfolio](LRFT.Id,'LRefFileTypes',null))=1 and (Mep.MepPortfolioId in (select MepPortfolioId from MEntityPortfolios where MepEntityId=@EntityId and MepEntityType='LUsers'))) or ([dbo].[FnHasPortfolio](LRFT.Id,'LRefFileTypes',null)=0)) order by LRFT.LrftName";
                SqlConnection conn = new SqlConnection(connString);
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@EntityId", System.Data.SqlDbType.Int);
                cmd.Parameters.Add("@CompanyId", System.Data.SqlDbType.Int);
                cmd.Parameters["@EntityId"].Value = LUserDetails.Id;
                cmd.Parameters["@CompanyId"].Value = CompanyId;
                conn.Open();
                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to  datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();
                return Ok(dataTable);
            }
            return Ok();
        }

        // GET: api/LRefFileTypes/5
        [ResponseType(typeof(LRefFileType))]
        public async Task<IHttpActionResult> GetLRefFileType(int id)
        {
            var LRefFileType = db.LRefFileTypes.Where(p => p.Id == id).Include(c => c.GCompany).Select(x => new { x.Id,x.LrftCompanyId,x.LrftCreatedById,x.LrftCreatedDateTime,x.LrftDescription,x.LrftName,x.LrftUpdatedById,x.LrftUpdatedDateTime, x.GCompany.GcCompanyName }).FirstOrDefault();
            if (LRefFileType == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "REF FILE TYPE")));
            }
            return Ok(LRefFileType);
        }

        // PUT: api/LRefFileTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLRefFileType(int id, LRefFileType LRefFileType)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "REF FILE TYPE")));
            }

            if (id != LRefFileType.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "REF FILE TYPE")));
            }
            try
            {
                db.Entry(LRefFileType).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LRefFileTypeExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "REF FILE TYPE")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        //add data of grid in one call
        [ResponseType(typeof(LRefFileType))]
        [HttpPost]
        public IHttpActionResult PostLRefFileTypeGrid(string[] ModelData,int CompanyId,string LoggedInUserId)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "REF FILE TYPE")));
            }
           
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                    //delete all existing records
                    //db.Database.ExecuteSqlCommand("delete from LRefFileTypes");
                    var modelList = ModelData;//.Split(',');
                    for (var i = 0; i < modelList.Length; i=i++)
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        string[] DataArray = js.Deserialize<string[]>(modelList[i]);
                        var Id = Convert.ToInt32(DataArray[0]);
                        if (Id == 0)
                        {
                            var LReftypemodel = new LRefFileType
                            {
                                LrftUpdatedById = LoggedInUserId,
                                LrftUpdatedDateTime = DateTime.UtcNow,
                                LrftCompanyId = CompanyId,
                                LrftCreatedById = LoggedInUserId,
                                LrftCreatedDateTime = DateTime.UtcNow,
                                LrftName = DataArray[1],
                                LrftDescription = DataArray[2]
                            };
                            if (!string.IsNullOrEmpty(LReftypemodel.LrftName))
                            {
                                db.LRefFileTypes.Add(LReftypemodel);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var RefFileType = db.LRefFileTypes.Find(Id);
                            RefFileType.LrftUpdatedById = LoggedInUserId;//using same value as it is logged in user id
                            RefFileType.LrftUpdatedDateTime = DateTime.UtcNow;
                            RefFileType.LrftName = modelList[i + 1];
                            RefFileType.LrftDescription = modelList[i + 2];
                            db.Entry(RefFileType).State = EntityState.Modified;
                            db.SaveChanges();
                         ////updating using sql query
                         //   var xx = db.Database.ExecuteSqlCommand("update LRefFileTypes set ");
                        }
                    }

                    tran.Commit();
                }
                catch (DbEntityValidationException e)
                {
                    tran.Rollback();
                    string ErrorMessage = null;
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        ErrorMessage += string.Format("Entity of type {0} in state {1} has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ErrorMessage += string.Format("- Property: {0}, Error: {1}", ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw e;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            
            return Ok();
        }

        // POST: api/LRefFileTypes
        [HttpPut]
        public async Task<IHttpActionResult> PutLRefFileType(LRefFileType LRefFileType, string PortfolioList, string LoggedInRoleId, string UserName, string Workflow)
        {

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Entry(LRefFileType).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    //Add portfolio
                    if (!string.IsNullOrEmpty(PortfolioList))
                    {
                        //Remove Existing Portfolios
                        var ExistingPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityId == LRefFileType.Id).Where(p => p.MepEntityType == "LRefFileTypes").ToList();
                        db.MEntityPortfolios.RemoveRange(ExistingPortfolios);
                        db.SaveChanges();
                        var PFArray = PortfolioList.Split(',').ToList();
                        foreach (var PF in PFArray)
                        {
                            var Id = Convert.ToInt32(PF);
                            var MEntityPortfolio = new MEntityPortfolio { MepPortfolioId = Id, MepEntityId = LRefFileType.Id, MepEntityType = "LRefFileTypes" };
                            db.MEntityPortfolios.Add(MEntityPortfolio);
                            db.SaveChanges();
                        }
                    }


                    transaction.Commit();
                }
                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    {
                        //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                    }
                    else
                    {
                        throw ex;//This exception will be handled in FilterConfig's CustomHandler
                    }
                }
            }
            return Ok(LRefFileType);
        }


        // POST: api/LRefFileTypes
        [HttpPost]
        public async Task<IHttpActionResult> PostLRefFileType(LRefFileType LRefFileType, string PortfolioList,string LoggedInRoleId, string UserName, string Workflow)
        {
          
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.LRefFileTypes.Add(LRefFileType);
                    await db.SaveChangesAsync();
                    //Add portfolio
                    if (!string.IsNullOrEmpty(PortfolioList))
                    {
                        var PFArray = PortfolioList.Split(',').ToList();
                        foreach (var PF in PFArray)
                        {
                            var Id = Convert.ToInt32(PF);
                            var MEntityPortfolio = new MEntityPortfolio { MepPortfolioId = Id, MepEntityId = LRefFileType.Id, MepEntityType = "LRefFileTypes" };
                            db.MEntityPortfolios.Add(MEntityPortfolio);
                            db.SaveChanges();
                        }
                    }

                   
                    transaction.Commit();
                }
                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    {
                        //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                    }
                    else
                    {
                        throw ex;//This exception will be handled in FilterConfig's CustomHandler
                    }
                }
            }
            return Ok(LRefFileType);
        }

        // DELETE: api/LRefFileTypes/5
        [ResponseType(typeof(LRefFileType))]
        public async Task<IHttpActionResult> DeleteLRefFileType(int id, string UserName, string Workflow)
        {
            LRefFileType LRefFileType = await db.LRefFileTypes.FindAsync(id);
            if (LRefFileType == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "REF FILE TYPE")));
            }
            //JS:-I don’t think we will be applying FK in 2.1, so let’s put manual check.
            //check for ref files using this ref file type
            if (db.LRefFiles.Where(p => p.LrfRefFileTypeId == id).Count() > 0)
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, "Reference Files already exist for this RefFile Type. Hence it cannot be deleted."));//type 2 error
            }
            try
            {
                db.LRefFileTypes.Remove(LRefFileType);
                await db.SaveChangesAsync();
                return Ok(LRefFileType);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
        }

        //// POST: api/LRefFileTypes
        //[ResponseType(typeof(LRefFileType))]
        //public async Task<IHttpActionResult> PostLRefFileType(LRefFileType LRefFileType)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        //return BadRequest(ModelState);
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "REF FILE TYPE")));
        //    }
        //    try
        //    {
        //        db.LRefFileTypes.Add(LRefFileType);
        //        await db.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
        //    }
        //    return CreatedAtRoute("DefaultApi", new { id = LRefFileType.Id }, LRefFileType);
        //}

        //// DELETE: api/LRefFileTypes/5
        //[ResponseType(typeof(LRefFileType))]
        //public async Task<IHttpActionResult> DeleteLRefFileType(int id)
        //{
        //    LRefFileType LRefFileType = await db.LRefFileTypes.FindAsync(id);
        //    if (LRefFileType == null)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "REF FILE TYPE")));
        //    }
        //    try
        //    {
        //        db.LRefFileTypes.Remove(LRefFileType);
        //        await db.SaveChangesAsync();
        //        return Ok(LRefFileType);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
        //    }
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LRefFileTypeExists(int id)
        {
            return db.LRefFileTypes.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("VUpdatedFiles", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "REF FILES", "VIEW(S)"));
            if (SqEx.Message.IndexOf("UQ_LRefFileTypes_LrftName_LrftCompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return ("Cannot Insert Duplicate RefFile Type");
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
