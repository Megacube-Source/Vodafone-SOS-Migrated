using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class AssigneeListViewModel
    {
        [RestrictSpecialChar]
        public string FullName { get; set; }

        [RestrictSpecialChar]
        public string Id { get; set; }
    }
}