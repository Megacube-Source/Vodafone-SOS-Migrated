using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class ManualAdjustmentsRestClient:IManualAdjustmentsRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public ManualAdjustmentsRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }

       public List<string> UploadManualAdjustments(List<UploadManualAdjustmentViewModel> serverData,string FileName,string CommissionPeriod,string UpdatedBy,
           string PortfolioList,string AtachmentFiles, string LoggedInRoleId,string FilePath,string WorkflowName,int CompanyId, string BatchName)
        {
            //MAParameterCarrier objData = new MAParameterCarrier();
            //objData.FileName = FileName;
            //objData.CommissionPeriod = CommissionPeriod;
            //objData.UpdatedBy = UpdatedBy;
            //objData.PortfolioList = PortfolioList;
            //objData.AtachmentFiles = string.IsNullOrEmpty(AtachmentFiles)?"": AtachmentFiles;
            //objData.LoggedInRoleId = LoggedInRoleId;
            //objData.WorkflowName = WorkflowName;
            //objData.CompanyId = CompanyId;
            //objData.BatchName = BatchName;
            if(serverData.Count > 0)
            {
                serverData[0].ParameterCarrier = PortfolioList;
            }

            var request = new RestRequest("api/ManualAdjustments/PostUploadManualAdjustMent?FileName={FileName}&CommissionPeriod={CommissionPeriod}&UpdatedBy={UpdatedBy}&AtachmentFiles={AtachmentFiles}&LoggedInRoleId={LoggedInRoleId}&WorkflowName={WorkflowName}&FilePath={FilePath}&BatchName={BatchName}&CompanyId={CompanyId}", Method.POST) { RequestFormat = DataFormat.Json };                    
            request.AddBody(serverData);

            request.AddParameter("LoggedInRoleId", LoggedInRoleId, ParameterType.UrlSegment);
            request.AddParameter("FileName", FileName, ParameterType.UrlSegment);
            request.AddParameter("CommissionPeriod", CommissionPeriod, ParameterType.UrlSegment);
            request.AddParameter("UpdatedBy", UpdatedBy, ParameterType.UrlSegment);
           // request.AddParameter("PortfolioList", PortfolioList, ParameterType.UrlSegment);
            request.AddParameter("FilePath", FilePath, ParameterType.UrlSegment);
            request.AddParameter("WorkflowName", WorkflowName, ParameterType.UrlSegment);
            request.AddParameter("BatchName", BatchName, ParameterType.UrlSegment);
            request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
            request.AddParameter("AtachmentFiles", (string.IsNullOrEmpty(AtachmentFiles)) ? "" : AtachmentFiles, ParameterType.UrlSegment);
            var response = _client.Execute<List<string>>(request);

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
            if (response.StatusCode == HttpStatusCode.OK && response.Data.Count == 1)//Update Batch Ordinal in Workflow if no error is found
            {
                int BatchId = 0;
                try
                { 
                    string source = response.Content;
                    dynamic data = JsonConvert.DeserializeObject(source);
                    BatchId = Convert.ToInt32(data);
                   
                }
                catch
                {
                    return response.Data;
                }
                IGenericGridRestClient GGRC = new GenericGridRestClient();
                GGRC.UpdateActionStatus(WorkflowName, Convert.ToString(BatchId), CompanyId, "Approve", UpdatedBy, string.Empty, LoggedInRoleId,string.Empty);
                return null;
            }

            return response.Data;
        }
       
    }

    interface IManualAdjustmentsRestClient
    {
        List<string> UploadManualAdjustments(List<UploadManualAdjustmentViewModel> serverData, string FileName, string CommissionPeriod, string UpdatedBy, string PortfolioList, string AtachmentFiles, string LoggedInRoleId, string FilePath, string WorkflowName, int CompanyId,string BatchName);
    }
}