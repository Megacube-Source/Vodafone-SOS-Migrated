using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class PayeeResultViewModel
    {
        public string FullName { get; set; }
        public string LpPayeeCode { get; set; }
        public int Id { get; set; }

        public Nullable<int> LppParentPayeeId { get; set; }
        public string LpFirstName { get; set; }
        public string LpLastName { get; set; }
        public string LpPrimaryChannel { get; set; }

        public string PortfolioId { get; set; }
    }
}