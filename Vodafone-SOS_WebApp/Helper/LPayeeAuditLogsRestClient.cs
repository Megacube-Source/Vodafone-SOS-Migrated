////Code Review for this file (from security perspective) done

//using Newtonsoft.Json;
//using RestSharp;
//using System;
////code review comment using System.Collections.Generic;
//using System.Configuration;
//using System.Net;
//using Vodafone_SOS_WebApp.ViewModels;


//namespace Vodafone_SOS_WebApp.Helper
//{
//    public class LPayeeAuditLogsRestClient:ILPayeeAuditLogsRestClient
//    {
//         private readonly RestClient _client;
//        private readonly string _url = ConfigurationManager.AppSettings["webapibaseurl"];
//        public LPayeeAuditLogsRestClient()
//        {
//            _client = new RestClient { BaseUrl = new System.Uri(_url) };
//        }
//        //public IEnumerable<LPayeeAuditLogViewModel> GetAll()
//        //{
//        //    var request = new RestRequest("api/LPayeeAuditLogs/GetLPayeeAuditLogs", Method.GET) { RequestFormat = DataFormat.Json };

//        //    var response = _client.Execute<List<LPayeeAuditLogViewModel>>(request);

//        //    if (response.Data == null)
//        //        throw new Exception(response.ErrorMessage);

//        //    return response.Data;
//        //}

//        //public IEnumerable<LPayeeAuditLogViewModel> GetByCompanyId(int CompanyId)
//        //{
//        //    var request = new RestRequest("api/LPayeeAuditLogs/GetLPayeeAuditLogsByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
//        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
//        //    var response = _client.Execute<List<LPayeeAuditLogViewModel>>(request);

//        //    if (response.Data == null)
//        //        throw new Exception(response.ErrorMessage);

//        //    return response.Data;
//        //}

//        //public IEnumerable<LPayeeAuditLogViewModel> GetAuditLogUnderStartDateEndDate(DateTime StartDate,DateTime EndDate,int CompanyId)
//        //{
//        //    //Converting date parameter as string for time being will undo this once solution is found
//        //    var StartDateTime = StartDate.ToString("yyyy-MM-dd");
//        //    var EndDateTime = EndDate.ToString("yyyy-MM-dd");
//        //    var request = new RestRequest("api/LPayeeAuditLogs/GetAuditLogUnderStartDateEndDate?CompanyId={CompanyId}&StartDate={StartDate}&EndDate={EndDate}", Method.GET) { RequestFormat = DataFormat.Json };
//        //    request.AddParameter("StartDate", StartDateTime, ParameterType.UrlSegment);
//        //    request.AddParameter("EndDate", EndDateTime, ParameterType.UrlSegment);
//        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
//        //    var response = _client.Execute<List<LPayeeAuditLogViewModel>>(request);
//        //    //var response = _client.Execute(request);
//        //    if (response.Data == null)
//        //        throw new Exception(response.ErrorMessage);
//        //    return response.Data;
//        //}
//        //public LPayeeAuditLogViewModel GetById(int id)
//        //{
//        //    var request = new RestRequest("api/LPayeeAuditLogs/GetLPayeeAuditLog/{id}", Method.GET) { RequestFormat = DataFormat.Json };

//        //    request.AddParameter("id", id, ParameterType.UrlSegment);
//        //    var response = _client.Execute<LPayeeAuditLogViewModel>(request);

//        //    if (response.Data == null)
//        //        throw new Exception(response.ErrorMessage);

//        //    return response.Data;

//        //}
       

//        public void Add(LPayeeAuditLogViewModel serverData)
//        {
//            var request = new RestRequest("api/LPayeeAuditLogs/PostLPayeeAuditLog", Method.POST) { RequestFormat = DataFormat.Json };
//            request.AddBody(serverData);

//            var response = _client.Execute<LPayeeAuditLogViewModel>(request);

//            if (response.StatusCode == HttpStatusCode.InternalServerError)
//            {
//                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
//                ex.Data.Add("ErrorCode", response.StatusCode);
//                string source = response.Content;
//                dynamic data = JsonConvert.DeserializeObject(source);
//                string xx = data.Message;
//                ex.Data.Add("ErrorMessage", xx);
//                throw ex;
//            }
//            if (response.StatusCode == HttpStatusCode.BadRequest)
//            {
//                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
//                ex.Data.Add("ErrorCode", response.StatusCode);
//                string source = response.Content;
//                dynamic data = JsonConvert.DeserializeObject(source);
//                string xx = data.Message;
//                ex.Data.Add("ErrorMessage", xx);
//                throw ex;
//            }
//        }
//        //public void Update(LPayeeAuditLogViewModel serverData)
//        //{
//        //    var request = new RestRequest("api/LPayeeAuditLogs/PutLPayeeAuditLog/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
//        //    request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
//        //    request.AddBody(serverData);

//        //    var response = _client.Execute<LPayeeAuditLogViewModel>(request);

//        //    if (response.StatusCode == HttpStatusCode.NotFound)
//        //    {
//        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
//        //        ex.Data.Add("ErrorCode", response.StatusCode);
//        //        string source = response.Content;
//        //        dynamic data = JsonConvert.DeserializeObject(source);
//        //        string xx = data.Message;
//        //        ex.Data.Add("ErrorMessage", xx);
//        //        throw ex;
//        //    }



//        //    if (response.StatusCode == HttpStatusCode.InternalServerError)
//        //    {
//        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
//        //        ex.Data.Add("ErrorCode", response.StatusCode);
//        //        string source = response.Content;
//        //        dynamic data = JsonConvert.DeserializeObject(source);
//        //        string xx = data.Message;
//        //        ex.Data.Add("ErrorMessage", xx);
//        //        throw ex;
//        //    }
//        //    if (response.StatusCode == HttpStatusCode.BadRequest)
//        //    {
//        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
//        //        ex.Data.Add("ErrorCode", response.StatusCode);
//        //        string source = response.Content;
//        //        dynamic data = JsonConvert.DeserializeObject(source);
//        //        string xx = data.Message;
//        //        ex.Data.Add("ErrorMessage", xx);
//        //        throw ex;
//        //    }

//        //}
//        //public void Delete(int id)
//        //{
//        //    var request = new RestRequest("api/LPayeeAuditLogs/DeleteLPayeeAuditLog/{id}", Method.DELETE);
//        //    request.AddParameter("id", id, ParameterType.UrlSegment);

//        //    var response = _client.Execute<LPayeeAuditLogViewModel>(request);

//        //    if (response.StatusCode != HttpStatusCode.OK)
//        //    {

//        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
//        //        ex.Data.Add("ErrorCode", response.StatusCode);
//        //        string source = response.Content;
//        //        dynamic data = JsonConvert.DeserializeObject(source);
//        //        string xx = data.Message;
//        //        ex.Data.Add("ErrorMessage", xx);
//        //        throw ex;
//        //    }
//        //}

//    }
//    interface ILPayeeAuditLogsRestClient
//    { 
//        //code review comment IEnumerable<LPayeeAuditLogViewModel> GetAll();
//        //code review comment IEnumerable<LPayeeAuditLogViewModel> GetByCompanyId(int CompanyId);
//        //code review comment IEnumerable<LPayeeAuditLogViewModel> GetAuditLogUnderStartDateEndDate(DateTime StartDate, DateTime EndDate, int CompanyId);
//        //code review comment LPayeeAuditLogViewModel GetById(int id);
//        void Add(LPayeeAuditLogViewModel VodafoneAuditTableViewModel);
//        //code review comment void Update(LPayeeAuditLogViewModel VodafoneAuditTableViewModel);
//        //code review comment void Delete(int id);
//    }
//}