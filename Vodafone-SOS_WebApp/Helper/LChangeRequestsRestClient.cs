using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LChangeRequestsRestClient:ILChangeRequestsRestClient
    {
         private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LChangeRequestsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LChangeRequestViewModel> GetAll()
        {
            var request = new RestRequest("api/LChangeRequests/GetLChangeRequests", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<LChangeRequestViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void UpdateChangeRequestStatus(Sp_UpdateItemStatusViewModel serverData)
        {
            var request = new RestRequest("api/LChangeRequests/Sp_UpdateChangeRequestStatus", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<LPayeeViewModel>(request);

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

        public void UpdateChangeRequestStatusForUser(Sp_UpdateItemStatusViewModel serverData)
        {
            var request = new RestRequest("api/LChangeRequests/PutUpdateUserChangeRequest", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<LPayeeViewModel>(request);

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

        public IEnumerable<LChangeRequestViewModel> GetByStatusNameCompanyIdEntityName(string StatusName,int CompanyId,string EntityName,string ReportsToId)
        {
            var request = new RestRequest("api/LChangeRequests/GetLChangeRequestByStatusNameCompanyIdEntityName?StatusName={StatusName}&CompanyId={CompanyId}&EntityName={EntityName}&ReportsToId={ReportsToId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("StatusName",StatusName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("EntityName", EntityName, ParameterType.UrlSegment);
            request.AddParameter("ReportsToId", ReportsToId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LChangeRequestViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }


        public IEnumerable<LChangeRequestViewModel> GetChangeRequestUnderStartDateEndDate(DateTime StartDate,DateTime EndDate,int CompanyId)
        {
            //Converting date parameter as string for time being will undo this once solution is found
            var StartDateTime = StartDate.ToString("yyyy-MM-dd");
            var EndDateTime = EndDate.ToString("yyyy-MM-dd");
            var request = new RestRequest("api/LChangeRequests/GetChangeRequestUnderStartDateEndDate?CompanyId={CompanyId}&StartDate={StartDate}&EndDate={EndDate}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("StartDate", StartDateTime, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDateTime, ParameterType.UrlSegment);
            var response = _client.Execute<List<LChangeRequestViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public IEnumerable<LChangeRequestViewModel> GetByStatusNameCompanyIdEntityNameCreatedById(string StatusName, int CompanyId, string EntityName,string CreatedByUserId)
        {
            var request = new RestRequest("api/LChangeRequests/GetLChangeRequestByStatusNameCompanyIdEntityNameCreatedById?StatusName={StatusName}&CompanyId={CompanyId}&EntityName={EntityName}&CreatedByUserId={CreatedByUserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("EntityName", EntityName, ParameterType.UrlSegment);
            request.AddParameter("CreatedByUserId", CreatedByUserId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LChangeRequestViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        //
        public LChangeRequestViewModel GetById(int id)
        {
            var request = new RestRequest("api/LChangeRequests/GetLChangeRequest/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LChangeRequestViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public LChangeRequestViewModel Add(LChangeRequestViewModel serverData,string LoggedRoleId,string WorkflowName)
        {
            var request = new RestRequest("api/LChangeRequests/PostLChangeRequest?LoggedInRoleId={LoggedInRoleId}&WorkflowName={WorkflowName}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("LoggedInRoleId", LoggedRoleId, ParameterType.UrlSegment);
            request.AddParameter("WorkflowName", WorkflowName, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LChangeRequestViewModel>(request);

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

        public void Update(LChangeRequestViewModel serverData)
        {
            var request = new RestRequest("api/LChangeRequests/PutLChangeRequest/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LChangeRequestViewModel>(request);

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
            var request = new RestRequest("api/LChangeRequests/DeleteLChangeRequest/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<LChangeRequestViewModel>(request);

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

    interface ILChangeRequestsRestClient
    {
        IEnumerable<LChangeRequestViewModel> GetAll();
        void UpdateChangeRequestStatusForUser(Sp_UpdateItemStatusViewModel serverData);
        IEnumerable<LChangeRequestViewModel> GetByStatusNameCompanyIdEntityName(string StatusName, int CompanyId, string EntityName, string ReportsToId);
        IEnumerable<LChangeRequestViewModel> GetByStatusNameCompanyIdEntityNameCreatedById(string StatusName, int CompanyId, string EntityName, string CreatedByUserId);
        LChangeRequestViewModel GetById(int id);
        IEnumerable<LChangeRequestViewModel> GetChangeRequestUnderStartDateEndDate(DateTime StartDate, DateTime EndDate, int CompanyId);
        void UpdateChangeRequestStatus(Sp_UpdateItemStatusViewModel model);
        LChangeRequestViewModel Add(LChangeRequestViewModel serverData, string LoggedRoleId, string WorkflowName);
        void Update(LChangeRequestViewModel LChangeRequestViewModel);
        void Delete(int id);
    }
}

