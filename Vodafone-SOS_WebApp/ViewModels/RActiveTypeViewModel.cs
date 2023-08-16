//Code Review Comment using System;
//Code Review Comment using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//Code Review Comment using System.Linq;
//Code Review Comment using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class RActiveTypeViewModel
    {
        public int Id { get; set; }
        //Code Review Comment
        //Code Review Comment [Required(ErrorMessage = "Company Id is required")]
        //Code Review Comment Display(Name = "Company Id")]
        public int RatCompanyId { get; set; }

        [MaxLength(50, ErrorMessage = "The Activity Type Name can be maximum 50 characters")]
        [Required(ErrorMessage = "Activity Type Name is required")]
        [Display(Name = "Activity Type Name")]
        [RestrictSpecialChar]
        public string RatName { get; set; }

        [MaxLength(2000, ErrorMessage = "The Activity Type Description can be maximum 2000 characters")]
        [Display(Name = "Activity Type Description")]
        [RestrictSpecialChar]
        public string RatDescription { get; set; }

        //object of parent table defined to get only desired object values and not table as whole
        //Code Review Comment

        //Code Review Comment [Display(Name = "Company Name")]
        //Code Review Comment [RestrictSpecialChar]

        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        //Code Review Comment RK 27052017 Commented Below Line during code review 
        //Code Review Comment [Required(ErrorMessage = "Is Active is required")] 
        [Display(Name = "Is Active")]
        public bool RatIsActive{ get; set; }
    }
}