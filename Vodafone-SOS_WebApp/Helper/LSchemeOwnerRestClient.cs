//Code Review for this file (from security perspective) done

using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LSchemeOwnerRestClient: ILSchemeOwnerRestClient
    {
           private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LSchemeOwnerRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        //Method for SysCat Dropdown
        public IEnumerable<UserAsDropdownViewModel> GetActiveReportingAnalysts(int CompanyId)
        {
             var request = new RestRequest("api/LSchemeOwner/GetActiveReportingAnalysts?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment); 
            var response = _client.Execute<List<UserAsDropdownViewModel>>(request); 
            return response.Data;
        }


        public IEnumerable<LSchemeOwnerViewModel> GetByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/LSchemeOwner/GetByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LSchemeOwnerViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public string DownloadSchemeOwners(string UserHexID, int CompanyID, string CompanyCode, string LoggedInUserName, int LoggedRoleId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LSchemeOwner/DownloadSchemeOwners?UserHexID={UserHexID}&CompanyID={CompanyID}&CompanyCode={CompanyCode}&LoggedInUserName={LoggedInUserName}&LoggedRoleId={LoggedRoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserHexID", UserHexID, ParameterType.UrlSegment);
            request.AddParameter("CompanyID", CompanyID, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserName", LoggedInUserName, ParameterType.UrlSegment);
            request.AddParameter("LoggedRoleId", LoggedRoleId, ParameterType.UrlSegment);

            var response = _client.Execute<dynamic>(request);
            var res = response.Data.ToString();
            return res;

        }

        public string DownloadSchemeOwners(int CompanyId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LSchemeOwner/DownloadSchemeOwners?CompanyId={CompanyId}&UserName={UserName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        } 
 
        public LSchemeOwnerViewModel GetById(int id)
        {
            var request = new RestRequest("api/LSchemeOwner/GetById/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LSchemeOwnerViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
   
        public void Update(LSchemeOwnerViewModel serverData , string UserName, string Workflow)
        {
            var request = new RestRequest("api/LSchemeOwner/Put/{id}?UserName={UserName}&Workflow={Workflow}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", string.IsNullOrEmpty(Workflow)?"No Workflow":Workflow, ParameterType.UrlSegment);
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData); 
            var response = _client.Execute<LSchemeOwnerViewModel>(request);

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
        public void Add(LSchemeOwnerViewModel serverData, string UserName, string Workflow)
        {
            var request = new RestRequest("api/LSchemeOwner/Post?UserName={UserName}&Workflow={Workflow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", string.IsNullOrEmpty(Workflow) ? "No Workflow" : Workflow, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LSchemeOwnerViewModel>(request);

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


        public void Delete(int id,string UserName, string Workflow)
        {
            var request = new RestRequest("api/LSchemeOwner/Delete/{id}?UserName={UserName}&Workflow={Workflow}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", string.IsNullOrEmpty(Workflow) ? "No Workflow" : Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<LSchemeOwnerViewModel>(request);

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
    interface ILSchemeOwnerRestClient
    {
        IEnumerable<UserAsDropdownViewModel> GetActiveReportingAnalysts(int CompanyId);
        LSchemeOwnerViewModel GetById(int id); 
        IEnumerable<LSchemeOwnerViewModel> GetByCompanyId(int CompanyId);
         void Delete(int id, string UserName, string Workflow);
        void Update(LSchemeOwnerViewModel serverData , string UserName, string Workflow);
        void Add(LSchemeOwnerViewModel serverData, string UserName, string Workflow);
        string DownloadSchemeOwners(int CompanyId);
    }
}