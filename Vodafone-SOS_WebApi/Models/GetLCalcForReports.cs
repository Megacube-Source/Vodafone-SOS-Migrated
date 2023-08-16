using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class GetLCalcForReports
    {
        public string Source { get; set; }
        public string PrimaryChannel { get; set; }

        public string PayeeList { get; set; }
      
        public string FromOrderDate { get; set; }
        public string ToOrderDate { get; set; }
        public string FromConnectionDate { get; set; }
        public string ToConnectionDate { get; set; }
        public string FromTerminationDate { get; set; }
        public string ToTerminationDate { get; set; }
        public string MinSubscriberNumber { get; set; }
        public string MaxSubscriberNumber { get; set; }
        public string MinBAN { get; set; }
        public string MaxBAN { get; set; }
        public string MinIMEI { get; set; }
        public string MaxIMEI { get; set; }

        public string ActivityType { get; set; }
        public string ProductCode { get; set; }

        public string CommissionType { get; set; }
        public string MinContractDuration { get; set; }
        public string MaxContractDuration { get; set; }
        public string MinCommissionAmount { get; set; }
        public string MaxCommissionAmount { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string CompanyCode { get; set; }
    }
}