using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class MAspnetUsersGScurityQuestionViewModel
    {
        public int Id { get; set; }
        [RestrictSpecialChar]
        [Display(Name = "User Id")]
        [MaxLength(128, ErrorMessage = "The user id can be maximum 128 characters")]      
        public string MAuqsqUserId { get; set; }

        [Display(Name = "Question Id")]
        [Required(ErrorMessage ="Question is Required")]
        public int MAuqsqQuestionId { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "Answers")]
        [MaxLength(200, ErrorMessage = " Answer  can be maximum 200 characters")]
        [Required(ErrorMessage = "Answer  is required")]
        public string MAugsqAnswer { get; set; }
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

        [RestrictSpecialChar]
        [Display(Name = "Questions")]
        [MaxLength(4000, ErrorMessage = "The Questions  can be maximum 4000 characters")]
        [Required]
        public string GsqQuestion { get; set; }

    }
}