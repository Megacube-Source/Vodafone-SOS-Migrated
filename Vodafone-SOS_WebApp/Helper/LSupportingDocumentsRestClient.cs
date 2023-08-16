using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LSupportingDocumentsRestClient:ILSupportingDocumentsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public LSupportingDocumentsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

       

        public IEnumerable<LSupportingDocumentViewModel> GetByEntityType(string EntityType, int EntityId)
        {
            var request = new RestRequest("api/LSupportingDocuments/GetLSupportingDocuments?EntityType={EntityType}&EntityId={EntityId}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);

            var response = _client.Execute<List<LSupportingDocumentViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public void AddSupportingDocs(string EntityType, int EntityId, string FileNameList, string FilePath, string LoggedInUserId)
        {
            var request = new RestRequest("api/LSupportingDocuments/PostLSupportingDocuments?EntityType={EntityType}&EntityId={EntityId}&FileNameList={FileNameList}&FilePath={FilePath}&LoggedInUserId={LoggedInUserId}", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("FileNameList", FileNameList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("LoggedInUserId", LoggedInUserId, ParameterType.UrlSegment);

            var response = _client.Execute(request);

        }

        public void Delete(int id)
        {
            var request = new RestRequest("api/LSupportingDocuments/DeleteLSupportingDocument/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LSupportingDocumentViewModel>(request);

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

        public void UpdateAttachment(int id, string CreatedBy, string FileName, string FilePath, string entitytype)
        {
            var request = new RestRequest(Method.GET) { RequestFormat = DataFormat.Json };
            request.Resource = "api/LSupportingDocuments/UpdateAttachmentTicket?id={id}&FileName={FileName}&FilePath={FilePath}&CreatedBy={CreatedBy}&entitytype={entitytype}";
            request.AddParameter("id", id, ParameterType.UrlSegment);
            request.AddParameter("FileName", string.IsNullOrEmpty(FileName) ? "" : FileName, ParameterType.UrlSegment);
            request.AddParameter("FilePath", string.IsNullOrEmpty(FilePath) ? "" : FilePath, ParameterType.UrlSegment);
            request.AddParameter("CreatedBy", string.IsNullOrEmpty(CreatedBy) ? "" : CreatedBy, ParameterType.UrlSegment);
            request.AddParameter("entitytype", string.IsNullOrEmpty(entitytype) ? "" : entitytype, ParameterType.UrlSegment);
            var response = _client.Execute<dynamic>(request);
        }

    }

    interface ILSupportingDocumentsRestClient
    {
        void AddSupportingDocs(string EntityType, int EntityId, string FileNameList, string FilePath, string LoggedInUserId);
        IEnumerable<LSupportingDocumentViewModel> GetByEntityType(string EntityType, int EntityId);
        void Delete(int id);
        void UpdateAttachment(int id, string CreatedBy, string FileName, string FilePath, string entitytype);

    }
}