using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class LCompanySpecificForms
    {

        [Required]
        public int Id { get; set; }

        [Required]
        public string FormName { get; set; }

        public string BannerText { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }
}