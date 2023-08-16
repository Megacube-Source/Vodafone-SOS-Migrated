using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class XBatchesRestClient:IXBatchesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public XBatchesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public XBatchViewModel GetByBatchNumber(int BatchNumber)
        {
            var request = new RestRequest("api/Xbatches/GetXBatchByBatchNumber?BatchNumber={BatchNumber}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            var response = _client.Execute<XBatchViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public XBatchViewModel Update(XBatchViewModel serverData)
        {
            var request = new RestRequest("api/Xbatches/PutXBatch/{Id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<XBatchViewModel>(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }
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
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }
            return response.Data;
        }
    }

    interface IXBatchesRestClient
    {
        XBatchViewModel GetByBatchNumber(int BatchNumber);
        XBatchViewModel Update(XBatchViewModel serverData);
    }
}