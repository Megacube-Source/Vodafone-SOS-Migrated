using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class PayeePortfolioViewModel
    {
        public virtual LPayee LPayee { get; set; }
        public virtual PayeeCalcViewModel PortfolioViewModel { get; set; }
    }
}