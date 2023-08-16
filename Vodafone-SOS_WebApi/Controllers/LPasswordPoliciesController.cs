using System;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LPasswordPoliciesController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        public IHttpActionResult GetLPasswordPolicies(int CompanyId,string UserId)
        {
            var xx = (from aa in db.LPasswordPolicies.Where(p=>p.CompanyId==CompanyId)
                      select new PasswordPolicyViewModel{MaxAgeDays=aa.MaxAgeDays,MinLength= aa.MinLength,MinLowercase= aa.MinLowercase,MinNumbers= aa.MinNumbers,MinSpecialChars= aa.MinSpecialChars,MinUppercase= aa.MinUppercase
                      ,ReminderDays= aa.ReminderDays,
                         MinAgeDays= aa.MinAgeDays,DaysToExpirePassword=aa.MaxAgeDays
                      }).FirstOrDefault();
            if (db.LPasswordHistories.Where(p => p.UserId == UserId).Count() > 0)
            {
                var LastCreatedDate = db.LPasswordHistories.Where(p => p.UserId == UserId).Max(p => p.CreatedDateTime);
                xx.DaysToExpirePassword = Convert.ToInt32((LastCreatedDate.AddDays(xx.MaxAgeDays) - DateTime.UtcNow).TotalDays);

            }
            return Ok(xx);
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            return SqEx.Message;
        }
        }
    }
