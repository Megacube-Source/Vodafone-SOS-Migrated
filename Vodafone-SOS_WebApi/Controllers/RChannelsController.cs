//Code Review Convert SQL to LINQ completed
using System;
using System.Collections.Generic;
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
    public class RChannelsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
       
        public IHttpActionResult GetRChannelsByQuery(string Query)
        {
            var xx = db.Database.SqlQuery<RChannel>(Query);
            return Ok(xx);
        }
        //Method to get company specific data for dropdown in WebApplication
        public IHttpActionResult GetRChannelsDropdownData(int CompanyId,string PrimaryChannel)
        {
            var data = db.RChannels.Where(p => p.RcCompanyId == CompanyId).Where(p=>p.RcPrimaryChannel==PrimaryChannel).Where(p => p.RcIsActive==true).Select(x => new { x.RcName, x.Id }).OrderBy(p => p.RcName).AsEnumerable();
            return Ok(data);
        }

        // GET: api/RChannels?CompanyId=5
        public IHttpActionResult GetRChannelsByCompanyId(int CompanyId)
        {
            var xx = (from aa in db.RChannels.Where(p => p.RcCompanyId == CompanyId).Include(c => c.GCompany)
                      select new {aa.RcIsActive, aa.Id, aa.RcCompanyId, aa.RcName, aa.GCompany.GcCompanyName, aa.RcDescription,aa.RcPrimaryChannel }).OrderByDescending(p=>p.RcIsActive).ThenBy(p=>p.RcName);
            return Ok(xx);
        }

        // GET: api/RChannels/5
        [ResponseType(typeof(RChannel))]
        public async Task<IHttpActionResult> GetRChannel(int id)
        {
            var RChannel = db.RChannels.Where(p => p.Id == id).Include(c => c.GCompany).Select(x => new {x.RcIsActive, x.RcPrimaryChannel,x.Id, x.RcDescription, x.GCompany.GcCompanyName, x.RcCompanyId, x.RcName }).FirstOrDefault();
            if (RChannel == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ChannelName")));
            }
            return Ok(RChannel);
        }

        // PUT: api/RChannels/5
        [ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutRChannel(int id, RChannel RChannel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        //return BadRequest(ModelState);
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ChannelName")));
        //    }

        //    if (id != RChannel.Id)
        //    {
        //        //return BadRequest();
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ChannelName")));
        //    }
        //    using (var tran = db.Database.BeginTransaction())
        //    {
        //        try
        //        {

        //            //Make its associated portfolios InActive if Channel Status is InActive and vise versa
        //            if (RChannel.RcIsActive.Value)
        //            {
        //                db.Entry(RChannel).State = EntityState.Modified;
        //                await db.SaveChangesAsync();

        //                var Portfolios = db.LPortfolios.Where(p => p.LpChannelId == RChannel.Id).ToList();

        //                //changes by SG for portfolio creation or activation
        //                var count = Portfolios.Count;
        //                if (count == 2)
        //                {//for the particular channel, update the status in LPortfolio
        //                    foreach (var PF in Portfolios)
        //                    {
        //                        PF.LpIsActive = true;
        //                        db.Entry(PF).State = EntityState.Modified;
        //                        await db.SaveChangesAsync();
        //                    }
        //                }
        //                else if (count == 1)
        //                {//for one entry in Lportfolio table, update its status and insert new entry for other BU;
        //                    foreach (var PF in Portfolios)
        //                    {
        //                        var existingBU = PF.LpBusinessUnit;
        //                        var newBU = existingBU.Equals("CBU") ? "EBU" : " CBU";
        //                        PF.LpIsActive = true;
        //                        db.Entry(PF).State = EntityState.Modified;
        //                        var PortfolioModel = new LPortfolio
        //                        { LpBusinessUnit = newBU, LpChannelId = RChannel.Id, LpIsActive = true, LpCompanyId = RChannel.RcCompanyId };
        //                        db.LPortfolios.Add(PortfolioModel);
        //                        await db.SaveChangesAsync();
        //                    }

        //                }
        //                else if (count == 0)
        //                {
        //                    //if there is no portfolio for the Channel, its all the combinations with the Primary Channel and Business Unit are added in LPortfolios
        //                    var BusinessUnit = new List<string> { "CBU", "EBU" };
        //                    foreach (var BU in BusinessUnit)
        //                    {
        //                        var PortfolioModel = new LPortfolio { LpBusinessUnit = BU, LpChannelId = RChannel.Id, LpIsActive = true, LpCompanyId = RChannel.RcCompanyId };
        //                        db.LPortfolios.Add(PortfolioModel);
        //                        await db.SaveChangesAsync();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //var data = (from aa in db.RChannels.Where(p => p.Id.Equals("LPayees", StringComparison.OrdinalIgnoreCase)).Where(p => EntityArray.Contains(p.MepEntityId)).Include(p => p.LPortfolio.RChannel)
        //                var LBatchesdata = (from RC in db.RChannels.Where(x=>x.Id.Equals(RChannel.Id))
        //                            join LP in db.LPortfolios on RC.Id equals LP.LpChannelId 
        //                            join ME in db.MEntityPortfolios.Where(y=>y.MepEntityType.Equals("LBatches"))  on LP.Id equals ME.MepPortfolioId
        //                            join LB in db.LBatches.Where(x=>x.WFStatus !=  "Completed")  on ME.MepEntityId equals LB.Id
        //                            select new { LB.WFStatus }).ToList();

        //                var LChangeRequestsdata = (from RC in db.RChannels.Where(x => x.Id.Equals(RChannel.Id))
        //                                    join LP in db.LPortfolios on RC.Id equals LP.LpChannelId
        //                                    join ME in db.MEntityPortfolios.Where(y => y.MepEntityType.Equals("LChangeRequests")) on LP.Id equals ME.MepPortfolioId
        //                                    join LB in db.LChangeRequests.Where(x => x.WFStatus != "Completed") on ME.MepEntityId equals LB.Id
        //                                    select new { LB.WFStatus }).ToList();

        //                var LClaimsdata = (from RC in db.RChannels.Where(x => x.Id.Equals(RChannel.Id))
        //                                           join LP in db.LPortfolios on RC.Id equals LP.LpChannelId
        //                                           join ME in db.MEntityPortfolios.Where(y => y.MepEntityType.Equals("LClaims")) on LP.Id equals ME.MepPortfolioId
        //                                           join LB in db.LClaims.Where(x => x.WFStatus != "Completed") on ME.MepEntityId equals LB.Id
        //                                           select new { LB.WFStatus }).ToList();

        //                var LDocumentSetsdata = (from RC in db.RChannels.Where(x => x.Id.Equals(RChannel.Id))
        //                                   join LP in db.LPortfolios on RC.Id equals LP.LpChannelId
        //                                   join ME in db.MEntityPortfolios.Where(y => y.MepEntityType.Equals("LDocumentSets")) on LP.Id equals ME.MepPortfolioId
        //                                   join LB in db.LDocumentSets.Where(x => x.WFStatus != "Completed") on ME.MepEntityId equals LB.Id
        //                                   select new { LB.WFStatus }).ToList();

        //                var LPayeesdata = (from RC in db.RChannels.Where(x => x.Id.Equals(RChannel.Id))
        //                                         join LP in db.LPortfolios on RC.Id equals LP.LpChannelId
        //                                         join ME in db.MEntityPortfolios.Where(y => y.MepEntityType.Equals("LPayees")) on LP.Id equals ME.MepPortfolioId
        //                                         join LB in db.LPayees.Where(x => x.WFStatus != "Completed") on ME.MepEntityId equals LB.Id
        //                                         select new { LB.WFStatus }).ToList();

        //                var LRefFilesdata = (from RC in db.RChannels.Where(x => x.Id.Equals(RChannel.Id))
        //                                   join LP in db.LPortfolios on RC.Id equals LP.LpChannelId
        //                                   join ME in db.MEntityPortfolios.Where(y => y.MepEntityType.Equals("LRefFiles")) on LP.Id equals ME.MepPortfolioId
        //                                   join LB in db.LRefFiles.Where(x => x.WFStatus != "Completed") on ME.MepEntityId equals LB.Id
        //                                   select new { LB.WFStatus }).ToList();

        //                var LUsersdata = (from RC in db.RChannels.Where(x => x.Id.Equals(RChannel.Id))
        //                                     join LP in db.LPortfolios on RC.Id equals LP.LpChannelId
        //                                     join ME in db.MEntityPortfolios.Where(y => y.MepEntityType.Equals("LUsers")) on LP.Id equals ME.MepPortfolioId
        //                                     join LB in db.LUsers.Where(x => x.WFStatus != "Completed") on ME.MepEntityId equals LB.Id
        //                                     select new { LB.WFStatus }).ToList();

        //                var LSchemesdata = (from RC in db.RChannels.Where(x => x.Id.Equals(RChannel.Id))
        //                                  join LP in db.LPortfolios on RC.Id equals LP.LpChannelId
        //                                  join ME in db.MEntityPortfolios.Where(y => y.MepEntityType.Equals("LSchemes")) on LP.Id equals ME.MepPortfolioId
        //                                  join LB in db.LSchemes.Where(x => x.WFStatus != "Completed") on ME.MepEntityId equals LB.Id
        //                                  select new { LB.WFStatus }).ToList();



        //                if (LBatchesdata.Count > 0 || LChangeRequestsdata.Count > 0 || LClaimsdata.Count > 0 || LDocumentSetsdata.Count > 0 || LPayeesdata.Count > 0 || LRefFilesdata.Count > 0 || LUsersdata.Count > 0 || LSchemesdata.Count > 0)  
        //                {
        //                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Channel can't ne deactivated as it is used in the application."));
        //                }
        //                else {
        //                    db.Entry(RChannel).State = EntityState.Modified;
        //                    await db.SaveChangesAsync();

        //                    var Portfolios = db.LPortfolios.Where(p => p.LpChannelId == RChannel.Id).ToList();
        //                    foreach (var PF in Portfolios)
        //                    {
        //                        PF.LpIsActive = false;
        //                        db.Entry(PF).State = EntityState.Modified;
        //                        await db.SaveChangesAsync();
        //                    }

        //                }
        //            }

        //            tran.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            tran.Rollback();
        //            ////Log error in GerrorLogs
        //            //string[] s = Request.RequestUri.AbsolutePath.Split('/');
        //            //string Controller = s[2];
        //            //string Action = s[3];
        //            //Globals.LogApiError(Controller,Action,ex.Message+Environment.NewLine+ ex.StackTrace+ex.InnerException);
        //            if (!RChannelExists(id))
        //            {
        //                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ChannelName")));
        //            }
        //            else
        //            {
        //                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
        //            }
        //        }
        //    }
        //    return StatusCode(HttpStatusCode.NoContent);
        //}
        public static DataSet GetData(SqlCommand cmd)
        {
            DataSet ds = new DataSet();
            String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(strConnString);
            SqlDataAdapter sda = new SqlDataAdapter();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            try
            {
                con.Open();
                sda.SelectCommand = cmd;
                sda.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                sda.Dispose();
                con.Dispose();
            }
        }
       public async Task<IHttpActionResult> PutRChannel(int id, RChannel RChannel)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ChannelName")));
            }

            if (id != RChannel.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ChannelName")));
            }
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {

                    //Make its associated portfolios InActive if Channel Status is InActive and vise versa
                    if (RChannel.RcIsActive.Value)
                    {
                        db.Entry(RChannel).State = EntityState.Modified;
                         await db.SaveChangesAsync();
                        //db.SaveChangesAsync();

                        var Portfolios = db.LPortfolios.Where(p => p.LpChannelId == RChannel.Id).ToList();

                        //changes by SG for portfolio creation or activation
                        var count = Portfolios.Count;
                        if (count == 2)
                        {//for the particular channel, update the status in LPortfolio
                            foreach (var PF in Portfolios)
                            {
                                PF.LpIsActive = true;
                                db.Entry(PF).State = EntityState.Modified;
                                 await db.SaveChangesAsync();
                                //db.SaveChangesAsync();
                            }
                        }
                        else if (count == 1)
                        {//for one entry in Lportfolio table, update its status and insert new entry for other BU;
                            foreach (var PF in Portfolios)
                            {
                                var existingBU = PF.LpBusinessUnit;
                                var newBU = existingBU.Equals("CBU") ? "EBU" : " CBU";
                                PF.LpIsActive = true;
                                db.Entry(PF).State = EntityState.Modified;
                                var PortfolioModel = new LPortfolio
                                { LpBusinessUnit = newBU, LpChannelId = RChannel.Id, LpIsActive = true, LpCompanyId = RChannel.RcCompanyId };
                                db.LPortfolios.Add(PortfolioModel);
                                 await db.SaveChangesAsync();
                                //db.SaveChangesAsync();
                            }

                        }
                        else if (count == 0)
                        {
                            //if there is no portfolio for the Channel, its all the combinations with the Primary Channel and Business Unit are added in LPortfolios
                            var BusinessUnit = new List<string> { "CBU", "EBU" };
                            foreach (var BU in BusinessUnit)
                            {
                                var PortfolioModel = new LPortfolio { LpBusinessUnit = BU, LpChannelId = RChannel.Id, LpIsActive = true, LpCompanyId = RChannel.RcCompanyId };
                                db.LPortfolios.Add(PortfolioModel);
                                 await db.SaveChangesAsync();
                                //db.SaveChangesAsync();
                            }
                        }
                    }
                    else
                    {
                        DataSet dsErrors = new DataSet();
                        var Query = "Exec dbo.USPCheckChannelDependancy @ChannelID";
                        
                        SqlCommand cmd = new SqlCommand(Query);
                        cmd.Parameters.AddWithValue("@ChannelID", RChannel.Id);

                        dsErrors = GetData(cmd);
                        if (dsErrors.Tables.Count > 0)
                        {
                            if (dsErrors.Tables[0].Rows.Count > 0)
                            {
                                return Ok(dsErrors.Tables[0]);
                            }

                            else
                            {
                                db.Entry(RChannel).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                                

                                var Portfolios = db.LPortfolios.Where(p => p.LpChannelId == RChannel.Id).ToList();
                                foreach (var PF in Portfolios)
                                {
                                    PF.LpIsActive = false;
                                    db.Entry(PF).State = EntityState.Modified;
                                    await db.SaveChangesAsync();

                                }

                            }
                        }
                        else
                        {
                            db.Entry(RChannel).State = EntityState.Modified;
                            await db.SaveChangesAsync();

                            var Portfolios = db.LPortfolios.Where(p => p.LpChannelId == RChannel.Id).ToList();
                            foreach (var PF in Portfolios)
                            {
                                PF.LpIsActive = false;
                                db.Entry(PF).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ////Log error in GerrorLogs
                    //string[] s = Request.RequestUri.AbsolutePath.Split('/');
                    //string Controller = s[2];
                    //string Action = s[3];
                    //Globals.LogApiError(Controller,Action,ex.Message+Environment.NewLine+ ex.StackTrace+ex.InnerException);
                    if (!RChannelExists(id))
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ChannelName")));
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                    }
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        // POST: api/RChannels
        [ResponseType(typeof(RChannel))]
        public async Task<IHttpActionResult> PostRChannel(RChannel RChannel)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ChannelName")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.RChannels.Add(RChannel);
                    await db.SaveChangesAsync();
                    if (RChannel.RcIsActive.Value)
                    {
                        var LstSuperUsers = db.LUsers.Where(x => x.LuCompanyId == RChannel.RcCompanyId   && x.IsSuperUser == true && (x.WFStatus == "Completed" || x.WFStatus == "Suspended" || x.WFStatus == "Saved" || x.WFStatus == "InProgress")).ToList();
                        //if a new channel is added, its all the combinations with the Primary Channel and Business Unit are added in LPortfolios
                        var BusinessUnit = new List<string> { "CBU", "EBU" };
                        foreach (var BU in BusinessUnit)
                        {
                            var PortfolioModel = new LPortfolio { LpBusinessUnit = BU, LpChannelId = RChannel.Id, LpIsActive = true, LpCompanyId = RChannel.RcCompanyId };
                            db.LPortfolios.Add(PortfolioModel);
                            db.SaveChanges();

                            foreach (var LU in LstSuperUsers)
                            {
                                var LstRoles = db.AspNetUserRoles.Where(x => x.UserId == LU.LuUserId).ToList();
                                foreach (var role in LstRoles)
                                {
                                    var MEntityPortfolio = new MEntityPortfolio { MepPortfolioId = PortfolioModel.Id, MepEntityType = "LUsers", MepEntityId = LU.Id, MepRoleId = role.RoleId };
                                    db.MEntityPortfolios.Add(MEntityPortfolio);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
                transaction.Commit();
            }
            return Ok();
            //return CreatedAtRoute("DefaultApi", new { id = RChannel.Id }, RChannel);
        }

        // DELETE: api/RChannels/5
        [ResponseType(typeof(RChannel))]
        public async Task<IHttpActionResult> DeleteRChannel(int id)
        {
            RChannel RChannel = await db.RChannels.FindAsync(id);
            if (RChannel == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ChannelName")));
            }
            try
            {
                db.RChannels.Remove(RChannel);
                await db.SaveChangesAsync();

                return Ok(RChannel);
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

        private bool RChannelExists(int id)
        {
            return db.RChannels.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("UQ_RChannels_RcPrimaryChannel", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "CHANNEL"));
            else if (SqEx.Message.IndexOf("FK_RChannels_LPortfolios", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "CHANNEL", "PORTFOLIOS"));
            else if (SqEx.Message.IndexOf("FK_RChannels_RSubChannels", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "CHANNEL", "SUB CHANNEL"));
            else if (SqEx.Message.IndexOf("VChannels", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "CHANNEL", "VIEW(S)"));
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
    }
}
