using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LCompanySpecificRawDataColumnsRestClient:ILCompanySpecificRawDataColumnsRestClient
    {
          private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LCompanySpecificRawDataColumnsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LCompanySpecificRawDataColumnViewModel> GetAll()
        {
            var request = new RestRequest("api/LCompanySpecificRawDataColumns/GetLCompanySpecificRawDataColumns", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<LCompanySpecificRawDataColumnViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<LCompanySpecificRawDataColumnViewModel> GetRawDataColumnsByRawDataTableId(int RawDataTableId)
        {
            var request = new RestRequest("api/LCompanySpecificRawDataColumns/GetRawDataColumnsForGridByRawDataTableId?RawDataTableId={RawDataTableId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RawDataTableId", RawDataTableId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificRawDataColumnViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<CompanySpecificLabelViewModel> GetLRawDataColumns()
    {
        var request = new RestRequest("api/LCompanySpecificRawDataColumns/GetLCompanySpecificRawDataColumnNames", Method.GET) { RequestFormat = DataFormat.Json };

        var response = _client.Execute<List<CompanySpecificLabelViewModel>>(request);

        if (response.Data == null)
            throw new Exception(response.ErrorMessage);

        return response.Data;

    }

        public IEnumerable<CompanySpecificLabelViewModel> GetXSchemaColumns(int RawDataTableId)
        {
            var request = new RestRequest("api/LCompanySpecificRawDataColumns/GetCompanySpecificColumnsofXSchema?RawDataTableId={RawDataTableId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RawDataTableId", RawDataTableId, ParameterType.UrlSegment);
            var response = _client.Execute<List<CompanySpecificLabelViewModel>>(request);
            return response.Data;

        }
        public IEnumerable<LCompanySpecificRawDataColumnViewModel> GetByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/LCompanySpecificRawDataColumns/GetLCompanySpecificRawDataColumnsForClaimsByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificRawDataColumnViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public LCompanySpecificRawDataColumnViewModel GetById(int id)
        {
            var request = new RestRequest("api/LCompanySpecificRawDataColumns/GetCompanySpecificColumn/{Id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LCompanySpecificRawDataColumnViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void AddGridData(string model,Nullable<int> RawDataTableId,string RawDataTableName,int CompanyId)
        {
            var GridModel = new GridDataViewModel();
            GridModel.GridData = model;
            var request = new RestRequest("api/LCompanySpecificRawDataColumns/PostRawDataColumnsGridData?RawDataTableId={RawDataTableId}&RawDataTableName={RawDataTableName}&CompanyId={CompanyId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("RawDataTableId", (RawDataTableId.HasValue)?RawDataTableId:0, ParameterType.UrlSegment);
            request.AddParameter("RawDataTableName", RawDataTableName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddBody(GridModel);
            var response = _client.Execute<LCompanySpecificRawDataColumnViewModel>(request);
           
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

            if(!string.IsNullOrEmpty(response.Content))//Adding this condition as we are sending errored rows from api to be displayed to user.
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

        public void Update(LCompanySpecificRawDataColumnViewModel serverData)
        {
            var request = new RestRequest("api/LCompanySpecificRawDataColumns/PutCompanySpecificColumn/{Id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LCompanySpecificRawDataColumnViewModel>(request);

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
            var request = new RestRequest("api/LCompanySpecificRawDataColumns/DeleteCompanySpecificColumn/{Id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<LCompanySpecificRawDataColumnViewModel>(request);

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
        public void DeleteByRawDataTableId(int RawDataTableId)
        {
            var request = new RestRequest("api/LCompanySpecificRawDataColumns/DeleteAllRowsByRawDataTableId?RawDataTableId={RawDataTableId}", Method.DELETE);
            request.AddParameter("RawDataTableId", RawDataTableId, ParameterType.UrlSegment);

            var response = _client.Execute<LCompanySpecificRawDataColumnViewModel>(request);

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
    interface ILCompanySpecificRawDataColumnsRestClient
    {
        IEnumerable<LCompanySpecificRawDataColumnViewModel> GetAll();
        LCompanySpecificRawDataColumnViewModel GetById(int id);
        IEnumerable<LCompanySpecificRawDataColumnViewModel> GetByCompanyId(int CompanyId);
        IEnumerable<CompanySpecificLabelViewModel> GetLRawDataColumns();
        IEnumerable<CompanySpecificLabelViewModel> GetXSchemaColumns(int RawDataTableId);
        IEnumerable<LCompanySpecificRawDataColumnViewModel> GetRawDataColumnsByRawDataTableId(int RawDataTableId);

        void AddGridData(string model, Nullable<int> RawDataTableId, string RawDataTableName, int CompanyId);
        void Update(LCompanySpecificRawDataColumnViewModel LCompanySpecificRawDataColumnViewModel);
        void Delete(int id);
        void DeleteByRawDataTableId(int RawDataTableId);
    }
}