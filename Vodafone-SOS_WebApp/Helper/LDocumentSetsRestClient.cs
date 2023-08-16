using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LDocumentSetsRestClient:ILDocumentSetsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LDocumentSetsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public string Add(LDocumentSetsViewModel serverData, string LoggedInRoleId, string WorkflowName, string AttachedFiles, string AttachedFilePath, string PortfolioList, string SupportingDocuments,string SupportingDocumentFilePath,string PayeeList,string RedirectToUrl)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LDocumentSets/PostLDocumentSet?AttachedFilePath={AttachedFilePath}&LoggedInRoleId={LoggedInRoleId}&UserName={UserName}&Workflow={Workflow}&PayeeList={PayeeList}&SupportingDocumentFilePath={SupportingDocumentFilePath}", Method.POST) { RequestFormat = DataFormat.Json };
            serverData.ParameterCarrier = PortfolioList;
            serverData.AttachedFiles = AttachedFiles;
            serverData.SupportingDocumentFiles = SupportingDocuments;
            request.AddBody(serverData);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
           // request.AddParameter("AttachedFiles", AttachedFiles, ParameterType.UrlSegment);
            request.AddParameter("AttachedFilePath", AttachedFilePath, ParameterType.UrlSegment);
           // request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
           // request.AddParameter("SupportingDocuments", (string.IsNullOrEmpty(SupportingDocuments)) ? "" : SupportingDocuments, ParameterType.UrlSegment);
            //request.AddParameter("PayeeList", PayeeList, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocumentFilePath", (string.IsNullOrEmpty(SupportingDocumentFilePath)) ? "" : SupportingDocumentFilePath, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<LDocumentSetsViewModel>(request);

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
            string Datasource = response.Content;
            dynamic IdList = JsonConvert.DeserializeObject(Datasource);
            return Convert.ToString(IdList);
        }

        public LDocumentSetsViewModel GetById(int Id)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LDocumentSets/GetLDocumentSetById/{Id}?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<LDocumentSetsViewModel>(request);
            return response.Data;
        }
    }

    interface ILDocumentSetsRestClient
    {
        LDocumentSetsViewModel GetById(int Id);
        string Add(LDocumentSetsViewModel serverData, string LoggedInRoleId, string WorkflowName, string AttachedFiles, string AttachedFilePath, string PortfolioList, string SupportingDocuments, string SupportingDocumentFilePath, string PayeeList, string RedirectToUrl);
    }
}