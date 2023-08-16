using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class VitalStatsViewModel
    {
        [Display]
        [RestrictSpecialChar]
        public string WorkflowName { get; set; }

        [Display]
        [Required]
        public int GRPCount { get; set; }

        [Display]
        [Required]
        public int ALCount { get; set; }

        [Display]
        [Required]
        public int CZCount { get; set; }

        [Display]
        public int GBCount { get; set; }

        [Display]
        public int GHCount { get; set; }

        [Display]
        [Required]
        public int GRCount { get; set; }

        [Display]
        [Required]
        public int HUCount { get; set; }

        [Display]
        [Required]
        public int MTCount { get; set; }

        [Display]
        [Required]
        public int QACount { get; set; }

        [Display]
        [Required]
        public int ROCount { get; set; }

        [Display]
        [Required]
        public int INCount { get; set; }
    }

    }