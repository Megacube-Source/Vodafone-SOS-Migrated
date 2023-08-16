using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class TroubleShootingRestClient : ITroubleShootingRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public TroubleShootingRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public bool CheckADAccountExist(string Email)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/TroubleShooting/CheckADAccountExist?Email={Email}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            var response = _client.Execute<bool>(request);
            return response.Data;
        }
        public bool UpdateActivateCreationLogin(string Email)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/TroubleShooting/UpdateActivateCreationLogin?Email={Email}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            var response = _client.Execute<bool>(request);
            return response.Data;
        }

        public bool UpdateDeActivateCreationLogin(string Email)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/TroubleShooting/UpdateDeActivateCreationLogin?Email={Email}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            var response = _client.Execute<bool>(request);
            return response.Data;
        }

        
        public IEnumerable<dynamic> GetPayeeData(string Email)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/TroubleShooting/GetPayeeData?Email={Email}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }
        public IEnumerable<dynamic> GetUserData(string Email)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/TroubleShooting/GetUserData?Email={Email}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }
        public IEnumerable<dynamic> GetUserLobbyData(string Email)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/TroubleShooting/GetUserLobbyData?Email={Email}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }
    }
    interface ITroubleShootingRestClient
    {
        bool CheckADAccountExist(string Email);
        IEnumerable<dynamic> GetPayeeData(string Email);
        IEnumerable<dynamic> GetUserData(string Email);
        IEnumerable<dynamic> GetUserLobbyData(string Email);
        bool UpdateActivateCreationLogin(string Email);

        bool UpdateDeActivateCreationLogin(string Email);
    }
}