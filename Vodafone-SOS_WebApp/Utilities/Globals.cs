using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Vodafone_SOS_WebApp.Helper;

using Amazon;
using Amazon.S3;
//using Amazon.S3.Model;
//using Amazon.S3.Transfer;
//using Amazon.S3.IO;
using System.Text;
using System.Security.Cryptography;
using Vodafone_SOS_WebApp.ViewModels;

using Microsoft.Win32;
using Ionic.Zip;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web.SessionState;
using RestSharp;
using Newtonsoft.Json;
using Amazon.S3.Model;
using System.IO.Packaging;
using Amazon.S3.IO;
using System.Reflection;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Data.SqlClient;
using ClosedXML.Excel;

namespace Vodafone_SOS_WebApp.Utilities
{
    public struct SessionData
    {
        public SessionData(string userID, string UserSessionID)
        {
            _UserID = userID;
            _UserSessionID = UserSessionID;
        }

        public string _UserID { get; private set; }
        public string _UserSessionID { get; private set; }
        //List<SessionData> SList = new List<SessionData>();
    }

    ////SS has added a extension method below to allow One Object of Payee to Copy its data to another Object
    //public static class CopyPayeeObject
    //{
    //    public static void CopyProperties(this LPayeeViewModel source, LPayeeViewModel destination)
    //    {
    //        // Iterate the Properties of the destination instance and  
    //        // populate them from their source counterparts  
    //        PropertyInfo[] destinationProperties = destination.GetType().GetProperties();
    //        foreach (PropertyInfo destinationPi in destinationProperties)
    //        {
    //            PropertyInfo sourcePi = source.GetType().GetProperty(destinationPi.Name);
    //            destinationPi.SetValue(destination, sourcePi.GetValue(source, null), null);
    //        }
    //    }
    //}
    ////Section Ends


    public static class Globals
    {

        public const string RegExCharacterOnly = @"^[a-zA-Z\s][a-zA-Z\s]+$";
        public const string RegExCharacterWithSpace = @"^[a-zA-Z\s][a-zA-Z\s]*$";
        public const string RegExAlphanumeric = @"^[a-zA-Z0-9\s][a-zA-Z0-9\s]+$";
        public const string RegExAlphanumericWithSpace = @"^[a-zA-Z0-9\s][a-zA-Z0-9\s]+$";
        public static List<SessionData> LstSessionIDs = new List<SessionData>();
        public static string ErrorPageUrl = "/Home/ErrorPage";

        public static string SOSAWSAccessKey = GetValue("sos_accesskey");
        public static string SOSAWSSecretKey = GetValue("sos_secretkey");
        public static string A2SS3AccessKey = GetValue("int_accesskey");
        public static string A2SS3SecretKey = GetValue("int_secretkey");

        public static string GetValue(string Key)
        {
            IGKeyValuesRestClient KVRC = new GKeyValuesRestClient();
            var Policy = KVRC.GetByName(Key, 1);
            if (Policy != null)
               return WebUtility.HtmlDecode(Policy.GkvValue);
            else
                return "";
        }

        //new HTTP Status codes in accordance with their types has been defined.
        public enum ExceptionType
        {
            /*An exception/error condition in the application which is unhandled in the system and the system don’t have any predefined mechanism to recover*/
            Type1 = 551,
            /*- A server-side validation failure, such as unique key constraint failure while trying inserting data in a unique column*/
            Type2 = 552,
            /*In some cases, we will need to display a popup message to the user (maybe with some relevant information about the process in which error occurred) and then redirect user to another page*/
            Type3 = 553,
            /*When some validation fails, user can be presented with a popup message describing the failed validation, and then keep user on the same page*/
            Type4 = 554
        };
        public static void CreateFolderInS3(string FolderPath)
        {

            string _awsAccessKey = SOSAWSAccessKey;// ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey; // ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    CannedACL = S3CannedACL.Private,
                    Key = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["SOSBucketFolder"], FolderPath, ""),
                    //ContentBody = UserName
                };
                client.PutObject(request);
            }


        }
        public static void CreateFolderInS3Root(string FolderPath)
        {

            string _awsAccessKey = SOSAWSAccessKey;// ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey;// ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    CannedACL = S3CannedACL.Private,
                    Key = string.Format("{0}/{1}",  FolderPath, ""),
                    //ContentBody = UserName
                };
                client.PutObject(request);
            }


        }
        public static bool FileExistsInS3(string FileName, string UserName, string CompanyCode)
        {
            string _awsAccessKey = SOSAWSAccessKey; // ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey; // ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                S3FileInfo s3FileInfo = new Amazon.S3.IO.S3FileInfo(client, _bucketName, string.Format("{0}/{1}/{2}/", CompanyCode, UserName, FileName));
                return s3FileInfo.Exists;

            }

        }
        public static bool FolderExistsInS3(string FolderPath)
        {
            string _awsAccessKey = SOSAWSAccessKey;// ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey; // ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                S3FileInfo s3FileInfo = new Amazon.S3.IO.S3FileInfo(client, _bucketName, string.Format("{0}/{1}/", FolderPath, ""));
                return s3FileInfo.Exists;

            }


        }
        public static void CreateFolderInA2S(string FolderPath, string FolderName)
        {

            string _awsAccessKey = A2SS3AccessKey; // ConfigurationManager.AppSettings["A2SS3AccessKey"];
            string _awsSecretKey = A2SS3SecretKey; // ConfigurationManager.AppSettings["A2SS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["A2SS3Bucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    CannedACL = S3CannedACL.Private,
                    Key = string.Format("{0}/{1}", FolderPath, ""),
                    ContentBody = FolderName
                };
                client.PutObject(request);
            }


        }
        public static bool FolderExistsInA2S(string FolderPath)
        {
            string _awsAccessKey = A2SS3AccessKey; // ConfigurationManager.AppSettings["A2SS3AccessKey"];
            string _awsSecretKey = A2SS3SecretKey; // ConfigurationManager.AppSettings["A2SS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["A2SS3Bucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                S3FileInfo s3FileInfo = new Amazon.S3.IO.S3FileInfo(client, _bucketName, string.Format("{0}/{1}", FolderPath, ""));
                return s3FileInfo.Exists;

            }


        }

        //Method to Upload File To A2S bucket
        public static bool UploadToA2S(Stream stream, string FileName, string FilePath)
        {
            string _awsAccessKey = A2SS3AccessKey; // ConfigurationManager.AppSettings["A2SS3AccessKey"];
            string _awsSecretKey = A2SS3SecretKey; // ConfigurationManager.AppSettings["A2SS3SecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["A2SS3Bucketname"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    CannedACL = S3CannedACL.Private,//PERMISSION TO FILE PUBLIC ACCESIBLE
                    Key = string.Format("{0}/{1}", FilePath, FileName),
                    InputStream = stream//SEND THE FILE STREAM
                };
                client.PutObject(request);
            }
            return true;
        }
        //
        public static bool UploadToS3(Stream stream, string FileName, string FilePath)
        {
            string _awsAccessKey = SOSAWSAccessKey; // ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey;// ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];
            //try
            //{
            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    CannedACL = S3CannedACL.Private,//PERMISSION TO FILE PUBLIC ACCESIBLE
                    Key = string.Format("{0}/{1}", FilePath, FileName),
                    InputStream = stream//SEND THE FILE STREAM
                };

                client.PutObject(request);
            }
            return true;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
        }

        //Upload Supporting Doc
        public static AttachedFilesViewModel AttachSupportingDocs(HttpPostedFileBase[] File1,string EntityName)
        {
            var UserName = System.Web.HttpContext.Current.Session["UserName"];
            string ClaimFileName = "";
            string PayeeUserFriendlyFileName = "";
            var CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"].ToString();
            //
            string filePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["AttachedClaimDocumentPath"], System.Web.HttpContext.Current.Session["CompanyCode"] + "/"+EntityName+"/SupportingDocuments");

            foreach (HttpPostedFileBase file in File1)
            {
                if (file.ContentLength > 0)
                {

                    var fileLocation = "";
                    string fileExtension = System.IO.Path.GetExtension(file.FileName);
                    string name = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                    string FileNames = name + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmm") + fileExtension;


                    fileLocation = filePath + "/" + FileNames;
                    //check if directory exists or not. iIf notcreate that directory
                    bool exists = System.IO.Directory.Exists(filePath);
                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                    }
                    file.SaveAs(fileLocation);

                    //made a comma seperated list of user friendly file names
                    if (string.IsNullOrEmpty(PayeeUserFriendlyFileName))
                    {
                        PayeeUserFriendlyFileName = name + fileExtension;
                    }
                    else
                    {
                        PayeeUserFriendlyFileName = PayeeUserFriendlyFileName + "," + name + fileExtension;
                    }
                    //made a comma seperated list of  file names
                    if (string.IsNullOrEmpty(ClaimFileName))
                    {
                        ClaimFileName = FileNames;
                    }
                    else
                    {
                        ClaimFileName = ClaimFileName + "," + FileNames;
                    }
                }
            }

            return new AttachedFilesViewModel { FileName = ClaimFileName, FilePath = filePath };
        }
        //The below method will contain the code section in which we will be generating exception from the response recived from api project
        public static void GenerateException(IRestResponse response,string RedirectToUrl)
        {
            if (string.IsNullOrEmpty(RedirectToUrl))
            {
                RedirectToUrl = "/Home/ErrorPage";
            }
            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            ex.Data.Add("ErrorCode", (int)response.StatusCode);
            ex.Data.Add("RedirectToUrl", RedirectToUrl);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            string xx = data.Message;
            ex.Data.Add("ErrorMessage", xx);
            throw ex;
        }
        //To set variables in session  variable
        public static void SetSessionVariable(int CompanyId,List<AspnetRoleViewModel> Roles,string UserId,string UserName,string FirstName,string LastName,string PhoneNumber,int LUserId, string SelectedLandingPageUserRole, string SelectedLandingPageUserRoleID, Boolean SelectedLandingPageUserRoleShowOnDashBoard)
        {
            for (int i = 0; i < LstSessionIDs.Count; i++)
            {
                if(LstSessionIDs[i]._UserID == UserId)
                {
                    LstSessionIDs.RemoveAt(i);
                }
            }
            LstSessionIDs.Add(new SessionData(UserId, System.Web.HttpContext.Current.Session.SessionID));
            System.Web.HttpContext.Current.Session["FirstName"] = (FirstName != null) ? FirstName : "";
            System.Web.HttpContext.Current.Session["LastName"] = (LastName != null) ? LastName : "";
            System.Web.HttpContext.Current.Session["PhoneNumber"] = (PhoneNumber != null) ? PhoneNumber : "";
            System.Web.HttpContext.Current.Session["CompanyId"] =(CompanyId!=0)?CompanyId:0;            
            System.Web.HttpContext.Current.Session["UserRole"] = SelectedLandingPageUserRole;                
            System.Web.HttpContext.Current.Session["UserRoleId"] = SelectedLandingPageUserRoleID;
            System.Web.HttpContext.Current.Session["ShowOnDashBoard"] = SelectedLandingPageUserRoleShowOnDashBoard;            
            System.Web.HttpContext.Current.Session["Roles"] = (Roles != null) ? Roles : null;
            System.Web.HttpContext.Current.Session["UserId"] = (UserId!=null)?UserId:"";
            System.Web.HttpContext.Current.Session["UserName"]=(UserName!=null)?UserName:"";
            System.Web.HttpContext.Current.Session["LUserId"] = Convert.ToInt32(LUserId);
            //to default page number of LRawData Grid to 1
            IGCompaniesRestClient GCRC = new GCompaniesRestClient();
            var Company = GCRC.GetById(CompanyId);
            System.Web.HttpContext.Current.Session["CompanyName"] = (Company != null) ? Company.GcCompanyName :"";
            System.Web.HttpContext.Current.Session["CompanyCode"] = (Company != null) ? Company.GcCode : "";
            System.Web.HttpContext.Current.Session["RawDataPage"] = 1;
            System.Web.HttpContext.Current.Session["ExcludedRawDataPage"] = 1;
            System.Web.HttpContext.Current.Session["ErrorRawDataPage"] = 1;
            
        }
        /*
        this method has been added to convert filestream to byte array because the Amazon file stream connection get closed as we move
       out of DownloadFromS3 method
            */
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        //************************
        public static void CopyFileFromS3toS3(string srcPath, string srcFileName, string destPath, string destFileName)
        {
            try
            {
                string _awsAccessKey = SOSAWSAccessKey;// ConfigurationManager.AppSettings["SOSAWSAccessKey"];
                string _awsSecretKey = SOSAWSSecretKey;// ConfigurationManager.AppSettings["SOSAWSSecretKey"];
                string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];
                string A2SAccessKey = A2SS3AccessKey;// ConfigurationManager.AppSettings["A2SS3AccessKey"];
                string A2SSecretKey = A2SS3SecretKey;// ConfigurationManager.AppSettings["A2SS3SecretKey"];
                string A2SbucketName = ConfigurationManager.AppSettings["A2SS3Bucketname"];
                IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey);
                //CopyObjectRequest copyrequest = new CopyObjectRequest
                //{
                //    SourceBucket = _bucketName,
                //    SourceKey = string.Format("{0}{1}", srcPath, srcFileName),
                //    DestinationBucket = _bucketName,
                //    DestinationKey = string.Format("{0}{1}", destPath, destFileName),
                //};
               // CopyObjectResponse response =  client.CopyObject(copyrequest);//within same bucket

                IAmazonS3 A2SClient = new AmazonS3Client(A2SAccessKey, A2SSecretKey);
                S3DirectoryInfo destination  = new S3DirectoryInfo(client, _bucketName, string.Format("{0}{1}", destPath, destFileName));
                S3DirectoryInfo source = new S3DirectoryInfo(A2SClient, A2SbucketName, string.Format("{0}{1}", srcPath, ""));
                source.CopyTo(destination);
            }
            catch (Exception ex)
            {
            }
            
        }
        public static void DeleteMultipleFilesFromS3(string path)
        {
            string _awsAccessKey = SOSAWSAccessKey;// ConfigurationManager.AppSettings["SOSAWSAccessKey"];
            string _awsSecretKey = SOSAWSSecretKey; // ConfigurationManager.AppSettings["SOSAWSSecretKey"];
            string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];
            try
            {
                using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
                {
                    var allKeys = GetFolderContents(client, _bucketName, path);
                    allKeys.Remove(path);//we donot want to delete forzip folder
                    if(allKeys.Count() >0)
                        DeleteAllKeys(client, _bucketName, allKeys);
                }
            }
            catch(Exception ex)
            {

            }
        }
        private static List<string> GetFolderContents(IAmazonS3 client , string bucketName, string folderPath)
        {
            var r = new List<string>();
            try
            {
                ListObjectsRequest request = new ListObjectsRequest
                {
                    BucketName = bucketName,
                    Prefix = folderPath,
                    MaxKeys = 100
                };
                do
                {
                    ListObjectsResponse response = client.ListObjects(request);

                    foreach (S3Object entry in response.S3Objects)
                        r.Add(entry.Key);

                    // If response is truncated, set the marker to get the next 
                    // set of keys.
                    if (response.IsTruncated)
                        request.Marker = response.NextMarker;
                    else
                        request = null;
                } while (request != null);
               
            }
            catch (Exception ex)
            {
                // throw error
            }
            return r;
        }
        private static int DeleteAllKeys(IAmazonS3 client, string bucketName, List<string> allKeys)
        {
            var multiObjectDeleteRequest = new DeleteObjectsRequest();
            multiObjectDeleteRequest.BucketName = bucketName;
            foreach (var key in allKeys)
                multiObjectDeleteRequest.AddKey(key, null); // version ID is null
            try
            {
                var response = client.DeleteObjects(multiObjectDeleteRequest);
                return response.DeletedObjects.Count;
            }
            catch (DeleteObjectsException e)
            {
                // throw exception.
            }
            return 0;
        }

        //public static void DeleteFromS3(string FileName, string FilePath,bool deleteRecursive)
        //{
        //    try
        //    {
        //        string _awsAccessKey = ConfigurationManager.AppSettings["SOSAWSAccessKey"];
        //        string _awsSecretKey = ConfigurationManager.AppSettings["SOSAWSSecretKey"];
        //        string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];
        //        using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
        //        {
        //            var deleteObjectRequest = new DeleteObjectRequest
        //            {
        //                BucketName = _bucketName,
        //                Key = string.Format("{0}{1}", FilePath, FileName),
        //            };
        //            S3DirectoryInfo directoryToDelete = new S3DirectoryInfo(client, _bucketName, FilePath);
        //            if (directoryToDelete.Exists)
        //            {
        //                //directoryToDelete.Delete(deleteRecursive);//delete recursively
        //            }
        //            //client.DeleteObject(deleteObjectRequest);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        //section to download file from S3 drive drectly 
        public static byte[] DownloadFromS3(string FileName,string FilePath)
        {
            try
            {
                string _awsAccessKey = SOSAWSAccessKey;// ConfigurationManager.AppSettings["SOSAWSAccessKey"];
                string _awsSecretKey = SOSAWSSecretKey;// ConfigurationManager.AppSettings["SOSAWSSecretKey"];
                string _bucketName = ConfigurationManager.AppSettings["SOSBucketname"];
                byte[] FileData;
                using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
                {
                    GetObjectRequest request = new GetObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = string.Format("{0}{1}", FilePath, FileName),
                    };

                    using (GetObjectResponse response = client.GetObject(request))
                    {
                        FileData = ReadFully(response.ResponseStream);//response.WriteResponseStreamToFile(dest);
                    }
                }
                return FileData;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public static byte[] DownloadFromA2S(string FileName, string FilePath)
        {
            try
            {
                string _awsAccessKey = A2SS3AccessKey; // ConfigurationManager.AppSettings["A2SS3AccessKey"];
                string _awsSecretKey = A2SS3SecretKey; // ConfigurationManager.AppSettings["A2SS3SecretKey"];
                string _bucketName = ConfigurationManager.AppSettings["A2SS3Bucketname"];
                byte[] FileData;
                using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
                {
                    GetObjectRequest request = new GetObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = string.Format("{0}{1}", FilePath, FileName),
                    };

                    using (GetObjectResponse response = client.GetObject(request))
                    {
                        FileData = ReadFully(response.ResponseStream);//response.WriteResponseStreamToFile(dest);
                    }
                }
                return FileData;
            }
            catch(Exception ex)
            {
                //For Testing Purpose
                return null;
            }
        }
        //This method returns content type based on file extension
        public static string GetFileContentType(string fileextension)
        {
            //set the default content-type
            const string DEFAULT_CONTENT_TYPE = "application/unknown"; //"application/force-download";
            RegistryKey regkey, fileextkey;
            string filecontenttype;
            //the file extension to lookup
            try
            {
                //look in HKCR
                regkey = Registry.ClassesRoot;
                //look for extension
                fileextkey = regkey.OpenSubKey(fileextension);
                //retrieve Content Type value
                if (fileextension == "csv")
                {
                    filecontenttype = "text/csv";
                }
                else if(fileextension == "xlsx")
                {
                    filecontenttype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                }
                else
                {
                    filecontenttype = fileextkey.GetValue("Content Type", DEFAULT_CONTENT_TYPE).ToString();
                }
                //for csv
                
                //cleanup
                fileextkey = null;
                regkey = null;
            }
            catch
            {
                filecontenttype = DEFAULT_CONTENT_TYPE;
            }
            //print the content type
            return filecontenttype;
        }
        public static bool SendEmail(string EmailBody, string EmailSubject, string ToAddresses, string CcAddresses, string AttachmentFilePath)
        {
            try
            {
                String[] s = ToAddresses.Split(',');
                for (int j = 0; j < s.Length; j++)
                {
                    s[j] = s[j].Trim();
                }
                SmtpClient Client = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                Client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SenderEmail"], ConfigurationManager.AppSettings["SenderPassword"]);
                Client.Port = 25;
                MailMessage message = new MailMessage();
                message.From = new MailAddress(ConfigurationManager.AppSettings["SenderEmail"]);
                message.To.Add(s[0]);
                if (!string.IsNullOrEmpty(CcAddresses))
                {
                    message.CC.Add(CcAddresses);
                }
                if (!string.IsNullOrEmpty(AttachmentFilePath))
                {
                    var Attachment = new System.Net.Mail.Attachment(AttachmentFilePath);
                    message.Attachments.Add(Attachment);
                }
                message.Body = EmailBody;
                message.Subject = EmailSubject;
                message.IsBodyHtml = true;
                Client.Send(message);
                for (int i = 1; i < s.Length; i++)
                {
                    MailMessage message1 = new MailMessage();
                    message1.From = new MailAddress(ConfigurationManager.AppSettings["SenderEmail"]);
                    message1.To.Add(s[i]);
                    message1.Body = EmailBody;
                    message1.Subject = EmailSubject;
                    if (!string.IsNullOrEmpty(AttachmentFilePath))
                    {
                        var Attachment = new System.Net.Mail.Attachment(AttachmentFilePath);
                        message.Attachments.Add(Attachment);
                    }
                    Client.Send(message1);
                }
                return (true);
            }
            catch (Exception ex)
            {
                return (false);
            }
        }
        //This method will return dropdown data of Status in All R Tables controlllers 
        //public static SelectList GetStatus()
        //{
        //    string[] Status = { "Active", "InActive" };
        //    var x = new SelectList(Status);
        //    return x;
        //}
        //public static SelectList GetStatus(string SelectedStatus)
        //{
        //    string[] Status = { "Direct", "InActive" };
        //    var x = new SelectList(Status, SelectedStatus);
        //    return x;
        //}
        public static bool UploadToS3(HttpPostedFileBase file)
        {
            try
            {

                string publicKey = SOSAWSAccessKey; //ConfigurationManager.AppSettings["AWSAccessKey"];
                string secretKey = SOSAWSSecretKey; // ConfigurationManager.AppSettings["AWSSecretKey"];
                string bucketName = ConfigurationManager.AppSettings["AWSBucket"];



                IAmazonS3 client;
                //using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretKey))
                //{
                //    var request = new PutObjectRequest()
                //    {
                //        BucketName = _bucketName,
                //        CannedACL = S3CannedACL.Private,//PERMISSION TO FILE PUBLIC ACCESIBLE
                //        Key = string.Format("UPLOADS/{0}", file.FileName),
                //        InputStream = file.InputStream//SEND THE FILE STREAM
                //    };

                //    client.PutObject(request);
                //}
            }
            catch (Exception ex)
            {


            }
            return true;
        }
        private static string CreateSignature(string secretKey, string policy)
        {
            var encoding = new ASCIIEncoding();
            var policyBytes = encoding.GetBytes(policy);
            var base64Policy = Convert.ToBase64String(policyBytes);
            var secretKeyBytes = encoding.GetBytes(secretKey);
            var hmacsha1 = new HMACSHA1(secretKeyBytes);
            var base64PolicyBytes = encoding.GetBytes(base64Policy);
            var signatureBytes = hmacsha1.ComputeHash(base64PolicyBytes);
            return Convert.ToBase64String(signatureBytes);
        }
        public static S3FileUploadViewModel GenerateViewModel(string publicKey, string secretKey, string bucketName, string fileName, string redirectUrl)
        {
            var fileUploadVM = new S3FileUploadViewModel();
            //fileUploadVM.FormAction = string.Format("https://{0}.s3.amazonaws.com/", bucketName);
            //fileUploadVM.FormMethod = "post";
            //fileUploadVM.FormEnclosureType = "multipart / form - data";
            //fileUploadVM.Bucket = bucketName;
            //fileUploadVM.FileId = fileName;
            //fileUploadVM.AWSAccessKey = publicKey;
            //fileUploadVM.RedirectUrl = redirectUrl; // one of private, public-read, public-read-write, or authenticated-read 
            //fileUploadVM.Acl = "private"; // Do what you have to to create the policy string here 
            //var policy = CreatePolicy();
            //ASCIIEncoding encoding = new ASCIIEncoding();
            //fileUploadVM.Base64Policy = Convert.ToBase64String(encoding.GetBytes(policy)); 
            //fileUploadVM.Signature = CreateSignature(secretKey, policy);
            return fileUploadVM;
    }
        //this method loads data in portfolio grid that mathches current user portfolio
        //Will apply this VG as Json library is not added
        //public static JsonResult GetPortfolioGrid()
        //{
        //    ILPortfoliosRestClient LPRC = new LPortfoliosRestClient();
        //    var ApiData = LPRC.GetByUserId(CurrentUserId);
        //    return Json(ApiData, JsonRequestBehavior.AllowGet);
        //}

        //This method returns list of active payees in comany with with current user is associated
        //  [ControllerActionFilter]
        //public static SelectList GetPayeeCode(string PortfolioList)
        //{
        //    var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
        //    var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
        //    int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        //    ILPayeesRestClient LPRC = new LPayeesRestClient();
        //    if (Role == "Payee")
        //    {
        //        var PayeeDetails = LPRC.GetByPayeeUserId(UserId);
        //        SelectList x;
        //        //if (PayeeDetails.IsParent)
        //        //{
        //            var Payees = LPRC.GetPayeeHierarchy(CompanyId, UserId,false);
        //            x = new SelectList(Payees, "Id", "FullName", PayeeDetails.Id);
        //        //}
        //        //else
        //        //{
        //        //    var Payees = new[] { new { Id = PayeeDetails.Id, FullName = PayeeDetails.LpFirstName + " " + PayeeDetails.LpLastName + "(" + PayeeDetails.LpPayeeCode + ")" } };
        //        //    x = new SelectList(Payees, "Id", "FullName", PayeeDetails.Id);
        //        //}

        //        return x;
        //    }
        //    else if (Role == "Channel Manager")/*Get all payyes where channel manager is loggedIn User ID*/
        //    {
        //        var PayeeDetails = LPRC.GetPayeeByChannelManagerUserID(CompanyId, UserId,false);
        //        var x = new SelectList(PayeeDetails, "Id", "FullName");
        //        return x;
        //    }
        //    else /*For all other roles get payee list by matching portfolio of payee with logged in user id*/
        //    {
        //        var Payees = LPRC.GetPayeeForClaimsDropdown(CompanyId,UserId,PortfolioList,string.Empty,string.Empty,99999,0,string.Empty);
        //        var x = new SelectList(Payees, "Id", "FullName");
        //        return x;
        //    }
        //    // return new SelectList(null);
        //}

        //Added BY RG
       // [ControllerActionFilter]
        //public static SelectList GetPayeeCode(int id,string PortfolioList)
        //{
        //    var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
        //    var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
        //    int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        //    ILPayeesRestClient LPRC = new LPayeesRestClient();
        //    if (Role == "Payee")
        //    {
        //       // var PayeeDetails = LPRC.GetPayeeByPayeeCode(CompanyId, id);

        //        SelectList x;
        //        //if (PayeeDetails.IsParent)
        //        //{
        //            var Payees = LPRC.GetPayeeHierarchy(CompanyId, UserId,false);
        //            x = new SelectList(Payees, "Id", "FullName", id);
        //        //}
        //        //else
        //        //{
        //        //    var Payees = new[] { new { Id = PayeeDetails.Id, FullName = PayeeDetails.LpFirstName + " " + PayeeDetails.LpLastName + " (" + PayeeDetails.LpPayeeCode + ")" } }.ToList();
        //        //    x = new SelectList(Payees, "Id", "FullName", PayeeDetails.Id);
        //        //}

        //        return x;
        //    }
        //    else if (Role == "Channel Manager")
        //    {
        //        var PayeeDetails = LPRC.GetPayeeByChannelManagerUserID(CompanyId, UserId,false);
        //        var x = new SelectList(PayeeDetails, "Id", "FullName");
        //        return x;
        //    }
        //    else /*For all other roles get payee list by matching portfolio of payee with logged in user id*/
        //    {
        //        var Payees = LPRC.GetPayeeForClaimsDropdown(CompanyId,UserId,PortfolioList, string.Empty, string.Empty, 99999, 0, string.Empty);
        //        var x = new SelectList(Payees, "Id", "FullName");
        //        return x;
        //    }

        //}


        
        //public static dynamic GetPayeeList(string PortfolioList,bool IsDataDisplayedInReport)
        //{
        //    var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
        //    var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
        //    int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        //    ILPayeesRestClient LPRC = new LPayeesRestClient();
        //    if (Role == "Payee")
        //    {
        //      //  var PayeeDetails = LPRC.GetByPayeeUserId(UserId);
        //        SelectList x;
        //        //if (PayeeDetails.IsParent)
        //        //{
        //            var Payees = LPRC.GetPayeeHierarchy(CompanyId, UserId, IsDataDisplayedInReport);
        //            return Payees;
        //        //}
        //        //else
        //        //{
        //        //    var PayeeList = new List<LPayeeViewModel>();
        //        //    var Payees = new LPayeeViewModel { Id = PayeeDetails.Id, FullName = PayeeDetails.LpFirstName + " " + PayeeDetails.LpLastName + "(" + PayeeDetails.LpPayeeCode + ")"  };
        //        //    PayeeList.Add(Payees);
        //        //    return PayeeList;
        //        //}

               
        //    }
        //    else if (Role == "Channel Manager")
        //    {
        //        var PayeeDetails = LPRC.GetPayeeByChannelManagerUserID(CompanyId, UserId, IsDataDisplayedInReport);
        //        return PayeeDetails;
        //    }
        //    else 
        //    {
        //        var Payees = LPRC.GetPayeeForClaimsDropdown(CompanyId, UserId,PortfolioList, string.Empty, string.Empty, 99999, 0, string.Empty);
        //        return Payees;
        //    }
           
        //}
        public static IEnumerable<LPayeeViewModel> GetPayeeListByPayeeId(string PayeeId)
        {
            var Role = System.Web.HttpContext.Current.Session["UserRole"].ToString();
            var UserId = System.Web.HttpContext.Current.Session["UserId"].ToString();
            int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            ILPayeesRestClient LPRC = new LPayeesRestClient();
            var xx=LPRC.GetPayeeHierarchyByPayeeId(CompanyId,PayeeId);
            return xx;
        }
        public static void LogUserEvent(string strUser,string strActivity, string strRemarks,Boolean blnIsSuccess,  string strActionByUserID, int CompanyId,string ClientIPAddress)
        {
            //Get Host name and IP Address
            string hostName = Dns.GetHostName();
            string strHostIP = ClientIPAddress;//SS commented for testing client IP Address//Dns.GetHostByName(hostName).AddressList[0].ToString();
            //Getting Timezone info
            TimeZone curTimeZone = TimeZone.CurrentTimeZone;
            string strHostTimeZone = curTimeZone.StandardName;
            //Browser details
            System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
            string strBrowserDetails =
            "Name = " + browser.Browser + ", " +"Type = " + browser.Type + ", "+ "Version = " + browser.Version + ", "+ "Major Version = " + browser.MajorVersion + ", "
            + "Minor Version = " + browser.MinorVersion + ", "+ "Platform = " + browser.Platform + ", "+ "Is Beta = " + browser.Beta + ", "+ "Is Crawler = " + browser.Crawler + ", "
            + "Is AOL = " + browser.AOL + ", "+ "Is Win16 = " + browser.Win16 + ", "+ "Is Win32 = " + browser.Win32 + ", "+ "Supports Frames = " + browser.Frames + ", "+ "Supports Tables = " + browser.Tables + ", "
            + "Supports Cookies = " + browser.Cookies + ", "+ "Supports VBScript = " + browser.VBScript + ", "+ "Supports JavaScript = " + ", " +browser.EcmaScriptVersion.ToString() + ", "
            + "Supports Java Applets = " + browser.JavaApplets + ", "+ "Supports ActiveX Controls = " + browser.ActiveXControls+ ", "+ "Supports JavaScript Version = " +browser["JavaScriptVersion"];

            GUserActivityLogViewModel UserLog = new GUserActivityLogViewModel();
            UserLog.UalActionById = strActionByUserID;
            UserLog.UalActivity = strActivity;
            UserLog.UalCompanyId = CompanyId;
            //UserLog.UalHostBrowserDetails = strBrowserDetails;
            //UserLog.UalHostIP = strHostIP;
            //UserLog.UalHostTimeZone = strHostTimeZone;
            UserLog.UalIsActivitySucceeded = blnIsSuccess;
            UserLog.UalRemarks = strRemarks;
            UserLog.UalUserId = strUser;
            UserLog.UalActivityDateTime = DateTime.UtcNow;
            //Vodafone_SOS_WebApp.Controllers.GUserActivityLogController UAL = new Controllers.GUserActivityLogController();
            //UAL.CreateLog(UserLog);
            IGUserActivityLogRestClient RestClient = new GUserActivityLogRestClient();
            RestClient.Add(UserLog);
        }
        //ReUsed RK's method to get details of ViewModel of User Activity so as to record faliure 
        public static GUserActivityLogViewModel GetLogUserEventModel()
        {
            //Get Host name and IP Address
            string hostName = Dns.GetHostName();
            string strHostIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            //Getting Timezone info
            TimeZone curTimeZone = TimeZone.CurrentTimeZone;
            string strHostTimeZone = curTimeZone.StandardName;
            //Browser details
            System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
            string strBrowserDetails =
            "Name = " + browser.Browser + ", " + "Type = " + browser.Type + ", " + "Version = " + browser.Version + ", " + "Major Version = " + browser.MajorVersion + ", "
            + "Minor Version = " + browser.MinorVersion + ", " + "Platform = " + browser.Platform + ", " + "Is Beta = " + browser.Beta + ", " + "Is Crawler = " + browser.Crawler + ", "
            + "Is AOL = " + browser.AOL + ", " + "Is Win16 = " + browser.Win16 + ", " + "Is Win32 = " + browser.Win32 + ", " + "Supports Frames = " + browser.Frames + ", " + "Supports Tables = " + browser.Tables + ", "
            + "Supports Cookies = " + browser.Cookies + ", " + "Supports VBScript = " + browser.VBScript + ", " + "Supports JavaScript = " + ", " + browser.EcmaScriptVersion.ToString() + ", "
            + "Supports Java Applets = " + browser.JavaApplets + ", " + "Supports ActiveX Controls = " + browser.ActiveXControls + ", " + "Supports JavaScript Version = " + browser["JavaScriptVersion"];

            GUserActivityLogViewModel UserLog = new GUserActivityLogViewModel();
            UserLog.UalHostBrowserDetails = strBrowserDetails;
            UserLog.UalHostIP = strHostIP;
            UserLog.UalHostTimeZone = strHostTimeZone;
            UserLog.UalActivityDateTime = DateTime.UtcNow;
            return UserLog;
        }
        //Methods introduced by Shubham for server side filtering
        public static string BuildQuery(System.Collections.Specialized.NameValueCollection FilterQuery)
        {
            try
            {
                //RK R2.3 17112018 to handle exception if wrong value is being passed to filterquery variable
                var filtersCount2 = int.Parse(FilterQuery.GetValues("filterscount")[0]);
            }
            catch (Exception)
            {
                return "";
               
            }
            var filtersCount = int.Parse(FilterQuery.GetValues("filterscount")[0]);
            var queryString = "";
            var tmpDataField = "";
            var tmpFilterOperator = "";
            var where = "";
            if (filtersCount > 0)
            {
                where = " AND (";
            }
            for (var i = 0; i < filtersCount; i += 1)
            {
                var filterValue = FilterQuery.GetValues("filtervalue" + i)[0];
                var filterCondition = FilterQuery.GetValues("filtercondition" + i)[0];
                var filterDataField = FilterQuery.GetValues("filterdatafield" + i)[0];
                var filterOperator = FilterQuery.GetValues("filteroperator" + i)[0];
                if (filterDataField != "")
                {
                    if (filterDataField.Contains("LpFirstName") || filterDataField.Contains("LpLastName") || filterDataField.Contains("LpTradingName")
                        || filterDataField.Contains("LcMSISDN") || filterDataField.Contains("LcCustomerName") || filterDataField.Contains("LcBAN")
                        || filterDataField.Contains("LcIMEI") || filterDataField.Contains("LcOrderNumber"))
                    {
                        filterDataField = "CONVERT(nvarchar(max), DecryptByKey("+ filterDataField + "))";
                    }
                }
                if (tmpDataField == "")
                {
                    tmpDataField = filterDataField;
                }
                else if (tmpDataField != filterDataField)
                {
                    where += ") AND (";
                }
                else if (tmpDataField == filterDataField)
                {
                    if (tmpFilterOperator == "0")
                    {
                        where += " AND ";
                    }
                    else
                    {
                        where += " OR ";
                    }
                }
                // build the "WHERE" clause depending on the filter's condition, value and datafield.
                where += GetFilterCondition(filterCondition, filterDataField, filterValue);
                if (i == filtersCount - 1)
                {
                    where += ")";
                }
                tmpFilterOperator = filterOperator;
                tmpDataField = filterDataField;
            }
            queryString += where;
            //queryString = queryString.Replace("LpFirstName", "CONVERT(nvarchar(max), DecryptByKey(LpFirstName))");
            //queryString = queryString.Replace("LpLastName", "CONVERT(nvarchar(max), DecryptByKey(LpLastName))");
            //queryString = queryString.Replace("LpTradingName", "CONVERT(nvarchar(max), DecryptByKey(LpTradingName))");
            ////Replace Claims Encrypted Columns
            //queryString = queryString.Replace("LcMSISDN", "CONVERT(nvarchar(max), DecryptByKey(LcMSISDN))");
            //queryString = queryString.Replace("LcCustomerName", "CONVERT(nvarchar(max), DecryptByKey(LcCustomerName))");
            //queryString = queryString.Replace("LcBAN", "CONVERT(nvarchar(max), DecryptByKey(LcBAN))");
            //queryString = queryString.Replace("LcIMEI", "CONVERT(nvarchar(max), DecryptByKey(LcIMEI))");
            //queryString = queryString.Replace("LcOrderNumber", "CONVERT(nvarchar(max), DecryptByKey(LcOrderNumber))");
            return queryString;
        }
        //convert the filter condtion in a sql statement
        public static string GetFilterCondition(string filterCondition, string filterDataField, string filterValue)
        {
            //List of available filters present in JqxGrid are present in case statements to form Sql Query
            switch (filterCondition)
            {
                case "NOT_EMPTY":
                case "NOT_NULL":
                    return " " + filterDataField + " NOT LIKE N'" + "" + "'";
                case "EMPTY":
                case "NULL":
                    return " " + filterDataField + " LIKE N'" + "" + "'";
                case "CONTAINS_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE N'%" + filterValue + "%'" + " COLLATE SQL_Latin1_General_CP1_CS_AS";
                case "CONTAINS":
                    return " " + filterDataField + " LIKE N'%" + filterValue + "%'";
                case "DOES_NOT_CONTAIN_CASE_SENSITIVE":
                    return " " + filterDataField + " NOT LIKE N'%" + filterValue + "%'" + " COLLATE SQL_Latin1_General_CP1_CS_AS"; ;
                case "DOES_NOT_CONTAIN":
                    return " " + filterDataField + " NOT LIKE N'%" + filterValue + "%'";
                case "EQUAL_CASE_SENSITIVE":
                    return " " + filterDataField + " = N'" + filterValue + "'" + " COLLATE SQL_Latin1_General_CP1_CS_AS"; ;
                case "EQUAL":
                    if(filterDataField.ToLower().Contains("date"))
                    return " " + filterDataField + " = Convert(date,'" + filterValue + "') ";
                    else
                        return " " + filterDataField + " = '" + filterValue + "'";
                case "NOT_EQUAL_CASE_SENSITIVE":
                    return " BINARY " + filterDataField + " <> '" + filterValue + "'";
                case "NOT_EQUAL":
                    if (filterDataField.ToLower().Contains("date"))
                        return " " + filterDataField + " <> Convert(date,'" + filterValue + "') ";
                    else
                    return " " + filterDataField + " <> '" + filterValue + "'";
                case "GREATER_THAN":
                    if (filterDataField.ToLower().Contains("date"))
                        return " " + filterDataField + " > Convert(date,'" + filterValue + "') ";
                    else
                    return " " + filterDataField + " > '" + filterValue + "'";
                case "LESS_THAN":
                    if (filterDataField.ToLower().Contains("date"))
                        return " " + filterDataField + " < Convert(date,'" + filterValue + "') ";
                    else
                    return " " + filterDataField + " < '" + filterValue + "'";
                case "GREATER_THAN_OR_EQUAL":
                    if (filterDataField.ToLower().Contains("date"))
                        return " " + filterDataField + " >= Convert(date,'" + filterValue + "') ";
                    else
                    return " " + filterDataField + " >= '" + filterValue + "'";
                case "LESS_THAN_OR_EQUAL":
                    if (filterDataField.ToLower().Contains("date"))
                        return " " + filterDataField + " <= Convert(date,'" + filterValue + "') ";
                    else
                    return " " + filterDataField + " <= '" + filterValue + "'";
                case "STARTS_WITH_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE N'" + filterValue + "%'" + " COLLATE SQL_Latin1_General_CP1_CS_AS"; ;
                case "STARTS_WITH":
                    return " " + filterDataField + " LIKE N'" + filterValue + "%'";
                case "ENDS_WITH_CASE_SENSITIVE":
                    return " " + filterDataField + " LIKE N'%" + filterValue + "'" + " COLLATE SQL_Latin1_General_CP1_CS_AS"; ;
                case "ENDS_WITH":
                    return " " + filterDataField + " LIKE N'%" + filterValue + "'";
            }
            return "";
        }
        //RK created method to export data in excel from datatable
        public static void ExportFromDataTable(string CompanyCode, string LoggedInUserName, string FileName, DataTable tb)
        {
            
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < tb.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string columnName = tb.Columns[j].ToString();
                cell.SetCellValue(columnName);
                // GC is used to avoid error System.argument exception
                GC.Collect();
            }

            //loops through data  
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < tb.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = tb.Columns[j].ToString();
                    cell.SetCellValue(tb.Rows[i][columnName].ToString());
                }
            }
            //Create OPCO Folder
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode);
            }
            //Create UserName Folder
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName);
            }

            //using (ZipFile zip = new ZipFile())
            //{
            FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName , FileName + ".xlsx"), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            //    //tb.TableName = "Tmp";
            //    //zip.AddEntry("Temp.xml", (name, stream) => tb.WriteXml(stream));
            //    zip.AddFile(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip", FileNameInsideZip + ".xlsx"), "");
            //    zip.Save(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip", FileName));
            //    System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + CompanyCode + "/" + LoggedInUserName + "/forzip", FileNameInsideZip + ".xlsx"));
            xfile.Close();

            //}
        }
        public static void CreateExcelFromDataTable(DataTable DT, string strFileName, string strWorksheetName)
        {
            try
            {
                if (strWorksheetName == "") strWorksheetName = "Sheet1";
                var workbook = new XLWorkbook();
                workbook.Worksheets.Add(DT, strWorksheetName);
                var ws = workbook.Worksheet(1);
                workbook.SaveAs(strFileName);
            }
            catch (Exception ex)
            { }
        }

        //-- RS Implemented on 22 march 2019 " with the below method we can convert list to a datatable"
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
    public class RestrictSpecialCharAttribute : RegularExpressionAttribute
    {
        static RestrictSpecialCharAttribute()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RestrictSpecialCharAttribute), typeof(RegularExpressionAttributeAdapter));
        }
        //implemented by RS : Now it only accept A-Za-z0-9'_@.-:, and space
         //public RestrictSpecialCharAttribute() : base("[A-Za-z0-9-'\n?(),:_@. ]*") { ErrorMessage = "Special characters are not allowed."; }

        //Changed in the method by RS on 1st Feb with the below implementation the symbols will be restricting rest all will be allowed. 
        public RestrictSpecialCharAttribute() : base("[^<>&%]*$") { ErrorMessage = "Special characters are not allowed."; }
    }




    //Thisclass contains the required methods of System.IO.Compression to create zip files
    public static class ZipHelper
    {
        public static void ZipFiles(string path, IEnumerable<string> files,
               CompressionOption compressionLevel = CompressionOption.Normal)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                ZipHelper.ZipFilesToStream(fileStream, files, compressionLevel);
            }
        }

        public static byte[] ZipFilesToByteArray(IEnumerable<string> files,
               CompressionOption compressionLevel = CompressionOption.Normal)
        {
            byte[] zipBytes = default(byte[]);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                ZipHelper.ZipFilesToStream(memoryStream, files, compressionLevel);
                memoryStream.Flush();
                zipBytes = memoryStream.ToArray();
            }

            return zipBytes;
        }

      

        private static void ZipFilesToStream(Stream destination,
          IEnumerable<string> files, CompressionOption compressionLevel)
        {
            using (Package package = Package.Open(destination, FileMode.Create))
            {
                foreach (string path in files)
                {
                    // fix for white spaces in file names (by ErrCode)
                    Uri fileUri = PackUriHelper.CreatePartUri(new Uri(@"/" +
                                  Path.GetFileName(path), UriKind.Relative));
                    string contentType = @"data/" + ZipHelper.GetFileExtentionName(path);

                    using (Stream zipStream =
                            package.CreatePart(fileUri, contentType, compressionLevel).GetStream())
                    {
                        using (FileStream fileStream = new FileStream(path, FileMode.Open))
                        {
                            fileStream.CopyTo(zipStream);
                        }
                    }
                }
            }
        }

        private static string GetFileExtentionName(string path)
        {
            string extention = Path.GetExtension(path);
            if (!string.IsNullOrWhiteSpace(extention) && extention.StartsWith("."))
            {
                extention = extention.Substring(1);
            }

            return extention;
        }
    }
}