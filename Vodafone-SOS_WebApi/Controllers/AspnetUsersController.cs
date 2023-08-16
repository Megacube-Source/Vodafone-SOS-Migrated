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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using System.Data.Entity.Core.Objects;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class AspnetUsersController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
        public AspnetUsersController()
        {
        }
        public AspnetUsersController(ApplicationUserManager userManager,
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
        public IHttpActionResult GetClaimsAnalystByCompanyId(int CompanyId)
        {
            var xx = db.Database.SqlQuery<AspnetUserViewModel>("select AU.UserName,AU.Id from AspNetUserRoles AUR inner join AspNetUsers AU on AUR.UserId=AU.Id inner join AspNetRoles AR on AUR.RoleId=AR.Id where AR.Name='Claims Analyst'  and AU.GcCompanyId=" + CompanyId).ToList();
            return Ok(xx);
        }
        //This method is used to fill dropdown of channel manger in Payee
        public IHttpActionResult GetChannelManagerByCompanyCode(string CompanyCode)
        {
            var xx = db.Database.SqlQuery<AspnetUserViewModel>("select (LU.LuFirstName+' '+LU.LuLastName) as FullName,AU.Id from AspNetUserRoles AUR inner join AspNetUsers AU on AUR.UserId=AU.Id inner join LUsers LU on LU.LuUserId=AU.Id inner join AspNetRoles AR on AUR.RoleId=AR.Id where AR.Name='Channel Manager' and LU.WFStatus='Completed' and AR.CompanyCode='" + CompanyCode+"'").ToList();
            return Ok(xx);
        }

        public IHttpActionResult GetSalesOperationsByCompanyCode(string CompanyCode)
        {
            var xx = db.Database.SqlQuery<AspnetUserViewModel>("select (LU.LuFirstName+' '+LU.LuLastName) as FullName,AU.Id from AspNetUserRoles AUR inner join AspNetUsers AU on AUR.UserId=AU.Id inner join LUsers LU on LU.LuUserId=AU.Id inner join AspNetRoles AR on AUR.RoleId=AR.Id where AR.Name='Sales Operations' and LU.WFStatus='Completed' and AR.CompanyCode='" + CompanyCode + "'").ToList();
            return Ok(xx);
        }

        //public IHttpActionResult GeUsersByCompanyId(int CompanyId)
        //{
        //    var xx = db.Database.SqlQuery<AspnetUserViewModel>("select AU.UserName,AU.Id from AspNetUserRoles AUR inner join AspNetUsers AU on AUR.UserId=AU.Id inner join AspNetRoles AR on AUR.RoleId=AR.Id where  AU.GcCompanyId=" + CompanyId).ToList();
        //    return Ok(xx);
        //}
        //select list of users based on a role named as claims analyst to display dropdown of claims analyst user
        public IHttpActionResult GetAspNetUsersByCompanyId(int CompanyId)
        {
            var xx = (from aa in db.AspNetUsers.Where(p => p.GcCompanyId == CompanyId)
                      join yy in db.LUsers on aa.Id equals yy.LuUserId
                      select new { aa.Id, UserName = yy.LuFirstName + " " + yy.LuLastName, aa.Email }).OrderBy(p => p.UserName);
            return Ok(xx);
        }

        public IHttpActionResult GetActiveAspNetUsersByCompanyId(int CompanyId)
        {
            //MS R2.4 show only active users for on behalf of

            List<string> selectedusers = new List<string>();
            selectedusers = db.AspNetUsers.Where(p => p.GcCompanyId.Equals(CompanyId)).Select(p => p.Id).ToList();

            var UserList = db.LUsers.Where(p => p.LuStatus == "Active" && selectedusers.Contains(p.LuUserId)).Select(p => new { UserName = p.LuFirstName + " " + p.LuLastName, Id = p.LuUserId, Email=p.LuEmail }).ToList();
            return Ok(UserList);

        }

        public IHttpActionResult GetAspNetUser(string Id)
        {
            var Roles= UserManager.GetRoles(Id);
            var RolesList = string.Join(",", Roles);
            var xx = (from aa in db.AspNetUsers.Where(p => p.Id==Id) select new { aa.Id, aa.UserName,RolesList=RolesList,aa.Email }).FirstOrDefault();
            return Ok(xx);
        }
        public IHttpActionResult GetAspNetUsersByEmailId(string strEmailID)
        {
            var xx = (from aa in db.AspNetUsers.Where(p => p.Email == strEmailID)
                      select new { aa.Id,aa.GcCompanyId }).FirstOrDefault();
            return Ok(xx);
        }

        [HttpGet]
        public string GetUsersByEmailId(string EmailId)
        {
            var xx = (from aa in db.AspNetUsers.Where(p => p.Email == EmailId)
                      select aa).FirstOrDefault();

            var text = "";
            if (xx == null)
            {
                text = "User not registered";
            }
            else
            {
                text = xx.Email;
            }
             
            return text;

           
        }



   }


    
}
