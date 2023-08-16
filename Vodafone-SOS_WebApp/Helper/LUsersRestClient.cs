//Code Review for this file (from security perspective) done

using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using Vodafone_SOS_WebApp.Utilities;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LUsersRestClient:ILUsersRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LUsersRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

       

        //public IEnumerable<LUserViewModel> GetDropDownDataByCompanyId(int CompanyId)
        //{
        //    var request = new RestRequest("api/LUsers/GetLUsersDropdownData?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LUserViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public IEnumerable<LUserViewModel> GetByCompanyIdStatusName(int CompanyId,string Status,string ReportsToId)
        //{
        //    var request = new RestRequest("api/LUsers/GetLUsersByCompanyIdStatus?CompanyId={CompanyId}&Status={Status}&ReportsToId={ReportsToId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("Status", Status, ParameterType.UrlSegment);
        //    request.AddParameter("ReportsToId", ReportsToId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LUserViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public IEnumerable<AspnetRoleViewModel> GetUserRoles(string UserId,string CompanyCode)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUsers/GetLUserRoles?UserId={UserId}&CompanyCode={CompanyCode}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetRoleViewModel>>(request);

            return response.Data;
        }

        //GetLUserRolesbyId
        public IEnumerable<AspnetRoleViewModel> GetLUserRolesbyId(string UserId, string CompanyCode)
        {
            //string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            //if (string.IsNullOrEmpty(Workflow))
            //{
            //    Workflow = "No Workflow";
            //}
           // string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUsers/GetLUserRolesbyId?UserId={UserId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetRoleViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<AspnetRoleViewModel> GetUserRoleByIdForMissingPortfolio(string UserId, string CompanyCode)
        {
            //string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            //if (string.IsNullOrEmpty(Workflow))
            //{
            //    Workflow = "No Workflow";
            //}
            // string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUsers/GetUserRoleByIdForMissingPortfolio?UserId={UserId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetRoleViewModel>>(request);

            return response.Data;
        }

        public string GetUserRoleIdbyRoleName(string rolename, string CompanyCode)
        {
            
            var request = new RestRequest("api/LUsers/GetUserRoleIdbyRoleName?rolename={rolename}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("rolename", rolename, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetRoleViewModel>>(request);

            return response.Content;
        }
        //GetAllEmailId
        public IEnumerable<LUserViewModel> GetAllEmailId(int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }           
            var request = new RestRequest("api/LUsers/GetAllEmailId?CompanyId={CompanyId}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);         
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LUserViewModel>>(request);
            return response.Data;
        }

        public IEnumerable<AspnetRoleViewModel> GetUserPayeeRoles(string UserId, string CompanyCode)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUsers/GetLUserPayeeRoles?UserId={UserId}&CompanyCode={CompanyCode}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<AspnetRoleViewModel>>(request);

            return response.Data;
        }
        public LUserViewModel GetUserDetailsByAspNetUserID(string strUserID)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUsers/GetUserDetailsByAspNetUserID?strUserID={strUserID}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("strUserID", strUserID, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<LUserViewModel>(request);
            return response.Data;
        }

        //public IEnumerable<LUserViewModel> GetByCompanyIdStatusNameCreatedByUserId(int CompanyId,string Status,string CreatedByUserId)
        //{
        //    var request = new RestRequest("api/LUsers/GetLUsersByCompanyIdStatusCreatedByUserId?CompanyId={CompanyId}&Status={Status}&CreatedByUserId={CreatedByUserId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("Status", Status, ParameterType.UrlSegment);
        //    request.AddParameter("CreatedByUserId", CreatedByUserId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LUserViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
        public LUserViewModel GetById(int id)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUsers/GetLUser/{id}?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LUserViewModel>(request);

            return response.Data;
        }

        public string GetEmailVerified(string EmailID, int CompanyID, string radioValue)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUsers/GetEmailVerified?EmailID={EmailID}&CompanyID={CompanyID}&radioValue= {radioValue}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EmailID", EmailID, ParameterType.UrlSegment);
            request.AddParameter("CompanyID", CompanyID, ParameterType.UrlSegment); 
            request.AddParameter("radioValue", radioValue, ParameterType.UrlSegment); 
             var response = _client.Execute<dynamic>(request);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {

                return "User/Payee not found in opco.";
            }
            if (response.StatusCode == HttpStatusCode.SeeOther)
            {

                return "User/Payee is not Active";
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {

                return "Error";
            }
            return Convert.ToString(response.Data);
        }

        public string SavePortfolioedForAllRolesByUserEmailID(string EmailID, int CompanyID, string radioValue)
        {
            var request = new RestRequest("api/LUsers/GetEmailVerified?EmailID={EmailID}&CompanyID={CompanyID}&radioValue= {radioValue}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EmailID", EmailID, ParameterType.UrlSegment);
            request.AddParameter("CompanyID", CompanyID, ParameterType.UrlSegment);
            request.AddParameter("radioValue", radioValue, ParameterType.UrlSegment); 
            var response = _client.Execute<dynamic>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {


                string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
                var request1 = new RestRequest("api/LUsers/SavePortfolioedForAllRolesByUserEmailID?EmailID={EmailID}&CompanyID={CompanyID}&radioValue={radioValue}", Method.GET) { RequestFormat = DataFormat.Json };
                request1.AddParameter("EmailID", EmailID, ParameterType.UrlSegment);
                request1.AddParameter("CompanyID", CompanyID, ParameterType.UrlSegment);
                request1.AddParameter("radioValue", radioValue, ParameterType.UrlSegment);
                var response1 = _client.Execute<dynamic>(request1);
                if (response1.StatusCode != HttpStatusCode.OK)
                {

                    return "Error1";
                }
                return Convert.ToString("Success");
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {

                    return "User/Payee not found in opco.";
                }
                else if (response.StatusCode == HttpStatusCode.SeeOther)
                {

                    return "User/Payee is not Active";
                }
                else 
                {

                    return "Error";
                }
            }
        }

        


        public int Add(LUserViewModel serverData,string LoggedInRoleId,string WorkflowName,string AttachmentPath,string RedirectToUrl,bool CheckDuplicateUser,string Source,string UserLobbyId)
        {
            //Adding PortfolioList to model
            if(serverData.SamePortfoliosForAllRoles)
            {
                serverData.ParameterCarrier = serverData.PortfolioList;
            }
            else
            {
                serverData.ParameterCarrier = serverData.RoleBasedPortfolios;
            }
            //string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(WorkflowName))
            {
                WorkflowName = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUsers/PostLUser?LoggedInRoleId={LoggedInRoleId}&UserName={UserName}&Workflow={Workflow}&AttachmentPath={AttachmentPath}&SamePortfoliosForAllRoles={SamePortfoliosForAllRoles}&CheckDuplicateUser={CheckDuplicateUser}&Source={Source}&UserLobbyId={UserLobbyId}", Method.POST) { RequestFormat = DataFormat.Json };
           // request.AddParameter("PortfolioList", (string.IsNullOrEmpty(serverData.PortfolioList))?"":serverData.PortfolioList ,ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", WorkflowName, ParameterType.UrlSegment);
           // request.AddParameter("RoleBasedPortfolios", (string.IsNullOrEmpty(serverData.RoleBasedPortfolios)) ? "" : serverData.RoleBasedPortfolios, ParameterType.UrlSegment);
            request.AddParameter("AttachmentPath", (string.IsNullOrEmpty(AttachmentPath))?"":AttachmentPath, ParameterType.UrlSegment);
            request.AddParameter("SamePortfoliosForAllRoles", serverData.SamePortfoliosForAllRoles, ParameterType.UrlSegment);
            request.AddParameter("CheckDuplicateUser", CheckDuplicateUser, ParameterType.UrlSegment);
            request.AddParameter("Source", Source, ParameterType.UrlSegment);
            request.AddParameter("UserLobbyId", UserLobbyId, ParameterType.UrlSegment);
            //UserLobbyId
            request.AddBody(serverData);
            var response = _client.Execute<int>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                //call globals method to generate exception based on response
                Globals.GenerateException(response,RedirectToUrl);
            }
            return response.Data;
        }

        public void Update(LUserViewModel serverData,string AttachmentPath,string RedirectToUrl)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            //var Roles = string.Join(",",System.Web.HttpContext.Current.Session["Roles"] as List<string>);
            var request = new RestRequest("api/LUsers/PutLUser/{id}?Roles={Roles}&Atachments={Atachments}&PortfolioList={PortfolioList}&AttachmentPath={AttachmentPath}&UserName={UserName}&Workflow={Workflow}&RoleBasedPortfolios={RoleBasedPortfolios}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddParameter("Atachments", serverData.FileNames, ParameterType.UrlSegment);
            request.AddParameter("Roles", serverData.RoleList, ParameterType.UrlSegment);
            request.AddParameter("AttachmentPath", AttachmentPath, ParameterType.UrlSegment);
            request.AddParameter("PortfolioList", serverData.PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("RoleBasedPortfolios", (string.IsNullOrEmpty(serverData.RoleBasedPortfolios)) ? "" : serverData.RoleBasedPortfolios, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LUserViewModel>(request);


            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                if (string.IsNullOrEmpty(RedirectToUrl))
                {
                    RedirectToUrl = "/Home/ErrorPage";
                }
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", (int)response.StatusCode);
                ex.Data.Add("RedirectToUrl", RedirectToUrl);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }

        }

        //public void UpdateUserStatus(LUserViewModel serverData)
        //{
        //    var request = new RestRequest("api/LUsers/PutUpdateUserStatus", Method.PUT) { RequestFormat = DataFormat.Json };
        //    request.AddBody(serverData);

        //    var response = _client.Execute<LUserViewModel>(request);

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
        //        string xx = data.ExceptionMessage;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }
        //    if (response.StatusCode == HttpStatusCode.BadRequest)
        //    {
        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.ExceptionMessage;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }

        //}

        public void Delete(int id,string Comments,string RedirectToUrl)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUsers/DeleteLUser/{Id}?UserName={UserName}&Workflow={Workflow}&Comments={Comments}", Method.DELETE);
            request.AddParameter("Id", id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                if (string.IsNullOrEmpty(RedirectToUrl))
                {
                    RedirectToUrl = "/Home/ErrorPage";
                }
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", (int)response.StatusCode);
                ex.Data.Add("RedirectToUrl", RedirectToUrl);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }

        }
        public DataTable ValidateUserUpload(string FileName, string UserName, string LoggedInRoleId, int iCompanyId)
        {
            
            var request = new RestRequest("api/LUsers/ValidateUploadUser?FileName={FileName}&UserName={UserName}&LoggedInRoleId={LoggedInRoleId}&iCompanyId={iCompanyId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("FileName", FileName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("iCompanyId", iCompanyId, ParameterType.UrlSegment);

            var response = _client.Execute<dynamic>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", (int)response.StatusCode);
                ex.Data.Add("RedirectToUrl", "/Home/ErrorPage");
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }
            var res = JsonConvert.DeserializeObject<DataTable>(response.Content);
            return res;
        }
        public DataTable UploadValidUsers( string UserName, string LoggedInRoleId, int iCompanyId)
        {

            var request = new RestRequest("api/LUsers/UploadUser?FileName={FileName}&UserName={UserName}&LoggedInRoleId={LoggedInRoleId}&iCompanyId={iCompanyId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("FileName", "", ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("iCompanyId", iCompanyId, ParameterType.UrlSegment);

            var response = _client.Execute<dynamic>(request);

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {

                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", (int)response.StatusCode);
                ex.Data.Add("RedirectToUrl", "/Home/ErrorPage");
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }
            var res = JsonConvert.DeserializeObject<DataTable>(response.Content);
            return res;
        }

        public string GetSudmitableorNot(int Transactionid, string Workflow, string Role, int CompanyID)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GenericGrid/GetSudmitableornot?Transactionid={Transactionid}&Role={Role}&Workflow={Workflow}&CompanyID={CompanyID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Transactionid", Transactionid, ParameterType.UrlSegment);
            request.AddParameter("Role", Role, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("CompanyID", CompanyID, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            var res = response.Data.ToString();
            return res;

        }
        public string GetMyUsersReport(string UserHexID, int CompanyID, string CompanyCode, string LoggedInUserName,int LoggedRoleId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUsers/GetMyUsersReportData?UserHexID={UserHexID}&CompanyID={CompanyID}&CompanyCode={CompanyCode}&LoggedInUserName={LoggedInUserName}&LoggedRoleId={LoggedRoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserHexID", UserHexID, ParameterType.UrlSegment);
            request.AddParameter("CompanyID", CompanyID, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserName", LoggedInUserName, ParameterType.UrlSegment);
            request.AddParameter("LoggedRoleId", LoggedRoleId, ParameterType.UrlSegment);
            
            var response = _client.Execute<dynamic>(request);
            var res = response.Data.ToString();
            return res;

        }

        public void SavePortfolios(string SelectedPortfolios, int UserId, string RoleId, string RedirectToUrl)
        {
            
            var request = new RestRequest("api/LUsers/SavePortfolios/?SelectedPortfolios={SelectedPortfolios}&UserId={UserId}&RoleId={RoleId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("SelectedPortfolios", SelectedPortfolios, ParameterType.UrlSegment);
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            //request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                if (string.IsNullOrEmpty(RedirectToUrl))
                {
                    RedirectToUrl = "/Home/ErrorPage";
                }
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", (int)response.StatusCode);
                ex.Data.Add("RedirectToUrl", RedirectToUrl);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }

        }

        public void SaveMissingPortfolios(string SelectedPortfolios, int UserId, string RoleId, string RedirectToUrl)
        {

            var request = new RestRequest("api/LUsers/SaveMissingPortfolios/?SelectedPortfolios={SelectedPortfolios}&UserId={UserId}&RoleId={RoleId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("SelectedPortfolios", SelectedPortfolios, ParameterType.UrlSegment);
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            //request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.NoContent)
            {
                if (string.IsNullOrEmpty(RedirectToUrl))
                {
                    RedirectToUrl = "/Home/ErrorPage";
                }
                var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
                ex.Data.Add("ErrorCode", (int)response.StatusCode);
                ex.Data.Add("RedirectToUrl", RedirectToUrl);
                string source = response.Content;
                dynamic data = JsonConvert.DeserializeObject(source);
                string xx = data.Message;
                ex.Data.Add("ErrorMessage", xx);
                throw ex;

            }

        }


    }

    interface ILUsersRestClient
    {
        LUserViewModel GetById(int id);
        //code review comment void UpdateUserStatus(LUserViewModel serverData);
        IEnumerable<AspnetRoleViewModel> GetUserRoles(string UserId, string CompanyCode);
        IEnumerable<AspnetRoleViewModel> GetUserPayeeRoles(string UserId, string CompanyCode);
        LUserViewModel GetUserDetailsByAspNetUserID(string strUserID);
        //code review comment IEnumerable<LUserViewModel> GetDropDownDataByCompanyId(int CompanyId);
        //code review comment IEnumerable<LUserViewModel> GetByCompanyIdStatusName(int CompanyId, string Status, string ReportsToId);
        //code review comment IEnumerable<LUserViewModel> GetByCompanyIdStatusNameCreatedByUserId(int CompanyId, string Status, string CreatedByUserId);
        int Add(LUserViewModel serverData, string LoggedInRoleId, string WorkflowName, string AttachmentPath, string RedirectToUrl,bool CheckDuplicateUser,string Source,string UserLobbyId);
        void Update(LUserViewModel serverData, string AttachmentPath,string RedirectToUrl);
        void Delete(int id,string Comments,string RedirectToUrl);
        DataTable ValidateUserUpload(string FileName, string UserName, string LoggedInRoleId, int iCompanyId);
        DataTable UploadValidUsers(string UserName, string LoggedInRoleId, int iCompanyId);
        string GetSudmitableorNot(int Transactionid, string Workflow, string Role, int CompanyID);
        string GetMyUsersReport(string UserHexID, int CompanyID, string CompanyCode, string LoggedInUserName, int LoggedRoleId);
        IEnumerable<LUserViewModel> GetAllEmailId(int CompanyId);
        IEnumerable<AspnetRoleViewModel> GetLUserRolesbyId(string UserId, string CompanyCode);
        string GetUserRoleIdbyRoleName(string rolename, string CompanyCode);
        void SavePortfolios(string SelectedPortfolios, int UserId, string RoleId, string RedirectToUrl);

        string GetEmailVerified(string id, int CompanyId, string radioValue);

        string SavePortfolioedForAllRolesByUserEmailID(string id, int CompanyId, string radioValue);

        void SaveMissingPortfolios(string SelectedPortfolios, int UserId, string RoleId, string RedirectToUrl);

        IEnumerable<AspnetRoleViewModel> GetUserRoleByIdForMissingPortfolio(string UserId, string CompanyCode);
        //GetRoleById
    }
}