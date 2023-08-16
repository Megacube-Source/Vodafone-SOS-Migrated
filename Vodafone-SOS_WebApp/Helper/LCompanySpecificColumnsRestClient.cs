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
    public class LCompanySpecificColumnsRestClient:ILCompanySpecificColumnsRestClient
    {
         private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LCompanySpecificColumnsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        //code review comment
        //public IEnumerable<LCompanySpecificColumnViewModel> GetAll()
        //{
        //    var request = new RestRequest("api/LCompanySpecificColumns/GetLCompanySpecificColumns", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
        //public LCompanySpecificColumnViewModel GetById(int id)
        //{
        //    var request = new RestRequest("api/LCompanySpecificColumns/GetCompanySpecificColumn/{Id}", Method.GET) { RequestFormat = DataFormat.Json };

        //    request.AddParameter("id", id, ParameterType.UrlSegment);
        //    var response = _client.Execute<LCompanySpecificColumnViewModel>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
        //public void Update(LCompanySpecificColumnViewModel serverData)
        //{
        //    var request = new RestRequest("api/LCompanySpecificColumns/PutCompanySpecificColumn/{Id}", Method.PUT) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
        //    request.AddBody(serverData);

        //    var response = _client.Execute<LCompanySpecificColumnViewModel>(request);

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
        //    var request = new RestRequest("api/LCompanySpecificColumns/DeleteCompanySpecificColumn/{Id}", Method.DELETE);
        //    request.AddParameter("id", id, ParameterType.UrlSegment);

        //    var response = _client.Execute<LCompanySpecificColumnViewModel>(request);

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
        public IEnumerable<LCompanySpecificColumnViewModel> GetPayeeColumnsByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/GetPayeeLCompanySpecificColumnsByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
         public IEnumerable<LCompanySpecificColumnViewModel> GetClaimsColumnsByCompanyId(int CompanyId)
         {
             var request = new RestRequest("api/LCompanySpecificColumns/GetLCompanySpecificColumnsForClaimsByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
             request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
             var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

             if (response.Data == null)
                 throw new Exception(response.ErrorMessage);

             return response.Data;
         }

         public IEnumerable<LCompanySpecificColumnViewModel> GetPayeeColumnsByCompanyIdForGrid(int CompanyId)
         {
             var request = new RestRequest("api/LCompanySpecificColumns/GetPayeeLCompanySpecificColumnsByCompanyIdForGrid?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
             request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
             var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

             if (response.Data == null)
                 throw new Exception(response.ErrorMessage);

             return response.Data;
         }
         public IEnumerable<LCompanySpecificColumnViewModel> GetClaimsColumnsByCompanyIdForGrid(int CompanyId)
         {
             var request = new RestRequest("api/LCompanySpecificColumns/GetLCompanySpecificColumnsForClaimsByCompanyIdGrid?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
             request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
             var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

             return response.Data;
         }

        public IEnumerable<LCompanySpecificColumnViewModel> GetCalculationsColumnsByCompanyIdForGrid(int CompanyId)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/GetLCompanySpecificColumnsForCalculationsByCompanyIdGrid?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<LCompanySpecificColumnViewModel> GetPayColumnsByCompanyIdForGrid(int CompanyId)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/GetLCompanySpecificColumnsForPayByCompanyIdGrid?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<LCompanySpecificColumnViewModel> GetClaimsDownloadTemplateByCompanyId(int CompanyId)
         {
             var request = new RestRequest("api/LCompanySpecificColumns/GetColumnsForDownloadTemplateForClaims?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
             request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
             var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);
            
             return response.Data;
         }
        
        public void Add(LCompanySpecificColumnViewModel serverData)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/PostCompanySpecificColumn", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<LCompanySpecificColumnViewModel>(request);

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

        public IEnumerable<LCompanySpecificForms> getBannerDetail(int companyid, string formname)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/GetBannerTextValue?CompanyId={companyid}&formname={formname}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("companyid", companyid, ParameterType.UrlSegment);
            request.AddParameter("formname", formname, ParameterType.UrlSegment);
            //var response = _client.Execute(request);

            var response = _client.Execute<List<LCompanySpecificForms>>(request);

            //   return response.Data;

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
            return response.Data;
        }

        //GetBannerDetailById
        //public LCompanySpecificForms getBannerDetailByID(int id)
        //{
        //    var request = new RestRequest("api/LCompanySpecificColumns/GetBannerDetailById?id={id}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("id", id, ParameterType.UrlSegment);

        //    var response = _client.Execute<LCompanySpecificForms>(request);

        //    //   return response.Data;

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
        //    return response.Data;
        //}



        public LCompanySpecificForms getBannerDetailByID(int id)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/GetBannerDetailById/{Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LCompanySpecificForms>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }



        public void AddBannerDetail(LCompanySpecificForms formdata)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/PostCompanySpecificForm", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(formdata);

            var response = _client.Execute<LCompanySpecificForms>(request);

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


        public void Update(LCompanySpecificForms serverData)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/Put/{Id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<LCompanySpecificForms>(request);

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
        public void DeleteAllFormColumns(string ChooseForm,int CompanyId)
        {
            var request = new RestRequest("api/LCompanySpecificColumns/DeleteAllColumns?FormType={FormType}&CompanyId={CompanyId}", Method.DELETE);
            request.AddParameter("FormType", ChooseForm, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<LCompanySpecificColumnViewModel>(request);

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

    interface ILCompanySpecificColumnsRestClient
    {
        //IEnumerable<LCompanySpecificColumnViewModel> GetAll();
        //LCompanySpecificColumnViewModel GetById(int id);
        IEnumerable<LCompanySpecificColumnViewModel> GetClaimsDownloadTemplateByCompanyId(int CompanyId);
        IEnumerable<LCompanySpecificColumnViewModel> GetPayeeColumnsByCompanyId(int CompanyId);
        IEnumerable<LCompanySpecificColumnViewModel> GetPayeeColumnsByCompanyIdForGrid(int CompanyId);
        IEnumerable<LCompanySpecificColumnViewModel> GetClaimsColumnsByCompanyIdForGrid(int CompanyId);
        IEnumerable<LCompanySpecificColumnViewModel> GetCalculationsColumnsByCompanyIdForGrid(int CompanyId);
        IEnumerable<LCompanySpecificColumnViewModel> GetClaimsColumnsByCompanyId(int CompanyId);
        void Add(LCompanySpecificColumnViewModel LCompanySpecificColumnViewModel);
        IEnumerable<LCompanySpecificColumnViewModel> GetPayColumnsByCompanyIdForGrid(int CompanyId);
        //void Update(LCompanySpecificColumnViewModel LCompanySpecificColumnViewModel);
        //void Delete(int id);
        void DeleteAllFormColumns(string ChooseForm, int CompanyId);
        void AddBannerDetail(LCompanySpecificForms formdata);

        IEnumerable<LCompanySpecificForms> getBannerDetail(int companyid, string formname);
        LCompanySpecificForms getBannerDetailByID(int id);
        void Update(LCompanySpecificForms serverData);
    }
}