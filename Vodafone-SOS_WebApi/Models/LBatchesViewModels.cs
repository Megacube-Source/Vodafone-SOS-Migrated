using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class LBatchViewModelForAnalystGrid
    {
        public int Id { get; set; }
        public Nullable<int> LbRawDataTableId { get; set; }
        public string LbCommissionPeriod { get; set; }
        public string LrdtName { get; set; }
        public int LbBatchNumber { get; set; }
        public int LbRecordCount { get; set; }
        public DateTime LbUploadStartDateTime { get; set; }
        public string LbAllocatedToId { get; set; }
        public string LbPrimaryChannel { get; set; }
        public string LbBusinessUnit { get; set; }
        public string LbSubChannel { get; set; }
    }

    public partial class LBatchViewModelForPayeeGrid
    {
        public int Id { get; set; }
        public string LbStatus { get; set; }
        public int LbBatchNumber { get; set; }
        public string LbfFileName { get; set; }
        public int? LbRecordCount { get; set; }
        public DateTime LbUploadStartDateTime { get; set; }
        public int? IsImport { get; set; }

    }
}