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
    public class LRawDataTablesRestClient:ILRawDataTablesRestClient
    {
          private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LRawDataTablesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        //public IEnumerable<LRawDataTableViewModel> GetAll()
        //{
        //    var request = new RestRequest("api/LRawDataTables/GetLRawDataTables", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<LRawDataTableViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public IEnumerable<LRawDataTableViewModel> GetDropDownDataByCompanyId(int CompanyId)
        //{
        //    var request = new RestRequest("api/LRawDataTables/GetLRawDataTablesDropdownData?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId",CompanyId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LRawDataTableViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public IEnumerable<dynamic> GetXTablesList(string CompanyCode)
        {
            var request = new RestRequest("api/LRawDataTables/GetXTablesList?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);

            return response.Data;
        }

        public void DeRegister(int RawDataTableId)
        {
            var request = new RestRequest("api/LRawDataTables/DeRegisterTables?RawDataTableId={RawDataTableId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("RawDataTableId", RawDataTableId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
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
        }

        //Get columns if rawdata table is not mapped yet
        public IEnumerable<CompanySpecificLabelViewModel> GetColumnsByTableName(string RawDataTableName, string CompanyCode)
        {
            var request = new RestRequest("api/LRawDataTables/GetTableColumnsByRawDataTableName?CompanyCode={CompanyCode}&RawDataTableName={RawDataTableName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RawDataTableName", RawDataTableName, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<CompanySpecificLabelViewModel>>(request);
            return response.Data;
        }

        public IEnumerable<LRawDataTableViewModel> GetByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/LRawDataTables/GetLRawDataTablesByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRawDataTableViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        //public LRawDataTableViewModel GetById(int id)
        //{
        //    var request = new RestRequest("api/LRawDataTables/GetLRawDataTable/{id}", Method.GET) { RequestFormat = DataFormat.Json };

        //    request.AddParameter("id", id, ParameterType.UrlSegment);
        //    var response = _client.Execute<LRawDataTableViewModel>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

    }

    interface ILRawDataTablesRestClient
    {
        IEnumerable<CompanySpecificLabelViewModel> GetColumnsByTableName(string RawDataTableName, string CompanyCode);
        void DeRegister(int RawDataTableId);
        //code review comment LRawDataTableViewModel GetById(int id);
        //code review comment IEnumerable<LRawDataTableViewModel> GetDropDownDataByCompanyId(int CompanyId);
        IEnumerable<LRawDataTableViewModel> GetByCompanyId(int CompanyId);
        IEnumerable<dynamic> GetXTablesList(string CompanyCode);

    }
}