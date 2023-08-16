using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class XRawData_BI_PREPAID_ACTIVATION_REPORTRestClient:IXRawData_BI_PREPAID_ACTIVATION_REPORTRestClient
    {
          private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public XRawData_BI_PREPAID_ACTIVATION_REPORTRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<XRawData_BI_PREPAID_ACTIVATION_REPORTViewModel> GetAll()
        {
            var request = new RestRequest("api/XRawData_BI_PREPAID_ACTIVATION_REPORT/GetXRawData_BI_PREPAID_ACTIVATION_REPORT", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<XRawData_BI_PREPAID_ACTIVATION_REPORTViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
    }

    interface IXRawData_BI_PREPAID_ACTIVATION_REPORTRestClient
    {
        IEnumerable<XRawData_BI_PREPAID_ACTIVATION_REPORTViewModel> GetAll();
    }
}