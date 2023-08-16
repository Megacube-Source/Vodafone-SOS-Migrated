//Code Review for this file (from security perspective) done

using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;
using static Vodafone_SOS_WebApp.Utilities.Globals;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LUploadedFilesController : Controller//PrimaryController//RK R2.3 17112018 made this change (comment URL Tempring) so that review can open in new tab
    {
        ILRefFileTypesRestClient LFTRC = new LRefFileTypeRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        IGenericGridRestClient GGRC = new GenericGridRestClient();
        string UserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        IRStatusesRestClient RSRC = new RStatusesRestClient();
        ILPayeesRestClient LPRC = new LPayeesRestClient();
        ILRefFilesRestClient RestClient = new LRefFilesRestClient();
        
        private SelectList GetRefFileType()
        {
            var ApiData = LFTRC.GetByPortfolioMatching(CompanyId, UserId, LoggedInRoleId);
            var x = new SelectList(ApiData, "Id", "LrftName");
            return x;
        }
        private SelectList GetRefFileType(int id)
        {
            var ApiData = LFTRC.GetByPortfolioMatching(CompanyId,UserId,LoggedInRoleId);
            var x = new SelectList(ApiData, "Id", "LrftName",id);
            return x;
        }
        
        private SelectList GetPayees()
        {
            ILPayeesRestClient LPRC = new LPayeesRestClient();
            var ActivePayees = LPRC.GetActivePayee(CompanyId).Select(p => new {FullName=p.LpFirstName+" "+p.LpLastName+" ("+p.LpPayeeCode+")",p.Id });
            var x = new SelectList(ActivePayees,"Id","FullName");
            return x;
        }
        
        private SelectList GetYear()
        {
            //
            var MyList = Enumerable.Range((DateTime.UtcNow.AddYears(-2).Year), 2100 - DateTime.UtcNow.AddYears(1).Year);
            //var MyList = Enumerable.Range((DateTime.Parse("01/01/2016").Year), 2100 - DateTime.UtcNow.AddYears(1).Year);
            var x = new SelectList(MyList,MyList);
            return x;
        }
        
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Uploaded Files";
            //ViewBag.UploadType = UploadType;
            //if (UploadType == "Reference")
            //{
            //    ViewBag.LufRefFileTypeId = GetRefFileType();
            //}
            //if (UploadType == "PayeeDocument")
            //{
            //    ViewBag.LufPayeeId = GetPayees();
            //}
            //create dropdown for year data
            ViewBag.LrfRefFileTypeId = GetRefFileType();
            ViewBag.LrfYear = GetYear();
            var model = new LRefFileViewModel();
            model.LrfMonth = DateTime.UtcNow.Month.ToString("D2");
            model.LrfYear = DateTime.UtcNow.Year;
            return View(model);
        }

       [HttpPost]
     //  [ValidateAntiForgeryToken]//RK added while code review
        [ControllerActionFilter]
        public JsonResult UploadReFileData(LRefFileViewModel LRFVM,HttpPostedFileBase[] File1,string PortfolioList,HttpPostedFileBase[] FileUpload)
        {
            /*07 June R2.1 change:-For actual Reffiles upload, instead of putting It in SOS S3 bucket first and then copying it to s2a (int bucket/transfer/s2a), 
             * we will now directly upload the file to s2a (int bucket/transfer/s2a). 
             * This is to avoid the bug that AWS sometimes fails to copy files from SOS bucket to INT bucket after approval. 
             * So not, there will be no movement of the file once it is uploaded directly in the target integration bucket. 
             * It will be visible to Alteryx once it is finally approved in the VUploadsFiles (as usual)
            It will be uploaded directly to
            <int bucket>/transfer/s2a/<OPCO>/upload/RefFiles/<RefFile Type>/<Year>/<Month>*/
            try
            {
               
                if (File1[0] != null)
                {
                    var UserName = System.Web.HttpContext.Current.Session["UserName"];
                    var Role = System.Web.HttpContext.Current.Session["UserRole"];
                    var companyName = System.Web.HttpContext.Current.Session["CompanyName"].ToString();
                    var RefFileType = "";
                    string AttachedFileNames = "";
                    var SupportingFileNames = "";
                    var filePath = "";
                    var SupportingDocFilePath = "";
                    if (LRFVM.LrfRefFileTypeId.HasValue)
                    {
                        RefFileType = LFTRC.GetById(LRFVM.LrfRefFileTypeId.Value).LrftName;
                    }
                    //else if(LRFVM.LrfPayeeId.HasValue)
                    //{
                    //   PayeeCode = LPRC.GetById(LRFVM.LrfPayeeId.Value).LpPayeeCode;
                    //}

                    foreach (HttpPostedFileBase files in File1)
                    {
                        string fileExtension = System.IO.Path.GetExtension(files.FileName);
                        string name = System.IO.Path.GetFileNameWithoutExtension(files.FileName);
                        string ManipulatedFileName = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                        if (string.IsNullOrEmpty(AttachedFileNames))
                        {
                            AttachedFileNames = ManipulatedFileName;
                            LRFVM.AttachedFilesName = AttachedFileNames;
                        }
                        else
                        {
                            AttachedFileNames = AttachedFileNames + "," + ManipulatedFileName;
                            LRFVM.AttachedFilesName = AttachedFileNames;
                        }

                        // filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/RefFiles/" + RefFileType);
                        //fileLocation = filePath + "/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;//As disscussed with VG file name will have datetime stamp as suffix
                        ////check if directory exists or not. iIf notcreate that directory
                        //bool exists = System.IO.Directory.Exists(filePath);
                        //if (!exists)
                        //{
                        //    System.IO.Directory.CreateDirectory(filePath);
                        //}
                        //files.SaveAs(fileLocation);

                        //SS:- Changing the refile path from sos to a2s i.e transfer/s2a/<opco>/upload/RefFiles/<RefFileType>/<year>/<month>
                        filePath = ConfigurationManager.AppSettings["S2AS3BucketFolder"] +"/"+ CompanyCode + "/Upload/RefFiles/" + RefFileType+"/"+ LRFVM.LrfYear+"/"+LRFVM.LrfMonth;
                        if (!Globals.FolderExistsInA2S(filePath))
                        {
                            Globals.CreateFolderInA2S(filePath, LRFVM.LrfMonth);
                        }
                        Globals.UploadToA2S(files.InputStream, ManipulatedFileName, filePath);
                        //filePath = ConfigurationManager.AppSettings["SOSBucketRootFolder"] + CompanyCode + "/RefFiles/" + RefFileType;
                        //if (!Globals.FolderExistsInS3(filePath))
                        //{
                        //    Globals.CreateFolderInS3Root(filePath);
                        //}
                        //Globals.UploadToS3(files.InputStream, ManipulatedFileName,  filePath);
                        //While saving bucket path we append it with /
                        filePath =filePath+"/";
                    }

                    //Add Description which is RefFileType:File1,File2....
                    LRFVM.LrfDescription = RefFileType + " : " + AttachedFileNames;
                    if(LRFVM.LrfDescription.Length > 1000)
                        LRFVM.LrfDescription = LRFVM.LrfDescription.Substring(0, 1000);
                    if (FileUpload != null)
                    {
                        foreach (HttpPostedFileBase files in FileUpload)
                    {
                            var fileLocation = string.Empty;
                            string fileExtension = System.IO.Path.GetExtension(files.FileName);
                            string name = System.IO.Path.GetFileNameWithoutExtension(files.FileName);
                            string ManipulatedFileName = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                            if (string.IsNullOrEmpty(SupportingFileNames))
                            {
                                SupportingFileNames = ManipulatedFileName;
                            }
                            else
                            {
                                SupportingFileNames = SupportingFileNames + "," + ManipulatedFileName;
                            }

                            //SupportingDocFilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/RefFiles/" + RefFileType);
                            //fileLocation = SupportingDocFilePath + "/SupportingDocuments/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;//As disscussed with VG file name will have datetime stamp as suffix
                            //                                                                                                                          //check if directory exists or not. iIf notcreate that directory
                            //bool exists = System.IO.Directory.Exists(filePath + "/SupportingDocuments");
                            //if (!exists)
                            //{
                            //    System.IO.Directory.CreateDirectory(filePath + "/SupportingDocuments");
                            //}
                            //files.SaveAs(fileLocation);

                            ////SS Changed the method to upload Supporting Doc in Ref Files to bucket
                            //SupportingDocFilePath = ConfigurationManager.AppSettings["SOSBucketRootFolder"] + CompanyCode + "/RefFiles/" + RefFileType+ "/SupportingDocuments";
                            //if (!Globals.FolderExistsInS3( SupportingDocFilePath))
                            //{
                            //    Globals.CreateFolderInS3Root(SupportingDocFilePath);
                            //}
                            //Globals.UploadToS3(files.InputStream, ManipulatedFileName, SupportingDocFilePath);

                            //SS:- Changing the refile path from sos to a2s i.e transfer/s2a/<opco>/upload/RefFiles/<RefFileType>/<year>/<month>/SupportingDocuments
                            SupportingDocFilePath = ConfigurationManager.AppSettings["S2AS3BucketFolder"] + "/" + CompanyCode + "/Upload/RefFiles/" + RefFileType + "/" + LRFVM.LrfYear + "/" + LRFVM.LrfMonth+ "/SupportingDocuments";
                            if (!Globals.FolderExistsInA2S(SupportingDocFilePath))
                            {
                                Globals.CreateFolderInA2S(filePath, "SupportingDocuments");
                            }
                            Globals.UploadToA2S(files.InputStream, ManipulatedFileName, SupportingDocFilePath);

                            //While saving bucket path we append it with /
                            SupportingDocFilePath =  SupportingDocFilePath + "/";

                        }
                    }

                    if (!string.IsNullOrEmpty(LRFVM.WFComments))//currently comments are hidden in form will remove this if not required
                    {
                        LRFVM.WFComments = "[" + UserName + "] [" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] " + LRFVM.WFComments;
                    }
                 
                    LRFVM.LrfUPdatedById = System.Web.HttpContext.Current.Session["UserId"].ToString();
                    LRFVM.LrfUpdatedDateTime = DateTime.UtcNow;
                    LRFVM.LrfCreatedById = System.Web.HttpContext.Current.Session["UserId"].ToString();
                    LRFVM.WFCompanyId = CompanyId;
                    LRFVM.LrfCreatedDateTime = DateTime.UtcNow;
                   
                    LRFVM.LrfCompanyId = CompanyId;
                    LRFVM.WFCurrentOwnerId = UserId;
                    LRFVM.WFRequesterId = UserId;
                    LRFVM.WFRequesterRoleId = LoggedInRoleId;
                    string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
                   var s= RestClient.Add(LRFVM,LoggedInRoleId,WorkflowName,AttachedFileNames,filePath,PortfolioList,SupportingFileNames,null,SupportingDocFilePath);
                    //Auto approve workflow after addition
                    try//Auto Approve the RefFile and display erro in case any validation fails
                    {
                        GGRC.UpdateActionStatus(WorkflowName, Convert.ToString(s), CompanyId, "Approve", UserId, string.Empty, LoggedInRoleId,string.Empty);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = ex.Data["ErrorMessage"].ToString();
                    }
                }

                var OutputJson = new { ErrorMessage = "Ref File added sucessfully", PopupMessage = "", RedirectToUrl = "/GenericGrid/Index" };
                return Json(OutputJson, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                //If exception generated from Api redirect control to view page where actions will be taken as per error type.
                if (!string.IsNullOrEmpty(ex.Data["ErrorMessage"] as string))
                {
                    switch ((int)ex.Data["ErrorCode"])
                    {
                        case (int)ExceptionType.Type1:
                            //redirect user to gneric error page
                            var OutputJson1 = new { ErrorMessage = "", PopupMessage = "", RedirectToUrl = Globals.ErrorPageUrl };
                            return Json(OutputJson1, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type2:
                            //redirect user (with appropriate errormessage) to same page (using viewmodel,controller nameand method name) from where request was initiated 
                            var OutputJson2 = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                            return Json(OutputJson2, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type3:
                            //Send Ex.Message to the error page which will be displayed as popup
                            var OutputJson3 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = ex.Data["RedirectToUrl"] as string };
                            return Json(OutputJson3, JsonRequestBehavior.AllowGet);
                        case (int)ExceptionType.Type4:
                            var OutputJson4 = new { ErrorMessage = "", PopupMessage = ex.Data["ErrorMessage"] as string, RedirectToUrl = "" };
                            return Json(OutputJson4, JsonRequestBehavior.AllowGet);
                    }
                    var OutputJson = new { ErrorMessage = ex.Data["ErrorMessage"] as string, PopupMessage = "", RedirectToUrl = "" };
                    return Json(OutputJson, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //If exception does not match any type. Make an entry in GErrorLog as it may be an exception generated from Web App
                    TempData["Error"] = "Record could not be uploaded";
                    throw ex;
                }
            }
        }
        //[ControllerActionFilter]
        public ActionResult Review(int TransactionId, int WFConfigId)
        {
            ViewBag.ActionItems = GGRC.GetSecondaryFormButtons(WFConfigId, LoggedInRoleId,UserId, TransactionId);
            var ReFFileDetails = RestClient.GetById(TransactionId);
            ViewBag.LrfRefFileTypeId = GetRefFileType(ReFFileDetails.LrfRefFileTypeId.Value);
            ViewBag.LrfYear = GetYear();
            return View(ReFFileDetails);
        }

        //This method is  defined to update status of  uploaded files and also update who and when columns during update
        //public ActionResult UpdateUploadStatus(int Id,string Status,string Comments,HttpPostedFileBase[] TestResults)
        //{
        //    var UploadedFile=new LRefFileViewModel();
        //    try
        //    {
        //        //getting details of uploded file before updating to get analyst name which will bw used while saving file
        //        UploadedFile = RestClient.GetById(Id);
        //        var companyName = System.Web.HttpContext.Current.Session["CompanyName"].ToString();
        //        var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
        //        var UserName = System.Web.HttpContext.Current.Session["UserName"].ToString();
        //        var UserRole= System.Web.HttpContext.Current.Session["UserRole"].ToString();
        //        var model = new LRefFileViewModel();
        //        //First save test results if procvided by system analyst
        //        if (TestResults!=null)
        //        {
        //            var TestResult = "";
        //            foreach(HttpPostedFileBase files in TestResults)
        //            {
        //                var fileLocation = "";
        //                string fileExtension = System.IO.Path.GetExtension(files.FileName);
        //                string name = System.IO.Path.GetFileNameWithoutExtension(files.FileName);
        //                if (string.IsNullOrEmpty(TestResult))
        //                {
        //                    TestResult = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
        //                }
        //                else
        //                {
        //                    TestResult =TestResult+","+ name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension ;
        //                }
        //                var filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], companyName + "/ RefFiles / Schemes");
        //                fileLocation = filePath + "/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
        //                //check if directory exists or not. iIf notcreate that directory
        //                bool exists = System.IO.Directory.Exists(filePath);
        //                if (!exists)
        //                {
        //                    System.IO.Directory.CreateDirectory(filePath);
        //                }
        //                files.SaveAs(fileLocation);
        //            }
        //            model.LufSchemeTestResults = TestResult;
        //            model.LufIsSchemeTested = true;
        //        }

        //        model.LufUPdatedById = UserId;
        //        model.Id = Id;
        //        model.LufUpdatedDateTime = DateTime.UtcNow.Date;
        //        model.LufStatus = Status;
        //        RestClient.UpdateStatus(model,Comments,UserName);

        //        //The files will be copied to s2a bucket if approval suceeds
        //        if (Status == "Approved")
        //        {
        //            //If manager approves the Uploaded Files save this file in a seperate location defined for analyst and manager seperately
        //            var FileArray = UploadedFile.LufFileName.Split(',');
        //            ZipFile zip = new ZipFile();
        //            zip.AlternateEncodingUsage = ZipOption.AsNecessary;
        //            zip.AddDirectoryByName("Files");
        //            for (var i = 0; i < FileArray.Length; i++)
        //            {
        //                var AnalystfilePath = "";
        //                var ManagerfilePath = "";
        //                //The above lines are added to save the file in a new path after approval by manager
        //                //switch (UploadedFile.LufUploadType)
        //                //{
        //                //    case "Reference":
        //                        string Previouspath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], UploadedFile.GcCode + "/RefFiles/" + UploadedFile.LrftName);
        //                        //Analyst copy of file is saved
        //                        AnalystfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_S2ACommomFilePath"], UploadedFile.GcCode + "/" + UploadedFile.UpdatedBy + "/RefFiles/" + UploadedFile.LrftName + "/" + UploadedFile.LufYear + "/" + UploadedFile.LufMonth);
        //                        //check if directory exists or not.If not create a new one
        //                        bool exists = System.IO.Directory.Exists(AnalystfilePath);
        //                        if (!exists)
        //                        {
        //                            System.IO.Directory.CreateDirectory(AnalystfilePath);
        //                        }
        //                        System.IO.File.Copy(Previouspath + "/" + FileArray[i], AnalystfilePath + "/" + FileArray[i]);

        //                        //Manager Copy of File is saved
        //                        ManagerfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_S2ACommomFilePath"], UploadedFile.GcCode + "/" + UserName + "/RefFiles/" + UploadedFile.LrftName + "/" + UploadedFile.LufYear + "/" + UploadedFile.LufMonth);
        //                        //check if directory exists or not.If not create a new one
        //                        exists = System.IO.Directory.Exists(ManagerfilePath);
        //                        if (!exists)
        //                        {
        //                            System.IO.Directory.CreateDirectory(ManagerfilePath);
        //                        }
        //                        System.IO.File.Copy(Previouspath + "/" + FileArray[i], ManagerfilePath + "/" + FileArray[i]);

        //                break;
        //            case "PayeeDocument":

        //                string PayeePreviousPath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/Payees/" + UploadedFile.LpPayeeCode);
        //                //Analyst copy of file is saved
        //                AnalystfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], UploadedFile.GcCode + "/Payees/" + UploadedFile.LpPayeeCode + "/Documents /Sent/" + UploadedFile.LufYear + "/" + UploadedFile.LufMonth);
        //                //check if directory exists or not
        //                bool Payeeexists = System.IO.Directory.Exists(AnalystfilePath);
        //                if (!Payeeexists)
        //                {
        //                    System.IO.Directory.CreateDirectory(AnalystfilePath);
        //                }
        //                System.IO.File.Copy(PayeePreviousPath + "/" + FileArray[i], AnalystfilePath + "/" + FileArray[i]);
        //                //The below code attach documents email to payee for his approved documents 
        //                if (!string.IsNullOrEmpty(FileArray[i]))
        //                {
        //                    var filePath = AnalystfilePath + "/" + FileArray[i];
        //                    zip.AddFile(filePath, "Files");
        //                }
        //                //Manager copy of file is saved here
        //                //ManagerfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], UploadedFile.GcCompanyName + "/Documents/Payees" + UploadedFile.LufYear + "/" + UploadedFile.LufMonth + "/" + UploadedFile.LpPayeeCode);
        //                ////check if directory exists or not
        //                //exists = System.IO.Directory.Exists(ManagerfilePath);
        //                //if (!exists)
        //                //{
        //                //    System.IO.Directory.CreateDirectory(ManagerfilePath);
        //                //}
        //                //System.IO.File.Copy(PayeePreviousPath + "/" + UploadedFile.LufFileName, ManagerfilePath + "/" + UploadedFile.LufUserFriendlyFileName);

        //                break;
        //            case "Scheme":

        //                string SchemePreviousPath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], UploadedFile.GcCode + "/RefFiles/Schemes");
        //                //Analyst copy of file is saved
        //                AnalystfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_S2ACommomFilePath"], UploadedFile.GcCode+"/" + UploadedFile.UpdatedBy + "/RefFiles/Scheme/" + UploadedFile.LufYear + "/" + UploadedFile.LufMonth);
        //                //check if directory exists or not
        //                bool Schemeexists = System.IO.Directory.Exists(AnalystfilePath);
        //                if (!Schemeexists)
        //                {
        //                    System.IO.Directory.CreateDirectory(AnalystfilePath);
        //                }
        //                System.IO.File.Copy(SchemePreviousPath + "/" + FileArray[i], AnalystfilePath + "/" + FileArray[i]);
        //                //Manager copy of file is saved here
        //                ManagerfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_S2ACommomFilePath"], UploadedFile.GcCode +"/"+ UserName + "/RefFiles/Scheme/" + UploadedFile.LufYear + "/" + UploadedFile.LufMonth);
        //                //check if directory exists or not
        //                exists = System.IO.Directory.Exists(ManagerfilePath);
        //                if (!exists)
        //                {
        //                    System.IO.Directory.CreateDirectory(ManagerfilePath);
        //                }
        //                System.IO.File.Copy(SchemePreviousPath + "/" + FileArray[i], ManagerfilePath + "/" + FileArray[i]);
        //                break;
        //        }
        //    }

        //    //Send Email to Payee after copying all his files
        //    if (UploadedFile.LufUploadType.Equals("PayeeDocument"))
        //    {
        //        string ReceiverEmail = "";//If Bloack Access is true for Payee then Email will be sent to created by user
        //        if (UploadedFile.LpCreateLogin)
        //        {
        //            ReceiverEmail = UploadedFile.PayeeCreatedBy;
        //            if (System.IO.File.Exists(ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/File.zip"))
        //            {
        //                System.IO.File.Delete(ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/File.zip");//Delete the file if exist
        //            }
        //            zip.Save(ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/File.zip");
        //            Globals.SendEmail("Your following documents have been approved by Manager which are attached in this Email", "Vodafone LITE", ReceiverEmail, null, ConfigurationManager.AppSettings["S3_SosFilePath"] + "/Temp/File.zip");
        //        }



        //            }
        //        }
        //        return RedirectToAction("Index","GenericGrid");
        //    }
        //    catch(Exception ex)
        //    {
        //        TempData["Error"] = ex.Message;
        //        throw ex;
        //       // return RedirectToAction("Index", new { UploadType = UploadedFile.LufUploadType });
        //    }
        //}


        //This method will be called when user clicks on download button in Index Page
        // [HttpPost]
        //public ActionResult Index(int UploadedFileId)
        //{
        //    var UserRole = System.Web.HttpContext.Current.Session["UserRole"];
        //    var UploadedFiles = RestClient.GetById(UploadedFileId);
        //    string[] s = UploadedFiles.LufFileName.Split(',');
        //    //Add test results file if available in database
        //    if (UserRole.Equals("Manager"))
        //    {
        //        string[] TestResult = UploadedFiles.LufSchemeTestResults.Split(',');
        //        s = s.Union(TestResult).ToArray();
        //    }
        //    using (ZipFile zip= new ZipFile())
        //    {
        //       // zip.AlternateEncodingUsage = ZipOption.AsNecessary;
        //        zip.AddDirectoryByName("Files");
        //        for (var i = 0; i < s.Length; i++)
        //        {
        //            if (!string.IsNullOrEmpty(s[i]))
        //            {
        //                var filePath = "";
        //                switch (UploadedFiles.LufUploadType)
        //                { //Note: These file paths are defined in the file uploads userguide.
        //                    case "Reference":
        //                        filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], UploadedFiles.GcCode + "/RefFiles/" + UploadedFiles.LrftName);
        //                        break;
        //                    case "PayeeDocument":
        //                        filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/Payees/" + UploadedFiles.LpPayeeCode);
        //                        break;
        //                    case "RawData":
        //                        filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], UploadedFiles.GcCode + "/RawData");
        //                        break;
        //                    case "Scheme":
        //                        filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], UploadedFiles.GcCode + "/RefFiles/Schemes");
        //                        break;

        //                }
        //                var FileName = filePath + "/" + s[i];
        //                if (System.IO.Directory.Exists(filePath))
        //                {
        //                    zip.AddFile(FileName, "Files");
        //                }
        //            }
        //        }
        //        Response.Clear();
        //        Response.BufferOutput = false;
        //        string zipName = String.Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd"));
        //        Response.ContentType = "application/zip";
        //        Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
        //        zip.Save(Response.OutputStream);
        //        Response.End();
        //    }
        //    ViewBag.UploadType = UploadedFiles.LufUploadType;
        //    return View();
        //}
    }
}