using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;


namespace Vodafone_SOS_WebApp.Helper
{
    public class LMessagesRestClient
    {
         private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LMessagesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public IEnumerable<LMessageBoardViewModel> GetMyMessages(string UserID)
        {
            var request = new RestRequest("api/LMessages/GetMyMessages?UserID={UserID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserID", UserID, ParameterType.UrlSegment);
            var response = _client.Execute<List<LMessageBoardViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        public IEnumerable<LMessageBoardViewModel> GetMySentMessages(string UserID)
        {
            var request = new RestRequest("api/LMessages/GetMySentMessages?UserID={UserID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserID", UserID, ParameterType.UrlSegment);
            var response = _client.Execute<List<LMessageBoardViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public Boolean IsMessageBoardAvailable(int RoleID)
        {
            var request = new RestRequest("api/LMessages/IsMessageBoardAvailable?RoleID={RoleID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RoleID", RoleID, ParameterType.UrlSegment);
            var response = _client.Execute<Boolean>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public int GetUnreadMessageCount(string UserId)
        {
            
            var request = new RestRequest("api/LMessages/GetUnreadMessageCount?UserId={UserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return Convert.ToInt32(response);
        }
        public IEnumerable<LMessageBoardViewModel> GetRolesToSendMessage(string RoleID)
        {
            var request = new RestRequest("api/LMessages/GetRoleListToSendMessage?RoleID={RoleID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RoleID", RoleID, ParameterType.UrlSegment);
            var response = _client.Execute<List<LMessageBoardViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public LMessageBoardViewModel GetById(int id)
        {
            var request = new RestRequest("api/LMessages/GetLMessages/{id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LMessageBoardViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public List<string> GetUsersListToSendMessage(string RoleName, string Portfolios, int iCompanyId, string ValueType)
        {
            var request = new RestRequest("api/LMessages/GetUsersListToSendMessage?RoleName={RoleName}&Portfolios={Portfolios}&iCompanyId={iCompanyId}&ValueType={ValueType}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RoleName", RoleName, ParameterType.UrlSegment);
            request.AddParameter("Portfolios", Portfolios, ParameterType.UrlSegment);
            request.AddParameter("iCompanyId", iCompanyId, ParameterType.UrlSegment);
            request.AddParameter("ValueType", ValueType, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);

            return response.Data;
        }

        public void Add(LMessageBoardViewModel serverData)
        {
            var request = new RestRequest("api/LMessages/PostLMessages", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<LMessageBoardViewModel>(request);

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

        public void Update(LMessageBoardViewModel serverData)
        {
            var request = new RestRequest("api/LMessages/PutLMessages/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LMessageBoardViewModel>(request);

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
        public void SetMessageAsRead(LMessageBoardViewModel serverData)
        {
            var request = new RestRequest("api/LMessages/SetMessageAsRead", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);
            var response = _client.Execute(request);

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
        public void Delete(int id)
        {
            var request = new RestRequest("api/LMessages/DeleteLMessages/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LMessageBoardViewModel>(request);
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

    interface ILMessagesRestClient
    {
        IEnumerable<LMessageBoardViewModel> GetMyMessages(string UserID);
        IEnumerable<LMessageBoardViewModel> GetMySentMessages(string UserID);
        Boolean IsMessageBoardAvailable(int RoleID);
        IEnumerable<LMessageBoardViewModel> GetRolesToSendMessage(string RoleID);
        LMessageBoardViewModel GetById(int id);
        List<string> GetUsersListToSendMessage(string RoleName, string Portfolios, int iCompanyId, string ValueType);
        void Add(LMessageBoardViewModel LMessageBoardViewModel);
        void Update(LMessageBoardViewModel LMessageBoardViewModel);
        void Delete(int id);
        void SetMessageAsRead(int MessageId, string UserId);
    }
}

    
