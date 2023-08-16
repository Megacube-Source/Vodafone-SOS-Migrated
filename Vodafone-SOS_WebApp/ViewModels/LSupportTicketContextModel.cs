using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LSupportTicketContextModel
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(30, ErrorMessage = "The Ticket Number can be maximum 30 characters")]
        [Required(ErrorMessage = "Ticket Number is required")]
        [Display(Name = "Ticket Number")]
        [RestrictSpecialChar]
        public string LstTicketNumber { get; set; }

        [MaxLength(20, ErrorMessage = "Phone Number can be maximum 20 characters long")]
        
        [Display(Name = "Phone Number")]
        [RestrictSpecialChar]
        public string LstPhone { get; set; }

        [MaxLength(20, ErrorMessage = "Type can be maximum 20 characters long")]
        [Required(ErrorMessage = "Type is required")]
        [Display(Name = "Type")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LstType { get; set; }

        [Display(Name = "Stage")]
        public Nullable<int> LstStageId { get; set; }

        [MaxLength(20, ErrorMessage = "Closure Code can be maximum 20 characters long")]
        [Display(Name = "Closure Code")]
        [RestrictSpecialChar]
        public string LstClosureCode { get; set; }

        [MaxLength(20, ErrorMessage = "Severity can be maximum 20 characters long")]
        [Required(ErrorMessage = "Severity is required")]
        [Display(Name = "Severity")]
        [RestrictSpecialChar]
        public string LstSeverity { get; set; }

        [MaxLength(100, ErrorMessage = "Impact can be maximum 100 characters long")]
        
        [Display(Name = "Impact")]
        [RestrictSpecialChar]
        public string LstImpact { get; set; }

        [MaxLength(10, ErrorMessage = "Priority can be maximum 10 characters long")]
        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LstPriority { get; set; }
      

        [MaxLength(400, ErrorMessage = "Summary can be maximum 400 characters long")]
        [Required(ErrorMessage = "Summary is required")]
        [RestrictSpecialChar] 
        [Display(Name = "Summary")]
        
        public string LstSummary { get; set; }

        [Required(ErrorMessage = "Created Date Time is required")]
        [Display(Name = "Created Date Time")]
        public System.DateTime LstCreatedDateTime { get; set; }
        [MaxLength(500, ErrorMessage = "CC can be maximum 500 characters long")]
        [Display(Name = "Copy Email To")]
        [RestrictSpecialChar]
        public string LstCC { get; set; }

        [MaxLength(20, ErrorMessage = "Status can be maximum 20 characters long")]
        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        [RestrictSpecialChar]
        public string LstStatus { get; set; }

        [Display(Name = "Last Updated Date Time")]
        public DateTime LstLastUpdatedDateTime { get; set; }

        [Display(Name = "Team Id")]
        public Nullable<int> LstTeamId { get; set; }

        [MaxLength(128, ErrorMessage = "The Created By Id can be maximum 128 characters")]
        [Required(ErrorMessage = "Created By Id is required")]
        [Display(Name = "Created By ")]
        [RestrictSpecialChar]
        public string LstCreatedById { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int LstCompanyId { get; set; }

        [MaxLength(128, ErrorMessage = "Created on Behalf can be maximum 128 characters long")]
        [Display(Name = "Created On Behalf Of")]
        [RestrictSpecialChar]
        public string LstCreatedOnBehalfOfId { get; set; }

        [MaxLength(128)]
        [Display(Name = "Last Updated By")]
        [RestrictSpecialChar]
        public string LstLastUpdatedById { get; set; }

        [RestrictSpecialChar]
        public string LstL1Id { get; set; }

        [RestrictSpecialChar]
        public string LstL2Id { get; set; }

        [RestrictSpecialChar]
        public string LstL3Id { get; set; }

        [RestrictSpecialChar]
        public string LstCurrentOwnerId { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage ="Category is required")]
        public int LstCategoryId { get; set; }

        [RestrictSpecialChar]
        public string LstLastUpdatedUserName { get; set; }

        [RestrictSpecialChar]
        public string LstCreatedByUserName { get; set; }
        //For Other Linked tables





        [MaxLength(50, ErrorMessage = "Requestor Name can be maximum 50 characters long")]
        [Required(ErrorMessage = "Requestor is required")]
        [Display(Name = "Requestor")]
        [RestrictSpecialChar]
        public string LstRequestor { get; set; }

        [MaxLength(100, ErrorMessage = "Email ID can be maximum 100 characters long")]
        [Required(ErrorMessage = "Email ID is required")]
        [Display(Name = "Email ID")]
        [RestrictSpecialChar]
        public string LstEmail { get; set; }

        
        
        //[MaxLength(1000, ErrorMessage = "Description can be maximum 1000 characters long")]
        //[Required(ErrorMessage = "Description is required")]
        //[Display(Name = "Description")]
        //[RestrictSpecialChar]
        //public string LstDescription { get; set; }
        

        //[Display(Name = "Description")]
        [Display(Name = "Existing Comments")]
        [Required(ErrorMessage = "Comments is required")]
        [RestrictSpecialChar]
      
        public string LstExDescription { get; set; }
        //Not used in data saving, created to refer only
        public int LstQuickTicketID { get; set; }
        [Display(Name = "Quick Ticket")]

        [RestrictSpecialChar]
        public string LstQuickTicketName { get; set; }




        //public int Id { get; set; }
        public DateTime LsrResponseDateTime { get; set; }
       

        [MaxLength(1000, ErrorMessage = "Description can be maximum 1000 characters long")]
        [Required(ErrorMessage = "Description is required")]
        [RestrictSpecialChar]
        [Display(Name = "Description")]      
        public string LsrDescription { get; set; }
        

        [RestrictSpecialChar]
        public string LsrUploadedFileNames { get; set; }

        [RestrictSpecialChar]
        public string LsrTicketStatus { get; set; }
        public int LsrSupportTicketId { get; set; }

        [RestrictSpecialChar]
        public string LsrResponseById { get; set; }

        [RestrictSpecialChar]
        public string LsrResponseByName { get; set; }

        [RestrictSpecialChar]
        public string LuFirstName { get; set; }

        [RestrictSpecialChar]
        public string LuLastName { get; set; }

        public int Ordinal { get; set; }
        //TicketAssignment
        public DateTime LstaCreatedDateTime { get; set; }
        public int LstaSupportTicketId { get; set; }

        [RestrictSpecialChar]
        public string LstaAssignedToId { get; set; }

        [RestrictSpecialChar]
        public string LstaAssignedById { get; set; }
        public int LstaSupportTeamId { get; set; }

        //Other Values

        [RestrictSpecialChar]
        public string OpCoName { get; set; }

        public int Counts { get; set; }

        [RestrictSpecialChar]
        public string GcCode { get; set; }

        public int Requester { get; set; }
        public int L1 { get; set; }
        public int L2 { get; set; }
        public int Closed { get; set; }

    }

    public class ClosedTicketsModel
    {
        public string Month { get; set; }
        public string OpCo { get; set; }
        public string Priority { get; set; }
        public int Count { get; set; }
    }
}