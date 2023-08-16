using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class MGMenusAspnetRolesRestClient:IMGMenusAspnetRolesRestClient
    {
 private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public MGMenusAspnetRolesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<MGMenusAspnetRoleViewModel> GetAll()
        {
            var request = new RestRequest("api/MGMenusAspnetRoles/GetMGMenusAspnetRoles", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<MGMenusAspnetRoleViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<MGMenusAspnetRoleViewModel> GetByUserRole(string UserRole,string CompanyCode)
        {
            var request = new RestRequest("api/MGMenusAspnetRoles/GetMGMenusAspnetRolesByUserRole?UserRole={UserRole}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserRole", UserRole, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<MGMenusAspnetRoleViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        
        public MGMenusAspnetRoleViewModel GetById(int id)
        {
            var request = new RestRequest("api/MGMenusAspnetRoles/GetMGMenusAspnetRole/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<MGMenusAspnetRoleViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void Add(MGMenusAspnetRoleViewModel serverData)
        {
            var request = new RestRequest("api/MGMenusAspnetRoles/PostMGMenusAspnetRole", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<MGMenusAspnetRoleViewModel>(request);

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

        public void Update(MGMenusAspnetRoleViewModel serverData)
        {
            var request = new RestRequest("api/MGMenusAspnetRoles/PutMGMenusAspnetRole/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<MGMenusAspnetRoleViewModel>(request);

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
            var request = new RestRequest("api/MGMenusAspnetRoles/DeleteMGMenusAspnetRole/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<MGMenusAspnetRoleViewModel>(request);

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

    interface IMGMenusAspnetRolesRestClient
    {
        IEnumerable<MGMenusAspnetRoleViewModel> GetAll();
        MGMenusAspnetRoleViewModel GetById(int id);
        void Add(MGMenusAspnetRoleViewModel MGMenusAspnetRoleViewModel);
        IEnumerable<MGMenusAspnetRoleViewModel> GetByUserRole(string UserRole,string CompanyCode);
        void Update(MGMenusAspnetRoleViewModel MGMenusAspnetRoleViewModel);
        void Delete(int id);
    }
}