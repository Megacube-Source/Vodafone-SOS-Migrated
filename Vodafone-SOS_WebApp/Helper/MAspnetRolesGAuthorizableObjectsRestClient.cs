//Code Review for this file (from security perspective) done

using System;
using System.Collections.Generic;
using Vodafone_SOS_WebApp.ViewModels;
using RestSharp;
using System.Net;
using System.Configuration;

namespace Vodafone_SOS_WebApp.Helper
{
    public class MAspnetRolesGAuthorizableObjectsRestClient : IMAspnetRolesGAuthorizableObjectsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public MAspnetRolesGAuthorizableObjectsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public int GetCount(string UserRoleId, string CurrentActionKey)
        {

            var request = new RestRequest("api/MAspnetRolesGAuthorizableObjects/GetCount?UserRoleId={UserRoleId}&CurrentActionKey={CurrentActionKey}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserRoleId", UserRoleId, ParameterType.UrlSegment);
            request.AddParameter("CurrentActionKey", CurrentActionKey, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new Exception(response.ErrorMessage);
            }
            return response.Data;
        }

        public IEnumerable<MAspnetRolesAuthorizableObjectsViewModel> GetRolesByCompanyId(int CompanyId)
        {

            var request = new RestRequest("api/MAspnetRolesGAuthorizableObjects/GetRolesByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<MAspnetRolesAuthorizableObjectsViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<GAuthorizableObjectsViewModel> GetObjectsColumns(string RoleId)
        {//GetObjectsList
           // RoleId = "63";
            var request = new RestRequest("api/MAspnetRolesGAuthorizableObjects/GetGridData?RoleId={RoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            var response = _client.Execute<List<GAuthorizableObjectsViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;

        }

        public void SaveGrid(string model,string RoleId)
        {
            var GridModel = new GridDataViewModel();
            GridModel.GridData = model;
            var request = new RestRequest("api/MAspnetRolesGAuthorizableObjects/PostGridData?RoleId={RoleId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            request.AddBody(GridModel);
            var response = _client.Execute<GAuthorizableObjectsViewModel>(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }
        }
    }

    interface IMAspnetRolesGAuthorizableObjectsRestClient
    {
        int GetCount(string RoleId, string CurrentActionKey);

        IEnumerable<MAspnetRolesAuthorizableObjectsViewModel> GetRolesByCompanyId(int CompanyId);

        IEnumerable<GAuthorizableObjectsViewModel> GetObjectsColumns(string RoleId);

        void SaveGrid(string model,string RoleId);
    }
}