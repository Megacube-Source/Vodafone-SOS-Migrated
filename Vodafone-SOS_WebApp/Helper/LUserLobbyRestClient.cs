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
    public class LUserLobbyRestClient: ILUserLobbyRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LUserLobbyRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LobbyForCreateUserViewModel> GetUsersFromLobby(string UserType,string CompanyCode)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUserLobby/GetUsersFromLobby?UserType={UserType}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserType", UserType, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LobbyForCreateUserViewModel>>(request);
            return response.Data;
        }
        public LUserViewModel GetLobbyUserById(int Id)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUserLobby/GetFinOpsUserById?Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<LUserViewModel>(request);
            return response.Data;
        }
        //CheckUserInSOSAndCallSP
        public string CheckUserInSOSAndCallSP(string User, int LobbyUserId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUserLobby/CheckUserInSOSAndCallSP?User={User}&LobbyUserId={LobbyUserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("User", User, ParameterType.UrlSegment);
            request.AddParameter("LobbyUserId", LobbyUserId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            return response.Content;
        }
        public LPayeeViewModel GetLobbyPayeeById(int Id)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUserLobby/GetLobbyPayeeById?Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<LPayeeViewModel>(request);
            return response.Data;
        }
        public void RejectUser(int id, string LoggedInUser, int LoggedInRoleId,string RedirectToUrl)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LUserLobby/RejectUser/{Id}?LoggedInUser={LoggedInUser}&LoggedInRoleId={LoggedInRoleId}", Method.GET);
            request.AddParameter("Id", id, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUser", LoggedInUser, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
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
        public int Getlobbycounts(string CompanyCode)
        {
            var request = new RestRequest("api/LUserLobby/Getlobbycounts?CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        public IEnumerable<LobbyUserViewModel> GetLobbyGrid(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery, string CompanyCode)
        {
            var request = new RestRequest("api/LUserLobby/GetlobbylogGridData?CompanyCode={CompanyCode}&pagesize={pagesize}&pagenum={pagenum}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LobbyUserViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

    }
    interface ILUserLobbyRestClient
    {
        IEnumerable<LobbyForCreateUserViewModel> GetUsersFromLobby(string UserType, string CompanyCode);
        LUserViewModel GetLobbyUserById(int Id);
        LPayeeViewModel GetLobbyPayeeById(int Id);
        void RejectUser(int id, string LoggedInUser, int LoggedInRoleId, string RedirectToUrl);
        int Getlobbycounts(string CompanyCode);
        string CheckUserInSOSAndCallSP(string User, int LobbyUserId);
        IEnumerable<LobbyUserViewModel> GetLobbyGrid(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery, string CompanyCode);
    }
}