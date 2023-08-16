using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class ADUserViewModel
    {
        //SamAccountName or username
        public string SamAccountName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public bool Status { get; set; }
       

    }

    public class CreateLoginViewModel
    { 
        public string Email { get; set; }
        public string UserType { get; set; }
        public bool CreateLogin { get; set; } 
    }

}