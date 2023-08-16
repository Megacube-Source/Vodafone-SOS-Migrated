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
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LRefFileTypesController : PrimaryController
    {
        
        ILRefFileTypesRestClient RestClient = new LRefFileTypeRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string UserRole = System.Web.HttpContext.Current.Session["UserRole"].ToString();
        string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
        // GET: LRefFileTypes

        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Ref File Type";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Index(object[] model)
        {
            try
            {
                var objects = JsonConvert.DeserializeObject<List<object>>(model[0] as string);
                var result = objects.Select(obj => JsonConvert.SerializeObject(obj)).ToArray();
                //JavaScriptSerializer js = new JavaScriptSerializer();
                //string[] DataArray = js.Deserialize<string[]>(result[0]);
                //This method passes the comma seperated list of grid items to api for addition/updation
                var Filetype = new LRefFileTypeViewModel();
                Filetype.LrftCompanyId = CompanyId;
                Filetype.LrftCreatedById = System.Web.HttpContext.Current.Session["UserId"].ToString();
                Filetype.LrftCreatedDateTime = DateTime.UtcNow;
                Filetype.ModelData = result;
                RestClient.AddgRIDdATA(Filetype,CompanyId,LoggedInUserId);
                TempData["Message"] = "Reference File Type sucessfully added";
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {

                TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                return RedirectToAction("Index");
            }
        }

        // GET: RRefFileType/Create
        //[ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Ref File Type";
            return View();
        }

        // POST: RRefFileType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       // [ControllerActionFilter]
        public ActionResult Create(LRefFileTypeViewModel LRFT,string PortfolioList)
        {
            try
            {
                LRFT.LrftUpdatedById = LoggedInUserId;
                LRFT.LrftUpdatedDateTime = DateTime.UtcNow;
                LRFT.LrftCreatedById = LoggedInUserId;
                LRFT.LrftCreatedDateTime = DateTime.UtcNow;
                LRFT.LrftCompanyId = CompanyId;
                RestClient.Add(LRFT,PortfolioList,LoggedInRoleId,LoggedInUserName,Workflow);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(LRFT);
            }
        }

        // GET: RRefFileType/Edit/5
        //[ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            LRefFileTypeViewModel rDeviceTypeViewModel = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Edit RefFile Type";
            if (rDeviceTypeViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rDeviceTypeViewModel);
        }

        // POST: RRefFileType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[ControllerActionFilter]
        public ActionResult Edit(LRefFileTypeViewModel LRFT,string PortfolioList)
        {
            try
            {
                LRFT.LrftUpdatedById = LoggedInUserId;
                LRFT.LrftUpdatedDateTime = DateTime.UtcNow;
                LRFT.LrftCompanyId = CompanyId;
                RestClient.Update(LRFT,PortfolioList,LoggedInRoleId,LoggedInUserName,Workflow);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];
                return View(LRFT);
            }
        }

        //method to get list of lreffiletypes to load in grid
        [ControllerActionFilter]
        public JsonResult GetRefFileTypes()
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId).ToList();
            //var LastRow = new LRefFileTypeViewModel { LrftCompanyId = CompanyId, LrftName = "", LrftDescription = "",Id=0 };
            //ApiData.Add(LastRow);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //RS method to download referencefiles on the basis of companyid (13/03/2019)
        public ActionResult DownloadRefFileTypes()
        {
            string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
            string UserEmail = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);

            var FileName = RestClient.DownloadRefFileTypes(CompanyId);//filename returning from the stored proc rest data will be get generated in API only

            var FilesPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/" + FileName;
            //NOTE:-Refreshing Directory so that web server can see the file otherwise it gives a no file found message
            Thread.Sleep(10000);
            var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/" + FileName;
            DirectoryInfo dir = new DirectoryInfo(FilePath);
            dir.Refresh();
            if (System.IO.File.Exists(FilesPath))
            {
                return File(FilesPath, "application/octet-stream", FileName);
            }
            else
            {
                TempData["Error"] = "No Data Found";
            }

            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }
        [ControllerActionFilter]
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
    }
}