using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class RChannelsRestClient:IRChannelsRestClient
    {
         private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public RChannelsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<RChannelViewModel> GetAll()
        {
            var request = new RestRequest("api/RChannels/GetRChannels", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<RChannelViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<RChannelViewModel> GetDataByQuery(string Query)
        {
            var request = new RestRequest("api/RChannels/GetRChannelsByQuery?Query={Query}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Query", Query, ParameterType.UrlSegment);
            var response = _client.Execute<List<RChannelViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public IEnumerable<RChannelViewModel> GetDropDownDataByCompanyId(int CompanyId,string PrimaryChannel)
        {
            var request = new RestRequest("api/RChannels/GetRChannelsDropdownData?CompanyId={CompanyId}&PrimaryChannel={PrimaryChannel}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId",CompanyId, ParameterType.UrlSegment);
            request.AddParameter("PrimaryChannel", PrimaryChannel, ParameterType.UrlSegment);
            var response = _client.Execute<List<RChannelViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<RChannelViewModel> GetByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/RChannels/GetRChannelsByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<RChannelViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public RChannelViewModel GetById(int id)
        {
            var request = new RestRequest("api/RChannels/GetRChannel/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<RChannelViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void Add(RChannelViewModel serverData)
        {
            var request = new RestRequest("api/RChannels/PostRChannel", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<RChannelViewModel>(request);

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

        public DataTable Update(RChannelViewModel serverData)
        {
            var request = new RestRequest("api/RChannels/PutRChannel/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<RChannelViewModel>(request);

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
            var res = JsonConvert.DeserializeObject<DataTable>(response.Content);
            return res;

        }

        public void Delete(int id)
        {
            var request = new RestRequest("api/RChannels/DeleteRChannel/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<RChannelViewModel>(request);

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

    interface IRChannelsRestClient
    {
        IEnumerable<RChannelViewModel> GetAll();
        RChannelViewModel GetById(int id);
        IEnumerable<RChannelViewModel> GetDropDownDataByCompanyId(int CompanyId, string PrimaryChannel);
        IEnumerable<RChannelViewModel> GetDataByQuery(string Query);
        IEnumerable<RChannelViewModel> GetByCompanyId(int CompanyId);
        void Add(RChannelViewModel RChannelViewModel);
        DataTable Update(RChannelViewModel RChannelViewModel);
        void Delete(int id);
    }
}