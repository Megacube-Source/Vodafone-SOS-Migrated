using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LDashboardConfigViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Type is required")]
        [Display(Name = "KPI Type")]
        [RestrictSpecialChar]
        public string KpiTypeName { get; set; }
        [Display(Name = "KPI Type")]
        public int KpiTypeId { get; set; }

        [Required(ErrorMessage = "Group is required")]
        [Display(Name = "KPI Group")]
        [RestrictSpecialChar]
        public string KpiGroupName { get; set; }
        [Display(Name = "KPI Group")]
        public int KpiGroupId { get; set; }

        [RestrictSpecialChar]
        public string KpiGroupNames { get; set; }
        [Required(ErrorMessage = "KPI is required")]
        [Display(Name = "KPI Name")]
        [RestrictSpecialChar]
        public string KpiName { get; set; }
        [Display(Name = "KPI Name")]
        public int KpiId { get; set; }

        //  [RestrictSpecialChar] as not required for dropdown fields
        public string KpiIds { get; set; }

        [RestrictSpecialChar]
        public string KpiNames { get; set; }

        [Display(Name = "Channel ")]
        [RestrictSpecialChar]
        public string PortfolioIds { get; set; }

        [Display(Name = "Payee ")]
        [RestrictSpecialChar]
        public string PayeeCodes { get; set; }

        [Display(Name = "Graph Type ")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string GraphType { get; set; }

        [Display(Name = "Dimension ")]
        [RestrictSpecialChar]
        public string Dimension { get; set; }

        [Required(ErrorMessage = "Label is required")]
        [Display(Name = "Label ")]
        public string TileLabel { get; set; }
        public bool IsGraph { get; set; }

        [RestrictSpecialChar]
        public string UserId { get; set; }

        [RestrictSpecialChar]
        public string RoleId { get; set; }
        public int TileOrdinal { get; set; }
        public int CompanyId { get; set; }

        [RestrictSpecialChar]
        public string TileType { get; set; }

        [RestrictSpecialChar]
        public string TileGroup { get; set; }


        [RestrictSpecialChar]
        public string PeriodCodes { get; set; }
        //tile Data
        public int TileNumber { get; set; }
        public decimal TileValue { get; set; }
        //GraphData

        [RestrictSpecialChar]
        public string XGraphItem { get; set; }
        public decimal XGraphValue { get; set; }

        [RestrictSpecialChar]
        public string Xportfolio { get; set; }

        [RestrictSpecialChar]
        public string XCommissionPeriod { get; set; }

        [RestrictSpecialChar]
        public string XPayeeName { get; set; }
        public string BatchStatus{ get; set; }
    }
}