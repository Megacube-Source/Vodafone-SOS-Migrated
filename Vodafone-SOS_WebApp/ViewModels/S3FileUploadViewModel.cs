using System;
using System.ComponentModel.DataAnnotations;
using Vodafone_SOS_WebApp.Utilities;


namespace Vodafone_SOS_WebApp.ViewModels
{
    public class S3FileUploadViewModel
    {
        [RestrictSpecialChar]
        public string FormAction { get; set; }

        [RestrictSpecialChar]
        public string FormMethod { get; set; }

        [RestrictSpecialChar]
        public string FormEnclosureType { get; set; }

        [RestrictSpecialChar]
        public string Bucket { get; set; }

        [RestrictSpecialChar]
        public string FileId { get; set; }

        [RestrictSpecialChar]
        public string AWSAccessKey { get; set; }

        [RestrictSpecialChar]
        public string RedirectUrl { get; set; }

        [RestrictSpecialChar]
        public string Acl { get; set; }

        [RestrictSpecialChar]
        public string Base64Policy { get; set; }

        [RestrictSpecialChar]
        public string Signature { get; set; }
    }
}