using System;
using System.ServiceProcess;

using System.IO;
using System.Threading;
using System.Configuration;

using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using Microsoft.Owin.Security;
using System.DirectoryServices.AccountManagement;
using System.Activities.Statements;

namespace Vodafone_SOS_NightlyJob
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.ScheduleService();
        }

        protected override void OnStop()
        {
            this.Schedular.Dispose();
        }
        private Timer Schedular;
        private static readonly IAuthenticationManager authenticationManager;
        //This service will be called Daily at 23:59 to check for Effective Dating in Payees
        public void ScheduleService()
        {
                try
                {
                    Schedular = new Timer(new TimerCallback(SchedularCallback));
                    //string mode = ConfigurationManager.AppSettings["Mode"].ToUpper();
                    string mode = "DAILY";
                    //Set the Default Time.
                    DateTime scheduledTime = DateTime.MinValue;

                    if (mode == "DAILY")
                    {
                        //Get the Scheduled Time from AppSettings.
                        scheduledTime = DateTime.Parse(ConfigurationManager.AppSettings["ScheduledTime"]);
                        if (DateTime.Now > scheduledTime)
                        {
                            //If Scheduled Time is passed set Schedule for the next day.
                            scheduledTime = scheduledTime.AddDays(1);
                        }
                     else 
                    {
                        CheckPayeeEffectiveDating();
                        CheckUserPasswordExpiration();
                    }
                    }

                    TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                    //Get the difference in Minutes between the Scheduled and Current Time.
                    int dueTime = Math.Abs(Convert.ToInt32(timeSpan.TotalMilliseconds));

                    //Change the Timer's Due Time.
                    Schedular.Change(dueTime, Timeout.Infinite);
                }
                catch (Exception ex)
                {
                    //Stop the Windows Service.
                    using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController("SimpleService"))
                    {
                        serviceController.Stop();
                    }
                }
        }

        private void SchedularCallback(object e)
        {
            this.ScheduleService();
        }

        private void CheckPayeeEffectiveDating()
        {
            //Connect to database
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();

            var CurrentDate = DateTime.UtcNow.AddMinutes(1).ToString("yyyy-MM-dd");
            var FutureDate=DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd");
            //Check for payees whose Effective End Date is Current Date. Then make those Payees InActive in the System
            string qry = "Select Id,LpUserId,LpEmail from LPayees where LpEffectiveEndDate<='" + CurrentDate + "' and LpEffectiveEndDate is not null and WFStatus='Completed'";
            SqlCommand cmd = new SqlCommand(qry, cn);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable tb = new DataTable();
            da.Fill(tb);

            if (tb.Rows.Count > 0)//check if there are any payee records to be made InActive
            {

                //Loop through the list and make payees inactive one by one
                foreach (DataRow dr in tb.Rows) {
                    qry = "update LPayees set WFStatus='InActive' where Id=" + dr.Field<int>("Id");
                    cmd = new SqlCommand(qry, cn);
                    cmd.ExecuteScalar();
                    qry = "update AspnetUsers set IsActive=0 where Id='" + dr.Field<string>("LpUserId")+"'";
                    cmd = new SqlCommand(qry, cn);
                    cmd.ExecuteScalar();
                    DeleteUser(dr.Field<string>("LpEmail"));//Delete AD account
                }
        }
            cn.Close();
        }

        /*This method would check for Password Expiry for the Users and Add entry in LEmailBucket
        */
        private void CheckUserPasswordExpiration()
        {
            /*Example: User 1 changed password on 30/5  (so Max(LPasswordHistory.CreatedDate = 30/5)
             MaxAgeDays (for that CompanyID)= 60 
             That means his password expires on 30/7
             Say for that OpCo, LpasswordPolicy. ReminderDays = 15. That means, he should start getting reminder from 15/7
             So nightly job should check if Todays Date > Max for that user(LPasswordHistory.CreatedDate) + MaxAgeDays - LpasswordPolicy. ReminderDays
             1)	Send reminders to users who’s password is about to expire: Check in Max(LPasswordHistory.CreatedDate)+ MaxAgeDays for each user. 
             If it is greater than GetDate() + LpasswordPolicy. ReminderDays
            */
            //Connect to database
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cn.Open();

            string qry = "select Id from AspNetUsers where Email='sos@vodafone.com'";
            SqlCommand cmd = new SqlCommand(qry, cn);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd);
            DataTable tb1 = new DataTable();
            da1.Fill(tb1);
            cn.Close();
            cn.Open();
            var SystemUserId = tb1.Rows[0].Field<string>("Id");

            qry = "select Id,Email,GcCompanyId from AspnetUsers where IsActive=1";
            cmd = new SqlCommand(qry, cn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable tb = new DataTable();
            da.Fill(tb);
            cn.Close();
            cn.Open();

            if (tb.Rows.Count > 0)
            {
                foreach (DataRow dr in tb.Rows)
                {
                    try {
                        var ReminderDays = 0;
                        var MaxAgeDays = 0;
                        DateTime MaxPasswordCreatedDate;
                        //get Last Created Date
                        qry = "select Max(CreatedDateTime) as MaxCreatedDateTime from LPasswordHistory where UserId='" + dr.Field<string>("Id") + "'";
                        cmd = new SqlCommand(qry, cn);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable PasswordHistoryTable = new DataTable();
                        sda.Fill(PasswordHistoryTable);
                        cn.Close();
                        cn.Open();
                        MaxPasswordCreatedDate = PasswordHistoryTable.Rows[0].Field<DateTime>("MaxCreatedDateTime");
                        //get Password Policy
                        var CompanyId = dr.Field<dynamic>("GcCompanyId");
                        qry = "select MaxAgeDays,ReminderDays from LPasswordPolicies where CompanyId='" +CompanyId  + "'";
                        cmd = new SqlCommand(qry, cn);
                        SqlDataAdapter PolicySDA = new SqlDataAdapter(cmd);
                        DataTable PasswordPolicyTable = new DataTable();
                        PolicySDA.Fill(PasswordPolicyTable);
                        ReminderDays = PasswordPolicyTable.Rows[0].Field<dynamic>("ReminderDays");
                        MaxAgeDays = PasswordPolicyTable.Rows[0].Field<dynamic>("MaxAgeDays");
                        cn.Close();
                        int DaysWhenNotificationStart = MaxAgeDays - ReminderDays;
                        DateTime NotificationStartDate = MaxPasswordCreatedDate.AddDays(DaysWhenNotificationStart);
                        /*nightly job should check if Todays Date > Max for that user(LPasswordHistory.CreatedDate) + MaxAgeDays 
                      Then, Call AD password expire service
                       */
                        var ExpiryDate = MaxPasswordCreatedDate.AddDays(MaxAgeDays);
                        if (DateTime.UtcNow > ExpiryDate)
                        {
                           // ExpireUserPassword(dr.Field<dynamic>("Email")); JS directed not to expire password in AD 24July2017
                            qry = "Exec dbo.SpLogEmail @ReceiverEmail,null,null,null,@EmailSubject,@EmailBody,1,'Notifier','High',null,'InQueue',null,@SystemUser,@SystemUser,'Test Vodafone Lite SES'";
                            cmd = new SqlCommand(qry, cn);
                            cmd.Parameters.AddWithValue("@ReceiverEmail", dr.Field<dynamic>("Email"));
                            cmd.Parameters.AddWithValue("@SystemUser", SystemUserId);
                            cmd.Parameters.AddWithValue("@EmailSubject", "LITE SOS Password Expiry Notification");
                            cmd.Parameters.AddWithValue("@EmailBody", "Your SOS LITE Password has expired. Please contact L2 support.");
                            cn.Open();
                            cmd.ExecuteScalar();
                            cn.Close();
                        }
                        else if (DateTime.UtcNow > NotificationStartDate)
                        {
                            var NoOfDaysLeftInPasswordExpiry = Math.Round((MaxPasswordCreatedDate.AddDays(MaxAgeDays) - DateTime.UtcNow).TotalDays);
                            qry = "Exec dbo.SpLogEmail @ReceiverEmail,null,null,null,@EmailSubject,@EmailBody,1,'Notifier','High',null,'InQueue',null,@SystemUser,@SystemUser,'Test Vodafone Lite SES'";
                            cmd = new SqlCommand(qry, cn);
                            cmd.Parameters.AddWithValue("@ReceiverEmail", dr.Field<dynamic>("Email"));
                            cmd.Parameters.AddWithValue("@SystemUser", SystemUserId);
                            cmd.Parameters.AddWithValue("@EmailSubject", "LITE SOS Password Expiry Notification");
                            cmd.Parameters.AddWithValue("@EmailBody", "Your SOS LITE Password will expire in " + NoOfDaysLeftInPasswordExpiry + " days. Please reset your Password before that.");
                            cn.Open();
                            cmd.ExecuteScalar();
                            cn.Close();
                        }
                       
                    }
                    catch(Exception ex)
                    {
                        //continue
                        continue;
                    }
                    }
            }
           
        }

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
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc,Email);
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



    }
}
