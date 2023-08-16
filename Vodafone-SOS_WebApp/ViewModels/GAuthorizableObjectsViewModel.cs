using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public class GAuthorizableObjectsViewModel
    {
        [MaxLength(300, ErrorMessage = "The Controller Name can be maximum 300 characters")]
        [Required(ErrorMessage = "Controller  Name is required")]
        [Display(Name = "Controller  Name")]
        [RestrictSpecialChar]
        public string GaoControllerName { get; set; }

        [MaxLength(300, ErrorMessage = "The Controller Method Name can be maximum 300 characters")]
        [Required(ErrorMessage = "Controller Method Name is required")]
        [Display(Name = "Controller Method   Name")]
        [RestrictSpecialChar]

        public string GaoControllerMethodName { get; set; }

        public   int Id { get; set; }

        [MaxLength(500, ErrorMessage = "The Authorizable Object Description can be maximum 500 characters")]
        [Display(Name = "Authorizable Object Discription ")]
        [RestrictSpecialChar]
        public   string GaoDescription { get; set; }

        public bool flag { get; set; }

        [RestrictSpecialChar]
        public string RoleId { get; set; }
        public int ObjectId { get; set; }
    }
}