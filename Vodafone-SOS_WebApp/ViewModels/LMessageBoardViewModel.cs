using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LMessageBoardViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Message text is required.")]
        [Display(Name = "Message Text")]
        [RestrictSpecialChar]
        public string Message { get; set; }

        [Display(Name = "Important?")]
        public bool IsImportant { get; set; }

        [RestrictSpecialChar]
        public string CreatedById { get; set; }

        [RestrictSpecialChar]
        public string CreatedByEmailId { get; set; }

        [RestrictSpecialChar]
        public string UpdatedById { get; set; }
        [RestrictSpecialChar]
        public string UpdatedByEmailId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public Nullable<DateTime> UpdatedDateTime { get; set; }

        [RestrictSpecialChar]
        public string SenderRoleID { get; set; }

        [RestrictSpecialChar]
        public string SenderRoleName { get; set; }

        public int CompanyID { get; set; }

        public int MessageID { get; set; }

        [RestrictSpecialChar]
        public string RecipientID { get; set; }

        [RestrictSpecialChar]
        public string RecipientEmailID { get; set; }
        [Display(Name = "Recipient Role")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string RecipientRoleID { get; set; }

        [RestrictSpecialChar]
        public string RecipientRoleName { get; set; }
        public Nullable<DateTime> ReadRecieptDateTime { get; set; }

        [RestrictSpecialChar]
        public string SelectedPortfolios { get; set; }

        [RestrictSpecialChar]
        public List<string> UsersToSendMessage { get; set; }
    }
}