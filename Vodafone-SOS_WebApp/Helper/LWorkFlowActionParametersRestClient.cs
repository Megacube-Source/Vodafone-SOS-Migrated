//Code Review for this file (from security perspective) done

using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LWorkFlowActionParametersRestClient :ILWorkFlowActionParametersRestClient
    {

        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LWorkFlowActionParametersRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public LWorkFlowActionParameterViewModel GetById(int id)
        {
            var request = new RestRequest("api/LWorkFlowActionParameters/GetById/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LWorkFlowActionParameterViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void Add(LWorkFlowActionParameterViewModel serverData)
        {
            var request = new RestRequest("api/LWorkFlowActionParameters/PostLWorkFlowActionParameters", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<LWorkFlowActionParameterViewModel>(request);

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
        }

        public void Update(LWorkFlowActionParameterViewModel serverData)
        {
            var request = new RestRequest("api/LWorkFlowActionParameters/PutLWorkFlowActionParameters/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LWorkFlowActionParameterViewModel>(request);

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

        }

        public void Delete(int id)
        {
            var request = new RestRequest("api/LWorkFlowActionParameters/DeleteLWorkFlowActionParameters/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<LWorkFlowActionParameterViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK)
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

        //method added by SG
        public IEnumerable<LWorkFlowActionParameterViewModel> GetByActionItemId(Nullable<int> ActionItemId)
        {
            var request = new RestRequest("api/LWorkFlowActionParameters/GetByActionItemId?ActionItemId={ActionItemId}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("ActionItemId", ActionItemId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LWorkFlowActionParameterViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

    }

    interface ILWorkFlowActionParametersRestClient

    {

        LWorkFlowActionParameterViewModel GetById(int id);
        void Add(LWorkFlowActionParameterViewModel LWorkFlowActionParametersViewModel);
        void Update(LWorkFlowActionParameterViewModel LWorkFlowActionParametersViewModel);
        void Delete(int id);
        IEnumerable<LWorkFlowActionParameterViewModel> GetByActionItemId(Nullable<int> ActionItemId);
    }

}