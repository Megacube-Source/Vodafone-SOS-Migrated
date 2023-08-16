using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class LMessageBoardViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsImportant { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByEmailId { get; set; }
        public string UpdatedById { get; set; }
        public string UpdatedByEmailId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public Nullable<DateTime> UpdatedDateTime { get; set; }
        public string SenderRoleID { get; set; }
        public string SenderRoleName { get; set; }
        public int CompanyID { get; set; }

        public int MessageID { get; set; }
        public string RecipientID { get; set; }
        public string RecipientEmailID { get; set; }
        public string RecipientRoleID { get; set; }
        public string RecipientRoleName { get; set; }
        public Nullable<DateTime> ReadRecieptDateTime { get; set; }
        public string SelectedPortfolios { get; set; }
        public List<string> UsersToSendMessage { get; set; }
    }
}