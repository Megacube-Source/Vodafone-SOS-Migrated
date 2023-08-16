//Code Review for this file (from security perspective) done

using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using Vodafone_SOS_WebApp.ViewModels;


namespace Vodafone_SOS_WebApp.Helper
{
    public class LPayRestClient:ILPayRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LPayRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public int GetPayForPayeeCounts(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, string PortfolioList, int LoggedinLUserId, string LoggedInRoleName,string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPay/GetPayByPayeeCommissionPeriodCompanyIdCounts?CompanyId={CompanyId}&CommissionPeriodIdList={CommissionPeriodIdList}&Status={Status}&UserName={UserName}&Workflow={Workflow}&LoggedInRoleName={LoggedInRoleName}&LoggedinLUserId={LoggedinLUserId}&LoggedInUserId={LoggedInUserId}", Method.POST) { RequestFormat = DataFormat.Json };
            var PayeecalModel = new PayeeCalcViewModel { PortfolioList = PortfolioList, PayeeList = PayeeIdList };
            request.AddBody(PayeecalModel);
            //request.AddParameter("PayeeIdList", PayeeIdList, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("CommissionPeriodIdList", CommissionPeriodIdList, ParameterType.UrlSegment);
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            //request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedinLUserId", LoggedinLUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleName", LoggedInRoleName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }


        public IEnumerable<dynamic> GetPayForPayeeReports(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, int PageSize, int PageNumber, string sortdatafield, string sortorder, string FilterQuery, string PortfolioList, string LoggedInRoleName, int LoggedinLUserId,string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPay/GetPayByPayeeCommissionPeriodCompanyId?CompanyId={CompanyId}&CommissionPeriodIdList={CommissionPeriodIdList}&Status={Status}&PageSize={PageSize}&PageNumber={PageNumber}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&UserName={UserName}&Workflow={Workflow}&LoggedinLUserId={LoggedinLUserId}&LoggedInRoleName={LoggedInRoleName}&LoggedInUserId={LoggedInUserId}", Method.POST) { RequestFormat = DataFormat.Json };
            var PayeecalModel = new PayeeCalcViewModel { PortfolioList = PortfolioList, PayeeList = PayeeIdList };
            request.AddBody(PayeecalModel);
            //request.AddParameter("PayeeIdList", PayeeIdList, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("CommissionPeriodIdList", CommissionPeriodIdList, ParameterType.UrlSegment);
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            request.AddParameter("PageSize", PageSize, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", PageNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedinLUserId", LoggedinLUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleName", LoggedInRoleName, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        //This method is to Get the Summary report depending upon the Grouping selected
        public IEnumerable<dynamic> GetSummaryForPayee(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, bool BatchNo, bool PrimaryChannelchecked, bool CommPeriodchecked, bool Payeechecked, string Status, string UserRole, string LoggedInUserId,bool CommyTypechecked)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var BatchName = false;
            var request = new RestRequest("api/LPay/GetLPaySummary?CompanyId={CompanyId}&CommissionPeriodIdList={CommissionPeriodIdList}&BatchNo={BatchNo}&BatchName={BatchName}&PrimaryChannelchecked={PrimaryChannelchecked}&CommPeriodchecked={CommPeriodchecked}&Payeechecked={Payeechecked}&Status={Status}&UserName={UserName}&Workflow={Workflow}&LoggedInUserId={LoggedInUserId}&UserRole={UserRole}&CommyTypechecked={CommyTypechecked}", Method.POST) { RequestFormat = DataFormat.Json };
            // request.AddParameter("PayeeIdList", PayeeIdList, ParameterType.UrlSegment);
            var PayeecalModel = new PayeeCalcViewModel { PayeeList = PayeeIdList };
            request.AddBody(PayeecalModel);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("CommissionPeriodIdList", CommissionPeriodIdList, ParameterType.UrlSegment);
            request.AddParameter("BatchNo", BatchNo, ParameterType.UrlSegment);
            request.AddParameter("BatchName",BatchName, ParameterType.UrlSegment);
            request.AddParameter("PrimaryChannelchecked", PrimaryChannelchecked, ParameterType.UrlSegment);
            request.AddParameter("CommPeriodchecked", CommPeriodchecked, ParameterType.UrlSegment);
            request.AddParameter("Payeechecked", Payeechecked, ParameterType.UrlSegment);
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("UserRole", UserRole, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CommyTypechecked", CommyTypechecked, ParameterType.UrlSegment);
            
            var response = _client.Execute<List<dynamic>>(request);


            return response.Data;
        }


        public string DownloadPayForPayeeReports(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, string LoggedInUserName, string CompanyCode, string PortfolioList, int LoggedinLUserId, string LoggedInRoleName,string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPay/DownloadPayByPayeeCommissionPeriodCompanyId?CompanyCode={CompanyCode}&PayeeIdList={PayeeIdList}&CompanyId={CompanyId}&CommissionPeriodIdList={CommissionPeriodIdList}&Status={Status}&PortfolioList={PortfolioList}&UserName={UserName}&Workflow={Workflow}&LoggedinLUserId={LoggedinLUserId}&LoggedInRoleName={LoggedInRoleName}&LoggedInUserId={LoggedInUserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PayeeIdList", string.IsNullOrEmpty(PayeeIdList) ? string.Empty : PayeeIdList, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("CommissionPeriodIdList", CommissionPeriodIdList, ParameterType.UrlSegment);
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("LoggedinLUserId", LoggedinLUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleName", LoggedInRoleName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        //This method is to Get the Summary report depending upon the Grouping selected
        public IEnumerable<dynamic> GetSummaryForPayee(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, bool BatchNo, bool Source, bool CommType, bool CommPeriodchecked, bool Payeechecked, string Status, string LoggedInUserId, string UserRole)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var BatchName = false;
            var request = new RestRequest("api/LPay/GetPaySummary?PayeeIdList={PayeeIdList}&CompanyId={CompanyId}&CommissionPeriodIdList={CommissionPeriodIdList}&BatchNo={BatchNo}&BatchName={BatchName}&Source={Source}&CommType={CommType}&CommPeriodchecked={CommPeriodchecked}&Payeechecked={Payeechecked}&Status={Status}&UserName={UserName}&Workflow={Workflow}&LoggedInUserId={LoggedInUserId}&UserRole={UserRole}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PayeeIdList", string.IsNullOrEmpty(PayeeIdList) ? string.Empty : PayeeIdList, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("CommissionPeriodIdList", CommissionPeriodIdList, ParameterType.UrlSegment);
            request.AddParameter("BatchNo", BatchNo, ParameterType.UrlSegment);
            request.AddParameter("BatchName", BatchName, ParameterType.UrlSegment);
            request.AddParameter("CommType", CommType, ParameterType.UrlSegment);
            request.AddParameter("Source", Source, ParameterType.UrlSegment);
            request.AddParameter("CommPeriodchecked", CommPeriodchecked, ParameterType.UrlSegment);
            request.AddParameter("Payeechecked", Payeechecked, ParameterType.UrlSegment);
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("UserRole", UserRole, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);


            return response.Data;
        }


        public IEnumerable<LPayRowCountsViewModel> GetLPayCounts(int SOSBatchNumber, int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPay/GetLPayCounts?SOSBatchNumber={SOSBatchNumber}&UserName={UserName}&Workflow={Workflow}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayRowCountsViewModel>>(request);

            if (response.Data == null)
                return new List<LPayRowCountsViewModel>();

            return response.Data;
        }


        public IEnumerable<dynamic> GetLPayForGrid(int SOSBatchNumber, int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery, int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPay/GetLPayForGrid?SOSBatchNumber={SOSBatchNumber}&PageNumber={PageNumber}&PageSize={PageSize}&UserName={UserName}&Workflow={Workflow}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", pagenum, ParameterType.UrlSegment);
            request.AddParameter("PageSize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);

            return response.Data;
        }

        public string DownloadLPayForGrid(int TransactionId,string CompanyCode,string LoggedInUserName)
        {
            var request = new RestRequest("api/LPay/DownloadLPayGrid?TransactionId={TransactionId}&CompanyCode={CompanyCode}&LoggedInUserName={LoggedInUserName}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("TransactionId", TransactionId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserName", LoggedInUserName, ParameterType.UrlSegment);

            var response = _client.Execute<List<LPayViewModel>>(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }
        
        //Code review comment
        //public void UpdateStatus(int id, string Comments, string Status)
        //{
        //    var request = new RestRequest("api/LPay/UpdatePayStatus/{Id}?Comments={Comments}&Status={Status}", Method.POST) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("Id", id, ParameterType.UrlSegment);
        //    if (!string.IsNullOrEmpty(Comments))
        //    {
        //        request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
        //    }
        //    request.AddParameter("Status", Status, ParameterType.UrlSegment);
        //    var response = _client.Execute<LPayViewModel>(request);
        //}
    }
    interface ILPayRestClient
    {
        //code review comment void UpdateStatus(int id, string Comments, string Status);
        IEnumerable<LPayRowCountsViewModel> GetLPayCounts(int SOSBatchNumber, int CompanyId);
        IEnumerable<dynamic> GetLPayForGrid(int SOSBatchNumber, int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery, int CompanyId);
        string DownloadLPayForGrid(int TransactionId, string CompanyCode,string LoggedInUserName);
        int GetPayForPayeeCounts(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, string PortfolioList, int LoggedinLUserId, string LoggedInRoleName,string LoggedInUserId);
        IEnumerable<dynamic> GetPayForPayeeReports(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, int PageSize, int PageNumber, string sortdatafield, string sortorder, string FilterQuery, string PortfolioList, string LoggedInRoleName, int LoggedinLUserId,string LoggedInUserId);
        string DownloadPayForPayeeReports(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, string LoggedInUserName, string CompanyCode, string PortfolioList, int LoggedinLUserId, string LoggedInRoleName,string LoggedInUserId);
        IEnumerable<dynamic> GetSummaryForPayee(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, bool BatchNo, bool PrimaryChannelchecked, bool CommPeriodchecked, bool Payeechecked, string Status, string UserRole, string LoggedInUserId, bool CommyTypechecked);
    }
}