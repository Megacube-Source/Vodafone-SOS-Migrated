//Code Review for this file (from security perspective) done

using System.Configuration;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class UserGuidesController : PrimaryController
    {
        // GET: UserGuides
        [ControllerActionFilter]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReleaseNotes()
        {
            Helper.UserGuidesRestClient URC = new Helper.UserGuidesRestClient();
            
            string ReleaseNotesHTML = URC.GetReleaseNotes();
            ViewBag.ReleaseNotesHTML = ReleaseNotesHTML;
            System.Web.HttpContext.Current.Session["Title"] = " Manage Release Notes";
            return View();
        }
        //
        //This method will download the UserGuide as per the Guide Name passed in parameter
        [ControllerActionFilter]
        public ActionResult DownloadUserGuide(string GuideName)
        {
            var UserGuideRelativeFilePath = ConfigurationManager.AppSettings["SOSBucketRootFolder"]+ ConfigurationManager.AppSettings["UserGuidesFilePath"] + "/" ;
            var FileDatainBytes = Globals.DownloadFromS3(GuideName + ".pdf",UserGuideRelativeFilePath);
            if(FileDatainBytes!=null)
            {
                return File(FileDatainBytes,"application/pdf", GuideName + ".pdf");
            }
            TempData["Error"] = "No File found";
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }
        //
        // This Method will download the Realese Notes
        //[ControllerActionFilter]
        public ActionResult DownloadReleaseNotes(string ReleaseNotesName)
        {
            //var ReleaseNotesRelativeFilePath = ConfigurationManager.AppSettings["ReleseNotesFilePath"] + "/" + ReleaseNotesName + ".pdf";
            //var FileDatainBytes = Globals.DownloadFromS3(ReleaseNotesName + ".pdf", ReleaseNotesRelativeFilePath);
            //if (FileDatainBytes != null)
            //{
            //    return File(ReleaseNotesRelativeFilePath, "application/pdf", ReleaseNotesName + ".pdf");
            //}
            //TempData["Error"] = "No File found";
            //return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            var ReleaseNotesRelativeFilePath = "S:\\ReleaseNotes\\" + ReleaseNotesName + ".pdf";
            System.IO.FileInfo fi = new System.IO.FileInfo(ReleaseNotesRelativeFilePath);
            if(fi.Exists)
            {
                return File(ReleaseNotesRelativeFilePath, "application/pdf", ReleaseNotesName + ".pdf");
            }
            else
            {
                TempData["Error"] = "No File Found";
                return Redirect(System.Web.HttpContext.Current.Session["From"] as string);
            }
        }
    }
}