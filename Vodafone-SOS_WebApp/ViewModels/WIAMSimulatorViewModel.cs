using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class CreateViewModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "E-mail Address is required")]
        [Display(Name ="E-mail Address")]
        [MaxLength(255, ErrorMessage = "E-mail Address can be maximum 255 characters")]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(255, ErrorMessage = "First Name can be maximum 255 characters")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(255, ErrorMessage = "Last Name can be maximum 255 characters")]
        public string LastName { get; set; }
        public string UserGroup { get; set; }
        [Required(ErrorMessage = "CompanyCode is required")]
        public string CompanyCode { get; set; }
        [Display(Name = "Requester Email")]
        public string RequesterEmail { get; set; }
        [Display(Name = "Manager - Email")]
        public string ManagerEmail { get; set; }
        [Required(ErrorMessage ="AD User Field is required")]
        [Display(Name = "Is AD User?")]
        public bool IsVFADUser { get; set; }
        [Required(ErrorMessage = "UserType is required")]
        public string UserType { get; set; }

        [Display(Name = "Mobile")]
        [MaxLength(20, ErrorMessage = "Phone can be maximum 20 characters")]
        public string Phone { get; set; }

        [Display(Name = "Payee Code")]
        [MaxLength(255, ErrorMessage = "Payee Code can be maximum 255 characters")]
        public string PayeeCode { get; set; }
        public string ErrorMessage { get; set; }

    }

    public class IndexViewModel
    {
        public string ErrorMessage { get; set; }
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
        [Required(ErrorMessage = "E-mail Address is required")]
        [Display(Name = "E-mail Address")]
        [MaxLength(255, ErrorMessage = "E-mail Address can be maximum 255 characters")]
        public string Email { get; set; }
        public string CompanyCode { get; set; }
        [Display(Name = "Requestor Email")]
        [MaxLength(255, ErrorMessage = "Requestor Email can be maximum 255 characters")]
        public string RequestorEmail { get; set; }
    }

    public class DisableViewModel
    {
        [Required(ErrorMessage = "E-mail Address is required")]
        [Display(Name = "E-mail Address")]
        [MaxLength(255, ErrorMessage = "E-mail Address can be maximum 255 characters")]
        public string Email { get; set; }
        public string CompanyCode { get; set; }

        [Display(Name = "Requestor Email")]
        [MaxLength(255, ErrorMessage = "Requestor Email can be maximum 255 characters")]

        public string RequestorEmail { get; set; }
    }

    public class RevokeViewModel
    {
        [Required(ErrorMessage = "E-mail Address is required")]
        [Display(Name = "E-mail Address")]
        [MaxLength(255, ErrorMessage = "E-mail Address can be maximum 255 characters")]
        public string Email { get; set; }

        [Display(Name = "Requestor Email")]
        [MaxLength(255, ErrorMessage = "Requestor Email can be maximum 255 characters")]
        public string RequestorEmail { get; set; }
    }
    public class UpdateViewModel
    {

        [Required(ErrorMessage = "Old E-mail Address is required")]
        [Display(Name = "Old E-mail Address")]
        [MaxLength(255, ErrorMessage = "Old E-mail Address can be maximum 255 characters")]
        public string OldEmail { get; set; }

        [Required(ErrorMessage = "New E-mail Address is required")]
        [Display(Name = "New E-mail Address")]
        [MaxLength(255, ErrorMessage = "New E-mail Address can be maximum 255 characters")]
        public string NewEmail { get; set; }

        [Display(Name = "Requestor Email")]
        [MaxLength(255, ErrorMessage = "Requestor Email can be maximum 255 characters")]
        public string RequestorEmail { get; set; }
    }
    public class SetUserGroupViewModel
    {
        [Required(ErrorMessage = "E-mail Address is required")]
        [Display(Name = "E-mail Address")]
        [MaxLength(255, ErrorMessage = "E-mail Address can be maximum 255 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Old UserGroup is required")]
        [Display(Name = "Old UserGroup")]
        public string OldUserGroup { get; set; }

        [Required(ErrorMessage = "New UserGroup is required")]
        [Display(Name = "New UserGroup")]
        public string NewUserGroup { get; set; }

        [Display(Name = "Requestor Email")]
        [MaxLength(255, ErrorMessage = "Requestor Email can be maximum 255 characters")]
        public string RequestorEmail { get; set; }
    }




}