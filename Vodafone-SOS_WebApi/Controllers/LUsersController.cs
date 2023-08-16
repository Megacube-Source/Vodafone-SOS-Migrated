using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Threading;
using System.IO;
//using ICSharpCode.SharpZipLib.Zip;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Ionic.Zip;

namespace Vodafone_SOS_WebApi.Controllers
{
   [CustomExceptionFilter]
    public class LUsersController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
        public LUsersController()
        {
        }
        public LUsersController(ApplicationUserManager userManager,
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
        public IHttpActionResult GetLUsersDropdownData(int CompanyId, string UserName, string Workflow)
        {
            var data = db.LUsers.Where(p => p.LuCompanyId== CompanyId).Select(x => new { FullName=x.LuFirstName+" "+x.LuLastName, x.Id }).OrderBy(p => p.FullName).AsEnumerable();
            return Ok(data);
        }

        //This method is used to populate the List box having roles in user create/edit page
        public IHttpActionResult GetLUserRoles(string UserId,string CompanyCode, string UserName, string Workflow)
        {
            //List of roles provided by JS which are not shown in Listbox
            var ExcludedRoles = new string[] { "BI", "Finance Decision Support", "Group Admin", "L2 Admin", "MDM Analyst", "OpCoAdmin", "Payee" };
            var ExistingRoles = UserManager.GetRoles(UserId);
            var RoleList = (from aa in db.AspNetRoles.Where(p=>ExcludedRoles.Contains(p.Name)==false).Where(p=>p.CompanyCode==CompanyCode) select new { aa.Id, aa.Name, Selected = (ExistingRoles.Contains(aa.Name)) ? true : false }).OrderByDescending(p=>p.Selected).ThenBy(p=>p.Name);
            return Ok(RoleList);
        }



        public IHttpActionResult GetLUserRolesbyId(string UserId, string CompanyCode)
        {
            var ExcludedRoles = new string[] { "BI", "Finance Decision Support", "Group Admin", "L2 Admin", "MDM Analyst", "OpCoAdmin", "Payee" };
            var ExistingRoles = UserManager.GetRoles(UserId);
            var RoleList = (from aa in db.AspNetRoles.Where(p => ExcludedRoles.Contains(p.Name) == false).Where(p => p.CompanyCode == CompanyCode).Where(p => ExistingRoles.Contains(p.Name) == true) select new { aa.Id, aa.Name });
            return Ok(RoleList);
        }

        public IHttpActionResult GetUserRoleByIdForMissingPortfolio(string UserId, string CompanyCode)
        {
            var ExcludedRoles = new string[] { ""};
            var ExistingRoles = UserManager.GetRoles(UserId);
            var RoleList = (from aa in db.AspNetRoles.Where(p => ExcludedRoles.Contains(p.Name) == false).Where(p => p.CompanyCode == CompanyCode).Where(p => ExistingRoles.Contains(p.Name) == true) select new { aa.Id, aa.Name });
            return Ok(RoleList);
        }




        public IHttpActionResult GetLUserPayeeRoles(string UserId, string CompanyCode, string UserName, string Workflow)
        {
            var ExistingRoles = UserManager.GetRoles(UserId);
            var RoleList = (from aa in db.AspNetRoles.Where(p => ExistingRoles.Contains(p.Name) == true).Where(p => p.CompanyCode == CompanyCode) select new { aa.Id, aa.Name, Selected = (ExistingRoles.Contains(aa.Name)) ? true : false }).OrderByDescending(p => p.Selected).ThenBy(p => p.Name);
            return Ok(RoleList);
        }


        [HttpGet]
        public IHttpActionResult GetAllEmailId(int CompanyId,string Workflow)
        {
            var xx = (from aa in db.LUsers.Where(p => p.LuCompanyId == CompanyId && (p.WFStatus == "Completed" || p.WFStatus == "Suspended"))
                      select new
                      {
                          aa.Id,
                          aa.LuEmail

                      }).OrderBy(p => p.LuEmail).ToList();
            return Ok(xx);
        }

        public  IHttpActionResult GetUserDetailsByAspNetUserID(string strUserID, string UserName, string Workflow)
        {
            var USR = db.LUsers.Where(p => p.LuUserId == strUserID).Select(x => new
            {
                x.LuFirstName,x.LuLastName,x.LuEmail
            }).FirstOrDefault();
            if (USR == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "USER DETAILS")));
            }
            var result = new LUser { LuFirstName = USR.LuFirstName, LuLastName = USR.LuLastName, LuEmail = USR.LuEmail };
            return Ok(result);
            
        }
        // GET: api/LUsers?CompanyId=5
        public IHttpActionResult GetLUsersByCompanyIdStatus(int CompanyId,string Status,string ReportsToId, string UserName, string Workflow)
        {
            var xx = (from aa in db.LUsers.Where(p => p.LuCompanyId == CompanyId).Where(p=>p.LuStatus==Status).Include(c => c.GCompany).ToList()
                      join yy in db.LUsers on aa.LuUpdatedById equals yy.LuUserId
                      join bb in db.AspNetUsers on aa.LuCreatedById equals bb.Id
                      join cc in db.AspNetUsers on aa.LuUpdatedById equals cc.Id
                      join tt in db.AspNetUsers on aa.LuReportsToId equals tt.Id
                      into grp
                      from c in grp.DefaultIfEmpty()//this line will give result even if updated by is null
                      where aa.LuCreatedById==ReportsToId||yy.LuReportsToId==ReportsToId
                      select new {FullName= aa.LuFirstName+" "+aa.LuLastName, aa.LuPhone, aa.LuStatus,aa.LuIsManager,aa.LuUpdatedById,aa.LuUpdatedDateTime,
                          Roles = string.Join(",", UserManager.GetRoles(aa.LuUserId).ToList()),
                          ReportsTo= (c != null) ? c.UserName : "",
                          aa.LuReportsToId,UpdatedBy=cc.UserName,CreatedBy=bb.UserName,aa.LuCreatedById,aa.LuCreatedDateTime, aa.GCompany.GcCompanyName, aa.Id }).OrderBy(p => p.FullName).ToList();
            return Ok(xx);
        }
        public IHttpActionResult GetLUsersByCompanyIdStatusCreatedByUserId(int CompanyId, string Status,string CreatedByUserId, string UserName, string Workflow)
        {
            var xx = (from aa in db.LUsers.Where(p => p.LuCompanyId == CompanyId).Where(p=>p.LuStatus==Status).Where(p=>p.LuCreatedById==CreatedByUserId).Include(c => c.GCompany).ToList()
                      join bb in db.AspNetUsers on aa.LuCreatedById equals bb.Id
                      join cc in db.AspNetUsers on aa.LuUpdatedById equals cc.Id
                      join tt in db.AspNetUsers on aa.LuReportsToId equals tt.Id
                      into grp
                      from c in grp.DefaultIfEmpty()//this line will give result even if updated by is null
                      select new
                      {
                          FullName = aa.LuFirstName + " " + aa.LuLastName,
                          aa.LuPhone,
                          aa.LuStatus,
                          aa.LuIsManager,
                          aa.LuUpdatedById,
                          aa.LuUpdatedDateTime,
                          aa.LuReportsToId,
                          UpdatedBy = cc.UserName,
                          CreatedBy = bb.UserName,
                          aa.LuCreatedById,ReportsTo=(c!=null)?c.UserName:"",
                          aa.LuCreatedDateTime,
                          aa.GCompany.GcCompanyName,
                          aa.Id,Roles=string.Join(",",UserManager.GetRoles(aa.LuUserId).ToList())
                      }).OrderBy(p => p.FullName).ToList();
            return Ok(xx);
        }

        // GET: api/LUsers/5
        [ResponseType(typeof(LUser))]
        public async Task<IHttpActionResult> GetLUser(int id, string UserName, string Workflow)
        {
            var Users = db.LUsers.Find(id);
            var UserId=Users.LuUserId;
            var Company = db.GCompanies.Where(p => p.Id == Users.WFCompanyId).FirstOrDefault();
            // var Roles = string.Join(",", UserManager.GetRoles(UserId).ToList());
            //change method to get roles with getting roles with portfolio associated with it
            var RolesArray = UserManager.GetRoles(UserId).OrderBy(x=>x).ToList();
            var MepPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityId == id).Where(p => p.MepEntityType == "LUsers").Include(p=>p.LPortfolio).Include(p=>p.LPortfolio.RChannel).ToList();
            var RolesPortfolioString = "";
            var RolesPortfolioNamesString = "";
            foreach (var role in RolesArray)
            {
                RolesPortfolioString += role + ",";
                RolesPortfolioNamesString += role + ",";
                var RoleId = db.AspNetRoles.Where(p => p.Name == role).Where(p => p.CompanyCode == Company.GcCode).FirstOrDefault();
                if(RoleId!=null)
                {
                    RolesPortfolioString += string.Join(",",MepPortfolios.Where(p => p.MepRoleId == RoleId.Id).Select(p=>p.MepPortfolioId))+"|";
                    RolesPortfolioNamesString += string.Join(",", MepPortfolios.Where(p => p.MepRoleId == RoleId.Id).Select(p => p.LPortfolio.RChannel.RcPrimaryChannel +"."+p.LPortfolio.LpBusinessUnit+"."+p.LPortfolio.RChannel.RcName)) + "|";
                }
            }
            RolesPortfolioString = RolesPortfolioString.TrimEnd('|');
            //but payee is inactive, do not chose
            var PayeeRecord = db.LPayees.Where(p => p.LpUserId == Users.LuUserId).Where(p=>p.WFStatus == "Completed").FirstOrDefault();
            if (PayeeRecord == null)
            {
                var LUser = (from aa in db.LUsers.Where(p => p.Id == id).Include(c => c.GCompany).ToList()
                             join bb in db.AspNetUsers on aa.LuCreatedById equals bb.Id
                             join cc in db.AspNetUsers on aa.LuUpdatedById equals cc.Id
                             //      join pl in db.LPayees
                             //on aa.LuUserId equals pl.LpUserId into pp
                             //      from pl in pp.DefaultIfEmpty()
                             select new
                             {
                                 aa.WFComments,
                                 aa.WFCompanyId,
                                 aa.WFAnalystId,
                                 aa.WFCurrentOwnerId,
                                 aa.WFManagerId,
                                 aa.WFOrdinal,
                                 aa.WFRequesterId,
                                 aa.WFRequesterRoleId,
                                 aa.WFStatus,
                                 aa.WFType,
                                 aa.LuFirstName,
                                 aa.LuLastName,
                                 aa.LuIsAlteryxUser,
                                 FullName = aa.LuFirstName + " " + aa.LuLastName,
                                 aa.Id,
                                 aa.LuUserId,
                                 aa.LuPhone,
                                 aa.LuStatus,
                                 aa.LuIsManager,
                                 aa.LuUpdatedById,
                                 aa.LuUpdatedDateTime,
                                 aa.LuReportsToId,
                                 UpdatedBy = cc.UserName,
                                 CreatedBy = bb.UserName,
                                 aa.LuCreatedById,
                                 aa.LuCreatedDateTime,
                                 aa.LuBlockNotification,
                                 aa.GCompany.GcCompanyName,
                                 aa.LuEmail,
                                 aa.LuCompanyId,
                                 RoleList = RolesPortfolioNamesString,
                                 RoleBasedPortfolios = RolesPortfolioString,
                                 aa.IsSuperUser,
                                 aa.LuCreateLogin
                             }).FirstOrDefault();
                if (LUser == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "USER")));
                }
                return Ok(LUser);
            }
            else
            {
                var LUser = (from aa in db.LUsers.Where(p => p.Id == id).Include(c => c.GCompany).ToList()
                             join bb in db.AspNetUsers on aa.LuCreatedById equals bb.Id
                             join cc in db.AspNetUsers on aa.LuUpdatedById equals cc.Id
                             select new
                             {
                                 aa.WFComments,
                                 aa.WFCompanyId,
                                 aa.WFAnalystId,
                                 aa.WFCurrentOwnerId,
                                 aa.WFManagerId,
                                 aa.WFOrdinal,
                                 aa.WFRequesterId,
                                 aa.WFRequesterRoleId,
                                 aa.WFStatus,
                                 aa.WFType,
                                 aa.LuFirstName,
                                 aa.LuLastName,
                                 aa.LuIsAlteryxUser,
                                 FullName = aa.LuFirstName + " " + aa.LuLastName,
                                 aa.Id,
                                 aa.LuUserId,
                                 aa.LuPhone,
                                 aa.LuStatus,
                                 aa.LuIsManager,
                                 aa.LuUpdatedById,
                                 aa.LuUpdatedDateTime,
                                 aa.LuReportsToId,
                                 UpdatedBy = cc.UserName,
                                 CreatedBy = bb.UserName,
                                 aa.LuCreatedById,
                                 aa.LuCreatedDateTime,
                                 aa.LuBlockNotification,
                                 aa.GCompany.GcCompanyName,
                                 aa.LuEmail,
                                 aa.LuCompanyId,
                                 RoleList = RolesPortfolioNamesString,
                                 RoleBasedPortfolios = RolesPortfolioString,
                                 PayeeId=PayeeRecord.Id,
                                 aa.IsSuperUser
                             }).FirstOrDefault();
                if (LUser == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "USER")));
                }
                return Ok(LUser);

            }
           
        }

        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetEmailVerified(string EmailID, int CompanyID, string radioValue)
        {
            if (radioValue.Trim() == "FinopsUser")
            {
                var Users = db.LUsers.Where(x => x.LuEmail == EmailID & x.WFCompanyId == CompanyID).FirstOrDefault();
                if (Users == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("Not Found in Opco", "USER")));
                }
                else
                {
                    var Users1 = db.LUsers.Where(x => x.LuEmail == EmailID & x.WFCompanyId == CompanyID & (x.WFStatus.Contains("Completed") || x.WFStatus.Contains("Suspended"))).FirstOrDefault();
                    if (Users1 != null)
                    {
                        return Ok(Users1.Id);
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.SeeOther, string.Format("User Not Active", "USER")));
                    }
                }
            }
            else
            {
                var Payees = db.LPayees.Where(x => x.LpEmail == EmailID & x.WFCompanyId == CompanyID).FirstOrDefault();
                if (Payees == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("Not Found in Opco", "USER")));
                }
                else
                {
                    
                    var Payees1 = db.LPayees.Where(x => x.LpEmail == EmailID & x.WFCompanyId == CompanyID & (x.WFStatus.Contains("Completed") || x.WFStatus.Contains("Suspended"))).FirstOrDefault();
                    if (Payees1 == null)
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.SeeOther, string.Format("User Not Active", "USER")));
                    }
                    else
                    {
                        return Ok(Payees1.Id);
                    }
                }
            }
            //var Users = db.LUsers.Where(x => x.LuEmail == EmailID & x.WFCompanyId == CompanyID).FirstOrDefault();
            //if (Users == null)
            //{
            //    var Payees = db.LPayees.Where(x => x.LpEmail == EmailID & x.WFCompanyId == CompanyID).FirstOrDefault();
            //    if (Payees == null)
            //    {
            //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("Not Found in Opco", "USER")));
            //    }
            //    else
            //    {
            //        var Payees1 = db.LPayees.Where(x => x.LpEmail == EmailID & x.WFCompanyId == CompanyID & (x.WFStatus.Contains("Completed") || x.WFStatus.Contains("Suspended"))).FirstOrDefault();
            //        if (Payees1 == null)
            //        {
            //            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.SeeOther, string.Format("User Not Active", "USER")));
            //        }
            //        else
            //        {
            //            return Ok(Payees1.Id);
            //        }
            //    }               
            //}
            

            //var Users1 = db.LUsers.Where(x => x.LuEmail == EmailID & x.WFCompanyId == CompanyID & (x.WFStatus.Contains("Completed") || x.WFStatus.Contains("Suspended"))).FirstOrDefault();
            //if (Users1 == null)
            //{
            //    var Payees1 = db.LPayees.Where(x => x.LpEmail == EmailID & x.WFCompanyId == CompanyID & (x.WFStatus.Contains("Completed") || x.WFStatus.Contains("Suspended"))).FirstOrDefault();
            //    if (Payees1 == null)
            //    {
            //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.SeeOther, string.Format("User Not Active", "USER")));
            //    }
            //    else
            //    {
            //        return Ok(Payees1.Id);
            //    }

            //}
            //else
            //{
            //    return Ok(Users1.Id);
            //}
       }

        [ResponseType(typeof(string))]
        [HttpGet]
        public async Task<IHttpActionResult> SavePortfolioedForAllRolesByUserEmailID(string EmailID, int CompanyID, string radioValue) // 2.8.2
        {
            var EntityId = 0;
            var EntityType = string.Empty;
            var UserID = string.Empty;
            var GCompaniesData = db.GCompanies.Where(x => x.Id == CompanyID).FirstOrDefault();
            if(radioValue.Trim() == "FinopsUser")
            {
                var Users = db.LUsers.Where(x => x.LuEmail == EmailID & x.WFCompanyId == CompanyID).FirstOrDefault();
                EntityId = Users.Id;
                EntityType = "LUsers";
                UserID = Users.LuUserId;
            }
            else
            {
                var Payees = db.LPayees.Where(x => x.LpEmail == EmailID & x.WFCompanyId == CompanyID).FirstOrDefault();
                EntityType = "LPayees";
                EntityId = Payees.Id;
                UserID = Payees.LpUserId;
            }

            Globals.GiveAllPortfolios(EmailID, EntityType, 0);

                //var ExcludedRoles = new string[] { "" };
                //var ExistingRoles = UserManager.GetRoles(Convert.ToString(UserID));
                //var RoleList = (from aa in db.AspNetRoles.Where(p => ExcludedRoles.Contains(p.Name) == false).Where(p => p.CompanyCode == GCompaniesData.GcCode).Where(p => ExistingRoles.Contains(p.Name) == true) select new { aa.Id, aa.Name });
                //foreach (var role in RoleList)
                //{
                //    // var PortfolioList = db.MEntityPortfolios.Where(p => p.LPortfolio.LpIsActive == true).Where(p => p.MepEntityId.Equals(EntityId)).Where(p => p.MepRoleId == role.Id).Select(p => new { p.LPortfolio.Id }).ToList();
                //    var PortfolioList = db.MEntityPortfolios.Where(p => p.LPortfolio.LpIsActive == true).Where(p => p.MepEntityId.Equals(EntityId)).Where(p => p.MepRoleId == role.Id).Select(p => new { p.LPortfolio.Id }).ToList();
                //    // var LPortfolios = db.LPortfolios.Where(p => p.LpIsActive == true).Where(p => p.LpCompanyId == GCompaniesData.Id).Include(p => p.RChannel).Select(p => new { p.Id, p.LpBusinessUnit, p.RChannel.RcPrimaryChannel, p.RChannel.RcName }).OrderBy(p => p.RcPrimaryChannel).ThenBy(p => p.LpBusinessUnit).ThenBy(p => p.RcName);
                //    var LPortfolios = db.LPortfolios.Where(p => p.LpIsActive == true).Where(p => p.LpCompanyId == GCompaniesData.Id).Select(p => new { p.Id }).ToList();
                //    var result = LPortfolios.Where(p => !PortfolioList.Any(l => p.Id == l.Id)).ToList(); ;
                //    foreach (var LPortfo in result) //2.8.2
                //    {
                //        var MentityPortfolio = new MEntityPortfolio { MepEntityId = EntityId, MepEntityType = EntityType, MepPortfolioId = LPortfo.Id, MepRoleId = role.Id.ToString() };
                //        db.MEntityPortfolios.Add(MentityPortfolio);

                //    }
                //}
                //db.SaveChanges();     
                return Ok("");
        }

        public string GetUserRoleIdbyRoleName(string rolename, string CompanyCode)
        {
            //var roleid = (from aa in db.AspNetRoles.Where(p => p.CompanyCode == companycode).Where(p => p.Name == rolename)
            //              select new { aa.Id });

            //return (roleid);
            var id = db.AspNetRoles.Where(p => p.CompanyCode == CompanyCode).Where(p => p.Name == rolename).Select(p => p.Id).FirstOrDefault();


            //var xx = (from aa in db.AspNetRoles.Where(p => p.CompanyCode == CompanyCode).Where(p => p.Name == rolename)

            //          select new { aa.Id }).FirstOrDefault();
            return id;



        }
        // PUT: api/LUsers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLUser(int id, LUser LUser, string Roles, string Atachments, string PortfolioList, string AttachmentPath, string UserName, string Workflow,string RoleBasedPortfolios)
        {

            if (id != LUser.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "USER")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
            {
                    var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (!string.IsNullOrEmpty(Atachments))
                {
                    var Atachment = Atachments.Split(',').ToList();
                    //var Uploadedfile = db.LUploadedFiles.Where(p => p.LufUserId == LUser.Id).FirstOrDefault();
                    foreach (var file in Atachment)
                    {
                        var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = LUser.LuCreatedById, LsdUpdatedById = LUser.LuUpdatedById, LsdFileName = file, LsdFilePath = AttachmentPath, LsdEntityType = "LUsers", LsdEntityId = LUser.Id, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedDateTime = DateTime.UtcNow };
                        db.LSupportingDocuments.Add(LSupportingDocuments);
                        db.SaveChanges();
                    }
                }
                var Company = db.GCompanies.Find(LUser.LuCompanyId);

                if (!string.IsNullOrEmpty(Roles))
                {
                    var RoleList = Roles.Split(',').ToList();//Update user roles
                                                             //var ExistingRoles= UserManager.GetRoles(LUser.LuUserId).ToArray();
                                                             //  UserManager.RemoveFromRoles(LUser.LuUserId,ExistingRoles);
                    db.Database.ExecuteSqlCommand("delete from AspNetUserRoles where UserId ={0}",LUser.LuUserId);
                    db.Database.ExecuteSqlCommand("delete from MEntityPortfolios where MepEntityType='LUsers' and MepEntityId="+LUser.Id);
                        foreach (var role in RoleList)
                    {
                        db.Database.ExecuteSqlCommand("insert into AspNetUserRoles(UserId,RoleId) values({0},(select Id from AspNetRoles where Name={1} and CompanyCode={2}))", LUser.LuUserId, role, Company.GcCode);
                            var RoleId = db.AspNetRoles.Where(p => p.Name == role).Where(p => p.CompanyCode == Company.GcCode).FirstOrDefault().Id;
                           
                                if (!string.IsNullOrEmpty(RoleBasedPortfolios))
                                {
                                    var PortfolioArray = RoleBasedPortfolios.Split('|').ToList();
                                    foreach (var Pf in PortfolioArray)
                                    {
                                        if (!string.IsNullOrEmpty(Pf))
                                        {
                                            var RolePortfolioList = Pf.Split(',');
                                            if (RolePortfolioList[0] == role)
                                            {
                                                for (int i = 1; i < RolePortfolioList.Count(); i++)
                                                {

                                                    var PortfolioId = Convert.ToInt32(RolePortfolioList[i]);
                                                    var MentityPortfolio = new MEntityPortfolio { MepEntityId = LUser.Id, MepEntityType = "LUsers", MepPortfolioId = PortfolioId, MepRoleId = RoleId };
                                                    db.MEntityPortfolios.Add(MentityPortfolio);
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(PortfolioList))
                                {
                                    
                                        var PortfolioArray = PortfolioList.Split(',').ToList();
                                        foreach (var portfolio in PortfolioArray)
                                        {
                                            var PortfolioId = Convert.ToInt32(portfolio);
                                            var MentityPortfolio = new MEntityPortfolio { MepEntityId = LUser.Id, MepEntityType = "LUsers", MepPortfolioId = PortfolioId, MepRoleId = RoleId };
                                            db.MEntityPortfolios.Add(MentityPortfolio);
                                            db.SaveChanges();
                                        }
                                    }

                            
                        }
                }

                    //If Super User Call then add all Portfolio for that all roles
                    
                    // UserManager.AddToRoles(LUser.LuUserId,RoleList);
                    db.Entry(LUser).State = EntityState.Modified;
                await db.SaveChangesAsync();

                    transaction.Commit();
                    if (LUser.IsSuperUser == true)
                    {
                        Globals.GiveAllPortfolios(LUser.LuEmail, "LUsers", 0);
                    }
                    //Add Entry in Audit Log
                    Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                           "Create", LUser.LuCreatedById, DateTime.UtcNow, LUser.WFStatus, LUser.WFStatus,
                          WFDetails.RwfBaseTableName, LUser.Id, Convert.ToString(LUser.LuFirstName + " " + LUser.LuLastName), WFDetails.Id, LUser.WFCompanyId, string.Empty, LUser.WFRequesterRoleId, null);
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
            return StatusCode(HttpStatusCode.NoContent);
        }

        //public async Task<IHttpActionResult> PutUpdateUserStatus(LUser LUser)//This method will update status of user with appending comments
        //{
        //    try
        //    {
        //        var User = db.LUsers.Find(LUser.Id);
        //        User.LuStatus = LUser.LuStatus;
        //        User.LuUpdatedById = LUser.LuUpdatedById;
        //        User.LuUpdatedDateTime = DateTime.UtcNow;
        //        if (!string.IsNullOrEmpty(LUser.WFComments))
        //        {
        //            User.WFComments = LUser.WFComments+Environment.NewLine+User.WFComments;
        //        }
        //        db.Entry(User).State = EntityState.Modified;
        //        await db.SaveChangesAsync();

               
        //        //Make user login active only if user is approved by the manager
        //        if (LUser.LuStatus.Equals("Active"))
        //        {
        //            var AspUser = db.AspNetUsers.Find(User.LuUserId);
        //            AspUser.IsActive = true;
        //            db.Entry(AspUser).State = EntityState.Modified;
        //            await db.SaveChangesAsync();
        //            //if(User.LuBlockNotification)//If block notification is set to true then user will not receive email instead created by user will receive email
        //            //{
        //            //    var CreatedByEmail = db.AspNetUsers.Where(p => p.Id == User.LuCreatedById).FirstOrDefault().UserName;
        //            //    //Send Welcome Email to Created By User
        //            //    Globals.SendEmail(CreatedByEmail, null, "Vodafone LITE", "This is to inform you that You are approved as a User in Vodafone Lite . You can Login in to application from thid emaila address as your username . Please find your password in next mail");
        //            //    Globals.SendEmail(CreatedByEmail, null, "Vodafone LITE", "Your Vodafone LITE Password is " + ConfigurationManager.AppSettings["DefaultUserPassword"]);
        //            //}
        //            //else
        //            //{
        //            //    //Send Welcome Email to Approved User
        //            //    Globals.SendEmail(User.LuEmail, null, "Vodafone LITE", "This is to inform you that You are approved as a User in Vodafone Lite . You can Login in to application from thid emaila address as your username . Please find your password in next mail");
        //            //    Globals.SendEmail(User.LuEmail, null, "Vodafone LITE", "Your Vodafone LITE Password is " + ConfigurationManager.AppSettings["DefaultUserPassword"]);
        //            //}
                  
        //        }
        //    }
        //    catch (DbEntityValidationException e)
        //    {
        //        transaction.Rollback();
        //        string ErrorMessage = null;
        //        foreach (var eve in e.EntityValidationErrors)
        //        {
        //            //ErrorMessage += string.Format("Entity of type {0} in state {1} has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                ErrorMessage += ve.ErrorMessage + Environment.NewLine;//string.Format("- Property: {0}, Error: {1}", ve.PropertyName, ve.ErrorMessage);
        //            }
        //        }
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ErrorMessage));
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction.Rollback();

        //        if (ex.GetBaseException().GetType() == typeof(SqlException))//check for database exception type
        //        {
        //            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
        //        }
        //        else
        //        {
        //            throw ex;
        //        }

        //    }
        //    return Ok();
        //}

        // POST: api/LUsers
        [ResponseType(typeof(LUser))]
        public async Task<IHttpActionResult> PostLUser(PostLUserViewModel LUser,string LoggedInRoleId, string UserName, string Workflow, string AttachmentPath,bool SamePortfoliosForAllRoles,bool CheckDuplicateUser,string Source,string UserLobbyId)
        {
            //if (!ModelState.IsValid)
            //{
            //    //return BadRequest(ModelState);
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "USER")));
            //}
            //Shivani Changes for WIAM integration. 09 May 2019
            bool CreateLogin = Convert.ToBoolean(LUser.LuCreateLogin);

            var LuserModel = new LUser();
            var Company = db.GCompanies.Find(LUser.LuCompanyId);
            bool IsLobbyUser = false;
            if (!string.IsNullOrEmpty(Source) && "Lobby".Equals(Source))
            {
                IsLobbyUser = true;
            }
            if (CheckDuplicateUser)
            {
                /*Logic for checking duplicate User*/
                var DuplicateUser = db.LUsers.Where(p => p.LuEmail == LUser.LuEmail && (p.WFStatus.Equals("InActive", StringComparison.OrdinalIgnoreCase) || p.WFStatus.Equals("Rejected", StringComparison.OrdinalIgnoreCase))).FirstOrDefault();
                var DuplicateOtherOpcoUser = db.LUsers.Where(p => p.LuEmail == LUser.LuEmail && p.LuCompanyId != LUser.LuCompanyId).FirstOrDefault();
                var DuplicateUserCompleted = db.LUsers.Where(p => p.LuEmail == LUser.LuEmail && (p.WFStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) || p.WFStatus.Equals("InProgress", StringComparison.OrdinalIgnoreCase))).FirstOrDefault();

                var DuplicatePayeeCompleted = db.LPayees.Where(p => p.LpEmail == LUser.LuEmail && (p.WFStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) || p.WFStatus.Equals("InProgress", StringComparison.OrdinalIgnoreCase))).FirstOrDefault();

                if (DuplicateUser != null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, "User by same email already exist as InActive, do you want to re-use same user record?"));
                }
                if (DuplicateOtherOpcoUser != null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, "User by same email id already exist in another opco.If you want to use in this opco then kindly delete user from the another opco."));
                }
                if (DuplicateUserCompleted != null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, "User already exists in the system."));
                }

                if (DuplicatePayeeCompleted != null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, "Payee already exists in the system."));
                }
            }
            else
            {
                LuserModel = db.LUsers.Where(p => p.LuEmail == LUser.LuEmail).FirstOrDefault();
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var WFStatus = string.IsNullOrEmpty(LUser.WFStatus) ? "Saved" : LUser.WFStatus;
                    var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    //Note: Calculate Ordinal based on the Current Role who has created User based on RoleId and Opco and WorkflowName
                    var ConfigData = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LUser.LuCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault();
                    var Ordinal = 0; //setting Ordinal to default 0 value.
                string AnalystId = null;
                    bool IsActive = false;
                    if (ConfigData != null)//Checking if ConfigData is not null and set Ordinal/AnalystId
                {
                    Ordinal = ConfigData.LwfcOrdinalNumber;
                    if (ConfigData.LwfcActingAs == "Analyst")
                    {
                        AnalystId = LUser.WFCurrentOwnerId;
                    }
                }
                    if (LUser.LuStatus.Equals("Active"))//This Status will be set when user will be created by L2
                    {
                        //Change Ordinal to max one
                        //SS, also when a user is created by L2_Admin, it's ordinal should be set to max ordinal for user workflow, instead of 0
                        int MaxOrdinal= db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LUser.LuCompanyId).Where(p => p.RWorkFlow.RwfName == Workflow).Max(p=>p.LwfcOrdinalNumber);
                        Ordinal = MaxOrdinal;
                        IsActive = true;
                    }

                    /*Dup Payee and User check with existing payee/user with WFStatus=Saved, InProgress, Completed*/
                    var DeniedStatus = "Saved,InProgress,Completed".Split(',').ToList();
                    var ExistingPayee = db.LPayees.Where(p => p.LpEmail == LUser.LuEmail && DeniedStatus.Contains(p.WFStatus)).FirstOrDefault();
                    if (ExistingPayee != null)
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, "Cannot Insert Duplicate Email"));//type 2 error
                    }
                    if (LuserModel.Id > 0)
                    {
                        LuserModel.WFAnalystId = AnalystId;
                        LuserModel.WFCurrentOwnerId = LUser.WFCurrentOwnerId;
                            LuserModel.WFRequesterRoleId = LoggedInRoleId;
                            LuserModel.WFType = WFDetails.RwfWFType;
                            LuserModel.WFRequesterId = LUser.WFRequesterId;
                            LuserModel.WFOrdinal = Ordinal;
                            LuserModel.WFComments = LUser.WFComments;
                            LuserModel.WFCompanyId = LUser.WFCompanyId;
                            LuserModel.WFStatus = WFStatus;
                            LuserModel.LuBlockNotification = LUser.LuBlockNotification;
                            LuserModel.LuFirstName = LUser.LuFirstName;
                            LuserModel.LuLastName = LUser.LuLastName;
                            LuserModel.LuEmail = LUser.LuEmail;
                            LuserModel.LuPhone = LUser.LuPhone;
                            LuserModel.LuIsAlteryxUser = false;
                            LuserModel.LuCreatedById = LUser.LuCreatedById;
                            LuserModel.LuCreatedDateTime = LUser.LuCreatedDateTime;
                            LuserModel.LuUpdatedById = LUser.LuUpdatedById;
                            LuserModel.LuUpdatedDateTime = LUser.LuUpdatedDateTime;
                            LuserModel.LuStatus = LUser.LuStatus;
                            LuserModel.LuCompanyId = LUser.LuCompanyId;
                            LuserModel.LuBand = LUser.LuBand;
                            LuserModel.LuIsManager = false;
                            LuserModel.LuReportsToId = LUser.LuReportsToId;
                        LuserModel.WFAnalystId = null;
                        LuserModel.WFManagerId = null;
                        LuserModel.IsSuperUser = LUser.IsSuperUser;
                        db.Entry(LuserModel).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        //delete existing roles and Portfolios
                        var ExistingPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityType == "LUsers").Where(p => p.MepEntityId == LuserModel.Id).AsEnumerable();
                        db.MEntityPortfolios.RemoveRange(ExistingPortfolios);
                       await db.SaveChangesAsync();
                       db.Database.ExecuteSqlCommand("delete from AspnetUserRoles where UserId={0}",LuserModel.LuUserId);
                        db.SaveChanges();
                        //21August2019 - ShivaniG
                        //delete Security questions and pasword history
                        var SecurityQues = db.MAspnetUsersGSecurityQuestions.Where(p => p.MAuqsqUserId == LuserModel.LuUserId).AsEnumerable();
                        db.MAspnetUsersGSecurityQuestions.RemoveRange(SecurityQues);
                        await db.SaveChangesAsync();
                        var PwdHistory = db.LPasswordHistories.Where(p => p.UserId == LuserModel.LuUserId).AsEnumerable();
                        db.LPasswordHistories.RemoveRange(PwdHistory);
                        await db.SaveChangesAsync();
                        db.Database.ExecuteSqlCommand("Update  XSchema" + Company.GcCode.ToUpper() + ".XReportUsers set  XUserEmailID = XUserEmailID + 'X' where XUserEmailID='" + LuserModel.LuEmail + "'");
                        //update acceptPolicies
                        var aspnetuser = db.AspNetUsers.Where(p => p.Email == LuserModel.LuEmail).FirstOrDefault();
                        aspnetuser.PolicyAccepted = false;
                        aspnetuser.GcCompanyId = LuserModel.LuCompanyId;
                        aspnetuser.IsActive = true;
                        db.Entry(aspnetuser).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        //Add Entry in Audit Log
                        Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                               "Create", LuserModel.LuCreatedById, DateTime.UtcNow, LuserModel.WFStatus, LuserModel.WFStatus,
                              WFDetails.RwfBaseTableName, LuserModel.Id,
                              LuserModel.LuEmail, //Convert.ToString(LuserModel.LuFirstName+" "+LuserModel.LuLastName), 
                              WFDetails.Id, LuserModel.WFCompanyId, string.Empty, LuserModel.WFRequesterRoleId, null);
                    }
                    else {
                        LuserModel = new LUser
                        {
                            WFAnalystId = AnalystId,
                            WFCurrentOwnerId = LUser.WFCurrentOwnerId,
                            WFRequesterRoleId = LoggedInRoleId,
                            WFType = WFDetails.RwfWFType,
                            WFRequesterId = LUser.WFRequesterId,
                            WFOrdinal = Ordinal,
                            WFComments = LUser.WFComments,
                            WFCompanyId = LUser.WFCompanyId,
                            WFStatus = WFStatus,
                            LuBlockNotification = LUser.LuBlockNotification,
                            LuFirstName = LUser.LuFirstName,
                            LuLastName = LUser.LuLastName,
                            LuEmail = LUser.LuEmail,
                            LuPhone = LUser.LuPhone,
                            LuIsAlteryxUser = false,
                            LuCreatedById = LUser.LuCreatedById,
                            LuCreatedDateTime = LUser.LuCreatedDateTime,
                            LuUpdatedById = LUser.LuUpdatedById,
                            LuUpdatedDateTime = LUser.LuUpdatedDateTime,
                            LuStatus = LUser.LuStatus,
                            LuCompanyId = LUser.LuCompanyId,
                            LuBand = LUser.LuBand,
                            LuIsManager = false,
                            LuReportsToId = LUser.LuReportsToId,
                            LuCreateLogin = CreateLogin,
                            LuCreatedByForm = true,
                            IsSuperUser = LUser.IsSuperUser
                            //added by RS for saving 1 in LuCreated by form during create user in table as per requirement
                        };//IsManager has been not used thus passing hardcode value for it
                        db.LUsers.Add(LuserModel);
                        await db.SaveChangesAsync();
                        bool NewAspnetUser = true;
                        //Update Status in Lobby
                        if (!string.IsNullOrEmpty(Source) && "Lobby".Equals(Source))
                        {
                            int iUserLobbyId = Convert.ToInt32(UserLobbyId);
                            db.Database.ExecuteSqlCommand("update LUserLobby set status='Accepted', UpdatedDateTime='" + DateTime.UtcNow + "', UpdatedBy='" + UserName + "',UpdatedByRoleId=" + LoggedInRoleId + " where Id = " + iUserLobbyId);
                            await db.SaveChangesAsync();
                        }
                        //rehire case, we should set aspnetuser.isactive to = 1 r3.1,cleanup existing roles,portfolios,pwd history,security questions
                        {
                            db.Database.ExecuteSqlCommand("update AspNetUsers set IsActive= 1,GcCompanyId ={1},PolicyAccepted = 0  where Email = {0}", LuserModel.LuEmail, LuserModel.LuCompanyId);
                            await db.SaveChangesAsync();
                           
                            var AspNetUser = db.AspNetUsers.Where(a => a.Email == LuserModel.LuEmail).FirstOrDefault();
                            if (AspNetUser != null)
                            {
                                NewAspnetUser = false; //rehire case, then no need to make entry. This variable is used to avoid checking into AspNetSuers table again which was causing exception may be due to deadlock
                                var ExistingRoles = db.AspNetUserRoles.Where(p => p.UserId == AspNetUser.Id).ToList();
                                db.AspNetUserRoles.RemoveRange(ExistingRoles);
                                await db.SaveChangesAsync();
                                //Get security questions
                                var questions = db.MAspnetUsersGSecurityQuestions.Where(a => a.MAuqsqUserId == AspNetUser.Id).ToList();
                                db.MAspnetUsersGSecurityQuestions.RemoveRange(questions);
                                await db.SaveChangesAsync();
                                //Get Password History
                                var PwdHistory = db.LPasswordHistories.Where(a => a.UserId == AspNetUser.Id).ToList();
                                db.LPasswordHistories.RemoveRange(PwdHistory);
                                await db.SaveChangesAsync();
                            }
                            //Get Payees Portfolios or User portfolios for the Email used
                            var PayeeData = db.LPayees.Where(a => a.LpEmail == LuserModel.LuEmail).FirstOrDefault();
                            if(PayeeData != null)
                            {
                                var ExistingPayeePortfolios = db.MEntityPortfolios.Where(a => a.MepEntityId == PayeeData.Id).Where(a => a.MepEntityType == "LPayees").ToList();
                                db.MEntityPortfolios.RemoveRange(ExistingPayeePortfolios);
                                await db.SaveChangesAsync();
                            }
                            var UserData = db.LUsers.Where(a => a.LuEmail == LuserModel.LuEmail).FirstOrDefault();
                            if (UserData != null)
                            {
                                var ExistingUserPortfolios = db.MEntityPortfolios.Where(a => a.MepEntityId == UserData.Id).Where(a => a.MepEntityType == "LUsers").ToList();
                                db.MEntityPortfolios.RemoveRange(ExistingUserPortfolios);
                                await db.SaveChangesAsync();
                            }
                            
                        }

                        //Add Entry in Audit Log
                        Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                               "Create", LuserModel.LuCreatedById, DateTime.UtcNow, LuserModel.WFStatus, LuserModel.WFStatus,
                              WFDetails.RwfBaseTableName, LuserModel.Id, Convert.ToString(LuserModel.LuFirstName + " " + LuserModel.LuLastName), WFDetails.Id, LuserModel.WFCompanyId, string.Empty, LuserModel.WFRequesterRoleId, null);
                        var user = new ApplicationUser() { UserName = LUser.LuEmail, Email = LUser.LuEmail, GcCompanyId = LUser.LuCompanyId, IsActive = IsActive };//Is Active flag is set to false if a user is not approved from manager
                        //Insert only new user, rehire case is excluded which causes deadlock (exception)
                        if (NewAspnetUser)
                        {
                            IdentityResult result = await UserManager.CreateAsync(user, ConfigurationManager.AppSettings["DefaultUserPassword"]);
                        }
                    }
                  
                    //lines added by shubham to add roles to user after registering
                    var RegUser = db.AspNetUsers.Where(p=>p.Email==LuserModel.LuEmail).FirstOrDefault();
                    var AspRoles = db.AspNetRoles.Where(p => p.Id == LoggedInRoleId).FirstOrDefault();

                    if(RegUser != null)
                    {
                        RegUser.GcCompanyId = LUser.WFCompanyId;
                      
                        if (AspRoles != null)
                        {
                            if(AspRoles.Name == "L2 Admin")
                            {
                                RegUser.IsActive = true;
                                RegUser.PolicyAccepted = false;
                            }
                        }
                        db.Entry(RegUser).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    foreach (var role in LUser.Roles)
                    {
                        db.Database.ExecuteSqlCommand("insert into AspNetUserRoles(UserId,RoleId) values({0},(select Id from AspNetRoles where Name={1} and CompanyCode={2}))", RegUser.Id, role, Company.GcCode);
                        //UserManager.AddToRole(RegUser.Id, role);//user manager method has stopped working since opco column has beed added in aspnet roles
                        ////add portfolio to user 
                        var RoleId = db.AspNetRoles.Where(p => p.Name == role).Where(p => p.CompanyCode == Company.GcCode).FirstOrDefault().Id;
                        if (SamePortfoliosForAllRoles==false)
                        {
                            var PortfolioArray = LUser.ParameterCarrier.Split('|').ToList();
                            foreach (var Pf in PortfolioArray)
                            {
                                if (!string.IsNullOrEmpty(Pf))
                                {
                                    var RolePortfolioList = Pf.Split(',');
                                    if (RolePortfolioList[0] == role)
                                    {
                                        for (int i = 1; i < RolePortfolioList.Count(); i++)
                                        {

                                            var PortfolioId = Convert.ToInt32(RolePortfolioList[i]);
                                            var MentityPortfolio = new MEntityPortfolio { MepEntityId = LuserModel.Id, MepEntityType = "LUsers", MepPortfolioId = PortfolioId, MepRoleId = RoleId };
                                            db.MEntityPortfolios.Add(MentityPortfolio);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        else //if (!string.IsNullOrEmpty(PortfolioList))
                        {
                            
                                var PortfolioArray = LUser.ParameterCarrier.Split(',').ToList();
                                foreach (var portfolio in PortfolioArray)
                                {
                                    var PortfolioId = Convert.ToInt32(portfolio);
                                    var MentityPortfolio = new MEntityPortfolio { MepEntityId = LuserModel.Id, MepEntityType = "LUsers", MepPortfolioId = PortfolioId, MepRoleId = RoleId };
                                    db.MEntityPortfolios.Add(MentityPortfolio);
                                    db.SaveChanges();
                                }
                            
                        }

                        
                    }
                    if (string.IsNullOrEmpty(LuserModel.LuUserId))
                    {
                        //Update User Id in Lusers table
                        LuserModel.LuUserId = RegUser.Id;
                        db.Entry(LuserModel).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                  

                    if (!string.IsNullOrEmpty(LUser.FileNames))
                    {
                        //Add files in LSupporting Documents
                        var FilesArray = LUser.FileNames.Split(',').ToList();
                        foreach (var file in FilesArray)
                        {
                            var LSupportingDocuments =new LSupportingDocument{LsdCreatedById=LuserModel.LuCreatedById,LsdUpdatedById=LuserModel.LuUpdatedById,LsdFileName= file,LsdFilePath=AttachmentPath,LsdEntityType="LUsers",LsdEntityId=LuserModel.Id,LsdCreatedDateTime=DateTime.UtcNow,LsdUpdatedDateTime=DateTime.UtcNow};
                            db.LSupportingDocuments.Add(LSupportingDocuments);
                            db.SaveChanges();
                            }
                        }

                        transaction.Commit();

                    //If super User the add all portfolio for mention roles.
                    if (LuserModel.IsSuperUser == true)
                    {
                        Globals.GiveAllPortfolios(LuserModel.LuEmail, "LUsers", 0);
                    }
                }
                //catch (DbEntityValidationException dbex)
                //{
                //    transaction.Rollback();
                //    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                //    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                //}
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    //{
                    //    //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                    //    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                    //}
                    //else
                    {
                       // Globals.SendEmail("ssharma@megacube.com.au", null, "LUsers-Create", ex.Message+Environment.NewLine+ex.StackTrace+ex.InnerException, "QA");
                        throw ex;//This exception will be handled in FilterConfig's CustomHandler
                    }
                }
            }
            return Ok(LuserModel.Id);
        }

        [ResponseType(typeof(void))]      
        public async Task<IHttpActionResult> SavePortfolios( string SelectedPortfolios, int UserId, int RoleId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(SelectedPortfolios))
                    {
                        var PortfolioArray = SelectedPortfolios.Split(',').ToList();
                        foreach (var portfolio in PortfolioArray)
                        {
                            var PortfolioId = Convert.ToInt32(portfolio);
                            var MentityPortfolio = new MEntityPortfolio { MepEntityId = UserId, MepEntityType = "LUsers", MepPortfolioId = PortfolioId, MepRoleId = RoleId.ToString() };
                            db.MEntityPortfolios.Add(MentityPortfolio);
                            db.SaveChanges();
                        }
                    }
                    await db.SaveChangesAsync();
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
                transaction.Commit();
            }
            return StatusCode(HttpStatusCode.NoContent);




        }

        // DELETE: api/LUsers/5
        //[ResponseType(typeof(LUser))]
        public async Task<IHttpActionResult> DeleteLUser(int Id, string UserName, string Workflow,string Comments)
        {
            LUser LUser = await db.LUsers.FindAsync(Id);
            if (LUser == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "USER")));
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
                    var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();

                    //code to delete LUser record
                    var userId = LUser.LuUserId;
                    LUser.WFStatus = "InActive";
                    LUser.WFComments = Comments;
                    db.Entry(LUser).State = EntityState.Modified;
                    //db.LUsers.Remove(LUser);
                    await db.SaveChangesAsync();
                    //code to delete user roles
                    db.Database.ExecuteSqlCommand("delete from AspNetUserRoles where UserId ='" + LUser.LuUserId + "'");
                    //db.Database.ExecuteSqlCommand("delete from MEntityPortfolios where MepEntityType='LUsers' and MepEntityId ="+LUser.Id);
                    //db.Database.ExecuteSqlCommand("delete from MEntityPortfolios where MepEntityType='LUsers' and MepEntityId =" + LUser.Id);
                    //var ExistingRoles = UserManager.GetRoles(userId).ToArray();
                    //await UserManager.RemoveFromRolesAsync(userId, ExistingRoles);
                    //code to delete aspnet user
                    AspNetUser AspUser = db.AspNetUsers.Find(userId);
                    AspUser.IsActive = false;
                    db.Entry(AspUser).State = EntityState.Modified;
                    // db.AspNetUsers.Remove(AspUser);
                    await db.SaveChangesAsync();
                    var Portfolios = db.MEntityPortfolios.Where(p => p.MepEntityType == "LUsers").Where(p => p.MepEntityId == Id);
                    db.MEntityPortfolios.RemoveRange(Portfolios);
                    await db.SaveChangesAsync();
                    //Added by Sachin- Remove the user AD Account
                    if (ProjectEnviournment.Equals("Prod", StringComparison.OrdinalIgnoreCase))
                    {
                        ADUserViewModel ADmodel = new ADUserViewModel();
                        ADmodel.Email = AspUser.Email;
                        Globals.DeleteUser(ADmodel);
                    }

                    transaction.Commit();
                    var CompanyDetaild = db.GCompanies.Where(a => a.Id == LUser.LuCompanyId).FirstOrDefault();
                    //R3.1 - Delete User folder from S3
                    if (System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyDetaild.GcCode + "/" + LUser.LuEmail))
                    {
                        System.IO.Directory.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyDetaild.GcCode + "/" + LUser.LuEmail, true);
                    }

                    return Ok();
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
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LUserExists(int id)
        {
            return db.LUsers.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            if (SqEx.Message.IndexOf("FNAllocateCommisionBatch", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "USERS", "DATABASE OBJECTS"));

            if (SqEx.Message.IndexOf("UQ_LUsers_LuEmail", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "USERS", "EMAIL"));
            else
            {
                //callGlobals.ExecuteSPLogError SP here and log SQL SqEx.Message
                //Add complete Url in description
                var UserName = "";//System.Web.HttpContext.Current.Session["UserName"] as string;
                string UrlString = Convert.ToString(Request.RequestUri.AbsolutePath);
                var ErrorDesc = "";
                var Desc = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                if (Desc.Count() > 0)
                    ErrorDesc = string.Join(",", Desc);
                string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3 rd index position
               Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
                //Globals.LogError(SqEx.Message, ErrorDesc);
                return Globals.SomethingElseFailedInDBErrorMessage;
            }
        }

        private Boolean blnCheckTemplateColumns(DataTable dt)
        {
            if (!dt.Columns.Contains("useremailid")) return false;
            if (!dt.Columns.Contains("UserRole")) return false;
            if (!dt.Columns.Contains("PrimaryChannel")) return false;
            if (!dt.Columns.Contains("Channel")) return false;
            if (!dt.Columns.Contains("BusinessUnit")) return false;

            return true;
        }

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
        [HttpPost]
        public IHttpActionResult ValidateUploadUser(string FileName, string UserName, string LoggedInRoleId, int iCompanyId)
        {
            
            DataSet dsErrors = new DataSet();
            string excelConnectionString = string.Empty;
            var CompanyDetails = db.GCompanies.Where(p => p.Id == iCompanyId).FirstOrDefault();
            string S3BucketRootFolder = ConfigurationManager.AppSettings["SOSBucketRootFolder"];
            string S3TargetPath = S3BucketRootFolder + CompanyDetails.GcCode + "/upload/users/" + FileName;
            var bytedata = Globals.DownloadFromS3(S3TargetPath, "");
            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet ds2 = new DataSet();
            DataTable dtdata = null;
            
            try
            {
                string fileExtension = System.IO.Path.GetExtension(FileName);
                string name = System.IO.Path.GetFileNameWithoutExtension(FileName);
                string FileName_New = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + "_UPLOAD" + fileExtension;
                OleDbConnection con = null;
                string path = ConfigurationManager.AppSettings["TempDocumentPath"];
                string fullpath = path + "\\" + FileName_New;
                System.IO.File.WriteAllBytes(fullpath, bytedata);

                //string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
                //SG - 2020/19/02- OLEDB connectionstring will be read from web.config file
                string connectionString = string.Format(ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"].Replace("{0}", fullpath));

                con = new System.Data.OleDb.OleDbConnection(connectionString);
                con.Open();
                OleDbDataAdapter cmd2 = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [UserPortfolios$]", con);
                cmd2.Fill(ds2);
                dtdata = ds2.Tables[0];
                con.Close();
                if (!blnCheckTemplateColumns(dtdata))
                {
                    DataTable dtE = new DataTable();
                    dtE.Columns.Add("ExceptionMessage");
                    dtE.Rows.Add("Invalid Template File, Please download fresh template and use the same for uploading claims.");
                    return Ok(dtE);
                }
                db.Database.ExecuteSqlCommand("delete from TUploadUserPortfolios where WFRequester = '" + UserName.ToString() + "' and WFCompanyId = '" + iCompanyId.ToString() + "'");
                
                #region Prepare data for bulk insert
                System.Data.DataColumn newColumn1 = new System.Data.DataColumn("WFRequesterRoleId", typeof(System.String));
                newColumn1.DefaultValue = LoggedInRoleId.ToString();
                dtdata.Columns.Add(newColumn1);
                System.Data.DataColumn newColumn2 = new System.Data.DataColumn("WFCompanyID", typeof(System.String));
                newColumn2.DefaultValue = iCompanyId.ToString();
                dtdata.Columns.Add(newColumn2);
                System.Data.DataColumn newColumn3 = new System.Data.DataColumn("WFRequester", typeof(System.String));
                newColumn3.DefaultValue = UserName;
                dtdata.Columns.Add(newColumn3);
                System.Data.DataColumn newColumn4 = new System.Data.DataColumn("PortfolioId", typeof(System.String));
                newColumn4.DefaultValue = "0";
                dtdata.Columns.Add(newColumn4);
                System.Data.DataColumn newColumn5 = new System.Data.DataColumn("RoleId", typeof(System.String));
                newColumn5.DefaultValue = "0";
                dtdata.Columns.Add(newColumn5);
                System.Data.DataColumn newColumn6 = new System.Data.DataColumn("ValidationStatus", typeof(System.String));
                newColumn6.DefaultValue = "";
                dtdata.Columns.Add(newColumn6);
                System.Data.DataColumn newColumn7 = new System.Data.DataColumn("ValidationMessage", typeof(System.String));
                newColumn7.DefaultValue = "";
                dtdata.Columns.Add(newColumn7);
                System.Data.SqlClient.SqlBulkCopy sqlBulk = new SqlBulkCopy(db.Database.Connection.ConnectionString);

                dtdata.Rows.RemoveAt(0);//Removing the 1st row of data table as it contains the column alias.

                sqlBulk.ColumnMappings.Add("UserEmailID", "UserEmailID");
                sqlBulk.ColumnMappings.Add("UserRole", "UserRole");
                sqlBulk.ColumnMappings.Add("PrimaryChannel", "PrimaryChannel");
                sqlBulk.ColumnMappings.Add("Channel", "Channel");
                sqlBulk.ColumnMappings.Add("BusinessUnit", "BusinessUnit");
                sqlBulk.ColumnMappings.Add("RoleId", "RoleId");
                sqlBulk.ColumnMappings.Add("PortfolioId", "PortfolioId");
                sqlBulk.ColumnMappings.Add("WFRequesterRoleID", "WFRequesterRoleID");
                sqlBulk.ColumnMappings.Add("WFCompanyID", "WFCompanyID");
                sqlBulk.ColumnMappings.Add("WFRequester", "WFRequester");
                sqlBulk.ColumnMappings.Add("ValidationStatus", "ValidationStatus");
                sqlBulk.ColumnMappings.Add("ValidationMessage", "ValidationMessage");
                sqlBulk.DestinationTableName = "TUploadUserPortfolios";
                #endregion
                try
                {
                    sqlBulk.WriteToServer(dtdata);
                    
                    var Query = "Exec dbo.USPValidateAndInsertUsersUploadData @UserID,@UserRoleID,@CompanyID,@UploadData,@FileName";
                    SqlCommand cmd = new SqlCommand(Query);
                    cmd.Parameters.AddWithValue("@UserID", UserName);
                    cmd.Parameters.AddWithValue("@UserRoleID", LoggedInRoleId.ToString());
                    cmd.Parameters.AddWithValue("@CompanyID", iCompanyId.ToString());
                    cmd.Parameters.AddWithValue("@UploadData", 0);
                    cmd.Parameters.AddWithValue("@FileName", FileName);
                    dsErrors = GetData(cmd);
                   
                    if (dsErrors.Tables.Count > 0)
                    {
                        if (dsErrors.Tables[0].Rows.Count > 0)
                        {
                            return Ok(dsErrors.Tables[0]);
                        }
                        else
                        {
                            return Ok();
                        }
                    }
                    else
                    {
                        return Ok();
                    }

                }
                catch (Exception ex)
                {
                    var models = new GErrorLog { GelUserName = "SOS", GelController = "LUsers", GelMethod = "ValidateUploadUser", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
                    db.GErrorLogs.Add(models);
                    db.SaveChanges();
                    DataTable dtE = new DataTable();
                    dtE.Columns.Add("BulkUploadFailed");
                    return Ok(dtE);
                }
                
            }
            catch (Exception ex)
            {
                
                var models = new GErrorLog { GelUserName = "SOS", GelController = "LUsers", GelMethod = "ValidateUploadUser", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };

                db.GErrorLogs.Add(models);
                db.SaveChanges();
                DataTable dtE = new DataTable();
                dtE.Columns.Add("BulkUploadFailed");
                return Ok(dtE);

            }
           
        }
        public IHttpActionResult UploadUser( string UserName, string LoggedInRoleId, int iCompanyId)
        {
            var Query = "Exec dbo.USPValidateAndInsertUsersUploadData @UserID,@UserRoleID,@CompanyID,@UploadData,@FileName";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@UserID", UserName);
            cmd.Parameters.AddWithValue("@UserRoleID", LoggedInRoleId.ToString());
            cmd.Parameters.AddWithValue("@CompanyID", iCompanyId.ToString());
            cmd.Parameters.AddWithValue("@UploadData", 1);
            cmd.Parameters.AddWithValue("@FileName", "");
            DataSet dsErrors = GetData(cmd);
            if (dsErrors.Tables.Count > 0)
            {
                if (dsErrors.Tables[0].Rows.Count > 0)
                {
                    return Ok(dsErrors.Tables[0]);
                }
                else
                {
                    return Ok();
                }
            }
            else
            {
                return Ok();
            }
        }
        [ResponseType(typeof(string))]
        public string GetMyUsersReportData(string UserHexID, int CompanyID, string CompanyCode, string LoggedInUserName, int LoggedRoleId)
        {
            //columnnames must not be any of following: 'WFComments','CreatedByForm','CreatedById','CreatedDateTime','ParameterCarrier','Status','WFCompanyId')
            //WFStatus is being placed in the file as hard coded
            DataTable tb = new DataTable();
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand("USPGetMYUsersList", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserHexID", UserHexID);
            cmd.Parameters.AddWithValue("@LoggedRoleId", LoggedRoleId);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("UserPortfolios");
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < tb.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = tb.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }

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
            IRow dRow = sheet1.GetRow(0);
            sheet1.RemoveRow(dRow);
            sheet1.ShiftRows(0, sheet1.LastRowNum, -1);
            dRow = sheet1.GetRow(0);
            dRow.ZeroHeight = true;
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
            if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/" + "MyUsersReport.xlsx"))
            {
                System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/" + "MyUsersReport.xlsx");
            }
            using (ZipFile zip = new ZipFile())
            {
                FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/", "MyUsersReport.xlsx"), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                workbook.Write(xfile);
                xfile.Close();
            }
            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            di.Refresh();
            return Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/", "MyUsersReport.xlsx");
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> SaveMissingPortfolios(string SelectedPortfolios, int UserId, int RoleId)
        {
            var EntityType = "";
            var LUser = db.LUsers.Where(p => p.Id.Equals(UserId)).FirstOrDefault();
            if (LUser == null)
            {
                var payees = db.LPayees.Where(p => p.Id == UserId).FirstOrDefault();
                if(payees != null)
                {
                    EntityType = "LPayees";
                }
            }
            else
            {
                EntityType = "LUsers";
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(SelectedPortfolios))
                    {
                        var PortfolioArray = SelectedPortfolios.Split(',').ToList();
                        foreach (var portfolio in PortfolioArray)
                        {
                            var PortfolioId = Convert.ToInt32(portfolio);
                            var MentityPortfolio = new MEntityPortfolio { MepEntityId = UserId, MepEntityType = EntityType, MepPortfolioId = PortfolioId, MepRoleId = RoleId.ToString() };
                            db.MEntityPortfolios.Add(MentityPortfolio);
                            db.SaveChanges();
                        }
                    }
                    await db.SaveChangesAsync();
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
                transaction.Commit();
            }
            return StatusCode(HttpStatusCode.NoContent);




        }

    }
}
