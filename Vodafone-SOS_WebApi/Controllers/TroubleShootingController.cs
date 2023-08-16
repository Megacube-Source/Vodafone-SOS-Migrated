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
    [CustomExceptionFilter]
    public class TroubleShootingController : ApiController
    {
        // GET: SupportSystem
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        [HttpGet]
        // WIAMSupport
        public IHttpActionResult CheckADAccountExist(string Email)
        {
            ADUserViewModel model = new ADUserViewModel { Email = Email };
            bool exists = Globals.CheckADAccount(model);
            return Ok(exists);
        }
        [HttpGet]
        public IHttpActionResult GetPayeeData(string Email)
        {
            var PayeeData = (from aa in  db.LPayees.Where(a => a.LpEmail == Email)
                             join gc in db.GCompanies on aa.LpCompanyId equals gc.Id
                             join au in db.AspNetUsers on aa.LpCreatedById equals au.Id
                             join au1 in db.AspNetUsers on aa.LpUpdatedById equals au1.Id
                             select new {
                                  OpCo = gc.GcCode,
                                  CreateLogin = aa.LpCreateLogin,
                                  BlockNotification = aa.LpBlockNotification,
                                 // Phone = aa.LpPhone,//commenting this because Phone needs Decryption. If in future it is required, SP will be called for same.
                                  CreatedBy = au.Email,
                                  CreatedOn = aa.LpCreatedDateTime,
                                  UpdatedBy = au1.Email,
                                  UpdatedOn = aa.LpUpdatedDateTime,
                                  aa.LpEffectiveEndDate,
                                  aa.LpPayeeCode,
                                  Status = aa.WFStatus
                              }).ToList();
            return Ok(PayeeData);
        }

        [HttpGet]
        public IHttpActionResult UpdateActivateCreationLogin(string Email)
        {
            try
            {
                List<LPayee> LstPayee = db.LPayees.Where(p => p.LpEmail == Email).ToList();
                List<LUser> PayeeAsFinOpsLst = db.LUsers.Where(a => a.LuEmail == Email).ToList();
                if (PayeeAsFinOpsLst.Count > 0)
                {
                    foreach (var PayeeAsFinOps in PayeeAsFinOpsLst)
                    {
                        PayeeAsFinOps.LuCreateLogin = true;
                        db.Entry(PayeeAsFinOps).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (LstPayee.Count > 0)
                {
                    foreach (var Payee in LstPayee)
                    {
                        Payee.LpCreateLogin = true;
                        db.Entry(Payee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                   
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        [HttpGet]
        public IHttpActionResult UpdateDeActivateCreationLogin(string Email)
        {
            try
            {
                List<LPayee> LstPayee = db.LPayees.Where(p => p.LpEmail == Email).ToList();
                List<LUser> PayeeAsFinOpsLst = db.LUsers.Where(a => a.LuEmail == Email).ToList();
                if (PayeeAsFinOpsLst.Count > 0)
                {
                    foreach (var PayeeAsFinOps in PayeeAsFinOpsLst)
                    {
                        PayeeAsFinOps.LuCreateLogin = false;
                        db.Entry(PayeeAsFinOps).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (LstPayee.Count > 0)
                {
                    foreach (var Payee in LstPayee)
                    {
                        Payee.LpCreateLogin = false;
                        db.Entry(Payee).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        [HttpGet]
        public IHttpActionResult UpdatePayeeCreationLoginField(string Email)
        {
            try
            {
                LPayee Payee = db.LPayees.Where(p => p.LpEmail == Email).FirstOrDefault();
                var PayeeAsFinOps = db.LUsers.Where(a => a.LuEmail == Email).FirstOrDefault();
                if (PayeeAsFinOps != null)
                {
                    PayeeAsFinOps.LuCreateLogin = !PayeeAsFinOps.LuCreateLogin;
                    db.Entry(PayeeAsFinOps).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (Payee != null)
                {
                    Payee.LpCreateLogin = !Payee.LpCreateLogin;
                    db.Entry(PayeeAsFinOps).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        [HttpGet]
        public  IHttpActionResult GetUserData(string Email)
        {
            var UserData = (from aa in db.LUsers.Where(a => a.LuEmail == Email)
                             join gc in db.GCompanies on aa.LuCompanyId equals gc.Id
                             join au in db.AspNetUsers on aa.LuCreatedById equals au.Id
                            join au1 in db.AspNetUsers on aa.LuUpdatedById equals au1.Id
                            select new
                             {
                                 OpCo = gc.GcCode,
                                 CreateLogin = aa.LuCreateLogin,
                                 BlockNotification = aa.LuBlockNotification,
                                 Phone = aa.LuPhone,
                                 CreatedBy = au.Email,
                                 CreatedOn = aa.LuCreatedDateTime,
                                 UpdatedBy = au1.Email,
                                 UpdatedOn = aa.LuUpdatedDateTime,
                                 Status = aa.WFStatus
                             }).ToList();
            return Ok(UserData);
        }
        [HttpGet]
        public IHttpActionResult GetUserLobbyData(string Email)
        {
            var LobbyData = (from aa in db.LUserLobbies.Where(a => a.Email == Email)
                             select new
                             {
                                  aa.RequestType,
                                  aa.Status,
                                 aa.PayeeCode,
                                 CreatedOn = aa.CreatedDateTime,
                                 UpdatedOn = aa.UpdatedDateTime,
                             }).ToList();
            return Ok(LobbyData);
        }
    }
}