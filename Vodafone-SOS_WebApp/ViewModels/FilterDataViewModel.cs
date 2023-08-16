using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class FilterDataViewModel
    {
        public string filtervalue { get; set; }
        public string filtercondition { get; set; }
        public string filteroperator { get; set; }
        public string filterdatafield { get; set; }
        public string IsUsed { get; set; }
    }
}