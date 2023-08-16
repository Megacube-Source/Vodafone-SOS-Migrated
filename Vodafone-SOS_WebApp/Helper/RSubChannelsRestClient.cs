using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class RSubChannelsRestClient:IRSubChannelsRestClient
    {
         private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public RSubChannelsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<RSubChannelViewModel> GetAll()
        {
            var request = new RestRequest("api/RSubChannels/GetRSubChannels", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<RSubChannelViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<RSubChannelViewModel> GetDropDownDataByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/RSubChannels/GetRSubChannelsDropdownData?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId",CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<RSubChannelViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<RSubChannelViewModel> GetDropDownDataByChannelId(int ChannelId)
        {
            var request = new RestRequest("api/RSubChannels/GetRSubChannelsDropdownDataByChannelId?ChannelId={ChannelId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ChannelId", ChannelId, ParameterType.UrlSegment);
            var response = _client.Execute<List<RSubChannelViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<RSubChannelViewModel> GetByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/RSubChannels/GetRSubChannelsByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<RSubChannelViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public RSubChannelViewModel GetById(int id)
        {
            var request = new RestRequest("api/RSubChannels/GetRSubChannel/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<RSubChannelViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void Add(RSubChannelViewModel serverData)
        {
            var request = new RestRequest("api/RSubChannels/PostRSubChannel", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<RSubChannelViewModel>(request);

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

        public void Update(RSubChannelViewModel serverData)
        {
            var request = new RestRequest("api/RSubChannels/PutRSubChannel/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<RSubChannelViewModel>(request);

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
            var request = new RestRequest("api/RSubChannels/DeleteRSubChannel/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<RSubChannelViewModel>(request);

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
    interface IRSubChannelsRestClient
    {
        IEnumerable<RSubChannelViewModel> GetAll();
        RSubChannelViewModel GetById(int id);
        IEnumerable<RSubChannelViewModel> GetDropDownDataByCompanyId(int CompanyId);
        IEnumerable<RSubChannelViewModel> GetDropDownDataByChannelId(int ChannelId);
        IEnumerable<RSubChannelViewModel> GetByCompanyId(int CompanyId);
        void Add(RSubChannelViewModel RSubChannelViewModel);
        void Update(RSubChannelViewModel RSubChannelViewModel);
        void Delete(int id);
    }
}