//Code Review for this file (from security perspective) done
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
//code review comment using System.Collections.Generic;
using System.Configuration;
//code review comment using System.Linq;
using System.Net;
//code review comment using System.Web;
using Vodafone_SOS_WebApp.ViewModels;
namespace Vodafone_SOS_WebApp.Helper
{
    public class GUserActivityLogRestClient: IGUserActivityLogRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public GUserActivityLogRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public void Add(GUserActivityLogViewModel serverData)
        {
            var request = new RestRequest("api/GUserActivityLog/PostGUserActivityLog", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<GUserActivityLogViewModel>(request);

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

        public int GetUserActivityLogCounts(string EmailId)
        {
            var request = new RestRequest("api/GUserActivityLog/GetUserActivityLogCounts?EmailId={EmailId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EmailId", EmailId, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);


            return response.Data;

        }
        public List<GUserActivityLogViewModel> GetUserActivityLog(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, string EmailId)
        {


            var request = new RestRequest("api/GUserActivityLog/GetUserActivityLog?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&EmailId={EmailId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("EmailId", EmailId, ParameterType.UrlSegment);
            var response = _client.Execute<List<GUserActivityLogViewModel>>(request);

            return response.Data;

        }
    }
    interface IGUserActivityLogRestClient
    {
        
        void Add(GUserActivityLogViewModel GUserActivityLog);
        List<GUserActivityLogViewModel> GetUserActivityLog(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, string EmailId);

        int GetUserActivityLogCounts(string EmailId);

    }
}
