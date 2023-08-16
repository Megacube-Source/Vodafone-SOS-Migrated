using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public partial class LBatchViewModel
    {
        public int Id { get; set; }

        [RestrictSpecialChar]
        public string LbBatchName { get; set; }

        [Required(ErrorMessage = "Batch Number is required")]
        [Display(Name = "Batch Number")]
        public int LbBatchNumber { get; set; }

        [Required(ErrorMessage = "Upload Start Date Time is required")]
        [Display(Name = "Upload Start Date Time")]
        public DateTime LbUploadStartDateTime { get; set; }

        [Display(Name = "Upload Finish Date Time")]
        public Nullable<DateTime> LbUploadFinishDateTime { get; set; }

        [MaxLength(20, ErrorMessage = "The Batch Type Name can be maximum 20 characters")]
        [Required(ErrorMessage = "Batch Type Name is required")]
        [Display(Name = "Batch Type Name")]
        [RestrictSpecialChar]
        public string LbBatchType { get; set; }

        [Display(Name = "Record Count")]
        public int LbRecordCount { get; set; }

        [Display(Name = "Alteryx Batch Number")]
        public int LbAlteryxBatchNumber { get; set; }

        [Display(Name = "Comments")]
        [RestrictSpecialChar]
        public string LbComments { get; set; }

        [MaxLength(200, ErrorMessage = "The File Name can be maximum 200 characters")]
        [Required(ErrorMessage = "File Name is required")]
        [Display(Name = "File Name")]
        [RestrictSpecialChar]
        public string LbFileName { get; set; }

        [Display(Name = "Upload Finish Date Time")]
           //[DataType(DataType.Date)]
        public Nullable<DateTime> LbFileTimeStamp { get; set; }

        [Required(ErrorMessage = "Company Id is required")]
        [Display(Name = "Company Id")]
        public int LbCompanyId { get; set; }

         [Display(Name = "Parent Batch Id")]
        public Nullable<int> LbParentBatchId { get; set; }

        [MaxLength(50, ErrorMessage = "The Status can be maximum 50 characters")]
        [Required(ErrorMessage = "Status is required")]
         [Display(Name = "Status")]
        [RestrictSpecialChar]
        public string LbStatus { get; set; }

        //objects of parent tables defined to get only desired objects and not table as whole
         [Display(Name = "Company Name")]
        [RestrictSpecialChar]
        public string GcCompanyName { get; set; }

        //[Display(Name = "Parent Batch")]

         [Display(Name = "Status")]
        [RestrictSpecialChar]
        public string RsStatus { get; set; }

         [Display(Name = "Table Name")]
        [RestrictSpecialChar]
        public string LrdtName { get; set; }

        //userId foreign key
        [MaxLength(128, ErrorMessage = "The UpdatedBy can be maximum 128 characters")]
        [Required(ErrorMessage = "UpdatedBy is required")]
        [Display(Name = "UpdatedBy")]
        [RestrictSpecialChar]
        public string LbUpdatedBy { get; set; }

         
         [Display(Name = "RawData Table")]
         public Nullable<int> LbRawDataTableId { get; set; }

         [Display(Name = "Commission Period")]
        [RestrictSpecialChar]
        public string LbCommissionPeriod { get; set; }

        [Display(Name = "Allocated To Id")]
        [RestrictSpecialChar]
        public string LbAllocatedToId { get; set; }

        [Display(Name = "LbPrimaryChannel")]
        [RestrictSpecialChar]
        public string LbPrimaryChannel{ get; set; }

        [Display(Name = "LbBusinessUnit")]
        [RestrictSpecialChar]
        public string LbBusinessUnit { get; set; }

        [Display(Name = "LbSubChannel")]
        [RestrictSpecialChar]
        public string LbSubChannel { get; set; }

        [RestrictSpecialChar]
        public string WFRequesterRoleId { get; set; }

        [RestrictSpecialChar]
        public string WFComments { get; set; }
    }

    public partial class LBatchViewModelForAnalystGrid
    {
        public int Id { get; set; }

        public Nullable<int> LbRawDataTableId { get; set; }

        [Display(Name = "Commission Period")]
        [RestrictSpecialChar]
        public string LbCommissionPeriod { get; set; }

        //TableName from LRawDataTables
        [Display(Name = "Raw Data Type")]
        [RestrictSpecialChar]
        public string LrdtName { get; set; }

        [Display(Name = "Batch Number")]
        public int LbBatchNumber { get; set; }

        [Display(Name = "Record Count")]
        [Required(ErrorMessage = "Record Count is required")]
        public int LbRecordCount { get; set; }

        [Display(Name = "Upload Start Date Time")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LbUploadStartDateTime { get; set; }

    }


    public partial class LBatchViewModelForPayeeGrid
    {
        public int Id { get; set; }
        public string LbStatus { get; set; }
        public int LbBatchNumber { get; set; }
        public string LbfFileName { get; set; }
        public int? LbRecordCount { get; set; }
        public DateTime LbUploadStartDateTime { get; set; }
        public int? IsImport { get; set; }

    }
}