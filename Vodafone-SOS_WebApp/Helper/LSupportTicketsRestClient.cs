//Code Review for this file (from security perspective) done

using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;
//code review comment using Newtonsoft.Json.Linq;

namespace Vodafone_SOS_WebApp.Helper
{
    
    public class LSupportTicketsRestClient:ILSupportTicketsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public LSupportTicketsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        //public IEnumerable<LSupportTicketsViewModel> GetAll()
        //{
        //    var request = new RestRequest("api/", Method.GET) { RequestFormat = DataFormat.Json };
        //    var response = _client.Execute<List<LSupportTicketsViewModel>>(request);
        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);
        //    return response.Data;
        //}
        public LSupportTicketContextModel GetById(int id)
        {
            var request = new RestRequest("api/LSupportTickets/GetTicketDetails/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LSupportTicketContextModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            
            return response.Data;
        }

        public void ExecuteMassActions(string Action, string TransactionIdList, string LoggedInUserId, string Comments)
        {
            var request = new RestRequest("api/LSupportTickets/MassActionUpdate?Action={Action}&TransactionIdList={TransactionIdList}&LoggedInUserId={LoggedInUserId}&Comments={Comments}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Action", Action, ParameterType.UrlSegment);
            request.AddParameter("TransactionIdList", TransactionIdList, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
            var response = _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(response.ErrorMessage);
        }

        public void ExecuteMassSelfAssignsL2(string TransactionIdList, string LoggedInUserId, string Comments)
        {
            var request = new RestRequest("api/LSupportTickets/MassSelfAssignL2?TransactionIdList={TransactionIdList}&LoggedInUserId={LoggedInUserId}&Comments={Comments}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("TransactionIdList", TransactionIdList, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
            var response = _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(response.ErrorMessage);
        }

        public string  CreateNewTicket(LSupportTicketContextModel serverData,string FileName,string FilePath,string PortfolioList)
        {
            
            var request = new RestRequest("api/LSupportTickets/CreateTicket?FileName={FileName}&FilePath={FilePath}&PortfolioList={PortfolioList}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("FileName", string.IsNullOrEmpty(FileName)?"":FileName, ParameterType.UrlSegment);
            request.AddParameter("FilePath", string.IsNullOrEmpty(FilePath) ? "" : FilePath, ParameterType.UrlSegment);
            request.AddParameter("PortfolioList", string.IsNullOrEmpty(PortfolioList) ? "" : PortfolioList, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LSupportTicketContextModel>(request);

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
            string Content = response.Content;
            dynamic Res = JsonConvert.DeserializeObject(Content);
            return Res;
        }

        public void UpdateTicket(LSupportTicketContextModel serverData,string StrUpdateType, string FileName, string FilePath, string PortfolioList)
        {
            var request = new RestRequest(Method.POST) { RequestFormat = DataFormat.Json };
            if (StrUpdateType == "Update")
            {
                request.Resource = "api/LSupportTickets/UpdateTicket/{id}?FileName={FileName}&FilePath={FilePath}&PortfolioList={PortfolioList}";
                request.AddParameter("FileName", string.IsNullOrEmpty(FileName) ? "" : FileName, ParameterType.UrlSegment);
                request.AddParameter("FilePath", string.IsNullOrEmpty(FilePath) ? "" : FilePath, ParameterType.UrlSegment);
                request.AddParameter("PortfolioList", string.IsNullOrEmpty(PortfolioList) ? "" : PortfolioList, ParameterType.UrlSegment);
            }
            if (StrUpdateType == "Forward") request.Resource = "api/LSupportTickets/ForwardTicket/{id}";
            if (StrUpdateType == "BackToL1") request.Resource = "api/LSupportTickets/BackToL1Ticket/{id}";
            if (StrUpdateType == "BackToRequestor") request.Resource = "api/LSupportTickets/BackToRequestorTicket/{id}";
            if (StrUpdateType == "SelfAssign") request.Resource = "api/LSupportTickets/SelftAssignTicket/{id}";
            if (StrUpdateType == "AssignedToL2") request.Resource = "api/LSupportTickets/AssignTicketToL2/{id}";
            //var request = new RestRequest("api/LSupportTickets/UpdateTicket/{id}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            //request.AddParameter("AssigneeId", serverData.LstaAssignedToId, ParameterType.UrlSegment);
            //request.AddParameter("LoggedInUserId", System.Web.HttpContext.Current.Session["UserId"].ToString(), ParameterType.UrlSegment);
            request.AddBody(serverData);
            var response = _client.Execute<LSupportTicketContextModel>(request);
            //if (response.StatusCode == HttpStatusCode.NotFound)
            //{
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}
            //if (response.StatusCode == HttpStatusCode.InternalServerError)
            //{
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}
            //if (response.StatusCode == HttpStatusCode.BadRequest)
            //{
            //    var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //    ex.Data.Add("ErrorCode", response.StatusCode);
            //    string source = response.Content;
            //    dynamic data = JsonConvert.DeserializeObject(source);
            //    string xx = data.Message;
            //    ex.Data.Add("ErrorMessage", xx);
            //    throw ex;
            //}

        }

        public string UpdateAttachment(int id ,string CreatedBy, string FileName, string FilePath, string Type)
        {
                var request = new RestRequest(Method.GET) { RequestFormat = DataFormat.Json };         
                request.Resource = "api/LSupportTickets/UpdateAttachmentTicket?id={id}&FileName={FileName}&FilePath={FilePath}&CreatedBy={CreatedBy}&Type={Type}";
                request.AddParameter("id", id, ParameterType.UrlSegment);
                request.AddParameter("FileName", string.IsNullOrEmpty(FileName) ? "" : FileName, ParameterType.UrlSegment);
                request.AddParameter("FilePath", string.IsNullOrEmpty(FilePath) ? "" : FilePath, ParameterType.UrlSegment);
                request.AddParameter("CreatedBy", string.IsNullOrEmpty(CreatedBy) ? "" : CreatedBy, ParameterType.UrlSegment);
                request.AddParameter("Type", string.IsNullOrEmpty(Type) ? "" : Type, ParameterType.UrlSegment);
                var response = _client.Execute<dynamic>(request);
                return Convert.ToString(response.Content);
        }
            //public void DeleteTicket(int id)
            //{
            //    var request = new RestRequest("api/{id}", Method.DELETE);
            //    request.AddParameter("id", id, ParameterType.UrlSegment);
            //    var response = _client.Execute<LSupportTicketsViewModel>(request);
            //    if (response.StatusCode != HttpStatusCode.OK)
            //    {

            //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
            //        ex.Data.Add("ErrorCode", response.StatusCode);
            //        string source = response.Content;
            //        dynamic data = JsonConvert.DeserializeObject(source);
            //        string xx = data.Message;
            //        ex.Data.Add("ErrorMessage", xx);
            //        throw ex;
            //    }
            //}
            public IEnumerable<LSupportTicketsViewModel> GetRequestorTicket(string strUserID,int CompanyId,string RoleId)
        {
            var request = new RestRequest("api/LSupportTickets/GetRequestorTickets?strUserId={strUserId}&CompanyId={CompanyId}&RoleId={RoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("strUserId", strUserID, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LSupportTicketsViewModel>>(request);
            return response.Data;
        }

        public string DownloadTicket(string strUserID, int CompanyId, string RoleId,string TabName)
        {
            var request = new RestRequest("api/LSupportTickets/DownloadSupportTickets?strUserId={strUserId}&CompanyId={CompanyId}&RoleId={RoleId}&TabName={TabName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("strUserId", strUserID, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            request.AddParameter("TabName", TabName, ParameterType.UrlSegment);
            var response = _client.Execute<List<LSupportTicketsViewModel>>(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        public IEnumerable<LSupportTicketsViewModel> GetSystemAnalystTicket(string strUserID,int CompanyId, string RoleId)
        {
            var request = new RestRequest("api/LSupportTickets/GetSystemAnalystTickets?strUserId={strUserId}&CompanyId={CompanyId}&RoleId={RoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("strUserId", strUserID, ParameterType.UrlSegment);
           request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LSupportTicketsViewModel>>(request);
            
            return response.Data;
        }
        public IEnumerable<LSupportTicketsViewModel> GetL2SupportTicket(string strUserID,int CompanyId,string RoleId)
        {
            var request = new RestRequest("api/LSupportTickets/GetL2SupportTickets?strUserId={strUserId}&CompanyId={CompanyId}&RoleId={RoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("strUserId", strUserID, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LSupportTicketsViewModel>>(request);
            
            return response.Data;
        }

        //public IEnumerable<LSupportTicketsViewModel> GetAllickets(string strUserID,int CompanyId)
        //{
        //    var request = new RestRequest("api/LSupportTickets/GetAllTickets?strUserId={strUserId}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("strUserId", strUserID, ParameterType.UrlSegment);
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LSupportTicketsViewModel>>(request);
        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);
        //    return response.Data;
        //}

        public IEnumerable<LSupportTicketsViewModel> GetTicketsForDateRange(DateTime? dtFrom, DateTime? dtTo,string searchstring ,string RoleId, string strUserId, int CompanyId)
        {
            if (dtFrom != null)
            { 
                var request = new RestRequest("api/LSupportTickets/GetTicketsForDateRange?dtFrom={dtFrom}&dtTo={dtTo}&searchstring={searchstring}&RoleId={RoleId}&strUserId={strUserId}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
                request.AddParameter("dtFrom", dtFrom, ParameterType.UrlSegment);
                request.AddParameter("dtTo", dtTo, ParameterType.UrlSegment);
                request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
                request.AddParameter("strUserId", strUserId, ParameterType.UrlSegment);
                request.AddParameter("searchstring", searchstring, ParameterType.UrlSegment);
                request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
                var response = _client.Execute<List<LSupportTicketsViewModel>>(request);
                if (response.Data == null)
                    throw new Exception(response.ErrorMessage);
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<LSupportTicketsViewModel> GetSearchTickets(string LstTicketSearchString,string strUserID,string strUserRole)
        {
            if (LstTicketSearchString != null)
            {
                //api/LSupportTickets/GetSearchTickets?LstTicketSearchString=test&strUserID=d7c2eb89-6813-42c1-947a-eeb5902007b4&strUserRole=Reporting Analyst
                var request = new RestRequest("api/LSupportTickets/GetSearchTickets?LstTicketSearchString={LstTicketSearchString}&strUserID={strUserID}&strUserRole={strUserRole}", Method.GET) { RequestFormat = DataFormat.Json };
                request.AddParameter("LstTicketSearchString", LstTicketSearchString, ParameterType.UrlSegment);
                request.AddParameter("strUserID", strUserID, ParameterType.UrlSegment);
                request.AddParameter("strUserRole", strUserRole, ParameterType.UrlSegment);
                var response = _client.Execute<List<LSupportTicketsViewModel>>(request);
                if (response.Data == null)
                    throw new Exception(response.ErrorMessage);
                return response.Data;
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<RSupportTeamsViewModel>GetTeamDetails()
        {
            var request = new RestRequest("api/LSupportTickets/GetTeamIds", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _client.Execute<List<RSupportTeamsViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public string GetTeamDetail(string iRoleId)
        {
            var request = new RestRequest("api/LSupportTickets/GetTeamName?RoleId={RoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RoleId", iRoleId, ParameterType.UrlSegment);
            var response = _client.Execute<RSupportTeamsViewModel>(request);
            if (response.Data == null)
                return null;
            return response.Data.RstTeamName;
        }

        public int? GetTeamById(string strTeamName)
        {
            var request = new RestRequest("api/LSupportTickets/GetTeamIdByName?TeamName={TeamName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("TeamName", strTeamName, ParameterType.UrlSegment);
            var response = _client.Execute<RSupportTeamsViewModel>(request);
            if (response.Data == null)
                return null;
            return response.Data.Id;
        }

        public IEnumerable<AssigneeListViewModel> GetL2AssigneeList(string strTeamName)
        {
            var request = new RestRequest("api/LSupportTickets/GetL2Assignees?TeamName={TeamName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("TeamName", strTeamName, ParameterType.UrlSegment);
            var response = _client.Execute<List<AssigneeListViewModel>>(request);
            return response.Data;
        }

        public string GetTicketResponses(int id)
        {
            var request = new RestRequest("api/LSupportTickets/GetTicketResponses/{id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }
        public IEnumerable<RSupportTeamsViewModel> GetUserSupportLevel(string Roleid)
        {
            var request = new RestRequest("api/LSupportTickets/GetUserSupportLevel/{Roleid}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Roleid", Roleid, ParameterType.UrlSegment);
            var response = _client.Execute<List<RSupportTeamsViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        public int GetSupportTicketSummaryCounts()
        {
            var request = new RestRequest("api/LSupportTickets/GetSupportTicketSummaryCounts", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        //GetSupportTicketChart
        public IEnumerable<LSupportTicketContextModel> GetSupportTicketChart()
        {
            var request = new RestRequest("api/LSupportTickets/GetSupportTicketChart", Method.GET) { RequestFormat = DataFormat.Json };
         
            var response = _client.Execute<List<LSupportTicketContextModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<LSupportTicketContextModel> GetSupportTicketSummaryForDashBoard(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {
            var request = new RestRequest("api/LSupportTickets/GetSupportTicketSummaryForDashBoard?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<LSupportTicketContextModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public int GetSupportTicketDetailCounts()
        {
            var request = new RestRequest("api/LSupportTickets/GetSupportTicketDetailCounts", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        public IEnumerable<LSupportTicketContextModel> GetSupportTicketDetailForDashBoard(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            var request = new RestRequest("api/LSupportTickets/GetSupportTicketDetailForDashBoard?pagesize={pagesize}&pagenum={pagenum}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<LSupportTicketContextModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public int GetClosedTicketsCount()
        {
            var request = new RestRequest("api/LSupportTickets/GetClosedTicketsCount", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        public IEnumerable<ClosedTicketsModel> GetClosedTicketsData()
        {
            var request = new RestRequest("api/LSupportTickets/GetClosedTicketsData", Method.GET) { RequestFormat = DataFormat.Json };
             var response = _client.Execute<List<ClosedTicketsModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public int GetL2SOSTicketcounts()
        {
            var request = new RestRequest("api/LSupportTickets/GetL2SOSTicketcounts", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        public int GetL2ALTTicketcounts()
        {
            var request = new RestRequest("api/LSupportTickets/GetL2ALTTicketcounts", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<int>(request);

            return response.Data;
        }

        //GetL2ALTTicketcounts
    }

    interface ILSupportTicketsRestClient
    {
        //code review comment IEnumerable<LSupportTicketsViewModel> GetAll();
        LSupportTicketContextModel GetById(int id);
        //void CreateNewTicket(LSupportTicketsViewModel serverData);
        string CreateNewTicket(LSupportTicketContextModel serverData, string FileName, string FilePath,string PortfolioList);
        void UpdateTicket(LSupportTicketContextModel serverData, string StrUpdateType, string FileName, string FilePath, string PortfolioList);
        //code review comment void DeleteTicket(int id);
        IEnumerable<LSupportTicketsViewModel> GetRequestorTicket(string strUserID,int CompanyId, string RoleId);
        // IEnumerable<LSupportTicketsViewModel> GetAllickets(string strUserID,int CompanyId);

        IEnumerable<LSupportTicketsViewModel> GetSystemAnalystTicket(string strUserID,int CompanyId, string RoleId);
        IEnumerable<LSupportTicketsViewModel> GetL2SupportTicket(string strUserID,int CompanyId,string RoleId);
        string GetTicketResponses(int id);
        IEnumerable<RSupportTeamsViewModel> GetUserSupportLevel(string Roleid);
        IEnumerable<RSupportTeamsViewModel> GetTeamDetails();
        IEnumerable<LSupportTicketsViewModel> GetTicketsForDateRange(DateTime? dtFrom, DateTime? dtTo, string searchstring, string RoleId, string strUserId, int CompanyId);
        string  GetTeamDetail(String iRoleId);
        IEnumerable<AssigneeListViewModel> GetL2AssigneeList(string strTeamName);
        string DownloadTicket(string strUserID, int CompanyId, string RoleId, string TabName);
        void ExecuteMassActions(string Action, string TransactionIdList, string LoggedInUserId, string Comments);
        //IEnumerable<LSupportTicketsViewModel> GetSearchTickets(string LstTicketSearchString);
        int GetSupportTicketSummaryCounts();
        IEnumerable<LSupportTicketContextModel> GetSupportTicketSummaryForDashBoard(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery);
        IEnumerable<LSupportTicketContextModel> GetSupportTicketDetailForDashBoard(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery);
        int GetSupportTicketDetailCounts();
        IEnumerable<ClosedTicketsModel> GetClosedTicketsData( );
        int GetClosedTicketsCount();
        int GetL2SOSTicketcounts();
        int GetL2ALTTicketcounts();
        IEnumerable<LSupportTicketContextModel> GetSupportTicketChart();
    }
}