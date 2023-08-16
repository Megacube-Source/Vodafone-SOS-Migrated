using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class MAspnetUsersGScurityQuestionViewModel
    {
        public int Id { get; set; }
        public string MAuqsqUserId { get; set; }
        public int MAuqsqQuestionId { get; set; }
        public string MAugsqAnswer { get; set; }
        public int Question1 { get; set; }
        public int Question2 { get; set; }
        public int Question3 { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }

    }
}