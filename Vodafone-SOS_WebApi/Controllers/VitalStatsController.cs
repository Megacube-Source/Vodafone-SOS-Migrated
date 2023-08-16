using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vodafone_SOS_WebApi.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class VitalStatsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        [HttpGet]
        private List<VitalStatsViewModel> VitalStatsView(string StartDate, string EndDate)
        {
            var vital = new List<VitalStatsViewModel>();
            var WFlows = db.RWorkFlows.ToList();
            var companies = db.GCompanies.ToList();
           

            for (int i = 0; i < WFlows.Count; i++)
            {
                var vitalmodel = new VitalStatsViewModel();
                vitalmodel.WorkflowName = WFlows.ElementAt(i).RwfName;
                String qry = "";

                switch (WFlows.ElementAt(i).RwfName)
                {
                    case "Users":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LuCreatedDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' ";
                        break;
                    case "UsersCR":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LcrCreatedDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' and WFType= '" + WFlows.ElementAt(i).RwfWFType + "' ";
                        break;
                    case "Payees":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LpCreatedDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' ";
                        break;
                    case "PayeesCR":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LcrCreatedDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' and WFType= '" + WFlows.ElementAt(i).RwfWFType + "' ";
                        break;
                    case "ManualAdjustments":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LbUploadStartDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' and WFType= '" + WFlows.ElementAt(i).RwfWFType + "' ";
                        break;
                    case "Claims":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LcCreatedDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' ";
                        break;
                    case "RawData":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LbUploadStartDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' and WFType= '" + WFlows.ElementAt(i).RwfWFType + "' ";
                        break;
                    case "Calc":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LbUploadStartDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' and WFType= '" + WFlows.ElementAt(i).RwfWFType + "' ";
                        break;
                    case "RefFiles":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LrfCreatedDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' ";
                        break;
                    case "Schemes":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LsCreatedDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' ";
                        break;
                    case "Pay":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LbUploadStartDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' and WFType= '" + WFlows.ElementAt(i).RwfWFType + "' ";
                        break;
                    case "Accruals":
                        qry = "select count(*)  from " + WFlows.ElementAt(i).RwfBaseTableName + " where LaCreatedDateTime BETWEEN '" + StartDate + "' AND '" + EndDate + "' ";
                        break;
                }
                string Query = "";
                for (int j = 0; j < companies.Count; j++)
                {
                    Query = qry + " and WFCompanyid=" + companies.ElementAt(j).Id + " ";
                    var xx = db.Database.SqlQuery<int>(Query).FirstOrDefault();
                    switch (companies.ElementAt(j).GcCode)
                    {
                        case "99":
                            vitalmodel.GRPCount = xx;
                            break;
                        case "AL":
                            vitalmodel.ALCount = xx;
                            break;
                        case "CZ":
                            vitalmodel.CZCount = xx;
                            break;
                        case "GB":
                            vitalmodel.GBCount = xx;
                            break;
                        case "GH":
                            vitalmodel.GHCount = xx;
                            break;
                        case "GR":
                            vitalmodel.GRCount = xx;
                            break;
                        case "HU":
                            vitalmodel.HUCount = xx;
                            break;
                        case "MT":
                            vitalmodel.MTCount = xx;
                            break;
                        case "QA":
                            vitalmodel.QACount = xx;
                            break;
                        case "RO":
                            vitalmodel.ROCount = xx;
                            break;
                        case "IN":
                            vitalmodel.INCount = xx;
                            break;



                    }

                }
                vital.Add(vitalmodel);
            }

            return vital;

        }

        [HttpGet]
        public IHttpActionResult GetVitalStatForOpcos(string StartDate, string EndDate)
        {
            var vital = VitalStatsView(StartDate, EndDate);
            return Ok(vital);
           
        }

        [HttpGet]

        public IHttpActionResult DownloadVitalStatsForReports(string StartDate, string EndDate)
        {

            try
            {
                var vital = VitalStatsView(StartDate, EndDate);


                //Create a excel in S drive for the VitalStats
                var CfileLocation = ConfigurationManager.AppSettings["CalcDocumentPath"] + "/ExportVitalStatsReport.xlsx";
                
                if (System.IO.File.Exists(CfileLocation))
                    System.IO.File.Delete(CfileLocation);
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet1 = workbook.CreateSheet("Sheet 1");
                IRow row1 = sheet1.CreateRow(0);
                var tb = new DataTable(typeof(VitalStatsViewModel).Name);
                PropertyInfo[] props = typeof(VitalStatsViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                for (var j = 0; j < props.Length; j++)
                {
                    // var displayName=PayeeModel.GetDisplayName()
                    var PropName = props[j].Name;
                    ICell cell = row1.CreateCell(j);
                    string columnName = PropName;//tb.Columns[j].ToString();
                    cell.SetCellValue(columnName);
                    //method to size column width and GC is used to avoid error System.argument exception

                    GC.Collect();
                    // tb.Columns.Add(PropName);
                    //  }

                }

                for (int i = 0; i < vital.Count(); i++)
                {
                    IRow row = sheet1.CreateRow(i + 1);
                    //var values = new object[props.Length];
                    for (var j = 0; j < props.Length; j++)
                    {
                        var value = props[j].GetValue(vital.ElementAt(i), null);
                        ICell cell = row.CreateCell(j);
                        if (value == null)
                        {
                            cell.SetCellValue("");
                        }
                        else
                        {
                            cell.SetCellValue(value.ToString());
                        }

                    }

                   
                }
                
                if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["CalcDocumentPath"]))//create directory if not present
                {
                    System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["CalcDocumentPath"]);
                }
                FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["CalcDocumentPath"] , "ExportVitalStatsReport.xlsx"), FileMode.Create, System.IO.FileAccess.Write);
                workbook.Write(xfile);

                xfile.Close();

                return Ok(CfileLocation);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex));
            }

        }

    }
}
