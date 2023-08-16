using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;
//using System.Web.Mvc;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class SupportSystemViewModel
    {
    }
    public partial class SupportSystemCategoriesViewModel
    {
        
        
        public int Id { get; set; }
        [MaxLength(20, ErrorMessage = "Category Name can be maximum 20 characters long")]
        [Required(ErrorMessage = "Category Name is required")]
        [Display(Name = "Category Name")]
        [RestrictSpecialChar]
        public string RscName { get; set; }
        [MaxLength(100, ErrorMessage = "Category Description can be maximum 100 characters long")]
        [Required(ErrorMessage = "Category Description is required")]
        [Display(Name = "Category Description")]
        [RestrictSpecialChar]
        public string RscDescription { get; set; }
        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        public int RscCompanyId { get; set; }

        [MaxLength(400, ErrorMessage = "Ticket Description can be maximum 400 characters long")]
        [Required(ErrorMessage = "Ticket Description is required")]
        [Display(Name = "Ticket Description")]
        [RestrictSpecialChar]
        public string RscTicketDescription { get; set; }

        //object of parent table is defined to get desired objects instead of Table data of entire table
        [Display(Name = "Company Name")]
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

    }

    public partial class SupportSystemStagesViewModel
    {
        public int Id { get; set; }
        [MaxLength(20, ErrorMessage = "Ticket Stages Name can be maximum 20 characters long")]
        [Required(ErrorMessage = "Ticket Stages Name is required")]
        [Display(Name = "Ticket Stages Name")]
        [RestrictSpecialChar]
        public string RtsName { get; set; }
        [MaxLength(100, ErrorMessage = "Ticket Stages Description can be maximum 100 characters long")]
        [Required(ErrorMessage = "Ticket Stages Description is required")]
        [Display(Name = "Ticket Stages Description")]
        [RestrictSpecialChar]
        public string RtsDescription { get; set; }

        [Display(Name = "Is Active")]
        public bool RtsIsActive { get; set; }

    }

    public partial class SupportSystemTeamsViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string RstTeamName { get; set; }
        public int RstRoleId { get; set; }
    }
    public partial class SupportSystemQuickTicketsViewModel
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "UI Label can be maximum 50 characters long")]
        [Required(ErrorMessage = "UI Label is required")]
        [Display(Name = "UI Label")]
        [RestrictSpecialChar]
        public string RsqtUILabel { get; set; }

        [MaxLength(400, ErrorMessage = "Summary can be maximum 400 characters long")]
        [Required(ErrorMessage = "Summary is required")]
        [Display(Name = "Summary")]
        [RestrictSpecialChar]
        public string RsqtSummary { get; set; }//in table, the column name is RsqtSmmary, neeed to fix this

        [MaxLength(400, ErrorMessage = "Comments can be maximum 400 characters long")]
        [Required(ErrorMessage = "Comments is required")]
        [Display(Name = "Comments")]
        [RestrictSpecialChar]
        public string RsqtComments { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int RsqtCategoryId { get; set; }

        [Display(Name = "Category Name")]
        [RestrictSpecialChar]
        public string RscName { get; set; }
        
        [Required(ErrorMessage = "Ticket Description is required")]
        [Display(Name = "Ticket Description")]
        [RestrictSpecialChar]
        public string RsqtTicketDescription { get; set; }
        
    }
}