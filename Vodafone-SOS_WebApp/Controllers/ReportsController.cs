using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.ViewModels;
using Vodafone_SOS_WebApp.Helper;
using System.IO;
using System.Configuration;
using Ionic.Zip;
using System.Data.OleDb;
using static Vodafone_SOS_WebApp.Utilities.Globals;
using Vodafone_SOS_WebApp.Utilities;
using System.Threading;
using System.Data;
using System.Web.Script.Serialization;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Vodafone_SOS_WebApp.Controllers
{
    [HandleCustomError]
    public class ReportsController : PrimaryController
    {
        // GET: Reports
        ReportsRestClient RRC = new ReportsRestClient();
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        string UserEmail = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        string UserRole = Convert.ToString(System.Web.HttpContext.Current.Session["UserRole"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        public ActionResult Index()
        {

            return View();
        }
        
        [CheckReportSession]       
        public JsonResult GetXReportsTreeStructure(string strTreeType, string strCommPeriod, string sortdatafield, string sortorder, int? pagesize, int? pagenum)
        {
            //shivani - 21 Nov2019 - pagesize and pagenum parameters converted to nullable
            if (strCommPeriod == null) strCommPeriod = "";
            if (strTreeType == null) strTreeType = "R";
            var qry = Request.QueryString;
            var FilterQuery = "";
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            if (qry.GetValues("filterscount") != null)
            {
                 FilterQuery = Globals.BuildQuery(qry);
                //FilterQuery = FilterQuery.Replace("'%", "''%");
                //FilterQuery = FilterQuery.Replace("%'", "%''");
            }
           // var FilterQuery = "";
            var data = RRC.GetXReportsTreeStructure(strTreeType, UserEmail, UserRole, CompanyCode, strCommPeriod, FilterQuery, sortdatafield, sortorder, pagesize==null?0:(int)pagesize, pagenum==null?0:(int)pagenum);
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //this method is created by RS on 22 Jan 2019 to download files on select all 
        public ActionResult selectalldownload(string strTreeType, string strCommPeriod, string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum)
        {
            if (pagesize == null) pagesize = 200;
            if (pagenum == null) pagenum = 0;
            if (strCommPeriod == null) strCommPeriod = "";
            if (strTreeType == null) strTreeType = "R";
            var qry = Request.QueryString;
            var FilterQuery = "";
            //Generate the filters as per the parameter passed in request in the form of a  Sql Query 
            if (qry.GetValues("filterscount") != null)
            {
                FilterQuery = Globals.BuildQuery(qry);
            }
            var data = RRC.GetXReportsTreeStructure(strTreeType, UserEmail, UserRole, CompanyCode, strCommPeriod, FilterQuery, sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum));

            var FilesToBezipped = new List<string>();
            //MyReportsViewModel data1 = new MyReportsViewModel();
            for (int i = 0; i < data.Count(); i++)
            {
                string ReportName = data.ElementAt(i).XCommissionPeriod;
                string ReportPeriod = data.ElementAt(i).XDisplayLocation;
              string  strFileName = ReportName;
                strFileName = strFileName.Replace("[Non-Prelim]", "");
                strFileName = strFileName.Replace("[Prelim]", "");
                strFileName = strFileName.Replace("[Approved]", "");
                //  var FilesToBezipped = new List<string>();
                string strBaseLocation = "";

                if (ReportPeriod == "My Reports")
                {

                    strBaseLocation = "transfer/a2s/" + CompanyCode + "/Reports/";//R3.1 CompanyCode.ToLower() is removed
                }
                else
                {
                    strBaseLocation = "transfer/a2s/" + CompanyCode + "/Reports/" + ReportPeriod + "/";
                }
                var bytedata = Globals.DownloadFromA2S(strFileName, strBaseLocation);
                if (bytedata != null)
                {
                    if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
                        System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");
                    var TempFileFolder = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + strFileName;
                    System.IO.File.WriteAllBytes(TempFileFolder, bytedata); // Save File
                    FilesToBezipped.Add(strFileName);
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
                return File(ZippedData, "application/zip", "ExportMyReports_" + data.Count() + ".zip");
            }
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }





        public JsonResult GetReportCounts(string strTreeType, string strCommPeriod)
        {
            if (strCommPeriod == null) strCommPeriod = "";
            if (strTreeType == null) strTreeType = "R";
            var data = RRC.GetXReportsTreeStructureCount(strTreeType, UserEmail, UserRole, CompanyCode, strCommPeriod,"");
            return Json(data.Count(), JsonRequestBehavior.AllowGet);
        }
        [CheckReportSession]
        public ActionResult DownloadXReports(string strFileName, string strParent)
        {

            try
            {
                if (strFileName == "") return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
                if (strParent == "") return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
                strFileName = strFileName.Replace("[Non-Prelim]", "");
                strFileName = strFileName.Replace("[Prelim]", "");
                strFileName = strFileName.Replace("[Approved]", "");
                string strBaseLocation = "";
                if (strParent == "My Reports")
                {
                    //R3.1  CompanyCode.ToLower() removed
                    strBaseLocation = ConfigurationManager.AppSettings["AlteryxReportsPath"] + CompanyCode + "\\Reports\\" + strFileName;
                }
                else
                {
                    strBaseLocation = ConfigurationManager.AppSettings["AlteryxReportsPath"] + CompanyCode + "\\Reports\\" + strParent + "\\" + strFileName;
                }

                FileInfo fi = new FileInfo(strBaseLocation);
                if (fi.Exists)
                {
                    return File(strBaseLocation, "application/pdf", strFileName);
                }
                else
                {
                    //return this.Json(new { success = false, message = "Underlying fIle is not available. Please contact support." });
                    //TempData["Message"] = "Underlying fIle is not available. Please contact support.";
                    //return Redirect(System.Web.HttpContext.Current.Session["from"] as string);

                    TempData["ErrorMessage"] = "File does not exist in FileSystem";
                    return Redirect(System.Web.HttpContext.Current.Session["from"] as string);

                }
            }
            catch (Exception)
            {
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }


        }
        public ActionResult DownloadPayeeDocuments(string strFileName, string strParent, string DocSetId)
        {
            if (strParent == "My Documents" || strParent == "") return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            int iDocID = 0;
            //shivani - 21 Nov2019 - condition updated from DocSetId != 0 to string null check
            if (!string.IsNullOrEmpty(DocSetId)) iDocID = Convert.ToInt32(DocSetId);
            LAttachmentsRestClient LRC = new LAttachmentsRestClient();
            // List<LAttachmentViewModel> FilesList = (List<LAttachmentViewModel>)LRC.GetByEntityType("LDocumentSets", iDocID); UserEmail GetLAttachmentsForPayeeID
             List<LAttachmentViewModel> FilesList = (List<LAttachmentViewModel>)LRC.GetLAttachmentsForPayeeID("LDocumentSets", iDocID, UserEmail);  
            if (FilesList.Count() == 0)
            {
                TempData["Error"] = "Report not found";
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            else
            {
                Boolean blnSingleFile = false;
                if (FilesList.Count() == 1)
                {
                    //if (System.IO.File.Exists(FilesList[0].LaFilePath + "\\" + FilesList[0].LaFileName)|| FilesList[0].LaFilePath.Contains("transfer/"))//if file exists then export it
                    //{
                    blnSingleFile = true;
                    //}
                }
                else if (FilesList.Count() > 1)
                {
                    using (ZipFile zip = new ZipFile())
                    {
                        for (int i = 0; i < FilesList.Count(); i++)
                        {
                            var FileData = Globals.DownloadFromA2S(FilesList[i].LaFileName, FilesList[i].LaFilePath);//check for the file in A2S bucket
                            if (FileData == null)//now try downloading from sos bucket
                            {
                                FileData = Globals.DownloadFromS3(FilesList[i].LaFileName, FilesList[i].LaFilePath);
                            }
                            if (FileData != null)//check if file data is received in the form of byte array
                            {
                                if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/forzip"))
                                    System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/forzip");
                                var TempFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/forzip/" + FilesList[i].LaFileName;
                                System.IO.File.WriteAllBytes(TempFilePath, FileData); // Requires System.IO
                                                                                      //Adding in zip for the case if attachments are found ahead then files will be exported as zip
                                zip.AddFile(TempFilePath, "");
                            }
                            //if (System.IO.File.Exists(FilesList[i].LaFilePath + "\\" + FilesList[i].LaFileName))//if file exists then export it
                            //{
                            //    zip.AddFile(FilesList[i].LaFilePath + "\\" + FilesList[i].LaFileName, "");
                            //}
                        }
                        zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                        //delete previous file if present in temp
                        if (System.IO.File.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/forzip/" + strFileName + ".zip"))
                        {
                            System.IO.File.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/forzip/" + strFileName + ".zip");
                        }
                        zip.Save(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/forzip/" + strFileName + ".zip");
                        //Delete unnecessary files of temp folder
                        if (System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/forzip"))
                        {
                            //System.IO.DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/forzip");//Now Delete all files
                            //foreach (FileInfo file in di.GetFiles())
                            //{
                            //    file.Delete();
                            //}
                            foreach (var files in FilesList)
                            {
                                var TempFilePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/forzip/" + files.LaFileName;
                                System.IO.File.Delete(TempFilePath);
                            }
                        }
                    }
                }
                if (blnSingleFile)
                {
                    //if (FilesList[0].LaFilePath.Contains("transfer/"))//SS is adding temp condition as I am unable to find when file will come from bucket and when from S drive
                    //{
                    var ByteData = Globals.DownloadFromA2S(FilesList[0].LaFileName, FilesList[0].LaFilePath);//check for the file in A2S bucket
                    if (ByteData == null)//now try downloading from sos bucket
                    {
                        ByteData = Globals.DownloadFromS3(FilesList[0].LaFileName, FilesList[0].LaFilePath);
                    }
                    if (ByteData == null)
                    {
                        TempData["Error"] = "Report not found";
                        return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
                    }
                    return File(ByteData, "application/unknown", FilesList[0].LaFileName);
                    //}
                    //else
                    //{
                    //    return File(FilesList[0].LaFilePath + "\\" + FilesList[0].LaFileName, "application/pdf", FilesList[0].LaFileName);
                    //}
                }
                else
                {
                    return File(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + UserEmail + "/forzip/" + strFileName + ".zip", "application/zip/Payees", strFileName + ".zip");
                }

            }
        }
        public JsonResult LoadKPIData(string KPIName)
        {
            string fileLocation = "D:\\Templates\\DashboardKPIs-v3.xlsx";
            string excelConnectionString = string.Empty;
            excelConnectionString = string.Format(System.Configuration.ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"], fileLocation);
            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
            excelConnection.Open();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string[] excelSheets = new string[dt.Rows.Count];
            System.Data.DataSet ds = new System.Data.DataSet();
            string query = string.Format("Select * from [Commission Values$]");//, excelSheets[0]);
            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
            {
                dataAdapter.Fill(ds);
            }
            System.Data.DataTable DT = ds.Tables[0];

            System.Data.DataTable DTFiltered = DT.AsEnumerable().Where(r => r.Field<String>("KPI Name") == KPIName).CopyToDataTable();

            //System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            //Dictionary<string, object> row;
            //foreach (System.Data.DataRow dr in DTFiltered.Rows)
            //{
            //    row = new Dictionary<string, object>();
            //    foreach (System.Data.DataColumn col in ds.Tables[0].Columns)
            //    {
            //        row.Add(col.ColumnName, dr[col]);
            //    }
            //    rows.Add(row);
            //}
            for (int i = 0; i < DTFiltered.Columns.Count; i++)
            {
                DTFiltered.Columns[i].ColumnName = DTFiltered.Columns[i].ColumnName.Replace(" ", "");
            }

            var payeedata = new List<ReportsViewModel>();

            for (int i = 0; i < DTFiltered.Rows.Count; i++)
            {
                if (Convert.ToDouble(DTFiltered.Rows[i]["KPIValue"].ToString()) > 0)
                {
                    var pd = new ReportsViewModel();
                    pd.KPIValue = DTFiltered.Rows[i]["KPIValue"].ToString();
                    pd.PayeeName = DTFiltered.Rows[i]["PayeeName"].ToString();
                    payeedata.Add(pd);
                }
            }

            //return Json(payeedata);
            return Json(payeedata, JsonRequestBehavior.AllowGet);

        }

        public ActionResult KPIGraph()
        {
            return View();
        }

        public ActionResult DownloadXReportsBulk(string data)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                MyReportsViewModel[] reportdata = js.Deserialize<MyReportsViewModel[]>(data);
                var FilesToBezipped = new List<string>();


                for (int i = 0; i < reportdata.Length; i++)
                {
                    string strFileName = reportdata[i].strFileName;
                    string strParent = reportdata[i].strParent;

                    strFileName = strFileName.Replace("[Non-Prelim]", "");
                    strFileName = strFileName.Replace("[Prelim]", "");
                    strFileName = strFileName.Replace("[Approved]", "");
                    string strBaseLocation = "";

                    if (strParent == "My Reports")
                    {
                        //R3.1 CompanyCode.ToLower() removed
                        strBaseLocation = "transfer/a2s/" + CompanyCode + "/Reports/";
                    }
                    else
                    {
                        strBaseLocation = "transfer/a2s/" + CompanyCode + "/Reports/" + strParent + "/";
                    }
                    var bytedata = Globals.DownloadFromA2S(strFileName, strBaseLocation);
                    if (bytedata != null)
                    {
                        if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip"))
                            System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip");
                        var TempFileFolder = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip/" + strFileName;
                        System.IO.File.WriteAllBytes(TempFileFolder, bytedata); // Save File
                        FilesToBezipped.Add(strFileName);
                    }
                }

                if (reportdata.Count() > 0)
                {
                    FilesToBezipped = FilesToBezipped.Select(p => new { CompleteFileName = (ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip" + "/" + p) }).Select(p => p.CompleteFileName).ToList();
                    var ZippedData = ZipHelper.ZipFilesToByteArray(FilesToBezipped, System.IO.Packaging.CompressionOption.Normal);
                    foreach (var file in FilesToBezipped)
                    {
                        System.IO.File.Delete(file);
                    }
                    return File(ZippedData, "application/zip", "ExportMyReports_" + reportdata.Count() + ".zip");
                }
            }
            catch (Exception Ex)
            {
                return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
            }
            return Redirect(System.Web.HttpContext.Current.Session["from"] as string);
        }
        
        //public ActionResult DownloadMyClaimsReport()
        //{

        //    return File("");
        //}
    }

}