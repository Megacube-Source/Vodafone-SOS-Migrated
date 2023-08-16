using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Providers;
using Vodafone_SOS_WebApi.Results;
using System.Net;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Vodafone_SOS_WebApi.Utilities;
using System.Configuration;
using System.Data.Entity;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace Vodafone_SOS_WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    [CustomExceptionFilter]
 
    public class AccountController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
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

        //This method is used for MFA Authentication in SOS which was Generated and Sent via SMS to the User.
        //[HttpGet]
        //public IHttpActionResult VerifyMFAOTP(string UserId)
        //{
        //    return Ok();
        //}

        //private VodafoneSOSLiteEntities db = new VodafoneSOSLiteEntities();
        //This method is not being used currently, as we have changed the approach for forgotPassword.
        //Method to check Email id is valid or not, And send PasswordReset Token in Email.
        [HttpGet]
        [OverrideAuthorization]
        public IHttpActionResult ForgotPassowrd(string Email)
        {
            //verify Email exists or not.
            var user = UserManager.FindByEmail(Email);
            if (user != null)
            {
                var UserId = user.Id;
                //Generate Reset Password Token.
                var token = UserManager.GeneratePasswordResetToken(UserId);
                token = HttpUtility.UrlEncode(token);
                //Send Reset Password Link
                var result = SendMail(Email,token, UserId);
               
                return Ok();
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid UserId"));
            }

        }

        [HttpPost]
        [OverrideAuthorization]
        public IHttpActionResult GenerateOTPnSendMail(MAspnetUsersGScurityQuestionViewModel model)
        {
            var ans1 = db.Database.SqlQuery<string>("select MAugsqAnswer from MAspnetUsersGSecurityQuestions where MAuqsqUserId='"+model.MAuqsqUserId + "' and MAuqsqQuestionId="+model.Question1).FirstOrDefault();
            //need to check one question at a time.
            // var ans2 = db.Database.SqlQuery<string>("select MAugsqAnswer from MAspnetUsersGSecurityQuestions where MAuqsqUserId='" + model.MAuqsqUserId + "' and MAuqsqQuestionId=" + model.Question2).FirstOrDefault();
            if (ans1.ToLower().Equals((model.Answer1.ToLower())))//case-insensitive comparison
                                                                 //&& ans2.Equals(model.Answer2))
            {
                RandomPassword rnd = new RandomPassword();
                var OTP = rnd.Generate(8);
                OTP = OTP.Substring(0, 8);
                var OTPValidity = ConfigurationManager.AppSettings["OTPValidity"];
               var OTPValidUpto = DateTime.UtcNow.AddMinutes(Convert.ToInt32(OTPValidity));
               // db.Database.ExecuteSqlCommand("update AspNetUsers set OTP='" + OTP + "',OTPValidUpto=DATEADD(minute,"+OTPValidity+",GETDATE()) where Id='"+ model.MAuqsqUserId+"'");
                //ApplicationUser u = UserManager.FindById(model.MAuqsqUserId);
                //u.OTP = OTP;
                //u.OTPValidUpto = OTPValidUpto;
                //UserManager.Update(u);
                var AspUser = db.AspNetUsers.Find(model.MAuqsqUserId);
                AspUser.OTP = OTP;
                AspUser.OTPValidUpto = OTPValidUpto;
                db.Entry(AspUser).State = EntityState.Modified;
                db.SaveChanges();
                var result = SendMail(AspUser.Email, OTP, model.MAuqsqUserId);
                // Globals.SendEmail(Email, null, code + "is the verification Code", code + "is your verifcation OTP code. Please use this OTP to reset your password.Expiration is  " + OTPValidUpto, "QA");
                return Ok();
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Incorrect Answer. Please try again."));
            }
        }


        [HttpGet]
        [OverrideAuthorization]
        public IHttpActionResult VerifyOTP(string OTP, string UserId)
        {
            var validity = DateTime.UtcNow;
            var AspUser = db.AspNetUsers.Find(UserId);
            if (OTP.Equals(AspUser.OTP) && validity < AspUser.OTPValidUpto)
            {
                //Invalidate OTP once it is used.
                AspUser.OTP = "Expired";
                var OTPValidity = ConfigurationManager.AppSettings["OTPValidity"];
                AspUser.OTPValidUpto = DateTime.UtcNow.AddHours(-(Convert.ToInt32(OTPValidity)));
                db.Entry(AspUser).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(true);
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid OTP. Please Try again."));
            }

        } 


        private int SendMail(string ToEmail, string OTP, string UserId)
        {
            //get the email template from database and retrieve Email Subject and body
            var EmailTemplateName = ConfigurationManager.AppSettings["EmailTemplateName"];
            //EMail Template is not working properly. Will be workingon it in next phase. -SG
            //var Template = db.Database.SqlQuery<LEmailTemplate>("Select Id,LetTemplateName,LetEmailSubject,LetEmailBody,LetSignature from LEmailTemplates where LetTemplateName= "+ EmailTemplateName).FirstOrDefault();
            //string EmailSubject = Template.LetEmailSubject;
            //string EmailBody = Template.LetEmailBody;
           var OTPValidity = ConfigurationManager.AppSettings["OTPValidity"];
            var EmailSubject = "LITE - Forgot Password OTP";
            var EmailBody = "As per your request, a One Time Password (OTP) has been generated and the same is " + OTP
                + "<br/><br/>Please use this OTP to complete the forgot password process."
                + "<br/><br/>Note: OTP will expire in " + OTPValidity + "mins. If expired, the transaction would have to be re-initiated and a new OTP to be generated."
                + "<br/><br/>LITE Support Team"
                + "<br/><br/><b>** This is an auto-generated email. Please do not reply to this email.**<b>"
                + "<br/><br/>For any queries, please contact our local support";
            //call to Stored Procedure SpLogEmail which inserts the record in LEmailBucket for sending email.

            Globals.ExecuteSPLogEmail(ToEmail, null,null, null, EmailSubject, EmailBody, true, "Notifier","High", null, "InQueue", null, UserId, UserId, "Test Vodafone Lite SES","","","");
            return 1;//This 1 denote sucess event of EF which is not getting here now
        }

        //Method to Get Email 
        [HttpGet]
        [OverrideAuthorization]
        public IHttpActionResult GetIdByEmailId(string Email)
        {
            var user = db.AspNetUsers.Where(p => p.Email == Email).Select(p=>new {p.Email,p.GcCompanyId,p.Id }).FirstOrDefault();//UserManager.FindByEmail(Email);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid User"));
            }

        }

        //method used to get user details to web App and Authenticate user details
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        [OverrideAuthorization]
        public IHttpActionResult GetUserInfo(string HostBrowserDetails,string HostIP,string HostTimeZone, string Email, string Password,string MFAOTP)
        {
            // ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            try
            {
                var users = UserManager.FindByEmail(Email);
               if(users==null)
                {
                    return Ok("User not registered");
                }
                var UserDetails = db.AspNetUsers.Where(p => p.Email.Equals(Email)).FirstOrDefault();
                //Get details of opco
                var Company = db.GCompanies.Find(UserDetails.GcCompanyId);
                //10) The first time login password change should check whether user is Active before asking for new passowrd
                if (UserDetails.IsActive==false)
                {
                    return Ok("User is Inactive");
                }
                DateTime LockUnTilDateTime = DateTime.UtcNow;
                if (UserDetails.LockoutEndDateUtc.HasValue)
                {
                    LockUnTilDateTime = UserDetails.LockoutEndDateUtc.Value;
                }

                if (LockUnTilDateTime <= DateTime.UtcNow)
                {
                    //AD Authentication changes starts here---------------SG
                    ADUserViewModel model = new ADUserViewModel();
                model.Email = Email;
                model.Password = Password;
                    var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
                    var result = Globals.SignIn(model, ProjectEnviournment);
                    //result.IsSuccess = true;
                if (!result.IsSuccess)
                {
                    if (result.ErrorMessage.Contains("incorrect"))
                    {

                        //Add Entry in User Log for Failed Login
                        var GUserActivityModel = new GUserActivityLog();
                        GUserActivityModel.UalActionById = UserDetails.Id;
                        GUserActivityModel.UalActivity = "FailedLogin";
                        GUserActivityModel.UalRemarks = "LogIn Failed";
                        GUserActivityModel.UalActivityDateTime = DateTime.UtcNow;
                        GUserActivityModel.UalCompanyId = UserDetails.GcCompanyId;
                        GUserActivityModel.UalIsActivitySucceeded = true;
                        GUserActivityModel.UalUserId = UserDetails.Id;
                        //GUserActivityModel.UalHostBrowserDetails = HostBrowserDetails;
                        //GUserActivityModel.UalHostIP =HostIP;
                       // GUserActivityModel.UalHostTimeZone = HostTimeZone;
                        db.GUserActivityLogs.Add(GUserActivityModel);
                        db.SaveChanges();

                        //checking User Activity and LockAccount if consecutive UnSucessfull Attempts are made
                        var LPasswordPolicies = db.LPasswordPolicies.Where(p => p.CompanyId == UserDetails.GcCompanyId).FirstOrDefault();
                            /*If failed login,  if  LockoutFailedAttempts  > 0,check if the number of consecutive failed exceeds LockoutFailedAttempts, 
                            If yes, update ASPNetUser table with GetDate()+LockoutMins in the column <LockoutEndDateUtc> for that user.*/
                            if (LPasswordPolicies != null)
                            {
                                if (LPasswordPolicies.LockoutFailedAttempts > 0)
                                {
                                    var UserActivityLog = db.GUserActivityLogs.Where(p => p.UalUserId == UserDetails.Id).OrderByDescending(p => p.UalActivityDateTime).Take(LPasswordPolicies.LockoutFailedAttempts).ToList();
                                    var LockoutFailedAttempts = LPasswordPolicies.LockoutFailedAttempts;
                                    if (UserActivityLog.Where(p => p.UalActivity.Equals("FailedLogin")).Count() >= LockoutFailedAttempts)
                                    {
                                        UserDetails.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(LPasswordPolicies.LockoutMins);
                                        db.Entry(UserDetails).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Add Entry For User Lockout
                                        GUserActivityModel = new GUserActivityLog();
                                        GUserActivityModel.UalActionById = UserDetails.Id;
                                        GUserActivityModel.UalActivity = "Lockout";
                                        GUserActivityModel.UalRemarks = "LogIn Locked Out";
                                        GUserActivityModel.UalActivityDateTime = DateTime.UtcNow;
                                        GUserActivityModel.UalCompanyId = UserDetails.GcCompanyId;
                                        GUserActivityModel.UalIsActivitySucceeded = true;
                                        GUserActivityModel.UalUserId = UserDetails.Id;
                                        //GUserActivityModel.UalHostBrowserDetails = HostBrowserDetails;
                                        //GUserActivityModel.UalHostIP = HostIP;
                                       // GUserActivityModel.UalHostTimeZone = HostTimeZone;
                                        db.GUserActivityLogs.Add(GUserActivityModel);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There is something wrong with Password Policy. Please contact L2Admin"));
                            }
                        return Ok(result.ErrorMessage);
                    }
                    else if (result.ErrorMessage.Contains("reset"))
                    {
                            //redirect user to Login Page Only if He/She is Authenticated
                            return Ok(result.ErrorMessage);//need to reset password
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.ErrorMessage));
                    }
                }
                    //AD chnages ends here------------------------SG

                    //Get Roles
                    var xx = users;
                    var roles = (List<string>)UserManager.GetRolesAsync(xx.Id).Result;
                    var PasswordPolicies = db.LPasswordPolicies.Where(p => p.CompanyId == xx.GcCompanyId).FirstOrDefault();
                    //Check if MFA Enabled for any role then User Needs to be redirected to MFA Screen
                    /*Upon successful login (AD authentication), check if user has MFAEnabled role
                        MFARoleCount = Select count(*) from ASPNetUserRoles where user = logged in user AND ASPNetRole.MFAEnabled = 1
                        If MFARoleCount > 0  - - that means user has atleast one role assigned that requires MFA for that OpCo
                        Then 
                        a)	Send OTP token on SMS using clean phone number from FNReturnPhoneNumber (pass phone by first checking in LPayees table and then LUsers table.
                        Reason being, if he is a Payee with FinOps role, then we want to treat LPayees as master table, otherwise, he is a pure FinOps user) 
                        (also make entry in SMSBucket with status Sent. If any error received from API, update Status to ‘Failed’ and put failure return code+message 
                        in SMSBucket.Comments) and EMAIL (EmailBucket using EmailTemplate ‘MFA’).
                        Save it in ASPNetUser.OTP, ASPNetUser.OTPValidUpto = GetDate() + LPasswordPolicy.MFAOTPValidMins (for that OpCo)*/
                    if (db.AspNetRoles.Where(p => roles.Contains(p.Name)).Where(p=>p.CompanyCode==Company.GcCode).Any(p => p.MFAEnabled) && string.IsNullOrEmpty(MFAOTP))
                    {
                        //Generate OTP and Send it to the User Via SMS
                        Random rd = new Random();
                        var RandomNumber = rd.Next(100000, 999999);
                        //Get PhoneNumber
                        string ReceiverPhoneNumber;
                        if (db.LPayees.Where(p => p.LpUserId == xx.Id).Count() > 0)
                        {
                            ReceiverPhoneNumber = Convert.ToString(db.LPayees.Where(p => p.LpUserId == xx.Id).Select(p => p.LpPhone).FirstOrDefault());//SS Decrypted
                        }
                        else
                        {
                            ReceiverPhoneNumber = db.LUsers.Where(p => p.LuUserId == xx.Id).Select(p => p.LuPhone).FirstOrDefault();
                        }
                        //Update AspNetUser
                        UserDetails.OTP = RandomNumber.ToString();
                        UserDetails.OTPValidUpto = DateTime.UtcNow.AddMinutes(Convert.ToDouble(PasswordPolicies.MFAOTPValidMins));
                        db.Entry(UserDetails).State = EntityState.Modified;
                        db.SaveChanges();
                       // db.Database.ExecuteSqlCommand("update AspnetUsers set OTP='" + RandomNumber.ToString() + "',OTPValidUpto=GetDate()+" + PasswordPolicies.MFAOTPValidMins + " ");
                        //call DB function to clear phone and add ISD code
                        var ManipulatedPhoneNumber = db.Database.SqlQuery<string>("select [dbo].[FNReturnPhoneNumber]({0},{1})", ReceiverPhoneNumber, Company.GcCode).FirstOrDefault();
                        string SMSAccessKey = Globals.GetValue("sns_accesskey"); //ConfigurationManager.AppSettings["SMSAccessKey"];
                        string SMSSecretKey = Globals.GetValue("sns_secretkey");  //ConfigurationManager.AppSettings["SMSSecretKey"];
                        var snsClient = new AmazonSimpleNotificationServiceClient(SMSAccessKey, SMSSecretKey);
                        string message = "Your MFA token for login into LITE is " + RandomNumber + ". If you did not request this, please contact your LITE support";//"SOS SMS message";
                        string phoneNumber = ManipulatedPhoneNumber;//"+919871073209";//"+61450280180";
                        //var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
                        //ENV is calculated above
                        //var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
                        //string Status = "InQueue";
                        if (!string.IsNullOrEmpty(ManipulatedPhoneNumber))
                        {
                            //If we are doing MFA Authentication in test(Non Prod) it should send SMS to Dummy Number(by concating test to mobile number).
                            if (!ProjectEnviournment.Equals("Prod",StringComparison.OrdinalIgnoreCase))
                            {
                                phoneNumber = "T" + phoneNumber;
                                //Status = "InQueueTest";// so that  even if the service is running, we will not have SMS bouncing. 
                            }
                            var LSMS = new LSMSBucket {
                                Recipient= phoneNumber,
                                Message=message,
                                Status="Sent",
                                //Status = Status,
                                CreatedById=UserDetails.Id,
                                UpdatedById = UserDetails.Id,
                                CreatedDateTime=DateTime.UtcNow,UpdatedDateTime=DateTime.UtcNow
                            };
                            db.LSMSBuckets.Add(LSMS);
                            db.SaveChanges();
                            //byte[] data = UTF8Encoding.UTF8.GetBytes("mySenderID");
                            //This piece of code is moved o service
                            try
                            {
                                Dictionary<string, MessageAttributeValue> smsAttributes = new Dictionary<string, MessageAttributeValue>();
                                var response = snsClient.Publish(new PublishRequest()
                                {
                                    Message = message,
                                    PhoneNumber = phoneNumber,
                                    MessageAttributes = smsAttributes
                                });
                            }
                            catch (Exception ex)
                            {
                                //Add Error Code and Message in  Comments in SMS table. Also Update Status to failed.
                                LSMS.Comments = ex.Message + ex.StackTrace;
                                LSMS.Status = "Failed";
                                db.Entry(LSMS).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        //Get receiver Name based on Project Env

                        var ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, model.Email).FirstOrDefault();
                        //log email through SMTP details in web.config in email bucket
                        Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null,
                          "MFA token for LITE",message, true, "Error", "High", null, "InQueue", null, xx.Id, xx.Id, "Test Vodafone Lite SES", null, null, null);
                        return Ok("MFAScreen");
                    }
                    else if (db.AspNetRoles.Where(p => roles.Contains(p.Name)).Any(p => p.MFAEnabled) && !string.IsNullOrEmpty(MFAOTP))
                    {
                        //Valiadate The OTP and return Error if Invalid
                        if(UserDetails.OTP!=MFAOTP)
                        {
                            return Ok("Invalid MFA code entered");
                        }
                        else if (UserDetails.OTP == MFAOTP && UserDetails.OTPValidUpto < DateTime.UtcNow)
                        {
                            return Ok("MFA code expired. Please generate new code from login screen.");
                        }
                    }

                        //if user is Authenticated check if his password is expired or not
                        var LPasswordHistory = db.LPasswordHistories.Where(p => p.UserId == xx.Id).ToList();
                    var PasswordExpiryDate = DateTime.UtcNow;
                    if(LPasswordHistory.Count()>0&&PasswordPolicies!=null)
                    {
                        Nullable<DateTime> MaxPasswordResetDate = LPasswordHistory.Max(p => p.CreatedDateTime);
                        PasswordExpiryDate = MaxPasswordResetDate.Value.AddDays(PasswordPolicies.MaxAgeDays);
                    }
                    /*If reached here check that the Expiry  date has exceeded today's date then redirect user to reset password screen
                     Either Password is expired or User is Logging in first time*/
                    if(PasswordExpiryDate<DateTime.UtcNow||LPasswordHistory.Count()==0)
                    {
                        return Ok("reset password");
                    }

                    //get list of roles(Name and Id ) from Aspnet roles which are assigned to user. SS also added ShowOnDashboard on 16Feb2018
                    var AspRolesList = (List<AspnetRoleViewModel>)db.AspNetRoles.Where(p => roles.Contains(p.Name)).Where(p => p.CompanyCode == Company.GcCode).Select(p => new AspnetRoleViewModel { Id = p.Id, Name = p.Name,ShowDashboard=p.ShowDashboard }).ToList();
                    //var UserDetails = db.LUsers.Where(p => p.LuUserId == xx.Id).FirstOrDefault();
                    //Get User details from LUsers
                    string FirstName = "";
                    string LastName = "";
                    string PhoneNumber = "";
                    int LUserId = 0;
                    var UserDetail = db.LUsers.Where(p => p.LuUserId == xx.Id).FirstOrDefault();
                    var SecurityQuestionAnswer = db.MAspnetUsersGSecurityQuestions.Where(x => x.MAuqsqUserId == xx.Id).ToList();
                    //var PayeeDetails = db.LPayees.Where(p => p.LpUserId == xx.Id).FirstOrDefault();
                    //RK 17042017 changed the condition to pick data based on email id as LpUserId may be null in case of payee.
                    var PayeeDetails = db.LPayees.Where(p => p.LpEmail == xx.Email).FirstOrDefault();
                    if (UserDetail != null)//if he/she is a user
                    {
                        LUserId = UserDetail.Id;
                        FirstName = UserDetail.LuFirstName;
                        LastName = UserDetail.LuLastName;
                        PhoneNumber = UserDetail.LuPhone;
                    }
                    else if (PayeeDetails != null)//if he/she is a payee
                    {
                        LUserId = PayeeDetails.Id;

                        var Query = "Exec USPGetLPayeeById @Id";
                        SqlCommand cmd = new SqlCommand(Query);
                        cmd.Parameters.AddWithValue("@Id", LUserId);
                        DataSet ds = Globals.GetData(cmd);
                        if (ds.Tables[0].Rows.Count == 1)
                        {
                            FirstName = Convert.ToString(ds.Tables[0].Rows[0]["LpFirstName"]);
                            LastName = Convert.ToString(ds.Tables[0].Rows[0]["LpLastName"]);
                            PhoneNumber = Convert.ToString(ds.Tables[0].Rows[0]["LpPhone"]);
                        }
                    }
                    if (xx.IsActive)
                    {
                        return Ok(new LoginViewModel
                        {
                            // Email = User.Identity.GetUserName(),
                            Email = xx.Email,
                            GcCompanyId = xx.GcCompanyId,
                            Roles = AspRolesList,
                            Id = xx.Id,
                            FirstName = FirstName,
                            LastName = LastName,
                            PhoneNumber = PhoneNumber,
                            LUserId= LUserId,
                            PolicyAccepted=UserDetails.PolicyAccepted,
                            Password=Password,
                            ObjScurityQuestion = SecurityQuestionAnswer
                            //IsManager = UserDetails.LuIsManager,
                            //FullName = UserDetails.LuFirstName + " " + UserDetails.LuLastName
                        });
                    }
                    else
                    {
                        return Ok("User is InActive");
                    }
                }
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Your account is locked and will be unlocked once lockout period (30 minutes) is over"));
                }
            }
            catch (Exception ex)
            {//"The UserName is not registered"
             // throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex));//ex
                throw ex;
            }
        }

        [HttpPost]
        [OverrideAuthorization]
        public IHttpActionResult VerifyMFAOtp(string Email,string MFAOTP)
        {
            var users = UserManager.FindByEmail(Email);
            var UserDetails = db.AspNetUsers.Where(a => a.Email == Email).FirstOrDefault();
            var xx = users;
                //Valiadate The OTP and return Error if Invalid
                if (UserDetails.OTP != MFAOTP)
                {
                    return Ok("Invalid MFA code entered");
                }
                else if (UserDetails.OTP == MFAOTP && UserDetails.OTPValidUpto < DateTime.UtcNow)
                {
                    return Ok("MFA code expired. Please generate new code from login screen.");
                }
            var roles = (List<string>)UserManager.GetRolesAsync(xx.Id).Result;
            var PasswordPolicies = db.LPasswordPolicies.Where(p => p.CompanyId == xx.GcCompanyId).FirstOrDefault();

            //if user is Authenticated check if his password is expired or not
            var LPasswordHistory = db.LPasswordHistories.Where(p => p.UserId == xx.Id).ToList();
            var PasswordExpiryDate = DateTime.UtcNow;
            if (LPasswordHistory.Count() > 0 && PasswordPolicies != null)
            {
                Nullable<DateTime> MaxPasswordResetDate = LPasswordHistory.Max(p => p.CreatedDateTime);
                PasswordExpiryDate = MaxPasswordResetDate.Value.AddDays(PasswordPolicies.MaxAgeDays);
            }
            /*If reached here check that the Expiry  date has exceeded today's date then redirect user to reset password screen
             Either Password is expired or User is Logging in first time*/
            if (PasswordExpiryDate < DateTime.UtcNow || LPasswordHistory.Count() == 0)
            {
                return Ok("reset password");
            }

            //get list of roles(Name and Id ) from Aspnet roles which are assigned to user. SS also added ShowOnDashboard on 16Feb2018
            var AspRolesList = (List<AspnetRoleViewModel>)db.AspNetRoles.Where(p => roles.Contains(p.Name)).Where(p => p.CompanyCode == UserDetails.GCompany.GcCode).Select(p => new AspnetRoleViewModel { Id = p.Id, Name = p.Name, ShowDashboard = p.ShowDashboard }).ToList();
            //var UserDetails = db.LUsers.Where(p => p.LuUserId == xx.Id).FirstOrDefault();
            //Get User details from LUsers
            string FirstName = "";
            string LastName = "";
            string PhoneNumber = "";
            int LUserId = 0;
            var UserDetail = db.LUsers.Where(p => p.LuUserId == xx.Id).FirstOrDefault();
            var SecurityQuestionAnswer = db.MAspnetUsersGSecurityQuestions.Where(x => x.MAuqsqUserId == xx.Id).ToList();
            //var PayeeDetails = db.LPayees.Where(p => p.LpUserId == xx.Id).FirstOrDefault();
            //RK 17042017 changed the condition to pick data based on email id as LpUserId may be null in case of payee.
            var PayeeDetails = db.LPayees.Where(p => p.LpEmail == xx.Email).FirstOrDefault();
            if (UserDetail != null)//if he/she is a user
            {
                LUserId = UserDetail.Id;
                FirstName = UserDetail.LuFirstName;
                LastName = UserDetail.LuLastName;
                PhoneNumber = UserDetail.LuPhone;
            }
            else if (PayeeDetails != null)//if he/she is a payee
            {
                LUserId = PayeeDetails.Id;

                var Query = "Exec USPGetLPayeeById @Id";
                SqlCommand cmd = new SqlCommand(Query);
                cmd.Parameters.AddWithValue("@Id", LUserId);
                DataSet ds = Globals.GetData(cmd);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    FirstName = Convert.ToString(ds.Tables[0].Rows[0]["LpFirstName"]);
                    LastName = Convert.ToString(ds.Tables[0].Rows[0]["LpLastName"]);
                    PhoneNumber = Convert.ToString(ds.Tables[0].Rows[0]["LpPhone"]);
                }
            }
            if (xx.IsActive)
            {
                return Ok(new LoginViewModel
                {
                    // Email = User.Identity.GetUserName(),
                    Email = xx.Email,
                    GcCompanyId = xx.GcCompanyId,
                    Roles = AspRolesList,
                    Id = xx.Id,
                    FirstName = FirstName,
                    LastName = LastName,
                    PhoneNumber = PhoneNumber,
                    LUserId = LUserId,
                    PolicyAccepted = UserDetails.PolicyAccepted,
                    ObjScurityQuestion = SecurityQuestionAnswer
                    //Password = Password  // SG - 15 Feb2019 - Password does not seem to be required further. so commenting it.
                    //IsManager = UserDetails.LuIsManager,
                    //FullName = UserDetails.LuFirstName + " " + UserDetails.LuLastName
                });
            }
            else
            {
                return Ok("User is InActive");
            }
        
        }


        [HttpGet]
        [AllowAnonymous]
        public string getBannerText()
        {
            var text = "";
            try
            {
                 text = Globals.getBannerValue();
                
            }
            catch (Exception ex)
            {

                text = "System Outage, Please watch out this space for updates";
            }
            return text;

        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetSelectedLandingPage(string Userid)
        {
            string Rolename = string.Empty;
            var UserDetails = db.AspNetUserRoles.Where(p => p.UserId.Equals(Userid) & p.IsDefault == true).FirstOrDefault();
            if (UserDetails != null)
            {
                Rolename = db.AspNetRoles.Where(p => p.Id == UserDetails.RoleId).Select(p => p.Name).FirstOrDefault();
            }
            return Ok(Rolename);


        }
            [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult UpdateSelectedLandingPage(string Userid, string RoleName, int CountryID)
        {
            
            var CompanyGcCode = db.GCompanies.Where(x => x.Id == CountryID).Select(x => x.GcCode).FirstOrDefault();
            var RoleID = db.AspNetRoles.Where(x => x.Name == RoleName & x.CompanyCode == CompanyGcCode).Select(x=>x.Id).FirstOrDefault();

            var AspNetUserRolesList = db.AspNetUserRoles.Where(p => p.UserId == Userid).ToList(); 
            foreach(AspNetUserRole obj in AspNetUserRolesList)
            {
                obj.IsDefault = false;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }


            var AspNetUserRoles = db.AspNetUserRoles.Where(p => p.UserId == Userid & p.RoleId == RoleID).FirstOrDefault();
            AspNetUserRoles.IsDefault = true;
            db.Entry(AspNetUserRoles).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetUserInformation(string Email)
        {
            var users = UserManager.FindByEmail(Email);
            if (users == null)
            {
                return Ok("User not registered");
            }
            var UserDetails = db.AspNetUsers.Where(p => p.Email.Equals(Email)).FirstOrDefault();
            //Get details of opco
            var Company = db.GCompanies.Find(UserDetails.GcCompanyId);
            //Get Roles
            var xx = users;
            var roles = (List<string>)UserManager.GetRolesAsync(xx.Id).Result;
            
            //get list of roles(Name and Id ) from Aspnet roles which are assigned to user. SS also added ShowOnDashboard on 16Feb2018
            var AspRolesList = (List<AspnetRoleViewModel>)db.AspNetRoles.Where(p => roles.Contains(p.Name)).Where(p => p.CompanyCode == Company.GcCode).Select(p => new AspnetRoleViewModel { Id = p.Id, Name = p.Name, ShowDashboard = p.ShowDashboard }).ToList();
            //var UserDetails = db.LUsers.Where(p => p.LuUserId == xx.Id).FirstOrDefault();
            //Get User details from LUsers
            string FirstName = "";
            string LastName = "";
            string PhoneNumber = "";
            int LUserId = 0;
            var UserDetail = db.LUsers.Where(p => p.LuUserId == xx.Id).FirstOrDefault();
            var SecurityQuestionAnswer = db.MAspnetUsersGSecurityQuestions.Where(x => x.MAuqsqUserId == xx.Id).ToList();
            //var PayeeDetails = db.LPayees.Where(p => p.LpUserId == xx.Id).FirstOrDefault();
            //RK 17042017 changed the condition to pick data based on email id as LpUserId may be null in case of payee.
            var PayeeDetails = db.LPayees.Where(p => p.LpEmail == xx.Email).FirstOrDefault();
            if (UserDetail != null)//if he/she is a user
            {
                LUserId = UserDetail.Id;
                FirstName = UserDetail.LuFirstName;
                LastName = UserDetail.LuLastName;
                PhoneNumber = UserDetail.LuPhone;
            }
            else if (PayeeDetails != null)//if he/she is a payee
            {
                LUserId = PayeeDetails.Id;

                var Query = "Exec USPGetLPayeeById @Id";
                SqlCommand cmd = new SqlCommand(Query);
                cmd.Parameters.AddWithValue("@Id", LUserId);
                DataSet ds = Globals.GetData(cmd);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    FirstName = Convert.ToString(ds.Tables[0].Rows[0]["LpFirstName"]);
                    LastName = Convert.ToString(ds.Tables[0].Rows[0]["LpLastName"]);
                    PhoneNumber = Convert.ToString(ds.Tables[0].Rows[0]["LpPhone"]);
                }
            }
            if (xx.IsActive)
            {
                return Ok(new LoginViewModel
                {
                    // Email = User.Identity.GetUserName(),
                    Email = xx.Email,
                    GcCompanyId = xx.GcCompanyId,
                    Roles = AspRolesList,
                    Id = xx.Id,
                    FirstName = FirstName,
                    LastName = LastName,
                    PhoneNumber = PhoneNumber,
                    LUserId = LUserId,
                    PolicyAccepted = UserDetails.PolicyAccepted,
                    ObjScurityQuestion = SecurityQuestionAnswer
                    //IsManager = UserDetails.LuIsManager,
                    //FullName = UserDetails.LuFirstName + " " + UserDetails.LuLastName
                });
            }
            else
            {
                return Ok("User is InActive");
            }
        }

        //Update User Accepting Policies
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult UpdatePolicyAccepted(string UserId)
        {
            var AspNetUserDetail = db.AspNetUsers.Where(p => p.Id == UserId).FirstOrDefault();
            AspNetUserDetail.PolicyAccepted = true;
            db.Entry(AspNetUserDetail).State = EntityState.Modified;
            db.SaveChanges();
            return Ok();
        }


        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword --used already existing method to change password
        [Route("ChangePassword")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(model.UserId, model.OldPassword,
                model.NewPassword);
            
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
       // public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        public async Task<IHttpActionResult> SetPassword(ADUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //default code is commentedd out as AD account is being used and AD password is to be updated
            //IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            //if (!result.Succeeded)
            //{
            //    return GetErrorResult(result);
            //}
            AuthenticationResult result = new AuthenticationResult();

           result = Globals.SetUserPassword(model);
            if (result.IsSuccess)
            {
                return Ok(true);
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.ErrorMessage));
            }


            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                
                 ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        //This method has been changed by shubham to register user in asp net user along with its role
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email,GcCompanyId=model.GcCompanyId,IsActive=true };//Is Active flag is set to true if a user is registered from UI
            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            //lines added by shubham to add roles to user after registering
             var RegUser = UserManager.FindByEmail(user.Email);
            foreach (var role in model.Roles)
            {
                UserManager.AddToRole(RegUser.Id, role);
            }
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok(new {UserId=RegUser.Id });
        }

        //This method is defined to register a new user programatically whereever needed by other controllers
        public string RegisterNewUser(RegisterBindingModel model)
        {
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, GcCompanyId = model.GcCompanyId, IsActive = model.IsActive.Value };
            IdentityResult result = UserManager.Create(user, model.Password);
            var RegisteredUser = UserManager.FindByEmail(user.Email);
            foreach (var role in model.Roles)
            {
                UserManager.AddToRole(RegisteredUser.Id, role);
            }
            return RegisteredUser.Id;
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result); 
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
        
       


        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
