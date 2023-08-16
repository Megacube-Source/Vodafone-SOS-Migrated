using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class RWorkFlowViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Name")]
        [RestrictSpecialChar]
        public string RwfName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "UI Label")]
        [RestrictSpecialChar]
        public string RwfUILabel { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "BaseTable")]
        [RestrictSpecialChar]
        public string RwfBaseTableName { get; set; }

        [Required]
        [Display(Name = "CR Allowed")]
        public bool RwfCRAllowed { get; set; }

        [Display(Name = "CR WF Name")]
        [RestrictSpecialChar]
        public string RwfCRWFName { get; set; }

        [RestrictSpecialChar]
        [Display(Name = "WF Type")]
        
        public string RwfWFType { get; set; }


        [RestrictSpecialChar]
        public string GcCode { get; set; }
        public int LBatches { get; set; }
        public int LPayees { get; set; }
        public int LClaims { get; set; }
        public int LUsers { get; set; }
        public int LChangeRequests { get; set; }
        public int LRefFiles { get; set; }
        public int LSchemes { get; set; }
        public int LAccruals { get; set; }
        public int LDocumentSets { get; set; }
    }
}