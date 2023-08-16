using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class RWorkFlowsRestClient : IRWorkFlowsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public RWorkFlowsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public void Add(RWorkFlowViewModel model)
        {
            var request = new RestRequest("api/RWorkFlows/PostRWorkFlow",Method.POST) { RequestFormat = DataFormat.Json};
            request.AddBody(model);
            var response = _client.Execute<RWorkFlowViewModel>(request);
            if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
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


        public IEnumerable<RWorkFlowViewModel> Get()
        {
            var request = new RestRequest("api/RWorkFlows/GetRWorkFlows", Method.GET) { RequestFormat = DataFormat.Json };
           
            var response = _client.Execute<List<RWorkFlowViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public string GetCRName(string WorkflowName)
        {
            var request = new RestRequest("api/RWorkFlows/GetCRWorkflowname?WorkflowName={WorkflowName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("WorkflowName", WorkflowName, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        public RWorkFlowViewModel GetById(int id)
        {
            var request = new RestRequest("api/RWorkFlows/GetRWorkFlowById/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<RWorkFlowViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void Update(RWorkFlowViewModel serverData)
        {
            var request = new RestRequest("api/RWorkFlows/PutRWorkFlow/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<RWorkFlowViewModel>(request);

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

        public void Delete(int id)
        {
            var request = new RestRequest("api/RWorkFlows/DeleteRWorkFlow/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<RWorkFlowViewModel>(request);

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

        //public int GetCountsForCompletedItems()
        //{
        //    var request = new RestRequest("api/RWorkFlows/GetCountsForCompletedItems", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<int>(request);

        //    return response.Data;
        //}


        //public IEnumerable<RWorkFlowViewModel> GetCompletedItems()
        //{
        //    var request = new RestRequest("api/RWorkFlows/GetCompletedItems", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<RWorkFlowViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //GetExceptionCount
        public List<dynamic> GetCompletedListcolumnlist(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {

            var request = new RestRequest("api/RWorkFlows/GetCompletedListcolumnlist?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&Intervalid={Intervalid}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;

        }
        public int GetCountsForCompletedItems()
        {
            var request = new RestRequest("api/RWorkFlows/GetCountsForCompletedItems", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        //GetExceptionSummary
        public List<dynamic> GetCompletedItems(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {
            var request = new RestRequest("api/RWorkFlows/GetCompletedItems?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

    }


    interface IRWorkFlowsRestClient
    {
        void Add(RWorkFlowViewModel model);
         IEnumerable<RWorkFlowViewModel> Get();
        void Delete(int id);
        void Update(RWorkFlowViewModel serverData);
        RWorkFlowViewModel GetById(int id);
        string GetCRName(string WorkflowName);
        int GetCountsForCompletedItems();
        List<dynamic> GetCompletedItems(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery);
        List<dynamic> GetCompletedListcolumnlist(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery);
    }

}