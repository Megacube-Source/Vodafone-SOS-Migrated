//Code Review for this file (from security perspective) done

using RestSharp;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.ViewModels;
using System.Net;
using Newtonsoft.Json;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LWorkFlowConfigRestClient : ILWorkFlowConfigRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public LWorkFlowConfigRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public bool CheckCanCreate(string WorkflowName, string LoggedInRoleId, int CompanyId)
        {
            var request = new RestRequest("api/LWorkflowConfig/GetCheckCanCreate?WorkflowName={WorkflowName}&LoggedInRoleId={LoggedInRoleId}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("WorkflowName", WorkflowName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<bool>(request);

            return response.Data;
        }

        public IEnumerable<LWorkflowConfigViewModel> GetByWFId(int CompanyId, int WorkflowId)
        {
            var request = new RestRequest("api/LWorkflowConfig/GetByWFId?CompanyId={CompanyId}&WorkflowId={WorkflowId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("WorkflowId", WorkflowId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LWorkflowConfigViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }



        public void Update(LWorkflowConfigViewModel serverData)
        {
            var request = new RestRequest("api/LWorkflowConfig/PutLWorkflowConfig/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<RWorkFlowViewModel>(request);

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

        }

        public void Delete(int id)
        {
            var request = new RestRequest("api/LWorkflowConfig/DeleteLWorkflowConfig/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<LWorkflowConfigViewModel>(request);

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


        public void Add(LWorkflowConfigViewModel model)
        {
            var request = new RestRequest("api/LWorkflowConfig/PostLWorkFlowConfig", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(model);
            var response = _client.Execute<LWorkflowConfigViewModel>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
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
        public IEnumerable<AspnetRoleViewModel> GetRolesByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/AspnetRoles/GetRolesByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetRoleViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        


        public LWorkflowConfigViewModel GetById(int id)
        {
            var request = new RestRequest("api/LWorkflowConfig/GetById/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LWorkflowConfigViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

    }
    interface ILWorkFlowConfigRestClient
    {
        bool CheckCanCreate(string WorkflowName, string LoggedInRoleId, int CompanyId);
        IEnumerable<LWorkflowConfigViewModel> GetByWFId(int CompanyId, int WFId);
        void Update(LWorkflowConfigViewModel serverData);
        void Delete(int id);
        LWorkflowConfigViewModel GetById(int id);
        void Add(LWorkflowConfigViewModel model);
        IEnumerable<AspnetRoleViewModel> GetRolesByCompanyId(int CompanyId);
        
    }

}