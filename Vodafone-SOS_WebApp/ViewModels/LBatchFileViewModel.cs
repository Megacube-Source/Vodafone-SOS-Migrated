using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;



namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LBatchFileViewModel
    {
        public int Id { get; set; }
       
        [Required(ErrorMessage = "File Batch Id  is required")]
        [Display(Name = "File Batch Id")]
        public int LbfBatchId { get; set; }

        [MaxLength(200, ErrorMessage = "The File Name can be maximum 200 characters")]
        [Required(ErrorMessage = "File Name is required")]
        [Display(Name =" File Name")]
        [RestrictSpecialChar]
        public string LbfFileName { get; set; }

        [Display(Name = " File TimeStamp")]
         public Nullable<System.DateTime> LbfFileTimeStamp { get; set; }
    }
}