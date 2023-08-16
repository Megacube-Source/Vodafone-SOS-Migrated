using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LEmailBucketRestClient: ILEmailBucketRestClient
    {

        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];


        public LEmailBucketRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public DateTime? GetEmailBucketMinDate()
        {
            var request = new RestRequest("api/LEmailBucket/GetLEmailBucketMinDate", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<DateTime?>(request);
            return response.Data;
        }
        public int GetEmailBucketcounts()
        {
            var request = new RestRequest("api/LEmailBucket/GetLEmailBucketCount", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }
        
            public DateTime GetEmailBucketMaxDate()
        {
            var request = new RestRequest("api/LEmailBucket/GetLEmailBucketMaxDate", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<DateTime>(request);
            return response.Data;
        }
          public string GetEmailBucketMaxStatus()
        {
            var request = new RestRequest("api/LEmailBucket/GetLEmailBucketMaxStatus", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _client.Execute(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        //method to get the data's count for summary tab for L2Admin Page
        public int GetEmailBucketSummaryCounts()
        {
            var request = new RestRequest("api/LEmailBucket/GetEmailBucketSummaryCount", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _client.Execute<int>(request);
            return response.Data;
        }

        //method to get the data for detail tab for L2Admin Page
        public IEnumerable<LEmailBucketViewModel> GetEmailBucketDetail(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            var request = new RestRequest("api/LEmailBucket/GetEmailBucketDetail?pagesize={pagesize}&pagenum={pagenum}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<LEmailBucketViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        //method to get the data's count for the detail tab for L2Admin Page
        public int GetEmailBucketDetailCounts()
        {
            var request = new RestRequest("api/LEmailBucket/GetEmailBucketDetailCount", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        //method to get the data for summary tab for L2Admin Page
        public IEnumerable<LEmailBucketViewModel> GetEmailBucketSummary(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {
            var request = new RestRequest("api/LEmailBucket/GetEmailBucketSummary?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<LEmailBucketViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        //method to get the data for Email Bucket Chart
        public IEnumerable<LEmailBucketViewModel> GetEmailBucketChart()
        {
            var request = new RestRequest("api/LEmailBucket/GetEmailBucketChart", Method.GET) { RequestFormat = DataFormat.Json };
           
            var response = _client.Execute<List<LEmailBucketViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
    }
    interface ILEmailBucketRestClient
    {
        DateTime? GetEmailBucketMinDate();
        int GetEmailBucketcounts();
        DateTime GetEmailBucketMaxDate();
        string GetEmailBucketMaxStatus();
        IEnumerable<LEmailBucketViewModel> GetEmailBucketSummary(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery);
        int GetEmailBucketDetailCounts();
        int GetEmailBucketSummaryCounts();
        IEnumerable<LEmailBucketViewModel> GetEmailBucketDetail(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery);
        IEnumerable<LEmailBucketViewModel> GetEmailBucketChart();
    }
}