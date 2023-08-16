using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Mvc;
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;

namespace Vodafone_SOS_WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
    //Class to handle unhandled error
    //public class TraceSourceExceptionLogger:ExceptionLogger
    //{
    //    private readonly TraceSource _traceSource;
    //    private VodafoneSOSLiteEntities db = new VodafoneSOSLiteEntities();
    //    public TraceSourceExceptionLogger(TraceSource traceSource)
    //    {
    //        _traceSource = traceSource;
    //    }
    //    public override void Log(ExceptionLoggerContext context)
    //    {
    //        var Controller = context.Request.RequestUri.AbsolutePath;
    //        string actionName = context.Request.Method.Method;
    //        var statusMessage = context.Exception.Message+" "+context.Exception.StackTrace;
    //        // Add Error Log to database Table
    //        var model = new GErrorLog { GelUserName = System.Web.HttpContext.Current.Session["UserName"].ToString(), GelController = Controller, GelMethod = actionName, GelErrorDateTime = DateTime.UtcNow, GelStackTrace = statusMessage.ToString(), GelSourceProject = "[Vodafone-SOS WebApi]" };
    //        db.GErrorLogs.Add(model);
    //        db.SaveChanges();
    //        _traceSource.TraceEvent(TraceEventType.Error,1,"An error has occurred");
    //        //Send mail to  l2 admin
    //        //var Subject = ConfigurationManager.AppSettings["ExceptionEmailSubject"];
    //        //string Body;
    //        //var UserName = (System.Web.HttpContext.Current.Session["UserName"] != null) ? System.Web.HttpContext.Current.Session["UserName"].ToString() : "";
    //        //Body = "<table border='1'><tr><td>Application Name</td><td>" + ConfigurationManager.AppSettings["ExceptionEmailProjectName"] + "</td></tr><tr><td>Controller</td><td>" + Controller + "</td></tr><tr><td>Method Name</td><td>" + actionName + "</td></tr><tr><td>Exception Date/Time(Utc)</td><td>" + DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td>" + UserName + "</td></tr><tr><td>Stack Trace</td><td>" + statusMessage + "</td></tr></table>";

    //        //Globals.SendEmail(Body, Subject, ConfigurationManager.AppSettings["ExceptionEmailTo"], ConfigurationManager.AppSettings["ExceptionEmailCc"]);

    //    }

    //}

        //This attribute class is defined to log the unhandled errors in project to GError Logs
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        public override void OnException(HttpActionExecutedContext Context)
        {
            var UserName = "";//to be added by reading params and share with rely team
            var ErrorDesc = "";
            //if (System.Web.HttpContext.Current.Session.Keys.Count>0)
           //The belowline will add Parameter list passed in Api call to description column of Error Log
            var Desc = Context.Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
            //populate GelUserName column by reading its value from  (Context.Request.GetQueryNameValuePairs())
         //  var UserNameFi = Context.Request.GetQueryNameValuePairs().Where(p => p.Key.Equals("UserName", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (Desc.Count()>0)
            ErrorDesc = string.Join(",", Desc);
            //Gets the Complete Url of Api call
            string UrlString = Convert.ToString(Context.Request.RequestUri.AbsolutePath);
            //This array will provide controller name at second and action name at 3 rd index position (like localhost:81/api/<controller>/<action>)
            string[] UrlParams = Context.Request.RequestUri.AbsolutePath.Split('/');
            try
            {
                string exceptionMessage = Context.Exception.Message + Environment.NewLine + Context.Exception.StackTrace;
                if (Context.Exception.InnerException != null)
                {
                    exceptionMessage += Environment.NewLine + Context.Exception.InnerException.Message + Environment.NewLine + Context.Exception.InnerException.StackTrace;

                    if (Context.Exception.InnerException.InnerException != null)
                    {
                        exceptionMessage += Environment.NewLine + Context.Exception.InnerException.InnerException.Message + Environment.NewLine + Context.Exception.InnerException.InnerException.StackTrace;
                    }
                }
                //var actionName= Context.Request.Method.Method;
                // var Controller = Context.Request.RequestUri.AbsolutePath;
                // Add Error Log to database 
                Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", UrlParams[2], UrlParams[3],exceptionMessage,UserName,"Type1", ErrorDesc,"","L2 Admin",null,null,"New");
                db.SaveChanges();
                //Add entry in email bucket
                var Body = "<table border='1'><tr><td>Application Name</td><td>" + ConfigurationManager.AppSettings["ExceptionEmailProjectName"] +
                    "</td></tr><tr><td>Controller</td><td>" + UrlParams[2] + "</td></tr><tr><td>Method Name</td><td>" + UrlParams[3] + "</td></tr><tr><td>Exception Date/Time(Utc)</td><td>" +
                    DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td>" + UserName + "</td></tr><tr><td>Stack Trace</td><td>" + exceptionMessage + "</td></tr></table>";
                //log email through SMTP details in web.config in email bucket
                //SS This section is throwing error as Created By and UpdatedByuserId is not supplied. Commented on 9 June
                Globals.ExecuteSPLogEmail(ConfigurationManager.AppSettings["ExceptionEmailTo"], ConfigurationManager.AppSettings["ExceptionEmailCc"], null, null,
                   ConfigurationManager.AppSettings["ExceptionEmailSubject"],
                   Body, true, "Error", "Normal", null, "InQueue", null, null, null, "Test Vodafone Lite SES", null, null, null);
            }
            catch (Exception ex)
            {
                //Send Email regarding error to L2 Admin in case of any exception generated in filter config
                var Body = "<table border='1'><tr><td>Application Name</td><td>" + ConfigurationManager.AppSettings["ExceptionEmailProjectName"] + "</td></tr><tr><td>Controller</td><td>" + UrlParams[2] + "</td></tr><tr><td>Method Name</td><td>" + UrlParams[3] + "</td></tr><tr><td>Exception Date/Time(Utc)</td><td>" + DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td>" + UserName + "</td></tr><tr><td>Stack Trace</td><td>" + ex.Message+Environment.NewLine+ex.StackTrace + "</td></tr></table>";
                Globals.SendEmail(ConfigurationManager.AppSettings["ExceptionEmailCc"], ConfigurationManager.AppSettings["ExceptionEmailTo"], ConfigurationManager.AppSettings["ExceptionEmailSubject"], Body+Environment.NewLine+ErrorDesc, "QA");//Need to disscuss from Where CompanyCode will be derived
            }
           // Globals.SendEmail("ssharma@megacube.com.au", "shubhamshrm97@yahoo.com", "Support Ticket Create", Context.Exception.InnerException.Message + Environment.NewLine + Context.Exception.InnerException.StackTrace, "QA");
            throw new HttpResponseException(Context.Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type1, "Oops! Something went wrong. The issue has been reported and will be resolved soon."));

        }
    }

}

