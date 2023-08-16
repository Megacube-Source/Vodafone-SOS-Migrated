using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class AspnetUsersRestClient:IAspnetUsersRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public AspnetUsersRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<AspnetUserViewModel> GetUserByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/AspnetUsers/GetAspNetUsersByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetUserViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        //MS R2.4 show only active users for on behalf of
        public IEnumerable<AspnetUserViewModel> GetActivetUserByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/AspnetUsers/GetActiveAspNetUsersByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetUserViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public AspnetUserViewModel GetById(string Id)
        {
            var request = new RestRequest("api/AspnetUsers/GetAspNetUser/{Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id",Id, ParameterType.UrlSegment);
            var response = _client.Execute<AspnetUserViewModel>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public AspnetUserViewModel GetByEmailId(string EmailId)
        {
            var request = new RestRequest("api/AspnetUsers/GetAspNetUsersByEmailId/{EmailID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("strEmailID", EmailId, ParameterType.UrlSegment);
            var response = _client.Execute<AspnetUserViewModel>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        
        public string GetUserByEmailId(string EmailId)
        {
            var request = new RestRequest("api/AspnetUsers/GetUsersByEmailId?EmailId={EmailId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EmailId", EmailId, ParameterType.UrlSegment);
            var response = _client.Execute<AspnetUserViewModel>(request);
            var text = response.Content.Replace('"', ' ').Trim();
          
            return text;
               
        }

        public IEnumerable<AspnetUserViewModel> GetSalesOperationsByCompanyCode(string CompanyCode)
        {
            var request = new RestRequest("api/AspnetUsers/GetSalesOperationsByCompanyCode?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetUserViewModel>>(request);
            
            return response.Data;
        }

        public IEnumerable<AspnetUserViewModel> GetChannelManagerByCompanyCode(string CompanyCode)
        {
            var request = new RestRequest("api/AspnetUsers/GetChannelManagerByCompanyCode?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetUserViewModel>>(request);

            return response.Data;
        }

        
    }
    interface IAspnetUsersRestClient
    {
        AspnetUserViewModel GetById(string Id);
        AspnetUserViewModel GetByEmailId(string EmailId);
        IEnumerable<AspnetUserViewModel> GetUserByCompanyId(int CompanyId);
        IEnumerable<AspnetUserViewModel> GetActivetUserByCompanyId(int CompanyId);
        IEnumerable<AspnetUserViewModel> GetSalesOperationsByCompanyCode(string CompanyCode);
        IEnumerable<AspnetUserViewModel> GetChannelManagerByCompanyCode(string CompanyCode);
        string GetUserByEmailId(string EmailId);
    }
}