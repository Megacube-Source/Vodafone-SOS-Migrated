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
    public class LClaimsRestClient:ILClaimsRestClient
    {
         private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LClaimsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        //public IEnumerable<LClaimViewModel> GetAll()
        //{

        //    var request = new RestRequest("api/LClaims/GetLClaims", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<LClaimViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public IEnumerable<LCompanySpecificColumnViewModel> GetCompanySpecificLabels()
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/GetCompanySpecificColumns?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<DownloadClaimsGridViewModel> GetClaimsForReports(string PayeeId, int CompanyId,string StartDate,string EndDate, //int pagesize, int pagenum,string sortdatafield, string sortorder,string FilterQuery,
            string A01Filter, string A01, string A02Filter, string A02, string A03Filter, string A03, string A04Filter, string A04, string A05Filter, string A05,
            string A06Filter, string A06, string A07Filter, string A07, string A08Filter, string A08, string A09Filter, string A09, string A10Filter, string A10,
            string AllocationDateFilter, string AllocationDateFrom,
            string AllocationDateTo, string AlreadyPaidDateFilter, string AlreadyPaidDateFrom, string AlreadyPaidDateTo, string ConnectionDateFilter,
            string ConnectionDateFrom, string ConnectionDateTo, string LastReclaimDateFilter, string LastReclaimDateFrom,
            string LastReclaimDateTo, string OrderDateFilter, string OrderDateFrom, string OrderDateTo, string AlreadyPaidAmountFilter,
            string AlreadyPaidAmountFrom, string AlreadyPaidAmountTo, string ClawbackAmountFilter, string ClawbackAmountFrom, string ClawbackAmountTo,
            string ExpectedCommissionAmountFilter, string ExpectedCommissionAmountFrom, string ExpectedCommissionAmountTo, string PaymentAmountFilter,
            string PaymentAmountFrom, string PaymentAmountTo, string BANFilter, string BAN, string CustomerNameFilter, string CustomerName,
            string IMEIFilter, string IMEI, string MSISDNFilter, string MSISDN, string OrderNumberFilter, string OrderNumber, string PaymentBatchNumberFilter,
            string PaymentBatchNumber, string ReasonNonAutoPaymentFilter, string ReasonNonAutoPayment, string ClaimBatchNumberFilter, string ClaimBatchNumber,
            string ClawbackPayeeCodeFilter, string ClawbackPayeeCode, string BrandIds, string CommissionTypeIds, string DeviceTypeIds, string PaymentCommissionTypeIds,
            string ProductCodeIds, string StatusFilter, string Status, string CreatedByIds, string ActivityTypeIds, Boolean AlreadyPaidDealer, string RejectionReasonIds)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            if (AllocationDateFrom == null) AllocationDateFrom = "";
            if (AllocationDateTo == null) AllocationDateTo = "";
            if (AlreadyPaidDateFrom == null) AlreadyPaidDateFrom = "";
            if (AlreadyPaidDateTo == null) AlreadyPaidDateTo = "";
            if (ConnectionDateFrom == null) ConnectionDateFrom = "";
            if (ConnectionDateTo == null) ConnectionDateTo = "";
            if (LastReclaimDateFrom == null) LastReclaimDateFrom = "";
            if (LastReclaimDateTo == null) LastReclaimDateTo = "";
            if (OrderDateFrom == null) OrderDateFrom = "";
            if (OrderDateTo == null) OrderDateTo = "";
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            //&pagesize={pagesize}&pagenum={pagenum}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&UserName={UserName}&Workflow={Workflow}
            var request = new RestRequest("api/LClaims/GetLClaimsForReports?PayeeId={PayeeId}&StartDate={StartDate}&EndDate={EndDate}&CompanyId={CompanyId}&A01Filter={A01Filter}&A01={A01}&A02Filter={A02Filter}&A02={A02}&A03Filter={A03Filter}&A03={A03}&A04Filter={A04Filter}&A04={A04}&A05Filter={A05Filter}&A05={A05}&A06Filter={A06Filter}&A06={A06}&A07Filter={A07Filter}&A07={A07}&A08Filter={A08Filter}&A08={A08}&A09Filter={A09Filter}&A09={A09}&A10Filter={A10Filter}&A10={A10}&AllocationDateFilter={AllocationDateFilter}&AllocationDateFrom={AllocationDateFrom}&AllocationDateTo={AllocationDateTo}&AlreadyPaidDateFilter={AlreadyPaidDateFilter}&AlreadyPaidDateFrom={AlreadyPaidDateFrom}&AlreadyPaidDateTo={AlreadyPaidDateTo}&ConnectionDateFilter={ConnectionDateFilter}&ConnectionDateFrom={ConnectionDateFrom}&ConnectionDateTo={ConnectionDateTo}&LastReclaimDateFilter={LastReclaimDateFilter}&LastReclaimDateFrom={LastReclaimDateFrom}&LastReclaimDateTo={LastReclaimDateTo}&OrderDateFilter={OrderDateFilter}&OrderDateFrom={OrderDateFrom}&OrderDateTo={OrderDateTo}&AlreadyPaidAmountFilter={AlreadyPaidAmountFilter}&AlreadyPaidAmountFrom={AlreadyPaidAmountFrom}&AlreadyPaidAmountTo={AlreadyPaidAmountTo}&ClawbackAmountFilter={ClawbackAmountFilter}&ClawbackAmountFrom={ClawbackAmountFrom}&ClawbackAmountTo={ClawbackAmountTo}&ExpectedCommissionAmountFilter={ExpectedCommissionAmountFilter}&ExpectedCommissionAmountFrom={ExpectedCommissionAmountFrom}&ExpectedCommissionAmountTo={ExpectedCommissionAmountTo}&PaymentAmountFilter={PaymentAmountFilter}&PaymentAmountFrom={PaymentAmountFrom}&PaymentAmountTo={PaymentAmountTo}&BANFilter={BANFilter}&BAN={BAN}&CustomerNameFilter={CustomerNameFilter}&CustomerName={CustomerName}&IMEIFilter={IMEIFilter}&IMEI={IMEI}&MSISDNFilter={MSISDNFilter}&MSISDN={MSISDN}&OrderNumberFilter={OrderNumberFilter}&OrderNumber={OrderNumber}&PaymentBatchNumberFilter={PaymentBatchNumberFilter}&PaymentBatchNumber={PaymentBatchNumber}&ReasonNonAutoPaymentFilter={ReasonNonAutoPaymentFilter}&ReasonNonAutoPayment={ReasonNonAutoPayment}&ClaimBatchNumberFilter={ClaimBatchNumberFilter}&ClaimBatchNumber={ClaimBatchNumber}&ClawbackPayeeCodeFilter={ClawbackPayeeCodeFilter}&ClawbackPayeeCode={ClawbackPayeeCode}&BrandIds={BrandIds}&CommissionTypeIds={CommissionTypeIds}&DeviceTypeIds={DeviceTypeIds}&PaymentCommissionTypeIds={PaymentCommissionTypeIds}&ProductCodeIds={ProductCodeIds}&StatusFilter={StatusFilter}&Status={Status}&CreatedByIds={CreatedByIds}&ActivityTypeIds={ActivityTypeIds}&AlreadyPaidDealer={AlreadyPaidDealer}&RejectionReasonIds={RejectionReasonIds}", Method.GET) { RequestFormat = DataFormat.Json };
            //&BANFilter={BANFilter}&BAN={BAN}&CustomerNameFilter={CustomerNameFilter}&CustomerName={CustomerName}&IMEIFilter={IMEIFilter}&IMEI={IMEI}&MSISDNFilter={MSISDNFilter}&MSISDN={MSISDN}&OrderNumberFilter={OrderNumberFilter}&OrderNumber={OrderNumber}&PaymentBatchNumberFilter={PaymentBatchNumberFilter}&PaymentBatchNumber={PaymentBatchNumber}&ReasonNonAutoPaymentFilter={ReasonNonAutoPaymentFilter}&ReasonNonAutoPayment={ReasonNonAutoPayment}&ClaimBatchNumberFilter={ClaimBatchNumberFilter}&ClaimBatchNumber={ClaimBatchNumber}&ClawbackPayeeCodeFilter={ClawbackPayeeCodeFilter}&ClawbackPayeeCode={ClawbackPayeeCode}&BrandIds={BrandIds}&CommissionTypeIds={CommissionTypeIds}&DeviceTypeIds={DeviceTypeIds}&PaymentCommissionTypeIds={PaymentCommissionTypeIds}&ProductCodeIds={ProductCodeIds}&StatusFilter={StatusFilter}&Status={Status}&CreatedByIds={CreatedByIds}&ActivityTypeIds={ActivityTypeIds}&AlreadyPaidDealer={AlreadyPaidDealer}
            request.AddParameter("PayeeId", string.IsNullOrEmpty(PayeeId)?string.Empty:PayeeId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);
            //request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            //request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            //request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            //request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            //request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            //request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            //request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("A01Filter", A01Filter, ParameterType.UrlSegment);
            request.AddParameter("A01", A01, ParameterType.UrlSegment);
            request.AddParameter("A02Filter", A02Filter, ParameterType.UrlSegment);
            request.AddParameter("A02", A02, ParameterType.UrlSegment);
            request.AddParameter("A03Filter", A03Filter, ParameterType.UrlSegment);
            request.AddParameter("A03", A03, ParameterType.UrlSegment);
            request.AddParameter("A04Filter", A04Filter, ParameterType.UrlSegment);
            request.AddParameter("A04", A04, ParameterType.UrlSegment);
            request.AddParameter("A05Filter", A05Filter, ParameterType.UrlSegment);
            request.AddParameter("A05", A05, ParameterType.UrlSegment);
            request.AddParameter("A06Filter", A06Filter, ParameterType.UrlSegment);
            request.AddParameter("A06", A06, ParameterType.UrlSegment);
            request.AddParameter("A07Filter", A07Filter, ParameterType.UrlSegment);
            request.AddParameter("A07", A07, ParameterType.UrlSegment);
            request.AddParameter("A08Filter", A08Filter, ParameterType.UrlSegment);
            request.AddParameter("A08", A08, ParameterType.UrlSegment);
            request.AddParameter("A09Filter", A09Filter, ParameterType.UrlSegment);
            request.AddParameter("A09", A09, ParameterType.UrlSegment);
            request.AddParameter("A10Filter", A10Filter, ParameterType.UrlSegment);
            request.AddParameter("A10", A10, ParameterType.UrlSegment);
            //
            request.AddParameter("AllocationDateFilter", AllocationDateFilter, ParameterType.UrlSegment);
            request.AddParameter("AllocationDateFrom", AllocationDateFrom, ParameterType.UrlSegment);
            request.AddParameter("AllocationDateTo", AllocationDateTo, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidDateFilter", AlreadyPaidDateFilter, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidDateFrom", AlreadyPaidDateFrom, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidDateTo", AlreadyPaidDateTo, ParameterType.UrlSegment);
            request.AddParameter("ConnectionDateFilter", ConnectionDateFilter, ParameterType.UrlSegment);
            request.AddParameter("ConnectionDateFrom", ConnectionDateFrom, ParameterType.UrlSegment); 
            request.AddParameter("ConnectionDateTo", ConnectionDateTo, ParameterType.UrlSegment); 
            request.AddParameter("LastReclaimDateFilter", LastReclaimDateFilter, ParameterType.UrlSegment);
            request.AddParameter("LastReclaimDateFrom", LastReclaimDateFrom, ParameterType.UrlSegment);
            request.AddParameter("LastReclaimDateTo", LastReclaimDateTo, ParameterType.UrlSegment);
            request.AddParameter("OrderDateFilter", OrderDateFilter, ParameterType.UrlSegment);
            request.AddParameter("OrderDateFrom", OrderDateFrom, ParameterType.UrlSegment);
            request.AddParameter("OrderDateTo", OrderDateTo, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidAmountFilter", AlreadyPaidAmountFilter, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidAmountFrom", AlreadyPaidAmountFrom, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidAmountTo", AlreadyPaidAmountTo, ParameterType.UrlSegment);
            request.AddParameter("ClawbackAmountFilter", ClawbackAmountFilter, ParameterType.UrlSegment);
            request.AddParameter("ClawbackAmountFrom", ClawbackAmountFrom, ParameterType.UrlSegment);
            request.AddParameter("ClawbackAmountTo", ClawbackAmountTo, ParameterType.UrlSegment);
            request.AddParameter("ExpectedCommissionAmountFilter", ExpectedCommissionAmountFilter, ParameterType.UrlSegment);
            request.AddParameter("ExpectedCommissionAmountFrom", ExpectedCommissionAmountFrom, ParameterType.UrlSegment);
            request.AddParameter("ExpectedCommissionAmountTo", ExpectedCommissionAmountTo, ParameterType.UrlSegment);
            request.AddParameter("PaymentAmountFilter", PaymentAmountFilter, ParameterType.UrlSegment);
            request.AddParameter("PaymentAmountFrom", PaymentAmountFrom, ParameterType.UrlSegment);
            request.AddParameter("PaymentAmountTo", PaymentAmountTo, ParameterType.UrlSegment);

            request.AddParameter("BANFilter", BANFilter, ParameterType.UrlSegment);
            request.AddParameter("BAN", BAN, ParameterType.UrlSegment);
            request.AddParameter("CustomerNameFilter", CustomerNameFilter, ParameterType.UrlSegment);
            request.AddParameter("CustomerName", CustomerName, ParameterType.UrlSegment);
            request.AddParameter("IMEIFilter", IMEIFilter, ParameterType.UrlSegment);
            request.AddParameter("IMEI", IMEI, ParameterType.UrlSegment);
            request.AddParameter("MSISDNFilter", MSISDNFilter, ParameterType.UrlSegment);
            request.AddParameter("MSISDN", MSISDN, ParameterType.UrlSegment);
            request.AddParameter("OrderNumberFilter", OrderNumberFilter, ParameterType.UrlSegment);
            request.AddParameter("OrderNumber", OrderNumber, ParameterType.UrlSegment);
            request.AddParameter("PaymentBatchNumberFilter", PaymentBatchNumberFilter, ParameterType.UrlSegment);
            request.AddParameter("PaymentBatchNumber", PaymentBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("ReasonNonAutoPaymentFilter", ReasonNonAutoPaymentFilter, ParameterType.UrlSegment);
            request.AddParameter("ReasonNonAutoPayment", ReasonNonAutoPayment, ParameterType.UrlSegment);
            request.AddParameter("ClaimBatchNumberFilter", ClaimBatchNumberFilter, ParameterType.UrlSegment);
            request.AddParameter("ClaimBatchNumber", ClaimBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("ClawbackPayeeCodeFilter", ClawbackPayeeCodeFilter, ParameterType.UrlSegment);
            request.AddParameter("ClawbackPayeeCode", ClawbackPayeeCode, ParameterType.UrlSegment);
            request.AddParameter("BrandIds", BrandIds, ParameterType.UrlSegment);
            request.AddParameter("CommissionTypeIds", CommissionTypeIds, ParameterType.UrlSegment);
            request.AddParameter("DeviceTypeIds", DeviceTypeIds, ParameterType.UrlSegment);
            request.AddParameter("PaymentCommissionTypeIds", PaymentCommissionTypeIds, ParameterType.UrlSegment);
            request.AddParameter("ProductCodeIds", ProductCodeIds, ParameterType.UrlSegment);
            request.AddParameter("StatusFilter", StatusFilter, ParameterType.UrlSegment);
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            request.AddParameter("CreatedByIds", CreatedByIds, ParameterType.UrlSegment);
            request.AddParameter("ActivityTypeIds", ActivityTypeIds, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidDealer", AlreadyPaidDealer, ParameterType.UrlSegment);
            request.AddParameter("RejectionReasonIds", RejectionReasonIds, ParameterType.UrlSegment);

            var response = _client.Execute<List<DownloadClaimsGridViewModel>>(request);

            return response.Data;
        }
        public string DownloadClaimsForReports(string PayeeId, int CompanyId, string StartDate, string EndDate, //int pagesize, int pagenum,string sortdatafield, string sortorder,string FilterQuery,
            string A01Filter, string A01, string A02Filter, string A02, string A03Filter, string A03, string A04Filter, string A04, string A05Filter, string A05,
            string A06Filter, string A06, string A07Filter, string A07, string A08Filter, string A08, string A09Filter, string A09, string A10Filter, string A10,
            string AllocationDateFilter, string AllocationDateFrom,
            string AllocationDateTo, string AlreadyPaidDateFilter, string AlreadyPaidDateFrom, string AlreadyPaidDateTo, string ConnectionDateFilter,
            string ConnectionDateFrom, string ConnectionDateTo, string LastReclaimDateFilter, string LastReclaimDateFrom,
            string LastReclaimDateTo, string OrderDateFilter, string OrderDateFrom, string OrderDateTo, string AlreadyPaidAmountFilter,
            string AlreadyPaidAmountFrom, string AlreadyPaidAmountTo, string ClawbackAmountFilter, string ClawbackAmountFrom, string ClawbackAmountTo,
            string ExpectedCommissionAmountFilter, string ExpectedCommissionAmountFrom, string ExpectedCommissionAmountTo, string PaymentAmountFilter,
            string PaymentAmountFrom, string PaymentAmountTo, string BANFilter, string BAN, string CustomerNameFilter, string CustomerName,
            string IMEIFilter, string IMEI, string MSISDNFilter, string MSISDN, string OrderNumberFilter, string OrderNumber, string PaymentBatchNumberFilter,
            string PaymentBatchNumber, string ReasonNonAutoPaymentFilter, string ReasonNonAutoPayment, string ClaimBatchNumberFilter, string ClaimBatchNumber,
            string ClawbackPayeeCodeFilter, string ClawbackPayeeCode, string BrandIds, string CommissionTypeIds, string DeviceTypeIds, string PaymentCommissionTypeIds,
            string ProductCodeIds, string StatusFilter, string Status, string CreatedByIds, string ActivityTypeIds, Boolean AlreadyPaidDealer, string RejectionReasonIds)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            //string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            //var request = new RestRequest("api/LClaims/DownloadLClaimsForReports?PayeeId={PayeeId}&StartDate={StartDate}&EndDate={EndDate}&CompanyId={CompanyId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };

            if (AllocationDateFrom == null) AllocationDateFrom = "";
            if (AllocationDateTo == null) AllocationDateTo = "";
            if (AlreadyPaidDateFrom == null) AlreadyPaidDateFrom = "";
            if (AlreadyPaidDateTo == null) AlreadyPaidDateTo = "";
            if (ConnectionDateFrom == null) ConnectionDateFrom = "";
            if (ConnectionDateTo == null) ConnectionDateTo = "";
            if (LastReclaimDateFrom == null) LastReclaimDateFrom = "";
            if (LastReclaimDateTo == null) LastReclaimDateTo = "";
            if (OrderDateFrom == null) OrderDateFrom = "";
            if (OrderDateTo == null) OrderDateTo = "";
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/DownloadLClaimsForReports?PayeeId={PayeeId}&StartDate={StartDate}&EndDate={EndDate}&CompanyId={CompanyId}&A01Filter={A01Filter}&A01={A01}&A02Filter={A02Filter}&A02={A02}&A03Filter={A03Filter}&A03={A03}&A04Filter={A04Filter}&A04={A04}&A05Filter={A05Filter}&A05={A05}&A06Filter={A06Filter}&A06={A06}&A07Filter={A07Filter}&A07={A07}&A08Filter={A08Filter}&A08={A08}&A09Filter={A09Filter}&A09={A09}&A10Filter={A10Filter}&A10={A10}&AllocationDateFilter={AllocationDateFilter}&AllocationDateFrom={AllocationDateFrom}&AllocationDateTo={AllocationDateTo}&AlreadyPaidDateFilter={AlreadyPaidDateFilter}&AlreadyPaidDateFrom={AlreadyPaidDateFrom}&AlreadyPaidDateTo={AlreadyPaidDateTo}&ConnectionDateFilter={ConnectionDateFilter}&ConnectionDateFrom={ConnectionDateFrom}&ConnectionDateTo={ConnectionDateTo}&LastReclaimDateFilter={LastReclaimDateFilter}&LastReclaimDateFrom={LastReclaimDateFrom}&LastReclaimDateTo={LastReclaimDateTo}&OrderDateFilter={OrderDateFilter}&OrderDateFrom={OrderDateFrom}&OrderDateTo={OrderDateTo}&AlreadyPaidAmountFilter={AlreadyPaidAmountFilter}&AlreadyPaidAmountFrom={AlreadyPaidAmountFrom}&AlreadyPaidAmountTo={AlreadyPaidAmountTo}&ClawbackAmountFilter={ClawbackAmountFilter}&ClawbackAmountFrom={ClawbackAmountFrom}&ClawbackAmountTo={ClawbackAmountTo}&ExpectedCommissionAmountFilter={ExpectedCommissionAmountFilter}&ExpectedCommissionAmountFrom={ExpectedCommissionAmountFrom}&ExpectedCommissionAmountTo={ExpectedCommissionAmountTo}&PaymentAmountFilter={PaymentAmountFilter}&PaymentAmountFrom={PaymentAmountFrom}&PaymentAmountTo={PaymentAmountTo}&BANFilter={BANFilter}&BAN={BAN}&CustomerNameFilter={CustomerNameFilter}&CustomerName={CustomerName}&IMEIFilter={IMEIFilter}&IMEI={IMEI}&MSISDNFilter={MSISDNFilter}&MSISDN={MSISDN}&OrderNumberFilter={OrderNumberFilter}&OrderNumber={OrderNumber}&PaymentBatchNumberFilter={PaymentBatchNumberFilter}&PaymentBatchNumber={PaymentBatchNumber}&ReasonNonAutoPaymentFilter={ReasonNonAutoPaymentFilter}&ReasonNonAutoPayment={ReasonNonAutoPayment}&ClaimBatchNumberFilter={ClaimBatchNumberFilter}&ClaimBatchNumber={ClaimBatchNumber}&ClawbackPayeeCodeFilter={ClawbackPayeeCodeFilter}&ClawbackPayeeCode={ClawbackPayeeCode}&BrandIds={BrandIds}&CommissionTypeIds={CommissionTypeIds}&DeviceTypeIds={DeviceTypeIds}&PaymentCommissionTypeIds={PaymentCommissionTypeIds}&ProductCodeIds={ProductCodeIds}&StatusFilter={StatusFilter}&Status={Status}&CreatedByIds={CreatedByIds}&ActivityTypeIds={ActivityTypeIds}&AlreadyPaidDealer={AlreadyPaidDealer}&UserName={UserName}&RejectionReasonIds={RejectionReasonIds}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PayeeId", string.IsNullOrEmpty(PayeeId) ? string.Empty : PayeeId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);
            request.AddParameter("A01Filter", A01Filter, ParameterType.UrlSegment);
            request.AddParameter("A01", A01, ParameterType.UrlSegment);
            request.AddParameter("A02Filter", A02Filter, ParameterType.UrlSegment);
            request.AddParameter("A02", A02, ParameterType.UrlSegment);
            request.AddParameter("A03Filter", A03Filter, ParameterType.UrlSegment);
            request.AddParameter("A03", A03, ParameterType.UrlSegment);
            request.AddParameter("A04Filter", A04Filter, ParameterType.UrlSegment);
            request.AddParameter("A04", A04, ParameterType.UrlSegment);
            request.AddParameter("A05Filter", A05Filter, ParameterType.UrlSegment);
            request.AddParameter("A05", A05, ParameterType.UrlSegment);
            request.AddParameter("A06Filter", A06Filter, ParameterType.UrlSegment);
            request.AddParameter("A06", A06, ParameterType.UrlSegment);
            request.AddParameter("A07Filter", A07Filter, ParameterType.UrlSegment);
            request.AddParameter("A07", A07, ParameterType.UrlSegment);
            request.AddParameter("A08Filter", A08Filter, ParameterType.UrlSegment);
            request.AddParameter("A08", A08, ParameterType.UrlSegment);
            request.AddParameter("A09Filter", A09Filter, ParameterType.UrlSegment);
            request.AddParameter("A09", A09, ParameterType.UrlSegment);
            request.AddParameter("A10Filter", A10Filter, ParameterType.UrlSegment);
            request.AddParameter("A10", A10, ParameterType.UrlSegment);
            //
            request.AddParameter("AllocationDateFilter", AllocationDateFilter, ParameterType.UrlSegment);
            request.AddParameter("AllocationDateFrom", AllocationDateFrom, ParameterType.UrlSegment);
            request.AddParameter("AllocationDateTo", AllocationDateTo, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidDateFilter", AlreadyPaidDateFilter, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidDateFrom", AlreadyPaidDateFrom, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidDateTo", AlreadyPaidDateTo, ParameterType.UrlSegment);
            request.AddParameter("ConnectionDateFilter", ConnectionDateFilter, ParameterType.UrlSegment);
            request.AddParameter("ConnectionDateFrom", ConnectionDateFrom, ParameterType.UrlSegment);
            request.AddParameter("ConnectionDateTo", ConnectionDateTo, ParameterType.UrlSegment);
            request.AddParameter("LastReclaimDateFilter", LastReclaimDateFilter, ParameterType.UrlSegment);
            request.AddParameter("LastReclaimDateFrom", LastReclaimDateFrom, ParameterType.UrlSegment);
            request.AddParameter("LastReclaimDateTo", LastReclaimDateTo, ParameterType.UrlSegment);
            request.AddParameter("OrderDateFilter", OrderDateFilter, ParameterType.UrlSegment);
            request.AddParameter("OrderDateFrom", OrderDateFrom, ParameterType.UrlSegment);
            request.AddParameter("OrderDateTo", OrderDateTo, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidAmountFilter", AlreadyPaidAmountFilter, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidAmountFrom", AlreadyPaidAmountFrom, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidAmountTo", AlreadyPaidAmountTo, ParameterType.UrlSegment);
            request.AddParameter("ClawbackAmountFilter", ClawbackAmountFilter, ParameterType.UrlSegment);
            request.AddParameter("ClawbackAmountFrom", ClawbackAmountFrom, ParameterType.UrlSegment);
            request.AddParameter("ClawbackAmountTo", ClawbackAmountTo, ParameterType.UrlSegment);
            request.AddParameter("ExpectedCommissionAmountFilter", ExpectedCommissionAmountFilter, ParameterType.UrlSegment);
            request.AddParameter("ExpectedCommissionAmountFrom", ExpectedCommissionAmountFrom, ParameterType.UrlSegment);
            request.AddParameter("ExpectedCommissionAmountTo", ExpectedCommissionAmountTo, ParameterType.UrlSegment);
            request.AddParameter("PaymentAmountFilter", PaymentAmountFilter, ParameterType.UrlSegment);
            request.AddParameter("PaymentAmountFrom", PaymentAmountFrom, ParameterType.UrlSegment);
            request.AddParameter("PaymentAmountTo", PaymentAmountTo, ParameterType.UrlSegment);

            request.AddParameter("BANFilter", BANFilter, ParameterType.UrlSegment);
            request.AddParameter("BAN", BAN, ParameterType.UrlSegment);
            request.AddParameter("CustomerNameFilter", CustomerNameFilter, ParameterType.UrlSegment);
            request.AddParameter("CustomerName", CustomerName, ParameterType.UrlSegment);
            request.AddParameter("IMEIFilter", IMEIFilter, ParameterType.UrlSegment);
            request.AddParameter("IMEI", IMEI, ParameterType.UrlSegment);
            request.AddParameter("MSISDNFilter", MSISDNFilter, ParameterType.UrlSegment);
            request.AddParameter("MSISDN", MSISDN, ParameterType.UrlSegment);
            request.AddParameter("OrderNumberFilter", OrderNumberFilter, ParameterType.UrlSegment);
            request.AddParameter("OrderNumber", OrderNumber, ParameterType.UrlSegment);
            request.AddParameter("PaymentBatchNumberFilter", PaymentBatchNumberFilter, ParameterType.UrlSegment);
            request.AddParameter("PaymentBatchNumber", PaymentBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("ReasonNonAutoPaymentFilter", ReasonNonAutoPaymentFilter, ParameterType.UrlSegment);
            request.AddParameter("ReasonNonAutoPayment", ReasonNonAutoPayment, ParameterType.UrlSegment);
            request.AddParameter("ClaimBatchNumberFilter", ClaimBatchNumberFilter, ParameterType.UrlSegment);
            request.AddParameter("ClaimBatchNumber", ClaimBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("ClawbackPayeeCodeFilter", ClawbackPayeeCodeFilter, ParameterType.UrlSegment);
            request.AddParameter("ClawbackPayeeCode", ClawbackPayeeCode, ParameterType.UrlSegment);
            request.AddParameter("BrandIds", BrandIds, ParameterType.UrlSegment);
            request.AddParameter("CommissionTypeIds", CommissionTypeIds, ParameterType.UrlSegment);
            request.AddParameter("DeviceTypeIds", DeviceTypeIds, ParameterType.UrlSegment);
            request.AddParameter("PaymentCommissionTypeIds", PaymentCommissionTypeIds, ParameterType.UrlSegment);
            request.AddParameter("ProductCodeIds", ProductCodeIds, ParameterType.UrlSegment);
            request.AddParameter("StatusFilter", StatusFilter, ParameterType.UrlSegment);
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            request.AddParameter("CreatedByIds", CreatedByIds, ParameterType.UrlSegment);
            request.AddParameter("ActivityTypeIds", ActivityTypeIds, ParameterType.UrlSegment);
            request.AddParameter("AlreadyPaidDealer", AlreadyPaidDealer, ParameterType.UrlSegment);
            request.AddParameter("PayeeId", string.IsNullOrEmpty(PayeeId) ? string.Empty : PayeeId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("RejectionReasonIds", RejectionReasonIds, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }
        //Old Download method, Commented by RK
        //public string DownloadClaimsForReports(string PayeeId, int CompanyId, string StartDate, string EndDate,string LoggedInUserName)
        //{
            
        //    string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
        //    if (string.IsNullOrEmpty(Workflow))
        //    {
        //        Workflow = "No Workflow";
        //    }
        //    string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
        //    var request = new RestRequest("api/LClaims/DownloadLClaimsForReports?PayeeId={PayeeId}&StartDate={StartDate}&EndDate={EndDate}&CompanyId={CompanyId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("PayeeId", PayeeId, ParameterType.UrlSegment);
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
        //    request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);
        //    request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
        //    request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
        //    var response = _client.Execute(request);
        //    string source = response.Content;
        //    dynamic data = JsonConvert.DeserializeObject(source);
        //    return data;
        //}
        //method added by Ritu to count grid data

        //method added by Ritu to count grid data
        public int GetClaimsForReportCounts(string PayeeId, int CompanyId, string StartDate, string EndDate)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/GetLClaimsForReportCounts?PayeeId={PayeeId}&StartDate={StartDate}&EndDate={EndDate}&CompanyId={CompanyId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PayeeId", string.IsNullOrEmpty(PayeeId) ? string.Empty : PayeeId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("StartDate", StartDate, ParameterType.UrlSegment);
            request.AddParameter("EndDate", EndDate, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            return response.Data;
        }

        //public IEnumerable<DownloadClaimsGridViewModel> DownloadByStatusName(string StatusName, int CompanyId)
        //{
        //    var request = new RestRequest("api/LClaims/DownloadLClaimsByStatusNameCompanyId?StatusName={StatusName}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<DownloadClaimsGridViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
        //public IEnumerable<DownloadClaimsGridViewModel> DownloadByStatusNameCreatedById(string StatusName, int CompanyId, string CreatedByUserId)
        //{
        //    var request = new RestRequest("api/LClaims/DownloadLClaimsByStatusNameCompanyIdCreatedById?StatusName={StatusName}&CompanyId={CompanyId}&CreatedByUserId={CreatedByUserId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("CreatedByUserId", CreatedByUserId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<DownloadClaimsGridViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        public IEnumerable<LClaimViewModel> GetByStatusName(string StatusName, int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/GetLClaimsByStatusNameCompanyId?StatusName={StatusName}&CompanyId={CompanyId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("StatusName",StatusName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LClaimViewModel>>(request);
            
            return response.Data;
        }
        //public IEnumerable<LClaimViewModel> GetByStatusNameCreatedById(string StatusName, int CompanyId, string CreatedByUserId)
        //{
        //    var request = new RestRequest("api/LClaims/GetLClaimsByStatusNameCompanyIdCreatedById?StatusName={StatusName}&CompanyId={CompanyId}&CreatedByUserId={CreatedByUserId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("CreatedByUserId", CreatedByUserId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LClaimViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}


        public IEnumerable<LClaimViewModel> GetByStatusNameAllocatedToId(string StatusName, int CompanyId, string AllocatedToUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/GetLClaimsByStatusCompanyIdAllocatedToId?StatusName={StatusName}&CompanyId={CompanyId}&AllocatedToUserId={AllocatedToUserId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("AllocatedToUserId", AllocatedToUserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LClaimViewModel>>(request);

            return response.Data;
        }
        public IEnumerable<LClaimsViewModelForGrid> GetByStatusNamePayeeUserIdForGrid(string StatusName, int CompanyId, string CreatedByUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/GetLClaimsByStatusCompanyIdPayeeUserIdForGrid?StatusName={StatusName}&CompanyId={CompanyId}&CreatedByUserId={CreatedByUserId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("CreatedByUserId", CreatedByUserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LClaimsViewModelForGrid>>(request);
            
            return response.Data;
        }

        

        public LClaimViewModel GetById(int id)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/GetLClaim/{id}?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<List<LClaimViewModel>>(request);
            if (response.Data == null||response.Data.Count==0)
                return new LClaimViewModel();
            // var res = JsonConvert.DeserializeObject<LClaimViewModel>(response.Content);
            return response.Data[0];//As we are expecting only one row
            //return res;
        }

        public int Add(LClaimViewModel serverData,string WorkflowName,string AttachmentPath,string LoggedInRoleId,string RedirectToUrl)
        {
            //string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(WorkflowName))
            {
                WorkflowName = "Claims";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

            //PostLClaim(LClaimDecryptedViewModel LClaim, string AtachedFiles, string LoggedInRoleId, string AttachmentPath, string UserName, string Workflow)

            var request = new RestRequest("api/LClaims/PostLClaim?AtachedFiles={AtachedFiles}&LoggedInRoleId={LoggedInRoleId}&AttachmentPath={AttachmentPath}&UserName={UserName}&Workflow={Workflow}", Method.POST) { RequestFormat = DataFormat.Json };
           //SS Need this line to supply file names which are uploaded by user //RK21062017request.AddParameter("AtachedFiles", (string.IsNullOrEmpty(serverData.FileName))?"null":serverData.FileName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("AttachmentPath", AttachmentPath, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", WorkflowName, ParameterType.UrlSegment);
            request.AddParameter("AtachedFiles", (string.IsNullOrEmpty(serverData.FileName)) ? "null" : serverData.FileName, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<int>(request);

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
            return response.Data;
        }
        //validate
        public DataTable ValidateUploadClaims(List<LClaimViewModel> LClaimViewModel,string FileName,string LoggedInRoleId, string WorkflowName,string RedirectToUrl, int iCompanyId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/ValidateUploadLClaims?FileName={FileName}&LoggedInRoleId={LoggedInRoleId}&UserName={UserName}&Workflow={Workflow}&iCompanyId={iCompanyId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(LClaimViewModel);
            request.AddParameter("FileName", FileName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", WorkflowName, ParameterType.UrlSegment);
            request.AddParameter("iCompanyId", iCompanyId, ParameterType.UrlSegment);
            
            var response = _client.Execute<dynamic>(request);
           
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
            var res = JsonConvert.DeserializeObject<DataTable>(response.Content);
            return res;
        }
        public void UploadClaims( string LoggedInRoleId, string WorkflowName, string RedirectToUrl, int iCompanyId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/UploadClaims?LoggedInRoleId={LoggedInRoleId}&UserName={UserName}&Workflow={Workflow}&iCompanyId={iCompanyId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", WorkflowName, ParameterType.UrlSegment);
            request.AddParameter("iCompanyId", iCompanyId, ParameterType.UrlSegment);
            
            var response = _client.Execute<List<LClaimViewModel>>(request);
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
        public void Update(LClaimViewModel serverData,string AttachmentPath,string RedirectToUrl)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/PutLClaim/{id}?AtachedFiles={AtachedFiles}&AttachmentPath={AttachmentPath}&UserName={UserName}&Workflow={Workflow}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            //SS Need this statement to pass file names which are uploaded by user//RK21062017request.AddParameter("AtachedFiles", (string.IsNullOrEmpty(serverData.FileName)) ? "null" : serverData.FileName, ParameterType.UrlSegment);
            request.AddParameter("AtachedFiles", (string.IsNullOrEmpty(serverData.FileName)) ? "null" : serverData.FileName, ParameterType.UrlSegment);
            request.AddParameter("AttachmentPath", AttachmentPath, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LClaimViewModel>(request);
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

        public void Delete(int id,string RedirectToUrl)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/DeleteLClaim/{id}?UserName={UserName}&Workflow={Workflow}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<LClaimViewModel>(request);

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
        public string GetMyClaimsReport(string UserID, int CompanyID, string CompanyCode, string LoggedInUserName)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/GetMyClaimsReportData?UserID={UserID}&CompanyID={CompanyID}&CompanyCode={CompanyCode}&LoggedInUserName={LoggedInUserName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserID", UserID, ParameterType.UrlSegment);
            request.AddParameter("CompanyID", CompanyID, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserName", LoggedInUserName, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            var res = response.Data.ToString(); 
            return res;
            
        }
        public string GetMyClaimsAuditData(string UserID, int CompanyID, string CompanyCode, string LoggedInUserName)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LClaims/GetMyClaimsAuditData?UserID={UserID}&CompanyID={CompanyID}&CompanyCode={CompanyCode}&LoggedInUserName={LoggedInUserName}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserID", UserID, ParameterType.UrlSegment);
            request.AddParameter("CompanyID", CompanyID, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserName", LoggedInUserName, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            var res = response.Data.ToString();
            return res;

        }
        public string GetSudmitableorNot(int Transactionid, string Workflow, string Role, int CompanyID)
        {
            if (Workflow == null) Workflow = "";
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
        public IEnumerable<RChannelViewModel> GetPayeesChannels(int PayeeID, int CompanyId)
        {

            var request = new RestRequest("api/LClaims/GetPayeesChannels?PayeeID={PayeeID}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PayeeID", PayeeID, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);

            var response = _client.Execute<List<RChannelViewModel>>(request);

            return response.Data;
        }
        public IEnumerable<RChannelViewModel> GetByRChannelByTransactionID(int? TransactionID)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;           
            var request = new RestRequest("api/LClaims/GetByRChannelByTransactionID?TransactionID={TransactionID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("TransactionID", TransactionID, ParameterType.UrlSegment);
            
            var response = _client.Execute<List<RChannelViewModel>>(request);
            return response.Data;

        }

        public IEnumerable<SelectedItemViewModel> GetNextUserDetails(string workflow, int CompanyID, string LoggedInUserId, string LoggedinRoleID)
        {
            var request = new RestRequest("api/LClaims/GetNextUserDetails?Workflow={Workflow}&CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}&LoggedinRoleID={LoggedinRoleID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Workflow", workflow, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyID, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedinRoleID", LoggedinRoleID, ParameterType.UrlSegment);
            var response = _client.Execute<List<SelectedItemViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
    }

    interface ILClaimsRestClient
    {
        string DownloadClaimsForReports(string PayeeId, int CompanyId, string StartDate, string EndDate,//int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery,
            string A01Filter, string A01, string A02Filter, string A02, string A03Filter, string A03, string A04Filter, string A04, string A05Filter, string A05,
            string A06Filter, string A06, string A07Filter, string A07, string A08Filter, string A08, string A09Filter, string A09, string A10Filter, string A10,
            string AllocationDateFilter, string AllocationDateFrom,
            string AllocationDateTo, string AlreadyPaidDateFilter, string AlreadyPaidDateFrom, string AlreadyPaidDateTo, string ConnectionDateFilter,
            string ConnectionDateFrom, string ConnectionDateTo, string LastReclaimDateFilter, string LastReclaimDateFrom,
            string LastReclaimDateTo, string OrderDateFilter, string OrderDateFrom, string OrderDateTo, string AlreadyPaidAmountFilter,
            string AlreadyPaidAmountFrom, string AlreadyPaidAmountTo, string ClawbackAmountFilter, string ClawbackAmountFrom, string ClawbackAmountTo,
            string ExpectedCommissionAmountFilter, string ExpectedCommissionAmountFrom, string ExpectedCommissionAmountTo, string PaymentAmountFilter,
            string PaymentAmountFrom, string PaymentAmountTo, string BANFilter, string BAN, string CustomerNameFilter, string CustomerName,
            string IMEIFilter, string IMEI, string MSISDNFilter, string MSISDN, string OrderNumberFilter, string OrderNumber, string PaymentBatchNumberFilter,
            string PaymentBatchNumber, string ReasonNonAutoPaymentFilter, string ReasonNonAutoPayment, string ClaimBatchNumberFilter, string ClaimBatchNumber,
            string ClawbackPayeeCodeFilter, string ClawbackPayeeCode, string BrandIds, string CommissionTypeIds, string DeviceTypeIds, string PaymentCommissionTypeIds,
            string ProductCodeIds, string StatusFilter, string Status, string CreatedByIds, string ActivityTypeIds, Boolean AlreadyPaidDealer,string RejectionReasonIds);
      //  IEnumerable<LClaimViewModel> GetAll();
        IEnumerable<DownloadClaimsGridViewModel> GetClaimsForReports(string PayeeId, int CompanyId, string StartDate, string EndDate,//int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery,
            string A01Filter, string A01, string A02Filter, string A02, string A03Filter, string A03, string A04Filter, string A04, string A05Filter, string A05,
            string A06Filter, string A06, string A07Filter, string A07, string A08Filter, string A08, string A09Filter, string A09, string A10Filter, string A10,
            string AllocationDateFilter, string AllocationDateFrom,
            string AllocationDateTo, string AlreadyPaidDateFilter, string AlreadyPaidDateFrom, string AlreadyPaidDateTo, string ConnectionDateFilter,
            string ConnectionDateFrom, string ConnectionDateTo, string LastReclaimDateFilter, string LastReclaimDateFrom,
            string LastReclaimDateTo, string OrderDateFilter, string OrderDateFrom, string OrderDateTo, string AlreadyPaidAmountFilter,
            string AlreadyPaidAmountFrom, string AlreadyPaidAmountTo, string ClawbackAmountFilter, string ClawbackAmountFrom, string ClawbackAmountTo,
            string ExpectedCommissionAmountFilter, string ExpectedCommissionAmountFrom, string ExpectedCommissionAmountTo, string PaymentAmountFilter,
            string PaymentAmountFrom, string PaymentAmountTo,string BANFilter, string BAN, string CustomerNameFilter, string CustomerName,
            string IMEIFilter, string IMEI, string MSISDNFilter, string MSISDN, string OrderNumberFilter, string OrderNumber, string PaymentBatchNumberFilter,
            string PaymentBatchNumber, string ReasonNonAutoPaymentFilter, string ReasonNonAutoPayment, string ClaimBatchNumberFilter, string ClaimBatchNumber,
            string ClawbackPayeeCodeFilter, string ClawbackPayeeCode, string BrandIds, string CommissionTypeIds, string DeviceTypeIds, string PaymentCommissionTypeIds,
            string ProductCodeIds, string StatusFilter, string Status, string CreatedByIds, string ActivityTypeIds, Boolean AlreadyPaidDealer, string RejectionReasonIds);
        IEnumerable<LClaimViewModel> GetByStatusName(string StatusName, int CompanyId);
        int GetClaimsForReportCounts(string PayeeId, int CompanyId, string StartDate, string EndDate);
       // IEnumerable<LClaimViewModel> GetByStatusNameCreatedById(string StatusName, int CompanyId, string CreatedByUserId);
        IEnumerable<LClaimViewModel> GetByStatusNameAllocatedToId(string StatusName, int CompanyId, string AllocatedToUserId);
      //  IEnumerable<DownloadClaimsGridViewModel> DownloadByStatusNameCreatedById(string StatusName, int CompanyId, string CreatedByUserId);
       // IEnumerable<DownloadClaimsGridViewModel> DownloadByStatusName(string StatusName, int CompanyId);
        IEnumerable<LClaimsViewModelForGrid> GetByStatusNamePayeeUserIdForGrid(string StatusName, int CompanyId, string CreatedByUserId);
        IEnumerable<LCompanySpecificColumnViewModel> GetCompanySpecificLabels();
        LClaimViewModel GetById(int id);
        int Add(LClaimViewModel serverData, string WorkflowName, string AttachmentPath, string LoggedInRoleId,string RedirectToUrl);
        DataTable ValidateUploadClaims(List<LClaimViewModel> LClaimViewModel, string FileName, string LoggedInRoleId, string WorkflowName,string RedirectToUrl,int iCompanyId);
        void UploadClaims(string LoggedInRoleId, string WorkflowName, string RedirectToUrl, int iCompanyId);
        void Update(LClaimViewModel serverData, string AttachmentPath,string RedirectToUrl);
        void Delete(int id,string RedirectToUrl);
        string GetMyClaimsReport(string UserID, int CompanyID, string CompanyCode, string LoggedInUserName);
        string GetMyClaimsAuditData(string UserID, int CompanyID, string CompanyCode, string LoggedInUserName);
        string GetSudmitableorNot(int Transactionid, string Workflow, string Role, int CompanyID);

        IEnumerable<RChannelViewModel> GetPayeesChannels(int PayeeID, int CompanyId);
        IEnumerable<RChannelViewModel> GetByRChannelByTransactionID(int?TransactionID);
         IEnumerable<SelectedItemViewModel> GetNextUserDetails(string workflow, int CompanyID, string LoggedInUserId, string LoggedinRoleID);
    }
}