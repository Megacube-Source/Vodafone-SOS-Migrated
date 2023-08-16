using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{

    public class LReadyToPayRestClient:ILReadyToPayRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public LReadyToPayRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<XPayeeDocumentsViewModel> GetXPayeeDocuments(string CompanyCode, int BatchNumber)
        {
            var request = new RestRequest("api/LReadyToPay/GetXPayeeDocuments?CompanyCode={CompanyCode}&BatchNumber={BatchNumber}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", BatchNumber, ParameterType.UrlSegment);
            var response = _client.Execute<List<XPayeeDocumentsViewModel>>(request);
            return response.Data;
        }


        //ReadyToPayDataCounts
        public int ReadyToPayDataCounts(int iRTPID, int iCompanyID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strAction, string strPayBatchName, string strBatchCommPeriod, string strRTPData, string strCreatedBy, string strUpdatedBy, string strPortfolios)
        {
            LReadyToPayCount objLReadyToPayCount = new LReadyToPayCount();
            //var request = new RestRequest("api/LReadyToPay/GetDataCounts?iRTPID={iRTPID}&iCompanyID={iCompanyID}&blnIsBatchList={blnIsBatchList}&strType={strType}&strRTPStatus={strRTPStatus}&strAction={strAction}&strPayBatchName={strPayBatchName}&strBatchCommPeriod={strBatchCommPeriod}&strRTPData={strRTPData}&strCreatedBy={strCreatedBy}&strUpdatedBy={strUpdatedBy}&strPortfolios={strPortfolios}", Method.GET) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/LReadyToPay/GetDataCounts", Method.POST) { RequestFormat = DataFormat.Json };
            //var request = new RestRequest("api/LReadyToPay/GetData?iRTPID={iRTPID}&iCompanyID={iCompanyID}&blnIsBatchList={blnIsBatchList}&strType={strType}&strRTPStatus={strRTPStatus}", Method.GET) { RequestFormat = DataFormat.Json };
            //request.AddParameter("iRTPID", iRTPID, ParameterType.UrlSegment);
            //request.AddParameter("iCompanyID", iCompanyID, ParameterType.UrlSegment);
            //request.AddParameter("blnIsBatchList", blnIsBatchList, ParameterType.UrlSegment);
            //request.AddParameter("strType", strType, ParameterType.UrlSegment);
            //request.AddParameter("strRTPStatus", strRTPStatus, ParameterType.UrlSegment);
            //request.AddParameter("strAction", strAction, ParameterType.UrlSegment);
            //request.AddParameter("strPayBatchName", strPayBatchName, ParameterType.UrlSegment);
            //request.AddParameter("strBatchCommPeriod", strBatchCommPeriod, ParameterType.UrlSegment);
            //request.AddParameter("strRTPData", strRTPData, ParameterType.UrlSegment);
            //request.AddParameter("strCreatedBy", strCreatedBy, ParameterType.UrlSegment);
            //request.AddParameter("strUpdatedBy", strUpdatedBy, ParameterType.UrlSegment);
            //request.AddParameter("strPortfolios", strPortfolios, ParameterType.UrlSegment);
            objLReadyToPayCount.iRTPID =  iRTPID;
            objLReadyToPayCount.iCompanyID = iCompanyID;
            objLReadyToPayCount.blnIsBatchList = blnIsBatchList ;
            objLReadyToPayCount.strType = strType;
            objLReadyToPayCount.strRTPStatus = strRTPStatus;
            objLReadyToPayCount.strAction = strAction;
            objLReadyToPayCount.strPayBatchName = strPayBatchName;
            objLReadyToPayCount.strBatchCommPeriod = strBatchCommPeriod;
            objLReadyToPayCount.strRTPData = strRTPData;
            objLReadyToPayCount.strCreatedBy = strCreatedBy;
            objLReadyToPayCount.strUpdatedBy = strUpdatedBy;
            objLReadyToPayCount.strPortfolios = strPortfolios;
            request.AddBody(objLReadyToPayCount);

                 var response = _client.Execute<int>(request);
            return response.Data;
        }

        public IEnumerable<LReadyToPayViewModel> ReadyToPayData(int iRTPID, int iCompanyID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strAction, string strPayBatchName, string strBatchCommPeriod, string strRTPData, string strCreatedBy, string strUpdatedBy,string strPortfolios, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {
            LReadyToPayCount objLReadyToPayCount = new LReadyToPayCount();
            //var request = new RestRequest("api/LReadyToPay/GetData?iRTPID={iRTPID}&iCompanyID={iCompanyID}&blnIsBatchList={blnIsBatchList}&strType={strType}&strRTPStatus={strRTPStatus}&strAction={strAction}&strPayBatchName={strPayBatchName}&strBatchCommPeriod={strBatchCommPeriod}&strRTPData={strRTPData}&strCreatedBy={strCreatedBy}&strUpdatedBy={strUpdatedBy}&strPortfolios={strPortfolios}&sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}", Method.GET) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/LReadyToPay/GetData", Method.POST) { RequestFormat = DataFormat.Json };
            //var request = new RestRequest("api/LReadyToPay/GetData?iRTPID={iRTPID}&iCompanyID={iCompanyID}&blnIsBatchList={blnIsBatchList}&strType={strType}&strRTPStatus={strRTPStatus}", Method.GET) { RequestFormat = DataFormat.Json };
            //request.AddParameter("iRTPID", iRTPID, ParameterType.UrlSegment);
            //request.AddParameter("iCompanyID", iCompanyID, ParameterType.UrlSegment);
            //request.AddParameter("blnIsBatchList", blnIsBatchList, ParameterType.UrlSegment);
            //request.AddParameter("strType", strType, ParameterType.UrlSegment);
            //request.AddParameter("strRTPStatus", strRTPStatus, ParameterType.UrlSegment);
            //request.AddParameter("strAction", strAction, ParameterType.UrlSegment);
            //request.AddParameter("strPayBatchName", strPayBatchName, ParameterType.UrlSegment);
            //request.AddParameter("strBatchCommPeriod", strBatchCommPeriod, ParameterType.UrlSegment);
            //request.AddParameter("strRTPData", strRTPData, ParameterType.UrlSegment);
            //request.AddParameter("strCreatedBy", strCreatedBy, ParameterType.UrlSegment);
            //request.AddParameter("strUpdatedBy", strUpdatedBy, ParameterType.UrlSegment);
            //request.AddParameter("strPortfolios", strPortfolios, ParameterType.UrlSegment);
            //request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            //request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            //request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            //request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            //request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);

            objLReadyToPayCount.iRTPID = iRTPID;
            objLReadyToPayCount.iCompanyID = iCompanyID;
            objLReadyToPayCount.blnIsBatchList = blnIsBatchList;
            objLReadyToPayCount.strType = strType;
            objLReadyToPayCount.strRTPStatus = strRTPStatus;
            objLReadyToPayCount.strAction = strAction;
            objLReadyToPayCount.strPayBatchName = strPayBatchName;
            objLReadyToPayCount.strBatchCommPeriod = strBatchCommPeriod;
            objLReadyToPayCount.strRTPData = strRTPData;
            objLReadyToPayCount.strCreatedBy = strCreatedBy;
            objLReadyToPayCount.strUpdatedBy = strUpdatedBy;
            objLReadyToPayCount.strPortfolios = strPortfolios;

            objLReadyToPayCount.sortdatafield = string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield;
            objLReadyToPayCount.sortorder = string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder;
            objLReadyToPayCount.pagesize = pagesize;
            objLReadyToPayCount.pagenum = pagenum;
            objLReadyToPayCount.FilterQuery = string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery;

            request.AddBody(objLReadyToPayCount);
            var response = _client.Execute<List<LReadyToPayViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }

        public LReadyToPayViewModel GetRTPDetails(int Id)
        {
            var request = new RestRequest("api/LReadyToPay/GetRTPByID?Id={Id}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            var response = _client.Execute<LReadyToPayViewModel>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public string GetPaymentCount(int Id, string iCompanyID)
        {
            var request = new RestRequest("api/LReadyToPay/GetCountPaymentDocument?Id={Id}&CompanyCode={iCompanyID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("Id", Id, ParameterType.UrlSegment);
            request.AddParameter("iCompanyID", iCompanyID, ParameterType.UrlSegment);
            var response = _client.Execute<int>(request);
            
            return response.Content;
        }

        public void AddEditBatch(int iRTPID, int iCompanyID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strAction, string strPayBatchName, string strBatchCommPeriod, string strRTPData, string strCreatedBy, string strUpdatedBy, string strPortfolios,bool EmailDocuments, bool SendPayeeDocuments, string PayPublishEmailIds,string UserRole, Boolean IsClaimChanged, Boolean IsCalChanged, Boolean isMAChanged)
        {
            LReadyToPaySaveModel objSaveModelData = new LReadyToPaySaveModel();
            //var request = new RestRequest("api/LReadyToPay/SaveUpdateRTPData?iRTPID={iRTPID}&iCompanyID={iCompanyID}&blnIsBatchList={blnIsBatchList}&strType={strType}&strRTPStatus={strRTPStatus}&strAction={strAction}&strPayBatchName={strPayBatchName}&strBatchCommPeriod={strBatchCommPeriod}&strRTPData={strRTPData}&strCreatedBy={strCreatedBy}&strUpdatedBy={strUpdatedBy}&strPortfolios={strPortfolios}&EmailDocuments={EmailDocuments}&SendPayeeDocuments={SendPayeeDocuments}&PayPublishEmailIds={PayPublishEmailIds}&UserRole={UserRole}", Method.POST) { RequestFormat = DataFormat.Json };
            var request = new RestRequest("api/LReadyToPay/SaveUpdateRTPData", Method.POST) { RequestFormat = DataFormat.Json };
            objSaveModelData.iRTPID = iRTPID;
            objSaveModelData.iCompanyID = iCompanyID;
            objSaveModelData.blnIsBatchList = blnIsBatchList;
            objSaveModelData.strType = strType;
            objSaveModelData.strRTPStatus = strRTPStatus;
            objSaveModelData.strAction = strAction;
            objSaveModelData.strPayBatchName = strPayBatchName;
            objSaveModelData.strBatchCommPeriod = strBatchCommPeriod;
            objSaveModelData.strRTPData  = strRTPData;
            objSaveModelData.strCreatedBy = strCreatedBy;
            objSaveModelData.strUpdatedBy = strUpdatedBy;
            objSaveModelData.strPortfolios = strPortfolios;
            objSaveModelData.EmailDocuments = EmailDocuments;
            objSaveModelData.SendPayeeDocuments = SendPayeeDocuments;
            objSaveModelData.PayPublishEmailIds = PayPublishEmailIds;
            objSaveModelData.IsClaimChanged = IsClaimChanged;
            objSaveModelData.IsCalChanged = IsCalChanged;
            objSaveModelData.isMAChanged = isMAChanged;
            objSaveModelData.UserRole = UserRole;
            request.AddBody(objSaveModelData);
            //request.AddParameter("iRTPID", iRTPID, ParameterType.UrlSegment);
            //request.AddParameter("iCompanyID", iCompanyID, ParameterType.UrlSegment);
            //request.AddParameter("blnIsBatchList", blnIsBatchList, ParameterType.UrlSegment);
            //request.AddParameter("strType", strType, ParameterType.UrlSegment);
            //request.AddParameter("strRTPStatus", strRTPStatus, ParameterType.UrlSegment);
            //request.AddParameter("strAction", strAction, ParameterType.UrlSegment);
            //request.AddParameter("strPayBatchName", strPayBatchName, ParameterType.UrlSegment);
            //request.AddParameter("strBatchCommPeriod", strBatchCommPeriod, ParameterType.UrlSegment);
            //request.AddParameter("strRTPData", strRTPData, ParameterType.UrlSegment);
            //request.AddParameter("strCreatedBy", strCreatedBy, ParameterType.UrlSegment);
            //request.AddParameter("strUpdatedBy", strUpdatedBy, ParameterType.UrlSegment);
            //request.AddParameter("strPortfolios", strPortfolios, ParameterType.UrlSegment);
            //request.AddParameter("EmailDocuments", EmailDocuments, ParameterType.UrlSegment);
            //request.AddParameter("SendPayeeDocuments", SendPayeeDocuments, ParameterType.UrlSegment);
            //request.AddParameter("PayPublishEmailIds", string.IsNullOrEmpty(PayPublishEmailIds)?string.Empty:PayPublishEmailIds, ParameterType.UrlSegment);
            //request.AddParameter("UserRole", UserRole, ParameterType.UrlSegment);
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
        public IEnumerable<LReadyToPayViewModel>  GetNewRTPByCompanyID(int CompanyId, string strUserID)
        {
            var request = new RestRequest("api/LReadyToPay/GetNewRTPByCompanyID?CompanyId={CompanyId}&strUserID={strUserID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("strUserID", strUserID, ParameterType.UrlSegment);
            var response = _client.Execute<List<LReadyToPayViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public IEnumerable<LReadyToPayViewModel> GetCancelledRTPByCompanyID(int CompanyId, string strUserID)
        {
            var request = new RestRequest("api/LReadyToPay/GetCancelledRTPByCompanyID?CompanyId={CompanyId}&strUserID={strUserID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("strUserID", strUserID, ParameterType.UrlSegment);
            var response = _client.Execute<List<LReadyToPayViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public IEnumerable<LReadyToPayViewModel> GetPayGeneratedRTPByCompanyID(int CompanyId, string strUserID)
        {
            var request = new RestRequest("api/LReadyToPay/GetPayGeneratedRTPByCompanyID?CompanyId={CompanyId}&strUserID={strUserID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("strUserID", strUserID, ParameterType.UrlSegment);
            var response = _client.Execute<List<LReadyToPayViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public IEnumerable<LReadyToPayViewModel> GetPublishedRTPByCompanyID(int CompanyId, string strUserID)
        {
            var request = new RestRequest("api/LReadyToPay/GetPublishedRTPByCompanyID?CompanyId={CompanyId}&strUserID={strUserID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("strUserID", strUserID, ParameterType.UrlSegment);
            var response = _client.Execute<List<LReadyToPayViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public IEnumerable<LPortfolioViewModel> GetRTPPortfolios(int RTPId, int CompanyId, string strAction, string strUserID, string Role)
        {
            var request = new RestRequest("api/LReadyToPay/GetRTPPortfolios?RTPId={RTPId}&CompanyId={CompanyId}&strAction={strAction}&strUserID={strUserID}&Role={Role}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("RTPId", RTPId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("strAction", strAction, ParameterType.UrlSegment);
            request.AddParameter("strUserID", strUserID, ParameterType.UrlSegment);
            request.AddParameter("Role", Role, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPortfolioViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }
        public IEnumerable<XPayeeDocumentsViewModel> GetPayeeDocumentpaging(string companycode , int PaymentBatchNO, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery, string PayeeCode)
        {
            var request = new RestRequest("api/LReadyToPay/GetPayeeDocumentpaging?CompanyCode={CompanyCode}&BatchNumber={BatchNumber}&&sortdatafield={sortdatafield}&sortorder={sortorder}&pagesize={pagesize}&pagenum={pagenum}&FilterQuery={FilterQuery}&PayeeCode={PayeeCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyCode", companycode, ParameterType.UrlSegment);
            request.AddParameter("BatchNumber", PaymentBatchNO, ParameterType.UrlSegment);
            request.AddParameter("sortdatafield", string.IsNullOrEmpty(sortdatafield) ? string.Empty : sortdatafield, ParameterType.UrlSegment);
            request.AddParameter("sortorder", string.IsNullOrEmpty(sortorder) ? string.Empty : sortorder, ParameterType.UrlSegment);
            request.AddParameter("pagesize", pagesize, ParameterType.UrlSegment);
            request.AddParameter("pagenum", pagenum, ParameterType.UrlSegment);
            request.AddParameter("FilterQuery", string.IsNullOrEmpty(FilterQuery) ? string.Empty : FilterQuery, ParameterType.UrlSegment);
            request.AddParameter("PayeeCode", string.IsNullOrEmpty(PayeeCode) ? string.Empty : PayeeCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<XPayeeDocumentsViewModel>>(request);
            return response.Data;            
        }

        public IEnumerable<ValidateClaimsViewModel> GetClaimValidate(int iCompanyId, string Type, string CSVData, string strPortfolios)
        {
            LReadyToPayCount objLReadyToPayCount = new LReadyToPayCount();
          
            var request = new RestRequest("api/LReadyToPay/GetClaimValidate", Method.POST) { RequestFormat = DataFormat.Json };
            
            objLReadyToPayCount.iRTPID = 0;
            objLReadyToPayCount.iCompanyID = iCompanyId;
            objLReadyToPayCount.blnIsBatchList = false;
            objLReadyToPayCount.strType = Type;
            objLReadyToPayCount.strRTPStatus = CSVData;
            objLReadyToPayCount.strAction = "";
            objLReadyToPayCount.strPayBatchName = "";
            objLReadyToPayCount.strBatchCommPeriod = "";
            objLReadyToPayCount.strRTPData = "";
            objLReadyToPayCount.strCreatedBy = "";
            objLReadyToPayCount.strUpdatedBy = "";
            objLReadyToPayCount.strPortfolios = strPortfolios;

            objLReadyToPayCount.sortdatafield = "";
            objLReadyToPayCount.sortorder = ""; ;
            objLReadyToPayCount.pagesize = 0;
            objLReadyToPayCount.pagenum = 0;
            objLReadyToPayCount.FilterQuery = "" ;

            request.AddBody(objLReadyToPayCount);
            var response = _client.Execute<List<ValidateClaimsViewModel>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);
            return response.Data;
        }


    }
    interface ILReadyToPayRestClient
    {
        IEnumerable<XPayeeDocumentsViewModel> GetXPayeeDocuments(string CompanyCode, int BatchNumber);
        IEnumerable<LReadyToPayViewModel> ReadyToPayData(int iRTPID, int iCompanyID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strAction, string strPayBatchName, string strBatchCommPeriod, string strRTPData, string strCreatedBy, string strUpdatedBy, string strPortfolios, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery);
        LReadyToPayViewModel GetRTPDetails(int Id);
        IEnumerable<LReadyToPayViewModel> GetNewRTPByCompanyID(int CompanyId, string strUserID);
        IEnumerable<LReadyToPayViewModel> GetCancelledRTPByCompanyID(int CompanyId, string strUserID);
        IEnumerable<LReadyToPayViewModel> GetPayGeneratedRTPByCompanyID(int CompanyId, string strUserID);
        IEnumerable<LReadyToPayViewModel> GetPublishedRTPByCompanyID(int CompanyId,string strUserID);
        void AddEditBatch(int iRTPID, int iCompanyID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strAction, string strPayBatchName, string strBatchCommPeriod, string strRTPData, string strCreatedBy, string strUpdatedBy, string strPortfolios, bool EmailDocuments, bool SendPayeeDocuments, string PayPublishEmailIds, string UserRole, Boolean IsClaimChanged, Boolean IsCalChanged, Boolean isMAChanged);
        IEnumerable<LPortfolioViewModel> GetRTPPortfolios(int RTPId, int CompanyId, string strAction, string strUserID, string Role);
        int ReadyToPayDataCounts(int iRTPID, int iCompanyID, Boolean blnIsBatchList, string strType, string strRTPStatus, string strAction, string strPayBatchName, string strBatchCommPeriod, string strRTPData, string strCreatedBy, string strUpdatedBy, string strPortfolios);

        IEnumerable<ValidateClaimsViewModel> GetClaimValidate(int iCompanyId, string strType, string  strRTPStatus, string strPortfolios);
    }
}