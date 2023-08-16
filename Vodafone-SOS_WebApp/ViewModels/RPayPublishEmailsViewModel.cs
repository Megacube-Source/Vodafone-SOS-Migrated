using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class RPayPublishEmailsViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        [EmailAddress]
        public string EmailIds { get; set; }

        public int CompanyId { get; set; }

        [RestrictSpecialChar]
        public string Department { get; set; }
    }
}