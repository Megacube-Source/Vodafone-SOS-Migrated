//Code Review for this file (from security perspective) done

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LRawDataRestClient:ILRawDataRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LRawDataRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        //GetRawDataCounts  //GetRawDataCountsForOpcoDB
        public IEnumerable<LRawDataRowCountsViewModel> GetRawDataCounts(int CompanyId, int SOSBatchNumber)
        {
            var request = new RestRequest("api/LRawData/GetRawDataCounts?CompanyId={CompanyId}&SOSBatchNumber={SOSBatchNumber}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);

            var response = _client.Execute<List<LRawDataRowCountsViewModel>>(request);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = Regex.Replace(response.Content, "[^0-9A-Za-z ,]", " "); 
                ex.Data.Add("ErrorMessage",source);
                throw ex;
            }

            return response.Data;
        }

        public string DownloadRawData(int CompanyId, int SOSBatchNumber,int? RawDataTableId,string LoggedInUserName)
        {
            var request = new RestRequest("api/LRawData/DownloadLRawDataFile?SOSBatchNumber={SOSBatchNumber}&RawDataTableId={RawDataTableId}&CompanyId={CompanyId}&LoggedInUserName={LoggedInUserName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserName", LoggedInUserName, ParameterType.UrlSegment);
            request.AddParameter("RawDataTableId",(RawDataTableId.HasValue)?RawDataTableId:0, ParameterType.UrlSegment);//added NVL on 14MAy as I suspect this caused error
            var response = _client.Execute(request);

            //if (response.StatusCode != HttpStatusCode.OK)
            //{
            //    string Errorsource = response.Content;
            //    dynamic ErrorData = JsonConvert.DeserializeObject(Errorsource);
            //    return ErrorData.Message;
            //}


            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                string Errorsource = response.Content;
                dynamic ErrorData = JsonConvert.DeserializeObject(Errorsource);
                return ErrorData.Message;
            }
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        public string DownloadRawDataForStatuswise(int CompanyId, int SOSBatchNumber, int? RawDataTableId, string LoggedInUserName, string FilterQuery, string Status)
        {
            var request = new RestRequest("api/LRawData/DownloadLRawDataFileStatusWise?SOSBatchNumber={SOSBatchNumber}&RawDataTableId={RawDataTableId}&CompanyId={CompanyId}&LoggedInUserName={LoggedInUserName}&FilterQuery={FilterQuery}&Status={Status}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserName", LoggedInUserName, ParameterType.UrlSegment);
            request.AddParameter("RawDataTableId", (RawDataTableId.HasValue) ? RawDataTableId : 0, ParameterType.UrlSegment);//added NVL on 14MAy as I suspect this caused error
            request.AddParameter("FilterQuery", FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            var response = _client.Execute(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                string Errorsource = response.Content;
                dynamic ErrorData = JsonConvert.DeserializeObject(Errorsource);
                return ErrorData.Message;
            }
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        //public IEnumerable<LRawDataViewModel> GetAll()
        //{
        //    var request = new RestRequest("api/LRawData/GetLRawDatas", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<LRawDataViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}


        //GetLRawDataForGrid //GetRawDataFromOpCoDBForGrid

        public IEnumerable<dynamic> GetLRawDataForGrid(int CompanyId, int SOSBatchNumber, int PageNumber, int PageSize, int RawDataTableId, string Status, string FilterQuery)
        {
            var request = new RestRequest("api/LRawData/GetLRawDataForGrid?CompanyId={CompanyId}&SOSBatchNumber={SOSBatchNumber}&PageNumber={PageNumber}&PageSize={PageSize}&RawDataTableId={RawDataTableId}&Status={Status}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("RawDataTableId", RawDataTableId, ParameterType.UrlSegment); 
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            // var response1 = JObject.Parse(response.Content);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
               // string source = response.Content;
                string source = Regex.Replace(response.Content, "[^0-9A-Za-z ,]", " ");
                ex.Data.Add("ErrorMessage",source);
                throw ex;
            }

            return response.Data;
        }

        //public IEnumerable<LRawDataViewModel> GetByCompanyId(int CompanyId, int PageNumber, int BatchNumber)
        //{
        //    var request = new RestRequest("api/LRawData/GetLRawDatasByCompanyId?CompanyId={CompanyId}&PageNumber={PageNumber}&BatchNumber={BatchNumber}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
        //    request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LRawDataViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
        //public IEnumerable<LRawDataViewModel> GetExcludedDataByCompanyId(int CompanyId, int PageNumber, int BatchNumber)
        //{
        //    var request = new RestRequest("api/LRawData/GetLRawExcludedDatasByCompanyId?CompanyId={CompanyId}&BatchNumber={BatchNumber}&PageNumber={PageNumber}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
        //    request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LRawDataViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
        //public IEnumerable<LRawDataViewModel> GetErrorDataByCompanyId(int CompanyId, int PageNumber, int BatchNumber)
        //{
        //    var request = new RestRequest("api/LRawData/GetLRawErrorDatasByCompanyId?CompanyId={CompanyId}&BatchNumber={BatchNumber}&PageNumber={PageNumber}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
        //    request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LRawDataViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public LRawDataViewModel GetById(int id)
        //{
        //    var request = new RestRequest("api/LRawData/GetLRawData/{id}", Method.GET) { RequestFormat = DataFormat.Json };

        //    request.AddParameter("id", id, ParameterType.UrlSegment);
        //    var response = _client.Execute<LRawDataViewModel>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public void UpdateStatus(int id,string Comments,string Status, int CompanyId, int RawDataTableId,int SOSBatchNumber)
        {
            var request = new RestRequest("api/LRawData/UpdateRawDataStatus/{Id}?Comments={Comments}&Status={Status}&CompanyId={CompanyId}&RawDataTableId={RawDataTableId}&SOSBatchNumber={SOSBatchNumber}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", id, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("RawDataTableId", RawDataTableId, ParameterType.UrlSegment);
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);
            if (!string.IsNullOrEmpty(Comments))
            {
                request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
            }
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            var response = _client.Execute<LRawDataViewModel>(request);


        }

        //public LRawDataViewModel Update(LRawDataViewModel serverData)
        //{
        //    var request = new RestRequest("api/LRawData/PutLRawData/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
        //    request.AddBody(serverData);

        //    var response = _client.Execute<LRawDataViewModel>(request);

        //    if (response.StatusCode == HttpStatusCode.NotFound)
        //    {
        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }



        //    if (response.StatusCode == HttpStatusCode.InternalServerError)
        //    {
        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }
        //    if (response.StatusCode == HttpStatusCode.BadRequest)
        //    {
        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }
        //    return response.Data;
        //}
    }

    interface ILRawDataRestClient
    {
        //IEnumerable<LRawDataViewModel> GetAll();
        string DownloadRawData(int CompanyId, int SOSBatchNumber, int? RawDataTableId,string LoggedInUserName);
        string DownloadRawDataForStatuswise(int CompanyId, int SOSBatchNumber, int? RawDataTableId, string LoggedInUserName, string FilterQuery, string Status);
        void UpdateStatus(int id, string Comments, string Status, int CompanyId, int RawDataTableId,int SOSBatchNumber);
        IEnumerable<LRawDataRowCountsViewModel> GetRawDataCounts(int CompanyId, int SOSBatchNumber);
        IEnumerable<dynamic> GetLRawDataForGrid(int CompanyId, int SOSBatchNumber, int PageNumber, int PageSize, int RawDataTableId, string Status, string FilterQuery);
        //IEnumerable<LRawDataViewModel> GetByCompanyId(int CompanyId, int PageNumber, int BatchId);
        //IEnumerable<LRawDataViewModel> GetExcludedDataByCompanyId(int CompanyId, int PageNumber, int BatchId);
        //IEnumerable<LRawDataViewModel> GetErrorDataByCompanyId(int CompanyId, int PageNumber, int BatchId);
        //LRawDataViewModel Update(LRawDataViewModel serverData);
        //LRawDataViewModel GetById(int id);
    }
}