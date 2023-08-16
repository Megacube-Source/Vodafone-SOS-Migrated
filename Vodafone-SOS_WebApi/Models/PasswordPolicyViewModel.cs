using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApi.Models
{
    public class PasswordPolicyViewModel
    {
        public int MinLength { get; set; }
        public int MinUppercase { get; set; }
        public int MinLowercase { get; set; }
        public int MinNumbers { get; set; }
        public int MinSpecialChars { get; set; }
        public int MaxAgeDays { get; set; }
        public Nullable<int> ReminderDays { get; set; }
        public Nullable<int> MinAgeDays { get; set; }
       
        //Added to get how many days left to expire password
        public int DaysToExpirePassword { get; set; }
    }
}