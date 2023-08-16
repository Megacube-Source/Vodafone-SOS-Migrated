//Code Review for this file (from security perspective) done

using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
//Code review comment using System.Linq;
using System.Net;
//Code review comment using System.Web;
//using System.Web.Mvc;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{

    public class SupportSystemRestClient:ISupportSystemRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public SupportSystemRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        #region Category
        public void AddCategory(SupportSystemCategoriesViewModel serverData)
        {
            var request = new RestRequest("api/SupportSystem/PostSupportCategory", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<SupportSystemCategoriesViewModel>(request);

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

        public IEnumerable<SupportSystemCategoriesViewModel> GetCategoryList(int CompanyId)
        {
            var request = new RestRequest("api/SupportSystem/GetSupportCategoriesDropdownData?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<SupportSystemCategoriesViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<SupportSystemCategoriesViewModel> GetAllSupportCategories()
        {
            var request = new RestRequest("api/SupportSystem/GetAllSupportCategories", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<SupportSystemCategoriesViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public SupportSystemCategoriesViewModel GetCategoryById(int id)
        {
            var request = new RestRequest("api/SupportSystem/GetSupportCategory/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<SupportSystemCategoriesViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public void UpdateCategory(SupportSystemCategoriesViewModel serverData)
        {
            var request = new RestRequest("api/SupportSystem/UpdateCategory/{id}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<SupportSystemCategoriesViewModel>(request);

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
        public void DeleteCategory(int id)
        {
            var request = new RestRequest("api/SupportSystem/DeleteCategory/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<SupportSystemCategoriesViewModel>(request);

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
        #endregion

        #region QuickTicket
        public IEnumerable<SupportSystemQuickTicketsViewModel> GetAllSupportQuickTickets()
        {
            var request = new RestRequest("api/SupportSystem/GetAllSupportQuickTickets", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<SupportSystemQuickTicketsViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public void AddQuickTicket(SupportSystemQuickTicketsViewModel serverData)
        {
            var request = new RestRequest("api/SupportSystem/PostSupportQuickTicket", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<SupportSystemQuickTicketsViewModel>(request);

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
        public SupportSystemQuickTicketsViewModel GetQuickTicketById(int id)
        {
            var request = new RestRequest("api/SupportSystem/GetSupportQuickTicket/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<SupportSystemQuickTicketsViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public IEnumerable<SupportSystemQuickTicketsViewModel> GetQuickTicketList()
        {
            var request = new RestRequest("api/SupportSystem/GetQuickTicketDropdownData", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<SupportSystemQuickTicketsViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public void UpdateQuickTicket(SupportSystemQuickTicketsViewModel serverData)
        {
            var request = new RestRequest("api/SupportSystem/UpdateQuickTicket/{id}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<SupportSystemQuickTicketsViewModel>(request);

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
        public void DeleteQuickTicket(int id)
        {
            var request = new RestRequest("api/SupportSystem/DeleteQuickTicket/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<SupportSystemQuickTicketsViewModel>(request);

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

        #endregion

        #region TicketStages
        public void AddTicketStage(SupportSystemStagesViewModel serverData)
        {
            var request = new RestRequest("api/SupportSystem/PostTicketStages", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<SupportSystemStagesViewModel>(request);

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

        public IEnumerable<SupportSystemStagesViewModel> GetTicketStagesList()
        {
            var request = new RestRequest("api/SupportSystem/GetSupportTicketStagesDropdownData", Method.GET) { RequestFormat = DataFormat.Json };
            //var request = new RestRequest("api/SupportSystem/GetAllRTicketStages", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _client.Execute<List<SupportSystemStagesViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<SupportSystemStagesViewModel> GetAllTicketStages()
        {
            var request = new RestRequest("api/SupportSystem/GetAllRTicketStagesList", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<SupportSystemStagesViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public SupportSystemStagesViewModel GetTicketStageById(int id)
        {
            var request = new RestRequest("api/SupportSystem/GetRTicketStageDetails/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<SupportSystemStagesViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public void UpdateTicketStage(SupportSystemStagesViewModel serverData)
        {
            var request = new RestRequest("api/SupportSystem/UpdateTicketStage/{id}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<SupportSystemStagesViewModel>(request);

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
        public void DeleteTktStage(int id)
        {
            var request = new RestRequest("api/SupportSystem/DeleteRTktStage/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<SupportSystemStagesViewModel>(request);

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
        #endregion
    }
    public interface ISupportSystemRestClient
    {
        void AddCategory(SupportSystemCategoriesViewModel serverData);
        IEnumerable<SupportSystemCategoriesViewModel> GetCategoryList(int CompanyId);
        IEnumerable<SupportSystemCategoriesViewModel> GetAllSupportCategories();
        SupportSystemCategoriesViewModel GetCategoryById(int id);
        void UpdateCategory(SupportSystemCategoriesViewModel serverData);
        void DeleteCategory(int id);
        void AddQuickTicket(SupportSystemQuickTicketsViewModel serverData);
        IEnumerable<SupportSystemQuickTicketsViewModel> GetAllSupportQuickTickets();
        SupportSystemQuickTicketsViewModel GetQuickTicketById(int id);
        void UpdateQuickTicket(SupportSystemQuickTicketsViewModel serverData);
        void DeleteQuickTicket(int id);
        IEnumerable<SupportSystemQuickTicketsViewModel> GetQuickTicketList();



        void AddTicketStage(SupportSystemStagesViewModel serverData);
        IEnumerable<SupportSystemStagesViewModel> GetTicketStagesList();
        IEnumerable<SupportSystemStagesViewModel> GetAllTicketStages();
        SupportSystemStagesViewModel GetTicketStageById(int id);
        void UpdateTicketStage(SupportSystemStagesViewModel serverData);
        void DeleteTktStage(int id);
    }
}