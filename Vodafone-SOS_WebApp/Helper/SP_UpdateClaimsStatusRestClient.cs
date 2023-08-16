using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class Sp_UpdateClaimsStatusRestClient:ISp_UpdateClaimsStatusRestClient
    {
          private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public Sp_UpdateClaimsStatusRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public void UpdateClaimsStatus(Sp_UpdateStatusViewModel model)
        {
            var request = new RestRequest("api/Sp_UpdateClaimsData/Sp_UpdateClaimsStatus", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(model);
            //request.AddParameter("ClaimsList", ClaimsList, ParameterType.UrlSegment);
            //request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
            //request.AddParameter("AllocationDate", AllocationDate, ParameterType.UrlSegment);
            //request.AddParameter("AllocateTo",AllocateTo, ParameterType.UrlSegment);
            //request.AddParameter("AllocatedBy",AllocatedBy, ParameterType.UrlSegment);
            //request.AddParameter("ApprovedDate",ApprovedDate, ParameterType.UrlSegment);
            //request.AddParameter("ApprovedBy",ApprovedBy, ParameterType.UrlSegment);
            //request.AddParameter("RejectionReasonId",RejectionReasonId, ParameterType.UrlSegment);
            //request.AddParameter("LastReClaimDate",LastReClaimDate, ParameterType.UrlSegment);
            //request.AddParameter("IsReClaim",IsReClaim, ParameterType.UrlSegment);
            var response = _client.Execute(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }
           
        }

    }
    interface ISp_UpdateClaimsStatusRestClient
    {
        void UpdateClaimsStatus(Sp_UpdateStatusViewModel model);
    }
}