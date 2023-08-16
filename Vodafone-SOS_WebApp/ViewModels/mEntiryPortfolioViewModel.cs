using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class mEntiryPortfolioViewModel
    {
        //RK added this view model to send the portfolio list to WebAPI while uploading payee list
        public int MepPortfolioId { get; set; }
        public int MepEntityId { get; set; }

        [RestrictSpecialChar]
        public string MepEntityType { get; set; }

        [RestrictSpecialChar]
        public string PayeeCode { get; set; }
    }
}