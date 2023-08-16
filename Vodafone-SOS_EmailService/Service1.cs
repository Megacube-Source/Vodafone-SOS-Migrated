using System;
using System.ServiceProcess;

using System.IO;
using System.Threading;
using System.Configuration;

using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.IO;

namespace Vodafone_SOS_EmailService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //RK 31102019 added try catch block here
            try
            {
                this.WriteToFile("LiteSOSEmail Service started {0}");
                this.WriteToFile("LiteSOSEmail Service Mode: Interval");
                this.ScheduleService();
            }
            catch (Exception)
            {
                SqlConnection cn = new SqlConnection();
                cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                cn.Open();

                //R2.8.6 SG --Tempearly inserting into debug table to debug the issue.
                string qry = "insert into Debug(CreatedDateTime,msg) select getdate(),'EmailServiceLog: Failed to start' ";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd.ExecuteNonQuery();
            }
            
        }

        protected override void OnStop()
        {
            this.WriteToFile("LiteSOSEmail Service stopped {0}");
            this.Schedular.Dispose();
        }

        private Timer Schedular;

        public void ScheduleService()
        {

            //Set the Default Time.
            DateTime scheduledTime = DateTime.MinValue;
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback));
                //string mode = ConfigurationManager.AppSettings["Mode"].ToUpper();
               // string mode = "INTERVAL";

                //if (mode == "DAILY")
                //{
                //    //Get the Scheduled Time from AppSettings.
                //    scheduledTime = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]);
                //    if (DateTime.Now > scheduledTime)
                //    {
                //        //If Scheduled Time is passed set Schedule for the next day.
                //        scheduledTime = scheduledTime.AddDays(1);
                //    }
                //}

                //if (mode.ToUpper() == "INTERVAL")
                //{
                    //Get the Interval in Minutes from AppSettings.
                    int intervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMinutes"]);

                    //Set the Scheduled Time by adding the Interval to Current Time.
                    scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next Interval.
                        scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
                    }
                    else 
                    {
                        //Call method to send Email
                        SendEmails();
                    }
                   // }

                //TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                //string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                ////this.WriteToFile("LiteSOSEmail Service scheduled to run after: " + schedule + " {0}");
                ////Get the difference in Minutes between the Scheduled and Current Time.
                //int dueTime = Math.Abs(Convert.ToInt32(timeSpan.TotalMilliseconds));
                ////Change the Timer's Due Time.
                //Schedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
               
                WriteToFile("LiteSOSEmail Service Error on: {0} " + ex.Message + ex.StackTrace);
                //Stop the Windows Service.
                //using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController("LiteSOSEmailService"))
                //{
                //   // serviceController.Stop(); commented by Shubham as service should continue even after exception
                //}
            }
            //SS Even though Exception is generated we will Still Set the timer for the next schedule
            TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
            string schedule = string.Format("{0} day(s) {1} hour(s) {2} minute(s) {3} seconds(s)", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            //Get the difference in Minutes between the Scheduled and Current Time.
            int dueTime = Math.Abs(Convert.ToInt32(timeSpan.TotalMilliseconds));
            //Change the Timer's Due Time.
            Schedular.Change(dueTime, Timeout.Infinite);
        }

        private void SchedularCallback(object e)
        {
            this.WriteToFile("LiteSOSEmail Service Log CallBack: {0}");
            this.ScheduleService();
        }

        private void WriteToFile(string text)
        {
            //string path = "C:\\ServiceLog.txt";
            // string path = "C:\\Users\\Vikgup03\\Desktop\\ServiceLog.txt";//commented by SS for testing
            string path = ConfigurationManager.AppSettings["LogFilePath"];
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }
        //method to convert stram to byte array
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

        private void SendEmails()
        {

             string _awsSOSAccessKey = GetKeys("sos_accesskey"); //int_accesskey
             string _awsSOSSecretKey = GetKeys("sos_secretkey"); //int_secretkey
             string _awsIntAccessKey = GetKeys("int_accesskey"); //int_secretkey
             string _awsIntSecretKey = GetKeys("int_secretkey"); //int_secretkey

            
            //Connect to database
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString =  ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();

            //Get the list of unsent emails
            //Suggested Values - InQueue/Sent/Failed/Bouncedback
            //Replace * with actual column names when code starts running
            string qry = "Select B.LebBucketName,B.LebAccessKey,B.LebSecretKey,C.GecRequiresSSL,C.GecEmailId,C.GecDisplayName,C.GecReplyTo, C.GelSmtpHost,C.GecSmtpLoginId,C.GecSMTPPassword,C.GecPortNumber,B.LebReplyToList,B.LebPriority,B.LebIsHTML,B.LebRecipientList,B.LebCCList,B.LebBCCList,B.LebBody,B.LebSubject,B.LebAttachmentList,B.Id From LEmailBucket B Inner Join GEmailConfigurations C ON B.LebSenderConfigId = C.Id Where LebStatus = 'InQueue' ORDER BY CASE WHEN LebPriority LIKE '%Low' THEN 3 WHEN LebPriority LIKE '%Normal' THEN 2 WHEN LebPriority LIKE '%High' THEN 1 END";
            SqlCommand cmd = new SqlCommand(qry, cn);
           
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable tb = new DataTable();
            da.Fill(tb);
            
            if(tb.Rows.Count>0)//check if there are any pending emails that needs to be sent
            {
                //For each iteration of the list update the database with the status of the email sending for that particular row.
                //Loop through the list and send emails one by one
                foreach (DataRow dr in tb.Rows)
                {
                    try
                    {
                        SmtpClient Client = new SmtpClient(dr.Field<string>("GelSmtpHost"));
                    Client.Credentials = new NetworkCredential(dr.Field<string>("GecSmtpLoginId"), dr.Field<string>("GecSMTPPassword"));
                    Client.Port = dr.Field<int>("GecPortNumber");
                    Client.EnableSsl = dr.Field<bool>("GecRequiresSSL");
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(dr.Field<string>("GecEmailId"), dr.Field<string>("GecDisplayName"));
                   

                    //set priority
                    switch (dr.Field<string>("LebPriority").ToLower())
                    {
                        case "low":
                            message.Priority = MailPriority.Low;
                            break;
                        case "normal":
                            message.Priority = MailPriority.Normal;
                            break;
                        case "high":
                            message.Priority = MailPriority.High;
                            break;
                    }
                    var ReceipientArray = dr.Field<string>("LebRecipientList").Split(',');// comma, seperator;,
                    var ReplyToList=new System.Collections.Generic.List<string>();
                    //Add ReplyTo Email from LEmailBucket and if it null then use ReplyToEmail from GEmailConfigurations
                    if (!string.IsNullOrEmpty(dr.Field<string>("LebReplyToList")))
                    {
                        var ReplyToArray= dr.Field<string>("LebReplyToList").Split(',');
                        ReplyToList = ReplyToArray.ToList(); //|;
                    }
                    else
                    {
                        message.ReplyToList.Add(dr.Field<string>("GecReplyTo"));
                    }
                    //Loop through Receipient array
                    for (int j = 0; j < ReceipientArray.Length; j++)
                    {
                        message.To.Add(ReceipientArray[j]);//Add receipients to the mail
                        
                    }

                    for(var k=0;k<ReplyToList.Count();k++)
                    {
                            message.ReplyToList.Add(ReplyToList.ElementAt(k));
                    }

                    if (!string.IsNullOrEmpty(dr.Field<string>("LebCCList")))
                    {
                        var CCArray = dr.Field<string>("LebCCList").Split(',');
                        for (int j = 0; j < CCArray.Length; j++)
                        {
                            message.To.Add(CCArray[j]);//Add CC to the mail
                        }
                    }

                    if (!string.IsNullOrEmpty(dr.Field<string>("LebBCCList")))
                    {
                        var BCCArray = dr.Field<string>("LebBCCList").Split(',');
                        for (int j = 0; j < BCCArray.Length; j++)
                        {
                            message.To.Add(BCCArray[j]);//Add BCC to the mail
                        }
                    }

                    message.Body = dr.Field<string>("LebBody");
                    message.Subject = dr.Field<string>("LebSubject");
                    message.IsBodyHtml = dr.Field<bool>("LebIsHTML");
                        float FileSizeMB = 0;
                        if (!string.IsNullOrEmpty(dr.Field<string>("LebAttachmentList")))
                        {
                            //NOTE : We require that Files are to be downloaded from bucket (a2s) in below code.
                            //Get File from A2S S3
                            string _awsAccessKey = string.Empty;
                            string _awsSecretKey = string.Empty;
                            string _bucketName = ConfigurationManager.AppSettings.AllKeys.Where(key => key.StartsWith(dr.Field<string>("LebBucketName"))).Select(key => ConfigurationManager.AppSettings[key]).FirstOrDefault();//dr.Field<string>("LebBucketName"); 

                            if(dr.Field<string>("LebBucketName") == "SOSS3Bucketname")
                            {
                                _awsAccessKey = _awsSOSAccessKey;
                                _awsSecretKey = _awsSOSSecretKey;
                            } else
                            {
                                _awsAccessKey = _awsIntAccessKey;
                                _awsSecretKey = _awsIntSecretKey;
                            }
                         
                            //string _awsAccessKey = GetKeys(dr.Field<string>("LebAccessKey"));// ConfigurationManager.AppSettings.AllKeys.Where(key => key.StartsWith(dr.Field<string>("LebAccessKey"))).Select(key => ConfigurationManager.AppSettings[key]).FirstOrDefault();
                            //string _awsAccessKey = GetKeys(_bucketName, "A");// ConfigurationManager.AppSettings.AllKeys.Where(key => key.StartsWith(dr.Field<string>("LebAccessKey"))).Select(key => ConfigurationManager.AppSettings[key]).FirstOrDefault();
                            //string _awsSecretKey = GetKeys(dr.Field<string>("LebSecretKey")); //ConfigurationManager.AppSettings.AllKeys.Where(key => key.StartsWith(dr.Field<string>("LebSecretKey"))).Select(key => ConfigurationManager.AppSettings[key]).FirstOrDefault();//dr.Field<string>("LebSecretKey"); 
                            //string _awsSecretKey = GetKeys(_bucketName, "S"); //ConfigurationManager.AppSettings.AllKeys.Where(key => key.StartsWith(dr.Field<string>("LebSecretKey"))).Select(key => ConfigurationManager.AppSettings[key]).FirstOrDefault();//dr.Field<string>("LebSecretKey"); 
                            //If we need to select all files in a folder
                            if (dr.Field<string>("LebAttachmentList").ToString().Contains("*.*"))
                            {
                                AmazonS3Client s3Client = new AmazonS3Client(_awsAccessKey, _awsSecretKey);
                                var FolderCompleteName = dr.Field<string>("LebAttachmentList").ToString().Replace("\\*.*", "");
                                S3DirectoryInfo dir = new S3DirectoryInfo(s3Client, _bucketName, FolderCompleteName);


                                var files = dir.GetFiles();
                                foreach (IS3FileSystemInfo file in dir.GetFileSystemInfos())
                                {
                                    Console.WriteLine(file.Name);
                                    //NOTE:- As the SDK Request method support forward slash in path we need to replace the same to avoid errors
                                    FolderCompleteName = FolderCompleteName.Replace("\\", "/");
                                    GetObjectRequest request = new GetObjectRequest
                                    {
                                        BucketName = _bucketName,
                                        Key = FolderCompleteName + "/" + file.Name,
                                    };

                                    using (GetObjectResponse response = s3Client.GetObject(request))
                                    {
                                        var FileName = file.Name;
                                        //add attachment
                                        var ByteData = ReadFully(response.ResponseStream);
                                        //TotalSize = TotalSize + Math.Round((Convert.ToDouble(ByteData) / 1024f) / 1024f, 2);
                                         FileSizeMB = (ByteData.Length / 1024f) / 1024f;
                                        if (FileSizeMB > 10)
                                        {
                                            message.Body = message.Body + "<br><br> NOTE: The attachment has been stripped from this email since its size was bigger then 10MB.";
                                            //RK 17112019 rolled back to july 2019 version as the change which we di in R2.8.6 (October 2019) making the above change, corrupted the service file and once deployed, its not getting started.
                                        }
                                        else
                                        {
                                            var Attachment = new System.Net.Mail.Attachment(new MemoryStream(ByteData), FileName);
                                            message.Attachments.Add(Attachment);
                                        }
                                    }
                                }
                            }
                            else//Get Files from Pipe seperated string
                            {
                                var AttachmentArray = dr.Field<string>("LebAttachmentList").Split('|');//pipe seperated string
                                for (int j = 0; j < AttachmentArray.Length; j++)
                                {
                                    if (!string.IsNullOrEmpty(AttachmentArray[j]))
                                    {
                                        using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
                                        {
                                            GetObjectRequest request = new GetObjectRequest
                                            {
                                                BucketName = _bucketName,
                                                Key = AttachmentArray[j],
                                            };

                                            using (GetObjectResponse response = client.GetObject(request))
                                            {
                                                var FileName = AttachmentArray[j].Split('/').LastOrDefault();
                                                var ByteData = ReadFully(response.ResponseStream);
                                                 FileSizeMB = FileSizeMB + (ByteData.Length / 1024f) / 1024f;

                                            }
                                        }
                                    }
                                }
                                if (FileSizeMB < 10)
                                {
                                    for (int j = 0; j < AttachmentArray.Length; j++)
                                    {
                                        if (!string.IsNullOrEmpty(AttachmentArray[j]))
                                        {
                                            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
                                            {
                                                GetObjectRequest request = new GetObjectRequest
                                                {
                                                    BucketName = _bucketName,
                                                    Key = AttachmentArray[j],
                                                };

                                                using (GetObjectResponse response = client.GetObject(request))
                                                {
                                                    var FileName = AttachmentArray[j].Split('/').LastOrDefault();
                                                    //add attachment
                                                    var ByteData = ReadFully(response.ResponseStream);
                                                    var Attachment = new System.Net.Mail.Attachment(new MemoryStream(ByteData), FileName);
                                                    message.Attachments.Add(Attachment);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    message.Body = message.Body + "<br><br> NOTE: The attachment has been stripped from this email since its size was bigger then 10MB.";
                                }
                            }
                        }
                   
                        Client.Send(message);
                        cn.Close();
                        cn.Open();
                            qry = "update LEmailBucket set LebStatus = 'Sent',LebUpdatedDateTime=GetDate(),LebSendDateTime=GetDate() where Id=" + dr.Field<int>("Id");
                            cmd = new SqlCommand(qry, cn);
                            cmd.ExecuteScalar();
                        cn.Close();
                    }
                    catch(Exception ex)
                    {
                        cn.Close();
                        cn.Open();
                        WriteToFile("LiteSOSEmail Service Error on: {0} " + ex.Message + ex.StackTrace);
                        qry = "update LEmailBucket set LebStatus = 'Failed',LebUpdatedDateTime=GetDate(),LebComments='"+ex.InnerException.Message+"' where Id=" + dr.Field<int>("Id");
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();
                        cn.Close();
                        continue;
                    }
                }
               // cn.Close();
                }
            
            

        }

        private string GetKeys(string Key)
        {
            string keyValue = string.Empty;
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            try
            {
                //if ((BucketName == "prod-sos-bucket" || BucketName == "project-lite-staging-bucket20200326092710470000000001") && Type == "A") { Key = "sos_accesskey"; }
                //else if ((BucketName == "prod-sos-bucket" || BucketName == "project-lite-staging-bucket20200326092710470000000001") && Type == "S") { Key = "sos_secretkey"; }
                //else if ((BucketName == "prod-int-bucket" || BucketName == "project-lite-staging-sos-int20200415094638927500000001") && Type == "A") { Key = "int_accesskey"; }
                //else if ((BucketName == "prod-int-bucket" || BucketName == "project-lite-staging-sos-int20200415094638927500000001") && Type == "S") { Key = "int_secretkey"; }


                string qry = "select top 1 GkvValue as GkvValue from GKeyValues  where GkvKey = '" + Key+"'";
                SqlCommand cmd = new SqlCommand(qry, cn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable tb = new DataTable();
                da.Fill(tb);

                if (tb.Rows.Count > 0)
                {
                    foreach (DataRow dr in tb.Rows)
                    {

                        if (!string.IsNullOrEmpty(Convert.ToString(dr["GkvValue"])))
                        {

                            keyValue = Convert.ToString(dr["GkvValue"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cn.Close();
            }
            return keyValue;


        }
    }
}
