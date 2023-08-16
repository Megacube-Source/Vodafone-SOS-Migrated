using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class GErrorLogViewModel
    {
        public GErrorLogViewModel()
        {
            GelStatus = "New";
        }
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Error DateTime is required")]
        [Display(Name = " Date Time")]
        public System.DateTime GelErrorDateTime { get; set; }

        [MaxLength(50, ErrorMessage = " Source Project can be maximum 50 characters")]
        [Required(ErrorMessage = "Source Project  is required")]
        [Display(Name = "Source Project")]
        [RestrictSpecialChar]
        public string GelSourceProject { get; set; }

        [MaxLength(200, ErrorMessage = "Controller can be maximum 200 characters")]
        [Display(Name = "Controller")]
        [RestrictSpecialChar]
        public string GelController { get; set; }

        [MaxLength(200, ErrorMessage = " Method can be maximum 200 characters")]
        [Display(Name = "Method")]
        [RestrictSpecialChar]
        public string GelMethod { get; set; }
       
        [MaxLength(4000, ErrorMessage = " Controller can be maximum 4000 characters")]
        [Display(Name = "Stact Trace")]
        [RestrictSpecialChar]
        public string GelStackTrace { get; set; }
        [MaxLength(256, ErrorMessage = " User Name  can be maximum 256 characters")]
        [Display(Name = "User Name")]
        [RestrictSpecialChar]
        public string GelUserName { get; set; }

        [MaxLength(10, ErrorMessage = "Error Type can be maximum 10 characters")]
        [Display(Name = "Error Type")]
        [RestrictSpecialChar]
        public string GelErrorType { get; set; }

        [MaxLength(2000, ErrorMessage = " Error Description can be maximum 2000 characters")]
        [Display(Name = "Error Description")]
        [RestrictSpecialChar]
        public string GelErrorDescription { get; set; }

        [MaxLength(2000, ErrorMessage = " Resolution can be maximum 2000 characters")]
        [Display(Name = "Resolution")]
        [RestrictSpecialChar]
        public string GelResolution { get; set; }

        [MaxLength(20, ErrorMessage = " Error Owner can be maximum 20 characters")]
        [Display(Name = "Error Owner")]
        [RestrictSpecialChar]
        public string GelErrorOwner { get; set; }

        [MaxLength(100, ErrorMessage = " Field Name can be maximum 100 characters")]
        [Display(Name = "Field Name")]
        [RestrictSpecialChar]
        public string GelFieldName { get; set; }

        [Display(Name = "SOS Batch  Number")]
        public Nullable<int> GelSOSBatchNumber { get; set; }

        [RestrictSpecialChar]
        public string GelStatus { get; set; }

        public int counts { get; set; }

        public int Requester { get; set; }
        public int L1 { get; set; }
        public int L2 { get; set; }
        public int Closed { get; set; }

        /// <summary>
        /// Object is defined to display UserName and not UserId of User 
        /// </summary>
        // public string User { get; set; }
    }

    
}