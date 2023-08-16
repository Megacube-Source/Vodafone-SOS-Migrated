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
    public class LWorkflowGridColumnsRestClient:ILWorkflowGridColumnsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LWorkflowGridColumnsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        //public IEnumerable<LWorkflowGridColumnViewModel> GetAll()
        //{
        //    var request = new RestRequest("api/LWorkflowGridColumns/GetLWorkflowGridColumns", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<LWorkflowGridColumnViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public IEnumerable<LWorkflowConfigViewModel> GetRolesByWorkflow(string WorkFlow, int CompanyId)
        {
            var request = new RestRequest("api/LWorkflowGridColumns/GetTabsByWorkflowId?CompanyId={CompanyId}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Workflow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LWorkflowConfigViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<GenericGridDetails> GetByWorkFlow(string WorkFlow, int CompanyId,string LoggedInRoleId)
        {
            var request = new RestRequest("api/LWorkflowGridColumns/GetLWorkflowGridColumnsByWorkFlowId?WorkFlow={WorkFlow}&CompanyId={CompanyId}&LoggedInRoleId={LoggedInRoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("WorkFlow", WorkFlow, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<GenericGridDetails>>(request);
            
            return response.Data;
        }

        public LWorkflowGridColumnViewModel GetById(int id)
        {
            var request = new RestRequest("api/LWorkflowGridColumns/GetLWorkflowGridColumn/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LWorkflowGridColumnViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void Add(LWorkflowGridColumnViewModel serverData)
        {
            var request = new RestRequest("api/LWorkflowGridColumns/PostLWorkflowGridColumn", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<LWorkflowGridColumnViewModel>(request);

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

        public void Update(LWorkflowGridColumnViewModel serverData)
        {
            var request = new RestRequest("api/LWorkflowGridColumns/PutLWorkflowGridColumn/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LWorkflowGridColumnViewModel>(request);

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
            var request = new RestRequest("api/LWorkflowGridColumns/DeleteLWorkflowGridColumn/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<LWorkflowGridColumnViewModel>(request);

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

        //method added by SG
        public IEnumerable<LWorkflowGridColumnViewModel> GetByConfigId(Nullable<int> Configid,int RoleId)
        {
            var request = new RestRequest("api/LWorkflowGridColumns/GetWorkFlowGridColumnsByConfigId?Configid={Configid}&RoleId={RoleId}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("Configid", Configid, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LWorkflowGridColumnViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

         public IEnumerable<InformationSchemaViewModel> GetColumnNameByWFId(int WFId)
        {
            var request = new RestRequest("api/LWorkflowGridColumns/GetColumnNameByWFId?WFId={WFId}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("WFId", WFId, ParameterType.UrlSegment);
            var response = _client.Execute<List<InformationSchemaViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }


    }
    interface ILWorkflowGridColumnsRestClient

    {
        //code review comment IEnumerable<LWorkflowGridColumnViewModel> GetAll();
        IEnumerable<GenericGridDetails> GetByWorkFlow(string WorkFlow, int CompanyId, string LoggedInRoleId);
        IEnumerable<LWorkflowConfigViewModel> GetRolesByWorkflow(string WorkFlow, int CompanyId);
        LWorkflowGridColumnViewModel GetById(int id);
        void Add(LWorkflowGridColumnViewModel LWorkflowGridColumnViewModel);
        void Update(LWorkflowGridColumnViewModel LWorkflowGridColumnViewModel);
        void Delete(int id);
        IEnumerable<LWorkflowGridColumnViewModel> GetByConfigId(Nullable<int> Configid, int RoleId);
        IEnumerable<InformationSchemaViewModel> GetColumnNameByWFId(int WFId);
    }
}