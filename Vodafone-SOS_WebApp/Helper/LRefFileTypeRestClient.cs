//Code Review for this file (from security perspective) done

using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LRefFileTypeRestClient:ILRefFileTypesRestClient
    {
           private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LRefFileTypeRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        //public IEnumerable<LRefFileTypeViewModel> GetAll()
        //{
        //    var request = new RestRequest("api/LRefFileTypes/GetLRefFileTypes", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<LRefFileTypeViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public IEnumerable<LRefFileTypeViewModel> GetDropDownDataByCompanyId(int CompanyId)
        //{
        //    var request = new RestRequest("api/LRefFileTypes/GetLRefFileTypesDropdownData?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId",CompanyId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LRefFileTypeViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public IEnumerable<LRefFileTypeViewModel> GetByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/LRefFileTypes/GetLRefFileTypesByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRefFileTypeViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }


        public string DownloadRefFileTypes(int CompanyId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LRefFileTypes/DownloadRefFileTypes?CompanyId={CompanyId}&UserName={UserName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }



        public IEnumerable<LRefFileTypeViewModel> GetByPortfolioMatching(int CompanyId, string LoggedInUserId, string LoggedInRoleId)
        {
            var request = new RestRequest("api/LRefFileTypes/GetByPortfolioMatching?CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}&LoggedInRoleId={LoggedInRoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LRefFileTypeViewModel>>(request);

            return response.Data;
        }

        public LRefFileTypeViewModel GetById(int id)
        {
            var request = new RestRequest("api/LRefFileTypes/GetLRefFileType/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LRefFileTypeViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        //method to post grid daTA
        public void AddgRIDdATA(LRefFileTypeViewModel serverData,int CompanyId,string LoggedInUserId)
        {
            var request = new RestRequest("api/LRefFileTypes/PostLRefFileTypeGrid?CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData.ModelData);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            var response = _client.Execute<LRefFileTypeViewModel>(request);

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

        //public void Add(LRefFileTypeViewModel serverData)
        //{
        //    var request = new RestRequest("api/LRefFileTypes/PostLRefFileType", Method.POST) { RequestFormat = DataFormat.Json };
        //    request.AddBody(serverData);

        //    var response = _client.Execute<LRefFileTypeViewModel>(request);

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

        //public void Update(LRefFileTypeViewModel serverData)
        //{
        //    var request = new RestRequest("api/LRefFileTypes/PutLRefFileType/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
        //    request.AddBody(serverData);

        //    var response = _client.Execute<LRefFileTypeViewModel>(request);

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

        public void Add(LRefFileTypeViewModel serverData, string PortfolioList, string LoggedInRoleId, string UserName, string Workflow)
        {
            var request = new RestRequest("api/LRefFileTypes/PostLRefFileType?PortfolioList={PortfolioList}&LoggedInRoleId={LoggedInRoleId}&UserName={UserName}&Workflow={Workflow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);
            request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", string.IsNullOrEmpty(Workflow)?"No Workflow":Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<LRefFileTypeViewModel>(request);

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

        public void Update(LRefFileTypeViewModel serverData, string PortfolioList, string LoggedInRoleId, string UserName, string Workflow)
        {
            var request = new RestRequest("api/LRefFileTypes/PutLRefFileType/{id}?PortfolioList={PortfolioList}&LoggedInRoleId={LoggedInRoleId}&UserName={UserName}&Workflow={Workflow}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", string.IsNullOrEmpty(Workflow)?"No Workflow":Workflow, ParameterType.UrlSegment);
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LRefFileTypeViewModel>(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }



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


        public void Delete(int id,string UserName, string Workflow)
        {
            var request = new RestRequest("api/LRefFileTypes/DeleteLRefFileType/{id}?UserName={UserName}&Workflow={Workflow}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", string.IsNullOrEmpty(Workflow) ? "No Workflow" : Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<LRefFileTypeViewModel>(request);

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
    }
    interface ILRefFileTypesRestClient
    {
        //code review comment IEnumerable<LRefFileTypeViewModel> GetAll();
        LRefFileTypeViewModel GetById(int id);
        //code review comment IEnumerable<LRefFileTypeViewModel> GetDropDownDataByCompanyId(int CompanyId);
        IEnumerable<LRefFileTypeViewModel> GetByCompanyId(int CompanyId);
        IEnumerable<LRefFileTypeViewModel> GetByPortfolioMatching(int CompanyId,string LoggedInUserId,string LoggedInRoleId);
        //code review comment void Add(LRefFileTypeViewModel LRefFileTypeViewModel);
        void AddgRIDdATA(LRefFileTypeViewModel serverData, int CompanyId, string LoggedInUserId);
        //code review comment void Update(LRefFileTypeViewModel LRefFileTypeViewModel);
        void Delete(int id, string UserName, string Workflow);
        void Update(LRefFileTypeViewModel serverData, string PortfolioList, string LoggedInRoleId, string UserName, string Workflow);
        void Add(LRefFileTypeViewModel serverData, string PortfolioList, string LoggedInRoleId, string UserName, string Workflow);
        string DownloadRefFileTypes(int CompanyId);
    }
}