using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Vodafone_SOS_WebApp.Models;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class GSecurityQuestionsRestClient : IGSecurityQuestionsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
       

        public GSecurityQuestionsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
       
        //GET: Method to Get Question Answer in Reset Password from Api
        public IEnumerable<ChangePasswordBindingModel> GetSecurityQuestions()
        {
            var request = new RestRequest("api/GSecurityQuestions/GetGSecurityQuestions", Method.GET) { RequestFormat = DataFormat.Json };



            var response = _client.Execute<List<ChangePasswordBindingModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        //POST Method to Post Question Answer in Reset Password From  Api to Db 
        public IEnumerable<MAspnetUsersGScurityQuestionViewModel> AddQuestionAnswers(ChangePasswordBindingModel serverData)
        {
            
            var request = new RestRequest("api/GSecurityQuestions/PostQuestionAnswers", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddBody(serverData);

            var response = _client.Execute<List<MAspnetUsersGScurityQuestionViewModel>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        
        //GET: Method to Get Entered Question Answer Detail by User from db 
        public IEnumerable<MAspnetUsersGScurityQuestionViewModel> GetQuestionAnswersByUserId(string userid)
        {
            var request = new RestRequest("api/GSecurityQuestions/GetQuestionAnswersByUser?userid={userid}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("userid", userid, ParameterType.UrlSegment);
            var response = _client.Execute<List<MAspnetUsersGScurityQuestionViewModel>>(request);
            return response.Data;
        }
        //PUT: Method to update Question Answer by user 
        public void PutQuestionAnswer(string userid, ChangePasswordBindingModel model)
        {
            var request = new RestRequest("api/GSecurityQuestions/PutQuestionAnswers?userid={userid}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("userid", userid, ParameterType.UrlSegment);
            request.AddBody(model);
            var response = _client.Execute<List<MAspnetUsersGScurityQuestionViewModel>>(request);
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
        ////GET: Method to Get userid
        //    public string GetEmail(string Email)
        //    {
        //        var request = new RestRequest("api/Account/GetEmailFor?Email={Email}", Method.GET) { RequestFormat = DataFormat.Json };

        //        request.AddParameter("Email", Email, ParameterType.UrlSegment);
        //        var response = _client.Execute<LoginViewModel>(request);
        //        if (response.StatusCode == HttpStatusCode.BadRequest)
        //        {
        //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //            ex.Data.Add("ErrorCode", response.StatusCode);
        //            string source = response.Content;
        //            dynamic data = JsonConvert.DeserializeObject(source);
        //            string xx = data.Message;
        //            ex.Data.Add("ErrorMessage", xx);
        //            throw ex;
        //        }
        //        string source1 = response.Content;
        //        dynamic data1 = JsonConvert.DeserializeObject(source1);
        //        return data1;
        //    }


        



    }
    interface IGSecurityQuestionsRestClient
    {
        IEnumerable<ChangePasswordBindingModel> GetSecurityQuestions();
        void PutQuestionAnswer(string userid, ChangePasswordBindingModel model);
         IEnumerable<MAspnetUsersGScurityQuestionViewModel> AddQuestionAnswers(ChangePasswordBindingModel serverData);
        IEnumerable<MAspnetUsersGScurityQuestionViewModel> GetQuestionAnswersByUserId(string userid);

        //string GetEmail(string Email);
    }
}
