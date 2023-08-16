using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class PayeeCalcChartViewModel
    {
        public string CommissionPeriod { get; set; }
        public decimal PrelimCount { get; set; }
        public decimal CompletedCount { get; set; }
        public int RowNumber { get; set; }
    }
}