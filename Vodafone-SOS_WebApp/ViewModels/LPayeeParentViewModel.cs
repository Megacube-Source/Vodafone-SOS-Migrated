using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LPayeeParentViewModel
    {
        public int Id { get; set; }
        public int LppPayeeId { get; set; }
        public int LppParentPayeeId { get; set; }
        public System.DateTime LppEffectiveStartDate { get; set; }
        public Nullable<System.DateTime> LppEffectiveEndDate { get; set; }
    }
}