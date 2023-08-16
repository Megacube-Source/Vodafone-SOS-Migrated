using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LCommissionBatchAllacationRulesViewModel
    {


        [RestrictSpecialChar]
        public string UserName { get; set; }
        //string to store grid data 
        [RestrictSpecialChar]
        public string GridData { get; set; }

        public int Id { get; set; }

        [MaxLength(20, ErrorMessage = "Primary Channel  can be maximum 20 characters")]
        [Display(Name = "Primary Channel")]
        [RestrictSpecialChar]
        public string LrdbarPrimaryChannel { get; set; }

        [MaxLength(20, ErrorMessage = "Business Unit  can be maximum 20 characters")]
        [Display(Name = "Business Unit")]
        [RestrictSpecialChar]
        public string LrdbarBusinessUnit { get; set; }

        [MaxLength(100, ErrorMessage = "Channel  can be maximum 100 characters")]
        [Display(Name = "Channel")]
        [RestrictSpecialChar]
        public string LrdbarChannel { get; set; }

        [Required]
        [Display(Name = "Is Default/Not")]
        public bool LrdbarIsDefault { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Comapany Id")]
        public int LrdbarCompanyId { get; set; }

        [MaxLength(128, ErrorMessage = "Allocated To Id  can be maximum 128 characters")]
        [Required(ErrorMessage = "Allocated To Id is required")]
        [Display(Name = "Allocated To")]
        [RestrictSpecialChar]
        public string LrdbarAllocateToId { get; set; }

       



    }
}