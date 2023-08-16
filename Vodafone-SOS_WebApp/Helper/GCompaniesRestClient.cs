using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;


namespace Vodafone_SOS_WebApp.Helper
{
    public class GCompaniesRestClient:IGCompaniesRestClient
    {
         private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public GCompaniesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<GCompanyViewModel> GetAll()
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if(string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCompanies/GetGCompanies?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<GCompanyViewModel>>(request);
            
            return response.Data;
        }

        //
        public GCompanyViewModel GetById(int id)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCompanies/GetGCompany/{id}?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<GCompanyViewModel>(request);
            return response.Data;
        }

        public void Add(GCompanyViewModel serverData, string RedirectToUrl)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCompanies/PostGCompany?UserName={UserName}&Workflow={Workflow}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<GCompanyViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK&&response.StatusCode!=HttpStatusCode.Created&&response.StatusCode!=HttpStatusCode.NoContent)
            {
                if (string.IsNullOrEmpty(RedirectToUrl))
                {
                    RedirectToUrl = "/Home/ErrorPage";
                }
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                ex.Data.Add("RedirectToUrl", RedirectToUrl);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }
            //if (response.StatusCode == (HttpStatusCode)Globals.ExceptionType.Type2)
            //{
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}
            //if (response.StatusCode == (HttpStatusCode)Globals.ExceptionType.Type3)
            //{
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}
        }

        public void Update(GCompanyViewModel serverData, string RedirectToUrl)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCompanies/PutGCompany/{id}?UserName={UserName}&Workflow={Workflow}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<GCompanyViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                if (string.IsNullOrEmpty(RedirectToUrl))
                {
                    RedirectToUrl = "/Home/ErrorPage";
                }
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                ex.Data.Add("RedirectToUrl", RedirectToUrl);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }
            //if (response.StatusCode == HttpStatusCode.NotFound)
            //{
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}



            //if (response.StatusCode == HttpStatusCode.InternalServerError)
            //{
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}
            //if (response.StatusCode == HttpStatusCode.BadRequest)
            //{
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}

        }

        public void Delete(int id, string RedirectToUrl)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCompanies/DeleteGCompany/{id}&UserName={UserName}&Workflow={Workflow}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<GCompanyViewModel>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                if (string.IsNullOrEmpty(RedirectToUrl))
                {
                    RedirectToUrl = "/Home/ErrorPage";
                }
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                ex.Data.Add("RedirectToUrl", RedirectToUrl);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }
            //if (response.StatusCode != HttpStatusCode.OK)
            //{

            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}
        }

        public IEnumerable<GCompanyViewModel> GetCompanyforAuditorAll()
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCompanies/GetGCompaniesForAuditor?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute< List<GCompanyViewModel>>(request);

            return response.Data;
        }

        public GCompanyViewModel GetGCompaniesForAuditor(int Companyid)
        {
            
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GCompanies/GetGCompaniesForAuditor?UserName={UserName}&Companyid={Companyid}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Companyid", Companyid, ParameterType.UrlSegment);
            var response = _client.Execute<GCompanyViewModel>(request);

            return response.Data;
        }

    }

    interface IGCompaniesRestClient
    {
        IEnumerable<GCompanyViewModel> GetAll();
        GCompanyViewModel GetById(int id);
        void Add(GCompanyViewModel GCompanyViewModel,string RedirectToUrl);
        void Update(GCompanyViewModel GCompanyViewModel, string RedirectToUrl);
        void Delete(int id, string RedirectToUrl);

        IEnumerable<GCompanyViewModel> GetCompanyforAuditorAll();

        GCompanyViewModel GetGCompaniesForAuditor(int CompanyID);
    }

}