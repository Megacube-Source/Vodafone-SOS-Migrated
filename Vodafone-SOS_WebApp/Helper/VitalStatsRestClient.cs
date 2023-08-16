using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;
using System.Linq;
using System.Web;

namespace Vodafone_SOS_WebApp.Helper
{
    public class VitalStatsRestClient:IVitalStatsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public VitalStatsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<VitalStatsViewModel>GetVitalStatsForOpco(string StartDate, string EndDate)
        {
            var request = new RestRequest("api/VitalStats/GetVitalStatForOpcos?StartDate={StartDate}&EndDate={EndDate}", Method.GET) { RequestFormat = DataFormat.Json };
          
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);

            var response = _client.Execute<List<VitalStatsViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public void DownloadVitalStatsForReport(string StartDate, string EndDate)
        {
            var request = new RestRequest("api/VitalStats/DownloadVitalStatsForReports?StartDate={StartDate}&EndDate={EndDate}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);

            var response = _client.Execute<List<VitalStatsViewModel>>(request);
           


        }

       
    }
    interface IVitalStatsRestClient
    {
        IEnumerable<VitalStatsViewModel> GetVitalStatsForOpco(string StartDate, string EndDate);
      void DownloadVitalStatsForReport(string StartDate, string EndDate);

    }
}