//Code Review for this file (from security perspective) done

using Newtonsoft.Json;
using RestSharp;
using System;
//code review comment using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;


namespace Vodafone_SOS_WebApp.Helper
{
    public class LSchemesRestClient:ILSchemesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LSchemesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public void AttachTestResults(string LoggedInUserId, string AttachedFileList, string AttachedFilePath,int TransactionId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LSchemes/AttachtestResults?TransactionId={TransactionId}&AttachedFilePath={AttachedFilePath}&AttachedFileList={AttachedFileList}&LoggedInUserId={LoggedInUserId}&UserName={UserName}&Workflow={Workflow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("TransactionId", TransactionId, ParameterType.UrlSegment);
            request.AddParameter("AttachedFileList", AttachedFileList, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("AttachedFilePath", AttachedFilePath, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            var response = _client.Execute<LSchemeViewModel>(request);
        }

        public LSchemeViewModel GetById(int Id)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LSchemes/GetLSchemeById/{Id}?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id",Id, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            var response = _client.Execute<LSchemeViewModel>(request);
            return response.Data;
        }

        public int Add(LSchemeViewModel serverData, string LoggedInRoleId, string WorkflowName, string AttachedFiles, string AttachedFilePath, string PortfolioList, string SupportingDocuments,string RedirectToUrl)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/LSchemes/PostLScheme?AttachedFiles={AttachedFiles}&AttachedFilePath={AttachedFilePath}&SupportingDocuments={SupportingDocuments}&LoggedInRoleId={LoggedInRoleId}&Workflow={Workflow}&UserName={UserName}", Method.POST) { RequestFormat = DataFormat.Json };
            serverData.ParameterCarrier = PortfolioList;//Use Parameter carrier for portfolio List
            request.AddBody(serverData);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("WorkflowName", WorkflowName, ParameterType.UrlSegment);
            request.AddParameter("AttachedFiles", AttachedFiles, ParameterType.UrlSegment);
            request.AddParameter("AttachedFilePath", AttachedFilePath, ParameterType.UrlSegment);
           // request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocuments", (string.IsNullOrEmpty(SupportingDocuments)) ? "" : SupportingDocuments, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<LSchemeViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                if (string.IsNullOrEmpty(RedirectToUrl))
                {
                    RedirectToUrl = "/Home/ErrorPage";
                }
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", (int)response.StatusCode);
                ex.Data.Add("RedirectToUrl", RedirectToUrl);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }
            return response.Data.Id;
        }

    }

    interface ILSchemesRestClient
    {
        void AttachTestResults(string LoggedInUserId, string AttachedFileList, string AttachedFilePath, int TransactionId);
        LSchemeViewModel GetById(int Id);
        int Add(LSchemeViewModel serverData, string LoggedInRoleId, string WorkflowName, string AttachmentFiles, string AttachmentFilePath, string PortfolioList, string SupportingFiles,string RedirectToUrl);
    }
}