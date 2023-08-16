using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LCalcRestClient : ILCalcRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LCalcRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LCalcRowCountsViewModel> GetLCalcCounts(int SOSBatchNumber, int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/GetLCalcCounts?SOSBatchNumber={SOSBatchNumber}&UserName={UserName}&Workflow={Workflow}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCalcRowCountsViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<PayeeCalcChartViewModel> GetPayeeCalcForChart(int CompanyId, string PayeeUserId, int CommissionPeriodCount)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/GetPayeeCalculationsGraph?CompanyId={CompanyId}&PayeeUserId={PayeeUserId}&CommissionPeriodCount={CommissionPeriodCount}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("PayeeUserId", PayeeUserId, ParameterType.UrlSegment);
            request.AddParameter("CommissionPeriodCount", CommissionPeriodCount, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<PayeeCalcChartViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<PayeeCalcChartViewModel> GetPayeeCalcForChartByPayeeId(int CompanyId, int PayeeId, int CommissionPeriodCount)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/GetPayeeCalculationsGraphByPayeeId?CompanyId={CompanyId}&PayeeId={PayeeId}&CommissionPeriodCount={CommissionPeriodCount}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("PayeeId", PayeeId, ParameterType.UrlSegment);
            request.AddParameter("CommissionPeriodCount", CommissionPeriodCount, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<PayeeCalcChartViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<dynamic> GetLCalcForGrid(int SOSBatchNumber, int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery, int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/GetLCalcForGrid?SOSBatchNumber={SOSBatchNumber}&PageNumber={PageNumber}&PageSize={PageSize}&UserName={UserName}&Workflow={Workflow}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
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
        //
        public string DownloadLCalc(int SOSBatchNumber, string CompanyCode, string LoggedInUserName, int CompanyId, string FilterQuery)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/DownloadLCalc?SOSBatchNumber={SOSBatchNumber}&CompanyCode={CompanyCode}&UserName={UserName}&Workflow={Workflow}&CompanyId={CompanyId}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            //request.AddParameter("LoggedInUserName", LoggedInUserName, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute(request); //SG - 25Nov 2019 - WHy we are using List ofviewModel when we are just getting filename/ msg as string param.
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        public string DownloadLCalcData(int SOSBatchNumber, string CompanyCode, string LoggedInUserName, int CompanyId, string FilterQuery)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/DownloadLCalcData?SOSBatchNumber={SOSBatchNumber}&CompanyCode={CompanyCode}&UserName={UserName}&Workflow={Workflow}&CompanyId={CompanyId}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SOSBatchNumber", SOSBatchNumber, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            //request.AddParameter("LoggedInUserName", LoggedInUserName, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", FilterQuery, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCalcViewModel>>(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        public int GetLCalcForReportCounts(LCalcForReportsViewModel serverData, string CommissionPeriod)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/GetLCalForReportCounts?PageNumber={PageNumber}&PageSize={PageSize}&CompanyCode={CompanyCode}&PrimaryChannel={PrimaryChannel}&PayeeList={PayeeList}&MinIMEI={MinIMEI}&MaxIMEI={MaxIMEI}&MinBAN={MinBAN}&MaxBAN={MaxBAN}&MinContractDuration={MinContractDuration}&MaxContractDuration={MaxContractDuration}&MinCommissionAmount={MinCommissionAmount}&MaxCommissionAmount={MaxCommissionAmount}&MinOrderDate={MinOrderDate}&MaxOrderDate={MaxOrderDate}&MinConnectionDate={MinConnectionDate}&MaxConnectionDate={MaxConnectionDate}&MinTerminationDate={MinTerminationDate}&MaxTerminationDate={MaxTerminationDate}&UserName={UserName}&Workflow={Workflow}&MinSubscriberNumber={MinSubscriberNumber}&MaxSubscriberNumber={MaxSubscriberNumber}&ActivityType={ActivityType}&CommissionType={CommissionType}&ProductCode={ProductCode}&CommissionPeriod={CommissionPeriod}&Source={Source}", Method.GET) { RequestFormat = DataFormat.Json };
            //request.AddBody(serverData);
            var MinContractDuration = Convert.ToString(serverData.MinContractDuration);
            var MaxContractDuration = Convert.ToString(serverData.MaxContractDuration);
            var MinCommissionAmount = Convert.ToString(serverData.MinCommissionAmount);
            var MaxCommissionAmount = Convert.ToString(serverData.MaxCommissionAmount);

            request.AddParameter("CommissionPeriod", CommissionPeriod, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", serverData.CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", serverData.PageNumber, ParameterType.UrlSegment);
            request.AddParameter("PageSize", serverData.PageSize, ParameterType.UrlSegment);
            request.AddParameter("PrimaryChannel", (string.IsNullOrEmpty(serverData.PrimaryChannel)) ? "" : serverData.PrimaryChannel, ParameterType.UrlSegment);
            request.AddParameter("Source", (string.IsNullOrEmpty(serverData.Source)) ? "" : serverData.Source, ParameterType.UrlSegment);
            request.AddParameter("PayeeList", (string.IsNullOrEmpty(serverData.PayeeId)) ? "" : serverData.PayeeId, ParameterType.UrlSegment);
            request.AddParameter("MinBAN", (string.IsNullOrEmpty(serverData.MinBAN)) ? "" : serverData.MinBAN, ParameterType.UrlSegment);
            request.AddParameter("MaxBAN", (string.IsNullOrEmpty(serverData.MaxBAN)) ? "" : serverData.MaxBAN, ParameterType.UrlSegment);
            request.AddParameter("MinIMEI", (string.IsNullOrEmpty(serverData.MinIMEI)) ? "" : serverData.MinIMEI, ParameterType.UrlSegment);
            request.AddParameter("MaxIMEI", (string.IsNullOrEmpty(serverData.MaxIMEI)) ? "" : serverData.MaxIMEI, ParameterType.UrlSegment);
            request.AddParameter("MinContractDuration", (MinContractDuration == null) ? "" : MinContractDuration, ParameterType.UrlSegment);
            request.AddParameter("MaxContractDuration", (MaxContractDuration == null) ? "" : MaxContractDuration, ParameterType.UrlSegment);
            request.AddParameter("MinCommissionAmount", (MinCommissionAmount == null) ? "" : MinCommissionAmount, ParameterType.UrlSegment);
            request.AddParameter("MaxCommissionAmount", (MaxCommissionAmount == null) ? "" : MaxCommissionAmount, ParameterType.UrlSegment);
            request.AddParameter("MinSubscriberNumber", (serverData.MinSubscriberNumber == null) ? "" : serverData.MinSubscriberNumber, ParameterType.UrlSegment);
            request.AddParameter("MaxSubscriberNumber", (serverData.MaxSubscriberNumber == null) ? "" : serverData.MaxSubscriberNumber, ParameterType.UrlSegment);
            request.AddParameter("MinOrderDate", (serverData.FromOrderDate == null) ? string.Empty : serverData.FromOrderDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("MaxOrderDate", (serverData.ToOrderDate == null) ? string.Empty : serverData.ToOrderDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("MinConnectionDate", (serverData.FromConnectionDate == null) ? string.Empty : serverData.FromConnectionDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("MaxConnectionDate", (serverData.ToConnectionDate == null) ? string.Empty : serverData.ToConnectionDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("MinTerminationDate", (serverData.FromTerminationDate == null) ? string.Empty : serverData.FromTerminationDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("MaxTerminationDate", (serverData.ToTerminationDate == null) ? string.Empty : serverData.ToTerminationDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("ActivityType", (string.IsNullOrEmpty(serverData.ActivityType)) ? "" : serverData.ActivityType, ParameterType.UrlSegment);
            request.AddParameter("CommissionType", (string.IsNullOrEmpty(serverData.CommissionType)) ? "" : serverData.CommissionType, ParameterType.UrlSegment);
            request.AddParameter("ProductCode", (string.IsNullOrEmpty(serverData.ProductCode)) ? "" : serverData.ProductCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            return response.Data;
        }

        public IEnumerable<dynamic> GetLCalcForReports(string ProductCode, string CommissionType, string ActivityType, string FilterQuery, string Source, string PrimaryChannel, string PayeeList, string MinIMEI, string MaxIMEI, string MinBAN, string MaxBAN, string MinContractDuration, string MaxContractDuration, string MinCommissionAmount, string MaxCommissionAmount, string MinOrderDate, string MaxOrderDate, string MinConnectionDate, string MaxConnectionDate, string MinTerminationDate, string MaxTerminationDate, string sortdatafield, string sortorder, int pagesize, int pagenum, string CompanyCode, string MinSubscriberNumber, string MaxSubscriberNumber, string CommissionPeriod)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }

            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/GetLCalForReports?PageNumber={PageNumber}&PageSize={PageSize}&CompanyCode={CompanyCode}&PrimaryChannel={PrimaryChannel}&PayeeList={PayeeList}&MinIMEI={MinIMEI}&MaxIMEI={MaxIMEI}&MinBAN={MinBAN}&MaxBAN={MaxBAN}&MinContractDuration={MinContractDuration}&MaxContractDuration={MaxContractDuration}&MinCommissionAmount={MinCommissionAmount}&MaxCommissionAmount={MaxCommissionAmount}&MinOrderDate={MinOrderDate}&MaxOrderDate={MaxOrderDate}&MinConnectionDate={MinConnectionDate}&MaxConnectionDate={MaxConnectionDate}&MinTerminationDate={MinTerminationDate}&MaxTerminationDate={MaxTerminationDate}&UserName={UserName}&Workflow={Workflow}&MinSubscriberNumber={MinSubscriberNumber}&MaxSubscriberNumber={MaxSubscriberNumber}&ActivityType={ActivityType}&CommissionType={CommissionType}&ProductCode={ProductCode}&CommissionPeriod={CommissionPeriod}&Source={Source}", Method.GET) { RequestFormat = DataFormat.Json };
            //request.AddBody(serverData);
            request.AddParameter("CommissionPeriod", string.IsNullOrEmpty(CommissionPeriod) ? "" : CommissionPeriod, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", pagenum, ParameterType.UrlSegment);
            request.AddParameter("PageSize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("PrimaryChannel", (string.IsNullOrEmpty(PrimaryChannel)) ? "" : PrimaryChannel, ParameterType.UrlSegment);
            request.AddParameter("Source", (string.IsNullOrEmpty(Source)) ? "" : Source, ParameterType.UrlSegment);
            request.AddParameter("PayeeList", (string.IsNullOrEmpty(PayeeList)) ? "" : PayeeList, ParameterType.UrlSegment);
            request.AddParameter("MinBAN", (string.IsNullOrEmpty(MinBAN)) ? "" : MinBAN, ParameterType.UrlSegment);
            request.AddParameter("MaxBAN", (string.IsNullOrEmpty(MaxBAN)) ? "" : MaxBAN, ParameterType.UrlSegment);
            request.AddParameter("MinIMEI", (string.IsNullOrEmpty(MinIMEI)) ? "" : MinIMEI, ParameterType.UrlSegment);
            request.AddParameter("MaxIMEI", (string.IsNullOrEmpty(MaxIMEI)) ? "" : MaxIMEI, ParameterType.UrlSegment);
            request.AddParameter("MinContractDuration", (MinContractDuration == null) ? "" : MinContractDuration, ParameterType.UrlSegment);
            request.AddParameter("MaxContractDuration", (MaxContractDuration == null) ? "" : MaxContractDuration, ParameterType.UrlSegment);
            request.AddParameter("MinCommissionAmount", (MinCommissionAmount == null) ? "" : MinCommissionAmount, ParameterType.UrlSegment);
            request.AddParameter("MaxCommissionAmount", (MaxCommissionAmount == null) ? "" : MaxCommissionAmount, ParameterType.UrlSegment);
            request.AddParameter("MinSubscriberNumber", (MinSubscriberNumber == null) ? "" : MinSubscriberNumber, ParameterType.UrlSegment);
            request.AddParameter("MaxSubscriberNumber", (MaxSubscriberNumber == null) ? "" : MaxSubscriberNumber, ParameterType.UrlSegment);
            request.AddParameter("MinOrderDate", (string.IsNullOrEmpty(MinOrderDate)) ? string.Empty : MinOrderDate, ParameterType.UrlSegment);
            request.AddParameter("MaxOrderDate", (string.IsNullOrEmpty(MaxOrderDate)) ? string.Empty : MaxOrderDate, ParameterType.UrlSegment);
            request.AddParameter("MinConnectionDate", (string.IsNullOrEmpty(MinConnectionDate)) ? string.Empty : MinConnectionDate, ParameterType.UrlSegment);
            request.AddParameter("MaxConnectionDate", (string.IsNullOrEmpty(MaxConnectionDate)) ? string.Empty : MaxConnectionDate, ParameterType.UrlSegment);
            request.AddParameter("MinTerminationDate", (string.IsNullOrEmpty(MinTerminationDate)) ? string.Empty : MinTerminationDate, ParameterType.UrlSegment);
            request.AddParameter("MaxTerminationDate", (string.IsNullOrEmpty(MaxTerminationDate)) ? string.Empty : MaxTerminationDate, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("ActivityType", (string.IsNullOrEmpty(ActivityType)) ? "" : ActivityType, ParameterType.UrlSegment);
            request.AddParameter("CommissionType", (string.IsNullOrEmpty(CommissionType)) ? "" : CommissionType, ParameterType.UrlSegment);
            request.AddParameter("ProductCode", (string.IsNullOrEmpty(ProductCode)) ? "" : ProductCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<dynamic>>(request);

            return response.Data;
        }

        public string DownloadLCalcForReports(LCalcForReportsViewModel serverData, string LoggedInUserName, string CommissionPeriod)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var MinContractDuration = Convert.ToString(serverData.MinContractDuration);
            var MaxContractDuration = Convert.ToString(serverData.MaxContractDuration);
            var MinCommissionAmount = Convert.ToString(serverData.MinCommissionAmount);
            var MaxCommissionAmount = Convert.ToString(serverData.MaxCommissionAmount);
            var request = new RestRequest("api/LCalc/DownloadLCalForReports?PageNumber={PageNumber}&PageSize={PageSize}&CompanyCode={CompanyCode}&PrimaryChannel={PrimaryChannel}&PayeeList={PayeeList}&MinIMEI={MinIMEI}&MaxIMEI={MaxIMEI}&MinBAN={MinBAN}&MaxBAN={MaxBAN}&MinContractDuration={MinContractDuration}&MaxContractDuration={MaxContractDuration}&MinCommissionAmount={MinCommissionAmount}&MaxCommissionAmount={MaxCommissionAmount}&MinOrderDate={MinOrderDate}&MaxOrderDate={MaxOrderDate}&MinConnectionDate={MinConnectionDate}&MaxConnectionDate={MaxConnectionDate}&MinTerminationDate={MinTerminationDate}&MaxTerminationDate={MaxTerminationDate}&UserName={UserName}&Workflow={Workflow}&MinSubscriberNumber={MinSubscriberNumber}&MaxSubscriberNumber={MaxSubscriberNumber}&ActivityType={ActivityType}&CommissionType={CommissionType}&ProductCode={ProductCode}&CommissionPeriod={CommissionPeriod}&Source={Source}", Method.GET) { RequestFormat = DataFormat.Json };
            //request.AddBody(serverData);
            request.AddParameter("CommissionPeriod", CommissionPeriod, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", serverData.CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", serverData.PageNumber, ParameterType.UrlSegment);
            request.AddParameter("PageSize", serverData.PageSize, ParameterType.UrlSegment);
            request.AddParameter("PrimaryChannel", (string.IsNullOrEmpty(serverData.PrimaryChannel)) ? "" : serverData.PrimaryChannel, ParameterType.UrlSegment);
            request.AddParameter("Source", (string.IsNullOrEmpty(serverData.Source)) ? "" : serverData.Source, ParameterType.UrlSegment);
            request.AddParameter("PayeeList", (string.IsNullOrEmpty(serverData.PayeeId)) ? "" : serverData.PayeeId, ParameterType.UrlSegment);
            request.AddParameter("MinBAN", (string.IsNullOrEmpty(serverData.MinBAN)) ? "" : serverData.MinBAN, ParameterType.UrlSegment);
            request.AddParameter("MaxBAN", (string.IsNullOrEmpty(serverData.MaxBAN)) ? "" : serverData.MaxBAN, ParameterType.UrlSegment);
            request.AddParameter("MinIMEI", (string.IsNullOrEmpty(serverData.MinIMEI)) ? "" : serverData.MinIMEI, ParameterType.UrlSegment);
            request.AddParameter("MaxIMEI", (string.IsNullOrEmpty(serverData.MaxIMEI)) ? "" : serverData.MaxIMEI, ParameterType.UrlSegment);
            request.AddParameter("MinContractDuration", (MinContractDuration == null) ? "" : MinContractDuration, ParameterType.UrlSegment);
            request.AddParameter("MaxContractDuration", (MaxContractDuration == null) ? "" : MaxContractDuration, ParameterType.UrlSegment);
            request.AddParameter("MinCommissionAmount", (MinCommissionAmount == null) ? "" : MinCommissionAmount, ParameterType.UrlSegment);
            request.AddParameter("MaxCommissionAmount", (MaxCommissionAmount == null) ? "" : MaxCommissionAmount, ParameterType.UrlSegment);
            request.AddParameter("MinSubscriberNumber", (serverData.MinSubscriberNumber == null) ? "" : serverData.MinSubscriberNumber, ParameterType.UrlSegment);
            request.AddParameter("MaxSubscriberNumber", (serverData.MaxSubscriberNumber == null) ? "" : serverData.MaxSubscriberNumber, ParameterType.UrlSegment);
            request.AddParameter("MinOrderDate", (serverData.FromOrderDate == null) ? string.Empty : serverData.FromOrderDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("MaxOrderDate", (serverData.ToOrderDate == null) ? string.Empty : serverData.ToOrderDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("MinConnectionDate", (serverData.FromConnectionDate == null) ? string.Empty : serverData.FromConnectionDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("MaxConnectionDate", (serverData.ToConnectionDate == null) ? string.Empty : serverData.ToConnectionDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("MinTerminationDate", (serverData.FromTerminationDate == null) ? string.Empty : serverData.FromTerminationDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("MaxTerminationDate", (serverData.ToTerminationDate == null) ? string.Empty : serverData.ToTerminationDate.Value.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("ActivityType", (string.IsNullOrEmpty(serverData.ActivityType)) ? "" : serverData.ActivityType, ParameterType.UrlSegment);
            request.AddParameter("CommissionType", (string.IsNullOrEmpty(serverData.CommissionType)) ? "" : serverData.CommissionType, ParameterType.UrlSegment);
            request.AddParameter("ProductCode", (string.IsNullOrEmpty(serverData.ProductCode)) ? "" : serverData.ProductCode, ParameterType.UrlSegment);
            var response = _client.Execute(request);

            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }


        //public void UpdateStatus(int id,string Comments,string Status)
        //{
        //    var request = new RestRequest("api/LCalc/UpdateCalcStatus/{Id}?Comments={Comments}&Status={Status}", Method.POST) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("Id", id, ParameterType.UrlSegment);
        //    if (!string.IsNullOrEmpty(Comments))
        //    {
        //        request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
        //    }
        //    request.AddParameter("Status", Status, ParameterType.UrlSegment);
        //    var response = _client.Execute<LCalcViewModel>(request);
        //}


        //Payee calc counts
        public int GetCalcForPayeeCounts(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, string PortfolioList, int LoggedinLUserId, string LoggedInRoleName, string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/GetLCalcByPayeeCommissionPeriodCompanyIdCounts?CompanyId={CompanyId}&CommissionPeriodIdList={CommissionPeriodIdList}&Status={Status}&UserName={UserName}&Workflow={Workflow}&LoggedInRoleName={LoggedInRoleName}&LoggedinLUserId={LoggedinLUserId}&LoggedInUserId={LoggedInUserId}", Method.POST) { RequestFormat = DataFormat.Json };
            var PayeecalModel = new PayeeCalcViewModel { PortfolioList = PortfolioList, PayeeList = PayeeIdList };
            request.AddBody(PayeecalModel);
            // request.AddParameter("PayeeIdList", PayeeIdList, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("CommissionPeriodIdList", CommissionPeriodIdList, ParameterType.UrlSegment);
            request.AddParameter("Status", Status, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedinLUserId", LoggedinLUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleName", LoggedInRoleName, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }


        public IEnumerable<dynamic> GetCalcForPayeeReports(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, int PageSize, int PageNumber, string sortdatafield, string sortorder, string FilterQuery, string PortfolioList, string LoggedInRoleName, int LoggedinLUserId, string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/GetLCalcByPayeeCommissionPeriodCompanyId?&CompanyId={CompanyId}&CommissionPeriodIdList={CommissionPeriodIdList}&Status={Status}&PageSize={PageSize}&PageNumber={PageNumber}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}&UserName={UserName}&Workflow={Workflow}&LoggedinLUserId={LoggedinLUserId}&LoggedInRoleName={LoggedInRoleName}&LoggedInUserId={LoggedInUserId}", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddParameter("PayeeIdList", PayeeIdList, ParameterType.UrlSegment);
            var PayeecalModel = new PayeeCalcViewModel { PortfolioList = PortfolioList, PayeeList = PayeeIdList };
            request.AddBody(PayeecalModel);
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

        public string DownloadCalcForPayeeReports(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, string LoggedInUserName, string CompanyCode, string PortfolioList, int LoggedinLUserId, string LoggedInRoleName, string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/DownloadLCalcByPayeeCommissionPeriodCompanyId?LoggedinLUserId={LoggedinLUserId}&LoggedInRoleName={LoggedInRoleName}&CompanyCode={CompanyCode}&PayeeIdList={PayeeIdList}&CompanyId={CompanyId}&CommissionPeriodIdList={CommissionPeriodIdList}&Status={Status}&PortfolioList={PortfolioList}&UserName={UserName}&Workflow={Workflow}&LoggedInUserId={LoggedInUserId}", Method.GET) { RequestFormat = DataFormat.Json };
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
        public IEnumerable<dynamic> GetSummaryForPayee(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, bool BatchNo, bool Source, bool CommType, bool CommPeriodchecked, bool Payeechecked, string Status, string UserRole, string LoggedInUserId, bool MSISDN, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var BatchName = false;

            var request = new RestRequest("api/LCalc/GetLCalcSummary?CompanyId={CompanyId}&CommissionPeriodIdList={CommissionPeriodIdList}&BatchNo={BatchNo}&BatchName={BatchName}&Source={Source}&CommType={CommType}&CommPeriodchecked={CommPeriodchecked}&Payeechecked={Payeechecked}&Status={Status}&UserName={UserName}&Workflow={Workflow}&LoggedInUserId={LoggedInUserId}&UserRole={UserRole}&MSISDN={MSISDN}&PageNumber={PageNumber}&PageSize={PageSize}&sortdatafield={sortdatafield}&sortorder={sortorder}&FilterQuery={FilterQuery}", Method.POST) { RequestFormat = DataFormat.Json };
            // request.AddParameter("PayeeIdList", PayeeIdList, ParameterType.UrlSegment);
            var PayeecalModel = new PayeeCalcViewModel { PayeeList = PayeeIdList };
            request.AddBody(PayeecalModel);
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
            request.AddParameter("MSISDN", MSISDN, ParameterType.UrlSegment);
            request.AddParameter("PageNumber", pagenum, ParameterType.UrlSegment);
            request.AddParameter("PageSize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", "", ParameterType.UrlSegment);
            request.AddParameter("sortorder", "", ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", FilterQuery, ParameterType.UrlSegment);


            var response = _client.Execute<List<dynamic>>(request);

            return response.Data;
        }

        public int GetSummaryForPayeeCount(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, bool BatchNo, bool Source, bool CommType, bool CommPeriodchecked, bool Payeechecked, string Status, string UserRole, string LoggedInUserId, bool MSISDN)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var BatchName = false;

            var request = new RestRequest("api/LCalc/GetLCalcSummaryCount?CompanyId={CompanyId}&CommissionPeriodIdList={CommissionPeriodIdList}&BatchNo={BatchNo}&BatchName={BatchName}&Source={Source}&CommType={CommType}&CommPeriodchecked={CommPeriodchecked}&Payeechecked={Payeechecked}&Status={Status}&UserName={UserName}&Workflow={Workflow}&LoggedInUserId={LoggedInUserId}&UserRole={UserRole}&MSISDN={MSISDN}", Method.POST) { RequestFormat = DataFormat.Json };
            // request.AddParameter("PayeeIdList", PayeeIdList, ParameterType.UrlSegment);
            var PayeecalModel = new PayeeCalcViewModel { PayeeList = PayeeIdList };
            request.AddBody(PayeecalModel);
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
            request.AddParameter("MSISDN", MSISDN, ParameterType.UrlSegment);


            //dynamic response = _client.Execute<dynamic>(request);

            // return response;

            var response = _client.Execute<int>(request);
            return response.Data;
        }

        public void UpdateAcceptAttachment(string SelectedData, string AcceptedBy, DateTime AcceptedAt, string RedirectToUrl, int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;

            var request = new RestRequest("api/LCalc/UpdateAcceptance?SelectedData={SelectedData}&AcceptedBy={AcceptedBy}&AcceptedAt={AcceptedAt}&UserName={UserName}&Workflow={Workflow}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SelectedData", SelectedData, ParameterType.UrlSegment);
            request.AddParameter("AcceptedBy", AcceptedBy, ParameterType.UrlSegment);
            request.AddParameter("AcceptedAt", AcceptedAt, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<LCalcViewModel>(request);
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

        public void UpdateAcceptStatusForAll(int CompanyId, string PayeeIdList, string CommissionPeriodIdList, string PortfolioList, string AcceptedBy, DateTime AcceptedAt, string RedirectToUrl, int LoggedinLUserId, string LoggedInRoleName, string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/UpdatePayeeAcceptanceStatusForAll?CommissionPeriodIdList={CommissionPeriodIdList}&AcceptedBy={AcceptedBy}&AcceptedAt={AcceptedAt}&UserName={UserName}&Workflow={Workflow}&CompanyId={CompanyId}&LoggedinLUserId={LoggedinLUserId}&LoggedInRoleName={LoggedInRoleName}&LoggedInUserId={LoggedInUserId}", Method.POST) { RequestFormat = DataFormat.Json };
            var PayeecalModel = new PayeeCalcViewModel { PortfolioList = PortfolioList, PayeeList = PayeeIdList };
            request.AddBody(PayeecalModel);
            request.AddParameter("CommissionPeriodIdList", CommissionPeriodIdList, ParameterType.UrlSegment);
            //  request.AddParameter("PayeeIdList", PayeeIdList, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("AcceptedBy", AcceptedBy, ParameterType.UrlSegment);
            request.AddParameter("AcceptedAt", AcceptedAt, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("LoggedinLUserId", LoggedinLUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleName", LoggedInRoleName, ParameterType.UrlSegment);
            var response = _client.Execute<LCalcViewModel>(request);
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

        public void UpdatePayeeAttachment(string SelectedData, int AttachmentId, string RedirectToUrl, string LoggedInUserId, int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/UpdatePayeeAttachment?SelectedData={SelectedData}&AttachmentId={AttachmentId}&UserName={UserName}&Workflow={Workflow}&LoggedInUserId={LoggedInUserId}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SelectedData", SelectedData, ParameterType.UrlSegment);
            request.AddParameter("AttachmentId", AttachmentId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<LCalcViewModel>(request);
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

        public void UpdatePayeeAttachmentForAll(string PayeeIdList, string CommissionPeriodIdList, string PortfolioList, int AttachmentId, string RedirectToUrl, string LoggedInUserId, int CompanyId, int LoggedinLUserId, string LoggedInRoleName)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/UpdatePayeeAttachmentForAll?CommissionPeriodIdList={CommissionPeriodIdList}&AttachmentId={AttachmentId}&UserName={UserName}&Workflow={Workflow}&LoggedInUserId={LoggedInUserId}&CompanyId={CompanyId}&LoggedinLUserId={LoggedinLUserId}&LoggedInRoleName={LoggedInRoleName}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddParameter("CommissionPeriodIdList", CommissionPeriodIdList, ParameterType.UrlSegment);
            //request.AddParameter("PayeeIdList", PayeeIdList, ParameterType.UrlSegment);
            //request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            var PayeecalModel = new PayeeCalcViewModel { PortfolioList = PortfolioList, PayeeList = PayeeIdList };
            request.AddBody(PayeecalModel);
            request.AddParameter("AttachmentId", AttachmentId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("LoggedinLUserId", LoggedinLUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleName", LoggedInRoleName, ParameterType.UrlSegment);
            var response = _client.Execute<LCalcViewModel>(request);
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

        //This method will update the comments column for the selected Payee Records
        public void UpdateComments(string SelectedData, string Comments, string RedirectToUrl, string LoggedInUserId, int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LCalc/UpdateComments?SelectedData={SelectedData}&Comments={Comments}&UserName={UserName}&Workflow={Workflow}&LoggedInUserId={LoggedInUserId}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("SelectedData", SelectedData, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("Comments", Comments, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<LCalcViewModel>(request);
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

        public string CheckCompanySpecificMappedorNot(int CompanyId, string Type)
        {

            var request = new RestRequest("api/LCalc/CheckCompanySpecificMappedorNot?Type={Type}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Type", Type, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCalcViewModel>>(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }


    }
    interface ILCalcRestClient
    {
        IEnumerable<LCalcRowCountsViewModel> GetLCalcCounts(int SOSBatchNumber, int CompanyId);
        IEnumerable<PayeeCalcChartViewModel> GetPayeeCalcForChart(int CompanyId, string PayeeUserId, int CommissionPeriodCount);
        IEnumerable<PayeeCalcChartViewModel> GetPayeeCalcForChartByPayeeId(int CompanyId, int PayeeId, int CommissionPeriodCount);
        IEnumerable<dynamic> GetLCalcForGrid(int SOSBatchNumber, int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery, int CompanyId);
        string DownloadLCalc(int SOSBatchNumber, string CompanyCode, string LoggedInUserName, int CompanyId, string filterQuery);
        string DownloadLCalcData(int SOSBatchNumber, string CompanyCode, string LoggedInUserName, int CompanyId, string filterQuery);
        int GetLCalcForReportCounts(LCalcForReportsViewModel serverData, string CommissionPeriod);
        IEnumerable<dynamic> GetLCalcForReports(string ProductCode, string CommissionType, string ActivityType, string FilterQuery, string Source, string PrimaryChannel, string PayeeList, string MinIMEI, string MaxIMEI, string MinBAN, string MaxBAN, string MinContractDuration, string MaxContractDuration, string MinCommissionAmount, string MaxCommissionAmount, string MinOrderDate, string MaxOrderDate, string MinConnectionDate, string MaxConnectionDate, string MinTerminationDate, string MaxTerminationDate, string sortdatafield, string sortorder, int pagesize, int pagenum, string CompanyCode, string MinSubscriberNumber, string MaxSubscriberNumber, string CommissionPeriod);
        string DownloadLCalcForReports(LCalcForReportsViewModel serverData, string LoggedInUserName, string CommissionPeriod);
        int GetCalcForPayeeCounts(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, string PortfolioList, int LoggedinLUserId, string LoggedInRoleName, string LoggedInUserId);
        IEnumerable<dynamic> GetCalcForPayeeReports(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, int PageSize, int PageNumber, string sortdatafield, string sortorder, string FilterQuery, string PortfolioList, string LoggedInRoleName, int LoggedinLUserId, string LoggedInUserId);
        string DownloadCalcForPayeeReports(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, string Status, string LoggedInUserName, string CompanyCode, string PortfolioList, int LoggedinLUserId, string LoggedInRoleName, string LoggedInUserId);
        IEnumerable<dynamic> GetSummaryForPayee(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, bool BatchNo, bool Source, bool CommType, bool CommPeriodchecked, bool Payeechecked, string Status, string UserRole, string LoggedInUserId, bool MSISDN, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery);
        int GetSummaryForPayeeCount(string PayeeIdList, int CompanyId, string CommissionPeriodIdList, bool BatchNo, bool Source, bool CommType, bool CommPeriodchecked, bool Payeechecked, string Status, string UserRole, string LoggedInUserId, bool MSISDN);
        void UpdateAcceptAttachment(string SelectedData, string AcceptedBy, DateTime AcceptedAt, string RedirectToUrl, int CompanyId);
        void UpdateComments(string SelectedData, string Comments, string RedirectToUrl, string LoggedInUserId, int CompanyId);
        void UpdatePayeeAttachmentForAll(string PayeeIdList, string CommissionPeriodIdList, string PortfolioList, int AttachmentId, string RedirectToUrl, string LoggedInUserId, int CompanyId, int LoggedinLUserId, string LoggedInRoleName);
        void UpdatePayeeAttachment(string SelectedData, int AttachmentId, string RedirectToUrl, string LoggedInUserId, int CompanyId);
        void UpdateAcceptStatusForAll(int CompanyId, string PayeeIdList, string CommissionPeriodIdList, string PortfolioList, string AcceptedBy, DateTime AcceptedAt, string RedirectToUrl, int LoggedinLUserId, string LoggedInRoleName, string LoggedInUserId);
        string CheckCompanySpecificMappedorNot(int CompanyId, string Type);
    }
}