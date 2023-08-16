using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class RStatusesRestClient:IRStatusesRestClient
    {
         private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public RStatusesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<RStatusViewModel> GetAll()
        {
            var request = new RestRequest("api/RStatuses/GetRStatuses", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<RStatusViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<RStatusViewModel> GetDropDownDataByOwnerId(int OwnerId)
        {
            var request = new RestRequest("api/RStatuses/GetRStatusesDropdownData?OwnerId={OwnerId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("OwnerId", OwnerId, ParameterType.UrlSegment);
            var response = _client.Execute<List<RStatusViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<RStatusViewModel> GetDropDownDataByOwnerName(string OwnerName)
        {
            var request = new RestRequest("api/RStatuses/GetRStatusesByOwnerId?OwnerName={OwnerName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("OwnerName",OwnerName, ParameterType.UrlSegment);
            var response = _client.Execute<List<RStatusViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public IEnumerable<RStatusViewModel> GetByStatusName(string Name)
        {
            var request = new RestRequest("api/RStatuses/GetRStatusesByName?Name={Name}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Name", Name, ParameterType.UrlSegment);
            var response = _client.Execute<List<RStatusViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<RStatusViewModel> GetByOwnerId(int OwnerId)
        {
            var request = new RestRequest("api/RStatuses/GetRStatusesByOwnerId?OwnerId={OwnerId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("OwnerId",OwnerId, ParameterType.UrlSegment);
            var response = _client.Execute<List<RStatusViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public RStatusViewModel GetById(int id)
        {
            var request = new RestRequest("api/RStatuses/GetRStatus/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<RStatusViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void Add(RStatusViewModel serverData)
        {
            var request = new RestRequest("api/RStatuses/PostRStatus", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<RStatusViewModel>(request);

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

        public void Update(RStatusViewModel serverData)
        {
            var request = new RestRequest("api/RStatuses/PutRStatus/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<RStatusViewModel>(request);

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
            var request = new RestRequest("api/RStatuses/DeleteRStatus/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<RStatusViewModel>(request);

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

    interface IRStatusesRestClient
    {
        IEnumerable<RStatusViewModel> GetAll();
        IEnumerable<RStatusViewModel> GetDropDownDataByOwnerId(int OwnerId);
        IEnumerable<RStatusViewModel> GetByOwnerId(int OwnerId);
        IEnumerable<RStatusViewModel> GetDropDownDataByOwnerName(string OwnerName);
        IEnumerable<RStatusViewModel> GetByStatusName(string Name);
        RStatusViewModel GetById(int id);
        void Add(RStatusViewModel RStatusViewModel);
        void Update(RStatusViewModel RStatusViewModel);
        void Delete(int id);
    }
}