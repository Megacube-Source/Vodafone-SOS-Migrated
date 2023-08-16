//Code Review for this file (from security perspective) done

using System;
using System.Linq;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LCompanySpecificRawDataColumnsController : PrimaryController
    {
        ILRawDataTablesRestClient LRDT = new LRawDataTablesRestClient();
        IGCompaniesRestClient GCRC = new GCompaniesRestClient();
        ILCompanySpecificRawDataColumnsRestClient RestClient = new LCompanySpecificRawDataColumnsRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        string CompanyCode = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyCode"]);

        [ControllerActionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ControllerActionFilter]
        public ActionResult MappingRawDataColumns(Nullable<int> RawDataTableId,string FormType,string RawDataTableName,bool IsRawDataTableMapped)
        {
            ViewBag.RawDataTableId = RawDataTableId;
            ViewBag.FormType = FormType;
            ViewBag.RawDataTableName = RawDataTableName;
            ViewBag.IsRawDataTableMapped = IsRawDataTableMapped;
            //var CompanyDropdown = GCRC.GetAll();
            //ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName");
            return View();
        }

        [HttpPost, ActionName("MappingRawDataColumns")]
        [ControllerActionFilter]
        public ActionResult PostMappingRawDataColumns(Nullable<int> RawDataTableId, string model, string RawDataTableName)
        {
            try
            {
                //Calling api method to add /update grid data in LCompanySpecificRawData table
                RestClient.AddGridData(model, RawDataTableId,RawDataTableName,CompanyId);
                return RedirectToAction("Index","LRawDataTables");
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Data["ErrorMessage"];
                //var CompanyDropdown = GCRC.GetAll();
                //ViewBag.CompanyId = new SelectList(CompanyDropdown, "Id", "GcCompanyName");
                return RedirectToAction("Index", "LRawDataTables");
            }
        }

        //This method gets XSchema Tables list  by CompanyId as a parameter
        [ControllerActionFilter]
        public JsonResult GetXSchemaTableByCompanyId(int CompanyId)
        {
            var TableDropdown = LRDT.GetByCompanyId(CompanyId);
            var x = new SelectList(TableDropdown, "Id", "LrdtName");
            return Json(x, JsonRequestBehavior.AllowGet);
        }

        //This method loads data into grid which maps columns of XSchema tables with the LRaw Data Table . This column list is obtained by passing table id into a stored procedure (SpGetColumnNamesForXSchemaTable)
        [ControllerActionFilter]
        public JsonResult GetColumnMappingGrid(Nullable<int> RawDataTableId,string RawDataTableName,bool IsRawDataTableMapped)
        {
            ILRawDataTablesRestClient LRTRC = new LRawDataTablesRestClient();
            if(IsRawDataTableMapped)
            {
                var ApiData = RestClient.GetXSchemaColumns(RawDataTableId.Value);
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var xx = LRTRC.GetColumnsByTableName(RawDataTableName,CompanyCode);
                return Json(xx,JsonRequestBehavior.AllowGet);
            }
        }
        [ControllerActionFilter]
        public JsonResult GetAutoFillColumnMappingGrid(Nullable<int> TableId,string RawDataTableName)
        {
            //if (TableId.HasValue)
            //{
            ILRawDataTablesRestClient LRTRC = new LRawDataTablesRestClient();
                var NvarcharCount = 1;
                var IntCount = 1;
                var FloatCount = 1;
                var DateCount = 1;
                var DateTimeCount = 1;
            
            if (TableId.HasValue)
            {
              var  ApiData = RestClient.GetXSchemaColumns(TableId.Value).ToList();
                foreach (var data in ApiData)//iterate through all rows and assign default column of raw data to Xtable column
                {
                    switch (data.DataType)
                    {
                        case "nvarchar":
                        case "char":
                        case "varchar":
                        case "nchar":
                            var ColumnIndex = ("0" + NvarcharCount).ToString();
                            if (NvarcharCount < 100)
                            {
                                data.RawDataColumn = "A" + ColumnIndex.Substring(ColumnIndex.Length - 2);
                            }
                            else
                            {
                                data.RawDataColumn = "A" + ColumnIndex.Substring(ColumnIndex.Length - 3);
                            }
                            NvarcharCount++;
                            break;
                        case "datetime":
                            var DatetimeColumnIndex = ("0" + DateTimeCount).ToString();
                            data.RawDataColumn = "AT" + DatetimeColumnIndex.Substring(DatetimeColumnIndex.Length - 2);
                            DateTimeCount++;
                            break;


                        case "date":
                            var DateColumnIndex = ("0" + DateCount).ToString();
                            data.RawDataColumn = "AD" + DateColumnIndex.Substring(DateColumnIndex.Length - 2);
                            DateCount++;
                            break;
                        case "float":
                        case "numeric":
                        case "decimal":
                            var FloatColumnIndex = ("0" + FloatCount).ToString();
                            data.RawDataColumn = "AN" + FloatColumnIndex.Substring(FloatColumnIndex.Length - 2);
                            FloatCount++;
                            break;
                        case "int":
                        case "smallint":
                            var IntColumnIndex = ("0" + IntCount).ToString();
                            data.RawDataColumn = "AI" + IntColumnIndex.Substring(IntColumnIndex.Length - 2);
                            IntCount++;
                            break;

                    }
                }
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
            else
            {
              var  ApiData = LRTRC.GetColumnsByTableName(RawDataTableName, CompanyCode);
                foreach (var data in ApiData)//iterate through all rows and assign default column of raw data to Xtable column
                {
                    switch (data.DataType)
                    {
                        case "nvarchar":
                        case "char":
                        case "varchar":
                        case "nchar":
                            var ColumnIndex = ("0" + NvarcharCount).ToString();
                            if (NvarcharCount < 100)
                            {
                                data.RawDataColumn = "A" + ColumnIndex.Substring(ColumnIndex.Length - 2);
                            }
                            else
                            {
                                data.RawDataColumn = "A" + ColumnIndex.Substring(ColumnIndex.Length - 3);
                            }
                            NvarcharCount++;
                            break;
                        case "datetime":
                            var DatetimeColumnIndex = ("0" + DateTimeCount).ToString();
                            data.RawDataColumn = "AT" + DatetimeColumnIndex.Substring(DatetimeColumnIndex.Length - 2);
                            DateTimeCount++;
                            break;


                        case "date":
                            var DateColumnIndex = ("0" + DateCount).ToString();
                            data.RawDataColumn = "AD" + DateColumnIndex.Substring(DateColumnIndex.Length - 2);
                            DateCount++;
                            break;
                        case "float":
                        case "numeric":
                        case "decimal":
                            var FloatColumnIndex = ("0" + FloatCount).ToString();
                            data.RawDataColumn = "AN" + FloatColumnIndex.Substring(FloatColumnIndex.Length - 2);
                            FloatCount++;
                            break;
                        case "int":
                        case "smallint":
                            var IntColumnIndex = ("0" + IntCount).ToString();
                            data.RawDataColumn = "AI" + IntColumnIndex.Substring(IntColumnIndex.Length - 2);
                            IntCount++;
                            break;

                    }
                }
                return Json(ApiData, JsonRequestBehavior.AllowGet);
            }
               
               
            //}
            //    return Json(true, JsonRequestBehavior.AllowGet);
        }


        //method to get  list of columns of lrawdata to display as dropdown in mapping grid
        [ControllerActionFilter]
        public JsonResult GetLRawDataColumns()
        {
            var ApiData = RestClient.GetLRawDataColumns();
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }

       
    }
}