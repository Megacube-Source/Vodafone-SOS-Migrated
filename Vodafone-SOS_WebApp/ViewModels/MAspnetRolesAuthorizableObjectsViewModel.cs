using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class MAspnetRolesAuthorizableObjectsViewModel
    {
        [RestrictSpecialChar]
        public string RoleId { get; set; }

        [RestrictSpecialChar]
        public string RoleName { get; set; }

        [RestrictSpecialChar]
        public string ActionKey { get; set; }

        [RestrictSpecialChar]
        public string Count { get; set; }

    }
}