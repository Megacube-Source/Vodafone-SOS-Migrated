//Code Review for this file (from security perspective) done

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LSupportTicketsController : Controller//PrimaryController//RK R2.3 17112018 made this change (comment URL Tempring) so that review can open in new tab
    {
        LSupportTicketContextModel LT = new ViewModels.LSupportTicketContextModel();
        LSupportTicketsRestClient RestClient = new LSupportTicketsRestClient();
        LUsersRestClient UserRestClient = new LUsersRestClient();
        IAspnetUsersRestClient AURC = new AspnetUsersRestClient();
        string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        // GetSystemAnalystTickets: LSupportTickets
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        //string UserRole = System.Web.HttpContext.Current.Session["UserRole"].ToString();
        //string UserName = System.Web.HttpContext.Current.Session["UserName"].ToString();
        string UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
       
        string strUserRole = System.Web.HttpContext.Current.Session["UserRole"].ToString();
        string iRoleId = System.Web.HttpContext.Current.Session["UserRoleId"].ToString();
        Boolean blnIsSupportUser = false;
        string strSupportLevel = "";
        //DateTime dtSearchFrom, dtSearchTo;
        string CompanyName = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyName"]);
        string OpCoCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);


        public LSupportTicketsController()
        {
            CheckUserSupportLevel();
        }
       
        public void CheckUserSupportLevel()
        {
            RSupportTeamsViewModel rs = new ViewModels.RSupportTeamsViewModel();
            var ApiData = RestClient.GetUserSupportLevel(iRoleId);
            foreach (var item in ApiData)
            {
                if(item.RstRoleId!="")
                {
                    blnIsSupportUser = true;
                }
            }
            if (strUserRole.ToLower() == "system analyst")
            {
                blnIsSupportUser = true;
                strSupportLevel = "L1";
            }
            if (strUserRole.Contains("L2"))
            {
                blnIsSupportUser = true;
                strSupportLevel = "L2";
            }
            if (strUserRole.Contains("L3"))
            {
                blnIsSupportUser = true;
                strSupportLevel = "L3";
            }

        }

       
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Tickets";
            ViewBag.LstQuickTicketID = GetQuickTicketsList();
            ViewBag.blnSupportUserLoggedIn = blnIsSupportUser;
            ViewBag.strSupportLevel = strSupportLevel;
            return View();
        }

       
        public ActionResult CreateNormalTicket()
        {
            return RedirectToAction("Create", "LSupportTickets", new { Qtkt = 0 });
        }

       
        public ActionResult CreateQuickTicket(int id)
        {
            return RedirectToAction("Create", "LSupportTickets", new { Qtkt = id });
        }
       
        public ActionResult Create(int Qtkt)
        {
            ViewBag.LstCategoryID = GetCategoryList();
            ViewBag.LstStageID = GetTicketStageList(null);
            //LSupportTicketsViewModel LT = new ViewModels.LSupportTicketsViewModel();
            LSupportTicketContextModel LT = new ViewModels.LSupportTicketContextModel();
            if(Qtkt > 0)
            {
                SupportSystemRestClient SSRC = new SupportSystemRestClient();
                SupportSystemQuickTicketsViewModel SSQTV = SSRC.GetQuickTicketById(Qtkt);
                if(SSQTV.RsqtCategoryId >0) LT.LstCategoryId = SSQTV.RsqtCategoryId;
                LT.LstSummary = SSQTV.RsqtSummary;
                LT.LsrDescription = SSQTV.RsqtTicketDescription;
            }
            System.Web.HttpContext.Current.Session["Title"] = "Create New Ticket";
            //LT.LstRequestor = System.Web.HttpContext.Current.Session["FirstName"].ToString() + " " + System.Web.HttpContext.Current.Session["LastName"].ToString();

            LT.LstRequestor = System.Web.HttpContext.Current.Session["UserName"].ToString();       
            LT.LstEmail = System.Web.HttpContext.Current.Session["UserName"].ToString();
            LT.LstPhone = System.Web.HttpContext.Current.Session["PhoneNumber"].ToString();
            ViewBag.strSupportLevel = strSupportLevel;
            //ViewBag.LstQuickTicketID = GetQuickTicketsList();
            return View(LT);
        }

       
        public ActionResult Edit(int id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Edit Ticket";

            LSupportTicketContextModel model = RestClient.GetById(id);
            //Get Supporting Documents
            ILSupportingDocumentsRestClient SDRC = new LSupportingDocumentsRestClient();
            ViewBag.SupportingDocument = SDRC.GetByEntityType("LSupportTickets", model.Id);
            LUserViewModel Lu = UserRestClient.GetUserDetailsByAspNetUserID(model.LstCreatedById);
            model.LstRequestor = Lu.LuFirstName + " " + Lu.LuLastName;
            ISupportSystemRestClient SSRC = new SupportSystemRestClient();
            ViewBag.Category = SSRC.GetCategoryById(model.LstCategoryId).RscName;
            //if (model.LstStageId != null && model.LstStageId > 0)
            //    ViewBag.TicketStage = SSRC.GetTicketStageById(Convert.ToInt32(model.LstStageId)).RtsName;
            //else
            //    ViewBag.TicketSage = "";

                model.LstCreatedByUserName = model.LstRequestor;
            model.LstEmail = Lu.LuEmail;
            //Bind the description
            model.LstExDescription = model.LsrDescription;
            //Reset Lsr Description
            model.LsrDescription = null;
            //model.LstCategoryID = 6;
            //ViewBag.RsqtCategoryId = GetCategoryList();
            ViewBag.LstCategoryID = GetCategoryList();
            //var StageIdList = GetTicketStageList();

            //ViewBag.LstStageID = new SelectListItem(StageIdList, "Id", "RtsName");

            ViewBag.LstStageID = GetTicketStageList(null);

            //TempData["StageIdList"] = StageIdList;
            // CheckUserSupportLevel();
            ViewBag.strSupportLevel = strSupportLevel;
            ViewBag.LstTicketStatus = model.LstStatus;
            if (model.LstTeamId != null && model.LstTeamId > 0)
            {
                string strTeam = GetAllocatedTeamName(Convert.ToInt32(model.LstTeamId));
                if (strTeam.Contains("L1"))ViewBag.AllocatedToTeam = "L1";
                if (strTeam.Contains("L2")) ViewBag.AllocatedToTeam = "L2";
                if (strTeam.Contains("L3")) ViewBag.AllocatedToTeam = "L3";
            }
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
       
        //private string BuildExistingTicketDescription(int iTicketId)
        //{
        //    LUserViewModel Lm = new LUserViewModel();
        //    RSupportTeamsViewModel Rm = new RSupportTeamsViewModel();
        //   // string strUserName = "";
        //    //string strResultString = "";
        //    var strResultString = RestClient.GetTicketResponses(iTicketId);
        //   // var apidata = RestClient.GetTeamDetail(iRoleId);
           
        //    //foreach (var item in ApiData)
        //    //{
        //    //   // Lm = UserRestClient.GetUserDetailsByAspNetUserID(item.LsrResponseById);

        //    //    //strTeamName = apidata;

        //    //    // strUserName = Lm.LuFirstName + " " + Lm.LuLastName;
        //    //    //strUserName = Lm.LuEmail;
        //    //    if (strResultString != "") strResultString = strResultString + Environment.NewLine;
        //    //    strResultString += item.LsrDescription+"";
        //    //    //strResultString = strResultString + "[ "+ item.LsrDescription + " ]";
        //    //}
        //    return strResultString;
        //}
      
        private SelectList GetCategoryList()
        {
            ISupportSystemRestClient SSRC = new SupportSystemRestClient();
            var ApiData = SSRC.GetCategoryList(CompanyId);
            var x = new SelectList(ApiData, "Id", "RscName");
            return x;
        }

        //private SelectList GetTicketStageList()
        //{
        //    ISupportSystemRestClient SSRC = new SupportSystemRestClient();
        //    var ApiData = SSRC.GetTicketStagesList();
        //    var x = new SelectList(ApiData, "Id", "RtsName");
        //    return x;
        //}

        private SelectList GetTicketStageList(int? Selected)
        {
            ISupportSystemRestClient SSRC = new SupportSystemRestClient();
            var ApiData = SSRC.GetTicketStagesList();
            var x = new SelectList(ApiData, "Id", "RtsName", Selected);
            return x;
        }

        public SelectList GetQuickTicketsList()
        {
            ISupportSystemRestClient SSRC = new SupportSystemRestClient();
            var ApiData = SSRC.GetQuickTicketList();
            var x = new SelectList(ApiData, "Id", "RsqtUILabel");
            return x;
        }

        public JsonResult GetAspnetUsers()
        {
            IAspnetUsersRestClient AURC = new AspnetUsersRestClient();
            var ApiData = AURC.GetActivetUserByCompanyId(CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create(LSupportTicketContextModel STVM, HttpPostedFileBase[] FileUpload,string PortfolioList,bool SendtoNextlevel)
        {
          try
          {
                //call amethod to save file in s drive and return list of files
                var FileDetails = new AttachedFilesViewModel();
                if (FileUpload[0] != null)
                {
                    FileDetails = AttachSupportTicketFiles(FileUpload);
                }
                
                STVM.LstCompanyId = CompanyId;
                STVM.LstCreatedDateTime = DateTime.Now;
                STVM.LstStatus = "New";
                STVM.LsrTicketStatus = "Create";
                STVM.LstSeverity = "Dummy";//js asked to enter dummy value after commenting severity from clent side
                STVM.LstLastUpdatedDateTime = DateTime.Now;
               // STVM.LstCreatedById = UserId;
                STVM.LstLastUpdatedById = UserId;
                STVM.OpCoName = OpCoCode;
                STVM.LstTeamId = GetTeamID("L1 " + OpCoCode);
                string curUser = System.Web.HttpContext.Current.Session["UserName"].ToString();
                //MS R2.4 This is for behalf of ticket
                if (String.Compare(STVM.LstRequestor, curUser) !=0)
                { 
                    //STVM.Ordinal = 1;
                    STVM.LstCreatedById = STVM.LstCreatedOnBehalfOfId;
                    SendtoNextlevel = false;
                    //MS R2.4 This because the behalf of ticket also has forward to L2 functionality
                    STVM.LstTeamId = GetTeamID("L2 " + STVM.LstType);
                    STVM.Ordinal = 3;
                }
                else
                {
                    STVM.LstCreatedById = UserId;
                    ViewBag.strSupportLevel = strSupportLevel;
                    if (strSupportLevel == "L2")
                    {
                        STVM.LstL2Id = UserId;
                        STVM.Ordinal = 3;
                    }
                    else if (strSupportLevel == "L1")
                    {
                        STVM.LstL1Id = UserId;
                        STVM.Ordinal = 2;
                    }
                    else
                    {
                        STVM.Ordinal = 1;
                    }
                }
                
                //check if send to L2 is set to true assign ticket to L2
                if(SendtoNextlevel)
                {
                    if(STVM.Ordinal<3)
                        STVM.Ordinal += 1;
                    if(STVM.Ordinal==3)
                        STVM.LstTeamId = GetTeamID("L2 " + STVM.LstType);
                }
                if (!string.IsNullOrEmpty(STVM.LsrDescription))
                {
                    var apidata = RestClient.GetTeamDetail(iRoleId);
                    if (apidata == null)
                        apidata = System.Web.HttpContext.Current.Session["UserRole"] as string;

                    if (STVM.LstRequestor != System.Web.HttpContext.Current.Session["UserName"].ToString())
                    {
                        STVM.LsrDescription = "[" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] [" + UserName + "] [" + apidata + "]: Created the ticket on behalf of " + STVM.LstRequestor + ". The Ticket is also forwarded to L2 ["+DateTime.UtcNow.ToString("dd / MM / yyyy HH: mm")+"]["+UserName+"]["+apidata+"]:" + STVM.LsrDescription;
                    }
                    else
                    {
                        STVM.LsrDescription = "[" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] [" + UserName + "] [" + apidata + "]:" + STVM.LsrDescription;
                    }
                    
                }
                //}
                   var TicketNumber= RestClient.CreateNewTicket(STVM,FileDetails.FileName,FileDetails.FilePath,PortfolioList);
                TempData["Message"] = "Ticket with number:- " + TicketNumber + " created sucessfully.";

                //ViewBag.LstTicketNumber = STVM.LstTicketNumber;
                ViewBag.LstCategoryID = GetCategoryList();
                ViewBag.LstStageID = GetTicketStageList(null);
                ViewBag.Message = "Ticket Generated!";
                return RedirectToAction("Index");
          }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                return View(STVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LSupportTicketContextModel STVM,int? iTeamID, HttpPostedFileBase[] FileUpload, string PortfolioList, string stageName)
        {
            try
            {
                
                //call amethod to save file in s drive and return list of files
                var FileDetails = new AttachedFilesViewModel();
                
                //if (FileUpload[0] != null)
                //{
                //    FileDetails = AttachSupportTicketFiles(FileUpload);
                //}
                if (!string.IsNullOrEmpty(STVM.LsrDescription))
                {
                    var apidata = RestClient.GetTeamDetail(iRoleId);
                    if (apidata == null)
                        apidata = System.Web.HttpContext.Current.Session["UserRole"] as string;
                    STVM.LsrDescription = "["+DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm")+ "] ["+UserName+"] ["+apidata+"]:"+ STVM.LsrDescription;
                }
                STVM.LstLastUpdatedDateTime = DateTime.Now;
                STVM.LstLastUpdatedById = UserId;
                STVM.LsrResponseById = UserId;
                STVM.LsrResponseByName = "";
                STVM.LsrResponseDateTime = DateTime.Now;
                if (iTeamID > 0)
                { STVM.LstTeamId = iTeamID; }
                else
                {
                   // string TeamName = "L2"+" "+STVM.LstType;
                    //STVM.LstTeamId = GetTeamID("L2 " + STVM.LstType);

                    if (STVM.LstTeamId != null && STVM.LstTeamId > 0)
                    {
                        string strTeam = GetAllocatedTeamName(Convert.ToInt32(STVM.LstTeamId));
                        //MS R2.2.1 TeamId can be changed by Requestor (Non Support Staff only if has reached the L2 level otherwise he just need to forward to l1 and Forward to L2

                        if (strTeam.Contains("L2"))
                            STVM.LstTeamId = GetTeamID("L2 " + STVM.LstType); 
                        
                    }


                }
                RestClient.UpdateTicket(STVM,"Update",FileDetails.FileName,FileDetails.FilePath,PortfolioList);

                ViewBag.LstCategoryID = GetCategoryList();
                ViewBag.LstStageID = GetTicketStageList(null);
                ViewBag.Message = "Ticket Updated!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                ViewBag.LstCategoryID = GetCategoryList();
                return View(STVM);
            }
        }

      
        public JsonResult GetRequestorTickets()
        {
            var ApiData = RestClient.GetRequestorTicket(UserId,CompanyId,iRoleId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadTickets(string TabName)
        {
            IEnumerable<LSupportTicketsViewModel> ApiData=new List<LSupportTicketsViewModel>();
           if (TabName == "Requestor")
            {
               ApiData = RestClient.GetRequestorTicket(UserId, CompanyId, iRoleId).ToList();
            }
           else if(TabName== "SystemAnalyst")
            {
                ApiData = RestClient.GetSystemAnalystTicket(UserId, CompanyId, iRoleId);
            }
            else if(TabName=="L2Admin")
            {
                ApiData = RestClient.GetL2SupportTicket(UserId, CompanyId, iRoleId);
            }

           
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            IRow row1 = sheet1.CreateRow(0);
            var properties = typeof(LSupportTicketsViewModel).GetProperties();
            
            int count=0;
            for (int j = 0; j < properties.Length; j++)
            {
                var ColName = properties[j].Name.Replace("Lst", "");
                if (ColName == "RstName")
                { ColName = "Stage"; }
                if (ColName == "RscName")
                { ColName = "Category"; }
                ColName = ColName.Replace("Description", "Comment");
                ColName = ColName.Replace("Rst", "");
                ColName = ColName.Replace("Lsr","");

                if (ColumnToBeExportedInExcel(ColName))
                {
                    ICell cell = row1.CreateCell(count);
                    //string columnName = properties[j].GetValue(ApiData, null).ToString();
                    cell.SetCellValue(ColName);
                    // GC is used to avoid error System.argument exception
                    GC.Collect();
                    count++;
                }
                //else if (ColName == "Id")
                //{
                //    //Add Description column
                //    ICell DescriptionCell = row1.CreateCell(count);
                //    string ColumnValue = "Description";
                //    DescriptionCell.SetCellValue(ColumnValue);
                //    count++;
                //}
            }
             
            for (int i = 0;i<ApiData.Count(); i++)
                {
                count = 0;
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < properties.Length; j++)
                {
                    var ColName = properties[j].Name.Replace("Lst", "");
                    ColName = ColName.Replace("Rst", "");
                    ColName = ColName.Replace("Lsr", "");
                    if (ColumnToBeExportedInExcel(ColName))
                    {
                        ICell cell = row.CreateCell(count);
                        //string ColumnValue = properties[j].GetValue(ApiData.ElementAt(i), null) as string;//RK09112018 Commented as  date time values got missing
                        string ColumnValue = Convert.ToString(properties[j].GetValue(ApiData.ElementAt(i), null));
                        cell.SetCellValue(ColumnValue);
                        count++;
                    }
                    //else if(ColName=="LsrDescription")
                    //{
                    //    //Add Description column at last
                    //    ICell DescriptionCell = row.CreateCell(count);
                    //    string ColumnValue =//BuildExistingTicketDescription(Convert.ToInt32(properties[j].GetValue(ApiData.ElementAt(i), null)));
                    //    DescriptionCell.SetCellValue(ColumnValue);
                    //    count++;
                    //}
                }
               

            }

            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + OpCoCode + "/" + UserName))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + OpCoCode + "/" + UserName);
            }
            //Get fileName
            var FileName = "ExportSupportTicket_" + TabName + ".xlsx";
            FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + OpCoCode + "/" + UserName, FileName), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            xfile.Close();
            var CompleteFileName = Path.Combine(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + OpCoCode + "/" + UserName, FileName);
            return File(CompleteFileName,"application/zip",FileName);
        }

        public JsonResult GetSystemAnalystTickets()
        {
            var ApiData = RestClient.GetSystemAnalystTicket(UserId,CompanyId,iRoleId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetL2SupportTickets()
        {
            var ApiData = RestClient.GetL2SupportTicket(UserId,CompanyId,iRoleId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

     
        //public JsonResult GetAllTickets()
        //{
        //    var ApiData = RestClient.GetAllickets(UserId, CompanyId);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
    
        public ActionResult SearchTicketsByDate(LSupportTicketsViewModel Lst)
        {
            return Index();
        }

        
        public JsonResult GetTicketsForDateRange(DateTime? LstSearchTicketFromDate, DateTime? LstSearchTicketToDate, string LstSearchString)
        {
            var ApiData = RestClient.GetTicketsForDateRange(LstSearchTicketFromDate, LstSearchTicketToDate, LstSearchString, iRoleId, UserId, CompanyId);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadTicketsForDateRange( DateTime? LstSearchTicketFromDate, DateTime? LstSearchTicketToDate, string LstSearchString)
        {
            var ApiData = RestClient.GetTicketsForDateRange(LstSearchTicketFromDate, LstSearchTicketToDate, LstSearchString, iRoleId, UserId, CompanyId);
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            IRow row1 = sheet1.CreateRow(0);
            var properties = typeof(LSupportTicketsViewModel).GetProperties();

            int count = 0;
            for (int j = 0; j < properties.Length; j++)
            {
                var ColName = properties[j].Name.Replace("Lst", "");
                ColName = ColName.Replace("Rst", "");
                ColName = ColName.Replace("Lsr", "");
                if (ColumnToBeExportedInExcel(ColName))
                {
                    ICell cell = row1.CreateCell(count);
                    //string columnName = properties[j].GetValue(ApiData, null).ToString();
                    cell.SetCellValue(ColName);
                    // GC is used to avoid error System.argument exception
                    GC.Collect();
                    count++;
                }
                //else if (ColName == "Id")
                //{
                //    //Add Description column
                //    ICell DescriptionCell = row1.CreateCell(count);
                //    string ColumnValue = "Description";
                //    DescriptionCell.SetCellValue(ColumnValue);
                //    count++;
                //}
            }

            for (int i = 0; i < ApiData.Count(); i++)
            {
                count = 0;
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < properties.Length; j++)
                {
                    var ColName = properties[j].Name.Replace("Lst", "");
                    ColName = ColName.Replace("Rst", "");
                    ColName = ColName.Replace("Lsr", "");
                    if (ColumnToBeExportedInExcel(ColName))
                    {
                        ICell cell = row.CreateCell(count);
                        //string ColumnValue = properties[j].GetValue(ApiData.ElementAt(i), null) as string;//RK09112018 Commented as  date time values got missing
                        string ColumnValue = Convert.ToString(properties[j].GetValue(ApiData.ElementAt(i), null));
                        cell.SetCellValue(ColumnValue);
                        count++;
                    }
                }


            }

            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + OpCoCode + "/" + UserName))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + OpCoCode + "/" + UserName);
            }
            //Get fileName
            var FileName = "ExportSupportTicket_History.xlsx";
            FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + OpCoCode + "/" + UserName, FileName), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            xfile.Close();
            var CompleteFileName = Path.Combine(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + OpCoCode + "/" + UserName, FileName);
            return File(CompleteFileName, "application/zip", FileName);
        }
        public JsonResult GetSearchTickets(string LstTicketSearchString)
        {
            var ApiData = RestClient.GetSearchTickets(LstTicketSearchString, UserId,strUserRole);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
      
        private int GetTeamID(string strTeamName)
        {
            int iTeamID = 0;
            var ApiData = RestClient.GetTeamDetails();
            /*RK: its better to change the codelet the team names the way it is*/
            if (strTeamName == "L2 99"|| strTeamName == "L1 99")
                strTeamName = "L2 SOS";
            foreach (var item in ApiData)
            {
                if(item.RstTeamName == strTeamName)
                {
                   // iTeamID = item.RstTeamId;
                    iTeamID = item.Id;
                    return iTeamID;
                }
            }
            return iTeamID;
        }
      

        private string GetAllocatedTeamName(int iTeamId)
        {
            string strTeamName = "";
            var ApiData = RestClient.GetTeamDetails();
            foreach (var item in ApiData)
            {
                if (item.Id == iTeamId)
                {
                    strTeamName = item.RstTeamName;
                    return strTeamName;
                }
            }
            return strTeamName;
        }

        
        private void CopyUploadedFiles(HttpPostedFileBase[] FileUpload,string strTicketNumber)
        {
            string FileNames = null;
            var filePath = "";
            foreach (HttpPostedFileBase files in FileUpload)
            {
                if (files != null)
                {
                    var fileLocation = "";
                    string fileExtension = System.IO.Path.GetExtension(files.FileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(files.FileName);
                    if (string.IsNullOrEmpty(FileNames))
                    {
                        FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                    }
                    else
                    {
                        FileNames = FileNames + "," + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;
                    }
                    filePath = ConfigurationManager.AppSettings["S3_SupportFilePath"]+"\\"+ strTicketNumber;
                    fileLocation = filePath + "\\" + name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;//As disscussed with JS VG file name will have datetime stamp as suffix
                    bool exists = System.IO.Directory.Exists(filePath);
                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                    }
                    files.SaveAs(fileLocation);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult SelfAssign(LSupportTicketContextModel Tkt)
        {
            return SelfAssignTicket(Tkt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Withdraw(LSupportTicketContextModel Tkt)
        {
            return MoveTicket(Tkt, "Withdraw");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Close(LSupportTicketContextModel Tkt)
        {
            return MoveTicket(Tkt, "Closed");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult ReOpen(LSupportTicketContextModel Tkt)
        {
            return MoveTicket(Tkt, "ReOpen");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult BackToRequestor(LSupportTicketContextModel Tkt)
        {
            return MoveTicket(Tkt, "BackToRequestor");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult BackToL1(LSupportTicketContextModel Tkt)
        {
            return MoveTicket(Tkt, "BackToL1");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult BackToL2(LSupportTicketContextModel Tkt)
        {
            return MoveTicket(Tkt, "BackToL2");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult ForwardToL1(LSupportTicketContextModel Tkt)
        {
            return ForwardTicket(Tkt, "ForwardToL1");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult ForwardToL2(LSupportTicketContextModel Tkt)
        {
            return ForwardTicket(Tkt, "ForwardToL2");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ControllerActionFilter]
        //public ActionResult ForwardToL3(LSupportTicketContextModel Tkt)
        //{
        //    return ForwardTicket(Tkt, "ForwardToL3");
        //}

            //method to handle mass actions
        public ActionResult ExecuteMassAction(string Comments,string TransactionId,string ActionName)
        {
            if (!string.IsNullOrEmpty(Comments))
            {
                var apidata = RestClient.GetTeamDetail(iRoleId);
                if (apidata == null)
                    apidata = System.Web.HttpContext.Current.Session["UserRole"] as string;
                Comments = "[" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] [" + UserName + "] [" + apidata + "]:" + Comments;
            }
            RestClient.ExecuteMassActions(ActionName, TransactionId, UserId, Comments);
            return RedirectToAction("Index");
        }

        public ActionResult ExecuteMassSelfAssignL2(string Comments, string TransactionId)
        {
            if (!string.IsNullOrEmpty(Comments))
            {
                var apidata = RestClient.GetTeamDetail(iRoleId);
                if (apidata == null)
                    apidata = System.Web.HttpContext.Current.Session["UserRole"] as string;
                Comments = "[" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] [" + UserName + "] [" + apidata + "]:" + Comments;
            }
            RestClient.ExecuteMassSelfAssignsL2(TransactionId, UserId, Comments);
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult MoveTicket(LSupportTicketContextModel STVM, string strCommand)
        {
            if (!string.IsNullOrEmpty(STVM.LsrDescription))
            {
                var apidata = RestClient.GetTeamDetail(iRoleId);
                if (apidata == null)
                    apidata = System.Web.HttpContext.Current.Session["UserRole"] as string;
                STVM.LsrDescription = "[" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] [" + UserName + "] [" + apidata + "]: " + STVM.LsrDescription;
            }
            if (strCommand == "Withdraw" || strCommand == "Closed"|| strCommand == "ReOpen")
            {
                try
                {
                    STVM.LstLastUpdatedDateTime = DateTime.Now;
                    STVM.LstLastUpdatedById = UserId;
                    STVM.LsrResponseById = UserId;
                    STVM.LsrResponseByName = "";
                    STVM.LsrResponseDateTime = DateTime.Now;
                    STVM.LstStatus = strCommand;
                    STVM.LsrTicketStatus = strCommand;
                   // LsrTicketStaus
                    //if (iTeamID > 0) STVM.LstTeamId = iTeamID;
                    RestClient.UpdateTicket(STVM,"Update",string.Empty,string.Empty,string.Empty);
                    ViewBag.LstCategoryID = GetCategoryList();
                    ViewBag.LstStageID = GetTicketStageList(null);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];//.Message;
                    ViewBag.LstCategoryID = GetCategoryList();
                    return View(STVM);
                }
            }
            if(strCommand== "BackToL1")
            {
                STVM.LstLastUpdatedDateTime = DateTime.Now;
                STVM.LstLastUpdatedById = UserId;
                STVM.LsrResponseById = UserId;
                STVM.LsrResponseByName = "";
                STVM.LsrResponseDateTime = DateTime.Now;
                STVM.LstStatus = "WIP";
                STVM.LsrTicketStatus = strCommand;
                STVM.Ordinal = 2;
                //if (iTeamID > 0) STVM.LstTeamId = iTeamID;
                RestClient.UpdateTicket(STVM, "BackToL1", string.Empty, string.Empty, string.Empty);
                ViewBag.LstCategoryID = GetCategoryList();
                return RedirectToAction("Index");
            }
            if(strCommand== "BackToRequestor")
            {
                STVM.LstLastUpdatedDateTime = DateTime.Now;
                STVM.LstLastUpdatedById = UserId;
                STVM.LsrResponseById = UserId;
                STVM.LsrResponseByName = "";
                STVM.LsrResponseDateTime = DateTime.Now;
                STVM.LstStatus = "WIP";
                STVM.LsrTicketStatus = strCommand;
                STVM.Ordinal = 1;
                //if (iTeamID > 0) STVM.LstTeamId = iTeamID;
                RestClient.UpdateTicket(STVM, "BackToRequestor", string.Empty, string.Empty, string.Empty);
                ViewBag.LstCategoryID = GetCategoryList();
                return RedirectToAction("Index");

            }
            
            return View(STVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
    
        public ActionResult ForwardTicket(LSupportTicketContextModel STVM, string strCommand)
        {
            if (!string.IsNullOrEmpty(STVM.LsrDescription))
            {
                var apidata = RestClient.GetTeamDetail(iRoleId);
                if (apidata == null)
                    apidata = System.Web.HttpContext.Current.Session["UserRole"] as string;
                STVM.LsrDescription = "["+DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm")+ "] ["+UserName+"] ["+apidata+"]: " + STVM.LsrDescription;
            }
            STVM.LstLastUpdatedDateTime = DateTime.Now;
            STVM.LstLastUpdatedById = UserId;
            STVM.LsrResponseById = UserId;
            STVM.LsrResponseByName = "";
            STVM.LsrResponseDateTime = DateTime.Now;
            STVM.LsrTicketStatus = strCommand;
            if(strCommand=="ForwardToL1")
            {
                STVM.LstStatus = "WIP";
                //STVM.LstTeamId = GetTeamID("L1 " + STVM.LstType);
                STVM.LstCurrentOwnerId = STVM.LstL1Id;
                STVM.Ordinal = 2;
            }

            if (strCommand == "ForwardToL2")
            {
                STVM.LstStatus = "WIP";
                STVM.LstCurrentOwnerId = STVM.LstL2Id;
                STVM.LstTeamId = GetTeamID("L2 " + STVM.LstType);
                STVM.Ordinal = 3;
            }
            
            try
            {
                RestClient.UpdateTicket(STVM,"Forward", string.Empty, string.Empty, string.Empty);
                ViewBag.LstCategoryID = GetCategoryList();
                ViewBag.Message = "Ticket forwarded!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];
                ViewBag.LstCategoryID = GetCategoryList();
                return View(STVM);
                throw;
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
    
        public ActionResult SelfAssignTicket(LSupportTicketContextModel STVM)
        {
            STVM.LstLastUpdatedDateTime = DateTime.Now;
            STVM.LstLastUpdatedById = UserId;
            STVM.LsrResponseById = UserId;
            STVM.LsrResponseByName = "";
            STVM.LstStatus = "WIP";
            STVM.LsrTicketStatus = "SelfAssign";
            STVM.LsrResponseDateTime = DateTime.Now;
            if (strUserRole == "System Analyst")
            {
                STVM.LstL1Id = UserId;
                STVM.LstaAssignedToId = UserId;
                STVM.Ordinal = 2;
            }
            if (strUserRole.Contains("L2"))
            {
                STVM.LstL2Id = UserId;
                STVM.Ordinal = 3;
            }
            if (strUserRole.Contains("L3"))
                STVM.LstL3Id = UserId;
            STVM.LstCurrentOwnerId = UserId;
            STVM.LstaAssignedToId = UserId;
            try
            {
                STVM.LsrDescription = "SelfAssign";
                RestClient.UpdateTicket(STVM, "SelfAssign", string.Empty, string.Empty, string.Empty);
                ViewBag.LstCategoryID = GetCategoryList();
                ViewBag.Message = "Ticket assigned to you!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];
                ViewBag.LstCategoryID = GetCategoryList();
                return View(STVM);
                throw;
            }

        }

       
        public ActionResult SelfAssignTicket(int id)
        {
            LSupportTicketContextModel STVM = RestClient.GetById(id);
            STVM.LstLastUpdatedDateTime = DateTime.Now;
            STVM.LstLastUpdatedById = UserId;
            STVM.LsrResponseById = UserId;
            STVM.LsrResponseByName = "";
            STVM.LstStatus = "WIP";
            STVM.LsrTicketStatus = "SelfAssign";
            var apidata = RestClient.GetTeamDetail(iRoleId);
            if (apidata == null)
                apidata = System.Web.HttpContext.Current.Session["UserRole"] as string;
            STVM.LsrDescription = "[" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] [" + UserName + "] [" + apidata + "]:" + "SelfAssign";
            
            STVM.LsrResponseDateTime = DateTime.Now;
            if (strUserRole == "System Analyst")
            {
                STVM.LstL1Id = UserId;
                STVM.Ordinal = 2;
            }
            if (strUserRole.Contains("L2"))
            {
                STVM.LstL2Id = UserId;
                STVM.Ordinal = 3;
            }
            if (strUserRole.Contains("L3"))
                STVM.LstL3Id = UserId;
            STVM.LstCurrentOwnerId = UserId;
            STVM.LstaAssignedToId = UserId;
            try
            {
                RestClient.UpdateTicket(STVM, "SelfAssign", string.Empty, string.Empty, string.Empty);
                ViewBag.LstCategoryID = GetCategoryList();
                ViewBag.Message = "Ticket assigned to you!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];
                ViewBag.LstCategoryID = GetCategoryList();
               // return View(STVM);
                throw;
            }
        }

        [HttpGet]
        //[ControllerActionFilter]
        public ActionResult AssignTo(int TransactionId, string tckType)
        {
            LSupportTicketsRestClient RestClient = new LSupportTicketsRestClient();
            string TeamName = "L2 " + tckType;
            var AssigneeList = RestClient.GetL2AssigneeList(TeamName);
            ViewBag.Assignees = new SelectList(AssigneeList, "Id", "FullName");
            ViewBag.TransactionId = TransactionId;
            TempData["AssigneeList"] = AssigneeList;
            return View();
        }

        //This method will make the user as current owner whose id is passed as parameter
        [HttpPost]
        [ControllerActionFilter]
        public ActionResult AssignTo(string Assignees, int TransactionId)
        {
            var AssigneeList = TempData["AssigneeList"] as List<AssigneeListViewModel>;
            var AssineeId = Assignees;
            var AssigneeName = AssigneeList.Where(p => p.Id == Assignees).FirstOrDefault().FullName;

            LSupportTicketContextModel STVM = RestClient.GetById(TransactionId);
            STVM.LstLastUpdatedDateTime = DateTime.Now;
            STVM.LstL2Id = Assignees;
            STVM.Ordinal = 3;
            STVM.LstCurrentOwnerId = Assignees;
            STVM.LstStatus = "WIP";
            STVM.LstLastUpdatedById = UserId;
            STVM.LstTeamId = GetTeamID("L2 " + STVM.LstType);

            STVM.LsrSupportTicketId = TransactionId;
            STVM.LsrResponseById = UserId;
            STVM.LsrResponseByName = UserName;
            STVM.LsrTicketStatus = "AssignedToL2";
            var apidata = RestClient.GetTeamDetail(iRoleId);
            if (apidata == null)
                apidata = System.Web.HttpContext.Current.Session["UserRole"] as string;
            STVM.LsrDescription = "[" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "] [" + UserName + "] [" + apidata + "]:" + "AssignedToL2: " + AssigneeName;

            STVM.LsrResponseDateTime = DateTime.Now;

            STVM.LstaSupportTicketId = TransactionId;
            STVM.LstaAssignedToId = Assignees;
            STVM.LstaSupportTeamId = GetTeamID("L2 " + STVM.LstType);
            STVM.LstLastUpdatedUserName = UserName;
            STVM.LstaAssignedById = UserId;

            try
            {
                RestClient.UpdateTicket(STVM, "AssignedToL2", string.Empty, string.Empty, string.Empty);
                ViewBag.LstCategoryID = GetCategoryList();
                ViewBag.Message = "Ticket assigned to " + AssigneeName +"  !";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"];
                ViewBag.LstCategoryID = GetCategoryList();
                // return View(STVM);
                throw;
            }
        }


        public ActionResult Review(int Id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Review Ticket";

            LSupportTicketContextModel model = RestClient.GetById(Id);
            LUserViewModel Lu = UserRestClient.GetUserDetailsByAspNetUserID(model.LstCreatedById);
            model.LstRequestor = Lu.LuFirstName + " " + Lu.LuLastName;
            model.LstCreatedByUserName = model.LstRequestor;
            model.LstEmail = Lu.LuEmail;
            //Bind Description
            model.LstExDescription = model.LsrDescription;//BuildExistingTicketDescription(model.Id);
            //reset Description
            model.LsrDescription = null;
            ViewBag.LstCategoryID = GetCategoryList();
            ViewBag.LstStageID = GetTicketStageList(null);
            ViewBag.strSupportLevel = strSupportLevel;
            ViewBag.LstTicketStatus = model.LstStatus;
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        public AttachedFilesViewModel AttachSupportTicketFiles(HttpPostedFileBase[] File1)
        {
            var UserName = System.Web.HttpContext.Current.Session["UserName"];
            string FileName = "";
            var CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //SS using same Document path as AttachedClaimDocumentPath is mapped with S Drive
            string filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["AttachedClaimDocumentPath"], System.Web.HttpContext.Current.Session["CompanyCode"] + "/SupportTickets/SupportingDocuments");

            foreach (HttpPostedFileBase file in File1)
            {
                if (file.ContentLength > 0)
                {

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

                   
                    //made a comma seperated list of  file names
                    if (string.IsNullOrEmpty(FileName))
                    {
                        FileName = FileNames;
                    }
                    else
                    {
                        FileName = FileName + "," + FileNames;
                    }
                }
            }

            return new AttachedFilesViewModel { FileName = FileName, FilePath = filePath };
        }

        public bool ColumnToBeExportedInExcel(string ColumnName)
        {
            var ExcludedColName = new string[] {string.Empty, "FirstDescription", "Email","ExDescription","QuickTicketID","CreatedOnBehalfOfId","LastUpdatedById","CategoryID","LastUpdatedUserName","LastUpdatedUserName","CompanyId","CC","TeamId","ClosureCode","Id","L1Id", "L2Id", "L3Id", "Severity", "Impact", "CurrentOwnerId", "Ordinal", "CatagoryID", "UpdatedUserName", "CreatedByUserName", "Requestor", "QuickTicketId", "QuickTicketName", "SearchTicketFromDate", "SearchTicketToDate", "TicketSearchString" };
            if(ExcludedColName.Contains(ColumnName))
            {
                return false;
            }
            return true;
        }

        public JsonResult GetSupportTicketSummaryCounts()
        {
            var SummaryCount = RestClient.GetSupportTicketSummaryCounts();
            return Json(SummaryCount, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSupportTicketSummaryForDashBoard(string sortdatafield, string sortorder, Nullable<int> pagesize, Nullable<int> pagenum)
        {
            if (pagesize == null) pagesize = 0;
            if (pagenum == null) pagenum = 0;
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetSupportTicketSummaryForDashBoard(sortdatafield, sortorder, Convert.ToInt32(pagesize), Convert.ToInt32(pagenum), FilterQuery);
            
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        //GetSupportTicketChart
        public JsonResult GetSupportTicketChart()
        {
            var ApiData = RestClient.GetSupportTicketChart();

            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSupportTicketDetailCounts()
        {
            var SummaryCount = RestClient.GetSupportTicketDetailCounts();
            return Json(SummaryCount, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSupportTicketDetailForDashBoard(int pagesize, int pagenum, string sortdatafield, string sortorder)
        {
            var qry = Request.QueryString;
            var FilterQuery = Globals.BuildQuery(qry);
            var ApiData = RestClient.GetSupportTicketDetailForDashBoard(pagesize, pagenum, sortdatafield, sortorder, FilterQuery);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetClosedTicketsCount()
        //{ 
        //    var SummaryCount = RestClient.GetClosedTicketsCount();
        //    return Json(SummaryCount, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetClosedTicketsData(int pagesize, int pagenum, string sortdatafield, string sortorder)
        {
            
            var ApiData = RestClient.GetClosedTicketsData( );
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }

         
        [HttpPost]
        public async Task<JsonResult> UploadHomeReport(string id, string Type)
        {
            AttachedFilesViewModel FileDetails = new AttachedFilesViewModel();
          
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    HttpPostedFileBase hpf = fileContent;
                    AttachedFilesViewModel FileDataDetails = AttachSupportTicketFiles1(hpf);
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

                string Data = RestClient.UpdateAttachment(Convert.ToInt32(id),UserId, FileDetails.FileName, FileDetails.FilePath, Type);
                return Json(Data, JsonRequestBehavior.AllowGet);
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

        public AttachedFilesViewModel AttachSupportTicketFiles1(HttpPostedFileBase file)
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
                    string FileNamesReturn = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") ;


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
                

            return new AttachedFilesViewModel { FileName = FileNames, FilePath = filePath };
        }
    }
}