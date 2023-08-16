using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;
using Ionic.Zip;
using System.IO;
using Vodafone_SOS_WebApp.Utilities;
using System.Configuration;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;

namespace Vodafone_SOS_WebApp.Controllers
{
    [HandleCustomError]
    [SessionExpire]
    public class LReadyToPayController : Controller//PrimaryController//RK R2.3 17112018 made this change (comment URL Tempring) so that review can open in new tab
    {
        //Reporting Analyst : Commission->Payments
        LReadyToPayRestClient RestClient = new LReadyToPayRestClient();
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string strRoleName = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
        int iCompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        int iUserID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LUserId"]);
        string CurrentUserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        int iRTPID = 0;
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Ready To Pay Batches";
            LPortfoliosRestClient LP = new LPortfoliosRestClient();
            LP.GetByUserId(LoggedInUserId, LoggedInRoleId);
            ViewBag.PeriodName = GetCommissionPeriods();
            ViewBag.LoggedInRoleId = LoggedInRoleId;
            return View();
        }
        private SelectList GetCommissionPeriods()
        {
            //ILCommissionPeriodsRestClient CRC = new LCommissionPeriodsRestClient();
            //var ApiData = CRC.GetByCompanyIdStatus(iCompanyId, "Open");
            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
            var ApiData = CPRC.GetByCompanyId(iCompanyId);
            var x = new SelectList(ApiData, "LcpPeriodName", "LcpPeriodName");
            return x;
        }
        private SelectList GetPayPublishEmailIDs()
        {
            IRPayPublishEmailsRestClient IPRC = new RPayPublishEmailsRestClient();
            var ApiData = IPRC.GetEmailIDsDropdownData(iCompanyId);
            var x = new SelectList(ApiData, "EmailIds", "EmailIds");
            return x;
        }
        [ControllerActionFilter]
        public ActionResult CreatePaymentBatch()
        {
            ViewBag.PeriodName = GetCommissionPeriods();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult CreatePaymentBatch(LReadyToPayViewModel modal, string strKeyValues, string strSaveStatus, string strSelectedPortfolios)
        {

            string strPayBatchName = modal.PaymentBatchName;
            string strBatchCommPeriod = modal.PeriodName;
            string strRTPData = strKeyValues;
            string strAction = "Insert";
            RestClient.AddEditBatch(0, iCompanyId, false, strSaveStatus, "Insert", strAction, strPayBatchName, strBatchCommPeriod, strRTPData, LoggedInUserId, LoggedInUserId, strSelectedPortfolios, false, false, string.Empty, strRoleName,false,false,false);
            ViewBag.Message = "ReadyToPay Batch Created!";
            return RedirectToAction("Index");
        }
        [ControllerActionFilter]
        public ActionResult Edit(int id, bool? DisplayPayGrid)
        {
            if (!DisplayPayGrid.HasValue)
                DisplayPayGrid = false;
            ViewBag.DisplayPayGrid = DisplayPayGrid;
            //Get CompanySpecific Columns
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var PayApiData = LCSCRC.GetPayColumnsByCompanyIdForGrid(iCompanyId).OrderBy(p => p.LcscOrdinalPosition).ToList();
            //Now populate the DataType column Special case for calculations part
            foreach (var CompanySpecificColumn in PayApiData)
            {
                CompanySpecificColumn.DataType = CompanySpecificColumn.LcscDataType;//CompanySpecificColumn.ColumnName.Split('~').LastOrDefault();
                CompanySpecificColumn.ColumnName = CompanySpecificColumn.ColumnName;//CompanySpecificColumn.ColumnName.Split('~').FirstOrDefault();
                CompanySpecificColumn.LcscLabel = (string.IsNullOrEmpty(CompanySpecificColumn.LcscLabel)) ? CompanySpecificColumn.ColumnName.Replace("X", "") : CompanySpecificColumn.LcscLabel;
            }
            //Add to ViewBag
            ViewBag.CompSpecificColumn = PayApiData;
            System.Web.HttpContext.Current.Session["Title"] = "Edit Ready To Pay Batch";
            ViewBag.PeriodName = GetCommissionPeriods();
            iRTPID = id;
            LReadyToPayViewModel RTPV = GetByID(id);
            //Pass status wise row counts to view to be displayed on tab headers
            ILPayRestClient LPRC = new LPayRestClient();
            var PayCounts = LPRC.GetLPayCounts(RTPV.PaymentBatchNo, iCompanyId).FirstOrDefault();
            if (PayCounts != null)
                ViewBag.PayRowCount = PayCounts.RowCounts;
            else
                ViewBag.PayRowCount = 0;
            return View(RTPV);
        }
        [HttpPost]
       // [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit(LReadyToPayViewModel modal, string strKeyValues, string strSaveStatus, string strSelectedPortfolios,string IsClaimChanged, string IsCalChanged, string isMAChanged)
        {
            if (!ModelState.IsValid)
            {
                return View(modal);
            }

            string strPayBatchName = modal.PaymentBatchName;
            string strBatchCommPeriod = modal.PeriodName;
            string strRTPData = strKeyValues;
            RestClient.AddEditBatch(modal.Id, iCompanyId, false, strSaveStatus, "Update", "Update", strPayBatchName, strBatchCommPeriod, strRTPData, LoggedInUserId, LoggedInUserId, strSelectedPortfolios, false, false, string.Empty, strRoleName, Convert.ToBoolean(IsClaimChanged), Convert.ToBoolean(IsCalChanged), Convert.ToBoolean(isMAChanged));
            ViewBag.Message = "ReadyToPay Batch Updated!";
            return RedirectToAction("Index");
        }
        [ControllerActionFilter]
        public ActionResult Review(int id, Nullable<bool> DisplayPayGrid)
        {
            if (!DisplayPayGrid.HasValue)
                DisplayPayGrid = false;
            ViewBag.DisplayPayGrid = DisplayPayGrid;
            System.Web.HttpContext.Current.Session["Title"] = "Review Ready To Pay Batch";
            ViewBag.PeriodName = GetCommissionPeriods();
            iRTPID = id;
            LReadyToPayViewModel RTPV = GetByID(id);
            //Get CompanySpecific Columns
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var PayApiData = LCSCRC.GetPayColumnsByCompanyIdForGrid(iCompanyId).OrderBy(p => p.LcscOrdinalPosition).ToList();
            //Now populate the DataType column Special case for calculations part
            foreach (var CompanySpecificColumn in PayApiData)
            {
                CompanySpecificColumn.DataType = CompanySpecificColumn.LcscDataType;//CompanySpecificColumn.ColumnName.Split('~').LastOrDefault();
                CompanySpecificColumn.ColumnName = CompanySpecificColumn.ColumnName;//CompanySpecificColumn.ColumnName.Split('~').FirstOrDefault();
                CompanySpecificColumn.LcscLabel = (string.IsNullOrEmpty(CompanySpecificColumn.LcscLabel)) ? CompanySpecificColumn.ColumnName.Replace("X", "") : CompanySpecificColumn.LcscLabel;
            }
            //Add to ViewBag
            //Pass status wise row counts to view to be displayed on tab headers
            ILPayRestClient LPRC = new LPayRestClient();
            var PayCounts = LPRC.GetLPayCounts(RTPV.PaymentBatchNo, iCompanyId).FirstOrDefault();
            if (PayCounts != null)
                ViewBag.PayRowCount = PayCounts.RowCounts;
            else
                ViewBag.PayRowCount = 0;
            ViewBag.CompSpecificColumn = PayApiData;
            string CountPayment = GetPaymentCount(id, CompanyCode);
            RTPV.CountPayment = CountPayment;
            return View(RTPV);
        }
        //public ActionResult Cancel(int id)
        //{
        //    System.Web.HttpContext.Current.Session["Title"] = "Cancel Ready To Pay Batch";
        //    iRTPID = id;
        //    LReadyToPayViewModel RTPV = GetByID(id);
        //    return View(RTPV);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Cancel(int ID)
        {
            //string strPayBatchName = modal.PaymentBatchName;
            //string strBatchCommPeriod = modal.PeriodName;

            RestClient.AddEditBatch(ID, iCompanyId, false, "", "Cancel", "Cancel", "", "", "", LoggedInUserId, LoggedInUserId, "", false, false, string.Empty, strRoleName,false,false,false);
            ViewBag.Message = "ReadyToPay Batch cancelled!";
            return RedirectToAction("Index");
        }
        //public ActionResult Approve(int id)
        //{
        //    System.Web.HttpContext.Current.Session["Title"] = "Approve Ready To Pay Batch";
        //    iRTPID = id;
        //    LReadyToPayViewModel RTPV = GetByID(id);
        //    return View(RTPV);
        //}
        [ControllerActionFilter]
        public ActionResult Publish(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Publish Ready To Pay Batch";
            iRTPID = id;
            LReadyToPayViewModel RTPV = GetByID(id);
            string CountPayment = GetPaymentCount(id, CompanyCode);
            if (RTPV != null)
            {
                //CountPayment .replaceAll("^.|.$", "");
                RTPV.CountPayment = Convert.ToString(CountPayment);
            }
            ViewBag.PayPublishEmailIds = GetPayPublishEmailIDs();
            return View(RTPV);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Publish(LReadyToPayViewModel modal)
        {
            try
            {
                if (modal.BatchName == null)
                    modal.BatchName = string.Empty;
                if (modal.LbCommissionPeriod == null)
                    modal.LbCommissionPeriod = string.Empty;
                RestClient.AddEditBatch(modal.Id, iCompanyId, false, "", "Publish", "Publish", modal.PaymentBatchName, modal.LbCommissionPeriod, "", LoggedInUserId, LoggedInUserId, "", modal.EmailDocuments, modal.SendPayeeDocuments, modal.PayPublishEmailIds, strRoleName,false,false,false);
                ViewBag.Message = "ReadyToPay Batch published!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
            }
            return RedirectToAction("Index");
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Approve(LReadyToPayViewModel modal, string strKeyValues, string strSaveStatus)
        //{
        //    string strPayBatchName = modal.PaymentBatchName;
        //    string strBatchCommPeriod = modal.PeriodName;
        //    string strRTPData = strKeyValues;
        //    //RestClient.AddEditBatch(0, iCompanyId, false, strSaveStatus, "Approve", "Approve", strPayBatchName, strBatchCommPeriod, strRTPData, LoggedInUserId, LoggedInUserId, "");
        //    ViewBag.Message = "ReadyToPay Batch approved!";
        //    return RedirectToAction("Index");
        //}

        private LReadyToPayViewModel GetByID(int iTRPID)
        {
            var data = RestClient.GetRTPDetails(iTRPID);
            return (LReadyToPayViewModel)data;
        }
        private string GetPaymentCount(int iTRPID, string CountryCode)
        {
            string data = RestClient.GetPaymentCount(iTRPID, CountryCode);
            return data;
        }
        [ControllerActionFilter]
        public JsonResult GetNewRTPByCompanyID()
        {
            var ApiData = RestClient.GetNewRTPByCompanyID(iCompanyId, LoggedInUserId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        [ControllerActionFilter]
        public JsonResult GetCancelledRTPByCompanyID()
        {
            var ApiData = RestClient.GetCancelledRTPByCompanyID(iCompanyId, LoggedInUserId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        [ControllerActionFilter]
        public JsonResult GetPayGeneratedRTPByCompanyID()
        {
            var ApiData = RestClient.GetPayGeneratedRTPByCompanyID(iCompanyId, LoggedInUserId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        [ControllerActionFilter]
        public JsonResult GetPublishedRTPByCompanyID()
        {
            var ApiData = RestClient.GetPublishedRTPByCompanyID(iCompanyId, LoggedInUserId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        //RS IMPLEMENTATION

        
             public JsonResult GetCountsBatchesForNew(int iRTPID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strPortfolios)
        {


            var data = RestClient.ReadyToPayDataCounts(iRTPID, iCompanyId, blnIsBatchList, strType, "",strRTPStatus, "", "", "", "", "", strPortfolios);
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetCountsBatches(int iRTPID, string strType, string strAction, string strPortfolios)
        {
           
            var data = RestClient.ReadyToPayDataCounts(iRTPID, iCompanyId, Convert.ToBoolean("False"), strType, "", strAction, "", "", "", "", "", strPortfolios);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //RS Implementation


       // [ControllerActionFilter]
        public JsonResult GetBatchesForNewTest(int iRTPID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strPortfolios, string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum)
        {
            if (pagesize == null) pagesize = 200;
            if (pagenum == null) pagenum = 0;
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            if(FilterQuery != null && FilterQuery != "")
            {
                TempData[strType] = FilterQuery;

            }

            var data = RestClient.ReadyToPayData(iRTPID, iCompanyId, blnIsBatchList, strType, strRTPStatus, "", "", "", "", "", "", strPortfolios, sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery);
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClaimValidate(string Type, string CSVData,string  strPortfolios)
        {
            
            var data = RestClient.GetClaimValidate(iCompanyId, Type, CSVData, strPortfolios);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBatchesForAddPaymentBatch(int iRTPID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strPortfolios, string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum)
        {
            if (pagesize == null) pagesize = 200;
            if (pagenum == null) pagenum = 0;
            var FilterQuery = string.Empty;
            if (TempData[strType] != null) {
                 FilterQuery = Convert.ToString(TempData[strType]);
               };
           // var qry = Request.QueryString;
           // var FilterQuery = Globals.BuildQuery(qry);
            List<AddPaymentModel> objlstaddPaymentModel = new List<AddPaymentModel>();

            var data = RestClient.ReadyToPayData(iRTPID, iCompanyId, blnIsBatchList, strType,"", strRTPStatus, "", "", "", "", "", strPortfolios, sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery);
            foreach (var data1 in data)
            {
                AddPaymentModel objAddPaymentModel = new AddPaymentModel();
                objAddPaymentModel.Identifier = data1.Identifier;
                objlstaddPaymentModel.Add(objAddPaymentModel);
            }
            return Json(objlstaddPaymentModel, JsonRequestBehavior.AllowGet);
        }
        [ControllerActionFilter]
        public JsonResult GetBatches(int iRTPID, string strType, string strAction, string strPortfolios, string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum)
        {
            if (pagesize == null) pagesize = 200;
            if (pagenum == null) pagenum = 0;
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            if (FilterQuery != null && FilterQuery != "")
            {
                TempData[strType] = FilterQuery;

            }
            var data = RestClient.ReadyToPayData(iRTPID, iCompanyId, Convert.ToBoolean("False"), strType, "", strAction, "", "", "", "", "", strPortfolios, sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [ControllerActionFilter]
        public JsonResult GetPortfolios()
        {
            ILPortfoliosRestClient LPRC = new LPortfoliosRestClient();
            var ApiData = LPRC.GetByUserIdForEditGrid(iUserID, iCompanyId, CompanyCode, strRoleName);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        [ControllerActionFilter]
        public JsonResult GetRTPPortFolios(int RTPId, string strAction)
        {
            var ApiData = RestClient.GetRTPPortfolios(RTPId, iCompanyId, strAction, LoggedInUserId, strRoleName);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        //DASHBOARD Button: Will download files from A2S/Pay/batchnumber_*) 
        public ActionResult DownloadDashboard(int RTPId)
        {
            try
            {
                //string FileName = "";
                //string FileExtension = "";
                //string FileType = "";
                var BatchDetails = RestClient.GetRTPDetails(RTPId);
                
                //Get File from A2S S3
                string DashboardFilePath = DashboardFilePath = System.Configuration.ConfigurationManager.AppSettings["A2SS3BucketFolder"].ToString() + "\\" + CompanyCode + "\\Pay\\" + BatchDetails.PaymentBatchNo;//R3.1 CompanyCode.ToLower() removed
                var ZippedData = GetFilesFromAWS(DashboardFilePath, CompanyCode, LoggedInUserName);
                if (ZippedData != null)
                {
                    return File(ZippedData, "application/zip", "ExportPayDashboard_" + BatchDetails.PaymentBatchNo + ".zip");
                }
               
                TempData["Message"] = "There is no Dashboard available";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            catch
            {
                TempData["Message"] = "There is no Dashboard available";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
        }


        //DOWNLOAD PaymentFile Button:  will download payment files from   (A2S/<opco>/PaymentFiles/<Paymentbatchnumber>/.) 
        public ActionResult DownloadPayementFiles(int RTPId)
        {
            try
            {
                var BatchDetails = RestClient.GetRTPDetails(RTPId);
                //Get File from A2S S3
                string DashboardFilePath = DashboardFilePath = System.Configuration.ConfigurationManager.AppSettings["A2SS3BucketFolder"].ToString() + "\\" + CompanyCode + "\\PaymentFiles\\" + BatchDetails.PaymentBatchNo;//R3.1 CompanyCode.ToLower() is removed
                var ZippedData = GetFilesFromAWS(DashboardFilePath, CompanyCode, LoggedInUserName);
                if (ZippedData != null)
                {
                   
                    return File(ZippedData, "application/zip", "ExportPayPayementFiles_" + BatchDetails.PaymentBatchNo + ".zip");
                }

                TempData["Message"] = "There is no PayementFile available";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            catch
            {
                TempData["Message"] = "There is no PayementFile available";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
        }

        //DOWNLOAD PayeeDocuments: will download all documents from A2S/<opco>/PayeeDocuments/<PaymentBatchNumber>/.
        /*Payment cycle, pay generated tab, clicking on download payee files gives null exception
          we need to handle this as it might be possible that files are not available
*/
        public ActionResult DownloadPayeeDocumentFiles(int RTPId)
        {
            try
            {
                var BatchDetails = RestClient.GetRTPDetails(RTPId);
                var ListOfPayeeFiles = RestClient.GetXPayeeDocuments(CompanyCode, BatchDetails.PaymentBatchNo);
                var FilesToBezipped = new List<string>();
                for (var i = 0; i < ListOfPayeeFiles.Count(); i++)
                {
                    var FileName = ListOfPayeeFiles.ElementAt(i).XFileName;
                    //var CompleteFileName = ListOfPayeeFiles.ElementAt(i).XFileLocation + "/" + ListOfPayeeFiles.ElementAt(i).XFileName;
                    var bytedata = Globals.DownloadFromA2S(FileName, ListOfPayeeFiles.ElementAt(i).XFileLocation);
                    if (bytedata != null)
                    {
                        if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
                            System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");
                        var TempFileFolder = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + ListOfPayeeFiles.ElementAt(i).XPayeeCode + "_" + FileName;
                        System.IO.File.WriteAllBytes(TempFileFolder, bytedata); // Save File
                        //Attaching FileName because same filename could be sent from Alteryx for different payees
                        FilesToBezipped.Add(ListOfPayeeFiles.ElementAt(i).XPayeeCode + "_" + FileName);
                    }
                }

                if (ListOfPayeeFiles.Count() > 0)
                {
                    FilesToBezipped = FilesToBezipped.Select(p => new { CompleteFileName = (ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip" + "/" + p) }).Select(p => p.CompleteFileName).ToList();
                    var ZippedData = ZipHelper.ZipFilesToByteArray(FilesToBezipped, System.IO.Packaging.CompressionOption.Normal);
                    foreach (var file in FilesToBezipped)
                    {
                        System.IO.File.Delete(file);
                    }
                    return File(ZippedData, "application/zip", "ExportPayPayeeDocuments_" + BatchDetails.PaymentBatchNo + ".zip");
                }

                TempData["Message"] = "There is no PayeeDocument available";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            catch (Exception Ex)
            {
                TempData["Message"] = "There is no PayeeDocument available";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
        }

        public byte[] GetFilesFromAWS(string DashboardFilePath, string CompanyCode, string UserName)
        {

            var FileName = "";
            string _awsAccessKey = Globals.GetValue("int_accesskey"); //ConfigurationManager.AppSettings["A2SS3AccessKey"];
            string _awsSecretKey = Globals.GetValue("int_secretkey"); //ConfigurationManager.AppSettings["A2SS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["A2SS3Bucketname"];

            AmazonS3Client s3Client = new AmazonS3Client(_awsAccessKey, _awsSecretKey);
            S3DirectoryInfo dir = new S3DirectoryInfo(s3Client, _bucketName, DashboardFilePath);
            foreach (IS3FileSystemInfo file in dir.GetFileSystemInfos())
            {
                //Console.WriteLine(file.FullName);
                var CompleteFilePath = DashboardFilePath + "\\" + file.Name;
                CompleteFilePath = CompleteFilePath.Replace("\\", "/");
                GetObjectRequest request = new GetObjectRequest
                {

                    BucketName = _bucketName,
                    Key = CompleteFilePath,
                };

                using (GetObjectResponse response = s3Client.GetObject(request))
                {
                    FileName = file.Name;
                    var TempFileFolder = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserName + "/forzip/" + FileName;
                    if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserName + "/forzip/"))
                        System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserName + "/forzip/");
                    System.IO.File.WriteAllBytes(TempFileFolder, Globals.ReadFully(response.ResponseStream)); // Save File

                }
            }
            DirectoryInfo d = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserName + "/forzip");//Assuming Test is your Folder
            var Files = d.GetFiles().Select(p => p.FullName).ToList();
            if (Files.Count() > 0)
            {
                //var FilesToBezipped = LSupportingDocs.Select(p => new { CompleteFileName = (ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + p.OriginalFileName) }).Select(p => p.CompleteFileName).ToList();
                var ZippedData = ZipHelper.ZipFilesToByteArray(Files, System.IO.Packaging.CompressionOption.Normal);
                //System.IO.DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"]);//Now Delete all files
                foreach (string file in Files)
                {
                    System.IO.File.Delete(file);
                }
                return ZippedData;
            }
            return null;
        }
        [HttpGet]
        public ActionResult DownloadPayeeDocument(int RTPId)

        {
            LReadyToPayViewModel RTPV = GetByID(RTPId);
            string CountPayment = GetPaymentCount(RTPId, CompanyCode);
            RTPV.CountPayment = CountPayment;
            return View(RTPV);
        }
        public JsonResult GetPDCounts(int RTPId)
        {
            string ApiCounts = GetPaymentCount(RTPId, CompanyCode);
            return Json(ApiCounts, JsonRequestBehavior.AllowGet);
        }

        // [ControllerActionFilter]
        //public JsonResult GetRTPBatch(int RTPId)
        //{
        //    var BatchDetails = RestClient.GetRTPDetails(RTPId);
        //    var ListOfPayeeFiles = RestClient.GetXPayeeDocuments(CompanyCode, BatchDetails.PaymentBatchNo);
        //    return Json(ListOfPayeeFiles, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetRTPBatch(int RTPId, string sortdatafield, string sortorder, int pagesize, int pagenum, string PaymentBatchNo, string Payeecode)
        {

            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var BatchDetails = RestClient.GetRTPDetails(RTPId);
            var ApiData = RestClient.GetPayeeDocumentpaging(CompanyCode, BatchDetails.PaymentBatchNo, sortdatafield, sortorder, pagesize, pagenum, FilterQuery,Payeecode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadPayeeDocumentFilesNew(string filedata, string PaymentBatchNo)
        {
            try
            {
                List<string> data = filedata.Split(',').ToList<string>();
                //var BatchDetails = RestClient.GetRTPDetails(RTPId);
                //var ListOfPayeeFiles = RestClient.GetXPayeeDocuments(CompanyCode, BatchDetails.PaymentBatchNo);
                var FilesToBezipped = new List<string>();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> separateddata = data[i].Split('$').ToList<string>();
                    var FileName = separateddata[0];
                    var Filefilepath = separateddata[1];
                    var PayeeCode = separateddata[2];
                    var bytedata = Globals.DownloadFromA2S(FileName, Filefilepath);
                    if (bytedata != null)
                    {
                        if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
                            System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");
                        var TempFileFolder = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + PayeeCode + "_" + FileName;
                        System.IO.File.WriteAllBytes(TempFileFolder, bytedata); // Save File
                        //Attaching FileName because same filename could be sent from Alteryx for different payees
                        FilesToBezipped.Add(PayeeCode + "_" + FileName);
                    }
                }

                if (data.Count() > 0)
                {
                    FilesToBezipped = FilesToBezipped.Select(p => new { CompleteFileName = (ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip" + "/" + p) }).Select(p => p.CompleteFileName).ToList();
                    var ZippedData = ZipHelper.ZipFilesToByteArray(FilesToBezipped, System.IO.Packaging.CompressionOption.Normal);
                    foreach (var file in FilesToBezipped)
                    {
                        System.IO.File.Delete(file);
                    }
                    return File(ZippedData, "application/zip", "ExportPayPayeeDocuments_" + PaymentBatchNo + ".zip");
                }

                TempData["Message"] = "There is no PayeeDocument available";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            catch (Exception Ex)
            {
                TempData["Message"] = "There is no PayeeDocument available";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
        }
        [HttpPost
            ]
        public ActionResult DownloadPayeeDocument(LReadyToPayViewModel RTPV)

        {
            try
            {
                List<string> data = RTPV.DocList.Split(',').ToList<string>();
                //var BatchDetails = RestClient.GetRTPDetails(RTPId);
                //var ListOfPayeeFiles = RestClient.GetXPayeeDocuments(CompanyCode, BatchDetails.PaymentBatchNo);
                var FilesToBezipped = new List<string>();
                for (var i = 0; i < data.Count(); i++)
                {
                    List<string> separateddata = data[i].Split('$').ToList<string>();
                    var FileName = separateddata[0];
                    var Filefilepath = separateddata[1];
                    var PayeeCode = separateddata[2];
                    var bytedata = Globals.DownloadFromA2S(FileName, Filefilepath);
                    if (bytedata != null)
                    {
                        if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
                            System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");
                        var TempFileFolder = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + PayeeCode + "_" + FileName;
                        System.IO.File.WriteAllBytes(TempFileFolder, bytedata); // Save File
                        //Attaching FileName because same filename could be sent from Alteryx for different payees
                        FilesToBezipped.Add(PayeeCode + "_" + FileName);
                    }
                }

                if (data.Count() > 0)
                {
                    FilesToBezipped = FilesToBezipped.Select(p => new { CompleteFileName = (ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip" + "/" + p) }).Select(p => p.CompleteFileName).ToList();
                    var ZippedData = ZipHelper.ZipFilesToByteArray(FilesToBezipped, System.IO.Packaging.CompressionOption.Normal);
                    foreach (var file in FilesToBezipped)
                    {
                        System.IO.File.Delete(file);
                    }
                    return File(ZippedData, "application/zip", "ExportPayPayeeDocuments_" + RTPV.PaymentBatchNo + ".zip");
                }

                TempData["Message"] = "There is no PayeeDocument available";

            }
            catch (Exception ex)
            {
            }
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);

        }
    }
}