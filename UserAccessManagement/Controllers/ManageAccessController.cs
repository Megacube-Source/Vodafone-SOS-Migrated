using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UserAccessManagement.Models;
using WIAM_SOS.Utilities;
using WIAM_SOS.ViewModel;
using static WIAM_SOS.Utilities.ActiveDirectory;

namespace WIAM_SOS.Controllers
{
    [CustomExceptionFilter]
    public class ManageAccessController : ApiController
    {
        private SosDbEntities db = new SosDbEntities();
        private LUserLobby lobby = new LUserLobby { CompanyCode = "NA", IsVFADUser = false, Email="NA" ,CreatedBy="WIAM",UpdatedBy="WIAM",
            CreatedDateTime = DateTime.UtcNow , UpdatedDateTime = DateTime.UtcNow ,
            UserType = "NA" , Status="New", RequestType="NA"};
        [HttpPost]
        [Route("webservice")]
        public IHttpActionResult webservice(IndexViewModel model)
        {
            if (model == null)
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidJson.ToString(), "Invalid JSON", null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidJson, "Invalid JSON"));
            }
            //populate lobby object from incoming details
                lobby.CompanyCode = string.IsNullOrEmpty(model.CompanyCode) ? null : model.CompanyCode.Length > 2 ? model.CompanyCode.Substring(0,2):model.CompanyCode;
                lobby.FirstName = string.IsNullOrEmpty(model.FirstName) ? null : model.FirstName.Length > 255 ? model.FirstName.Substring(0,255): model.FirstName;
                lobby.LastName = string.IsNullOrEmpty(model.LastName) ? null : model.LastName.Length > 255 ? model.LastName.Substring(0,255):model.LastName;
                lobby.Email = string.IsNullOrEmpty(model.Email) ? null : model.Email.Length> 255 ? model.Email.Substring(0,255) : model.Email;
                lobby.Phone = string.IsNullOrEmpty(model.Phone) ? null : model.Phone.Length > 20 ? model.Phone.Substring(0,20) : model.Phone;
                lobby.UserType = string.IsNullOrEmpty(model.UserType) ? "NA" : model.UserType.Length > 255 ? model.UserType.Substring(0,255) : model.UserType;
                lobby.PayeeCode = string.IsNullOrEmpty(model.PayeeCode) ? null : model.PayeeCode.Length > 255 ? model.PayeeCode.Substring(0,255): model.PayeeCode;
                lobby.IsVFADUser =  model.VFADUser == null ? false :(bool) model.VFADUser;
                lobby.Status = "New";
                lobby.UserGroup = string.IsNullOrEmpty(model.UserGroup) ? null : model.UserGroup.Length > 255 ? model.UserGroup.Substring(0,255): model.UserGroup;
                lobby.NewUserGroup = string.IsNullOrEmpty(model.NewUserGroup) ? null : model.NewUserGroup.Length > 255 ? model.NewUserGroup.Substring(0,255): model.NewUserGroup;
                lobby.ManagerEmail = string.IsNullOrEmpty(model.ManagerEmail) ? null : model.ManagerEmail.Length > 255 ? model.ManagerEmail.Substring(0,255): model.ManagerEmail;
                lobby.RequestorEmail = string.IsNullOrEmpty(model.RequestorEmail) ? null : model.RequestorEmail.Length > 255 ? model.RequestorEmail.Substring(0,255): model.RequestorEmail;
                lobby.RequestType = model.OperationType;
                lobby.CreatedBy = "WIAM";
                lobby.CreatedDateTime = DateTime.UtcNow;
                lobby.UpdatedBy = "WIAM";
                lobby.UpdatedDateTime = DateTime.UtcNow;
                lobby.UpdatedByRoleId = null;
                lobby.Comments = null;
            string ProjectEnvironment = Globals.GetProjectEnv();
            string ResponseMessage = Globals.ProcessingSuccess;
            if (string.IsNullOrEmpty(model.OperationType))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidOperation.ToString(), "OperationType Cannot be null", null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidOperation, "OperationType Cannot be null"));
            }
            switch (model.OperationType.ToLower())
            {
                case "create":
                    CreateViewModel CreateViewModel = new CreateViewModel { Email = model.Email,CompanyCode = model.CompanyCode,FirstName = model.FirstName,
                        LastName = model.LastName, Phone= model.Phone, VFADUser = model.VFADUser == null ? false : (bool)model.VFADUser, PayeeCode = model.PayeeCode, ManagerEmail = model.ManagerEmail,
                        RequestorEmail = model.RequestorEmail, UserGroup = model.UserGroup, UserType = model.UserType};
                    ResponseMessage = Create(CreateViewModel,lobby, ProjectEnvironment);
                    break;
                case "enable":
                    EnableViewModel EnableViewModel = new EnableViewModel { Email = model.Email, RequestorEmail = model.RequestorEmail, CompanyCode = model.CompanyCode };
                    ResponseMessage=Enable(EnableViewModel,lobby, ProjectEnvironment);
                    break;
                case "disable":
                    DisableViewModel DisableViewModel = new DisableViewModel { Email = model.Email, RequestorEmail = model.RequestorEmail, CompanyCode = model.CompanyCode };
                    ResponseMessage = Disable(DisableViewModel,lobby, ProjectEnvironment);
                    break;
                case "update":
                    UpdateViewModel UpdateViewModel = new UpdateViewModel { OldEmail = model.OldEmail, NewEmail =  model.NewEmail, RequestorEmail = model.RequestorEmail };
                    Update(UpdateViewModel,lobby, ProjectEnvironment);
                    break;
                case "revoke":
                    RevokeViewModel RevokeViewModel = new RevokeViewModel { Email = model.Email, RequestorEmail = model.RequestorEmail};
                    ResponseMessage = Revoke(RevokeViewModel,lobby, ProjectEnvironment);
                    break;
                case "setusergroup":
                    SetUserGroupViewModel UserGroupViewModel = new SetUserGroupViewModel { Email = model.Email, RequestorEmail = model.RequestorEmail,OldUserGroup= model.OldUserGroup, NewUserGroup = model.NewUserGroup };
                    ResponseMessage = SetUserGroup(UserGroupViewModel, lobby, ProjectEnvironment);
                    break;

                default:
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidOperation.ToString(), "OperationType not supported", null);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidOperation, "OperationType not supported"));
            }
            return Ok(ResponseMessage);
        }
        private string Create(CreateViewModel model, LUserLobby lobby, string ProjectEnvironment)
        {
            #region Validation for Input data
            //CompanyCode(2char only, SOS DB existence)
            string CCValidationMsg = Globals.ValidateCompanyCode(model.CompanyCode);
            if (!"Success".Equals(CCValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidCompanyCode.ToString(), CCValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidCompanyCode, CCValidationMsg));
            }

            int CompanyId = db.GCompanies.Where(a => a.GcCode == model.CompanyCode).Select(a => a.Id).FirstOrDefault();
            string UserType = model.UserType;

            //Validate Email Address (structure only)
            string EmailValidationMsg = Globals.ValidateEmailAddress(model.Email, "");
            if (!"Success".Equals(EmailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), EmailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, EmailValidationMsg));
            }
            //Validate Phone (Regex,Length only)
            if (!string.IsNullOrEmpty(model.Phone))
            {
                string PhoneValidationMsg = Globals.ValidatePhone(model.Phone);
                if (!"Success".Equals(PhoneValidationMsg))
                {
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidPhone.ToString(), PhoneValidationMsg, null);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidPhone, PhoneValidationMsg));
                }
            }
            //Validate FirstName(Regex,Length only)
            if (!string.IsNullOrEmpty(model.FirstName))
            {
                string FNameValidationMsg = Globals.ValidateName(model.FirstName, " First Name");
                if (!"Success".Equals(FNameValidationMsg))
                {
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidName.ToString(), FNameValidationMsg, null);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidName, FNameValidationMsg));
                }
            }
            //Validate LastName(Regex,Length only)
            if (!string.IsNullOrEmpty(model.LastName))
            {
                string LNameValidationMsg = Globals.ValidateName(model.LastName, " Last Name");
                if (!"Success".Equals(LNameValidationMsg))
                {
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidName.ToString(), LNameValidationMsg, null);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidName, LNameValidationMsg));
                }
            }
            //Validate Manager Email Address(Structure only)
            if (!string.IsNullOrEmpty(model.ManagerEmail))
            {
                string ManagerEmailValidationMsg = Globals.ValidateManagerEmail(model.ManagerEmail);
                if (!"Success".Equals(ManagerEmailValidationMsg))
                {
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), ManagerEmailValidationMsg, null);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, ManagerEmailValidationMsg));
                }
            }
            //Validate RequestorEmail Address(Structure only)
            if (!string.IsNullOrEmpty(model.RequestorEmail))
            {
                string RequesterEmailValidationMsg = Globals.ValidateRequestorEmail(model.RequestorEmail);
                if (!"Success".Equals(RequesterEmailValidationMsg))
                {
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), RequesterEmailValidationMsg, null);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, RequesterEmailValidationMsg));
                }
            }

            //Validate UserType(possible values)
            if (!string.IsNullOrEmpty(model.UserType))
            {
                string UserTypeValidationMsg = Globals.ValidateUserType(model.UserType);
                if (!"Success".Equals(UserTypeValidationMsg))
                {
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidUserType.ToString(), UserTypeValidationMsg, null);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidUserType, UserTypeValidationMsg));
                }
                //SG - 02/09/2019 removing payeecode check.
                //if ("Payee".Equals(model.UserType,StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(model.PayeeCode))//when Payee, PayeeCodde is required.
                //{
                //    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidPayeeCode.ToString(), "PayeeCode is mandatory for Payee", null);
                //    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidPayeeCode, "PayeeCode is mandatory for Payee"));
                //}
            }
            //Validate PayeeCode(length)
            if (!string.IsNullOrEmpty(model.PayeeCode))
            {
                string PayeeCodeValidationMsg = Globals.ValidatePayeeCode(model.PayeeCode);
                if (!"Success".Equals(PayeeCodeValidationMsg))
                {
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidPayeeCode.ToString(), PayeeCodeValidationMsg, null);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidPayeeCode, PayeeCodeValidationMsg));
                }
            }
            bool IsValidUserGroup = true ;
            //Validadte UserGroup(AD existence)
            if (!string.IsNullOrEmpty(model.UserGroup))
            {
                if (model.UserGroup.StartsWith("OpCo"))
                {
                    model.UserGroup += " " + model.CompanyCode;
                }
                bool CheckinAD = false;//????? whwat if userGroup does not exist in ad, should we create user without Group
                string UserGroupValidationMsg = Globals.ValidateUserGroup(model.UserGroup, "", CheckinAD);
                if (!"Success".Equals(UserGroupValidationMsg))
                {
                    IsValidUserGroup = false;
                   // Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidUserGroup.ToString(), UserGroupValidationMsg, null);
                    //throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidUserGroup, UserGroupValidationMsg));
                }
            }
            #endregion

            LPayee Payee = null;
            LUser FinOpUser = null;
            string WFStatus = null;
            bool CreateLogin = false;
            string CreatedById = null;
            bool UserExistsInLite = false;
            //get user existence in SOS db
            if (!string.IsNullOrEmpty(model.UserType))
            {
                if (UserType.ToLower().Equals("finops"))
                {
                    FinOpUser = db.LUsers.Where(p => p.LuEmail == model.Email).Where(p => p.LuCompanyId == CompanyId).FirstOrDefault();
                    if (FinOpUser != null)
                    {
                        WFStatus = FinOpUser.WFStatus;
                        CreateLogin = Convert.ToBoolean(FinOpUser.LuCreateLogin);
                        //CreatedById = FinOpUser.WFRequesterId;
                        UserExistsInLite = true;
                    }
                }
                else if (UserType.ToLower().Equals("payee"))
                {
                    //SG 15 Jan 2021 - WIAM team now sending PayeeCode for Each CREATE request. 
                    //SO switching back the logic to already existing logic of Payee search on PayeeCode 
                    //search payee on PayeeCode
                    Payee = db.LPayees.Where(p => p.LpPayeeCode == model.PayeeCode).Where(p => p.WFCompanyId == CompanyId).FirstOrDefault();
                    //again search on Email, and populate lobby PayeeCode if null.
                    //Payee = db.LPayees.Where(p => p.LpEmail == model.Email).Where(p => p.LpCompanyId == CompanyId).FirstOrDefault();
                    if (Payee != null)
                    {
                        WFStatus = Payee.WFStatus;
                        CreateLogin = Convert.ToBoolean(Payee.LpCreateLogin);
                        // CreatedById = Payee.WFRequesterId;
                        UserExistsInLite = true;
                        //SG 15 Jan 2021 - commenting on same basis
                        //if (string.IsNullOrEmpty(lobby.PayeeCode))
                        //{
                        //    lobby.PayeeCode = Payee.LpPayeeCode;
                        //}
                    }
                }
            }
            //Find out whether user exists in AD
            bool UserExistsInAD = ActiveDirectory.CheckUserExistInAD(model.Email, ProjectEnvironment);

            //Get CreatedById for SpLogEmail, Fixing it to L2Admin(System User)
            var xx = db.AspNetUsers.Where(a => a.Email == "L2Admin@Vodafone.com").FirstOrDefault();
            if(xx != null)
                CreatedById = xx.Id;
            string returnMsg = null;
            string Password = ConfigurationManager.AppSettings["DefaultPassword"];
            string UserIdentifier = model.FirstName + " " + model.LastName;

            string ResponseCode = null, ResponseMessage= null, LobbyComments = null, LobbyStatus = null;
            bool SOSActionRequired = false;

            if (UserExistsInAD)
            {
                if (UserExistsInLite)
                {
                    LobbyComments = "AD Account already Exist;UserExistsInAD:" + UserExistsInAD + ";UserExistsInLite:" + UserExistsInLite + " ";
                    LobbyStatus = "Error";
                    SOSActionRequired=  true;
                    ResponseCode = Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString();
                    ResponseMessage = "AD Account already Exist";
                    returnMsg=  ResponseMessage;
                }
                else//does not exist in SOS
                {
                    if (string.IsNullOrEmpty(UserType))
                    {
                        LobbyComments = "AD Account already Exist;UserExistsInAD:" + UserExistsInAD + ";UserExistsInLite:" + UserExistsInLite + " ";
                        LobbyStatus = "Error";
                        ResponseCode = Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString();
                        returnMsg = "AD Account already Exist";
                        ResponseMessage = returnMsg;
                    }
                    else//usertype is populated with Finops/Payee
                    {
                        //check user exists in Lobby as NewUser
                        var userinLobby = db.LUserLobbies.Where(a => a.Email == model.Email).Where(a => a.CompanyCode == model.CompanyCode).Where(a => a.Status == "NewUser").FirstOrDefault();
                        if(userinLobby != null)
                        {
                            LobbyStatus = "DuplicateCreate";
                        }
                        else
                        {
                            LobbyStatus = "NewUser";
                        }
                        LobbyComments = "AD account already Exists and user added to SOS;UserExistsInAD:" + UserExistsInAD + ";UserExistsInLite:" + UserExistsInLite + ";UserType:" + UserType;
                        ResponseCode = Globals.ResponseCodes.Ok.ToString();
                        returnMsg = "AD account already Exists and user added to SOS Lobby";
                        ResponseMessage = returnMsg;
                    }
                }
                
            }
            else//does not exist in AD
            {
                if (UserExistsInLite)
                {
                    LobbyComments = "AD User Created;UserExistsInAD:" + UserExistsInAD + ";UserExistsInLite:" + UserExistsInLite + " ";
                    LobbyStatus = "Success";
                    //SG 15 Jan 2021
                    //if (string.IsNullOrEmpty(lobby.PayeeCode))
                    //    lobby.PayeeCode = model.PayeeCode;// when User exists in SOS, we will update PayeeCode.
                    ResponseCode = Globals.ResponseCodes.Ok.ToString();
                    SOSActionRequired = true;
                    returnMsg = CreateUserInAD(model.Email, ProjectEnvironment, CompanyId, UserIdentifier, CreatedById, Password, model.UserGroup, WFStatus);
                    if ("Success".Equals(returnMsg))
                    {
                        switch (WFStatus.ToLower())
                        {
                            case "rejected":
                                returnMsg = "AD Account created. However User exist in SOS as Rejected";
                                break;
                            case "inprogress":
                                returnMsg = "AD Account created. However User is under approval process in SOS";
                                break;
                            case "inactive":
                                returnMsg = "AD Account Created and User rehired in SOS";
                                break;
                            case "suspended":
                                returnMsg = "AD Account Created. However User exist in SOS as Suspended";
                                break;
                            case "completed":
                                returnMsg = "AD Account Created.User already exists in SOS as Approved";
                                break;
                        }
                    }
                    ResponseMessage = returnMsg;
                }
                else  // User does not Exists In SOS
                {
                    LobbyComments = "AD Account Created;UserExistsInAD:" + UserExistsInAD + ";UserExistsInLite:" + UserExistsInLite + ";UserType:" + UserType;
                    ResponseCode = Globals.ResponseCodes.Ok.ToString();
                    returnMsg = CreateUserInAD(model.Email, ProjectEnvironment, CompanyId, UserIdentifier, CreatedById, Password, model.UserGroup,null);
                    if (string.IsNullOrEmpty(UserType))
                    {
                        LobbyStatus = "Success";
                    }
                    else
                    {
                        var userinLobby = db.LUserLobbies.Where(a => a.Email == model.Email).Where(a => a.CompanyCode == model.CompanyCode).Where(a => a.Status == "NewUser").FirstOrDefault();
                        if (userinLobby != null)
                        {
                            LobbyStatus = "DuplicateCreate";
                        }
                        else
                        {
                            LobbyStatus = "NewUser";
                        }
                    }
                    ResponseMessage = returnMsg;
                }
            }

            //Update SOS DB
            if (SOSActionRequired)
            {
                if (UserType.Equals("FinOps"))
                {
                    if (WFStatus.ToLower().Equals("inactive"))
                    {
                        FinOpUser.WFStatus = "Completed";
                    }
                    FinOpUser.LuCreateLogin = true;
                    db.Entry(FinOpUser).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (UserType.Equals("Payee"))
                {
                    if (WFStatus.ToLower().Equals("inactive"))
                    {
                        Payee.WFStatus = "Completed";
                    }
                    string ExistingPayeeEmail = Payee.LpEmail;
                    if (!Payee.LpEmail.Equals(model.Email))//update Payee email when Email passed is different from the existing payee Email.
                    {
                        Payee.LpEmail = model.Email;
                    }
                    if (ExistingPayeeEmail.Contains("system.com"))//existing email was like ‘%system.com’ then set BlockNotification=0                    
                    {
                        Payee.LpEmail = model.Email;//update email
                        Payee.LpBlockNotification = false;
                        Globals.UpdateAspUser(ExistingPayeeEmail, model.Email, model.CompanyCode);//update aspnetusers table and other
                    }
                    Payee.LpCreateLogin = true;
                    db.Entry(Payee).State = EntityState.Modified;
                    db.SaveChanges();
                    //check in LUsers once
                    var PayeeAsFinOps = db.LUsers.Where(a => a.LuEmail == ExistingPayeeEmail).Where(a => a.WFCompanyId == CompanyId).FirstOrDefault();
                    if(PayeeAsFinOps != null)
                    {
                        PayeeAsFinOps.LuEmail = model.Email;
                        PayeeAsFinOps.LuCreateLogin = true;
                        db.Entry(PayeeAsFinOps).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                Globals.UpdateAspUser(model.Email, model.Email, model.CompanyCode);//update IsActive to 1 in AspNetUser
            }
            //Update User Lobby
            UpdateLobbyAndSendResponse(lobby, LobbyStatus, ResponseCode, ResponseMessage, LobbyComments);
            //??????  what should be response
            if (!ResponseCode.Equals("Ok"))
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.RequestCouldNotBeProcessed, returnMsg));
            }
            return returnMsg;
        }
        private string Enable(EnableViewModel model, LUserLobby lobby, string ProjectEnvironment)
        {
            #region Validation for Input data
            //CompanyCode
            string CCValidationMsg = Globals.ValidateCompanyCode(model.CompanyCode);
            if (!"Success".Equals(CCValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidCompanyCode.ToString(), CCValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidCompanyCode, CCValidationMsg));
            }
            //EmailAddress
            string EmailValidationMsg = Globals.ValidateEmailAddress(model.Email, "");
            if (!"Success".Equals(EmailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), EmailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, EmailValidationMsg));
            }
            //RequestorEmail
            string ReqMailValidationMsg = Globals.ValidateRequestorEmail(model.RequestorEmail);
            if (!"Success".Equals(ReqMailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), ReqMailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, ReqMailValidationMsg));
            }
            #endregion

            //User Existence in AD
            bool UserExistsInAD = ActiveDirectory.CheckUserExistInAD(model.Email, ProjectEnvironment);
            
            string returnMsg = null;
            string ResponseCode = null, ResponseMessage = null, LobbyComments = null, LobbyStatus = null;
            
            if(UserExistsInAD)
            {
                //Check if Account is PROD account
                bool IsProdUser = IsUserMemberOfProd(model.Email);
                if (IsProdUser && !ProjectEnvironment.Equals("PROD",StringComparison.OrdinalIgnoreCase))
                {
                    LobbyComments = "Request cannot be processed as this is PROD account.";
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString(),
                        LobbyComments, LobbyComments);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.RequestCouldNotBeProcessed,
                    LobbyComments));
                }
                bool IsEnabled = ActiveDirectory.CheckUserEnabled(model.Email);
                if (IsEnabled)
                {
                    LobbyComments = Globals.UserAlreadyEnabled +";UserExistsInAD:" + UserExistsInAD + ";IsEnabled:" + IsEnabled + " ";
                    LobbyStatus = "Error";
                    ResponseCode = Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString();
                    ResponseMessage = Globals.UserAlreadyEnabled;
                    returnMsg = ResponseMessage;
                }
                else
                {
                    LobbyComments = "AD Account Enabled;UserExistsInAD:" + UserExistsInAD + ";IsEnabled:" + IsEnabled + " ";
                    LobbyStatus = "Success";
                    ResponseCode = Globals.ResponseCodes.Ok.ToString();
                    var r1= ActiveDirectory.ActivateUser(model.Email, ProjectEnvironment);
                    if (r1.IsSuccess)
                    {
                        ResponseMessage = "AD Account Enabled";
                    }
                    else
                    {
                        ResponseMessage = r1.ErrorMessage;
                    }
                    returnMsg = ResponseMessage;
                }
            }
            else
            {
                LobbyComments = "AD Account does not Exist;UserExistsInAD:" + UserExistsInAD + " ";
                LobbyStatus = "Error";
                ResponseCode = Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString();
                ResponseMessage = Globals.UserDoesNotExistinAD;
                returnMsg = ResponseMessage;
            }
            UpdateLobbyAndSendResponse(lobby, LobbyStatus, ResponseCode, ResponseMessage, LobbyComments);
            //??????  what should be responseCode
            if (!ResponseCode.Equals("Ok"))
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.RequestCouldNotBeProcessed, returnMsg));
            }
            return returnMsg;

        }

        private string Disable(DisableViewModel model, LUserLobby lobby, string ProjectEnvironment)
        {
            #region Validation for Input data
            //CompanyCode
            string CCValidationMsg = Globals.ValidateCompanyCode(model.CompanyCode);
            if (!"Success".Equals(CCValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidCompanyCode.ToString(), CCValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidCompanyCode, CCValidationMsg));
            }
            //EmailAddress
            string EmailValidationMsg = Globals.ValidateEmailAddress(model.Email, "");
            if (!"Success".Equals(EmailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), EmailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, EmailValidationMsg));
            }
            //RequestorEmail
            string ReqMailValidationMsg = Globals.ValidateRequestorEmail(model.RequestorEmail);
            if (!"Success".Equals(ReqMailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), ReqMailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, ReqMailValidationMsg));
            }
            #endregion

            //User Existence in AD
            bool UserExistsInAD = ActiveDirectory.CheckUserExistInAD(model.Email, ProjectEnvironment);

            string returnMsg = null;
            string ResponseCode = null, ResponseMessage = null, LobbyComments = null, LobbyStatus = null;
            //User existence in SOS
            bool UserExistsInSOS = false;

            //get Type of user and user object
            UserTypeViewModel UserTypeModel = GetUserType(model.Email, model.CompanyCode);
            string UserType = UserTypeModel.UserType;
            LPayee Payee = UserTypeModel.Payee;
            LUser FinOpUser = UserTypeModel.FinOps;
            string WFStatus = null;
            if (UserType.Equals("FinOps"))
            {
                WFStatus = FinOpUser.WFStatus;
                UserExistsInSOS = true;
            }
            else if (UserType.Equals("Payee"))
            {
                WFStatus = Payee.WFStatus;
                UserExistsInSOS = true;
            }
            if (UserExistsInAD)
            {
                //Check if Account is PROD account
                bool IsProdUser = IsUserMemberOfProd(model.Email);
                if (IsProdUser && ! ProjectEnvironment.Equals("PROD",StringComparison.OrdinalIgnoreCase))
                {
                    LobbyComments = "Request cannot be processed as this is PROD account.";
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString(),
                        LobbyComments, LobbyComments);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.RequestCouldNotBeProcessed,
                    LobbyComments));
                }
                bool IsEnabled = ActiveDirectory.CheckUserEnabled(model.Email);
                if (!IsEnabled)
                {
                    LobbyComments = Globals.UserAlreadyDisabled + ";UserExistsInAD:" + UserExistsInAD + ";IsEnabled:" + IsEnabled + " ";
                    LobbyStatus = "Error";
                    ResponseCode = Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString();
                    ResponseMessage = Globals.UserAlreadyDisabled;
                    returnMsg = ResponseMessage;
                }
                if(IsEnabled)
                {
                    LobbyComments = "AD Account Disabled;UserExistsInAD:" + UserExistsInAD + ";IsEnabled:" + IsEnabled + " ";
                    LobbyStatus = "Success";
                    ResponseCode = Globals.ResponseCodes.Ok.ToString();
                    var r1 = ActiveDirectory.DeactivateUser(model.Email,ProjectEnvironment);
                    if (!r1.IsSuccess)
                    {
                        ResponseMessage = r1.ErrorMessage;
                    }
                    else
                    {
                        ResponseMessage = "AD Account Disabled";
                    }
                    returnMsg = ResponseMessage;
                }
                if (UserExistsInSOS)
                {
                    if (UserType.Equals("FinOps"))
                    {
                        FinOpUser.WFStatus = "WIAMDisabled";
                        FinOpUser.LuBlockNotification = true;
                        db.Entry(FinOpUser).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (UserType.Equals("Payee"))
                    {
                        Payee.WFStatus = "WIAMDisabled";
                        Payee.LpBlockNotification = true;
                        db.Entry(Payee).State = EntityState.Modified;
                        db.SaveChanges();
                        //check in LUsers once
                        int CompanyId = db.GCompanies.Where(a => a.GcCode == model.CompanyCode).Select(a => a.Id).FirstOrDefault();
                        var PayeeAsFinOps = db.LUsers.Where(a => a.LuEmail == model.Email).Where(a => a.WFCompanyId == CompanyId).FirstOrDefault();
                        if (PayeeAsFinOps != null)
                        {
                            PayeeAsFinOps.WFStatus = "WIAMDisabled";
                            PayeeAsFinOps.LuBlockNotification = true;
                            db.Entry(PayeeAsFinOps).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                LobbyComments = "AD Account does not Exist;UserExistsInAD:" + UserExistsInAD + " ";
                LobbyStatus = "Error";
                ResponseCode = Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString();
                ResponseMessage = Globals.UserDoesNotExistinAD;
                returnMsg = ResponseMessage;
            }
            UpdateLobbyAndSendResponse(lobby, LobbyStatus, ResponseCode, ResponseMessage, LobbyComments);
            //??????  what should be responseCode
            if (!ResponseCode.Equals("Ok"))
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.RequestCouldNotBeProcessed, returnMsg));
            }
            return returnMsg;

        }

        private string Revoke(RevokeViewModel model, LUserLobby lobby, string ProjectEnvironment)
        {
            string CompanyCode = "99";
            lobby.CompanyCode = CompanyCode;
            #region Validation for Input data
            //EmailAddress
            string EmailValidationMsg = Globals.ValidateEmailAddress(model.Email, "");
            if (!"Success".Equals(EmailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), EmailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, EmailValidationMsg));
            }
            //RequestorEmail
            string ReqMailValidationMsg = Globals.ValidateRequestorEmail(model.RequestorEmail);
            if (!"Success".Equals(ReqMailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), ReqMailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, ReqMailValidationMsg));
            }
            #endregion

            //User Existence in AD
            bool UserExistsInAD = ActiveDirectory.CheckUserExistInAD(model.Email, ProjectEnvironment);

            //User existence in SOS
            bool UserExistsInSOS = false;

            //get Type of user and user object
            UserTypeViewModel UserTypeModel = GetUserType(model.Email, CompanyCode);
            string UserType = UserTypeModel.UserType;
            LPayee Payee = UserTypeModel.Payee;
            LUser FinOpUser = UserTypeModel.FinOps;
            string WFStatus = null;
            bool CreateLogin = false;
            string CreatedById = null;
            if (UserType.Equals("FinOps"))
            {
                WFStatus = FinOpUser.WFStatus;
                CreateLogin = Convert.ToBoolean(FinOpUser.LuCreateLogin);
                CompanyCode = FinOpUser.GCompany.GcCode;
                CreatedById = FinOpUser.WFRequesterId;
                UserExistsInSOS = true;
            }
            else if (UserType.Equals("Payee"))
            {
                WFStatus = Payee.WFStatus;
                CreateLogin = Payee.LpCreateLogin;
                CompanyCode = Payee.GCompany.GcCode;
                CreatedById = Payee.WFRequesterId;
                UserExistsInSOS = true;
            }
            //Delete User folder from S3
            if (System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + model.Email))
            {
                System.IO.Directory.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + model.Email, true);
            }
            string returnMsg = null;
            string ResponseCode = null, ResponseMessage = null, LobbyComments = null, LobbyStatus = null;
            bool SOSActionRequired = false;
            if (UserExistsInAD)
            {
                //Check if Account is PROD account
                bool IsProdUser = IsUserMemberOfProd(model.Email);
                if (IsProdUser && !ProjectEnvironment.Equals("PROD", StringComparison.OrdinalIgnoreCase))
                {
                    LobbyComments = "Request cannot be processed as this is PROD account.";
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString(),
                        LobbyComments, LobbyComments);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.RequestCouldNotBeProcessed,
                    LobbyComments));
                }
                LobbyComments = Globals.ProcessingSuccess + ";UserExistsInAD:" + UserExistsInAD + ";UserExistsInSOS:" + UserExistsInSOS + ";UserType:" + UserType;
                LobbyStatus = "Success";
                ResponseCode = Globals.ResponseCodes.Ok.ToString();
                ResponseMessage = Globals.ProcessingSuccess;
                var r1 = ActiveDirectory.DeleteUser(model.Email, ProjectEnvironment);
                if (!r1.IsSuccess)
                {
                    ResponseMessage = r1.ErrorMessage;
                }
                returnMsg = ResponseMessage;
                if (UserExistsInSOS)
                {
                    SOSActionRequired = true;
                }
                
            }
            else
            {
                LobbyComments = "AD Account does not Exist " + UserExistsInAD + " ";
                LobbyStatus = "Error";
                ResponseCode = Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString();
                ResponseMessage = Globals.UserDoesNotExistinAD;
                returnMsg = ResponseMessage;
                if (UserExistsInSOS)
                {
                    SOSActionRequired = true;
                }
            }
            UpdateLobbyAndSendResponse(lobby, LobbyStatus, ResponseCode, ResponseMessage, LobbyComments);
            if (SOSActionRequired)
            {
                
                switch (UserType)
                {

                    case "FinOps":
                        //Terminate User
                        Globals.TerminateUser(model.Email);
                        FinOpUser.LuCreateLogin = false;
                        FinOpUser.WFStatus = "InActive";
                        FinOpUser.WFComments += "Revoked By WIAM";
                        FinOpUser.WFUpdatedDateTime = DateTime.UtcNow;
                        db.Entry(FinOpUser).State = EntityState.Modified;
                        db.SaveChanges();
                        //Commenting on 20Aug19 by ShivaniG, As per email and discussion for sync up WIAM/non-WIAM
                        //we need to remove only FinOps user roles
                        //db.Database.ExecuteSqlCommand("delete from AspNetUserRoles where UserId ='" + FinOpUser.LuUserId 
                        //    + "' and RoleId not in (select Id from AspNetRoles where Name='Payee' and CompanyCode ='" + CompanyCode+ "') ");
                        //db.Database.ExecuteSqlCommand("delete from MEntityPortfolios where MepEntityType ='LUsers' and MepEntityId= " + FinOpUser.Id );
                        break;
                    case "Payee":
                        //If LPayees.EndDate is NULL, then set LPayees.EndDate = SYSDATE
                        if(Payee.LpEffectiveEndDate == null)
                        {
                            Payee.LpEffectiveEndDate = DateTime.UtcNow;
                        }
                        Payee.WFStatus = "InActive";
                        Payee.WFUpdatedDateTime = DateTime.Now;
                        Payee.WFComments+= " Revoked By WIAM";
                        Payee.LpCreateLogin = false; 
                        db.Entry(Payee).State = EntityState.Modified;
                        db.SaveChanges();
                        Globals.TerminateUser(Payee.LpEmail);
                        //A user can be payee as well as FinOps
                        var PayeeasFinOps = db.LUsers.Where(p => p.LuEmail == model.Email).FirstOrDefault();
                        if (PayeeasFinOps != null)
                        {
                            PayeeasFinOps.LuCreateLogin = false;
                            PayeeasFinOps.WFStatus = "InActive";
                            PayeeasFinOps.WFComments += " Revoked By WIAM";
                            PayeeasFinOps.WFUpdatedDateTime = DateTime.UtcNow;
                            db.Entry(PayeeasFinOps).State = EntityState.Modified;
                            db.SaveChanges();
                            Globals.TerminateUser(model.Email);
                            //Commenting on 20Aug19 by ShivaniG, As per email and discussion for sync up WIAM/non-WIAM
                            //we need to remove Payee as well as FinOps roles
                            //db.Database.ExecuteSqlCommand("delete from AspNetUserRoles where UserId ='" + PayeeasFinOps.LuUserId + "' ");
                            //db.Database.ExecuteSqlCommand("delete from MEntityPortfolios where MepEntityType ='LUsers' and MepEntityId= " + PayeeasFinOps.Id);
                        }
                        break;
                }
            }

            //??????  what should be responseCode
            if (!ResponseCode.Equals("Ok"))
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.RequestCouldNotBeProcessed, returnMsg));
            }
            return returnMsg;

        }

        private string Update(UpdateViewModel model,LUserLobby lobby, string ProjectEnvironment)
        {
            //as we are not getting CompanyCode in Request. Assuming CompanCode = 99
            string CompanyCode = "99";
            lobby.CompanyCode = CompanyCode;
            lobby.Email = model.OldEmail;
            lobby.NewEmail = model.NewEmail;
            #region Validation for Input data
            //OldEmailAddress
            string EmailValidationMsg = Globals.ValidateEmailAddress(model.OldEmail,"Old");
            if (!"Success".Equals(EmailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), EmailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, EmailValidationMsg));
            }
            //NewEmailAddress
            string NewEmailValidationMsg = Globals.ValidateEmailAddress(model.NewEmail, "New");
            if (!"Success".Equals(NewEmailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), NewEmailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, NewEmailValidationMsg));
            }
            if (model.OldEmail.Equals(model.NewEmail, StringComparison.OrdinalIgnoreCase))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.OldNewEmailSame.ToString(), "Old and New Email should be different", null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.OldNewEmailSame, "Old and New Email should be different"));
            }

            //RequestorEmail
            string ReqMailValidationMsg = Globals.ValidateRequestorEmail(model.RequestorEmail);
            if (!"Success".Equals(ReqMailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), ReqMailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, ReqMailValidationMsg));
            }

            #endregion
            bool ExistsInSOS = false;
            bool UserExistsInAD = ActiveDirectory.CheckUserExistInAD(model.OldEmail, ProjectEnvironment);

            //get Type of user and user object
            UserTypeViewModel UserTypeModel = GetUserType(model.OldEmail, CompanyCode);
            string UserType = UserTypeModel.UserType;
            LPayee Payee = UserTypeModel.Payee;
            LUser FinOpUser = UserTypeModel.FinOps;
            string WFStatus = null;
            bool CreateLogin = false;
            string CreatedById = null;
            if (UserType.Equals("FinOps"))
            {
                WFStatus = FinOpUser.WFStatus;
                CreateLogin = Convert.ToBoolean(FinOpUser.LuCreateLogin);
                CompanyCode = FinOpUser.GCompany.GcCode;
                CreatedById = FinOpUser.WFRequesterId;
                ExistsInSOS = true;
            }
            else if (UserType.Equals("Payee"))
            {
                WFStatus = Payee.WFStatus;
                CreateLogin = Payee.LpCreateLogin;
                CompanyCode = Payee.GCompany.GcCode;
                CreatedById = Payee.WFRequesterId;
                ExistsInSOS = true;
            }
            lobby.CompanyCode = CompanyCode;

            //check for NewEmail is already Taken - raise error - check in AD
            bool IsNewEmailAlreadyTaken = CheckUserExistInAD(model.NewEmail,null);
            if (IsNewEmailAlreadyTaken)
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.ADNewEmailAlreadyTaken.ToString(), "Email cannot be changed as new email is already taken",
                    "Email cannot be changed as new email is already taken");
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.ADNewEmailAlreadyTaken,
                    "Email cannot be changed as new email is already taken"));
            }

            string returnMsg = null;
            string ResponseCode = null, ResponseMessage = null, LobbyComments = null, LobbyStatus = null;
            bool SOSActionRequired = false;
            if (UserExistsInAD)
            {
                //Check if Account is PROD account
                bool IsProdUser = IsUserMemberOfProd(model.OldEmail);
                if (IsProdUser && !ProjectEnvironment.Equals("PROD", StringComparison.OrdinalIgnoreCase))
                {
                    LobbyComments = "Request cannot be processed as this is PROD account.";
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString(),
                        LobbyComments, LobbyComments);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.RequestCouldNotBeProcessed,
                    LobbyComments));
                }
                var r1 = ActiveDirectory.UpdateUser(model.OldEmail, model.NewEmail, ProjectEnvironment);
                if (!r1.IsSuccess)
                {
                    LobbyComments = r1.ErrorMessage + ";UserExistsInAD:" + UserExistsInAD + ";ExistsInSOS:" + ExistsInSOS + ";UserType:" + UserType;
                    LobbyStatus = "Error";
                    ResponseCode = Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString();
                    ResponseMessage = r1.ErrorMessage;
                    returnMsg = ResponseMessage;
                }
                else
                {
                    LobbyComments = Globals.ProcessingSuccess + ";UserExistsInAD:" + UserExistsInAD + ";ExistsInSOS:" + ExistsInSOS + ";UserType:" + UserType;
                    LobbyStatus = "Success";
                    ResponseCode = Globals.ResponseCodes.Ok.ToString();
                    ResponseMessage = Globals.ProcessingSuccess;
                    returnMsg = ResponseMessage;
                    //Update email in lobby as well
                    string sql = "Update  LUserLobby set  Email = {0}  where Email= {1} and Status = 'NewUser'";
                    db.Database.ExecuteSqlCommand(sql, model.NewEmail, model.OldEmail);

                    if (ExistsInSOS)
                    {
                        SOSActionRequired = true;
                    }
                }
            }
            else
            {
                LobbyComments = "AD Account does not Exist " + UserExistsInAD + " ";
                LobbyStatus = "Error";
                ResponseCode = Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString();
                ResponseMessage = Globals.UserDoesNotExistinAD;
                returnMsg = ResponseMessage;
            }

            UpdateLobbyAndSendResponse(lobby, LobbyStatus, ResponseCode, ResponseMessage, LobbyComments);
            if (SOSActionRequired)
            {
                Globals.UpdateAspUser(model.OldEmail, model.NewEmail, CompanyCode);
                switch (UserType)
                {
                    case "FinOps":
                        FinOpUser.LuEmail = model.NewEmail;
                        db.Entry(FinOpUser).State = EntityState.Modified;
                        db.SaveChanges();
                        break;
                    case "Payee":
                        string ExistingPayeeEmail = Payee.LpEmail;
                        Payee.LpEmail = model.NewEmail;
                        db.Entry(Payee).State = EntityState.Modified;
                        db.SaveChanges();
                        //check in LUsers once
                        int CompanyId = db.GCompanies.Where(a => a.GcCode == CompanyCode).Select(a => a.Id).FirstOrDefault();
                        var PayeeAsFinOps = db.LUsers.Where(a => a.LuEmail == ExistingPayeeEmail).Where(a => a.WFCompanyId == CompanyId).FirstOrDefault();
                        if (PayeeAsFinOps != null)
                        {
                            PayeeAsFinOps.LuEmail = model.NewEmail;
                            PayeeAsFinOps.LuCreateLogin = true;
                            db.Entry(PayeeAsFinOps).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        break;
                }
            }
            //??????  what should be responseCode
            if (!ResponseCode.Equals("Ok"))
            {
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.RequestCouldNotBeProcessed, returnMsg));
            }
            return returnMsg;
        }

        private string SetUserGroup(SetUserGroupViewModel model,LUserLobby lobby, string ProjectEnvironment)
        {
            //as we are not getting CompanyCode in Request. Assuming CompanCode = 99, which will be updated after getting User information.
            string CompanyCode = "99";
            lobby.CompanyCode = CompanyCode;
            lobby.UserGroup = model.OldUserGroup;
            //get Type of user and user object
            UserTypeViewModel UserTypeModel = GetUserType(model.Email, CompanyCode);
            string UserType = UserTypeModel.UserType;
            LPayee Payee = UserTypeModel.Payee;
            LUser FinOpUser = UserTypeModel.FinOps;
            string WFStatus = null;
            bool CreateLogin = false;
            string CreatedById = null;
            if (UserType.Equals("FinOps"))
            {
                WFStatus = FinOpUser.WFStatus;
                CreateLogin = Convert.ToBoolean(FinOpUser.LuCreateLogin);
                CompanyCode = FinOpUser.GCompany.GcCode;
                CreatedById = FinOpUser.WFRequesterId;
            }
            else if (UserType.Equals("Payee"))
            {
                WFStatus = Payee.WFStatus;
                CreateLogin = Payee.LpCreateLogin;
                CompanyCode = Payee.GCompany.GcCode;
            }
            else//find user in Lobby table
            {
                var lobbyuser = db.LUserLobbies.Where(a => a.Email == model.Email).Where(a => a.Status == "NewUser").FirstOrDefault();
                if (lobbyuser != null)
                {
                    CompanyCode = lobbyuser.CompanyCode;
                }
            }

            #region Validation for Input data
            //EmailAddress
            string EmailValidationMsg = Globals.ValidateEmailAddress(model.Email,"");
            if (!"Success".Equals(EmailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), EmailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, EmailValidationMsg));
            }
            //RequestorEmail
            string ReqMailValidationMsg = Globals.ValidateRequestorEmail(model.RequestorEmail);
            if (!"Success".Equals(ReqMailValidationMsg))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidEmail.ToString(), ReqMailValidationMsg, null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidEmail, ReqMailValidationMsg));
            }
            lobby.CompanyCode = CompanyCode;
            //UserGroup
            //Validadte Old UserGroup
            if (!string.IsNullOrEmpty(model.OldUserGroup))
            {
                if (model.OldUserGroup.StartsWith("OpCo"))
                {
                    model.OldUserGroup += " " + CompanyCode;
                }
                lobby.UserGroup = model.OldUserGroup;
                string UserGroupValidationMsg = Globals.ValidateUserGroup(model.OldUserGroup, "Old", true);
                if (!"Success".Equals(UserGroupValidationMsg))
                {
                    if (CompanyCode.Equals("99"))
                    {
                        Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidUserGroup.ToString(), "CompanyCode could not be calculated", null);
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidUserGroup, "CompanyCode could not be calculated"));
                    }
                    else
                    {
                        Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidUserGroup.ToString(), UserGroupValidationMsg, null);
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidUserGroup, UserGroupValidationMsg));
                    }
                }
            }
            
            //Validadte New UserGroup
            if (!string.IsNullOrEmpty(model.NewUserGroup))
            {
                if (model.NewUserGroup.StartsWith("OpCo"))
                {
                    model.NewUserGroup += " " + CompanyCode;
                }
                lobby.NewUserGroup = model.NewUserGroup;
                string UserGroupValidationMsg = Globals.ValidateUserGroup(model.NewUserGroup, "New", true);
                if (!"Success".Equals(UserGroupValidationMsg))
                {
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidUserGroup.ToString(), UserGroupValidationMsg, null);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidUserGroup, UserGroupValidationMsg));
                }
            }
            if (string.IsNullOrEmpty(model.OldUserGroup) && string.IsNullOrEmpty(model.NewUserGroup))
            {
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.InvalidUserGroup.ToString(), "Atleast one valid OldUserGroup or NewUserGroup should be provided", null);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.InvalidUserGroup, "Atleast one valid OldUserGroup or NewUserGroup should be provided"));
            }

            
            

            #endregion


            bool UserExistsInAD = ActiveDirectory.CheckUserExistInAD(model.Email, ProjectEnvironment);

            string Comments = null;

            if (UserExistsInAD)
            {
                bool IsProdUser = IsUserMemberOfProd(model.Email);
                if (IsProdUser && !ProjectEnvironment.Equals("PROD", StringComparison.OrdinalIgnoreCase))
                {
                    Comments = "Request cannot be processed as this is PROD account.";
                    Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.RequestCouldNotBeProcessed.ToString(),
                        Comments, Comments);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.RequestCouldNotBeProcessed,
                    Comments));
                }
                bool IsEnabled = ActiveDirectory.CheckUserEnabled(model.Email);
                Comments = "Straight SetUserGroup;UserExistsInAD:" + UserExistsInAD + ";IsEnabled:" + IsEnabled;
                
                //Remove old user Group provided
                if (!string.IsNullOrEmpty(model.OldUserGroup))
                {
                    var result =ActiveDirectory.RemoveUserFromGroup(model.Email, model.OldUserGroup, ProjectEnvironment);
                    if (!result.IsSuccess)//Something went wrong with AD operation
                    {
                        Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.ADServerUnavailable.ToString(), Globals.ADServerUnavailable, result.ErrorMessage);
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.ADServerUnavailable, 
                            result.ErrorMessage));
                    }
                }
                //Add User to New Group
                if (!string.IsNullOrEmpty(model.NewUserGroup))
                {
                    //Add user to new group
                    var result = ActiveDirectory.AddUserToGroup(model.Email, model.NewUserGroup, ProjectEnvironment);
                    if (!result.IsSuccess)//Something went wrong with AD operation
                    {
                        Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.ADServerUnavailable.ToString(), Globals.ADServerUnavailable, result.ErrorMessage);
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.ADServerUnavailable, 
                            result.ErrorMessage));
                    }
                }
                Globals.UpdateLobbyAndSendResponse(lobby, "Success", Globals.ResponseCodes.Ok.ToString(), Globals.ProcessingSuccess, Comments);
            }
            else
            {
                Comments = Globals.UserDoesNotExistinAD+ ";UserExistsInAD:" + UserExistsInAD + ";";
                Globals.UpdateLobbyAndSendResponse(lobby, "Error", Globals.ResponseCodes.ADUserDoesNotExist.ToString(), Globals.UserDoesNotExistinAD, Comments);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.ADUserDoesNotExist,
                    Globals.UserDoesNotExistinAD));
            }
            return Globals.ProcessingSuccess;
                
        }

        //this method returns the type of User i.e. FinOp or Payee type. 
        //If user email is found in AspNetUsers table, then user is FinOp. If found in LPayees, then user is Payee
        private UserTypeViewModel GetUserType(string Email,string CompanyCode)
        {
            int CompanyId = db.GCompanies.Where(a => a.GcCode == CompanyCode).Select(a => a.Id).FirstOrDefault();
            UserTypeViewModel UserTypeModel = new UserTypeViewModel();
            //string UserType = null;
            //If found in LPayees, then user is Payee. else if user email is found in LUsers table, then user is FinOp. 
            //One user might be Payee and FinOps As well. We need to get the Payee who is active, If He is inactive, check in LUsers
            //Reason being, if he is a Payee with FinOps role, then we want to treat LPayees as master table, otherwise, he is a pure FinOps user) 
            LPayee payee = null;
            if ("99".Equals(CompanyCode))//99 compnaycode is used in Update And SetUserGroup
                payee = db.LPayees.Where(p => p.LpEmail == Email).FirstOrDefault();
            else
                payee = db.LPayees.Where(p => p.LpEmail == Email).Where(p => p.LpCompanyId == CompanyId).FirstOrDefault();

            if (payee != null)
            {
                UserTypeModel.Payee = payee;
                UserTypeModel.FinOps = null;
                UserTypeModel.UserType = "Payee";
            }
            else
            {
                LUser FinOps = null;
                if ("99".Equals(CompanyCode))
                    FinOps = db.LUsers.Where(p => p.LuEmail == Email).FirstOrDefault();
                else
                    FinOps = db.LUsers.Where(p => p.LuEmail == Email).Where(p => p.LuCompanyId == CompanyId).FirstOrDefault();
                if (FinOps != null)
                {
                    UserTypeModel.FinOps = FinOps;
                    UserTypeModel.Payee = null;
                    UserTypeModel.UserType = "FinOps";
                }
                else
                {
                    UserTypeModel.UserType = "NotFound";
                    UserTypeModel.FinOps = null;
                    UserTypeModel.Payee = null;
                }
            }
            return UserTypeModel;
        }


        private void UpdateLobbyAndSendResponse(LUserLobby lobby, string Status, string ResponseCode, string ResponseMessage, string Comments)
        {
            lobby.Status = Status;
            lobby.ResponseCode = ResponseCode;
            lobby.Response = Status;
            lobby.ResponseMessage = ResponseMessage;
            lobby.Comments = Comments;

            db.LUserLobbies.Add(lobby);
            db.SaveChanges();
        }


        private bool CheckNewEmailAvailibility(string Email)
        {
            //Not putting OpCo check as Email has to be unique across all OpCo
            string UserType = null;
            //If found in LPayees, then user is Payee. else if user email is found in LUsers table, then user is FinOp. 
            //One user might be Payee and FinOps As well. We need to get the Payee who is active, If He is inactive, check in LUsers
            LPayee payee = null;
            payee = db.LPayees.Where(p => p.LpEmail == Email).Where(p=>p.WFStatus == "Completed" || p.WFStatus == "Suspended" || p.WFStatus == "InProgress").FirstOrDefault();

            if (payee != null)
            {
                UserType = "Payee";
            }
            else
            {
                LUser FinOps = db.LUsers.Where(p => p.LuEmail == Email).FirstOrDefault();
                if (FinOps != null)
                {
                    UserType = "FinOps";
                }
                else
                {
                   UserType = "Invalid";
                }
            }

            if (UserType.Equals("Invalid"))
                return false;
            else
                return true;
        }




        private string CreateUserInAD(string Email,string ProjectEnvironment, int CompanyId, string UserIdentifier, string CreatedById, string Password,string UserGroup,string SOSUserStatus)
        {
            string returnMsg = null;
            var r1 = ActiveDirectory.CreateUser(Email, ProjectEnvironment, CompanyId, UserIdentifier, CreatedById, null, Password, SOSUserStatus);
            if (r1.IsSuccess)
            {
                if (!string.IsNullOrEmpty(UserGroup))
                {
                    var r2 = ActiveDirectory.AddUserToGroup(Email, UserGroup, ProjectEnvironment);
                    if (!r2.IsSuccess)
                    {
                        returnMsg = "AD Account created. But Usergroup could not be added as it does not exist";
                    }
                    else
                    {
                        returnMsg = "Success";
                    }
                }
                else
                {
                    returnMsg = "Success";
                }
            }
            else
            {
                returnMsg = r1.ErrorMessage;
            }
            return returnMsg;
        }
    }

}