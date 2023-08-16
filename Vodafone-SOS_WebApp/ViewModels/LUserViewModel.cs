using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LUserViewModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [MaxLength(128, ErrorMessage = "User Id can be maximum 128 characters")]
        [Display(Name = "User Id")]
        [RestrictSpecialChar]
        public string LuUserId { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int LuCompanyId { get; set; }

        [MaxLength(128, ErrorMessage = "Created By Id  can be maximum 128 characters")]
        [Required(ErrorMessage = "Created By Id is required")]
        [Display(Name = "Created By")]
        [RestrictSpecialChar]
        public string LuCreatedById { get; set; }

        [MaxLength(128, ErrorMessage = "Updated By Id can be maximum 128 characters")]
        [Required(ErrorMessage = "Updated By Id is required")]
        [Display(Name = "Updated By")]
        [RestrictSpecialChar]
        public string LuUpdatedById { get; set; }

        [MaxLength(128, ErrorMessage = "Reports To Id can be maximum 128 characters")]
        [Display(Name ="Reports To")]
        [RestrictSpecialChar]
        public string LuReportsToId { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(200, ErrorMessage = "First Name can be maximum 200 characters")]
        [Required(ErrorMessage = "First Name is required")]
        [RestrictSpecialChar]
        public string LuFirstName { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(200, ErrorMessage = "last Name can be maximum 200 characters")]
        [Required(ErrorMessage = "last Name is required")]
        [RestrictSpecialChar]
        public string LuLastName { get; set; }

        [Display(Name = "Email")]
        [MaxLength(200, ErrorMessage = "Email can be maximum 200 characters")]
        [Required(ErrorMessage = "Email is required")]
        [RestrictSpecialChar]
        [EmailAddress(ErrorMessage = "Invalid Email Address")] //R2.2.1 Added email validation.
        public string LuEmail { get; set; }

        [Display(Name = "Phone")]
        [MaxLength(20, ErrorMessage = "Phone can be maximum 20 Length")]
        [RestrictSpecialChar]
        public string LuPhone { get; set; }

        [Required(ErrorMessage = "Is Manager is required")]
        [Display(Name = "Is Manager")]
        public bool LuIsManager { get; set; }

        [Display(Name = "Band")]
        [MaxLength(20, ErrorMessage = "Band can be maximum 20 Length")]
        [RestrictSpecialChar]
        public string LuBand { get; set; }

        [Display(Name = "Status")]
        [MaxLength(50, ErrorMessage = "status can be maximum 50 characters")]
        [Required(ErrorMessage = "Status is required")]
        [RestrictSpecialChar]
        public string LuStatus { get; set; }

        [Required(ErrorMessage = "CreatedDateTime is required")]
        [Display(Name ="CreatedDateTime")]
        public System.DateTime LuCreatedDateTime { get; set; }

        [Required(ErrorMessage = "UpdatedDateTime is required")]
        [Display(Name = "UpdatedDateTime")]
        public System.DateTime LuUpdatedDateTime { get; set; }

        [Required(ErrorMessage = "Block Notification is required")]
        [Display(Name ="Block Notification")]
        public bool LuBlockNotification { get; set; }

        [Display(Name = "A01")]
        [MaxLength(255, ErrorMessage = "A01 can be maximum 255 characters")]
        [RestrictSpecialChar]
        public string A01 { get; set; }

        [Display(Name = "A02")]
        [MaxLength(255, ErrorMessage = "A02 can be maximum 255 characters")]
        [RestrictSpecialChar]
        public string A02 { get; set; }

        [Display(Name = "A03")]
        [MaxLength(255, ErrorMessage = "A03 can be maximum 255 characters")]
        [RestrictSpecialChar]
        public string A03 { get; set; }

        [Display(Name = "A04")]
        [MaxLength(255, ErrorMessage = "A04 can be maximum 255 characters")]
        [RestrictSpecialChar]
        public string A04 { get; set; }

        [Display(Name = "A05")]
        [MaxLength(255, ErrorMessage = "A05 can be maximum 255 characters")]
        [RestrictSpecialChar]
        public string A05 { get; set; }

        [Display(Name = "AN01")]
        public Nullable<decimal> AN01 { get; set; }

        [Display(Name = "AN02")]
        public Nullable<decimal> AN02 { get; set; }

        [Display(Name = "AN03")]
        public Nullable<decimal> AN03 { get; set; }

        [Display(Name = "AN04")]
        public Nullable<decimal> AN04 { get; set; }

        [Display(Name = "AN05")]
        public Nullable<decimal> AN05 { get; set; }

        [Display(Name = "AD01")]
        public Nullable<System.DateTime> AD01 { get; set; }

        [Display(Name = "AD02")]
        public Nullable<System.DateTime> AD02 { get; set; }

        [Display(Name = "AD03")]
        public Nullable<System.DateTime> AD03 { get; set; }

        [Display(Name = "AD04")]
        public Nullable<System.DateTime> AD04 { get; set; }

        [Display(Name = "AD05")]
        public Nullable<System.DateTime> AD05 { get; set; }

        //[MaxLength(4000, ErrorMessage = "Comments can be maximum 4000 characters")]
        //[Display(Name ="Existing Comments")]
        //[RestrictSpecialChar]
        //public string LuComments { get; set; }

        [Display(Name = "AlteryxUser ")]
        [Required(ErrorMessage = "AlteryxUser is required")]
        public bool LuIsAlteryxUser { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterId { get; set; }

        [RestrictSpecialChar]
        public string WFAnalystId { get; set; }

        [RestrictSpecialChar]
        public string WFManagerId { get; set; }
        public Nullable<int> WFOrdinal { get; set; }

        [RestrictSpecialChar]
        public string WFCurrentOwnerId { get; set; }

        [RestrictSpecialChar]
        public string WFStatus { get; set; }

        [RestrictSpecialChar]
        public string WFType { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterRoleId { get; set; }

        public int WFCompanyId { get; set; }
        [Display(Name = "Existing Comments")]
        [RestrictSpecialChar]
        public string WFComments { get; set; }

        //These objects are defined to display username in grid

        [RestrictSpecialChar]
        public string CreatedBy { get; set; }

        [RestrictSpecialChar]
        public string UpdatedBy { get; set; }

        [RestrictSpecialChar]
        public string ReportsTo { get; set; }

        [RestrictSpecialChar]
        public string FullName { get; set; }

        [RestrictSpecialChar]
        public string RoleList { get; set; }

        [RestrictSpecialChar]
        public string Comments { get; set; }

        [RestrictSpecialChar]
        public List<string > Roles { get; set; }

        [RestrictSpecialChar]
        public string FileNames { get; set; }

        [RestrictSpecialChar]
        public string PortfolioList { get; set; }

        [RestrictSpecialChar]
        public string RoleBasedPortfolios { get; set; }
        public bool SamePortfoliosForAllRoles { get; set; }
        [RestrictSpecialChar]
        public string ParameterCarrier { get; set; }

        public int? PayeeId { get; set; }//to check if user and payee is created from same email

        public string SubmitClicked { get; set; }
        public bool? LuCreateLogin { get; set; }

        [Display(Name = "SuperUser")]
        public bool IsSuperUser { get; set; }
    }

    public class UserAsDropdownViewModel
    {
        public string LuEmail { get; set; }
        public int Id { get; set; }

         
        public string LuUserId { get; set; }
    }
}