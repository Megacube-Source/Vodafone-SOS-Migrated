using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Net;
using System.Net.Mail;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Vodafone_SOS_NightlyJobConsole
{
    class Program
    {
        static String FileName = DateTime.UtcNow.ToString("yyyyMMdd") + "_AuditReport.xlsx";
        static string ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"].ToLower();
        static string AWSKey = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["AuditS3RootFolder"], ProjectEnviournment, FileName);
        static Boolean IsMailSend = false;
        static string body = "<table><tr><td>Application Name</td><td>Method Name</td><td>Exception Date/Time(Utc)</td><td>Stack Trace</td>	</tr>";
        //static string _awsAccessKey = GetKeys("audit_accesskey"); //int_accesskey
        //static string _awsSecretKey = GetKeys("audit_secretkey"); //int_secretkey
        static string UserId = GetKeys("smtp_accesskey");   //ConfigurationManager.AppSettings["SMTPUserId"].ToString();
        static string Password = GetKeys("smtp_secretkey");  //ConfigurationManager.AppSettings["SMTPPassword"].ToString();
        static void Main(string[] args)
        {
            //These two methods will be called when The exe file of project will be execcuted
             //WriteToFile("Nightly Job execution started on {0}");
            try
            {
                CheckPayeeEffectiveDating();
                WriteToFile("CheckPayeeEffectiveDating completed on {0}");
            }
            catch (Exception ex)
            {
                WriteToFile("CheckPayeeEffectiveDating failed due to " + ex.StackTrace.ToString());
            }
            try
            {
                CheckDoulblePatentPayee();
                WriteToFile("CheckDoulbleParentPayee completed on {0}");
            }
            catch (Exception ex)
            {
                WriteToFile("CheckDoulbleParentPayee failed due to " + ex.StackTrace.ToString());
            }

            //Call a method to Generate AuditReport on 1st of every month
            try
            {
                GenerateAuditReport();
                WriteToFile("GenerateAuditReport completed on {0}");
            }
            catch (Exception ex)
            {
                WriteToFile("GenerateAuditReport failed due to " + ex.StackTrace.ToString());
            }
            try
            {
                UpdateMissingCliamsPortfolio();
                WriteToFile("UpdateMissingCliamsPortfolio completed on {0}");
            }
            catch (Exception ex)
            {
                WriteToFile("UpdateMissingCliamsPortfolio failed due to " + ex.StackTrace.ToString());
            }

            try
            {
                UpdateMissingPayeesPortfolio();
                WriteToFile("UpdateMissingPayeesPortfolio completed on {0}");
            }
            catch (Exception ex)
            {
                WriteToFile("UpdateMissingPayeesPortfolio failed due to " + ex.StackTrace.ToString());
            }
            try
            {
                DeleteBucketPayeeOlderthanMonth();
                WriteToFile("DeleteBucketPayeeOlderthanMonth completed on {0}");
            }
            catch (Exception ex)
            {
                WriteToFile("DeleteBucketPayeeOlderthanMonth failed due to " + ex.StackTrace.ToString());
            }
            try
            {
                AcceptLobbyUsers();
                WriteToFile("AcceptLobbyUsers completed on {0}");
            }
            catch (Exception ex)
            {
                WriteToFile("AcceptLobbyUsers failed due to " + ex.StackTrace.ToString());
            }

            try
            {
                DataPurge();
                WriteToFile("DataPurge completed on {0}");
            }
            catch (Exception ex)
            {
                WriteToFile("DataPurge failed due to " + ex.StackTrace.ToString());
            }
            try
            {
                BlockNotificationForDummyUsers();
                WriteToFile("BlockNotification completed on {0}");
            }
            catch (Exception ex)
            {
                WriteToFile("BlockNotification failed due to " + ex.StackTrace.ToString());
            }
            try
            {
                CheckDuplicatePayees();
                WriteToFile("CheckDuplicatePayees completed on {0}");
            }
            catch (Exception ex)
            {
                WriteToFile("CheckDuplicatePayees failed due to " + ex.StackTrace.ToString());
            }
            try
            {
                //AutoClose all tickets if they have been open for more than 60 days, other than 'Add To Requirements'
                CloseTickets();
                WriteToFile("CloseTickets completed on {0}");
            }
            catch (Exception ex)
            {
                WriteToFile("CloseTickets failed due to " + ex.StackTrace.ToString());
            }
            body = body + "</table>";
            if(IsMailSend)
            {
                SendEmail(ConfigurationManager.AppSettings["ExceptionEmailCc"], ConfigurationManager.AppSettings["ExceptionEmailTo"], ConfigurationManager.AppSettings["ExceptionEmailSubject"], body);
                
            }
            WriteToFile("Nightly Job sucessfully executed on {0}");
            WriteToFile("-----------------------------------------------------");
        }

        //Method to Add entry in Log File
        private static void WriteToFile(string text)
        {
            string path = ConfigurationManager.AppSettings["LogFilePath"];
            //using (StreamWriter writer = new StreamWriter(path, true))
            //{
            //    writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
            //    writer.Close();
            //    }
            }

        private static void CheckPayeeEffectiveDating()
        {
           
                //Connect to database
                SqlConnection cn = new SqlConnection();
                cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                cn.Open();
            var ErrorFlag = false;
                var CurrentDate = DateTime.UtcNow.AddMinutes(1).ToString("yyyy-MM-dd");
                var FutureDate = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd");
            //Check for payees whose Effective End Date is less than equal to Current Date. Then make those Payees InActive in the System
            //string qry = "Select Id,LpUserId,LpEmail from LPayees where LpEffectiveEndDate<='" + CurrentDate + "' and LpEffectiveEndDate is not null and WFStatus in ('Completed','Saved','InProgress')";

            string qry = "Select P.Id,P.LpUserId,P.LpEmail from LPayees P join gkeyvalues g on p.LpCompanyId = G.GkvCompanyId and g.GkvKey='WIAMEnabled' where LpEffectiveEndDate<='" + CurrentDate + "' and LpEffectiveEndDate is not null and WFStatus in ('Completed','Saved','InProgress','Suspended') and ((GkvValue = 'no' and  p.LpCreateLogin= 1) or P.LpCreateLogin=0)"; //We have put this one 'LpEffectiveEndDate is not null' to avoid comparing null value with currentdatetime.
            SqlCommand cmd = new SqlCommand(qry, cn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable tb = new DataTable();
                da.Fill(tb);

                if (tb.Rows.Count > 0)//check if there are any payee records to be made InActive
                {
               
                    WriteToFile("Looping through List of Payees which are made InActive");
                    //Loop through the list and make payees inactive one by one in LPayees and AspnetUsers
                    foreach (DataRow dr in tb.Rows)
                    {
                    try
                    {
                        Console.WriteLine("PayeeId Updating:-" + dr.Field<int>("Id"));
                        qry = "update LPayees set WFStatus='InActive', WFUpdatedDateTime=getdate() where Id=" + dr.Field<int>("Id");
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
                        //Since Test and Prod have common AD so adding below line so that we dont accidently remove AD account present in ProdDB while testing in Dev or test
                        if (ProjectEnviournment.Equals("Prod", StringComparison.OrdinalIgnoreCase))
                        {
                            DeleteUser(dr.Field<string>("LpEmail"));//Delete AD account
                        }

                        // Added on 20/8/2019:Comment following code with comments: “Do not remove Roles, Portfolios, LPasswordHistory, SecurityQuestions, Update email in Xschema.Xreports.These will be cleaned up after 2 years as part of NIghtlyJob.Purge

                        //delete payee portfolios
                        //RK 07102018 commented portfolios deletion as part of R2.2
                        //qry = "delete from MEntityPortfolios where MepEntityType='LPayees' and MepEntityId=" + dr.Field<int>("Id");
                        //cmd = new SqlCommand(qry, cn);
                        //cmd.ExecuteScalar();
                        //if (!string.IsNullOrEmpty(dr.Field<string>("LpUserId")))
                        //{

                        //    string qryCount = "select * from AspnetUserRoles  where UserId = '" + dr.Field<string>("LpUserId") + "' ";
                        //    SqlCommand cmd1 = new SqlCommand(qryCount, cn);

                        //    SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        //    DataTable tb1 = new DataTable();
                        //    da.Fill(tb1);
                        //    if (tb1.Rows.Count == 1)
                        //    {
                        //Also check if User record exist with same email so as to deactivate the FinOps roles.
                        //qry = "update LUsers set WFStatus='InActive' where LuUserId='" + dr.Field<string>("LpUserId") + "'";
                        //cmd = new SqlCommand(qry, cn);
                        //cmd.ExecuteScalar();
                        // }
                        //do not do because it gives message 'User is InActive', which is confusing. Also, it creates problems if user has FinOps and Payee accounts both
                        //qry = "update AspnetUsers set IsActive=0 where Id='" + dr.Field<string>("LpUserId") + "'";
                        //cmd = new SqlCommand(qry, cn);
                        //cmd.ExecuteScalar();

                        //delete user roles
                        //RK 07102018 commented role deletion as part of R2.2
                        //qry = "delete from AspnetUserRoles where UserId='" + dr.Field<string>("LpUserId") + "'";
                        //qry = "delete AUR from AspnetUserRoles AUR JOIN AspNetRoles AR  on AR.Id = AUR.RoleId where Aur.UserId = '" + dr.Field<string>("LpUserId") + "' and AR.Name = 'Payee'";
                        //cmd = new SqlCommand(qry, cn);
                        //cmd.ExecuteScalar();
                        // }

                    }
                    catch (Exception ex)
                    {
                        ErrorFlag = true;
                        WriteToFile("Error Generated from Nightly Job(CheckPayeeEffectiveDating) :" + ex.StackTrace);
                        // Add Error Log to database 
                        qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'CheckPayeeEffectiveDating', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();
                        cn.Close();
                        continue;
                    }
                }
                    WriteToFile("Payees sucessfully made InActive");
                if (ErrorFlag)//Send Email Notification via SMTP
                {
                    IsMailSend = true;
                    //Send Email regarding error to L2 Admin in case of any exception generated in method
                    //var Body = "<table border='1'><tr><td>Application Name</td><td>Vodafone_SOS_NightlyJobConsole</td></tr><tr><td>Controller</td><td>CheckPayeeEffectiveDating</td></tr><tr><td>Method Name</td><td></td></tr><tr><td>Exception Date/Time(Utc)</td><td>" + DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td>System</td></tr><tr><td>Stack Trace</td><td>(CheckPayeeEffectiveDating Failed) sent to ExceptionEmail (multiple emails can be listed) variable configured in WebConfig</td></tr></table>";
                    //SendEmail(ConfigurationManager.AppSettings["ExceptionEmailCc"], ConfigurationManager.AppSettings["ExceptionEmailTo"], "Nightly Job Failed - CheckPayeeEffectiveDating", Body);
                    body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>CheckPayeeEffectiveDating</td><td>" + DateTime.UtcNow.ToString() + "</td><td>(CheckPayeeEffectiveDating Failed) sent to ExceptionEmail (multiple emails can be listed) variable configured in WebConfig</td></tr>";
                }

            }
                cn.Close();
           
        }

        /*This method would check for Password Expiry for the Users and Add entry in LEmailBucket
        */
        //private static void CheckUserPasswordExpiration()
        //{
        //    /*Example: User 1 changed password on 30/5  (so Max(LPasswordHistory.CreatedDate = 30/5)
        //     MaxAgeDays (for that CompanyID)= 60 
        //     That means his password expires on 30/7
        //     Say for that OpCo, LpasswordPolicy. ReminderDays = 15. That means, he should start getting reminder from 15/7
        //     0) So nightly job should check if Todays Date > Max for that user(LPasswordHistory.CreatedDate) + MaxAgeDays - LpasswordPolicy. ReminderDays, then expire the User's Password
        //     1)	Send reminders to users who’s password is about to expire: Check in Max(LPasswordHistory.CreatedDate)+ MaxAgeDays for each user. 
        //     If it is greater than GetDate() + LpasswordPolicy. ReminderDays
        //    */
        //    var ErrorFlag = false;
        //    //Connect to database
        //    SqlConnection cn = new SqlConnection();
        //    cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        //    cn.Open();
        //    //System User Id
        //    string qry = "select Id from AspNetUsers where Email='"+ConfigurationManager.AppSettings["SystemUser"] +"'";
        //    SqlCommand cmd = new SqlCommand(qry, cn);
        //    SqlDataAdapter da1 = new SqlDataAdapter(cmd);
        //    DataTable tb1 = new DataTable();
        //    da1.Fill(tb1);
        //    cn.Close();
        //    cn.Open();
        //    var SystemUserId = tb1.Rows[0].Field<string>("Id");

        //    qry = "select Id,Email,GcCompanyId from AspnetUsers where IsActive=1";
        //    cmd = new SqlCommand(qry, cn);
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    DataTable tb = new DataTable();
        //    da.Fill(tb);
        //    cn.Close();
        //    cn.Open();

        //    if (tb.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in tb.Rows)
        //        {
        //            try
        //            {
        //                var ReminderDays = 0;
        //                var MaxAgeDays = 0;
        //                DateTime MaxPasswordCreatedDate;
        //                //get Last Created Date
        //                qry = "select Max(CreatedDateTime) as MaxCreatedDateTime from LPasswordHistory where UserId='" + dr.Field<string>("Id") + "'";
        //                cmd = new SqlCommand(qry, cn);
        //                SqlDataAdapter sda = new SqlDataAdapter(cmd);
        //                DataTable PasswordHistoryTable = new DataTable();
        //                sda.Fill(PasswordHistoryTable);
        //                cn.Close();
        //                cn.Open();
        //                MaxPasswordCreatedDate = PasswordHistoryTable.Rows[0].Field<DateTime>("MaxCreatedDateTime");
        //                //get Password Policy
        //                var CompanyId = dr.Field<dynamic>("GcCompanyId");
        //                qry = "select MaxAgeDays,ReminderDays from LPasswordPolicies where CompanyId='" + CompanyId + "'";
        //                cmd = new SqlCommand(qry, cn);
        //                SqlDataAdapter PolicySDA = new SqlDataAdapter(cmd);
        //                DataTable PasswordPolicyTable = new DataTable();
        //                PolicySDA.Fill(PasswordPolicyTable);
        //                ReminderDays = PasswordPolicyTable.Rows[0].Field<dynamic>("ReminderDays");
        //                MaxAgeDays = PasswordPolicyTable.Rows[0].Field<dynamic>("MaxAgeDays");
        //                cn.Close();
        //                int DaysWhenNotificationStart = MaxAgeDays - ReminderDays;
        //                DateTime NotificationStartDate = MaxPasswordCreatedDate.AddDays(DaysWhenNotificationStart);
        //                /*nightly job should check if Todays Date > Max for that user(LPasswordHistory.CreatedDate) + MaxAgeDays 
        //              Then, Call AD password expire service
        //               */
        //                var ExpiryDate = MaxPasswordCreatedDate.AddDays(MaxAgeDays);
        //                //if (DateTime.UtcNow > ExpiryDate) JS advised to not send 
        //                //{
        //                //    // ExpireUserPassword(dr.Field<dynamic>("Email")); JS directed not to expire password in AD as on 24July2017
        //                //    qry = "Exec dbo.SpLogEmail @ReceiverEmail,null,null,null,@EmailSubject,@EmailBody,1,'Notifier','High',null,'InQueue',null,@SystemUser,@SystemUser,'Test Vodafone Lite SES'";
        //                //    cmd = new SqlCommand(qry, cn);
        //                //    cmd.Parameters.AddWithValue("@ReceiverEmail", dr.Field<dynamic>("Email"));
        //                //    cmd.Parameters.AddWithValue("@SystemUser", SystemUserId);
        //                //    cmd.Parameters.AddWithValue("@EmailSubject", "LITE SOS Password Expiry Notification");
        //                //    cmd.Parameters.AddWithValue("@EmailBody", "Your LITE SOS  Password has expired. Please contact L2 support.");
        //                //    cn.Open();
        //                //    cmd.ExecuteScalar();
        //                //    cn.Close();
        //                //}
        //                if (DateTime.UtcNow > NotificationStartDate)
        //                {
        //                    var NoOfDaysLeftInPasswordExpiry = Math.Round((MaxPasswordCreatedDate.AddDays(MaxAgeDays) - DateTime.UtcNow).TotalDays);
        //                    qry = "Exec dbo.SpLogEmail @ReceiverEmail,null,null,null,@EmailSubject,@EmailBody,1,'Notifier','High',null,'InQueue',null,@SystemUser,@SystemUser,'Test Vodafone Lite SES'";
        //                    cmd = new SqlCommand(qry, cn);
        //                    cmd.Parameters.AddWithValue("@ReceiverEmail", dr.Field<dynamic>("Email"));
        //                    cmd.Parameters.AddWithValue("@SystemUser", SystemUserId);
        //                    cmd.Parameters.AddWithValue("@EmailSubject", "LITE SOS Password Expiry Reminder");
        //                    cmd.Parameters.AddWithValue("@EmailBody", "Your LITE SOS password will expire in " + NoOfDaysLeftInPasswordExpiry + ", please reset your password before that by going to SOS (URL: www.vodafonelite.com) and clicking on your username on top right corner. If you password has already expired, then you will be prompted to enter new password when you try to login. In case you have forgotten you password, you can retrieve it by clicking the (Forgot Password) link under the login section (and follow instructions).");
        //                    cn.Open();
        //                    cmd.ExecuteScalar();
        //                    cn.Close();
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                //continue
        //                ErrorFlag = true;
        //                WriteToFile("Error Generated from Nightly Job(CheckUserPasswordExpiration) :" + ex.StackTrace);
        //                // Add Error Log to database 
        //                qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'CheckUserPasswordExpiration', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
        //                cmd = new SqlCommand(qry, cn);
        //                cmd.ExecuteScalar();
        //                cn.Close();
        //                continue;
        //            }
        //        }
        //    }
        //    /*However if there is any issue due to DB connection, then there will be entry only in ServiceLogs.txt. 
        //     * In any case, if there is any error (ErrorFlag = 1 at the end of processing each method) Email 
        //     * (Subject: Nightly Job Failed - <Method Name>; Body <Method name> Failed) sent to ExceptionEmail (multiple emails can be listed) 
        //     * variable configured in WebConfig*/
        //    if (ErrorFlag)//Send Email Notification via SMTP
        //    {
        //        //Send Email regarding error to L2 Admin in case of any exception generated in method
        //        var Body = "<table border='1'><tr><td>Application Name</td><td>Vodafone_SOS_NightlyJobConsole</td></tr><tr><td>Controller</td><td>CheckUserPasswordExpiration</td></tr><tr><td>Method Name</td><td></td></tr><tr><td>Exception Date/Time(Utc)</td><td>" + DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td>System</td></tr><tr><td>Stack Trace</td><td>(CheckUserPasswordExpiration Failed) sent to ExceptionEmail (multiple emails can be listed) variable configured in WebConfig</td></tr></table>";
        //        SendEmail(ConfigurationManager.AppSettings["ExceptionEmailCc"], ConfigurationManager.AppSettings["ExceptionEmailTo"], "Nightly Job Failed - CheckUserPasswordExpiration", Body);
        //    }

        //}

        //This method returns the Principal Context using Admin user and password.
        private static PrincipalContext getPrincipalContext()
        {
            string AdminUserName = ConfigurationManager.AppSettings["ADUserName"];
            string AdminUserPassword = ConfigurationManager.AppSettings["ADUserPassword"];
            string stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string OUstring = ConfigurationManager.AppSettings["ActiveDirectoryOU"];
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, stringDomainName, OUstring, AdminUserName, AdminUserPassword);
            return ctx;
        }

        //Delete an existing user 
        public static void DeleteUser(string Email)
        {

            PrincipalContext pc = getPrincipalContext();
            UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, Email);
            if (userPrincipal != null)
            {
                userPrincipal.Delete();//deleted successfully
            }
        }

        //Expire the AD password for the user
        public void ExpireUserPassword(string Email)
        {
            PrincipalContext pc = getPrincipalContext();
            UserPrincipal UserPrincipal = UserPrincipal.FindByIdentity(pc, Email);
            if (UserPrincipal != null)
            {
                try
                {
                    UserPrincipal.ExpirePasswordNow();//using the property to expire password
                    UserPrincipal.Enabled = true;
                    UserPrincipal.Save();
                }
                catch (Exception ex)
                {
                    //continue
                }
            }

        }

        public static  bool SendEmail(string ToAddress, string CcAddress, string EmailSubject, string EmailBody)
        {
            try
            {
                //SmtpClient Client = new SmtpClient("mail.megacube.com.au");
                //Client.Credentials = new NetworkCredential("notifier@megacube.com.au", "notifier#123");
                //Client.Port = 25;
                //Client.EnableSsl = false;
                //MailMessage message = new MailMessage();
                //message.From = new MailAddress("notifier@megacube.com.au");
                //string UserId = ConfigurationManager.AppSettings["SMTPUserId"].ToString();
                //string Password = ConfigurationManager.AppSettings["SMTPPassword"].ToString();
                SmtpClient Client = new SmtpClient("email-smtp.eu-west-1.amazonaws.com");
                Client.Credentials = new NetworkCredential(UserId, Password);
                Client.Port = 587;
                Client.EnableSsl = true ;
                MailMessage message = new MailMessage();
                message.From = new MailAddress("sos@vodafonelite.com", "Vodafone Lite Nightly Job");

                //Get receiver Name based on Project Env
                //Connect to database
                SqlConnection cn = new SqlConnection();
                cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                cn.Open();
                var ReceiverEmail = ToAddress;
                try
                {
                    var qry = "select dbo.FnGetSenderEmail('" + ConfigurationManager.AppSettings["ProjectEnviournment"] + "','" + ToAddress + "')";
                    SqlCommand cmd = new SqlCommand(qry, cn);
                    SqlDataAdapter SDA = new SqlDataAdapter(cmd);
                    DataTable ReceiverTable = new DataTable();
                    SDA.Fill(ReceiverTable);
                     ReceiverEmail = ReceiverTable.Rows[0][0].ToString();
                    cn.Close();
                }
                catch { }
                message.To.Add(ReceiverEmail);
                message.Body = EmailBody;
                message.Priority = MailPriority.High;
                
                message.Subject = EmailSubject;
                message.IsBodyHtml = true;
                Client.Send(message);
                WriteToFile("SendEmail completed on {0}");
                return true;
            }
            catch(Exception ex)
            {
                WriteToFile("SendEmail failed due to "+ ex.StackTrace.ToString());
                return false;
            }
        }

        //method to generate audit report on first of every month
        public static void GenerateAuditReport()
        {
            //Connect to database
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            try
            {
                //check if folder exist in bucket if not then create a folder and upload a file
                //check if Today's Date is same as the day when report is generated every month
                var AuditReportRunDay = Convert.ToInt32(ConfigurationManager.AppSettings["AuditReportRunDay"]);

                if (!FolderExistsInAuditBucket(ProjectEnviournment))
                {
                    CreateFolderInAuditBucket(ProjectEnviournment);//Create folder
                }
                if (!FileExistsInS3() && AuditReportRunDay == DateTime.UtcNow.Day)
                {
                    
                    //Query framed by RK to get the Audit data of the users
                    //string qry = "SELECT U.Email as 'User Email',G.GcCompanyName as 'OpCo' ,Roles = STUFF((SELECT ', ' + R.Name FROM AspNetUserRoles t1 inner join AspNetRoles R on t1.RoleId = R.Id WHERE t1.UserId = U.id FOR XML PATH ('')), 1, 1, '') ,convert(varchar,max(L.UalActivityDateTime )) as 'LastLoggedIn' from AspNetUsers U inner join GCompanies G on U.GcCompanyId = G.Id left join GUserActivityLog L on U.Id = L.UalUserId and UalActivity = 'LoggedIn' where U.IsActive = 1 group by U.id, U.Email,G.GcCompanyName order by G.GcCompanyName, U.Email";
                    string qry = "SELECT U.Email as 'User Email',G.GcCompanyName as 'OpCo' ,Roles = STUFF((SELECT ', ' + R.Name FROM AspNetUserRoles t1 inner join AspNetRoles R on t1.RoleId = R.Id WHERE t1.UserId = U.id FOR XML PATH ('')), 1, 1, '') ,convert(varchar,max(L.UalActivityDateTime )) as 'LastLoggedIn' from AspNetUsers U inner join GCompanies G on U.GcCompanyId = G.Id left join GUserActivityLog L on U.Id = L.UalUserId and UalActivity = 'LoggedIn' where U.IsActive = 1 and L.UalActivityDateTime between (GETDATE()-30) and (GETDATE()) group by U.id, U.Email,G.GcCompanyName,L.UalActivityDateTime order by L.UalActivityDateTime desc, G.GcCompanyName, U.Email";
                    SqlCommand cmd = new SqlCommand(qry, cn);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable tb = new DataTable();
                    da.Fill(tb);
                    cn.Close();
                    //Export Datatable to a excel file
                    ExportFileFromDataTable(ProjectEnviournment, tb);
                }
            }
            catch (Exception ex)
            {
               
                string qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'GenerateAuditReport', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
                cn.Close();
            }

        }

        //The below lines of code converts the data returned from api to a datatable
        public static void ExportFileFromDataTable(string ProjectEnviournment, DataTable tb)
        {
            //var tb = new DataTable();
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
            /* 2.We now have S3 bucket to place Audit reports. Lets place SOS audit reports there.
                 FilePath: -sos /< projectenviournment >/ yyyyMM */
            FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/temp/",FileName), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            //upload file now
            UploadToAuditS3(FileName,ProjectEnviournment);
            System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/temp/", FileName));
            xfile.Close();
        }

        public static bool UploadToAuditS3(string FileName, string ProjectEnviournment)
        {
            //FilePath:- sos/<projectenviournment>/yyyyMM/
            //string _awsAccessKey = ConfigurationManager.AppSettings["AuditS3AccessKey"];
            //string _awsSecretKey = ConfigurationManager.AppSettings["AuditS3SecretKey"];

             string _awsAccessKey = GetKeys("audit_accesskey");
             string _awsSecretKey = GetKeys("audit_secretkey");

            string _bucketName = ConfigurationManager.AppSettings["AuditS3BucketName"];
            Stream stream = new MemoryStream(File.ReadAllBytes(Path.Combine(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/temp/", FileName)));
            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    CannedACL = S3CannedACL.Private,//PERMISSION TO FILE PUBLIC ACCESIBLE
                    StorageClass = S3StorageClass.IntelligentTiering,
                    Key = AWSKey,
                    InputStream = stream//SEND THE FILE STREAM
                };

                client.PutObject(request);
            }
            stream.Dispose();
            return true;
        }

        //Create folder in bucket
        public static void CreateFolderInAuditBucket(string ProjectEnviournment)
        {
            //string _awsAccessKey = ConfigurationManager.AppSettings["AuditS3AccessKey"];
            //string _awsSecretKey = ConfigurationManager.AppSettings["AuditS3SecretKey
            string _awsAccessKey = GetKeys("audit_accesskey");
            string _awsSecretKey = GetKeys("audit_secretkey");
            string _bucketName = ConfigurationManager.AppSettings["AuditS3BucketName"];
            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    StorageClass = S3StorageClass.IntelligentTiering,
                    CannedACL = S3CannedACL.Private,
                    Key = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["AuditS3RootFolder"], ProjectEnviournment,""),
                    ContentBody = ProjectEnviournment
                };
                client.PutObject(request);
            }
        }

        //check if folder exist 
        public static bool FolderExistsInAuditBucket(string ProjectEnviournment)
        {
            //string _awsAccessKey = ConfigurationManager.AppSettings["AuditS3AccessKey"];
            //string _awsSecretKey = ConfigurationManager.AppSettings["AuditS3SecretKey"];
            string _awsAccessKey = GetKeys("audit_accesskey");
            string _awsSecretKey = GetKeys("audit_secretkey");
            string _bucketName = ConfigurationManager.AppSettings["AuditS3BucketName"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                S3FileInfo s3FileInfo = new Amazon.S3.IO.S3FileInfo(client, _bucketName, string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["AuditS3RootFolder"], ProjectEnviournment, ""));
                return s3FileInfo.Exists;

            }
        }

        public static bool FileExistsInS3()
        {
            //string _awsAccessKey = ConfigurationManager.AppSettings["AuditS3AccessKey"];
            //string _awsSecretKey = ConfigurationManager.AppSettings["AuditS3SecretKey"];
            string _awsAccessKey = GetKeys("audit_accesskey");
            string _awsSecretKey = GetKeys("audit_secretkey");
            string _bucketName = ConfigurationManager.AppSettings["AuditS3BucketName"];

            using (IAmazonS3 client = new AmazonS3Client(_awsAccessKey, _awsSecretKey))
            {
                S3FileInfo s3FileInfo = new Amazon.S3.IO.S3FileInfo(client, _bucketName, string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["AuditS3RootFolder"], ProjectEnviournment, FileName));
                return s3FileInfo.Exists;

            }

        }

        private static void CheckDoulblePatentPayee()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            try
            {
                //Connect to database
                string ExceptionEmailCc = ConfigurationManager.AppSettings["ExceptionEmailCc "];
                string ExceptionEmailTo = ConfigurationManager.AppSettings["ExceptionEmailTo"];
                string ExceptionEmailSubject = ConfigurationManager.AppSettings["ExceptionEmailSubject"];

               
                Boolean isProblemInLPParentPayee = false;
                Boolean isEmailBucketStuck = false;

                //Check for payees whose Effective End Date is Current Date. Then make those Payees InActive in the System
                string qry = "select LppPayeeId,LppEffectiveEndDate,count(LppPayeeId) as DataCount from LPayeeParents  group by LppPayeeId,LppEffectiveEndDate having count(LppPayeeId) > 1 order by LppPayeeId";
                SqlCommand cmd = new SqlCommand(qry, cn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable tb = new DataTable();
                da.Fill(tb);

                if (tb.Rows.Count > 0)//check if there are any payee records to be made InActive
                {

                    //Loop through the list and make payees inactive one by one
                    foreach (DataRow dr in tb.Rows)
                    {
                        //if (dr.Field<DateTime>("LppEffectiveEndDate") == null && dr.Field<int>("DataCount") > 1)
                        //{
                        //    isProblemInLPParentPayee = true;
                        //}
                        if (string.IsNullOrEmpty(Convert.ToString(dr["LppEffectiveEndDate"])) && dr.Field<int>("DataCount") > 1)
                        {

                            isProblemInLPParentPayee = true;
                        }

                    }
                }

                string EmailQuery = "select Count(*) as EmailCount from LEmailBucket where LebStatus = 'InQueue'";
                SqlCommand cmdEmail = new SqlCommand(EmailQuery, cn);
                SqlDataAdapter da1 = new SqlDataAdapter(cmdEmail);
                DataTable tb1 = new DataTable();
                da1.Fill(tb1);
                cn.Close();

                int EmailCount = tb1.Rows[0].Field<int>("EmailCount");
                if (EmailCount > 100)
                {
                    //isEmailBucketStuck = true;
                    IsMailSend = true;
                    body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>CheckDoulblePatentPayee</td><td>" + DateTime.UtcNow.ToString() + "</td><td>it seems Email bucket got stuck.</td></tr>";
                }
                if (isProblemInLPParentPayee == true)
                {
                    //isEmailBucketStuck = true;
                    IsMailSend = true;
                    body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>CheckDoulblePatentPayee</td><td>" + DateTime.UtcNow.ToString() + "</td><td>There are some payee with two or more parents with EndDate as null, please check the same using following query: </td><td>" + qry + "</td></tr>";
                }
            }
            catch(Exception ex)
            {
                
                string qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'CheckDoulblePatentPayee', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                SqlCommand cmd = new SqlCommand(qry, cn);                
                cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
                cn.Close();
            }

            //if (isProblemInLPParentPayee == true && isEmailBucketStuck == true)
            //{
            //    SendEmail(ExceptionEmailTo, ExceptionEmailCc, ExceptionEmailSubject, "There are some payee with two or more EndDate as null and Email bucket got stuck.");
            //}
            //else if (isProblemInLPParentPayee == true && isEmailBucketStuck == false)
            //{
            //    SendEmail(ExceptionEmailTo, ExceptionEmailCc, ExceptionEmailSubject, "There are some payee with two or more EndDate as null ");
            //}
            //else if (isProblemInLPParentPayee == false && isEmailBucketStuck == true)
            //{
            //    SendEmail(ExceptionEmailTo, ExceptionEmailCc, ExceptionEmailSubject, "Email bucket got stuck.");
            //}
        }

        private static void UpdateMissingCliamsPortfolio()
        {


            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            try
            {
                //Check for payees whose Effective End Date is less than equal to Current Date. Then make those Payees InActive in the System
                string qry = " insert into MEntityPortfolios (MepPortfolioId,MepEntityType,MepEntityId)select M.MepPortfolioId,'LClaims',C.id from LClaims C inner join LPayees P on C.LcPayeeId = P.Id inner join MEntityPortfolios M on M.MepEntityId = P.id and M.MepEntityType = 'LPayees' where C.id not in (select MepEntityId from MEntityPortfolios where MepEntityType = 'LClaims' )";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd.ExecuteNonQuery();

                cn.Close();
            }
            catch (Exception ex)
            {
                IsMailSend = true;
                body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>UpdateMissingCliamsPortfolio</td><td>" + DateTime.UtcNow.ToString() + "</td><td>" + ex.Message + "</td></tr>";
                
                string qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'UpdateMissingCliamsPortfolio', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
                cn.Close();
            }

        }

        private static void UpdateMissingPayeesPortfolio()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            try
            {
               
                SqlCommand command = new SqlCommand("USPUpdateMissingPortfolioForPayees", cn);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();

                cn.Close();
            }
            catch (Exception ex)
            {
                IsMailSend = true;
                body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>UpdateMissingPayeesPortfolio</td><td>" + DateTime.UtcNow.ToString() + "</td><td>" + ex.Message + "</td></tr>";
                
                string qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'UpdateMissingPayeesPortfolio', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
                cn.Close();
            }


        }

        private static void DeleteBucketPayeeOlderthanMonth()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            try
            {
               
                SqlCommand command = new SqlCommand("USPDeleteBucketPayeeOlderthanMonth", cn);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();

                cn.Close();
            }

            catch (Exception ex)
            {
                IsMailSend = true;
                body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>DeleteBucketPayeeOlderthanMonth</td><td>" + DateTime.UtcNow.ToString() + "</td><td>" + ex.Message + "</td></tr>";
                
                string qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'DeleteBucketPayeeOlderthanMonth', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
                cn.Close();

            }
        }
        private static void BlockNotificationForDummyUsers()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            try
            {
                SqlCommand command = new SqlCommand("USPBlockNotificationDummyUsers", cn);
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
                cn.Close();
            }
            catch (Exception ex)
            {
                IsMailSend = true;
                body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>BlockNotificationForDummyUsers</td><td>" + DateTime.UtcNow.ToString() + "</td><td>" + ex.Message + "</td></tr>";
                string qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'BlockNotificationForDummyUsers', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
                cn.Close();

            }

        }

        private static void AcceptLobbyUsers()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            try
            {
                
                SqlCommand command = new SqlCommand("USPAcceptLobbyUsers", cn);
                command.Parameters.AddWithValue("@LobbyId",(object)System.DBNull.Value);//sending null parameter
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();

                cn.Close();
            }
            catch(Exception ex)
            {
                IsMailSend = true;
                body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>AcceptLobbyUsers</td><td>" + DateTime.UtcNow.ToString() + "</td><td>"+ex.Message+"</td></tr>";
                
                string qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'AcceptLobbyUsers', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
                cn.Close();

            }

        }

        private static void CheckDuplicatePayees()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            try
            {
                //duplicate userid
                string query = "select count(*) as UserIdCount from  LPayees group by LpUserId having count(*)> 1";
                SqlCommand cmd1 = new SqlCommand(query, cn);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                DataTable tb1 = new DataTable();
                da1.Fill(tb1);
                cn.Close();
                if (tb1.Rows.Count > 0) { 
                int UserIdCount = tb1.Rows[0].Field<int>("UserIdCount");
                    if (UserIdCount > 1)
                    {
                        IsMailSend = true;
                        body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>CheckDuplicatePayees</td><td>" + DateTime.UtcNow.ToString() + "</td><td>Duplicate Userid found in LPayees." + query + "</td></tr>";
                    }
                }
                //
                query = "select count(*) as PayeeCodeCount from LPayees group by LpPayeeCode having count(*)> 1";
                cmd1 = new SqlCommand(query, cn);
                da1 = new SqlDataAdapter(cmd1);
                DataTable tb2 = new DataTable();
                da1.Fill(tb2);
                cn.Close();
                if (tb2.Rows.Count > 0)
                {
                    int PayeeCodeCount = tb2.Rows[0].Field<int>("PayeeCodeCount");
                    if (PayeeCodeCount > 1)
                    {
                        IsMailSend = true;
                        body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>CheckDuplicatePayees</td><td>" + DateTime.UtcNow.ToString() + "</td><td>Duplicate PayeeCode found in LPayees." + query + "</td></tr>";
                    }
                }
                query = "select count(*) as EmailCount from LPayees group by LpEmail having count(*)> 1";
                cmd1 = new SqlCommand(query, cn);
                da1 = new SqlDataAdapter(cmd1);
                DataTable tb3 = new DataTable();
                da1.Fill(tb3);
                cn.Close();
                if (tb1.Rows.Count > 0)
                {
                    int EmailCount = tb3.Rows[0].Field<int>("EmailCount");
                    if (EmailCount > 1)
                    {
                        IsMailSend = true;
                        body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>CheckDuplicatePayees</td><td>" + DateTime.UtcNow.ToString() + "</td><td>Duplicate Email found in LPayees." + query + "</td></tr>";
                    }
                }
                body = body + "<tr><td>To fix the records, run the following query." +
                    " Update LPayees set LpCreateLogin=0,WFStatus='InActive',LpEffectiveEndDate=GETDATE(),WFUpdatedDateTime=GETDATE() "
                        +" ,WFComments = wfcomments + ' Manually terminated from OpCo <OldOpCo> " +
                        "since payee has moved to <NewOpCo>' where LpEmail =<email> and WFCompanyId = <OpCo> ";
                body = body + "Along with the query to fix the records, check AspNetUser for IsActive and GCCompanyId. ASPNetRoles for correct roles and remove old portfolios.</td></tr>";
            }
            catch (Exception ex)
            {
                IsMailSend = true;
                body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>BlockNotificationForDummyUsers</td><td>" + DateTime.UtcNow.ToString() + "</td><td>" + ex.Message + "</td></tr>";
                string qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'BlockNotificationForDummyUsers', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
                cn.Close();
            }

        }
        private static void DataPurge()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            int KeepDataForYears = Convert.ToInt32(ConfigurationManager.AppSettings["KeepDataForYears"].ToString());
            var PurgeDate = DateTime.UtcNow.AddYears(-KeepDataForYears).ToString("yyyy-MM-dd");
            try
            {

                SqlCommand command = new SqlCommand("SpPurgeData", cn);
                command.Parameters.AddWithValue("@DatePurge", PurgeDate);//sending null parameter
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();

                cn.Close();
            }
            catch (Exception ex)
            {
                IsMailSend = true;
                body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>DataPurge</td><td>" + DateTime.UtcNow.ToString() + "</td><td>" + ex.Message + "</td></tr>";

                WriteToFile("Error Generated from Nightly Job(DataPurge) :" + ex.StackTrace);
                // Add Error Log to database 
                string qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'DataPurge', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
                cn.Close();

            }

        }


        private static void DataPurgeOld()
        {
            //Connect to database
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            int KeepDataForYears = Convert.ToInt32(ConfigurationManager.AppSettings["KeepDataForYears"].ToString());
            var PurgeDate = DateTime.UtcNow.AddYears(-KeepDataForYears).ToString("yyyy-MM-dd"); 

            try
            {
                #region OnlyPayee-PurgeData
                string qry = "Select Id,LpUserId,LpEmail,wfCompanyId from LPayees Lp where WFUpdatedDateTime <='" + PurgeDate + "' and WFUpdatedDateTime is not null  and WFStatus not in('Completed','Suspended','Saved','InProgress')  and NOT EXISTS (select * from LUsers lu where lu.LuEmail = Lp.LpEmail and WFStatus  in ('Completed','Suspended','Saved','InProgress')) ";
                SqlCommand cmd = new SqlCommand(qry, cn);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable tb = new DataTable();
                da.Fill(tb);
                if (tb.Rows.Count > 0)//check if there are any payee records to be made InActive
                {
                    foreach (DataRow dr in tb.Rows)
                    {
                        //delete payee portfolios
                        qry = "delete from MEntityPortfolios where MepEntityType='LPayees' and MepEntityId=" + dr.Field<int>("Id");
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        //delete Roles
                        // qry = "delete AUR from AspnetUserRoles AUR JOIN AspNetRoles AR  on AR.Id = AUR.RoleId where Aur.UserId = '" + dr.Field<string>("LpUserId") + "' and AR.Name = 'Payee'";
                        qry = "delete from AspnetUserRoles where UserId='" + dr.Field<string>("LpUserId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        //delete PasswordHistory
                        qry = "delete from LPasswordHistory where UserId = '" + dr.Field<string>("LpUserId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        //delete SecurityQuestions
                        qry = "delete from  MAspnetUsersGSecurityQuestions  where MAuqsqUserId = '" + dr.Field<string>("LpUserId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        qry = "select GcCode from GCompanies where id = '" + dr.Field<int>("wfCompanyId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        string returnValue = (string)cmd.ExecuteScalar();

                        //cmd.CommandText = "select GcCode from GCompanies where id = '" + dr.Field<string>("LpCompanyId") + "'";
                        //int returnValue = (int)mySqlCommand.ExecuteScalar();

                        qry = "delete from XSchema" + returnValue + ".XReportUsers where XUserEmailID= '" + dr.Field<string>("LpEmail") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteNonQuery();

                    }

                }
                #endregion

                #region OnlyUser-PurgeData
                //qry = "Select Id,LuUserId,LuEmail,LuCompanyId from LUsers where LuUpdatedDateTime <='" + PurgeDate + "'  and WFStatus = 'InActive' and LuUserId not in (select LpUserId from LPayees where LpUserId is not null) ";
                qry = "Select Id,LuUserId,LuEmail,wfCompanyId from LUsers Lu where WFUpdatedDateTime <='" + PurgeDate + "' and Lu.WFStatus not in('Completed','Suspended','Saved','InProgress') and NOT EXISTS (select * from LPayees lp where lp.LpEmail = Lu.LuEmail and Lp.WFStatus  in ('Completed','Suspended','Saved','InProgress'))";
                cmd = new SqlCommand(qry, cn);

                SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                DataTable tb1 = new DataTable();
                da1.Fill(tb1);
                if (tb1.Rows.Count > 0)//check if there are any payee records to be made InActive
                {
                    foreach (DataRow dr in tb1.Rows)
                    {
                        //delete payee portfolios
                        qry = "delete from MEntityPortfolios where MepEntityType='LUsers' and MepEntityId=" + dr.Field<int>("Id");
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        //delete Roles
                        qry = "delete from AspnetUserRoles where UserId='" + dr.Field<string>("LuUserId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        //delete PasswordHistory
                        qry = "delete from LPasswordHistory where UserId = '" + dr.Field<string>("LuUserId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        //delete SecurityQuestions
                        qry = "delete from  MAspnetUsersGSecurityQuestions  where MAuqsqUserId = '" + dr.Field<string>("LuUserId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        qry = "select GcCode from GCompanies where id = '" + dr.Field<int>("wfCompanyId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        string returnValue = (string)cmd.ExecuteScalar();

                        //cmd.CommandText = "select GcCode from GCompanies where id = '" + dr.Field<string>("LpCompanyId") + "'";
                        //int returnValue = (int)mySqlCommand.ExecuteScalar();

                        qry = "delete from XSchema" + returnValue + ".XReportUsers where XUserEmailID=  '" + dr.Field<string>("LuEmail") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                    }

                }
                #endregion

                #region BothUserPayee-PurgeData
                //qry = "Select lu.Id,lu.LuUserId,lu.LuEmail,lu.LuCompanyId from LUsers lu JOIN LPayees lp ON lu.LuUserId	= lp.LpUserId where ((Lu.WFStatus = 'InActive' and lu.LuUpdatedDateTime <='" + PurgeDate + "' )  or (Lp.WFStatus = 'InActive' and lp.LpUpdatedDateTime <='" + TwoYearBackDate + "' ))";
                qry = "Select lu.Id as UserID,Lp.id as PayeeID,lu.LuUserId,lu.LuEmail,lu.wfCompanyId from LUsers lu JOIN LPayees lp ON lu.LuEmail	= lp.LpEmail where ((Lu.WFStatus not in('Completed','Suspended','Saved','InProgress') and lu.WFUpdatedDateTime <='" + PurgeDate + "' )  and (Lp.WFStatus  not in('Completed','Suspended','Saved','InProgress') and lp.WFUpdatedDateTime <='" + PurgeDate + "' ))";
                cmd = new SqlCommand(qry, cn);

                SqlDataAdapter da2 = new SqlDataAdapter(cmd);
                DataTable tb2 = new DataTable();
                da2.Fill(tb2);
                if (tb2.Rows.Count > 0)
                {
                    foreach (DataRow dr in tb2.Rows)
                    {
                        //delete payee portfolios
                        qry = "delete from MEntityPortfolios where MepEntityType='LPayees' and MepEntityId=" + dr.Field<int>("PayeeID");
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        //delete User portfolios
                        qry = "delete from MEntityPortfolios where MepEntityType='LUsers' and MepEntityId=" + dr.Field<int>("UserID");
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        //delete Roles
                        qry = "delete from AspnetUserRoles where UserId='" + dr.Field<string>("LuUserId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        //delete PasswordHistory
                        qry = "delete from LPasswordHistory where UserId = '" + dr.Field<string>("LuUserId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        //delete SecurityQuestions
                        qry = "delete from  MAspnetUsersGSecurityQuestions  where MAuqsqUserId = '" + dr.Field<string>("LuUserId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                        qry = "select GcCode from GCompanies where id = '" + dr.Field<int>("wfCompanyId") + "'";
                        cmd = new SqlCommand(qry, cn);
                        string returnValue = (string)cmd.ExecuteScalar();

                        qry = "delete from XSchema" + returnValue + ".XReportUsers where XUserEmailID= '" + dr.Field<string>("LuEmail") + "'";
                        cmd = new SqlCommand(qry, cn);
                        cmd.ExecuteScalar();

                    }

                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteToFile("Error Generated from Nightly Job(DataPurge) :" + ex.StackTrace);
                // Add Error Log to database 
                string qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'DataPurge', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                 SqlCommand cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
                cn.Close();
            }
            finally
            {
                cn.Close();
            }
            
        }
        
        private static void CloseTickets()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            try
            {
                //update tickets which are older than 60 days
                string qry = "Update LSupportTickets set LstStatus= 'Closed' where LstLastUpdatedDateTime <= DATEADD(day, -60, GETDATE()) " +
                    " and lststatus IN ('New','WIP') AND LstStageId  NOT in (select Id from RTicketStages where RtsName in ('Add to Requirements','Booked for release') ) ";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
               

            }
            catch (Exception ex)
            {
                IsMailSend = true;
                body = body + "<tr><td>Vodafone_SOS_NightlyJobConsole</td><td>CloseTickets</td><td>" + DateTime.UtcNow.ToString() + "</td><td>" + ex.Message + "</td></tr>";
                string qry = "Exec SpLogError 'Vodafone_SOS_NightlyJobConsole','Nightly Job', 'CloseTickets', '" + ex.StackTrace + "', 'System', 'Type1', null, '', 'L2 Admin', null, null, 'New' ";
                SqlCommand cmd = new SqlCommand(qry, cn);
                cmd = new SqlCommand(qry, cn);
                cmd.ExecuteScalar();
                cn.Close();
            }
            cn.Close();
        }

        private static string GetKeys(string Key)
        {
            string keyValue = string.Empty;
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();
            try
            {
                string qry = "select top 1 GkvValue as GkvValue from GKeyValues  where GkvKey = '" + Key +"'" ;
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

                            keyValue= Convert.ToString(dr["GkvValue"]);
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
