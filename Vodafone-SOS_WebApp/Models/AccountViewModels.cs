using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [RestrictSpecialChar]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class UnlockUserViewModel
    {
        [Required]
        [RestrictSpecialChar]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [RestrictSpecialChar]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [RestrictSpecialChar]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        //line added to add roles of the user 
        public List<AspnetRoleViewModel> Roles { get; set; }
        public int GcCompanyId { get; set; }

        //UserId represents Id in AspNetUser
        public string Id { get; set; }
        public bool IsManager { get; set; }
        public string FullName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        [Display(Name = "IsActive")]
        public bool Status { get; set; }

        public int LUserId { get; set; }

        public string ClientIPAddress { get; set; }
        public string MFAOTP { get; set; }
        public bool PolicyAccepted { get; set; }

        public List<MAspnetUsersGSecurityQuestion> ObjScurityQuestion { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        //object added to add company 
        [Required]
        [Display(Name = "Company")]
        public int GcCompanyId { get; set; }
        public bool IsActive { get; set; }

        //line to define role while registering user
        [Required]
        [Display(Name = "Role")]
        public List<string> Roles { get; set; }

       // public string Role { get; set; }

        public string UserId { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
        public int Id { get; set; }
        public string GsqQuestion { get; set; }

    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    //model for changing password added by shubham

    public class ChangePasswordBindingModel
    {
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string Password { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Question Id")]
        [Required(ErrorMessage = "Question is Required")]
        public int MAuqsqQuestionId { get; set; }

        public string GsqQuestion { get; set; }
        [Required(ErrorMessage = "Question1 is required")]

        [Display(Name = "Question1")]
        public int Question1 { get; set; }
        [Display(Name = "Question2")]
        [Required(ErrorMessage = "Question2 is required")]

        public int Question2 { get; set; }
        [Display(Name = "Question3")]
        [Required(ErrorMessage = "Question3 is required")]

        public int Question3 { get; set; }
        [RestrictSpecialChar]
        [Required(ErrorMessage = "Answer1 is required")]
        [MaxLength(200, ErrorMessage = "Answer1 can be maximum 200 characters")]

        public string Answer1 { get; set; }
        [RestrictSpecialChar]
        [Required(ErrorMessage = "Answer2 is required")]
        [MaxLength(200, ErrorMessage = "Answer2 can be maximum 200 characters")]

        public string Answer2 { get; set; }
        [Required(ErrorMessage = "Answer3 is required")]
        [RestrictSpecialChar]
        [MaxLength(200, ErrorMessage = "Answer3  can be maximum 200 characters")]
        public string Answer3 { get; set; }
        //public string MAugsqAnswer { get; set; }
        public string MAuqsqUserId { get; set; }

        //added by SG - for forgot password utility
        [MaxLength(8)]
        public string OTP { get; set; }
        public string OTPValidUpto{get;set;}

    }

    public class ResetPasswordBindingModel
    {
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string Password { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        

        //added by SG - for forgot password utility
        [MaxLength(8)]
        public string OTP { get; set; }
        public string OTPValidUpto { get; set; }

    }

    public class ResetSecurityQuestionsBindingModel
    {
        

        [Required]
        [Display(Name = "ID")]
        public int Id { get; set; }

        public string Email { get; set; }

        [Display(Name = "Question Id")]
        [Required(ErrorMessage = "Question is Required")]
        public int MAuqsqQuestionId { get; set; }

        public string GsqQuestion { get; set; }
        [Required(ErrorMessage = "Question1 is required")]

        [Display(Name = "Question1")]
        public int Question1 { get; set; }
        [Display(Name = "Question2")]
        [Required(ErrorMessage = "Question2 is required")]

        public int Question2 { get; set; }
        [Display(Name = "Question3")]
        [Required(ErrorMessage = "Question3 is required")]

        public int Question3 { get; set; }
        [RestrictSpecialChar]
        [Required(ErrorMessage = "Answer1 is required")]
        [MaxLength(200, ErrorMessage = "Answer1 can be maximum 200 characters")]

        public string Answer1 { get; set; }
        [RestrictSpecialChar]
        [Required(ErrorMessage = "Answer2 is required")]
        [MaxLength(200, ErrorMessage = "Answer2 can be maximum 200 characters")]

        public string Answer2 { get; set; }
        [Required(ErrorMessage = "Answer3 is required")]
        [RestrictSpecialChar]
        [MaxLength(200, ErrorMessage = "Answer3  can be maximum 200 characters")]
        public string Answer3 { get; set; }
        //public string MAugsqAnswer { get; set; }
        public string MAuqsqUserId { get; set; }

        //added by SG - for forgot password utility
        [MaxLength(8)]
        public string OTP { get; set; }
        public string OTPValidUpto { get; set; }

    }

    public  class MAspnetUsersGSecurityQuestion
    {
        public int Id { get; set; }
        public string MAuqsqUserId { get; set; }
        public int MAuqsqQuestionId { get; set; }
        public string MAugsqAnswer { get; set; }
    }

    public class CreateLoginViewModel
    {
        [Required]
        [Display(Name = "UserType")] 
        public string UserType { get; set; }

        [Required]
        [Display(Name = "Email")]
        [RestrictSpecialChar]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "CreateLogin")] 
        public bool CreateLogin { get; set; }
    }
}












