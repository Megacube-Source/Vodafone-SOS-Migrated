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
    public class LPortfoliosController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        //SS Create Just for Payees
        public IHttpActionResult GetPortfolioDetailsByEntityType(string EntityType, string EntityIdList)
        {
            var EntityArray = EntityIdList.Split(',').Select(x => int.Parse(x)).ToList();
            var PortfolioList = from aa in db.MEntityPortfolios.Where(p => p.MepEntityType.Equals(EntityType, StringComparison.OrdinalIgnoreCase)).Where(p =>EntityArray.Contains(p.MepEntityId)).Include(p=>p.LPortfolio.RChannel)
                                join bb in db.LPayees on aa.MepEntityId equals bb.Id select new { aa.MepPortfolioId, aa.LPortfolio.RChannel.RcName, aa.LPortfolio.LpBusinessUnit, aa.LPortfolio.RChannel.RcPrimaryChannel,bb.LpPayeeCode };
            return Ok(PortfolioList);
        }
        
        public IHttpActionResult GetLPortFolios(int CompanyId)
        {
            var LPortfolios = db.LPortfolios.Where(p=>p.LpIsActive==true).Where(p => p.LpCompanyId == CompanyId).Include(p=>p.RChannel).Select(p=>new { p.Id,p.LpBusinessUnit,p.RChannel.RcPrimaryChannel,p.RChannel.RcName}).OrderBy(p => p.RcPrimaryChannel).ThenBy(p => p.LpBusinessUnit).ThenBy(p => p.RcName);
            return Ok(LPortfolios);
        }


        public IHttpActionResult GetUnAssignedPortfolioNamesByCompanyId(int CompanyId)
        {
            var LPortfolios = db.LPortfolios.Where(p => p.LpIsActive == true).Where(p => p.LpCompanyId == CompanyId).Include(p => p.RChannel).Select(p => new { p.Id, p.RChannel.RcName }).OrderBy(p => p.RcName);
            return Ok(LPortfolios);
        }

        //Get LoggedIn User Portfolios
        public IHttpActionResult GetLPortFolioByUserId(string UserId,string RoleId)
        {
            var EntityId = 0;
            //Get list of portfolios associated with user
            var RoleName = db.AspNetRoles.Where(x => x.Id == RoleId).Select(x => x.Name).FirstOrDefault();
            if (RoleName != "Payee")
            {
                var LUser = db.LUsers.Where(p => p.LuUserId.Equals(UserId)).FirstOrDefault();
                if (LUser == null)
                {
                    EntityId = db.LPayees.Where(p => p.LpUserId == UserId).FirstOrDefault().Id;
                }
                else
                {
                    EntityId = LUser.Id;
                }
            }
            else
            {
                EntityId = db.LPayees.Where(p => p.LpUserId == UserId).FirstOrDefault().Id;
            }
           
            var PortfolioList = db.MEntityPortfolios.Where(p=>p.LPortfolio.LpIsActive==true).Where(p => p.MepEntityId.Equals(EntityId)).Where(p=>p.MepRoleId==RoleId).Select(p => new { p.LPortfolio.Id,p.LPortfolio.LpBusinessUnit,p.LPortfolio.RChannel.RcPrimaryChannel,p.LPortfolio.RChannel.RcName}).OrderBy(p=>p.RcPrimaryChannel).ThenBy(p=>p.LpBusinessUnit).ThenBy(p=>p.RcName);
            return Ok(PortfolioList);
        }

        public IHttpActionResult GetLPortFolioByUserIdForL2Admin(int UserId, string RoleId, string Type)
        {
            //var EntityId = 0;
            ////Get list of portfolios associated with user
            //var LUser = db.LUsers.Where(p => p.LuUserId.Equals(UserId)).FirstOrDefault();
            //if (LUser == null)
            //{
            //    EntityId = db.LPayees.Where(p => p.LpUserId == UserId).FirstOrDefault().Id;
            //}
            //else
            //{
            //    EntityId = LUser.Id;
            //}

            var PortfolioList = db.MEntityPortfolios.Where(p => p.LPortfolio.LpIsActive == true).Where(p => p.MepEntityType.Equals(Type)).Where(p => p.MepEntityId.Equals(UserId)).Where(p => p.MepRoleId == RoleId).Select(p => new { p.LPortfolio.Id, p.LPortfolio.LpBusinessUnit, p.LPortfolio.RChannel.RcPrimaryChannel, p.LPortfolio.RChannel.RcName }).OrderBy(p => p.RcPrimaryChannel).ThenBy(p => p.LpBusinessUnit).ThenBy(p => p.RcName);
            return Ok(PortfolioList);
        }

        //Get LoggedIn User Portfolios and with refilestypes
        public IHttpActionResult GetLPortFolioByUserIdWithRefilesTypes(string UserId, string RoleId, int RefTypeID)
        {
            var EntityId = 0;
            //Get list of portfolios associated with user
            var LUser = db.LUsers.Where(p => p.LuUserId.Equals(UserId)).FirstOrDefault();
           
             EntityId = LUser.Id;

            var UserPortfolioList = db.MEntityPortfolios.Where(p => p.LPortfolio.LpIsActive == true).Where(p => p.MepEntityId.Equals(EntityId)).Where(p => p.MepRoleId == RoleId).Select(p => new { p.LPortfolio.Id, p.LPortfolio.LpBusinessUnit, p.LPortfolio.RChannel.RcPrimaryChannel, p.LPortfolio.RChannel.RcName }).OrderBy(p => p.RcPrimaryChannel).ThenBy(p => p.LpBusinessUnit).ThenBy(p => p.RcName);

            var RefFilesPortfolioList = db.MEntityPortfolios.Where(p => p.LPortfolio.LpIsActive == true).Where(p => p.MepEntityId.Equals(RefTypeID)).Where(p => p.MepEntityType == "LRefFileTypes").Select(p => new { p.LPortfolio.Id, p.LPortfolio.LpBusinessUnit, p.LPortfolio.RChannel.RcPrimaryChannel, p.LPortfolio.RChannel.RcName }).OrderBy(p => p.RcPrimaryChannel).ThenBy(p => p.LpBusinessUnit).ThenBy(p => p.RcName);

            var TotalData = UserPortfolioList.Intersect(RefFilesPortfolioList);
            if(TotalData.Count() > 0)
            {
                return Ok(TotalData);
            }
            else
            {
                return Ok(UserPortfolioList);
            }

            
        }

        public IHttpActionResult GetLPortFolioByEntityId(string EntityType, int EntityId)
        {
            var PortfolioList = db.MEntityPortfolios.Where(p => p.MepEntityType.Equals(EntityType, StringComparison.OrdinalIgnoreCase)).Where(p => p.MepEntityId.Equals(EntityId)).Select(p =>p.MepPortfolioId );
            return Ok(PortfolioList);
        }

        public IHttpActionResult GetLPortFolioByUserIdForEditGrid(int UserId, int CompanyId, string Role,string CompanyCode)
        {
            //Get list of portfolios associated with user
            // var AlreadyAssignedPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityType == "LUsers").Where(p => p.MepEntityId == UserId).Where(p=>p.MepRoleId==RoleId).Select(p => p.MepPortfolioId).ToList();
            var ExistingPortfolios = new List<int>();
            if (string.IsNullOrEmpty(Role))
            {
                ExistingPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityType == "LUsers").Where(p => p.MepEntityId == UserId).Select(p => p.MepPortfolioId).ToList();
            }
            else
            {
                var RoleId = db.AspNetRoles.Where(p => p.Name == Role).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault().Id;
                ExistingPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityId == UserId).Where(p => p.MepRoleId == RoleId).Select(p => p.MepPortfolioId).ToList();
            }
            var PortfolioList = db.LPortfolios.Where(p => p.LpCompanyId == CompanyId).Where(p => p.LpIsActive == true).Select(p => new { p.Id, p.LpBusinessUnit, p.RChannel.RcPrimaryChannel, p.RChannel.RcName, Select = (ExistingPortfolios.Contains(p.Id)) ? true : false }).OrderByDescending(p => p.Select).ThenBy(p => p.RcPrimaryChannel).ThenBy(p => p.LpBusinessUnit).ThenBy(p => p.RcName);
            return Ok(PortfolioList);
        }

        public IHttpActionResult GetLPortFolioByLoggedInUserIdForEditGrid(string UserId,int TransactionId,string EntityType,string Role,string LoggedInRoleId,string CompanyCode)
        {
            //Get list of portfolios associated with user
            var LUserId = db.LUsers.Where(p => p.LuUserId.Equals(UserId)).FirstOrDefault().Id;
            var ExistingPortfolios =new List<int>();
            if (string.IsNullOrEmpty(Role))
            {
                ExistingPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityType == EntityType).Where(p => p.MepEntityId == TransactionId).Select(p => p.MepPortfolioId).ToList();
            }
            else
            {
                var RoleId = db.AspNetRoles.Where(p => p.Name == Role).Where(p => p.CompanyCode == CompanyCode).FirstOrDefault().Id;
                ExistingPortfolios= db.MEntityPortfolios.Where(p => p.MepEntityType == EntityType).Where(p => p.MepEntityId == TransactionId).Where(p=>p.MepRoleId==RoleId).Select(p => p.MepPortfolioId).ToList();
            }
            if (EntityType != "LSupportTickets")
            {
                var PortfolioList = db.MEntityPortfolios.Where(p => p.LPortfolio.LpIsActive == true).Where(p => p.MepEntityId.Equals(LUserId)).Where(p => p.MepRoleId == LoggedInRoleId).Select(p => new { p.LPortfolio.Id, p.LPortfolio.LpBusinessUnit, p.LPortfolio.RChannel.RcPrimaryChannel, p.LPortfolio.RChannel.RcName, Select = (ExistingPortfolios.Contains(p.MepPortfolioId)) ? true : false }).OrderByDescending(p => p.Select).ThenBy(p => p.RcPrimaryChannel).ThenBy(p => p.LpBusinessUnit).ThenBy(p => p.RcName);
                return Ok(PortfolioList);

            }
            else
            {
                var ExistingSupportPortfolio = db.MSupportTicketPortfolios.Where(p => p.TicketId == TransactionId).Select(p => p.PortfolioId).ToList();
                var PortfolioList = db.MEntityPortfolios.Where(p => p.MepEntityId.Equals(LUserId)).Where(p => p.MepRoleId == LoggedInRoleId).Select(p => new { p.LPortfolio.Id, p.LPortfolio.LpBusinessUnit, p.LPortfolio.RChannel.RcPrimaryChannel, p.LPortfolio.RChannel.RcName, Select = (ExistingSupportPortfolio.Contains(p.MepPortfolioId)) ? true : false }).OrderByDescending(p => p.Select).ThenBy(p => p.RcPrimaryChannel).ThenBy(p => p.LpBusinessUnit).ThenBy(p => p.RcName);
                return Ok(PortfolioList);
            }
           
        }

        public IHttpActionResult PutUpdatePortfolio(int EntityId,string EntityType,string PortfolioList,string CompanyCode)
        {
            string RoleId = null;
            //If EntityType is Payee then get Payee RoleId
            if(EntityType=="LPayees")
            {
                var Role = db.AspNetRoles.Where(p => p.CompanyCode == CompanyCode).Where(p => p.Name.Equals("Payee", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (Role != null)
                    RoleId = Role.Id;
            }
            //delete existing portfolio and add new portfolio
            if (!string.IsNullOrEmpty(PortfolioList))
            {
                var Portfolio = db.MEntityPortfolios.Where(p => p.MepEntityId == EntityId).Where(p => p.MepEntityType == EntityType).Where(p=>p.MepRoleId==RoleId).ToList();
                    db.MEntityPortfolios.RemoveRange(Portfolio);
                    db.SaveChanges();
         
                var NewPortfolioList = PortfolioList.Split(',').ToList();
                foreach(var PF in NewPortfolioList)
                {
                    var PFId = Convert.ToInt32(PF);
                    var MEntityportfolios = new MEntityPortfolio {MepEntityId=EntityId,MepPortfolioId=PFId,MepEntityType=EntityType,MepRoleId=RoleId };
                    db.MEntityPortfolios.Add(MEntityportfolios);
                    db.SaveChanges();
                }
                /*Special case for payees if Portfolios are updated the also update primary channel of Payees in Case of entity type is LPayees*/
                if(EntityType=="LPayees")
                {
                    var PortfolioId = Convert.ToInt32(NewPortfolioList.ElementAt(0));
                    var PortfolioDetails = db.LPortfolios.Where(p => p.Id == PortfolioId).Select(p=>new {p.Id,p.RChannel.RcPrimaryChannel }).FirstOrDefault();
                    if(PortfolioDetails!=null)
                    {
                        var PayeeDetails = db.LPayees.Where(p => p.Id == EntityId).FirstOrDefault();
                        if(PayeeDetails!=null)
                        {
                            PayeeDetails.LpPrimaryChannel = PortfolioDetails.RcPrimaryChannel;
                            db.Entry(PayeeDetails).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            return Ok();
        }

    }
}
