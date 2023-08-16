using RestSharp;
using System;
using System.Configuration;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LCommonFunctionsRestClient : ILCommonFunctionsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LCommonFunctionsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public bool DoesUserHaveRole(string UserId, string Role)
        {
            var request = new RestRequest("api/LCommonFunctions/DoesUserHaveRole?UserId={UserId}&Role={Role}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("Role", Role, ParameterType.UrlSegment);

            var response = _client.Execute<bool>(request);
            return response.Data;
        }
    }

    interface ILCommonFunctionsRestClient
    {
        bool DoesUserHaveRole(string UserId, string Role);
    }
}