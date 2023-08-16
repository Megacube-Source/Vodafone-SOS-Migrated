using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Http;
using UserAccessManagement.Models;

namespace WIAM_SOS.Utilities
{
    public class Globals
    {
        private static SosDbEntities db = new SosDbEntities();
        public static string UserInWF { get { return "Currently User in approval workflow. Please perform this action after User is approved in SOS"; } }
        public static string UserDoesNotExistinSOS { get { return "User does not exist in SOS"; } }
        public static string UserDoesNotExistinAD { get { return "User does not exist in AD"; } }
        
        public static string UserRejected { get { return "User already Exist as Rejected"; } }
        public static string UserTerminated { get { return "User already Exist as Terminated. Please re-hire in SOS first."; } }
        public static string ProcessingSuccess { get { return "Request processed successfully."; } }
        public static string UserAlreadyEnabled { get { return "AD Account already exists as Enabled"; } }
        public static string UserAlreadyDisabled { get { return "AD Account already exists as Disabled"; } }
        public static string ADServerUnavailable { get { return "AD Server is temporarily unavailable"; } }
        public static string SomethingElseFailedInDBErrorMessage { get { return "Oops! Something went wrong. The issue has been reported and will be resolved soon. You can reach L2 Admin with error reference #{0}"; } }
        public static string NewEmailAlreadyExist {get{return "Email cannot be changed as new email is already taken"; } }
        public enum ResponseCodes
        {
            SomethingWentWrong = 560,
            InvalidJson = 561,
            InvalidOperation = 562,
            InvalidCompanyCode = 563,
            InvalidEmail = 564,
            InvalidPayeeCode = 565,
            InvalidUserType = 567,
            InvalidPhone = 568,
            InvalidName = 569,
            InvalidUserGroup = 570,

            UserDoesNotExist = 571,
            UserTerminated = 572,
            UserAlreadyEnabled = 573,
            UserAlreadyDisabled = 574,
            ADUserDoesNotExist = 575,
            ADNewEmailAlreadyTaken = 576,
            UserInWF = 577,
            UserRejected = 578,
            OldNewEmailSame = 579,
            ADServerUnavailable = 580,
            Ok = 200,
            RequestCouldNotBeProcessed = 581,
        };

        public static void UpdateLobbyAndSendResponse(LUserLobby lobby,string Status, string ResponseCode,string ResponseMessage,string Comments)
        {
            lobby.Status = Status;
            lobby.ResponseCode = ResponseCode;
            lobby.Response = Status;
            lobby.ResponseMessage = ResponseMessage;
            lobby.Comments = Comments;
            db.LUserLobbies.Add(lobby);
            db.SaveChanges();
        }
        public static string GetProjectEnv()
        {
             return db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
        }

        //This method validates the CompanyCode that has been received 
        public static string ValidateCompanyCode(string CompanyCode)
        {
            string ReturnMsg = null;
            //Check for nullablility
            if (string.IsNullOrEmpty(CompanyCode))
            {
                ReturnMsg = "Company Code is mandatory";
            }
            //check for perissible length
            else 
            {
                int LengthOfCCInDb = 2; //Another option is to get  the length of CC column from DB
                if (CompanyCode.Length != LengthOfCCInDb)
                {
                    ReturnMsg = "Invalid Company Code";
                }
                else
                {
                    //check whether CC exists in DB
                    var CompanyDetails = db.GCompanies.Where(a => a.GcCode == CompanyCode).FirstOrDefault();
                    if (CompanyDetails == null)
                    {
                        ReturnMsg = "Invalid Company Code";
                    }
                }
            }
            if (!string.IsNullOrEmpty(ReturnMsg))
                return ReturnMsg;
            else
                return "Success";
        }

        //This method validates the Email received 
        public static string ValidateEmailAddress(string Email, string param)
        {
            string ReturnMsg = null;
            //Check for nullablility
            if (string.IsNullOrEmpty(Email))
            {
                ReturnMsg = param +"Email is mandatory";
            }
            //check for perissible length
            else
            {
                int LengthOfEmailInDb = 255; //Another option is to get  the length of Email column from DB
                if (Email.Length > LengthOfEmailInDb)
                {
                    ReturnMsg = "Invalid "+ param + "Email";
                }
                else
                {
                    if(!IsEmailValid(Email))
                    {
                        ReturnMsg = "Invalid " + param + "Email";
                    }
                }
            }
            if (!string.IsNullOrEmpty(ReturnMsg))
                return ReturnMsg;
            else
                return "Success";
        }

        public static string ValidateRequestorEmail(string RequestorEmail)
        {
            string ReturnMsg = null;
            if (!string.IsNullOrEmpty(RequestorEmail))
            {
                int LengthOfReqEmailInDb = 255; //Another option is to get  the length of Email column from DB
                if (RequestorEmail.Length > LengthOfReqEmailInDb)
                {
                    ReturnMsg = "Invalid Requestor Email";
                }
                else
                {
                    if (!IsEmailValid(RequestorEmail))
                    {
                        ReturnMsg = "Invalid Requestor Email";
                    }
                }
            }
            if (!string.IsNullOrEmpty(ReturnMsg))
                return ReturnMsg;
            else
                return "Success";
        }

        public static string ValidateManagerEmail(string ManagerEmail)
        {
            string ReturnMsg = null;
            if (!string.IsNullOrEmpty(ManagerEmail))
            {
                int LengthOfMgrEmailInDb = 255; //Another option is to get  the length of Email column from DB
                if (ManagerEmail.Length > LengthOfMgrEmailInDb)
                {
                    ReturnMsg = "Invalid Manager Email";
                }
                else
                {
                    if (!IsEmailValid(ManagerEmail))
                    {
                        ReturnMsg = "Invalid Manager Email";
                    }
                }
            }
            if (!string.IsNullOrEmpty(ReturnMsg))
                return ReturnMsg;
            else
                return "Success";
        }

        public static string ValidateUserType(string UserType)
        {
            string ReturnMsg = null;
           //possible values - Finops /Payee
            if ( (!("finops".Equals(UserType.ToLower())) && !("payee".Equals(UserType.ToLower()))) )
            {
                ReturnMsg = "Invalid UserType.";
            }
            if (!string.IsNullOrEmpty(ReturnMsg))
                return ReturnMsg;
            else
                return "Success";
        }
        public static string ValidatePayeeCode(string PayeeCode)
        {
            string ReturnMsg = null;
            if (!string.IsNullOrEmpty(PayeeCode))
            {
                int LengthOfPayeeCodeInDb = 255;
                if (PayeeCode.Length > LengthOfPayeeCodeInDb)
                {
                    ReturnMsg = "Invalid PayeeCode";
                }
                //special characters not allowed
                var regexItem = new Regex("^[a-zA-Z0-9_]*$"); 
                if (!regexItem.IsMatch(PayeeCode))
                {
                    ReturnMsg = "Invalid PayeeCode";
                }
            }
            if (!string.IsNullOrEmpty(ReturnMsg))
                return ReturnMsg;
            else
                return "Success";
        }
        public static string ValidateUserGroup(string UserGroup,string param,bool CheckinAD)
        {
            string ReturnMsg = null;
            if (!string.IsNullOrEmpty(UserGroup))
            {
                int LengthInDb = 255;
                if (UserGroup.Length > LengthInDb)
                {
                    ReturnMsg = "Invalid " + param + "UserGroup";
                }
                else if (CheckinAD )
                {
                    if (!ActiveDirectory.CheckGroupExistence(UserGroup))
                        ReturnMsg = param + "UserGroup does not exist in AD";
                }
            }

            if (!string.IsNullOrEmpty(ReturnMsg))
                return ReturnMsg;
            else
                return "Success";
        }
        public static string ValidatePhone(string Phone)
        {
            string ReturnMsg = null;
            
                int LengthOfPhoneInDb = 20; //Another option is to get  the length of Phone column from DB
                if (Phone.Length > LengthOfPhoneInDb)
                {
                    ReturnMsg = "Invalid Phone";
                }
                //else
                //{
                //    if (!Regex.IsMatch(Phone, @"\d"))
                //    {
                //        ReturnMsg = "Invalid Phone";
                //    }
                //}
            if (!string.IsNullOrEmpty(ReturnMsg))
                return ReturnMsg;
            else
                return "Success";
        }
        public static string ValidateName(string Name, string param)
        {
            string ReturnMsg = null;

            int LengthOfNameInDb = 255; //Another option is to get  the length of Name column from DB
            if (Name.Length > LengthOfNameInDb)
            {
                ReturnMsg = "Invalid" + param ;
            }
            if (!string.IsNullOrEmpty(ReturnMsg))
                return ReturnMsg;
            else
                return "Success";
        }
        
        public static bool CheckUserExistence(string UserType)
        {
            if ("NotFound".Equals(UserType))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        public static bool CheckUserTermination(string WFStatus)
        {
            bool IsTerminated = false;
            if ("inactive".Equals(WFStatus.ToLower()))
            {
                IsTerminated = true;
            }
            else if ("completed".Equals(WFStatus.ToLower()) || "suspended".Equals(WFStatus.ToLower()))
            {
                IsTerminated = false;
            }
            //this else is kept in case we have some other condition as well
            else
            {
                IsTerminated = false;
            }
            return IsTerminated;
        }
        public static bool CheckUserEnability(bool CreateLoginFlag)
        {
            if (CreateLoginFlag)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //This method checks the Lobby table for Duplicate requests. If it is dupicate returns true else returns false.
        public static bool IsDuplicateRequest(LUserLobby DuplicacyCheckObj)
        {
            bool IsDuplicate = false;
            LUserLobby UserLobby = null;
            switch (DuplicacyCheckObj.RequestType)
            {
                case "Enable":
                case "Disable":
                case "Revoke":
                  UserLobby=  db.LUserLobbies.Where(p => p.Email == DuplicacyCheckObj.Email).Where(p => p.CompanyCode == DuplicacyCheckObj.CompanyCode)
                            .Where(p => p.RequestType == DuplicacyCheckObj.RequestType).Where(p => p.Status == DuplicacyCheckObj.Status).FirstOrDefault();
                    break;
            }
            if (UserLobby == null)
            {
                IsDuplicate = false;
            }
            else
            {
                IsDuplicate = true;
            }
            return IsDuplicate;
        }

        public static void TerminateUser(string Email)
        {
            try
            {
                //Also Delete entry from LPasswordHistory for this User so that user can be redirected to reset password when this AD account is created again in future.
                var AspnetUserDetail = db.AspNetUsers.Where(p => p.Email == Email).FirstOrDefault();
                if (AspnetUserDetail != null)
                {
                    
                    AspnetUserDetail.IsActive = false;
                    db.Entry(AspnetUserDetail).State = EntityState.Modified;
                    db.SaveChanges();
                    //Commenting on 20Aug19 by ShivaniG, As per email and discussion for sync up WIAM/non-WIAM
                    //var companycode = db.GCompanies.Where(x => x.Id == AspnetUserDetail.GcCompanyId).FirstOrDefault();
                    //var PasswordHistory = db.LPasswordHistories.Where(p => p.UserId == AspnetUserDetail.Id);
                    //db.LPasswordHistories.RemoveRange(PasswordHistory);
                    ////Also REmove the security question answer
                    //var SecurityQuestionsData = db.MAspnetUsersGSecurityQuestions.Where(x => x.MAuqsqUserId == AspnetUserDetail.Id);
                    //db.MAspnetUsersGSecurityQuestions.RemoveRange(SecurityQuestionsData);
                    //db.SaveChanges();
                    //db.Database.ExecuteSqlCommand("Update  XSchema" + companycode.GcCode.ToUpper() + ".XReportUsers set  XUserEmailID = XUserEmailID + 'X' where XUserEmailID='" + AspnetUserDetail.Email + "'");
                }
            }
            catch(Exception ex)
            {

            }
        }
        public static void UpdateAspUser(string OldEmail,string NewEmail,string CompanyCode)
        {
            //31Aug2020 - R3.1 - Somehow CompanyId was not correct in AspnetUsers table in case of Rehire. So, Converted to Direct Update SQL query so as not to update other columns
            db.Database.ExecuteSqlCommand("update AspNetUsers set IsActive= 1,Email = {1},UserName = {1} " +
                "where Email = {0}", OldEmail,NewEmail);
            db.SaveChanges();

            //var AspNetUser = db.AspNetUsers.Where(a => a.Email == OldEmail).FirstOrDefault();
            //if (AspNetUser != null)
            //{
            //    if (!OldEmail.Equals(NewEmail))//update Emails when different
            //    {
            //        AspNetUser.Email = NewEmail;
            //        AspNetUser.UserName = NewEmail;
            //    }
                
            //    AspNetUser.IsActive = true;//update IsActive to  1
            //    db.Entry(AspNetUser).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            string sql = "Update  XSchema" + CompanyCode.ToUpper() + ".XReportUsers set  XUserEmailID = {0}  where XUserEmailID= {1}";
            //db.Database.ExecuteSqlCommand("Update  XSchema" + CompanyCode.ToUpper() + ".XReportUsers set  XUserEmailID = " +model.NewEmail + " where XUserEmailID='" + model.OldEmail + "'");
            db.Database.ExecuteSqlCommand(sql, NewEmail, OldEmail);
        }

        public static bool IsEmailValid(string Email)
        {

            //Use C# standard mechanism for email validation 
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(Email);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //This method is using SMTP client to send emails directly, without db interaction(In some cases, when DB might fail)
        public static bool SendExceptionEmail(string EmailBody)
        {
            try
            {
                String FROM = ConfigurationManager.AppSettings["SenderEmailId"];   //This address must be verified.
                // Supply your SMTP credentials below. Note that your SMTP credentials are different from your AWS credentials.
                String SMTP_USERNAME = ConfigurationManager.AppSettings["SmtpLoginId"];  //SMTP username. 
                String SMTP_PASSWORD = ConfigurationManager.AppSettings["SmtpPassword"];  //SMTP password.

                // Amazon SES SMTP host name. This example uses the US West (Oregon) region.
                String HOST = ConfigurationManager.AppSettings["SmtpHost"];

                // The port you will connect to on the Amazon SES SMTP endpoint. We are choosing port 587 because we will use
                // STARTTLS to encrypt the connection.
                int PORT = Convert.ToInt32(ConfigurationManager.AppSettings["PortNumber"]);

                // Create an SMTP client with the specified host name and port.
                using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(HOST, PORT))
                {
                    // Create a network credential with your SMTP user name and password.
                    client.Credentials = new System.Net.NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                    // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
                    // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
                    client.EnableSsl = true;
                    string ToAddress = ConfigurationManager.AppSettings["ExceptionEmailTo"];
                    string EmailSubject = ConfigurationManager.AppSettings["ExceptionEmailSubject"];
                    MailMessage message = new MailMessage(FROM, ToAddress, EmailSubject, EmailBody);
                    message.IsBodyHtml = true;
                    //client.Send(FROM, ToAddress, EmailSubject, EmailBody);
                    client.Send(message);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}