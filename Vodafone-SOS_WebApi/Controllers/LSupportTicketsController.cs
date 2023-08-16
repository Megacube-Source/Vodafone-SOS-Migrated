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
using System.Collections;
using System.Data.Entity.Validation;
using System.Reflection;
using System.Configuration;
using System.Globalization;

namespace Vodafone_SOS_WebApi.Controllers
{
   [CustomExceptionFilter]
    public class LSupportTicketsController : ApiController
    {

        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        public object SqlMethods { get; private set; }

        //public IHttpActionResult GetMyTickets()
        //{
        //    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory)
        //              select new { aa.Id, aa.LstTicketNumber, aa.LstStatus, aa.LstPriority, aa.LstCategoryId, aa.RSupportCategory.RscName, aa.LstSummary, aa.LstTeamId, aa.LstCurrentOwnerId, aa.LstCreatedDateTime }).OrderBy(p => p.LstCreatedDateTime);

        //    return Ok(xx);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;

            //Depending upon the constraint failed return appropriate error message
            if (SqEx.Message.IndexOf("UQ_RSubChannels_ReasonText_CompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
                return ("Can not insert duplicate Sub Channel");
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }
        //[ResponseType(typeof(LSupportTicketContextModel))]
        [HttpPost]
        public IHttpActionResult CreateTicket(LSupportTicketContextModel LSTM ,string FileName, string FilePath,string PortfolioList)
        {
            //Getting ticket no from sequence
            var RawQuery = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo.SQ_TicketNumber");
            var Task = RawQuery.SingleAsync();
            var TktNo = Task.Result;
            Task.Dispose();

            //var iLAstID = 0;

            //iLAstID = db.LSupportTickets.Max(u => u.Id);
            string strOpCo = (from h in db.GCompanies
                              where h.Id==(LSTM.LstCompanyId)
                              select h.GcCode).SingleOrDefault();
            string strTicketNo = strOpCo + "0000001";
            if (TktNo != null)
            {
                int iTktNo = Convert.ToInt32(TktNo);
                strTicketNo = strOpCo;
                //Generating zeros for ticket number as apart from OpCo code, there needs to be 7 digits in ticket number
                for (int i = 0; i < 7 - (iTktNo).ToString().Length; i++)
                {
                    strTicketNo = strTicketNo + "0";
                }
                strTicketNo = strTicketNo + Convert.ToString((iTktNo));
            }

            LSupportTicket Lst = new Models.LSupportTicket();
            Lst.LstTicketNumber = strTicketNo;
            Lst.LstPhone = LSTM.LstPhone;
            Lst.LstType = LSTM.LstType;
            Lst.LstClosureCode = LSTM.LstClosureCode;
            Lst.LstSeverity = LSTM.LstSeverity;
            Lst.LstImpact = LSTM.LstImpact;
            Lst.LstPriority = LSTM.LstPriority;
            Lst.LstSummary = LSTM.LstSummary;
            Lst.LstCreatedDateTime = DateTime.Now; //LSTM.LstCreatedDateTime;
           // Lst.LstCC = LSTM.LstCC;
            Lst.LstStatus = LSTM.LstStatus;
            Lst.LstLastUpdatedDateTime = DateTime.Now; //LSTM.LstLastUpdatedDateTime;
            Lst.LstTeamId = LSTM.LstTeamId;
            Lst.LstCreatedById = LSTM.LstCreatedById;
            if(LSTM.LstCompanyId.HasValue)
            Lst.LstCompanyId = LSTM.LstCompanyId.Value;
            Lst.LstCreatedOnBehalfOfId = LSTM.LstCreatedOnBehalfOfId;
            Lst.LstLastUpdatedById = LSTM.LstLastUpdatedById;
            Lst.LstL1Id = LSTM.LstL1Id;
            Lst.LstL2Id = LSTM.LstL2Id;
            Lst.LstL3Id = LSTM.LstL3Id;
            Lst.LstCurrentOwnerId = LSTM.LstCurrentOwnerId;
            if (LSTM.LstCategoryID.HasValue)
                Lst.LstCategoryId = LSTM.LstCategoryID.Value;
            if (LSTM.LstStageID.HasValue)
                Lst.LstStageId = LSTM.LstStageID.Value;
            Lst.Ordinal = LSTM.Ordinal;
            LSupportRespons Lsr = new Models.LSupportRespons();
            Lsr.LsrResponseDateTime = DateTime.Now;
            Lsr.LsrDescription = LSTM.LsrDescription;
            Lsr.LsrTicketStatus = Lst.LstStatus;
            Lsr.LsrResponseById = Lst.LstCreatedById;

            //Ticket Assignment to L1(for that OpCo only)
            LSupportTicketAssignment LTA = new Models.LSupportTicketAssignment();
            LTA.LstaCreatedDateTime = DateTime.Now;
            LTA.LstaAssignedById = Lst.LstCreatedById;
            LTA.LstaSupportTeamId = Convert.ToInt32(Lst.LstTeamId);

            //if (!ModelState.IsValid)
            //{
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "TICKET")));
            //}
            try
            {

                db.LSupportTickets.Add(Lst);
                db.SaveChanges();

                if (!string.IsNullOrEmpty(FileName))
                {
                    var FilesArray = FileName.Split(',').ToList();
                    foreach (var file in FilesArray)
                    {
                        var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = LSTM.LstCreatedById, LsdUpdatedById = LSTM.LstCreatedById, LsdFileName = file, LsdFilePath = FilePath, LsdEntityType = "LSupportTickets", LsdEntityId = Lst.Id, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedDateTime = DateTime.UtcNow };
                        db.LSupportingDocuments.Add(LSupportingDocuments);
                        db.SaveChanges();
                    }
                }

                Lsr.LsrSupportTicketId = Lst.Id;
               // Lsr.LsrDescription = LSTM.LstDescription;
                db.LSupportResponses.Add(Lsr);
                db.SaveChanges();
                
                LTA.LstaSupportTicketId = Lst.Id;
                db.LSupportTicketAssignments.Add(LTA);
                db.SaveChanges();

                
                //Add Portfolio to Support Tickets
                if(!string.IsNullOrEmpty(PortfolioList))
                {
                    var PortfolioArray = PortfolioList.Split(',').ToList();
                    foreach (var PF in PortfolioArray)
                    {
                        var PortfolioId = Convert.ToInt32(PF);
                        var MSupportPortfolio = new MSupportTicketPortfolio { PortfolioId =PortfolioId,TicketId=Lst.Id};
                        db.MSupportTicketPortfolios.Add(MSupportPortfolio);
                        db.SaveChanges();
                    }
                }

            }
            catch (DbEntityValidationException dbex)
            {
                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {
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


            return Ok(strTicketNo);
            //return CreatedAtRoute("DefaultApi", new { id = LSTM.Id }, LSTM);
        }
        [ResponseType(typeof(void))]
        public void AddTicketResponse(LSupportRespons LSRespons)
        {
            db.LSupportResponses.Add(LSRespons);
            db.SaveChanges();

        }

        [HttpGet]
        public IHttpActionResult MassActionUpdate(string Action,string TransactionIdList,string LoggedInUserId,string Comments)
        {
            if(!string.IsNullOrEmpty(TransactionIdList))
            {
                var ListOfIds = TransactionIdList.Split(',');
                foreach(var Id in ListOfIds)
                {
                    var TicketId = Convert.ToInt32(Id);
                    var STVM = db.LSupportTickets.Where(p => p.Id == TicketId).FirstOrDefault();
                    //Perform Action only if LoggedInUser is the Current Owner of Ticket
                    //RK 30052018- As requested by business users (ANA) loggedin user can move tickets irrespective of the user is current owner or not
                    //same with created by user also
                    //if (STVM.LstCurrentOwnerId == LoggedInUserId||STVM.LstCreatedById==LoggedInUserId)
                    //{
                        switch (Action)
                        {
                            case "BackToL1":
                                STVM.LstStatus = "WIP";
                                STVM.Ordinal = 2;
                                break;
                            case "BackToRequestor":
                                STVM.Ordinal = 1;
                                STVM.LstCurrentOwnerId = STVM.LstCreatedById;
                                break;
                            case "ForwardToL1":
                                STVM.LstCurrentOwnerId = STVM.LstL1Id;
                                STVM.Ordinal = 2;
                                break;
                            case "ForwardToL2":
                                STVM.LstCurrentOwnerId = STVM.LstL2Id;
                                STVM.LstTeamId = db.RsupportTeams.Where(p => p.RstTeamName == ("L2 " + STVM.LstType)).Select(p => p.Id).FirstOrDefault();
                                STVM.Ordinal = 3;
                                break;
                        }
                   
                    STVM.LstLastUpdatedById = LoggedInUserId;
                    STVM.LstLastUpdatedDateTime = DateTime.UtcNow;
                    //Update SupportTicket
                    db.Entry(STVM).State = EntityState.Modified;
                    db.SaveChanges();
                    //Add entry in Support ticket responses
                    var Lsr = new LSupportRespons { LsrResponseDateTime = DateTime.Now, LsrDescription = Comments, LsrTicketStatus = Action, LsrSupportTicketId = STVM.Id, LsrResponseById =LoggedInUserId };
                    db.LSupportResponses.Add(Lsr);
                    db.SaveChanges();
                    //}
                  
                }
            }
            return Ok();
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> SelftAssignTicket(int id, LSupportTicketContextModel LSTM)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }

            if (id != LSTM.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }
            try
            {

                LSupportTicket Lst = new LSupportTicket();
                Lst.Id = LSTM.Id;
                Lst.LstTicketNumber = LSTM.LstTicketNumber;
                Lst.LstPhone = LSTM.LstPhone;
                Lst.LstType = LSTM.LstType;
                Lst.LstClosureCode = LSTM.LstClosureCode;
                Lst.LstSeverity = LSTM.LstSeverity;
                Lst.LstImpact = LSTM.LstImpact;
                Lst.LstPriority = LSTM.LstPriority;
                Lst.LstSummary = LSTM.LstSummary;
                if (LSTM.LstStageID.HasValue)
                    Lst.LstStageId = LSTM.LstStageID.Value;
                if (LSTM.LstCreatedDateTime.HasValue)
                    Lst.LstCreatedDateTime = LSTM.LstCreatedDateTime.Value;
                //Lst.LstCC = LSTM.LstCC;
                Lst.LstStatus = LSTM.LstStatus;
                Lst.LstLastUpdatedDateTime = DateTime.Now; //LSTM.LstLastUpdatedDateTime;
                Lst.LstTeamId = LSTM.LstTeamId;
                Lst.LstCreatedById = LSTM.LstCreatedById;
                if (LSTM.LstCompanyId.HasValue)
                    Lst.LstCompanyId = LSTM.LstCompanyId.Value;
                Lst.LstCreatedOnBehalfOfId = LSTM.LstCreatedOnBehalfOfId;
                Lst.LstLastUpdatedById = LSTM.LstLastUpdatedById;
                Lst.LstL1Id = LSTM.LstL1Id;
                Lst.LstL2Id = LSTM.LstL2Id;
                Lst.LstL3Id = LSTM.LstL3Id;
                Lst.LstCurrentOwnerId = LSTM.LstaAssignedToId;
                if (LSTM.LstCategoryID.HasValue)
                    Lst.LstCategoryId = LSTM.LstCategoryID.Value;
                Lst.Ordinal = LSTM.Ordinal;
                db.Entry(Lst).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //Ticket Response
                var Lsr = new LSupportRespons { LsrResponseDateTime = DateTime.Now, LsrDescription = LSTM.LsrDescription, LsrTicketStatus = LSTM.LstStatus, LsrSupportTicketId = LSTM.Id, LsrResponseById = LSTM.LstLastUpdatedById };
                db.LSupportResponses.Add(Lsr);
                db.SaveChanges();
                //Ticket Assignment to L1(for that OpCo only)
                LSupportTicketAssignment LTA = new Models.LSupportTicketAssignment();

                var Lta = new LSupportTicketAssignment { LstaCreatedDateTime = DateTime.Now, LstaSupportTicketId = LSTM.Id, LstaAssignedById = LSTM.LstLastUpdatedById, LstaAssignedToId = LSTM.LstCurrentOwnerId, LstaSupportTeamId = Convert.ToInt32(LSTM.LstTeamId) };
                db.LSupportTicketAssignments.Add(LTA);
                db.SaveChanges();


            }
            catch (Exception ex)
            {
                if (!TicketExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Support Ticket")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public IHttpActionResult MassSelfAssignL2(string TransactionIdList, string LoggedInUserId, string Comments)
        {
            if (!string.IsNullOrEmpty(TransactionIdList))
            {
                var ListOfIds = TransactionIdList.Split(',');
                foreach (var Id in ListOfIds)
                {
                    var TicketId = Convert.ToInt32(Id);
                    var STVM = db.LSupportTickets.Where(p => p.Id == TicketId).FirstOrDefault();

                    STVM.LstLastUpdatedDateTime = DateTime.UtcNow;
                    STVM.LstL2Id = LoggedInUserId;
                    STVM.LstLastUpdatedById = LoggedInUserId;
                    STVM.Ordinal = 3;
                    STVM.LstStatus = "WIP";
                    STVM.LstCurrentOwnerId = LoggedInUserId;
                    STVM.LstTeamId = db.RsupportTeams.Where(p => p.RstTeamName == ("L2 " + STVM.LstType)).Select(p => p.Id).FirstOrDefault();
                    //Update SupportTicket
                    db.Entry(STVM).State = EntityState.Modified;
                    db.SaveChanges();

                    //Add entry in Support ticket responses
                    var Lsr = new LSupportRespons { LsrResponseDateTime = DateTime.Now, LsrDescription = Comments, LsrTicketStatus = "SelfAssign", LsrSupportTicketId = STVM.Id, LsrResponseById = LoggedInUserId };
                    db.LSupportResponses.Add(Lsr);
                    db.SaveChanges();

                    //Ticket Assignment to L2
                    var Lta = new LSupportTicketAssignment { LstaCreatedDateTime = DateTime.Now, LstaSupportTicketId = STVM.Id, LstaAssignedById = STVM.LstLastUpdatedById, LstaAssignedToId = STVM.LstL2Id, LstaSupportTeamId = Convert.ToInt32(STVM.LstTeamId) };
                    db.LSupportTicketAssignments.Add(Lta);
                    db.SaveChanges();

                }
            }
            return Ok();
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> UpdateTicket(int id, LSupportTicketContextModel LSTM,string FileName,string FilePath,string PortfolioList)
        {
            //if (!ModelState.IsValid)
            //{
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            //}

            if (id != LSTM.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }
            try
            {
                LSupportTicket Lst = new Models.LSupportTicket();
                Lst.Id = LSTM.Id;
                Lst.LstTicketNumber = LSTM.LstTicketNumber;
                Lst.LstPhone = LSTM.LstPhone;
                Lst.LstType = LSTM.LstType;
                Lst.LstClosureCode = LSTM.LstClosureCode;
                Lst.LstSeverity = LSTM.LstSeverity;
                Lst.LstImpact = LSTM.LstImpact;
                Lst.LstPriority = LSTM.LstPriority;
                Lst.LstSummary = LSTM.LstSummary;
                if (LSTM.LstStageID.HasValue)
                    Lst.LstStageId = LSTM.LstStageID.Value;
                if (LSTM.LstCreatedDateTime.HasValue)
                    Lst.LstCreatedDateTime = LSTM.LstCreatedDateTime.Value;
                //Lst.LstCC = LSTM.LstCC;
                Lst.LstStatus = LSTM.LstStatus;
                Lst.LstLastUpdatedDateTime = DateTime.Now; //LSTM.LstLastUpdatedDateTime;
                if (LSTM.LstTeamId != null) Lst.LstTeamId = LSTM.LstTeamId;
                Lst.LstCreatedById = LSTM.LstCreatedById;
                if (LSTM.LstCompanyId.HasValue)
                    Lst.LstCompanyId = LSTM.LstCompanyId.Value;
                Lst.LstCreatedOnBehalfOfId = LSTM.LstCreatedOnBehalfOfId;
                Lst.LstLastUpdatedById = LSTM.LstLastUpdatedById;
                Lst.LstL1Id = LSTM.LstL1Id;
                Lst.LstL2Id = LSTM.LstL2Id;
                Lst.LstL3Id = LSTM.LstL3Id;
                Lst.LstCurrentOwnerId = LSTM.LstCurrentOwnerId;
                if (LSTM.LstCategoryID.HasValue)
                    Lst.LstCategoryId = LSTM.LstCategoryID.Value;
                Lst.Ordinal = LSTM.Ordinal;
                Lst.LstStageId = LSTM.LstStageID;
                db.Entry(Lst).State = EntityState.Modified;
                //db.SaveChanges();
                await db.SaveChangesAsync();
                var Lsr = new LSupportRespons { LsrResponseDateTime = DateTime.Now, LsrDescription = LSTM.LsrDescription, LsrTicketStatus = LSTM.LstStatus, LsrSupportTicketId = LSTM.Id, LsrResponseById = LSTM.LstLastUpdatedById };

                db.LSupportResponses.Add(Lsr);
                db.SaveChanges();


                if (!string.IsNullOrEmpty(FileName))
                {
                    var FilesArray = FileName.Split(',').ToList();
                    foreach (var file in FilesArray)
                    {
                        var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = LSTM.LstCreatedById, LsdUpdatedById = LSTM.LstCreatedById, LsdFileName = file, LsdFilePath = FilePath, LsdEntityType = "LSupportTickets", LsdEntityId = Lst.Id, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedDateTime = DateTime.UtcNow };
                        db.LSupportingDocuments.Add(LSupportingDocuments);
                        db.SaveChanges();
                    }
                }
                //Add Portfolio to Support Tickets
                if (!string.IsNullOrEmpty(PortfolioList))
                {
                    db.Database.ExecuteSqlCommand("delete from MSupportTicketPortfolios where TicketId={0}", Lst.Id);
                    var PortfolioArray = PortfolioList.Split(',').ToList();
                    foreach (var PF in PortfolioArray)
                    {
                        var PortfolioId = Convert.ToInt32(PF);
                        var MSupportPortfolio = new MSupportTicketPortfolio { PortfolioId = PortfolioId, TicketId = Lst.Id };
                        db.MSupportTicketPortfolios.Add(MSupportPortfolio);
                        db.SaveChanges();
                    }
                }
                ////Ticket Assignment to L1(for that OpCo only)
                //LSupportTicketAssignment LTA = new Models.LSupportTicketAssignment();
                //if (LSTM.LstTeamId != null)
                //{
                //    LTA.LstaCreatedDateTime = DateTime.Now;
                //    LTA.LstaAssignedById = Lst.LstLastUpdatedById;
                //    LTA.LstaSupportTeamId = Convert.ToInt32(LSTM.LstTeamId);
                //}

            }
            //catch (System.Data.Entity.Validation.DbEntityValidationException e)
            //{
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.ToString()));// GetCustomizedErrorMessage(ex)));
            //}
            //catch (Exception ex)
            //{
            //    if (!TicketExists(id))
            //    {
            //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Support Ticket")));
            //    }
            //    else
            //    {
            //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString()));// GetCustomizedErrorMessage(ex)));
            //    }
            //}
            catch (DbEntityValidationException dbex)
            {
                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {
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
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ForwardTicket(int id, LSupportTicketContextModel LSTM)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }

            if (id != LSTM.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }
            try
            {
                LSupportTicket Lst = new Models.LSupportTicket();
                Lst.Id = LSTM.Id;
                Lst.LstTicketNumber = LSTM.LstTicketNumber;
                Lst.LstPhone = LSTM.LstPhone;
                Lst.LstType = LSTM.LstType;
                Lst.LstClosureCode = LSTM.LstClosureCode;
                Lst.LstSeverity = LSTM.LstSeverity;
                Lst.LstImpact = LSTM.LstImpact;
                Lst.LstPriority = LSTM.LstPriority;
                Lst.LstSummary = LSTM.LstSummary;
                if (LSTM.LstStageID.HasValue)
                    Lst.LstStageId = LSTM.LstStageID.Value;
                if (LSTM.LstCreatedDateTime.HasValue)
                    Lst.LstCreatedDateTime = LSTM.LstCreatedDateTime.Value;
                //Lst.LstCC = LSTM.LstCC;
                Lst.LstStatus = LSTM.LstStatus;
                Lst.LstLastUpdatedDateTime = DateTime.Now; //LSTM.LstLastUpdatedDateTime;
                Lst.LstTeamId = LSTM.LstTeamId;
                Lst.LstCreatedById = LSTM.LstCreatedById;
                if (LSTM.LstCompanyId.HasValue)
                    Lst.LstCompanyId = LSTM.LstCompanyId.Value;
                Lst.LstCreatedOnBehalfOfId = LSTM.LstCreatedOnBehalfOfId;
                Lst.LstLastUpdatedById = LSTM.LstLastUpdatedById;
                Lst.LstL1Id = LSTM.LstL1Id;
                Lst.LstL2Id = LSTM.LstL2Id;
                Lst.LstL3Id = LSTM.LstL3Id;
                Lst.Ordinal = LSTM.Ordinal;
                Lst.LstCurrentOwnerId = LSTM.LstCurrentOwnerId;
                //Lst.LstCurrentOwnerId = LSTM.LstaAssignedToId;
                if (LSTM.LstCategoryID.HasValue)
                    Lst.LstCategoryId = LSTM.LstCategoryID.Value;

                db.Entry(Lst).State = EntityState.Modified;
                await db.SaveChangesAsync();
                var Lsr = new LSupportRespons { LsrResponseDateTime = DateTime.Now, LsrDescription = LSTM.LsrDescription, LsrTicketStatus = LSTM.LstStatus, LsrSupportTicketId = LSTM.Id, LsrResponseById = LSTM.LstLastUpdatedById };

                db.LSupportResponses.Add(Lsr);
                db.SaveChanges();

                ////Ticket Assignment to L1(for that OpCo only)
                var Lta = new LSupportTicketAssignment { LstaCreatedDateTime = DateTime.Now, LstaSupportTicketId = LSTM.Id, LstaAssignedById = LSTM.LstLastUpdatedById, LstaSupportTeamId = Convert.ToInt32(LSTM.LstTeamId) };
                LSupportTicketAssignment LTA = new Models.LSupportTicketAssignment();
                db.LSupportTicketAssignments.Add(Lta);
                db.SaveChanges();


            }
            catch (Exception ex)
            {
                if (!TicketExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Support Ticket")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> BackToL1Ticket(int id, LSupportTicketContextModel LSTM)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }

            if (id != LSTM.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }
            try
            {
                LSupportTicket Lst = new Models.LSupportTicket();
                Lst.Id = LSTM.Id;
                Lst.LstTicketNumber = LSTM.LstTicketNumber;
                Lst.LstPhone = LSTM.LstPhone;
                Lst.LstType = LSTM.LstType;
                Lst.LstClosureCode = LSTM.LstClosureCode;
                Lst.LstSeverity = LSTM.LstSeverity;
                Lst.LstImpact = LSTM.LstImpact;
                Lst.LstPriority = LSTM.LstPriority;
                Lst.LstSummary = LSTM.LstSummary;
                if (LSTM.LstStageID.HasValue)
                    Lst.LstStageId = LSTM.LstStageID.Value;
                if (LSTM.LstCreatedDateTime.HasValue)
                    Lst.LstCreatedDateTime = LSTM.LstCreatedDateTime.Value;
                //Lst.LstCC = LSTM.LstCC;
                Lst.LstStatus = LSTM.LstStatus;
                Lst.LstLastUpdatedDateTime = DateTime.Now; //LSTM.LstLastUpdatedDateTime;
                Lst.LstTeamId = LSTM.LstTeamId;
                Lst.LstCreatedById = LSTM.LstCreatedById;
                if (LSTM.LstCompanyId.HasValue)
                    Lst.LstCompanyId = LSTM.LstCompanyId.Value;
                Lst.LstCreatedOnBehalfOfId = LSTM.LstCreatedOnBehalfOfId;
                Lst.LstLastUpdatedById = LSTM.LstLastUpdatedById;
                Lst.LstL1Id = LSTM.LstL1Id;
                Lst.LstL2Id = LSTM.LstL2Id;
                Lst.LstL3Id = LSTM.LstL3Id;
                Lst.Ordinal = LSTM.Ordinal;
                Lst.LstCurrentOwnerId = LSTM.LstL1Id;
                // Lst.LstCurrentOwnerId = LSTM.LstaAssignedToId;
                if (LSTM.LstCategoryID.HasValue)
                    Lst.LstCategoryId = LSTM.LstCategoryID.Value;

                db.Entry(Lst).State = EntityState.Modified;
                await db.SaveChangesAsync();
                var Lsr = new LSupportRespons { LsrResponseDateTime = DateTime.Now, LsrDescription = LSTM.LsrDescription, LsrTicketStatus = LSTM.LstStatus, LsrSupportTicketId = LSTM.Id, LsrResponseById = LSTM.LstLastUpdatedById };

                db.LSupportResponses.Add(Lsr);
                db.SaveChanges();

                ////Ticket Assignment to L1(for that OpCo only)
                var Lta = new LSupportTicketAssignment { LstaCreatedDateTime = DateTime.Now, LstaSupportTicketId = LSTM.Id, LstaAssignedById = LSTM.LstLastUpdatedById, LstaSupportTeamId = Convert.ToInt32(LSTM.LstTeamId) };
                LSupportTicketAssignment LTA = new Models.LSupportTicketAssignment();
                db.LSupportTicketAssignments.Add(Lta);
                db.SaveChanges();


            }
            catch (Exception ex)
            {
                if (!TicketExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Support Ticket")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> BackToRequestorTicket(int id, LSupportTicketContextModel LSTM)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }

            if (id != LSTM.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }
            try
            {
                LSupportTicket Lst = new Models.LSupportTicket();
                Lst.Id = LSTM.Id;
                Lst.LstTicketNumber = LSTM.LstTicketNumber;
                Lst.LstPhone = LSTM.LstPhone;
                Lst.LstType = LSTM.LstType;
                Lst.LstClosureCode = LSTM.LstClosureCode;
                Lst.LstSeverity = LSTM.LstSeverity;
                Lst.LstImpact = LSTM.LstImpact;
                Lst.LstPriority = LSTM.LstPriority;
                Lst.LstSummary = LSTM.LstSummary;
                if(LSTM.LstStageID.HasValue)
                    Lst.LstStageId = LSTM.LstStageID.Value;
                if (LSTM.LstCreatedDateTime.HasValue)
                    Lst.LstCreatedDateTime = LSTM.LstCreatedDateTime.Value;
               // Lst.LstCC = LSTM.LstCC;
                Lst.LstStatus = LSTM.LstStatus;
                Lst.LstLastUpdatedDateTime = DateTime.Now; //LSTM.LstLastUpdatedDateTime;
                Lst.LstTeamId = LSTM.LstTeamId;
                Lst.LstCreatedById = LSTM.LstCreatedById;
                if (LSTM.LstCompanyId.HasValue)
                    Lst.LstCompanyId = LSTM.LstCompanyId.Value;
                Lst.LstCreatedOnBehalfOfId = LSTM.LstCreatedOnBehalfOfId;
                Lst.LstLastUpdatedById = LSTM.LstLastUpdatedById;
                Lst.LstL1Id = LSTM.LstL1Id;
                Lst.LstL2Id = LSTM.LstL2Id;
                Lst.LstL3Id = LSTM.LstL3Id;
                Lst.Ordinal = LSTM.Ordinal;
                Lst.LstCurrentOwnerId = LSTM.LstCreatedById;
                // Lst.LstCurrentOwnerId = LSTM.LstaAssignedToId;
                if (LSTM.LstCategoryID.HasValue)
                    Lst.LstCategoryId = LSTM.LstCategoryID.Value;

                db.Entry(Lst).State = EntityState.Modified;
                await db.SaveChangesAsync();
                var Lsr = new LSupportRespons { LsrResponseDateTime = DateTime.Now, LsrDescription = LSTM.LsrDescription, LsrTicketStatus = LSTM.LstStatus, LsrSupportTicketId = LSTM.Id, LsrResponseById = LSTM.LstLastUpdatedById };

                db.LSupportResponses.Add(Lsr);
                db.SaveChanges();

                ////Ticket Assignment to L1(for that OpCo only)
                var Lta = new LSupportTicketAssignment { LstaCreatedDateTime = DateTime.Now, LstaSupportTicketId = LSTM.Id, LstaAssignedById = LSTM.LstLastUpdatedById, LstaSupportTeamId = Convert.ToInt32(LSTM.LstTeamId) };
                LSupportTicketAssignment LTA = new Models.LSupportTicketAssignment();
                db.LSupportTicketAssignments.Add(Lta);
                db.SaveChanges();


            }
            catch (Exception ex)
            {
                if (!TicketExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Support Ticket")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError,ex));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> MassActionUpdateAssignTicket(int id, LSupportTicketContextModel LSTM)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }

            if (id != LSTM.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }
            try
            {

                LSupportTicket Lst = new LSupportTicket();
                Lst.Id = LSTM.Id;
                Lst.LstTicketNumber = LSTM.LstTicketNumber;
                Lst.LstPhone = LSTM.LstPhone;
                Lst.LstType = LSTM.LstType;
                Lst.LstClosureCode = LSTM.LstClosureCode;
                Lst.LstSeverity = LSTM.LstSeverity;
                Lst.LstImpact = LSTM.LstImpact;
                Lst.LstPriority = LSTM.LstPriority;
                Lst.LstSummary = LSTM.LstSummary;
                if (LSTM.LstCreatedDateTime.HasValue)
                    Lst.LstCreatedDateTime = LSTM.LstCreatedDateTime.Value;
                //Lst.LstCC = LSTM.LstCC;
                Lst.LstStatus = LSTM.LstStatus;
                Lst.LstLastUpdatedDateTime = DateTime.Now; //LSTM.LstLastUpdatedDateTime;
                Lst.LstTeamId = LSTM.LstTeamId;
                Lst.LstCreatedById = LSTM.LstCreatedById;
                if (LSTM.LstCompanyId.HasValue)
                    Lst.LstCompanyId = LSTM.LstCompanyId.Value;
                Lst.LstCreatedOnBehalfOfId = LSTM.LstCreatedOnBehalfOfId;
                Lst.LstLastUpdatedById = LSTM.LstLastUpdatedById;
                Lst.LstL1Id = LSTM.LstL1Id;
                Lst.LstL2Id = LSTM.LstL2Id;
                Lst.LstL3Id = LSTM.LstL3Id;
                Lst.LstCurrentOwnerId = LSTM.LstaAssignedToId;
                if (LSTM.LstCategoryID.HasValue)
                    Lst.LstCategoryId = LSTM.LstCategoryID.Value;
                Lst.Ordinal = LSTM.Ordinal;
                db.Entry(Lst).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //Ticket Response
                var Lsr = new LSupportRespons { LsrResponseDateTime = DateTime.Now, LsrDescription = LSTM.LsrDescription, LsrTicketStatus = LSTM.LstStatus, LsrSupportTicketId = LSTM.Id, LsrResponseById = LSTM.LstLastUpdatedById };
                db.LSupportResponses.Add(Lsr);
                db.SaveChanges();
                //Ticket Assignment to L1(for that OpCo only)
                LSupportTicketAssignment LTA = new Models.LSupportTicketAssignment();

                var Lta = new LSupportTicketAssignment { LstaCreatedDateTime = DateTime.Now, LstaSupportTicketId = LSTM.Id, LstaAssignedById = LSTM.LstLastUpdatedById, LstaAssignedToId = LSTM.LstCurrentOwnerId, LstaSupportTeamId = Convert.ToInt32(LSTM.LstTeamId) };
                db.LSupportTicketAssignments.Add(LTA);
                db.SaveChanges();


            }
            catch (Exception ex)
            {
                if (!TicketExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Support Ticket")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AssignTicketToL2(int id, LSupportTicketContextModel LSTM)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }

            if (id != LSTM.Id)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "TICKET")));
            }
            try
            {

                LSupportTicket Lst = new LSupportTicket();
                Lst.Id = LSTM.Id;
                Lst.LstTicketNumber = LSTM.LstTicketNumber;
                Lst.LstPhone = LSTM.LstPhone;
                Lst.LstType = LSTM.LstType;
                Lst.LstClosureCode = LSTM.LstClosureCode;
                Lst.LstSeverity = LSTM.LstSeverity;
                Lst.LstImpact = LSTM.LstImpact;
                Lst.LstPriority = LSTM.LstPriority;
                Lst.LstSummary = LSTM.LstSummary;
                if (LSTM.LstStageID.HasValue)
                    Lst.LstStageId = LSTM.LstStageID.Value;
                if (LSTM.LstCreatedDateTime.HasValue)
                    Lst.LstCreatedDateTime = LSTM.LstCreatedDateTime.Value;
                //Lst.LstCC = LSTM.LstCC;
                Lst.LstStatus = LSTM.LstStatus;
                Lst.LstLastUpdatedDateTime = DateTime.Now; //LSTM.LstLastUpdatedDateTime;
                Lst.LstTeamId = LSTM.LstTeamId;
                Lst.LstCreatedById = LSTM.LstCreatedById;
                if (LSTM.LstCompanyId.HasValue)
                    Lst.LstCompanyId = LSTM.LstCompanyId.Value;
                Lst.LstCreatedOnBehalfOfId = LSTM.LstCreatedOnBehalfOfId;
                Lst.LstLastUpdatedById = LSTM.LstLastUpdatedById;
                Lst.LstL1Id = LSTM.LstL1Id;
                Lst.LstL2Id = LSTM.LstL2Id;
                Lst.LstL3Id = LSTM.LstL3Id;
                Lst.LstCurrentOwnerId = LSTM.LstCurrentOwnerId;
                if (LSTM.LstCategoryID.HasValue)
                    Lst.LstCategoryId = LSTM.LstCategoryID.Value;
                Lst.Ordinal = LSTM.Ordinal;
                db.Entry(Lst).State = EntityState.Modified;
                await db.SaveChangesAsync();
                
                //Ticket Response
                var Lsr = new LSupportRespons { LsrResponseDateTime = DateTime.Now, LsrDescription = LSTM.LsrDescription, LsrTicketStatus = LSTM.LsrTicketStatus, LsrSupportTicketId = LSTM.Id, LsrResponseById = LSTM.LstLastUpdatedById };
                db.LSupportResponses.Add(Lsr);
                db.SaveChanges();
                //Ticket Assignment to L2
                //LSupportTicketAssignment LTA = new Models.LSupportTicketAssignment();

                //Ticket Assignment to L2
                var Lta = new LSupportTicketAssignment { LstaCreatedDateTime = DateTime.Now, LstaSupportTicketId = LSTM.Id, LstaAssignedById = LSTM.LstLastUpdatedById, LstaAssignedToId = LSTM.LstL2Id, LstaSupportTeamId = Convert.ToInt32(LSTM.LstTeamId) };
                db.LSupportTicketAssignments.Add(Lta);
                db.SaveChanges();


            }
            catch (Exception ex)
            {
                if (!TicketExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Support Ticket")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        public IHttpActionResult GetL2Assignees(string TeamName)
        {
            //first select the roles in the team
            var selectedroles = db.RsupportTeams.Where(p => p.RstTeamName.Equals(TeamName)).Select(p => p.RstRoleId).ToList();

            List<string> PossibleAssignees = new List<string>(); 
            foreach (var role in selectedroles)
            {
                PossibleAssignees = db.AspNetUserRoles.Where(p => role.Equals(p.RoleId)).Select(p => p.UserId).ToList();
            }

            var UserList = db.LUsers.Where(p => p.WFStatus=="Completed" && PossibleAssignees.Contains(p.LuUserId)).Select(p => new { FullName = p.LuFirstName + " " + p.LuLastName, Id = p.LuUserId }).ToList();
            return Ok(UserList);
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

        public IHttpActionResult GetRequestorTickets(string strUserId,int CompanyId,string RoleId)
        {
            var AllowedTeamName = "L2 SOS,L2 Infra,L2 Alteryx".Split(',');
            var L2TeamIdList = db.RsupportTeams.Where(p => AllowedTeamName.Contains(p.RstTeamName)).Select(p => p.RstRoleId).ToList();
            var RSupportTeamMember = db.RsupportTeams.Where(p => p.RstRoleId.Equals(RoleId)).FirstOrDefault();
            var yy = db.GCompanies.Where(c => c.GcCode == "99").FirstOrDefault().Id;


            string strOpCo = (from h in db.GCompanies
                              where h.Id == (CompanyId)
                              select h.GcCode).SingleOrDefault();

            /*NOTE: For Non-Support team member, show only those tickets that are created by me as a normal user*/
            if (RSupportTeamMember == null)
            {
                var payeeid = db.AspNetRoles.Where(a => a.Name == "Payee" && a.CompanyCode == strOpCo).Select(a => a.Id).FirstOrDefault();
                
                var role = db.AspNetUserRoles.Where(a => a.UserId == strUserId && a.RoleId == payeeid).Select(a => a.RoleId).FirstOrDefault();

                if (role != null )
                {
                   int parentpayeeid = db.LPayees.Where(a => a.LpUserId == strUserId).Select(a => a.Id).FirstOrDefault();
                   int parentpayeetableentry = db.LPayees.Where(a => a.LpUserId == strUserId).Select(a => a.Id).FirstOrDefault();
                   var haschildren = (from x in db.LPayeeParents where x.LppParentPayeeId == parentpayeetableentry select x.Id).FirstOrDefault();
                    if (haschildren ==0)
                    {
                        var payeedetail =
                            (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                                  // join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                             from CO in COJoin.DefaultIfEmpty()
                             join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                             from SO in SOjoin.DefaultIfEmpty()
                             join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                             from KO in KOJoin.DefaultIfEmpty()
                                  //condition for child payee
                              join l in db.LPayees on aa.LstCreatedById equals l.LpUserId
                              //join m in db.LPayeeParents on l.Id equals m.LppParentPayeeId
                              where
                               aa.LstCreatedById.Equals(strUserId) &&
                               aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 1 && l.Id == parentpayeeid 
                             select new
                             {
                                 aa.Id,
                                 aa.LstTicketNumber,
                                 aa.LstStatus,
                                 aa.LstPriority,
                                 aa.LstCategoryId,
                                 aa.RSupportCategory.RscName,
                                 aa.LstSummary,
                                 aa.LstTeamId,
                                 aa.LstType,
                                 aa.LstStageId,
                                 RstName = KO.RtsName,
                                 aa.RsupportTeam.RstTeamName,
                                 LstCreatedById = SO.Email,
                                 LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                 aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).AsEnumerable()
                            .Select(aa => new {
                                LsrDescription = GetSupportTicketResponses(aa.Id),
                                FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                aa.Id,
                                aa.LstTicketNumber,
                                aa.LstStatus,
                                aa.LstPriority,
                                aa.LstCategoryId,
                                aa.RscName,
                                aa.LstSummary,
                                aa.LstTeamId,
                                aa.LstType,
                                aa.LstStageId,
                                aa.RstName,
                                aa.RstTeamName,
                                aa.LstCreatedById,
                                aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                 aa.LstCreatedDateTime,
                                aa.LstLastUpdatedDateTime//,bb.LsrDescription
                             }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                        return Ok(payeedetail);

                        
                    }

                    else
                    {
                        var childlist = db.LPayeeParents.Where(p => p.LppParentPayeeId == parentpayeeid).Select(p => p.LppPayeeId).ToList();
                        var arrayitem = "'";
                        for (int i = 0; i < childlist.Count; i++)
                        {                           
                            int value = childlist[i];
                            var tt = db.LPayees.Where(p => p.Id == value).Select(p => p.LpUserId).FirstOrDefault();
                            if (tt !=null)
                            {
                                arrayitem +=  "','" + tt ;
                            }
                        }

                        arrayitem = arrayitem.Trim().TrimStart(',');
                        arrayitem = arrayitem + "'" + ",'" + strUserId + "'";
                        arrayitem = arrayitem.Substring(3);
                        var payeedetail =
                             (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where
                                //aa.LstCreatedById.Equals(strUserId) &&                               
                                aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 1 && arrayitem.Contains(aa.LstCreatedById)                               
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  RstName = KO.RtsName,
                                  aa.RsupportTeam.RstTeamName,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).AsEnumerable()
                             .Select(aa => new {
                                 LsrDescription = GetSupportTicketResponses(aa.Id),
                                 FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                 aa.Id,
                                 aa.LstTicketNumber,
                                 aa.LstStatus,
                                 aa.LstPriority,
                                 aa.LstCategoryId,
                                 aa.RscName,
                                 aa.LstSummary,
                                 aa.LstTeamId,
                                 aa.LstType,
                                 aa.LstStageId,
                                 aa.RstName,
                                 aa.RstTeamName,
                                 aa.LstCreatedById,
                                 aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                 aa.LstCreatedDateTime,
                                 aa.LstLastUpdatedDateTime//,bb.LsrDescription
                             }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                        return Ok(payeedetail);
                    }
                   
                }


                else
                {
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))

                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where
                               aa.LstCreatedById.Equals(strUserId) &&
                               aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 1
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  RstName = KO.RtsName,
                                  aa.RsupportTeam.RstTeamName,
                                  LstCreatedById = SO.Email,
                                 LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).AsEnumerable()
                              .Select(aa => new {
                                  LsrDescription = GetSupportTicketResponses(aa.Id),
                                  FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  aa.RstName,
                                  aa.RstTeamName,
                                  aa.LstCreatedById,
                                  aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                    //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                    return Ok(xx);
                }
                
            }
            /*NOTE: Or, if I am Support team member, show only those tickets on which I have worked as L1 or L2 OR if my portfolios match.*/
            else
            {
                if (L2TeamIdList.Contains(RoleId))
                {
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                              join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where
                               aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 1
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  RstName = KO.RtsName,
                                  aa.RsupportTeam.RstTeamName,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).AsEnumerable()
                              .Select(aa => new {
                                  LsrDescription = GetSupportTicketResponses(aa.Id),
                                  FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.RstName,
                                  aa.LstStageId,
                                  aa.RstTeamName,
                                  aa.LstCreatedById,
                                 aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
               // }).OrderBy(p => p.LstCreatedDateTime).ToList();
                    return Ok(xx);
                }
                else
                {

                    //Match Portfolios with loggedin user
                    var EntityId = 0;
                    var LoggedInUser = db.LUsers.Where(p => p.LuUserId == strUserId).FirstOrDefault();
                    if (LoggedInUser == null)
                    {
                        var Payee = db.LPayees.Where(p => p.LpUserId == strUserId).FirstOrDefault();
                        if (Payee != null)
                            EntityId = Payee.Id;
                    }
                    else
                    {
                        EntityId = LoggedInUser.Id;
                    }
                    var LoggedInUserPortfolios = db.MEntityPortfolios.Where(p => p.MepRoleId == RoleId).Where(p => p.MepEntityId == EntityId).Select(p => p.MepPortfolioId).ToList();
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                             // join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join PO in db.MSupportTicketPortfolios on aa.Id equals PO.TicketId
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where
                              (aa.LstL2Id == strUserId || aa.LstL1Id == strUserId || LoggedInUserPortfolios.Contains(PO.PortfolioId)) &&
                               aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 1
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  RstName = KO.RtsName,
                                  aa.RsupportTeam.RstTeamName,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).GroupBy(p=>p.Id).Select(p=>p.FirstOrDefault()).AsEnumerable()
                              .Select(aa => new {
                                  LsrDescription = GetSupportTicketResponses(aa.Id),
                                  FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  aa.RstName,
                                  aa.RstTeamName,
                                  aa.LstCreatedById,
                                  aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                    //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                    return Ok(xx);
                }
            }
            //LstcurrentownerID = (u.LuFirstName+" "+u.LuLastName),aa.LstCreatedDateTime}).OrderBy(p => p.LstCreatedDateTime);
            // aa.LstCurrentOwnerId ,aa.LstCreatedDateTime}).OrderBy(p => p.LstCreatedDateTime);
           // return Ok();
            
        }

        public IHttpActionResult GetSystemAnalystTickets(string strUserId,int CompanyId,string RoleId)
        {
            var AllowedTeamName = "L2 SOS,L2 Infra,L2 Alteryx".Split(',');
            var L2TeamIdList = db.RsupportTeams.Where(p => AllowedTeamName.Contains(p.RstTeamName)).Select(p => p.RstRoleId).ToList();
            var RSupportTeamMember = db.RsupportTeams.Where(p => p.RstRoleId.Equals(RoleId)).FirstOrDefault();
            var yy = db.GCompanies.Where(c => c.GcCode == "99").FirstOrDefault().Id;
            string strOpCo = (from h in db.GCompanies
                              where h.Id == (CompanyId)
                              select h.GcCode).SingleOrDefault();
            /*NOTE: For Non-Support team member, show only those tickets that are created by me as a normal user*/
            if (RSupportTeamMember == null)
            {
                var payeeid = db.AspNetRoles.Where(a => a.Name == "Payee" && a.CompanyCode == strOpCo).Select(a => a.Id).FirstOrDefault();

                var role = db.AspNetUserRoles.Where(a => a.UserId == strUserId && a.RoleId == payeeid).Select(a => a.RoleId).FirstOrDefault();

                if (role != null)
                {
                    int parentpayeeid = db.LPayees.Where(a => a.LpUserId == strUserId).Select(a => a.Id).FirstOrDefault();
                    int parentpayeetableentry = db.LPayees.Where(a => a.LpUserId == strUserId).Select(a => a.Id).FirstOrDefault();
                    var haschildren = (from x in db.LPayeeParents where x.LppParentPayeeId == parentpayeetableentry select x.Id).FirstOrDefault();
                    if (haschildren == 0)
                    {
                        var payeedetail =
                            (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                                 // join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                             join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                             from CO in COJoin.DefaultIfEmpty()
                             join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                             from SO in SOjoin.DefaultIfEmpty()
                             join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                             from KO in KOJoin.DefaultIfEmpty()
                                 //condition for child payee
                             join l in db.LPayees on aa.LstCreatedById equals l.LpUserId
                             //join m in db.LPayeeParents on l.Id equals m.LppParentPayeeId
                             where
                              aa.LstCreatedById.Equals(strUserId) &&
                              aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 2 && l.Id == parentpayeeid
                             select new
                             {
                                 aa.Id,
                                 aa.LstTicketNumber,
                                 aa.LstStatus,
                                 aa.LstPriority,
                                 aa.LstCategoryId,
                                 aa.RSupportCategory.RscName,
                                 aa.LstSummary,
                                 aa.LstTeamId,
                                 aa.LstType,
                                 aa.LstStageId,
                                 RstName = KO.RtsName,
                                 aa.RsupportTeam.RstTeamName,
                                 LstCreatedById = SO.Email,
                                 LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                 aa.LstCreatedDateTime,
                                 aa.LstLastUpdatedDateTime//,bb.LsrDescription
                             }).AsEnumerable()
                            .Select(aa => new {
                                LsrDescription = GetSupportTicketResponses(aa.Id),
                                FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                aa.Id,
                                aa.LstTicketNumber,
                                aa.LstStatus,
                                aa.LstPriority,
                                aa.LstCategoryId,
                                aa.RscName,
                                aa.LstSummary,
                                aa.LstTeamId,
                                aa.LstType,
                                aa.LstStageId,
                                aa.RstName,
                                aa.RstTeamName,
                                aa.LstCreatedById,
                                aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                aa.LstCreatedDateTime,
                                aa.LstLastUpdatedDateTime//,bb.LsrDescription
                            }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                        return Ok(payeedetail);


                    }

                    else
                    {
                        var childlist = db.LPayeeParents.Where(p => p.LppParentPayeeId == parentpayeeid).Select(p => p.LppPayeeId).ToList();
                        var arrayitem = "'";
                        for (int i = 0; i < childlist.Count; i++)
                        {
                            int value = childlist[i];
                            var tt = db.LPayees.Where(p => p.Id == value).Select(p => p.LpUserId).FirstOrDefault();
                            if (tt != null)
                            {
                                arrayitem += "','" + tt;
                            }
                        }

                        arrayitem = arrayitem.Trim().TrimStart(',');
                        arrayitem = arrayitem + "'" + ",'" + strUserId + "'";
                        arrayitem = arrayitem.Substring(3);
                        var payeedetail =
                             (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where
                                //aa.LstCreatedById.Equals(strUserId) &&                               
                                aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 2 && arrayitem.Contains(aa.LstCreatedById)
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  RstName = KO.RtsName,
                                  aa.RsupportTeam.RstTeamName,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).AsEnumerable()
                             .Select(aa => new {
                                 LsrDescription = GetSupportTicketResponses(aa.Id),
                                 FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                 aa.Id,
                                 aa.LstTicketNumber,
                                 aa.LstStatus,
                                 aa.LstPriority,
                                 aa.LstCategoryId,
                                 aa.RscName,
                                 aa.LstSummary,
                                 aa.LstTeamId,
                                 aa.LstType,
                                 aa.LstStageId,
                                 aa.RstName,
                                 aa.RstTeamName,
                                 aa.LstCreatedById,
                                 aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                 aa.LstCreatedDateTime,
                                 aa.LstLastUpdatedDateTime//,bb.LsrDescription
                             }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                        return Ok(payeedetail);
                    }

                }
                else
                { 
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                              // join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                          join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                          from CO in COJoin.DefaultIfEmpty()
                          join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                          from SO in SOjoin.DefaultIfEmpty()
                          join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                          from KO in KOJoin.DefaultIfEmpty()
                          where
                           (aa.LstCreatedById.Equals(strUserId)) &&
                           aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 2
                          select new
                          {
                              aa.Id,
                              aa.LstTicketNumber,
                              aa.LstStatus,
                              aa.LstPriority,
                              aa.LstCategoryId,
                              aa.RSupportCategory.RscName,
                              aa.LstSummary,
                              aa.LstTeamId,
                              aa.LstType,
                              aa.LstStageId,
                              RstName = KO.RtsName,
                              aa.RsupportTeam.RstTeamName,
                              LstCreatedById = SO.Email,
                              LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                              aa.LstCreatedDateTime,
                              aa.LstLastUpdatedDateTime
                              //,bb.LsrDescription
                          }).AsEnumerable()
                              .Select(aa => new {
                                  LsrDescription = GetSupportTicketResponses(aa.Id),
                                  FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  aa.RstTeamName,
                                  aa.LstCreatedById,
                                  aa.RstName,
                                  aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                //LstcurrentownerID = (u.LuFirstName+" "+u.LuLastName),aa.LstCreatedDateTime}).OrderBy(p => p.LstCreatedDateTime);
                // aa.LstCurrentOwnerId ,aa.LstCreatedDateTime}).OrderBy(p => p.LstCreatedDateTime);

                    return Ok(xx);
                }
            }
            else
            {
                if (L2TeamIdList.Contains(RoleId))//role is l2
                {
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                             // join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where
                               aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 2
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  RstName = KO.RtsName,
                                  aa.RsupportTeam.RstTeamName,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).AsEnumerable()
                              .Select(aa => new {
                                  LsrDescription = GetSupportTicketResponses(aa.Id),
                                  FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  aa.RstName,
                                  aa.RstTeamName,
                                  aa.LstCreatedById,
                                  aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                                }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                    //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                    return Ok(xx);
                }
                else
                {
                    //Match Portfolios with loggedin user
                    var EntityId = 0;
                    var LoggedInUser = db.LUsers.Where(p => p.LuUserId == strUserId).FirstOrDefault();
                    if (LoggedInUser == null)
                    {
                        var Payee = db.LPayees.Where(p => p.LpUserId == strUserId).FirstOrDefault();
                        if (Payee != null)
                            EntityId = Payee.Id;
                    }
                    else
                    {
                        EntityId = LoggedInUser.Id;
                    }
                    var LoggedInUserPortfolios = db.MEntityPortfolios.Where(p => p.MepRoleId == RoleId).Where(p => p.MepEntityId == EntityId).Select(p => p.MepPortfolioId).ToList();
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                              //join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join PO in db.MSupportTicketPortfolios on aa.Id equals PO.TicketId
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where
                              (aa.LstL2Id == strUserId || aa.LstL1Id == strUserId || LoggedInUserPortfolios.Contains(PO.PortfolioId)) &&
                               aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 2
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  aa.RsupportTeam.RstTeamName,
                                  RstName = KO.RtsName,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).GroupBy(p=>p.Id).Select(p=>p.FirstOrDefault()).AsEnumerable()
                              .Select(aa => new {
                                  LsrDescription = GetSupportTicketResponses(aa.Id),
                                  FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  aa.RstTeamName,
                                  aa.RstName,
                                  aa.LstCreatedById,
                                  aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                                  }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                    //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                    return Ok(xx);
                }
            }
        }

        public IHttpActionResult GetL2SupportTickets(string strUserId,string RoleId,int CompanyId)
        {
            var AllowedTeamName = "L2 SOS,L2 Infra,L2 Alteryx".Split(',');
            var L2TeamIdList = db.RsupportTeams.Where(p => AllowedTeamName.Contains(p.RstTeamName)).Select(p => p.RstRoleId).ToList();
            var RSupportTeamMember = db.RsupportTeams.Where(p => p.RstRoleId.Equals(RoleId)).FirstOrDefault();
            var yy = db.GCompanies.Where(c => c.GcCode == "99").FirstOrDefault().Id;
            string strOpCo = (from h in db.GCompanies
                              where h.Id == (CompanyId)
                              select h.GcCode).SingleOrDefault();
            /*NOTE: For Non-Support team member, show only those tickets that are created by me as a normal user*/
            if (RSupportTeamMember == null)
            {

                var payeeid = db.AspNetRoles.Where(a => a.Name == "Payee" && a.CompanyCode == strOpCo).Select(a => a.Id).FirstOrDefault();

                var role = db.AspNetUserRoles.Where(a => a.UserId == strUserId && a.RoleId == payeeid).Select(a => a.RoleId).FirstOrDefault();



                if (role != null)
                {
                    int parentpayeeid = db.LPayees.Where(a => a.LpUserId == strUserId).Select(a => a.Id).FirstOrDefault();
                    int parentpayeetableentry = db.LPayees.Where(a => a.LpUserId == strUserId).Select(a => a.Id).FirstOrDefault();
                    var haschildren = (from x in db.LPayeeParents where x.LppParentPayeeId == parentpayeetableentry select x.Id).FirstOrDefault();
                    if (haschildren == 0)
                    {
                        var payeedetail =
                            (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                                 // join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                             join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                             from CO in COJoin.DefaultIfEmpty()
                             join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                             from SO in SOjoin.DefaultIfEmpty()
                             join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                             from KO in KOJoin.DefaultIfEmpty()
                                 //condition for child payee
                             join l in db.LPayees on aa.LstCreatedById equals l.LpUserId
                             //join m in db.LPayeeParents on l.Id equals m.LppParentPayeeId
                             where
                              aa.LstCreatedById.Equals(strUserId) &&
                              aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 3 && l.Id == parentpayeeid
                             select new
                             {
                                 aa.Id,
                                 aa.LstTicketNumber,
                                 aa.LstStatus,
                                 aa.LstPriority,
                                 aa.LstCategoryId,
                                 aa.RSupportCategory.RscName,
                                 aa.LstSummary,
                                 aa.LstTeamId,
                                 aa.LstType,
                                 aa.LstStageId,
                                 RstName = KO.RtsName,
                                 aa.RsupportTeam.RstTeamName,
                                 LstCreatedById = SO.Email,
                                 LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                 aa.LstCreatedDateTime,
                                 aa.LstLastUpdatedDateTime//,bb.LsrDescription
                             }).AsEnumerable()
                            .Select(aa => new {
                                LsrDescription = GetSupportTicketResponses(aa.Id),
                                FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                aa.Id,
                                aa.LstTicketNumber,
                                aa.LstStatus,
                                aa.LstPriority,
                                aa.LstCategoryId,
                                aa.RscName,
                                aa.LstSummary,
                                aa.LstTeamId,
                                aa.LstType,
                                aa.LstStageId,
                                aa.RstName,
                                aa.RstTeamName,
                                aa.LstCreatedById,
                                aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                aa.LstCreatedDateTime,
                                aa.LstLastUpdatedDateTime//,bb.LsrDescription
                            }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                        return Ok(payeedetail);


                    }

                    else
                    {
                        var childlist = db.LPayeeParents.Where(p => p.LppParentPayeeId == parentpayeeid).Select(p => p.LppPayeeId).ToList();
                        var arrayitem = "'";
                        for (int i = 0; i < childlist.Count; i++)
                        {
                            int value = childlist[i];
                            var tt = db.LPayees.Where(p => p.Id == value).Select(p => p.LpUserId).FirstOrDefault();
                            if (tt != null)
                            {
                                arrayitem += "','" + tt;
                            }
                        }

                        arrayitem = arrayitem.Trim().TrimStart(',');
                        arrayitem = arrayitem + "'" + ",'" + strUserId + "'";
                        arrayitem = arrayitem.Substring(3);
                        var payeedetail =
                             (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where
                                //aa.LstCreatedById.Equals(strUserId) &&                               
                                aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 3 && arrayitem.Contains(aa.LstCreatedById)
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  RstName = KO.RtsName,
                                  aa.RsupportTeam.RstTeamName,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).AsEnumerable()
                             .Select(aa => new {
                                 LsrDescription = GetSupportTicketResponses(aa.Id),
                                 FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                 aa.Id,
                                 aa.LstTicketNumber,
                                 aa.LstStatus,
                                 aa.LstPriority,
                                 aa.LstCategoryId,
                                 aa.RscName,
                                 aa.LstSummary,
                                 aa.LstTeamId,
                                 aa.LstType,
                                 aa.LstStageId,
                                 aa.RstName,
                                 aa.RstTeamName,
                                 aa.LstCreatedById,
                                 aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                 aa.LstCreatedDateTime,
                                 aa.LstLastUpdatedDateTime//,bb.LsrDescription
                             }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                        return Ok(payeedetail);
                    }

                }


                else
                {
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser)
                                  //join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where
                               (aa.LstCreatedById.Equals(strUserId)) &&
                               aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 3 && aa.LstCompanyId == CompanyId
                              // && (aa.LstTeamId.HasValue)?L2TeamIdList.Contains(aa.LstTeamId.Value):1==0//check for team id only if it is populated otherwise do not show that record
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  aa.RsupportTeam.RstTeamName,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  RstName = KO.RtsName,
                                  aa.LstCreatedDateTime, aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).AsEnumerable()
                                  .Select(aa => new {
                                      LsrDescription = GetSupportTicketResponses(aa.Id),
                                      FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                      aa.Id,
                                      aa.LstTicketNumber,
                                      aa.LstStatus,
                                      aa.LstPriority,
                                      aa.LstCategoryId,
                                      aa.RscName,
                                      aa.RstName,
                                      aa.LstSummary,
                                      aa.LstTeamId,
                                      aa.LstType,
                                      aa.LstStageId,
                                      aa.RstTeamName,
                                      aa.LstCreatedById,
                                      aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                      aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                    //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                    //LstcurrentownerID = (u.LuFirstName+" "+u.LuLastName),aa.LstCreatedDateTime}).OrderBy(p => p.LstCreatedDateTime);
                    // aa.LstCurrentOwnerId ,aa.LstCreatedDateTime}).OrderBy(p => p.LstCreatedDateTime);

                    return Ok(xx);
                }
            }
            else
            {
                if (L2TeamIdList.Contains(RoleId))//Role belongs to L2
                {
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                             // join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where
                               aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 3
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  RstName = KO.RtsName,
                                  aa.RsupportTeam.RstTeamName,
                                  aa.RTicketStage.RtsName,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).AsEnumerable()
                              .Select(aa => new {
                                  LsrDescription = GetSupportTicketResponses(aa.Id),
                                  FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.RstName,
                                  aa.RstTeamName,
                                  aa.LstCreatedById,
                                  aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                    //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                    return Ok(xx);
                }
                else
                {
                    //Match Portfolios with loggedin user
                    var EntityId = 0;
                    var LoggedInUser = db.LUsers.Where(p => p.LuUserId == strUserId).FirstOrDefault();
                    if (LoggedInUser == null)
                    {
                        var Payee = db.LPayees.Where(p => p.LpUserId == strUserId).FirstOrDefault();
                        if (Payee != null)
                            EntityId = Payee.Id;
                    }
                    else
                    {
                        EntityId = LoggedInUser.Id;
                    }
                    var LoggedInUserPortfolios = db.MEntityPortfolios.Where(p => p.MepRoleId == RoleId).Where(p => p.MepEntityId == EntityId).Select(p => p.MepPortfolioId).ToList();
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                             // join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join PO in db.MSupportTicketPortfolios on aa.Id equals PO.TicketId
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where
                              (aa.LstL2Id == strUserId || aa.LstL1Id == strUserId || LoggedInUserPortfolios.Contains(PO.PortfolioId)) &&
                               aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 3
                              //  && (aa.LstTeamId.HasValue) ? L2TeamIdList.Contains(aa.LstTeamId.Value) : 1 == 0//check for team id only if it is populated otherwise do not show that record
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  RstName = KO.RtsName,
                                  aa.LstStageId,
                                  aa.RsupportTeam.RstTeamName,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).GroupBy(p=>p.Id).Select(p=>p.FirstOrDefault()).AsEnumerable()
                              .Select(aa => new {
                                  LsrDescription = GetSupportTicketResponses(aa.Id),
                                  FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.RstName,
                                  aa.LstStageId,
                                  aa.RstTeamName,
                                  aa.LstCreatedById,
                                  aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                  aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                    //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                    return Ok(xx);
                }
            }
        }

        //public IHttpActionResult GetAllTickets(string strUserId,int CompanyId)
        //{
        //    var yy = db.GCompanies.Where(c=>c.GcCode=="99").FirstOrDefault().Id;
        //        var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c=>c.LstCompanyId==CompanyId || c.LstCompanyId==yy)
        //                  join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
        //                  from CO in COJoin.DefaultIfEmpty()
        //                  join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
        //                  from SO in SOjoin.DefaultIfEmpty()
        //                  where !aa.LstCreatedById.Equals(strUserId) && !aa.LstL1Id.Equals(strUserId) && !aa.LstL2Id.Equals(strUserId) && !aa.LstL3Id.Equals(strUserId)
        //              select new
        //              {
        //                  aa.Id,
        //                  aa.LstTicketNumber,
        //                  aa.LstStatus,
        //                  aa.LstPriority,
        //                  aa.LstCategoryId,
        //                  aa.RSupportCategory.RscName,
        //                  aa.LstSummary,
        //                  aa.LstTeamId,
        //                  aa.RsupportTeam.RstTeamName,
        //                  LstCreatedById=SO.Email,
        //                  LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
        //                  aa.LstCreatedDateTime
        //              }).OrderBy(p => p.LstCreatedDateTime);

        //    return Ok(xx);
        //}

        /*History tab: for non support (not L1 or L2) user: only show where requestor is the user 
         * L1: portfolio matching, requestor or current owner (within same OpCo) 
         * L2: show all within the date range(irrespective of opco, owner, creator)*/
        public IHttpActionResult GetTicketsForDateRange(DateTime dtFrom, DateTime dtTo,string searchstring, string RoleId,string strUserId, int CompanyId)
        {
            //For resolving below issue in history tab that it does not show the data created on same date
            var FromDate = dtFrom;
            var ToDate = dtTo.AddDays(1).AddMinutes(-1);
            var AllowedTeamName = "L2 SOS,L2 Infra,L2 Alteryx".Split(',');
            var L2TeamIdList = db.RsupportTeams.Where(p => AllowedTeamName.Contains(p.RstTeamName)).Select(p => p.RstRoleId).ToList();
            var RSupportTeamMember = db.RsupportTeams.Where(p => p.RstRoleId.Equals(RoleId)).FirstOrDefault();
            var yy = db.GCompanies.Where(c => c.GcCode == "99").FirstOrDefault().Id;
            string strOpCo = (from h in db.GCompanies
                              where h.Id == (CompanyId)
                              select h.GcCode).SingleOrDefault();           
            //Starts-------if searchstring does not contains value--------------------------
            if (string.IsNullOrEmpty(searchstring))
            {
                /*NOTE: For Non-Support team member, show only those tickets that are created by me as a normal user*/
                if (RSupportTeamMember == null)
                {
                    var payeeid = db.AspNetRoles.Where(a => a.Name == "Payee" && a.CompanyCode == strOpCo).Select(a => a.Id).FirstOrDefault();

                    var role = db.AspNetUserRoles.Where(a => a.UserId == strUserId && a.RoleId == payeeid).Select(a => a.RoleId).FirstOrDefault();
                    if (role != null)
                    {
                        int parentpayeeid = db.LPayees.Where(a => a.LpUserId == strUserId).Select(a => a.Id).FirstOrDefault();
                        int parentpayeetableentry = db.LPayees.Where(a => a.LpUserId == strUserId).Select(a => a.Id).FirstOrDefault();
                        var haschildren = (from x in db.LPayeeParents where x.LppParentPayeeId == parentpayeetableentry select x.Id).FirstOrDefault();
                        if (haschildren == 0)
                        {
                            var payeedetail =
                                (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                                     // join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                             join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                                 from CO in COJoin.DefaultIfEmpty()
                                 join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                                 from SO in SOjoin.DefaultIfEmpty()
                                 join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                                 from KO in KOJoin.DefaultIfEmpty()
                                     //condition for child payee
                             join l in db.LPayees on aa.LstCreatedById equals l.LpUserId
                             //join m in db.LPayeeParents on l.Id equals m.LppParentPayeeId
                             where
                             (aa.LstStatus == "Closed" || aa.LstStatus == "Withdraw" )&&
                                  aa.LstCreatedById.Equals(strUserId) && l.Id == parentpayeeid
                                 select new
                                 {
                                     aa.Id,
                                     aa.LstTicketNumber,
                                     aa.LstStatus,
                                     aa.LstPriority,
                                     aa.LstCategoryId,
                                     aa.RSupportCategory.RscName,
                                     aa.LstSummary,
                                     aa.LstTeamId,
                                     aa.LstType,
                                     aa.LstStageId,
                                     RstName = KO.RtsName,
                                     aa.RsupportTeam.RstTeamName,
                                     LstCreatedById = SO.Email,
                                     LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                 aa.LstCreatedDateTime,
                                     aa.LstLastUpdatedDateTime//,bb.LsrDescription
                             }).AsEnumerable()
                                .Select(aa => new {
                                    LsrDescription = GetSupportTicketResponses(aa.Id),
                                    FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                    aa.Id,
                                    aa.LstTicketNumber,
                                    aa.LstStatus,
                                    aa.LstPriority,
                                    aa.LstCategoryId,
                                    aa.RscName,
                                    aa.LstSummary,
                                    aa.LstTeamId,
                                    aa.LstType,
                                    aa.LstStageId,
                                    aa.RstName,
                                    aa.RstTeamName,
                                    aa.LstCreatedById,
                                    aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                aa.LstCreatedDateTime,
                                    aa.LstLastUpdatedDateTime//,bb.LsrDescription
                            }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                            //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                            return Ok(payeedetail);


                        }

                        else
                        {
                            var childlist = db.LPayeeParents.Where(p => p.LppParentPayeeId == parentpayeeid).Select(p => p.LppPayeeId).ToList();
                            var arrayitem = "'";
                            for (int i = 0; i < childlist.Count; i++)
                            {
                                int value = childlist[i];
                                var tt = db.LPayees.Where(p => p.Id == value).Select(p => p.LpUserId).FirstOrDefault();
                                if (tt != null)
                                {
                                    arrayitem += "','" + tt;
                                }
                            }

                            arrayitem = arrayitem.Trim().TrimStart(',');
                            arrayitem = arrayitem + "'" + ",'" + strUserId + "'";
                            arrayitem = arrayitem.Substring(3);
                            var payeedetail =
                                 (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                                  join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                                  from CO in COJoin.DefaultIfEmpty()
                                  join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                                  from SO in SOjoin.DefaultIfEmpty()
                                  join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                                  from KO in KOJoin.DefaultIfEmpty()
                                  where
                                   (aa.LstStatus == "Closed" || aa.LstStatus == "Withdraw") &&  arrayitem.Contains(aa.LstCreatedById)
                                  select new
                                  {
                                      aa.Id,
                                      aa.LstTicketNumber,
                                      aa.LstStatus,
                                      aa.LstPriority,
                                      aa.LstCategoryId,
                                      aa.RSupportCategory.RscName,
                                      aa.LstSummary,
                                      aa.LstTeamId,
                                      aa.LstType,
                                      aa.LstStageId,
                                      RstName = KO.RtsName,
                                      aa.RsupportTeam.RstTeamName,
                                      LstCreatedById = SO.Email,
                                      LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                  aa.LstCreatedDateTime,
                                      aa.LstLastUpdatedDateTime//,bb.LsrDescription
                              }).AsEnumerable()
                                 .Select(aa => new {
                                     LsrDescription = GetSupportTicketResponses(aa.Id),
                                     FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                     aa.Id,
                                     aa.LstTicketNumber,
                                     aa.LstStatus,
                                     aa.LstPriority,
                                     aa.LstCategoryId,
                                     aa.RscName,
                                     aa.LstSummary,
                                     aa.LstTeamId,
                                     aa.LstType,
                                     aa.LstStageId,
                                     aa.RstName,
                                     aa.RstTeamName,
                                     aa.LstCreatedById,
                                     aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                 aa.LstCreatedDateTime,
                                     aa.LstLastUpdatedDateTime//,bb.LsrDescription
                             }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                            //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                            return Ok(payeedetail);
                        }

                    }
                    else
                    {
                        var zz = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(aa => aa.LstCreatedDateTime >= FromDate && aa.LstCreatedDateTime <= ToDate).Where(aa => aa.LstStatus == "Closed" || aa.LstStatus == "Withdraw")
                                  join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                                  from CO in COJoin.DefaultIfEmpty()
                                  join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                                  from SO in SOjoin.DefaultIfEmpty()
                                  join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                                  from KO in KOJoin.DefaultIfEmpty()
                                  where
                                   ( aa.LstCompanyId == CompanyId)
                                  select new
                                  {
                                      aa.Id,
                                      aa.LstTicketNumber,
                                      aa.LstStatus,
                                      aa.LstPriority,
                                      aa.LstCategoryId,
                                      aa.RSupportCategory.RscName,
                                      aa.LstSummary,
                                      aa.LstTeamId,
                                      aa.LstType,
                                      aa.LstStageId,
                                      aa.RsupportTeam.RstTeamName,
                                      RstName = KO.RtsName,
                                      LstCreatedById = SO.Email,
                                      LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                      aa.LstCreatedDateTime,
                                      aa.LstLastUpdatedDateTime
                                  }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime);

                        return Ok(zz);
                    }
                   
                }
                else
                {
                    if (L2TeamIdList.Contains(RoleId))//role is l2
                    {
                        var L2Tickets = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(aa => aa.LstCreatedDateTime >= FromDate && aa.LstCreatedDateTime <= ToDate).Where(aa => aa.LstStatus == "Closed" || aa.LstStatus == "Withdraw")
                                         join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                                         from CO in COJoin.DefaultIfEmpty()
                                         join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                                         from SO in SOjoin.DefaultIfEmpty()
                                         join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                                         from KO in KOJoin.DefaultIfEmpty()
                                         
                                         select new
                                         {
                                             aa.Id,
                                             aa.LstTicketNumber,
                                             aa.LstStatus,
                                             aa.LstPriority,
                                             aa.LstCategoryId,
                                             aa.RSupportCategory.RscName,
                                             aa.LstSummary,
                                             aa.LstTeamId,
                                             aa.LstType,
                                             aa.LstStageId,
                                             aa.RsupportTeam.RstTeamName,
                                             RstName = KO.RtsName,
                                             LstCreatedById = SO.Email,
                                             LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                             aa.LstCreatedDateTime,
                                             aa.LstLastUpdatedDateTime
                                         }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime);
                        return Ok(L2Tickets);
                    }
                    else
                    {
                        //Match Portfolios with loggedin user
                        var EntityId = 0;
                        var LoggedInUser = db.LUsers.Where(p => p.LuUserId == strUserId).FirstOrDefault();
                        if (LoggedInUser == null)
                        {
                            var Payee = db.LPayees.Where(p => p.LpUserId == strUserId).FirstOrDefault();
                            if (Payee != null)
                                EntityId = Payee.Id;
                        }
                        else
                        {
                            EntityId = LoggedInUser.Id;
                        }
                        var LoggedInUserPortfolios = db.MEntityPortfolios.Where(p => p.MepRoleId == RoleId).Where(p => p.MepEntityId == EntityId).Select(p => p.MepPortfolioId).ToList();
                        var OtherRoles = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(aa => aa.LstCreatedDateTime >= FromDate && aa.LstCreatedDateTime <= ToDate).Where(aa => aa.LstStatus == "Closed" || aa.LstStatus == "Withdraw")
                                          join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                                          from CO in COJoin.DefaultIfEmpty()
                                          join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                                          from SO in SOjoin.DefaultIfEmpty()
                                          join PO in db.MSupportTicketPortfolios on aa.Id equals PO.TicketId
                                          join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                                          from KO in KOJoin.DefaultIfEmpty()
                                          where
                                  (aa.LstL2Id == strUserId || aa.LstL1Id == strUserId || LoggedInUserPortfolios.Contains(PO.PortfolioId))
                                          select new
                                          {
                                              aa.Id,
                                              aa.LstTicketNumber,
                                              aa.LstStatus,
                                              aa.LstPriority,
                                              aa.LstCategoryId,
                                              aa.RSupportCategory.RscName,
                                              aa.LstSummary,
                                              aa.LstTeamId,
                                              aa.LstType,
                                              aa.LstStageId,
                                              RstName = KO.RtsName,
                                              aa.RsupportTeam.RstTeamName,
                                              LstCreatedById = SO.Email,
                                              LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                              aa.LstCreatedDateTime,
                                              aa.LstLastUpdatedDateTime
                                          }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime);
                        return Ok(OtherRoles);
                    }
                }
            } //Ends-------if searchstring does not contains value--------------------------

            //Starts-------if searchstring contains value--------------------------
            else
            {
                /*NOTE: For Non-Support team member, show only those tickets that are created by me as a normal user*/
                if (RSupportTeamMember == null)
                {

                    var payeeid = db.AspNetRoles.Where(a => a.Name == "Payee" && a.CompanyCode == strOpCo).Select(a => a.Id).FirstOrDefault();

                    var role = db.AspNetUserRoles.Where(a => a.UserId == strUserId && a.RoleId == payeeid).Select(a => a.RoleId).FirstOrDefault();
                    if (role != null)
                    {
                        int parentpayeeid = db.LPayees.Where(a => a.LpUserId == strUserId).Select(a => a.Id).FirstOrDefault();
                        int parentpayeetableentry = db.LPayees.Where(a => a.LpUserId == strUserId).Select(a => a.Id).FirstOrDefault();
                        var haschildren = (from x in db.LPayeeParents where x.LppParentPayeeId == parentpayeetableentry select x.Id).FirstOrDefault();
                        if (haschildren == 0)
                        {
                            var payeedetail =
                                (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                                     // join bb in db.LSupportResponses on aa.Id equals bb.LsrSupportTicketId
                                 join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                                 from CO in COJoin.DefaultIfEmpty()
                                 join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                                 from SO in SOjoin.DefaultIfEmpty()
                                 join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                                 from KO in KOJoin.DefaultIfEmpty()
                                     //condition for child payee
                                 join l in db.LPayees on aa.LstCreatedById equals l.LpUserId
                                 join m in db.LSupportResponses on aa.Id equals m.LsrSupportTicketId
                                 where ((aa.LstTicketNumber.Contains(searchstring) || aa.LstSummary.Contains(searchstring) || m.LsrDescription.Contains(searchstring)) && (aa.LstCreatedById.Equals(strUserId)) && l.Id == parentpayeeid && (aa.LstStatus == "Closed" || aa.LstStatus == "Withdraw") )
                                 select new
                                 {
                                     aa.Id,
                                     aa.LstTicketNumber,
                                     aa.LstStatus,
                                     aa.LstPriority,
                                     aa.LstCategoryId,
                                     aa.RSupportCategory.RscName,
                                     aa.LstSummary,
                                     aa.LstTeamId,
                                     aa.LstType,
                                     aa.LstStageId,
                                     RstName = KO.RtsName,
                                     aa.RsupportTeam.RstTeamName,
                                     LstCreatedById = SO.Email,
                                     LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                     aa.LstCreatedDateTime,
                                     aa.LstLastUpdatedDateTime//,bb.LsrDescription
                                 }).AsEnumerable()
                                .Select(aa => new {
                                    LsrDescription = GetSupportTicketResponses(aa.Id),
                                    FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                    aa.Id,
                                    aa.LstTicketNumber,
                                    aa.LstStatus,
                                    aa.LstPriority,
                                    aa.LstCategoryId,
                                    aa.RscName,
                                    aa.LstSummary,
                                    aa.LstTeamId,
                                    aa.LstType,
                                    aa.LstStageId,
                                    aa.RstName,
                                    aa.RstTeamName,
                                    aa.LstCreatedById,
                                    aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                    aa.LstCreatedDateTime,
                                    aa.LstLastUpdatedDateTime//,bb.LsrDescription
                                }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                            //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                            return Ok(payeedetail);


                        }

                        else
                        {
                            var childlist = db.LPayeeParents.Where(p => p.LppParentPayeeId == parentpayeeid).Select(p => p.LppPayeeId).ToList();
                            var arrayitem = "'";
                            for (int i = 0; i < childlist.Count; i++)
                            {
                                int value = childlist[i];
                                var tt = db.LPayees.Where(p => p.Id == value).Select(p => p.LpUserId).FirstOrDefault();
                                if (tt != null)
                                {
                                    arrayitem += "','" + tt;
                                }
                            }

                            arrayitem = arrayitem.Trim().TrimStart(',');
                            arrayitem = arrayitem + "'" + ",'" + strUserId + "'";
                            arrayitem = arrayitem.Substring(3);
                            var payeedetail =
                                 (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(c => c.LstCompanyId == CompanyId || (CompanyId == 1 && c.LstCompanyId == c.LstCompanyId))
                                  join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                                  from CO in COJoin.DefaultIfEmpty()
                                  join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                                  from SO in SOjoin.DefaultIfEmpty()
                                  join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                                  from KO in KOJoin.DefaultIfEmpty()
                                  join l in db.LSupportResponses on aa.Id equals l.LsrSupportTicketId
                                  where ((aa.LstTicketNumber.Contains(searchstring) || aa.LstSummary.Contains(searchstring) || l.LsrDescription.Contains(searchstring)) && (aa.LstCreatedById.Equals(strUserId))  && arrayitem.Contains(aa.LstCreatedById) && (aa.LstStatus == "Closed" || aa.LstStatus == "Withdraw"))
                                  select new
                                  {
                                      aa.Id,
                                      aa.LstTicketNumber,
                                      aa.LstStatus,
                                      aa.LstPriority,
                                      aa.LstCategoryId,
                                      aa.RSupportCategory.RscName,
                                      aa.LstSummary,
                                      aa.LstTeamId,
                                      aa.LstType,
                                      aa.LstStageId,
                                      RstName = KO.RtsName,
                                      aa.RsupportTeam.RstTeamName,
                                      LstCreatedById = SO.Email,
                                      LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                      aa.LstCreatedDateTime,
                                      aa.LstLastUpdatedDateTime//,bb.LsrDescription
                                  }).AsEnumerable()
                                 .Select(aa => new {
                                     LsrDescription = GetSupportTicketResponses(aa.Id),
                                     FirstDescription = GetFirstResponseByTicketId(aa.Id),
                                     aa.Id,
                                     aa.LstTicketNumber,
                                     aa.LstStatus,
                                     aa.LstPriority,
                                     aa.LstCategoryId,
                                     aa.RscName,
                                     aa.LstSummary,
                                     aa.LstTeamId,
                                     aa.LstType,
                                     aa.LstStageId,
                                     aa.RstName,
                                     aa.RstTeamName,
                                     aa.LstCreatedById,
                                     aa.LstCurrentOwnerId,//aa.AspNetUser.Email,
                                     aa.LstCreatedDateTime,
                                     aa.LstLastUpdatedDateTime//,bb.LsrDescription
                                 }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                            //}).OrderBy(p => p.LstCreatedDateTime).ToList();
                            return Ok(payeedetail);
                        }

                    }
                    else {

                        var zz = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Include(U => U.AspNetUser).Where(aa => aa.LstCreatedDateTime >= FromDate && aa.LstCreatedDateTime <= ToDate).Where(aa => aa.LstStatus == "Closed" || aa.LstStatus == "Withdraw")
                                  join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                                  from CO in COJoin.DefaultIfEmpty()
                                  join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                                  from SO in SOjoin.DefaultIfEmpty()
                                  join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                                  from KO in KOJoin.DefaultIfEmpty()
                                  join l in db.LSupportResponses on aa.Id equals l.LsrSupportTicketId
                                  //where
                                  // (aa.LstCreatedById.Equals(strUserId))
                                  where ((aa.LstTicketNumber.Contains(searchstring) || aa.LstSummary.Contains(searchstring) || l.LsrDescription.Contains(searchstring)) && (aa.LstCreatedById.Equals(strUserId)))
                                  //searchstring condition added by RS on 1st march 2019
                                  select new
                                  {
                                      aa.Id,
                                      aa.LstTicketNumber,
                                      aa.LstStatus,
                                      aa.LstPriority,
                                      aa.LstCategoryId,
                                      aa.RSupportCategory.RscName,
                                      aa.LstSummary,
                                      aa.LstTeamId,
                                      aa.LstType,
                                      aa.LstStageId,
                                      aa.RsupportTeam.RstTeamName,
                                      RstName = KO.RtsName,
                                      LstCreatedById = SO.Email,
                                      LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                      aa.LstCreatedDateTime,
                                      aa.LstLastUpdatedDateTime
                                  }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime);

                        return Ok(zz);

                    }

                    
                }
                else
                {
                    if (L2TeamIdList.Contains(RoleId))//role is l2
                    {
                        var L2Tickets = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(aa => aa.LstCreatedDateTime >= FromDate && aa.LstCreatedDateTime <= ToDate).Where(aa => aa.LstStatus == "Closed" || aa.LstStatus == "Withdraw")
                                         join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                                         from CO in COJoin.DefaultIfEmpty()
                                         join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                                         from SO in SOjoin.DefaultIfEmpty()
                                         join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                                         from KO in KOJoin.DefaultIfEmpty()
                                         join l in db.LSupportResponses on aa.Id equals l.LsrSupportTicketId
                                         where ((aa.LstTicketNumber.Contains(searchstring) || aa.LstSummary.Contains(searchstring) || l.LsrDescription.Contains(searchstring)))//added by RS on 1st march 2019
                                         //where
                                         // aa.LstStatus != "Closed" && aa.LstStatus != "Withdraw" && aa.Ordinal == 2
                                         select new
                                         {
                                             aa.Id,
                                             aa.LstTicketNumber,
                                             aa.LstStatus,
                                             aa.LstPriority,
                                             aa.LstCategoryId,
                                             aa.RSupportCategory.RscName,
                                             aa.LstSummary,
                                             aa.LstTeamId,
                                             aa.LstType,
                                             aa.LstStageId,
                                             aa.RsupportTeam.RstTeamName,
                                             RstName = KO.RtsName,
                                             LstCreatedById = SO.Email,
                                             LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                             aa.LstCreatedDateTime,
                                             aa.LstLastUpdatedDateTime
                                         }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime);
                        return Ok(L2Tickets);
                    }
                    else
                    {
                        //Match Portfolios with loggedin user
                        var EntityId = 0;
                        var LoggedInUser = db.LUsers.Where(p => p.LuUserId == strUserId).FirstOrDefault();
                        if (LoggedInUser == null)
                        {
                            var Payee = db.LPayees.Where(p => p.LpUserId == strUserId).FirstOrDefault();
                            if (Payee != null)
                                EntityId = Payee.Id;
                        }
                        else
                        {
                            EntityId = LoggedInUser.Id;
                        }
                        var LoggedInUserPortfolios = db.MEntityPortfolios.Where(p => p.MepRoleId == RoleId).Where(p => p.MepEntityId == EntityId).Select(p => p.MepPortfolioId).ToList();
                        var OtherRoles = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(aa => aa.LstCreatedDateTime >= FromDate && aa.LstCreatedDateTime <= ToDate).Where(aa => aa.LstStatus == "Closed" || aa.LstStatus == "Withdraw")
                                          join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                                          from CO in COJoin.DefaultIfEmpty()
                                          join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                                          from SO in SOjoin.DefaultIfEmpty()
                                          join PO in db.MSupportTicketPortfolios on aa.Id equals PO.TicketId
                                          join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                                          from KO in KOJoin.DefaultIfEmpty()
                                          join l in db.LSupportResponses on aa.Id equals l.LsrSupportTicketId
                                          where ((aa.LstTicketNumber.Contains(searchstring) || aa.LstSummary.Contains(searchstring) || l.LsrDescription.Contains(searchstring)))//added by RS on 1st march 2019
                                         && (aa.LstL2Id == strUserId || aa.LstL1Id == strUserId || LoggedInUserPortfolios.Contains(PO.PortfolioId))
                                          select new
                                          {
                                              aa.Id,
                                              aa.LstTicketNumber,
                                              aa.LstStatus,
                                              aa.LstPriority,
                                              aa.LstCategoryId,
                                              aa.RSupportCategory.RscName,
                                              aa.LstSummary,
                                              aa.LstTeamId,
                                              aa.LstType,
                                              aa.LstStageId,
                                              RstName = KO.RtsName,
                                              aa.RsupportTeam.RstTeamName,
                                              LstCreatedById = SO.Email,
                                              LstCurrentOwnerId = CO.Email,//aa.AspNetUser.Email,
                                              aa.LstCreatedDateTime,
                                              aa.LstLastUpdatedDateTime
                                          }).OrderByDescending(p => p.LstLastUpdatedDateTime).ToList();
                        //}).OrderBy(p => p.LstCreatedDateTime);
                        return Ok(OtherRoles);
                    }
                }
            }
            //Starts-------if searchstring contains value--------------------------







            /*  var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(t => t.RsupportTeam).Include(U => U.AspNetUser).Where(aa => aa.LstCreatedDateTime >= FromDate && aa.LstCreatedDateTime <= ToDate).Where(aa => aa.LstStatus == "Closed" || aa.LstStatus == "Withdraw")
                        join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                        from CO in COJoin.DefaultIfEmpty()
                        join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                        from SO in SOjoin.DefaultIfEmpty()
                        select new
                        {
                            aa.Id,
                            aa.LstTicketNumber,
                            aa.LstStatus,
                            aa.LstPriority,
                            aa.LstCategoryId,
                            aa.RSupportCategory.RscName,
                            aa.LstSummary,
                            aa.LstTeamId,
                            aa.RsupportTeam.RstTeamName,
                            LstCreatedById = SO.Email,
                            LstCurrentOwnerId = CO.Email,
                            aa.LstCreatedDateTime
                        }).OrderBy(p => p.LstCreatedDateTime).ToList();

              return Ok(xx);*/
        }

        public IHttpActionResult GetSearchTickets(string LstTicketSearchString, string strUserID, string strUserRole)
        {
            var query = (from u in db.AspNetUsers.Include(c => c.GCompany)

                         where u.Id.Equals(strUserID)
                        
                         select new { u.GCompany.GcCode, u.GCompany.Id }).Single();

            string strcomp = query.GcCode;
            int iCompID = query.Id;
            if (strcomp == "99")
            {
                #region ForGroup
                if (strUserRole.Contains("L2") || strUserRole.Contains("L3"))
                {
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(U => U.AspNetUser1)
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where aa.LstTicketNumber.Contains(LstTicketSearchString) || aa.LstSummary.Contains(LstTicketSearchString)
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  RstName = KO.RtsName,
                                  aa.RsupportTeam.RstTeamName,
                                  //LstCurrentOwnerId = aa.AspNetUser1.Email,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,
                                  aa.LstCreatedDateTime
                              }).OrderBy(p => p.LstCreatedDateTime);
                    return Ok(xx);
                }
                else if (strUserRole.Contains("L1"))
                {
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(U => U.AspNetUser1)
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where aa.LstCompanyId.Equals(iCompID) && (aa.LstTicketNumber.Contains(LstTicketSearchString) || aa.LstSummary.Contains(LstTicketSearchString))
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.LstType,
                                  aa.LstStageId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  RstName = KO.RtsName,
                                  aa.RsupportTeam.RstTeamName,
                                  //LstCurrentOwnerId = aa.AspNetUser1.Email,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,
                                  aa.LstCreatedDateTime
                              }).OrderBy(p => p.LstCreatedDateTime);
                    return Ok(xx);
                }
                else
                {
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(U => U.AspNetUser1)
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where aa.LstCompanyId.Equals(iCompID) && (aa.LstCreatedById.Equals(strUserID) || aa.LstLastUpdatedById.Equals(strUserID))
                              && (aa.LstTicketNumber.Contains(LstTicketSearchString) || aa.LstSummary.Contains(LstTicketSearchString))
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  RstName = KO.RtsName,
                                  aa.LstType,
                                  aa.LstStageId,
                                  aa.RsupportTeam.RstTeamName,
                                  //LstCurrentOwnerId = aa.AspNetUser1.Email,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,
                                  aa.LstCreatedDateTime
                              }).OrderBy(p => p.LstCreatedDateTime);
                    return Ok(xx);
                }
                #endregion
            }
            else
            {
                #region OpCo Specific
                if (strUserRole.Contains("L1"))
                {
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(U => U.AspNetUser)

                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where aa.LstCompanyId.Equals(iCompID) && (aa.LstTicketNumber.Contains(LstTicketSearchString) || aa.LstSummary.Contains(LstTicketSearchString))
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.LstType,
                                  RstName = KO.RtsName,
                                  aa.LstStageId,
                                  aa.RsupportTeam.RstTeamName,
                                 // LstCurrentOwnerId = aa.AspNetUser.Email,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,
                                  aa.LstCreatedDateTime
                              }).OrderBy(p => p.LstCreatedDateTime);
                    return Ok(xx);
                }
                else
                {
                    var xx = (from aa in db.LSupportTickets.Include(c => c.RSupportCategory).Include(U => U.AspNetUser)
                              join i in db.AspNetUsers on aa.LstCurrentOwnerId equals i.Id into COJoin
                              from CO in COJoin.DefaultIfEmpty()
                              join j in db.AspNetUsers on aa.LstCreatedById equals j.Id into SOjoin
                              from SO in SOjoin.DefaultIfEmpty()
                              join k in db.RTicketStages on aa.LstStageId equals k.Id into KOJoin
                              from KO in KOJoin.DefaultIfEmpty()
                              where (aa.LstCreatedById.Equals(strUserID) || aa.LstCurrentOwnerId.Equals(strUserID)) && (aa.LstTicketNumber.Contains(LstTicketSearchString) || aa.LstSummary.Contains(LstTicketSearchString))
                              select new
                              {
                                  aa.Id,
                                  aa.LstTicketNumber,
                                  aa.LstStatus,
                                  aa.LstPriority,
                                  aa.LstCategoryId,
                                  aa.LstType,
                                  RstName = KO.RtsName,
                                  aa.LstStageId,
                                  aa.RSupportCategory.RscName,
                                  aa.LstSummary,
                                  aa.LstTeamId,
                                  aa.RsupportTeam.RstTeamName,
                                  //LstCurrentOwnerId = aa.AspNetUser.Email,
                                  LstCreatedById = SO.Email,
                                  LstCurrentOwnerId = CO.Email,
                                  aa.LstCreatedDateTime
                              }).OrderBy(p => p.LstCreatedDateTime);
                    return Ok(xx);
                }
                #endregion
            }

            //return Ok("");
        }

        //SSmade a function to be used by linq
        private string GetSupportTicketResponses(int id)
        {
            var xx = db.Database.SqlQuery<string>("select [dbo].[FnGetSupportDescription]({0})", id).FirstOrDefault();
            return (xx);
        }

        //Get First Response of any ticket
        private string GetFirstResponseByTicketId(int id)
        {
            var FirstResponse = db.LSupportResponses.Where(p => p.LsrSupportTicketId == id).Select(p => p.LsrDescription).FirstOrDefault();
            return FirstResponse;
        }

        //SS Need to change its logic as it is not showing comments for L2
        //public IHttpActionResult GetTicketResponses(int id)
        //{
        //    //var xx = (from aa in db.LSupportResponses
        //    //              //join u in db.LUsers on aa.LsrResponseById equals u.LuUserId
        //    //          join u in db.AspNetUsers on aa.LsrResponseById equals u.Id
        //    //          where aa.LsrSupportTicketId.Equals(id)
        //    //          select new { aa.Id, aa.LsrResponseDateTime, aa.LsrDescription, aa.LsrUploadedFileNames, aa.LsrTicketStatus, aa.LsrResponseById, u.Email, u.UserName }).OrderByDescending(p => p.LsrResponseDateTime);
        //    //SS Created a New Db which will return string of Description in a one go
        //    var xx = db.Database.SqlQuery<string>("select [dbo].[FnGetSupportDescription]({0})",id).FirstOrDefault();
        //    return Ok(xx);
        //}

        public IHttpActionResult GetAnalystUsers(string strOpCo)
        {


            var xx = (from aa in db.AspNetRoles
                      where aa.Name.Equals("System Analyst") && aa.CompanyCode.Equals(strOpCo)
                      select new { aa.Id });
            return Ok(xx);
        }
        public IHttpActionResult GetUserSupportLevel(string id)
        {
            var xx = (from aa in db.RsupportTeams
                      where aa.RstRoleId.Equals(id)
                      select new { aa.Id, aa.RstTeamName, aa.RstRoleId });

            return Ok(xx);
        }
        public IHttpActionResult GetLevel2SupportUsers(string strType)
        {
            var xx = (from aa in db.RsupportTeams
                      join r in db.AspNetRoles on aa.RstRoleId equals r.Id

                      where aa.RstTeamName.Equals("L2 " + strType)
                      select new { aa.RstRoleId });
            return Ok(xx);
        }
        public IHttpActionResult GetLevel3SupportUsers(string strType)
        {
            var xx = (from aa in db.RsupportTeams
                      join r in db.AspNetRoles on aa.RstRoleId equals r.Id

                      where aa.RstTeamName.Equals("L3 " + strType)
                      select new { aa.RstRoleId });
            return Ok(xx);
        }

        [ResponseType(typeof(LSupportTicketContextModel))]
        public IHttpActionResult GetTicketDetails(int id)
        {
            var LTkts = db.LSupportTickets.Where(p => p.Id == id).Include(l => l.AspNetUser).Select(x => new
            {
                x.Id,
                x.LstCategoryId,
                x.LstCC,
                x.LstCompanyId,
                x.LstCreatedById,
                x.LstCreatedDateTime,
                x.LstCreatedOnBehalfOfId,
                x.LstCurrentOwnerId,
                x.LstImpact,
                x.LstL1Id,
                x.LstL2Id,
                x.LstL3Id,
                x.LstLastUpdatedById,
                x.LstLastUpdatedDateTime,
                x.LstPhone,
                x.LstPriority,
                x.LstSeverity,
                x.LstStatus,
                x.LstSummary,
                x.LstTeamId,
                x.LstTicketNumber,
                x.LstType,
                x.LstStageId,
                x.Ordinal,
                LstLastUpdatedUserName = (x.AspNetUser.UserName)
            }).AsEnumerable()
            .Select(x=>
            new
            {
                LsrDescription=GetSupportTicketResponses(x.Id),
                x.Id,
                x.LstCategoryId,
                x.LstCC,
                x.LstCompanyId,
                x.LstCreatedById,
                x.LstCreatedDateTime,
                x.LstCreatedOnBehalfOfId,
                x.LstCurrentOwnerId,
                x.LstImpact,
                x.LstL1Id,
                x.LstL2Id,
                x.LstL3Id,
                x.LstLastUpdatedById,
                x.LstLastUpdatedDateTime,
                x.LstPhone,
                x.LstPriority,
                x.LstSeverity,
                x.LstStatus,
                x.LstSummary,
                x.LstTeamId,
                x.LstTicketNumber,
                x.LstType,
                x.LstStageId,
                x.Ordinal,
                x.LstLastUpdatedUserName
            }).FirstOrDefault();
            if (LTkts == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "SUPPORT TICKETS")));
            }
            return Ok(LTkts);
        }
        private bool TicketExists(int id)
        {
            return db.LSupportTickets.Count(e => e.Id == id) > 0;
        }
        public IHttpActionResult GetTeamIds()
        {
            var xx = (from aa in db.RsupportTeams
                      select new {
                          aa.Id,
                          aa.RstTeamName,
                          aa.RstRoleId,
                          aa.RstEmailID
                      });
            return Ok(xx);
        }

        public IHttpActionResult GetL2TeamIds()
        {
            var xx = (from aa in db.RsupportTeams
                      where aa.RstTeamName.Contains("L2")
                      select new
                      {
                          aa.Id,
                          aa.RstTeamName,
                          aa.RstRoleId,
                          aa.RstEmailID
                      });
            return Ok(xx);
        }

        public IHttpActionResult  GetTeamName(string RoleId)
        {
            var xx = (from aa in db.RsupportTeams
                      where aa.RstRoleId.Equals(RoleId) 
                       select new
                      {
                          
                          aa.RstTeamName
                          
                      }).FirstOrDefault();
            return Ok(xx);
        }

        public IHttpActionResult GetTeamIdByName(string TeamName)
        {
            var xx = (from aa in db.RsupportTeams
                      where aa.RstTeamName.Equals(TeamName)
                      select new
                      {

                          aa.Id

                      }).FirstOrDefault();
            return Ok(xx);
        }

        #region Methods for the demo of sql injection
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> AllowInjection(int id, string strPhone, string strSummary)
        //{
        //    db.Database.ExecuteSqlCommand("update LSupportTickets set LstPhone = '"+ strPhone +"' , LstSummary = '" + strSummary +"'  where id =" + id + "");
        //    return StatusCode(HttpStatusCode.NoContent);
        //}
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> RestrictInjection(int id, string strPhone, string strSummary)
        //{
        //    db.Database.ExecuteSqlCommand("update LSupportTickets set LstPhone = {0}, LstSummary = {1}  where id ={2} ", strPhone, strSummary, id);
        //    return StatusCode(HttpStatusCode.NoContent);
        //}
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> TryLinQ(int id, string strPhone, string strSummary)
        //{
        //    try
        //    {
        //        LSupportTicket Lst = new Models.LSupportTicket();
        //        Lst.Id = id;
        //        Lst.LstPhone = strPhone;
        //        Lst.LstSummary = strSummary;

        //        db.Entry(Lst).State = EntityState.Modified;
        //        //db.SaveChanges();
        //        db.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return StatusCode(HttpStatusCode.NoContent);
        //}
        #endregion

        public DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {
            DataTable dt = new DataTable();


            PropertyInfo[] columns = null;

            if (Linqlist == null) return dt;

            foreach (T Record in Linqlist)
            {

                if (columns == null)
                {
                    columns = ((Type)Record.GetType()).GetProperties();
                    foreach (PropertyInfo GetProperty in columns)
                    {
                        Type colType = GetProperty.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dt.Columns.Add(new DataColumn(GetProperty.Name, colType));
                    }
                }

                DataRow dr = dt.NewRow();

                foreach (PropertyInfo pinfo in columns)
                {
                    dr[pinfo.Name] = pinfo.GetValue(Record, null) == null ? DBNull.Value : pinfo.GetValue
                    (Record, null);
                }

                dt.Rows.Add(dr);
            }
            return dt;
        }

        [HttpGet]
        public IHttpActionResult GetSupportTicketDetailForDashBoard(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            string Qry = string.Empty;
            if (FilterQuery == null)
            {
                Qry = "SELECT * FROM(SELECT gc.GcCode, LstTicketNumber,LstSummary,LstCreatedDateTime,LstCreatedById,LstCurrentOwnerId,LstStatus, ROW_NUMBER() OVER(ORDER BY LstCreatedDateTime) AS RowNum FROM LSupportTickets inner join GCompanies gc on gc.Id = LstCompanyId WHERE LstStatus != 'closed' and LstCreatedDateTime>= dateadd(dd, -90, getdate())) AS RowConstrainedResult WHERE RowNum > @P1 AND RowNum <= @P2 ORDER BY LstCreatedDateTime desc";// Order by @P3";

            }
            else
            {
               // Qry = "SELECT * FROM(SELECT gc.GcCode, LebRecipientList, LebSubject, LebEmailType, LebCreatedDateTime, LebCreatedById, LebStatus, LebUpdatedDateTime, ROW_NUMBER() OVER(ORDER BY LebCreatedDateTime) AS RowNum FROM LEmailBucket inner join AspNetUsers ap on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId WHERE LebCreatedDateTime >= dateadd(dd, -90, getdate())) AS RowConstrainedResult WHERE RowNum > @P1 AND RowNum <= @P2" + FilterQuery + "ORDER BY LebCreatedDateTime desc";// Order by @P3";
                Qry = "SELECT * FROM(SELECT gc.GcCode, LstTicketNumber,LstSummary,LstCreatedDateTime,LstCreatedById,LstCurrentOwnerId,LstStatus, ROW_NUMBER() OVER(ORDER BY LstCreatedDateTime) AS RowNum FROM LSupportTickets inner join GCompanies gc on gc.Id = LstCompanyId WHERE LstStatus != 'closed' and LstCreatedDateTime>= dateadd(dd, -90, getdate())) AS RowConstrainedResult WHERE RowNum > @P1 AND RowNum <= @P2" + FilterQuery + "ORDER BY LstCreatedDateTime desc";
            }

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            //parameterList.Add(new SqlParameter("@P3", qq));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<LSupportTicketContextModel>(Qry, parameters).ToList();

            return Ok(xx);
            //string Qry = "SELECT gc.GcCode,LstTicketNumber,LstSummary,LstCreatedDateTime,LstCreatedById,LstCurrentOwnerId,LstStatus FROM LSupportTickets inner join GCompanies gc on gc.Id = LstCompanyId WHERE LstStatus != 'closed' and LstCreatedDateTime>= dateadd(dd, -90, getdate()) group by gc.GcCode, LstTicketNumber,LstSummary,LstCreatedDateTime,LstCreatedById,LstCurrentOwnerId,LstStatus";
            //var xx = db.Database.SqlQuery<LSupportTicketContextModel>(Qry).ToList();
            //return Ok(xx);
        }

        [HttpGet]
        public IHttpActionResult GetSupportTicketDetailCounts()
        {
            string Qry = "SELECT gc.GcCode,LstTicketNumber,LstSummary,LstCreatedDateTime,LstCreatedById,LstCurrentOwnerId,LstStatus FROM LSupportTickets inner join GCompanies gc on gc.Id = LstCompanyId WHERE LstStatus != 'closed' and LstCreatedDateTime>= dateadd(dd, -90, getdate()) group by gc.GcCode, LstTicketNumber,LstSummary,LstCreatedDateTime,LstCreatedById,LstCurrentOwnerId,LstStatus";
            var xx = db.Database.SqlQuery<LSupportTicketContextModel>(Qry).Count();
            return Ok(xx);
        }
        [HttpGet]
        public IHttpActionResult GetClosedTicketsData()
        {
            var today =  DateTime.UtcNow;
            var month = new DateTime(today.Year, today.Month, 1);
            var Displaymonth = new DateTime(today.Year, today.Month, 1);
            var GridDisplayMonth = Displaymonth.AddMonths(-1);
            var first = month.AddMonths(-1);
            var last = month.AddDays(-1);
            string monthStr = month.ToString("MM");
            string monthStrDisplay = GridDisplayMonth.ToString("MM");
            string Qry = "select @monthStrDisplay as Month,C.GcCode as OpCo, T.LstPriority as [Priority], count(T.Id) as [Count] "
                        + " from LSupportTickets T inner join GCompanies C on T.LstCompanyId = C.Id"
                        + " where LstType = 'SOS'  and LstStatus in ('Closed', 'Withdraw') " +
                        " and Dateadd(dd, datediff(dd, 0, T.LstLastUpdatedDateTime),0) between @StartDate and @EndDate"
                        + " group by C.GcCode, T.LstPriority order by 1,2,3";
            var tb = new DataTable();
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Qry, conn);
            cmd.Parameters.AddWithValue("month", monthStr);
            cmd.Parameters.AddWithValue("monthStrDisplay", monthStrDisplay);
            cmd.Parameters.AddWithValue("StartDate", first.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("EndDate", last.ToString("yyyy-MM-dd"));
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            return Ok(tb); 
        }

        //[HttpGet]
        //public IHttpActionResult GetClosedTicketsCount()
        //{
        //    var today = DateTime.UtcNow;
        //    var month = new DateTime(today.Year, today.Month, 1);
        //    var first = month.AddMonths(-1);
        //    var last = month.AddDays(-1);
        //    string Qry = "select Count(*) "
        //                + " from LSupportTickets T inner join GCompanies C on T.LstCompanyId = C.Id"
        //                + " where LstType = 'SOS'  and LstStatus in ('Closed', 'Withdraw') " +
        //                " and Dateadd(dd, datediff(dd, 0, T.LstLastUpdatedDateTime),0) between {0} and {1}"
        //                + "  ";
        //    int cnt = db.Database.SqlQuery<int>(Qry, first.ToString("yyyy-MM-dd"), last.ToString("yyyy-MM-dd")).FirstOrDefault();
        //    return Ok(cnt);
        //}

        //public IHttpActionResult GetSupportTicketSummaryCounts()
        //{
        //    //string Qry = "SELECT g.Id,g.GcCode, a.Requester, b.L1 ,c.L2,d.Closed FROM GCompanies g LEFT JOIN " +
        //    //             "(SELECT  LstCompanyId, count(*) as Requester FROM LSupportTickets where Ordinal = 1 and LstStatus not in ('closed', 'withdraw') " +
        //    //              "Group by LstCompanyId)  a on g.Id = a.LstCompanyId left join " +
        //    //              "(SELECT LstCompanyId, count(*) as L1 FROM LSupportTickets where Ordinal = 2 and LstStatus not in('closed', 'withdraw') " +
        //    //              "Group by LstCompanyId)  b on g.Id = b.LstCompanyId left join " +
        //    //              "(SELECT LstCompanyId, count(*) as L2 FROM LSupportTickets where Ordinal = 3 and LstStatus not in('closed', 'withdraw') " +
        //    //              "Group by LstCompanyId)  c on g.Id = c.LstCompanyId left join " +
        //    //              "(SELECT LstCompanyId, count(*) as Closed FROM LSupportTickets where LstStatus = 'closed' " +
        //    //              "Group by LstCompanyId)  d on g.Id = d.LstCompanyId " +
        //    //              "left join LSupportTickets st on st.LstCompanyId = g.Id " +
        //    //              "where a.Requester is not null and b.L1 is not null and c.L2 is not null and d.closed is not null " +
        //    //              "AND st.LstCreatedDateTime >= dateadd(dd, -90, getdate())group by g.Id,g.GcCode, a.Requester, b.L1 ,c.L2,d.closed ";

        //    string Qry = "SELECT g.Id,g.GcCode, Convert(varchar, ISNULL(a.Requester, 0)) as Requester, Convert(varchar, ISNULL(b.L1, 0)) as L1 ," +
        //           " Convert(varchar, ISNULL(c.L2, 0)) as L2, Convert(varchar, ISNULL(d.Closed, 0)) as Closed FROM GCompanies g LEFT JOIN " +
        //           " (SELECT  LstCompanyId, Convert(varchar, ISNULL(count(*), 0)) as Requester FROM LSupportTickets where Ordinal = 1  and LstStatus not in ('closed', 'withdraw') " +
        //           " Group by LstCompanyId)  a on g.Id = a.LstCompanyId and g.GcCode != '99' left join " +
        //          " (SELECT LstCompanyId, Convert(varchar, ISNULL(count(*), 0 )) as L1 FROM LSupportTickets where Ordinal = 2 and LstStatus not in('closed', 'withdraw') " +
        //           " Group by LstCompanyId)  b on g.Id = b.LstCompanyId and g.GcCode != '99' left join " +
        //           " (SELECT LstCompanyId, Convert(varchar, ISNULL(count(*), 0 )) as L2 FROM LSupportTickets where Ordinal = 3 and LstStatus not in('closed', 'withdraw') " +
        //           " Group by LstCompanyId)  c on g.Id = c.LstCompanyId and g.GcCode != '99' left join " +
        //           " (SELECT LstCompanyId, Convert(varchar, ISNULL(count(*), 0 )) as Closed FROM LSupportTickets where LstStatus = 'closed' " +
        //           " Group by LstCompanyId)  d on g.Id = d.LstCompanyId left join LSupportTickets st on st.LstCompanyId = g.Id and g.GcCode != '99' " +
        //           " where a.Requester is not null OR b.L1 is not null OR c.L2 is not null OR d.closed is not null and st.LstCreatedDateTime >= dateadd(dd, -90, getdate()) " +
        //           " group by g.Id,g.GcCode, a.Requester, b.L1 ,c.L2,d.closed";

        //    var xx = db.Database.SqlQuery<LSupportTicketContextModel>(Qry).Count();
        //    return Ok(xx);
        //}


        public async Task<IHttpActionResult> GetSupportTicketSummaryCounts()
        {
            var tb = new DataTable();

            string Query = "Exec [spGetSupportTicketItemsCounts]";

            ////using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);

            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
           
            sda.Fill(tb);
            var count = tb.Rows.Count;
          
            conn.Close();
            // return Ok(ds);
            return Ok(count);
        }
        //public IHttpActionResult GetSupportTicketSummaryForDashBoard(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        //{
        //    var SortQuery = "";
        //    if (!string.IsNullOrEmpty(sortdatafield))
        //    {
        //        SortQuery = " order by " + sortdatafield + " " + sortorder;
        //    }
        //    else
        //    {
        //        SortQuery = " ORDER BY Id desc";
        //    }

        //    string Qry = string.Empty;           
        //    if (FilterQuery == null)
        //    {
        //        //Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(SELECT g.Id, g.GcCode, a.Requester, b.L1, c.L2, d.Closed FROM GCompanies g LEFT JOIN " +
        //        //      "(SELECT  LstCompanyId, count(*) as Requester FROM LSupportTickets where Ordinal = 1 and LstStatus not in ('closed', 'withdraw') " +
        //        //      "Group by LstCompanyId)  a on g.Id = a.LstCompanyId left join " +
        //        //      "(SELECT LstCompanyId, count(*) as L1 FROM LSupportTickets where Ordinal = 2 and LstStatus not in ('closed', 'withdraw') " +
        //        //      "Group by LstCompanyId)  b on g.Id = b.LstCompanyId left join " +
        //        //      "(SELECT LstCompanyId, count(*) as L2 FROM LSupportTickets where Ordinal = 3 and LstStatus not in ('closed', 'withdraw') " +
        //        //      "Group by LstCompanyId)  c on g.Id = c.LstCompanyId left join " +
        //        //      "(SELECT LstCompanyId, count(*) as Closed FROM LSupportTickets where LstStatus = 'closed' " +
        //        //      "Group by LstCompanyId)  d on g.Id = d.LstCompanyId " +
        //        //      "left join LSupportTickets st on st.LstCompanyId = g.Id " +
        //        //      "where a.Requester is not null and b.L1 is not null and c.L2 is not null and d.closed is not null " +
        //        //      "AND st.LstCreatedDateTime >= dateadd(dd, -90, getdate())group by g.Id, g.GcCode, a.Requester, b.L1, c.L2, d.closed)A " +
        //        //      ") B WHERE B.datacount > @P1 AND B.datacount <= @P2 ";

        //        Qry= "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(SELECT g.Id,g.GcCode, Convert(varchar, ISNULL(a.Requester, 0)) as Requester, Convert(varchar, ISNULL(b.L1, 0)) as L1 ," + 
        //            " Convert(varchar, ISNULL(c.L2, 0)) as L2, Convert(varchar, ISNULL(d.Closed, 0)) as Closed FROM GCompanies g LEFT JOIN "+
        //            " (SELECT  LstCompanyId, Convert(varchar, ISNULL(count(*), 0)) as Requester FROM LSupportTickets where Ordinal = 1  and LstStatus not in ('closed', 'withdraw') "+
        //            " Group by LstCompanyId)  a on g.Id = a.LstCompanyId and g.GcCode != '99' left join "+
        //           " (SELECT LstCompanyId, Convert(varchar, ISNULL(count(*), 0 )) as L1 FROM LSupportTickets where Ordinal = 2 and LstStatus not in('closed', 'withdraw') "+
        //            " Group by LstCompanyId)  b on g.Id = b.LstCompanyId and g.GcCode != '99' left join "+
        //            " (SELECT LstCompanyId, Convert(varchar, ISNULL(count(*), 0 )) as L2 FROM LSupportTickets where Ordinal = 3 and LstStatus not in('closed', 'withdraw') "+
        //            " Group by LstCompanyId)  c on g.Id = c.LstCompanyId and g.GcCode != '99' left join "+
        //            " (SELECT LstCompanyId, Convert(varchar, ISNULL(count(*), 0 )) as Closed FROM LSupportTickets where LstStatus = 'closed' "+
        //            " Group by LstCompanyId)  d on g.Id = d.LstCompanyId left join LSupportTickets st on st.LstCompanyId = g.Id and g.GcCode != '99' "+
        //            " where a.Requester is not null OR b.L1 is not null OR c.L2 is not null OR d.closed is not null and st.LstCreatedDateTime >= dateadd(dd, -90, getdate()) "+
        //            " group by g.Id,g.GcCode, a.Requester, b.L1 ,c.L2,d.closed)A" +
        //            ") B WHERE B.datacount > @P1 AND B.datacount <= @P2 ";
        //    }
        //    else
        //    {
        //        FilterQuery = "WHERE 1=1 " + FilterQuery;


        //        Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(SELECT g.Id,g.GcCode, Convert(varchar, ISNULL(a.Requester, 0)) as Requester, Convert(varchar, ISNULL(b.L1, 0)) as L1 ," +
        //           " Convert(varchar, ISNULL(c.L2, 0)) as L2, Convert(varchar, ISNULL(d.Closed, 0)) as Closed FROM GCompanies g LEFT JOIN " +
        //           " (SELECT  LstCompanyId, Convert(varchar, ISNULL(count(*), 0)) as Requester FROM LSupportTickets where Ordinal = 1  and LstStatus not in ('closed', 'withdraw') " +
        //           " Group by LstCompanyId)  a on g.Id = a.LstCompanyId and g.GcCode != '99' left join " +
        //          " (SELECT LstCompanyId, Convert(varchar, ISNULL(count(*), 0 )) as L1 FROM LSupportTickets where Ordinal = 2 and LstStatus not in('closed', 'withdraw') " +
        //           " Group by LstCompanyId)  b on g.Id = b.LstCompanyId and g.GcCode != '99' left join " +
        //           " (SELECT LstCompanyId, Convert(varchar, ISNULL(count(*), 0 )) as L2 FROM LSupportTickets where Ordinal = 3 and LstStatus not in('closed', 'withdraw') " +
        //           " Group by LstCompanyId)  c on g.Id = c.LstCompanyId and g.GcCode != '99' left join " +
        //           " (SELECT LstCompanyId, Convert(varchar, ISNULL(count(*), 0 )) as Closed FROM LSupportTickets where LstStatus = 'closed' " +
        //           " Group by LstCompanyId)  d on g.Id = d.LstCompanyId left join LSupportTickets st on st.LstCompanyId = g.Id and g.GcCode != '99' " +
        //           " where a.Requester is not null OR b.L1 is not null OR c.L2 is not null OR d.closed is not null and st.LstCreatedDateTime >= dateadd(dd, -90, getdate()) " +
        //           " group by g.Id,g.GcCode, a.Requester, b.L1 ,c.L2,d.closed)A" +
        //             FilterQuery + ") B WHERE B.datacount > @P1 AND B.datacount <= @P2 ";

        //        //Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(SELECT g.Id, g.GcCode, a.Requester, b.L1, c.L2, d.Closed FROM GCompanies g LEFT JOIN " +
        //        //       "(SELECT  LstCompanyId, count(*) as Requester FROM LSupportTickets where Ordinal = 1 and LstStatus not in ('closed', 'withdraw') " +
        //        //       "Group by LstCompanyId)  a on g.Id = a.LstCompanyId left join " +
        //        //       "(SELECT LstCompanyId, count(*) as L1 FROM LSupportTickets where Ordinal = 2 and LstStatus not in ('closed', 'withdraw') " +
        //        //       "Group by LstCompanyId)  b on g.Id = b.LstCompanyId left join " +
        //        //       "(SELECT LstCompanyId, count(*) as L2 FROM LSupportTickets where Ordinal = 3 and LstStatus not in ('closed', 'withdraw') " +
        //        //       "Group by LstCompanyId)  c on g.Id = c.LstCompanyId left join " +
        //        //       "(SELECT LstCompanyId, count(*) as Closed FROM LSupportTickets where LstStatus = 'closed' " +
        //        //       "Group by LstCompanyId)  d on g.Id = d.LstCompanyId " +
        //        //       "left join LSupportTickets st on st.LstCompanyId = g.Id " +
        //        //       "where a.Requester is not null and b.L1 is not null and c.L2 is not null and d.closed is not null " +
        //        //       "AND st.LstCreatedDateTime >= dateadd(dd, -90, getdate())group by g.Id, g.GcCode, a.Requester, b.L1, c.L2, d.closed)A " +
        //        //        FilterQuery + ") B WHERE B.datacount > @P1 AND B.datacount <= @P2";


        //    }

        //    List<SqlParameter> parameterList = new List<SqlParameter>();
        //    parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
        //    parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
        //    //parameterList.Add(new SqlParameter("@P3", qq));
        //    SqlParameter[] parameters = parameterList.ToArray();
        //    var xx = db.Database.SqlQuery<LSupportTicketContextModel>(Qry, parameters).ToList();
        //    return Ok(xx);

        //}


[HttpGet]
        public async Task<IHttpActionResult> GetSupportTicketSummaryForDashBoard(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {

            DataTable dt = new DataTable();
            string Query = "Exec [spGetSupportTicketItems] @pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery";
            ////using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
           
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            dt.Columns.Remove(dt.Columns["datacount"]);
            conn.Close();         
            return Ok(dt);
        }




        public IHttpActionResult GetSupportTicketChart()
        {
            //string Qry = "SELECT g.Id,g.GcCode, a.Requester, b.L1 ,c.L2,d.Closed FROM GCompanies g LEFT JOIN "+
            //           "(SELECT  LstCompanyId, count(*) as Requester FROM LSupportTickets where Ordinal = 1 and LstStatus not in ('closed', 'withdraw') " +
            //            "Group by LstCompanyId)  a on g.Id = a.LstCompanyId left join " +
            //            "(SELECT LstCompanyId, count(*) as L1 FROM LSupportTickets where Ordinal = 2 and LstStatus not in('closed', 'withdraw') " +
            //            "Group by LstCompanyId)  b on g.Id = b.LstCompanyId left join " +
            //            "(SELECT LstCompanyId, count(*) as L2 FROM LSupportTickets where Ordinal = 3 and LstStatus not in('closed', 'withdraw') " +
            //            "Group by LstCompanyId)  c on g.Id = c.LstCompanyId left join " +
            //            "(SELECT LstCompanyId, count(*) as Closed FROM LSupportTickets where LstStatus = 'closed' " +
            //            "Group by LstCompanyId)  d on g.Id = d.LstCompanyId " +
            //            "left join LSupportTickets st on st.LstCompanyId = g.Id " +
            //            "where a.Requester is not null and b.L1 is not null and c.L2 is not null and d.closed is not null " +
            //            "AND st.LstCreatedDateTime >= dateadd(dd, -90, getdate())group by g.Id,g.GcCode, a.Requester, b.L1 ,c.L2,d.closed ";


            //var xx = db.Database.SqlQuery<LSupportTicketContextModel>(Qry).ToList();
            //return Ok(xx);

            var tb = new DataTable();

            string Query = "Exec [spGetSupportTicketItemsCounts]";

            ////using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);

            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);

            sda.Fill(tb);
            var count = tb.Rows.Count;

            conn.Close();
            // return Ok(ds);
            return Ok(tb);

        }

        //int GetL2SOSTicketcounts()
        public IHttpActionResult GetL2SOSTicketcounts()
        {

            var xx = db.LSupportTickets.Count(a => a.Ordinal == 3 && a.LstStatus != "closed" && a.LstStatus != "withdraw" && a.LstType=="SOS");
            return Ok(xx);
        }
        //GetL2ALTTicketcounts
        public IHttpActionResult GetL2ALTTicketcounts()
        {
            var xx = db.LSupportTickets.Count(a => a.Ordinal == 3 && a.LstStatus != "closed" && a.LstStatus != "withdraw" && a.LstType == "ALTERYX");
            return Ok(xx);
        }

        [HttpGet]
        public IHttpActionResult UpdateAttachmentTicket(int id, string FileName, string FilePath, string CreatedBy, string Type)
        {
            string ReturnString = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(FileName))
                {
                    var FilesArray = FileName.Split(',').ToList();
                    foreach (var file in FilesArray)
                    {
                        var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = CreatedBy, LsdUpdatedById = CreatedBy, LsdFileName = file, LsdFilePath = FilePath, LsdEntityType = Type, LsdEntityId = id, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedDateTime = DateTime.UtcNow };
                        db.LSupportingDocuments.Add(LSupportingDocuments);
                        db.SaveChanges();
                        if (ReturnString == "")
                        {
                            ReturnString = file + ':' + LSupportingDocuments.Id;
                        }
                        else
                        {
                            ReturnString = ReturnString + ',' + file + ':' + LSupportingDocuments.Id;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
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
            //return StatusCode(HttpStatusCode.NoContent);
            return Ok(ReturnString);
        }
    }
}