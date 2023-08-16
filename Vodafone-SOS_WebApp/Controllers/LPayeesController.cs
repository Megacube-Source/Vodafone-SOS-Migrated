//Code Review for this file (from security perspective) done

using CsvHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Models;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;
using System.ComponentModel.DataAnnotations;
using static Vodafone_SOS_WebApp.Utilities.Globals;
using System.Globalization;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LPayeesController : Controller// PrimaryController//RS with reference to :-RK R2.3 17112018 made this change (comment URL Tempring) so that review can open in new tab
    {
        ILPayeesRestClient RestClient = new LPayeesRestClient();
        ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
        ILWorkFlowConfigRestClient LWFCRC = new LWorkFlowConfigRestClient();
        ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
        IRWorkFlowsRestClient RWFRC = new RWorkFlowsRestClient();
        IAspnetUsersRestClient AURC = new AspnetUsersRestClient();
        IGenericGridRestClient GGRC = new GenericGridRestClient();
        ILCompanySpecificColumnsRestClient LCSC = new LCompanySpecificColumnsRestClient();
        ILAttachmentsRestClient LARC = new LAttachmentsRestClient();
        ILUsersRestClient USERS = new LUsersRestClient();
        string Workflow = Convert.ToString(System.Web.HttpContext.Current.Session["Workflow"]);
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        string UserRole = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
        string UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedRoleId = System.Web.HttpContext.Current.Session["UserRoleId"] as string;
        ILPayeesRestClient LPRC = new LPayeesRestClient();//added by SG for Payee Calc screen//???Why this Object is defined as PayeesRestClient is defined at the top

        [ControllerActionFilter]
        public ActionResult PayeeTree()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Payee Tree";
            return View();
        }

        [ControllerActionFilter]
        public ActionResult PayeeTreeByPortfolio()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Payee TreeBy Portfolio";
            return View();
        }

        //This is atemporary method created by SS to display Graph for Payee Calculations
        [ControllerActionFilter]
        public ActionResult PayeeCalcGraph()
        {
            ViewBag.Payee = new SelectList(RestClient.GetActivePayee(CompanyId),"Id","FullName");
            IXQlickRestClient XQRC = new XQlickRestClient();
            ViewBag.QlickUrl = XQRC.GetByRole(UserRole,CompanyId).FirstOrDefault().XURL;
            return View();
        }
        //method to get data for displaying commission chart to payee
        [ControllerActionFilter]
        public JsonResult GetPayeeDashboardChartData(int CommissionPeriodCount,int PayeeId)
        {
            int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            ILCalcRestClient LCRC = new LCalcRestClient();
            var ApiData = LCRC.GetPayeeCalcForChartByPayeeId(CompanyId, PayeeId, CommissionPeriodCount);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetChannelByPrimaryChannel(string PrimaryChannel)
        {
            IRChannelsRestClient RCRC = new RChannelsRestClient();
            var ApiData = RCRC.GetDropDownDataByCompanyId(CompanyId,PrimaryChannel);
            var x = new SelectList(ApiData, "Id", "RcName");
            return Json(x,JsonRequestBehavior.AllowGet);
        }

        private SelectList GetChannel()
        {
            IRChannelsRestClient RCRC = new RChannelsRestClient();
            var ApiData = RCRC.GetDropDownDataByCompanyId(CompanyId,"Direct");
            var x = new SelectList(ApiData, "Id", "RcName");
            return x;
        }

        private SelectList GetChannel(int id,string PrimaryChannel)
        {
            IRChannelsRestClient RCRC = new RChannelsRestClient();
            var ApiData = RCRC.GetDropDownDataByCompanyId(CompanyId,PrimaryChannel);
            var x = new SelectList(ApiData, "Id", "RcName", id);
            return x;
        }

        [ControllerActionFilter]
        public JsonResult GetSubChannelByChannelId(int ChannelId)
        {
            IRSubChannelsRestClient RCRC = new RSubChannelsRestClient();
            var ApiData = RCRC.GetDropDownDataByChannelId(ChannelId);
            var x = new SelectList(ApiData, "Id", "RscName");
            return Json(x, JsonRequestBehavior.AllowGet);
        }

        //This method loads data in dropdown of subchannels where the id passed is the subchannel id which would be displayed as selected in dropdown
        private SelectList GetSubChannel(int id, int ChannelId)
        {
            IRSubChannelsRestClient RCRC = new RSubChannelsRestClient();
            var ApiData = RCRC.GetDropDownDataByChannelId(ChannelId);
            var x = new SelectList(ApiData, "Id", "RscName", id);
            return x;
        }

        //This method loads data in dropdown of subchannel
        private SelectList GetSubChannel()
        {
            IRSubChannelsRestClient RCRC = new RSubChannelsRestClient();
            var ApiData = RCRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ApiData, "Id", "RscName");
            return x;
        }

        //This method loads the data in dropdown for the channel manger
        private SelectList GetChannelManager()
        {
            //This method returns the list of  channel manager
            var ApiData = AURC.GetChannelManagerByCompanyCode(CompanyCode);
            var x = new SelectList(ApiData, "Id", "FullName");
            return x;
        }

        //This method loads the data in dropdown for the channel manger where the id passed will be the selected value in dropdown
        private SelectList GetChannelManager(string Id)
        {
            var ApiData = AURC.GetChannelManagerByCompanyCode(CompanyCode);
            var x = new SelectList(ApiData, "Id", "FullName", Id);
            return x;
        }

        //This method loads the data in Parent Payee dropdown
        //private SelectList GetParent()
        //{
        //    var ApiData = RestClient.GetParentDropDown(CompanyId);
        //    ViewBag.ParentDetails = ApiData;
        //    var x = new SelectList(ApiData, "Id", "FullName");
        //    return x;
        //}

        ////This method loads the data in the parent payee dropdown where the id passed will be the selected value of parent payee in dropdown
        //private SelectList GetParent(int id)
        //{
        //    var ApiData = RestClient.GetParentDropDown(CompanyId);
        //    ViewBag.ParentDetails = ApiData;
        //    var x = new SelectList(ApiData, "Id", "FullName", id);
        //    return x;
        //}

        private SelectList GetPrimaryChannel()
        {
            IList<SelectListItem> PrimaryChannel = new List<SelectListItem>
            {
                new SelectListItem{Text = "direct", Value = "direct"},
                new SelectListItem{Text = "indirect", Value = "indirect"},
            };
            //string[] PrimaryChannel = { "direct", "indirect" };
            var x = new SelectList(PrimaryChannel ,"Value", "Text");
            return x;
        }
        
        private SelectList GetPrimaryChannel(string SelectedPrimaryChannel)
        {
            //string[] PrimaryChannel = { "direct", "indirect" };
            IList<SelectListItem> PrimaryChannel = new List<SelectListItem>
            {
                new SelectListItem{Text = "direct", Value = "direct"},
                new SelectListItem{Text = "indirect", Value = "indirect"},
            };
            var x = new SelectList(PrimaryChannel, "Value", "Text", SelectedPrimaryChannel);
            return x;
        }

        private SelectList GetBusinessUnit()
        {
            string[] BusinessUnit = { "CBU", "EBU", "BOTH" };
            var x = new SelectList(BusinessUnit);
            return x;
        }
        
        private SelectList GetBusinessUnit(string SelectedBusinessUnit)
        {
            string[] BusinessUnit = { "CBU", "EBU", "BOTH" };
            var x = new SelectList(BusinessUnit, SelectedBusinessUnit);
            return x;
        }


        //This method is called when we create/edit/delete a payee
        [ControllerActionFilter]
        public ActionResult Create(Nullable<int> TransactionId, string PrimaryChannel, string FormType, string Source, int? UserLobbyId)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Payee";
            //passing data of LCompanySpecificColumns data
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId);
            ViewBag.CompanySpecificColumn = CompanySpecificData;
            ViewBag.FormType = FormType;
            ViewBag.Source = Source;
            ViewBag.UserLobbyId = UserLobbyId;
            ViewBag.IsWIAMEnabled = RestClient.CheckIsWIAMEnabled();//Added by Shivani for WIAM Integration - 13 May2019

            string formname = "Payee";
            var BannerValue = LCSCRC.getBannerDetail(CompanyId, formname);

            if (BannerValue == null)
            {
                ViewBag.BannerValue = "";
            }
            else
            {
                ViewBag.BannerValue = BannerValue.FirstOrDefault().BannerText;
            }


            /*This section gives empty dropdown values in LDropDownValues*/
            ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            /*section ends here*/

            //Data is passed to view as per the status of page in which it is opened
            if (FormType.Equals("Create"))
            {
                if (TransactionId.HasValue)
                {
                    var parent = RestClient.GetById(TransactionId.Value);
                    if (parent.LpChannelId.HasValue)
                    {
                        ViewBag.LpChannelId = GetChannel(parent.LpChannelId.Value,parent.LpPrimaryChannel);
                    }
                    else
                    {
                        ViewBag.LpChannelId = GetChannel();
                    }
                    //ViewBag.LppParentPayeeId = GetParent(TransactionId.Value);

                    if (parent.LpSubChannelId.HasValue)
                    {
                        ViewBag.LpSubChannelId = GetSubChannel(parent.LpSubChannelId.Value, parent.LpChannelId.Value);
                    }
                    else
                    {//passing 0 as id to resolve error and get  no item as selected
                        ViewBag.LpSubChannelId = GetSubChannel();
                    }
                    ViewBag.LpChannelManager = GetChannelManager(parent.LpChannelManager);
                    ViewBag.LpPrimaryChannel = GetPrimaryChannel(parent.LpPrimaryChannel);
                    ViewBag.LpBusinessUnit = GetBusinessUnit(parent.LpBusinessUnit);
                    var Payeemodel = new LPayeeViewModel();
                    Payeemodel.LppParentPayeeId = parent.Id;
                    Payeemodel.LpChannelId = parent.LpChannelId;
                    Payeemodel.LpSubChannelId = parent.LpSubChannelId;
                    Payeemodel.LpPrimaryChannel = parent.LpPrimaryChannel;
                    Payeemodel.LpBusinessUnit = parent.LpBusinessUnit;
                    Payeemodel.LpAddress = parent.LpAddress;
                    Payeemodel.LpEffectiveStartDate = DateTime.UtcNow;//default start date is sys date
                    return View(Payeemodel);
                }
                //If parent Id is not passed then blank model with necessary fields are passed
                var model = new LPayeeViewModel();
                //SG changes - Lobby screen updates
                if (!string.IsNullOrEmpty(Source) && "Lobby".Equals(Source))
                {
                    ILUserLobbyRestClient LRC = new LUserLobbyRestClient();
                    model = LRC.GetLobbyPayeeById(Convert.ToInt32(UserLobbyId));
                }
                if (PrimaryChannel == "Direct")
                {
                    model.LpPrimaryChannel = "Direct";
                }
                if (PrimaryChannel == "InDirect")
                {
                    model.LpPrimaryChannel = "InDirect";
                }
                // ViewBag.LpStatusId = GetStatus();
                ViewBag.LpPrimaryChannel = GetPrimaryChannel();
                ViewBag.LpBusinessUnit = GetBusinessUnit();
                ViewBag.LpChannelId = GetChannel();
               // ViewBag.LppParentPayeeId = GetParent();
                ViewBag.LpChannelManager = GetChannelManager();
                model.LpEffectiveStartDate = DateTime.UtcNow;
                //ViewBag.LpSubChannelId = GetSubChannel();
                return View(model);
            }
            else
            {
                LPayeeViewModel lPayeeViewModel = RestClient.GetById(TransactionId.Value);
                //Get Supporting Documents by passing Entity id
                ViewBag.SupportingDocuments = LSDRC.GetByEntityType("LPayees", TransactionId.Value);
                // ViewBag.LpStatusId = GetStatus();
                ViewBag.LpPrimaryChannel = GetPrimaryChannel(lPayeeViewModel.LpPrimaryChannel);
                ViewBag.LpBusinessUnit = GetBusinessUnit(lPayeeViewModel.LpBusinessUnit);
                ViewBag.GetSudmitableorNot = RestClient.GetSudmitableorNot(TransactionId.Value, Workflow, UserRole, CompanyId);
                if (lPayeeViewModel.LpChannelId.HasValue)
                {
                    ViewBag.LpChannelId = GetChannel(lPayeeViewModel.LpChannelId.Value,lPayeeViewModel.LpPrimaryChannel);
                }
                else
                {
                    ViewBag.LpChannelId = GetChannel();
                }
                //if (lPayeeViewModel.LppParentPayeeId.HasValue)
                //{
                //    ViewBag.LppParentPayeeId = GetParent(lPayeeViewModel.LppParentPayeeId.Value);
                //}
                //else
                //{
                //    ViewBag.LppParentPayeeId = GetParent();
                //}
                if (lPayeeViewModel.LpSubChannelId.HasValue)
                {
                    ViewBag.LpSubChannelId = GetSubChannel(lPayeeViewModel.LpSubChannelId.Value, lPayeeViewModel.LpChannelId.Value);
                }
                else
                {
                    ViewBag.LpSubChannelId = GetSubChannel();
                }
                ViewBag.LpChannelManager = GetChannelManager(lPayeeViewModel.LpChannelManager);
                // ViewBag.LpSubChannelId = GetSubChannel(lPayeeViewModel.LpSubChannelId.Value);
                if (lPayeeViewModel == null)
                {
                    return HttpNotFound();
                }
                return View(lPayeeViewModel);
            }
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create(LPayeeViewModel LPVM, string StatusName, HttpPostedFileBase[] File1, string PortfolioList, string Source, string UserLobbyId, string IsWIAMEnabled)
        {
            ViewBag.Source = Source;
            ViewBag.UserLobbyId = UserLobbyId;
            bool CreateLogin = false;
            if (LPVM.FormType == "Create")
            { 
                if (!string.IsNullOrEmpty(IsWIAMEnabled) && "yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(Source) && "lobby".Equals(Source, StringComparison.OrdinalIgnoreCase))
                        CreateLogin = true;//Source Lobby
                    else
                        CreateLogin = false;//Source Payee form
                }
                else//WIAM Disabled
                {
                    if (!string.IsNullOrEmpty(Source) && "lobby".Equals(Source, StringComparison.OrdinalIgnoreCase))
                        CreateLogin = true;//Source Lobby
                    else
                        CreateLogin = LPVM.LpCreateLogin;//Source Payee form
                }
                LPVM.LpCreateLogin = CreateLogin;
            }
            try
            {
                if (LPVM.LpSubChannelId == 0)//If subchannelId is returned 0 in any case then it should be converted to null
                {
                    LPVM.LpSubChannelId = null;
                }
                if (LPVM.FormType == "Create")
                {
                    //Check for if payee has added documents or not
                    if (ValidatePayee(LPVM))
                    {
                        //to check if payee has added documents or not
                        if (File1[0] != null)
                        {
                            //This method will save payee documents in path specified in web.config files and return Payee filenames and userfriendly file names
                            LPayeeViewModel PayeeFiles = AttachPayeeFiles(File1, LPVM.LpPayeeCode);
                            LPVM.LpUserFriendlyFileNames = PayeeFiles.LpUserFriendlyFileNames;
                            LPVM.LpFileNames = PayeeFiles.LpFileNames;
                            LPVM.FilePath = PayeeFiles.FilePath;
                        }
                        var UserName = System.Web.HttpContext.Current.Session["UserName"].ToString();
                        var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
                        var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
                        if (LPVM.LpSubChannelId == 0)
                        {
                            LPVM.LpSubChannelId = null;
                        }
                        if (!string.IsNullOrEmpty(LPVM.Comments))
                        {
                            LPVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + LPVM.Comments;
                        }
                        IRStatusesRestClient RSRC = new RStatusesRestClient();

                        LPVM.LpCreatedById = UserId;
                        LPVM.LpCompanyId = CompanyId;
                        LPVM.LpCreatedDateTime = DateTime.UtcNow;
                        LPVM.LpUpdatedById = UserId;
                        LPVM.LpUpdatedDateTime = DateTime.UtcNow;
                        LPVM.WFCurrentOwnerId = UserId;
                        LPVM.WFRequesterId = UserId;// WFCurrentOwnerId
                        LPVM.WFRequesterRoleId = LoggedRoleId;
                        LPVM.WFCompanyId = CompanyId;
                        LPVM.Id = 0;
                        var s = RestClient.Add(LPVM, LoggedRoleId, Workflow, LPVM.FilePath, PortfolioList, null,Source,UserLobbyId);

                        //add data in parent payee
                        if (LPVM.LppParentPayeeId.HasValue)
                        {
                            ILPayeeParentsRestClient LPPRC = new LPayeeParentsRestClient();
                            var Parent = new LPayeeParentViewModel { LppEffectiveEndDate = null, LppEffectiveStartDate = LPVM.LpEffectiveStartDate, LppParentPayeeId = LPVM.LppParentPayeeId.Value, LppPayeeId = s };
                            LPPRC.Add(Parent);
                        }

                        ////Add details in Audit Log
                        //ILPayeeAuditLogsRestClient PALRC = new LPayeeAuditLogsRestClient();
                        //var AuditModel = new LPayeeAuditLogViewModel();
                        //AuditModel.LpalPayeeId = s; AuditModel.LpalUpdatedById = UserId;
                        //if (Role == "System Analyst")
                        //{
                        //    AuditModel.LpalAction = "Pending Approval";
                        //}
                        //else
                        //{
                        //    AuditModel.LpalAction = "Accepted";
                        //}
                        //AuditModel.LpalUpdatedDateTime = DateTime.UtcNow;
                        //PALRC.Add(AuditModel);

                        //Auto approve workflow after addition
                        //RK Removed call for Approval logic for here, now the approval and sending it to next ordinal will be there in api itslef
                        //string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
                        try//Auto Approve the change request and display erro in case any validation fails
                        {

                            GGRC.UpdateActionStatus(Workflow, Convert.ToString(s), CompanyId, "Approve", UserId, string.Empty, LoggedRoleId, string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                            if (LPVM.FormType == "Create" && "Lobby".Equals(Source))
                            {
                                return RedirectToAction("AcceptLobbyUsers", "LUserLobby");
                            }
                            return RedirectToAction("Index", "GenericGrid");
                        }
                        if (LPVM.FormType == "Create" && "Lobby".Equals(Source))
                        {
                            return RedirectToAction("AcceptLobbyUsers", "LUserLobby");
                        }
                        return RedirectToAction("Index", "GenericGrid");
                    }
                    else
                    {
                        /*This section gives empty dropdown values in LDropDownValues*/
                        ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        /*section ends here*/
                        ViewBag.LpPrimaryChannel = GetPrimaryChannel(LPVM.LpPrimaryChannel);
                        ViewBag.LpBusinessUnit = GetBusinessUnit(LPVM.LpBusinessUnit);
                        if (LPVM.LpChannelId.HasValue)
                        {
                            ViewBag.LpChannelId = GetChannel(LPVM.LpChannelId.Value, LPVM.LpPrimaryChannel);
                        }
                        else
                        {
                            ViewBag.LpChannelId = GetChannel();
                        }
                        //if (LPVM.LppParentPayeeId.HasValue)
                        //{
                        //    ViewBag.LppParentPayeeId = GetParent(LPVM.LppParentPayeeId.Value);
                        //}
                        //else
                        //{
                        //    ViewBag.LppParentPayeeId = GetParent();
                        //}
                        if (LPVM.LpSubChannelId.HasValue)
                        {
                            ViewBag.LpSubChannelId = GetSubChannel(LPVM.LpSubChannelId.Value, LPVM.LppParentPayeeId.Value);
                        }
                        else
                        {
                            ViewBag.LpSubChannelId = GetSubChannel();
                        }
                        ViewBag.LpChannelManager = GetChannelManager(LPVM.LpChannelManager);
                        ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                        var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId);
                        ViewBag.CompanySpecificColumn = CompanySpecificData;
                        ViewBag.FormType = LPVM.FormType;
                        return View(LPVM);
                    }
                }
                //This code is executed if status of page is review 
                //if (LPVM.FormType == "Review")
                //{
                //    var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
                //    var UserName = System.Web.HttpContext.Current.Session["UserName"].ToString();
                //    var model = new Sp_UpdateItemStatusViewModel();
                //    model.ItemList = LPVM.Id.ToString();
                //    model.UpdatedBy = UserId;
                //    model.UpdatedDateTime = DateTime.UtcNow;
                //    model.StatusName = StatusName;
                //    if (!string.IsNullOrEmpty(LPVM.Comments))
                //    {
                //        model.Comments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + LPVM.Comments + Environment.NewLine + LPVM.WFComments;
                //    }

                //    RestClient.UpdatePayeeStatus(model);
                //    if (StatusName == "Approved")
                //    {
                //        var PayeeDetails = RestClient.GetById(LPVM.Id);
                //        string ReceiverEmail = "";//if create login flag is set to true then only this user will get registered in application otherwise this step would be skipped
                //        if (PayeeDetails.LpCreateLogin)
                //        {

                //            ReceiverEmail = PayeeDetails.LpEmail;
                //            var RoleList = new List<string>();
                //            RoleList.Add("Payee");
                //            var RegisterUserViewModel = new RegisterViewModel { Email = PayeeDetails.LpEmail, Password = "Vodafone!23", ConfirmPassword = "Vodafone!23", GcCompanyId = CompanyId, Roles = RoleList };
                //            IAccountsRestClient ARRC = new AccountsRestClient();
                //            var Result = ARRC.Register(RegisterUserViewModel);
                //            PayeeDetails.LpUserId = Result.UserId;
                //            RestClient.Update(PayeeDetails, null, string.Empty,PortfolioList,UserId);
                //            Globals.SendEmail("Welcome to Vodafone LITE your access to the Commission Portal.You have now been approved into system and you can access system from your Start Date :" + PayeeDetails.LpEffectiveStartDate.Date + ".Please use following credentials to Login" + Environment.NewLine + "Url:test.Vodafonelite.com " + Environment.NewLine + "Email : " + PayeeDetails.LpEmail + Environment.NewLine + " Password :'Vodafone!23'" + Environment.NewLine + "You will be asked to change your password upon first login." + Environment.NewLine + "If you forget your password click on 'Forget password link'", "Payee Registration", ReceiverEmail, "info@megacube.com.au", null);
                //        }
                //    }
                //    //Add details in Audit Log
                //    ILPayeeAuditLogsRestClient PALRC = new LPayeeAuditLogsRestClient();
                //    var AuditModel = new LPayeeAuditLogViewModel();
                //    AuditModel.LpalPayeeId = LPVM.Id; AuditModel.LpalUpdatedById = UserId;
                //    AuditModel.LpalAction = StatusName;
                //    AuditModel.LpalUpdatedDateTime = DateTime.UtcNow;
                //    PALRC.Add(AuditModel);

                //}

                //If status is edit then Edit method is called and its result is stored in a bool value for success or faliure
                //This method is called to add change in change request for an approved payee
                if (LPVM.FormType == "Edit")
                {
                    var x = Edit(LPVM, PortfolioList);
                    //update portfolios
                    LPORC.UpdatePortfolio(LPVM.Id, "LPayees", PortfolioList, CompanyCode);
                    if (!x)
                    {
                        /*This section gives empty dropdown values in LDropDownValues*/
                        ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        /*section ends here*/
                        //Get Supporting Documents by passing Entity id
                        ViewBag.SupportingDocuments = LSDRC.GetByEntityType("LPayees", LPVM.Id);
                        ViewBag.LpPrimaryChannel = GetPrimaryChannel(LPVM.LpPrimaryChannel);
                        ViewBag.LpBusinessUnit = GetBusinessUnit(LPVM.LpBusinessUnit);
                        ViewBag.FormType = LPVM.FormType;
                        if (LPVM.LpChannelId.HasValue)
                        {
                            ViewBag.LpChannelId = GetChannel(LPVM.LpChannelId.Value, LPVM.LpPrimaryChannel);
                        }
                        else
                        {
                            ViewBag.LpChannelId = GetChannel();
                        }
                        //if (LPVM.LppParentPayeeId.HasValue)
                        //{
                        //    ViewBag.LppParentPayeeId = GetParent(LPVM.LppParentPayeeId.Value);
                        //}
                        //else
                        //{
                        //    ViewBag.LppParentPayeeId = GetParent();
                        //}
                        if (LPVM.LpSubChannelId.HasValue)
                        {
                            ViewBag.LpSubChannelId = GetSubChannel(LPVM.LpSubChannelId.Value, LPVM.LppParentPayeeId.Value);
                        }
                        else
                        {
                            ViewBag.LpSubChannelId = GetSubChannel();
                        }
                        ViewBag.LpChannelManager = GetChannelManager(LPVM.LpChannelManager);
                        //passing data of LCompanySpecificColumns data
                        ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                        var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId);
                        ViewBag.CompanySpecificColumn = CompanySpecificData;
                        var PayeeModel = RestClient.GetById(LPVM.Id);

                        return View(PayeeModel);
                        
                    }
                    
                }

                

                    //This method is called to edit details of payee which is not approved yet and result is stored in a variable which is of bool type for sucess or faliure
                    if (LPVM.FormType == "EditDetails")
                    {
                    //to check if payee has added documents or not
                    //if (File1[0] != null)
                    //{
                    //    //This method will save payee documents in path specified in web.config files and return Payee filenames and userfriendly file names
                    //    LPayeeViewModel PayeeFiles = AttachPayeeFiles(File1, LPVM.LpPayeeCode);
                    //    LPVM.LpUserFriendlyFileNames = PayeeFiles.LpUserFriendlyFileNames;
                    //    LPVM.LpFileNames = PayeeFiles.LpFileNames;
                    //    LPVM.FilePath = PayeeFiles.FilePath;
                    //}
                    LPVM.ParameterCarrier = PortfolioList;
                    var y = EditDetails(LPVM,PortfolioList);
                    //update por    hhvtfolios
                    LPORC.UpdatePortfolio(LPVM.Id, "LPayees", PortfolioList,CompanyCode);
                    if (LPVM.SubmitClicked == "True")
                    {
                        if (RestClient.GetSudmitableorNot(LPVM.Id, Workflow, UserRole, CompanyId) == "True")
                        {
                            GGRC.UpdateActionStatus(Workflow, Convert.ToString(LPVM.Id), CompanyId, "Approve", UserId, "", LoggedRoleId, string.Empty);
                        }
                    }
                    if (!y)
                    {
                        /*This section gives empty dropdown values in LDropDownValues*/
                        ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        /*section ends here*/
                        //Get Supporting Documents by passing Entity id
                        ViewBag.SupportingDocuments = LSDRC.GetByEntityType("LPayees", LPVM.Id);
                        ViewBag.LpPrimaryChannel = GetPrimaryChannel(LPVM.LpPrimaryChannel);
                        ViewBag.LpBusinessUnit = GetBusinessUnit(LPVM.LpBusinessUnit);
                        ViewBag.FormType = LPVM.FormType;
                        if (LPVM.LpChannelId.HasValue)
                        {
                            ViewBag.LpChannelId = GetChannel(LPVM.LpChannelId.Value,LPVM.LpPrimaryChannel);
                        }
                        else
                        {
                            ViewBag.LpChannelId = GetChannel();
                        }
                        //if (LPVM.LppParentPayeeId.HasValue)
                        //{
                        //    ViewBag.LppParentPayeeId = GetParent(LPVM.LppParentPayeeId.Value);
                        //}
                        //else
                        //{
                        //    ViewBag.LppParentPayeeId = GetParent();
                        //}
                        if (LPVM.LpSubChannelId.HasValue)
                        {
                            ViewBag.LpSubChannelId = GetSubChannel(LPVM.LpSubChannelId.Value, LPVM.LppParentPayeeId.Value);
                        }
                        else
                        {
                            ViewBag.LpSubChannelId = GetSubChannel();
                        }
                        ViewBag.LpChannelManager = GetChannelManager(LPVM.LpChannelManager);

                        //passing data of LCompanySpecificColumns data
                        ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                        var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId);
                        ViewBag.CompanySpecificColumn = CompanySpecificData;
                        return View(LPVM);
                        
                    }
                }

                //This method is called when page status id delete and the result is stored in a bool variable for sucess or faliure
                if (LPVM.FormType == "Delete")
                {
                    var z = DeleteConfirmed(LPVM);
                    if (!z)
                    {
                        /*This section gives empty dropdown values in LDropDownValues*/
                        ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                        /*section ends here*/
                        ViewBag.LpPrimaryChannel = GetPrimaryChannel(LPVM.LpPrimaryChannel);
                        ViewBag.LpBusinessUnit = GetBusinessUnit(LPVM.LpBusinessUnit);
                        ViewBag.FormType = LPVM.FormType;
                        if (LPVM.LpChannelId.HasValue)
                        {
                            ViewBag.LpChannelId = GetChannel(LPVM.LpChannelId.Value,LPVM.LpPrimaryChannel);
                        }
                        else
                        {
                            ViewBag.LpChannelId = GetChannel();
                        }
                        //if (LPVM.LppParentPayeeId.HasValue)
                        //{
                        //    ViewBag.LppParentPayeeId = GetParent(LPVM.LppParentPayeeId.Value);
                        //}
                        //else
                        //{
                        //    ViewBag.LppParentPayeeId = GetParent();
                        //}
                        if (LPVM.LpSubChannelId.HasValue)
                        {
                            ViewBag.LpSubChannelId = GetSubChannel(LPVM.LpSubChannelId.Value, LPVM.LppParentPayeeId.Value);
                        }
                        else
                        {
                            ViewBag.LpSubChannelId = GetSubChannel();
                        }
                        ViewBag.LpChannelManager = GetChannelManager(LPVM.LpChannelManager);

                        //passing data of LCompanySpecificColumns data
                        ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                        var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId);
                        ViewBag.CompanySpecificColumn = CompanySpecificData;
                        return View(LPVM);
                    }
                }
                if(LPVM.FormType == "Create" && "Lobby".Equals(Source))
                {
                    return RedirectToAction("AcceptLobbyUsers", "LUserLobby");
                }

                return RedirectToAction("Index", "GenericGrid");
            }
            catch (Exception ex)
            {

                /*This section gives empty dropdown values in LDropDownValues*/
                ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
                /*section ends here*/
                ViewBag.LpPrimaryChannel = GetPrimaryChannel(LPVM.LpPrimaryChannel);
                ViewBag.LpBusinessUnit = GetBusinessUnit(LPVM.LpBusinessUnit);
                ViewBag.FormType = LPVM.FormType;
                if (LPVM.LpChannelId.HasValue)
                {
                    ViewBag.LpChannelId = GetChannel(LPVM.LpChannelId.Value,LPVM.LpPrimaryChannel);
                }
                else
                {
                    ViewBag.LpChannelId = GetChannel();
                }
                //if (LPVM.LppParentPayeeId.HasValue)
                //{
                //    ViewBag.LppParentPayeeId = GetParent(LPVM.LppParentPayeeId.Value);
                //}
                //else
                //{
                //    ViewBag.LppParentPayeeId = GetParent();
                //}
                if (LPVM.LpSubChannelId.HasValue)
                {
                    ViewBag.LpSubChannelId = GetSubChannel(LPVM.LpSubChannelId.Value, LPVM.LppParentPayeeId.Value);
                }
                else
                {
                    ViewBag.LpSubChannelId = GetSubChannel();
                }
                ViewBag.LpChannelManager = GetChannelManager(LPVM.LpChannelManager);
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                //passing data of LCompanySpecificColumns data
                ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId);
                ViewBag.CompanySpecificColumn = CompanySpecificData;
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(Globals.ErrorPageUrl);
                    case (int)ExceptionType.Type2:
                        //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                        return View(LPVM);
                    case (int)ExceptionType.Type3:
                        //Send Ex.Message to the error page which will be displayed as popup
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        return Redirect(ex.Data["RedirectToUrl"] as string);
                    case (int)ExceptionType.Type4:
                        ViewBag.Error = ex.Data["ErrorMessage"].ToString();
                        return View(LPVM);
                    default:
                        throw ex;
                }
            }
        }
        
        //get method to open diabled page to view payee details
        [ControllerActionFilter]
        public ActionResult ViewPayeeDetails(int TransactionId, int WFConfigId)
        {
            //Get ActionItems to be displayed 
            ViewBag.ActionItems = GGRC.GetSecondaryFormButtons(WFConfigId, LoggedRoleId, UserId, TransactionId);
            ViewBag.IsWIAMEnabled = RestClient.CheckIsWIAMEnabled();//Added by Shivani for WIAM Integration - 12 june 2019
            /*This section gives empty dropdown values in LDropDownValues*/
            ViewBag.A01 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A02 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A03 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A04 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A05 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A06 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A07 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A08 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A09 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            ViewBag.A10 = new SelectList(new List<LDropDownValueViewModel>(), "LdvCode", "LdvValue");
            /*section ends here*/
            //passing data of LCompanySpecificColumns data
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId);
            ViewBag.CompanySpecificColumn = CompanySpecificData;
            System.Web.HttpContext.Current.Session["Title"] = "Payee Details";
            LPayeeViewModel lPayeeViewModel = RestClient.GetById(TransactionId);
            // ViewBag.LpStatusId = GetStatus();
            if (lPayeeViewModel.LpChannelId.HasValue)
            {
                ViewBag.LpChannelId = GetChannel(lPayeeViewModel.LpChannelId.Value,lPayeeViewModel.LpPrimaryChannel);
            }
            else
            {
                ViewBag.LpChannelId = GetChannel();
            }
            //if (lPayeeViewModel.LppParentPayeeId.HasValue)
            //{
            //    ViewBag.LppParentPayeeId = GetParent(lPayeeViewModel.LppParentPayeeId.Value);
            //}
            //else
            //{
            //    ViewBag.LppParentPayeeId = GetParent();
            //}
            if (lPayeeViewModel.LpSubChannelId.HasValue)
            {
                ViewBag.LpSubChannelId = GetSubChannel(lPayeeViewModel.LpSubChannelId.Value, lPayeeViewModel.LppParentPayeeId.Value);
            }
            else
            {
                ViewBag.LpSubChannelId = GetSubChannel();
            }
            ViewBag.LpChannelManager = GetChannelManager(lPayeeViewModel.LpChannelManager);
            // ViewBag.LpSubChannelId = GetSubChannel(lPayeeViewModel.LpSubChannelId.Value);

            string formname = "Payee";
            var BannerValue = LCSCRC.getBannerDetail(CompanyId, formname);

            if (BannerValue == null)
            {
                ViewBag.BannerValue = "";
            }
            else
            {
                ViewBag.BannerValue = BannerValue.FirstOrDefault().BannerText;
            }
            if (lPayeeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(lPayeeViewModel);
        }

        
        //[HttpPost]
        /// <summary>
        /// This method is called when we edit payee which is not approved yet and we directly update the Lpayees and Lparentpayees table 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        [ControllerActionFilter]
        public bool EditDetails(LPayeeViewModel model,string PortfolioList)
        {
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            string formname = "Payee";
            var BannerValue = LCSCRC.getBannerDetail(CompanyId, formname);

            if (BannerValue == null)
            {
                ViewBag.BannerValue = "";
            }
            else
            {
                ViewBag.BannerValue = BannerValue.FirstOrDefault().BannerText;
            }
            try
            {
                //to check validation of payee
                if (ValidatePayee(model))
                {
                    var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
                    IRStatusesRestClient RSRC = new RStatusesRestClient();
                    var UserName = System.Web.HttpContext.Current.Session["UserName"].ToString();
                    model.LpUpdatedById = UserId;
                    var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
                    //if (model.LpStatus.Equals("Withdrawn"))
                    //{
                    //    var model1 = new Sp_UpdateItemStatusViewModel();
                    //    model1.ItemList = model.Id.ToString();
                    //    model1.UpdatedBy = UserId;
                    //    model1.UpdatedDateTime = DateTime.UtcNow;
                    //    model1.StatusName = "Withdrawn";
                    //    RestClient.UpdatePayeeStatus(model1);
                    //}
                    //else
                    //{

                        model.LpUpdatedDateTime = DateTime.UtcNow;
                        if (!string.IsNullOrEmpty(model.Comments))
                        {
                            model.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm tt") + "]" + model.Comments + Environment.NewLine + model.WFComments;
                        }
                        
                        model.WFCompanyId = CompanyId;
                        RestClient.Update(model, model.LpFileNames, model.FilePath,PortfolioList,UserId,null);
                   // }
                    //update record of parentpayee
                    if (model.LppParentPayeeId.HasValue)
                    {
                        ILPayeeParentsRestClient LPPRC = new LPayeeParentsRestClient();
                        var PayeeParent = LPPRC.GetByPayeeId(model.Id);
                        PayeeParent.LppEffectiveStartDate = model.LpEffectiveStartDate;
                        PayeeParent.LppParentPayeeId = model.LppParentPayeeId.Value;
                        //if existing payee has no parent add a parent payee row in table otherwise update the existing one
                        if (PayeeParent.Id == 0)
                        {
                            PayeeParent.LppPayeeId = model.Id;
                            LPPRC.Add(PayeeParent);
                        }
                        else
                        {
                            LPPRC.Update(PayeeParent);
                        }
                   }
                    //Add details in Audit Log
                    //ILPayeeAuditLogsRestClient PALRC = new LPayeeAuditLogsRestClient();
                    //var AuditModel = new LPayeeAuditLogViewModel();
                    //AuditModel.LpalPayeeId = model.Id; AuditModel.LpalUpdatedById = UserId;
                    //if (Role == "System Analyst")
                    //{
                    //    AuditModel.LpalAction = "PendingApproval";
                    //}
                    //else
                    //{
                    //    AuditModel.LpalAction = "Accepted";
                    //}
                    //AuditModel.LpalUpdatedDateTime = DateTime.UtcNow;
                    //PALRC.Add(AuditModel);
                    return true;
                }

                else
                {
                    if (model.LpChannelId.HasValue)
                    {
                        ViewBag.LpChannelId = GetChannel(model.LpChannelId.Value,model.LpPrimaryChannel);
                    }
                    else
                    {
                        ViewBag.LpChannelId = GetChannel();
                    }
                    //if (model.LppParentPayeeId.HasValue)
                    //{
                    //    ViewBag.LppParentPayeeId = GetParent(model.LppParentPayeeId.Value);
                    //}
                    //else
                    //{
                    //    ViewBag.LppParentPayeeId = GetParent();
                    //}
                    if (model.LpSubChannelId.HasValue)
                    {
                        ViewBag.LpSubChannelId = GetSubChannel(model.LpSubChannelId.Value, model.LpChannelId.Value);
                    }
                    else
                    {
                        ViewBag.LpSubChannelId = GetSubChannel();
                    }
                    ViewBag.LpChannelManager = GetChannelManager(model.LpChannelManager);
                    //passing data of LCompanySpecificColumns data
                    //ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                    var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId);
                    ViewBag.CompanySpecificColumn = CompanySpecificData;
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (model.LpChannelId.HasValue)
                {
                    ViewBag.LpChannelId = GetChannel(model.LpChannelId.Value,model.LpPrimaryChannel);
                }
                else
                {
                    ViewBag.LpChannelId = GetChannel();
                }
                //if (model.LppParentPayeeId.HasValue)
                //{
                //    ViewBag.LppParentPayeeId = GetParent(model.LppParentPayeeId.Value);
                //}
                //else
                //{
                //    ViewBag.LppParentPayeeId = GetParent();
                //}
                if (model.LpSubChannelId.HasValue)
                {
                    ViewBag.LpSubChannelId = GetSubChannel(model.LpSubChannelId.Value, model.LpChannelId.Value);
                }
                else
                {
                    ViewBag.LpSubChannelId = GetSubChannel();
                }
                ViewBag.LpChannelManager = GetChannelManager(model.LpChannelManager);
                TempData["Message"] = ex.Data["ErrorMessage"].ToString();
                //passing data of LCompanySpecificColumns data
               // ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId);
                ViewBag.CompanySpecificColumn = CompanySpecificData;
                return false;
            }
        }

        [ControllerActionFilter]
        public bool Edit(LPayeeViewModel lPayeeViewModel,string PortfolioList)
        {
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            string formname = "Payee";
            var BannerValue = LCSCRC.getBannerDetail(CompanyId, formname);

            if (BannerValue == null)
            {
                ViewBag.BannerValue = "";
            }
            else
            {
                ViewBag.BannerValue = BannerValue.FirstOrDefault().BannerText;
            }
            try
            {
                ILChangeRequestsRestClient CRRC = new LChangeRequestsRestClient();
                IRChannelsRestClient RCRC = new RChannelsRestClient();
                IRSubChannelsRestClient RSRC = new RSubChannelsRestClient();
                IRStatusesRestClient RSTRC = new RStatusesRestClient();
                    var UserName = System.Web.HttpContext.Current.Session["UserName"].ToString();
                    //register user
                    if (!string.IsNullOrEmpty(lPayeeViewModel.Comments))
                    {
                        lPayeeViewModel.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "]" + lPayeeViewModel.Comments + Environment.NewLine + lPayeeViewModel.WFComments;
                    }

                    string CRWorkflow = RWFRC.GetCRName(Workflow);
                    //update the Workflow with CR Workflow
                    Workflow = CRWorkflow;
                    var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
                    //var StatusId = 0;
                    //if (Role == "System Analyst")
                    //{
                    //    StatusId = RSTRC.GetByStatusName("PendingApproval").Where(p => p.RsoStatusOwner == "ChangeRequest").FirstOrDefault().Id;
                    //}
                    //else
                    //{
                    //    StatusId = RSTRC.GetByStatusName("Accepted").Where(p => p.RsoStatusOwner == "ChangeRequest").FirstOrDefault().Id;
                    //}
                    var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
                    lPayeeViewModel.LpUpdatedDateTime = DateTime.UtcNow;
                    lPayeeViewModel.LpUpdatedById = UserId;
                    //Get Previous values of This Payee by Get By Id
                    var PreviousData = RestClient.GetById(lPayeeViewModel.Id);
                    //Get data of LCompanySpecificColumns data
                    //ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                    var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId).ToList();

                    if (lPayeeViewModel.LppParentPayeeId != PreviousData.LppParentPayeeId)
                    {
                        if (ValidatePayee(PreviousData))
                        {
                            var model = new LChangeRequestViewModel();
                            model.LcrColumnName = "LppParentPayeeId";
                            model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Parent Payee");
                            model.LcrEntityName = "LPayees";
                            if (lPayeeViewModel.LppParentPayeeId.HasValue)
                            {
                                var Parent = RestClient.GetById(lPayeeViewModel.LppParentPayeeId.Value);
                                model.LcrNewValue = Parent.LpFirstName + " " + Parent.LpLastName + "(" + Parent.LpPayeeCode + ")";
                                model.LcrNewId = Parent.Id.ToString();
                            }
                            model.LcrAction = "Edit";
                            if (PreviousData.LppParentPayeeId.HasValue)
                            {
                                var OldParent = RestClient.GetById(PreviousData.LppParentPayeeId.Value);
                                model.LcrOldValue = OldParent.LpFirstName + " " + OldParent.LpLastName + "(" + OldParent.LpPayeeCode + ")";
                                model.LcrOldId = OldParent.Id.ToString();
                            }
                            model.WFComments = lPayeeViewModel.WFComments;
                            model.LcrCompanyId = CompanyId; model.WFCompanyId = CompanyId;
                            model.LcrCreatedDateTime = DateTime.UtcNow;
                            model.LcrCreatedById = UserId;
                            model.LcrUpdatedDateTime = DateTime.UtcNow;
                            model.LcrUpdatedById = UserId;
                            model.LcrRowId = lPayeeViewModel.Id;
                            model.WFCurrentOwnerId = UserId;
                            model.WFRequesterId = UserId;
                            model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                            var s = CRRC.Add(model, LoggedRoleId, Workflow);
                            //Auto approve workflow after addition
                            try//Auto Approve the change request and display erro in case any validation fails
                            {
                            GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty, LoggedRoleId, string.Empty);
                            }
                            catch (Exception ex)
                            {
                                TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                            }
                        
                    }
                        else
                        {
                        return false;
                        }
                    
                }
                #region DeadCode
                //if (lPayeeViewModel.LppParentPayeeId != PreviousData.LppParentPayeeId)
                //{
                //    var model = new LChangeRequestViewModel();
                //    model.LcrColumnName = "LppParentPayeeId";
                //    model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Parent Payee");
                //    model.LcrEntityName = "LPayees";
                //    model.WFComments = lPayeeViewModel.WFComments;
                //    if (PreviousData.LppParentPayeeId.HasValue)
                //    {
                //        var old = RestClient.GetById(PreviousData.LppParentPayeeId.Value);
                //        model.LcrOldId = PreviousData.LppParentPayeeId.ToString();
                //        model.LcrOldValue = old.LpFirstName + " " + old.LpLastName + " (" + old.LpPayeeCode + ")";
                //    }
                //    if (lPayeeViewModel.LppParentPayeeId.HasValue)
                //    {
                //        var newer = RestClient.GetById(lPayeeViewModel.LppParentPayeeId.Value);
                //        model.LcrNewId = lPayeeViewModel.LppParentPayeeId.ToString();
                //        model.LcrNewValue = newer.LpFirstName + " " + newer.LpLastName + " (" + newer.LpPayeeCode + ")";
                //    }
                //    model.LcrCreatedDateTime = DateTime.UtcNow;
                //    model.LcrCreatedById = UserId;
                //    model.LcrUpdatedDateTime = DateTime.UtcNow;
                //    model.LcrUpdatedById = UserId;
                //    model.LcrRowId = lPayeeViewModel.Id;

                //    model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                //    model.LcrAction = "Edit";
                //    model.WFCurrentOwnerId = UserId;
                //    model.WFRequesterId = UserId;
                //    model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                //    var s = CRRC.Add(model, LoggedRoleId, Workflow);
                //    //Auto approve workflow after addition
                //    try//Auto Approve the change request and display erro in case any validation fails
                //    {
                //       GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                //    }
                //    catch (Exception ex)
                //    {
                //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                //    }
                //}

                //if (lPayeeViewModel.LpChannelId != PreviousData.LpChannelId)
                //{
                //    var model = new LChangeRequestViewModel();
                //    model.LcrColumnName = "LpChannelId";
                //    model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Channel/Region");
                //    model.LcrEntityName = "LPayees";
                //    model.WFComments = lPayeeViewModel.WFComments;
                //    if (lPayeeViewModel.LpChannelId.HasValue)
                //    {
                //        model.LcrNewId = lPayeeViewModel.LpChannelId.Value.ToString();
                //        var ChannelDetails = RCRC.GetById(lPayeeViewModel.LpChannelId.Value);
                //        model.LcrNewValue = ChannelDetails.RcName+" ("+ ChannelDetails.RcPrimaryChannel+")";
                //    }
                //    if (PreviousData.LpChannelId.HasValue)
                //    {
                //        model.LcrOldId = PreviousData.LpChannelId.Value.ToString();
                //        var PreviousChannel = RCRC.GetById(PreviousData.LpChannelId.Value);
                //        model.LcrOldValue = PreviousChannel.RcName+" ("+ PreviousChannel.RcPrimaryChannel+")";
                //    }
                //    model.LcrCreatedDateTime = DateTime.UtcNow;
                //    model.LcrCreatedById = UserId;
                //    model.LcrUpdatedDateTime = DateTime.UtcNow;
                //    model.LcrUpdatedById = UserId;
                //    model.LcrRowId = lPayeeViewModel.Id;

                //    model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                //    model.LcrAction = "Edit";
                //    model.WFCurrentOwnerId = UserId;
                //    model.WFRequesterId = UserId;
                //    model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                //    var s = CRRC.Add(model, LoggedRoleId, Workflow);
                //    //Auto approve workflow after addition
                //    try//Auto Approve the change request and display erro in case any validation fails
                //    {
                //       GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                //    }
                //    catch (Exception ex)
                //    {
                //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                //    }
                //}

                //if (lPayeeViewModel.LpSubChannelId != PreviousData.LpSubChannelId)
                //{
                //    var model = new LChangeRequestViewModel();
                //    model.LcrColumnName = "LpSubChannelId";
                //    model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "SubChannel");
                //    model.LcrEntityName = "LPayees";
                //    if (lPayeeViewModel.LpSubChannelId.HasValue)
                //    {
                //        model.LcrNewId = lPayeeViewModel.LpSubChannelId.Value.ToString();
                //        model.LcrNewValue = RSRC.GetById(lPayeeViewModel.LpSubChannelId.Value).RscName;
                //    }
                //    if (PreviousData.LpSubChannelId.HasValue)
                //    {
                //        model.LcrOldId = PreviousData.LpSubChannelId.Value.ToString();
                //        model.LcrOldValue = RSRC.GetById(PreviousData.LpSubChannelId.Value).RscName;
                //    }
                //    model.WFComments = lPayeeViewModel.WFComments;
                //    model.LcrCreatedDateTime = DateTime.UtcNow;
                //    model.LcrCreatedById = UserId;
                //    model.LcrUpdatedDateTime = DateTime.UtcNow;
                //    model.LcrUpdatedById = UserId;
                //    model.LcrRowId = lPayeeViewModel.Id;

                //    model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                //    model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                //    model.LcrAction = "Edit";
                //    model.WFCurrentOwnerId = UserId;
                //    model.WFRequesterId = UserId;
                //    var s = CRRC.Add(model, LoggedRoleId, Workflow);
                //    //Auto approve workflow after addition
                //    try//Auto Approve the change request and display erro in case any validation fails
                //    {
                //       GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                //    }
                //    catch (Exception ex)
                //    {
                //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                //    }
                //}
                #endregion
                    if (blnCompareVals(lPayeeViewModel.LpTIN,PreviousData.LpTIN))//(lPayeeViewModel.LpTIN  != PreviousData.LpTIN)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change.Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpTIN";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "TIN");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.LpTIN;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrOldValue = PreviousData.LpTIN;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.LcrAction = "Edit";
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                    }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.LpTradingName, PreviousData.LpTradingName))//((lPayeeViewModel.LpTradingName ?? "").ToString() != PreviousData.LpTradingName)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpTradingName";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Trading Name");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.LpTradingName;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrOldValue = PreviousData.LpTradingName;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.LcrAction = "Edit";
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (lPayeeViewModel.LpEffectiveEndDate != PreviousData.LpEffectiveEndDate)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpEffectiveEndDate";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Effective End Date");
                        model.LcrEntityName = "LPayees";
                        if (lPayeeViewModel.LpEffectiveEndDate.HasValue)
                            model.LcrNewValue = lPayeeViewModel.LpEffectiveEndDate.Value.ToString("dd/MM/yyyy");
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        if (PreviousData.LpEffectiveEndDate.HasValue)
                            model.LcrOldValue = PreviousData.LpEffectiveEndDate.Value.ToString("dd/MM/yyyy");
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if(blnCompareVals(lPayeeViewModel.LpFirstName, PreviousData.LpFirstName))// ((lPayeeViewModel.LpFirstName ?? "").ToString() != PreviousData.LpFirstName)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpFirstName";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "First Name");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.LpFirstName;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.LpFirstName;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }
                    //
                    if(blnCompareVals(lPayeeViewModel.LpLastName, PreviousData.LpLastName))// ((lPayeeViewModel.LpLastName ?? "" ).ToString() != PreviousData.LpLastName)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpLastName";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Last Name");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.LpLastName;
                        model.LcrCompanyId = CompanyId; model.WFCompanyId = CompanyId;
                        model.LcrOldValue = PreviousData.LpLastName;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }
                    #region DeadCode
                //if (lPayeeViewModel.LpPayeeCode != PreviousData.LpPayeeCode)
                //{
                //    var model = new LChangeRequestViewModel();
                //    model.LcrColumnName = "LpPayeeCode";
                //    model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Payee Code");
                //    model.LcrEntityName = "LPayees";
                //    model.LcrNewValue = lPayeeViewModel.LpPayeeCode;
                //    model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                //    model.LcrOldValue = PreviousData.LpPayeeCode;
                //    model.WFComments = lPayeeViewModel.WFComments;
                //    model.LcrAction = "Edit";
                //    model.LcrCreatedDateTime = DateTime.UtcNow;
                //    model.LcrCreatedById = UserId;
                //    model.LcrUpdatedDateTime = DateTime.UtcNow;
                //    model.LcrUpdatedById = UserId;
                //    model.LcrRowId = lPayeeViewModel.Id;

                //    model.WFCurrentOwnerId = UserId;
                //    model.WFRequesterId = UserId;
                //    model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                //    var s = CRRC.Add(model, LoggedRoleId, Workflow);
                //    //Auto approve workflow after addition
                //    try//Auto Approve the change request and display erro in case any validation fails
                //    {
                //       GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                //    }
                //    catch (Exception ex)
                //    {
                //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                //    }
                //}
                #endregion
                    if (blnCompareVals(lPayeeViewModel.LpPhone, PreviousData.LpPhone))//((lPayeeViewModel.LpPhone ?? "").ToString() != PreviousData.LpPhone)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpPhone";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Phone");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.LpPhone;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.LpPhone;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }
                #region Email Business unit, primary channel commented
                //if (lPayeeViewModel.LpEmail != PreviousData.LpEmail)
                //{
                //    var model = new LChangeRequestViewModel();
                //    model.LcrColumnName = "LpEmail";
                //    model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Email");
                //    model.LcrEntityName = "LPayees";
                //    model.LcrNewValue = lPayeeViewModel.LpEmail;
                //    model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                //    model.LcrOldValue = PreviousData.LpEmail;
                //    model.WFComments = lPayeeViewModel.WFComments;
                //    model.LcrAction = "Edit";
                //    model.LcrCreatedDateTime = DateTime.UtcNow;
                //    model.LcrCreatedById = UserId;
                //    model.LcrUpdatedDateTime = DateTime.UtcNow;
                //    model.LcrUpdatedById = UserId;
                //    model.LcrRowId = lPayeeViewModel.Id;
                //    model.WFCurrentOwnerId = UserId;
                //    model.WFRequesterId = UserId;
                //    model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                //    var s = CRRC.Add(model, LoggedRoleId, Workflow);
                //    //Auto approve workflow after addition
                //    try//Auto Approve the change request and display erro in case any validation fails
                //    {
                //       GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                //    }
                //    catch (Exception ex)
                //    {
                //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                //    }
                //}

                //if (lPayeeViewModel.LpPrimaryChannel != PreviousData.LpPrimaryChannel)
                //{
                //    var model = new LChangeRequestViewModel();
                //    model.LcrColumnLabel = (CompanySpecificData.Where(p => p.ColumnName.Equals("TIN", StringComparison.OrdinalIgnoreCase)).Count() > 0) ? CompanySpecificData.Where(p => p.ColumnName.Equals("TIN", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().ColumnLabel : "Primary Channel";
                //    model.LcrColumnName = "LpPrimaryChannel";
                //    model.LcrEntityName = "LPayees";
                //    model.LcrNewValue = lPayeeViewModel.LpPrimaryChannel;
                //    model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                //    model.LcrOldValue = PreviousData.LpPrimaryChannel;
                //    model.WFComments = lPayeeViewModel.WFComments;
                //    model.LcrAction = "Edit";
                //    model.LcrCreatedDateTime = DateTime.UtcNow;
                //    model.LcrCreatedById = UserId;
                //    model.LcrUpdatedDateTime = DateTime.UtcNow;
                //    model.LcrUpdatedById = UserId;
                //    model.LcrRowId = lPayeeViewModel.Id;
                //   
                //    model.WFCurrentOwnerId = UserId;
                //    model.WFRequesterId = UserId;
                //    model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                //    var s = CRRC.Add(model, LoggedRoleId, Workflow);
                //    //Auto approve workflow after addition
                //    try//Auto Approve the change request and display erro in case any validation fails
                //    {
                //       GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId);
                //    }
                //    catch (Exception ex)
                //    {
                //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                //    }
                //}

                //if (lPayeeViewModel.LpBusinessUnit != PreviousData.LpBusinessUnit)
                //{
                //    var model = new LChangeRequestViewModel();
                //    model.LcrColumnName = "LpBusinessUnit";
                //    model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Business Unit");
                //    model.LcrEntityName = "LPayees";
                //    model.LcrNewValue = lPayeeViewModel.LpBusinessUnit;
                //    model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                //    model.LcrOldValue = PreviousData.LpBusinessUnit;
                //    model.WFComments = lPayeeViewModel.WFComments;
                //    model.LcrAction = "Edit";
                //    model.LcrCreatedDateTime = DateTime.UtcNow;
                //    model.LcrCreatedById = UserId;
                //    model.LcrUpdatedDateTime = DateTime.UtcNow;
                //    model.LcrUpdatedById = UserId;
                //    model.LcrRowId = lPayeeViewModel.Id;

                //    model.WFCurrentOwnerId = UserId;
                //    model.WFRequesterId = UserId;
                //    model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                //    var s = CRRC.Add(model, LoggedRoleId, Workflow);
                //    //Auto approve workflow after addition
                //    try//Auto Approve the change request and display erro in case any validation fails
                //    {
                //       GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                //    }
                //    catch (Exception ex)
                //    {
                //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                //    }
                //}
                #endregion
                    if (blnCompareVals(lPayeeViewModel.LpChannelManager, PreviousData.LpChannelManager))//(lPayeeViewModel.LpChannelManager != PreviousData.LpChannelManager)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        IAspnetUsersRestClient AURC = new AspnetUsersRestClient();
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpChannelManager";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Channel Manager");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewId = lPayeeViewModel.LpChannelManager;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldId = PreviousData.LpChannelManager;
                        if(!string.IsNullOrEmpty(lPayeeViewModel.LpChannelManager))
                        model.LcrNewValue = AURC.GetById(lPayeeViewModel.LpChannelManager).Email;
                        if (!string.IsNullOrEmpty(PreviousData.LpChannelManager))
                            model.LcrOldValue = AURC.GetById(PreviousData.LpChannelManager).Email;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.LpPosition, PreviousData.LpPosition)) //(lPayeeViewModel.LpPosition  != PreviousData.LpPosition)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpPosition";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Position");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.LpPosition;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.LpPosition;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }
                   
                    if (blnCompareVals(lPayeeViewModel.A01, PreviousData.A01)) //((lPayeeViewModel.A01 ?? "").ToString() != PreviousData.A01)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "A01";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName,"A01");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.A01;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.A01;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.A02, PreviousData.A02)) //((lPayeeViewModel.A02 ?? "").ToString() != PreviousData.A02)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "A02";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "A02");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.A02;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.A02;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.A03, PreviousData.A03)) //((lPayeeViewModel.A03 ?? "").ToString() != PreviousData.A03)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "A03";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "A03");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.A03;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.A03;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.A04, PreviousData.A04)) //((lPayeeViewModel.A04 ?? "").ToString() != PreviousData.A04)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "A04";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "A04");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.A04;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.A04;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.A05, PreviousData.A05)) //((lPayeeViewModel.A05 ?? "").ToString() != PreviousData.A05)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "A05";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "A05");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.A05;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.A05;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.A06, PreviousData.A06)) //((lPayeeViewModel.A06 ?? "").ToString() != PreviousData.A06)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "A06";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "A06");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.A06;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.A06;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.A07, PreviousData.A07)) //((lPayeeViewModel.A07 ?? "").ToString() != PreviousData.A07)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "A07";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "A07");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.A07;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.A07;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.A08, PreviousData.A08)) //((lPayeeViewModel.A08 ?? "").ToString() != PreviousData.A08)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "A08";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "A08");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.A08;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.A08;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.A09, PreviousData.A09)) //((lPayeeViewModel.A09 ?? "").ToString() != PreviousData.A09)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "A09";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "A09");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.A09;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.A09;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.A10, PreviousData.A10)) //((lPayeeViewModel.A10 ?? "").ToString() != PreviousData.A10)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "A10";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "A10");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.A10;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrOldValue = PreviousData.A10;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrAction = "Edit";
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (lPayeeViewModel.LpEffectiveStartDate.Date != PreviousData.LpEffectiveStartDate.Date)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpEffectiveStartDate";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Start Date");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.LpEffectiveStartDate.ToString("dd/MM/yyyy");
                        model.LcrAction = "Edit";
                        model.LcrOldValue = PreviousData.LpEffectiveStartDate.ToString("dd/MM/yyyy");
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.LpAddress, PreviousData.LpAddress)) //((lPayeeViewModel.LpAddress ?? "").ToString() != PreviousData.LpAddress)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpAddress";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Address");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.LpAddress;
                        model.LcrAction = "Edit";
                        model.LcrOldValue = PreviousData.LpAddress;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.LpDistributionChannel, PreviousData.LpDistributionChannel)) //(lPayeeViewModel.LpDistributionChannel  != PreviousData.LpDistributionChannel)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpDistributionChannel";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Distribution Channel");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = lPayeeViewModel.LpDistributionChannel;
                        model.LcrAction = "Edit";
                        model.LcrOldValue = PreviousData.LpDistributionChannel;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (lPayeeViewModel.LpCanRaiseClaims  != PreviousData.LpCanRaiseClaims)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpCanRaiseClaims";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Can Raise Claims");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = Convert.ToString(lPayeeViewModel.LpCanRaiseClaims);
                        model.LcrAction = "Edit";
                        model.LcrOldValue = Convert.ToString(PreviousData.LpCanRaiseClaims);
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrCompanyId = CompanyId; model.WFCompanyId = CompanyId;
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;
                       
                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (lPayeeViewModel.LpCreateLogin != PreviousData.LpCreateLogin)
                    {
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpCreateLogin";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "Create Login");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewValue = Convert.ToString(lPayeeViewModel.LpCreateLogin);
                        model.LcrAction = "Edit";
                        model.LcrOldValue = Convert.ToString(PreviousData.LpCreateLogin);
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrCompanyId = CompanyId; model.WFCompanyId = CompanyId;
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;

                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty, LoggedRoleId, string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    }

                    if (blnCompareVals(lPayeeViewModel.LpFinOpsRoles, PreviousData.LpFinOpsRoles)) //(lPayeeViewModel.LpFinOpsRoles != PreviousData.LpFinOpsRoles)
                    {
                    //Dev 6.6) When changing Payee (adding FinOpsRole), do not create CR when if CreateLogin=0
                    //Since Payee does not have CreateLogin checked, it cannot have FinOps Role. Please create and approve CreateLogin CR first and then come back to this screen to assign FinOps role
                    //Commented by sachin - Since Payee does not have CreateLogin checked, it cannot have FinOps Role. Please create and approve CreateLogin CR first and then come back to this screen to assign FinOps role
                    //if (PreviousData.LpCreateLogin)
                    //{
                        TempData["Message"] = "A Change Request has been created for the requested change. Once the CR is approved then the change will become effective.";
                        IAspnetRolesRestClient ARRC = new AspnetRolesRestClient();
                        var model = new LChangeRequestViewModel();
                        model.LcrColumnName = "LpFinOpsRoles";
                        model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, model.LcrColumnName, "FinOps Roles");
                        model.LcrEntityName = "LPayees";
                        model.LcrNewId = Convert.ToString(lPayeeViewModel.LpFinOpsRoles);
                        model.LcrAction = "Edit";
                        model.LcrOldId = Convert.ToString(PreviousData.LpFinOpsRoles);
                        model.LcrOldValue = string.IsNullOrEmpty(PreviousData.LpFinOpsRoles) ?null:ARRC.GetById(PreviousData.LpFinOpsRoles).Name;
                        model.LcrNewValue = string.IsNullOrEmpty(lPayeeViewModel.LpFinOpsRoles) ? null : ARRC.GetById(lPayeeViewModel.LpFinOpsRoles).Name; ;
                        model.WFComments = lPayeeViewModel.WFComments;
                        model.LcrCompanyId = CompanyId; model.WFCompanyId = CompanyId;
                        model.LcrCreatedDateTime = DateTime.UtcNow;
                        model.LcrCreatedById = UserId;
                        model.LcrUpdatedDateTime = DateTime.UtcNow;
                        model.LcrUpdatedById = UserId;
                        model.LcrRowId = lPayeeViewModel.Id;

                        model.WFCurrentOwnerId = UserId;
                        model.WFRequesterId = UserId;
                        model.LcrCreatedByForm = true;
                        model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                        var s = CRRC.Add(model, LoggedRoleId, Workflow);
                        //Auto approve workflow after addition
                        try//Auto Approve the change request and display erro in case any validation fails
                        {
                           GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty, LoggedRoleId, string.Empty);
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                        }
                    //}
                    //else
                    //{
                    //    TempData["Message"] = "Since Payee does not have CreateLogin checked, it cannot have FinOps Role. Please create and approve CreateLogin CR first and then come back to this screen to assign FinOps role";
                    //    return false;
                    //}
                    }


                    //var ExistingPortfolios = LPORC.GetByEntityId("LPayees",lPayeeViewModel.Id);
                    //var NewPortfolioList = PortfolioList.Split(',').ToList();

                    //SS commented CR of Portfolios on 28JUL2017
                    //if (!Enumerable.SequenceEqual(ExistingPortfolios.OrderBy(t => t), NewPortfolioList.OrderBy(t => t)))
                    //{

                    //    var model = new LChangeRequestViewModel();
                    //    model.LcrColumnName = "Portfolios";
                    //    model.LcrColumnLabel = "Portfolios";
                    //    model.LcrEntityName = "LPayees";
                    //    model.LcrNewValue = Convert.ToString(lPayeeViewModel.LpCanRaiseClaims);
                    //    model.LcrAction = "Edit";
                    //    model.LcrOldValue = Convert.ToString(PreviousData.LpCanRaiseClaims);
                    //    model.LcrOldId = string.Join(",", ExistingPortfolios);
                    //    model.LcrNewId = PortfolioList;
                    //    model.WFComments = lPayeeViewModel.WFComments;
                    //    model.LcrCompanyId = CompanyId; model.WFCompanyId = CompanyId;
                    //    model.LcrCreatedDateTime = DateTime.UtcNow;
                    //    model.LcrCreatedById = UserId;
                    //    model.LcrUpdatedDateTime = DateTime.UtcNow;
                    //    model.LcrUpdatedById = UserId;
                    //    model.LcrRowId = lPayeeViewModel.Id;

                    //    model.WFCurrentOwnerId = UserId;
                    //    model.WFRequesterId = UserId;
                    //    model.LcrEffectiveStartDate = lPayeeViewModel.LcrEffectiveStartDate;
                    //    var s = CRRC.Add(model, LoggedRoleId, Workflow);
                    //    //Auto approve workflow after addition
                    //    try//Auto Approve the change request and display erro in case any validation fails
                    //    {
                    //       GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty, LoggedRoleId, string.Empty);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                    //    }
                    //}

                    return true;
           
            }
            catch (Exception ex)
            {
                if (lPayeeViewModel.LpChannelId.HasValue)
                {
                    ViewBag.LpChannelId = GetChannel(lPayeeViewModel.LpChannelId.Value,lPayeeViewModel.LpPrimaryChannel);
                }
                else
                {
                    ViewBag.LpChannelId = GetChannel();
                }
                //if (lPayeeViewModel.LppParentPayeeId.HasValue)
                //{
                //    ViewBag.LppParentPayeeId = GetParent(lPayeeViewModel.LppParentPayeeId.Value);
                //}
                //else
                //{
                //    ViewBag.LppParentPayeeId = GetParent();
                //}
                if (lPayeeViewModel.LpSubChannelId.HasValue)
                {
                    ViewBag.LpSubChannelId = GetSubChannel(lPayeeViewModel.LpSubChannelId.Value, lPayeeViewModel.LpChannelId.Value);
                }
                else
                {
                    ViewBag.LpSubChannelId = GetSubChannel();
                }
                ViewBag.LpChannelManager = GetChannelManager(lPayeeViewModel.LpChannelManager);
                TempData["Message"] = ex.Data["ErrorMessage"].ToString();
                //passing data of LCompanySpecificColumns data
                //ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
                var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId);
                ViewBag.CompanySpecificColumn = CompanySpecificData;
                return false;
            }
        }

        [ControllerActionFilter]
        public bool DeleteConfirmed(LPayeeViewModel LPVM)
        {
            try
            {
                ILChangeRequestsRestClient CRRC = new LChangeRequestsRestClient();
                IRStatusesRestClient RSTRC = new RStatusesRestClient();
                var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
                var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
                var StatusId = 0;
                if (Role == "System Analyst")
                {
                    StatusId = RSTRC.GetByStatusName("PendingApproval").Where(p => p.RsoStatusOwner == "ChangeRequest").FirstOrDefault().Id;
                }
                else
                {
                    StatusId = RSTRC.GetByStatusName("Accepted").Where(p => p.RsoStatusOwner == "ChangeRequest").FirstOrDefault().Id;
                }
                var model = new LChangeRequestViewModel();
                model.LcrColumnLabel = string.Empty;
                model.LcrColumnName = string.Empty;
                model.LcrEntityName = "LPayees";
                model.LcrNewValue = null;
                model.LcrOldValue = null;
                model.WFComments = null;
                model.LcrAction = "Delete";
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.LcrRowId = LPVM.Id;
                model.LcrCompanyId = CompanyId;model.WFCompanyId=CompanyId;
               
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = LPVM.LcrEffectiveStartDate;
                var s = CRRC.Add(model, LoggedRoleId, Workflow);
                var CRWorkflow = RWFRC.GetCRName(Workflow);
                //Auto approve workflow after addition
                try//Auto Approve the change request and display erro in case any validation fails
                {
                   GGRC.UpdateActionStatus(Workflow,Convert.ToString(s.Id), CompanyId, "Approve", UserId, string.Empty,LoggedRoleId,string.Empty);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                }
                // RestClient.Delete(LPVM.Id);
                return true;
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Data["ErrorMessage"].ToString();
                return false;
            }
        }

        //Portfolio Tree
       // [ControllerActionFilter]
        public JsonResult GetApprovedPayeePortfolioTree(Nullable<DateTime> AsOfDate)
        {
            ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
            var UserPortfolios = LPORC.GetByUserId(UserId,LoggedRoleId);
            var root = UserPortfolios.Select(p=> new { LpFullName = "", LpTradingName = "", id = "P"+p.Id, parentid = "-1", text =p.RcPrimaryChannel+"."+p.LpBusinessUnit+"."+p.RcName, value = "0" }).ToList();

            var Payees = RestClient.GetApprovedPayeePortfolioTree(AsOfDate.Value.ToString("yyyy-MM-dd"), CompanyId, UserId);
            var PayeeList = Payees.Select(p => new { LpFullName = p.LpFirstName + " " + p.LpLastName, p.LpTradingName, id = "P"+p.PortfolioId+"|"+p.Id, parentid = (p.LppParentPayeeId.HasValue) ? "P" + p.PortfolioId + "|"+ p.LppParentPayeeId.Value.ToString() : "P" + p.PortfolioId, text = p.FullName, value = "" + p.Id }).OrderBy(p => p.LpTradingName).ThenBy(p => p.LpFullName);
            var result = root.Union(PayeeList);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetApprovedPayeeTree(Nullable<DateTime> AsOfDate)
        {

            var root = new[] { new { LpFullName = "", LpTradingName = "", id = "D1", parentid = "-1", text = "Direct", value = "0" }, new { LpFullName = "", LpTradingName = "", id = "I1", parentid = "-1", text = "InDirect", value = "0" } };

            var Payees = RestClient.GetApprovedPayeeTree(AsOfDate.Value.ToString("yyyy-MM-dd"), CompanyId, "Direct");
            var Direct = Payees.Select(p => new { LpFullName = p.LpFirstName + " " + p.LpLastName, p.LpTradingName, id = "" + p.Id, parentid = (p.LppParentPayeeId.HasValue) ? p.LppParentPayeeId.Value.ToString() : "D1", text = p.LpFirstName + " " + p.LpLastName, value = "" + p.Id }).OrderBy(p => p.LpTradingName).ThenBy(p => p.LpFullName);
            var Indirect = RestClient.GetApprovedPayeeTree(AsOfDate.Value.ToString("yyyy-MM-dd"), CompanyId, "InDirect").Select(p => new { LpFullName = p.LpFirstName + " " + p.LpLastName, p.LpTradingName, id = "" + p.Id, parentid = (p.LppParentPayeeId.HasValue) ? p.LppParentPayeeId.Value.ToString() : "I1", text = p.LpFirstName + " " + p.LpLastName, value = "" + p.Id }).OrderBy(p => p.LpTradingName).ThenBy(p => p.LpFullName);
            var result = root.Union(Direct).Union(Indirect);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetPayeeDetails(int PayeeId)
        {
            var PayeeDetails = RestClient.GetById(PayeeId);
            var result = "<h3>" + PayeeDetails.LpFirstName + " " + PayeeDetails.LpLastName + "  (" + PayeeDetails.LpPayeeCode + ") </h3><ul><li>Parent : " + PayeeDetails.ParentName + "  (" + PayeeDetails.ParentCode + ") </li><li>Trading Name : " + PayeeDetails.LpTradingName + "</li><li>Phone : " + PayeeDetails.LpPhone + "</li><li>Email : " + PayeeDetails.LpEmail + "</li><li>Channel : " + PayeeDetails.RcName + "</li><li>SubChannel : " + PayeeDetails.RscName + "</li><li>Primary Channel : " + PayeeDetails.LpPrimaryChannel + "</li><li>Business Unit : " + PayeeDetails.LpBusinessUnit + "</li><li> Address :" + PayeeDetails.LpAddress + "</li><li>Distribution Channel : " + PayeeDetails.LpDistributionChannel + "</li><li>Position : " + PayeeDetails.LpPosition + "</li><li> Start Date : " + PayeeDetails.LpEffectiveStartDate + "</li><li> End Date : " + PayeeDetails.LpEffectiveEndDate + "</li><li>Created By: " + PayeeDetails.CreatedBy + "</li><li> Created Date Time : " + PayeeDetails.LpCreatedDateTime + "</li><li>Updated By : " + PayeeDetails.UpdatedBy + "</li><li>Updated Date Time : " + PayeeDetails.LpUpdatedDateTime + "</li></ul>";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //method to validate payee while create/edit
        //a)	ChangeEffectiveStartDate has to be Between ParentPayee's (record in LPayee table) EffectiveStartDate and EffectiveEndDate. If NOT, show message ‘Parent Payee is not valid for requested date. Choose a date between <ParentPayee's EffectiveStartDate> and < ParentPayee's EffectiveEndDate>“. This is to ensure that invalid Parent is not chosen
        //b)	ChangeEffectiveStartDate has to be 1 day in future of latest ParentPayee’s EffectiveStartDate (PayeeParent table) for that Payee. This is ensure that we do not have multiple ParentPayee records with start date and end date on same day
        [ControllerActionFilter]
        public bool ValidatePayee(LPayeeViewModel model)
        {
            if (model.LppParentPayeeId.HasValue)
            {
                Nullable<DateTime> EffectiveDate = new Nullable<DateTime>();
                if (model.FormType == "Create"|| model.FormType == "EditDetails")
                {
                    EffectiveDate = model.LpEffectiveStartDate;
                }
                else
                {
                    if (model.LcrEffectiveStartDate.HasValue)
                        EffectiveDate = model.LcrEffectiveStartDate.Value.Date;
                }
                var ParentDetails = RestClient.GetById(model.LppParentPayeeId.Value);
                //Call the new SP from 29Dec2017 to validate Payee
                if (EffectiveDate.HasValue)
                {
                    var ValidationResult = RestClient.ValidatePayeeParent(model.LpPayeeCode, ParentDetails.LpPayeeCode, model.LpPrimaryChannel, EffectiveDate.Value);
                    if (String.IsNullOrEmpty(ValidationResult))
                    {
                        return true;
                    }
                    else {
                        TempData["Message"] = ValidationResult;
                        return false;
                            }
                }
                //ILPayeeParentsRestClient LPPRC = new LPayeeParentsRestClient();
                //Nullable<DateTime> EffectiveDate = new Nullable<DateTime>();
                //if (model.FormType == "Create")
                //{
                //    EffectiveDate = model.LpEffectiveStartDate;
                //}
                //else
                //{
                //    if (model.LcrEffectiveStartDate.HasValue)
                //        EffectiveDate = model.LcrEffectiveStartDate.Value.Date;
                //}
                //if (EffectiveDate.HasValue)
                //{
                //    var AllParents = LPPRC.GetAllParentsByPayeeId(model.Id).Select(p => p.LppEffectiveStartDate);
                //    if (EffectiveDate >= ParentDetails.LpEffectiveStartDate && ((ParentDetails.LpEffectiveEndDate.HasValue) ? ParentDetails.LpEffectiveEndDate.Value : DateTime.UtcNow.AddDays(30)) >= DateTime.UtcNow)
                //    {
                //        if (AllParents.Contains(model.LpEffectiveStartDate))
                //        {
                //            TempData["Message"] = "Two Parents cannot have same effective start date";
                //            return false;
                //        }
                //        else
                //        {
                //            return true;
                //        }
                //    }
                //    else
                //    {
                //        TempData["Message"] = "Parent Payee is not valid for requested date. Choose a date between  " + ParentDetails.LpEffectiveStartDate.Date.ToString("dd/MM/yyyy") + "  and  " + ((ParentDetails.LpEffectiveEndDate.HasValue) ? ParentDetails.LpEffectiveEndDate.Value.Date.ToString("dd/MM/yyyy") : "forever") + " . ";
                //        return false;
                //    }
                //}
                return true;
            }
            else
            {
                return true;
            }
        }

        

        [HttpGet]
        public ActionResult UploadPayee()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Upload Payee";
            Helper.LPayeesRestClient URC = new Helper.LPayeesRestClient();            /*added for payeeuploadhelp*/
            string PayeeUploadHelpHTML = URC.GetPayeeUploadHelp();
            ViewBag.PayeeUploadHelpHTML = PayeeUploadHelpHTML;
            return View();
        }

        //This method is called when user licks on upload button after adding payee file
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPayee(HttpPostedFileBase File1)
        {
            ILCompanySpecificColumnsRestClient PayeeColsClient = new LCompanySpecificColumnsRestClient();
            string fileLocation = "";
            try
            {
                if (Request.Files["File1"].ContentLength > 0)
                {
                    // As directed by JS the file names would have date time stamp as suffix
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["File1"].FileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(Request.Files["File1"].FileName);
                    string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                    var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        //providing Random Name to file saved in content --> PayeeFiles
                        fileLocation = string.Format("{0}/{1}", ConfigurationManager.AppSettings["PayeeDocumentPath"], FileNames);

                        Request.Files["File1"].SaveAs(fileLocation);
                        #region Loading sheets for excel in dataset
                        //connection string
                        string excelConnectionString = string.Empty;
                        // This line is added to make a connection with the excel sheet saved  to read data from it
                        excelConnectionString = string.Format(ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"], fileLocation);
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        //OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        //Get Payee Sheet Column list from OLEDB schema
                        DataTable PayeeSheetColumns = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, "Payees$",null });
                        var columnListPayee = new List<string>();
                        if (PayeeSheetColumns != null)
                        {
                            columnListPayee.AddRange(from DataRow column in PayeeSheetColumns.Rows select column["Column_name"].ToString());
                        }
                        #region Validating Payee Template First
                        if (!ValidatePayeeSheetHeader(columnListPayee))
                        {
                            TempData["Message"] = "Uploaded file does not seem to match the Payee template. Kindly download fresh template and try again.";
                            excelConnection.Dispose();
                            if (System.IO.File.Exists(fileLocation))
                                System.IO.File.Delete(fileLocation);
                            return View();
                        }

                        #endregion
                        //Get Portfolio Sheet Column list from OLEDB schema
                        DataTable PortfolioSheetColumns = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, "Portfolios$", null });
                        var columnListPortfolio = new List<string>();
                        if (PortfolioSheetColumns != null)
                        {
                            columnListPortfolio.AddRange(from DataRow column in PortfolioSheetColumns.Rows select column["Column_name"].ToString());
                        }
                        #region Validating Payee Template First
                        if (!ValidatePortfolioSheetHeader(columnListPortfolio))
                        {
                            TempData["Message"] = "Uploaded file does not seem to match the Portfolio template. Kindly download fresh template and try again.";
                            excelConnection.Dispose();
                            if (System.IO.File.Exists(fileLocation))
                                System.IO.File.Delete(fileLocation);
                            return View();
                        }
                        #endregion
                        //DataTable dtPayee = new DataTable();
                        //DataTable dtPortfolio = new DataTable();
                        OleDbCommand command_reader = new OleDbCommand();
                        try
                        {
                            //check whether Payee sheet has data or not
                            string dataReader = "SELECT count(*) from [Payees$]";
                            command_reader = new OleDbCommand(dataReader, excelConnection);
                            int PayeeRowCount = (int)command_reader.ExecuteScalar();
                            if (PayeeRowCount == 1)
                            {
                                TempData["Message"] = "Uploaded file contains no payee data.";
                                excelConnection.Dispose();
                                if (System.IO.File.Exists(fileLocation))
                                    System.IO.File.Delete(fileLocation);
                                command_reader.Dispose();
                                return View();
                            }
                            //check whether Portfolio sheet has data or not
                            dataReader = "SELECT count(*) from [Portfolios$]";
                            command_reader = new OleDbCommand(dataReader, excelConnection);
                            int PortfolioRowCount = (int)command_reader.ExecuteScalar();
                            if (PortfolioRowCount == 0)
                            {
                                TempData["Message"] = "Uploaded file contains no portfolio data.";
                                excelConnection.Dispose();
                                if (System.IO.File.Exists(fileLocation))
                                    System.IO.File.Delete(fileLocation);
                                command_reader.Dispose();
                                return View();
                            }
                            command_reader.Dispose();
                            //string query = string.Format("Select * from [Payees$]");
                            //using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
                            //{
                            //    dataAdapter.Fill(dtPayee);
                            //}
                            ////For portfolio sheet
                            // query = string.Format("Select * from [Portfolios$]");
                            //using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
                            //{
                            //    dataAdapter.Fill(dtPortfolio);
                            //}
                        }
                        catch (Exception)
                        {
                            TempData["Message"] = "Uploaded file does not seem to match the template. Kindly download fresh template and try again.";
                           // excelConnection1.Dispose();
                            excelConnection.Dispose();
                            if (System.IO.File.Exists(fileLocation))
                                System.IO.File.Delete(fileLocation);
                            command_reader.Dispose();
                            return View();
                        }

                        #endregion
                        var CompPayeeColumn = PayeeColsClient.GetPayeeColumnsByCompanyIdForGrid(CompanyId);
                        if (CompPayeeColumn.Count() == 0)
                        {
                            TempData["Message"] = "Configuration issue with Company Specific columns, Contact Admin";
                            TempData["PayeeModelList"] = null;
                            //excelConnection1.Dispose();
                            excelConnection.Dispose();
                            if (System.IO.File.Exists(fileLocation))
                                System.IO.File.Delete(fileLocation);
                            
                            return View();
                        }
                        #region Validating Template First
                        //if (!blnCheckTemplateColumns(ds.Tables[0]))
                        //{
                        //    TempData["Message"] = "Uploaded file does not seem to match the Payee template. Kindly download fresh template and try again.";
                        //    //excelConnection1.Dispose();
                        //    excelConnection.Dispose();
                        //    if (System.IO.File.Exists(fileLocation))
                        //        System.IO.File.Delete(fileLocation);
                        //    return View();
                        //}
                        
                        //if (!blnCheckTemplateColumnsForPortfolio(dsPortfolio.Tables[0]))
                        //{
                        //    TempData["Message"] = "Uploaded file does not seem to match the Portfolio template. Kindly download fresh template and try again.";
                        //    //excelConnection1.Dispose();
                        //    excelConnection.Dispose();
                        //    if (System.IO.File.Exists(fileLocation))
                        //        System.IO.File.Delete(fileLocation);
                        //    return View();
                        //}
                        #endregion
                        //check whether Payee sheet has data or not
                        //int PayeeCount = dtPayee.Rows.Count;
                        //if (PayeeCount == 1)
                        //{
                        //    TempData["Message"] = "Uploaded file contains no payee data.";
                        //    excelConnection.Dispose();
                        //    if (System.IO.File.Exists(fileLocation))
                        //        System.IO.File.Delete(fileLocation);
                        //    return View();
                        //}
                        ////check whether Portfolio sheet has data or not
                        //int PortfolioCount = dtPortfolio.Rows.Count;
                        //if (PortfolioCount == 0)
                        //{
                        //    TempData["Message"] = "Uploaded file contains no portfolio data.";
                        //    excelConnection.Dispose();
                        //    if (System.IO.File.Exists(fileLocation))
                        //        System.IO.File.Delete(fileLocation);
                        //    return View();
                        //}
                        //upload file to S3
                        string filePath = string.Format("{0}{1}", ConfigurationManager.AppSettings["SOSBucketRootFolder"], CompanyCode + "/upload/payees");
                        if (!Globals.FolderExistsInS3(filePath))
                        {
                            Globals.CreateFolderInS3Root(filePath);
                        }
                        Globals.UploadToS3(File1.InputStream, FileNames, filePath);
                        //call RestClient for bulk upload
                        var Result = RestClient.UploadPayee(FileNames, LoggedRoleId, CompanyId, UserId,null);
                        ViewBag.ReturnMessage = "Your file has been successfully validated and added in the grid. Please press Upload button under Actions column to import this file in SOS. ";
                        System.Web.HttpContext.Current.Session["File1"] = null;
                        File1 = null;
                        return View();
                    }

                }
                System.Web.HttpContext.Current.Session["File1"] = null;
                File1 = null;
                return RedirectToAction("Index", "GenericGrid");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Record could not be validated";
                System.Web.HttpContext.Current.Session["File1"] = null;
                File1 = null;
                throw ex;
            }
        }
       
        [HttpGet]
        public ActionResult DownloadDocument(int Id)
        {
            var model = GetBatchDetailsById(Id);
            string FileName = model.LbfFileName;
            string S3BucketRootFolder = ConfigurationManager.AppSettings["SOSBucketRootFolder"];
            string S3TargetPath = S3BucketRootFolder + CompanyCode + "/upload/payees/" + FileName;
            string OriginalFileName = FileName.Split('.')[0];
            string extn = FileName.Split('.')[1];
            //var FilePath = string.Format("{0}{1}", ConfigurationManager.AppSettings["SOSBucketRootFolder"], CompanyCode + "/upload/payees");
            var FileData = Globals.DownloadFromS3(S3TargetPath, "");
            return File(FileData, "application/unknown", FileName);
        }
        private LBatchViewModelForPayeeGrid GetBatchDetailsById(int Id)
        {
            ILBatchesRestClient BRC = new LBatchesRestClient();
            LBatchViewModelForPayeeGrid model = BRC.GetDetailsById(CompanyId, Id);
            return model;
        }
        public JsonResult GetGridDataFields()
        {
            var data = RestClient.GetGridDataFields(CompanyId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult DownloadErrorFile(int Id)
        {
            var model = GetBatchDetailsById(Id);
            int BatchNumber = model.LbBatchNumber;
            //call to API for getting filename.
            string FileName = RestClient.DownloadPayeeUploadErrors(CompanyId, BatchNumber);
            Thread.Sleep(10000);
            string S3BucketRootFolder = ConfigurationManager.AppSettings["SOSBucketRootFolder"];
            string S3TargetPath = S3BucketRootFolder + CompanyCode + "/upload/payees/" + FileName;
            var FileData = Globals.DownloadFromS3(S3TargetPath, "");
            return File(FileData, "application/unknown", FileName);
            //string FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/payees/"+ FileName;
            //DirectoryInfo dir = new DirectoryInfo(FilePath);
            //dir.Refresh();
            //if (System.IO.File.Exists(FilePath))
            //{
            //    return File(FilePath, "application/octet-stream", FileName);
            //}
            //else
            //{
            //    return null;
            //}
        }
        [HttpGet]
        public JsonResult UploadValidatedPayeeBatch(int Id)
        {
            var model = GetBatchDetailsById(Id);
            int BatchNumber = model.LbBatchNumber;
            RestClient.UploadValidatedPayeeBatch(CompanyId, BatchNumber,UserId,Convert.ToInt32(LoggedRoleId));
            return Json(String.Empty,JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetXUploadPayeeCountByBatchNumber(int Id)
        {
            //var model = GetBatchDetailsById(Id);
            int BatchNumber = Id;
            int count = RestClient.GetXUploadPayeeCountByBatchNumber(CompanyId, BatchNumber);
            return Json(count, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetXUploadPayeeByBatchNumber(int BatchNo,string today, string sortdatafield, string sortorder, int? pagesize, int? pagenum)
        {
           // var model = GetBatchDetailsById(Id);
            int BatchNumber = BatchNo;
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            int PageSize = 200;
            if (pagesize != null)
            {
                PageSize = (int)pagesize;
            }
            int PageNum = 0;
            if (pagenum != null)
            {
                PageNum = (int)pagenum;
            }
            var data = RestClient.GetXUploadPayeeByBatchNumber(CompanyId, BatchNumber, sortdatafield, sortorder, PageSize, pagenum, FilterQuery);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletePayeeUploadBatch(int Id)
        {
            RestClient.DeletePayeeUploadBatch(Id);
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        Match stringMatch;
        //04122017 RK Commented method for Validate file as now both validate and upload will be taken care from same method
        #region DeadCode
        //[HttpPost]
        //[ValidateAntiForgeryToken]//rk added while code review
        //public ActionResult Validate(HttpPostedFile File1)
        //{
        //    IRChannelsRestClient RCRC = new RChannelsRestClient();
        //    IRSubChannelsRestClient RSCRC = new RSubChannelsRestClient();
        //    IAspnetUsersRestClient ASPUsers = new AspnetUsersRestClient();

        //    string fileLocation = "";
        //    try
        //    {
        //        if (Request.Files["File1"].ContentLength > 0)
        //        {
        //            // As directed by JS the file names would have date time stamp as suffix
        //            string fileExtension = System.IO.Path.GetExtension(Request.Files["File1"].FileName);
        //            string name = System.IO.Path.GetFileNameWithoutExtension(Request.Files["File1"].FileName);
        //            string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
        //            var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
        //            if (fileExtension == ".xls" || fileExtension == ".xlsx")
        //            {
        //                //providing Random Name to file saved in content --> PayeeFiles
        //                fileLocation = string.Format("{0}/{1}", ConfigurationManager.AppSettings["PayeeDocumentPath"], FileNames);

        //                Request.Files["File1"].SaveAs(fileLocation);
        //                #region Loading sheets for excel in dataset
        //                //connection string
        //                string excelConnectionString = string.Empty;
        //                // This line is added to make a connection with the excel sheet saved  to read data from it
        //                excelConnectionString = string.Format(ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"], fileLocation);
        //                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
        //                excelConnection.Open();
        //                DataTable dt = new DataTable();
        //                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //                if (dt == null)
        //                {
        //                    return null;
        //                }
        //                string[] excelSheets = new string[dt.Rows.Count];
        //                int t = 0;
        //                //excel data is saved in temporary file here
        //                foreach (DataRow row in dt.Rows)
        //                {
        //                    excelSheets[t] = row["TABLE_NAME"].ToString();
        //                    t++;
        //                }
        //                OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
        //                DataSet ds = new DataSet();
        //                DataSet dsPortfolio = new DataSet();
        //                string query = string.Format("Select * from [Payees$]");//, excelSheets[0]);
        //                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
        //                {
        //                    dataAdapter.Fill(ds);
        //                }
        //                //For portfolio sheet
        //                string query1 = string.Format("Select * from [Portfolios$]");//, excelSheets[1]);
        //                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query1, excelConnection1))
        //                {
        //                    dataAdapter.Fill(dsPortfolio);
        //                }
        //                #endregion
        //                //This variable is defined to store payee details if no error found in that row
        //                var ModelList = new List<LPayeeViewModel>();
        //                //This variable is defined to make a list of record to display in grid if record is Invalid
        //                var ErroredPayeeList = new List<LPayeeViewModel>();
        //                bool IsUploadedDataValid = true;
        //                //These lines are added to get data from reference table which will be used for validating rows
        //                var ParentPayee = RestClient.GetActivePayee(CompanyId);
        //                var AllPayeesList = RestClient.GetAllPayeeCodesAndEmailID(CompanyId);
        //                var Channel = RCRC.GetByCompanyId(CompanyId);
        //                var SubChannel = RSCRC.GetByCompanyId(CompanyId);
        //                var CurrentDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        //                var PayeeList = RestClient.GetApprovedPayeeTree(CurrentDate, CompanyId, "Direct").ToList();
        //                var UsersList = ASPUsers.GetUserByCompanyId(CompanyId);
        //                var UserRoles = System.Web.HttpContext.Current.Session["Roles"];
        //                var UserPortFolios = LPORC.GetByUserId(UserId,LoggedRoleId);
        //                string strSheetEmailIDs = "";
        //                string strPayCodes = "";
        //                string strPayeeParents = "";

        //                Boolean blnEmailFound = false;
        //                Boolean blnPayeeFound = false;
        //                Boolean blnParentFound = false;
        //                Boolean blnUserPortfolioMatch = false;
                        
        //                //## VALIDATIONS
        //                //After filling the data read from excel sheet in a data set this for loop reads the data row by row and checks for validation simultaneously
        //                //we also need to check the payees portfolio with the logged in user portfolio
        //                //if not match then show exception that "Payee portfolio does not match with your portfolio".
        //                Boolean blnPayeePortfoliofound = false;
        //                for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
        //                {
        //                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpPayeeCode"].ToString()))
        //                    {
        //                        blnPayeePortfoliofound = false;
        //                        string ErrorList = "";
        //                        blnEmailFound = false;
        //                        var model = new LPayeeViewModel();
        //                        #region Payee Code
        //                        if (string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpPayeeCode"].ToString()))
        //                        {
        //                            IsUploadedDataValid = false;
        //                            ErrorList = ErrorList + " | Payee Code is Required";
        //                        }
        //                        else
        //                        {
        //                            model.LpPayeeCode = ds.Tables[0].Rows[i]["LpPayeeCode"].ToString().Trim();
        //                            string[] strPayeeCode = strPayCodes.Split(';');
        //                            for (int d = 0; d < strPayeeCode.Length; d++)
        //                            {
        //                                if (strPayeeCode[d] == ds.Tables[0].Rows[i]["LpPayeeCode"].ToString().Trim())
        //                                {
        //                                    blnPayeeFound = true;
        //                                    IsUploadedDataValid = false;
        //                                    break;
        //                                }
        //                            }
        //                            if (!blnPayeeFound)
        //                            {
        //                                if (ParentPayee.Where(p => p.LpPayeeCode.Equals(ds.Tables[0].Rows[i]["LpPayeeCode"].ToString())).Count() == 0)//(!RestClient.CheckEmailExists(ds.Tables[0].Rows[i]["Email ID"].ToString().Trim()))
        //                                {
        //                                    if (ds.Tables[0].Rows[i]["LpPayeeCode"].ToString().Length <= 20)
        //                                    {
        //                                        model.LpPayeeCode = ds.Tables[0].Rows[i]["LpPayeeCode"].ToString().Trim();
        //                                        strPayCodes = strPayCodes + ds.Tables[0].Rows[i]["LpPayeeCode"].ToString().Trim() + ";";
        //                                    }
        //                                    else
        //                                    {

        //                                        IsUploadedDataValid = false;
        //                                        ErrorList = ErrorList + " | Payee Code can be of maximum 20 characters";
        //                                    }

        //                                }
        //                                else
        //                                {
        //                                    IsUploadedDataValid = false;

        //                                    ErrorList = ErrorList + " | Payee Code already exists in system";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                IsUploadedDataValid = false;

        //                                ErrorList = ErrorList + " | Duplicate Payee Code found in list";
        //                            }

        //                        }
        //                        //RK added to check payee code and emailID existence irrespective of their status
        //                        if (AllPayeesList.Where(p => p.LpPayeeCode.Equals(model.LpPayeeCode)).Count() > 0)//(!RestClient.CheckEmailExists(ds.Tables[0].Rows[i]["Email ID"].ToString().Trim()))
        //                        {
        //                            IsUploadedDataValid = false;
        //                            ErrorList = ErrorList + " | Payee Code already exists in the system";
        //                        }
                                
        //                        #endregion
        //                        #region Portfolio check and data selection for Primary channel, Channel and Business Unit
        //                        if (model.LpPayeeCode != "")
        //                        {
        //                            blnUserPortfolioMatch = false;
        //                            for (int k = 0; k < dsPortfolio.Tables[0].Rows.Count; k++)
        //                            {
        //                                if (dsPortfolio.Tables[0].Rows[k]["Payee Code"].ToString().Trim() == model.LpPayeeCode.Trim())
        //                                {
        //                                    blnPayeePortfoliofound = true;
        //                                    #region Primary Channel
        //                                    if (string.IsNullOrEmpty(dsPortfolio.Tables[0].Rows[k]["Primary Channel"].ToString()))
        //                                    {
        //                                        IsUploadedDataValid = false;
        //                                        ErrorList = ErrorList + " | Primary Channel is Required";
        //                                    }
        //                                    else if (dsPortfolio.Tables[0].Rows[k]["Primary Channel"].ToString().ToLower().Trim().Equals("direct") || dsPortfolio.Tables[0].Rows[k]["Primary Channel"].ToString().ToLower().Trim().Equals("indirect"))
        //                                    {
        //                                        model.LpPrimaryChannel = (dsPortfolio.Tables[0].Rows[k]["Primary Channel"].ToString()).Trim();
        //                                    }
        //                                    else
        //                                    {
        //                                        IsUploadedDataValid = false;
        //                                        ErrorList = ErrorList + " | Incorrect Input in Primary Channel";
        //                                    }
        //                                    #endregion
        //                                    #region Channel
        //                                    if (string.IsNullOrEmpty(dsPortfolio.Tables[0].Rows[k]["Channel"].ToString()))
        //                                    {
        //                                        IsUploadedDataValid = false;
        //                                        ErrorList = ErrorList + " | Channel is Required";
        //                                    }
        //                                    else
        //                                    {
        //                                        if (!string.IsNullOrEmpty(dsPortfolio.Tables[0].Rows[k]["Primary Channel"].ToString()))
        //                                        {
        //                                            if (Channel.Where(p => p.RcName.ToLower().Equals(dsPortfolio.Tables[0].Rows[k]["Channel"].ToString().ToLower()) && p.RcPrimaryChannel.ToLower().Equals(dsPortfolio.Tables[0].Rows[k]["Primary Channel"].ToString().ToLower())).Where(P => P.RcIsActive.Equals(true)).Count() > 0)
        //                                            {
        //                                                model.LpChannelId = Channel.FirstOrDefault().Id;
        //                                            }
        //                                            else
        //                                            {
        //                                                IsUploadedDataValid = false;
        //                                                ErrorList = ErrorList + " | InCorrect combination of Primary Channel and Channel (Portfolio)";
        //                                            }
        //                                        }
        //                                        //if (Channel.Where(p => p.RcName.ToLower().Equals(dsPortfolio.Tables[0].Rows[k]["Channel"].ToString())).Where(P=> P.RcPrimaryChannel.ToLower().Equals(dsPortfolio.Tables[0].Rows[k]["Primary Channel"].ToString())).Where(P=> P.RcIsActive.Equals(1)).Count() > 0)
        //                                        //{
        //                                        //    model.LpChannelId = Channel.Where(p => p.RcName.Equals(dsPortfolio.Tables[0].Rows[k]["Channel"].ToString())).FirstOrDefault().Id;
        //                                        //}
        //                                        else
        //                                        {
        //                                            IsUploadedDataValid = false;
        //                                            ErrorList = ErrorList + " | InCorrect combination of Primary Channel and Channel (Portfolio)";
        //                                        }
        //                                    }
        //                                    #endregion
        //                                    #region Business Unit
        //                                    if (string.IsNullOrEmpty(dsPortfolio.Tables[0].Rows[k]["Business Unit"].ToString()))
        //                                    {
        //                                        IsUploadedDataValid = false;
        //                                        ErrorList = ErrorList + " | Business Unit is Required";
        //                                    }
        //                                    else if (dsPortfolio.Tables[0].Rows[k]["Business Unit"].ToString().Trim().ToUpper().Equals("EBU") || dsPortfolio.Tables[0].Rows[k]["Business Unit"].ToString().Trim().ToUpper().Equals("CBU"))
        //                                    {
        //                                        model.LpBusinessUnit = (dsPortfolio.Tables[0].Rows[k]["Business Unit"].ToString()).Trim();
        //                                    }
        //                                    else
        //                                    {
        //                                        IsUploadedDataValid = false;
        //                                        ErrorList = ErrorList + " | Incorrect Input in Business Unit";
        //                                    }
        //                                    #endregion
        //                                    //User portfolio check goes here

        //                                    foreach (var uPort in UserPortFolios)
        //                                    {
        //                                        if (uPort.RcName.ToLower() == dsPortfolio.Tables[0].Rows[k]["Channel"].ToString().ToLower() && uPort.RcPrimaryChannel.ToLower() == dsPortfolio.Tables[0].Rows[k]["Primary Channel"].ToString().ToLower() && uPort.LpBusinessUnit.ToLower() == dsPortfolio.Tables[0].Rows[k]["Business Unit"].ToString().ToLower())
        //                                        {
        //                                            blnUserPortfolioMatch = true;
        //                                            continue;
        //                                        }
        //                                    }
        //                                    if (!blnUserPortfolioMatch)
        //                                    {
        //                                        IsUploadedDataValid = false;
        //                                        ErrorList = ErrorList + " | Portfolio for Payee not matching with your portfolio";
        //                                    }
        //                                }
        //                            }
        //                            if (!blnPayeePortfoliofound)
        //                            {
        //                                IsUploadedDataValid = false;
        //                                ErrorList = ErrorList + " | Portfolio for Payee code not found";
        //                            }
        //                        }

        //                        #endregion

        //                        #region Subchannel

        //                        //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpSubChannelId"].ToString()))
        //                        //{
        //                        //    if (SubChannel.Where(p => p.RscName.Equals(ds.Tables[0].Rows[i]["LpSubChannelId"].ToString())).Count() > 0)
        //                        //    {
        //                        //        model.LpSubChannelId = SubChannel.Where(p => p.RscName.Equals(ds.Tables[0].Rows[i]["LpSubChannelId"].ToString())).FirstOrDefault().Id;
        //                        //    }
        //                        //    else
        //                        //    {
        //                        //        IsUploadedDataValid = false;
        //                        //        ErrorList = ErrorList + " | InCorrect Input in SubChannel";
        //                        //    }
        //                        //    if (model.LpSubChannelId != null || model.LpSubChannelId > 0)
        //                        //    {
        //                        //        //RK Added to check combination of channel and sub channel
        //                        //        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpSubChannelId"].ToString()))
        //                        //        {
        //                        //            if (SubChannel.Where(p => p.RscChannelId.Equals(model.LpChannelId) && p.RscName.Equals(ds.Tables[0].Rows[i]["LpSubChannelId"].ToString())).Count() > 0)
        //                        //            {
        //                        //            }
        //                        //            else
        //                        //            {
        //                        //                IsUploadedDataValid = false;
        //                        //                ErrorList = ErrorList + " | InCorrect combination of Channel and Sub Channel";
        //                        //            }
        //                        //        }
        //                        //    }
        //                        //}
        //                        #endregion
        //                        #region Distribution channel
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpDistributionChannel"].ToString()))
        //                        {
        //                            model.LpDistributionChannel = (ds.Tables[0].Rows[i]["LpDistributionChannel"].ToString()).Trim();
        //                        }
        //                        #endregion
        //                        #region ParentCode
        //                        blnParentFound = false;
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpParentCode"].ToString()))
        //                        {//Note :we are not invalidationg payee if parent code does not match as parent record can be in file itself
        //                         //RK added validation, parent code needs to be in db or within the list
        //                            if (ParentPayee.Where(p => p.LpPayeeCode.Equals(ds.Tables[0].Rows[i]["LpParentCode"].ToString())).Count() > 0)//(!RestClient.CheckEmailExists(ds.Tables[0].Rows[i]["Email ID"].ToString().Trim()))
        //                            {
        //                                blnParentFound = true;
        //                            }
        //                            if (!blnParentFound)
        //                            {
        //                                for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
        //                                {
        //                                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[k]["LpPayeeCode"].ToString()) && i != k)
        //                                    {
        //                                        if (ds.Tables[0].Rows[i]["LpParentCode"].ToString() == ds.Tables[0].Rows[k]["LpPayeeCode"].ToString().Trim())
        //                                        {
        //                                            blnParentFound = true;
        //                                            break;
        //                                        }
        //                                    }

        //                                }
        //                            }
        //                            if (!blnParentFound)
        //                            {
        //                                IsUploadedDataValid = false;
        //                                ErrorList = ErrorList + " | Parent Code is not found in system or uploaded file for this payee.";
        //                            }
        //                            else if (ParentPayee.Where(p => p.LpPayeeCode.Equals(ds.Tables[0].Rows[i]["LpParentCode"].ToString())).Count() > 0)
        //                            {
        //                                model.LppParentPayeeId = ParentPayee.FirstOrDefault().Id;
        //                            }
        //                            model.ParentCode = ds.Tables[0].Rows[i]["LpParentCode"].ToString().Trim();
        //                        }
        //                        #endregion
        //                        #region FirstName
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpFirstName"].ToString()))
        //                        {
        //                            //Adding the first name in model before validation as data is being displayed in errored grid
        //                            model.LpFirstName = (ds.Tables[0].Rows[i]["LpFirstName"].ToString()).Trim();
        //                            if (ds.Tables[0].Rows[i]["LpFirstName"].ToString().Length > 100)
        //                            {
        //                                IsUploadedDataValid = false;
        //                                ErrorList = ErrorList + " | Payee First Name can be of maximum 100 characters";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            IsUploadedDataValid = false;
        //                            ErrorList = ErrorList + " | Payee First Name is required";
        //                        }
        //                        #endregion
        //                        #region LastName
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpLastName"].ToString()))
        //                        {
        //                            model.LpLastName = (ds.Tables[0].Rows[i]["LpLastName"].ToString()).Trim();
        //                            if (ds.Tables[0].Rows[i]["LpLastName"].ToString().Length > 100)
        //                            {
        //                                IsUploadedDataValid = false;
        //                                ErrorList = ErrorList + " | Payee Last Name can be of maximum 100 characters";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            IsUploadedDataValid = false;
        //                            ErrorList = ErrorList + " | Payee Last Name is required";
        //                        }
        //                        #endregion
        //                        #region TradingName
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpTradingName"].ToString()))
        //                        {
        //                            model.LpTradingName = (ds.Tables[0].Rows[i]["LpTradingName"].ToString()).Trim();
        //                            if (ds.Tables[0].Rows[i]["LpTradingName"].ToString().Length > 200)
        //                            {

        //                                IsUploadedDataValid = false;
        //                                ErrorList = ErrorList + " | Trading Name can be of maximum 200 characters";
        //                            }
        //                        }
        //                        #endregion
        //                        #region Others
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpPhone"].ToString()))
        //                        {
        //                            model.LpPhone = ds.Tables[0].Rows[i]["LpPhone"].ToString().Trim();
        //                        }
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpTIN"].ToString()))
        //                        {
        //                            model.LpTIN = ds.Tables[0].Rows[i]["LpTIN"].ToString().Trim();
        //                        }
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpPosition"].ToString()))
        //                        {
        //                            model.LpPosition = ds.Tables[0].Rows[i]["LpPosition"].ToString().Trim();
        //                        }

        //                        if ((ds.Tables[0].Rows[i]["LpCanRaiseClaims"].ToString().Equals("Y")))
        //                        {
        //                            model.LpCanRaiseClaims = true;
        //                        }
        //                        if ((ds.Tables[0].Rows[i]["LpCreateLogin"].ToString().Equals("Y")))
        //                        {
        //                            model.LpCreateLogin = true;
        //                        }


        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["WFComments"].ToString()))
        //                        {
        //                            model.WFComments = "[" + System.Web.HttpContext.Current.Session["UserName"] + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + ds.Tables[0].Rows[i]["WFComments"].ToString();
        //                        }

        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A01"].ToString()))
        //                        {
        //                            model.A01 = ds.Tables[0].Rows[i]["A01"].ToString().Trim();
        //                        }
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A02"].ToString()))
        //                        {
        //                            model.A02 = ds.Tables[0].Rows[i]["A02"].ToString().Trim();
        //                        }
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A03"].ToString()))
        //                        {
        //                            model.A03 = ds.Tables[0].Rows[i]["A03"].ToString().Trim();
        //                        }
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A04"].ToString()))
        //                        {
        //                            model.A04 = ds.Tables[0].Rows[i]["A04"].ToString().Trim();
        //                        }
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A05"].ToString()))
        //                        {
        //                            model.A05 = ds.Tables[0].Rows[i]["A05"].ToString().Trim();
        //                        }
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A06"].ToString()))
        //                        {
        //                            model.A06 = ds.Tables[0].Rows[i]["A06"].ToString().Trim();
        //                        }
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A07"].ToString()))
        //                        {
        //                            model.A07 = ds.Tables[0].Rows[i]["A07"].ToString().Trim();
        //                        }
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A08"].ToString()))
        //                        {
        //                            model.A08 = ds.Tables[0].Rows[i]["A08"].ToString().Trim();
        //                        }
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A09"].ToString()))
        //                        {
        //                            model.A09 = ds.Tables[0].Rows[i]["A09"].ToString().Trim();
        //                        }
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A10"].ToString()))
        //                        {
        //                            model.A10 = ds.Tables[0].Rows[i]["A10"].ToString().Trim();
        //                        }
        //                        #endregion
        //                        #region Address
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpAddress"].ToString()))
        //                        {
        //                            model.LpAddress = (ds.Tables[0].Rows[i]["LpAddress"].ToString()).Trim();
        //                        }
        //                        else
        //                        {
        //                            //Address is manadatory only if parent is null
        //                            if (string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpParentCode"].ToString()))
        //                            {
        //                                IsUploadedDataValid = false;
        //                                ErrorList = ErrorList + " | Address is Required";
        //                            }
        //                        }
        //                        #endregion
        //                        #region Channel Manager
        //                        //Based on discussion with JAS on 30052017, channel manager needs to be the email ID of the user.
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpChannelManager"].ToString()))
        //                        {
        //                            if (!RestClient.CheckEmailExists(ds.Tables[0].Rows[i]["LpChannelManager"].ToString().Trim()))
        //                            {
        //                                IsUploadedDataValid = false;
        //                                ErrorList = ErrorList + " | Provided Channel Manager is not a valid user in system";
        //                            }
        //                            else
        //                            {
        //                                if (UsersList.Where(p => p.Email.ToLower().Equals(ds.Tables[0].Rows[i]["LpChannelManager"].ToString().ToLower())).Count() > 0)
        //                                {
        //                                    model.LpChannelManager = UsersList.Where(p => p.Email.ToLower().Equals(ds.Tables[0].Rows[i]["LpChannelManager"].ToString().ToLower().Trim())).FirstOrDefault().Id;
        //                                    //Need to verify user roles also , user must has the role of Channel Manager
        //                                    //AspnetRoles-->AspNetUserRoles
        //                                    //Need to check active users only
        //                                    var RolesForUser = USERS.GetUserRoles(model.LpChannelManager, CompanyCode);
        //                                    Boolean blnIsChannelManager = false;
        //                                    foreach (var role in RolesForUser)
        //                                    {
        //                                        if (role.Selected == true && role.Name == "Channel Manager") blnIsChannelManager = true;

        //                                    }
        //                                    if (!blnIsChannelManager)
        //                                    {
        //                                        IsUploadedDataValid = false;
        //                                        ErrorList = ErrorList + " | Provided Channel Manager does not has Channel Manager role in system";
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    IsUploadedDataValid = false;
        //                                    ErrorList = ErrorList + " | Provided Channel Manager is not a valid user in system";
        //                                }

        //                            }
        //                        }
        //                        #endregion
        //                        #region Email Check
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpEmail"].ToString()))
        //                        {
        //                            string[] strEmail = strSheetEmailIDs.Split(';');
        //                            for (int d = 0; d < strEmail.Length; d++)
        //                            {
        //                                if (strEmail[d] == ds.Tables[0].Rows[i]["LpEmail"].ToString().Trim())
        //                                {
        //                                    blnEmailFound = true;
        //                                    IsUploadedDataValid = false;
        //                                    break;
        //                                }
        //                            }
        //                            if (!blnEmailFound)
        //                            {
        //                                if (AllPayeesList.Where(p => p.LpEmail.Equals(ds.Tables[0].Rows[i]["LpEmail"].ToString())).Count() > 0)//(!RestClient.CheckEmailExists(ds.Tables[0].Rows[i]["Email ID"].ToString().Trim()))
        //                                {
        //                                    IsUploadedDataValid = false;
        //                                    ErrorList = ErrorList + " | Email already exists in system";
        //                                }
        //                                else if (!RestClient.CheckEmailExists(ds.Tables[0].Rows[i]["LpEmail"].ToString().Trim()))
        //                                {
        //                                    model.LpEmail = (ds.Tables[0].Rows[i]["LpEmail"].ToString()).Trim();
        //                                    strSheetEmailIDs = strSheetEmailIDs + ds.Tables[0].Rows[i]["LpEmail"].ToString().Trim() + ";";
        //                                }
        //                                else
        //                                {
        //                                    IsUploadedDataValid = false;
        //                                    ErrorList = ErrorList + " | Email already exists in system";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                IsUploadedDataValid = false;
        //                                ErrorList = ErrorList + " | Duplicate Email found in list";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            IsUploadedDataValid = false;
        //                            ErrorList = ErrorList + " | Email is required";
        //                        }
        //                        #endregion
        //                        #region Effective start and end dates
        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpEffectiveStartDate"].ToString()))
        //                        {
        //                            var Dates = ds.Tables[0].Rows[i]["LpEffectiveStartDate"].ToString();
        //                            model.LpEffectiveStartDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["LpEffectiveStartDate"].ToString());
        //                            model.StartDate = ds.Tables[0].Rows[i]["LpEffectiveStartDate"].ToString();
        //                            if (!ValidatePayee(model))
        //                            {
        //                                IsUploadedDataValid = false;
        //                                ErrorList = ErrorList + " | The Effective start Date of this Payee should be between  Parent Effective Start Date and End Date ";
        //                            }
        //                        }
        //                        else
        //                        {

        //                            IsUploadedDataValid = false;
        //                            ErrorList = ErrorList + " | Effective start Date is Required ";
        //                        }

        //                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpEffectiveEndDate"].ToString()))
        //                        {
        //                            model.LpEffectiveEndDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["LpEffectiveEndDate"].ToString());
        //                            model.EndDate = ds.Tables[0].Rows[i]["LpEffectiveEndDate"].ToString();
        //                            //RK added following
        //                            if (model.LpEffectiveEndDate < model.LpEffectiveStartDate)
        //                            {
        //                                IsUploadedDataValid = false;
        //                                ErrorList = ErrorList + " | Effective End Date must be greater than Effective Start Date ";
        //                            }
        //                        }
        //                        #endregion
        //                        #region WF Columns
        //                        model.LpCompanyId = CompanyId;
        //                        model.LpCreatedById = System.Web.HttpContext.Current.Session["UserId"].ToString();
        //                        model.LpCreatedDateTime = DateTime.UtcNow;
        //                        model.WFRequesterId = UserId;
        //                        model.WFCurrentOwnerId = UserId;
        //                        model.WFRequesterRoleId = System.Web.HttpContext.Current.Session["UserRoleId"].ToString();
        //                        //RK Added
        //                        //RK Commented on 30112017
        //                        //string StatusId = "";
        //                        //if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LPStatus"].ToString())) StatusId = ds.Tables[0].Rows[i]["LPStatus"].ToString();
        //                        //else if (Role == "System Analyst") StatusId = "PendingApproval";
        //                        //else StatusId = "Accepted";

        //                       // model.LpStatus = StatusId;
        //                        model.WFStatus = "Saved";
        //                        model.LpCreatedById = UserId;
        //                        model.LpUpdatedById = UserId;
        //                        model.LpUpdatedDateTime = DateTime.UtcNow;
        //                        model.WFRequesterRoleId = System.Web.HttpContext.Current.Session["UserRoleId"].ToString();
        //                        model.WFCompanyId = CompanyId;
        //                        model.Id = 0;
        //                        #endregion
        //                        if (!ValidationSpecialChar(model))
        //                        {
        //                            IsUploadedDataValid = false;
        //                            ErrorList = ErrorList + " | Special character not allowed.";
        //                        }
        //                        if (ErrorList != "" && ErrorList.Length > 2) model.ErrorMessage = ErrorList.Substring(2, ErrorList.Length - 2);
        //                        //If no error is found so far row model is added in model list otherwise in ErroredPayeeList
        //                        if (!string.IsNullOrEmpty(ErrorList))
        //                        {
        //                            ErroredPayeeList.Add(model);
        //                        }
        //                    }
        //                }
                        
        //                //if a single record came out as Invalid No record is added and list of records with error is added in a temp variable to be displayed in grid
        //                if (IsUploadedDataValid)
        //                {
        //                    TempData["Message"] = "Record has been validated sucessfully";
        //                    TempData["PayeeModelList"] = "";
        //                    excelConnection1.Dispose();
        //                    excelConnection.Dispose();
        //                    if (System.IO.File.Exists(fileLocation))
        //                        System.IO.File.Delete(fileLocation);
        //                    return RedirectToAction("UploadPayeeHierarchy", "Lpayees");
        //                }
        //                else
        //                {
        //                    TempData["Message"] = "Record could not be validated";
        //                    TempData["PayeeModelList"] = ErroredPayeeList;
        //                    excelConnection1.Dispose();
        //                    excelConnection.Dispose();
        //                    if (System.IO.File.Exists(fileLocation))
        //                        System.IO.File.Delete(fileLocation);
        //                    return RedirectToAction("UploadPayeeHierarchy", "Lpayees");
        //                }

        //            }

        //        }
        //        return RedirectToAction("UploadPayeeHierarchy", "Lpayees");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Message"] = "File could not be Validated";
                
        //        throw ex;
        //    }
        //}
        #endregion
        private Boolean ValidationSpecialChar(LPayeeViewModel model)
        {
            if (!string.IsNullOrEmpty(model.A01)) { stringMatch = Regex.Match(model.A01, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A02)) { stringMatch = Regex.Match(model.A02, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A03)) { stringMatch = Regex.Match(model.A03, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A04)) { stringMatch = Regex.Match(model.A04, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A05)) { stringMatch = Regex.Match(model.A05, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A06)) { stringMatch = Regex.Match(model.A06, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A07)) { stringMatch = Regex.Match(model.A07, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A08)) { stringMatch = Regex.Match(model.A08, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A09)) { stringMatch = Regex.Match(model.A09, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.A10)) { stringMatch = Regex.Match(model.A10, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.Comments)) { stringMatch = Regex.Match(model.Comments, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LbfFileName)) { stringMatch = Regex.Match(model.LbfFileName, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpAddress)) { stringMatch = Regex.Match(model.LpAddress, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpBusinessUnit)) { stringMatch = Regex.Match(model.LpBusinessUnit, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpChannelManager)) { stringMatch = Regex.Match(model.LpChannelManager, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpDistributionChannel)) { stringMatch = Regex.Match(model.LpDistributionChannel, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpEmail)) { stringMatch = Regex.Match(model.LpEmail, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpFileNames)) { stringMatch = Regex.Match(model.LpFileNames, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpFirstName)) { stringMatch = Regex.Match(model.LpFirstName, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpLastName)) { stringMatch = Regex.Match(model.LpLastName, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpPayeeCode)) { stringMatch = Regex.Match(model.LpPayeeCode, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpPhone)) { stringMatch = Regex.Match(model.LpPhone, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpPosition)) { stringMatch = Regex.Match(model.LpPosition, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpPrimaryChannel)) { stringMatch = Regex.Match(model.LpPrimaryChannel, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpTIN)) { stringMatch = Regex.Match(model.LpTIN, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.LpTradingName)) { stringMatch = Regex.Match(model.LpTradingName, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.ParentCode)) { stringMatch = Regex.Match(model.ParentCode, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }
            if (!string.IsNullOrEmpty(model.ParentName)) { stringMatch = Regex.Match(model.ParentName, "[^<>'\"]*$", RegexOptions.IgnoreCase); if (stringMatch.Value == "") { return false; } }

            return true;
        }
        private Boolean ValidatePayeeSheetHeader(List<string> columnList)
        {
            //GiveCMRole		
            if (!columnList.Contains("GiveCMRole")) return false;
            if (!columnList.Contains("LpParentCode")) return false;
            if (!columnList.Contains("LpDistributionChannel")) return false;
            if (!columnList.Contains("LpParentCode")) return false;
            if (!columnList.Contains("LpPayeeCode")) return false;
            if (!columnList.Contains("LpFirstName")) return false;
            if (!columnList.Contains("LpLastName")) return false;
            if (!columnList.Contains("LpTradingName")) return false;
            if (!columnList.Contains("LpPhone")) return false;
            if (!columnList.Contains("LpEmail")) return false;
            if (!columnList.Contains("LpAddress")) return false;
            if (!columnList.Contains("LpTIN")) return false;
            if (!columnList.Contains("LpChannelManager")) return false;
            if (!columnList.Contains("LpPosition")) return false;
            if (!columnList.Contains("LpCanRaiseClaims")) return false;
            if (RestClient.CheckIsWIAMEnabled().ToLower() != "yes")
            {
                if (!columnList.Contains("LpCreateLogin")) return false;
            }
            if (!columnList.Contains("LpEffectiveStartDate")) return false;
            if (!columnList.Contains("LpEffectiveEndDate")) return false;
            if (!columnList.Contains("WFComments")) return false;
            if (!columnList.Contains("A01")) return false;
            if (!columnList.Contains("A02")) return false;
            if (!columnList.Contains("A03")) return false;
            if (!columnList.Contains("A04")) return false;
            if (!columnList.Contains("A05")) return false;
            if (!columnList.Contains("A06")) return false;
            if (!columnList.Contains("A07")) return false;
            if (!columnList.Contains("A08")) return false;
            if (!columnList.Contains("A09")) return false;
            if (!columnList.Contains("A10")) return false;
            return true;
        }
        private Boolean ValidatePortfolioSheetHeader(List<string> columnList)
        {
            if (!columnList.Contains("Payee Code")) return false;
            if (!columnList.Contains("Primary Channel")) return false;
            if (!columnList.Contains("Channel")) return false;
            if (!columnList.Contains("Business Unit")) return false;
            return true;
        }
        private Boolean blnCheckTemplateColumns(DataTable dt)
        {
            //GiveCMRole		
            if (!dt.Columns.Contains("GiveCMRole")) return false;
            if (!dt.Columns.Contains("LpParentCode")) return false;
            //if (!dt.Columns.Contains("LpPrimaryChannel")) return false;
            //if (!dt.Columns.Contains("LpBusinessUnit")) return false;
            if (!dt.Columns.Contains("LpDistributionChannel")) return false;
            //if (!dt.Columns.Contains("LpChannelId")) return false;
            //if (!dt.Columns.Contains("LpSubChannelId")) return false;
            if (!dt.Columns.Contains("LpParentCode")) return false;
            if (!dt.Columns.Contains("LpPayeeCode")) return false;
            if (!dt.Columns.Contains("LpFirstName")) return false;
            if (!dt.Columns.Contains("LpLastName")) return false;
            if (!dt.Columns.Contains("LpTradingName")) return false;
            if (!dt.Columns.Contains("LpPhone")) return false;
            if (!dt.Columns.Contains("LpEmail")) return false;
            if (!dt.Columns.Contains("LpAddress")) return false;
            if (!dt.Columns.Contains("LpTIN")) return false;
            if (!dt.Columns.Contains("LpChannelManager")) return false;
            if (!dt.Columns.Contains("LpPosition")) return false;
            if (!dt.Columns.Contains("LpCanRaiseClaims")) return false;
            if(RestClient.CheckIsWIAMEnabled().ToLower() != "yes")
            {
                if (!dt.Columns.Contains("LpCreateLogin")) return false;
            }
            
            //if (!dt.Columns.Contains("LpAuthorisedPayeeVerification")) return false;
            if (!dt.Columns.Contains("LpEffectiveStartDate")) return false;
            if (!dt.Columns.Contains("LpEffectiveEndDate")) return false;
            if (!dt.Columns.Contains("WFComments")) return false;
            if (!dt.Columns.Contains("A01")) return false;
            if (!dt.Columns.Contains("A02")) return false;
            if (!dt.Columns.Contains("A03")) return false;
            if (!dt.Columns.Contains("A04")) return false;
            if (!dt.Columns.Contains("A05")) return false;
            if (!dt.Columns.Contains("A06")) return false;
            if (!dt.Columns.Contains("A07")) return false;
            if (!dt.Columns.Contains("A08")) return false;
            if (!dt.Columns.Contains("A09")) return false;
            if (!dt.Columns.Contains("A10")) return false;
            return true;
        }
        //private Boolean blnCheckTemplateColumnsForPortfolio(DataTable dt)
        //{
        //    if (!dt.Columns.Contains("Payee Code")) return false;
        //    if (!dt.Columns.Contains("Primary Channel")) return false;
        //    if (!dt.Columns.Contains("Channel")) return false;
        //    if (!dt.Columns.Contains("Business Unit")) return false;
        //    return true;
        //}
        //This method is used to display error grid in the view by getting data from temp
        [ControllerActionFilter]
        public JsonResult DisplayUploadPayeeHierarchyErrorGrid()
        {
            var ApiData = TempData["PayeeModelList"] as List<LPayeeViewModel>;
            TempData["PayeeModelList"] = ApiData;
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //This method export data in csv for  the error grid displayed if any validation of payee fails
        [ControllerActionFilter]
        public ActionResult ExportUploadPayeeHierarchyErrorGrid() //JsonResult
        {

            var ApiData = TempData["PayeeModelList"] as List<LPayeeViewModel>;
            TempData["PayeeModelList"] = ApiData;
            var fileLocation = ConfigurationManager.AppSettings["PayeeDocumentPath"] + "/ExportPayeeErrorList.csv";
            if (System.IO.File.Exists(fileLocation))
                System.IO.File.Delete(fileLocation);
            //This line adds temporary file with name ExportPayeeErrorList and it will be exported on sucess event of this function
            using (var TextWriter = new StreamWriter(fileLocation))
            using (var csv = new CsvWriter(TextWriter))
            {

                DataRow dr;
                DataTable dt = new DataTable();
                //DataColumn Company = new DataColumn("Company");
                //DataColumn PayeeName = new DataColumn("Payee Name");
                DataColumn PayeeCode = new DataColumn("Payee Code");
                //DataColumn ParentCode = new DataColumn("Parent Code");
                //DataColumn ParentName = new DataColumn("Parent Name");
                //DataColumn StartDate = new DataColumn("Start Date");
                //DataColumn EndDate = new DataColumn("End Date");
                DataColumn Error = new DataColumn("Error");

                //dt.Columns.Add(Company);
                //dt.Columns.Add(PayeeName);
                dt.Columns.Add(PayeeCode);
                //dt.Columns.Add(ParentCode);
                //dt.Columns.Add(ParentName);
                //dt.Columns.Add(StartDate);
                //dt.Columns.Add(EndDate);
                dt.Columns.Add(Error);

                foreach (var data in ApiData)
                {
                    dr = dt.NewRow();
                    //dr["Company"] = "";
                    //dr["Payee Name"] = data.LpFirstName + " " + data.LpLastName;
                    dr["Payee Code"] = data.LpPayeeCode;
                    //dr["Parent Code"] = data.ParentCode;
                    //dr["Parent Name"] = "";
                    dr["Error"] = data.ErrorMessage;
                    //dr["Start Date"] = data.StartDate;
                    //dr["End Date"] = data.EndDate;

                    dt.Rows.Add(dr.ItemArray);
                }

                foreach (DataColumn column in dt.Columns)
                {
                    csv.WriteField(column.ColumnName);
                }
                csv.NextRecord();

                foreach (DataRow row in dt.Rows)
                {
                    for (var i = 0; i < dt.Columns.Count; i++)
                    {
                        csv.WriteField(row[i]);
                    }
                    csv.NextRecord();
                }
            }
            if (System.IO.File.Exists(fileLocation))
            {
                return File(fileLocation, "application/pdf", "ExportPayeeErrorList.csv");
            }
            TempData["Error"] = "No File found";
            return RedirectToAction("UploadPayeeHierarchy", "LPayees");
            //return Json(true, JsonRequestBehavior.AllowGet);
        }

        //This method is defined to attach files with payee while creation
        [ControllerActionFilter]
        public LPayeeViewModel AttachPayeeFiles(HttpPostedFileBase[] File1, string PayeeCode)
        {
            var UserName = System.Web.HttpContext.Current.Session["UserName"];
            var Payeemodel = new LPayeeViewModel();
            string PayeeFileName = "";
            string PayeeUserFriendlyFileName = "";
            var companyName = System.Web.HttpContext.Current.Session["CompanyName"].ToString();
            foreach (HttpPostedFileBase file in File1)
            {
                if (file.ContentLength > 0)
                {

                    var fileLocation = "";
                    string fileExtension = System.IO.Path.GetExtension(file.FileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                    string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;

                    var filePath = "";
                    /*OLD: Payee : S:\<opco>\Payees\<payeecode>\Documents\Attached
                     NEW: Payee : S:\<opco>\Payees\<payeecode>\SupportingDocuments    (while creating Payees)*/
                    filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["AttachedPayeeDocumentPath"], CompanyCode + "/Payees/" + PayeeCode + "/SupportingDocuments");
                    Payeemodel.FilePath = filePath;
                    fileLocation = filePath + "/" + FileNames;
                    //check if directory exists or not. iIf notcreate that directory
                    bool exists = System.IO.Directory.Exists(filePath);
                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                    }
                    file.SaveAs(fileLocation);

                    //made a comma seperated list of user friendly file names
                    if (string.IsNullOrEmpty(PayeeUserFriendlyFileName))
                    {
                        PayeeUserFriendlyFileName = name + fileExtension;
                    }
                    else
                    {
                        PayeeUserFriendlyFileName = PayeeUserFriendlyFileName + "," + name + fileExtension;
                    }
                    //made a comma seperated list of  file names
                    if (string.IsNullOrEmpty(PayeeFileName))
                    {
                        PayeeFileName = FileNames;
                    }
                    else
                    {
                        PayeeFileName = PayeeFileName + "," + FileNames;
                    }
                }
            }
            Payeemodel.LpFileNames = PayeeFileName;
            Payeemodel.LpUserFriendlyFileNames = PayeeUserFriendlyFileName;
            return Payeemodel;
        }

        //This method download grid data in csv format with same filters as applied in view
        //[ControllerActionFilter]
        //public JsonResult DownloadPayeeGridByTabName(string TabName)
        //{
        //    //This variable is defined to get userid of current logged in user in application
        //    var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
        //    IEnumerable<DownloadPayeeGridByTabNameViewModel> ApiData = new List<DownloadPayeeGridByTabNameViewModel>();
        //    switch (TabName)
        //    {
        //        case "Open":
        //            if (UserRole == "Sales Operations")
        //            {
        //                ApiData = RestClient.DownloadPayeeGridByStatusNameCreatedByUserId("Accepted", CompanyId, UserId);
        //            }
        //            else if (UserRole == "System Analyst" || UserRole == "Manager")
        //            {
        //                ApiData = RestClient.DownloadPayeeGridByStatusNameReportsToId("Accepted", CompanyId, UserId);
        //            }
        //            break;
        //        case "ReExamine":
        //            ApiData = RestClient.DownloadPayeeGridByStatusNameReportsToId("ReExamine", CompanyId, UserId);
        //            break;
        //        case "PendingApproval":
        //            ApiData = RestClient.DownloadPayeeGridByStatusNameReportsToId("PendingApproval", CompanyId, UserId);
        //            break;
        //        case "Withdrawn":
        //            ApiData = RestClient.DownloadPayeeGridByStatusNameReportsToId("Withdrawn", CompanyId, UserId);
        //            break;
        //        case "Rejected":
        //            ApiData = RestClient.DownloadPayeeGridByStatusNameReportsToId("Rejected", CompanyId, UserId);
        //            break;
        //        case "Approved":
        //            ApiData = RestClient.DownloadPayeeGridByStatusNameReportsToId("Approved", CompanyId, UserId);
        //            break;
        //    }
        //    var CompanySpecificColumns = LCSC.GetPayeeColumnsByCompanyId(CompanyId);
        //    var CfileLocation = Server.MapPath(ConfigurationManager.AppSettings["PayeeDocumentPath"] + "/ExportPayeeGrid.csv");
        //    var FilesPath = ConfigurationManager.AppSettings["PayeeDocumentPath"] + "/ExportPayeeGrid.csv";
        //    if (System.IO.File.Exists(CfileLocation))
        //        System.IO.File.Delete(CfileLocation);
        //    using (var CTextWriter = new StreamWriter(CfileLocation))
        //    using (var Ccsv = new CsvWriter(CTextWriter))
        //    {
        //        //The below lines of code converts the data returned from api to a datatable
        //        var tb = new DataTable(typeof(DownloadPayeeGridByTabNameViewModel).Name);

        //        PropertyInfo[] props = typeof(DownloadPayeeGridByTabNameViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //        // LPayeeViewModel PayeeModel = new LPayeeViewModel();
        //        foreach (var prop in props)
        //        {
        //            // var displayName=PayeeModel.GetDisplayName()
        //            if (CompanySpecificColumns.Where(p => p.LcscColumnName == prop.Name).Where(p => string.IsNullOrEmpty(p.LcscLabel) == false).Count() > 0)
        //            {
        //                tb.Columns.Add(CompanySpecificColumns.Where(p => p.LcscColumnName == prop.Name).FirstOrDefault().LcscLabel);
        //            }
        //            else
        //            {
        //                tb.Columns.Add(prop.Name);
        //            }
        //        }

        //        foreach (var item in ApiData)
        //        {
        //            var values = new object[props.Length];
        //            for (var i = 0; i < props.Length; i++)
        //            {
        //                values[i] = props[i].GetValue(item, null);
        //            }

        //            tb.Rows.Add(values);
        //        }


        //        foreach (DataColumn column in tb.Columns)
        //        {
        //            Ccsv.WriteField(column.ColumnName);
        //        }
        //        Ccsv.NextRecord();

        //        foreach (DataRow row in tb.Rows)
        //        {
        //            for (var i = 0; i < tb.Columns.Count; i++)
        //            {
        //                Ccsv.WriteField(row[i]);
        //            }
        //            Ccsv.NextRecord();
        //        }
        //    }

        //    return Json(FilesPath, JsonRequestBehavior.AllowGet);
        //}

        //This method returns menu items to payee Portfolio tree
       // [ControllerActionFilter]
        public JsonResult GetTreeMenuItemsForPortfolioTree(int SelectedValue)
        {
            //Firse check that clicked tree item is direct and indirect as they have hardcode id equal to zero
            if (SelectedValue == 0)
            {
                switch (UserRole)
                {
                    case "Sales Operations":
                    case "System Analyst":
                        if (LWFCRC.CheckCanCreate("Payees", LoggedRoleId, CompanyId))
                        {
                            var MenuLinks = new[] { new { id = 1, text = "<a href='/LPayees/Create?FormType=Create'>Create</a>" } };
                            return Json(MenuLinks, JsonRequestBehavior.AllowGet);
                        }
                        break;
                }

            }
            else
            {
                switch (UserRole)
                {
                    case "Manager":
                        var MenuLinks = new[] { new { id = 1, text = "<a href='/LPayees/ViewPayeeDetails?TransactionId=" + SelectedValue + "&WFConfigId=0' >Review</a>" } }.ToList();
                        return Json(MenuLinks, JsonRequestBehavior.AllowGet);
                    case "Sales Operations":
                    case "System Analyst":
                        if (LWFCRC.CheckCanCreate("Payees", LoggedRoleId, CompanyId))
                        {
                            MenuLinks = new[] { new { id = 1, text = "<a href='/LPayees/ViewPayeeDetails?TransactionId=" + SelectedValue + "&WFConfigId=0' >Review</a>" }, new { id = 2, text = "<a href='/LPayees/Create?TransactionId=" + SelectedValue + "&FormType=Create'>Create</a>" }, new { id = 3, text = "<a href='/LPayees/Create?TransactionId=" + SelectedValue + "&FormType=Edit'>Edit</a>" } }.ToList();
                            return Json(MenuLinks, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            MenuLinks = new[] { new { id = 1, text = "<a href='/LPayees/ViewPayeeDetails?TransactionId=" + SelectedValue + "&WFConfigId=0' >Review</a>" }, new { id = 2, text = "<a href='/LPayees/Create?TransactionId=" + SelectedValue + "&FormType=Edit'>Edit</a>" } }.ToList();
                            return Json(MenuLinks, JsonRequestBehavior.AllowGet);
                        }
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //This method returns menu items to payee tree
        [ControllerActionFilter]
        public JsonResult GetTreeMenuItems(int SelectedValue)
        {
            //Firse check that clicked tree item is direct and indirect as they have hardcode id equal to zero
            if (SelectedValue == 0)
            {
                switch (UserRole)
                {
                    case "Sales Operations":
                    case "System Analyst":
                        if (LWFCRC.CheckCanCreate("Payees", LoggedRoleId, CompanyId))
                        {
                            var MenuLinks = new[] { new { id = 1, text = "<a href='/LPayees/Create?FormType=Create'>Create</a>" } };
                            return Json(MenuLinks, JsonRequestBehavior.AllowGet);
                        }
                        break;
                }

            }
            else
            {
                switch (UserRole)
                {
                    case "Manager":
                        var MenuLinks = new[] { new { id = 1, text = "<a href='/LPayees/ViewPayeeDetails?TransactionId="+SelectedValue+"&WFConfigId=0' >Review</a>" } }.ToList();
                        return Json(MenuLinks, JsonRequestBehavior.AllowGet);
                    case "Sales Operations":
                    case "System Analyst":
                        if (LWFCRC.CheckCanCreate("Payees", LoggedRoleId, CompanyId))
                        {
                            MenuLinks = new[] { new { id = 1, text = "<a href='/LPayees/ViewPayeeDetails?TransactionId=" + SelectedValue + "&WFConfigId=0' >Review</a>" }, new { id = 2, text = "<a href='/LPayees/Create?TransactionId=" + SelectedValue + "&FormType=Create'>Create</a>" }, new { id = 3, text = "<a href='/LPayees/Create?TransactionId=" + SelectedValue + "&FormType=Edit'>Edit</a>" } }.ToList();
                            return Json(MenuLinks, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            MenuLinks = new[] { new { id = 1, text = "<a href='/LPayees/ViewPayeeDetails?TransactionId=" + SelectedValue + "&WFConfigId=0' >Review</a>" }, new { id = 2, text = "<a href='/LPayees/Create?TransactionId=" + SelectedValue + "&FormType=Edit'>Edit</a>" } }.ToList();
                            return Json(MenuLinks, JsonRequestBehavior.AllowGet);
                        }
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //method to get CompanySpecific Label if Present otherwise use default Label
        private string SetCompanySpecificLabel(List<LCompanySpecificColumnViewModel> LCSCVM,string ColumnName,string DefaultLabel)
        {
            //Replace Lp from Column Name
            ColumnName = ColumnName.Replace("Lp", "");
            if(LCSCVM.Where(p=>p.LcscColumnName==ColumnName).Count()>0)
            {
                var CompanySpecificLabel = LCSCVM.Where(p => p.LcscColumnName == ColumnName).FirstOrDefault().LcscLabel;
                if(!string.IsNullOrEmpty(CompanySpecificLabel))
                {
                    return CompanySpecificLabel;
                }
            }
            return DefaultLabel;
        }

        //get Portfolio grid for Payees
        [ControllerActionFilter]
        public JsonResult GetPortfolioGrid(Nullable<int> PayeeId)
        {
            if(PayeeId.HasValue&&PayeeId!=0)
            {
                var ApiData = LPORC.GetByLoggedInUserIdForEdit(UserId,PayeeId.Value,"LPayees",LoggedRoleId, "Payee", CompanyCode);
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var ApiData = LPORC.GetByUserId(UserId,LoggedRoleId);
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
        }
        List<LChangeRequestViewModel> ChangeRequestModelList = new List<LChangeRequestViewModel>();
        private Boolean LogChangeRequest( LPayeeViewModel mNewPayeeData,IEnumerable<LPayeeViewModel> ParentPayee, out string strCRBuildError)
        {
            Boolean blnCRCreated = false;
            //var ParentPayee = RestClient.GetActivePayee(CompanyId);
            var mOldPayeeData = new LPayeeViewModel();
            strCRBuildError = "";
            if (ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode)).Count() == 0) return false;
            mOldPayeeData.Id = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().Id;
            mOldPayeeData.LpPayeeCode = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpPayeeCode;
            mOldPayeeData.LpFirstName = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpFirstName;
            mOldPayeeData.LpLastName = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpLastName;
            mOldPayeeData.LpTradingName = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpTradingName;
            mOldPayeeData.LpPhone = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpPhone;
            mOldPayeeData.LpAddress = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpAddress;
            mOldPayeeData.LpTIN = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpTIN;
            mOldPayeeData.LpDistributionChannel = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpDistributionChannel;
            mOldPayeeData.A01 = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().A01;
            mOldPayeeData.A02 = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().A02;
            mOldPayeeData.A03 = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().A03;
            mOldPayeeData.A04 = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().A04;
            mOldPayeeData.A05 = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().A05;
            mOldPayeeData.A06 = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().A06;
            mOldPayeeData.A07 = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().A07;
            mOldPayeeData.A08 = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().A08;
            mOldPayeeData.A09 = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().A09;
            mOldPayeeData.A10 = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().A10;
            mOldPayeeData.LpFinOpsRoles = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpFinOpsRoles;
            //RK Added on 02122017
            mOldPayeeData.ParentCode = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().ParentCode;
            mOldPayeeData.LpEffectiveStartDate = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpEffectiveStartDate;
            mOldPayeeData.LpEffectiveEndDate = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpEffectiveEndDate;
            mOldPayeeData.LpChannelManager = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpChannelManager;
            mOldPayeeData.LpPosition = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpPosition;
            mOldPayeeData.LpCanRaiseClaims = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpCanRaiseClaims;
            mOldPayeeData.LpCreateLogin = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpCreateLogin;
            mOldPayeeData.LpPrimaryChannel = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.LpPayeeCode.ToString())).FirstOrDefault().LpPrimaryChannel;
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var CompanySpecificData = LCSCRC.GetPayeeColumnsByCompanyId(CompanyId).ToList();
            //GiveCMRole
            if (blnCompareVals(mOldPayeeData.LpFinOpsRoles, mNewPayeeData.LpFinOpsRoles))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (!string.IsNullOrEmpty(mOldPayeeData.LpFinOpsRoles)) model.LcrOldValue = "Channel Manager";
                else model.LcrOldValue = mOldPayeeData.LpFinOpsRoles;
                if (!string.IsNullOrEmpty(mNewPayeeData.LpFinOpsRoles)) model.LcrNewValue = "Channel Manager";
                else model.LcrNewValue = mNewPayeeData.LpFinOpsRoles;
                model.LcrOldId = mOldPayeeData.LpFinOpsRoles;
                model.LcrNewId = mNewPayeeData.LpFinOpsRoles;
                model.LcrColumnLabel = "FinOpsRoles";
                model.LcrColumnName = "LpFinOpsRoles";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.LpFirstName, mNewPayeeData.LpFirstName))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.LpFirstName)) mOldPayeeData.LpFirstName = "";
                if (string.IsNullOrEmpty(mNewPayeeData.LpFirstName)) mNewPayeeData.LpFirstName = "";
                model.LcrOldValue = mOldPayeeData.LpFirstName;
                model.LcrNewValue = mNewPayeeData.LpFirstName.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpFirstName", "LpFirstName");
                model.LcrColumnName = "LpFirstName";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.LpLastName, mNewPayeeData.LpLastName))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.LpLastName)) mOldPayeeData.LpLastName = "";
                if (string.IsNullOrEmpty(mNewPayeeData.LpLastName)) mNewPayeeData.LpLastName = "";
                model.LcrOldValue = mOldPayeeData.LpLastName;
                model.LcrNewValue = mNewPayeeData.LpLastName.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpLastName", "LpLastName");
                model.LcrColumnName = "LpLastName";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.LpTradingName, mNewPayeeData.LpTradingName))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.LpTradingName)) mOldPayeeData.LpTradingName = "";
                if (string.IsNullOrEmpty(mNewPayeeData.LpTradingName)) mNewPayeeData.LpTradingName = "";
                model.LcrOldValue = mOldPayeeData.LpTradingName;
                model.LcrNewValue = mNewPayeeData.LpTradingName.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpTradingName", "LpTradingName");
                model.LcrColumnName = "LpTradingName";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.LpPhone, mNewPayeeData.LpPhone))
            //if (mOldPayeeData.LpPhone.Trim() != mNewPayeeData.LpPhone.Trim())
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.LpPhone)) mOldPayeeData.LpPhone = "";
                if (string.IsNullOrEmpty(mNewPayeeData.LpPhone)) mNewPayeeData.LpPhone = "";
                model.LcrOldValue = mOldPayeeData.LpPhone;
                model.LcrNewValue = mNewPayeeData.LpPhone.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpPhone", "LpPhone");
                model.LcrColumnName = "LpPhone";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.LpAddress, mNewPayeeData.LpAddress))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.LpAddress)) mOldPayeeData.LpAddress = "";
                if (string.IsNullOrEmpty(mNewPayeeData.LpAddress)) mNewPayeeData.LpAddress = "";
                model.LcrOldValue = mOldPayeeData.LpAddress;
                model.LcrNewValue = mNewPayeeData.LpAddress.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpAddress", "LpAddress");
                model.LcrColumnName = "LpAddress";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.LpTIN, mNewPayeeData.LpTIN))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.LpTIN)) mOldPayeeData.LpTIN = "";
                if (string.IsNullOrEmpty(mNewPayeeData.LpTIN)) mNewPayeeData.LpTIN = "";
                model.LcrOldValue = mOldPayeeData.LpTIN;
                model.LcrNewValue = mNewPayeeData.LpTIN.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpTIN", "LpTIN");
                model.LcrColumnName = "LpTIN";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.LpDistributionChannel, mNewPayeeData.LpDistributionChannel))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.LpDistributionChannel)) mOldPayeeData.LpDistributionChannel = "";
                if (string.IsNullOrEmpty(mNewPayeeData.LpDistributionChannel)) mNewPayeeData.LpDistributionChannel = "";
                model.LcrOldValue = mOldPayeeData.LpDistributionChannel;
                model.LcrNewValue = mNewPayeeData.LpDistributionChannel.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpDistributionChannel", "LpDistributionChannel");
                model.LcrColumnName = "LpDistributionChannel";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            #region AColumns
            if (blnCompareVals(mOldPayeeData.A01, mNewPayeeData.A01))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.A01)) mOldPayeeData.A01 = "";
                if (string.IsNullOrEmpty(mNewPayeeData.A01)) mNewPayeeData.A01 = "";
                model.LcrOldValue = mOldPayeeData.A01;
                model.LcrNewValue = mNewPayeeData.A01.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "A01", "A01");
                model.LcrColumnName = "A01";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.A02, mNewPayeeData.A02))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.A02)) mOldPayeeData.A02 = "";
                if (string.IsNullOrEmpty(mNewPayeeData.A02)) mNewPayeeData.A02 = "";
                model.LcrOldValue = mOldPayeeData.A02;
                model.LcrNewValue = mNewPayeeData.A02.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "A02", "A02");
                model.LcrColumnName = "A02";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.A03, mNewPayeeData.A03))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.A03)) mOldPayeeData.A03 = "";
                if (string.IsNullOrEmpty(mNewPayeeData.A03)) mNewPayeeData.A03 = "";
                model.LcrOldValue = mOldPayeeData.A03;
                model.LcrNewValue = mNewPayeeData.A03.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "A03", "A03");
                model.LcrColumnName = "A03";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.A04, mNewPayeeData.A04))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.A04)) mOldPayeeData.A04 = "";
                if (string.IsNullOrEmpty(mNewPayeeData.A04)) mNewPayeeData.A04 = "";
                model.LcrOldValue = mOldPayeeData.A04;
                model.LcrNewValue = mNewPayeeData.A04.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "A04", "A04");
                model.LcrColumnName = "A04";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.A05, mNewPayeeData.A05))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.A05)) mOldPayeeData.A05 = "";
                if (string.IsNullOrEmpty(mNewPayeeData.A05)) mNewPayeeData.A05 = "";
                model.LcrOldValue = mOldPayeeData.A05;
                model.LcrNewValue = mNewPayeeData.A05.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "A05", "A05");
                model.LcrColumnName = "A05";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.A06, mNewPayeeData.A06))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.A06)) mOldPayeeData.A06 = "";
                if (string.IsNullOrEmpty(mNewPayeeData.A06)) mNewPayeeData.A06 = "";
                model.LcrOldValue = mOldPayeeData.A06;
                model.LcrNewValue = mNewPayeeData.A06.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "A06", "A06");
                model.LcrColumnName = "A06";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.A07, mNewPayeeData.A07))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.A07)) mOldPayeeData.A07 = "";
                if (string.IsNullOrEmpty(mNewPayeeData.A07)) mNewPayeeData.A07 = "";
                model.LcrOldValue = mOldPayeeData.A07;
                model.LcrNewValue = mNewPayeeData.A07.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "A07", "A07");
                model.LcrColumnName = "A07";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.A08, mNewPayeeData.A08))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.A08)) mOldPayeeData.A08 = "";
                if (string.IsNullOrEmpty(mNewPayeeData.A08)) mNewPayeeData.A08 = "";
                model.LcrOldValue = mOldPayeeData.A08;
                model.LcrNewValue = mNewPayeeData.A08.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "A08", "A08");
                model.LcrColumnName = "A08";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.A09, mNewPayeeData.A09))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.A09)) mOldPayeeData.A09 = "";
                if (string.IsNullOrEmpty(mNewPayeeData.A09)) mNewPayeeData.A09 = "";
                model.LcrOldValue = mOldPayeeData.A09;
                model.LcrNewValue = mNewPayeeData.A09.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "A09", "A09");
                model.LcrColumnName = "A09";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.A10, mNewPayeeData.A10))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.A10)) mOldPayeeData.A10 = "";
                if (string.IsNullOrEmpty(mNewPayeeData.A10)) mNewPayeeData.A10 = "";
                model.LcrOldValue = mOldPayeeData.A10;
                model.LcrNewValue = mNewPayeeData.A10.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "A10", "A10");
                model.LcrColumnName = "A10";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            #endregion
            //RK Added on 02122017
            if (blnCompareVals(mOldPayeeData.ParentCode, mNewPayeeData.ParentCode))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = DateTime.Today; //mNewPayeeData.LcrEffectiveStartDate;//RK29012018
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode;
                //RK27042018
                if (!string.IsNullOrEmpty(mNewPayeeData.ParentCode))
                {
                    if(ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.ParentCode.Trim())).Count()>0)
                    {
                        //RK26052018
                        //As part of Release2.0, it was decided that while creating CR for change in parent, no need to keep the full name for old and new value
                        //So commented below statement and stored only the payee code of the old parent
                        //model.LcrNewValue = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.ParentCode.Trim())).FirstOrDefault().FullName.ToString();//ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.ParentCode.Trim())).FirstOrDefault().LpFirstName.ToString() + " " + strNewLastName + "(" + mNewPayeeData.ParentCode.Trim() + ")"; // mNewPayeeData.ParentCode.Trim();
                        model.LcrNewValue = mNewPayeeData.ParentCode.Trim();
                    }
                    else
                    {
                        strCRBuildError = "Payee CR cannot be created for change in Payee Parent as the parent must exists in system.";
                        blnCRCreated =false;
                        return false;
                    }
                    
                }
                
                if (!string.IsNullOrEmpty(mOldPayeeData.ParentCode))
                {
                    //RK26052018
                    //As part of Release2.0, it was decided that while creating CR for change in parent, no need to keep the full name for old and new value
                    //So commented below statement and stored only the payee code of the old parent
                    //model.LcrOldValue = ParentPayee.Where(p => p.LpPayeeCode.Equals(mOldPayeeData.ParentCode.Trim())).FirstOrDefault().FullName.ToString();// ParentPayee.Where(p => p.LpPayeeCode.Equals(mOldPayeeData.ParentCode.Trim())).FirstOrDefault().LpFirstName.ToString() + " " + strOldLastName + "("+ mOldPayeeData.ParentCode.Trim() + ")";// mOldPayeeData.ParentCode;
                    model.LcrOldValue = mOldPayeeData.ParentCode;
                }
                
                model.LcrColumnLabel = "ParentPayee";// SetCompanySpecificLabel(CompanySpecificData, "ParentCode", "ParentCode");
                if (!string.IsNullOrEmpty(mNewPayeeData.ParentCode)) model.LcrNewId = ParentPayee.Where(p => p.LpPayeeCode.Equals(mNewPayeeData.ParentCode.Trim())).FirstOrDefault().Id.ToString() ;//Id of new parent//RK29012018
                if (!string.IsNullOrEmpty(mOldPayeeData.ParentCode)) model.LcrOldId = ParentPayee.Where(p => p.LpPayeeCode.Equals(mOldPayeeData.ParentCode.Trim())).FirstOrDefault().Id.ToString(); //Id of old parent//RK29012018
                model.LcrColumnName = "LppParentPayeeId";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (mOldPayeeData.LpEffectiveStartDate != mNewPayeeData.LpEffectiveStartDate)
            {
                //DateTime efOldSdt = efOldSdt = DateTime.ParseExact(mOldPayeeData.LpEffectiveStartDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime efNewSdt = DateTime.ParseExact(mNewPayeeData.LpEffectiveStartDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //if (efNewSdt != efOldSdt)
                if (mOldPayeeData.LpEffectiveStartDate != mNewPayeeData.LpEffectiveStartDate)
                {
                    var model = new LChangeRequestViewModel();
                    model.LcrEntityName = "LPayees";
                    model.LcrRowId = mOldPayeeData.Id;
                    model.LcrCompanyId = CompanyId;
                    model.WFCompanyId = CompanyId;
                    model.LcrCreatedDateTime = DateTime.UtcNow;
                    model.LcrCreatedById = UserId;
                    model.LcrUpdatedDateTime = DateTime.UtcNow;
                    model.LcrUpdatedById = UserId;
                    model.WFComments = mNewPayeeData.WFComments;
                    model.LcrAction = "Edit";
                    model.WFCurrentOwnerId = UserId;
                    model.WFRequesterId = UserId;
                    model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                    model.LpPayeeCode = mOldPayeeData.LpPayeeCode;
                    model.LcrOldValue = mOldPayeeData.LpEffectiveStartDate.ToString("dd/MM/yyyy");
                    model.LcrNewValue = mNewPayeeData.LpEffectiveStartDate.ToString("dd/MM/yyyy");
                    model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpEffectiveStartDate", "LpEffectiveStartDate");
                    model.LcrColumnName = "LpEffectiveStartDate";
                    ChangeRequestModelList.Add(model);
                    blnCRCreated = true;
                }
            }
            if (mOldPayeeData.LpEffectiveEndDate != mNewPayeeData.LpEffectiveEndDate)
            {
                //DateTime efOldEdt = new DateTime();
                //if(mOldPayeeData.LpEffectiveEndDate!=null) efOldEdt =  DateTime.ParseExact(mOldPayeeData.LpEffectiveEndDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime efNewEdt = new DateTime();
                //if(mNewPayeeData.LpEffectiveEndDate!= null) efNewEdt = DateTime.ParseExact(mNewPayeeData.LpEffectiveEndDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //if (efNewEdt != efOldEdt)
                if (mOldPayeeData.LpEffectiveEndDate != mNewPayeeData.LpEffectiveEndDate)
                {
                    var model = new LChangeRequestViewModel();
                    model.LcrEntityName = "LPayees";
                    model.LcrRowId = mOldPayeeData.Id;
                    model.LcrCompanyId = CompanyId;
                    model.WFCompanyId = CompanyId;
                    model.LcrCreatedDateTime = DateTime.UtcNow;
                    model.LcrCreatedById = UserId;
                    model.LcrUpdatedDateTime = DateTime.UtcNow;
                    model.LcrUpdatedById = UserId;
                    model.WFComments = mNewPayeeData.WFComments;
                    model.LcrAction = "Edit";
                    model.WFCurrentOwnerId = UserId;
                    model.WFRequesterId = UserId;
                    model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                    model.LpPayeeCode = mOldPayeeData.LpPayeeCode;
                    //Modify For Issue raised on 27/06/2018 mail
                    if(mOldPayeeData.LpEffectiveEndDate == null)
                    {
                        model.LcrOldValue = "";
                    }else
                    {
                        DateTime dt = Convert.ToDateTime(mOldPayeeData.LpEffectiveEndDate);
                        model.LcrOldValue = dt.ToString("dd/MM/yyyy");
                    }

                    if (mNewPayeeData.LpEffectiveEndDate == null)
                    {
                        model.LcrNewValue = "";
                    }
                    else
                    {
                        DateTime dt = Convert.ToDateTime(mNewPayeeData.LpEffectiveEndDate);
                        model.LcrNewValue = dt.ToString("dd/MM/yyyy");
                    }
                    //model.LcrNewValue = mNewPayeeData.LpEffectiveEndDate.ToString();
                    model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpEffectiveEndDate", "LpEffectiveEndDate");
                    model.LcrColumnName = "LpEffectiveEndDate";
                    ChangeRequestModelList.Add(model);
                    blnCRCreated = true;
                }
            }
            if (blnCompareVals(mOldPayeeData.LpChannelManager, mNewPayeeData.LpChannelManager))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.LpChannelManager)) mOldPayeeData.LpChannelManager = "";
                if (string.IsNullOrEmpty(mNewPayeeData.LpChannelManager)) mNewPayeeData.LpChannelManager = "";
                model.LcrOldValue = mOldPayeeData.LpChannelManager;
                model.LcrNewValue = mNewPayeeData.LpChannelManager.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpChannelManager", "LpChannelManager");
                model.LcrColumnName = "LpChannelManager";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (blnCompareVals(mOldPayeeData.LpPosition, mNewPayeeData.LpPosition))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.LpPosition)) mOldPayeeData.LpPosition = "";
                if (string.IsNullOrEmpty(mNewPayeeData.LpPosition)) mNewPayeeData.LpPosition = "";
                model.LcrOldValue = mOldPayeeData.LpPosition;
                model.LcrNewValue = mNewPayeeData.LpPosition.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpPosition", "LpPosition");
                model.LcrColumnName = "LpPosition";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            if (mOldPayeeData.LpCanRaiseClaims != mNewPayeeData.LpCanRaiseClaims)
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                model.LcrOldValue = mOldPayeeData.LpCanRaiseClaims.ToString().Trim();
                model.LcrNewValue = mNewPayeeData.LpCanRaiseClaims.ToString().Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpCanRaiseClaims", "LpCanRaiseClaims");
                model.LcrColumnName = "LpCanRaiseClaims";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            
            if (RestClient.CheckIsWIAMEnabled().ToLower() != "yes")
            {
                if (mOldPayeeData.LpCreateLogin != mNewPayeeData.LpCreateLogin)
                {
                    var model = new LChangeRequestViewModel();
                    model.LcrEntityName = "LPayees";
                    model.LcrRowId = mOldPayeeData.Id;
                    model.LcrCompanyId = CompanyId;
                    model.WFCompanyId = CompanyId;
                    model.LcrCreatedDateTime = DateTime.UtcNow;
                    model.LcrCreatedById = UserId;
                    model.LcrUpdatedDateTime = DateTime.UtcNow;
                    model.LcrUpdatedById = UserId;
                    model.WFComments = mNewPayeeData.WFComments;
                    model.LcrAction = "Edit";
                    model.WFCurrentOwnerId = UserId;
                    model.WFRequesterId = UserId;
                    model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                    model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                    model.LcrOldValue = mOldPayeeData.LpCreateLogin.ToString().Trim();
                    model.LcrNewValue = mNewPayeeData.LpCreateLogin.ToString().Trim();
                    model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpCreateLogin", "LpCreateLogin");
                    model.LcrColumnName = "LpCreateLogin";
                    ChangeRequestModelList.Add(model);
                    blnCRCreated = true;
                }
            }
            //Primary channel
            if (blnCompareVals(mOldPayeeData.LpPrimaryChannel, mNewPayeeData.LpPrimaryChannel))
            {
                var model = new LChangeRequestViewModel();
                model.LcrEntityName = "LPayees";
                model.LcrRowId = mOldPayeeData.Id;
                model.LcrCompanyId = CompanyId;
                model.WFCompanyId = CompanyId;
                model.LcrCreatedDateTime = DateTime.UtcNow;
                model.LcrCreatedById = UserId;
                model.LcrUpdatedDateTime = DateTime.UtcNow;
                model.LcrUpdatedById = UserId;
                model.WFComments = mNewPayeeData.WFComments;
                model.LcrAction = "Edit";
                model.WFCurrentOwnerId = UserId;
                model.WFRequesterId = UserId;
                model.LcrEffectiveStartDate = mNewPayeeData.LcrEffectiveStartDate;
                model.LpPayeeCode = mOldPayeeData.LpPayeeCode.Trim();
                if (string.IsNullOrEmpty(mOldPayeeData.LpPrimaryChannel)) mOldPayeeData.LpPrimaryChannel = "";
                if (string.IsNullOrEmpty(mNewPayeeData.LpPrimaryChannel)) mNewPayeeData.LpPrimaryChannel = "";
                model.LcrOldValue = mOldPayeeData.LpPrimaryChannel;
                model.LcrNewValue = mNewPayeeData.LpPrimaryChannel.Trim();
                model.LcrColumnLabel = SetCompanySpecificLabel(CompanySpecificData, "LpPrimaryChannel", "LpPrimaryChannel");
                model.LcrColumnName = "LpPrimaryChannel";
                ChangeRequestModelList.Add(model);
                blnCRCreated = true;
            }
            //mOldPayeeData.LpCreateLogin = Pa
            return blnCRCreated;
        }
        private Boolean blnCompareVals(string strVal1, string strVal2)
        {
            if (strVal1 == null && strVal2 == null) return false;
            if (string.IsNullOrEmpty(strVal1) && string.IsNullOrEmpty(strVal2)) return false;
            if (string.IsNullOrEmpty(strVal1)) strVal1 = "";
            if (string.IsNullOrEmpty(strVal2)) strVal2 = "";
            if (strVal1.Trim().ToLower() == strVal2.Trim().ToLower()) return false;
            return true;
        }
        private void ValidateUploadPayee(DataSet ds, DataSet dsPortfolio, Boolean blnUpload)
        {
            //RK 28102017 Need to abandon this method
            
            IRChannelsRestClient RCRC = new RChannelsRestClient();
            IRSubChannelsRestClient RSCRC = new RSubChannelsRestClient();
            IAspnetUsersRestClient ASPUsers = new AspnetUsersRestClient();
            ILCompanySpecificColumnsRestClient PayeeColsClient = new LCompanySpecificColumnsRestClient();
            var ModelList = new List<LPayeeViewModel>();
            var PortfolioList = new List<mEntiryPortfolioViewModel>();
            var ErroredPayeeList = new List<LPayeeViewModel>();
            bool IsUploadedDataValid = true;
            //These lines are added to get data from reference table which will be used for validating rows
            var ParentPayee = RestClient.GetActivePayee(CompanyId);
            var AllPayeesList = RestClient.GetAllPayeeCodesAndEmailID(CompanyId);
            var Channel = RCRC.GetByCompanyId(CompanyId);
            var SubChannel = RSCRC.GetByCompanyId(CompanyId);
            var CurrentDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
         //   var PayeeList = RestClient.GetApprovedPayeeTree(CurrentDate, CompanyId, "Direct").ToList();
            var UsersList = ASPUsers.GetUserByCompanyId(CompanyId);
            var UserRoles = System.Web.HttpContext.Current.Session["Roles"];
            var UserPortFolios = LPORC.GetByUserId(UserId,LoggedRoleId);
            var CompPayeeColumn = PayeeColsClient.GetPayeeColumnsByCompanyIdForGrid(CompanyId);
            string strSheetEmailIDs = "";
            string strPayCodes = "";
            string strPayeeParents = "";

            Boolean blnEmailFound = false;
            Boolean blnPayeeFound = false;
            Boolean blnParentFound = false;
            Boolean blnUserPortfolioMatch = false;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var model = new LPayeeViewModel();
                string ErrorList = "";
                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpPayeeCode"].ToString()))
                {
                    if (ds.Tables[0].Rows[i]["LpPayeeCode"].ToString().Length <= 20)
                    {
                        model.LpPayeeCode = ds.Tables[0].Rows[i]["LpPayeeCode"].ToString().Trim();
                        strPayCodes = strPayCodes + ds.Tables[0].Rows[i]["LpPayeeCode"].ToString().Trim() + ";";
                    }
                    else
                    {
                        IsUploadedDataValid = false;
                        ErrorList = ErrorList + " | Payee Code can be of maximum 20 characters";
                    }

                    #region FirstName
                    if (CompPayeeColumn.Where(p => p.ColumnName.Equals("LpFirstName")).Where(p => p.LcscIsMandatory.Equals(true)).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpFirstName"].ToString()))
                        {
                            model.LpFirstName = (ds.Tables[0].Rows[i]["LpFirstName"].ToString()).Trim();
                            if (ds.Tables[0].Rows[i]["LpFirstName"].ToString().Length > 100)
                            {
                                IsUploadedDataValid = false;
                                ErrorList = ErrorList + " | Payee First Name can be of maximum 100 characters";
                            }
                        }
                        else
                        {
                            IsUploadedDataValid = false;
                            ErrorList = ErrorList + " | Payee First Name is required";
                        }
                    }
                    #endregion
                    #region LastName
                    if (CompPayeeColumn.Where(p => p.ColumnName.Equals("LpLastName")).Where(p => p.LcscIsMandatory.Equals(true)).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpLastName"].ToString()))
                        {
                            model.LpLastName = (ds.Tables[0].Rows[i]["LpLastName"].ToString()).Trim();
                            if (ds.Tables[0].Rows[i]["LpLastName"].ToString().Length > 100)
                            {
                                IsUploadedDataValid = false;
                                ErrorList = ErrorList + " | Payee Last Name can be of maximum 100 characters";
                            }
                        }
                        else
                        {
                            IsUploadedDataValid = false;
                            ErrorList = ErrorList + " | Payee Last Name is required";
                        }
                    }
                    #endregion
                    #region TradingName
                    if (CompPayeeColumn.Where(p => p.ColumnName.Equals("LpTradingName")).Where(p => p.LcscIsMandatory.Equals(true)).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpTradingName"].ToString()))
                        {
                            model.LpTradingName = (ds.Tables[0].Rows[i]["LpTradingName"].ToString()).Trim();
                            if (ds.Tables[0].Rows[i]["LpTradingName"].ToString().Length > 200)
                            {

                                IsUploadedDataValid = false;
                                ErrorList = ErrorList + " | Trading Name can be of maximum 200 characters";
                            }
                        }
                    }
                    #endregion
                    #region Others
                    if (CompPayeeColumn.Where(p => p.ColumnName.Equals("LpPhone")).Where(p => p.LcscIsMandatory.Equals(true)).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpPhone"].ToString()))
                        {
                            model.LpPhone = ds.Tables[0].Rows[i]["LpPhone"].ToString().Trim();
                        }
                        else
                        {
                            IsUploadedDataValid = false;
                            ErrorList = ErrorList + " | Value for " + CompPayeeColumn.Where(p => p.ColumnName.Equals("LpPhone")).FirstOrDefault().ColumnLabel ?? "LpPhone" + " is required";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpPhone"].ToString()))
                        {
                            model.LpPhone = ds.Tables[0].Rows[i]["LpPhone"].ToString().Trim();
                        }
                    }
                    if (CompPayeeColumn.Where(p => p.ColumnName.Equals("LpTIN")).Where(p => p.LcscIsMandatory.Equals(true)).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpTIN"].ToString()))
                        {
                            model.LpTIN = ds.Tables[0].Rows[i]["LpTIN"].ToString().Trim();
                        }
                        else
                        {
                            IsUploadedDataValid = false;
                            ErrorList = ErrorList + " | Value for " + CompPayeeColumn.Where(p => p.ColumnName.Equals("LpTIN")).FirstOrDefault().ColumnLabel ?? "LpTIN" + " is required";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpTIN"].ToString()))
                        {
                            model.LpTIN = ds.Tables[0].Rows[i]["LpTIN"].ToString().Trim();
                        }
                    }
                    if (CompPayeeColumn.Where(p => p.ColumnName.Equals("LpPosition")).Where(p => p.LcscIsMandatory.Equals(true)).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpPosition"].ToString()))
                        {
                            model.LpPosition = ds.Tables[0].Rows[i]["LpPosition"].ToString().Trim();
                        }
                        else
                        {
                            IsUploadedDataValid = false;
                            ErrorList = ErrorList + " | Value for " + CompPayeeColumn.Where(p => p.ColumnName.Equals("LpPosition")).FirstOrDefault().ColumnLabel ?? "LpPosition" + " is required";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpPosition"].ToString()))
                        {
                            model.LpPosition = ds.Tables[0].Rows[i]["LpPosition"].ToString().Trim();
                        }
                    }
                    if (CompPayeeColumn.Where(p => p.ColumnName.Equals("LpCanRaiseClaims")).Where(p => p.LcscIsMandatory.Equals(true)).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpCanRaiseClaims"].ToString()))
                        {
                            if ((ds.Tables[0].Rows[i]["LpCanRaiseClaims"].ToString().Equals("Y")))
                            {
                                model.LpCanRaiseClaims = true;
                            }
                        }
                        else
                        {
                            IsUploadedDataValid = false;
                            ErrorList = ErrorList + " | Value for " + CompPayeeColumn.Where(p => p.ColumnName.Equals("LpCanRaiseClaims")).FirstOrDefault().ColumnLabel ?? "LpCanRaiseClaims" + " is required";
                        }
                    }
                    else
                    {
                        if ((ds.Tables[0].Rows[i]["LpCanRaiseClaims"].ToString().Equals("Y")))
                        {
                            model.LpCanRaiseClaims = true;
                        }
                    }
                    if (CompPayeeColumn.Where(p => p.ColumnName.Equals("LpCreateLogin")).Where(p => p.LcscIsMandatory.Equals(true)).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["LpCreateLogin"].ToString()))
                        {
                            if ((ds.Tables[0].Rows[i]["LpCreateLogin"].ToString().Equals("Y")))
                            {
                                model.LpCreateLogin = true;
                            }
                        }
                        else
                        {
                            IsUploadedDataValid = false;
                            ErrorList = ErrorList + " | Value for " + CompPayeeColumn.Where(p => p.ColumnName.Equals("LpCreateLogin")).FirstOrDefault().ColumnLabel ?? "LpCreateLogin" + " is required";
                        }
                    }
                    else
                    {
                        if ((ds.Tables[0].Rows[i]["LpCreateLogin"].ToString().Equals("Y")))
                        {
                            model.LpCreateLogin = true;
                        }
                    }
                    if (CompPayeeColumn.Where(p => p.ColumnName.Equals("WFComments")).Where(p => p.LcscIsMandatory.Equals(true)).Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["WFComments"].ToString()))
                        {
                            model.WFComments = "[" + System.Web.HttpContext.Current.Session["UserName"] + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + ds.Tables[0].Rows[i]["WFComments"].ToString();
                        }
                        else
                        {
                            IsUploadedDataValid = false;
                            ErrorList = ErrorList + " | Value for " + CompPayeeColumn.Where(p => p.ColumnName.Equals("WFComments")).FirstOrDefault().ColumnLabel ?? "WFComments" + " is required";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["WFComments"].ToString()))
                        {
                            model.WFComments = "[" + System.Web.HttpContext.Current.Session["UserName"] + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + ds.Tables[0].Rows[i]["WFComments"].ToString();
                        }
                    }
                    #endregion
                    #region AColumns
                    string strAColumnErrors = "";
                    #region A01
                    strAColumnErrors = ValidateAColumns("A01", CompPayeeColumn, ds.Tables[0].Rows[i]["A01"].ToString());
                    if (strAColumnErrors == "")
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A01"].ToString())) model.A01 = ds.Tables[0].Rows[i]["A01"].ToString();
                    }
                    else
                    {
                        IsUploadedDataValid = false;
                        ErrorList = ErrorList + " | " + strAColumnErrors;
                    }
                    #endregion
                    #region A02
                    strAColumnErrors = ValidateAColumns("A02", CompPayeeColumn, ds.Tables[0].Rows[i]["A02"].ToString());
                    if (strAColumnErrors == "")
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A02"].ToString())) model.A01 = ds.Tables[0].Rows[i]["A02"].ToString();
                    }
                    else
                    {
                        IsUploadedDataValid = false;
                        ErrorList = ErrorList + " | " + strAColumnErrors;
                    }
                    #endregion  
                    #region A03
                    strAColumnErrors = ValidateAColumns("A03", CompPayeeColumn, ds.Tables[0].Rows[i]["A03"].ToString());
                    if (strAColumnErrors == "")
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A03"].ToString())) model.A01 = ds.Tables[0].Rows[i]["A03"].ToString();
                    }
                    else
                    {
                        IsUploadedDataValid = false;
                        ErrorList = ErrorList + " | " + strAColumnErrors;
                    }
                    #endregion
                    #region A04
                    strAColumnErrors = ValidateAColumns("A04", CompPayeeColumn, ds.Tables[0].Rows[i]["A04"].ToString());
                    if (strAColumnErrors == "")
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A04"].ToString())) model.A01 = ds.Tables[0].Rows[i]["A04"].ToString();
                    }
                    else
                    {
                        IsUploadedDataValid = false;
                        ErrorList = ErrorList + " | " + strAColumnErrors;
                    }
                    #endregion
                    #region A05
                    strAColumnErrors = ValidateAColumns("A05", CompPayeeColumn, ds.Tables[0].Rows[i]["A05"].ToString());
                    if (strAColumnErrors == "")
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A05"].ToString())) model.A01 = ds.Tables[0].Rows[i]["A05"].ToString();
                    }
                    else
                    {
                        IsUploadedDataValid = false;
                        ErrorList = ErrorList + " | " + strAColumnErrors;
                    }
                    #endregion
                    #region A06
                    strAColumnErrors = ValidateAColumns("A06", CompPayeeColumn, ds.Tables[0].Rows[i]["A06"].ToString());
                    if (strAColumnErrors == "")
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A06"].ToString())) model.A01 = ds.Tables[0].Rows[i]["A06"].ToString();
                    }
                    else
                    {
                        IsUploadedDataValid = false;
                        ErrorList = ErrorList + " | " + strAColumnErrors;
                    }
                    #endregion
                    #region A07
                    strAColumnErrors = ValidateAColumns("A07", CompPayeeColumn, ds.Tables[0].Rows[i]["A07"].ToString());
                    if (strAColumnErrors == "")
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A07"].ToString())) model.A01 = ds.Tables[0].Rows[i]["A07"].ToString();
                    }
                    else
                    {
                        IsUploadedDataValid = false;
                        ErrorList = ErrorList + " | " + strAColumnErrors;
                    }
                    #endregion
                    #region A08
                    strAColumnErrors = ValidateAColumns("A08", CompPayeeColumn, ds.Tables[0].Rows[i]["A08"].ToString());
                    if (strAColumnErrors == "")
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A08"].ToString())) model.A01 = ds.Tables[0].Rows[i]["A08"].ToString();
                    }
                    else
                    {
                        IsUploadedDataValid = false;
                        ErrorList = ErrorList + " | " + strAColumnErrors;
                    }
                    #endregion
                    #region A09
                    strAColumnErrors = ValidateAColumns("A09", CompPayeeColumn, ds.Tables[0].Rows[i]["A09"].ToString());
                    if (strAColumnErrors == "")
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A09"].ToString())) model.A01 = ds.Tables[0].Rows[i]["A09"].ToString();
                    }
                    else
                    {
                        IsUploadedDataValid = false;
                        ErrorList = ErrorList + " | " + strAColumnErrors;
                    }
                    #endregion
                    #region A10
                    strAColumnErrors = ValidateAColumns("A10", CompPayeeColumn, ds.Tables[0].Rows[i]["A10"].ToString());
                    if (strAColumnErrors == "")
                    {
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["A10"].ToString())) model.A01 = ds.Tables[0].Rows[i]["A10"].ToString();
                    }
                    else
                    {
                        IsUploadedDataValid = false;
                        ErrorList = ErrorList + " | " + strAColumnErrors;
                    }
                    #endregion
                    #endregion
                }
            }
        }
        ILDropDownValuesRestClient DRCV = new LDropDownValuesRestClient();
        private string ValidateAColumns( string strColumnName, IEnumerable<LCompanySpecificColumnViewModel> CompPayeeColumn, string strColumnValue)
        {
            string strRetVal = "";
            Boolean blnValueMatched = false;
            if (CompPayeeColumn.Where(p => p.ColumnName.Equals(strColumnName)).Where(p => p.LcscIsMandatory.Equals(true)).Count() > 0)
            {
                if (string.IsNullOrEmpty(strColumnValue))
                {
                    //strRetVal = "Value of " + CompPayeeColumn.Where(p => p.ColumnName.Equals(strColumnName)).FirstOrDefault().LcscLabel ?? strColumnName ;
                    //strRetVal += " is required";
                    strRetVal = "Value of " + GetColumnLabelOrName(strColumnName, CompPayeeColumn, strColumnName) + " is required";
                }
                else
                {
                    int? iDropdownID = Convert.ToInt32(CompPayeeColumn.Where(p => p.ColumnName.Equals(strColumnName)).FirstOrDefault().LcscDropDownId);
                    if (iDropdownID > 0)
                    {
                        var dropdownvalues = DRCV.GetByDropDownId(Convert.ToInt32(iDropdownID));
                        foreach (var ddvalue in dropdownvalues)
                        {
                            if (ddvalue.LdvValue.ToString().ToLower() == strColumnValue.ToLower()) blnValueMatched = true;
                        }
                        if (!blnValueMatched)
                        {
                            //strRetVal = "Invalid value for " + CompPayeeColumn.Where(p => p.ColumnName.Equals(strColumnName)).FirstOrDefault().LcscLabel ?? strColumnName;
                            strRetVal = "Invalid value for " + GetColumnLabelOrName(strColumnName, CompPayeeColumn, strColumnName);
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(strColumnValue))
                {
                    int? iDropdownID = Convert.ToInt32(CompPayeeColumn.Where(p => p.ColumnName.Equals(strColumnName)).FirstOrDefault().LcscDropDownId);
                    if (iDropdownID > 0)
                    {
                        var dropdownvalues = DRCV.GetByDropDownId(Convert.ToInt32(iDropdownID));
                        foreach (var ddvalue in dropdownvalues)
                        {
                            if (ddvalue.LdvValue.ToString().ToLower() == strColumnValue.ToLower()) blnValueMatched = true;
                        }
                        if (!blnValueMatched)
                        {
                            //strRetVal = "Invalid value for " + CompPayeeColumn.Where(p => p.ColumnName.Equals(strColumnName)).FirstOrDefault().LcscLabel ?? strColumnName;
                            strRetVal = "Invalid value for " + GetColumnLabelOrName(strColumnName, CompPayeeColumn, strColumnName);
                        }
                    }
                }
            }
            //if (CompPayeeColumn.Where(p => p.ColumnName.Equals(strColumnName)).Where(p => p.LscsIsDropDownAllowed.Equals(true)).Count() > 0)
            //{
                
            //}
            return strRetVal;
        }
        private string GetColumnLabelOrName(string strcolumnName, IEnumerable<LCompanySpecificColumnViewModel> CompPayeeColumn, string strUserFriendlyName)
        {
            
            string strLable = strUserFriendlyName;
            if(CompPayeeColumn.Where(p=> p.ColumnName.Equals(strcolumnName)).Count() >0)
            {
                strLable = Convert.ToString(CompPayeeColumn.Where(p => p.ColumnName.Equals(strcolumnName)).FirstOrDefault().LcscLabel);
                if (strLable == null || strLable == "")
                {
                    strLable = strUserFriendlyName;
                }
            }
            return strLable;
        }

        //[ControllerActionFilter]
        public JsonResult GetFinOpsRolesList()
        {
            //Only Adding ChannelManager as per requirement
            IAspnetRolesRestClient AURC = new AspnetRolesRestClient();
            var FinOpsRoles = AURC.GetFinOpsByCompanyCode(CompanyCode);
            return Json(FinOpsRoles,JsonRequestBehavior.AllowGet);
        }

        //Get counts for the Parent Payee Grid
        public JsonResult GetParentPayeeGridCounts()
        {
            var ApiCounts = RestClient.GetParentPayeeGridCounts(CompanyId);
            return Json(ApiCounts, JsonRequestBehavior.AllowGet);
        }

        //Get data for Parent Payee Grid
        public JsonResult GetParentPayeeGrid(string sortdatafield, string sortorder, int pagesize, int pagenum,int? ParentPayeeId)
        {
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetParentPayeeData(CompanyId,sortdatafield,sortorder,pagesize,pagenum,FilterQuery, ParentPayeeId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //Get counts for the  Payee Grid by portfolio grid
        public JsonResult GetPayeeGridByPortfolioMatchingCounts(string PortfolioList)
        {
            if (PortfolioList == null)
                PortfolioList = string.Empty;
            var ApiCounts = RestClient.GetPayeeCountsForPortfolioMatching(CompanyId,UserId,PortfolioList,UserRole);
            return Json(ApiCounts, JsonRequestBehavior.AllowGet);
        }

        //Get data for  Payee Grid
        public JsonResult GetPayeeByPortfolioMatchingGrid(string PortfolioList,string sortdatafield, string sortorder, int pagesize, int pagenum,string PayeeId)
        {
            if (PortfolioList == null)
                PortfolioList = string.Empty;
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            IEnumerable<LPayeeViewModel> ApiData = RestClient.GetPayeeForClaimsDropdown(CompanyId,UserId,PortfolioList, sortdatafield, sortorder, pagesize, pagenum, FilterQuery, UserRole,PayeeId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPayeeByPortfolioMatchingGridCommon(string PortfolioList, string sortdatafield, string sortorder, int pagesize, int pagenum, string PayeeId)
        {
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = "";
            List<LPayeeAddViewModel> objList = new List<LPayeeAddViewModel>();
            IEnumerable<LPayeeViewModel> ApiData = RestClient.GetPayeeForClaimsDropdown(CompanyId, UserId, PortfolioList, sortdatafield, sortorder, pagesize, pagenum, FilterQuery, UserRole, PayeeId);
            foreach(LPayeeViewModel row in ApiData)
            {
                LPayeeAddViewModel obj = new LPayeeAddViewModel();
                obj.Id = row.Id;
                obj.LpFirstName = row.LpFirstName;
                obj.LpLastName = row.LpLastName;
                obj.LpPayeeCode = row.LpPayeeCode;
                objList.Add(obj);


            }
            return Json(objList, JsonRequestBehavior.AllowGet);
        }

        //Get counts for the  Payee Grid by portfolio grid
        public JsonResult GetParentsByParentId(int PayeeId)
        {
            var ApiDatas = RestClient.GetParentsByPayeeId(PayeeId);
            return Json(ApiDatas, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> UploadAutoAttachment(string id,string PayeeCode, string Type)
        {
            AttachedFilesViewModel FileDetails = new AttachedFilesViewModel();
            GenericGridRestClient RestClient = new GenericGridRestClient();

            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    HttpPostedFileBase hpf = fileContent;
                    AttachedFilesViewModel PayeeFiles = AttachPayeeFilesOneByOne(hpf, PayeeCode);
                    
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

                string data = RestClient.UpdateAttachment(Convert.ToInt32(id), UserId, FileDetails.FileName, FileDetails.FilePath, Type);
                return Json(data, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            //   return Json("File uploaded successfully");
        }

        public AttachedFilesViewModel AttachPayeeFilesOneByOne(HttpPostedFileBase File1, string PayeeCode)
        {
            var UserName = System.Web.HttpContext.Current.Session["UserName"];
            var companyName = System.Web.HttpContext.Current.Session["CompanyName"].ToString();
            
                
            var fileLocation = "";
            string fileExtension = System.IO.Path.GetExtension(File1.FileName);
            string name = System.IO.Path.GetFileNameWithoutExtension(File1.FileName);
            string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;

            var filePath = "";
            filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["AttachedPayeeDocumentPath"], CompanyCode + "/Payees/" + PayeeCode + "/SupportingDocuments");
            fileLocation = filePath + "/" + FileNames;
            //check if directory exists or not. iIf notcreate that directory
            bool exists = System.IO.Directory.Exists(filePath);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(filePath);
            }
            File1.SaveAs(fileLocation);

            return new AttachedFilesViewModel { FileName = FileNames, FilePath = filePath };
        }

    }

}
