using UserAccessManagement.Models;

namespace WIAM_SOS.ViewModel
{
    public class IndexViewModel
    {
        public string OperationType { get; set; }
        public string Email { get; set; }
        public string CompanyCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public bool? VFADUser { get; set; }
        public string ManagerEmail { get; set; }
        public string RequestorEmail { get; set; }
        public string PayeeCode { get; set; }
        public string UserType { get; set; }
        public string UserGroup { get; set; }
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
        public string OldUserGroup { get; set; }
        public string NewUserGroup { get; set; }

    }

    public class EnableViewModel
    {
        public string Email { get; set; }
        public string CompanyCode { get; set; }
        public string RequestorEmail { get; set; }
    }

    public class DisableViewModel
    {
        public string Email { get; set; }
        public string CompanyCode { get; set; }
        public string RequestorEmail { get; set; }
    }

    public class CreateViewModel
    {
        public string Email { get; set; }
        public string CompanyCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public bool VFADUser { get; set; }
        public string ManagerEmail { get; set; }
        public string RequestorEmail { get; set; }
        public string PayeeCode { get; set; }
        public string UserType { get; set; }
        public string UserGroup { get; set; }
    }

    public class RevokeViewModel
    {
        public string Email { get; set; }
        public string RequestorEmail { get; set; }
        //public string CompanyCode { get; set; }
    }
    public class UpdateViewModel
    {
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
        public string RequestorEmail { get; set; }
       // public string CompanyCode { get; set; }
    }
    public class SetUserGroupViewModel
    {
        public string Email { get; set; }
        public string OldUserGroup { get; set; }
        public string NewUserGroup { get; set; }
        public string RequestorEmail { get; set; }
       // public string CompanyCode { get; set; }
    }

    public class UserTypeViewModel
    {
        public LPayee Payee { get; set; }
        public LUser FinOps { get; set; }
        public string UserType { get; set; }
    }
}