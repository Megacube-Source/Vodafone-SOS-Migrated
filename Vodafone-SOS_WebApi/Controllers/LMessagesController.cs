using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;


namespace Vodafone_SOS_WebApi.Controllers
{ 
 public class LMessagesController : ApiController
{     
       private SOSEDMV10Entities db = new SOSEDMV10Entities();

        // Get MyMessages
        [HttpGet]
        public IHttpActionResult GetMyMessages(string UserID)
        {
            var xx = (from aa in db.LMessageRecipients.Where(p => p.RecipientID == UserID).Include(c => c.LMessage).Include(d => d.AspNetUser)
                      select new { aa.Id, aa.MessageID, CreatedDateTime= aa.LMessage.CreatedDateTime, CreatedByEmailId = aa.LMessage.AspNetUser.Email, aa.LMessage.Message,aa.LMessage.IsImportant,aa.RecipientID ,aa.ReadRecieptDateTime  }).OrderByDescending(c=>c.CreatedDateTime).AsEnumerable();
            return Ok(xx);
        }
        [HttpGet]
        public IHttpActionResult GetMySentMessages(String UserID)
        {
            //var data = db.LMessages.Where(p => p.CreatedByID == UserID).Select(X => new { X.Message, X.Id, X.IsImportant, X.SenderID, X.SenderRoleID, X.UpdatedByID, X.UpdatedDateTime, X.CretedDateTime, X.CreatedByID, X.CompanyID, CreatedByEmailId = X.AspNetUser.Email }).OrderByDescending(c => c.CretedDateTime).AsEnumerable();
            
            var data = db.LMessages.Where(p => p.CreatedByID == UserID).Select(X => new { X.Message, X.Id, CreatedDateTime=  X.CreatedDateTime, X.IsImportant, X.SenderRoleID,  CreatedByEmailId = X.AspNetUser.Email }).AsEnumerable();
            return Ok(data);
        }
        [HttpGet]
        public Boolean IsMessageBoardAvailable(int RoleID)
        {
            Boolean blnvalue = false;
            var data = db.LMessageAllowedRoles.Where(p => p.SenderRoleID == RoleID.ToString() || p.ReciepientRoleId == RoleID.ToString()) .FirstOrDefault();
            if (data != null) blnvalue = true;
            return blnvalue;
        }
        [HttpGet]
        public IHttpActionResult GetUsersListToSendMessage(string RoleName, string Portfolios, int iCompanyId, string ValueType)
        {
            
            List<string> UserIdList = new List<string>();
            if(ValueType == "Email")
            {
                var RawQuery = db.Database.SqlQuery<string>("select distinct AU.Email from AspnetUsers AU left join LUsers Lu on AU.Id=Lu.LuUserId and Lu.WFCompanyId = " + iCompanyId.ToString() + " left join LPayees Lp on AU.Id = Lp.LpUserId and  Lp.WFCompanyId = " + iCompanyId.ToString() + "  inner join MEntityPortfolios EP on (Lu.Id = EP.MepEntityId and (EP.MepEntityType   = 'LPayees' OR EP.MepEntityType   = 'LUsers' )) inner join AspNetUserRoles AUR on AUR.UserId=AU.Id  inner join AspNetRoles AR on AUR.RoleId = AR.Id where  AR.name = '" + RoleName + "' and AU.IsActive=1 and EP.MepPortfolioId in (select * from dbo.SplitString('" + Portfolios + "',','))");
                var Task = RawQuery.ToList();
                UserIdList = Task.ToList();
            }
            else
            {
                var RawQuery = db.Database.SqlQuery<string>("select distinct case AR.name when 'Payee' then Lp.LpUserId else Lu.LuUserId end as UsersToSendMessage from AspnetUsers AU left join LUsers Lu on AU.Id=Lu.LuUserId and Lu.WFCompanyId = " + iCompanyId.ToString() + " left join LPayees Lp on AU.Id = Lp.LpUserId and  Lp.WFCompanyId = " + iCompanyId.ToString() + "  inner join MEntityPortfolios EP on (Lu.Id = EP.MepEntityId and (EP.MepEntityType   = 'LPayees' OR EP.MepEntityType   = 'LUsers' )) inner join AspNetUserRoles AUR on AUR.UserId=AU.Id  inner join AspNetRoles AR on AUR.RoleId = AR.Id where  AR.name = '" + RoleName + "' and AU.IsActive=1 and EP.MepPortfolioId in (select * from dbo.SplitString('" + Portfolios + "',','))");
                var Task = RawQuery.ToList();
                UserIdList = Task.ToList();
            }
            //if (UserIds!="")
            //{
            //    UserIdList.Add(UserIds);
            //}
            //else
            //{
            //    UserIdList.Add(string.Empty);
            //}
            return Ok(UserIdList);
        }
        [HttpGet]
        public IHttpActionResult GetRoleListToSendMessage(int RoleID)
        {

            var data = (from AR in db.LMessageAllowedRoles
                     join Role in db.AspNetRoles on AR.ReciepientRoleId equals Role.Id
                     where AR.SenderRoleID == RoleID.ToString()
                     orderby Role.Name
                     select new
                     {
                         RecipientRoleID = Role.Id,
                         RecipientRoleName = Role.Name
                     }).AsEnumerable();
            return Ok(data);
        }
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> SetMessageAsRead(LMessageBoardViewModel MessageDetail)
        {
            //var Msg = db.LMessageRecipients.Where(p => p.MessageID == MessageDetail.Id).Where(p => p.RecipientID == MessageDetail.RecipientID).Where(p => p.ReadRecieptDateTime == null).Select(x => new { x.Id});
            //if(Msg==null)
            //{
                LMessageRecipient LMR = new LMessageRecipient();
                LMR.Id = MessageDetail.Id;
                LMR.RecipientID = MessageDetail.RecipientID;
                LMR.MessageID = MessageDetail.MessageID;
                LMR.RecipientRoleID = Convert.ToInt32(MessageDetail.RecipientRoleID);
                LMR.ReadRecieptDateTime = DateTime.Now;
                db.Entry(LMR).State = EntityState.Modified;
                await db.SaveChangesAsync();
            //}
            return Ok();
        }
        // GET api/LMessages/5
        [ResponseType(typeof(LMessage))]
        public async Task<IHttpActionResult> GetLMessages(int id)
        {
            var LMessages = db.LMessageRecipients.Where(p => p.Id == id).Include(c => c.LMessage).Select
                (x => new { x.Id, x.MessageID,  x.LMessage.Message,x.LMessage.IsImportant, x.RecipientID,x.RecipientRoleID,x.ReadRecieptDateTime }).FirstOrDefault();
            //var LMessages = db.LMessages.Where(p => p.Id == id).Include(c => c.GCompany).Select(x => new { x.IsImportant, x.Id, x.CompanyID, x.Message, x.SenderID, x.GCompany.GcCompanyName }).FirstOrDefault();
            if (LMessages == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Message")));
            }
            return Ok(LMessages);

        }
        
        // POST api/LMessages

        [ResponseType(typeof(LMessage))]
        public async Task<IHttpActionResult> PostLMessages(LMessageBoardViewModel LMessages)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "Message")));
            }
            try
            {
                LMessage Lm = new LMessage();
                Lm.CompanyID = LMessages.CompanyID;
                Lm.Message = LMessages.Message;
                Lm.IsImportant = LMessages.IsImportant;
                Lm.SenderID = LMessages.CreatedById;
                Lm.CreatedByID = LMessages.CreatedById;
                Lm.CreatedDateTime = DateTime.Today;
                Lm.SenderRoleID = Convert.ToInt32(LMessages.SenderRoleID);
                db.LMessages.Add(Lm);
                db.SaveChanges();
                LMessages.Id = Lm.Id;
                for (int i = 0; i < LMessages.UsersToSendMessage.Count; i++)
                {
                    LMessageRecipient Lmr = new LMessageRecipient();
                    Lmr.MessageID = LMessages.Id;
                    Lmr.RecipientRoleID = Convert.ToInt32(LMessages.RecipientRoleID);
                    Lmr.RecipientID = LMessages.UsersToSendMessage[i];
                    db.LMessageRecipients.Add(Lmr);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return Ok();

        }
        public async Task<IHttpActionResult> SaveMessage(LMessageBoardViewModel Lmessages,string Message,string SenderId, string CreatedById )
    { var LMessagesModel = new LMessage();
        var LMessageRecipients = new LMessageRecipient();
        {
            try
            {
                LMessagesModel.CreatedByID = Lmessages.CreatedById;
                    LMessagesModel.Message = Lmessages.Message;
                    LMessagesModel.IsImportant = Lmessages.IsImportant;
                    LMessagesModel.SenderID = Lmessages.SenderRoleID;
                    //LMessagesModel.CreatedDateTime = Lmessages.CreatedDateTime;
                    LMessagesModel.CompanyID = Lmessages.CompanyID;
                    LMessagesModel.UpdatedByID = Lmessages.UpdatedById;
                    LMessagesModel.UpdatedDateTime = Lmessages.UpdatedDateTime;
                    LMessageRecipients.MessageID = LMessageRecipients.MessageID;
                    LMessageRecipients.RecipientRoleID = LMessageRecipients.RecipientRoleID;
                    LMessageRecipients.ReadRecieptDateTime = LMessageRecipients.ReadRecieptDateTime;
                    db.LMessages.Add(LMessagesModel);
                db.LMessageRecipients.Add(LMessageRecipients);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
            return Ok();
        }
    }
    
        // DELETE api/LMessages/5
        [ResponseType(typeof(LMessage))]
        public async Task<IHttpActionResult> DeleteLMessages(int id)
        {
            LMessageRecipient LMessageRecipient = await db.LMessageRecipients.FindAsync(id);
            if (LMessageRecipient == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Message")));
            }
            try
            {
                db.LMessageRecipients.Remove(LMessageRecipient);
                await db.SaveChangesAsync();
                return Ok(LMessageRecipient);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }

            //GET Method to Get count where Recipient ID = User ID from LMessageRecipients
        }
        public IHttpActionResult GetUnreadMessageCount(String UserID)
        {
            var xx = db.LMessageRecipients.Where(a => a.RecipientID == UserID).Where(p => p.ReadRecieptDateTime == null).Count();
            return Ok(xx);
        }
        
        public IHttpActionResult GetRolesToSendMessages(int RoleID)
        {
            var xx = (from aa in db.LMessageAllowedRoles.Where(p => p.SenderRoleID == RoleID.ToString()).Include(d => d.AspNetRole)
                      select new { aa.Id, aa.ReciepientRoleId, ReceipientRoleName = aa.AspNetRole.Name }).AsEnumerable();
            return Ok(xx);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LMessagesExists(int id)
        {
            return db.LMessages.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("LMessages", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "Message", "VIEW(S)"));
            else
                //Something else failed return original error message as retrieved from database
                return SqEx.Message;
        }

    }
}

