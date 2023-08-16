using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
using System.Configuration;
using System.IO;

namespace Vodafone_SOS_WebApi.Controllers
{
    public class LDashboardConfigController : ApiController
    {
        // GET: LDashboardConfig
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        public async Task<IHttpActionResult> GetKpiData(int identifier, string strType, string strCompanyCode, string strUserId, string strRoleId, string strPeriod, string strDimension, string strPortfolioList, string strPayeeList, string BatchStatus)
        {
            var RawQuery = db.Database.SqlQuery<LDashboardConfigViewModel>("exec spGetKpiData " + identifier.ToString() + ",'" + strType + "','" + strCompanyCode + "','" + strUserId + "','" + strRoleId + "','" + strPeriod + "','" + strDimension + "','" + strPortfolioList + "','" + strPayeeList + "','" + BatchStatus + "'");
            var Task = RawQuery.ToList();
            var ListData = Task.ToList();
            return Ok(ListData);
        }



        [HttpGet]
        public IHttpActionResult DownloadKpiData(int identifier, string strType, string strCompanyCode, string strUserId, string strRoleId, string strPeriod, string strDimension, string strPortfolioList, string strPayeeList, string BatchStatus, string UserName)
        {

            var CompanyDetails = db.GCompanies.Where(p => p.GcCode == strCompanyCode).FirstOrDefault();

            var LTile = db.LDashboardConfigs.Where(p => p.Id == identifier).Where(p => p.UserId == strUserId).Where(p => p.RoleId == strRoleId).Where(p => p.CompanyId == CompanyDetails.Id)
                 .Select(x => new {x.TileLabel}).FirstOrDefault();

          
            
            //var fileinitialname = ListData.FirstOrDefault().TileLabel;

            string RawQuery = ("exec spGetKpiData " + identifier.ToString() + ",'" + strType + "','" + strCompanyCode + "','" + strUserId + "','" + strRoleId + "','" + strPeriod + "','" + strDimension + "','" + strPortfolioList + "','" + strPayeeList + "','" + BatchStatus + "'");
            //var Task = RawQuery.ToList();
            //var ListData = Task.ToList();
            //Globals.ExportToExcel(dt, path, Filename);

            var FileName = LTile.TileLabel + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".xlsx";
            var FileNamewithoutextention = LTile.TileLabel + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmss");
           
            DataTable xx = Globals.GetDdatainDataTable(RawQuery);

           var TempPath = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyDetails.GcCode + "/" + UserName + "/" ;

            Globals.ExportToExcel(xx,  TempPath, FileName);


            //Globals.CreateExcelFromDataTable(xx, FileName, "", CompanyDetails.GcCode, UserName, FileNamewithoutextention);
            return Ok(FileName);


        }




        [ResponseType(typeof(LDashboardConfigViewModel))]
        public async Task<IHttpActionResult> GetConfigurationById(int id,string UserID, string RoleId,int CompanyId)
        {
            var LTile = db.LDashboardConfigs.Where(p => p.Id == id).Where(p=>p.UserId == UserID).Where(p=>p.RoleId==RoleId).Where(p=>p.CompanyId==CompanyId)
                .Select(x => new { x.Id, x.KpiTypeId, x.KpiGroupId, x.KpiIds, x.PortfolioIds, x.PayeeCodes, x.TileOrdinal, x.TileLabel,x.Dimension,x.GraphType }).FirstOrDefault();
            if (LTile == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "Tile")));
            }
            return Ok(LTile);
        }
        [HttpPost]
        public async Task<IHttpActionResult> SaveConfigurationSetting(LDashboardConfigViewModel LCVM)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                LDashboardConfig LC = new Models.LDashboardConfig();
                LC.Id = LCVM.Id;
                LC.UserId = LCVM.UserId;
                LC.RoleId = LCVM.RoleId;
                LC.KpiTypeId = LCVM.KpiTypeId;
                LC.KpiGroupId = LCVM.KpiGroupId;
                LC.KpiIds = LCVM.KpiIds;
                LC.PeriodIntervals = "";
                LC.PortfolioIds = LCVM.PortfolioIds;
                LC.PayeeCodes = LCVM.PayeeCodes;
                LC.Dimension = LCVM.Dimension;
                LC.GraphType = LCVM.GraphType;
                LC.TileOrdinal = LCVM.TileOrdinal;
                LC.TileLabel = LCVM.TileLabel;
                LC.CompanyId = LCVM.CompanyId;
                LC.IsGraph = LCVM.IsGraph;
                try
                {
                    if(LCVM.Id==0)
                    {
                        int? iOrdinal = db.LDashboardConfigs.Where(p => p.UserId == LCVM.UserId).Where(p => p.IsGraph == LCVM.IsGraph).Where(p => p.CompanyId == LCVM.CompanyId).Select(p => p.TileOrdinal).Max();
                        if (iOrdinal == null) iOrdinal = 0;
                        LC.TileOrdinal = iOrdinal + 1;
                        db.LDashboardConfigs.Add(LC);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        db.Entry(LC).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    
                }
            }
            return Ok();
        }
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteConfiguration(int id)
        {
            LDashboardConfig Config = await db.LDashboardConfigs.FindAsync(id);
            if (Config == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ChannelName")));
            }
            try
            {
                db.LDashboardConfigs.Remove(Config);
                await db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}