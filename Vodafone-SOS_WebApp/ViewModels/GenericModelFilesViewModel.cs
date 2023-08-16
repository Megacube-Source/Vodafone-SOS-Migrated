using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class GenericModelFilesViewModel
    {
        [RestrictSpecialChar]
        public string DisplayFileName { get; set; }
        [RestrictSpecialChar]
        public string FileName { get; set; }
        [RestrictSpecialChar]
        public string FilePath { get; set; }
    }

    

}