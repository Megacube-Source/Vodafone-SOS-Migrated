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
    public class LCommissionPeriodsRestClient:ILCommissionPeriodsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LCommissionPeriodsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LCommissionPeriodViewModel> GetByCompanyIdStatus(int CompanyId, string Status)
        {
            var request = new RestRequest("api/LCommissionPeriods/GetLCommissionPeriodsByCompanyIdStatus?CompanyId={CompanyId}&Status={Status}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCommissionPeriodViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        //Method added by SG - Payee Calc screen
        public IEnumerable<LCommissionPeriodViewModel> GetByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/LCommissionPeriods/GetLCommissionPeriodsStatusByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);            
            var response = _client.Execute<List<LCommissionPeriodViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public LCommissionPeriodViewModel GetById(int id)
        {
            var request = new RestRequest("api/LCommissionPeriods/GetLCommissionPeriod/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LCommissionPeriodViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public LCommissionPeriodViewModel Update(LCommissionPeriodViewModel serverData)
        {
            var request = new RestRequest("api/LCommissionPeriods/PutLCommissionPeriod/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LCommissionPeriodViewModel>(request);

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

            return response.Data;
        }
    }

    interface ILCommissionPeriodsRestClient
    {
        IEnumerable<LCommissionPeriodViewModel> GetByCompanyIdStatus(int CompanyId, string Status);
        LCommissionPeriodViewModel GetById(int id);
        LCommissionPeriodViewModel Update(LCommissionPeriodViewModel serverData);
        IEnumerable<LCommissionPeriodViewModel> GetByCompanyId(int CompanyId);
    }
}