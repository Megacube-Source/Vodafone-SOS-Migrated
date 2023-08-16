using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class LUserLobbyViewModel
    {
    }

    public partial class LobbyForCreateUserViewModel
    {
        public int Id { get; set; }
        public string CompanyCode { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string UserType { get; set; }
        public string PayeeCode { get; set; }
    }
    public partial class LobbyForFinOpsViewModel
    {
        public string LuEmail { get; set; }
        public string LuFirstName { get; set; }

        public string LuLastName { get; set; }
        public string LuPhone { get; set; }
    }

    public partial class LobbyForPayeesViewModel
    {
        public string LpEmail { get; set; }
        public string LpFirstName { get; set; }

        public string LpLastName { get; set; }
        public string LpPhone { get; set; }
        //LpPayeeCode
        public string LpPayeeCode { get; set; }
    }

    public class LobbyUserViewModel
    {
        public int Id { get; set; }
        public string CompanyCode { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Phone { get; set; }

        public string UserType { get; set; }
        public string PayeeCode { get; set; }
        public bool IsVFADUser { get; set; }
        public string Status { get; set; }

        public string UserGroup { get; set; }
        public string NewUserGroup { get; set; }
        public string NewEmail { get; set; }

        public string RequestorEmail { get; set; }
        public string ManagerEmail { get; set; }
        public string RequestType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public int? UpdatedByRoleId  { get;set;}

        public string Response { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string Comments { get; set; }


    }
}