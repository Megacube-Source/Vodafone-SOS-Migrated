//Code Review for this file (from security perspective) done

using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
//code review comment using System.Text.RegularExpressions;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LBatchesRestClient:ILBatchesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LBatchesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        #region Dead code by RK/VG 10062017
        /*
         public IEnumerable<LBatchViewModel> GetAll()
        {
            var request = new RestRequest("api/Lbatches/GetLBatches", Method.GET) { RequestFormat = DataFormat.Json };

            var response = _client.Execute<List<LBatchViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public IEnumerable<LBatchViewModelForAnalystGrid> GetByCompanyIdBatchTypeBatchStatus(int CompanyId, string BatchType, string BatchStatus, string AspnetUserid, bool IsManager)
        {
            var request = new RestRequest("api/Lbatches/GetLBatchesByCompanyIdBatchTypeBatchStatus?CompanyId={CompanyId}&BatchType={BatchType}&BatchStatus={BatchStatus}&AspnetUserid={AspnetUserid}&IsManager={IsManager}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("BatchType", BatchType, ParameterType.UrlSegment);
            request.AddParameter("BatchStatus", BatchStatus, ParameterType.UrlSegment);
            request.AddParameter("AspnetUserid", AspnetUserid, ParameterType.UrlSegment);
            request.AddParameter("IsManager", IsManager, ParameterType.UrlSegment);

            var response = _client.Execute<List<LBatchViewModelForAnalystGrid>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
         public IEnumerable<LBatchViewModelForAnalystGrid> GetCalcBatchGridForPayee(int CompanyId, string BatchType, string BatchStatus, string PeriodStatus, string AspnetUserid)
        {
            var request = new RestRequest("api/Lbatches/GetCalcBatchGridForPayee?CompanyId={CompanyId}&BatchType={BatchType}&BatchStatus={BatchStatus}&PeriodStatus={PeriodStatus}&AspnetUserid={AspnetUserid}&IsManager={IsManager}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("BatchType", BatchType, ParameterType.UrlSegment);
            request.AddParameter("BatchStatus", BatchStatus, ParameterType.UrlSegment);
            request.AddParameter("AspnetUserid", AspnetUserid, ParameterType.UrlSegment);
            PeriodStatus = "'" + Regex.Replace(PeriodStatus, @"\s+", "").Replace(",", "','") + "'";
            request.AddParameter("PeriodStatus", PeriodStatus, ParameterType.UrlSegment);


            var response = _client.Execute<List<LBatchViewModelForAnalystGrid>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<LBatchViewModel> GetByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/Lbatches/GetLBatchesByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LBatchViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        

        

       
        public LBatchViewModel Update(LBatchViewModel serverData)
        {
            var request = new RestRequest("api/Lbatches/PutLBatch/{Id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LBatchViewModel>(request);

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
         */
        #endregion

        //public IEnumerable<LBatchViewModel> GetByCompanyStatusId(int CompanyId,int StatusId)
        //{
        //    var request = new RestRequest("api/Lbatches/GetLBatchesByCompanyIdByStatusId?CompanyId={CompanyId}&StatusId={StatusId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("StatusId", StatusId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LBatchViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}



        //Below method was designed to entertain the situation when we were using Periods but now as we are not using it, this method is not in use.
        //Not deleting this method because it may be used in future in calculations and/or payment processes.
        public void UpdateBatchStatus(int SOSBatchNumber, string BatchLevelComments, string NewStatus,string RedirectToUrl)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/Lbatches/UpdateBatchStatus?SOSBatchNumber={SOSBatchNumber}&BatchLevelComments={BatchLevelComments}&NewStatus={NewStatus}&UserName={UserName}&Workflow={Workflow}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("BatchLevelComments", "BatchLevelComments", ParameterType.UrlSegment);
            request.AddParameter("NewStatus", NewStatus, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LBatchViewModel>>(request);

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
        
             public void UpdateBatchLevelComments(int SOSBatchNumber, string BatchLevelComments, string LoggedInUserName,string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/Lbatches/UpdateBatchLevelComments?SOSBatchNumber={SOSBatchNumber}&BatchLevelComments={BatchLevelComments}&UserName={UserName}&Workflow={Workflow}&LoggedInUserId={LoggedInUserId}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("BatchLevelComments", BatchLevelComments, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserName", LoggedInUserName, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LBatchViewModel>>(request);
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
        public LBatchViewModel GetById(int id)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/Lbatches/GetLBatch/{id}?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LBatchViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public LBatchViewModel GetByBatchNumber(int BatchNumber)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/Lbatches/GetLBatchByBatchNumber?BatchNumber={BatchNumber}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            var response = _client.Execute<LBatchViewModel>(request);

            return response.Data;
        }

        public IEnumerable<LBatchViewModel> GetByCommPeriodIdList(string CommissionPeriodIdList,int CompanyId, string PortfolioList,string LoggedInRoleId, string LoggedInUserId, string LoggedInRoleName)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/Lbatches/GetByCommPeriodIdList?CommissionPeriodIdList={CommissionPeriodIdList}&CompanyId={CompanyId}&UserName={UserName}&Workflow={Workflow}&PortfolioList={PortfolioList}&LoggedInRoleId={LoggedInRoleId}&LoggedInUserId={LoggedInUserId}&LoggedInRoleName={LoggedInRoleName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("CommissionPeriodIdList", CommissionPeriodIdList, ParameterType.UrlSegment);
            request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleName", LoggedInRoleName, ParameterType.UrlSegment);
            var response = _client.Execute<List<LBatchViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
       
        public IEnumerable<LBatchViewModelForPayeeGrid> GetByUserForPayeeUploadGrid(int CompanyId, string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/Lbatches/GetByUserForPayeeUploadGrid?CompanyId={CompanyId}&AspnetUserid={LoggedInUserId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LBatchViewModelForPayeeGrid>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        } 

        public LBatchViewModelForPayeeGrid GetDetailsById(int CompanyId,int Id)
        {
            var request = new RestRequest("api/Lbatches/GetById?CompanyId={CompanyId}&Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<LBatchViewModelForPayeeGrid>(request);
            return response.Data;
        }
    }
}

    interface ILBatchesRestClient
    {

        LBatchViewModelForPayeeGrid GetDetailsById(int CompanyId, int Id);
        IEnumerable<LBatchViewModelForPayeeGrid> GetByUserForPayeeUploadGrid(int CompanyId, string LoggedInUserId);
        //Code Review Comment IEnumerable<LBatchViewModel> GetAll();
        LBatchViewModel GetById(int id);
        void UpdateBatchStatus(int SOSBatchNumber, string BatchLevelComments, string NewStatus, string RedirectToUrl);
        LBatchViewModel GetByBatchNumber(int BatchNumber);
    IEnumerable<LBatchViewModel> GetByCommPeriodIdList(string CommissionPeriodIdList, int CompanyId, string PortfolioList, string LoggedInRoleId, string LoggedInUserId, string LoggedInRoleName);
        void UpdateBatchLevelComments(int SOSBatchNumber, string BatchLevelComments, string LoggedInUserName,string LoggedInUserId);

        //Code Review Comment LBatchViewModel Update(LBatchViewModel serverData);
        //IEnumerable<LBatchViewModel> GetByCompanyStatusId(int CompanyId, int StatusId);
        //Code Review Comment IEnumerable<LBatchViewModelForAnalystGrid> GetByCompanyIdBatchTypeBatchStatus(int CompanyId, string BatchType, string BatchStatus, string AspnetUserid, bool IsManager);
        //Code Review Comment IEnumerable<LBatchViewModelForAnalystGrid> GetByCompanyIdBatchTypeBatchStatusPeriodStatus(int CompanyId, string BatchType, string BatchStatus, string PeriodStatus, string AspnetUserid, bool IsManager);
        //Code Review Comment IEnumerable<LBatchViewModelForAnalystGrid> GetCalcBatchGridForPayee(int CompanyId, string BatchType, string BatchStatus, string PeriodStatus, string AspnetUserid);
        //Code Review Comment IEnumerable<LBatchViewModel> GetByCompanyId(int CompanyId);
}
//}