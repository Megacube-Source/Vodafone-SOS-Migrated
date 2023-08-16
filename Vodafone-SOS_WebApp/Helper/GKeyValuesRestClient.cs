//Code Review for this file (from security perspective) done
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;


namespace Vodafone_SOS_WebApp.Helper
{
    public class GKeyValuesRestClient:IGKeyValuesRestClient
    {
          private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public GKeyValuesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        /// <summary>
        /// Created by Rakhi Singh
        /// Method to get the counts for Key Values from the GKeyValues for L2Admin Page
        /// </summary>
        /// <returns></returns>
        public int CountsForGKeyValueForConfiguration()
        {
            var request = new RestRequest("api/GKeyValues/GetGKeyValueCountForConfiguration", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        /// <summary>
        /// Created by Rakhi Singh 
        /// Method to get the Key Values from the GKeyValues for L2Admin Page
        /// </summary>       
        public IEnumerable<GKeyValueViewModel> GetGKeyValueForConfiguration(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            var request = new RestRequest("api/GKeyValues/GetGKeyValueForConfiguration?pagesize={pagesize}&pagenum={pagenum}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);

            var response = _client.Execute<List<GKeyValueViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<GKeyValueViewModel> GetByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/GKeyValues/GetGKeyValuesByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<GKeyValueViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public GKeyValueViewModel GetByName(string KeyName, int CompanyId)
        {
            var request = new RestRequest("api/GKeyValues/GetGKeyValuesByName?CompanyId={CompanyId}&KeyName={KeyName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("KeyName", KeyName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<GKeyValueViewModel>(request);
            return response.Data;
        }

        public GKeyValueViewModel GetById(int id)
        {
            var request = new RestRequest("api/GKeyValues/GetGKeyValue/{Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", id, ParameterType.UrlSegment);
            var response = _client.Execute<GKeyValueViewModel>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }      

        public void Add(GKeyValueViewModel serverData)
        {
            var request = new RestRequest("api/GKeyValues/PostGKeyValue", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<GKeyValueViewModel>(request);

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

        public void Update(GKeyValueViewModel serverData)
        {
            var request = new RestRequest("api/GKeyValues/PutGKeyValue/{Id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<GKeyValueViewModel>(request);           

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
        
       //method used to delete the selected config for deletion 
        public void Delete(int id)
        {
            var request = new RestRequest("api/GKeyValues/DeleteGKeyVAlue/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute(request);
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


        //-- no longer used
        //public IEnumerable<GKeyValueViewModel> GetAll()
        //{
        //    var request = new RestRequest("api/GKeyValues/GetGKeyValues", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<GKeyValueViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
        //public void Update(GKeyValueViewModel model, string RedirectToUrl)
        //{
        //    var request = new RestRequest("api/GKeyValues/PutGKeyValue?id={id}", Method.PUT) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("id", model.Id, ParameterType.UrlSegment);
        //    request.AddBody(model);
        //    var response = _client.Execute<int>(request);
        //    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
        //    {
        //            throw new Exception(response.ErrorMessage);
        //    }
        //}

    }
    interface IGKeyValuesRestClient
    {
        //Code review comment IEnumerable<GKeyValueViewModel> GetAll();
        GKeyValueViewModel GetById(int id);
        IEnumerable<GKeyValueViewModel> GetByCompanyId(int CompanyId);
        void Add(GKeyValueViewModel GKeyValueViewModel);
        void Update(GKeyValueViewModel GKeyValueViewModel);
        void Delete(int id);
        GKeyValueViewModel GetByName(string KeyName, int CompanyId);
        IEnumerable<GKeyValueViewModel> GetGKeyValueForConfiguration(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery);
        int CountsForGKeyValueForConfiguration();
    }
}