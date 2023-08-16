using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class AspnetRolesRestClient:IAspnetRolesRestClient
    {
          private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public AspnetRolesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<AspnetRoleViewModel> GetByCompanyCode(string CompanyCode)
        {
            var request = new RestRequest("api/AspnetRoles/GetAspnetRoles?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetRoleViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void UpdateMFAForRoles(string RoleList,string CompanyCode)
        {
            var request = new RestRequest("api/AspnetRoles/UpdateMFAForRoles?RoleList={RoleList}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RoleList", RoleList, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute(request);

        }

        public IEnumerable<AspnetRoleViewModel> GetMFARoles(string CompanyCode)
        {
            var request = new RestRequest("api/AspnetRoles/GetMFARoles?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetRoleViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<AspnetRoleViewModel> GetFinOpsByCompanyCode(string CompanyCode)
        {
            var request = new RestRequest("api/AspnetRoles/GetAspnetFinOpsRoles?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetRoleViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }



        public int GetByUserRoleCompanyCode(string CompanyCode, string UserRole)
        {
            var request = new RestRequest("api/AspnetRoles/GetAspnetRoleId?CompanyCode={CompanyCode}&UserRole={UserRole}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserRole", UserRole, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
            public IEnumerable<AspnetUserViewModel> GetClaimsAnalystByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/AspNetRoles/GetClaimsAnalystByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetUserViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }


        public IEnumerable<AspnetUserViewModel> GetManagerByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/AspNetRoles/GetManagerByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetUserViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<AspnetUserViewModel> GetUserList()
        {
            var request = new RestRequest("api/AspNetRoles/GetAspNetUsers", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<AspnetUserViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public AspnetRoleViewModel GetById(string id)
        {
            var request = new RestRequest("api/AspNetRoles/GetAspNetRole/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<AspnetRoleViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public AspnetRoleViewModel Add(AspnetRoleViewModel serverData)
        {
            var request = new RestRequest("api/AspNetRoles/PostAspNetRole", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<AspnetRoleViewModel>(request);

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

        public void Update(AspnetRoleViewModel serverData)
        {
            var request = new RestRequest("api/AspNetRoles/PutAspNetRole/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<AspnetRoleViewModel>(request);

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

        public void Delete(string id)
        {
            var request = new RestRequest("api/AspNetRoles/DeleteAspNetRole/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<AspnetRoleViewModel>(request);

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
    interface IAspnetRolesRestClient
    {
        IEnumerable<AspnetRoleViewModel> GetByCompanyCode(string CompanyCode);
        //IEnumerable<AspnetUserViewModel> GetUserList();
        IEnumerable<AspnetUserViewModel> GetClaimsAnalystByCompanyId(int CompanyId);
        IEnumerable<AspnetUserViewModel> GetManagerByCompanyId(int CompanyId);
        AspnetRoleViewModel GetById(string id);
        AspnetRoleViewModel Add(AspnetRoleViewModel AspnetRoleViewModel);
        void Update(AspnetRoleViewModel AspnetRoleViewModel);
        void Delete(string id);
        int GetByUserRoleCompanyCode(string CompanyCode, string UserRole);
        IEnumerable<AspnetRoleViewModel> GetFinOpsByCompanyCode(string CompanyCode);
        IEnumerable<AspnetRoleViewModel> GetMFARoles(string CompanyCode);
        void UpdateMFAForRoles(string RoleList, string CompanyCode);
    }
}