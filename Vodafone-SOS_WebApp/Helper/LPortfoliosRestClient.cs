using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LPortfoliosRestClient:ILPortfoliosRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LPortfoliosRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LPortfolioViewModel> GetByEntityIdList(string EntityType,string EntityIdList)
        {
            var request = new RestRequest("api/LPortfolios/GetPortfolioDetailsByEntityType?EntityType={EntityType}&EntityIdList={EntityIdList}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityIdList", EntityIdList, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPortfolioViewModel>>(request);

            if (response.Data == null)
               return null;

            return response.Data;
        }

        public IEnumerable<LPortfolioViewModel> GetByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/LPortfolios/GetLPortFolios?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPortfolioViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }


        public IEnumerable<LPortfolioViewModel> GetUnAssignedPortfolioNamesByCompanyId(int CompanyId)
        {
            var request = new RestRequest("api/LPortfolios/GetUnAssignedPortfolioNamesByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPortfolioViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }


        public IEnumerable<LPortfolioViewModel> GetByUserId(string UserId, string RoleId)
        {
            var request = new RestRequest("api/LPortfolios/GetLPortFolioByUserId?UserId={UserId}&RoleId={RoleId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPortfolioViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<LPortfolioViewModel> GetByUserIdForL2Admin(int UserId, string RoleId, string Type)
        {
            var request = new RestRequest("api/LPortfolios/GetLPortFolioByUserIdForL2Admin?UserId={UserId}&RoleId={RoleId}&Type={Type}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            request.AddParameter("Type", Type, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPortfolioViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        public IEnumerable<LPortfolioViewModel> GetByUserIdAndRefTypeID(string UserId, string RoleId, int RefTypeID)
        {
            var request = new RestRequest("api/LPortfolios/GetLPortFolioByUserIdWithRefilesTypes?UserId={UserId}&RoleId={RoleId}&RefTypeID={RefTypeID}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("RoleId", RoleId, ParameterType.UrlSegment);
            request.AddParameter("RefTypeID", RefTypeID, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPortfolioViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<string> GetByEntityId(string EntityType, int EntityId)
        {
            var request = new RestRequest("api/LPortfolios/GetLPortFolioByEntityId?EntityType={EntityType}&EntityId={EntityId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);
            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<LPortfolioViewModel> GetByLoggedInUserIdForEdit(string UserId,int TransactionId,string EntityType, string LoggedInRoleId, string Role,string CompanyCode)
        {
            var request = new RestRequest("api/LPortfolios/GetLPortFolioByLoggedInUserIdForEditGrid?UserId={UserId}&TransactionId={TransactionId}&EntityType={EntityType}&Role={Role}&LoggedInRoleId={LoggedInRoleId}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            request.AddParameter("Role", Role, ParameterType.UrlSegment);
            request.AddParameter("TransactionId", TransactionId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPortfolioViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<LPortfolioViewModel> GetByUserIdForEditGrid(int UserId,int CompanyId,string CompanyCode,string Role)
        {
            var request = new RestRequest("api/LPortfolios/GetLPortFolioByUserIdForEditGrid?UserId={UserId}&CompanyId={CompanyId}&Role={Role}&CompanyCode={CompanyCode}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("Role", Role, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPortfolioViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }



        public void UpdatePortfolio(int EntityId, string EntityType, string PortfolioList,string CompanyCode)
        {
            var request = new RestRequest("api/LPortfolios/PutUpdatePortfolio?EntityId={EntityId}&EntityType={EntityType}&PortfolioList={PortfolioList}&CompanyCode={CompanyCode}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("CompanyCode", CompanyCode, ParameterType.UrlSegment);
            var response = _client.Execute<List<LPortfolioViewModel>>(request);
            
        }

    }
    interface ILPortfoliosRestClient
    {
        IEnumerable<LPortfolioViewModel> GetByEntityIdList(string EntityType, string EntityIdList);
        void UpdatePortfolio(int EntityId, string EntityType, string PortfolioList,string CompanyCode);
        IEnumerable<LPortfolioViewModel> GetByCompanyId(int CompanyId);
        IEnumerable<string> GetByEntityId(string EntityType, int EntityId);
        IEnumerable<LPortfolioViewModel> GetByUserId(string UserId,string RoleId);
        IEnumerable<LPortfolioViewModel> GetByUserIdAndRefTypeID(string UserId, string RoleId, int reftypeid );
        IEnumerable<LPortfolioViewModel> GetByUserIdForEditGrid(int UserId, int CompanyId, string CompanyCode, string Role);
        IEnumerable<LPortfolioViewModel> GetByLoggedInUserIdForEdit(string UserId, int TransactionId,string EntityType, string LoggedInRoleId,string Role,string CompanyCode);
        IEnumerable<LPortfolioViewModel> GetUnAssignedPortfolioNamesByCompanyId(int CompanyId);

        IEnumerable<LPortfolioViewModel> GetByUserIdForL2Admin(int UserId, string RoleId, string Type);
    }
}