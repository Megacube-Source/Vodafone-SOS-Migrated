using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using Vodafone_SOS_WebApp.Models;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LPayeesRestClient : ILPayeesRestClient
    {
        private readonly RestClient _client;
        private readonly string _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public LPayeesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public string ValidatePayeeParent(string  PayeeCode,string ParentCode,string PrimaryChannel,DateTime EffectiveDate)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/GetParentPayeeValidationResult?PayeeCode={PayeeCode}&ParentCode={ParentCode}&EffectiveDate={EffectiveDate}&PrimaryChannel={PrimaryChannel}&CompanyId= {CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PayeeCode", PayeeCode, ParameterType.UrlSegment);
            request.AddParameter("ParentCode", ParentCode, ParameterType.UrlSegment);
            request.AddParameter("PrimaryChannel", PrimaryChannel, ParameterType.UrlSegment);
            request.AddParameter("EffectiveDate", EffectiveDate.ToString("yyyy-MM-dd"), ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response.Content == null)
                return null;
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        
        public string MyPayeeReport(int CompanyId, string LoggedInUserId, string UserRole, string PortfolioList, string LoggedInUserName,bool DownloadReportData)
        {
            var request = new RestRequest("api/LPayees/DownloadPayeeReport?CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}&UserRole={UserRole}&PortfolioList={PortfolioList}&LoggedInUserName={LoggedInUserName}&DownloadReportData={DownloadReportData}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("UserRole", UserRole, ParameterType.UrlSegment);
            request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserName", LoggedInUserName, ParameterType.UrlSegment);
            request.AddParameter("DownloadReportData", DownloadReportData, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            string source = response.Content;
            dynamic data = JsonConvert.DeserializeObject(source);
            return data;
        }

        //Code reviewed but not commenting the below method as it might be used in future
        public bool CanRaiseClaims(string PayeeUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/CanRaiseClaims?PayeeUserId={PayeeUserId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PayeeUserId", PayeeUserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<bool>(request);
            return response.Data;
        }

        //public IEnumerable<LPayeeViewModel> GetAll()
        //{
        //    var request = new RestRequest("api/LPayees/GetLPayees", Method.GET) { RequestFormat = DataFormat.Json };

        //    var response = _client.Execute<List<LPayeeViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}
        public IEnumerable<LCompanySpecificColumnViewModel> GetLPayeesColumnsForGrid()
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/GetLPayeesColumnsForGrid?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LCompanySpecificColumnViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<LPayeeViewModel> GetApprovedPayeeTree(string AsOfDate, int CompanyId, string PrimaryChannel)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/GetApprovedPayeeForTree?PayeeAsOfDate={PayeeAsOfDate}&LpCompanyId={LpCompanyId}&PrimaryChannel={PrimaryChannel}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PayeeAsOfDate", AsOfDate, ParameterType.UrlSegment);
            request.AddParameter("LpCompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("PrimaryChannel", PrimaryChannel, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayeeViewModel>>(request);
            
            return response.Data;
        }

        public IEnumerable<LPayeeViewModel> GetApprovedPayeePortfolioTree(string AsOfDate, int CompanyId, string LoggedInUserId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/GetApprovedPayeeForPortfolioTree?PayeeAsOfDate={PayeeAsOfDate}&LpCompanyId={LpCompanyId}&LoggedInUserId={LoggedInUserId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PayeeAsOfDate", AsOfDate, ParameterType.UrlSegment);
            request.AddParameter("LpCompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayeeViewModel>>(request);

            return response.Data;
        }

        public IEnumerable<LPayeeViewModel> GetParentDropDown(int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/GetParentDropDown?CompanyId={CompanyId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayeeViewModel>>(request);

            return response.Data;
        }

        public int GetParentPayeeGridCounts(int CompanyId)
        {
            var request = new RestRequest("api/LPayees/GetParentPayeeGridCounts?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            int counts = (int)JsonConvert.DeserializeObject(response.Content, (typeof(int)));
            return counts;
        }

        public IEnumerable<LPayeeViewModel> GetParentPayeeData(int CompanyId ,string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, int? ParentPayeeId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/GetParentPayeeGridData?CompanyId={CompanyId}&sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&ParentPayeeId={ParentPayeeId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield)?string.Empty:sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder)?string.Empty:sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery)?string.Empty:FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("ParentPayeeId", ParentPayeeId.HasValue ? (object)ParentPayeeId : string.Empty, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayeeViewModel>>(request);

           // DataTable dt = (DataTable)JsonConvert.DeserializeObject(response.Content, (typeof(DataTable)));
            return response.Data;
        }

        public IEnumerable<LPayeeViewModel> GetActivePayee(int CompanyId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/GetActivePayeeByCompanyId?CompanyId={CompanyId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayeeViewModel>>(request);
            return response.Data;
        }

        public dynamic GetPayeeHierarchy(int CompanyId, string PayeeUserId,bool IsDataToBeDisplayedInReport)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
           
            var request = new RestRequest("api/LPayees/GetPayeeHierarchy?CompanyId={CompanyId}&PayeeUserId={PayeeUserId}&UserName={UserName}&Workflow={Workflow}&IsDataToBeDisplayedInReport={IsDataToBeDisplayedInReport}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("PayeeUserId", PayeeUserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("IsDataToBeDisplayedInReport", IsDataToBeDisplayedInReport, ParameterType.UrlSegment);
            if (IsDataToBeDisplayedInReport)
            {
                var response = _client.Execute<DataTable>(request);
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(response.Content, (typeof(DataTable)));
                return dt;
            }
            else
            {
                var response = _client.Execute<List<LPayeeViewModel>>(request);
                List<LPayeeViewModel> data = (List<LPayeeViewModel>)JsonConvert.DeserializeObject(response.Content, (typeof(List<LPayeeViewModel>)));
                return data;
            }
        }

        public List<string> GetParentListByPayeeId(string PayeeIdList)
        {
            var request = new RestRequest("api/LPayees/GetParentListByPayeeId?PayeeIdList={PayeeIdList}", Method.POST) { RequestFormat = DataFormat.Json };
            // request.AddParameter("PayeeIdList", PayeeIdList, ParameterType.UrlSegment);
            request.AddBody(new PayeeIdListViewModel{ PayeeIdList=PayeeIdList});
            var response = _client.Execute<List<string>>(request);

            return response.Data;
        }

        public IEnumerable<LPayeeViewModel> GetPayeeHierarchyByPayeeId(int CompanyId, string PayeeId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/LPayees/GetPayeeHierarchyByPayeeId?CompanyId={CompanyId}&PayeeId={PayeeId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("PayeeId", PayeeId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayeeViewModel>>(request);

            return response.Data;
        }
        //int CompanyId, string LoggedInUserId, string UserName, string Workflow, string PortfolioList,string sortdatafield,string sortorder,int pagesize,int pagenum,string FilterQuery,string UserRole
        public IEnumerable<LPayeeViewModel> GetPayeeForClaimsDropdown(int CompanyId, string LoggedInUserId,string PortfolioList, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, string UserRole,string PayeeId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
                                         //api/LPayees/GetApprovedPayeeForClaimsDropdown?CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}&UserName={UserName}&Workflow={Workflow}&PortfolioList={PortfolioList}&sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&UserRole={UserRole}
            var request = new RestRequest("api/LPayees/GetApprovedPayeeForClaimsDropdown?CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}&UserName={UserName}&Workflow={Workflow}&PortfolioList={PortfolioList}&sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&UserRole={UserRole}&PayeeId={PayeeId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield)?string.Empty:sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("PayeeId", string.IsNullOrEmpty(PayeeId) ? string.Empty : PayeeId, ParameterType.UrlSegment);
            request.AddParameter("UserRole", UserRole, ParameterType.UrlSegment);
            //if (IsDataToBeDisplayedInReport)
            //{
            //    var response = _client.Execute<DataTable>(request);
            //    DataTable dt = (DataTable)JsonConvert.DeserializeObject(response.Content, (typeof(DataTable)));
            //    return dt;
            //}
            //else
            //{
            var response = _client.Execute<List<LPayeeViewModel>>(request);
                List<LPayeeViewModel> data = (List<LPayeeViewModel>)JsonConvert.DeserializeObject(response.Content, (typeof(List<LPayeeViewModel>)));
                return data;
            //}
        }

        public int GetPayeeCountsForPortfolioMatching(int CompanyId, string LoggedInUserId, string PortfolioList, string UserRole)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/LPayees/GetApprovedPayeeCountsForClaimsDropdown?CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}&UserName={UserName}&Workflow={Workflow}&PortfolioList={PortfolioList}&UserRole={UserRole}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("UserRole", UserRole, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            if (response != null)
            {
                int data = (int)JsonConvert.DeserializeObject(response.Content, (typeof(int)));
                return data;
            }
            else
            {
                return 0;
            }
        }

        //public LPayeeViewModel GetPayeeByPayeeCode(int CompanyId,string PayeeCode)
        //{
        //    var request = new RestRequest("api/LPayees/GetPayeeDetailsByPayeeCode?PayeeCode={PayeeCode}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("PayeeCode", PayeeCode, ParameterType.UrlSegment);
        //    var response = _client.Execute<LPayeeViewModel>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public void UpdatePayeeStatus(Sp_UpdateItemStatusViewModel serverData)
        //{
        //    var request = new RestRequest("api/LPayees/UpdatePayeeStatus", Method.POST) { RequestFormat = DataFormat.Json };
        //    request.AddBody(serverData);

        //    var response = _client.Execute<LPayeeViewModel>(request);

        //    if (response.StatusCode == HttpStatusCode.InternalServerError)
        //    {
        //        var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
        //        ex.Data.Add("ErrorCode", response.StatusCode);
        //        string source = response.Content;
        //        dynamic data = JsonConvert.DeserializeObject(source);
        //        string xx = data.Message;
        //        ex.Data.Add("ErrorMessage", xx);
        //        throw ex;
        //    }
        //    if (response.StatusCode == HttpStatusCode.BadRequest)
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

        //public IEnumerable<LPayeeViewModel> GetByStatusName(string StatusName,int CompanyId,string ReportsToId)
        //{
        //    var request = new RestRequest("api/LPayees/GetLPayeesByStatusNameCompanyId?StatusName={StatusName}&CompanyId={CompanyId}&ReportsToId={ReportsToId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
        //    request.AddParameter("CompanyId",CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("ReportsToId", ReportsToId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LPayeeViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public IEnumerable<DownloadPayeeGridByTabNameViewModel> DownloadPayeeGridByStatusNameCreatedByUserId(string StatusName, int CompanyId, string CreatedByUserId)
        //{
        //    var request = new RestRequest("api/LPayees/GetDownloadPayeeGridByStatusNameCreatedByUserId?CompanyId={CompanyId}&StatusName={StatusName}&CreatedByUserId={CreatedByUserId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("CreatedByUserId", CreatedByUserId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<DownloadPayeeGridByTabNameViewModel>>(request);
        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public IEnumerable<DownloadPayeeGridByTabNameViewModel> DownloadPayeeGridByStatusNameReportsToId(string StatusName, int CompanyId, string ReportsToId)
        //{
        //    var request = new RestRequest("api/LPayees/GetDownloadPayeeGridByStatusNameReportsToId?CompanyId={CompanyId}&StatusName={StatusName}&ReportsToId={ReportsToId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<DownloadPayeeGridByTabNameViewModel>>(request);
        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public IEnumerable<LPayeeViewModel> GetByStatusNamePrimaryChannel(string StatusName, int CompanyId,string PrimaryChannel)
        //{
        //    var request = new RestRequest("api/LPayees/GetLPayeesByStatusNameCompanyIdPrimaryChannel?StatusName={StatusName}&CompanyId={CompanyId}&PrimaryChannel={PrimaryChannel}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("PrimaryChannel", PrimaryChannel, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LPayeeViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public IEnumerable<LPayeeViewModel> GetByStatusNameCreatedByUserId(string StatusName, int CompanyId,string CreatedByUserId)
        //{
        //    var request = new RestRequest("api/LPayees/GetLPayeesByStatusNameCompanyIdCreatedById?StatusName={StatusName}&CompanyId={CompanyId}&CreatedByUserId={CreatedByUserId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("StatusName", StatusName, ParameterType.UrlSegment);
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    request.AddParameter("CreatedByUserId",CreatedByUserId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LPayeeViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        //public LPayeeViewModel GetFileNameByPayeeId(int PayeeId)
        //{
        //    var request = new RestRequest("api/LPayees/GetPayeeFilesByPayeeCode?PayeeId={PayeeId}", Method.GET) { RequestFormat = DataFormat.Json };

        //    request.AddParameter("PayeeId", PayeeId, ParameterType.UrlSegment);
        //    var response = _client.Execute<LPayeeViewModel>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;

        //}

        public LPayeeViewModel GetById(int id)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/LPayees/GetLPayee/{id}?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayeeViewModel>>(request);
            var PayeeData= response.Data;
            if (PayeeData == null||PayeeData.Count==0)
                return new LPayeeViewModel();
            return PayeeData[0];

        }
        public LPayeeViewModel GetPayeeDetailsById(int id)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/LPayees/GetLPayeeById/{id}?UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LPayeeViewModel>(request);

            return response.Data;

        }
        //CheckIsWIAMEnabled(int CompanyId)
        public string CheckIsWIAMEnabled()
        {
            int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            var request = new RestRequest("api/LPayees/CheckIsWIAMEnabled?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            var res = response.Data.ToString();
            return res;
        }

        public int GetPayeeDetailsByPayeeCodeId(string PayeeCode)
        {
            int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

            var request = new RestRequest("api/LPayees/GetLPayeeByPayeeCode?PayeeCode={PayeeCode}&CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("PayeeCode", PayeeCode, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);

            return response.Data;

        }
        
        public LPayeeViewModel GetByPayeeUserId(string PayeeUserId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/LPayees/GetPayeeInformationByUserId?PayeeUserId={PayeeUserId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("PayeeUserId", PayeeUserId, ParameterType.UrlSegment);
            var response = _client.Execute<LPayeeViewModel>(request);

            return response.Data;

        }

        //Method returns the list of Payee list where LoginId is ChannelManager
        public dynamic GetPayeeByChannelManagerUserID(int CompanyId, string LoggedInUserId,bool IsDataToBeDisplayedInReport)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/LPayees/GetPayeeInformationByChannelManagerUserId?CompanyId={CompanyId}&LoggedInUserId={LoggedInUserId}&UserName={UserName}&Workflow={Workflow}&IsDataToBeDisplayedInReport={IsDataToBeDisplayedInReport}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("IsDataToBeDisplayedInReport", IsDataToBeDisplayedInReport, ParameterType.UrlSegment);
            if (IsDataToBeDisplayedInReport)
            {
                var response = _client.Execute<DataTable>(request);
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(response.Content, (typeof(DataTable)));
                return dt;
            }
            else
            {
                var response = _client.Execute<List<LPayeeViewModel>>(request);
                List<LPayeeViewModel> data = (List<LPayeeViewModel>)JsonConvert.DeserializeObject(response.Content, (typeof(List<LPayeeViewModel>)));
                return data;
            }

        }

        public List<LPayeeViewModel> AddUploadedPayees(List<LPayeeViewModel> serverData, List<mEntiryPortfolioViewModel> portFolioList, List<LChangeRequestViewModel> ChangeRequests, string LoggedInRoleId, string WorkflowName, string FileName,string RedirectToUrl)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            PayeeChangeRequestPortfolioViewModel PCP = new PayeeChangeRequestPortfolioViewModel();
            PCP.PayeeModels = serverData;
            PCP.Portfolios = portFolioList;
            PCP.ChangeRequests = ChangeRequests;
            var request = new RestRequest("api/LPayees/PostUploadLPayee?LoggedInRoleId={LoggedInRoleId}&FileName={FileName}&UserName={UserName}&Workflow={Workflow}", Method.POST) { RequestFormat = DataFormat.Json };
            //request.AddBody(serverData);
            //request.AddBody(portFolioList);
            request.AddBody(PCP);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("FileName", FileName, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayeeViewModel>>(request);

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

        public int Add(LPayeeViewModel serverData, string LoggedInRoleId, string WorkflowName, string AttachmentPath, string PortfolioList,string RedirectToUrl,string Source,string UserLobbyId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            serverData.ParameterCarrier = PortfolioList;
            var request = new RestRequest("api/LPayees/PostLPayee?LoggedInRoleId={LoggedInRoleId}&AttachmentPath={AttachmentPath}&UserName={UserName}&Workflow={Workflow}&Source={Source}&UserLobbyId={UserLobbyId}", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("AttachmentPath", string.IsNullOrEmpty(AttachmentPath) ? "" : AttachmentPath, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("Source", Source, ParameterType.UrlSegment);
            request.AddParameter("UserLobbyId", UserLobbyId, ParameterType.UrlSegment);
            // request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
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
        public void Update(LPayeeViewModel serverData, string AttachedFiles, string AttachmentPath, string PortfolioList, string LoggedInUserId,string RedirectToUrl)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            //var request = new RestRequest("api/LPayees/PutLPayee/{id}?AttachedFiles={AttachedFiles}&AttachmentPath={AttachmentPath}&PortfolioList={PortfolioList}&LoggedInUserId={LoggedInUserId}&UserName={UserName}&Workflow={Workflow}&LppParentPayeeId={LppParentPayeeId}", Method.PUT) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/LPayees/PutLPayee/{id}?AttachedFiles={AttachedFiles}&AttachmentPath={AttachmentPath}&LoggedInUserId={LoggedInUserId}&UserName={UserName}&Workflow={Workflow}&LppParentPayeeId={LppParentPayeeId}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddParameter("AttachedFiles", string.IsNullOrEmpty(AttachedFiles) ? "" : AttachedFiles, ParameterType.UrlSegment);
            request.AddParameter("AttachmentPath", string.IsNullOrEmpty(AttachmentPath) ? "" : AttachmentPath, ParameterType.UrlSegment);
            //request.AddParameter("PortfolioList", string.IsNullOrEmpty(PortfolioList) ? "" : PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", string.IsNullOrEmpty(LoggedInUserId) ? "" : LoggedInUserId, ParameterType.UrlSegment);
            request.AddParameter("LppParentPayeeId", serverData.LppParentPayeeId.HasValue ? serverData.LppParentPayeeId.Value : 0, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LPayeeViewModel>(request);

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
        //public void Delete(int id)
        //{
        //    var request = new RestRequest("api/LPayees/DeleteLPayee/{id}", Method.DELETE);
        //    request.AddParameter("id", id, ParameterType.UrlSegment);

        //    var response = _client.Execute<LPayeeViewModel>(request);

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

        public bool CheckEmailExists(string strEmailID)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/LPayees/CheckEmailExists?strEmailID={strEmailID}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("strEmailID", strEmailID, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<bool>(request);
            return response.Data;
        }
        public IEnumerable<LPayeeViewModel> GetAllPayeeCodesAndEmailID(int CompanyId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/LPayees/GetAllPayeeCodesAndEmailID?CompanyId={CompanyId}&UserName={UserName}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayeeViewModel>>(request);
           
            return response.Data;
        }
        public string GetPayeeUploadHelp()        /*added for payeeuploadhelp*/
        {
            var request = new RestRequest("api/LPayees/GetPayeeUploadHelp", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _client.Execute<LPayeeViewModel>(request);
            if (response.Data != null)
            return response.Data.GkvValue;
            return null;
        }

        public List<LPayeeViewModel> GetParentsByPayeeId(int PayeeId)
        {
            var request = new RestRequest("api/LPayees/GetParentByPayeeId?PayeeId={PayeeId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PayeeId", PayeeId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPayeeViewModel>>(request);
            string source = response.Content;
            if (source == null)
                return null;
           // dynamic data = JsonConvert.DeserializeObject<LPayeeViewModel>(source);
            return response.Data;
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

        public DataTable UploadPayee(string FileName, string LoggedInRoleId, int iCompanyId, string UpdatedBy, string RedirectToUrl)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            string WorkflowName = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(WorkflowName))
            {
                WorkflowName = "No Workflow";
            }
            var request = new RestRequest("api/LPayees/UploadPayee?FileName={FileName}&UserName={UserName}&LoggedInRoleId={LoggedInRoleId}&iCompanyId={iCompanyId}&WorkflowName={WorkflowName}&UpdatedBy={UpdatedBy}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("FileName", FileName, ParameterType.UrlSegment);
            request.AddParameter("UserName", UserName, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("iCompanyId", iCompanyId, ParameterType.UrlSegment);
            request.AddParameter("WorkflowName", WorkflowName, ParameterType.UrlSegment);
            request.AddParameter("UpdatedBy", UpdatedBy, ParameterType.UrlSegment);
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

        public void UploadValidatedPayeeBatch(int CompanyId, int BatchNumber, string AspNetUserId, int LoggedinRoleId)
        {
            string Workflow = System.Web.HttpContext.Current.Session["Workflow"] as string;
            if (string.IsNullOrEmpty(Workflow))
            {
                Workflow = "No Workflow";
            }
            var request = new RestRequest("api/LPayees/UploadValidatedPayeeBatch?CompanyId={CompanyId}&BatchNumber={BatchNumber}&AspNetUserId={AspNetUserId}&LoggedinRoleId={LoggedinRoleId}&Workflow={Workflow}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            request.AddParameter("AspNetUserId", AspNetUserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedinRoleId", LoggedinRoleId, ParameterType.UrlSegment);
            request.AddParameter("Workflow", Workflow, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            return;
        }
        public string DownloadPayeeUploadErrors(int CompanyId, int BatchNumber)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/DownloadPayeeUploadErrors?CompanyId={CompanyId}&BatchNumber={BatchNumber}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            var res = response.Data.ToString();
            return res;
        }
        public IEnumerable<PayeeUploadViewModelForReviewGrid> GetGridDataFields(int CompanyId)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/GetGridDataFields?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List< PayeeUploadViewModelForReviewGrid>>(request);
            //var res = response.Data.ToString();
            return response.Data;
        }


        public string UploadValidatedPayees(int CompanyId, int BatchNumber)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/UploadValidatedPayees?CompanyId={CompanyId}&BatchNumber={BatchNumber}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            var res = response.Data.ToString();
            return res;
        }
        public int GetXUploadPayeeCountByBatchNumber(int CompanyId, int BatchNumber)
        {
            var request = new RestRequest("api/LPayees/GetXUploadPayeeCountByBatchNumber?CompanyId={CompanyId}&BatchNumber={BatchNumber}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            return response.Data;
        }
        public dynamic GetXUploadPayeeByBatchNumber(int CompanyId, int BatchNumber, string sortdatafield, string sortorder, int? pagesize, int? pagenum, string FilterQuery)
        {
            string UserName = System.Web.HttpContext.Current.Session["UserName"] as string;
            var request = new RestRequest("api/LPayees/GetXUploadPayeeByBatchNumber?CompanyId={CompanyId}&BatchNumber={BatchNumber}&sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? "" : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? "" : sortorder, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? "" : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
            return response.Data;
        }
        public void DeletePayeeUploadBatch(int Id)
        {
            var request = new RestRequest("api/LPayees/DeletePayeeUploadBatch?Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute(request);
            return;
        }

    }

    interface ILPayeesRestClient
    {
        IEnumerable<PayeeUploadViewModelForReviewGrid> GetGridDataFields(int CompanyId);
        void DeletePayeeUploadBatch(int Id);
        void UploadValidatedPayeeBatch(int CompanyId, int BatchNumber, string AspNetUserId, int LoggedinRoleId);
        int GetXUploadPayeeCountByBatchNumber(int CompanyId, int BatchNumber);
        dynamic GetXUploadPayeeByBatchNumber(int CompanyId, int BatchNumber, string sortdatafield, string sortorder, int? pagesize, int? pagenum, string FilterQuery);
        string DownloadPayeeUploadErrors(int CompanyId, int BatchNumber);
        DataTable UploadPayee(string FileName, string LoggedInRoleId, int iCompanyId, string UpdatedBy, string RedirectToUrl);

        List<LPayeeViewModel> GetParentsByPayeeId(int PayeeId);
        bool CanRaiseClaims(string PayeeUserId);
        dynamic GetPayeeHierarchy(int CompanyId, string PayeeUserId,bool IsDataToBeDisplayedInReport);
        //code review comment IEnumerable<LPayeeViewModel> GetAll();
        //code review comment LPayeeViewModel GetPayeeByPayeeCode(int CompanyId, string PayeeCode);
        //code review comment LPayeeViewModel GetFileNameByPayeeId(int PayeeId);
        //IEnumerable<DownloadPayeeGridByTabNameViewModel> DownloadPayeeGridByStatusNameCreatedByUserId(string StatusName, int CompanyId, string CreatedByUserId);
       // IEnumerable<DownloadPayeeGridByTabNameViewModel> DownloadPayeeGridByStatusNameReportsToId(string StatusName, int CompanyId, string ReportsToId);
        //code review comment IEnumerable<LPayeeViewModel> GetByStatusName(string StatusName, int CompanyId, string ReportsToId);
        //code review comment IEnumerable<LPayeeViewModel> GetByStatusNameCreatedByUserId(string StatusName, int CompanyId, string CreatedByUserId);
        //code review comment IEnumerable<LPayeeViewModel> GetByStatusNamePrimaryChannel(string StatusName, int CompanyId, string PrimaryChannel);
        IEnumerable<LPayeeViewModel> GetActivePayee(int CompanyId);
        IEnumerable<LPayeeViewModel> GetApprovedPayeeTree(string AsOfDate, int CompanyId, string PrimaryChannel);
        IEnumerable<LPayeeViewModel> GetApprovedPayeePortfolioTree(string AsOfDate, int CompanyId, string LoggedInUserId);
        LPayeeViewModel GetById(int id);
        IEnumerable<LPayeeViewModel> GetPayeeForClaimsDropdown(int CompanyId, string LoggedInUserId, string PortfolioList, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery,string UseRole, string PayeeId);
        LPayeeViewModel GetByPayeeUserId(string id);
        IEnumerable<LPayeeViewModel> GetParentDropDown(int CompanyId);
        IEnumerable<LCompanySpecificColumnViewModel> GetLPayeesColumnsForGrid();
      //  void UpdatePayeeStatus(Sp_UpdateItemStatusViewModel model);
        int Add(LPayeeViewModel serverData, string LoggedInRoleId, string WorkflowName, string AttachmentPath,string PortfolioList,string RedirectToUrl, string Source, string UserLobbyId);
        List<LPayeeViewModel> AddUploadedPayees(List<LPayeeViewModel> serverData,  List<mEntiryPortfolioViewModel> portFolioList, List<LChangeRequestViewModel> ChangeRequests, string LoggedInRoleId, string WorkflowName, string FileName, string RedirectToUrl);
        void Update(LPayeeViewModel serverData, string AttachedFiles, string AttachmentPath, string PortfolioList, string LoggedInUserId,string RedirectToUrl);
        //code review comment void Delete(int id);

       dynamic GetPayeeByChannelManagerUserID(int CompanyId, string LoggedInUserId,bool IsDataToBeDisplayedInReport);
        bool CheckEmailExists(string strEmailID);
        LPayeeViewModel GetPayeeDetailsById(int id);
        IEnumerable<LPayeeViewModel> GetPayeeHierarchyByPayeeId(int CompanyId, string PayeeId);
        IEnumerable<LPayeeViewModel> GetAllPayeeCodesAndEmailID(int CompanyId);
        string ValidatePayeeParent(string PayeeCode, string ParentCode, string PrimaryChannel, DateTime EffectiveDate);
        List<string> GetParentListByPayeeId(string PayeeIdList);
        string GetPayeeUploadHelp();
        string MyPayeeReport(int CompanyId, string LoggedInUserId, string UserRole, string PortfolioList, string LoggedInUserName, bool DownloadReportData);
        int GetParentPayeeGridCounts(int CompanyId);
        IEnumerable<LPayeeViewModel> GetParentPayeeData(int CompanyId, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, int? ParentPayeeId);
        int GetPayeeCountsForPortfolioMatching(int CompanyId, string LoggedInUserId, string PortfolioList,string UserRole);

        string GetSudmitableorNot(int Transactionid, string Workflow, string Role, int CompanyID);

        int GetPayeeDetailsByPayeeCodeId(String PayeeCode);
        string CheckIsWIAMEnabled();
    }
}