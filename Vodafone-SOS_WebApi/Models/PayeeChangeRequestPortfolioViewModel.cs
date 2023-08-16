using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class PayeeChangeRequestPortfolioViewModel
    {
        public List<UploadPayeeViewModel> PayeeModels { get; set; }
        public List<LChangeRequestViewModel> ChangeRequests { get; set; }
        public List<mEntityPortfolioViewModel> Portfolios { get; set; }
    }
}