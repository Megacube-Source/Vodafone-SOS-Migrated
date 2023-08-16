//Code Review for this file (from security perspective) done

using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Models;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;
using static Vodafone_SOS_WebApp.Utilities.Globals;
using System.Data;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LUsersController : Controller// PrimaryController//RS with reference to :-RK R2.3 17112018 made this change (comment URL Tempring) so that review can open in new tab
    {
        string LoggedRoleId = System.Web.HttpContext.Current.Session["UserRoleId"] as string;
        string LoggedInUserId = System.Web.HttpContext.Current.Session["UserId"] as string;
        string Workflow = Convert.ToString(System.Web.HttpContext.Current.Session["Workflow"]);
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        IGenericGridRestClient GGRC = new GenericGridRestClient();
        IRWorkFlowsRestClient RWFRC = new RWorkFlowsRestClient();
        ILUsersRestClient RestClient = new LUsersRestClient();
        IAspnetRolesRestClient ARRC = new AspnetRolesRestClient();
        ILChangeRequestsRestClient LCRRC = new LChangeRequestsRestClient();
        IRStatusesRestClient RSTRC = new RStatusesRestClient();
        ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
        IAspnetUsersRestClient AURC = new AspnetUsersRestClient();
        ILPortfoliosRestClient LPRC = new LPortfoliosRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string UserRole = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
        string UserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        string CurrentUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string CurrentUserRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);

        [ControllerActionFilter]
        public ActionResult SosAdminIndex()
        {
            return View();
        }

        //Get list of roles in list box where selected roles are checked for user
        [ControllerActionFilter]
        public JsonResult GetRolesListForEdit(string UserId)
        {
            var ApiData = RestClient.GetUserRoles(UserId, CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRolesListForUserPayee(string UserId)
        {
            var ApiData = RestClient.GetUserPayeeRoles(UserId, CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //Get list of roles to be displayed in a listbox
        [ControllerActionFilter]
        public JsonResult GetRolesList()
        {
            var ApiData = ARRC.GetByCompanyCode(CompanyCode).Where(p => p.Name != "Payee");
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult Create(string Source,int? UserLobbyId)
        {
            //Code to Set Page title
            System.Web.HttpContext.Current.Session["Title"] = "Create User";
            var WFName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            System.Web.HttpContext.Current.Session["Workflow"] = WFName;
            ViewBag.Source = Source;
            ViewBag.UserLobbyId = UserLobbyId;
            // ViewBag.LuReportsToId = GetReportsTo();
            if (String.IsNullOrEmpty(Source))
            {
                return View(new LUserViewModel());
            }
            else
            {
                ILUserLobbyRestClient LRC = new LUserLobbyRestClient();
                var lobby = LRC.GetLobbyUserById(Convert.ToInt32(UserLobbyId));
                return View(lobby);
            }
            
        }

        [HttpPost]
        [ControllerActionFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LUserViewModel LUVM, HttpPostedFileBase[] FileUpload, bool CheckDuplicateUser,string Source,string UserLobbyId)
        {
            try
            {
                bool CreateLogin = false;
                ILPayeesRestClient PRC = new LPayeesRestClient();//As CheckIsWIAMEnabled fn is defined in Payee, so using its RestClient
                string IsWIAMEnabled = PRC.CheckIsWIAMEnabled();//Added by Shivani for WIAM Integration 
                //WIAM Enabled
                if (!string.IsNullOrEmpty(IsWIAMEnabled) && "yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(Source) && "lobby".Equals(Source, StringComparison.OrdinalIgnoreCase))
                        CreateLogin = true;//Source Lobby
                    else
                        CreateLogin = false;//Source user form
                }
                else//WIAM Disabled
                {
                    CreateLogin = true;
                }

                
                LUVM.LuCreateLogin = CreateLogin;
                 string FileNames = null;
                var filePath = "";
                foreach (HttpPostedFileBase files in FileUpload)
                {
                    if (files != null)
                    {
                        var fileLocation = "";
                        string fileExtension = System.IO.Path.GetExtension(files.FileName);
                        string name = System.IO.Path.GetFileNameWithoutExtension(files.FileName);
                        if (string.IsNullOrEmpty(FileNames))
                        {
                            FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                        }
                        else
                        {
                            FileNames = FileNames + "," + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                        }
                        /*OLD: User  : S:\<opco>\Users\Attached
                          NEW: User  : S:\<opco>\Users\<UserName>\SupportingDocuments
                         */
                        filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/Users/" + LUVM.LuEmail + "/SupportingDocuments");

                        fileLocation = filePath + "/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;//As disscussed with JS VG file name will have datetime stamp as suffix
                                                                                                                              //check if directory exists or not. iIf notcreate that directory
                        bool exists = System.IO.Directory.Exists(filePath);
                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(filePath);
                        }

                        files.SaveAs(fileLocation);
                    }
                }
                if (UserRole == "Sales Operations")
                {
                    LUVM.LuStatus = "Accepted";
                }
                else if (UserRole == "System Analyst")
                {
                    LUVM.LuStatus = "PendingApproval";
                }
                else if (UserRole == "L2 Admin")
                {//changes by SG - for adding user in AD after setting status as Completed
                    LUVM.LuStatus = "Active";
                    LUVM.WFStatus = "Completed";
                    LUVM.WFRequesterId = CurrentUserId;
                    LUVM.WFRequesterRoleId = CurrentUserRoleId;
                    LUVM.WFCurrentOwnerId = CurrentUserId;
                    LUVM.WFType = "LUsers";
                }
                LUVM.LuCompanyId = CompanyId;
                LUVM.LuCreatedById = CurrentUserId;
                LUVM.LuCreatedDateTime = DateTime.UtcNow;
                LUVM.LuUpdatedById = CurrentUserId;
                LUVM.LuUpdatedDateTime = DateTime.UtcNow;
                LUVM.Roles = LUVM.RoleList.Split(',').ToList();
                LUVM.WFCurrentOwnerId = CurrentUserId;
                LUVM.WFRequesterId = CurrentUserId;
                LUVM.WFCompanyId = CompanyId;
                if (!string.IsNullOrEmpty(LUVM.Comments))
                {
                    LUVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + LUVM.Comments;
                }
                LUVM.FileNames = FileNames;
                LUVM.WFRequesterRoleId = LoggedRoleId;
                var WfName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                //Setting WorkFlow Name in case of L2Admin create page for AD - SG
                if (string.IsNullOrEmpty(WfName) && UserRole == "L2 Admin")
                {
                    WfName = "Users";
                }
                //Trim the three columns suggested by JS
                LUVM.LuFirstName = LUVM.LuFirstName.Trim();
                LUVM.LuLastName = LUVM.LuLastName.Trim();
                LUVM.LuEmail = LUVM.LuEmail.Trim();
                var result = RestClient.Add(LUVM, CurrentUserRoleId, WfName, filePath, null, CheckDuplicateUser, Source, UserLobbyId);
                //if ("Lobby".Equals(Source))
                //{
                //    return RedirectToAction("AcceptLobbyUsers", "LUserLobby");
                //}
                if (UserRole == "L2 Admin")
                {
                    if (result != 0)
                    {
                        //add user in AD also
                        IADAccountRestClient ADRC = new ADAccountRestClient();
                        LoginViewModel model = new LoginViewModel();
                        model.Email = LUVM.LuEmail;
                        model.Password = "Vodafone!23";// having default password
                        model.Status = true;//Activate the user
                        ADRC.CreateUser(model);
                    }
                    return RedirectToAction("SosAdminIndex");
                }
                else
                {
                    return RedirectToAction("UpdateBaseTableWfStatus", "GenericGrid", new { TransactionId = result, ActionName = "Approve" });//Auto Approve after creating record in Flex workflow
                }
            }
            catch (Exception ex)
            {
                if (ex.Data["ErrorMessage"].ToString().Contains("User by same email already exist as InActive"))
                {
                    //SG - 9/8/2018 - Commenting following now we are removing portfolios and roles for the user who has been terminated(WFStatus='InActive')
                    //if(LUVM.RoleBasedPortfolios!= null)
                    //    ViewBag.RoleBasedPortfolioIdList = LUVM.RoleBasedPortfolios.Split('|');
                    //if (LUVM.RoleList != null)
                    //    ViewBag.RoleBasedPortfolioNameList = LUVM.RoleList.Split('|');
                    TempData["ConfirmationMessage"] = ex.Data["ErrorMessage"].ToString();//This will open a dialog box for User
                }
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage1"] = ex.Data["ErrorMessage"].ToString();
                        ViewBag.UserLobbyId = UserLobbyId;
                        ViewBag.Source = Source;
                        return View(LUVM);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        ViewBag.UserLobbyId = UserLobbyId;
                        ViewBag.Source = Source;
                        return View(LUVM);
                    default:
                        throw ex;
                }
            }
        }

        //This method will display form of users in Edit or review mode as per form type passed
        [ControllerActionFilter]
        public ActionResult Edit(int TransactionId, string FormType, Nullable<int> WFConfigId)
        {
            //Get ActionItems to be displayed 
            if (WFConfigId.HasValue)
                ViewBag.ActionItems = GGRC.GetSecondaryFormButtons(WFConfigId.Value, LoggedRoleId, LoggedInUserId, TransactionId);

            //Pass form type in view
            var UserViewModel = RestClient.GetById(TransactionId);
            //Dev 6.7) In LUser Edit, if edited User (not the login user) already Payee role, then show only assigned roles (minus Payee)
            if (UserViewModel.PayeeId == null)
            {
                ViewBag.IsUserPayee = false;
            }
            else
            {
                ViewBag.IsUserPayee = true;
            }
            ViewBag.RoleBasedPortfolioIdList = UserViewModel.RoleBasedPortfolios.Split('|');
            ViewBag.RoleBasedPortfolioNameList = UserViewModel.RoleList.Split('|');
            //Get Supporting Documents by passing Entity id
            ViewBag.SupportingDocuments = LSDRC.GetByEntityType("LUsers", TransactionId);
            ViewBag.GetSudmitableorNot = RestClient.GetSudmitableorNot(TransactionId, Workflow, UserRole, CompanyId);
            //Code to Set Page title
            System.Web.HttpContext.Current.Session["Title"] = "Edit User";
            // ViewBag.LuReportsToId = GetReportsTo(UserViewModel.LuReportsToId);
            ViewBag.FormType = (string.IsNullOrEmpty(FormType)) ? "Edit" : FormType;
            ILPayeesRestClient PRC = new LPayeesRestClient();//As CheckIsWIAMEnabled fn is defined in Payee, so using its RestClient
            ViewBag.IsWIAMEnabled = PRC.CheckIsWIAMEnabled();//Added by Shivani for WIAM Integration - 24 May2019
            return View(UserViewModel);
        }

        [HttpPost]
        [ControllerActionFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LUserViewModel LUVM, string FormType, HttpPostedFileBase[] FileUpload, string PortfolioNamesList)
        {
            try
            {
                var filePath = "";
                var FileNames = "";
                //if form type isedit update then update database table
                if (FormType.Equals("Edit") || FormType.Equals("Rehire"))
                {
                    //foreach (HttpPostedFileBase files in FileUpload)
                    //{

                    //    if (files != null)
                    //    {
                    //        var fileLocation = "";
                    //        string fileExtension = System.IO.Path.GetExtension(files.FileName);
                    //        string name = System.IO.Path.GetFileNameWithoutExtension(files.FileName);
                    //        if (string.IsNullOrEmpty(FileNames))
                    //        {
                    //            FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                    //        }
                    //        else
                    //        {
                    //            FileNames = FileNames + "," + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                    //        }

                    //        /*OLD: User  : S:\<opco>\Users\Attached
                    //         NEW: User  : S:\<opco>\Users\<UserName>\SupportingDocuments
                    //        */
                    //        filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/Users/" + LUVM.LuEmail + "/SupportingDocuments");

                    //        fileLocation = filePath + "/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;//As disscussed with JS VG file name will have datetime stamp as suffix
                    //                                                                                                              //check if directory exists or not. iIf notcreate that directory
                    //        bool exists = System.IO.Directory.Exists(filePath);
                    //        if (!exists)
                    //        {
                    //            System.IO.Directory.CreateDirectory(filePath);
                    //        }

                    //        files.SaveAs(fileLocation);
                    //    }
                    //}
                    LUVM.FileNames = FileNames;
                    LUVM.LuUpdatedById = CurrentUserId;
                    LUVM.LuUpdatedDateTime = DateTime.UtcNow;
                    LUVM.WFCompanyId = CompanyId;
                    //set status as accepted in case of rehire
                    if (FormType.Equals("Rehire"))
                    {
                        LUVM.LuStatus = "Accepted";
                    }
                    if (!string.IsNullOrEmpty(LUVM.Comments))
                    {
                        LUVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + LUVM.Comments + Environment.NewLine + LUVM.WFComments;
                    }

                    RestClient.Update(LUVM, filePath, null);

                    if (LUVM.SubmitClicked == "True")
                    {
                        if (RestClient.GetSudmitableorNot(LUVM.Id, Workflow, UserRole, CompanyId) == "True")
                        {
                            GGRC.UpdateActionStatus(Workflow, Convert.ToString(LUVM.Id), CompanyId, "Approve", LoggedInUserId, "", LoggedRoleId, string.Empty);
                        }
                    }
                }

                //if form type is change request add dirty field in change request
                if (FormType.Equals("ChangeRequest"))
                {
                    //Get the previous records of Luser Table
                    var PreviousUserRecord = RestClient.GetById(LUVM.Id);
                    //Nullable<int> StatusId = null;
                    //if (UserRole == "System Analyst")
                    //{
                    //    StatusId = RSTRC.GetByStatusName("PendingApproval").Where(p => p.RsoStatusOwner == "ChangeRequest").FirstOrDefault().Id;
                    //}
                    //if (UserRole == "Sales Operations")
                    //{
                    //    StatusId = RSTRC.GetByStatusName("Accepted").Where(p => p.RsoStatusOwner == "ChangeRequest").FirstOrDefault().Id;
                    //}
                    //update portfolios is commented as Portfolio will be a part of CR process from 22Sep2017
                    // LPRC.UpdatePortfolio(LUVM.Id,"LUsers",LUVM.PortfolioList);

                    //Check for changes in roles
                    if (PreviousUserRecord.RoleBasedPortfolios != LUVM.RoleBasedPortfolios)
                    {
                        /*Change User CR creation to create CR for only those roles where portfolios have changed. Also create 1 portfolio per role*/
                        var PreviousPortfolioArray = PreviousUserRecord.RoleBasedPortfolios.Split('|');
                        var NewRoleBasedPortfolioArray = LUVM.RoleBasedPortfolios.Split('|');
                        var PreviousRoleNameArray = PreviousUserRecord.RoleList.Split('|');
                        var NewRoleNamesArray = PortfolioNamesList.Split('|');
                        string NewValues = null;
                        String OldValues = null;
                        /*The loop will execute for the max length of old value and new value to avois following issue:
                         * If you want to take out the role, simply uncheck the role (instead of taking out the portfolios). It should then raise a CR with role change.
                         * */
                        var Length = PreviousPortfolioArray.Length;
                        if (NewRoleBasedPortfolioArray.Length > PreviousPortfolioArray.Length)
                            Length = NewRoleBasedPortfolioArray.Length;
                        for (var k = 0; k < Length; k++)
                        {
                            if (k < NewRoleBasedPortfolioArray.Length)
                                NewValues = NewRoleBasedPortfolioArray[k];
                            if (k < PreviousPortfolioArray.Length)
                                OldValues = PreviousPortfolioArray[k];
                            //Compare values
                            if (NewValues != OldValues)
                            {
                                var ChangeRequestModel = new LChangeRequestViewModel();
                                if (k < NewRoleBasedPortfolioArray.Length)
                                    ChangeRequestModel.LcrNewId = NewRoleBasedPortfolioArray[k];//LUVM.RoleBasedPortfolios;
                                if (k < PreviousPortfolioArray.Length)
                                    ChangeRequestModel.LcrOldId = PreviousPortfolioArray[k];//PreviousUserRecord.RoleBasedPortfolios;
                                if (k < PreviousRoleNameArray.Length)
                                {
                                    if (PreviousRoleNameArray[k].Length > 255)
                                    {
                                        ChangeRequestModel.LcrOldValue = PreviousRoleNameArray[k].Substring(0, 255);//PreviousUserRecord.RoleList;
                                    }
                                    else
                                    {
                                        ChangeRequestModel.LcrOldValue = PreviousRoleNameArray[k];//PreviousUserRecord.RoleList;
                                    }
                                }

                                if (k < NewRoleNamesArray.Length)
                                {
                                    if (NewRoleNamesArray[k].Length > 255)
                                    {
                                        ChangeRequestModel.LcrNewValue = NewRoleNamesArray[k].Substring(0, 255);//PreviousUserRecord.RoleList;
                                    }
                                    else
                                    {
                                        ChangeRequestModel.LcrNewValue = NewRoleNamesArray[k];//PreviousUserRecord.RoleList;
                                    }

                                }
                                ChangeRequestModel.LcrEntityName = "LUsers";
                                ChangeRequestModel.LcrAction = "Edit";
                                ChangeRequestModel.LcrColumnLabel = "Roles";
                                ChangeRequestModel.LcrColumnName = "Roles";
                                if (!string.IsNullOrEmpty(LUVM.Comments))
                                    ChangeRequestModel.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + LUVM.Comments;
                                ChangeRequestModel.LcrCompanyId = CompanyId;
                                ChangeRequestModel.WFCompanyId = CompanyId;
                                ChangeRequestModel.LcrCreatedById = CurrentUserId;
                                ChangeRequestModel.LcrCreatedDateTime = DateTime.UtcNow;
                                ChangeRequestModel.LcrUpdatedById = CurrentUserId;
                                ChangeRequestModel.LcrUpdatedDateTime = DateTime.UtcNow;
                                ChangeRequestModel.LcrEffectiveStartDate = DateTime.UtcNow;
                                ChangeRequestModel.LcrRowId = LUVM.Id;
                                ChangeRequestModel.WFCurrentOwnerId = CurrentUserId;
                                ChangeRequestModel.WFRequesterId = CurrentUserId;
                                ChangeRequestModel.EmailID = LUVM.LuEmail;
                                var s = LCRRC.Add(ChangeRequestModel, LoggedRoleId, Workflow);
                                string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                                string CRName = RWFRC.GetCRName(WorkflowName);//Get Change Request name of this workflow
                                                                              //Auto approve workflow after addition
                                try//Auto Approve the change request and display erro in case any validation fails
                                {
                                    GGRC.UpdateActionStatus(CRName, Convert.ToString(s.Id), CompanyId, "Approve", LoggedInUserId, string.Empty, CurrentUserRoleId, string.Empty);
                                }
                                catch (Exception ex)
                                {
                                    TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                                }
                            }
                        }
                    }
                    //Check for changes in other records
                    var properties = typeof(LUserViewModel).GetProperties();

                    //get Aspnet Userlist from Api
                    IAspnetRolesRestClient ARRC = new AspnetRolesRestClient();
                    var AspnetUsers = AURC.GetUserByCompanyId(CompanyId);//
                    foreach (var property in properties)
                    {
                        var propertyName = property.Name;
                        var propertyValue = property.GetValue(LUVM);
                        if (propertyName.Equals("LuFirstName") || propertyName.Equals("LuLastName") || propertyName.Equals("LuPhone") || propertyName.Equals("LuBlockNotification") || propertyName.Equals("IsSuperUser"))
                        {
                            var type = typeof(LUserViewModel);
                            var memInfo = type.GetMember(propertyName);
                            var attributes = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false);
                            var displayname = ((System.ComponentModel.DataAnnotations.DisplayAttribute)attributes[0]).Name;
                            var OldValueObj = property.GetValue(PreviousUserRecord);
                            var NewValueObj = property.GetValue(LUVM);
                            string OldValue = Convert.ToString(OldValueObj);
                            string NewValue = Convert.ToString(NewValueObj);
                            var DataType = property.GetType();
                            string NewId = null;
                            string OldId = null;
                            if (DataType.Equals(typeof(int)))
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(NewValue)))
                                {
                                    NewId = NewValue;
                                    NewValue = AspnetUsers.Where(p => p.Id == NewId).FirstOrDefault().UserName;
                                }
                                if (!string.IsNullOrEmpty(OldValue))
                                {
                                    OldId = OldValue;
                                    OldValue = AspnetUsers.Where(p => p.Id == NewId).FirstOrDefault().UserName;
                                }
                            }
                            if (!String.Equals(OldValue, NewValue, StringComparison.OrdinalIgnoreCase))
                            {
                                var ChangeRequestModel = new LChangeRequestViewModel();
                                ChangeRequestModel.LcrNewValue = NewValue;
                                ChangeRequestModel.LcrOldValue = OldValue;
                                //ChangeRequestModel.LcrOldId = OldId;
                                //ChangeRequestModel.LcrNewId = NewId;
                                ChangeRequestModel.LcrEntityName = "LUsers";
                                ChangeRequestModel.LcrAction = "Edit";
                                ChangeRequestModel.LcrColumnLabel = displayname;
                                ChangeRequestModel.LcrColumnName = propertyName;
                                if (!string.IsNullOrEmpty(LUVM.Comments))
                                    ChangeRequestModel.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + LUVM.Comments;
                                ChangeRequestModel.LcrCompanyId = CompanyId;
                                ChangeRequestModel.WFCompanyId = CompanyId;
                                ChangeRequestModel.LcrCreatedById = CurrentUserId;
                                ChangeRequestModel.LcrCreatedDateTime = DateTime.UtcNow;
                                ChangeRequestModel.LcrUpdatedById = CurrentUserId;
                                ChangeRequestModel.LcrUpdatedDateTime = DateTime.UtcNow;
                                ChangeRequestModel.LcrEffectiveStartDate = DateTime.UtcNow;
                                ChangeRequestModel.LcrRowId = LUVM.Id;
                                ChangeRequestModel.WFCurrentOwnerId = CurrentUserId;
                                ChangeRequestModel.WFRequesterId = CurrentUserId;
                                ChangeRequestModel.EmailID = LUVM.LuEmail;
                                ChangeRequestModel.LcrCreatedByForm = true;
                                var result = LCRRC.Add(ChangeRequestModel, LoggedRoleId, Workflow);
                                string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                                string CRName = RWFRC.GetCRName(WorkflowName);//System.Web.HttpContext.Current.Session["Workflow"] as string;

                                try//Auto Approve the change request and display erro in case any validation fails
                                {
                                    TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                                    GGRC.UpdateActionStatus(CRName, Convert.ToString(result.Id), CompanyId, "Approve", LoggedInUserId, string.Empty, CurrentUserRoleId, string.Empty);

                                }
                                catch (Exception ex)
                                {
                                    TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                                }
                            }
                        }

                    }
                }

                return RedirectToAction("Index", "GenericGrid");
            }
            catch (Exception ex)
            {
                ViewBag.FormType = (string.IsNullOrEmpty(FormType)) ? "Edit" : FormType;
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return View(LUVM);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return View(LUVM);
                    default:
                        throw ex;
                }
            }
        }

        //This method would be called if user clicks delete button to  delete a user
        [ControllerActionFilter]
        public ActionResult Delete(int id, string Status, string Comments, string EmailID)
        {
            try
            {
                if (String.IsNullOrEmpty(Status))
                {
                    RestClient.Delete(id, Comments, null);
                }
                else
                {
                    var ChangeRequestModel = new LChangeRequestViewModel();
                    ChangeRequestModel.LcrEntityName = "LUsers";
                    ChangeRequestModel.LcrAction = "Delete";
                    ChangeRequestModel.LcrCompanyId = CompanyId;
                    ChangeRequestModel.WFCompanyId = CompanyId;
                    ChangeRequestModel.WFComments = Comments;
                    ChangeRequestModel.EmailID = EmailID;
                    ChangeRequestModel.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + Comments;
                    //adding string.empty as these columns are not null in database will remove after correction with JS/VG
                    ChangeRequestModel.LcrColumnName = string.Empty;
                    ChangeRequestModel.LcrColumnLabel = string.Empty;
                    ChangeRequestModel.LcrCreatedById = CurrentUserId;
                    ChangeRequestModel.LcrCreatedDateTime = DateTime.UtcNow;
                    ChangeRequestModel.LcrUpdatedById = CurrentUserId;
                    ChangeRequestModel.LcrUpdatedDateTime = DateTime.UtcNow;
                    ChangeRequestModel.LcrEffectiveStartDate = DateTime.UtcNow;
                    ChangeRequestModel.WFStatus = "Saved";
                    ChangeRequestModel.WFCurrentOwnerId = LoggedInUserId;
                    ChangeRequestModel.WFRequesterId = LoggedInUserId;
                    ChangeRequestModel.LcrRowId = id;
                    string NewWorkFlow = "UsersCR";
                    var s = LCRRC.Add(ChangeRequestModel, LoggedRoleId, NewWorkFlow);
                    string WorkflowName = NewWorkFlow; // System.Web.HttpContext.Current.Session["Workflow"] as string;
                    string CRName = RWFRC.GetCRName(Workflow);//System.Web.HttpContext.Current.Session["Workflow"] as string;
                                                              //Auto approve workflow after addition
                    try//Auto Approve the change request and display erro in case any validation fails
                    {
                        GGRC.UpdateActionStatus(CRName, Convert.ToString(s.Id), CompanyId, "Approve", LoggedInUserId, string.Empty, CurrentUserRoleId, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                    }
                }
                return RedirectToAction("Index", "GenericGrid");

            }
            catch (Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    default:
                        throw ex;
                }
            }
        }

        // [ControllerActionFilter]
        public ActionResult DeleteCR(int id, string Status, string Comments, string EmailID)
        {
            try
            {
                var ChangeRequestModel = new LChangeRequestViewModel();
                ChangeRequestModel.LcrEntityName = "LUsers";
                ChangeRequestModel.LcrAction = "Delete";
                ChangeRequestModel.LcrCompanyId = CompanyId;
                ChangeRequestModel.WFCompanyId = CompanyId;
                ChangeRequestModel.WFComments = Comments;
                ChangeRequestModel.EmailID = EmailID;
                ChangeRequestModel.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + Comments;
                //adding string.empty as these columns are not null in database will remove after correction with JS/VG
                ChangeRequestModel.LcrColumnName = string.Empty;
                ChangeRequestModel.LcrColumnLabel = string.Empty;
                ChangeRequestModel.LcrCreatedById = CurrentUserId;
                ChangeRequestModel.LcrCreatedDateTime = DateTime.UtcNow;
                ChangeRequestModel.LcrUpdatedById = CurrentUserId;
                ChangeRequestModel.LcrUpdatedDateTime = DateTime.UtcNow;
                ChangeRequestModel.LcrEffectiveStartDate = DateTime.UtcNow;
                ChangeRequestModel.WFStatus = "Saved";
                ChangeRequestModel.WFCurrentOwnerId = LoggedInUserId;
                ChangeRequestModel.WFRequesterId = LoggedInUserId;
                ChangeRequestModel.LcrRowId = id;
                string NewWorkFlow = "UsersCR";
                var s = LCRRC.Add(ChangeRequestModel, LoggedRoleId, NewWorkFlow);
                string WorkflowName = NewWorkFlow; // System.Web.HttpContext.Current.Session["Workflow"] as string;
                string CRName = RWFRC.GetCRName(Workflow);//System.Web.HttpContext.Current.Session["Workflow"] as string;
                                                          //Auto approve workflow after addition
                try//Auto Approve the change request and display erro in case any validation fails
                {
                    GGRC.UpdateActionStatus(CRName, Convert.ToString(s.Id), CompanyId, "Approve", LoggedInUserId, string.Empty, CurrentUserRoleId, string.Empty);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                }

                return Json("Success");

            }
            catch (Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    default:
                        throw ex;
                }
            }
        }

        [ControllerActionFilter]
        public JsonResult GetPortfolioGrid()
        {
            var ApiData = LPRC.GetByCompanyId(CompanyId);



            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        //GetUnAssignedPortfolioNamesByCompanyId
        public JsonResult GetUnAssignedPortfolioNamesByCompanyId()
        {
            var ApiData = LPRC.GetUnAssignedPortfolioNamesByCompanyId(CompanyId);
            var portfolioname = ApiData.FirstOrDefault().RcName;
            ViewBag.portfolioname = portfolioname;
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //This method will load the grid of LPortfolio in Users edit page
        [ControllerActionFilter]
        public JsonResult GetPortfolioGridForEditPage(int UserId, string Role)
        {
            if (Role == null)
                Role = string.Empty;
            var ApiData = LPRC.GetByUserIdForEditGrid(UserId, CompanyId, CompanyCode, Role);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadUsers()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Upload Users";
            return View();
        }
        [HttpPost]
        public ActionResult UploadUsers(HttpPostedFileBase File1)
        {
            try
            {
                if (Request.Files["File1"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["File1"].FileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(Request.Files["File1"].FileName);
                    string ManipulatedFileName = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + fileExtension;
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string filePath = string.Format("{0}{1}", ConfigurationManager.AppSettings["SOSBucketRootFolder"], CompanyCode + "/upload/users");
                        if (!Globals.FolderExistsInS3(filePath))
                        {
                            Globals.CreateFolderInS3Root(filePath);
                        }
                        Globals.UploadToS3(File1.InputStream, ManipulatedFileName, filePath);
                        DataTable DT = RestClient.ValidateUserUpload(ManipulatedFileName, LoggedInUserName, CurrentUserRoleId, CompanyId);
                        if (DT.Rows.Count > 0)
                        {
                            if (DT.Rows.Count == 1)
                            {
                                if (DT.Rows[0][0].ToString() == "Validated")
                                {
                                    TempData["Message"] = "Record has been validated sucessfully. Please press upload button to create your validated User Details";
                                    TempData["Status"] = "Validated";
                                    return View();
                                }
                                else if (DT.Rows[0][0].ToString() == "BlankFile")
                                {
                                    TempData["Message"] = "Upload file has no record to validate.";
                                    TempData["Status"] = "BlankFile";
                                    return View();
                                }
                                else if (DT.Rows[0][0].ToString() == "BulkUploadFailed")
                                {
                                    TempData["Message"] = "Upload template file seems to be invalid. Please download fresh template and try again";
                                    TempData["Status"] = "BulkUploadFailed";
                                    return View();
                                }
                                else
                                {
                                    //var result = RestClient.ValidateUploadClaims(ModelList, ManipulatedFileName, LoggedInRoleId, Workflow, null, CompanyId);
                                    Globals.ExportFromDataTable(CompanyCode, LoggedInUserName, "UserUploadError", DT);
                                    var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/UserUploadError.xlsx";
                                    DirectoryInfo dir = new DirectoryInfo(FilePath);
                                    dir.Refresh();
                                    if (System.IO.File.Exists(FilePath))
                                    {
                                        TempData["Message"] = "Record could not be validated, please refer to the downloaded excel file.";
                                        TempData["Status"] = "ValidateFailed";
                                        TempData["ErrorFilePath"] = FilePath;
                                        return View();
                                    }
                                    else
                                    {
                                        TempData["Message"] = "Record could not be validated.";
                                        TempData["Status"] = "ValidateFailed";
                                        return View();
                                    }
                                }
                            }
                            else
                            {
                                Globals.ExportFromDataTable(CompanyCode, LoggedInUserName, "UserUploadError", DT);
                                var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/UserUploadError.xlsx";
                                DirectoryInfo dir = new DirectoryInfo(FilePath);
                                dir.Refresh();
                                if (System.IO.File.Exists(FilePath))
                                {
                                    TempData["Message"] = "Record could not be validated, please refer to the downloaded excel file.";
                                    TempData["ErrorFilePath"] = FilePath;
                                    return View();
                                }
                                else
                                {
                                    TempData["Message"] = "Record could not be validated.";
                                    return View();
                                }
                            }
                        }
                        DT.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        public string UploadValidUsers()
        {
            try
            {
                RestClient.UploadValidUsers(LoggedInUserName, LoggedRoleId, CompanyId);
                TempData["Message"] = "Users uploaded successfully.";
                return "Success";
            }
            catch (Exception)
            {
                TempData["Message"] = "Record could not be uploaded";
                //return View();
                return "Error";
            }

        }
        public ActionResult DownloadErrorFile()
        {
            var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/UserUploadError.xlsx";
            DirectoryInfo dir = new DirectoryInfo(FilePath);
            dir.Refresh();
            if (System.IO.File.Exists(FilePath))
            {
                return File(FilePath, "application/octet-stream", "UserUploadError.xlsx");//application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
            }
            else
            {
                return null;
            }
        }
        public ActionResult DownloadMyUserReport()
        {
            ILUsersRestClient ILu = new LUsersRestClient();
            var FileName = ILu.GetMyUsersReport(LoggedInUserId, CompanyId, CompanyCode, LoggedInUserName,Convert.ToInt32(LoggedRoleId));
            Thread.Sleep(1000);
            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            di.Refresh();
            var ByteData1 = Globals.DownloadFromS3("MyUsersReport.xlsx", CompanyCode + "/" + LoggedInUserName + "/");
            if (ByteData1 != null)//now try downloading from sos bucket
            {
                return File(ByteData1, "application/unknown", "MyUsersReport.xlsx");
            }
            else
            {
                TempData["Error"] = "No File found";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }

        }

        //Rs implemented this method on 11th Feb 2019
        //Description: to get the view of Missing Portfolios, this method is called from Database Menu click (Users => MissingPortfolios)
        public ActionResult MissingPortfolios()
        {
            //ViewBag.LuEmail = GetEmailIds(null);
            return View();
        }

        //Rs implemented this method on 12th Feb 2019
        //Description: to get all the emailids in the dropdown on Missing Portfolios page based on Opco
        private SelectList GetEmailIds(int? Selected)
        {
            var EmailIdList = RestClient.GetAllEmailId(CompanyId);
            var x = new SelectList(EmailIdList, "Id", "LuEmail", Selected);
            return x;
        }

        //Rs implemented this method on 14th Feb 2019
        //Description: to get therole of selected useremail from the dropdown on Missing Portfolios page
        public JsonResult GetUserRoleById(int UserId)
        {
            var ApiData = RestClient.GetById(UserId);
            var SelecterTypes = RestClient.GetLUserRolesbyId(ApiData.LuUserId, CompanyCode);
            return Json(SelecterTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserRoleByIdForMissingPortfolio(int UserId, string radioValue)
        {
            if(radioValue.Trim() == "FinopsUser")
            {
                var ApiData = RestClient.GetById(UserId);
                var SelecterTypes = RestClient.GetUserRoleByIdForMissingPortfolio(ApiData.LuUserId, CompanyCode);
                return Json(SelecterTypes, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ILPayeesRestClient ObjLpayeeRestClient = new LPayeesRestClient();
                //ApiData = ObjLpayeeRestClient.GetById(UserId);
                var ApiData1 = ObjLpayeeRestClient.GetById(UserId);
                var SelecterTypes = RestClient.GetUserRoleByIdForMissingPortfolio(ApiData1.LpUserId, CompanyCode);
                return Json(SelecterTypes, JsonRequestBehavior.AllowGet);
            }
            //var ApiData = RestClient.GetById(UserId);
            //if(ApiData.Id == 0)
            //{
            //    ILPayeesRestClient ObjLpayeeRestClient = new LPayeesRestClient();
            //    //ApiData = ObjLpayeeRestClient.GetById(UserId);
            //    var ApiData1 = ObjLpayeeRestClient.GetById(UserId);
            //    var SelecterTypes = RestClient.GetUserRoleByIdForMissingPortfolio(ApiData1.LpUserId, CompanyCode);
            //    return Json(SelecterTypes, JsonRequestBehavior.AllowGet);
            //}else
            //{
            //    var SelecterTypes = RestClient.GetUserRoleByIdForMissingPortfolio(ApiData.LuUserId, CompanyCode);
            //    return Json(SelecterTypes, JsonRequestBehavior.AllowGet);
            //}           
        }

        public JsonResult GetEmailVerified(string UserEmail, string radioValue)
        {
            var ApiData = RestClient.GetEmailVerified(UserEmail, CompanyId, radioValue);
           // var SelecterTypes = RestClient.GetLUserRolesbyId(ApiData.LuUserId, CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePortfolioedForAllRolesByUserEmailID(string  UserEmail,string radioValue)
        {
            var ApiData = RestClient.SavePortfolioedForAllRolesByUserEmailID(UserEmail, CompanyId, radioValue);
            // var SelecterTypes = RestClient.GetLUserRolesbyId(ApiData.LuUserId, CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        //Rs implemented this method on 14th Feb 2019
        //Description: to save the selected portfolios from Unassigned grid to assigned grid
        public JsonResult AssignPortfolios(string CommaSeperatedString, int UserId, string rolename)
        {
            var selectedportfolios = CommaSeperatedString.TrimStart(',');
            var ApiData1 = RestClient.GetById(UserId); //method to get the user detail by id of email selected in dropdown            
            var roleid = RestClient.GetUserRoleIdbyRoleName(rolename, CompanyCode); //method to get the id of rolename           
            roleid = roleid.Replace('"', ' ').Trim();
            RestClient.SavePortfolios(selectedportfolios, ApiData1.Id, roleid, null);
            return Json(JsonRequestBehavior.AllowGet);
        }

        public JsonResult AssignMissingPortfolios(string CommaSeperatedString, int UserId, string rolename, string radioValue)
        {
            var selectedportfolios = CommaSeperatedString.TrimStart(',');
            if (radioValue.Trim() == "FinopsUser")
            {
                var ApiData1 = RestClient.GetById(UserId);
                var roleid = RestClient.GetUserRoleIdbyRoleName(rolename, CompanyCode); //method to get the id of rolename           
                roleid = roleid.Replace('"', ' ').Trim();
                RestClient.SaveMissingPortfolios(selectedportfolios, ApiData1.Id, roleid, null);
                return Json(JsonRequestBehavior.AllowGet);
            }
            else
            {

                ILPayeesRestClient ObjLpayeeRestClient = new LPayeesRestClient();
                var ApiData1 = ObjLpayeeRestClient.GetById(UserId);
                //var ApiData1 = RestClient.GetById(UserId);
                var roleid = RestClient.GetUserRoleIdbyRoleName(rolename, CompanyCode); //method to get the id of rolename           
                roleid = roleid.Replace('"', ' ').Trim();
                RestClient.SaveMissingPortfolios(selectedportfolios, ApiData1.Id, roleid, null);
                return Json(JsonRequestBehavior.AllowGet);
            }
                
           //method to get the user detail by id of email selected in dropdown            
            
        }

        //Rs implemented this method on 14th Feb 2019
        //Description: This method is called to fill the Assigned Portfolio grid on Missing portfolios page
        public JsonResult GetPortfoliobyUserId(int UserId, string rolename, string radioValue)
        {
            var RoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
            if (radioValue.Trim() == "FinopsUser")
            {
                var ApiData1 = RestClient.GetById(UserId);
                ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
                var roleid = RestClient.GetUserRoleIdbyRoleName(rolename, CompanyCode); //method to get the id of rolename           
                roleid = roleid.Replace('"', ' ').Trim();
                var AssignedData = LPORC.GetByUserIdForL2Admin(ApiData1.Id, roleid,"LUsers");//Assigned
                var ApiData2 = LPRC.GetByCompanyId(CompanyId); //Unassigned
                var result = ApiData2.Where(p => !AssignedData.Any(l => p.Id == l.Id));
                return Json(new { argAssigned = AssignedData, argUnAssigned = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ILPayeesRestClient ObjLpayeeRestClient = new LPayeesRestClient();
                var ApiData2 = ObjLpayeeRestClient.GetById(UserId);
                ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
                var roleid = RestClient.GetUserRoleIdbyRoleName(rolename, CompanyCode); //method to get the id of rolename           
                roleid = roleid.Replace('"', ' ').Trim();
                var AssignedData1 = LPORC.GetByUserIdForL2Admin(ApiData2.Id, roleid,"LPayees");//Assigned
                var ApiData3 = LPRC.GetByCompanyId(CompanyId); //Unassigned
                var result = ApiData3.Where(p => !AssignedData1.Any(l => p.Id == l.Id));
                return Json(new { argAssigned = AssignedData1, argUnAssigned = result }, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        public async Task<JsonResult> UploadAutoAttachment(string id, string EmailID, string Type)
        {
            AttachedFilesViewModel FileDetails = new AttachedFilesViewModel();
            GenericGridRestClient RestClient = new GenericGridRestClient();
             string ReturnString = string.Empty;
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    HttpPostedFileBase hpf = fileContent;
                    AttachedFilesViewModel PayeeFiles = AttachPayeeFilesOneByOne(hpf, EmailID);

                    if (string.IsNullOrEmpty(FileDetails.FileName))
                    {
                        FileDetails.FileName = PayeeFiles.FileName;
                    }
                    else
                    {
                        FileDetails.FileName = FileDetails.FileName + "," + PayeeFiles.FileName;
                    }

                    FileDetails.FilePath = PayeeFiles.FilePath;
                }

                string Data = RestClient.UpdateAttachment(Convert.ToInt32(id), LoggedInUserId, FileDetails.FileName, FileDetails.FilePath, Type);
                return Json(Data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            //   return Json("File uploaded successfully");
        }

        public AttachedFilesViewModel AttachPayeeFilesOneByOne(HttpPostedFileBase files, string EmailID)
        {

            var fileLocation = "";

            var filePath = "";
            var FileNames = "";



            if (files != null)
            {
                string fileExtension = System.IO.Path.GetExtension(files.FileName);
                string name = System.IO.Path.GetFileNameWithoutExtension(files.FileName);
                //if (string.IsNullOrEmpty(files.FileName))
                //{
                    FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                //}
                //else
                //{
                //    FileNames = FileNames + "," + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                //}
                
                filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/Users/" + EmailID + "/SupportingDocuments");

                fileLocation = filePath + "/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;//As disscussed with JS VG file name will have datetime stamp as suffix
                                                                                                                      //check if directory exists or not. iIf notcreate that directory
                bool exists = System.IO.Directory.Exists(filePath);
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                files.SaveAs(fileLocation);
            }
            return new AttachedFilesViewModel { FileName = FileNames, FilePath = filePath };
        }


    }
}