using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LRefFileViewModel
    {
        public int Id { get; set; }
        [RestrictSpecialChar]
        public string LrfCreatedById { get; set; }

        [RestrictSpecialChar]
        public string LrfUPdatedById { get; set; }

        [RestrictSpecialChar]
        public string ParameterCarrier { get; set; }

        [Required]
        [Display(Name ="Ref File Type")]
        public Nullable<int> LrfRefFileTypeId { get; set; }
        public int LrfCompanyId { get; set; }

        [Display(Name = "Description")]
        [RestrictSpecialChar]
        public string LrfDescription { get; set; }

        [Display(Name = "Year")]
        public Nullable<int> LrfYear { get; set; }

        [Display(Name ="Month")]
        //  [RestrictSpecialChar] as not required for dropdown fields
        public string LrfMonth { get; set; }
      //  [RestrictSpecialChar]
       // public string LrfComments { get; set; }
        public System.DateTime LrfCreatedDateTime { get; set; }
        public System.DateTime LrfUpdatedDateTime { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterId { get; set; }

        [RestrictSpecialChar]
        public string WFAnalystId { get; set; }

        [RestrictSpecialChar]
        public string WFManagerId { get; set; }

        [RestrictSpecialChar]
        public string WFCurrentOwnerId { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterRoleId { get; set; }
        public Nullable<int> WFOrdinal { get; set; }

        [RestrictSpecialChar]
        public string WFStatus { get; set; }

        [RestrictSpecialChar]
        public string WFType { get; set; }
        public int WFCompanyId { get; set; }

         [Display(Name = "Comments")]
        [RestrictSpecialChar]
        [MaxLength(4000,ErrorMessage ="Maxumum 4000 charcters are allowed")]
        public string WFComments { get; set; }

        public string AttachedFilesName { get; set; }
        [Display(Name = "Ref File Name")]
        public string LrfRefFileName { get; set; }

    }

    
}