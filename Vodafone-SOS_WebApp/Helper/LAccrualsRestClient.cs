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
    public class LAccrualsRestClient : ILAccrualsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LAccrualsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }


        public LAccrualViewModel GetById(int Id)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LAccruals/GetLAccrualById/{Id}?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<LAccrualViewModel>(request);
            return response.Data;
        }

        public int Add(LAccrualViewModel serverData, string LoggedInRoleId, string WorkflowName, string AttachedFiles, string AttachedFilePath, string PortfolioList, string SupportingDocuments,string RedirectToUrl)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
           // var PortfolioModel = new PortfolioViewModel { PortfolioList=PortfolioList};
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LAccruals/PostLAccrual?AttachedFiles={AttachedFiles}&AttachedFilePath={AttachedFilePath}&SupportingDocuments={SupportingDocuments}&LoggedInRoleId={LoggedInRoleId}&UserName={UserName}&Workflow={Workflow}", Method.POST) { RequestFormat = DataFormat.Json };
            serverData.ParameterCarrier = PortfolioList;//using parametercarrier for  portfoliolist 
            request.AddBody(serverData);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("AttachedFiles", AttachedFiles, ParameterType.UrlSegment);
            request.AddParameter("AttachedFilePath", AttachedFilePath, ParameterType.UrlSegment);
           // request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocuments", (string.IsNullOrEmpty(SupportingDocuments)) ? "" : SupportingDocuments, ParameterType.UrlSegment);
            var response = _client.Execute<LAccrualViewModel>(request);

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

    interface ILAccrualsRestClient
    {
        LAccrualViewModel GetById(int Id);
        int Add(LAccrualViewModel serverData, string LoggedInRoleId, string WorkflowName, string AttachmentFiles, string AttachmentFilePath, string PortfolioList, string SupportingFiles,string RedirectToUrl);
    }
}