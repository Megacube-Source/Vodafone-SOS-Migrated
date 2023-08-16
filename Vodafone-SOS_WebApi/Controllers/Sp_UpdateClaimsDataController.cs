using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
namespace Vodafone_SOS_WebApi.Controllers
{
    public class Sp_UpdateClaimsDataController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        public IHttpActionResult Sp_UpdateClaimsStatus(Sp_UpdateClaimsStatusViewModel model)
        {
            try
            {
               // db.SpUpdateClaimsData(model.ClaimsList, model.StatusName,model.AllocationDate,model.AllocateTo,model.AllocatedBy,model.ApprovedDate,model.ApprovedBy,model.RejectionReasonId,model.LastReClaimDate,model.IsReClaim,model.Comments);
                return Ok();
            }
            catch(Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
            }
        }
        private string GetCustomizedErrorMessage(Exception ex)
        {

            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            //Something else failed return original error message as retrieved from database
            return SqEx.Message;
        }
    }
}
