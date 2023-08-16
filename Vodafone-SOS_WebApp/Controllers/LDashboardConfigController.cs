using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [HandleCustomError]
    [SessionExpire]
    public class LDashboardConfigController : PrimaryController
    {
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string strRoleName = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
        int iCompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        int iUserID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LUserId"]);
        int iUserRoleID = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string CurrentUserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        string UserEmail = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        //string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);

        ILDashboardConfigRestClient RestClient = new LDashboardConfigRestClient();
        // GET: DashboardConfig
        private SelectList GetKpiTypes()
        {
            var ApiData = RestClient.GetKpiData("0", "Type", CompanyCode, LoggedInUserId, LoggedInRoleId, "Period", "Dim", "ports", "payees", "");
            var x = new SelectList(ApiData, "KpiTypeId", "KpiTypeName");
            return x;
        }
        private SelectList GetKpiGroups(int iTypeId)
        {
            var ApiData = RestClient.GetKpiData(iTypeId.ToString(), "Group", CompanyCode, LoggedInUserId, LoggedInRoleId, "Period", "Dim", "ports", "payees", "");
            var x = new SelectList(ApiData, "KpiGroupId", "KpiGroupName");
            return x;
        }
        private SelectList GetKpiDropdown(int iGroupId)
        {
            var ApiData = RestClient.GetKpiData(iGroupId.ToString(), "Kpi", CompanyCode, LoggedInUserId, LoggedInRoleId, "Period", "Dim", "ports", "payees", "");
            var x = new SelectList(ApiData, "KpiId", "KpiName");
            return x;
        }
        public JsonResult GetKpiGroups(string TypeId)
        {
            var ApiData = RestClient.GetKpiData(TypeId, "Group", CompanyCode, LoggedInUserId, LoggedInRoleId, "Period", "Dim", "ports", "payees", "");
            //var x = new SelectList(ApiData, "KpiGroupID", "KpiGroupName");
            //return x;
            var x = new SelectList(ApiData, "KpiGroupID", "KpiGroupName");
            return Json(x, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetKpis(string GroupIds)
        {
            var ApiData = RestClient.GetKpiData(GroupIds, "Kpi", CompanyCode, LoggedInUserId, LoggedInRoleId, "Period", "Dim", "ports", "payees", "");
            //var x = new SelectList(ApiData, "KpiId", "KpiName");
            //return Json(x, JsonRequestBehavior.AllowGet);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPortfolioForDropDown()
        {
            var RoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
            ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
            var ApiData = LPORC.GetByUserId(LoggedInUserId, RoleId).Select(p => new { p.Id, Portfolio = p.RcName + " (" + p.RcPrimaryChannel + ")" }).GroupBy(p => p.Portfolio).Select(p => p.FirstOrDefault());
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetPayeeList(string PortfolioList)
        //{
        //    if (PortfolioList == null)
        //        PortfolioList = string.Empty;
        //    var ApiData = Globals.GetPayeeList(PortfolioList, false);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}

        //This loads payee in Dropdown//
        public JsonResult GetPayeeByPortfolioMatchingGrid(string PortfolioList)
        {
            if (PortfolioList == null)
                PortfolioList = string.Empty;
            var qry = Request.QueryString;
            //string UserId = null;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            //var FilterQuery = Globals.BuildQuery(qry);
            //var ApiData = RestClient.GetPayeeForDropdown(iCompanyId, iUserID.ToString(), PortfolioList, "sortdatafield", "sortorder", 4000, 1, "");
            ILPayeesRestClient LPRC = new LPayeesRestClient();
            var UserRole = System.Web.HttpContext.Current.Session["UserRole"] as String;
            IEnumerable<LPayeeViewModel> ApiData = LPRC.GetPayeeForClaimsDropdown(iCompanyId, LoggedInUserId, PortfolioList, string.Empty, string.Empty, 4000, 0, string.Empty, UserRole, string.Empty);
            var Result = ApiData.Select(p => new { p.Id, FullName = p.LpFirstName + " " + p.LpLastName + " (" + p.LpPayeeCode + ")" });
            //return Json(ApiData, JsonRequestBehavior.AllowGet);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        private SelectList BlankList()
        {
            var NoData = "";
            var x = new SelectList(NoData, "", "");
            return x;
        }
        public ActionResult Index()
        {
            ViewBag.KpiTypeID = GetKpiTypes();
            ViewBag.KpiGroupID = BlankList();
            ViewBag.KpiId = BlankList();
            ViewBag.ChannelIds = GetKpiTypes();
            ViewBag.PayeeCodes = BlankList();
            ViewBag.GraphType = GetKpiTypes();
            ViewBag.ConfigType = "Dashboard";
            return View();
        }
        public ActionResult AddGraph()
        {
            ViewBag.KpiTypeID = GetKpiTypes();
            ViewBag.KpiGroupID = BlankList();
            ViewBag.KpiId = BlankList();
            ViewBag.ChannelIds = GetKpiTypes();
            ViewBag.PayeeCodes = BlankList();
            ViewBag.GraphType = GetKpiTypes();
            ViewBag.ConfigType = "Dashboard";
            //else ViewBag.ConfigType = "Tile";
            return View();
        }
        public ActionResult ConfigureTile(int id)
        {
            LDashboardConfigViewModel Model = RestClient.GetConfigurationById(id, CurrentUserId, iUserRoleID.ToString(), iCompanyId);
            ViewBag.KpiTypeId = GetKpiTypes();
            ViewBag.KpiGroupId = GetKpiGroups(Model.KpiTypeId);
            ViewBag.KpiIds = GetKpiDropdown(Model.KpiGroupId);
            ViewBag.PayeeCodes = BlankList();
            ViewBag.GraphType = GetKpiTypes();
            //ViewBag.KpiList = Model.KpiIds;
            ViewBag.PortfolioList = Model.PortfolioIds;
            ViewBag.PayeeList = Model.PayeeCodes;
            return View(Model);

        }
        public ActionResult ConfigureGraph(int? id)
        {
            if (id == null) id = 0;
            LDashboardConfigViewModel Model = RestClient.GetConfigurationById(Convert.ToInt32(id), CurrentUserId, iUserRoleID.ToString(), iCompanyId);
            ViewBag.KpiTypeId = GetKpiTypes();
            ViewBag.KpiGroupId = GetKpiGroups(Model.KpiTypeId);
            ViewBag.PayeeCodes = BlankList();
            ViewBag.KpiList = Model.KpiIds;
            ViewBag.PortfolioList = Model.PortfolioIds;
            ViewBag.Dimension = Model.Dimension;
            ViewBag.PayeeList = Model.PayeeCodes;
            return View(Model);
        }
        public ActionResult AddTile()
        {
            ViewBag.KpiTypeID = GetKpiTypes();
            ViewBag.KpiGroupID = BlankList();
            ViewBag.KpiId = BlankList();
            ViewBag.ChannelIds = GetKpiTypes();
            ViewBag.PayeeCodes = BlankList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGraph(LDashboardConfigViewModel model, string KpiList, string PortfolioList, string PayeeList, string Dimension)
        {
            model.UserId = CurrentUserId;
            model.RoleId = iUserRoleID.ToString();
            model.KpiIds = KpiList;
            model.PortfolioIds = PortfolioList;
            model.PayeeCodes = PayeeList;
            model.TileOrdinal = 1;
            model.CompanyId = iCompanyId;
            model.Dimension = Dimension;
            model.IsGraph = true;
            RestClient.SaveConfigurationSetting(model);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigureGraph(LDashboardConfigViewModel model, string KpiList, string PortfolioList, string PayeeList, string Dimension)
        {
            model.UserId = CurrentUserId;
            model.RoleId = iUserRoleID.ToString();
            model.KpiIds = KpiList;
            model.PortfolioIds = PortfolioList;
            model.PayeeCodes = PayeeList;
            model.Dimension = Dimension;
            //model.TileOrdinal = 1;
            model.CompanyId = iCompanyId;
            //model.GraphType = "";
            model.IsGraph = true;
            RestClient.SaveConfigurationSetting(model);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfigureTile(LDashboardConfigViewModel model, string PortfolioList, string PayeeList)
        {
            model.UserId = CurrentUserId;
            model.RoleId = iUserRoleID.ToString();
            //model.KpiIds = KpiList;
            model.PortfolioIds = PortfolioList;
            model.PayeeCodes = PayeeList;
            model.Dimension = "";
            //model.TileOrdinal = 1;
            model.CompanyId = iCompanyId;
            model.GraphType = "";
            model.IsGraph = false;
            RestClient.SaveConfigurationSetting(model);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTile(LDashboardConfigViewModel model, string KpiList, string PortfolioList, string PayeeList)
        {
            model.UserId = CurrentUserId;
            model.RoleId = iUserRoleID.ToString();
            model.KpiIds = KpiList;
            model.PortfolioIds = PortfolioList;
            Console.WriteLine(PortfolioList);
            model.PayeeCodes = PayeeList;
            model.Dimension = "";
            model.TileOrdinal = 1;
            model.CompanyId = iCompanyId;
            model.GraphType = "";
            model.IsGraph = false;
            RestClient.SaveConfigurationSetting(model);
            return RedirectToAction("Index");
        }
        public ActionResult DeleteConfiguration(int ID)
        {
            RestClient.DeleteConfiguration(ID);
            ViewBag.Message = "Tile Deleted!";
            return RedirectToAction("Index");
        }
        public JsonResult GetTilesList()
        {
            var ApiData = RestClient.GetKpiData("0", "TileList", CompanyCode, LoggedInUserId, LoggedInRoleId, "Period", "Dim", "ports", "payees", "");
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGraphDetails(int? Id)
        {
            if (Id == null) Id = 0;
            //try
            //{
            //    if (Id < 0) Id = 0;
            //}
            //catch (Exception)
            //{
            //    Id = 0;
            //}
            var ApiData = RestClient.GetConfigurationById(Convert.ToInt32(Id), CurrentUserId, iUserRoleID.ToString(), iCompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGraphList()
        {
            var ApiData = RestClient.GetKpiData("0", "GraphList", CompanyCode, LoggedInUserId, LoggedInRoleId, "Period", "Dim", "ports", "payees", "");
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTilesData(string strPeriods, string BatchStatus)
        {
            var ApiData = RestClient.GetKpiData("0", "TileData", CompanyCode, LoggedInUserId, LoggedInRoleId, strPeriods, "Dim", "ports", "payees", BatchStatus);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGraphData(int? GraphId, string strPeriods, string BatchStatus)
        {
            if (GraphId == null) GraphId = 0;
            //try
            //{
            //    if (GraphId < 0) GraphId = 0;
            //}
            //catch (Exception)
            //{
            //    GraphId = 0;

            //}
            var ApiData = RestClient.GetKpiData(GraphId.ToString(), "GraphData", CompanyCode, LoggedInUserId, LoggedInRoleId, strPeriods, "", "", "", BatchStatus);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGraphDataByChannelAndPayee(int? GraphId, string strPeriods, string strDimension, string strChannels, string strPayeeIds, string BatchStatus)
        {
            if (GraphId == null) GraphId = 0;
            //try
            //{
            //    if (GraphId < 0) GraphId = 0;
            //}
            //catch (Exception)
            //{
            //    GraphId = 0;
            //}
            var ApiData = RestClient.GetKpiData(GraphId.ToString(), "GraphData", CompanyCode, LoggedInUserId, LoggedInRoleId, strPeriods, strDimension, strChannels, strPayeeIds, BatchStatus);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGraphDataForModel(int? GraphId, string strPeriods, string strDimension, string strPortfolios, string strPayees, string BatchStatus)
        {
            if (GraphId == null) GraphId = 0;
            //try
            //{
            //    if (GraphId < 0) GraphId = 0;
            //}
            //catch (Exception)
            //{
            //    GraphId = 0;
            //}
            var ApiData = RestClient.GetKpiData(GraphId.ToString(), "GraphData", CompanyCode, LoggedInUserId, LoggedInRoleId, strPeriods, strDimension, strPortfolios, strPayees, BatchStatus);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGraphKpiNames(int GraphId)
        {
            var ApiData = RestClient.GetKpiData(GraphId.ToString(), "KpiNames", CompanyCode, LoggedInUserId, LoggedInRoleId, "", "", "", "", "");
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCommissionPeriod()
        {
            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
            var ApiData = CPRC.GetByCompanyId(iCompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCurrentPeriod()
        {
            var ApiData = RestClient.GetKpiData("", "Period", CompanyCode, LoggedInUserId, LoggedInRoleId, "Period", "Dim", "ports", "payees", "");
            return Json(ApiData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetMyActions()
        {
            ReportsRestClient RRC = new ReportsRestClient();
            var data = RRC.GetXReportsTreeStructure("A", UserEmail, strRoleName, CompanyCode, "My Actions", "", "", "", 0, 0);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadGraphData(int? GraphId, string strPeriods, string BatchStatus)
        {
            if (GraphId == null) GraphId = 0;
            var FileName  = RestClient.DownloadGraphData(GraphId.ToString(), "GraphData", CompanyCode, LoggedInUserId, LoggedInRoleId, strPeriods, "", "", "", BatchStatus);




           

            //var response = _client.Execute(request);
            //string source = response.Content;
            //dynamic data = JsonConvert.DeserializeObject(source);
            //return data;

            var FilesPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/" + FileName;
            //NOTE:-Refreshing Directory so that web server can see the file otherwise it gives a no file found message
            Thread.Sleep(10000);
            var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/" + FileName;
            DirectoryInfo dir = new DirectoryInfo(FilePath);
            dir.Refresh();
            if (System.IO.File.Exists(FilesPath))
            {
                return File(FilesPath, "application/octet-stream", FileName);//application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
            }
            else
            {
                TempData["Error"] = "No Data Found";
            }

            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }
           
    }


   
}


