using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.Models;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class AccountsRestClient:IAccountsRestClient
    {
        private readonly RestClient _client;
        private readonly string _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public AccountsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public LoginViewModel VerifyMFAOtp(string Email, string MFAOTP)
        {
            var request = new RestRequest("api/Account/VerifyMFAOtp?Email={Email}&MFAOTP={MFAOTP}", Method.POST) { RequestFormat = DataFormat.Json };
            var LogUserEventModel = Globals.GetLogUserEventModel();
            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            request.AddParameter("MFAOTP", (string.IsNullOrEmpty(MFAOTP)) ? string.Empty : MFAOTP, ParameterType.UrlSegment);
            var response = _client.Execute<LoginViewModel>(request);
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
            if (response.Data == null)//Response will be null when user need to reset Password  or user is InActive
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                ex.Data.Add("ErrorMessage", data);
                throw ex;
            }
            return response.Data;

        }


        public LoginViewModel  GetUser(LoginViewModel model)
        {
            var request = new RestRequest("api/Account/UserInfo?HostBrowserDetails={HostBrowserDetails}&HostIP={HostIP}&HostTimeZone={HostTimeZone}&Email={Email}&Password={Password}&MFAOTP={MFAOTP}", Method.GET) { RequestFormat = DataFormat.Json };
            var LogUserEventModel = Globals.GetLogUserEventModel();
            request.AddParameter("Email",model.Email, ParameterType.UrlSegment);
            request.AddParameter("Password",model.Password ,ParameterType.UrlSegment);
            request.AddParameter("HostBrowserDetails", LogUserEventModel.UalHostBrowserDetails, ParameterType.UrlSegment);
            request.AddParameter("HostIP", LogUserEventModel.UalHostIP, ParameterType.UrlSegment);
            request.AddParameter("HostTimeZone", LogUserEventModel.UalHostTimeZone, ParameterType.UrlSegment);
            request.AddParameter("MFAOTP", (string.IsNullOrEmpty(model.MFAOTP))?string.Empty:model.MFAOTP, ParameterType.UrlSegment);
            var response = _client.Execute<LoginViewModel>(request);

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
            if(response.Data==null)//Response will be null when user need to reset Password  or user is InActive
            {
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", response.StatusCode);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                ex.Data.Add("ErrorMessage", data);
                throw ex;
            }
            return response.Data;

        }

        public LoginViewModel GetUserInformation(string  Email)
        {
            var request = new RestRequest("api/Account/GetUserInformation?Email={Email}", Method.GET) { RequestFormat = DataFormat.Json };
            var LogUserEventModel = Globals.GetLogUserEventModel();
            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            var response = _client.Execute<LoginViewModel>(request);

            return response.Data;

        }

        public string ForgotPassowrd(string Email)
        {
            var request = new RestRequest("api/Account/ForgotPassowrd?Email={Email}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            var response = _client.Execute<LoginViewModel>(request);
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
            string source1 = response.Content;
            dynamic data1 = JsonConvert.DeserializeObject(source1);
            return data1;
        }

        public void UpdatePolicyCheck(string UserId)
        {
            var request = new RestRequest("api/Account/UpdatePolicyAccepted?UserId={UserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
        }

        public AspnetUserViewModel GetIdByEmail(string Email)
        {
            var request = new RestRequest("api/Account/GetIdByEmailId?Email={Email}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("Email", Email, ParameterType.UrlSegment);
            var response = _client.Execute<AspnetUserViewModel>(request);
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
            //string source1 = response.Content;
            //dynamic data1 = JsonConvert.DeserializeObject(source1);
            return response.Data;
        }



        public void ForgetPassword(string Email)
         {
             var request = new RestRequest("api/Account/ForgetPassword?Email={Email}", Method.POST) { RequestFormat = DataFormat.Json };

             request.AddParameter("Email", Email, ParameterType.UrlSegment);
             var response = _client.Execute<LoginViewModel>(request);

             if (response.StatusCode != HttpStatusCode.OK)
                 throw new Exception(response.Content);
         }
        public LoginViewModel ResetPassword(string UserId,string Password,string code)
        {
            var request = new RestRequest("api/Account/ResetPassword?UserId={UserId}&Password={Password}&code={code}", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddParameter("UserId",UserId, ParameterType.UrlSegment);
            request.AddParameter("Password",Password, ParameterType.UrlSegment);
            request.AddParameter("code",code, ParameterType.UrlSegment);
            var response = _client.Execute<LoginViewModel>(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception(response.Content);

            return response.Data;
        }

        public RegisterViewModel Register(RegisterViewModel model)
        {
            
            var UserRequest = new RestRequest("api/Account/Register", Method.POST) { RequestFormat = DataFormat.Json };
            UserRequest.AddBody(model);
            var UserResponse = _client.Execute<RegisterViewModel>(UserRequest);
            if (UserResponse.StatusCode == HttpStatusCode.InternalServerError)
            {
                var ex = new Exception(String.Format("{0},{1}", UserResponse.ErrorMessage, UserResponse.StatusCode));
                ex.Data.Add("ErrorCode", UserResponse.StatusCode);
                string source = UserResponse.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;
            }
            if (UserResponse.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception(UserResponse.Content);
            return UserResponse.Data;
        }

        public ChangePasswordViewModel ChangePassword(ChangePasswordViewModel model)
        {

            var UserRequest = new RestRequest("api/Account/ChangePassword", Method.POST) { RequestFormat = DataFormat.Json };
            UserRequest.AddBody(model);
            var UserResponse = _client.Execute<ChangePasswordViewModel>(UserRequest);
            return UserResponse.Data;
        }

        public MAspnetUsersGScurityQuestionViewModel VerifyToken(string UserId,string Token)
        {
            var request = new RestRequest("api/Account/VerifyToken?UserId={UserId}&Token={Token}", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("Token", Token, ParameterType.UrlSegment);
            var response = _client.Execute<MAspnetUsersGScurityQuestionViewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(response.Content);
            return response.Data;

        }
        
        public ChangePasswordViewModel GenerateOTP(ChangePasswordBindingModel model)
        {

            var request = new RestRequest("api/Account/GenerateOTPnSendMail", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(model);
           // request.AddParameter("Email", model.Email, ParameterType.UrlSegment);
            //request.AddParameter("UserId", model.MAuqsqUserId, ParameterType.UrlSegment);
            var response = _client.Execute<ChangePasswordViewModel>(request);
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
        public ChangePasswordViewModel VerifyOTP(string OTP, string Email,string UserId)
        {

            var UserRequest = new RestRequest("api/Account/VerifyOTP?OTP={OTP}&Email={Email}&UserId={UserId}", Method.GET) { RequestFormat = DataFormat.Json };
            UserRequest.AddParameter("OTP", OTP, ParameterType.UrlSegment);
            UserRequest.AddParameter("Email", Email, ParameterType.UrlSegment);
            UserRequest.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            var response = _client.Execute<ChangePasswordViewModel>(UserRequest);
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
            return response.Data;
        }

        //method to get the banner text from db
        public string getBannerText()
        {
            var text = "";            
            var request = new RestRequest("api/Account/getBannerText", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _client.Execute(request);
            text = response.Content.Replace('"', ' ').Trim();
            if (response.StatusCode != HttpStatusCode.OK)
            {                    
                text = "System Outage, Please watch out this space for updates";
            }            
            return text;
        }

        public string GetSelectedLandingPage(string Userid)
        {
            var text = "";
            var request = new RestRequest("api/Account/GetSelectedLandingPage?Userid={Userid}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Userid", Userid, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            text = response.Content.Replace('"', ' ').Trim();
            
            return text;
        }

        public void UpdateSelectedLandingPage(string UserId, string RoleName, int CountryID)
        {
            var request = new RestRequest("api/Account/UpdateSelectedLandingPage?Userid={Userid}&RoleName={RoleName}&CountryID={CountryID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Userid", UserId, ParameterType.UrlSegment);
            request.AddParameter("RoleName", RoleName, ParameterType.UrlSegment);
            request.AddParameter("CountryID", CountryID, ParameterType.UrlSegment);
            var response = _client.Execute(request);

        }



    }


    interface IAccountsRestClient
    {
        void UpdatePolicyCheck(string UserId);
        LoginViewModel VerifyMFAOtp(string Email, string MFAOTP);
        LoginViewModel GetUser(LoginViewModel LoginViewModel);
        LoginViewModel ResetPassword(string UserId, string Password, string code);
        void ForgetPassword(string Email);
        RegisterViewModel Register(RegisterViewModel model);
        ChangePasswordViewModel ChangePassword(ChangePasswordViewModel model);
        string ForgotPassowrd(string Email);
        AspnetUserViewModel GetIdByEmail(string Email);
        MAspnetUsersGScurityQuestionViewModel VerifyToken(string UserId, string Token);
        ChangePasswordViewModel GenerateOTP(ChangePasswordBindingModel model);
        ChangePasswordViewModel VerifyOTP(string OTP,string Email, string UserId);
        LoginViewModel GetUserInformation(string Email);
        string getBannerText();
        string GetSelectedLandingPage(string Userid);
        void UpdateSelectedLandingPage(string UserId, string RoleName, int CountryID);
        // object GetEmail(object email);
        // string VerifyEmail(object email);
    }
}

