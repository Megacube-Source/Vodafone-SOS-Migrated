using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LDashboardConfigRestClient : ILDashboardConfigRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public LDashboardConfigRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<LDashboardConfigViewModel> GetKpiData(string identifier, string strType, string strCompanyCode, string strUserId, string strRoleId, string strPeriod, string strDimension, string strPortfolioList, string strPayeeList, string BatchStatus)
        {
            if (strPeriod == null) strPeriod = "";
            if (BatchStatus == null) BatchStatus = "";
            var request = new RestRequest("api/LDashboardConfig/GetKpiData?identifier={identifier}&strType={strType}&strCompanyCode={strCompanyCode}&strUserId={strUserId}&strRoleId={strRoleId}&strPeriod={strPeriod}&strDimension={strDimension}&strPortfolioList={strPortfolioList}&strPayeeList={strPayeeList}&BatchStatus={BatchStatus}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("identifier", identifier, ParameterType.UrlSegment);
            request.AddParameter("strType", strType, ParameterType.UrlSegment);
            request.AddParameter("strCompanyCode", strCompanyCode, ParameterType.UrlSegment);
            request.AddParameter("strUserId", strUserId, ParameterType.UrlSegment);
            request.AddParameter("strRoleId", strRoleId, ParameterType.UrlSegment);
            request.AddParameter("strPeriod", strPeriod, ParameterType.UrlSegment);
            request.AddParameter("strDimension", strDimension, ParameterType.UrlSegment);
            request.AddParameter("strPortfolioList", strPortfolioList, ParameterType.UrlSegment);
            request.AddParameter("strPayeeList", strPayeeList, ParameterType.UrlSegment);
            request.AddParameter("BatchStatus", BatchStatus, ParameterType.UrlSegment);
            var response = _client.Execute<List<LDashboardConfigViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public void SaveConfigurationSetting(LDashboardConfigViewModel serverData)
        {
            var request = new RestRequest("api/LDashboardConfig/SaveConfigurationSetting", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);
            var response = _client.Execute<LDashboardConfigViewModel>(request);
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
        public LDashboardConfigViewModel GetConfigurationById(int id, string UserID, string RoleId, int CompanyId)
        {
            var request = new RestRequest("api/LDashboardConfig/GetConfigurationById?id={id}&UserID={UserID}&RoleId={RoleId}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("UserID", UserID, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<LDashboardConfigViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public void DeleteConfiguration(int id)
        {
            var request = new RestRequest("api/LDashboardConfig/DeleteConfiguration/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute(request);
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




        public string DownloadGraphData(string identifier, string strType, string strCompanyCode, string strUserId, string strRoleId, string strPeriod, string strDimension, string strPortfolioList, string strPayeeList, string BatchStatus)
        {

            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            if (strPeriod == null) strPeriod = "";
            if (BatchStatus == null) BatchStatus = "";
            var request = new RestRequest("api/LDashboardConfig/DownloadKpiData?identifier={identifier}&strType={strType}&strCompanyCode={strCompanyCode}&strUserId={strUserId}&strRoleId={strRoleId}&strPeriod={strPeriod}&strDimension={strDimension}&strPortfolioList={strPortfolioList}&strPayeeList={strPayeeList}&BatchStatus={BatchStatus}&UserName={UserName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("identifier", identifier, ParameterType.UrlSegment);
            request.AddParameter("strType", strType, ParameterType.UrlSegment);
            request.AddParameter("strCompanyCode", strCompanyCode, ParameterType.UrlSegment);
            request.AddParameter("strUserId", strUserId, ParameterType.UrlSegment);
            request.AddParameter("strRoleId", strRoleId, ParameterType.UrlSegment);
            request.AddParameter("strPeriod", strPeriod, ParameterType.UrlSegment);
            request.AddParameter("strDimension", strDimension, ParameterType.UrlSegment);
            request.AddParameter("strPortfolioList", strPortfolioList, ParameterType.UrlSegment);
            request.AddParameter("strPayeeList", strPayeeList, ParameterType.UrlSegment);
            request.AddParameter("BatchStatus", BatchStatus, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);

            var response = _client.Execute(request);

            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;

         
        }











    }
    interface ILDashboardConfigRestClient
    {
        IEnumerable<LDashboardConfigViewModel> GetKpiData(string identifier, string strType, string strCompanyCode, string strUserId, string strRoleId, string strPeriod, string strDimension, string strPortfolioList, string strPayeeList, string BatchStatus);
        void SaveConfigurationSetting(LDashboardConfigViewModel serverData);
        LDashboardConfigViewModel GetConfigurationById(int id, string UserID, string RoleId, int CompanyId);
        void DeleteConfiguration(int id);

        string DownloadGraphData(string identifier, string strType, string strCompanyCode, string strUserId, string strRoleId, string strPeriod, string strDimension, string strPortfolioList, string strPayeeList, string BatchStatus);
    }
}