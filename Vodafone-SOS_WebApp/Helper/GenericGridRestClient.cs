using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class GenericGridRestClient:IGenericGridRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //
        public GenericGridRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public string ExportGenericGrid(int WFConfigId, string LoggedInRoleId, string LoggedInUserId, string Workflow, int CompanyId,string CompanyCode,string TabName, string PortfolioList, string FilterQuery)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/GenericGrid/SaveGenericGridToFile?Workflow={Workflow}&UserName={UserName}&LoggedInRoleId={LoggedInRoleId}&CompanyId={CompanyId}&WFConfigId={WFConfigId}&LoggedInUserId={LoggedInUserId}&CompanyCode={CompanyCode}&TabName={TabName}&PortfolioList={PortfolioList}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("WFConfigId", WFConfigId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("TabName", TabName, ParameterType.UrlSegment);
            request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", FilterQuery, ParameterType.UrlSegment);
            
            var response = _client.Execute<dynamic>(request);
            dynamic data = JsonConvert.DeserializeObject(response.Content);
            return data;
        }


        public int GetGenericGridCounts(int WorkflowConfigId, string LoggedInRoleId, string LoggedInUserId, string WorkflowName, int CompanyId, string TabName,string PortfolioList)
        {
            var request = new RestRequest("api/GenericGrid/GetGenericGridDataCounts?WorkflowConfigId={WorkflowConfigId}&LoggedInRoleId={LoggedInRoleId}&LoggedInUserId={LoggedInUserId}&WorkflowName={WorkflowName}&CompanyId={CompanyId}&TabName={TabName}&PortfolioList={PortfolioList}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("WorkflowConfigId", WorkflowConfigId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("WorkflowName", WorkflowName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("TabName", TabName, ParameterType.UrlSegment);
            request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            return response.Data;
        }


        public IEnumerable<dynamic> GetByWorkflowId(int WorkflowConfigId,string LoggedInRoleId,string LoggedInUserId,string WorkflowName,int CompanyId, int PageSize, int PageNumber ,string sortdatafield, string sortorder, string FilterQuery,string TabName,string PortfolioList)
        {
            //Added by SG- R2.3
            GenericGridRequestData data = new GenericGridRequestData();
            data.WorkflowConfigId = WorkflowConfigId;
            data.LoggedInRoleId = LoggedInRoleId;
            data.LoggedInUserId = LoggedInUserId;
            data.WorkflowName = WorkflowName;
            data.CompanyId = CompanyId;
            data.PageSize = PageSize;
            data.PageNumber = PageNumber;
            data.sortdatafield = string.IsNullOrEmpty(sortdatafield) ? null : sortdatafield;
            data.sortorder = string.IsNullOrEmpty(sortorder) ? null : sortorder;
            data.FilterQuery = string.IsNullOrEmpty(FilterQuery) ? null : FilterQuery;
            data.TabName = TabName;
            data.PortfolioList = string.IsNullOrEmpty(PortfolioList) ? null : PortfolioList;
            var request = new RestRequest("api/GenericGrid/GetGenericGridData", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(data);

            //END by SG- R2.3
            //Commented by SG- R2.3
            //var request = new RestRequest("api/GenericGrid/GetGenericGridData?WorkflowConfigId={WorkflowConfigId}&LoggedInRoleId={LoggedInRoleId}&LoggedInUserId={LoggedInUserId}&WorkflowName={WorkflowName}&CompanyId={CompanyId}&PageSize={PageSize}&PageNumber={PageNumber}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&TabName={TabName}&PortfolioList={PortfolioList}", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddParameter("WorkflowConfigId", WorkflowConfigId, ParameterType.UrlSegment);
            //request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            //request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            //request.AddParameter("WorkflowName", WorkflowName, ParameterType.UrlSegment);
            //request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            //request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            //request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            //request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            //request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            //request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            //request.AddParameter("TabName", TabName, ParameterType.UrlSegment);
            //request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            //End by SG- R2.3
            //request.AddBody(data);
            var response = _client.Execute<List<dynamic>>(request);
          //send data
            return response.Data;
        }

        public string GetSecondaryFormButtons(int WFConfigId, string LoggedInRoleId, string LoggedInUserId, int TransactionId)
        {
            var request = new RestRequest("api/GenericGrid/GetSecondaryFormActionItems?WFConfigId={WFConfigId}&TransactionId={TransactionId}&LoggedInRoleId={LoggedInRoleId}&LoggedInUserId={LoggedInUserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("WFConfigId", WFConfigId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("TransactionId", TransactionId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
              
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        public List<string> GetGridBottomButtons(string Workflow,int CompanyId , string LoggedInRoleId, string LoggedInUserId)
        {
            var request = new RestRequest("api/GenericGrid/GetGridBottomActionItems?Workflow={Workflow}&CompanyId={CompanyId}&LoggedInRoleId={LoggedInRoleId}&LoggedInUserId={LoggedInUserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }
            //string source = response.Content;
            //dynamic data = JsonConvert.DeserializeObject(source);
            return response.Data;
        }


        public void UpdateActionStatus(string WorkFlowName, string TransactionId, int CompanyId,string Action,string LoggedInUserId,string Comments,string LoggedInRoleId,string AssigneeId)
        {
            if(string.IsNullOrEmpty(Comments))
            Comments = "Empty";//"SS is passing text to avoid error then will replace it from empty string
            OtherAPIData objTrans = new OtherAPIData();
            objTrans.TransactionID = TransactionId;
            //var request = new RestRequest("api/GenericGrid/UpdateActionStatus?Action={Action}&WorkFlowName={WorkFlowName}&TransactionId={TransactionId}&CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}&Comments={Comments}&LoggedInRoleId={LoggedInRoleId}&AssigneeId={AssigneeId}", Method.GET) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/GenericGrid/UpdateActionStatus?Action={Action}&WorkFlowName={WorkFlowName}&CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}&Comments={Comments}&LoggedInRoleId={LoggedInRoleId}&AssigneeId={AssigneeId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(objTrans);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("Action", Action, ParameterType.UrlSegment);
            request.AddParameter("WorkFlowName", WorkFlowName, ParameterType.UrlSegment);
            request.AddParameter("TransactionId", TransactionId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("AssigneeId",string.IsNullOrEmpty(AssigneeId)?"": AssigneeId, ParameterType.UrlSegment);
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

        public void UpdateActionStatusNew(string WorkFlowName, string[] TransactionId, int CompanyId, string Action, string LoggedInUserId, string Comments, string LoggedInRoleId, string AssigneeId)
        {
            if (string.IsNullOrEmpty(Comments))
                Comments = "Empty";//"SS is passing text to avoid error then will replace it from empty string
                                   //OtherAPIDataNew objTrans = new OtherAPIDataNew();
                                   // objTrans.TransactionID = TransactionId;
            List<string> objTrans = new List<string>();
            foreach( string s in TransactionId)
            {
                objTrans.Add(s);
            }
            

            //var request = new RestRequest("api/GenericGrid/UpdateActionStatus?Action={Action}&WorkFlowName={WorkFlowName}&TransactionId={TransactionId}&CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}&Comments={Comments}&LoggedInRoleId={LoggedInRoleId}&AssigneeId={AssigneeId}", Method.GET) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/GenericGrid/UpdateActionStatusNew?Action={Action}&WorkFlowName={WorkFlowName}&CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}&Comments={Comments}&LoggedInRoleId={LoggedInRoleId}&AssigneeId={AssigneeId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(objTrans);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("Action", Action, ParameterType.UrlSegment);
            request.AddParameter("WorkFlowName", WorkFlowName, ParameterType.UrlSegment);
            request.AddParameter("TransactionId", TransactionId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("AssigneeId", string.IsNullOrEmpty(AssigneeId) ? "" : AssigneeId, ParameterType.UrlSegment);
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


        public void SelfAssign(string WorkFlowName, int TransactionId, int CompanyId, string AssigneeName, string AssigneeId,string RoleId)
        {
            var request = new RestRequest("api/GenericGrid/SelfAssign?WorkFlowName={WorkFlowName}&TransactionId={TransactionId}&CompanyId={CompanyId}&AssigneeId={AssigneeId}&AssigneeName={AssigneeName}&RoleId={RoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("WorkFlowName", WorkFlowName, ParameterType.UrlSegment);
            request.AddParameter("TransactionId", TransactionId, ParameterType.UrlSegment);
            request.AddParameter("AssigneeId", AssigneeId, ParameterType.UrlSegment);
            request.AddParameter("AssigneeName", AssigneeName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
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

        }

        public void AssignTo(string WorkFlowName, int TransactionId, int CompanyId, string AssigneeName, string AssigneeId, string RoleId)
        {
            var request = new RestRequest("api/GenericGrid/AssignTo?WorkFlowName={WorkFlowName}&TransactionId={TransactionId}&CompanyId={CompanyId}&AssigneeId={AssigneeId}&AssigneeName={AssigneeName}&RoleId={RoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("WorkFlowName", WorkFlowName, ParameterType.UrlSegment);
            request.AddParameter("TransactionId", TransactionId, ParameterType.UrlSegment);
            request.AddParameter("AssigneeId", AssigneeId, ParameterType.UrlSegment);
            request.AddParameter("AssigneeName", AssigneeName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
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

        }

        public IEnumerable<AssigneeListViewModel> GetAssigneeList(string WorkFlowName, int TransactionId, int CompanyId)
        {
            var request = new RestRequest("api/GenericGrid/GetAssigneeList?WorkFlowName={WorkFlowName}&TransactionId={TransactionId}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("WorkFlowName", WorkFlowName, ParameterType.UrlSegment);
            request.AddParameter("TransactionId", TransactionId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
         
            var response = _client.Execute<List<AssigneeListViewModel>>(request);
            return response.Data;
        }

        public DataTable WorkflowGridCounts(string LoggedInRoleId, string LoggedInUserId, string WorkflowName, int CompanyId, string PortfolioList)
        {
            var request = new RestRequest("api/GenericGrid/WorkflowGridCounts?LoggedInRoleId={LoggedInRoleId}&LoggedInUserId={LoggedInUserId}&WorkflowName={WorkflowName}&CompanyId={CompanyId}&PortfolioList={PortfolioList}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("WorkflowName", WorkflowName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("PortfolioList", (String.IsNullOrEmpty(PortfolioList))?String.Empty:PortfolioList, ParameterType.UrlSegment);
            var response = _client.Execute<DataTable>(request);
            DataTable DT = JsonConvert.DeserializeObject<DataTable>(response.Content);
            return DT;
        }

        public IEnumerable<dynamic> GetUserPreferenceData(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string SessionId)
        {
            var request = new RestRequest("api/GenericGrid/GetUserPreferenceData?UserId={UserId}&EntityName={EntityName}&EntityItem={EntityItem}&ConfigType={ConfigType}&WFConfigId={WFConfigId}&SessionId={SessionId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("EntityName", EntityName, ParameterType.UrlSegment);
            request.AddParameter("EntityItem", EntityItem, ParameterType.UrlSegment);
            request.AddParameter("ConfigType", ConfigType, ParameterType.UrlSegment);
            request.AddParameter("WFConfigId", WFConfigId, ParameterType.UrlSegment);
            request.AddParameter("SessionId", SessionId, ParameterType.UrlSegment);
            //request.AddParameter("PortfolioList", (String.IsNullOrEmpty(PortfolioList)) ? String.Empty : PortfolioList, ParameterType.UrlSegment);
            //var response = _client.Execute<DataTable>(request);
            var response = _client.Execute<List<dynamic>>(request);
            //send data
            return response.Data;
            //DataTable DT = JsonConvert.DeserializeObject<DataTable>(response.Content);
            //return DT;
        }

        public string SaveUserPreferenceData(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string Config, string SessionId)
        {
            var request = new RestRequest("api/GenericGrid/SaveUserPreferenceData?UserId={UserId}&EntityName={EntityName}&EntityItem={EntityItem}&ConfigType={ConfigType}&WFConfigId={WFConfigId}&Config={Config}&SessionId={SessionId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("EntityName", EntityName, ParameterType.UrlSegment);
            request.AddParameter("EntityItem", EntityItem, ParameterType.UrlSegment);
            request.AddParameter("ConfigType", ConfigType, ParameterType.UrlSegment);
            request.AddParameter("WFConfigId", WFConfigId, ParameterType.UrlSegment);
            request.AddParameter("Config", Config, ParameterType.UrlSegment);
            request.AddParameter("SessionId", SessionId, ParameterType.UrlSegment);
            //request.AddParameter("PortfolioList", (String.IsNullOrEmpty(PortfolioList)) ? String.Empty : PortfolioList, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            return response.Data;
        }

        public string DeleteUserPreferenceData(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string SessionId)
        {
            var request = new RestRequest("api/GenericGrid/DeleteUserPreferenceData?UserId={UserId}&EntityName={EntityName}&EntityItem={EntityItem}&ConfigType={ConfigType}&WFConfigId={WFConfigId}&Config={Config}&SessionId={SessionId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("EntityName", EntityName, ParameterType.UrlSegment);
            request.AddParameter("EntityItem", EntityItem, ParameterType.UrlSegment);
            request.AddParameter("ConfigType", ConfigType, ParameterType.UrlSegment);
            request.AddParameter("WFConfigId", WFConfigId, ParameterType.UrlSegment);
            request.AddParameter("SessionId", SessionId, ParameterType.UrlSegment);
            //request.AddParameter("PortfolioList", (String.IsNullOrEmpty(PortfolioList)) ? String.Empty : PortfolioList, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            return "";
        }
        public IEnumerable<LChangeRequestViewModel> GetChangeRequestDetailbyId(int id)
        {
            var request = new RestRequest("api/GenericGrid/GetChangeRequestDetailbyId?id={id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<List<LChangeRequestViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        public string UpdateAttachment(int id, string CreatedBy, string FileName, string FilePath, string Type)
        {
            var request = new RestRequest(Method.GET) { RequestFormat = DataFormat.Json };
            request.Resource = "api/GenericGrid/UpdateAttachmentCommon?id={id}&FileName={FileName}&FilePath={FilePath}&CreatedBy={CreatedBy}&Type={Type}";
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("FileName", string.IsNullOrEmpty(FileName) ? "" : FileName, ParameterType.UrlSegment);
            request.AddParameter("FilePath", string.IsNullOrEmpty(FilePath) ? "" : FilePath, ParameterType.UrlSegment);
            request.AddParameter("CreatedBy", string.IsNullOrEmpty(CreatedBy) ? "" : CreatedBy, ParameterType.UrlSegment);
            request.AddParameter("Type", string.IsNullOrEmpty(Type) ? "" : Type, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            return Convert.ToString(response.Content);
        }


    }
    interface IGenericGridRestClient
    {
        int GetGenericGridCounts(int WorkflowConfigId, string LoggedInRoleId, string LoggedInUserId, string WorkflowName, int CompanyId,string TabName,string PortfolioList);
        IEnumerable<AssigneeListViewModel> GetAssigneeList(string WorkFlowName, int TransactionId, int CompanyId);
        void SelfAssign(string WorkFlowName, int TransactionId, int CompanyId, string AssigneeName, string AssigneeId, string RoleId);
        void AssignTo(string WorkFlowName, int TransactionId, int CompanyId, string AssigneeName, string AssigneeId, string RoleId);
        IEnumerable<dynamic> GetByWorkflowId(int WorkflowConfigId, string LoggedInRoleId, string LoggedInUserId, string WorkflowName, int CompanyId, int PageSize, int PageNumber, string sortdatafield, string sortorder, string FilterQuery,string TabName,string PortfolioList);
       // IEnumerable<dynamic> GetChangeRequestByWorkflowId(int WorkflowConfigId, string LoggedInRoleId, string LoggedInUserId, string WorkflowName, int CompanyId);
        void UpdateActionStatus(string WorkFlowName, string TransactionId, int CompanyId, string Action, string LoggedInUserId, string Comments, string LoggedInRoleId, string AssigneeId);

        void UpdateActionStatusNew(string WorkFlowName, string[] TransactionId, int CompanyId, string Action, string LoggedInUserId, string Comments, string LoggedInRoleId, string AssigneeId);
        string GetSecondaryFormButtons(int WFConfigId, string LoggedInRoleId, string LoggedInUserId, int TransactionId);
        List<string> GetGridBottomButtons(string Workflow, int CompanyId, string LoggedInRoleId, string LoggedInUserId);
        string ExportGenericGrid(int WFConfigId, string LoggedInRoleId, string LoggedInUserId, string Workflow, int CompanyId, string CompanyCode, string TabName, string PortfolioList, string query);
        DataTable WorkflowGridCounts(string LoggedInRoleId, string LoggedInUserId, string WorkflowName, int CompanyId, string PortfolioList);

        IEnumerable<dynamic> GetUserPreferenceData(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string SessionId);

        string SaveUserPreferenceData(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string Config, string SessionId);

        string DeleteUserPreferenceData(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string SessionId);

        IEnumerable<LChangeRequestViewModel> GetChangeRequestDetailbyId(int id);
        string UpdateAttachment(int id, string CreatedBy, string FileName, string FilePath, string Type);
    }
}