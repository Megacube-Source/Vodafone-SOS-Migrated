using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LRefFilesRestClient:ILRefFilesRestClient
    {
            private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LRefFilesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        //public IEnumerable<LRefFileViewModel> GetAll()
        //{
        //    var request = new RestRequest("api/LUploadedFilesGetLUploadedFiles", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<LRefFileViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public IEnumerable<LRefFileViewModel> GetByRefTypeId(int RefFileTypeId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUploadedFiles/GetLUploadedFilesByCompanyId?RefFileTypeId={RefFileTypeId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RefFileTypeId", RefFileTypeId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRefFileViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        //public IEnumerable<LRefFileViewModel> GetByStatusName(string StatusName,string UploadType ,string ReportsToId)
        //{
        //    var request = new RestRequest("api/LUploadedFiles/GetLUploadedFilesByStatusName?StatusName={StatusName}&UploadType={UploadType}&ReportsToId={ReportsToId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("StatusName",StatusName, ParameterType.UrlSegment);
        //    request.AddParameter("UploadType", UploadType, ParameterType.UrlSegment);
        //    request.AddParameter("ReportsToId", ReportsToId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LRefFileViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public IEnumerable<LRefFileViewModel> GetByStatusNameAndUserId(string StatusName,string UserId,string UploadType)
        //{
        //    var request = new RestRequest("api/LUploadedFiles/GetLUploadedFilesByStatusNameAndUserId?StatusName={StatusName}&UserId={UserId}&UploadType={UploadType}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
        //    request.AddParameter("UserId",UserId, ParameterType.UrlSegment);
        //    request.AddParameter("UploadType", UploadType, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LRefFileViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public LRefFileViewModel GetById(int id)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRefFiles/GetLRefFile/{id}?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LRefFileViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

       
        public int Add(LRefFileViewModel serverData,string LoggedInRoleId,string WorkflowName,string AttachmentFiles,string AttachmentFilePath,string PortfolioList,string SupportingFiles,string RedirectToUrl,string SupportingDocFilePath)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            

            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            // var request = new RestRequest("api/LRefFiles/PostLRefFile?LoggedInRoleId={LoggedInRoleId}&AttachmentFiles={AttachmentFiles}&AttachmentFilePath={AttachmentFilePath}&SupportingFiles={SupportingFiles}&UserName={UserName}&Workflow={Workflow}&SupportingDocFilePath={SupportingDocFilePath}", Method.POST) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/LRefFiles/PostLRefFile?LoggedInRoleId={LoggedInRoleId}&AttachmentFilePath={AttachmentFilePath}&SupportingFiles={SupportingFiles}&UserName={UserName}&Workflow={Workflow}&SupportingDocFilePath={SupportingDocFilePath}", Method.POST) { RequestFormat = DataFormat.Json };
            serverData.ParameterCarrier = PortfolioList;//Use parmeter carrier to send portfolioList
            request.AddBody(serverData);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            //request.AddParameter("AttachmentFiles", AttachmentFiles, ParameterType.UrlSegment);
            request.AddParameter("AttachmentFilePath", AttachmentFilePath, ParameterType.UrlSegment);
            //request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("SupportingFiles", (string.IsNullOrEmpty(SupportingFiles))?"":SupportingFiles, ParameterType.UrlSegment);
            request.AddParameter("SupportingDocFilePath", (string.IsNullOrEmpty(SupportingDocFilePath)) ? "" : SupportingDocFilePath, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

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
            return response.Data;
        }

        //public void Update(LRefFileViewModel serverData)
        //{
        //    var request = new RestRequest("api/LUploadedFiles/PutLUploadedFile/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
        //    request.AddBody(serverData);

        //    var response = _client.Execute<LRefFileViewModel>(request);

        //    if (response.StatusCode == HttpStatusCode.NotFound)
        //    {
        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }



        //    if (response.StatusCode == HttpStatusCode.InternalServerError)
        //    {
        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }
        //    if (response.StatusCode == HttpStatusCode.BadRequest)
        //    {
        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }

        //}

        //public void UpdateStatus(LRefFileViewModel serverData,string Comments,string UpdatedBy)
        //{
        //    var request = new RestRequest("api/LUploadedFiles/PutUpdateStatus?Comments={Comments}&UpdatedBy={UpdatedBy}", Method.PUT) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
        //    request.AddParameter("UpdatedBy", UpdatedBy, ParameterType.UrlSegment);
        //    request.AddBody(serverData);

        //    var response = _client.Execute<LRefFileViewModel>(request);

        //    if (response.StatusCode == HttpStatusCode.NotFound)
        //    {
        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }



        //    if (response.StatusCode == HttpStatusCode.InternalServerError)
        //    {
        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }
        //    if (response.StatusCode == HttpStatusCode.BadRequest)
        //    {
        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }

        //}


        //public void Delete(int id)
        //{
        //    var request = new RestRequest("api/LUploadedFiles/DeleteLUploadedFile/{id}", Method.DELETE);
        //    request.AddParameter("id", id, ParameterType.UrlSegment);

        //    var response = _client.Execute<LRefFileViewModel>(request);

        //    if (response.StatusCode != HttpStatusCode.OK)
        //    {

        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }
        //}
    }

    interface ILRefFilesRestClient
    {
        //IEnumerable<LRefFileViewModel> GetAll();
        LRefFileViewModel GetById(int id);
        IEnumerable<LRefFileViewModel> GetByRefTypeId(int RefFileTypeId);
        //IEnumerable<LRefFileViewModel> GetByStatusNameAndUserId(string StatusName, string UserId, string UploadType);
        //IEnumerable<LRefFileViewModel> GetByStatusName(string StatusName, string UploadType, string ReportsToId);
        int Add(LRefFileViewModel serverData, string LoggedInRoleId, string WorkflowName, string AttachmentFiles, string AttachmentFilePath, string PortfolioList,string SupportingFiles, string RedirectToUrl,string SupportingDocFilePath);
       // void Update(LRefFileViewModel LRefFileViewModel);
       // void UpdateStatus(LRefFileViewModel serverData, string Comments, string UpdatedBy);
       // void Delete(int id);
    }
}