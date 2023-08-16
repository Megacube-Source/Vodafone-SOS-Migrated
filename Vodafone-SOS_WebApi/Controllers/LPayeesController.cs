using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data.Entity.Validation;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Data.OleDb;

namespace Vodafone_SOS_WebApi.Controllers
{
    [AllowAnonymous]
    [CustomExceptionFilter]
    public class LPayeesController : AccountController //ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        ////Global Method to get Payee Hierarchy Data based on roles
        //private IEnumerable<dynamic> GetPayeeHierechyList(string UserRole,string LoggedInUserId,int CompanyId)
        //{

        //    return PayeeDataToBeReturned;
        //}

        //This method will download Payee Record To an excel based on Role loggedIn
        [HttpGet]
        public IHttpActionResult DownloadPayeeReport(int CompanyId, string LoggedInUserId, string UserRole, string PortfolioList, string LoggedInUserName, bool DownloadReportData)
        {
            var CompanyDetails = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
            //Get Company Specific Column
            DataSet ds = new DataSet();
            var CompanySpecificColumn = db.LCompanySpecificColumns.Where(p => p.LcscCompanyId == CompanyId).Where(p => p.LcscTableName == "LPayees").OrderByDescending(p => p.LcscDisplayOnForm).ThenBy(p => p.LcscOrdinalPosition).ToList();
            var PortfolioData = new[] { new { MepPortfolioId = 0, RcName = string.Empty, LpBusinessUnit = string.Empty, RcPrimaryChannel = string.Empty, LpPayeeCode = string.Empty } }.ToList();
            //If the flag to download data along with report is checked then only data is fetched from DB to be written in a file
            if (DownloadReportData)
            {
                //Section to get Payee Data from Stored Procedure starts Here
                var Query = "Exec SPGetPayeeData @UserRole,@LoggedInUserId,@CompanyId,@PortfolioList,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,null";
                SqlCommand cmd = new SqlCommand(Query);
                cmd.Parameters.AddWithValue("@PortfolioList", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@UserRole", UserRole);
                cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
                cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                cmd.Parameters.AddWithValue("@sortdatafield", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@sortorder", System.DBNull.Value);
                cmd.Parameters.AddWithValue("@pagesize", 99999);
                cmd.Parameters.AddWithValue("@pagenum", 0);
                cmd.Parameters.AddWithValue("@FilterQuery", (object)System.DBNull.Value);
                ds = Globals.GetData(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //Section to get Payee Data Ends Here
                    var EntityArray = ds.Tables[0].AsEnumerable().Select(p => p.Field<int>("Id")).ToList();
                    PortfolioData = (from aa in db.MEntityPortfolios.Where(p => p.MepEntityType.Equals("LPayees", StringComparison.OrdinalIgnoreCase)).Where(p => EntityArray.Contains(p.MepEntityId)).Include(p => p.LPortfolio.RChannel)
                                     join bb in db.LPayees on aa.MepEntityId equals bb.Id
                                     select new { aa.MepPortfolioId, aa.LPortfolio.RChannel.RcName, aa.LPortfolio.LpBusinessUnit, aa.LPortfolio.RChannel.RcPrimaryChannel, bb.LpPayeeCode }).OrderBy(x=>x.LpPayeeCode).ThenBy(x=>x.RcPrimaryChannel).ThenBy(x => x.RcName).ThenBy(x=>x.LpBusinessUnit).ToList();
                }
            }
            //var ListOfPayeeIds = string.Join(",", PayeeData.Select(r => r.Id).ToList());
            ////GetParent List
            //var ParentList = LPARC.GetParentListByPayeeId(ListOfPayeeIds);
            //Get Portfolio Data
            // var PortfolioData = LPORC.GetByEntityIdList("LPayees", ListOfPayeeIds);
            //Generate a Workbook having Sheet to contain data as wellas Portfolios
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Payees");
            IRow row1 = sheet1.CreateRow(0);
            //First row of  headers sameas DB columns
            //Extra Columns added by RK
            ICell Extracell = row1.CreateCell(0);
            Extracell.SetCellValue("GiveCMRole");
            Extracell = row1.CreateCell(1);
            Extracell.SetCellValue("LpParentCode");
            Extracell = row1.CreateCell(2);
            Extracell.SetCellValue("LpPrimaryChannel");
            Extracell = row1.CreateCell(3);
            Extracell.SetCellValue("WFComments");
            string IsWIAMEnabled = db.Database.SqlQuery<string>("select [dbo].[FNIsWIAMEnabled]({0})", CompanyId).FirstOrDefault();
            int iExclColIndex = 0;
            for (int j = 0; j < CompanySpecificColumn.Count(); j++)
            {
                
                if (IsWIAMEnabled.ToLower() == "yes" && CompanySpecificColumn.ElementAt(j).LcscColumnName.ToLower() == "createlogin")
                { }
                else
                {
                    string columnName = CompanySpecificColumn.ElementAt(j).LcscColumnName;
                    ICell cell = row1.CreateCell(iExclColIndex + 4);

                    if (columnName != "WFComments")
                    {
                        if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                            columnName = CompanySpecificColumn.ElementAt(j).LcscColumnName;
                        else
                            columnName = "Lp" + CompanySpecificColumn.ElementAt(j).LcscColumnName;
                        cell.SetCellValue(columnName);
                        iExclColIndex++;
                    }
                }
                // GC is used to avoid error System.argument exception
                GC.Collect();
            }
            if (IsWIAMEnabled.ToLower() == "yes")
            {
                ICell cell2 = row1.CreateCell(iExclColIndex + 4);//RK So that if WIAM is enabled, create login column will appear at last in the 1st row
                cell2.SetCellValue("LpCreateLogin");
            }

            row1.ZeroHeight = true;
            IRow row2 = sheet1.CreateRow(1);

            //Extra Columns added by RK
            Extracell = row2.CreateCell(0);
            Extracell.SetCellValue("GiveCMRole");
            Extracell = row2.CreateCell(1);
            Extracell.SetCellValue("Parent Code");
            Extracell = row2.CreateCell(2);
            Extracell.SetCellValue("Primary Channel *");
            Extracell = row2.CreateCell(3);
            Extracell.SetCellValue("Comments");
            iExclColIndex = 0;
            //Second row of  headers sameas Company Specific Labels
            for (int j = 0; j < CompanySpecificColumn.Count(); j++)
            {
                string ColumnLabel = "";
                string columnName = CompanySpecificColumn.ElementAt(j).LcscColumnName;
                if (IsWIAMEnabled.ToLower() == "yes" && columnName.ToLower() == "createlogin")
                { }
                else
                {
                    
                    ICell cell = row2.CreateCell(iExclColIndex + 4);
                    if (columnName != "WFComments")
                    {
                        if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                            columnName = CompanySpecificColumn.ElementAt(j).LcscColumnName;
                        else
                            columnName = "Lp" + CompanySpecificColumn.ElementAt(j).LcscColumnName;
                        ColumnLabel = CompanySpecificColumn.ElementAt(j).LcscLabel;
                        if (string.IsNullOrEmpty(ColumnLabel))
                            ColumnLabel = CompanySpecificColumn.ElementAt(j).LcscColumnName;
                        //Check for displayOnForm and IsManadatory
                        if (CompanySpecificColumn.ElementAt(j).LcscDisplayOnForm)
                        {
                            if (CompanySpecificColumn.ElementAt(j).LcscIsMandatory == true)
                            {
                                if (columnName == "LpEffectiveStartDate" || columnName == "LpEffectiveEndDate")
                                    cell.SetCellValue(ColumnLabel + " * (dd/MM/yyyy)");
                                else
                                {
                                    if (IsWIAMEnabled.ToLower() == "yes" && columnName.ToLower() == "lpemail")
                                    {
                                        cell.SetCellValue(ColumnLabel);
                                    }
                                    else
                                    {
                                        cell.SetCellValue(ColumnLabel + " *");
                                    }
                                }

                            }
                            else
                            {
                                cell.SetCellValue(ColumnLabel);
                                if (IsWIAMEnabled.ToLower() == "no" && columnName.ToLower() == "lpemail")
                                {
                                    cell.SetCellValue(ColumnLabel + " *");
                                }
                                
                            }
                            //cell.SetCellValue(ColumnLabel);
                        }
                        iExclColIndex++;
                    }
                }
                

                // GC is used to avoid error System.argument exception
                GC.Collect();
            }
            //If the flag to download data along with report is checked then only data is looped through and written in file
            if (DownloadReportData)
            {
                //loops through data  
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 2);
                    //Extra Columns added by RK
                    Extracell = row.CreateCell(0);
                    Extracell.SetCellValue(string.IsNullOrEmpty(ds.Tables[0].Rows[i].Field<string>("LpFinOpsRoles")) ? "N" : "Y");
                    Extracell = row.CreateCell(1);
                    Extracell.SetCellValue(ds.Tables[0].Rows[i].Field<string>("ParentCode"));
                    Extracell = row.CreateCell(2);
                    Extracell.SetCellValue(ds.Tables[0].Rows[i].Field<string>("LpPrimaryChannel"));
                    Extracell = row.CreateCell(3);
                    Extracell.SetCellValue(ds.Tables[0].Rows[i].Field<string>("WFComments"));
                    int iColIndex = 0;
                    for (int j = 0; j < CompanySpecificColumn.Count(); j++)
                    {
                        if (IsWIAMEnabled.ToLower() == "yes" && CompanySpecificColumn.ElementAt(j).LcscColumnName.ToLower() == "createlogin")
                        { }
                        else
                        {
                            //iColIndex = j;
                            ICell cell = row.CreateCell(iColIndex + 4);
                            string columnName = CompanySpecificColumn.ElementAt(j).LcscColumnName;
                            if (columnName != "WFComments")
                            {

                                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                    columnName = CompanySpecificColumn.ElementAt(j).LcscColumnName;
                                else
                                    columnName = "Lp" + CompanySpecificColumn.ElementAt(j).LcscColumnName;
                                if (CompanySpecificColumn.ElementAt(j).LcscDisplayOnForm)
                                {
                                    //check for boolean and datetime columns
                                    if ((columnName == "LpEffectiveStartDate" || columnName == "LpEffectiveEndDate") && !string.IsNullOrEmpty((ds.Tables[0].Rows[i][columnName].ToString())))
                                        cell.SetCellValue(Convert.ToDateTime(ds.Tables[0].Rows[i][columnName].ToString()).ToString("dd/MM/yyyy"));
                                    else if (columnName == "LpCanRaiseClaims" || columnName == "LpCreateLogin" || columnName == "LpCreatedByForm") //RK 14122018 Removed block notification column from here || columnName == "LpBlockNotification")
                                        cell.SetCellValue((Convert.ToBoolean(ds.Tables[0].Rows[i][columnName].ToString())) ? "Y" : "N");
                                    else if (columnName == "LpChannelManager" && !string.IsNullOrEmpty((ds.Tables[0].Rows[i][columnName].ToString())))
                                    {
                                        var ChannelManagerId = ds.Tables[0].Rows[i][columnName] as string;
                                        cell.SetCellValue(db.AspNetUsers.Where(p => p.Id == (ChannelManagerId)).Select(p => p.Email).FirstOrDefault());
                                    }
                                    else
                                        cell.SetCellValue(ds.Tables[0].Rows[i][columnName].ToString());
                                }
                                iColIndex++;
                            }
                        }
                        

                    }
                }
            }
                //Portfolio Sheet
                ISheet sheetPort = workbook.CreateSheet("Portfolios");
                IRow rowPort = sheetPort.CreateRow(0);
                ICell Portfoliocell = rowPort.CreateCell(0);
                Portfoliocell.SetCellValue("Payee Code");
                Portfoliocell = rowPort.CreateCell(1);
                Portfoliocell.SetCellValue("Primary Channel");
                Portfoliocell = rowPort.CreateCell(2);
                Portfoliocell.SetCellValue("Channel");
                Portfoliocell = rowPort.CreateCell(3);
                Portfoliocell.SetCellValue("Business Unit");
            //If the flag to download data along with report is checked then only data is looped through and written in file
            if (DownloadReportData)
            {
                //loops through Portfolio 
                for (int i = 0; i < PortfolioData.Count(); i++)
                {
                    IRow row = sheetPort.CreateRow(i + 1);
                    Portfoliocell = row.CreateCell(0);
                    Portfoliocell.SetCellValue(PortfolioData.ElementAt(i).LpPayeeCode);
                    Portfoliocell = row.CreateCell(1);
                    Portfoliocell.SetCellValue(PortfolioData.ElementAt(i).RcPrimaryChannel);
                    Portfoliocell = row.CreateCell(2);
                    Portfoliocell.SetCellValue(PortfolioData.ElementAt(i).RcName);
                    Portfoliocell = row.CreateCell(3);
                    Portfoliocell.SetCellValue(PortfolioData.ElementAt(i).LpBusinessUnit);
                }

            }
            //var FileName = CompanyDetails.GcCode+"_MyPayeesReport.xlsx";
            string FileName = "Mypayees_"+ LoggedInUserName + ".xlsx";//shortened the name because it throws IOException "File Already Exists" if the path\filename is too long.
            string FilePath = ConfigurationManager.AppSettings["SOSBucketFolder"] + "/" + CompanyDetails.GcCode + "/" + LoggedInUserName + "/" + FileName;
            string TempPath = ConfigurationManager.AppSettings["SOSTempPath"] ;
            #region commneted R3.1
            //R3.1 SG - 14082020 -not required
            //if (System.IO.File.Exists(FilePath))
            //{
            //    System.IO.File.Delete(FilePath);
            //}
            //if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyDetails.GcCode + "/" + LoggedInUserName))
            //{
            //    System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyDetails.GcCode + "/" + LoggedInUserName);
            //}
            //FileStream xfile = new FileStream(FilePath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            //workbook.Write(xfile);
            //delete file if it exist
            //if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyDetails.GcCode + "/" + LoggedInUserName))//create directory if not present
            //{
            //    System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyDetails.GcCode + "/" + LoggedInUserName);
            //}
            //if (System.IO.File.Exists(Path.Combine(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyDetails.GcCode + "/" + LoggedInUserName, FileName)))
            //{
            //    System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyDetails.GcCode + "/" + LoggedInUserName, FileName));
            //}
            #endregion
            //Create folder if it does not exist
            if (!Globals.FolderExistsInS3(LoggedInUserName, CompanyDetails.GcCode))
            {
                Globals.CreateFolderInS3(LoggedInUserName, CompanyDetails.GcCode);
            }
            if (Globals.FileExistsInS3(FileName, LoggedInUserName, CompanyDetails.GcCode))
            {
                Globals.DeleteFileFromS3(FileName, CompanyDetails.GcCode, LoggedInUserName);
            }
            //still using Cloudberry s:\ filstream while creating xls needs proper windows path instead of aws sdk
            //string SDrivePath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + ConfigurationManager.AppSettings["SOSBucketFolder"] + "/" + CompanyDetails.GcCode + "/" + LoggedInUserName;
            if (File.Exists(Path.Combine(TempPath, FileName)))
            {
               File.Delete(Path.Combine(TempPath, FileName));
            }
            //FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyDetails.GcCode + "/" + LoggedInUserName, FileName), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            FileStream xfile = new FileStream(Path.Combine(TempPath, FileName), FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            xfile.Close();
            //R3.1 SG - 14082020 - not required
            //using (StreamReader sr = new StreamReader(Path.Combine(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyDetails.GcCode + "/" + LoggedInUserName, FileName)))
            using (StreamReader sr = new StreamReader(Path.Combine(TempPath, FileName)))
            {
                Globals.UploadToS3(sr.BaseStream, FileName, LoggedInUserName, CompanyDetails.GcCode);
            }
            //Now Delete the temporary file created in C drive
            if (File.Exists(Path.Combine(TempPath, FileName)))
            {
                File.Delete(Path.Combine(TempPath, FileName));
            }
            return Ok(FileName);
        }

        [HttpGet]  /*added for payeeuploadhelp*/
        public IHttpActionResult GetPayeeUploadHelp()
        {
            var xx = (from aa in db.GKeyValues
                      where aa.GkvKey == "PayeeUploadHelp"
                      select new
                      {
                          aa.GkvValue
                      }).FirstOrDefault();
            return Ok(xx);



        }

        //This method will return Parent Payee List as per Payee Id passed//
        [HttpPost]
        public IHttpActionResult GetParentListByPayeeId(PayeeIdListViewModel model)
        {
            var PayeeIdArray = model.PayeeIdList.Split(',').ToList();
            int? ParentId = null;
            List<string> ParentCodeList = new List<string>();
            foreach (var Id in PayeeIdArray)
            {
                //Get Parent Id fromDB function
                var RawQuery = db.Database.SqlQuery<Nullable<Int32>>("select dbo.GetPayeeParentId(" + Id + ",'" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "')");
                var Task = RawQuery.SingleAsync();
                ParentId = Task.Result;
                if (ParentId.HasValue)
                {
                    var ParentCode = db.LPayees.Where(p => p.Id == ParentId).FirstOrDefault();
                    ParentCodeList.Add(ParentCode.LpPayeeCode);
                }
                else
                {
                    ParentCodeList.Add(string.Empty);
                }
            }
            return Ok(ParentCodeList);
        }
        //This method will validate Payee based on parenPayee validations
        public IHttpActionResult GetParentPayeeValidationResult(string PayeeCode, string ParentCode, string PrimaryChannel, string EffectiveDate, int CompanyId)
        {
            var result = db.Database.SqlQuery<string>("Exec SPValidatePayeeParent {0},{1},{2},{3},{4} ", PayeeCode, ParentCode, PrimaryChannel, EffectiveDate, CompanyId).FirstOrDefault();
            return Ok(result);
        }
        //This method return a value whether payee iss authorized to enter a claim

        [HttpGet]
        public bool CanRaiseClaims(string PayeeUserId, string UserName, string Workflow)
        {
            bool CanRaiseClaim = false;
            var CanRaiseClaims = db.LPayees.Where(p => p.LpUserId == PayeeUserId).FirstOrDefault();
            if (CanRaiseClaims != null)
            {
                CanRaiseClaim = CanRaiseClaims.LpCanRaiseClaims;
            }
            return CanRaiseClaim;
        }

        [HttpGet]
        public bool CheckEmailExists(string strEmailID, string UserName, string Workflow)
        {
            bool blnReturnVal = false;
            var EmailExists = db.AspNetUsers.Any(x => x.Email.ToLower() == strEmailID.ToLower());
            if (EmailExists)
            {
                blnReturnVal = true;
            }
            else
            {
                blnReturnVal = false;
            }

            if (!blnReturnVal)
            {
                var EmailExists1 = db.LPayees.Any(p => p.LpEmail.ToLower() == strEmailID.ToLower() && (p.WFStatus.ToLower() == "saved" || p.WFStatus.ToLower() == "inprogress" || p.WFStatus.ToLower() == "completed"));
                if (EmailExists1)
                    blnReturnVal = true;
            }

            return blnReturnVal;

        }

        public IHttpActionResult GetPayeeInformationByUserId(string PayeeUserId, string UserName, string Workflow)
        {

            var PayeeData = Globals.ExecuteSPGetFilteredPayeeResults(null, null, null, null, null, null, null, null, null, null, null, PayeeUserId);//db.LPayees.Where(p => p.LpUserId == PayeeUserId).FirstOrDefault().Id;
            var PayeeId = PayeeData.Rows[0].Field<int>("Id");
            // var PayeeDetails = db.LPayees.Find(PayeeId);
            bool IsParent = false;
            if (db.LPayeeParents.Where(p => p.LppParentPayeeId == PayeeId).Count() > 0)
            {
                IsParent = true;
            }
            var xx = new UploadPayeeViewModel { LpLastName = Convert.ToString(PayeeData.Rows[0].Field<string>("LpLastName")), LpFirstName = Convert.ToString(PayeeData.Rows[0].Field<string>("LpFirstName")), Id = PayeeData.Rows[0].Field<int>("Id"), LpPayeeCode = PayeeData.Rows[0].Field<string>("LpPayeeCode") };
            return Ok(xx);
        }

        //public IHttpActionResult GetPayeeFilesByPayeeCode(int PayeeId, string UserName, string Workflow)
        //{
        //    var PayeeDetails = db.LPayees.Where(p => p.Id == PayeeId).Select(p => new { p.LpFileNames, p.LpUserFriendlyFileNames, p.Id, p.LpPayeeCode }).FirstOrDefault();
        //    return Ok(PayeeDetails);
        //}

        public IHttpActionResult GetPayeeDetailsByPayeeCode(string PayeeCode, int CompanyId, string UserName, string Workflow)
        {
            // var PayeeDetails = db.LPayees.Where(p => p.LpPayeeCode == PayeeCode).Where(p => p.LpCompanyId == CompanyId).FirstOrDefault();
            //Getting DecryptedData from SP
            var PayeeDetails = Globals.ExecuteSPGetFilteredPayeeResults(null, null, CompanyId, null, null, PayeeCode, null, null, null, null, null, null);//db.LPayees.Where(p => p.LpUserId == PayeeUserId).FirstOrDefault().Id;

            bool IsParent = false;
            if (PayeeDetails.Rows[0].Field<int?>("LppParentPayeeId") != null)
            {
                IsParent = true;
            }
            var result = new PayeeResultViewModel { Id = PayeeDetails.Rows[0].Field<int>("Id"), FullName = PayeeDetails.Rows[0].Field<string>("LpFirstName") + " " + PayeeDetails.Rows[0].Field<string>("LpLastName") + " (" + PayeeDetails.Rows[0].Field<string>("LpPayeeCode") + ")", LpPayeeCode = PayeeDetails.Rows[0].Field<string>("LpPayeeCode"), LpFirstName = Convert.ToString(PayeeDetails.Rows[0].Field<string>("LpFirstName")), LpLastName = Convert.ToString(PayeeDetails.Rows[0].Field<string>("LpLastName")) };//SS Decrypted
            return Ok(result);
        }
        // GET: api/LPayees
        //This code is commented as we will not get data without companyId filter
        //public IHttpActionResult GetLPayees()
        //{
        //    var xx = (from aa in db.LPayees.Include(p => p.RChannel).Include(p => p.RSubChannel) select new {aa.Id,aa.LpAddress,aa.LpChannelId,aa.LpComments,aa.LpEffectiveEndDate,aa.LpEffectiveStartDate,aa.LpEmail,aa.LpFirstName,aa.LpLastName,
        //      aa.LpCompanyId,aa.LpPayeeCode,aa.LpPhone,aa.LpChannelManager,aa.LpTIN,aa.LpPrimaryChannel,aa.LpTradingName,aa.LpDistributionChannel,aa.LpStatus }).OrderBy(p=>p.LpTradingName).ThenBy(p=>p.LpFirstName).ThenBy(p=>p.LpLastName);
        //    return Ok(xx);
        //}
        [HttpGet]
        //Get the value of IsWIAMEnabled from GkeyValue
        public IHttpActionResult CheckIsWIAMEnabled(int CompanyId)
        {
            string IsWIAMEnabled = db.Database.SqlQuery<string>("select [dbo].[FNIsWIAMEnabled]({0})", CompanyId).FirstOrDefault();
            return Ok(IsWIAMEnabled);
        }

        //method to return columns of lpayees table to be displayed in company specific columns grid
        public IHttpActionResult GetLPayeesColumnsForGrid(string UserName, string Workflow)
        {
            var xx = db.Database.SqlQuery<CompanySpecificColumnViewModel>("select Replace(COLUMN_NAME ,'Lp','') as ColumnName,IS_NULLABLE as IsNullable from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='LPayees'and Column_Name not in ('Id','LpUserId','LpCompanyId','LpStatusId','LpCreatedById','LpUpdatedById','LpBatchNumber','LpComments','LpFileNames','LpUserFriendlyFileNames','LpCreatedDateTime','LpUpdatedDateTime','LpStatus','LpBatchNumber','WFRequesterId','WFAnalystId','WFManagerId','WFOrdinal','WFCurrentOwnerId','WFStatus','WFType','WFRequesterRoleId','WFCompanyId','WFComments','LpBusinessUnit','LpSubChannelId','LpChannelId','LpFinOpsRoles')");
            return Ok(xx);
        }

        //method to update payee list
        //[HttpPost]
        //public IHttpActionResult UpdatePayeeStatus(Sp_UpdateItemStatusViewModel model)
        //{
        //    try
        //    {
        //        string Comments = null;
        //        //to check if provide parameter is an empty string or not
        //        if (!string.IsNullOrEmpty(model.Comments))
        //        {
        //            Comments = model.Comments;
        //        }
        //        var QueryResult = db.Database.ExecuteSqlCommand("UPDATE LPayees SET LpStatus = '" + model.StatusName + "',LpUpdatedById = CASE WHEN '" + model.UpdatedBy + "' IS NULL THEN LpUpdatedById ELSE '" + model.UpdatedBy + "' END,LpUpdatedDateTime = CASE WHEN '" + model.UpdatedDateTime.ToString("yyyy-MM-dd") + "' IS NULL THEN LpUpdatedDateTime ELSE '" + model.UpdatedDateTime.ToString("yyyy-MM-dd") + "' END,LpComments = CASE WHEN '" + Comments + "' IS NULL THEN LpComments ELSE '" + Comments + "'+char(13)+char(10)+Convert(NvarChar(max),LpComments) END WHERE Id IN (" + model.ItemList + ") ");
        //        // db.SpUpdatePayeeData(model.ItemList,model.StatusName,model.Comments,model.UpdatedBy,model.UpdatedDateTime);

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
        //    }
        //}

        public IHttpActionResult GetApprovedPayeeForTree(string PayeeAsOfDate, int LpCompanyId, string PrimaryChannel, string UserName, string Workflow)
        {
            //RK13122018: SG Stopped calling of this method as this throws exception in concatinating names due to encryption.
            // var xx = db.Database.SqlQuery<LPayee>("WITH MyCTE AS ( SELECT A.Id, A.LpFirstName, A.LpLastName,null as LpParentPayeeId FROM LPayees A WHERE A.Id not in (select LppPayeeId from LPayeeParents) and A.LpCompanyId=" + LPayee.LpCompanyId + " and A.LpStatusId=(select Id from RStatuses where RsStatus='Approved'  and RsOwnerId=(select Id from RStatusOwners where RsoStatusOwner='Payees')) UNION ALL SELECT B.Id, B.LpFirstName, B.LpLastName, PP.LppParentPayeeId FROM LPayees B inner join LPayeeParents PP on B.Id=PP.LppPayeeId INNER JOIN MyCTE C ON PP.LppParentPayeeId = C.Id WHERE B.LpCompanyId=" + LPayee.LpCompanyId + " and " + LPayee.PayeeAsOfDate + " between PP.LppEffectiveStartDate and pp.LppEffectiveEndDate and B.LpStatusId=(select Id from RStatuses where RsStatus='Approved'  and RsOwnerId=(select Id from RStatusOwners where RsoStatusOwner='Payees')) ) SELECT * FROM MyCTE");
            //var Query = "select Id,LpPayeeCode,(LpFirstName+' '+isnull(LpLastName,'')) as FullName,LpPrimaryChannel,LpFirstName,LpLastName,dbo.GetPayeeParentId(Id,{0}) as LppParentPayeeId from LPayees where LpCompanyId={1} and LpPrimaryChannel={2} and  LpEffectiveStartDate<={3} and IsNull(LpEffectiveEndDate,Convert(date,DateAdd(day,360,GETDATE())))>={4} and WFStatus='Completed' order by LpTradingName,LpFirstName,LpLastName";
            //var Query = "select Id,LpPayeeCode, LpFirstName as FullName,LpPrimaryChannel,LpFirstName,LpLastName,dbo.GetPayeeParentId(Id,{0}) as LppParentPayeeId from LPayees where LpCompanyId={1} and LpPrimaryChannel={2} and  LpEffectiveStartDate<={3} and IsNull(LpEffectiveEndDate,Convert(date,DateAdd(day,360,GETDATE())))>={4} and WFStatus='Completed' order by LpTradingName,LpFirstName,LpLastName";
            //var xx = db.Database.SqlQuery<PayeeResultViewModel>(Query, PayeeAsOfDate, LpCompanyId, PrimaryChannel, PayeeAsOfDate, PayeeAsOfDate).ToList();
            //return Ok(xx);
            var Query = "Exec USPGetPayeeTreeData @PayeeAsOfDate,@LpCompanyId,@PrimaryChannel";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@PayeeAsOfDate", PayeeAsOfDate);
            cmd.Parameters.AddWithValue("@LpCompanyId", LpCompanyId);
            cmd.Parameters.AddWithValue("@PrimaryChannel", PrimaryChannel);
            
            DataSet ds = Globals.GetData(cmd);
            return Ok(ds.Tables[0]);

        }


        //PortfolioList Tree
        public IHttpActionResult GetApprovedPayeeForPortfolioTree(string PayeeAsOfDate, int LpCompanyId, string LoggedInUserId, string UserName, string Workflow)
        {
            //Payee List Based OnPortfolio matching from Loggedin user
            //--PortfolioTree
            //Section to get Payee Data from Stored Procedure starts Here
            var Query = "Exec SPGetPayeeData @UserRole,@LoggedInUserId,@CompanyId,@PortfolioList,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,null";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@PortfolioList", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@UserRole", "PortfolioTree");
            cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
            cmd.Parameters.AddWithValue("@CompanyId",LpCompanyId);
            cmd.Parameters.AddWithValue("@sortdatafield", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@sortorder", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@pagesize", 99999);
            cmd.Parameters.AddWithValue("@pagenum", 0);
            cmd.Parameters.AddWithValue("@FilterQuery", (object)System.DBNull.Value);
            DataSet ds = Globals.GetData(cmd);
            //var Query = "select ('P'+Convert(nvarchar,MepPortfolioId)) as PortfolioId,LP.Id,LP.LpPayeeCode,(LP.LpFirstName+' '+isnull(LP.LpLastName,'')) as FullName,LP.LpPrimaryChannel,LP.LpFirstName,LP.LpLastName,dbo.GetPayeeParentId(LP.Id,{0}) as LppParentPayeeId from LPayees LP inner join MEntityPortfolios MEP on (MEP.MepEntityType='LPayees' and MEP.MepEntityId=LP.Id) where LpCompanyId={1} and  LpEffectiveStartDate<={2} and IsNull(LpEffectiveEndDate,Convert(date,DateAdd(day,360,GETDATE())))>={2} and WFStatus='Completed' and MEP.MepPortfolioId in(select MepPortfolioId from MEntityPortfolios where MepEntityType='LUsers' and MepEntityId=(select Id from LUsers where LuUserId={3})) order by LpTradingName,LpFirstName,LpLastName";
            //var xx = db.Database.SqlQuery<PayeeResultViewModel>(Query, PayeeAsOfDate, LpCompanyId, PayeeAsOfDate, LoggedInUserId).ToList();
            return Ok(ds.Tables[0]);
        }

        //This method will return Payee List based on Portfolio matching 
        public IHttpActionResult GetApprovedPayeeCountsForClaimsDropdown(int CompanyId, string LoggedInUserId, string UserName, string Workflow, string PortfolioList,string UserRole)
        {
            //Section to get Payee Data from Stored Procedure starts Here
            var Query = "Exec SPGetPayeeData @UserRole,@LoggedInUserId,@CompanyId,@PortfolioList,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,null";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@PortfolioList", string.IsNullOrEmpty(PortfolioList) ? (object)System.DBNull.Value : PortfolioList);
            cmd.Parameters.AddWithValue("@UserRole", UserRole);
            cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@sortdatafield", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@sortorder", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@pagesize", 99999);
            cmd.Parameters.AddWithValue("@pagenum", 0);
            cmd.Parameters.AddWithValue("@FilterQuery", System.DBNull.Value);
            DataSet ds = Globals.GetData(cmd);
            return Ok(ds.Tables[0].Rows.Count);
        }


        //This method will return Payee List based on Portfolio matching 
        public IHttpActionResult GetApprovedPayeeForClaimsDropdown(int CompanyId, string LoggedInUserId, string UserName, string Workflow, string PortfolioList,string sortdatafield,string sortorder,int pagesize,int pagenum,string FilterQuery,string UserRole,string PayeeId)
        {
            /*Comment children payee section from all roles other than Payee, CM. 
             * That is for all other roles, only portfolio matched payees will be shown*/
            // var LUsers = db.LUsers.Where(p => p.LuUserId == LoggedInUserId).FirstOrDefault();
            //var Query = "";
            //if (string.IsNullOrEmpty(PortfolioList))
            //{
            //    // Query = "WITH MyCTE AS ( SELECT A.Id,(A.LpFirstName+' '+isnull(A.LpLastName,'')+' ('+A.LpPayeeCode+')') as FullName,null as LppParentPayeeId FROM LPayees A inner join MEntityPortfolios MEP on (MEP.MepEntityId=A.Id and MepEntityType='LPayees') WHERE A.Id not in (select LppPayeeId from LPayeeParents) and MEP.MepPortfolioId in (select MepPortfolioId from MEntityPortfolios where MepEntityId={0} and MepEntityType='LUsers') and A.LpCompanyId={1} and A.WFStatus='Completed' UNION ALL SELECT B.Id, (B.LpFirstName+' '+isnull(B.LpLastName,'')+' ('+B.LpPayeeCode+')') as FullName, PP.LppParentPayeeId FROM LPayees B inner join LPayeeParents PP on B.Id=PP.LppPayeeId INNER JOIN MyCTE C ON PP.LppParentPayeeId = C.Id WHERE B.LpCompanyId={2} and GETDATE() between PP.LppEffectiveStartDate and isnull(pp.LppEffectiveEndDate,DateAdd(day,30,GETDATE())) and B.WFStatus='Completed' ) SELECT distinct FullName,Id,LppParentPayeeId FROM MyCTE order by FullName";
            //    Query = "SELECT distinct A.Id,(A.LpFirstName+' '+isnull(A.LpLastName,'')+' ('+A.LpPayeeCode+')') as FullName,null as LppParentPayeeId FROM LPayees A inner join MEntityPortfolios MEP on (MEP.MepEntityId=A.Id and MepEntityType='LPayees') WHERE MEP.MepPortfolioId in (select MepPortfolioId from MEntityPortfolios where MepEntityId={0} and MepEntityType='LUsers') and A.LpCompanyId={1} and A.WFStatus='Completed'  order by FullName";
            //}
            //else
            //{
            //    //As the Portfolioselected has to be from Both Business Unit so we will find the Portfolios assocate with this channel first
            //    // var xx = db.Database.SqlQuery<LPayee>("WITH MyCTE AS ( SELECT A.Id, A.LpFirstName, A.LpLastName,null as LpParentPayeeId FROM LPayees A WHERE A.Id not in (select LppPayeeId from LPayeeParents) and A.LpCompanyId=" + LPayee.LpCompanyId + " and A.LpStatusId=(select Id from RStatuses where RsStatus='Approved'  and RsOwnerId=(select Id from RStatusOwners where RsoStatusOwner='Payees')) UNION ALL SELECT B.Id, B.LpFirstName, B.LpLastName, PP.LppParentPayeeId FROM LPayees B inner join LPayeeParents PP on B.Id=PP.LppPayeeId INNER JOIN MyCTE C ON PP.LppParentPayeeId = C.Id WHERE B.LpCompanyId=" + LPayee.LpCompanyId + " and " + LPayee.PayeeAsOfDate + " between PP.LppEffectiveStartDate and pp.LppEffectiveEndDate and B.LpStatusId=(select Id from RStatuses where RsStatus='Approved'  and RsOwnerId=(select Id from RStatusOwners where RsoStatusOwner='Payees')) ) SELECT * FROM MyCTE");
            //   Query = "SELECT distinct A.Id,(A.LpFirstName+' '+isnull(A.LpLastName,'')+' ('+A.LpPayeeCode+')') as FullName,null as LppParentPayeeId FROM LPayees A inner join MEntityPortfolios MEP on (MEP.MepEntityId=A.Id and MepEntityType='LPayees') WHERE MEP.MepPortfolioId in (select Id from LPortfolios where LpChannelId in (select LpChannelId from LPortfolios where Id in (select cast(item as integer)  from dbo.SplitString('" + PortfolioList + "',','))) ) and MEP.MepPortfolioId in (select MepPortfolioId from MEntityPortfolios where MepEntityId={0} and MepEntityType='LUsers') and A.LpCompanyId={1} and A.WFStatus='Completed'  order by FullName";
            //}
            //var xx = db.Database.SqlQuery<PayeeResultViewModel>(Query,LUsers.Id,CompanyId,CompanyId).ToList();
            ////JS: Just use the ID from the current method to select complete Payee record again.SS:  Will change method later using single query later.
            //if (IsDataToBeDisplayedInReport)
            //{
            //    var PayeeList = xx.Select(p => p.Id).ToList();
            //    var PayeeDataToBeReturned = db.LPayees.Where(p => PayeeList.Contains(p.Id)).ToList().Select(aa => new {
            //        //aa.WFComments,
            //        //aa.WFCompanyId,
            //        //aa.WFAnalystId,
            //        //aa.WFCurrentOwnerId,
            //        //aa.WFManagerId,
            //        //aa.WFOrdinal,
            //        //aa.WFRequesterId,
            //        //aa.WFRequesterRoleId,
            //        //aa.WFStatus,
            //        //aa.WFType,
            //        aa.A01,
            //        aa.A02,
            //        aa.A03,
            //        aa.A04,
            //        aa.A05,
            //        aa.A06,
            //        aa.A07,
            //        aa.A08,
            //        aa.A09,
            //        aa.A10,
            //        aa.Id,
            //        aa.LpAddress,
            //        aa.LpChannelId,
            //        LpEffectiveEndDate = (aa.LpEffectiveEndDate.HasValue) ? aa.LpEffectiveEndDate.Value.ToString("dd/MM/yyyy") : null,
            //        LpEffectiveStartDate = aa.LpEffectiveStartDate.ToString("dd/MM/yyyy"),
            //        aa.LpEmail,
            //        aa.LpFirstName,
            //        aa.LpLastName,
            //        aa.LpPayeeCode,
            //        aa.LpPhone,
            //        aa.LpChannelManager,
            //        aa.LpTIN,
            //        aa.LpPrimaryChannel,
            //        aa.LpBusinessUnit,
            //        LpCanRaiseClaims = (aa.LpCanRaiseClaims) ? "Y" : "N",
            //        aa.LpTradingName,
            //        aa.LpPosition,
            //        aa.LpSubChannelId,
            //        aa.LpDistributionChannel,
            //        aa.LpCompanyId,
            //        LpCreatedDateTime = (aa.LpCreatedDateTime.HasValue) ? aa.LpCreatedDateTime.Value.ToString("dd/MM/yyyy") : null,
            //        LpUpdatedDateTime = aa.LpUpdatedDateTime.HasValue ? aa.LpUpdatedDateTime.Value.ToString("dd/MM/yyyy") : null,
            //        aa.LpUpdatedById,
            //        aa.LpCreatedById,
            //        LpCreateLogin = (aa.LpCreateLogin) ? "Y" : "N",
            //        LpFinOpsRoles = string.IsNullOrEmpty(aa.LpFinOpsRoles) ? "N" : "Y",
            //    });
            //    return Ok(PayeeDataToBeReturned);
            //}
            //Section to get Payee Data from Stored Procedure starts Here
            var Query = "Exec SPGetPayeeData @UserRole,@LoggedInUserId,@CompanyId,@PortfolioList,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,@PayeeId";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@PortfolioList", string.IsNullOrEmpty(PortfolioList)?(object)System.DBNull.Value:PortfolioList);
            cmd.Parameters.AddWithValue("@UserRole", UserRole);
            cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield)?(object)System.DBNull.Value:sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : FilterQuery);
            cmd.Parameters.AddWithValue("@PayeeId", string.IsNullOrEmpty(PayeeId) ? (object)System.DBNull.Value : PayeeId);
            DataSet ds = Globals.GetData(cmd);
            return Ok(ds.Tables[0]);
        }

        //Added by SG To get List of Payees when CM will login into system
        public IHttpActionResult GetPayeeInformationByChannelManagerUserId(int CompanyId, string LoggedInUserId, string UserName, string Workflow, bool IsDataToBeDisplayedInReport)
        {
            // var xx = db.Database.SqlQuery<LPayee>("WITH MyCTE AS ( SELECT A.Id, A.LpFirstName, A.LpLastName,null as LpParentPayeeId FROM LPayees A WHERE A.Id not in (select LppPayeeId from LPayeeParents) and A.LpCompanyId=" + LPayee.LpCompanyId + " and A.LpStatusId=(select Id from RStatuses where RsStatus='Approved'  and RsOwnerId=(select Id from RStatusOwners where RsoStatusOwner='Payees')) UNION ALL SELECT B.Id, B.LpFirstName, B.LpLastName, PP.LppParentPayeeId FROM LPayees B inner join LPayeeParents PP on B.Id=PP.LppPayeeId INNER JOIN MyCTE C ON PP.LppParentPayeeId = C.Id WHERE B.LpCompanyId=" + LPayee.LpCompanyId + " and " + LPayee.PayeeAsOfDate + " between PP.LppEffectiveStartDate and pp.LppEffectiveEndDate and B.LpStatusId=(select Id from RStatuses where RsStatus='Approved'  and RsOwnerId=(select Id from RStatusOwners where RsoStatusOwner='Payees')) ) SELECT * FROM MyCTE");
            //var Query = "WITH MyCTE AS ( SELECT A.Id,(A.LpFirstName+' '+isnull(A.LpLastName,'')+' ('+A.LpPayeeCode+')') as FullName,null as LppParentPayeeId FROM LPayees A WHERE  A.LpChannelManager={0} and A.LpCompanyId={1} and A.WFStatus='Completed' UNION ALL SELECT B.Id, (B.LpFirstName+' '+isnull(B.LpLastName,'')+' ('+B.LpPayeeCode+')') as FullName, PP.LppParentPayeeId FROM LPayees B inner join LPayeeParents PP on B.Id=PP.LppPayeeId INNER JOIN MyCTE C ON PP.LppParentPayeeId = C.Id WHERE B.LpCompanyId={2} and GETDATE() between PP.LppEffectiveStartDate and isnull(pp.LppEffectiveEndDate,DateAdd(day,30,GETDATE())) and B.WFStatus='Completed' ) SELECT distinct FullName,Id FROM MyCTE order by FullName";
            //var xx = db.Database.SqlQuery<PayeeResultViewModel>(Query,LoggedInUserId,CompanyId,CompanyId).ToList();
            ////JS: Just use the ID from the current method to select complete Payee record again.SS:  Will change method later using single query later.
            //if (IsDataToBeDisplayedInReport)
            //{
            //    var PayeeList = xx.Select(p => p.Id).ToList();
            //    var PayeeDataToBeReturned = db.LPayees.Where(p => PayeeList.Contains(p.Id)).ToList().Select(aa => new {
            //        aa.A01,
            //        aa.A02,
            //        aa.A03,
            //        aa.A04,
            //        aa.A05,
            //        aa.A06,
            //        aa.A07,
            //        aa.A08,
            //        aa.A09,
            //        aa.A10,
            //        aa.Id,
            //        aa.LpAddress,
            //        aa.LpChannelId,
            //        LpEffectiveEndDate = (aa.LpEffectiveEndDate.HasValue) ? aa.LpEffectiveEndDate.Value.ToString("dd/MM/yyyy") : null,
            //        LpEffectiveStartDate = aa.LpEffectiveStartDate.ToString("dd/MM/yyyy"),
            //        aa.LpEmail,
            //        aa.LpFirstName,
            //        aa.LpLastName,
            //        aa.LpPayeeCode,
            //        aa.LpPhone,
            //        aa.LpChannelManager,
            //        aa.LpTIN,
            //        aa.LpPrimaryChannel,
            //        aa.LpBusinessUnit,
            //        LpCanRaiseClaims = (aa.LpCanRaiseClaims) ? "Y" : "N",
            //        aa.LpTradingName,
            //        aa.LpPosition,
            //        aa.LpSubChannelId,
            //        aa.LpDistributionChannel,
            //        aa.LpCompanyId,
            //        LpCreatedDateTime = (aa.LpCreatedDateTime.HasValue) ? aa.LpCreatedDateTime.Value.ToString("dd/MM/yyyy") : null,
            //        LpUpdatedDateTime = aa.LpUpdatedDateTime.HasValue ? aa.LpUpdatedDateTime.Value.ToString("dd/MM/yyyy") : null,
            //        aa.LpUpdatedById,
            //        aa.LpCreatedById,
            //        LpCreateLogin = (aa.LpCreateLogin) ? "Y" : "N",
            //        LpFinOpsRoles = string.IsNullOrEmpty(aa.LpFinOpsRoles) ? "N" : "Y",
            //    });
            //    return Ok(PayeeDataToBeReturned);
            //}

            //Section to get Payee Data from Stored Procedure starts Here
            var Query = "Exec SPGetPayeeData @UserRole,@LoggedInUserId,@CompanyId,@PortfolioList,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,null";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@PortfolioList", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@UserRole", "Channel Manager");
            cmd.Parameters.AddWithValue("@LoggedInUserId", LoggedInUserId);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@sortdatafield", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@sortorder", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@pagesize", 99999);
            cmd.Parameters.AddWithValue("@pagenum", 0);
            cmd.Parameters.AddWithValue("@FilterQuery", (object)System.DBNull.Value);
            DataSet ds = Globals.GetData(cmd);
            return Ok(ds.Tables[0]);
        }

        //Get Payee's Childern list when he is logged in to system.
        public IHttpActionResult GetPayeeHierarchy(int CompanyId, string PayeeUserId, string UserName, string Workflow, bool IsDataToBeDisplayedInReport)
        {
            //var PayeeId = db.LPayees.Where(p => p.LpUserId == PayeeUserId).FirstOrDefault().Id;
            ////var Query = "WITH parent AS (select LppParentPayeeId from LPayeeParents where LppParentPayeeId="+PayeeId+"), tree AS (SELECT x.LppParentPayeeId, x.LppPayeeId FROM LPayeeParents x INNER JOIN parent ON x.LppParentPayeeId = parent.LppParentPayeeId UNION ALL  /* Fetch records for all parents*/ SELECT y.LppParentPayeeId, y.LppPayeeId FROM LPayeeParents y INNER JOIN tree t ON y.LppParentPayeeId = t.LppPayeeId ) SELECT t.LppParentPayeeId as Id,(parent.LpFirstName+' '+IsNull(parent.LpLastName,'')+' ('+parent.LpPayeeCode+')') as FullName FROM tree t   inner join LPayees parent on t.LppParentPayeeId=parent.Id where parent.WFStatus='Completed'  union /* now add children to the result set*/ select t.LppPayeeId as Id,(child.LpFirstName+' '+IsNull(child.LpLastName,'')+' ('+child.LpPayeeCode+')') as FullName FROM tree t inner join LPayees child on t.LppPayeeId=child.Id where child.WFStatus='Completed' Union /*Following select is to handle the scenario when thelogin payee is the leaf of the tree (as the above queries will not pick the leaf because it is not parent in the LParentPayee table*/ select child.id as Id,(child.LpFirstName+' '+IsNull(child.LpLastName,'')+' ('+child.LpPayeeCode+')') as FullName FROM LPayees child where WFStatus='Completed' and Id="+PayeeId+" order by FullName";
            ////RK Added payee code in the select statement below
            //var Query = "WITH parent AS (select LppParentPayeeId from LPayeeParents where LppParentPayeeId=" + PayeeId + "), tree AS (SELECT x.LppParentPayeeId, x.LppPayeeId FROM LPayeeParents x INNER JOIN parent ON x.LppParentPayeeId = parent.LppParentPayeeId UNION ALL SELECT y.LppParentPayeeId, y.LppPayeeId FROM LPayeeParents y INNER JOIN tree t ON y.LppParentPayeeId = t.LppPayeeId ) SELECT t.LppParentPayeeId as Id,(parent.LpFirstName+' '+IsNull(parent.LpLastName,'')+' ('+parent.LpPayeeCode+')') as FullName, parent.LpPayeeCode FROM tree t inner join LPayees parent on t.LppParentPayeeId=parent.Id where parent.WFStatus='Completed'  union select t.LppPayeeId as Id,(child.LpFirstName+' '+IsNull(child.LpLastName,'')+' ('+child.LpPayeeCode+')') as FullName, child.LpPayeeCode FROM tree t inner join LPayees child on t.LppPayeeId=child.Id where child.WFStatus='Completed' Union select child.id as Id,(child.LpFirstName+' '+IsNull(child.LpLastName,'')+' ('+child.LpPayeeCode+')') as FullName, child.LpPayeeCode FROM LPayees child where WFStatus='Completed' and Id=" + PayeeId + " order by FullName";
            //var xx = db.Database.SqlQuery<PayeeResultViewModel>(Query).ToList();
            ////JS: Just use the ID from the current method to select complete Payee record again.SS:  Will change method later using single query later.
            //if (IsDataToBeDisplayedInReport)
            //{
            //    var PayeeList = xx.Select(p => p.Id).ToList();
            //    var PayeeDataToBeReturned = db.LPayees.Where(p => PayeeList.Contains(p.Id)).ToList().Select(aa => new {
            //        //aa.WFComments,
            //        //aa.WFCompanyId,
            //        //aa.WFAnalystId,
            //        //aa.WFCurrentOwnerId,
            //        //aa.WFManagerId,
            //        //aa.WFOrdinal,
            //        //aa.WFRequesterId,
            //        //aa.WFRequesterRoleId,
            //        //aa.WFStatus,
            //        //aa.WFType,
            //        aa.A01,
            //        aa.A02,
            //        aa.A03,
            //        aa.A04,
            //        aa.A05,
            //        aa.A06,
            //        aa.A07,
            //        aa.A08,
            //        aa.A09,
            //        aa.A10,
            //        aa.Id,
            //        aa.LpAddress,
            //        aa.LpChannelId,
            //        LpEffectiveEndDate = (aa.LpEffectiveEndDate.HasValue)?aa.LpEffectiveEndDate.Value.ToString("dd/MM/yyyy"):null,
            //        LpEffectiveStartDate = aa.LpEffectiveStartDate.ToString("dd/MM/yyyy"),
            //        aa.LpEmail,
            //        aa.LpFirstName,
            //        aa.LpLastName,
            //        aa.LpPayeeCode,
            //        aa.LpPhone,
            //        aa.LpChannelManager,
            //        aa.LpTIN,
            //        aa.LpPrimaryChannel,
            //        aa.LpBusinessUnit,
            //        LpCanRaiseClaims = (aa.LpCanRaiseClaims) ? "Y" : "N",
            //        aa.LpTradingName,
            //        aa.LpPosition,
            //        aa.LpSubChannelId,
            //        aa.LpDistributionChannel,
            //        aa.LpCompanyId,
            //        LpCreatedDateTime = (aa.LpCreatedDateTime.HasValue)?aa.LpCreatedDateTime.Value.ToString("dd/MM/yyyy"):null,
            //        LpUpdatedDateTime = aa.LpUpdatedDateTime.HasValue?aa.LpUpdatedDateTime.Value.ToString("dd/MM/yyyy"):null,
            //        aa.LpUpdatedById,
            //        aa.LpCreatedById,
            //        LpCreateLogin = (aa.LpCreateLogin) ? "Y" : "N",
            //        LpFinOpsRoles = string.IsNullOrEmpty(aa.LpFinOpsRoles) ? "N" : "Y",
            //    });
            //    return Ok(PayeeDataToBeReturned);
            //}
            //Section to get Payee Data from Stored Procedure starts Here
            
            //Section to get Payee Data from Stored Procedure starts Here
            var Query = "Exec SPGetPayeeData @UserRole,@LoggedInUserId,@CompanyId,@PortfolioList,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,null";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@PortfolioList", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@UserRole", "Payee");
            cmd.Parameters.AddWithValue("@LoggedInUserId", PayeeUserId);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@sortdatafield", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@sortorder", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@pagesize", 99999);
            cmd.Parameters.AddWithValue("@pagenum", 0);
            cmd.Parameters.AddWithValue("@FilterQuery", (object)System.DBNull.Value);
            DataSet ds = Globals.GetData(cmd);
            return Ok(ds.Tables[0]);
        }

        public IHttpActionResult GetPayeeHierarchyByPayeeId(int CompanyId, string PayeeId, string UserName, string Workflow)
        {
            var Query = "WITH parent AS (select LppParentPayeeId from LPayeeParents where LppParentPayeeId={0}), tree AS (SELECT x.LppParentPayeeId, x.LppPayeeId FROM LPayeeParents x INNER JOIN parent ON x.LppParentPayeeId = parent.LppParentPayeeId UNION ALL  /* Fetch records for all parents*/ SELECT y.LppParentPayeeId, y.LppPayeeId FROM LPayeeParents y INNER JOIN tree t ON y.LppParentPayeeId = t.LppPayeeId ) SELECT t.LppParentPayeeId as Id,(parent.LpFirstName+' '+IsNull(parent.LpLastName,'')+' ('+parent.LpPayeeCode+')') as FullName FROM tree t   inner join LPayees parent on t.LppParentPayeeId=parent.Id where parent.WFStatus='Completed'  union /* now add children to the result set*/ select t.LppPayeeId as Id,(child.LpFirstName+' '+IsNull(child.LpLastName,'')+' ('+child.LpPayeeCode+')') as FullName FROM tree t inner join LPayees child on t.LppPayeeId=child.Id where child.WFStatus='Completed' Union /*Following select is to handle the scenario when thelogin payee is the leaf of the tree (as the above queries will not pick the leaf because it is not parent in the LParentPayee table*/ select child.id as Id,(child.LpFirstName+' '+IsNull(child.LpLastName,'')+' ('+child.LpPayeeCode+')') as FullName FROM LPayees child where WFStatus='Completed' and Id={1} order by FullName";
            var xx = db.Database.SqlQuery<PayeeResultViewModel>(Query, PayeeId, PayeeId).ToList();
            return Ok(xx);
        }

        //This method returns all list of payees of the company whose status is neither withdrawn nor rejected.
        //This linq will be replaced by payee hierarchy query to get all the children of payees
        public IHttpActionResult GetParentDropDown(int CompanyId, string UserName, string Workflow)
        {
            //var CurrentDate = DateTime.UtcNow.Date;
            //var FutureDate = DateTime.UtcNow.Date.AddDays(30);
            //var xx = (from aa in db.LPayees.Where(p => p.WFStatus != "Withdrawn" && p.WFStatus != "Rejected").Where(p => p.LpCompanyId == CompanyId).Where(p => ((p.LpEffectiveEndDate.HasValue) ? p.LpEffectiveEndDate.Value : FutureDate) >= CurrentDate)
            //              //join bb in db.LPayeeParents.Include(p => p.LPayee).Where(p => p.LppEffectiveEndDate.HasValue) on aa.Id equals bb.LppPayeeId
            //          select new
            //          {
            //              aa.Id,
            //              FullName = aa.LpFirstName + " " + aa.LpLastName + " (" + aa.LpPayeeCode + ")",
            //              aa.LpPayeeCode,
            //              aa.LpBusinessUnit,aa.LpPrimaryChannel,aa.LpChannelId,aa.LpSubChannelId,aa.LpAddress
            //          }).OrderBy(p=>p.FullName);
            //Section to get Payee Data from Stored Procedure starts Here
            var Query = "Exec USPGetParentDropDown @CompanyId";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            DataSet ds = Globals.GetData(cmd);
            return Ok(ds.Tables[0]);
        }

        //Get Counts for Parent Payee Grid
        public IHttpActionResult GetParentPayeeGridCounts(int CompanyId)
        {
            //Section to get Payee Data from Stored Procedure starts Here
            var Query = "Exec USPGetParentDropDown @CompanyId,null,null,99999,0,null,null";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            DataSet ds = Globals.GetData(cmd);
            return Ok(ds.Tables[0].Rows.Count);
        }

        //Get Data for Parent Payee Grid
        public IHttpActionResult GetParentPayeeGridData(int CompanyId, string sortdatafield, string sortorder, int pagesize, int pagenum,string FilterQuery,string ParentPayeeId)
        {
            //Section to get Payee Data from Stored Procedure starts Here
            var Query = "Exec USPGetParentDropDown @CompanyId,@sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,@ParentPayeeId";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield)?(object)DBNull.Value:sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)DBNull.Value : sortorder);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)DBNull.Value : FilterQuery);
            cmd.Parameters.AddWithValue("@ParentPayeeId", string.IsNullOrEmpty(ParentPayeeId) ? (object)DBNull.Value : ParentPayeeId);
            DataSet ds = Globals.GetData(cmd);
            return Ok(ds.Tables[0]);
        }


        //RK Created on 16082017
        //This method will return all the payee codes along with email id for the company irrespective of their status
        //This will be used to check while payee upload
        public IHttpActionResult GetAllPayeeCodesAndEmailID(int CompanyId, string UserName, string Workflow)
        {
            //var CurrentDate = DateTime.UtcNow.Date;
            //var FutureDate = DateTime.UtcNow.Date.AddDays(30);
            //var xx = (from aa in db.LPayees.Where(p => p.LpCompanyId == CompanyId)
            //          select new
            //          {
            //              aa.Id,
            //              aa.LpPayeeCode,
            //              aa.LpEmail,
            //              aa.WFStatus
            //          });
            //Getting DecryptedData from SP
            var PayeeData = Globals.ExecuteSPGetFilteredPayeeResults(null, null, CompanyId, null, null, null, null, null, null, null, null, null);

            return Ok(PayeeData);
        }
        public IHttpActionResult GetActivePayeeByCompanyId(int CompanyId, string UserName, string Workflow)
        {
            //var CurrentDate = DateTime.UtcNow.Date;
            //var FutureDate = DateTime.UtcNow.Date.AddDays(30);
            //var UserList = db.AspNetUsers.Where(p => p.GcCompanyId == CompanyId).Select(p => new { p.Id, p.UserName });
            //var xx = (from aa in db.LPayees.Include(p => p.RChannel).Where(p=>p.WFStatus=="Completed").Include(p => p.RSubChannel).Where(p => CurrentDate >= p.LpEffectiveStartDate && CurrentDate < ((p.LpEffectiveEndDate.HasValue) ? p.LpEffectiveEndDate.Value : FutureDate)).Where(p => p.LpCompanyId == CompanyId)
            //          join bb in db.LPayeeParents.Include(p => p.LPayee).Where(p => p.LppEffectiveStartDate <= CurrentDate && ((p.LppEffectiveEndDate.HasValue) ? p.LppEffectiveEndDate.Value : FutureDate) >= CurrentDate) on aa.Id equals bb.LppPayeeId
            //            into grp
            //          from c in grp.DefaultIfEmpty()//this line will give result even if updated by is null
            //          select new
            //          {
            //              aa.Id,
            //              aa.LpAddress,
            //              aa.LpChannelId,
            //              aa.LpEffectiveEndDate,
            //              aa.LpEffectiveStartDate,
            //              aa.LpEmail,
            //              LppParentPayeeId = (c != null) ? c.LppParentPayeeId : 0,
            //              aa.LpFirstName,
            //              aa.LpLastName,
            //              aa.LpPayeeCode,
            //              aa.LpPhone,
            //              ParentCode = c.LPayee.LpPayeeCode,
            //              ParentName = c.LPayee.LpFirstName + " " + c.LPayee.LpLastName,
            //              aa.LpChannelManager,
            //              aa.LpTIN,
            //              aa.LpCompanyId,
            //              aa.LpBusinessUnit,
            //              aa.LpCanRaiseClaims,
            //              aa.LpPrimaryChannel,
            //              aa.RChannel.RcName,
            //              aa.RSubChannel.RscName,
            //              aa.LpTradingName,
            //              aa.LpDistributionChannel,
            //              CreatedBy = (aa.LpCreatedById != null) ? UserList.Where(p => p.Id == aa.LpCreatedById).FirstOrDefault().UserName : "",
            //              UpdatedBy = (aa.LpUpdatedById != null) ? UserList.Where(p => p.Id == aa.LpUpdatedById).FirstOrDefault().UserName : "",
            //              aa.LpCreatedDateTime,
            //              aa.LpUpdatedDateTime,
            //              aa.LpUpdatedById,
            //              aa.LpCreatedById,
            //              FullName = aa.LpFirstName + " " + aa.LpLastName + " (" + aa.LpPayeeCode + ")",
            //              aa.A01,aa.A02,aa.A03,aa.A04,aa.A05,aa.A06,aa.A07,aa.A08,aa.A09,aa.A10,aa.LpFinOpsRoles,//RK Added 02Nov2017
            //          }).OrderByDescending(p => p.LpCreatedDateTime);

            //Getting DecryptedData from SP
            var PayeeData = Globals.ExecuteSPGetFilteredPayeeResults(null, null, CompanyId, null, "Completed", null, null, null, null, null, null, null);//db.LPayees.Where(p => p.LpUserId == PayeeUserId).FirstOrDefault().Id;

            return Ok(PayeeData);

        }

        //public IHttpActionResult GetDownloadPayeeGridByStatusNameCreatedByUserId(int CompanyId, string StatusName, string CreatedByUserId)
        //{
        //    var CurrentDate = DateTime.UtcNow.Date;
        //    var FutureDate = DateTime.UtcNow.Date.AddDays(30);
        //    var UserList = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LPayees.Include(p => p.RChannel).Include(p => p.RSubChannel).Where(p => p.LpStatus == StatusName).Where(p => p.LpCreatedById == CreatedByUserId).Where(p => CurrentDate >= p.LpEffectiveStartDate && CurrentDate < ((p.LpEffectiveEndDate.HasValue) ? p.LpEffectiveEndDate.Value : FutureDate)).Where(p => p.LpCompanyId == CompanyId)
        //              join bb in db.LPayeeParents.Include(p => p.LPayee).Where(p => p.LppEffectiveStartDate <= CurrentDate && ((p.LppEffectiveEndDate.HasValue) ? p.LppEffectiveEndDate.Value : FutureDate) >= CurrentDate) on aa.Id equals bb.LppPayeeId
        //                into grp
        //              from c in grp.DefaultIfEmpty()//this line will give result even if parent is null
        //              select new
        //              {
        //                  Address = aa.LpAddress,
        //                  EffectiveEndDate = aa.LpEffectiveEndDate,
        //                  EffectiveStartDate = aa.LpEffectiveStartDate,
        //                  Email = aa.LpEmail,
        //                  FirstName = aa.LpFirstName,
        //                  LastName = aa.LpLastName,
        //                  PayeeCode = aa.LpPayeeCode,
        //                  Phone = aa.LpPhone,
        //                  ParentName = c.LPayee.LpFirstName + " " + c.LPayee1.LpLastName,
        //                  ChannelManager = aa.LpChannelManager,
        //                  Tin = aa.LpTIN,
        //                  BusinessUnit = aa.LpBusinessUnit,
        //                  CanRaiseClaims = aa.LpCanRaiseClaims,
        //                  AuthorisedPayeeVerification = aa.LpAuthorisedPayeeVerification,
        //                  PrimaryChannel = aa.LpPrimaryChannel,
        //                  aa.RChannel.RcName,
        //                  aa.RSubChannel.RscName,
        //                  TradingName = aa.LpTradingName,
        //                  DistributionChannel = aa.LpDistributionChannel,
        //                  CreatedBy = (aa.LpCreatedById != null) ? UserList.Where(p => p.Id == aa.LpCreatedById).FirstOrDefault().UserName : "",
        //                  UpdatedBy = (aa.LpUpdatedById != null) ? UserList.Where(p => p.Id == aa.LpUpdatedById).FirstOrDefault().UserName : "",
        //                  Status = aa.LpStatus,
        //                  aa.LpCreatedDateTime,
        //                  aa.LpUpdatedDateTime,
        //                  aa.A01,
        //                  aa.A02,
        //                  aa.A03,
        //                  aa.A04,
        //                  aa.A05,
        //                  aa.A06,
        //                  aa.A07,
        //                  aa.A08,
        //                  aa.A09,
        //                  aa.A10
        //              }).OrderByDescending(p => p.LpCreatedDateTime);
        //    return Ok(xx);

        //}
        //public IHttpActionResult GetDownloadPayeeGridByStatusNameReportsToId(int CompanyId, string StatusName, string ReportsToId)
        //{
        //    var CurrentDate = DateTime.UtcNow.Date;
        //    var FutureDate = DateTime.UtcNow.Date.AddDays(30);
        //    var UserList = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
        //    var xx = (from aa in db.LPayees.Include(p => p.RChannel).Include(p => p.RSubChannel).Where(p => p.LpStatus == StatusName).Where(p => CurrentDate >= p.LpEffectiveStartDate && CurrentDate < ((p.LpEffectiveEndDate.HasValue) ? p.LpEffectiveEndDate.Value : FutureDate)).Where(p => p.LpCompanyId == CompanyId)
        //              join bb in db.LPayeeParents.Include(p => p.LPayee).Where(p => p.LppEffectiveStartDate <= CurrentDate && ((p.LppEffectiveEndDate.HasValue) ? p.LppEffectiveEndDate.Value : FutureDate) >= CurrentDate) on aa.Id equals bb.LppPayeeId
        //              into grp
        //              from c in grp.DefaultIfEmpty()//this line will give result even if parent is null
        //              join Lu in db.LUsers on aa.LpUpdatedById equals Lu.LuUserId
        //            into grp1
        //              from u in grp1.DefaultIfEmpty()//this line will give result even if updated by is null
        //              where u.LuReportsToId == ReportsToId || aa.LpCreatedById == ReportsToId
        //              select new
        //              {
        //                  Address = aa.LpAddress,
        //                  EffectiveEndDate = aa.LpEffectiveEndDate,
        //                  EffectiveStartDate = aa.LpEffectiveStartDate,
        //                  Email = aa.LpEmail,
        //                  FirstName = aa.LpFirstName,
        //                  LastName = aa.LpLastName,
        //                  PayeeCode = aa.LpPayeeCode,
        //                  Phone = aa.LpPhone,
        //                  ParentName = c.LPayee.LpFirstName + " " + c.LPayee1.LpLastName,
        //                  ChannelManager = aa.LpChannelManager,
        //                  Tin = aa.LpTIN,
        //                  BusinessUnit = aa.LpBusinessUnit,
        //                  CanRaiseClaims = aa.LpCanRaiseClaims,
        //                  AuthorisedPayeeVerification = aa.LpAuthorisedPayeeVerification,
        //                  PrimaryChannel = aa.LpPrimaryChannel,
        //                  aa.RChannel.RcName,
        //                  aa.RSubChannel.RscName,
        //                  TradingName = aa.LpTradingName,
        //                  DistributionChannel = aa.LpDistributionChannel,
        //                  CreatedBy = (aa.LpCreatedById != null) ? UserList.Where(p => p.Id == aa.LpCreatedById).FirstOrDefault().UserName : "",
        //                  UpdatedBy = (aa.LpUpdatedById != null) ? UserList.Where(p => p.Id == aa.LpUpdatedById).FirstOrDefault().UserName : "",
        //                  Status = aa.LpStatus,
        //                  aa.LpCreatedDateTime,
        //                  aa.LpUpdatedDateTime,
        //                  aa.A01,
        //                  aa.A02,
        //                  aa.A03,
        //                  aa.A04,
        //                  aa.A05,
        //                  aa.A06,
        //                  aa.A07,
        //                  aa.A08,
        //                  aa.A09,
        //                  aa.A10
        //              }).OrderByDescending(p => p.LpCreatedDateTime);
        //    return Ok(xx);

        //}

        //public IHttpActionResult GetLPayeesByStatusNameCompanyId(string StatusName, int CompanyId, string ReportsToId)
        //{
        //    var xx = db.Database.SqlQuery<UploadPayeeViewModel>("with MyCte as(select P.Id,(P.LpFirstName+' '+Isnull(P.LpLastName,'')+' ('+P.LpPayeeCode+')') as FullName,P.LpCreatedById,P.LpBusinessUnit,P.LpPrimaryChannel,P.LpTradingName,P.LpPayeeCode,P.LpComments,P.LpEffectiveStartDate,P.LpEffectiveEndDate,P.LpCreatedDateTime,P.LpUpdatedDateTime ,(Select UserName  from AspNetUsers where Id=P.LpCreatedById) as CreatedBy ,(Select UserName  from AspNetUsers where Id=P.LpUpdatedById) as UpdatedBy,dbo.GetPayeeParentId(P.Id,P.LpEffectiveStartDate) as LppParentPayeeId from LPayees  P inner join LUsers U on P.LpUpdatedById=U.LuUserId where LpCompanyId=" + CompanyId + "  and LpStatus = '" + StatusName + "' and (U.LuReportsToId='" + ReportsToId + "' or P.LpCreatedById='" + ReportsToId + "')) select *,(select LpPayeeCode  from LPayees  where Id= LppParentPayeeId )as ParentCode  from MyCte order by LpCreatedDateTime desc").ToList();
        //    return Ok(xx);
        //}

        ////This method returns list of payee as per status name , Company Id and Created by Id
        //public IHttpActionResult GetLPayeesByStatusNameCompanyIdCreatedById(string StatusName, int CompanyId, string CreatedByUserId)
        //{
        //    var xx = db.Database.SqlQuery<UploadPayeeViewModel>("with MyCte as(select Id,(LpFirstName+' '+Isnull(LpLastName,'')+' ('+LpPayeeCode+')') as FullName,LpCreatedById,LpBusinessUnit,[LpPrimaryChannel],[LpTradingName],[LpPayeeCode],[LpComments],[LpEffectiveStartDate],[LpEffectiveEndDate],[LpCreatedDateTime],[LpUpdatedDateTime] ,(Select UserName  from AspNetUsers where Id=LpCreatedById) as CreatedBy ,(Select UserName  from AspNetUsers where Id=LpUpdatedById) as UpdatedBy,dbo.GetPayeeParentId(Id,LpEffectiveStartDate) as LppParentPayeeId from LPayees where LpCompanyId=" + CompanyId + " and LpStatus = '" + StatusName + "' and LpCreatedById='" + CreatedByUserId + "') select *,(select LpPayeeCode from LPayees where Id=LppParentPayeeId ) as ParentCode from MyCte order by LpCreatedDateTime desc").ToList();
        //    return Ok(xx);
        //}

        //public IHttpActionResult GetLPayeesByStatusNameCompanyIdPrimaryChannel(string StatusName, int CompanyId, string PrimaryChannel)
        //{
        //    var xx = db.Database.SqlQuery<UploadPayeeViewModel>("with MyCte as(select Id,(LpFirstName+' '+Isnull(LpLastName,'')+' ('+LpPayeeCode+')') as FullName,LpCreatedById,LpBusinessUnit,[LpPrimaryChannel],[LpTradingName],[LpPayeeCode],[LpComments],[LpEffectiveStartDate],[LpEffectiveEndDate],[LpChannelManager],[LpCreatedDateTime],[LpUpdatedDateTime] ,(Select UserName  from AspNetUsers where Id=LpCreatedById) as CreatedBy ,(Select UserName  from AspNetUsers where Id=LpUpdatedById) as UpdatedBy,dbo.GetPayeeParentId(Id,LpEffectiveStartDate) as LppParentPayeeId from LPayees where LpCompanyId={0}  and LpPrimaryChannel={1}   and  LpStatus = {2}) select *,(select LpPayeeCode from LPayees where Id=LppParentPayeeId ) as ParentCode from MyCte order by LpCreatedDateTime desc",CompanyId,PrimaryChannel,StatusName).ToList();
        //    return Ok(xx);
        //}

        ////method return the LPayee as per PayeeId only
        // GET: api/LPayees/5
        [ResponseType(typeof(LPayee))]
        public async Task<IHttpActionResult> GetLPayeeById(int id, string UserName, string Workflow)
        {
            //var LPayee = (from aa in db.LPayees.Where(p => p.Id == id)
            //              select new
            //              {
            //                  aa.WFComments,
            //                  aa.WFCompanyId,
            //                  aa.WFAnalystId,
            //                  aa.WFCurrentOwnerId,
            //                  aa.WFManagerId,
            //                  aa.WFOrdinal,
            //                  aa.WFRequesterId,
            //                  aa.WFRequesterRoleId,
            //                  aa.WFStatus,
            //                  aa.WFType,
            //                  aa.LpPayeeCode,
            //                  aa.A01,
            //                  aa.A02,
            //                  aa.A03,
            //                  aa.A04,
            //                  aa.A05,
            //                  aa.A06,
            //                  aa.A07,
            //                  aa.A08,
            //                  aa.A09,
            //                  aa.A10,
            //                  aa.Id,
            //                  aa.LpAddress,
            //                  aa.LpChannelId,
            //                  aa.LpEffectiveEndDate,
            //                  aa.LpEffectiveStartDate,
            //                  aa.LpEmail,
            //                  aa.LpFirstName,
            //                  aa.LpLastName,
            //                  aa.LpPhone,
            //                  aa.LpChannelManager,
            //                  aa.LpTIN,
            //                  aa.LpPrimaryChannel,
            //                  aa.RChannel.RcName,
            //                  aa.RSubChannel.RscName,
            //                  aa.LpBusinessUnit,
            //                  aa.LpCanRaiseClaims,
            //                  aa.LpTradingName,
            //                  aa.LpPosition,
            //                  aa.LpSubChannelId,
            //                  aa.LpDistributionChannel,
            //                  aa.LpCompanyId,
            //                  aa.LpCreatedDateTime,
            //                  aa.LpUpdatedDateTime,
            //                  aa.LpUpdatedById,
            //                  aa.LpCreatedById,
            //                  aa.LpCreateLogin
            //              }).FirstOrDefault();
            //Getting DecryptedData from SP
            var PayeeData = Globals.ExecuteSPGetFilteredPayeeResults(id, null, null, null, null, null, null, null, null, null, null, null);//db.LPayees.Where(p => p.LpUserId == PayeeUserId).FirstOrDefault().Id;

            return Ok(PayeeData);


        }

        // GET: api/LPayees/5
        [ResponseType(typeof(LPayee))]
        public async Task<IHttpActionResult> GetLPayee(int id, string UserName, string Workflow)
        {
            //// var UserList = db.AspNetUsers.Select(p => new { p.Id, p.UserName });
            //var SelectedPayee = db.LPayees.Find(id);
            //var EffectiveDate = SelectedPayee.LpEffectiveStartDate;
            //var StatusName = SelectedPayee.WFStatus;
            //Nullable<int> ParentId;
            //string FinOpsRoleStr = "";
            ////if finops roles are defined create a comma seperated list of their names
            //if (!string.IsNullOrEmpty(SelectedPayee.LpFinOpsRoles))
            //{
            //    var FinOpsArray = SelectedPayee.LpFinOpsRoles.Split(',');
            //    foreach(var role in FinOpsArray)
            //    {
            //        var Aspnetrole = db.AspNetRoles.Where(p => p.Id == role).FirstOrDefault();
            //        if (Aspnetrole != null)
            //            FinOpsRoleStr += Aspnetrole.Name + ",";
            //    }
            //    FinOpsRoleStr = FinOpsRoleStr.TrimEnd(',');
            //}
            ////JS: Always get Parent as per GetDate
            ////The sql function does not work in linq query hence its result is stored in a variable before forming linq query
            ////if (StatusName == "Completed")//If  Payee status is Approved the Payee's parentId as on current date
            ////{
            //var RawQuery = db.Database.SqlQuery<Nullable<Int32>>("select dbo.GetPayeeParentId(" + id + ",'" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "')");
            //    var Task = RawQuery.SingleAsync();
            //    ParentId = Task.Result;
            ////}
            ////else//otherwise get payee parentId  as on effective start date of payee
            ////{
            ////    var RawQuery = db.Database.SqlQuery<Nullable<Int32>>("select dbo.GetPayeeParentId(" + id + ",'" + EffectiveDate.ToString("yyyy-MM-dd") + "')");
            ////    var Task = RawQuery.SingleAsync();
            ////    ParentId = Task.Result;
            ////}
            //var ParentPayee = new LPayee();
            //if (ParentId.HasValue)
            //{
            //    ParentPayee = db.LPayees.Find(ParentId.Value);
            //}
            //var LPayee = (from aa in db.LPayees.Include(p => p.RChannel).Include(p => p.RSubChannel).Where(p => p.Id == id)
            //              join yy in db.AspNetUsers on aa.LpCreatedById equals yy.Id
            //              select new
            //              {
            //                  aa.WFComments,aa.WFCompanyId,
            //                  aa.WFAnalystId,aa.WFCurrentOwnerId,aa.WFManagerId,aa.WFOrdinal,aa.WFRequesterId,aa.WFRequesterRoleId,aa.WFStatus,aa.WFType,
            //                  ParentName = ParentPayee.LpFirstName + " " + ParentPayee.LpLastName,
            //                  ParentCode = ParentPayee.LpPayeeCode,//These two entities will be used in create and edit of claims
            //                  aa.A01,
            //                  aa.A02,
            //                  aa.A03,
            //                  aa.A04,
            //                  aa.A05,
            //                  aa.A06,
            //                  aa.A07,
            //                  aa.A08,
            //                  aa.A09,
            //                  aa.A10,
            //                  aa.Id,
            //                  aa.LpAddress,
            //                  aa.LpChannelId,
            //                  aa.LpEffectiveEndDate,
            //                  aa.LpEffectiveStartDate,
            //                  aa.LpEmail,
            //                  aa.LpFirstName,
            //                  aa.LpLastName,
            //                  LppParentPayeeId = ParentId,
            //                  aa.LpPayeeCode,
            //                  aa.LpPhone,
            //                  aa.LpChannelManager,
            //                  aa.LpTIN,
            //                  aa.LpPrimaryChannel,
            //                  aa.LpBusinessUnit,
            //                  aa.LpCanRaiseClaims,
            //                  aa.LpTradingName,
            //                  aa.LpPosition,
            //                  aa.LpSubChannelId,
            //                  aa.LpDistributionChannel,
            //                  aa.LpCompanyId,
            //                  CreatedBy = yy.UserName,//(aa.LpCreatedById != null) ? UserList.Where(p => p.Id == aa.LpCreatedById).FirstOrDefault().UserName : "",
            //                  //UpdatedBy = bb.UserName, //(aa.LpUpdatedById != null) ? UserList.Where(p => p.Id == aa.LpUpdatedById).FirstOrDefault().UserName : "",
            //                  aa.LpCreatedDateTime,
            //                  aa.LpUpdatedDateTime,
            //                  // ParentCode = aa.LPayee1.LpPayeeCode,ParentName=aa.LPayee1.LpFirstName+" "+aa.LpLastName,
            //                  aa.LpUpdatedById,
            //                  aa.LpCreatedById,
            //                  aa.LpCreateLogin,
            //                  LpFinOpsRoles=(string.IsNullOrEmpty(aa.LpFinOpsRoles)?null:aa.LpFinOpsRoles),FinOpsRoleString=FinOpsRoleStr
            //              }).FirstOrDefault();
            //Getting DecryptedData from SP
            var PayeeData = Globals.ExecuteSPGetFilteredPayeeResults(id, null, null, null, null, null, null, null, null, null, null, null);//db.LPayees.Where(p => p.LpUserId == PayeeUserId).FirstOrDefault().Id;

            if (PayeeData.Rows.Count == 0)
            {
                //return NotFound();
                //PAYEE to be displayed could not be found. Send appropriate response to the request.
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PAYEE")));
            }
            return Ok(PayeeData);
        }

        // PUT: api/LPayees/5 comment
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLPayee(int id, LPayeeDecryptedViewModel LPayee, string AttachedFiles, string AttachmentPath, string LoggedInUserId, string UserName, string Workflow, int LppParentPayeeId)
        {
            //if (!ModelState.IsValid)
            //{
            //    //return BadRequest(ModelState);
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PAYEE")));
            //}
            string PortfolioList = LPayee.ParameterCarrier;
            var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (id != LPayee.Id)
            {
                //return BadRequest();
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "PAYEE")));
            }
            string strPFName = LPayee.LpFirstName;
            string strPLName = LPayee.LpLastName;
           
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //var ExistingPayeeDetails = db.LPayees.Find(id);
                    // LPayee.WFCompanyId=LPayee.LpCompanyId;
                    //db.Entry(LPayee).State = EntityState.Modified;
                    //await db.SaveChangesAsync();
                    //Call global method to update Payee
                    Globals.ExecutePutLPayee(LPayee);

                    //RK Special treatment for first name , last name and trading name as while updation, it convert values in funny characters for greek characters. 
                    
                    //LPayeeNamesObjectsModel LOM = new LPayeeNamesObjectsModel();
                    //LOM.Id = LPayee.Id;
                    //LOM.LpFirstName = LPayee.LpFirstName;
                    //LOM.LpLastName = LPayee.LpLastName;
                    //LOM.LpTradingName = LPayee.LpTradingName;

                    //db.Entry(LOM).State = EntityState.Modified;
                    //await db.SaveChangesAsync();
                    //////////////////----------------------

                    //Delete the existing Parent
                    var ExistingParent = db.LPayeeParents.Where(p => p.LppPayeeId == LPayee.Id).ToList();
                    db.LPayeeParents.RemoveRange(ExistingParent);
                    db.SaveChanges();
                    //Check if the Parent Payee exist then we will set parent id in LPayeeParents and delete previous
                    if (LppParentPayeeId != 0)
                    {
                        var ParentPayeeModel = new LPayeeParent { LppParentPayeeId = LppParentPayeeId, LppPayeeId = LPayee.Id, LppEffectiveStartDate = LPayee.LpEffectiveStartDate };
                        db.LPayeeParents.Add(ParentPayeeModel);
                        db.SaveChanges();
                    }
                    ////add portfolio to Payees
                    if (!string.IsNullOrEmpty(PortfolioList))
                    {
                        //delete all portfolios from MEntityPortfolios which are (assigned to Payees and are availablein the list for Logged In user) and add the new Portfolios
                        //Get list of portfolios associated with Logged In user
                        var LUserId = db.LUsers.Where(p => p.LuUserId.Equals(LoggedInUserId)).FirstOrDefault().Id;
                        var ExistingPortfolios = db.MEntityPortfolios.Where(p => p.MepEntityType == "LUsers").Where(p => p.MepEntityId == LUserId).Select(p => p.MepPortfolioId).ToList();
                        var PayeePortfolioList = db.MEntityPortfolios.Where(p => p.MepEntityType.Equals("LPayees", StringComparison.OrdinalIgnoreCase))
                            .Where(p => p.MepEntityId.Equals(LPayee.Id)).Where(p => ExistingPortfolios.Contains(p.MepPortfolioId)).ToList();
                        var compcode = db.GCompanies.Where(p => p.Id.Equals(LPayee.LpCompanyId)).FirstOrDefault().GcCode;
                        var PayeeRoleID = db.AspNetRoles.Where(p => p.Name.Equals("Payee")).Where(p => p.CompanyCode.Equals(compcode.ToString())).FirstOrDefault().Id;
                        db.MEntityPortfolios.RemoveRange(PayeePortfolioList);
                        db.SaveChanges();
                        var PortfolioArray = PortfolioList.Split(',').ToList();
                        foreach (var portfolio in PortfolioArray)
                        {
                            var PortfolioId = Convert.ToInt32(portfolio);
                            var MentityPortfolio = new MEntityPortfolio { MepEntityId = LPayee.Id, MepEntityType = "LPayees", MepPortfolioId = PortfolioId, MepRoleId = PayeeRoleID.ToString() };
                            db.MEntityPortfolios.Add(MentityPortfolio);
                            db.SaveChanges();
                        }
                    }
                    //check if channel is is changed then only update the Portfolios
                    //if (ExistingPayeeDetails.LpChannelId != LPayee.LpChannelId)
                    //{
                    //Add Porfolio to Payee having same Primary Channel , Business Unit and Chanel
                    //var NewPortfolio = new List<LPortfolio>();
                    //if (LPayee.LpBusinessUnit.Equals("Both", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    NewPortfolio = db.LPortfolios.Where(p => p.LpBusinessUnit.Equals("CBU", StringComparison.OrdinalIgnoreCase)|| p.LpBusinessUnit.Equals("EBU", StringComparison.OrdinalIgnoreCase)).Where(p => p.LpChannelId == LPayee.LpChannelId).ToList();
                    //}
                    //else
                    //{
                    //    NewPortfolio = db.LPortfolios.Where(p => p.LpBusinessUnit.Equals(LPayee.LpBusinessUnit, StringComparison.OrdinalIgnoreCase)).Where(p => p.LpChannelId == LPayee.LpChannelId).ToList();
                    //}
                    //    if (NewPortfolio != null)
                    //    {
                    //        //remove existing portfolio
                    //        var ExistingPortfolio = db.MEntityPortfolios.Where(p => p.MepEntityType.Equals("LPayees")).Where(p => p.MepEntityId == LPayee.Id).ToList();
                    //        foreach (var xx in ExistingPortfolio)
                    //        {
                    //            db.MEntityPortfolios.Remove(xx);
                    //            db.SaveChanges();
                    //        }

                    //    foreach (var PF in NewPortfolio)
                    //    {
                    //        var PortfolioId = PF.Id;
                    //        var MEntityPortfolio = new MEntityPortfolio { MepPortfolioId =PortfolioId, MepEntityId = LPayee.Id, MepEntityType = "LPayees" };
                    //        db.MEntityPortfolios.Add(MEntityPortfolio);
                    //        db.SaveChanges();
                    //    }
                    //    }
                    // }

                    //////if (!string.IsNullOrEmpty(AttachedFiles))
                    //////{
                    //////    var FilesArray = AttachedFiles.Split(',').ToList();
                    //////    foreach (var file in FilesArray)
                    //////    {
                    //////        var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = LPayee.LpCreatedById, LsdUpdatedById = LPayee.LpUpdatedById, LsdFileName = file, LsdFilePath = AttachmentPath, LsdEntityType = "LPayees", LsdEntityId = LPayee.Id, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedDateTime = DateTime.UtcNow };
                    //////        db.LSupportingDocuments.Add(LSupportingDocuments);
                    //////        db.SaveChanges();
                    //////    }
                    //////}

                    //Add Entry in Audit Log
                    Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Edit",
                           "Update", LPayee.LpUpdatedById, DateTime.UtcNow, LPayee.WFStatus, LPayee.WFStatus,
                          WFDetails.RwfBaseTableName, LPayee.Id, LPayee.LpFirstName + " " + LPayee.LpLastName+" ("+LPayee.LpPayeeCode+")", WFDetails.Id, LPayee.WFCompanyId, string.Empty, LPayee.WFRequesterRoleId,null);

                }
                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    {
                        //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                    }
                    else
                    {
                        throw ex;//This exception will be handled in FilterConfig's CustomHandler
                    }
                }
                transaction.Commit();
            }
            //using (var transaction = db.Database.BeginTransaction())
            //{
            //    db.Database.ExecuteSqlCommand("exec SPTestUpdatePayee " + LPayee.Id.ToString() + " , N'" + strPFName + "' , N'" + strPLName + "'");
            //    transaction.Commit();
            //}
                
            return StatusCode(HttpStatusCode.NoContent);
        }

        
        // POST: api/LPayees
        [ResponseType(typeof(LPayee))]
        public async Task<IHttpActionResult> PostLPayee(LPayeeDecryptedViewModel LPayee, string LoggedInRoleId, string AttachmentPath, string UserName, string Workflow, string Source, string UserLobbyId)
        {
            //if (!ModelState.IsValid)//Commenting this on 11 Apr 2017 will uncomment once issue is found
            //{
            //    //return BadRequest(ModelState);
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "PAYEE")));
            //}
            int? PayeeId = null;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //Note: Calculate Ordinal based on the Current Role who has created Manual Adjustment based on RoleId and Opco and WorkflowName
                    var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LPayee.LpCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault().LwfcOrdinalNumber;
                    var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    var ConfigData = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == LPayee.LpCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault();

                    LPayee.WFAnalystId = (ConfigData.LwfcActingAs == "Analyst") ? LPayee.WFRequesterId : null;
                    LPayee.WFOrdinal = Ordinal;
                    LPayee.WFType = WFDetails.RwfWFType;
                    LPayee.WFStatus = "Saved";
                    LPayee.LpCreatedByForm = true;//Created by Form is true as this method is called from app
                    //Shivani - 21Jun2019 - This code has been moved to Trigger. Keeping it for future reference
                    //WIAM handling. When WIAM requested for Create for Payee and in the mean time, same payee is created from form/upload
                    //then later WIAM lobby user is accepted by SA in system.
                    /*bool IsLobbyUser = false;
                    if (!string.IsNullOrEmpty(Source) && "Lobby".Equals(Source))
                    {
                        IsLobbyUser = true;
                    }
                    if (IsLobbyUser)
                    {
                        //match payee on PayeeCode basis
                        var ExistingPayee = db.LPayees.Where(p => p.LpPayeeCode == LPayee.LpPayeeCode).Where(p=>p.LpCompanyId == LPayee.LpCompanyId).FirstOrDefault();
                        if (ExistingPayee != null)
                        {
                            var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
                            string ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, ExistingPayee.LpEmail).FirstOrDefault();
                            var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeEmail2UponApproval").Where(p => p.LetCompanyId == ExistingPayee.WFCompanyId).FirstOrDefault();
                            //get lobby user
                            int iUserLobbyId = Convert.ToInt32(UserLobbyId);
                            var LobbyUser = db.LUserLobbies.Where(a => a.Id == iUserLobbyId).FirstOrDefault();
                            string ExistingPayeeStatus = ExistingPayee.WFStatus;
                            switch (ExistingPayeeStatus.ToLower())
                            {
                                case "saved"://Set CraeteLogin to 1 and send welcomeEmail2
                                case "inProgress":
                                    ExistingPayee.LpCreateLogin = true;
                                    db.Entry(ExistingPayee).State = EntityState.Modified;
                                    db.SaveChanges();
                                    if (EmailTemplate != null)
                                    {
                                        string EmailBody = EmailTemplate.LetEmailBody;
                                        EmailBody = EmailBody.Replace("###EmailAddress###", ExistingPayee.LpEmail);
                                        string EmailSubject = EmailTemplate.LetEmailSubject;
                                        Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, ExistingPayee.WFRequesterId, ExistingPayee.WFRequesterId, "Test Vodafone Lite SES", null, null, null);
                                    }
                                    //4.update lobby status
                                    LobbyUser.Status = "Accepted"; LobbyUser.UpdatedDateTime = DateTime.UtcNow;
                                    LobbyUser.UpdatedBy = UserName; LobbyUser.UpdatedByRoleId = Convert.ToInt32(LoggedInRoleId);
                                    db.Entry(LobbyUser).State = EntityState.Modified;
                                    db.SaveChanges();
                                    break;
                                case "completed":
                                case "suspended":
                                    ExistingPayee.LpCreateLogin = true;
                                    db.Entry(ExistingPayee).State = EntityState.Modified;
                                    db.SaveChanges();


                                    if (ExistingPayee.LpEmail != LobbyUser.Email)
                                    {
                                        //1.update PayeeEmail
                                        ExistingPayee.LpEmail = LobbyUser.Email;
                                        db.Entry(ExistingPayee).State = EntityState.Modified;
                                        db.SaveChanges();
                                        var PayeeAsFinOps = db.LUsers.Where(a => a.LuEmail == ExistingPayee.LpEmail).Where(a => a.WFCompanyId == ExistingPayee.WFCompanyId).FirstOrDefault();
                                        //2.update finops user if any
                                        if (PayeeAsFinOps != null)
                                        {
                                            PayeeAsFinOps.LuEmail = LobbyUser.Email;
                                            db.Entry(PayeeAsFinOps).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        var AspNetUser = db.AspNetUsers.Where(a => a.Email == ExistingPayee.LpEmail).FirstOrDefault();
                                        //3.update Aspnetuser data
                                        if (AspNetUser != null)
                                        {
                                            AspNetUser.Email = LobbyUser.Email;
                                            AspNetUser.UserName = LobbyUser.Email;
                                            db.Entry(AspNetUser).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        string CC = db.GCompanies.Where(a => a.Id == Convert.ToInt32(ExistingPayee.LpCompanyId)).FirstOrDefault().GcCode;
                                        string sql = "Update  XSchema" + CC.ToUpper() + ".XReportUsers set  XUserEmailID = {0}  where XUserEmailID= {1}";
                                        db.Database.ExecuteSqlCommand(sql, LobbyUser.Email, ExistingPayee.LpEmail);
                                    }
                                    //4.update lobby status
                                    LobbyUser.Status = "Accepted"; LobbyUser.UpdatedDateTime = DateTime.UtcNow;
                                    LobbyUser.UpdatedBy = UserName; LobbyUser.UpdatedByRoleId = Convert.ToInt32(LoggedInRoleId);
                                    db.Entry(LobbyUser).State = EntityState.Modified;
                                    db.SaveChanges();
                                    //send WelcomeEmail2UponApproval
                                    if (EmailTemplate != null)
                                    {
                                        string EmailBody = EmailTemplate.LetEmailBody;
                                        EmailBody = EmailBody.Replace("###EmailAddress###", ExistingPayee.LpEmail);
                                        string EmailSubject = EmailTemplate.LetEmailSubject;
                                        Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, ExistingPayee.WFRequesterId, ExistingPayee.WFRequesterId, "Test Vodafone Lite SES", null, null, null);
                                    }
                                    break;
                                case "inactive":
                                case "rejected":
                                    //4.update lobby status
                                    LobbyUser.Status = "SystemRejected"; LobbyUser.UpdatedDateTime = DateTime.UtcNow;
                                    LobbyUser.UpdatedBy = UserName; LobbyUser.UpdatedByRoleId = Convert.ToInt32(LoggedInRoleId);
                                    db.Entry(LobbyUser).State = EntityState.Modified;
                                    db.SaveChanges();
                                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2,
                                        "User already " + ExistingPayeeStatus + " in SOS. The orphan WIAM request will now be rejected from Lobby.’"));//type 2 error

                            }

                            transaction.Commit();
                            return Ok();
                        }
                    }
                    else*/
                    { 
                        /*Dup Payee and User check with existing payee/user with WFStatus=Saved, InProgress, Completed*/
                        var DeniedStatus = "Saved,InProgress,Completed".Split(',').ToList();
                        var ExistingUser = db.LUsers.Where(p => p.LuEmail == LPayee.LpEmail && DeniedStatus.Contains(p.WFStatus)).FirstOrDefault();
                        var ExistingPayee = db.LPayees.Where(p => p.LpEmail == LPayee.LpEmail && DeniedStatus.Contains(p.WFStatus)).FirstOrDefault();
                        if (ExistingUser != null || ExistingPayee != null)
                        {
                            throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, "This Email is already Active in the system. Cannot assign existing active email to this Payee"));//type 2 error
                        }
                    }
                    //Call global method to save Payee
                    PayeeId = Globals.ExecutePostLPayee(LPayee);
                    //return Ok(ds.Tables[0]);
                    //db.LPayees.Add(LPayee);
                    //await db.SaveChangesAsync();

                    if (PayeeId.HasValue)
                    {
                        //Populate PayeeId
                        LPayee.Id = PayeeId.Value;
                        ////add portfolio to Payees
                        if (!string.IsNullOrEmpty(LPayee.ParameterCarrier))
                        {
                            var PortfolioArray = LPayee.ParameterCarrier.Split(',').ToList();
                            var Company = db.GCompanies.Where(p => p.Id == LPayee.WFCompanyId).FirstOrDefault();
                            var PayeeRoleId = db.AspNetRoles.Where(p => p.CompanyCode == Company.GcCode).Where(p => p.Name == "Payee").FirstOrDefault().Id;
                            foreach (var portfolio in PortfolioArray)
                            {
                                var PortfolioId = Convert.ToInt32(portfolio);
                                var MentityPortfolio = new MEntityPortfolio { MepEntityId = LPayee.Id, MepEntityType = "LPayees", MepPortfolioId = PortfolioId, MepRoleId = PayeeRoleId };
                                db.MEntityPortfolios.Add(MentityPortfolio);
                                db.SaveChanges();
                            }
                        }
                        //update UserLobby
                        if (!string.IsNullOrEmpty(Source) && "Lobby".Equals(Source))
                        {
                            int iUserLobbyId = Convert.ToInt32(UserLobbyId);
                            //string qry = "update LUserLobby set status='Accepted' ,UpdatedDateTime={0},UpdatedBy={1},UpdatedByRoleId={2} where Id = {3} ";
                            //db.Database.ExecuteSqlCommand(qry, DateTime.UtcNow, UserName, LoggedInRoleId, UserLobbyId);
                            db.Database.ExecuteSqlCommand("update LUserLobby set status='Accepted', UpdatedDateTime='" + DateTime.UtcNow + "', UpdatedBy='" + UserName + "',UpdatedByRoleId=" + LoggedInRoleId + " where Id = " + iUserLobbyId);
                            await db.SaveChangesAsync();
                        }


                        //var Portfolio = new List<LPortfolio>();
                        //if (LPayee.LpBusinessUnit.Equals("Both", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    Portfolio = db.LPortfolios.Where(p => p.RChannel.RcPrimaryChannel == LPayee.LpPrimaryChannel).Where(p => p.LpBusinessUnit.Equals("EBU", StringComparison.OrdinalIgnoreCase) || p.LpBusinessUnit.Equals("CBU", StringComparison.OrdinalIgnoreCase)).Where(p => p.LpChannelId == LPayee.LpChannelId).Where(p => p.LpIsActive == true).ToList();

                        //}
                        //else
                        //{
                        //    //Add Porfolio to Payee having same Primary Channel , Business Unit and Chanel
                        //    Portfolio = db.LPortfolios.Where(p => p.RChannel.RcPrimaryChannel.Equals(LPayee.LpPrimaryChannel, StringComparison.OrdinalIgnoreCase)).Where(p => p.LpBusinessUnit.Equals(LPayee.LpBusinessUnit, StringComparison.OrdinalIgnoreCase)).Where(p => p.LpChannelId == LPayee.LpChannelId).Where(p => p.LpIsActive == true).ToList();
                        //}

                        //if (Portfolio != null)
                        //{
                        //    foreach (var PF in Portfolio)
                        //    {
                        //        var MEntityPortfolio = new MEntityPortfolio { MepPortfolioId = PF.Id, MepEntityId = LPayee.Id, MepEntityType = "LPayees" };
                        //        db.MEntityPortfolios.Add(MEntityPortfolio);
                        //        db.SaveChanges();
                        //    }
                        //}
                        //add row in LSupportingDocuments if attachments are added while adding payee
                        if (!string.IsNullOrEmpty(LPayee.LpFileNames))
                        {
                            var FilesArray = LPayee.LpFileNames.Split(',').ToList();
                            foreach (var file in FilesArray)
                            {
                                var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = LPayee.LpCreatedById, LsdUpdatedById = LPayee.LpUpdatedById, LsdFileName = file, LsdFilePath = AttachmentPath, LsdEntityType = "LPayees", LsdEntityId = LPayee.Id, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedDateTime = DateTime.UtcNow };
                                db.LSupportingDocuments.Add(LSupportingDocuments);
                                db.SaveChanges();
                            }
                        }
                        //Add Entry in Audit Log
                        Globals.ExecuteSPAuditLog(Workflow, "Audit", null, "Create",
                               "Create", LPayee.LpUpdatedById, DateTime.UtcNow, LPayee.WFStatus, LPayee.WFStatus,
                              WFDetails.RwfBaseTableName, LPayee.Id, LPayee.LpFirstName + " " + LPayee.LpLastName+" ("+LPayee.LpPayeeCode+")", WFDetails.Id, LPayee.WFCompanyId, string.Empty, LPayee.WFRequesterRoleId, null);
                        // RK Logic to push data to next ordinal
                        //var CompanyId = LPayee.LpCompanyId;
                        //db.Database.ExecuteSqlCommand("update LPayees set WFOrdinal = " + (Ordinal + 1) + ", WFCurrentOwnerId = (select dbo.FNGetUserForAllocation('Lpayees','Lpayees'," + LPayee.Id.ToString() + "," + Ordinal.ToString() + "," + CompanyId + ")) where id = " + LPayee.Id + "");
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, "Payee could not be added."));//type 2 error
                    }
                }

                catch (DbEntityValidationException dbex)
                {
                    transaction.Rollback();
                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                    {
                        //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                    }
                    else
                    {
                        throw ex;//This exception will be handled in FilterConfig's CustomHandler
                    }
                }
                transaction.Commit();
            }
            return Ok(PayeeId);
        }

        // DELETE: api/LPayees/5
        [ResponseType(typeof(LPayee))]
        public async Task<IHttpActionResult> DeleteLPayee(int id, string UserName, string Workflow)
        {
            LPayee LPayee = await db.LPayees.FindAsync(id);
            if (LPayee == null)
            {
                //return NotFound();
                //CITY/POST CODE could not be found. Send appropriate response to the request.
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "PAYEE")));
            }
            try
            {
                db.LPayees.Remove(LPayee);
                await db.SaveChangesAsync();
                return Ok(LPayee);
            }
            catch (DbEntityValidationException dbex)
            {

                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {

                if (ex.GetBaseException().GetType() == typeof(SqlException))//check for exception type
                {
                    //Throw this as HttpResponse Exception to let user know about the mistakes they made, correct them and retry.
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, GetCustomizedErrorMessage(ex)));//type 2 error
                }
                else
                {
                    throw ex;//This exception will be handled in FilterConfig's CustomHandler
                }
            }
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LPayeeExists(int id)
        {
            return db.LPayees.Count(e => e.Id == id) > 0;
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //First check whether exception is sqlException type 
            //if (ex.GetType().IsAssignableFrom(typeof(SqlException))|| ex.InnerException.GetType().IsAssignableFrom(typeof(SqlException)))
            //{
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //TODO Bring all the constraints defined in LBatches,LbatchFiles,Lpayees and LParentPayees because weare inserting data in all four tables in this controller.
            //  if (SqEx.Message.IndexOf("UQ_LBatches_LbBatchNumber_LbCompanyId", StringComparison.OrdinalIgnoreCase) >= 0)
            // return ("Can not insert duplicate Batch");

            if (SqEx.Message.IndexOf("FK_LPayees_LPayeeParents_PayeeId", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PAYEES", "CHILD PAYEE"));
            if (SqEx.Message.IndexOf("VPayees", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CanNotUpdateDeleteErrorMessage, "PAYEES", "VIEW(S)"));
            else if (SqEx.Message.IndexOf("UQ_LPayees_LpCompanyId_LpPayeeCode", StringComparison.OrdinalIgnoreCase) >= 0)
                return (string.Format(Globals.CannotInsertDuplicateErrorMessage, "PAYEES", "PAYEE CODE"));

            //Depending upon the constraint failed return appropriate error message
            //Something else failed return original error message as retrieved from database
            else
            {
                //callGlobals.ExecuteSPLogError SP here and log SQL SqEx.Message
                //Add complete Url in description
                var UserName = "";//System.Web.HttpContext.Current.Session["UserName"] as string;
                string UrlString = Convert.ToString(Request.RequestUri.AbsolutePath);
                var ErrorDesc = "";
                var Desc = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                if (Desc.Count() > 0)
                    ErrorDesc = string.Join(",", Desc);
                string[] s = Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at 2nd and action name at 3 rd index position
                Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", s[2], s[3], SqEx.Message, UserName, "Type2", ErrorDesc, "resolution", "L2Admin", "field", 0, "New");
                //Globals.LogError(SqEx.Message, ErrorDesc);
                return Globals.SomethingElseFailedInDBErrorMessage;
            }
        }

        [HttpGet]
        public IHttpActionResult GetParentByPayeeId(int PayeeId)
        {
            var ParentTable = Globals.GetParentByPayeeId(PayeeId);
            return Ok(ParentTable);

        }
        //[ResponseType(typeof(LPayee))]
        public IHttpActionResult GetLPayeeByPayeeCode(string PayeeCode, int CompanyId)
        {

            int PayeeId = db.LPayees.Where(x => x.LpPayeeCode == PayeeCode && x.LpCompanyId == CompanyId).Select(x=>x.Id).FirstOrDefault();
            // var PayeeData = Globals.ExecuteSPGetFilteredPayeeResults(id, null, null, null, null, null, null, null, null, null, null, null);//db.LPayees.Where(p => p.LpUserId == PayeeUserId).FirstOrDefault().Id;

            return Ok(PayeeId);


        }
        [HttpGet]
        public IHttpActionResult UploadPayee(string FileName, string UserName, string LoggedInRoleId, int iCompanyId, string WorkflowName, string UpdatedBy)
        {
            DataSet dsErrors = new DataSet();
            var BatchModel = new LBatch();
            var RawQuery = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo.SQ_BatchNumber");
            var Task = RawQuery.SingleAsync();
            var BatchNumber = Task.Result;
            Task.Dispose();
            var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkflowName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == iCompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == WorkflowName).FirstOrDefault().LwfcOrdinalNumber;
            using (var transaction = db.Database.BeginTransaction())
            {
                //make entry to Batch and BatchFile
                BatchModel = new LBatch { WFCompanyId = iCompanyId, WFRequesterRoleId = LoggedInRoleId,
                WFType = WFDetails.RwfWFType, WFOrdinal = Ordinal, WFCurrentOwnerId = UpdatedBy, WFStatus = "Saved",
                LbRecordCount = null, WFRequesterId = UpdatedBy, LbBatchNumber = BatchNumber,
                LbBatchType = "LPayees", LbCompanyId = iCompanyId, LbStatus = "PayeeBatchAccepted",
                LbUpdatedBy = UpdatedBy, LbUploadStartDateTime = DateTime.UtcNow };
                db.LBatches.Add(BatchModel);
                db.SaveChanges();
                var LBatchFiles = new LBatchFile { LbfBatchId = BatchModel.Id, LbfFileName = FileName, LbfFileTimeStamp = DateTime.UtcNow };
                db.LBatchFiles.Add(LBatchFiles);
                db.SaveChanges();

                try
                {
                    
                    #region 
                    string excelConnectionString = string.Empty;

                    #region File reading
                    //CreateDebugEntry("Start reading file");
                    var CompanyDetails = db.GCompanies.Where(p => p.Id == iCompanyId).FirstOrDefault();
                    string S3BucketRootFolder = ConfigurationManager.AppSettings["SOSBucketRootFolder"];
                    string S3TargetPath = S3BucketRootFolder + CompanyDetails.GcCode + "/upload/payees/" + FileName;
                    var bytedata = Globals.DownloadFromS3(S3TargetPath, "");

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    DataSet dsPayee = new DataSet(); DataSet dsPortFolio = new DataSet();
                    DataTable dtdataPayee = null; DataTable dtdataPortfolio = null;
                    try
                    {
                        OleDbConnection con = null;
                        try
                        {
                            string fileExtension = System.IO.Path.GetExtension(FileName);
                            string name = System.IO.Path.GetFileNameWithoutExtension(FileName);
                            string FileName_New = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + "_UPLOAD" + fileExtension;

                            string path = ConfigurationManager.AppSettings["TempDocumentPath"];
                            string fullpath = path + "\\" + FileName_New;
                            System.IO.File.WriteAllBytes(fullpath, bytedata); // Save File
                                                                              //CreateDebugEntry("File saved from byte to excel.");
                                                                              //string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullpath + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
                                                                              //SG - 2020/19/02- OLEDB connectionstring will be read from web.config file
                            #region changes from shivani
                                //string connectionString = string.Format(ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"]);

                                //con = new System.Data.OleDb.OleDbConnection(connectionString);
                                //con.Open();
                                //OleDbDataAdapter cmd2 = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [Payees$]", con);
                                //cmd2.Fill(dsPayee);
                                //dtdataPayee = dsPayee.Tables[0];
                                //cmd2 = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [Portfolios$]", con);
                                //cmd2.Fill(dsPortFolio);
                                //dtdataPortfolio = dsPortFolio.Tables[0];
                                //con.Close();
                            #endregion
                            #region rk changes
                                string connectionString = string.Empty;
                                excelConnectionString = ConfigurationManager.AppSettings["MicrosoftOLEDBConnectionString"].Replace("{0}", fullpath);
                                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                                excelConnection.Open();
                                //dtdataPayee = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, "Payees$", null });
                                //dtdataPortfolio = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, "Portfolios$", null });
                                OleDbDataAdapter cmd2 = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [Payees$]", excelConnection);
                                cmd2.Fill(dsPayee);
                                dtdataPayee = dsPayee.Tables[0];
                                cmd2 = new System.Data.OleDb.OleDbDataAdapter("SELECT * from [Portfolios$]", excelConnection);
                                cmd2.Fill(dsPortFolio);
                                dtdataPortfolio = dsPortFolio.Tables[0];
                                excelConnection.Close();
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            DataTable dtE = new DataTable();
                            dtE.Columns.Add("ExceptionMessage");
                            dtE.Rows.Add(ex.ToString());
                            return Ok(dtE);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var models = new GErrorLog { GelUserName = "SOS", GelController = "LPayees", GelMethod = "UploadPayee", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
                        db.GErrorLogs.Add(models);
                        db.SaveChanges();
                    }
                    #endregion

                    dtdataPayee.Rows.RemoveAt(0);//Removing the 1st row of data table as it contains the column alias.
                    #region Prepare data for bulk insert FOR Xschema.Xpayee
                    System.Data.DataColumn newColumn1 = new System.Data.DataColumn("XCompanyId", typeof(System.String));
                    newColumn1.DefaultValue = iCompanyId.ToString();
                    dtdataPayee.Columns.Add(newColumn1);
                    System.Data.DataColumn newColumn2 = new System.Data.DataColumn("XChannelId", typeof(System.String));
                    dtdataPayee.Columns.Add(newColumn2);
                    System.Data.DataColumn newColumn3 = new System.Data.DataColumn("XSubChannelId", typeof(System.String));
                    dtdataPayee.Columns.Add(newColumn3);
                    //XFileNames
                    System.Data.DataColumn newColumn4 = new System.Data.DataColumn("XFileNames", typeof(System.String));
                    newColumn4.DefaultValue = FileName;
                    dtdataPayee.Columns.Add(newColumn4);
                    //XUserFriendlyFileNames
                    System.Data.DataColumn newColumn5 = new System.Data.DataColumn("XUserFriendlyFileNames", typeof(System.String));
                    newColumn5.DefaultValue = FileName;
                    dtdataPayee.Columns.Add(newColumn5);
                    //XCreatedDateTime
                    System.Data.DataColumn newColumn6 = new System.Data.DataColumn("XCreatedDateTime", typeof(System.DateTime));
                    newColumn6.DefaultValue = DateTime.UtcNow;
                    dtdataPayee.Columns.Add(newColumn6);
                    System.Data.DataColumn newColumn7 = new System.Data.DataColumn("XUpdatedDateTime", typeof(System.DateTime));
                    newColumn7.DefaultValue = DateTime.UtcNow;
                    dtdataPayee.Columns.Add(newColumn7);
                    System.Data.DataColumn newColumn8 = new System.Data.DataColumn("XBatchNumber", typeof(System.String));
                    newColumn8.DefaultValue = BatchNumber;
                    dtdataPayee.Columns.Add(newColumn8);
                    //AlreadyExists
                    System.Data.DataColumn newColumn9 = new System.Data.DataColumn("AlreadyExists", typeof(System.Boolean));
                    newColumn9.DefaultValue = false;
                    dtdataPayee.Columns.Add(newColumn9);

                    System.Data.SqlClient.SqlBulkCopy sqlBulk = new SqlBulkCopy(db.Database.Connection.ConnectionString);
                    sqlBulk.ColumnMappings.Add("XCompanyId", "XCompanyId");
                    sqlBulk.ColumnMappings.Add("XChannelId", "XChannelId");
                    sqlBulk.ColumnMappings.Add("XSubChannelId", "XSubChannelId");
                    sqlBulk.ColumnMappings.Add("LpPrimaryChannel", "XPrimaryChannel");
                    sqlBulk.ColumnMappings.Add("LpPayeeCode", "XPayeeCode");
                    sqlBulk.ColumnMappings.Add("LpEmail", "XEmail");
                    sqlBulk.ColumnMappings.Add("LpEffectiveStartDate", "XEffectiveStartDate");
                    sqlBulk.ColumnMappings.Add("LpEffectiveEndDate", "XEffectiveEndDate");
                    sqlBulk.ColumnMappings.Add("XFileNames", "XFileNames");
                    sqlBulk.ColumnMappings.Add("XUserFriendlyFileNames", "XUserFriendlyFileNames");
                    sqlBulk.ColumnMappings.Add("LpCanRaiseClaims", "XCanRaiseClaims");
                    sqlBulk.ColumnMappings.Add("LpChannelManager", "XChannelManager");
                    sqlBulk.ColumnMappings.Add("XCreatedDateTime", "XCreatedDateTime");
                    sqlBulk.ColumnMappings.Add("XUpdatedDateTime", "XUpdatedDateTime");
                    sqlBulk.ColumnMappings.Add("LpTIN", "XTIN");
                    sqlBulk.ColumnMappings.Add("LpDistributionChannel", "XDistributionChannel");
                    sqlBulk.ColumnMappings.Add("LpPosition", "XPosition");
                    sqlBulk.ColumnMappings.Add("XBatchNumber", "XBatchNumber");
                    sqlBulk.ColumnMappings.Add("WFComments", "Xcomments");
                    sqlBulk.ColumnMappings.Add("LpPhone", "XPhone");
                    sqlBulk.ColumnMappings.Add("LpAddress", "XAddress");
                    sqlBulk.ColumnMappings.Add("LpFirstName", "XFirstName");
                    sqlBulk.ColumnMappings.Add("LpLastName", "XLastName");
                    sqlBulk.ColumnMappings.Add("LpTradingName", "XTradingName");
                    sqlBulk.ColumnMappings.Add("A01", "XA01");
                    sqlBulk.ColumnMappings.Add("A02", "XA02");
                    sqlBulk.ColumnMappings.Add("A03", "XA03");
                    sqlBulk.ColumnMappings.Add("A04", "XA04");
                    sqlBulk.ColumnMappings.Add("A05", "XA05");
                    sqlBulk.ColumnMappings.Add("A06", "XA06");
                    sqlBulk.ColumnMappings.Add("A07", "XA07");
                    sqlBulk.ColumnMappings.Add("A08", "XA08");
                    sqlBulk.ColumnMappings.Add("A09", "XA09");
                    sqlBulk.ColumnMappings.Add("A10", "XA10");
                    sqlBulk.ColumnMappings.Add("LpCreateLogin", "XCreateLogin");
                    sqlBulk.ColumnMappings.Add("GiveCMRole", "GiveCMRole");
                    //RK Added mapping for ParentCode
                    sqlBulk.ColumnMappings.Add("LpParentCode", "XParentCode");
                    sqlBulk.DestinationTableName = "XSchema.XPayeeUpload";
                    #endregion
                    try
                    {
                        sqlBulk.WriteToServer(dtdataPayee);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var models = new GErrorLog { GelUserName = "SOS", GelController = "LPayees", GelMethod = "UploadPayee", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
                        db.GErrorLogs.Add(models);
                        db.SaveChanges();
                        DataTable dtE = new DataTable();
                        dtE.Columns.Add("ExceptionMessage");
                        return Ok(dtE);
                    }


                    #region Bulk inserrt for XSchema.XPayeePortfolios 
                    System.Data.DataColumn newColumn11 = new System.Data.DataColumn("XBatchNumber", typeof(System.String));
                    newColumn11.DefaultValue = BatchNumber;
                    dtdataPortfolio.Columns.Add(newColumn11);
                    System.Data.SqlClient.SqlBulkCopy sqlBulkPortfolio = new SqlBulkCopy(db.Database.Connection.ConnectionString);
                    sqlBulkPortfolio.ColumnMappings.Add("XBatchNumber", "XBatchNumber");
                    sqlBulkPortfolio.ColumnMappings.Add("Primary Channel", "XPrimaryChannel");
                    sqlBulkPortfolio.ColumnMappings.Add("Channel", "Xchannel");
                    sqlBulkPortfolio.ColumnMappings.Add("Business Unit", "XBusinessUnit");
                    sqlBulkPortfolio.ColumnMappings.Add("Payee Code", "XpayeeCode");
                    sqlBulkPortfolio.DestinationTableName = "XSchema.XPayeePortfolios";
                    try
                    {
                        sqlBulkPortfolio.WriteToServer(dtdataPortfolio);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var models = new GErrorLog { GelUserName = "SOS", GelController = "LPayees", GelMethod = "UploadPayee", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
                        db.GErrorLogs.Add(models);
                        db.SaveChanges();
                        DataTable dtE = new DataTable();
                        dtE.Columns.Add("ExceptionMessage");
                        return Ok(dtE);
                    }

                    #endregion
                    transaction.Commit();
                    
                    #endregion
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    var models = new GErrorLog { GelUserName = "SOS", GelController = "LClaims", GelMethod = "UploadClaims", GelErrorDateTime = DateTime.UtcNow, GelStackTrace = ex.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
                    db.GErrorLogs.Add(models);
                    db.SaveChanges();
                    DataTable dtE = new DataTable();
                    dtE.Columns.Add("ExceptionMessage");
                    return Ok(dtE);
                }
                //transaction.Commit();
            }
            var Query = "Exec dbo.USPValidatePayeesUploadData @UserID,@CompanyID,@BatchNo";
            SqlCommand cmd = new SqlCommand(Query);
            cmd.Parameters.AddWithValue("@UserID", UserName);
            cmd.Parameters.AddWithValue("@CompanyID", iCompanyId.ToString());
            cmd.Parameters.AddWithValue("@BatchNo", BatchNumber);
            dsErrors = Globals.GetData(cmd);
            return Ok();
        }
        [HttpGet]
        public IHttpActionResult GetGridDataFields(int CompanyId)
        {
            var Query = "Exec dbo.[SPGetUploadPayeeColumnHeaders] @CompanyID";
            DataTable dt = new DataTable();
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            conn.Close();
            //The Ado.Net code ends here

            return Ok(dt);
        }

        [HttpGet]
        public IHttpActionResult DownloadPayeeUploadErrors( int CompanyId, int BatchNumber)
        {
            var CompanyDetails = db.GCompanies.Where(p => p.Id == CompanyId).FirstOrDefault();
            string Filename = null;
            string Query = "select XPayeeCode as [Payee Code],XValidationMessage as [Validation Message] from XSchema.XPayeeUpload where XCompanyId="+ CompanyId+ " and XBatchNumber="+ BatchNumber+" and XValidationMessage is not null";
            DataSet ds = new DataSet(); 
            DataTable dtPayee = Globals.GetDdatainDataTable(Query);
            //R3.1 - SG 18082020 - Portfolio errors included in sheet as well.
            ds.Tables.Add(dtPayee);
            Query = "select XPayeeCode as [Payee Code],XValidationMessage as [Validation Message] from XSchema.XPayeePortfolios where XBatchNumber=" + BatchNumber + " and XValidationMessage is not null";
            DataTable dtPortfolio =  Globals.GetDdatainDataTable(Query);
            ds.Tables.Add(dtPortfolio);
            //rename tables
            ds.Tables[0].TableName = "Payees"; ds.Tables[1].TableName = "Portfolios";
            Filename = BatchNumber + "_PayeeUploadErrors" + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".xlsx";
            var TempPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyDetails.GcCode + "/upload/payees/";
            var OutPutMessage = Globals.ExportDataSetToExcel(ds, TempPath, Filename, "all text", "dd.mm.yyyy");
            //Globals.ExportToExcel(dtPayee, TempPath, Filename);
            return Ok(Filename);
        }

        public IHttpActionResult GetXUploadPayeeCountByBatchNumber(int CompanyId, int BatchNumber)
        {
            var Query = "select count(*) from XSchema.XPayeeUpload where XCompanyId={0} and XBatchNumber={1}";
            var count = db.Database.SqlQuery<int>(Query, CompanyId, BatchNumber).FirstOrDefault();
            return Ok(count);
        }
        [HttpGet]
        public IHttpActionResult GetXUploadPayeeByBatchNumber(int CompanyId, int BatchNumber, string sortdatafield, string sortorder, int? pagesize, int? pagenum,string FilterQuery)
        {
            DataTable dt = new DataTable();
            var Query = "exec SPGetXUploadPayeeByBatchNumber @CompanyId,@BatchNumber,@pagesize,@pagenum,@sortdatafield,@sortorder,@FilterQuery";
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@BatchNumber", BatchNumber);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : (object)sortorder);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : (object)sortdatafield);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : (object)FilterQuery);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            conn.Close();
            //The Ado.Net code ends here

            return Ok(dt);
        }
        [HttpGet]
        ///UploadValidatedPayeeBatch
        public IHttpActionResult UploadValidatedPayeeBatch(int CompanyId, int BatchNumber,string AspNetUserId,int LoggedinRoleId,string Workflow)
        {
            string Query = "exec SpUploadValidatedPayees {0},{1},{2},{3},{4}";
            //due to exception, using List<Object>
            //The data reader has more than one field. Multiple fields are not valid for EDM primitive or enumeration types.
            db.Database.SqlQuery<List<Object>>(Query, CompanyId, BatchNumber, AspNetUserId, LoggedinRoleId, Workflow).FirstOrDefault();

            //RK
            //Query = "exec SpPushUploadedPayeesToNextOrdinal {0},{1},{2},{3},{4}";
            ////due to exception, using List<Object>
            ////The data reader has more than one field. Multiple fields are not valid for EDM primitive or enumeration types.
            //db.Database.SqlQuery<List<Object>>(Query, CompanyId, BatchNumber, AspNetUserId, LoggedinRoleId, Workflow).FirstOrDefault();
            return Ok();
        }
        [HttpGet]
        public IHttpActionResult DeletePayeeUploadBatch(int Id)
        {
            var batch =db.LBatches.Where(a => a.Id == Id).FirstOrDefault();
                batch.LbStatus = "Deleted";
                db.Entry(batch).State = EntityState.Modified;
                db.SaveChanges();
            return Ok(); 
        }


    }
}
