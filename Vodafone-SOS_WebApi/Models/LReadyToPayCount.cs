using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class LReadyToPayCount
    {
        public int iRTPID { get; set; }
        public int iCompanyID { get; set; }
        public string strType { get; set; }
        public Boolean blnIsBatchList { get; set; }
        public string strRTPStatus { get; set; }
        public string strAction { get; set; }
        public string strPayBatchName { get; set; }
        public string strBatchCommPeriod { get; set; }
        public string strRTPData { get; set; }

        public string strCreatedBy { get; set; }
        public string strUpdatedBy { get; set; }
        public string strPortfolios { get; set; }


        public string sortdatafield { get; set; }

        public string sortorder { get; set; }
        public int pagesize { get; set; }
        public int pagenum { get; set; }
        public string FilterQuery { get; set; }

    }
}