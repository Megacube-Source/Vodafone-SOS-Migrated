using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class XQlickRestClient:IXQlickRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public XQlickRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public List<XQlickViewModel> GetByRole(string LoggedInRoleName,int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/XQlick/GetXQlickByRole?LoggedInRoleName={LoggedInRoleName}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("LoggedInRoleName", LoggedInRoleName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<XQlickViewModel>>(request);
            return response.Data;
        }
    }

    interface IXQlickRestClient
    {
        List<XQlickViewModel> GetByRole(string LoggedInRoleName, int CompanyId);
    }
}