//Code Review for this file (from security perspective) done

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LSchemeOwnerController : PrimaryController
    {
        
        ILSchemeOwnerRestClient RestClient = new LSchemeOwnerRestClient();
        string LoggedRoleId = System.Web.HttpContext.Current.Session["UserRoleId"] as string;
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string UserRole = System.Web.HttpContext.Current.Session["UserRole"].ToString();
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
        // GET: LSchemeOwner

        //[ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Scheme Owner";
            return View();
        }

          
        //[ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Scheme Owner";
            ViewBag.OwnerId = GetOwner(null);
            return View();
        }
        private SelectList GetOwner(string Selected)
        {
            string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //this code has to be updated to get the data from API.
            var ApiData = RestClient.GetActiveReportingAnalysts( CompanyId);
            var x = new SelectList(ApiData, "LuUserId", "LuEmail", Selected);
            return x;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(LSchemeOwnerViewModel model )
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                    model.UpdatedById = LoggedInUserId;
                    model.UpdatedDateTime = DateTime.UtcNow;
                    model.CreatedById = LoggedInUserId;
                    model.CreatedDateTime = DateTime.UtcNow;
                    model.CompanyId = CompanyId;
                    RestClient.Add(model, LoggedInUserName, Workflow);
                    return RedirectToAction("Index");
                //}
                //else
                //{
                //    ViewBag.OwnerId = GetOwner(null);
                //    return View(model);
                //}
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(model);
            }
        }

        // GET: RSchemeOwner/Edit/5
        //[ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            LSchemeOwnerViewModel model = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Edit Scheme Owner";
            ViewBag.OwnerId = GetOwner(model.OwnerId);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: RSchemeOwner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[ControllerActionFilter]
        public ActionResult Edit(LSchemeOwnerViewModel model, string PortfolioList)
        {
            try
            {
                model.UpdatedById = LoggedInUserId;
                model.UpdatedDateTime = DateTime.UtcNow;
                model.CompanyId = CompanyId;
                RestClient.Update(model,LoggedInUserName,Workflow);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];
                return View(model);
            }
        }

        //method to get list of LSchemeOwner to load in grid
        //[ControllerActionFilter]
        public JsonResult GetSchemeOwners()
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId).ToList(); 
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //RS method to download referencefiles on the basis of companyid (13/03/2019)
        //public ActionResult DownloadSchemeOwners()
        //{
        //    string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        //    string UserEmail = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);

        //    var FileName = RestClient.DownloadSchemeOwners(CompanyId);//filename returning from the stored proc rest data will be get generated in API only

        //    var FilesPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/" + FileName;
        //    //NOTE:-Refreshing Directory so that web server can see the file otherwise it gives a no file found message
        //    Thread.Sleep(10000);
        //    var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/" + FileName;
        //    DirectoryInfo dir = new DirectoryInfo(FilePath);
        //    dir.Refresh();
        //    if (System.IO.File.Exists(FilesPath))
        //    {
        //        return File(FilesPath, "application/octet-stream", FileName);
        //    }
        //    else
        //    {
        //        TempData["Error"] = "No Data Found";
        //    }

        //    return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        //}
        //[ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            try
            {
                RestClient.Delete(id,LoggedInUserName,Workflow);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                return RedirectToAction("Index");
            }
        }

        public ActionResult DownloadSchemeOwners()
        {
            LSchemeOwnerRestClient ILu = new LSchemeOwnerRestClient();
            var FileName = ILu.DownloadSchemeOwners(LoggedInUserId, CompanyId, CompanyCode, LoggedInUserName, Convert.ToInt32(LoggedRoleId));
            Thread.Sleep(2000);
            DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            di.Refresh();
            var ByteData1 = Globals.DownloadFromS3("SchemeOwners.xlsx", CompanyCode + "/" + LoggedInUserName + "/");
            if (ByteData1 != null)//now try downloading from sos bucket
            {
                return File(ByteData1, "application/unknown", "SchemeOwners.xlsx");
            }
            else
            {
                TempData["Error"] = "No File found";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }

        }
    }
}