using Ionic.Zip;//zip files can be created using System.IO.Compression namespace, no need to add any extra library for only zipping purpose
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;
using static Vodafone_SOS_WebApp.Utilities.Globals;

namespace Vodafone_SOS_WebApp.Controllers
{
    [HandleCustomError]
    [SessionExpire]
    public class LCalcController : PrimaryController
    {
        ILCalcRestClient RestClient = new LCalcRestClient();
        ILCommonFunctionsRestClient LcfRestClient = new LCommonFunctionsRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string LoggedInRoleName = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        int LoggedInLUserId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LUserId"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        string CompanyName = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyName"]);
        IRProductCodesRestClient RPCRC = new RProductCodesRestClient();
        IRActiveTypesRestClient RATRC = new RActiveTypesRestClient();
        IRCommisionTypesRestClient RCTRC = new RCommissionTypesRestClient();
        ILBatchesRestClient LBRC = new LBatchesRestClient();
        IGenericGridRestClient GGRC = new GenericGridRestClient();

        [ControllerActionFilter]
        public ActionResult Index(int TransactionId, int WFConfigId)
        {
            //Get CompanySpecific Columns
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
            var CompanySpecificColumns = LCSCRC.GetCalculationsColumnsByCompanyIdForGrid(CompanyId).ToList();
            //Now populate the DataType column Special case for calculations part
            foreach (var CompanySpecificColumn in CompanySpecificColumns)
            {
                CompanySpecificColumn.DataType = CompanySpecificColumn.LcscDataType;//CompanySpecificColumn.ColumnName.Split('~').LastOrDefault();
                CompanySpecificColumn.ColumnName = CompanySpecificColumn.ColumnName;//CompanySpecificColumn.ColumnName.Split('~').FirstOrDefault();
                CompanySpecificColumn.LcscLabel = (string.IsNullOrEmpty(CompanySpecificColumn.LcscLabel)) ? CompanySpecificColumn.ColumnName.Replace("X", "") : CompanySpecificColumn.LcscLabel;
            }
            //
            ViewBag.CompSpecificColumn = CompanySpecificColumns;
            //Get ActionItems to be displayed 
            var BatchDetails = LBRC.GetById(TransactionId);
            ViewBag.ActionItems = GGRC.GetSecondaryFormButtons(WFConfigId, LoggedInRoleId, LoggedInUserId, BatchDetails.Id);

            //Pass the batch level coments to be displayed on the view
            ViewBag.Comment = BatchDetails.WFComments;
            ViewBag.TransactionId = TransactionId;
            ViewBag.WFConfigId = WFConfigId;
            //Pass SOSBatchNumber to view for further use
            ViewBag.SOSBatchNumber = BatchDetails.LbBatchNumber;

            //Pass status wise row counts to view to be displayed on tab headers
            ILCalcRestClient LRDRC = new LCalcRestClient();
            ViewBag.CalcRowCount = LRDRC.GetLCalcCounts(BatchDetails.LbBatchNumber, CompanyId).FirstOrDefault().RowCounts;
            ViewBag.SupportingDocuments = LSDRC.GetByEntityType("Calc", TransactionId);
            //Set the title of session
            System.Web.HttpContext.Current.Session["Title"] = "Manage Calculations";
            return View();
        }

        //Attach Docs
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase[] File1, int TransactionId, int WFConfigId)
        {
            //to check if user has attached doc or not
            if (File1[0] != null)
            {
                var Files = AttachSupportingDocs(File1, "Calc");
                var FileName = Files.FileName;
                var FilePath = Files.FilePath;
                ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
                LSDRC.AddSupportingDocs("Calc", TransactionId, FileName, FilePath, LoggedInUserId);
            }
            return RedirectToAction("Index", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
        }

        [HttpPost]
        public async Task<JsonResult> UploadAutoAttachment(int Id)
        {
            AttachedFilesViewModel FileDetails = new AttachedFilesViewModel();
            GenericGridRestClient RestClient = new GenericGridRestClient();
            string ReturnString = string.Empty;
            try
            {
                foreach (string file in Request.Files)
                {
                    //var fileContent = Request.Files[file];
                    //HttpPostedFileBase hpf = fileContent;
                    //AttachedFilesViewModel PayeeFiles = AttachPayeeFilesOneByOne(hpf, EmailID);

                    //if (string.IsNullOrEmpty(FileDetails.FileName))
                    //{
                    //    FileDetails.FileName = PayeeFiles.FileName;
                    //}
                    //else
                    //{
                    //    FileDetails.FileName = FileDetails.FileName + "," + PayeeFiles.FileName;
                    //}

                    //FileDetails.FilePath = PayeeFiles.FilePath;
                    //var fileContent = Request.Files[file];
                    //HttpPostedFileBase hpf = fileContent;
                    ////var Files = AttachSupportingDocs(hpf, "Calc");
                    //var Files = AttachCalFilesOneByOne(hpf, "Calc");
                    
                    //var FileName = Files.FileName;
                    //var FilePath = Files.FilePath;
                    //ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
                    //LSDRC.AddSupportingDocs("Calc", TransactionId, FileName, FilePath, LoggedInUserId);



                    var fileContent = Request.Files[file];
                    HttpPostedFileBase hpf = fileContent;
                    AttachedFilesViewModel PayeeFiles = AttachCalFilesOneByOne(hpf, "Calc");

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

                string Data = RestClient.UpdateAttachment(Convert.ToInt32(Id), LoggedInUserId, FileDetails.FileName, FileDetails.FilePath, "Calc");
                return Json(Data, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            //   return Json("File uploaded successfully");
        }

        public AttachedFilesViewModel AttachCalFilesOneByOne(HttpPostedFileBase files, string EmailID)
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

                // filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/Users/" + EmailID + "/SupportingDocuments");
                 filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["AttachedClaimDocumentPath"], System.Web.HttpContext.Current.Session["CompanyCode"] + "/Calc/SupportingDocuments");

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

        //method to get list of Calc on Index Page
        //Note : The variables pagesize and pagenum are default variables for jqxgrid . Do not change the naming of these otherwise paging in jqxgrid may not work.
        [ControllerActionFilter]
        public JsonResult GetLCalc(int SOSBatchNumber, string Status, string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            if (sortdatafield == null)
                sortdatafield = string.Empty;
            if (sortorder == null)
            {
                sortorder = string.Empty;
            }
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            TempData["FilterQuery"] = FilterQuery;
            // int Page = Convert.ToInt32(System.Web.HttpContext.Current.Session["RawDataPage"]);
            var ApiData = RestClient.GetLCalcForGrid(SOSBatchNumber, pagesize, pagenum, sortdatafield, sortorder, FilterQuery, CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        private SelectList GetProductCode()
        {
            var ProductCode = RPCRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ProductCode, "RpcProductCode", "RpcProductCode");
            return x;
        }

        private SelectList GetActiveType()
        {
            var ActiveType = RATRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(ActiveType, "RatName", "RatName");
            return x;
        }

        private SelectList GetCommissionType()
        {
            var CommissionType = RCTRC.GetDropDownDataByCompanyId(CompanyId);
            var x = new SelectList(CommissionType, "RctName", "RctName");
            return x;
        }

        private SelectList GetSource()
        {
            string[] Source = { "Alteryx", "Manual" };
            var x = new SelectList(Source);
            return x;
        }

        private SelectList GetPrimaryChannel()
        {
            string[] PrimaryChannel = { "Direct", "InDirect" };
            var x = new SelectList(PrimaryChannel);
            return x;
        }

        //this method is defined to display view for reports in Calculations
        [ControllerActionFilter]
        // [ValidateAntiForgeryToken]
        public ActionResult GetCalcReports(LCalcForReportsViewModel LCRVM)
        {
            //Get CompanySpecific Columns
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var CompanySpecificColumns = LCSCRC.GetCalculationsColumnsByCompanyIdForGrid(CompanyId).ToList();
            //Now populate the DataType column Special case for calculations part
            foreach (var CompanySpecificColumn in CompanySpecificColumns)
            {
                CompanySpecificColumn.DataType = CompanySpecificColumn.LcscDataType;//CompanySpecificColumn.ColumnName.Split('~').LastOrDefault();
                CompanySpecificColumn.ColumnName = CompanySpecificColumn.ColumnName;//CompanySpecificColumn.ColumnName.Split('~').FirstOrDefault();
                CompanySpecificColumn.LcscLabel = (string.IsNullOrEmpty(CompanySpecificColumn.LcscLabel)) ? CompanySpecificColumn.ColumnName.Replace("X", "") : CompanySpecificColumn.LcscLabel;
            }
            ViewBag.CompSpecificColumn = CompanySpecificColumns;
            ViewBag.PayeeId = LCRVM.PayeeId;
            ViewBag.PrimaryChannel = GetPrimaryChannel();
            ViewBag.Source = GetSource();
            ViewBag.CommissionType = GetCommissionType();
            ViewBag.ActivityType = GetActiveType();
            ViewBag.ProductCode = GetProductCode();
            ViewBag.SelectionMode = "checkbox";//used int the partial view to decide whether payee grid will have single selection or multiple
            System.Web.HttpContext.Current.Session["Title"] = "Manage Calculations";
            return View(LCRVM);
        }

        [ControllerActionFilter]
        public JsonResult GetCalcReportsCountsJson(LCalcForReportsViewModel LCRVM, string CommissionPeriod)
        {
            LCRVM.PageNumber = 0;
            LCRVM.PageSize = 99999;
            LCRVM.CompanyCode = CompanyCode;
            //if (LCRVM.PayeeList == null)
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(string.Empty,false);
            //    LCRVM.PayeeList = string.Join(",", PayeeData.Select(p => p.Id));
            //}
            //else
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(LCRVM.PayeeList,false);
            //    var ChildPayeeList = string.Join(",", PayeeData.Select(p => p.Id));
            //    if (!string.IsNullOrEmpty(ChildPayeeList))
            //        LCRVM.PayeeList = LCRVM.PayeeList + "," + ChildPayeeList;
            //}
            var CalcReportsCount = RestClient.GetLCalcForReportCounts(LCRVM, CommissionPeriod);
            return Json(CalcReportsCount, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetCalcReportsJson(string CommissionPeriod, string ProductCode, string CommissionType, string ActivityType, string Source, string PrimaryChannel, string PayeeId, string MinIMEI, string MaxIMEI, string MinBAN, string MaxBAN, string MinContractDuration, string MaxContractDuration, string MinCommissionAmount, string MaxCommissionAmount, string FromOrderDate, string ToOrderDate, string FromConnectionDate, string ToConnectionDate, string FromTerminationDate, string ToTerminationDate, string MinSubscriberNumber, string MaxSubscriberNumber, string sortdatafield, string sortorder, int pagesize, int pagenum)//, string sortdatafield, string sortorder, int pagesize, int pagenum
        {
            //if(string.IsNullOrEmpty(PayeeList))
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(string.Empty,false);
            //    PayeeList = string.Join(",",PayeeData.Select(p=>p.Id));
            //}
            //else
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(PayeeList,false);
            //    var ChildPayeeList = string.Join(",", PayeeData.Select(p => p.Id));
            //    if (!string.IsNullOrEmpty(ChildPayeeList))
            //        PayeeList = PayeeList + "," + ChildPayeeList;
            //    //PayeeList = string.Join(",", Globals.GetPayeeList(PayeeList).Select(p => p.Id));
            //}
            var CalcReports = RestClient.GetLCalcForReports(ProductCode, CommissionType, ActivityType, "", Source, PrimaryChannel, PayeeId, MinIMEI, MaxIMEI, MinBAN, MaxBAN, MinContractDuration, MaxContractDuration, MinCommissionAmount, MaxCommissionAmount, FromOrderDate, ToOrderDate, FromConnectionDate, ToConnectionDate, FromTerminationDate, ToTerminationDate, sortdatafield, sortorder, pagesize, pagenum, CompanyCode, MinSubscriberNumber, MaxSubscriberNumber, CommissionPeriod);
            return Json(CalcReports, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]//RK added during code review
        // [ControllerActionFilter]
        public ActionResult DownloadCalcReportsJson(LCalcForReportsViewModel LCRVM, string CommissionPeriod)
        {
            LCRVM.PageNumber = 0;
            LCRVM.PageSize = Int32.MaxValue;//to get All records in the file
            LCRVM.CompanyCode = CompanyCode;
            //Get Payee based on Global Logic to get Payees based on role
            //if (LCRVM.PayeeList == null)
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(string.Empty,false);
            //    LCRVM.PayeeList = string.Join(",",PayeeData.Select(p => p.Id));
            //}
            //else
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(LCRVM.PayeeList,false);
            //   var ChildPayeeList = string.Join(",", PayeeData.Select(p => p.Id));
            //    if (!string.IsNullOrEmpty(ChildPayeeList))
            //        LCRVM.PayeeList = LCRVM.PayeeList + "," + ChildPayeeList;
            //}
            string result = RestClient.DownloadLCalcForReports(LCRVM, LoggedInUserName, CommissionPeriod);
            //NOTE:-Refreshing Directory so that web server can see the file otherwise it gives a no file found message
            Thread.Sleep(5000);
            var FilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip";
            DirectoryInfo dir = new DirectoryInfo(FilePath);
            dir.Refresh();

            var FileName = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + result;

            if (System.IO.File.Exists(FileName))
            {
                return File(FileName, "application/octet-stream", result);//application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
            }
            TempData["Error"] = "No Data found at " + FileName;
            return RedirectToAction("GetCalcReports");
        }


        //Changes by SG - For Payee Calc Screen
        [ControllerActionFilter]
        public ActionResult PayeeCalc(string CommissionPeriod, string PortfolioList, bool? LoadCalcGrid)
        {
            if (!LoadCalcGrid.HasValue)
                LoadCalcGrid = false;
            ViewBag.LoadCalcGrid = LoadCalcGrid;
            // ViewBag.PayeeId = Globals.GetPayeeCode();
            // ViewBag.CommissionPeriodId = GetCommissionPeriod();
            //Get CompanySpecific Columns
            ILCompanySpecificColumnsRestClient LCSCRC = new LCompanySpecificColumnsRestClient();
            var CompanySpecificColumns = LCSCRC.GetCalculationsColumnsByCompanyIdForGrid(CompanyId).ToList();
            //Now populate the DataType column Special case for calculations part
            foreach (var CompanySpecificColumn in CompanySpecificColumns)
            {
                CompanySpecificColumn.DataType = CompanySpecificColumn.LcscDataType;//CompanySpecificColumn.ColumnName.Split('~').LastOrDefault();
                CompanySpecificColumn.ColumnName = CompanySpecificColumn.ColumnName;//CompanySpecificColumn.ColumnName.Split('~').FirstOrDefault();
                CompanySpecificColumn.LcscLabel = (string.IsNullOrEmpty(CompanySpecificColumn.LcscLabel)) ? CompanySpecificColumn.ColumnName.Replace("X", "") : CompanySpecificColumn.LcscLabel;
            }
            ViewBag.CompSpecificColumn = CompanySpecificColumns;
            var ComPeriod = "";
            if (!string.IsNullOrEmpty(CommissionPeriod))
            {
                ComPeriod = CommissionPeriod;
            }
            ILBatchesRestClient BRC = new LBatchesRestClient();
            if (PortfolioList == null)
                PortfolioList = string.Empty;
            var Batches = BRC.GetByCommPeriodIdList(ComPeriod, CompanyId, PortfolioList, LoggedInRoleId, LoggedInUserId, LoggedInRoleName).Select(p => new { p.LbBatchNumber, FullName = p.LbBatchName + " (" + p.LbBatchNumber + ")" });
            ViewBag.BatchNos = new SelectList(Batches, "LbBatchNumber", "FullName");
            ViewBag.Status = GetStatus();
            ViewBag.SelectionMode = "checkbox";//used int the partial view to decide whether payee grid will have single selection or multiple
            //ViewBag.PayeeAll = PayeeAll;
            //ViewBag.PortfolioAll = PortfolioAll;
            System.Web.HttpContext.Current.Session["Title"] = "Manage Calculations review";

            //This variable gets the logged in user role from session
            var role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
            var CurrentUserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
            //If the user role is payee then check whether payee is authorized to enter claim or not 
            //This variable is defined to to pass information to view whether user is authorized to enter claim or not
            bool CanRaiseClaim = false;
            ViewBag.CanRaiseClaim = CanRaiseClaim;
            ILPayeesRestClient LPRC = new LPayeesRestClient();
            switch (role)
            {
                case "Payee":
                    CanRaiseClaim = LPRC.CanRaiseClaims(CurrentUserId);
                    ViewBag.CanRaiseClaim = CanRaiseClaim;
                    break;
                case "Sales Operations":
                    //Sales Operations role can always raise the claim
                    CanRaiseClaim = true;
                    ViewBag.CanRaiseClaim = CanRaiseClaim;
                    break;
                case "Channel Manager":
                    CanRaiseClaim = LPRC.CanRaiseClaims(CurrentUserId);
                    ViewBag.CanRaiseClaim = CanRaiseClaim;
                    break;
                case "Manager":
                    ViewBag.CanRaiseClaim = CanRaiseClaim;
                    break;
                case "System Analyst":
                    ViewBag.CanRaiseClaim = CanRaiseClaim;
                    break;

            }
            SaveSelectedPortfolio(CommissionPeriod);
            return View();
        }

        public JsonResult SaveSelectedPortfolio(string periods)
        {
            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
           // var ApiData = CPRC.GetById(Convert.ToInt16(periods));
            //var WFName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            SessionData ObjSession = new SessionData(LoggedInUserId, System.Web.HttpContext.Current.Session.SessionID);
            //var jsonSerialiser = new JavaScriptSerializer();
            GGRC.SaveUserPreferenceData(LoggedInUserId, "CalcReview", "Period", "FieldValue","0", periods, ObjSession._UserSessionID);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectedPortfolioFromUserPreference()
        {
            string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            SessionData ObjSession = new SessionData(LoggedInUserId, System.Web.HttpContext.Current.Session.SessionID);
            var ApiData = GGRC.GetUserPreferenceData(LoggedInUserId, "CalcReview", "Period", "FieldValue", "0", ObjSession._UserSessionID);
            return Json(ApiData.AsEnumerable(), JsonRequestBehavior.AllowGet);
        }
        //
        //[ControllerActionFilter]
        //public JsonResult GetPayeeList(string PortfolioList)
        //{
        //    if (PortfolioList == null)
        //        PortfolioList = string.Empty;
        //    var ApiData = Globals.GetPayeeList(PortfolioList,false);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}
        //Common dropdown to get list of commission period
       // [ControllerActionFilter]
        //changing the return type as we need multi selection box in Payee Calculations
        //        private SelectList GetCommissionPeriod()
        public JsonResult GetCommissionPeriod()
        {
            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
            var ApiData = CPRC.GetByCompanyId(CompanyId);
            //  var x = new SelectList(ApiData, "Id", "LcpStatus");
            //return x;
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]//RK added during code review
        public JsonResult GetPayeeCalcCounts(string PayeeId, string CommissionPeriodId, string TabName, string PortfolioList)
        {
            int GridCounts = 0;
            //Get Channel List if PortfolioList is null
            if (string.IsNullOrEmpty(PortfolioList))
            {
                ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
                PortfolioList = string.Join(",", LPORC.GetByUserId(LoggedInUserId, LoggedInRoleId).Select(p => p.Id));
            }
            //if Payee is not selected then get All Payees from Dropdown
            //if (string.IsNullOrEmpty(PayeeId))
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(PortfolioList,false);
            //   var PayeeList = PayeeData.Select(p => p.Id);
            //    PayeeId = string.Join(",", PayeeList);
            //}
            switch (TabName)
            {
                case "Prelims":
                    GridCounts = RestClient.GetCalcForPayeeCounts(PayeeId, CompanyId, CommissionPeriodId, "Prelim", PortfolioList, LoggedInLUserId, LoggedInRoleName, LoggedInUserId);
                    break;
                case "Approved":
                    GridCounts = RestClient.GetCalcForPayeeCounts(PayeeId, CompanyId, CommissionPeriodId, "Completed", PortfolioList, LoggedInLUserId, LoggedInRoleName, LoggedInUserId);
                    break;

            }
            ViewBag.CPId = CommissionPeriodId;
            ViewBag.PId = PayeeId;
            return Json(GridCounts, JsonRequestBehavior.AllowGet);
        }


        [ControllerActionFilter]
        public JsonResult GetPayeeCalcReports(string PayeeId, string CommissionPeriodId, string TabName, string sortdatafield, string sortorder, int pagesize, int pagenum, string PortfolioList)
        {
            //changes for filtering,sorting,paging

            //Get Channel List if PortfolioList is null
            if (string.IsNullOrEmpty(PortfolioList))
            {
                ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
                PortfolioList = string.Join(",", LPORC.GetByUserId(LoggedInUserId, LoggedInRoleId).Select(p => p.Id));
            }
            //if Payee is not selected then get All Payees from Dropdown
            //if (string.IsNullOrEmpty(PayeeId))
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(PortfolioList, false);
            //    var PayeeList = PayeeData.Select(p => p.Id);
            //    PayeeId = string.Join(",", PayeeList);
            //}
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            IEnumerable<dynamic> ApiData = new List<dynamic>();
            switch (TabName)
            {
                case "Prelims":
                    ApiData = RestClient.GetCalcForPayeeReports(PayeeId, CompanyId, CommissionPeriodId, "Prelim", pagesize, pagenum, sortdatafield, sortorder, FilterQuery, PortfolioList, LoggedInRoleName, LoggedInLUserId, LoggedInUserId);
                    //ILBatchesRestClient BRC = new LBatchesRestClient();
                    //var Batches = BRC.GetByCommPeriodIdList("", CompanyId).Select(p => new { p.LbBatchNumber, FullName = p.LbBatchName + "(" + p.LbBatchNumber + ")" });
                    //TempData["BatchNos"] = new SelectList(Batches, "LbBatchNumber", "FullName");
                    break;
                case "Approved":
                    ApiData = RestClient.GetCalcForPayeeReports(PayeeId, CompanyId, CommissionPeriodId, "Completed", pagesize, pagenum, sortdatafield, sortorder, FilterQuery, PortfolioList, LoggedInRoleName, LoggedInLUserId, LoggedInUserId);
                    break;
                    //separate method is created for Summary Tab. So commenting the below code.
                    //case "Summary":
                    //    ApiData = RestClient.GetCalcForPayeeReports(PayeeId, CompanyId, CommissionPeriodId, "Summary", pagesize, pagenum, sortdatafield, sortorder, FilterQuery);
                    //    break;

            }
            //ViewBag.CPId = CommissionPeriodId;
            //ViewBag.PId = PayeeId;
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetComments(int BatchNo)
        {
            ILBatchesRestClient BRC = new LBatchesRestClient();
            var x = BRC.GetByBatchNumber(BatchNo);
            string result = x.WFComments;
            if (result == null || result.Equals(""))
            {
                result = "No Previous Comments";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public JsonResult GetGridSummary(string PayeeId, string CommissionPeriodId, bool BatchNochecked, bool Sourcechecked, bool CommTypechecked, bool CommPeriodchecked, bool Payeechecked, string Status, bool MSISDN, string sortdatafield, string sortorder, int pagesize, int pagenum)
        {
            if (Status == null)
            {
                Status = "";
            }
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            IEnumerable<dynamic> ApiData = RestClient.GetSummaryForPayee(PayeeId, CompanyId, CommissionPeriodId, BatchNochecked, Sourcechecked, CommTypechecked, CommPeriodchecked, Payeechecked, Status, LoggedInRoleName, LoggedInUserId, MSISDN, sortdatafield, sortorder, pagesize, pagenum, FilterQuery);
            ViewBag.Status = GetStatus();
            ViewBag.CPId = CommissionPeriodId;
            ViewBag.PId = PayeeId;
            // ApiData = ApiData.Take(5000);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        // [ControllerActionFilter]
        public JsonResult GetGridSummaryCount(string PayeeId, string CommissionPeriodId, bool BatchNochecked, bool Sourcechecked, bool CommTypechecked, bool CommPeriodchecked, bool Payeechecked, string Status, bool MSISDN)
        {
            if (Status == null)
            {
                Status = "";
            }
            var qry = Request.QueryString;
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiDataCount = RestClient.GetSummaryForPayeeCount(PayeeId, CompanyId, CommissionPeriodId, BatchNochecked, Sourcechecked, CommTypechecked, CommPeriodchecked, Payeechecked, Status, LoggedInRoleName, LoggedInUserId, MSISDN);
            ViewBag.Status = GetStatus();
            ViewBag.CPId = CommissionPeriodId;
            ViewBag.PId = PayeeId;
            // ApiData = ApiData.Take(5000);
            return Json(ApiDataCount, JsonRequestBehavior.AllowGet);
        }

        private SelectList GetStatus()
        {
            //string[] Status = { "All", "Prelim", "Approved" };//RK Removed "All" as part of R2.1.4 release
            string[] Status = { "Prelim", "Approved" };
            var x = new SelectList(Status, "Prelim");
            return x;
        }


        private int AttachFileToPayee(HttpPostedFileBase FileUpload)
        {
            ILPayeesRestClient PRC = new LPayeesRestClient();
            string FileName = null;
            var filePath = "";
            var file = FileUpload;
            LAttachmentViewModel AVM = new LAttachmentViewModel();
            if (file != null)
            {
                var fileLocation = "";
                string fileExtension = System.IO.Path.GetExtension(FileUpload.FileName);
                string name = System.IO.Path.GetFileNameWithoutExtension(FileUpload.FileName);
                FileName = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                //Opco/PayeeCalcReviewAttachments/{LoggedInUserName}
                filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/PayeeCalcReviewAttachments/" + LoggedInUserName);
                fileLocation = filePath + "/" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                //check if directory exists or not. iIf notcreate that directory
                bool exists = System.IO.Directory.Exists(filePath);
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                file.SaveAs(fileLocation);
            }

            AVM.LaFileName = FileName;
            AVM.LaFilePath = filePath;
            AVM.LaEntityType = "EntityType=PayeeCalcReviewUser";
            AVM.LaEntityId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LUserId"]); ;
            AVM.LaType = "Payee Calc";
            AVM.LaCreatedDateTime = DateTime.UtcNow;
            AVM.LaUpdatedDateTime = DateTime.UtcNow;
            AVM.LaCreatedById = LoggedInUserId;
            AVM.LaUpdatedById = LoggedInUserId;
            ILAttachmentsRestClient attachmentClient = new LAttachmentsRestClient();
            AVM = attachmentClient.Add(AVM);
            return AVM.Id;
            // RestClient.UpdateAttachment(LCalc.Id,AVM.Id);

        }


        [ControllerActionFilter]
        public ActionResult AcceptAttach(string SelectedData, string CommissionPeriodIdList)
        {
            /*
                   string[] s = SelectedData[0].Split(',');
                 //  int AttachmentId = 0;
                   for (int j = 0; j < s.Length; j = j + 2)
                   {
                       var Id = Convert.ToInt32(s[j]);
                       var IsAccepted = Convert.ToBoolean(s[j + 1]);
                       var AcceptedBy = LoggedInUserId;
                       DateTime AcceptedAt = DateTime.UtcNow;
                       RestClient.UpdateAcceptAttachment(Id,AttachmentId,  AcceptedBy,  AcceptedAt);

                   }*/
            var AcceptedBy = LoggedInUserId;
            DateTime AcceptedAt = DateTime.UtcNow;
            RestClient.UpdateAcceptAttachment(SelectedData, AcceptedBy, AcceptedAt, null, CompanyId);
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }
        //SG - 9 Jun 2020 - code is not being used
        //[ControllerActionFilter]
        //public ActionResult DownloadDashboardFile(string PayeeId, string CommissionPeriodId)
        //{
        //    string FileName = "";
        //    string FileExtension = "";
        //    string FileType = "";
        //    //int PId = Convert.ToInt32(PayeeId);
        //    //ILPayeesRestClient PRC = new LPayeesRestClient();
        //    //var Payee = PRC.GetPayeeDetailsById(PId);
        //    ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
        //    int CPId = Convert.ToInt32(CommissionPeriodId);
        //    var CommissionPeriodDetails = CPRC.GetById(CPId);

        //    string DashboardFilePath = System.Configuration.ConfigurationManager.AppSettings["S3_A2SCommomFilePath"].ToString()
        //        + "/" + CompanyCode.ToLower() + "/SalesReports";
        //    string DashboardFileName = LoggedInUserName + "_" + CommissionPeriodDetails.LcpPeriodName + "_*.*";
        //    bool exists = System.IO.Directory.Exists(DashboardFilePath);
        //    if (exists)
        //    {
        //        var DashboardFileList = Directory.GetFiles(DashboardFilePath, DashboardFileName, SearchOption.AllDirectories).ToList();

        //        var IsSingleFile = true;
        //        using (ZipFile zip = new ZipFile())
        //        {
        //            zip.AlternateEncodingUsage = ZipOption.AsNecessary;

        //            for (var i = 0; i < DashboardFileList.Count(); i++)
        //            {
        //                if (DashboardFileList.Count() == 1)
        //                {

        //                    var xx = DashboardFileList[i].Split('.');
        //                    FileExtension = xx.LastOrDefault();
        //                    FileType = Globals.GetFileContentType(FileExtension);
        //                    FileName = DashboardFileList[i];
        //                    IsSingleFile = true;
        //                }
        //                else
        //                {
        //                    IsSingleFile = false;
        //                    zip.AddFile(DashboardFileList[i], "");
        //                }

        //            }

        //            //Download file if not attachments are present and make a zip if multiple files are present
        //            if (IsSingleFile)
        //            {

        //                if (System.IO.File.Exists(FileName))//if file exists then export it
        //                {
        //                    var FileNameWithoutPath = FileName.Split('/');
        //                    var zz = FileNameWithoutPath.LastOrDefault();
        //                    return File(FileName, FileType, zz);
        //                }
        //            }
        //            else
        //            {
        //                //delete previous file if present in temp
        //                if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + "Dashboard.zip"))
        //                {
        //                    System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + "Dashboard.zip");
        //                }
        //                zip.Save(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + "Dashboard.zip");
        //                return File(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + "Dashboard.zip", "application/zip/Payees", "SalesReport_" + CommissionPeriodDetails.LcpPeriodName + ".zip");
        //            }
        //        }
        //    }
        //    TempData["Message"] = "There is no Dashboard available";
        //    return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        //}

        [ControllerActionFilter]
        public ActionResult UpdateComments(string SelectedData, string PId, string CPId, string Comments)
        {
            try
            {
                RestClient.UpdateComments(SelectedData, Comments, null, LoggedInUserId, CompanyId);
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            catch (Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(ex.Data["RedirectToUrl"] as string);
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
        public JsonResult UpdateBatchComments(int SOSBatchNumber, string BatchLevelComments)
        {
            ILBatchesRestClient BRC = new LBatchesRestClient();
            BRC.UpdateBatchLevelComments(SOSBatchNumber, BatchLevelComments, LoggedInUserName, LoggedInUserId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [ControllerActionFilter]
        public ActionResult AddAttachmentsToAll(string PayeeIdList, string CommissionPeriodIdList, string PortfolioArray, HttpPostedFileBase FileUploadAll)
        {
            if (FileUploadAll != null)
            {
                //if Payee is not selected then get All Payees from Dropdown
                //if (string.IsNullOrEmpty(PayeeIdList))
                //{
                //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(string.Empty, false);
                //    var PayeeList = PayeeData.Select(p => p.Id);
                //    PayeeIdList = string.Join(",", PayeeList);
                //}
                var AttachmentId = AttachFileToPayee(FileUploadAll);
                RestClient.UpdatePayeeAttachmentForAll(PayeeIdList, CommissionPeriodIdList, PortfolioArray, AttachmentId, null, LoggedInUserId, CompanyId, LoggedInLUserId, LoggedInRoleName);
            }
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }

        [ControllerActionFilter]
        public ActionResult UpdateAcceptanceToAll(string PayeeId, string CommissionPeriodId, string PortfolioList)
        {
            try
            {
                //Get Channel List if PortfolioList is null
                if (string.IsNullOrEmpty(PortfolioList))
                {
                    ILPortfoliosRestClient LPORC = new LPortfoliosRestClient();
                    PortfolioList = string.Join(",", LPORC.GetByUserId(LoggedInUserId, LoggedInRoleId).Select(p => p.Id));
                }
                //if Payee is not selected then get All Payees from Dropdown
                //if (string.IsNullOrEmpty(PayeeId))
                //{
                //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(PortfolioList, false);
                //    var PayeeList = PayeeData.Select(p => p.Id);
                //    PayeeId = string.Join(",", PayeeList);
                //}
                RestClient.UpdateAcceptStatusForAll(CompanyId, PayeeId, CommissionPeriodId, PortfolioList, LoggedInUserId, DateTime.UtcNow, null, LoggedInLUserId, LoggedInRoleName, LoggedInUserId);
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            catch (Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(ex.Data["RedirectToUrl"] as string);
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
        public ActionResult DownloadPayeeCalcFile(string PayeeId, string CommissionPeriodId, int TabIndex, string PortfolioList)
        {
            //if Payee is not selected then get All Payees from Dropdown
            //if (string.IsNullOrEmpty(PayeeId))
            //{
            //    IEnumerable<LPayeeViewModel> PayeeData = Globals.GetPayeeList(PortfolioList,false);
            //    var PayeeList = PayeeData.Select(p => p.Id);
            //    PayeeId = string.Join(",", PayeeList);
            //}
            string FileName = "";
            switch (TabIndex)
            {
                case 0://Prelim
                    FileName = RestClient.DownloadCalcForPayeeReports(PayeeId, CompanyId, CommissionPeriodId, "Prelim", LoggedInUserName, CompanyCode, PortfolioList, LoggedInLUserId, LoggedInRoleName, LoggedInUserId);
                    break;
                case 1://Approved
                    FileName = RestClient.DownloadCalcForPayeeReports(PayeeId, CompanyId, CommissionPeriodId, "Completed", LoggedInUserName, CompanyCode, PortfolioList, LoggedInLUserId, LoggedInRoleName, LoggedInUserId);
                    break;
            }
            Thread.Sleep(3000);//Sleep to add delay to avoid issues in getting file from S Drive
            var CompleteFileName = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + FileName;
            if (System.IO.File.Exists(CompleteFileName))
            {
                return File(CompleteFileName, "application/unknown", FileName);
            }
            else
            {
                TempData["Error"] = "No File/Data found";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
        }

        // [ControllerActionFilter]
        public ActionResult DownloadAttachment(int AttachmentId, string CommissionPeriodIdList)
        {
            ILAttachmentsRestClient ARC = new LAttachmentsRestClient();
            var Attachment = ARC.GetById(AttachmentId);
            var CompleteFileName = Attachment.LaFilePath + "/" + Attachment.LaFileName;
            if (System.IO.File.Exists(CompleteFileName))
            {
                return File(CompleteFileName, "application/unknown", Attachment.LaFileName);
            }
            TempData["Error"] = "No Data/File found";
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }

        //This method will add Attchment to Payee Calc
        [HttpPost]
        [ControllerActionFilter]
        public ActionResult AddAttachment(HttpPostedFileBase FileUpload, string FormData, string CommissionPeriodIdList)
        {
            try
            {
                if (FileUpload != null)
                {
                    var AttachmentId = AttachFileToPayee(FileUpload);
                    RestClient.UpdatePayeeAttachment(FormData, AttachmentId, null, LoggedInUserId, CompanyId);
                }
                TempData["Error"] = "Attachment added sucessfully";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            catch (Exception ex)
            {
                switch ((int)ex.Data["ErrorCode"])
                {
                    case (int)ExceptionType.Type1:
                        //redirect user to gneric error page
                        return Redirect(ex.Data["RedirectToUrl"] as string);
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


    }
}