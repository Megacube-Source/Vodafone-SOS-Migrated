using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class WIAMSimulatorRestClient : IWIAMSimulatorRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["wiamwebservicebaseurl"];
        //
        public WIAMSimulatorRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public dynamic webservice(IndexViewModel model)
        {
           
            var request = new RestRequest("webservice", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(model);
            var response = _client.Execute<dynamic>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", (int)response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }
            return response.Data;
        }

    }

    public interface IWIAMSimulatorRestClient
    {
        dynamic webservice(IndexViewModel model);
    }
}