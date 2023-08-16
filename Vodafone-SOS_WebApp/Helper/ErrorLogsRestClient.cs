using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class ErrorLogsRestClient:IErrorLogsRestClient
    {
        private readonly RestClient _client;
        private readonly string _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public ErrorLogsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<GErrorLogViewModel> GetAll()
        {
            var request = new RestRequest("api/GErrorLogs/GetGErrorLogs", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<GErrorLogViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public GErrorLogViewModel GetById(int id)
        {
            var request = new RestRequest("api/GErrorLogs/GetGErrorLog/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<GErrorLogViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;

        }


        public void Add(GErrorLogViewModel serverData)
        {
            var request = new RestRequest("api/GErrorLogs/PostGErrorLog", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<GErrorLogViewModel>(request);

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
       
             public IEnumerable<GErrorLogViewModel> GetGErrorlogGrid(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
             {
              var request = new RestRequest("api/GErrorLogs/GetGErrorlogGridData?pagesize={pagesize}&pagenum={pagenum}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);

            var response = _client.Execute<List<GErrorLogViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        
             public int GetGErrorLogscounts()
        {
            var request = new RestRequest("api/GErrorLogs/GetErrorLogCount", Method.GET) { RequestFormat = DataFormat.Json };
          
            var response = _client.Execute<int>(request);

            return response.Data;
        }

        /// <summary>
        /// Method to get the total counts of Exception records for Summary tab on L2Admin Page
        /// </summary>        
        public int GetExceptionSummaryCounts()
        {
            var request = new RestRequest("api/GErrorLogs/GetExceptionSummaryCounts", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        /// <summary>
        ///  Method to get the data for summary tab of exception on L2Admin Page
        /// </summary>
        public IEnumerable<GErrorLogViewModel>GetExceptionSummary(string sortdatafield, string sortorder, int pagesize, int pagenum,string FilterQuery)
        {
            var request = new RestRequest("api/GErrorLogs/GetExceptionSummary?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<GErrorLogViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        /// <summary>
        /// Method to show the data on chart for Exception
        /// </summary>
        //public IEnumerable<GErrorLogViewModel> GetExceptionChart()
        //{
        //    var request = new RestRequest("api/GErrorLogs/GetExceptionChart", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<GErrorLogViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public string DownloadExceptionFile(string CompanyCode, string LoggedinUserName, string FilterQuery)
        {

            var request = new RestRequest("api/GErrorLogs/DownloadExceptionFile?CompanyCode={CompanyCode}&LoggedinUserName={LoggedinUserName}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("pagesize", 999999, ParameterType.UrlSegment);
            request.AddParameter("pagenum", 0, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", string.IsNullOrEmpty(CompanyCode) ? "" : CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("LoggedinUserName", string.IsNullOrEmpty(LoggedinUserName) ? "" : LoggedinUserName, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            
            var response = _client.Execute(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }



    }
    interface IErrorLogsRestClient
    {
        IEnumerable<GErrorLogViewModel> GetAll();
        GErrorLogViewModel GetById(int Id);
        void Add(GErrorLogViewModel ErrorLogViewModel);
        IEnumerable<GErrorLogViewModel> GetGErrorlogGrid(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery);
        int GetGErrorLogscounts();
        int GetExceptionSummaryCounts();
        //IEnumerable<GErrorLogViewModel> GetExceptionSummary();
        IEnumerable<GErrorLogViewModel> GetExceptionSummary(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery);
        //IEnumerable<GErrorLogViewModel> GetExceptionChart();
        string DownloadExceptionFile(string CompanyCode, string LoggedinUserName, string FilterQuery);

    }
}