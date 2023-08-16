//Code Review for this file (from security perspective) done

using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class RChannelsController : PrimaryController
    {
        IRChannelsRestClient RestClient = new RChannelsRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);

        // GET: RChannels
        [ControllerActionFilter]
        public JsonResult GetChannels()//string sortdatafield, string sortorder, int pagesize, int pagenum
        {
            var ApiData = RestClient.GetByCompanyId(CompanyId);
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Channels";
            return View();
        }

        // GET: RChannels/Create
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Channel";
            return View(new RChannelViewModel());
        }

        // POST: RChannels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create( RChannelViewModel RCVM)
        {
            try{
                RCVM.RcCompanyId = CompanyId;
               RestClient.Add(RCVM);
               return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(RCVM);
            }
        }

        // GET: RChannels/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Edit Channel";
            RChannelViewModel rChannelViewModel = RestClient.GetById(id);
            if (rChannelViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rChannelViewModel);
        }

        // POST: RChannels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit( RChannelViewModel rChannelViewModel)
        {
            try
            {
                 rChannelViewModel.RcCompanyId = CompanyId;
                    var result =  RestClient.Update(rChannelViewModel);
                if (result != null)
                {
                    Globals.ExportFromDataTable(CompanyCode, LoggedInUserName, "ChannelError", (DataTable)result);
                    var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/ChannelError.xlsx";

                    var FilePathDown =  CompanyCode + "/" + LoggedInUserName + "/";

                    DirectoryInfo dir = new DirectoryInfo(FilePath);
                    dir.Refresh();
                    if (System.IO.File.Exists(FilePath))
                    {
                        TempData["Message"] = "This channel can not be inactivated since there are transactions belonging to this channel. See downloaded file for channel dependancy";
                        TempData["ErrorFilePath"] = FilePath;
                        //return File(FilePath, "application/octet-stream", "ClaimsUploadError.xlsx");//application/vnd.openxmlformats-officedocument.spreadsheetml.shee)t
                        return View(rChannelViewModel);
                        //var ByteData = Globals.DownloadFromS3("ChannelError.xlsx", FilePathDown);
                        //return File(ByteData, "application/unknown", "ChannelError.xlsx");
                        //return File("ChannelError.xlsx", Globals.GetFileContentType("xlsx"), "ChannelError");
                    }
                }
                    return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(rChannelViewModel);
            }
        }

        public ActionResult DownloadErrorFile()
        {
            var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/ChannelError.xlsx";
            DirectoryInfo dir = new DirectoryInfo(FilePath);
            dir.Refresh();
            if (System.IO.File.Exists(FilePath))
            {
                return File(FilePath, "application/octet-stream", "ChannelError.xlsx");//application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
            }
            else
            {
                return null;
            }
        }

        // GET: RChannels/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Delete Channel";
            RChannelViewModel rChannelViewModel = RestClient.GetById(id);
            if (rChannelViewModel == null)
            {
                return HttpNotFound();
            }
            return View(rChannelViewModel);
        }

        // POST: RChannels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(RChannelViewModel RCVM)
        {
            try
            {
                RestClient.Delete(RCVM.Id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(RCVM);
            }
        }
    }
}
