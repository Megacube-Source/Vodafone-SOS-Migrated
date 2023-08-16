using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Vodafone_SOS_WebApp.Controllers;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class SearchEngineRestClient : ISearchEngineRestClient
    {

        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public SearchEngineRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }


        public int GetSummaryCount(int CountryID, string UserName, string Workflow, string LoggedInUserId, string UserRole, bool ChkSubscriberNumber, string SubscriberNumber, bool ChkCustomerSegment, string CustomerSegment, bool ChkActivityType, string ActivityType, bool ChkActivationOrder, string ActivationOrder, bool ChkCommType, string CommissionType,
                                                bool ChkChannel, string Channel, bool ChkParentPayee, string PayeeParent, bool ChkSubChannel, string SubChannel, bool ChkPayee, string Payees, bool ChkPeriod, string Period, bool ChkBatchStatus, string BatchStatus, string SelectedTab)
        {

            var request = new RestRequest("api/ComissionSearchEngine/GetSummaryCount", Method.POST) { RequestFormat = DataFormat.Json };

            SearchEngineViewModel SearchModel = new SearchEngineViewModel();
            SearchModel.ChkSubscriberNumber = ChkSubscriberNumber;
            SearchModel.SubscriberNumber = SubscriberNumber;
            SearchModel.ChkCustomerSegment = ChkCustomerSegment;
            SearchModel.CustomerSegment = CustomerSegment;
            SearchModel.ChkActivityType = ChkActivityType;
            SearchModel.ActivityType = ActivityType;
            SearchModel.ChkActivationOrder = ChkActivationOrder;
            SearchModel.ActivationOrder = ActivationOrder;
            SearchModel.ChkCommType = ChkCommType;
            SearchModel.CommissionType = CommissionType;
            SearchModel.ChkChannel = ChkChannel;
            SearchModel.Channel = Channel;
            SearchModel.ChkParentPayee = ChkParentPayee;
            SearchModel.PayeeParent = PayeeParent;
            SearchModel.ChkSubChannel = ChkSubChannel;
            SearchModel.SubChannel = SubChannel;
            SearchModel.ChkPayee = ChkPayee;
            SearchModel.Payees = Payees;
            SearchModel.ChkPeriod = ChkPeriod;
            SearchModel.Period = Period;
            SearchModel.ChkBatchStatus = ChkBatchStatus;
            SearchModel.BatchStatus = BatchStatus;
            SearchModel.CountryID = CountryID;
            SearchModel.UserName = UserName;
            SearchModel.Workflow = Workflow;
            SearchModel.LoggedInUserId = LoggedInUserId;
            SearchModel.UserRole = UserRole;
            SearchModel.SelectedTab = SelectedTab;
            request.AddBody(SearchModel);

            var response = _client.Execute<int>(request);
            return response.Data;
        }


        public IEnumerable<dynamic> GetSummary(int CountryID, string UserName, string Workflow, string LoggedInUserId, string UserRole, bool ChkSubscriberNumber, string SubscriberNumber, bool ChkCustomerSegment, string CustomerSegment, bool ChkActivityType, string ActivityType, bool ChkActivationOrder, string ActivationOrder, bool ChkCommType, string CommissionType,
                                                bool ChkChannel, string Channel, bool ChkParentPayee, string PayeeParent, bool ChkSubChannel, string SubChannel, bool ChkPayee, string Payees, bool ChkPeriod, string Period, bool ChkBatchStatus, string BatchStatus,  string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, string SelectedTab)
        {

            var request = new RestRequest("api/ComissionSearchEngine/GetSummary?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            SearchEngineViewModel SearchModel = new SearchEngineViewModel();
            SearchModel.ChkSubscriberNumber = ChkSubscriberNumber;
            SearchModel.SubscriberNumber = SubscriberNumber;
            SearchModel.ChkCustomerSegment = ChkCustomerSegment;
            SearchModel.CustomerSegment = CustomerSegment;
            SearchModel.ChkActivityType = ChkActivityType;
            SearchModel.ActivityType = ActivityType;
            SearchModel.ChkActivationOrder = ChkActivationOrder;
            SearchModel.ActivationOrder = ActivationOrder;
            SearchModel.ChkCommType = ChkCommType;
            SearchModel.CommissionType = CommissionType;
            SearchModel.ChkChannel = ChkChannel;
            SearchModel.Channel = Channel;
            SearchModel.ChkParentPayee = ChkParentPayee;
            SearchModel.PayeeParent = PayeeParent;
            SearchModel.ChkSubChannel = ChkSubChannel;
            SearchModel.SubChannel = SubChannel;
            SearchModel.ChkPayee = ChkPayee;
            SearchModel.Payees = Payees;
            SearchModel.ChkPeriod = ChkPeriod;
            SearchModel.Period = Period;
            SearchModel.ChkBatchStatus = ChkBatchStatus;
            SearchModel.BatchStatus = BatchStatus;
            SearchModel.CountryID = CountryID;
            SearchModel.UserName = UserName;
            SearchModel.Workflow = Workflow;
            SearchModel.LoggedInUserId = LoggedInUserId;
            SearchModel.UserRole = UserRole;
            SearchModel.SelectedTab = SelectedTab;
            request.AddBody(SearchModel);
            var response = _client.Execute<List<dynamic>>(request);
            return response.Data;
        }


        public string DownloadFile(string CompanyCode, string LoggedInUserId, string LoggedInRoleName, int countryID, bool ChkSubscriberNumber, string SubscriberNumber, bool ChkCustomerSegment, string CustomerSegment, bool ChkActivityType, string ActivityType, bool ChkActivationOrder, string ActivationOrder, bool ChkCommType, string CommissionType,
                                                bool ChkChannel, string Channel, bool ChkParentPayee, string PayeeParent, bool ChkSubChannel, string SubChannel, bool ChkPayee, string Payees, bool ChkPeriod, string Period, bool ChkBatchStatus, string BatchStatus, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, string SelectedTab, string LoggedinUserName)
        {

            var request = new RestRequest("api/ComissionSearchEngine/DownLoadFiles?sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("pagesize", 999999, ParameterType.UrlSegment);
            request.AddParameter("pagenum", 0, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            SearchEngineViewModel SearchModel = new SearchEngineViewModel();
            SearchModel.ChkSubscriberNumber = ChkSubscriberNumber;
            SearchModel.SubscriberNumber = SubscriberNumber;
            SearchModel.ChkCustomerSegment = ChkCustomerSegment;
            SearchModel.CustomerSegment = CustomerSegment;
            SearchModel.ChkActivityType = ChkActivityType;
            SearchModel.ActivityType = ActivityType;
            SearchModel.ChkActivationOrder = ChkActivationOrder;
            SearchModel.ActivationOrder = ActivationOrder;
            SearchModel.ChkCommType = ChkCommType;
            SearchModel.CommissionType = CommissionType;
            SearchModel.ChkChannel = ChkChannel;
            SearchModel.Channel = Channel;
            SearchModel.ChkParentPayee = ChkParentPayee;
            SearchModel.PayeeParent = PayeeParent;
            SearchModel.ChkSubChannel = ChkSubChannel;
            SearchModel.SubChannel = SubChannel;
            SearchModel.ChkPayee = ChkPayee;
            SearchModel.Payees = Payees;
            SearchModel.ChkPeriod = ChkPeriod;
            SearchModel.Period = Period;
            SearchModel.ChkBatchStatus = ChkBatchStatus;
            SearchModel.BatchStatus = BatchStatus;
            SearchModel.CountryID = countryID;
            SearchModel.UserName = "";
            SearchModel.Workflow = "";
            SearchModel.LoggedInUserId = LoggedInUserId;
            SearchModel.UserRole = LoggedInRoleName;
            SearchModel.CompanyCode = CompanyCode;
            if (SelectedTab == "0")
            {
                SearchModel.SelectedTab = "Claims";
            }
            else
            {
                SearchModel.SelectedTab = "XCalc";
            }
            SearchModel.LoggedinUserName = LoggedinUserName;
            request.AddBody(SearchModel);
            var response = _client.Execute(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }


    }
    interface ISearchEngineRestClient
    {
        int GetSummaryCount(int CountryID, string UserName, string Workflow, string LoggedInUserId, string UserRole, bool ChkSubscriberNumber, string SubscriberNumber, bool ChkCustomerSegment, string CustomerSegment, bool ChkActivityType, string ActivityType, bool ChkActivationOrder, string ActivationOrder, bool ChkCommType, string CommissionType,
                                                bool ChkChannel, string Channel, bool ChkParentPayee, string PayeeParent, bool ChkSubChannel, string SubChannel, bool ChkPayee, string Payees, bool ChkPeriod, string Period, bool ChkBatchStatus, string BatchStatus,  string SelectedTab);

        IEnumerable<dynamic> GetSummary(int CountryID, string UserName, string Workflow, string LoggedInUserId, string UserRole, bool ChkSubscriberNumber, string SubscriberNumber, bool ChkCustomerSegment, string CustomerSegment, bool ChkActivityType, string ActivityType, bool ChkActivationOrder, string ActivationOrder, bool ChkCommType, string CommissionType,
                                                bool ChkChannel, string Channel, bool ChkParentPayee, string PayeeParent, bool ChkSubChannel, string SubChannel, bool ChkPayee, string Payees, bool ChkPeriod, string Period, bool ChkBatchStatus, string BatchStatus, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, string SelectedTab);
        string DownloadFile(string CompanyCode, string LoggedInUserId, string LoggedInRoleName, int CountryID, bool ChkSubscriberNumber, string SubscriberNumber, bool ChkCustomerSegment, string CustomerSegment, bool ChkActivityType, string ActivityType, bool ChkActivationOrder, string ActivationOrder, bool ChkCommType, string CommissionType,
                                                bool ChkChannel, string Channel, bool ChkParentPayee, string PayeeParent, bool ChkSubChannel, string SubChannel, bool ChkPayee, string Payees, bool ChkPeriod, string Period, bool ChkBatchStatus, string BatchStatus, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, string SelectedTab, string LoggedinUserName);



    }
}