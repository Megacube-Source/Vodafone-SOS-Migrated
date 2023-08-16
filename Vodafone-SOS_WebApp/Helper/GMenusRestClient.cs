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
    public class GMenusRestClient:IGMenusRestClient
    {
          private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public GMenusRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<GMenuViewModel> GetAll()
        {
            var request = new RestRequest("api/GMenus/GetGMenus", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<GMenuViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        
        public GMenuViewModel GetById(int id)
        {
            var request = new RestRequest("api/GMenus/GetGMenu/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<GMenuViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public GMenuViewModel Add(GMenuViewModel serverData)
        {
            var request = new RestRequest("api/GMenus/PostGMenu", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<GMenuViewModel>(request);

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

        public void Update(GMenuViewModel serverData)
        {
            var request = new RestRequest("api/GMenus/PutGMenu/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<GMenuViewModel>(request);

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
            var request = new RestRequest("api/GMenus/DeleteGMenu/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<GMenuViewModel>(request);

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

    }
    interface IGMenusRestClient
    {
        IEnumerable<GMenuViewModel> GetAll();
        GMenuViewModel GetById(int id);
        GMenuViewModel Add(GMenuViewModel GMenuViewModel);
        void Update(GMenuViewModel GMenuViewModel);
        void Delete(int id);
    }
}