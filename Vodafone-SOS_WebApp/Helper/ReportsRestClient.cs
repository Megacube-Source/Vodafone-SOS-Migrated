using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class ReportsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public ReportsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<ReportsViewModel> GetXReportsTreeStructure(string strTreeType, string strUserEmail, string strUserRole, string strOpCoCode, string strCommissionPeriod, string FilterQuery, string sortdatafield, string sortorder,int  pagesize,int pagenum)
        {
            //api/Reports/GetXReportsTreeStructure?strUserEmail=shubhamshrm97@megacube.com.au&strUserRole=payee&strOpCoCode=QA
            if(sortdatafield == null)
            {
                sortdatafield = "";
            }
            if (sortorder == null)
            {
                sortorder = "";
            }
            var request = new RestRequest("api/Reports/GetXReportsTreeStructure?strTreeType={strTreeType}&strUserEmail={strUserEmail}&strUserRole={strUserRole}&strOpCoCode={strOpCoCode}&strCommissionPeriod={strCommissionPeriod}&sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("strTreeType", strTreeType, ParameterType.UrlSegment);
            request.AddParameter("strUserEmail", strUserEmail, ParameterType.UrlSegment);
            request.AddParameter("strUserRole", strUserRole, ParameterType.UrlSegment);
            request.AddParameter("strOpCoCode", strOpCoCode, ParameterType.UrlSegment);
            request.AddParameter("strCommissionPeriod", strCommissionPeriod, ParameterType.UrlSegment);
            //request.AddParameter("FilterQuery", FilterQuery, ParameterType.UrlSegment);
            //request.AddParameter("sortdatafield", sortdatafield, ParameterType.UrlSegment);
            //request.AddParameter("sortorder", sortorder, ParameterType.UrlSegment);
            //request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            //request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);


            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);

            var response = _client.Execute<List<ReportsViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public IEnumerable<ReportsViewModel> GetXReportsTreeStructureCount(string strTreeType, string strUserEmail, string strUserRole, string strOpCoCode, string strCommissionPeriod, string FilterQuery)
        {
            //api/Reports/GetXReportsTreeStructure?strUserEmail=shubhamshrm97@megacube.com.au&strUserRole=payee&strOpCoCode=QA
            var request = new RestRequest("api/Reports/GetXReportsTreeStructure?strTreeType={strTreeType}&strUserEmail={strUserEmail}&strUserRole={strUserRole}&strOpCoCode={strOpCoCode}&strCommissionPeriod={strCommissionPeriod}&FilterQuery={FilterQuery}&sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("strTreeType", strTreeType, ParameterType.UrlSegment);
            request.AddParameter("strUserEmail", strUserEmail, ParameterType.UrlSegment);
            request.AddParameter("strUserRole", strUserRole, ParameterType.UrlSegment);
            request.AddParameter("strOpCoCode", strOpCoCode, ParameterType.UrlSegment);
            request.AddParameter("strCommissionPeriod", strCommissionPeriod, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", "", ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", "", ParameterType.UrlSegment);
            request.AddParameter("sortorder", "", ParameterType.UrlSegment);
            request.AddParameter("pagesize", 0, ParameterType.UrlSegment);
            request.AddParameter("pagenum", 0, ParameterType.UrlSegment);
            var response = _client.Execute<List<ReportsViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
    }
    interface IReportsRestClient
    {
        IEnumerable<RDeviceTypeViewModel> GetXReportsTreeStructure(string strTreeType, string strUserEmail, string strUserRole, string strOpCoCode, string strCommissionPeriod);
        
    }
}