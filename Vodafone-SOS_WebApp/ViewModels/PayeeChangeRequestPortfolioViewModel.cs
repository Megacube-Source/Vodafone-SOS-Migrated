using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class PayeeChangeRequestPortfolioViewModel
    {
        public List<LPayeeViewModel> PayeeModels { get; set; }
        public List<LChangeRequestViewModel> ChangeRequests { get; set; }
        public List<mEntiryPortfolioViewModel> Portfolios { get; set; }
    }
}