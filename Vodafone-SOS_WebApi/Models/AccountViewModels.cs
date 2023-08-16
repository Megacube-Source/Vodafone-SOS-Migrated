using System;
using System.Collections.Generic;

namespace Vodafone_SOS_WebApi.Models
{
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
    //method to login user and Get User Details To WebApp application
    //Class created by shubham for sending user details to webApp
    public class LoginViewModel
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool PolicyAccepted { get; set; }
        public List<AspnetRoleViewModel> Roles { get; set; }//using single object to return list of Id and Name of Role assigned to User
        public bool IsManager { get; set; }

        public string FullName { get; set; }
        //line added by shubham to add companyId
        public int GcCompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int LUserId { get; set; }

        public List<MAspnetUsersGSecurityQuestion> ObjScurityQuestion { get; set; }
    }

    //Created By Shubham to pass role name and id on login
    public partial class AspnetRoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool ShowDashboard { get; set; }
    }

}
