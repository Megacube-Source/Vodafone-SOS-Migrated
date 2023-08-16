using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LAuditRestClient:ILAuditRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LAuditRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<GetLAuditForReports> GetAuditForReports(string Entity, int CompanyId, string StartDate, string EndDate, int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LAudit/GetLAuditForReports?Entity={Entity}&StartDate={StartDate}&EndDate={EndDate}&CompanyId={CompanyId}&pagesize={pagesize}&pagenum={pagenum}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Entity", Entity, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);

            var response = _client.Execute<List<GetLAuditForReports>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public int GetAuditCountsForReports(string  Entity, int CompanyId, string StartDate, string EndDate)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LAudit/GetLAuditForReportCounts?Entity={Entity}&StartDate={StartDate}&EndDate={EndDate}&CompanyId={CompanyId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Entity", Entity, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

          
            return response.Data;
        }

        public string DownloadAuditForReports(string Entity, int CompanyId, string StartDate, string EndDate, string LoggedInUserName)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LAudit/DownloadLAuditForReports?Entity={Entity}&StartDate={StartDate}&EndDate={EndDate}&CompanyId={CompanyId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Entity", Entity, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        public int GetCountsForNewItems() /*string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, int Intervalid*/
        {
           // var request = new RestRequest("api/LAudit/GetCountsForNewItems", Method.GET) { RequestFormat = DataFormat.Json };

            

            var request = new RestRequest("api/LAudit/GetCountsForNewItems?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&Intervalid={Intervalid}", Method.GET) { RequestFormat = DataFormat.Json };
            //request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            //request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            //request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            //request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            //request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            //request.AddParameter("Intervalid", Intervalid, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            return response.Data;

        }


        //public IEnumerable<GetLAuditForReports> GetNewItems(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        //{
        //    var request = new RestRequest("api/LAudit/GetNewItems?pagesize={pagesize}&pagenum={pagenum}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };

        //    request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
        //    request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
        //    request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
        //    request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
        //    request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);

        //    var response = _client.Execute<List<GetLAuditForReports>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public IEnumerable<GetLAuditForReports> GetNIForChart()
        {
            var request = new RestRequest("api/LAudit/GetNIForChart", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _client.Execute<List<GetLAuditForReports>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public List<dynamic> GetNewItemscolumnlist(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery,int Intervalid)
        {
            //var request = new RestRequest("api/LAudit/GetColumnsCountForNewItems", Method.GET) { RequestFormat = DataFormat.Json };

            //var response = _client.Execute<List<dynamic>>(request);

            //return response.Data;
            var request = new RestRequest("api/LAudit/GetNewItemscolumnlist?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&Intervalid={Intervalid}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("Intervalid", Intervalid, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);

            return response.Data;

        }

        public List<dynamic> GetDataForNewItemColumns(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery,int Intervalid)
        {


            var request = new RestRequest("api/LAudit/GetDataForNewItemColumns?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&Intervalid={Intervalid}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            //@Intervalid
            request.AddParameter("Intervalid", Intervalid, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);

            return response.Data;

        }
    }
    interface ILAuditRestClient
    {
        string DownloadAuditForReports(string Entity, int CompanyId, string StartDate, string EndDate, string LoggedInUserName);
        IEnumerable<GetLAuditForReports> GetAuditForReports(string Entity, int CompanyId, string StartDate, string EndDate, int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery);
        int GetAuditCountsForReports(string Entity, int CompanyId, string StartDate, string EndDate);
        int GetCountsForNewItems();
        // IEnumerable<GetLAuditForReports> GetNewItems(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery);
        List<dynamic> GetNewItemscolumnlist(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery,int Intervalid);
        List<dynamic> GetDataForNewItemColumns(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery,int Intervalid);
        IEnumerable<GetLAuditForReports> GetNIForChart();
    }
}