using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LPasswordPoliciesRestClient:ILPasswordPoliciesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LPasswordPoliciesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public LPasswordPolicyViewModel GetByCompanyId(int CompanyId,string UserId)
        {
            var request = new RestRequest("api/LPasswordPolicies/GetLPasswordPolicies?CompanyId={CompanyId}&UserId={UserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            var response = _client.Execute<LPasswordPolicyViewModel>(request);


            return response.Data;
        }

    }

     interface ILPasswordPoliciesRestClient
        {
        LPasswordPolicyViewModel GetByCompanyId(int CompanyId, string UserId);
        }
}