using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LBatchFilesRestClient:ILBatchFilesRestClient
    {

        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LBatchFilesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LBatchFileViewModel> GetBatchFilesByBatchId(int BatchId)
        {
            var request = new RestRequest("api/LBatchFiles/GetLBatchFilesByBatchId?BatchId={BatchId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("BatchId", BatchId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LBatchFileViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
    }

    interface ILBatchFilesRestClient
    {
        IEnumerable<LBatchFileViewModel> GetBatchFilesByBatchId(int BatchId);
    }
}