using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;


namespace Vodafone_SOS_WebApp.Helper
{
    public class LPayeeParentsRestClient:ILPayeeParentsRestClient
    {
          private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LPayeeParentsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LPayeeParentViewModel> GetAll()
        {
            var request = new RestRequest("api/LPayeeParents/GetPayeeParents", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<LPayeeParentViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public LPayeeParentViewModel GetByPayeeId(int PayeeId)
        {
            var request = new RestRequest("api/LPayeeParents/GetCurrentPayeeParentByPayeeId?PayeeId={PayeeId}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("PayeeId", PayeeId, ParameterType.UrlSegment);
            var response = _client.Execute<LPayeeParentViewModel>(request);

            return response.Data;
        }
        public IEnumerable<LPayeeParentViewModel> GetAllParentsByPayeeId(int PayeeId)
        {
            var request = new RestRequest("api/LPayeeParents/GetPayeeParentByPayeeId?PayeeId={PayeeId}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("PayeeId", PayeeId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayeeParentViewModel>>(request);

            return response.Data;
        }
        
        public LPayeeParentViewModel GetById(int id)
        {
            var request = new RestRequest("api/LPayeeParents/GetLPayeeParent/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LPayeeParentViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void Add(LPayeeParentViewModel serverData)
        {
            var request = new RestRequest("api/LPayeeParents/PostLPayeeParent", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<LPayeeParentViewModel>(request);

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

        public void Update(LPayeeParentViewModel serverData)
        {
            var request = new RestRequest("api/LPayeeParents/PutLPayeeParent/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LPayeeParentViewModel>(request);

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
            var request = new RestRequest("api/LPayeeParents/DeleteLPayeeParent/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<LPayeeParentViewModel>(request);

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

    interface ILPayeeParentsRestClient
    {
        IEnumerable<LPayeeParentViewModel> GetAll();
        LPayeeParentViewModel GetById(int id);
        IEnumerable<LPayeeParentViewModel> GetAllParentsByPayeeId(int PayeeId);
        LPayeeParentViewModel GetByPayeeId(int PayeeId);
        void Add(LPayeeParentViewModel LPayeeParentViewModel);
        void Update(LPayeeParentViewModel LPayeeParentViewModel);
        void Delete(int id);
    }
}