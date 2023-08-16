//using CsvHelper;
//using Ionic.Zip;
//using Microsoft.Win32;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.Entity;
//using System.Data.OleDb;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using Vodafone_SOS_WebApp.Helper;
//using Vodafone_SOS_WebApp.Models;
//using Vodafone_SOS_WebApp.Utilities;
//using Vodafone_SOS_WebApp.ViewModels;

//namespace Vodafone_SOS_WebApp.Controllers
//{
//    public class LPayeeAuditLogsController : Controller
//    {
//        //Code review comment//The entire controller is unused as it was created for demo purpose only//Need to remove from the project after version 1.3 release

//        //string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
//        //int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
//        //string CompanyName = System.Web.HttpContext.Current.Session["CompanyName"].ToString();
//        //// GET: LPayeeAuditLogs
//        ////[ControllerActionFilter]
//        //public ActionResult Index()
//        //{
//        //    return View();
//        //}

//        ////This method is called when we click on download button in payee grid to download list of files attached with Payee
//        ////[ControllerActionFilter]
//        //public ActionResult DownloadPayeeFiles(int PayeeId)
//        //{
//        //    ILPayeesRestClient LPRC = new LPayeesRestClient();
//        //    var PayeeFiles = LPRC.GetFileNameByPayeeId(PayeeId);
//        //   var s= PayeeFiles.LpFileNames.Split(',');
//        //    using (ZipFile zip = new ZipFile())
//        //    {
//        //        zip.AlternateEncodingUsage = ZipOption.AsNecessary;
//        //        zip.AddDirectoryByName("Files");
//        //        for (var i = 0; i < s.Length; i++)
//        //        {
//        //            if (!string.IsNullOrEmpty(s[i]))
//        //            {
//        //                var filePath = string.Format("{0}{1}", ConfigurationManager.AppSettings["AttachedPayeeDocumentPath"], CompanyCode + "/Payees/" + PayeeFiles.LpPayeeCode + "/Documents/Attached"); ;
//        //                var FileName = filePath + "/" + s[i];
//        //                //check if file exists then only add it to zip folder
//        //                if (System.IO.Directory.Exists(filePath))
//        //                {
//        //                    zip.AddFile(FileName, "Files");
//        //                }
//        //            }
//        //        }
//        //        Response.Clear();
//        //        Response.BufferOutput = false;
//        //        string zipName = String.Format("PayeeDocuments_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd"));
//        //        Response.ContentType = "application/zip";
//        //        Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
//        //        zip.Save(Response.OutputStream);
//        //        Response.End();
//        //    }
//        //    //var UserfriendlyFileNames = PayeeFiles.LpUserFriendlyFileNames.Split(',');
//        //    //     for (int i = 0; i < FileNames.Length; i++)
//        //    //{
//        //    //     if (!string.IsNullOrEmpty(FileNames[i]))
//        //    //     {
//        //    //         var filePath = string.Format("{0}{1}/{2}", ConfigurationManager.AppSettings["AttachedPayeeDocumentPath"], CompanyName + "/Payees/" + PayeeFiles.LpPayeeCode + "/Documents/Attached", FileNames[i]);
//        //    //         //    //req.DownloadFile("http:\\localhost:3264/Content/"+FileNames[0], filePath);
//        //    //         var FileExtension = FileNames[i].Split('.').LastOrDefault();
//        //    //         Response.AddHeader("Content-disposition", "attachment; filename=" + FileNames[i]);
//        //    //         Response.ContentType = Globals.GetFileContentType("."+FileExtension);//"application/pdf";
//        //    //         Response.TransmitFile(filePath);
//        //    //         Response.Clear();
//        //    //         Response.End();
//        //    //         // HttpResponse response = System.Web.HttpContext.Current.Response;
//        //    //         // var FileExtension = FileNames[i].Split('.').LastOrDefault();
//        //    //         // // Set the ContentType
//        //    //         // Response.ContentType = GetFileContentType("."+FileExtension);//"application/Image";
//        //    //         // response.AddHeader("Content-Disposition", "attachment;filename=" + FileNames[i]);
//        //    //         // // Write the document into the response
//        //    //         //byte[] data= req.DownloadData(filePath);
//        //    //         // //response.WriteFile(filePath);
//        //    //         // response.BinaryWrite(data);
//        //    //         // // response.WriteFile(Server.MapPath("~/Content/back.png"));
//        //    //         // response.End();
//        //    //     }
//        //    // }
//        //    return RedirectToAction("AuditorDashboard","Home");
//        //}
//        ////public ActionResult ViewPayeeAuditGrids(AuditFormViewModel AFVM)
//        ////{
//        ////    return RedirectToAction("AuditorDashboard", "Home", new {AFVM=AFVM });
//        ////}

//        ////This method loads the payee audit log grid in auditor dashboard
//        ////[ControllerActionFilter]
//        //public JsonResult GetAuditLog(DateTime StartDate, DateTime EndDate, string Entity)
//        //{
//        //    switch(Entity)
//        //    {
//        //        case "Payee":
//        //            //DateTime End = EndDate.AddDays(1).AddMinutes(-1);
//        //            ILPayeeAuditLogsRestClient PARC = new LPayeeAuditLogsRestClient();
//        //           // var AuditModel = new GetAuditLogUnderStartDateEndDateViewModel { CompanyId = CompanyId, StartDate = StartDate, EndDate = End };
//        //            var ApiData= PARC.GetAuditLogUnderStartDateEndDate(StartDate,EndDate,CompanyId);
//        //            return Json(ApiData, JsonRequestBehavior.AllowGet);
//        //        case "Scheme":
//        //            break;
//        //        case "Tariff":
//        //            break;
//        //        case "Commission":
//        //            break;
//        //        case "Claims":
//        //            break;
//        //    }
//        //    return Json(string.Empty, JsonRequestBehavior.AllowGet);
//        //}

//        //// This method is used to load data in change request grid in auditor dashboard
//        ////[ControllerActionFilter]
//        //public JsonResult GetChangeRequestGridForAuditor(DateTime StartDate, DateTime EndDate, string Entity)
//        //{
//        //    ILChangeRequestsRestClient LCRRC = new LChangeRequestsRestClient();
//        //    switch (Entity)
//        //    {
//        //        case "Payee":
//        //           // DateTime End = EndDate.AddDays(1).AddMinutes(-1);
//        //            //var AuditModel = new GetAuditLogUnderStartDateEndDateViewModel { CompanyId = CompanyId, StartDate = StartDate, EndDate = End };
//        //            var ApiData = LCRRC.GetChangeRequestUnderStartDateEndDate(StartDate,EndDate,CompanyId);
//        //            return Json(ApiData, JsonRequestBehavior.AllowGet);
//        //        break;
//        //        case "Scheme":
//        //        break;
//        //        case "Tariff":
//        //        break;
//        //        case "Commission":
//        //        break;
//        //        case"Claims":
//        //        break;
//        //    }
//        //    return Json(string.Empty, JsonRequestBehavior.AllowGet);
//        //}

//        //// This method is called when we click on download button in auditor dashboard to download change request grid in csv format
//        ////[ControllerActionFilter]
//        //public JsonResult ExportChangeRequestCsv(DateTime StartDate, DateTime EndDate, string Entity)
//        //{
//        //    ILChangeRequestsRestClient LCRRC = new LChangeRequestsRestClient();
//        //    switch(Entity)
//        //    {
//        //        case "Payee":
//        //            //get data of changerequest in same method
//        //                var ApiData = LCRRC.GetChangeRequestUnderStartDateEndDate(StartDate,EndDate,CompanyId);

//        //            //export change request
//        //            var CfileLocation = Server.MapPath(ConfigurationManager.AppSettings["PayeeAuditPath"] + "/ExportChangeRequest.csv");
//        //            if (System.IO.File.Exists(CfileLocation))
//        //                System.IO.File.Delete(CfileLocation);
//        //            using (var CTextWriter = new StreamWriter(CfileLocation))
//        //            using (var Ccsv = new CsvWriter(CTextWriter))
//        //            {
//        //                DataRow dr;
//        //                DataTable dt1 = new DataTable();
//        //                DataColumn Company1 = new DataColumn("Company");
//        //                DataColumn PayeeName1 = new DataColumn("Payee Name");
//        //                DataColumn PayeeCode1 = new DataColumn("Payee Code");
//        //                DataColumn CreatedBy = new DataColumn("Created By");
//        //                DataColumn CreatedDate = new DataColumn("Created Date Time");
//        //                DataColumn UpdatedBy1 = new DataColumn("Updated By");
//        //                DataColumn UpdatedDateTime1 = new DataColumn("Updated Date Time");
//        //                DataColumn OldValue = new DataColumn("Old Value");
//        //                DataColumn NewValue = new DataColumn("New Value");
//        //                DataColumn Status1 = new DataColumn("Status");
//        //                DataColumn Action = new DataColumn("Action");

//        //                dt1.Columns.Add(Company1);
//        //                dt1.Columns.Add(PayeeName1);
//        //                dt1.Columns.Add(PayeeCode1);
//        //                dt1.Columns.Add(OldValue);
//        //                dt1.Columns.Add(NewValue);

//        //                dt1.Columns.Add(CreatedBy);
//        //                dt1.Columns.Add(CreatedDate);
//        //                dt1.Columns.Add(UpdatedBy1);
//        //                dt1.Columns.Add(UpdatedDateTime1);
//        //                dt1.Columns.Add(Action);
//        //                dt1.Columns.Add(Status1);

//        //                foreach (var data in ApiData)
//        //                {
//        //                    dr = dt1.NewRow();
//        //                    dr["Company"] = data.GcCompanyName;
//        //                    dr["Payee Name"] = data.FullName;
//        //                    dr["Payee Code"] = data.LpPayeeCode;
//        //                    dr["Updated By"] = data.UpdatedBy;
//        //                    dr["Updated Date Time"] = data.LcrUpdatedDateTime;
//        //                    dr["Status"] = data.RsStatus;
//        //                    dr["Created By"] = data.CreatedBy;
//        //                    dr["Created Date Time"] = data.LcrCreatedDateTime;
//        //                    dr["Action"] = data.LcrAction;
//        //                    dr["Old Value"] = data.LcrNewValue;
//        //                    dr["New value"] = data.LcrNewValue;


//        //                    dt1.Rows.Add(dr.ItemArray);
//        //                }

//        //                foreach (DataColumn column in dt1.Columns)
//        //                {
//        //                    Ccsv.WriteField(column.ColumnName);
//        //                }
//        //                Ccsv.NextRecord();

//        //                foreach (DataRow row in dt1.Rows)
//        //                {
//        //                    for (var i = 0; i < dt1.Columns.Count; i++)
//        //                    {
//        //                        Ccsv.WriteField(row[i]);
//        //                    }
//        //                    Ccsv.NextRecord();
//        //                }
//        //            }
                
//        //        return Json(string.Empty, JsonRequestBehavior.AllowGet);
//        //    }
//        //    return Json(string.Empty, JsonRequestBehavior.AllowGet);
//        //}

//        ////  This method loads the payee grid in csv format when auditor clicks on download button
//        ////[ControllerActionFilter]
//        //public JsonResult ExportPayeecsv(DateTime StartDate,DateTime EndDate, string Entity)
//        //{

//        //    switch(Entity)
//        //    {
//        //        case "Payee":
//        //            ILPayeeAuditLogsRestClient PARC = new LPayeeAuditLogsRestClient();
//        //            var ApiData= PARC.GetAuditLogUnderStartDateEndDate(StartDate,EndDate,CompanyId).ToList();


//        //            var fileLocation = Server.MapPath(ConfigurationManager.AppSettings["PayeeAuditPath"] + "/ExportPayee.csv");
//        //            if (System.IO.File.Exists(fileLocation))
//        //                System.IO.File.Delete(fileLocation);
//        //            using (var TextWriter = new StreamWriter(fileLocation))
//        //            using (var csv = new CsvWriter(TextWriter))
//        //            {

//        //                DataRow dr;
//        //                DataTable dt = new DataTable();
//        //                DataColumn Company = new DataColumn("Company");
//        //                DataColumn PayeeName = new DataColumn("Payee Name");
//        //                DataColumn PayeeCode = new DataColumn("Payee Code");
//        //                DataColumn UpdatedBy = new DataColumn("Updated By");
//        //                DataColumn UpdatedDateTime = new DataColumn("Updated Date Time");
//        //                DataColumn CreatedBy = new DataColumn("Created By");
//        //                DataColumn CreatedDateTime = new DataColumn("Created Date Time");
//        //                DataColumn Status = new DataColumn("Status");

//        //                dt.Columns.Add(Company);
//        //                dt.Columns.Add(PayeeName);
//        //                dt.Columns.Add(PayeeCode);
//        //                dt.Columns.Add(CreatedBy);
//        //                dt.Columns.Add(CreatedDateTime);
//        //                dt.Columns.Add(UpdatedBy);
//        //                dt.Columns.Add(UpdatedDateTime);
//        //                dt.Columns.Add(Status);

//        //                foreach (var data in ApiData)
//        //                {
//        //                    dr = dt.NewRow();
//        //                    dr["Company"] = data.GcCompanyName;
//        //                    dr["Payee Name"] = data.FullName;
//        //                    dr["Payee Code"] = data.LpPayeeCode;
//        //                    dr["Created By"] = data.CreatedBy;
//        //                    dr["Created Date Time"] = data.LpCreatedDateTime;
//        //                    dr["Updated By"] = data.UpdatedBy;
//        //                    dr["Updated Date Time"] = data.LpalUpdatedDateTime;
//        //                    dr["Status"] = data.LpalAction;

//        //                    dt.Rows.Add(dr.ItemArray);
//        //                }

//        //                foreach (DataColumn column in dt.Columns)
//        //                {
//        //                    csv.WriteField(column.ColumnName);
//        //                }
//        //                csv.NextRecord();

//        //                foreach (DataRow row in dt.Rows)
//        //                {
//        //                    for (var i = 0; i < dt.Columns.Count; i++)
//        //                    {
//        //                        csv.WriteField(row[i]);
//        //                    }
//        //                    csv.NextRecord();
//        //                }


//        //            }
//        //            break;

//        //    }
//        //    return Json(string.Empty, JsonRequestBehavior.AllowGet);
//        //}

      
//    }
//}