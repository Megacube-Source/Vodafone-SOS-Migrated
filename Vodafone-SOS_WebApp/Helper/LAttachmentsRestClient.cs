//Code Review for this file (from security perspective) done
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LAttachmentsRestClient:ILAttachmentsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        #region Code Review comment by VG/RK 10062017
        /*
         * public IEnumerable<LAttachmentViewModel> GetByUploadedFileId(int UploadedFileId)
        {
            var request = new RestRequest("api/LAttachments/GetLAttachmentsByUploadedFileId?UploadedFileId={UploadedFileId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UploadedFileId", UploadedFileId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAttachmentViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        

        public IEnumerable<LAttachmentViewModel> GetByUserId(int UserId)
        {
            var request = new RestRequest("api/LAttachments/GetLAttachmentsByUserId?UserId={UserId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("UserId", UserId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAttachmentViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<LAttachmentViewModel> GetByPayeeId(int PayeeId)
        {
            var request = new RestRequest("api/LAttachments/GetLAttachmentsByPayeeId?PayeeId={PayeeId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("PayeeId", PayeeId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAttachmentViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        
        public IEnumerable<LAttachmentViewModel> GetByClaimId(int ClaimId)
        {
            var request = new RestRequest("api/LAttachments/GetLAttachmentsByClaimId?ClaimId={ClaimId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddParameter("ClaimId", ClaimId, ParameterType.UrlSegment);
            var response = _client.Execute<List<LAttachmentViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public LAttachmentViewModel GetById(int id)
        {
            var request = new RestRequest("api/LAttachments/GetLAttachment/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LAttachmentViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
        
        public void Update(LAttachmentViewModel serverData)
        {
            var request = new RestRequest("api/LAttachments/PutLAttachment/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
            request.AddBody(serverData);

            var response = _client.Execute<LAttachmentViewModel>(request);

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

        }
         * 
         */
        #endregion
        public LAttachmentsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

        public IEnumerable<LAttachmentViewModel> GetByEntityType(string EntityType, int EntityId)
        {
            var request = new RestRequest("api/LAttachments/GetLAttachments?EntityType={EntityType}&EntityId={EntityId}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);

            var response = _client.Execute<List<LAttachmentViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<LAttachmentViewModel> GetLAttachmentsForPayeeID(string EntityType, int EntityId, string PayeeEmail)
        {
            var request = new RestRequest("api/LAttachments/GetLAttachmentsForPayeeID?EntityType={EntityType}&EntityId={EntityId}&PayeeEmail={PayeeEmail}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("PayeeEmail", PayeeEmail, ParameterType.UrlSegment);

            var response = _client.Execute<List<LAttachmentViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<LAttachmentViewModel> GetByEntityTypeByPayeeId(string EntityType, int EntityId,string PayeeId)
        {
            var request = new RestRequest("api/LAttachments/GetLAttachments?EntityType={EntityType}&EntityId={EntityId}&PayeeId={PayeeId}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("EntityType", EntityType, ParameterType.UrlSegment);
            request.AddParameter("EntityId", EntityId, ParameterType.UrlSegment);
            request.AddParameter("PayeeId", PayeeId, ParameterType.UrlSegment);

            var response = _client.Execute<List<LAttachmentViewModel>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public LAttachmentViewModel GetById(int id)
        {
            var request = new RestRequest("api/LAttachments/GetLAttachment/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);
            var response = _client.Execute<LAttachmentViewModel>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        //updating return type as need to get the added object id
        public LAttachmentViewModel Add(LAttachmentViewModel serverData)
        {
            var request = new RestRequest("api/LAttachments/PostLAttachment", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(serverData);

            var response = _client.Execute<LAttachmentViewModel>(request);

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


        public void Delete(int id)
        {
            var request = new RestRequest("api/LAttachments/DeleteLAttachment/{id}", Method.DELETE);
            request.AddParameter("id", id, ParameterType.UrlSegment);

            var response = _client.Execute<LAttachmentViewModel>(request);

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
    }
    interface ILAttachmentsRestClient
    {
        //Code review comment IEnumerable<LAttachmentViewModel> GetByUploadedFileId(int UploadedFileId);
        IEnumerable<LAttachmentViewModel> GetByEntityType(string EntityType, int EntityId);
        LAttachmentViewModel GetById(int id);
        LAttachmentViewModel Add(LAttachmentViewModel LAttachmentViewModel);//updated return type as needed object that is added

        //Code review comment void Update(LAttachmentViewModel LAttachmentViewModel);
        //Code review comment IEnumerable<LAttachmentViewModel> GetByUserId(int UserId);
        //Code review comment IEnumerable<LAttachmentViewModel> GetByPayeeId(int PayeeId);
        //Code review comment IEnumerable<LAttachmentViewModel> GetByClaimId(int ClaimId);
        void Delete(int id);
    }
}