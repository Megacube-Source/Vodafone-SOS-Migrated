//Code Review for this file (from security perspective) done

using CsvHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [HandleCustomError]
    [SessionExpire]
    public class ManualAdjustmentsController : Controller//PrimaryController//RK R2.3 17112018 made this change (comment URL Tempring) so that review can open in new tab
    {
        // GET: ManualAdjustments
        IManualAdjustmentsRestClient RestClient = new ManualAdjustmentsRestClient();
        ILCommissionPeriodsRestClient LCPRC = new LCommissionPeriodsRestClient();
        IGenericGridRestClient GGRC = new GenericGridRestClient();
        ILBatchesRestClient LBRC = new LBatchesRestClient();
        ILPortfoliosRestClient LPRC = new LPortfoliosRestClient();

        string Workflow = Convert.ToString(System.Web.HttpContext.Current.Session["Workflow"]);
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
        string LoggedInRoleId = Convert.ToString(System.Web.HttpContext.Current.Session["UserRoleId"]);
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string CurrentUserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
        string CurrentUserName = System.Web.HttpContext.Current.Session["UserName"].ToString();

        [ControllerActionFilter]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "GenericGrid");
        }

        //This method will upload Manual Adjustment from excel and add it to LCalc table
        [ControllerActionFilter]
        public ActionResult UploadManualAdjustment()
        {
            var WfName = TempData["WorkFlow"] as string;
            TempData["WorkFlow"] = WfName;
            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
            var ApiData = CPRC.GetByCompanyId(CompanyId);
            ViewBag.CommissionPeriod = new SelectList(ApiData, "LcpPeriodName", "LcpPeriodName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult UploadManualAdjustment(HttpPostedFileBase File1,HttpPostedFileBase[] FileUpload,ManualAdjustmentViewModel MAVM)
        {
            var AtachmentfilePath = "";
            string fileLocation = "";
            try
            {
                if (Request.Files["File1"].ContentLength > 0)
                {
                    // As directed by JS the file names would have date time stamp as suffix
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["File1"].FileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(Request.Files["File1"].FileName);
                    string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + fileExtension;
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        /*OLD: Manual Adjustment : S:\<opco>\ManualAdj\Upload
                         NEW: Manual Adjustment : S:\<opco>\ManualAdj\<period>\Upload*/
                        ////var filePath = ConfigurationManager.AppSettings["ManualAdjustmentDocumentPath"] + "/" + CompanyCode + "/ManualAdj/" + MAVM.CommissionPeriod + "/Upload";
                        ////fileLocation = string.Format("{0}/{1}", ConfigurationManager.AppSettings["ManualAdjustmentDocumentPath"] + "/" + CompanyCode + "/ManualAdj/" + MAVM.CommissionPeriod + "/Upload", FileNames);
                        ////if (!System.IO.Directory.Exists(filePath))
                        ////{
                        ////    System.IO.Directory.CreateDirectory(filePath);
                        ////}
                        ////Request.Files["File1"].SaveAs(fileLocation);

                       // var filePath = ConfigurationManager.AppSettings["SOSBucketRootFolder"] + "/" + CompanyCode + "/ManualAdj/" + MAVM.CommissionPeriod + "/Upload";
                        var filePath =  CompanyCode + "/ManualAdj/" + MAVM.CommissionPeriod + "/Upload";
                        if (!Globals.FolderExistsInS3(filePath))
                        {
                            Globals.CreateFolderInS3(filePath);
                        }
                        Globals.UploadToS3(Request.Files["File1"].InputStream, FileNames, filePath );

                        var FileData = Globals.DownloadFromS3(FileNames, filePath + "/");
                        //var TempLocalFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/ManualAdj/" + MAVM.CommissionPeriod + "/Upload/" + FileNames;
                        //System.IO.File.WriteAllBytes(TempLocalFilePath, FileData); // Requires System.IO 

                        if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + CurrentUserName + "/forzip"))
                            System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + CurrentUserName + "/forzip");
                        var TempFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + CurrentUserName + "/forzip/" + FileNames;
                        System.IO.File.WriteAllBytes(TempFilePath, FileData);

                        //connection string
                        string excelConnectionString = string.Empty;
                        // This line is added to make a connection with the excel sheet saved  to read data from it
                        //excelConnectionString = string.Format(ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"], fileLocation);
                        excelConnectionString = string.Format(ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"], TempFilePath);
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();
                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }
                        string[] excelSheets = new string[dt.Rows.Count];
                        int t = 0;
                        //excel data is saved in temporary file here
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                        DataSet ds = new DataSet();
                        string query = "Select * from [LCalc$]";
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }
                        excelConnection.Close();//close oledb connection after filling datatable otherwise it will cause error in download

                        var ManualAdjustmentViewModellist =new List< UploadManualAdjustmentViewModel > ();
                        //After filling the data read from excel sheet in a data set this for loop reads the data row by row and checks for validation simultaneously
                        for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //Loop through view model to read all respective records 
                            var properties = typeof(UploadManualAdjustmentViewModel).GetProperties();
                            var model = new UploadManualAdjustmentViewModel();
                            foreach (var property in properties)
                            {
                                var propertyName = property.Name;
                                var PropertyType = property.PropertyType.Name;
                                if (
                                    property.PropertyType == typeof(Nullable<int>))
                                {
                                    PropertyType = "int";
                                }
                                else if (
                                   property.PropertyType == typeof(Nullable<DateTime>))
                                {
                                    PropertyType = "datetime";
                                }
                                else if (
                                  property.PropertyType == typeof(Nullable<decimal>))
                                {
                                    PropertyType = "decimal";
                                }
                                else
                                {
                                   PropertyType = property.PropertyType.Name;
                                }

                                if (!propertyName.Equals("ErrorMessage")&&!propertyName.Equals("XAdjustmenCode") && !propertyName.Equals("XSource") && !propertyName.Equals("XAlteryxTransactionNumber"))//As we do not want to read these properties from excel and assign them below
                                {
                                    if (ds.Tables[0].Columns.Contains(propertyName))
                                    {
                                        switch (PropertyType.ToLower())
                                        {
                                            case "string":
                                                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][propertyName].ToString()))
                                                {
                                                    property.SetValue(model, ds.Tables[0].Rows[i][propertyName].ToString());
                                                }
                                                break;
                                            case "int":
                                                // IsRequired=  Attribute.IsDefined(property, typeof(Required));
                                                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][propertyName].ToString()))
                                                {
                                                    property.SetValue(model, Convert.ToInt32(ds.Tables[0].Rows[i][propertyName].ToString()));
                                                }
                                                break;
                                            case "decimal":
                                                // IsRequired=  Attribute.IsDefined(property, typeof(Required));
                                                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][propertyName].ToString()))
                                                {
                                                    property.SetValue(model, Convert.ToDecimal(ds.Tables[0].Rows[i][propertyName].ToString()));
                                                }
                                                break;
                                            case "datetime":
                                                //IsRequired= Attribute.IsDefined(property, typeof(Required));
                                                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][propertyName].ToString()))
                                                {
                                                    //var TestData = ds.Tables[0].Rows[i][propertyName].ToString();
                                                    //var format = "MM/dd/yyyy";
                                                    //
                                                    try
                                                    {
                                                        //var dateTime = DateTime.ParseExact(TestData, format, CultureInfo.InvariantCulture);
                                                        property.SetValue(model, DateTime.ParseExact(ds.Tables[0].Rows[i][propertyName].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture));
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        try
                                                        {
                                                            property.SetValue(model, DateTime.ParseExact(ds.Tables[0].Rows[i][propertyName].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture));
                                                        }
                                                        catch (Exception ex1)
                                                        { }
                                                    }
                                                    // property.SetValue(model, Convert.ToDateTime(ds.Tables[0].Rows[i][propertyName].ToString()));
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                           // model.LcOpCoCode = CompanyCode;
                            model.XSource = "Manual";
                            ManualAdjustmentViewModellist.Add(model);
                        }

                        //add attachments
                        string Atachments = "";
                        foreach (HttpPostedFileBase files in FileUpload)
                        {
                            if (files != null)
                            {
                                var AtachmentfileLocation = "";
                                string AtachmentfileExtension = System.IO.Path.GetExtension(files.FileName);
                                string Atachmentname = System.IO.Path.GetFileNameWithoutExtension(files.FileName);

                                /*OLD: Manual Adj : S:\<opco>\ManualAdj\Attached
                                 NEW: Manual Adj : S:\<opco>\ManualAdj\<period>\SupportingDocuments*/
                                //AtachmentfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyCode + "/ManualAdj/"+ MAVM.CommissionPeriod + "/SupportingDocuments");
                                var AttachmentFileName = Atachmentname + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + AtachmentfileExtension;
                               // AtachmentfileLocation = AtachmentfilePath + "/" +AttachmentFileName ;//As disscussed with JS VG file name will have datetime stamp as suffix
                                                                                                                                      //check if directory exists or not. iIf notcreate that directory
                                //bool exists = System.IO.Directory.Exists(AtachmentfilePath);
                                //if (!exists)
                                //{
                                //    System.IO.Directory.CreateDirectory(AtachmentfilePath);
                                //}

                                //files.SaveAs(AtachmentfileLocation);

                                 AtachmentfilePath = CompanyCode + "/ManualAdj/" + MAVM.CommissionPeriod + "/SupportingDocuments";
                                if (!Globals.FolderExistsInS3(AtachmentfilePath))
                                {
                                    Globals.CreateFolderInS3(AtachmentfilePath);
                                }
                                Globals.UploadToS3(files.InputStream, AttachmentFileName, AtachmentfilePath);

                                if (string.IsNullOrEmpty(Atachments))
                                {
                                    Atachments = AttachmentFileName;
                                }
                                else
                                {
                                    Atachments = Atachments + "," + AttachmentFileName;
                                }
                            }
                        }
                        //Add attach files string to model
                       var ErrorList= RestClient.UploadManualAdjustments(ManualAdjustmentViewModellist, FileNames, MAVM.CommissionPeriod, CurrentUserId, MAVM.PortfolioList, Atachments,LoggedInRoleId,AtachmentfilePath,Workflow,CompanyId,MAVM.BatchName);
                      //If Error List is Empty Send back to Index Page
                      if(ErrorList !=null)
                        {
                            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
                            var ApiData = CPRC.GetByCompanyId(CompanyId);
                            ViewBag.CommissionPeriod = new SelectList(ApiData, "LcpPeriodName", "LcpPeriodName");
                          //  ViewBag.CommissionPeriod = new SelectList(CommissionPeriodList1, "LcpPeriodName", "LcpPeriodName");
                            for(var i=0;i<ErrorList.Count();i++)
                            {
                                
                                    ManualAdjustmentViewModellist.ElementAt(i).ErrorMessage = ErrorList.ElementAt(i);
                                
                            }
                            TempData["ManualAdjErrorGrid"] = ManualAdjustmentViewModellist;//.Where(p=>!string.IsNullOrEmpty(p.ErrorMessage));
                            ViewData["ErrorMessage"] = "Manual Adjustments could not be uploaded due to following Data errors. Please fix data and upload again";
                            return View();
                        }
                        //  BatchId = result.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                //var CommissionPeriodList1 = LCPRC.GetByCompanyIdStatus(CompanyId, "Open");
                //ViewBag.CommissionPeriod = new SelectList(CommissionPeriodList1, "LcpPeriodName", "LcpPeriodName");
                //ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];
                //return View();
                throw ex;
            }
            var WfName = TempData["WorkFlow"] as string;
            return RedirectToAction("Index", "GenericGrid",new { WorkFlow=WfName});
            //var CommissionPeriodList = LCPRC.GetByCompanyIdStatus(CompanyId, "Open");
            //ViewBag.CommissionPeriod = new SelectList(CommissionPeriodList, "LcpPeriodName", "LcpPeriodName");
            // return RedirectToAction("UpdateBaseTableWfStatus", "GenericGrid", new { WorkflowId = 1, TransactionId =BatchId, ActionName = "Approve" });//Approve from sales op
        }

        [ControllerActionFilter]
        public ActionResult Review(int TransactionId,int WFConfigId)
        {
            //Get BatchDetails
            ViewBag.ActionItems = GGRC.GetSecondaryFormButtons(WFConfigId, LoggedInRoleId, CurrentUserId, TransactionId);
            var BatchDetails = LBRC.GetById(TransactionId);
            ViewBag.TransactionId = TransactionId;
            ILCommissionPeriodsRestClient CPRC = new LCommissionPeriodsRestClient();
            var ApiData = CPRC.GetByCompanyId(CompanyId);
            ViewBag.CommissionPeriod = new SelectList(ApiData, "LcpPeriodName", "LcpPeriodName", BatchDetails.LbCommissionPeriod);
            //ViewBag.CommissionPeriod = new SelectList(CommissionPeriod, "LcpPeriodName", "LcpPeriodName",BatchDetails.LbCommissionPeriod);
            return View(new ManualAdjustmentViewModel { BatchName=BatchDetails.LbBatchName,CommissionPeriod=BatchDetails.LbCommissionPeriod});
        }

        [ControllerActionFilter]
        public JsonResult GetManualAdjustmentErrorGrid()
        {
            var ApiData = TempData["ManualAdjErrorGrid"] as List<UploadManualAdjustmentViewModel>;
            ApiData.RemoveAll(x => x.ErrorMessage == "");
            TempData["ManualAdjErrorGrid"] = ApiData;
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

        // [ControllerActionFilter]
        public ActionResult ExportUploadMannualAdjusmentErrorGrid() //JsonResult
        {

            var ApiData = TempData["ManualAdjErrorGrid"] as List<UploadManualAdjustmentViewModel>;
            ApiData.RemoveAll(x => x.ErrorMessage == "");
            TempData["ManualAdjErrorGrid"] = ApiData;

            // var ApiData = TempData["PayeeModelList"] as List<LPayeeViewModel>;
            // TempData["PayeeModelList"] = ApiData;
            var fileLocation = ConfigurationManager.AppSettings["PayeeDocumentPath"] + "/ExportMannualAdjusmentErrorList.csv";
            if (System.IO.File.Exists(fileLocation))
                System.IO.File.Delete(fileLocation);
            //This line adds temporary file with name ExportPayeeErrorList and it will be exported on sucess event of this function
            using (var TextWriter = new StreamWriter(fileLocation))
            using (var csv = new CsvWriter(TextWriter))
            {

                DataRow dr;
                DataTable dt = new DataTable();
                //DataColumn Company = new DataColumn("Company");
                //DataColumn PayeeName = new DataColumn("Payee Name");
                DataColumn PayeeCode = new DataColumn("Payee Code");
                //DataColumn ParentCode = new DataColumn("Parent Code");
                //DataColumn ParentName = new DataColumn("Parent Name");
                //DataColumn StartDate = new DataColumn("Start Date");
                //DataColumn EndDate = new DataColumn("End Date");
                DataColumn Error = new DataColumn("Error");

                //dt.Columns.Add(Company);
                //dt.Columns.Add(PayeeName);
                dt.Columns.Add(PayeeCode);
                //dt.Columns.Add(ParentCode);
                //dt.Columns.Add(ParentName);
                //dt.Columns.Add(StartDate);
                //dt.Columns.Add(EndDate);
                dt.Columns.Add(Error);

                foreach (var data in ApiData)
                {
                    dr = dt.NewRow();
                    //dr["Company"] = "";
                    //dr["Payee Name"] = data.LpFirstName + " " + data.LpLastName;
                    dr["Payee Code"] = data.XPayee;
                    //dr["Parent Code"] = data.ParentCode;
                    //dr["Parent Name"] = "";
                    dr["Error"] = data.ErrorMessage;
                    //dr["Start Date"] = data.StartDate;
                    //dr["End Date"] = data.EndDate;

                    dt.Rows.Add(dr.ItemArray);
                }

                foreach (DataColumn column in dt.Columns)
                {
                    csv.WriteField(column.ColumnName);
                }
                csv.NextRecord();

                foreach (DataRow row in dt.Rows)
                {
                    for (var i = 0; i < dt.Columns.Count; i++)
                    {
                        csv.WriteField(row[i]);
                    }
                    csv.NextRecord();
                }
            }
            if (System.IO.File.Exists(fileLocation))
            {
                return File(fileLocation, "application/pdf", "ExportMannualAdjusmentErrorList.csv");
            }
            TempData["Error"] = "No File found";
            return RedirectToAction("UploadManualAdjustment", "ManualAdjustments");
            //return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}