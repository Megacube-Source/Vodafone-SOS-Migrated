using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using Vodafone_SOS_WebApp.Models;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class ADAccountRestClient : IADAccountRestClient
    {
        private readonly RestClient _client;
        private readonly string _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public ADAccountRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public bool CreateUser(LoginViewModel serverData)
        {
            var request = new RestRequest("api/ADAccount/CreateUser", Method.POST) { RequestFormat = DataFormat.Json };
            
            request.AddBody(serverData);
           
            var response = _client.Execute<bool>(request);

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
            if (response.StatusCode == System.Net.HttpStatusCode.NotAcceptable)
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

    
    //ChangeUserPassword

    public bool ChangeUserPassword(ChangePasswordBindingModel serverData)
    {
        var request = new RestRequest("api/ADAccount/ChangeUserPassword", Method.POST) { RequestFormat = DataFormat.Json };

        request.AddBody(serverData);

        var response = _client.Execute<bool>(request);

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
       
        return response.Data;
    }

        public bool LoginUser(LoginViewModel serverData)
        {
            var request = new RestRequest("api/ADAccount/LoginUser", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddBody(serverData);

            var response = _client.Execute<bool>(request);

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
            else if (response.StatusCode == HttpStatusCode.NonAuthoritativeInformation)
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


        public bool UpdateDetails(LoginViewModel serverData)
        {
            var request = new RestRequest("api/ADAccount/UpdateUserDetails", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddBody(serverData);

            var response = _client.Execute<bool>(request);

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
            return response.Data;
        }



        public bool DeleteADUser(LoginViewModel serverData)
        {
            var request = new RestRequest("api/ADAccount/RemoveUser", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddBody(serverData);

            var response = _client.Execute<bool>(request);

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
            else if (response.StatusCode == HttpStatusCode.BadRequest)
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

        public bool EnableDisableADUser(LoginViewModel serverData)
        {
            var request = new RestRequest("api/ADAccount/ActivateDeactivate", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddBody(serverData);

            var response = _client.Execute<bool>(request);

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
            else if (response.StatusCode == HttpStatusCode.BadRequest)
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
        public bool CreateLogin(CreateLoginViewModel serverData)
        {
            var request = new RestRequest("api/ADAccount/CreateLogin", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);
            var response = _client.Execute<bool>(request);
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
            else if (response.StatusCode == HttpStatusCode.NotFound)
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

        public bool UnlockADUser(UnlockUserViewModel serverData)
        {
            var request = new RestRequest("api/ADAccount/UnlockUser", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddBody(serverData);

            var response = _client.Execute<bool>(request);

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
            else if (response.StatusCode == HttpStatusCode.BadRequest)
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


    interface IADAccountRestClient
    {
        bool CreateLogin(CreateLoginViewModel serverData);
        bool CreateUser(LoginViewModel serverData);
        bool ChangeUserPassword(ChangePasswordBindingModel serverData);
        bool LoginUser(LoginViewModel serverData);
        bool UpdateDetails(LoginViewModel serverData);

        bool DeleteADUser(LoginViewModel serverData);

        bool EnableDisableADUser(LoginViewModel serverData);

        bool UnlockADUser(UnlockUserViewModel serverData);
    }
}