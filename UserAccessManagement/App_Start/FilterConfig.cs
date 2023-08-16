using System;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using UserAccessManagement.Models;
using WIAM_SOS.Utilities;

namespace WIAM_SOS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }

    //This attribute class is defined to log the unhandled errors in project to GError Logs
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        private SosDbEntities db = new SosDbEntities();
        public override void OnException(HttpActionExecutedContext Context)
        {
            string exceptionMessage = Context.Exception.Message + Environment.NewLine + Context.Exception.StackTrace;
            if (Context.Exception.InnerException != null)
            {
                exceptionMessage += Environment.NewLine + Context.Exception.InnerException.Message + Environment.NewLine + Context.Exception.InnerException.StackTrace; ;

                if (Context.Exception.InnerException.InnerException != null)
                {
                    exceptionMessage += Environment.NewLine + Context.Exception.InnerException.InnerException.Message + Environment.NewLine + Context.Exception.InnerException.InnerException.StackTrace;
                }
            }
            //string[] s = Context.Request.RequestUri.AbsolutePath.Split('/');//This array will provide controller name at second and action name at 3rd index position
            int errorid = 0;
            try
            {
                ObjectParameter Result = new ObjectParameter("Result", typeof(string)); //return parameter is declared
                db.SpLogErrorNew("WIAM", "ManageAccess", "webservice", exceptionMessage, "WIAM", "Type1", exceptionMessage, "resolution", "L2 Admin", "field", 0, "New", Result).FirstOrDefault();
                db.SaveChanges();
                errorid = (int)Result.Value; //getting value of output parameter
            }
            catch (Exception ex)
            {
                //if something went wrong while adding error in db,generate email
                var Body = "<table border='1'><tr><td>Application Name</td><td>" + ConfigurationManager.AppSettings["ExceptionEmailProjectName"] 
                    + "</td></tr><tr><td>Controller</td><td>" + "ManageAccess" + "</td></tr><tr><td>Method Name</td><td>" + "webservice" 
                    + "</td></tr><tr><td>Exception Date/Time(Utc)</td><td>" +
                    DateTime.UtcNow.ToString() + "</td></tr><tr><td>User Name</td><td>" + "" + "</td></tr><tr><td>Stack Trace</td><td>" + exceptionMessage + "</td></tr></table>";
                //because system is unable to add row to error log, SendMail through SMTP details in web.config 
                Globals.SendExceptionEmail(Body);
            }
            throw new HttpResponseException(Context.Request.CreateErrorResponse((HttpStatusCode)Globals.ResponseCodes.SomethingWentWrong,
            // "Something Went Wrong. The issue has been reported. Please contact to Megacube."));
            (string.Format(Globals.SomethingElseFailedInDBErrorMessage, errorid)) ));

        }
    }

}
