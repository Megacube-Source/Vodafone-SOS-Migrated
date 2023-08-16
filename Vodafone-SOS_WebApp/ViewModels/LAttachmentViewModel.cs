using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LAttachmentViewModel
    {
        [RestrictSpecialChar]
        public string LaFilePath { get; set; }
        [RestrictSpecialChar]
        public string LaEntityType { get; set; }
        public int LaEntityId { get; set; }
        public System.DateTime LaCreatedDateTime { get; set; }
        public System.DateTime LaUpdatedDateTime { get; set; }
        [RestrictSpecialChar]
        public string LaCreatedById { get; set; }
        [RestrictSpecialChar]
        public string LaUpdatedById { get; set; }
        [RestrictSpecialChar]
        public string LaType { get; set; }


        public int Id { get; set; }

        [MaxLength(1000, ErrorMessage = " File Name can be maximum 1000 characters")]
        [Required(ErrorMessage = "File Name is required")]
        [Display(Name = "File Name")]

        [RestrictSpecialChar]
        public string LaFileName { get; set; }

        [Required(ErrorMessage = "Upload File Id is required")]
        [Display(Name = "Upload File Id")]
        public int LaUploadFileId { get; set; }

        [RestrictSpecialChar]
        public string LufPayeeId { get; set; }
    }
}