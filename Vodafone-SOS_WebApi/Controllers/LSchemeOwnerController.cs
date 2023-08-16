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
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Ionic.Zip;
using System.IO;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LSchemeOwnerController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        [HttpGet]
        public IHttpActionResult GetActiveReportingAnalysts(int CompanyId)
        {
            var xx = db.Database.SqlQuery<UserAsDropdownViewModel>("select Lu.Id,Lu.LuEmail,lu.LuUserId from LUsers lu  join  AspNetUserRoles aur  on lu.LuUserId = aur.UserId join AspNetRoles ar on ar.Id = aur.RoleId " +
                "join GCompanies gc on gc.GcCode = ar.CompanyCode where  gc.Id = {0} and  ar.Name = 'Reporting Analyst'  and lu.WFStatus = 'Completed'", CompanyId).ToList();
            return Ok(xx);
        }
        [HttpGet]
        public IHttpActionResult GetByCompanyId(int CompanyId)
        {
            var xx = (from aa in db.LSchemeOwners.Where(p => p.CompanyId == CompanyId)
                      join bb in db.LUsers on aa.OwnerId equals bb.LuUserId 
                      select new
                      {
                          aa.Id,
                          aa.OwnerId,
                          aa.Scheme,
                          aa.CreatedById,aa.CreatedDateTime,aa.UpdatedById,aa.UpdatedDateTime,aa.CompanyId,
                          OwnerName = bb.LuEmail 
                      }).ToList().OrderBy(a=>a.Scheme);
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

        // GET: api/LSchemeOwner/5
        [ResponseType(typeof(LSchemeOwner))]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var xx = (from aa in db.LSchemeOwners.Where(p => p.Id == id)
                      join bb in db.LUsers on aa.OwnerId equals bb.LuUserId
                      select new
                      {
                          aa.Id,
                          aa.OwnerId,
                          aa.Scheme,
                          aa.CreatedById,
                          aa.CreatedDateTime,
                          aa.UpdatedById,
                          aa.UpdatedDateTime,
                          aa.CompanyId,
                          OwnerName = bb.LuEmail
                      }).FirstOrDefault();
            return Ok(xx);
        }

        // PUT: api/LSchemeOwner/5
        [ResponseType(typeof(void))]
        [HttpPut]
        public async Task<IHttpActionResult> Put(int id, LSchemeOwner model)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SCHEME OWNER")));
            }

            if (id != model.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "SCHEME OWNER")));
            }
            try
            {
                db.Entry(model).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!LSchemeOwnerExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SCHEME OWNER")));
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
        public IHttpActionResult Post(LSchemeOwner model)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "SCHEME OWNER")));
            } 
            try
            { 
                db.LSchemeOwners.Add(model);
                db.SaveChanges();
            }

            catch (DbEntityValidationException e)
            {

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

                if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
                else
                {
                    throw ex;
                }
            }
             
            return Ok();
        }
         
        
        // DELETE: api/LRefFileTypes/5
        [ResponseType(typeof(LRefFileType))]
        public async Task<IHttpActionResult> Delete(int id, string UserName, string Workflow)
        {
            LSchemeOwner model = await db.LSchemeOwners.FindAsync(id);
            if (model == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SCHEME OWNER")));
            } 
            try
            {
                db.LSchemeOwners.Remove(model);
                await db.SaveChangesAsync();
                return Ok();
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

        private bool LSchemeOwnerExists(int id)
        {
            return db.LSchemeOwners.Count(e => e.Id == id) > 0;
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

        //[HttpGet]
        //public IHttpActionResult DownloadSupportTickets(string strUserId, int CompanyId, string RoleId,string TabName)
        //{
        //    var CompanyDetail = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
        //    var FileName = "SupportTickets_" + TabName + ".zip";
        //    if (TabName == "Requestor")
        //    {
        //        var AllowedTeamName = "L2 SOS,L2 Infra,L2 Alteryx".Split(',');
        //        var L2TeamIdList = db.RsupportTeams.Where(p => AllowedTeamName.Contains(p.RstTeamName)).Select(p => p.RstRoleId).ToList();
        //        var RSupportTeamMember = db.RsupportTeams.Where(p => p.RstRoleId.Equals(RoleId)).FirstOrDefault();
        //        var yy = db.GCompanies.Where(c => c.GcCode == "99").FirstOrDefault().Id;
        //        /*NOTE: For Non-Support team member, show only those tickets that are created by me as a normal user*/
        //        if (RSupportTeamMember == null)
        //        {

        //            var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
        //                      join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
        //                      from CO in COJoin.DefaultIfEmpty()
        //                      join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
        //                      from SO in SOjoin.DefaultIfEmpty()
        //                      where
        //                       aa.LstCreatedById.Equals(strUserId) &&
        //                       aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 1
        //                      select new
        //                      {
        //                          aa.Id,
        //                          aa.LstTicketNumber,
        //                          aa.LstStatus,
        //                          aa.LstPriority,
        //                          aa.LstCategoryId,
        //                          aa.RSupportCategory.RscName,
        //                          aa.LstSummary,
        //                          aa.LstTeamId,
        //                          aa.RsupportTeam.RstTeamName,
        //                          LstCreatedById = SO.Email,
        //                          LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
        //                          aa.LstCreatedDateTime
        //                      }).OrderBy(p => p.LstCreatedDateTime);

        //            return Ok(xx);
        //        }
        //        /*NOTE: Or, if I am Support team member, show only those tickets on which I have worked as L1 or L2 OR if my portfolios match.*/
        //        else
        //        {
        //            if (L2TeamIdList.Contains(RoleId))
        //            {
        //                var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
        //                          join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
        //                          from CO in COJoin.DefaultIfEmpty()
        //                          join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
        //                          from SO in SOjoin.DefaultIfEmpty()
        //                          where
        //                           aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 1
        //                          select new
        //                          {
        //                              aa.Id,
        //                              aa.LstTicketNumber,
        //                              aa.LstStatus,
        //                              aa.LstPriority,
        //                              aa.LstCategoryId,
        //                              aa.RSupportCategory.RscName,
        //                              aa.LstSummary,
        //                              aa.LstTeamId,
        //                              aa.RsupportTeam.RstTeamName,
        //                              LstCreatedById = SO.Email,
        //                              LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
        //                              aa.LstCreatedDateTime
        //                          }).OrderBy(p => p.LstCreatedDateTime);
        //                var Datatableresult = LINQResultToDataTable(xx);
        //                Globals.ExportZipFromDataTable(null, CompanyDetail.GcCode, strUserId, FileName, Datatableresult);
        //                return Ok(FileName);
        //            }
        //            else
        //            {

        //                //Match Portfolios with loggedin user
        //                var EntityId = 0;
        //                var LoggedInUser = db.LUsers.Where(p => p.LuUserId == strUserId).FirstOrDefault();
        //                if (LoggedInUser == null)
        //                {
        //                    var Payee = db.LPayees.Where(p => p.LpUserId == strUserId).FirstOrDefault();
        //                    if (Payee != null)
        //                        EntityId = Payee.Id;
        //                }
        //                else
        //                {
        //                    EntityId = LoggedInUser.Id;
        //                }
        //                var LoggedInUserPortfolios = db.MEntityPortfolios.Where(p => p.MepRoleId == RoleId).Where(p => p.MepEntityId == EntityId).Select(p => p.MepPortfolioId).ToList();
        //                var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
        //                          join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
        //                          from CO in COJoin.DefaultIfEmpty()
        //                          join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
        //                          from SO in SOjoin.DefaultIfEmpty()
        //                          join PO in db.MSupportTicketPortfolios on aa.Id equals PO.TicketId
        //                          where
        //                          (aa.LstL2Id == strUserId || aa.LstL1Id == strUserId || LoggedInUserPortfolios.Contains(PO.PortfolioId)) &&
        //                           aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 1
        //                          select new
        //                          {
        //                              aa.Id,
        //                              aa.LstTicketNumber,
        //                              aa.LstStatus,
        //                              aa.LstPriority,
        //                              aa.LstCategoryId,
        //                              aa.RSupportCategory.RscName,
        //                              aa.LstSummary,
        //                              aa.LstTeamId,
        //                              aa.RsupportTeam.RstTeamName,
        //                              LstCreatedById = SO.Email,
        //                              LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
        //                              aa.LstCreatedDateTime
        //                          }).OrderBy(p => p.LstCreatedDateTime);
        //                var Datatableresult = LINQResultToDataTable(xx);
        //                Globals.ExportZipFromDataTable(null, CompanyDetail.GcCode, strUserId, FileName, Datatableresult);
        //                return Ok(FileName);
        //            }
        //        }
        //    }
        //    else if(TabName== "SystemAnalyst")
        //    {
        //        var AllowedTeamName = "L2 SOS,L2 Infra,L2 Alteryx".Split(',');
        //        var L2TeamIdList = db.RsupportTeams.Where(p => AllowedTeamName.Contains(p.RstTeamName)).Select(p => p.RstRoleId).ToList();
        //        var RSupportTeamMember = db.RsupportTeams.Where(p => p.RstRoleId.Equals(RoleId)).FirstOrDefault();
        //        var yy = db.GCompanies.Where(c => c.GcCode == "99").FirstOrDefault().Id;
        //        /*NOTE: For Non-Support team member, show only those tickets that are created by me as a normal user*/
        //        if (RSupportTeamMember == null)
        //        {
        //            var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
        //                      join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
        //                      from CO in COJoin.DefaultIfEmpty()
        //                      join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
        //                      from SO in SOjoin.DefaultIfEmpty()
        //                      where
        //                       (aa.LstCreatedById.Equals(strUserId)) &&
        //                       aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 2
        //                      select new
        //                      {
        //                          aa.Id,
        //                          aa.LstTicketNumber,
        //                          aa.LstStatus,
        //                          aa.LstPriority,
        //                          aa.LstCategoryId,
        //                          aa.RSupportCategory.RscName,
        //                          aa.LstSummary,
        //                          aa.LstTeamId,
        //                          aa.RsupportTeam.RstTeamName,
        //                          LstCreatedById = SO.Email,
        //                          LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
        //                          aa.LstCreatedDateTime
        //                      }).OrderBy(p => p.LstCreatedDateTime);
        //            //LstcurrentownerID = (u.LuFirstName+" "+u.LuLastName),aa.LstCreatedDateTime}).OrderBy(p => p.LstCreatedDateTime);
        //            // aa.LstCurrentOwnerId ,aa.LstCreatedDateTime}).OrderBy(p => p.LstCreatedDateTime);

        //            var Datatableresult = LINQResultToDataTable(xx);
        //            Globals.ExportZipFromDataTable(null, CompanyDetail.GcCode, strUserId, FileName, Datatableresult);
        //            return Ok(FileName);
        //        }
        //        else
        //        {
        //            if (L2TeamIdList.Contains(RoleId))//role is l2
        //            {
        //                var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
        //                          join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
        //                          from CO in COJoin.DefaultIfEmpty()
        //                          join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
        //                          from SO in SOjoin.DefaultIfEmpty()
        //                          where
        //                           aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 2
        //                          select new
        //                          {
        //                              aa.Id,
        //                              aa.LstTicketNumber,
        //                              aa.LstStatus,
        //                              aa.LstPriority,
        //                              aa.LstCategoryId,
        //                              aa.RSupportCategory.RscName,
        //                              aa.LstSummary,
        //                              aa.LstTeamId,
        //                              aa.RsupportTeam.RstTeamName,
        //                              LstCreatedById = SO.Email,
        //                              LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
        //                              aa.LstCreatedDateTime
        //                          }).OrderBy(p => p.LstCreatedDateTime);
        //                var Datatableresult = LINQResultToDataTable(xx);
        //                Globals.ExportZipFromDataTable(null, CompanyDetail.GcCode, strUserId, FileName, Datatableresult);
        //                return Ok(FileName);
        //            }
        //            else
        //            {
        //                //Match Portfolios with loggedin user
        //                var EntityId = 0;
        //                var LoggedInUser = db.LUsers.Where(p => p.LuUserId == strUserId).FirstOrDefault();
        //                if (LoggedInUser == null)
        //                {
        //                    var Payee = db.LPayees.Where(p => p.LpUserId == strUserId).FirstOrDefault();
        //                    if (Payee != null)
        //                        EntityId = Payee.Id;
        //                }
        //                else
        //                {
        //                    EntityId = LoggedInUser.Id;
        //                }
        //                var LoggedInUserPortfolios = db.MEntityPortfolios.Where(p => p.MepRoleId == RoleId).Where(p => p.MepEntityId == EntityId).Select(p => p.MepPortfolioId).ToList();
        //                var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
        //                          join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
        //                          from CO in COJoin.DefaultIfEmpty()
        //                          join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
        //                          from SO in SOjoin.DefaultIfEmpty()
        //                          join PO in db.MSupportTicketPortfolios on aa.Id equals PO.TicketId
        //                          where
        //                          (aa.LstL2Id == strUserId || aa.LstL1Id == strUserId || LoggedInUserPortfolios.Contains(PO.PortfolioId)) &&
        //                           aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 2
        //                          select new
        //                          {
        //                              aa.Id,
        //                              aa.LstTicketNumber,
        //                              aa.LstStatus,
        //                              aa.LstPriority,
        //                              aa.LstCategoryId,
        //                              aa.RSupportCategory.RscName,
        //                              aa.LstSummary,
        //                              aa.LstTeamId,
        //                              aa.RsupportTeam.RstTeamName,
        //                              LstCreatedById = SO.Email,
        //                              LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
        //                              aa.LstCreatedDateTime
        //                          }).OrderBy(p => p.LstCreatedDateTime);
        //                var Datatableresult = LINQResultToDataTable(xx);
        //                Globals.ExportZipFromDataTable(null, CompanyDetail.GcCode, strUserId, FileName, Datatableresult);
        //                return Ok(FileName);
        //            }
        //        }
        //    }
        //    else if (TabName == "L2Admin")
        //    {
        //        var AllowedTeamName = "L2 SOS,L2 Infra,L2 Alteryx".Split(',');
        //        var L2TeamIdList = db.RsupportTeams.Where(p => AllowedTeamName.Contains(p.RstTeamName)).Select(p => p.RstRoleId).ToList();
        //        var RSupportTeamMember = db.RsupportTeams.Where(p => p.RstRoleId.Equals(RoleId)).FirstOrDefault();
        //        var yy = db.GCompanies.Where(c => c.GcCode == "99").FirstOrDefault().Id;
        //        /*NOTE: For Non-Support team member, show only those tickets that are created by me as a normal user*/
        //        if (RSupportTeamMember == null)
        //        {
        //            var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser)
        //                      join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
        //                      from CO in COJoin.DefaultIfEmpty()
        //                      join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
        //                      from SO in SOjoin.DefaultIfEmpty()
        //                      where
        //                       (aa.LstCreatedById.Equals(strUserId)) &&
        //                       aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 3
        //                      // && (aa.LstTeamId.HasValue)?L2TeamIdList.Contains(aa.LstTeamId.Value):1==0//check for team id only if it is populated otherwise do not show that record
        //                      select new
        //                      {
        //                          aa.Id,
        //                          aa.LstTicketNumber,
        //                          aa.LstStatus,
        //                          aa.LstPriority,
        //                          aa.LstCategoryId,
        //                          aa.RSupportCategory.RscName,
        //                          aa.LstSummary,
        //                          aa.LstTeamId,
        //                          aa.RsupportTeam.RstTeamName,
        //                          LstCreatedById = SO.Email,
        //                          LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
        //                          aa.LstCreatedDateTime
        //                      }).OrderBy(p => p.LstCreatedDateTime);
        //            //LstcurrentownerID = (u.LuFirstName+" "+u.LuLastName),aa.LstCreatedDateTime}).OrderBy(p => p.LstCreatedDateTime);
        //            // aa.LstCurrentOwnerId ,aa.LstCreatedDateTime}).OrderBy(p => p.LstCreatedDateTime);
        //            var Datatableresult = LINQResultToDataTable(xx);
        //            Globals.ExportZipFromDataTable(null, CompanyDetail.GcCode, strUserId, FileName, Datatableresult);
        //            return Ok(FileName);
        //        }
        //        else
        //        {
        //            if (L2TeamIdList.Contains(RoleId))//Role belongs to L2
        //            {
        //                var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
        //                          join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
        //                          from CO in COJoin.DefaultIfEmpty()
        //                          join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
        //                          from SO in SOjoin.DefaultIfEmpty()
        //                          where
        //                           aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 3
        //                          select new
        //                          {
        //                              aa.Id,
        //                              aa.LstTicketNumber,
        //                              aa.LstStatus,
        //                              aa.LstPriority,
        //                              aa.LstCategoryId,
        //                              aa.RSupportCategory.RscName,
        //                              aa.LstSummary,
        //                              aa.LstTeamId,
        //                              aa.RsupportTeam.RstTeamName,
        //                              LstCreatedById = SO.Email,
        //                              LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
        //                              aa.LstCreatedDateTime
        //                          }).OrderBy(p => p.LstCreatedDateTime);
        //                var Datatableresult = LINQResultToDataTable(xx);
        //                Globals.ExportZipFromDataTable(null, CompanyDetail.GcCode, strUserId, FileName, Datatableresult);
        //                return Ok(FileName);
        //            }
        //            else
        //            {
        //                //Match Portfolios with loggedin user
        //                var EntityId = 0;
        //                var LoggedInUser = db.LUsers.Where(p => p.LuUserId == strUserId).FirstOrDefault();
        //                if (LoggedInUser == null)
        //                {
        //                    var Payee = db.LPayees.Where(p => p.LpUserId == strUserId).FirstOrDefault();
        //                    if (Payee != null)
        //                        EntityId = Payee.Id;
        //                }
        //                else
        //                {
        //                    EntityId = LoggedInUser.Id;
        //                }
        //                var LoggedInUserPortfolios = db.MEntityPortfolios.Where(p => p.MepRoleId == RoleId).Where(p => p.MepEntityId == EntityId).Select(p => p.MepPortfolioId).ToList();
        //                var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
        //                          join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
        //                          from CO in COJoin.DefaultIfEmpty()
        //                          join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
        //                          from SO in SOjoin.DefaultIfEmpty()
        //                          join PO in db.MSupportTicketPortfolios on aa.Id equals PO.TicketId
        //                          where
        //                          (aa.LstL2Id == strUserId || aa.LstL1Id == strUserId || LoggedInUserPortfolios.Contains(PO.PortfolioId)) &&
        //                           aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 3
        //                          //  && (aa.LstTeamId.HasValue) ? L2TeamIdList.Contains(aa.LstTeamId.Value) : 1 == 0//check for team id only if it is populated otherwise do not show that record
        //                          select new
        //                          {
        //                              aa.Id,
        //                              aa.LstTicketNumber,
        //                              aa.LstStatus,
        //                              aa.LstPriority,
        //                              aa.LstCategoryId,
        //                              aa.RSupportCategory.RscName,
        //                              aa.LstSummary,
        //                              aa.LstTeamId,
        //                              aa.RsupportTeam.RstTeamName,
        //                              LstCreatedById = SO.Email,
        //                              LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
        //                              aa.LstCreatedDateTime
        //                          }).OrderBy(p => p.LstCreatedDateTime);
        //                var Datatableresult = LINQResultToDataTable(xx);
        //                Globals.ExportZipFromDataTable(null, CompanyDetail.GcCode, strUserId, FileName, Datatableresult);
        //                return Ok(FileName);
        //            }
        //        }
        //    }
        //    return Ok();
        //}

        [ResponseType(typeof(string))]
        [HttpGet]
        public string DownloadSchemeOwners(string UserHexID, int CompanyID, string CompanyCode, string LoggedInUserName, int LoggedRoleId)
        {
            // string Qry = "select 'Scheme','OwnerName' union all select * from (select aa.Scheme,bb.LuEmail as OwnerName from LSchemeOwner aa join LUsers bb on aa.OwnerId = bb.LuUserId where aa.CompanyId =" + CompanyID + ") cc Order By Scheme";
             string Qry = "select aa.Scheme,bb.LuEmail as OwnerName from LSchemeOwner aa join LUsers bb on aa.OwnerId = bb.LuUserId where aa.CompanyId =" + CompanyID + " Order By Scheme asc";

            DataTable tb = new DataTable();
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Qry, conn);
            

            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();

            //columnnames must not be any of following: 'WFComments','CreatedByForm','CreatedById','CreatedDateTime','ParameterCarrier','Status','WFCompanyId')
            //WFStatus is being placed in the file as hard coded
           
            //string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //SqlConnection conn = new SqlConnection(ConnectionString);
            //SqlCommand cmd = new SqlCommand("USPGetMYUsersList", conn);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@UserHexID", UserHexID);
            //cmd.Parameters.AddWithValue("@LoggedRoleId", LoggedRoleId);
            //conn.Open();
            //SqlDataAdapter sda = new SqlDataAdapter(cmd);
            //sda.Fill(tb);
            //conn.Close();

            

            //var tb = (from aa in db.LSchemeOwners.Where(p => p.CompanyId == CompanyID)
            //          join bb in db.LUsers on aa.OwnerId equals bb.LuUserId
            //          select new
            //          {
            //              aa.Id,
            //              aa.OwnerId,
            //              aa.Scheme,
            //              aa.CreatedById,
            //              aa.CreatedDateTime,
            //              aa.UpdatedById,
            //              aa.UpdatedDateTime,
            //              aa.CompanyId,
            //              OwnerName = bb.LuEmail
            //          }).ToList().OrderBy(a => a.Scheme);

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("SchemeOwners");
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < tb.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = tb.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }

            //{
            //    ICell cell = row1.CreateCell(0);

            //    cell.SetCellValue("Scheme");

            //    cell = row1.CreateCell(1);

            //    cell.SetCellValue("OwnerName");
            //}

            //loops through data  
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < tb.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = tb.Columns[j].ToString();
                    cell.SetCellValue(tb.Rows[i][columnName].ToString());
                }
            }
            //IRow dRow = sheet1.GetRow(0);
            //sheet1.RemoveRow(dRow);
            //sheet1.ShiftRows(0, sheet1.LastRowNum, -1);
            //dRow = sheet1.GetRow(0);
            //dRow.ZeroHeight = true;
            //Create OPCO Folder
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode);
            }
            //Create UserName Folder
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            }
            //Delete existing file
            if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/" + "SchemeOwners.xlsx"))
            {
                System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/" + "SchemeOwners.xlsx");
            }
            using (ZipFile zip = new ZipFile())
            {
                FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/", "SchemeOwners.xlsx"), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                workbook.Write(xfile);
                xfile.Close();
            }
            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            di.Refresh();
            return Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/", "SchemeOwners.xlsx");
        }


    }
}
