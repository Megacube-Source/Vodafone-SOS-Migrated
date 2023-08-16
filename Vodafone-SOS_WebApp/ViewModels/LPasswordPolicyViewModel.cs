using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LPasswordPolicyViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int MinLength { get; set; }
        public int MinUppercase { get; set; }
        public int MinLowercase { get; set; }
        public int MinNumbers { get; set; }
        public int MinSpecialChars { get; set; }
        public int MaxAgeDays { get; set; }
        public int ReminderDays { get; set; }
        public int MinAgeDays { get; set; }
        public int PreventReuse { get; set; }
        public int LockoutFailedAttempts { get; set; }
        public int LockoutMins { get; set; }
        //Added to get how many days left to expire password
        public int DaysToExpirePassword { get; set; }
    }
}