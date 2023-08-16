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
using System.Globalization;
using System.Data.Entity.Validation;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LChangeRequestsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
        public LChangeRequestsController()
        {
        }
        public LChangeRequestsController(ApplicationUserManager userManager,
          ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: api/LChangeRequests
        //public IHttpActionResult GetLChangeRequests()
        //{
        //    var userlist = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LChangeRequests.Include(p => p.RStatus)
        //             join yy in db.LPayees on aa.LcrRowId equals yy.Id select new
        //             {aa.LcrCompanyId,aa.LcrAction, aa.Id,aa.LcrColumnLabel,aa.LcrColumnName,aa.LcrComments,aa.LcrEntityName,aa.LcrNewValue,
        //              aa.GCompany.GcCompanyName,yy.LpFirstName,yy.LpLastName,yy.LpPayeeCode,aa.RStatus.RsStatus,aa.LcrOldValue,aa.LcrCreatedById,aa.LcrCreatedDateTime
        //              ,aa.LcrEffectiveStartDate,
        //              aa.LcrUpdatedById,
        //              aa.LcrUpdatedDateTime,
        //              aa.LcrRowId,
        //              CreatedBy = (!string.IsNullOrEmpty(aa.LcrCreatedById)) ? userlist.Where(p => p.Id == aa.LcrCreatedById).FirstOrDefault().UserName : "",
        //              UpdatedBy = (!string.IsNullOrEmpty(aa.LcrUpdatedById)) ? userlist.Where(p => p.Id == aa.LcrUpdatedById).FirstOrDefault().UserName : ""
        //             }).OrderByDescending(p => p.LcrCreatedDateTime);
        //    return Ok(xx);
        //}

        //get payee change request by status name and companyId
        public IHttpActionResult GetLChangeRequestByStatusNameCompanyIdEntityName(string StatusName,int CompanyId,string EntityName,string ReportsToId)
        {
            switch (EntityName)
            {
                case "Payee":
                    // var userlist = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
                    var xx = (from aa in db.LChangeRequests.Where(p => p.WFStatus == StatusName).Where(p => p.LcrCompanyId == CompanyId).Where(p => p.LcrEntityName == EntityName)
                              join yy in db.LPayees on aa.LcrRowId equals yy.Id
                              join bb in db.AspNetUsers on aa.LcrCreatedById equals bb.Id
                              join tt in db.AspNetUsers on aa.LcrUpdatedById equals tt.Id
                                into grp
                              from c in grp.DefaultIfEmpty()//this line will give result even if updated by is null
                              join Lu in db.LUsers on aa.LcrUpdatedById equals Lu.LuUserId
                     into grp1
                              from u in grp1.DefaultIfEmpty()//this line will give result even if updated by is null
                              where u.LuReportsToId == ReportsToId || aa.LcrCreatedById == ReportsToId
                              select new
                              {
                                  aa.Id,
                                  aa.LcrColumnLabel,
                                  aa.LcrColumnName,
                                  aa.LcrEntityName,
                                  aa.LcrNewValue,
                                  aa.LcrAction,
                                  aa.LcrOldValue,
                                  aa.LcrUpdatedDateTime,
                                  aa.LcrUpdatedById,
                                  FullName = yy.LpFirstName + " " + yy.LpLastName + " (" + yy.LpPayeeCode + ")",
                                  aa.LcrCreatedDateTime,
                                  aa.LcrEffectiveStartDate,
                                  aa.LcrCreatedById,
                                  aa.LcrCompanyId,
                                  aa.LcrRowId,
                                  yy.LpPayeeCode,
                                  yy.LpFirstName,
                                  yy.LpLastName,
                                  CreatedBy = bb.UserName,
                                  UpdatedBy = c.UserName
                              }).OrderByDescending(p => p.LcrCreatedDateTime);
                    return Ok(xx);
                case "User":
                    var User = (from aa in db.LChangeRequests.Where(p => p.WFStatus == StatusName).Where(p => p.LcrCompanyId == CompanyId).Where(p => p.LcrEntityName == EntityName)
                              join yy in db.LUsers on aa.LcrRowId equals yy.Id
                              join bb in db.AspNetUsers on aa.LcrCreatedById equals bb.Id
                              join tt in db.AspNetUsers on aa.LcrUpdatedById equals tt.Id
                                into grp
                              from c in grp.DefaultIfEmpty()//this line will give result even if updated by is null
                              select new
                              {
                                  aa.Id,
                                  aa.LcrColumnLabel,
                                  aa.LcrColumnName,
                                  aa.LcrEntityName,
                                  aa.LcrNewValue
                                  ,
                                  aa.LcrAction,
                                  aa.LcrOldValue,
                                  aa.LcrUpdatedDateTime,
                                  aa.LcrUpdatedById,
                                  FullName = yy.LuFirstName + " " + yy.LuLastName ,
                                  aa.LcrCreatedDateTime,
                                  aa.LcrEffectiveStartDate,
                                  aa.LcrCreatedById,
                                  aa.LcrCompanyId,
                                  aa.LcrRowId,
                                  CreatedBy = bb.UserName,
                                  UpdatedBy = c.UserName
                              }).OrderByDescending(p => p.LcrCreatedDateTime);
                    return Ok(User);
            }
            return Ok();
        }

        public IHttpActionResult GetChangeRequestUnderStartDateEndDate(int CompanyId, string StartDate, string EndDate)
        {
            //These vaiables are declared to convert start date and end date to datetime as in linq datetime objects are required for comparison
            var StartDateTime = DateTime.ParseExact(StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var EndDateTime = DateTime.ParseExact(EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            EndDateTime = EndDateTime.AddDays(1).AddMinutes(-1);
            var xx = (from p in db.LChangeRequests.Include(p => p.GCompany).Where(p => p.LcrUpdatedDateTime >= StartDateTime && p.LcrUpdatedDateTime < EndDateTime)
                      join yy in db.LPayees on p.LcrRowId equals yy.Id
                      join bb in db.AspNetUsers on p.LcrCreatedById equals bb.Id
                      join tt in db.AspNetUsers on p.LcrUpdatedById equals tt.Id
                      into grp from c in grp.DefaultIfEmpty()//this line will give result even if updated by is null
                      select new
                      {
                          CreatedBy=bb.UserName,
                        UpdatedBy = c.UserName,
                          p.GCompany.GcCompanyName,
                          p.LcrUpdatedById,
                          yy.LpPayeeCode,
                          FullName=yy.LpFirstName+" "+yy.LpLastName,
                          p.LcrAction,
                         p.LcrUpdatedDateTime,
                          p.LcrCreatedById,
                          p.LcrCreatedDateTime,
                          p.Id,
                          p.LcrColumnLabel,
                          p.LcrColumnName,
                          p.LcrEntityName,
                          p.LcrNewValue,
                          p.LcrOldValue
                      });
            return Ok(xx);
        }

        public IHttpActionResult GetLChangeRequestByStatusNameCompanyIdEntityNameCreatedById(string StatusName, int CompanyId, string EntityName,string CreatedByUserId)
        {
            switch (EntityName)
            {
                case "Payee":
                    var xx = (from aa in db.LChangeRequests.Where(p => p.WFStatus == StatusName).Where(p => p.LcrCreatedById == CreatedByUserId).Where(p => p.LcrCompanyId == CompanyId).Where(p => p.LcrEntityName == EntityName)
                              join yy in db.LPayees on aa.LcrRowId equals yy.Id
                              join bb in db.AspNetUsers on aa.LcrCreatedById equals bb.Id
                              join tt in db.AspNetUsers on aa.LcrUpdatedById equals tt.Id
                                into grp
                              from c in grp.DefaultIfEmpty()//this line will give result even if updated by is null
                              select new
                              {
                                  aa.Id,
                                  aa.LcrColumnLabel,
                                  aa.LcrColumnName,
                                  aa.LcrEntityName,
                                  aa.LcrNewValue
                                  ,
                                  aa.LcrAction,
                                  aa.LcrOldValue,
                                  aa.LcrUpdatedDateTime,
                                  aa.LcrUpdatedById,
                                  FullName = yy.LpFirstName + " " + yy.LpLastName + " (" + yy.LpPayeeCode + ")",
                                  aa.LcrCreatedDateTime,
                                  aa.LcrEffectiveStartDate,
                                  aa.LcrCreatedById,
                                  aa.LcrCompanyId,
                                  aa.LcrRowId,
                                  yy.LpPayeeCode,
                                  yy.LpFirstName,
                                  yy.LpLastName,
                                  CreatedBy = bb.UserName,
                                  UpdatedBy = c.UserName
                              }).OrderByDescending(p => p.LcrCreatedDateTime);
                    return Ok(xx);
                case "User":
                    var User = (from aa in db.LChangeRequests.Where(p => p.WFStatus == StatusName).Where(p => p.LcrCreatedById == CreatedByUserId).Where(p => p.LcrCompanyId == CompanyId).Where(p => p.LcrEntityName == EntityName)
                              join yy in db.LUsers on aa.LcrRowId equals yy.Id
                              join bb in db.AspNetUsers on aa.LcrCreatedById equals bb.Id
                              join tt in db.AspNetUsers on aa.LcrUpdatedById equals tt.Id
                                into grp
                              from c in grp.DefaultIfEmpty()//this line will give result even if updated by is null
                              select new
                              {
                                  aa.Id,
                                  aa.LcrColumnLabel,
                                  aa.LcrColumnName,
                                  aa.LcrEntityName,
                                  aa.LcrNewValue
                                  ,
                                  aa.LcrAction,
                                  aa.LcrOldValue,
                                  aa.LcrUpdatedDateTime,
                                  aa.LcrUpdatedById,
                                  FullName = yy.LuFirstName + " " + yy.LuLastName ,
                                  aa.LcrCreatedDateTime,
                                  aa.LcrEffectiveStartDate,
                                  aa.LcrCreatedById,
                                  aa.LcrCompanyId,
                                  aa.LcrRowId,
                                  CreatedBy = bb.UserName,
                                  UpdatedBy = c.UserName
                              }).OrderByDescending(p => p.LcrCreatedDateTime);
                    return Ok(User);
            }
            return Ok();
        }

        //method to update Change Request list --using same model for updating Change request which was used for Payees as parameters are same
        [HttpPost]
        public IHttpActionResult Sp_UpdateChangeRequestStatus(Sp_UpdateItemStatusViewModel model)
        {
            //db.SpUpdateChangeRequestData(model.ItemList, model.StatusName, model.Comments, model.UpdatedBy, model.UpdatedDateTime);
            //using (var transaction = db.Database.BeginTransaction())
            //{
            //    var ChangeRequest = new LChangeRequest();
            //    try
            //    {

            //Get the status id from status name
            //        var StatusId = db.RStatuses.Where(p => p.RsStatus == model.StatusName).Where(p => p.RStatusOwner.RsoStatusOwner == "ChangeRequest").FirstOrDefault().Id;
            //        var UserName = db.AspNetUsers.Find(model.UpdatedBy).UserName;
            //        //convert comma seperated list to an string of array
            //        var ItemArray = model.ItemList.Split(',');
            //        for(var i=0;i>ItemArray.Length;i=i+2)
            //        {
            //            //first update change request
            //            var ChangeRequestId = Convert.ToInt32(ItemArray[i]);
            //            ChangeRequest = db.LChangeRequests.Where(p=>p.Id==ChangeRequestId).FirstOrDefault();
            //            ChangeRequest.LcrUpdatedById = model.UpdatedBy;
            //            ChangeRequest.LcrUpdatedDateTime = model.UpdatedDateTime;
            //            ChangeRequest.LcrStatusId = StatusId;
            //            if(!string.IsNullOrEmpty(ItemArray[i + 1]))
            //            ChangeRequest.LcrComments = "["+UserName+"] ["+model.UpdatedDateTime+"] "+ItemArray[i + 1]+Environment.NewLine+ChangeRequest.LcrComments;
            //            db.Entry(ChangeRequest).State = EntityState.Modified;
            //            db.SaveChanges();
            //            //Then update Payee tables if Status is Approved
            //            if (model.StatusName == "Approved")
            //            {
            //                switch (ChangeRequest.LcrAction)
            //                {
            //                    case "Edit":

            //                        var SqlQuery = "update LPayees set LpUpdatedById='" + model.UpdatedBy + "' ,LpUpdatedDateTime= '" + model.UpdatedDateTime +" , ";
            //                        switch (ChangeRequest.LcrColumnName)
            //                        {
            //                            case "LpChannelId":
            //                            case "LpSubChannelId":
            //                                SqlQuery = SqlQuery + ChangeRequest.LcrColumnName + " = " + ChangeRequest.LcrNewId;
            //                                break;
            //                            case "LpEffectiveStartDate":
            //                            case "LpEffectiveEndDate":
            //                                SqlQuery = SqlQuery + ChangeRequest.LcrColumnName + " = '" + ChangeRequest.LcrNewValue+"' ";
            //                                break;
            //                            default:
            //                                SqlQuery = SqlQuery + ChangeRequest.LcrColumnName + " = '" + ChangeRequest.LcrNewValue + "' ";
            //                                break;
            //                        }
            //                        SqlQuery = SqlQuery + " where Id = "+ ChangeRequest.LcrRowId;
            //                        db.Database.ExecuteSqlCommand(SqlQuery);
            //                        break;
            //                    case "Delete":
            //                        var PayeeId = Convert.ToInt32(ChangeRequest.LcrRowId);
            //                        var ChangeStartDate = ChangeRequest.LcrEffectiveStartDate;
            //                        var PayeeParents = db.LPayeeParents.Where(p => p.LppPayeeId == PayeeId).Where(p => p.LppEffectiveEndDate == null).FirstOrDefault();
            //                        if (PayeeParents != null)
            //                        {
            //                            PayeeParents.LppEffectiveEndDate = ChangeStartDate;
            //                            db.Entry(PayeeParents).State = EntityState.Modified;
            //                            db.SaveChanges();
            //                        }
            //                        var Payee = db.LPayees.Find(PayeeId);
            //                        Payee.LpEffectiveEndDate = ChangeStartDate;
            //                        Payee.LpStatus = "InActive";
            //                        Payee.LpUpdatedById = model.UpdatedBy;
            //                        Payee.LpUpdatedDateTime = model.UpdatedDateTime;
            //                        db.Entry(Payee).State = EntityState.Modified;
            //                        db.SaveChanges();
            //                        var PayeeAuditLog = new LPayeeAuditLog {LpalAction="InActive",LpalPayeeId=PayeeId,LpalUpdatedById=model.UpdatedBy,LpalUpdatedDateTime=model.UpdatedDateTime };
            //                        db.LPayeeAuditLogs.Add(PayeeAuditLog);
            //                        db.SaveChanges();
            //                        break;
            //                }
            //            }
            //        }
            //        transaction.Commit();
            //        return Ok(ChangeRequest);
            //    }
            //    catch (Exception ex)
            //    {
            //        transaction.Rollback();
            //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            //    }
            //}
            return Ok();
        }

        //This method is called when user updates Lusers change request
        public IHttpActionResult PutUpdateUserChangeRequest(Sp_UpdateItemStatusViewModel ChangeRequestModel)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var ChangeRequestArray = ChangeRequestModel.ItemList.Split(',');
                    for (var i = 0; i < ChangeRequestArray.Length; i = i + 2)
                    {
                        if (!string.IsNullOrEmpty(ChangeRequestArray[i]))
                        {
                            var Id = Convert.ToInt32(ChangeRequestArray[i]);
                            var LChangeRequest = db.LChangeRequests.Find(Id);
                            if (!string.IsNullOrEmpty(ChangeRequestArray[i + 1]))
                            {
                                LChangeRequest.WFComments = "[" + ChangeRequestModel.UpdatedBy + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + ChangeRequestArray[i + 1] + Environment.NewLine + LChangeRequest.WFComments;
                            }
                            LChangeRequest.LcrUpdatedById = ChangeRequestModel.UpdatedBy;
                            LChangeRequest.LcrUpdatedDateTime = DateTime.UtcNow;
                            db.Entry(LChangeRequest).State = EntityState.Modified;
                            db.SaveChanges();
                            //Update LUser table if request is approved
                            if (ChangeRequestModel.StatusName == "Approved")
                            {
                                string sqlQuery = "";
                                switch (LChangeRequest.LcrAction)
                                {
                                    case "Edit":
                                        switch (LChangeRequest.LcrColumnName)
                                        {
                                            case "Roles":
                                                var User = db.LUsers.Find(LChangeRequest.LcrRowId);
                                                if (!string.IsNullOrEmpty(LChangeRequest.LcrOldValue))
                                                {
                                                    var ExistingRoles = UserManager.GetRoles(User.LuUserId).ToArray();
                                                    UserManager.RemoveFromRoles(User.LuUserId, ExistingRoles);
                                                }
                                                if (!string.IsNullOrEmpty(LChangeRequest.LcrNewValue))
                                                {
                                                    string[] RoleList = LChangeRequest.LcrNewValue.Split(',').ToArray();//Update user roles
                                                    UserManager.AddToRoles(User.LuUserId, RoleList);
                                                }
                                                break;
                                            case "LuReportsToId":
                                                sqlQuery = "Update LUsers set " + LChangeRequest.LcrColumnName + " = '" + LChangeRequest.LcrNewId + "' where Id=" + LChangeRequest.LcrRowId;
                                                db.Database.ExecuteSqlCommand(sqlQuery);
                                                break;
                                            default:
                                                sqlQuery = "Update LUsers set " + LChangeRequest.LcrColumnName + " = '" + LChangeRequest.LcrNewValue + "' where Id=" + LChangeRequest.LcrRowId;
                                                db.Database.ExecuteSqlCommand(sqlQuery);
                                                break;
                                        }
                                       
                                        break;
                                    case "Delete":
                                        sqlQuery = "Update LUsers set LuStatus = InActive where Id=" + LChangeRequest.LcrRowId;
                                        db.Database.ExecuteSqlCommand(sqlQuery);
                                        sqlQuery = "Update AspnetUsers set IsActive = 0 where Id=( select LuUserId from LUsers where Id =" + LChangeRequest.LcrRowId;
                                        db.Database.ExecuteSqlCommand(sqlQuery);
                                        break;
                                }
                            }
                        }
                    }
                    transaction.Commit();
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
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage));
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex));
                }
            }
            return Ok();
        }

        // GET: api/LChangeRequests/5
        [ResponseType(typeof(LChangeRequest))]
        public async Task<IHttpActionResult> GetLChangeRequest(int id)
        {
            var LChangeRequest = db.LChangeRequests.Where(p => p.Id == id).FirstOrDefault();
            if (LChangeRequest == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "CHANGE REQUEST")));
            }
            return Ok(LChangeRequest);
        }

        // PUT: api/LChangeRequests/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLChangeRequest(int id, LChangeRequest LChangeRequest)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "CHANGE REQUEST")));
            }

            if (id != LChangeRequest.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "CHANGE REQUEST")));
            }
            try
            {
                if (LChangeRequest.LcrAction == "Edit" && Convert.ToString(LChangeRequest.LcrOldValue) == "" && Convert.ToString(LChangeRequest.LcrNewValue) == "")
                {
                    //dont do anything
                }
                else
                {
                    db.Entry(LChangeRequest).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                
            }
            catch (Exception ex)
            {
                if (!LChangeRequestExists(id))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "CHANGE REQUEST")));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/LChangeRequests
        [ResponseType(typeof(LChangeRequest))]
        public async Task<IHttpActionResult> PostLChangeRequest(LChangeRequest LChangeRequest,string LoggedInRoleId,string WorkflowName)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "CHANGE REQUEST")));
            }
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkflowName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    //Note: Calculate Ordinal based on the Current Role who has created CR based on RoleId and Opco and WorkflowName
                    var ConfigData = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LChangeRequest.WFCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == WorkflowName).FirstOrDefault();
                    var Ordinal = 0; //setting Ordinal to default 0 value.
                    string AnalystId = null;
                    if (ConfigData != null)//Checking if ConfigData is not null
                    {
                        Ordinal = ConfigData.LwfcOrdinalNumber;
                        if (ConfigData.LwfcActingAs == "Analyst")
                        {
                            AnalystId = LChangeRequest.WFCurrentOwnerId;
                        }
                    }
                    //As soon as CHANGE REQUEST is created a database trigger will automatically Create a sequence for this CHANGE REQUEST which will provide sequence of Claim Ids 
                    //while saving Lclaims data and will also add a row in GKeyValues for this sequence Name.
                    LChangeRequest.WFAnalystId = AnalystId;
                        LChangeRequest.WFRequesterRoleId = LoggedInRoleId;
                    LChangeRequest.WFType = WFDetails.RwfWFType;
                    LChangeRequest.WFOrdinal = Ordinal;
                    LChangeRequest.WFStatus = "Saved";
                    LChangeRequest.LcrCreatedByForm = true;//RK Added 17122018 R2.4.1
                    if (LChangeRequest.LcrAction == "Edit" && Convert.ToString(LChangeRequest.LcrOldValue) == "" && Convert.ToString(LChangeRequest.LcrNewValue) == "")
                    {
                        //dont do anything
                    }
                    else
                    {
                        db.LChangeRequests.Add(LChangeRequest);
                        await db.SaveChangesAsync();
                    }
                    
                    //Add portfolios to this change request
                    //Get Portfolios of the Entity Type and add same portfoliosof Entity type as LChangeRequest
                    //RK R2.4.1 21122018 Added distinct
                    var PortfolioList = db.MEntityPortfolios.Where(p => p.MepEntityType == LChangeRequest.LcrEntityName).Where(p => p.MepEntityId == LChangeRequest.LcrRowId).Select(p => p.MepPortfolioId).Distinct().ToList();
                    foreach (var PF in PortfolioList)
                    {
                        var MEntityPortfolio = new MEntityPortfolio { MepPortfolioId = PF, MepEntityId = LChangeRequest.Id, MepEntityType = "LChangeRequests" };
                        db.MEntityPortfolios.Add(MEntityPortfolio);
                        db.SaveChanges();
                    }
                    //Add Entry in Audit Log
                    if (WorkflowName == "UsersCR")
                    {
                        //SG20201204 - R3.2 - EntityName manufactured from CRId instead of EmailId field which doesnot exist in LChangeRequest table
                        string EntityName = db.LUsers.Where(a=>a.Id == LChangeRequest.LcrRowId).FirstOrDefault().LuEmail;
                        if (LChangeRequest.LcrAction == "Delete")
                        {
                            Globals.ExecuteSPAuditLog(WorkflowName, "Audit", null, "Create",
                                   "Delete", LChangeRequest.LcrCreatedById, DateTime.UtcNow, LChangeRequest.WFStatus, LChangeRequest.WFStatus,
                                  WFDetails.RwfBaseTableName, LChangeRequest.Id,
                                  //Convert.ToString(LChangeRequest.EmailID), 
                                  EntityName,
                                  WFDetails.Id, LChangeRequest.WFCompanyId, string.Empty, LChangeRequest.WFRequesterRoleId,null);
                        }
                        else
                        {
                            Globals.ExecuteSPAuditLog(WorkflowName, "Audit", null, "Create",
                                                          "Create", LChangeRequest.LcrCreatedById, DateTime.UtcNow, LChangeRequest.WFStatus, LChangeRequest.WFStatus,
                                                         WFDetails.RwfBaseTableName, LChangeRequest.Id, 
                                                         //Convert.ToString(LChangeRequest.EmailID),
                                                         EntityName,
                                                         WFDetails.Id, LChangeRequest.WFCompanyId, string.Empty, LChangeRequest.WFRequesterRoleId,null);

                        }
                        
                    } else if (WorkflowName == "Users")
                    {
                        string EntityName = db.LUsers.Where(a => a.Id == LChangeRequest.LcrRowId).FirstOrDefault().LuEmail;

                        Globals.ExecuteSPAuditLog(WorkflowName, "Audit", null, "Create",
                                                          "Create", LChangeRequest.LcrCreatedById, DateTime.UtcNow, LChangeRequest.WFStatus, LChangeRequest.WFStatus,
                                                         WFDetails.RwfBaseTableName, LChangeRequest.Id,
                                                         //Convert.ToString(LChangeRequest.EmailID),
                                                         EntityName,
                                                         WFDetails.Id, LChangeRequest.WFCompanyId, string.Empty, LChangeRequest.WFRequesterRoleId, null);

                    }
                    else //Workflow Name = Payee OR PayeeCR
                    {
                        //RK R.2.9 Made changes to pick payee name for LAudit.LaEntityName, previously it saves LPayees as LaEntityName
                        Globals.ExecuteSPAuditLog(WorkflowName, "Audit", null, "Create",
                                                         "Create", LChangeRequest.LcrCreatedById, DateTime.UtcNow, LChangeRequest.WFStatus, LChangeRequest.WFStatus,
                                                        WFDetails.RwfBaseTableName, LChangeRequest.Id,
                                                         //Convert.ToString(LChangeRequest.LcrEntityName), 
                                                         Globals.GetPayeeName(Convert.ToInt32(LChangeRequest.LcrRowId),true),
                                                        WFDetails.Id, LChangeRequest.WFCompanyId, string.Empty, LChangeRequest.WFRequesterRoleId, null);


                    }
                    tran.Commit();
                    return CreatedAtRoute("DefaultApi", new { id = LChangeRequest.Id }, LChangeRequest);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
                }
            }
            
        }

        // DELETE: api/LChangeRequests/5
        [ResponseType(typeof(LChangeRequest))]
        public async Task<IHttpActionResult> DeleteLChangeRequest(int id)
        {
            LChangeRequest LChangeRequest = await db.LChangeRequests.FindAsync(id);
            if (LChangeRequest == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "CHANGE REQUEST")));
            }
            try
            {
                db.LChangeRequests.Remove(LChangeRequest);
                await db.SaveChangesAsync();
                return Ok(LChangeRequest);
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

        private bool LChangeRequestExists(int id)
        {
            return db.LChangeRequests.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
             if (SqEx.Message.IndexOf("SpUpdateChangeRequestData", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "CHANGE REQUEST", "DATABASE OBJECTS"));
             else
            //Something else failed return original error message as retrieved from database
            return SqEx.Message;
        }

    }
}
