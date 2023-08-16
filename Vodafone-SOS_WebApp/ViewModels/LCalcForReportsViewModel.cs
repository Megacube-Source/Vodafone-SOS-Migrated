using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LCalcForReportsViewModel
    {
        [RestrictSpecialChar]
        public string Source { get; set; }


        //  [RestrictSpecialChar] as not required for dropdown fields
        public string PrimaryChannel { get; set; }

        [RestrictSpecialChar]
        public string PayeeId { get; set; }

        [RestrictSpecialChar]
        public string CommissionPeriod { get; set; }
        public Nullable<DateTime> FromOrderDate { get; set; }
        public Nullable<DateTime> ToOrderDate { get; set; }
        public Nullable<DateTime> FromConnectionDate { get; set; }
        public Nullable<DateTime> ToConnectionDate { get; set; }
        public Nullable<DateTime> FromTerminationDate { get; set; }
        public Nullable<DateTime> ToTerminationDate { get; set; }

        [RestrictSpecialChar]
        public string MinSubscriberNumber { get; set; }

        [RestrictSpecialChar]
        public string MaxSubscriberNumber { get; set; }

        [RestrictSpecialChar]
        public string MinBAN { get; set; }

        [RestrictSpecialChar]
        public string MaxBAN { get; set; }

        [RestrictSpecialChar]
        public string MinIMEI { get; set; }

        [RestrictSpecialChar]
        public string MaxIMEI { get; set; }


        //  [RestrictSpecialChar] as not required for dropdown fields
        public string ActivityType { get; set; }

        [RestrictSpecialChar]
        public string ProductCode { get; set; }

        [RestrictSpecialChar]
        public string CommissionType { get; set; }
        public Nullable<int> MinContractDuration { get; set; }
        public Nullable<int> MaxContractDuration { get; set; }
        public Nullable<decimal> MinCommissionAmount { get; set; }
        public Nullable<decimal> MaxCommissionAmount { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        [RestrictSpecialChar]
        public string CompanyCode { get; set; }

        public Nullable<bool> IsGridLoading { get; set; }
    }
}