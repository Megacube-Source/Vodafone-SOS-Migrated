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
    public class LNotificationRestClient : ILNotificationRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LNotificationRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<LNotificationViewModel> GetNotificationByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/LNotification/GetNotificationByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LNotificationViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        //public RProductCodeViewModel GetById(int id)
        //{
        //    var request = new RestRequest("api/RProductCodes/GetRProductCode/{id}", Method.GET) { RequestFormat = DataFormat.Json };

        //    request.AddParameter("id", id, ParameterType.UrlSegment);
        //    var response = _client.Execute<RProductCodeViewModel>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}



        public bool Update(int id, Boolean IsActive )
        {
            LNotificationViewModel serverData = new LNotificationViewModel();
            serverData.id = id;
            if(IsActive == true)
            {
                serverData.IsActive = false;
            }
            else
            {
                serverData.IsActive = true;
            }
           
            serverData.StepName = "";
            serverData.Recipient = "";
            serverData.WorkFlowName = "";

            var request = new RestRequest("api/LNotification/UpdateNotificationId", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddBody(serverData);

            var response = _client.Execute<bool>(request);

            //var request = new RestRequest("api/LNotification/UpdateNotificationId/{id}", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddParameter("id", serverData.id, ParameterType.UrlSegment);
            //request.AddBody(serverData);

            // var response = _client.Execute<int>(request);

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
            return response.Data;

        }


    }
    interface ILNotificationRestClient
    {
        //LNotificationViewModel GetById(int id);
        IEnumerable<LNotificationViewModel> GetNotificationByCompanyId(int CompanyId);
        bool Update(int id, Boolean IsActive);
    }
}