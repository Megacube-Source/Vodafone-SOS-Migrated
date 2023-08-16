using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class LSupportingDocumentsController : Controller
    {
       // lsupp RestClient = new LSupportingDocumentsController();
        LSupportingDocumentsRestClient RestClient = new LSupportingDocumentsRestClient();
        string UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
        // GET: LSupportingDocuments
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> UploadHomeReport(string id, string entitytype)
        {
            AttachedFilesViewModel FileDetails = new AttachedFilesViewModel();

            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    HttpPostedFileBase hpf = fileContent;
                    AttachedFilesViewModel FileDataDetails = AttachEntityFiles(hpf);
                    if (string.IsNullOrEmpty(FileDetails.FileName))
                    {
                        FileDetails.FileName = FileDataDetails.FileName;
                    }
                    else
                    {
                        FileDetails.FileName = FileDetails.FileName + "," + FileDataDetails.FileName;
                    }

                    FileDetails.FilePath = FileDataDetails.FilePath;
                }

                RestClient.UpdateAttachment(Convert.ToInt32(id), UserId, FileDetails.FileName, FileDetails.FilePath, entitytype);
                return Json("Success", JsonRequestBehavior.AllowGet);
                //if (fileContent != null && fileContent.ContentLength > 0)
                //{
                //    // get a stream
                //    var stream = fileContent.InputStream;
                //    // and optionally write the file to disk
                //    var fileName = Path.GetFileName(file);
                //    var path = Path.Combine(Server.MapPath("~/App_Data/Images"), fileName);
                //    using (var fileStream = File.Create(path))
                //    {
                //        stream.CopyTo(fileStream);
                //    }
                //}
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            //   return Json("File uploaded successfully");
        }

        public AttachedFilesViewModel AttachEntityFiles(HttpPostedFileBase file)
        {
            var UserName = System.Web.HttpContext.Current.Session["UserName"];
            string FileName = "";
            var CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //SS using same Document path as AttachedClaimDocumentPath is mapped with S Drive
            string filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["AttachedClaimDocumentPath"], System.Web.HttpContext.Current.Session["CompanyCode"] + "/SupportTickets/SupportingDocuments");



            var fileLocation = "";
            string fileExtension = System.IO.Path.GetExtension(file.FileName);
            string name = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;


            fileLocation = filePath + "/" + FileNames;
            //check if directory exists or not. iIf notcreate that directory
            bool exists = System.IO.Directory.Exists(filePath);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(filePath);
            }
            file.SaveAs(fileLocation);


            ////made a comma seperated list of  file names
            //if (string.IsNullOrEmpty(FileName))
            //{
            //    FileName = FileNames;
            //}
            //else
            //{
            //    FileName = FileName + "," + FileNames;
            //}


            return new AttachedFilesViewModel { FileName = name, FilePath = filePath };
        }
    }
}