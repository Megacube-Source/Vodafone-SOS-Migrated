//Code Review for this file (from security perspective) done

using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;
using NPOI.HSSF.UserModel;
using System.Configuration;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LCompanySpecificColumnsController : PrimaryController
    {
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string OpCoCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);
        string LoggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        ILPayeesRestClient LPRC = new LPayeesRestClient();
        ILCompanySpecificColumnsRestClient RestClient = new LCompanySpecificColumnsRestClient();
        [ControllerActionFilter]
        public ActionResult Index(string ChooseForm)
        {
            ViewBag.ChooseForm = ChooseForm;
            ILDropDownsRestClient DDRC = new LDropDownsRestClient();
            ViewBag.DropdownId = new SelectList(DDRC.GetByCompanyId(CompanyId),"Id", "LdName");
            System.Web.HttpContext.Current.Session["Title"] = "Manage Company Specific Columns";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult SaveGridSelection(string[] model,string banner, string ChooseForm)
        {
            try
            {
                var Arr = model[0].Split(',');
                //delete all existing records for payee/claim
                RestClient.DeleteAllFormColumns(ChooseForm,CompanyId);
                var CompanySpecificForms = new LCompanySpecificForms();
                for (var i = 0; i < Arr.Length; i = i + 10)
                {
                    try
                    {
                        var CompanyLabel = new LCompanySpecificColumnViewModel();
                       
                        CompanyLabel.LcscColumnName = Arr[i].ToString();
                        CompanyLabel.LcscLabel = (string.IsNullOrEmpty(Arr[i + 1].ToString())) ? null : Arr[i + 1].ToString();
                        CompanyLabel.LcscDisplayOnForm = Convert.ToBoolean(Arr[i + 2]);
                        CompanyLabel.LcscIsMandatory= Convert.ToBoolean(Arr[i + 3]);
                        //JS added a constraint that do not save DropDownID where Display On Form = 0
                        if (CompanyLabel.LcscDisplayOnForm)
                        {
                            if (!string.IsNullOrEmpty(Arr[i + 4]))
                                CompanyLabel.LcscDropDownId = Convert.ToInt32(Arr[i + 4]);
                        }
                        CompanyLabel.LcscOrdinalPosition = Convert.ToInt32(Arr[i + 5]);
                        if(!string.IsNullOrEmpty(Arr[i + 6]))
                        CompanyLabel.LcscIsReportParameter = Convert.ToBoolean(Arr[i + 6]);
                        if (!string.IsNullOrEmpty(Arr[i + 7]))
                            CompanyLabel.LcscReportParameterOrdinal = Convert.ToInt32(Arr[i + 7]);
                        CompanyLabel.LcscCompanyId = CompanyId;
                        CompanyLabel.LcscDataType = ConvertSqlDataTypeToGridDataType(Convert.ToString(Arr[i + 8]));
                        CompanyLabel.LcscTooltip = (string.IsNullOrEmpty(Arr[i + 9].ToString())) ? null : Arr[i + 9].ToString();

                        ViewBag.ColumnTooltip = CompanyLabel.LcscTooltip;

                        if (ChooseForm.Equals("Payee"))
                        {
                            CompanyLabel.LcscTableName = "LPayees";
                            CompanySpecificForms.FormName = "Payee";
                        }
                        else if (ChooseForm.Equals("Claims"))
                        {
                            CompanyLabel.LcscTableName = "LClaims";
                            CompanySpecificForms.FormName = "Claims";
                        }
                        else if (ChooseForm.Equals("Calculations"))
                        {
                            //JS Directed to save data type with ~ tinda seperated string in LCompanySpecificColumns table. Later we will add a column for this
                            CompanyLabel.LcscTableName = "XCalc";
                            CompanySpecificForms.FormName = "Calculations";
                            //CompanyLabel.LcscColumnName+= "~"+ ConvertSqlDataTypeToGridDataType(Convert.ToString(Arr[i + 8]));
                        }
                        else if (ChooseForm.Equals("Pay"))
                        {
                            CompanyLabel.LcscTableName = "XPay";
                            CompanySpecificForms.FormName = "Pay";
                        }
                        
                        RestClient.Add(CompanyLabel);

                        
                    }
                    
                    catch(Exception ex)
                    {
                        //if any record is causing issues then remove previous added records from mapping table
                        RestClient.DeleteAllFormColumns(ChooseForm, CompanyId);
                        TempData["Error"] += ex.Data["ErrorMessage"].ToString();
                        return RedirectToAction("Index", new { ChooseForm = ChooseForm });
                    }

                }

                //for save and update the Bannertext value
                var existingbannerDetail = RestClient.getBannerDetail(CompanyId, CompanySpecificForms.FormName);
                if (existingbannerDetail == null)
                {
                    CompanySpecificForms.BannerText = banner;
                    CompanySpecificForms.CompanyId = CompanyId;
                    CompanySpecificForms.FormName = CompanySpecificForms.FormName;
                    RestClient.AddBannerDetail(CompanySpecificForms);
                }
                else               
                {
                    var id = existingbannerDetail.FirstOrDefault().Id;
                    var dataforupdate = RestClient.getBannerDetailByID(id);
                    dataforupdate.BannerText = banner;
                    dataforupdate.CompanyId = CompanyId;
                    dataforupdate.FormName = CompanySpecificForms.FormName;
                    RestClient.Update(dataforupdate);
                }

               var updatedBannerValue =  getBannerDetail(CompanySpecificForms.FormName);
                ViewBag.updatedBannerValue = updatedBannerValue.Data;
              
                //RK 28102017 stopped creating the file from here as it will get created while downloading template from generic grid.
                //try
                //{
                //    CreateDownloadableTemplate(ChooseForm);
                //}
                //catch (Exception ex)
                //{
                //    TempData["Error"] = ex.Data["ErrorMessage"].ToString();

                //}

                TempData["Message"] = "Company Specific Columns has been sucessfully added";
                return RedirectToAction("Index",new {ChooseForm=ChooseForm });
            }
            catch
            {
                TempData["Error"] += "Company Specific Columns could not be added";
                return RedirectToAction("Index",new {ChooseForm=ChooseForm });
            }
        }


        public JsonResult getBannerDetail(string Selection)
        {

            switch (Selection)
            {

                case "Payee":
                    string formname = "Payee";
                    var ApiData = RestClient.getBannerDetail(CompanyId, formname);
                    if (ApiData ==null)
                    {
                        return Json("", JsonRequestBehavior.AllowGet);
                      
                    }
                    else
                    {
                        ApiData.FirstOrDefault().BannerText = ApiData.FirstOrDefault().BannerText;
                        return Json(ApiData.FirstOrDefault().BannerText, JsonRequestBehavior.AllowGet);
                    }
                    

                case "Claims":
                    string formname1 = "Claims";
                    var ApiData1 = RestClient.getBannerDetail(CompanyId, formname1);
                    if (ApiData1 == null)
                    {
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ApiData1.FirstOrDefault().BannerText = ApiData1.FirstOrDefault().BannerText;
                        return Json(ApiData1.FirstOrDefault().BannerText, JsonRequestBehavior.AllowGet);
                    }

                  

                case "Calculations":
                    string formname2 = "Calculations";
                    var ApiData2 = RestClient.getBannerDetail(CompanyId, formname2);
                    if (ApiData2 == null)
                    {
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ApiData2.FirstOrDefault().BannerText = ApiData2.FirstOrDefault().BannerText;
                        return Json(ApiData2.FirstOrDefault().BannerText, JsonRequestBehavior.AllowGet);
                    }

                    


                case "Pay":
                    string formname3 = "Pay";
                    var ApiData3 = RestClient.getBannerDetail(CompanyId, formname3);
                    if (ApiData3 == null)
                    {
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ApiData3.FirstOrDefault().BannerText = ApiData3.FirstOrDefault().BannerText;
                        return Json(ApiData3.FirstOrDefault().BannerText, JsonRequestBehavior.AllowGet);
                    }

                   

                default:
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
        }

        //Convert 
        public string ConvertSqlDataTypeToGridDataType(string SqlDataType)
        {
            switch (SqlDataType)
            {
                case "varchar":
                case "nvarchar":
                    return "string";
                case "int":
                    return "int";
                case "date":
                case "datetime":
                    return "date";
                case "bit":
                    return "bool";
                case "bigint":
                    return "int";
                case "decimal":
                case "float":
                case "numeric":
                    return "float";
            }
            return string.Empty;
        }

        [ControllerActionFilter]
        public JsonResult GetColumnsGrid(string Selection)
        {
            ILClaimsRestClient LCRC = new LClaimsRestClient();
            switch(Selection)
            { 
                case "Payee":
                    if (RestClient.GetPayeeColumnsByCompanyIdForGrid(CompanyId).Count() > 1)
                    {
                        var ApiData = RestClient.GetPayeeColumnsByCompanyIdForGrid(CompanyId).Select(p => new { p.DataType, p.LcscIsReportParameter,p.LcscReportParameterOrdinal,p.LdName,p.LcscOrdinalPosition,ColumnLabel=p.LcscLabel, p.LcscTooltip, ColumnName =p.LcscColumnName, p.IsNullable, CanBeDisplayed = p.LcscDisplayOnForm,IsManadatory=p.LcscIsMandatory,p.LcscDropDownId }).OrderByDescending(p => p.CanBeDisplayed).ThenBy(p => p.LcscOrdinalPosition);
                        return Json(ApiData, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var ApiData = LPRC.GetLPayeesColumnsForGrid().Select(p => new { p.DataType, p.LcscIsReportParameter, p.LcscReportParameterOrdinal, p.LdName, p.LcscOrdinalPosition, p.ColumnName, p.LcscTooltip, p.IsNullable, CanBeDisplayed = (p.IsNullable == "NO") ? true : false, IsManadatory = (p.IsNullable == "NO") ? true : false }).OrderByDescending(p => p.CanBeDisplayed).ThenBy(p => p.LcscOrdinalPosition);
                        return Json(ApiData, JsonRequestBehavior.AllowGet);
                    }
                case "Claims":
                    if (RestClient.GetClaimsColumnsByCompanyIdForGrid(CompanyId).Count() > 1)
                    {
                        var ApiData = RestClient.GetClaimsColumnsByCompanyIdForGrid(CompanyId).Select(p => new { p.LcscDataType,p.DataType, p.LcscIsReportParameter, p.LcscReportParameterOrdinal, p.LdName, p.LcscOrdinalPosition, ColumnLabel = p.LcscLabel, p.LcscTooltip,ColumnName = p.LcscColumnName, p.IsNullable, CanBeDisplayed = p.LcscDisplayOnForm,IsManadatory=p.LcscIsMandatory,p.LcscDropDownId }).OrderByDescending(p => p.CanBeDisplayed).ThenBy(p => p.LcscOrdinalPosition);
                        return Json(ApiData, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var ApiData1 = LCRC.GetCompanySpecificLabels().Select(p => new { p.DataType, p.LcscIsReportParameter, p.LcscReportParameterOrdinal, p.LdName, p.LcscOrdinalPosition, p.ColumnName,p.LcscTooltip ,p.IsNullable, CanBeDisplayed = (p.IsNullable == "NO") ? true : false, IsManadatory = (p.IsNullable == "NO") ? true : false }).OrderByDescending(p => p.CanBeDisplayed).ThenBy(p => p.LcscOrdinalPosition);
                        return Json(ApiData1, JsonRequestBehavior.AllowGet);
                    }
                case "Calculations":
                    var ApiData2 = RestClient.GetCalculationsColumnsByCompanyIdForGrid(CompanyId);

                    if ((ApiData2.ElementAt(0).Id)==0)
                    {
                        var xx = ApiData2.Select(p => new { p.DataType, ColumnLabel = p.LcscLabel, p.LcscIsReportParameter, p.LcscReportParameterOrdinal, p.LdName, p.LcscOrdinalPosition, p.ColumnName, p.IsNullable, CanBeDisplayed = (p.IsNullable == "NO") ? true : false, IsManadatory = (p.IsNullable == "NO") ? true : false }).OrderByDescending(p => p.CanBeDisplayed).ThenBy(p => p.LcscOrdinalPosition);
                        return Json(xx, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var yy = ApiData2.Select(p => new { IsNullable=(p.LcscIsMandatory)?"NO":"YES", p.DataType, ColumnLabel = p.LcscLabel, p.LcscIsReportParameter, p.LcscReportParameterOrdinal, p.LdName, p.LcscOrdinalPosition, p.ColumnName, CanBeDisplayed = p.LcscDisplayOnForm, IsManadatory = p.LcscIsMandatory }).OrderByDescending(p => p.CanBeDisplayed).ThenBy(p => p.LcscOrdinalPosition);
                        return Json(yy, JsonRequestBehavior.AllowGet);
                    }
                case "Pay":
                    var PayApiData = RestClient.GetPayColumnsByCompanyIdForGrid(CompanyId);
                    if ((PayApiData.ElementAt(0).Id) == 0)
                    {
                        var xx = PayApiData.Select(p => new { p.DataType, ColumnLabel = p.LcscLabel, p.LcscIsReportParameter, p.LcscReportParameterOrdinal, p.LdName, p.LcscOrdinalPosition, p.ColumnName, p.IsNullable, CanBeDisplayed = (p.IsNullable == "NO") ? true : false, IsManadatory = (p.IsNullable == "NO") ? true : false }).OrderByDescending(p => p.CanBeDisplayed).ThenBy(p => p.LcscOrdinalPosition);
                        return Json(xx, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var yy = PayApiData.Select(p => new { IsNullable = (p.LcscIsMandatory) ? "NO" : "YES", p.DataType, ColumnLabel = p.LcscLabel, p.LcscIsReportParameter, p.LcscReportParameterOrdinal, p.LdName, p.LcscOrdinalPosition, p.ColumnName, CanBeDisplayed = p.LcscDisplayOnForm, IsManadatory = p.LcscIsMandatory }).OrderByDescending(p => p.CanBeDisplayed).ThenBy(p => p.LcscOrdinalPosition);
                        return Json(yy, JsonRequestBehavior.AllowGet);
                    }

                default:
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        }

        //method to get LDropDown values from Api to be displayed in companyspecific columns grid
        //[ControllerActionFilter]
        public JsonResult GetLDropDowns()
        {
            ILDropDownsRestClient DDRC = new LDropDownsRestClient();
            var LDropDowns = DDRC.GetByCompanyId(CompanyId);
            return Json(LDropDowns,JsonRequestBehavior.AllowGet);
        }

        private void CreateDownloadableTemplate(string strFileType)
        {
            //RK 28102017
            //This method needs to be abandoned as now the file will getting created while user tries to download the same from generic grid.
            #region csv Method
            //string strInitial = "";
            //StringBuilder sb = new StringBuilder();
            //string columnName = "";
            //string strValToWrite = "";
            //if (strFileType == "Payee")
            //    strInitial = "Lp";
            //else
            //    strInitial = "Lc";
            //strInitial = "Lp";
            //var APIData = RestClient.GetPayeeColumnsByCompanyIdForGrid(CompanyId);
            //for (int j = 0; j < APIData.Count(); j++)
            //{
            //    columnName = APIData.ElementAt(j).ColumnName;
            //    if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
            //        strValToWrite = columnName + ",";
            //    else
            //        strValToWrite = strInitial + columnName + ",";
            //    sb.Append(strValToWrite);
            //}
            //sb.Remove(sb.Length - 1, 1);
            //sb.AppendLine();

            //var APIData1 = RestClient.GetPayeeColumnsByCompanyIdForGrid(CompanyId);
            //for (int j = 0; j < APIData.Count(); j++)
            //{
            //    if (APIData1.ElementAt(j).LcscLabel == null)
            //        strValToWrite = APIData1.ElementAt(j).ColumnName + ",";
            //    else
            //        strValToWrite = APIData1.ElementAt(j).LcscLabel + ",";
            //    sb.Append(strValToWrite);
            //}
            //sb.Remove(sb.Length - 1, 1);
            //sb.AppendLine();

            //if (strFileType == "Payee")
            //{
            //    System.IO.File.WriteAllText("C:\\Users\\rajkum\\Desktop\\Payee Upload Files\\AutoPayeeUploadTemplate.xls", sb.ToString());
            //}
            //else
            //{
            //    System.IO.File.WriteAllText("ClaimsUploadTemplate.xlsx", sb.ToString());
            //}
            #endregion
            #region Execl Method
            //try
            //{
            //    string strUploadBasePath = ConfigurationManager.AppSettings["UploadTemplatePath"].ToString();
            //    string strInitial = "Lc", columnName = "", columnVal = "";
            //    int iDx = 0;

            //    Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
            //    Microsoft.Office.Interop.Excel.Workbook xlWorkbook = ExcelApp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            //    Microsoft.Office.Interop.Excel.Sheets xlSheets = null;
            //    Microsoft.Office.Interop.Excel.Worksheet xlWorksheet = null;
            //    xlSheets = ExcelApp.Sheets;


            //    Microsoft.Office.Interop.Excel.Range formatRange;
            //    if (strFileType == "Payee")
            //    {
            //        Microsoft.Office.Interop.Excel.Worksheet xlWorksheet1 = null;
            //        xlWorksheet1 = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.Add(xlSheets[1],
            //                    Type.Missing, Type.Missing, Type.Missing);
            //        xlWorksheet1.Name = "Portfolios";
            //        ExcelApp.Cells[1, 1] = "Payee Code";
            //        ExcelApp.Cells[1, 2] = "Primary Channel";
            //        ExcelApp.Cells[1, 3] = "Channel";
            //        ExcelApp.Cells[1, 4] = "Business Unit";
            //        xlWorksheet1.Columns.AutoFit();

            //        xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.Add(xlSheets[1],
            //                    Type.Missing, Type.Missing, Type.Missing);
            //        xlWorksheet.Name = "Payees";
            //        strInitial = "Lp";
            //        iDx = 1;
            //        ExcelApp.Cells[1, iDx] = "LpParentCode";
            //        iDx++;
            //        var APIData = RestClient.GetPayeeColumnsByCompanyIdForGrid(CompanyId);

            //        for (int j = 0; j < APIData.Count(); j++)
            //        {
            //            if (Convert.ToBoolean(APIData.ElementAt(j).LcscDisplayOnForm))
            //            {
            //                columnName = APIData.ElementAt(j).LcscColumnName;
            //                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
            //                    columnName = APIData.ElementAt(j).LcscColumnName;
            //                else
            //                    columnName = strInitial + APIData.ElementAt(j).LcscColumnName;
            //                ExcelApp.Cells[1, iDx] = columnName;
            //                iDx++;
            //            }

            //        }
            //        ExcelApp.Cells[1, iDx] = "WFComments";
            //        iDx++;
            //        for (int k = 0; k < APIData.Count(); k++)
            //        {
            //            if (!Convert.ToBoolean(APIData.ElementAt(k).LcscDisplayOnForm))
            //            {
            //                columnName = APIData.ElementAt(k).LcscColumnName;
            //                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
            //                    columnName = APIData.ElementAt(k).LcscColumnName;
            //                else
            //                    columnName = strInitial + APIData.ElementAt(k).LcscColumnName;
            //                ExcelApp.Cells[1, iDx] = columnName;
            //                iDx++;
            //            }
            //        }


            //        iDx = 1;
            //        ExcelApp.Cells[2, iDx] = "Parent Code";
            //        iDx++;
            //        var APIData1 = RestClient.GetPayeeColumnsByCompanyIdForGrid(CompanyId);
            //        for (int j = 0; j < APIData1.Count(); j++)
            //        {
            //            if (Convert.ToBoolean(APIData1.ElementAt(j).LcscDisplayOnForm))
            //            {
            //                if (APIData1.ElementAt(j).LcscLabel == null)
            //                    columnVal = APIData1.ElementAt(j).LcscColumnName;
            //                else
            //                    columnVal = APIData1.ElementAt(j).LcscLabel;
            //                ExcelApp.Cells[2, iDx] = columnVal;
            //                if (APIData1.ElementAt(j).IsNullable != "YES")
            //                {
            //                    formatRange = (Microsoft.Office.Interop.Excel.Range)ExcelApp.Cells[2, iDx];
            //                    formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
            //                    //ExcelApp.Cells[2, iDx].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
            //                }

            //                iDx++;
            //            }
            //        }
            //        ExcelApp.Cells[2, iDx] = "Comments";
            //        Range hiddenRange = ExcelApp.Range[xlWorksheet.Cells[1, 1], xlWorksheet.Cells[1, 1]];
            //        hiddenRange.EntireRow.Hidden = true;

            //        xlWorksheet.Columns.AutoFit();



            //        //xlWorkbook.Sheets.Move("Payees", "Payees");
            //        //xlWorkbook.Sheets["Payees"].Move(xlWorkbook.Sheets[1]);
            //        //xlWorkbook.Sheets.Move(After: xlWorkbook.Sheets.Count);


            //        ExcelApp.ActiveWorkbook.SaveCopyAs(strUploadBasePath + "\\Payee Upload\\" + OpCoCode + "_PayeesUpload.xlsx");
            //        ExcelApp.ActiveWorkbook.Saved = true;
            //        xlWorkbook.Close();
            //        ExcelApp.Quit();
            //    }
            //    else
            //    {
            //        xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.Add(xlSheets[1],
            //                    Type.Missing, Type.Missing, Type.Missing);

            //        xlWorksheet.Name = "Claims";
            //        strInitial = "Lc";
            //        iDx = 1;
            //        var APIData = RestClient.GetClaimsColumnsByCompanyIdForGrid(CompanyId);
            //        for (int j = 0; j < APIData.Count(); j++)
            //        {
            //            if (Convert.ToBoolean(APIData.ElementAt(j).LcscDisplayOnForm))
            //            {
            //                columnName = APIData.ElementAt(j).LcscColumnName;
            //                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
            //                    columnName = APIData.ElementAt(j).LcscColumnName;
            //                else
            //                    columnName = strInitial + APIData.ElementAt(j).LcscColumnName;
            //                ExcelApp.Cells[1, iDx] = columnName;
            //                iDx++;
            //            }

            //        }
            //        iDx = 1;
            //        var APIData1 = RestClient.GetClaimsColumnsByCompanyIdForGrid(CompanyId);
            //        for (int j = 0; j < APIData1.Count(); j++)
            //        {
            //            if (Convert.ToBoolean(APIData1.ElementAt(j).LcscDisplayOnForm))
            //            {
            //                if (APIData1.ElementAt(j).LcscLabel == null)
            //                    columnVal = APIData1.ElementAt(j).LcscColumnName;
            //                else
            //                    columnVal = APIData1.ElementAt(j).LcscLabel;
            //                ExcelApp.Cells[2, iDx] = columnVal;
            //                if (APIData1.ElementAt(j).IsNullable != "YES")
            //                {
            //                    //ExcelApp.Cells[2, iDx].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
            //                    formatRange = (Microsoft.Office.Interop.Excel.Range)ExcelApp.Cells[2, iDx];
            //                    formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
            //                }
            //                iDx++;
            //            }
            //        }
            //        Range hiddenRange = ExcelApp.Range[xlWorksheet.Cells[1, 1], xlWorksheet.Cells[1, 1]];
            //        hiddenRange.EntireRow.Hidden = true;

            //        xlWorksheet.Columns.AutoFit();

            //        ExcelApp.ActiveWorkbook.SaveCopyAs(strUploadBasePath + "\\Claims Upload\\" + OpCoCode + "_ClaimsUpload.xls");
            //        ExcelApp.ActiveWorkbook.Saved = true;
            //        xlWorkbook.Close();
            //        ExcelApp.Quit();
            //    }
            //}


            //catch (Exception ex)
            //{
            //    ErrorLogsRestClient er = new ErrorLogsRestClient();
            //    GErrorLogViewModel errlog = new GErrorLogViewModel();
            //    errlog.GelController = "LCompanSpecificColumns";
            //    errlog.GelErrorDateTime = DateTime.Now;
            //    errlog.GelErrorDescription = ex.ToString();
            //    errlog.GelErrorOwner = "";
            //    errlog.GelErrorType = "test";
            //    errlog.GelFieldName = "create excel";
            //    errlog.GelMethod = "CreateDownloadableTemplate";
            //    errlog.GelResolution = "";
            //    errlog.GelSOSBatchNumber = 0;
            //    errlog.GelSourceProject = "WebAPP";
            //    errlog.GelStackTrace = ex.StackTrace.ToString();
            //    errlog.GelUserName = "SOS";
            //    er.Add(errlog);
            //}
            #endregion
            #region using NPOI dll
            try
            {
                if (strFileType == "Payee")
                {
                    var APIData = RestClient.GetPayeeColumnsByCompanyIdForGrid(CompanyId);
                    string strUploadBasePath = ConfigurationManager.AppSettings["UploadTemplatePath"].ToString();
                    string strFileName = strUploadBasePath + "\\Payee Upload\\" + OpCoCode + "_PayeesUpload.xlsx";
                    NPOI.HSSF.UserModel.HSSFWorkbook workbook = new NPOI.HSSF.UserModel.HSSFWorkbook();
                    ICell cell;
                    ISheet sheet1 = workbook.CreateSheet("Payees");
                    IRow row1 = sheet1.CreateRow(0);
                    string columnName = "";
                    string strInitial = "Lp";
                    int iDx = 0;
                    cell = row1.CreateCell(iDx);
                    cell.SetCellValue("LpParentCode");
                    iDx++;
                    for (int j = 0; j < APIData.Count(); j++)
                    {
                        if (Convert.ToBoolean(APIData.ElementAt(j).LcscDisplayOnForm))
                        {
                            columnName = APIData.ElementAt(j).LcscColumnName;
                            if (columnName != "WFComments")
                            {
                                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                    columnName = APIData.ElementAt(j).LcscColumnName;
                                else
                                    columnName = strInitial + APIData.ElementAt(j).LcscColumnName;
                                cell = row1.CreateCell(iDx);
                                cell.SetCellValue(columnName);
                                
                            }
                            iDx++;
                            if (columnName == "WFComments") iDx = iDx - 1;
                        }

                    }
                    cell = row1.CreateCell(iDx);
                    cell.SetCellValue("WFComments");
                    iDx++;
                    for (int k = 0; k < APIData.Count(); k++)
                    {
                        if (!Convert.ToBoolean(APIData.ElementAt(k).LcscDisplayOnForm))
                        {
                            columnName = APIData.ElementAt(k).LcscColumnName;
                            if (columnName != "WFComments")
                            {
                                if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                    columnName = APIData.ElementAt(k).LcscColumnName;
                                else
                                    columnName = strInitial + APIData.ElementAt(k).LcscColumnName;
                                cell = row1.CreateCell(iDx);
                                cell.SetCellValue(columnName);
                                
                            }
                            iDx++;
                            if (columnName == "WFComments") iDx = iDx - 1;
                        }
                    }
                    row1.ZeroHeight = true;
                    iDx = 0;
                    IRow row2 = sheet1.CreateRow(1);
                    cell = row2.CreateCell(iDx);
                    cell.SetCellValue("ParentCode");
                    iDx++;
                    string columnVal = "";
                    var APIData1 = RestClient.GetPayeeColumnsByCompanyIdForGrid(CompanyId);
                    for (int j = 0; j < APIData1.Count(); j++)
                    {
                        if (Convert.ToBoolean(APIData1.ElementAt(j).LcscDisplayOnForm))
                        {
                            
                                if (APIData1.ElementAt(j).LcscLabel == null)
                                    columnVal = APIData1.ElementAt(j).LcscColumnName;
                                else
                                    columnVal = APIData1.ElementAt(j).LcscLabel;
                            if (columnVal != "WFComments")
                            {
                                cell = row2.CreateCell(iDx);
                                if (APIData1.ElementAt(j).LcscIsMandatory)
                                {
                                    cell.SetCellValue(columnVal + " *");
                                }
                                else
                                {
                                    cell.SetCellValue(columnVal);
                                }
                            }
                            
                            iDx++;
                            if (columnVal == "WFComments") iDx = iDx - 1;
                        }
                    }
                    cell = row2.CreateCell(iDx);
                    cell.SetCellValue("Comments");

                    //Portfolio
                    ISheet sheetPort = workbook.CreateSheet("Portfolios");
                    IRow rowPort = sheetPort.CreateRow(0);
                    cell = rowPort.CreateCell(0);
                    cell.SetCellValue("Payee Code");
                    cell = rowPort.CreateCell(1);
                    cell.SetCellValue("Primary Channel");
                    cell = rowPort.CreateCell(2);
                    cell.SetCellValue("Channel");
                    cell = rowPort.CreateCell(3);
                    cell.SetCellValue("Business Unit");
                    workbook.CreateSheet();
                    GC.Collect();
                    FileStream xfile = new FileStream(strFileName, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                    workbook.Write(xfile);
                    xfile.Close();

                }
                else
                {

                    var APIData = RestClient.GetClaimsColumnsByCompanyIdForGrid(CompanyId);
                    string strUploadBasePath = ConfigurationManager.AppSettings["UploadTemplatePath"].ToString();
                    string strFileName = strUploadBasePath + "\\Claims Upload\\" + OpCoCode + "_ClaimsUpload.xls";
                    NPOI.HSSF.UserModel.HSSFWorkbook workbook = new NPOI.HSSF.UserModel.HSSFWorkbook();
                    ICell cell;
                    ISheet sheet1 = workbook.CreateSheet("Claims");
                    IRow row1 = sheet1.CreateRow(0);
                    string columnName = "";
                    string strInitial = "Lc";
                    int iDx = 0;
                    for (int j = 0; j < APIData.Count(); j++)
                    {
                        if (Convert.ToBoolean(APIData.ElementAt(j).LcscDisplayOnForm))
                        {
                            columnName = APIData.ElementAt(j).LcscColumnName;
                            if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                columnName = APIData.ElementAt(j).LcscColumnName;
                            else
                                columnName = strInitial + APIData.ElementAt(j).LcscColumnName;
                            cell = row1.CreateCell(iDx);
                            cell.SetCellValue(columnName);
                            iDx++;
                        }

                    }

                    for (int k = 0; k < APIData.Count(); k++)
                    {
                        if (!Convert.ToBoolean(APIData.ElementAt(k).LcscDisplayOnForm))
                        {
                            columnName = APIData.ElementAt(k).LcscColumnName;
                            if (columnName == "A01" || columnName == "A02" || columnName == "A03" || columnName == "A04" || columnName == "A05" || columnName == "A06" || columnName == "A07" || columnName == "A08" || columnName == "A09" || columnName == "A10")
                                columnName = APIData.ElementAt(k).LcscColumnName;
                            else
                                columnName = strInitial + APIData.ElementAt(k).LcscColumnName;
                            cell = row1.CreateCell(iDx);
                            cell.SetCellValue(columnName);
                            iDx++;
                        }
                    }
                    row1.ZeroHeight = true;
                    iDx = 0;
                    IRow row2 = sheet1.CreateRow(1);
                    string columnVal = "";
                    var APIData1 = RestClient.GetClaimsColumnsByCompanyIdForGrid(CompanyId);
                    for (int j = 0; j < APIData1.Count(); j++)
                    {
                        if (Convert.ToBoolean(APIData1.ElementAt(j).LcscDisplayOnForm))
                        {
                            if (APIData1.ElementAt(j).LcscLabel == null)
                                columnVal = APIData1.ElementAt(j).LcscColumnName;
                            else
                                columnVal = APIData1.ElementAt(j).LcscLabel;
                            cell = row2.CreateCell(iDx);
                            if (APIData1.ElementAt(j).LcscIsMandatory)
                            {
                                cell.SetCellValue(columnVal + " *");
                            }
                            else
                            {
                                cell.SetCellValue(columnVal);
                            }
                            iDx++;
                        }
                    }

                    workbook.CreateSheet();
                    GC.Collect();
                    FileStream xfile = new FileStream(strFileName, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
                    workbook.Write(xfile);
                    xfile.Close();
                }

            }
            catch (Exception ex)
            {
                ErrorLogsRestClient er = new ErrorLogsRestClient();
                GErrorLogViewModel errlog = new GErrorLogViewModel();
                errlog.GelController = "LCompanSpecificColumns";
                errlog.GelErrorDateTime = DateTime.Now;
                errlog.GelErrorDescription = ex.ToString();
                errlog.GelErrorOwner = "";
                errlog.GelErrorType = "test";
                errlog.GelFieldName = "create excel";
                errlog.GelMethod = "CreateDownloadableTemplate";
                errlog.GelResolution = "";
                errlog.GelSOSBatchNumber = 0;
                errlog.GelSourceProject = "WebAPP";
                errlog.GelStackTrace = ex.StackTrace.ToString();
                errlog.GelUserName = "SOS";
                er.Add(errlog);
            }

            #endregion
        }
    }
}