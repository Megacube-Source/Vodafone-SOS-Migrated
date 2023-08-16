using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
using System.Globalization;
using System.Configuration;
using CsvHelper;
using System.IO;
using System.Reflection;
using System.Net;
using System.Net.Http;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class ADAccountController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        //Create new user
        public IHttpActionResult CreateUser(ADUserViewModel model)
        {
            var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
            try
            {
                var result = Globals.CreateUser(model.Email, ProjectEnviournment, null,null,null,null,model.Password);
                Globals.SendEmailSES(model.Email, null, "AD User Created", "User has been created", "QA");
                if (result.IsSuccess)
                {
                    return Ok(true);
                }
                else if (result.ErrorMessage.Contains("User Already exists"))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, result.ErrorMessage));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.ErrorMessage));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IHttpActionResult LoginUser(ADUserViewModel model)
        {
            var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
            var result = Globals.SignIn(model, ProjectEnviournment);
                if (result.IsSuccess)
                {
                    return Ok(true);
                }
                else if (result.ErrorMessage.Contains("incorrect"))
                {

                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NonAuthoritativeInformation, result.ErrorMessage));
                }
                else if (result.ErrorMessage.Contains("reset"))
                {
                    return Ok(false);//need to reset password
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.ErrorMessage));
                }

        }


        public IHttpActionResult ChangeUserPassword(ADUserViewModel model)
        {
            AuthenticationResult result = new AuthenticationResult();


            if (String.IsNullOrEmpty(model.Password))
            {
                if (Globals.CheckADAccount(model))
                {
                        if (Globals.CheckADAccount(model))
                        {
                            result = Globals.SetUserPassword(model);
                        }else
                        {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Email Id does not Exist in AD"));
                        }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Email Id does Not Exist in AD"));
                }
            }else
            { 
                    var User = db.AspNetUsers.Where(p => p.Email == model.Email).FirstOrDefault();
                var PasswordHistory = db.LPasswordHistories.Where(p => p.UserId == User.Id);
                if (PasswordHistory.Count() > 0)/*Added logic to set password instead of changing it when user logs in for the first time. To ensure that history days validation is not applied .*/
                {
                    result = Globals.ChangeMyPassword(model);
                }
                else
                {
                    result = Globals.SetUserPassword(model);
                }
                }

                if (result.IsSuccess)
                {
                /*If Password has changed sucessfully add an entryin LPassword History
                 We will first delete all older passwords (First In First Out method) more than number of LPasswordPolicies.PreventReuse for that companyID 
                 and then insert current password row for that user
                ToDo: PsuedoCode: Before you INSERT in this table, select count(*) from LPasswordHistory for login user
                If count >= LpasswordPolicy. PreventReuse, then DELETE oldest password (min CreatedDate) 
                Then INSERT “Current password”
                •	AspNetUserId nvarchar
                •	Password nvarchar 100 Nullable (Do not insert password – functionality parked)
                •	CreatedDate DateTime
                 */
                var UserDetail = db.AspNetUsers.Where(p => p.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if(UserDetail!=null)
                {
                    var PasswordHistory = db.LPasswordHistories.Where(p => p.UserId.Equals(UserDetail.Id)).ToList();
                    var PasswordPolicy = db.LPasswordPolicies.Where(p => p.CompanyId == UserDetail.GcCompanyId).FirstOrDefault();
                    /*JS 01/08/2017-Since we are using AD inherited policy of preventing password re-use, we need to code it here (also help us not saving user passwords in our database)*/
                    //if(PasswordHistory.Count()>=PasswordPolicy.PreventReuse)//If No of Password Hostory record exceeds Prevent reuse count then delte the oldest record
                    //{
                    //    var OldestPassword = PasswordHistory.OrderBy(p => p.CreatedDateTime).FirstOrDefault();
                    //    if (OldestPassword != null)
                    //    {
                    //        db.LPasswordHistories.Remove(OldestPassword);
                    //        db.SaveChanges();
                    //    }
                    //}

                    var NewPasswordDetails = new LPasswordHistory { UserId=UserDetail.Id,CreatedDateTime=DateTime.UtcNow};
                    db.LPasswordHistories.Add(NewPasswordDetails);
                    db.SaveChanges();
                }
                return Ok(true);
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.ErrorMessage));
                }

        }


        public IHttpActionResult UpdateUserDetails(ADUserViewModel model)
        {
                AuthenticationResult result = new AuthenticationResult();
                result = Globals.UpdateUserDetails(model);
                if (result.IsSuccess)
                {
                    return Ok(true);
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.ErrorMessage));
                }
                
        }

        [HttpGet]
        public async Task<IHttpActionResult> RemoveUser(ADUserViewModel model)
        {
            var Files = new List<string>(); 
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Database.ExecuteSqlCommand("delete from AspNetUserRoles where UserId = (select Id from AspNetUsers where email = '" + model.Email + "')");//update it to linq
                    var LUser = db.LUsers.Where(p => p.LuEmail == model.Email).FirstOrDefault();
                    var LPayee = db.LPayees.Where(p => p.LpEmail == model.Email).FirstOrDefault();            
                    if (LUser != null)
                    {
                        var Mportfolios = db.MEntityPortfolios.Where(p => p.MepEntityId == LUser.Id);
                        if (Mportfolios != null)
                        {
                            var portfoliosList = Mportfolios.ToList();
                            foreach (var portfolio in portfoliosList)
                            {
                                db.MEntityPortfolios.Remove(portfolio);//remove from MEntityPortfolios
                            }
                        }
                        var LSupportingDocs = db.LSupportingDocuments.Where(p => p.LsdCreatedById.Equals(LUser.Id));
                        if (LSupportingDocs != null)
                        {
                            var docList = LSupportingDocs.ToList();
                            foreach (var doc in docList)
                            {
                               string FilePath = doc.LsdFilePath.Replace('/','\\');
                                Files.Add(doc.LsdFilePath + "\\" +doc.LsdFileName);//get file location as well
                                db.LSupportingDocuments.Remove(doc);//remove from LSupportingDocuments
                            }
                        }
                        db.LUsers.Remove(LUser); // remove from LUsers
                    }
                    else if (LPayee != null)
                    {// redundant code for LUser and LPayee As dont have idea for defining IQueryable object. Need to fix it on code review
                        var Mportfolios = db.MEntityPortfolios.Where(p => p.MepEntityId == LPayee.Id);
                        if (Mportfolios != null)
                        {
                            var portfoliosList = Mportfolios.ToList();
                            foreach (var portfolio in portfoliosList)
                            {
                                db.MEntityPortfolios.Remove(portfolio);
                            }
                        }
                        var LSupportingDocs = db.LSupportingDocuments.Where(p => p.LsdCreatedById.Equals(LPayee.Id));
                        if (LSupportingDocs != null)
                        {
                            var docList = LSupportingDocs.ToList();
                            foreach (var doc in docList)
                            {
                                string FilePath = doc.LsdFilePath.Replace('/', '\\');
                                Files.Add(doc.LsdFilePath + "\\" + doc.LsdFileName);                                
                                db.LSupportingDocuments.Remove(doc);
                            }
                        }
                        db.LPayees.Remove(LPayee);//remove from LPayees
                    }
                    var AspUser = db.AspNetUsers.Where(p => p.Email == model.Email).FirstOrDefault();
                    db.AspNetUsers.Remove(AspUser);//remove from AspnetUsers.
                    
                   
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                //delete files from file system
                foreach (var File1 in Files)
                {
                    try
                    {
                        if (File.Exists(File1))
                        {
                            File.Delete(File1);
                        }
                    }
                    catch(IOException ioex)
                    {
                        throw ioex;
                    }
                }
                //need to delete files here
                //files will be removed even, in case AD user deletion fails and cannot recover.
                AuthenticationResult result = new AuthenticationResult();
                result = Globals.DeleteUser(model);//remove user from AD
                if (result.IsSuccess)
                {
                    await db.SaveChangesAsync();

                }
                else if (result.ErrorMessage.Contains("not found"))
                {
                    transaction.Rollback();
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User not found in Active Directory"));
                }
                else
                {
                    transaction.Rollback();

                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.ErrorMessage));
                }
            
            transaction.Commit();
            return Ok(true);
        
            }
        }

        public async Task<IHttpActionResult> ActivateDeactivate(ADUserViewModel model)
        {
            try
            {
                AuthenticationResult result = new AuthenticationResult();
                result = Globals.ActivateDeactivateUser(model);
                if (result.IsSuccess)
                {
                    var LUser = db.LUsers.Where(p => p.LuEmail == model.Email).FirstOrDefault();
                    if (LUser != null) {
                        if (model.Status)
                        {
                            LUser.WFStatus = "ADEnabled";
                        }
                        else
                        {
                            LUser.WFStatus = "ADDisabled";
                        }
                        db.Entry(LUser).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    return Ok(true);
                }
                else if(result.ErrorMessage.Contains("not found"))
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, result.ErrorMessage));
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.ErrorMessage));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IHttpActionResult> UnlockUser(ADUserViewModel model)
        {

            try
            {
                var AspNetUser = db.AspNetUsers.Where(p => p.Email == model.Email).FirstOrDefault();
                if (AspNetUser != null)
                {

                    AspNetUser.LockoutEndDateUtc = null;
                    await db.SaveChangesAsync();
                    return Ok(true);
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Please check the Email"));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IHttpActionResult CreateLogin(CreateLoginViewModel model)
        {
            string UserType = model.UserType;
            if (UserType.Equals("Payee"))
            {
                var Payee = db.LPayees.Where(a => a.LpEmail == model.Email).FirstOrDefault();
                if(Payee != null)
                {
                    Payee.LpCreateLogin = model.CreateLogin;
                    db.SaveChanges();
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Please check the Email"));
                }
            }
            else
            {
                var User = db.LUsers.Where(a => a.LuEmail == model.Email).FirstOrDefault();
                if (User != null)
                {
                    User.LuCreateLogin = model.CreateLogin;
                    db.SaveChanges();
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Please check the Email"));
                }
            }
            return Ok("Updated");
        }
    }
}