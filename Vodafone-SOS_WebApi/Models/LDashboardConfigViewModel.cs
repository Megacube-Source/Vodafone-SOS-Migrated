using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class LDashboardConfigViewModel
    {
        public int Id { get; set; }
        public string KpiTypeName { get; set; }
        public int KpiTypeId { get; set; }
        public string KpiGroupName { get; set; }
        public int KpiGroupId { get; set; }
        public string KpiGroupIds { get; set; }
        public string KpiGroupNames { get; set; }
        public string KpiName { get; set; }
        public int KpiId { get; set; }
        public string KpiIds { get; set; }
        public string KpiNames { get; set; }
        public string TileLabel { get; set; }

        //
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string PortfolioIds { get; set; }
        public string PayeeCodes { get; set; }
        public string Dimension { get; set; }
        public string GraphType { get; set; }
        public int TileOrdinal { get; set; }
        public int CompanyId { get; set; }
        public string TileType { get; set; }
        public string TileGroup { get; set; }
        public bool IsGraph { get; set; }
        public string PeriodCodes { get; set; }
        //tile Data
        public int TileNumber { get; set; }
        public decimal TileValue { get; set; }
        //GraphData
        public string XGraphItem { get; set; }
        public decimal XGraphValue { get; set; }
        public string Xportfolio { get; set; }
        public string XCommissionPeriod { get; set; }
        public string XPayeeName { get; set; }
        public string BatchStatus { get; set; }
    }
}