//Code Review for this file (from security perspective) done

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LRawDataController : Controller// PrimaryController//RS with reference to :-RK R2.3 17112018 made this change (comment URL Tempring) so that review can open in new tab
    {
        ILRawDataRestClient RestClient = new LRawDataRestClient();
        ILBatchesRestClient LBRC = new LBatchesRestClient();
        IGenericGridRestClient GGRC = new GenericGridRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        string LoggedInUserId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);
        // GET: LRawData
        //This method will return rows from LRawData table depending upon the SOSBatchNumber passed
        [ControllerActionFilter]
        public ActionResult Index(int TransactionId, int WFConfigId)
        {
           // Get ActionItems to be displayed
            var BatchDetails = LBRC.GetById(TransactionId);
            ViewBag.ActionItems = GGRC.GetSecondaryFormButtons(WFConfigId, LoggedInRoleId, LoggedInUserId, BatchDetails.Id);
            //Pass list of columns to be displayed in grid 
            ILCompanySpecificRawDataColumnsRestClient LCSRDC = new LCompanySpecificRawDataColumnsRestClient();
            ViewBag.RawDataColumnList = LCSRDC.GetRawDataColumnsByRawDataTableId(BatchDetails.LbRawDataTableId.Value).ToList();

            //Pass the batch level coments to be displayed on the view
            ViewBag.Comment = LBRC.GetByBatchNumber(BatchDetails.LbBatchNumber).WFComments;

            //Pass SOSBatchNumber to view for further use
            ViewBag.SOSBatchNumber = BatchDetails.LbBatchNumber;
            ViewBag.RawDataTableId = BatchDetails.LbRawDataTableId;
            ViewBag.TransactionId = TransactionId;
            ViewBag.WFConfigId = WFConfigId;
            var xx = new List<LRawDataRowCountsViewModel>();
            try
            {
                //Pass status wise row counts to view to be displayed on tab headers
                ILRawDataRestClient LRDRC = new LRawDataRestClient();
                xx = LRDRC.GetRawDataCounts(CompanyId, BatchDetails.LbBatchNumber).ToList();
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                return RedirectToAction("Index", "GenericGrid");
            }
            //These lines are added by Shubham to remove error generated in view in case of any of the three types are not found in database.
            //will replace once better method is found
            if (xx.Where(p => p.Status == null).Count() > 0)
            {
                xx.Remove(xx.Single(s => s.Status == null));
                xx.Add(new LRawDataRowCountsViewModel { Status = "NA", RowCounts = 0 });
            }
            else if (xx.Where(p => p.Status == "NA").Count() == 0)
            {
                xx.Add(new LRawDataRowCountsViewModel { Status = "NA", RowCounts = 0 });
            }
            if (xx.Where(p => p.Status == "Exclusion").Count() == 0)
            {
                xx.Add(new LRawDataRowCountsViewModel { Status = "Exclusion", RowCounts = 0 });
            }
            if (xx.Where(p => p.Status == "Error").Count() == 0)
            {
                xx.Add(new LRawDataRowCountsViewModel { Status = "Error", RowCounts = 0 });
            }
            ViewBag.StatusWiseRowCounts = xx;
            //Set the title of session
            System.Web.HttpContext.Current.Session["Title"] = "Manage Raw Data";
            
            return View();
        }

        //Attach Docs
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase[] File1, int TransactionId, int WFConfigId)
        {
            //to check if user has attached doc or not
            if (File1[0] != null)
            {
                var Files = Globals.AttachSupportingDocs(File1, "RawData");
                var FileName = Files.FileName;
                var FilePath = Files.FilePath;
                ILSupportingDocumentsRestClient LSDRC = new LSupportingDocumentsRestClient();
                LSDRC.AddSupportingDocs("RawData", TransactionId, FileName, FilePath, LoggedInUserId);
            }
            return RedirectToAction("Index", new { TransactionId = TransactionId, WFConfigId = WFConfigId });
        }

        //method to get list of Raw Data on Index Page
        //Note : The variables pagesize and pagenum are default variables for jqxgrid . Do not change the naming of these otherwise paging in jqxgrid may not work.
        [ControllerActionFilter]
        public JsonResult GetLRawData(int SOSBatchNumber, int RawDataTableId, string Status,int pagesize, int pagenum)
        {
            try
            {
                var qry = Request.QueryString;
                //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
                var FilterQuery = Globals.BuildQuery(qry);
                TempData["FilterQuery"] = FilterQuery;
                int Page = Convert.ToInt32(System.Web.HttpContext.Current.Session["RawDataPage"]);
                var ApiData = RestClient.GetLRawDataForGrid(CompanyId, SOSBatchNumber, pagenum, pagesize, RawDataTableId, Status, FilterQuery);
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        //method to save exclusions on LRaw data page and update this in LRawData
        [ControllerActionFilter]
        public ActionResult SaveExclusions(string[] ExcludedData,int TransactionId,int WFConfigId,int RawDataTableId,int SOSBatchNumber)
        {
            //TODO for Shubham - J+2 approach is not the correct way to  do this. Due to time constraint curently leting it go but needs to be changed.
           ILRawDataRestClient LRDRC=new LRawDataRestClient();
            string[] s = ExcludedData[0].Split(',');
            for (int j = 0; j < s.Length; j=j+2)
            {
                var Id = Convert.ToInt32(s[j]);
                if (!string.IsNullOrEmpty(s[j]))
                {
                    LRDRC.UpdateStatus(Id,s[j+1], "Exclusion",CompanyId,RawDataTableId,SOSBatchNumber);
                }
            }
            return RedirectToAction("Index", new {TransactionId=TransactionId,WFConfigId=WFConfigId});
        }

        //Include again in Data tab
        [ControllerActionFilter]
        public ActionResult SaveInclusions(string[] IncludedData, int TransactionId, int WFConfigId, int RawDataTableId,int SOSBatchNumber)
        {
            //TODO for Shubham - J + 2 approach is not appropriate and hence should be changed. Due to time constraint letting it go for the moment.
            ILRawDataRestClient LRDRC = new LRawDataRestClient();
            string[] s = IncludedData[0].Split(',');
            for (int j = 0; j < s.Length; j = j + 2)
            {
                var Id = Convert.ToInt32(s[j]);
                if (!string.IsNullOrEmpty(s[j]))
                {
                    LRDRC.UpdateStatus(Id, s[j + 1], string.Empty,CompanyId,RawDataTableId,SOSBatchNumber);
                }
            }
            return RedirectToAction("Index", new { TransactionId=TransactionId,WFConfigId=WFConfigId });
        }
        [ControllerActionFilter]
        public JsonResult GetRawDataColumnsForGrid(int RawDataTableId)
        {
            ILCompanySpecificRawDataColumnsRestClient LCSRDC = new LCompanySpecificRawDataColumnsRestClient();
            var RawDataColumnList = LCSRDC.GetRawDataColumnsByRawDataTableId(RawDataTableId).ToList();
            return Json(RawDataColumnList, JsonRequestBehavior.AllowGet);
        }
        
    }
}
