using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Utilities;

namespace Vodafone_SOS_WebApp.ViewModels
{
    public class AttachedFilesViewModel
    {
        //Did not applied Special Character as I need special character here
        [RestrictSpecialChar]
        public string FileName { get; set; }

        [RestrictSpecialChar]
        public string FilePath { get; set; }
    }
}