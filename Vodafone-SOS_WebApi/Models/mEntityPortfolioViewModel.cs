using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class mEntityPortfolioViewModel
    {
        //RK added this view model to send the portfolio list to WebAPI while uploading payee list
        public int MepPortfolioId { get; set; }
        public int MepEntityId { get; set; }
        public string MepEntityType { get; set; }
        public string PayeeCode { get; set; }
    }
}