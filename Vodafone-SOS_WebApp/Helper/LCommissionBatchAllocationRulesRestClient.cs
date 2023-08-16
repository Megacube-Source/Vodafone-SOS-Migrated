//Code Review for this file (from security perspective) done
//This process is no longer in use hence the file can be discarded if it is not required till version 1.3 release.
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Helper
{
    public class LCommissionBatchAllocationRulesRestClient : ILCommissionBatchAllocationRulesRestClient
    {

        //code review comment
        //private readonly RestClient _client;
        //private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        //public LCommissionBatchAllocationRulesRestClient()
        //{
        //    _client = new RestClient { BaseUrl = new System.Uri(_url) };
        //}

        ////This method gets the data using api method and returns the response got from api.
        //public IEnumerable<LCommissionBatchAllacationRulesViewModel> GetLCommissionBatchAllocationRulesByCompanyId(int CompanyId)
        //{
        //    var request = new RestRequest("api/LCommissionBatchallocationRules/GetLCommissionBatchAllocationByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<LCommissionBatchAllacationRulesViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

        ////this method deletes the selected row using api method: DeleteLCommissionBactchAllocationRule
        //public void DeleteCommissionAllocationRule(int id)
        //{
        //    var request = new RestRequest("api/LCommissionBatchAllocationRules/Delete/{id}", Method.DELETE);
        //    request.AddParameter("id", id, ParameterType.UrlSegment);

        //    var response = _client.Execute<LCommissionBatchAllacationRulesViewModel>(request);

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

        ////This method saves the Grid data using api method: SaveCommissionAllocationRule and returns the response got from api.
        //public void SaveCommissionAllocationRule(string GridData, int CompanyId)
        //{
        //    var GridModel = new GridDataViewModel { CompanyId = CompanyId, GridData = GridData };
        //    var request = new RestRequest("api/LCommissionBatchAllocationRules/PostGridData", Method.POST) { RequestFormat = DataFormat.Json };
        //    request.AddBody(GridModel);
        //    var response = _client.Execute<LCommissionBatchAllacationRulesViewModel>(request);

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
        ////to get the reporting analyst list
        //public IEnumerable<AspnetUserViewModel> GetReportingAnalystByCompanyId(int CompanyId)
        //{
        //    var request = new RestRequest("api/LCommissionBatchAllocationRules/GetReportingAnalystByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
        //    request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
        //    var response = _client.Execute<List<AspnetUserViewModel>>(request);

        //    if (response.Data == null)
        //        throw new Exception(response.ErrorMessage);

        //    return response.Data;
        //}

    }
    interface ILCommissionBatchAllocationRulesRestClient
    {
        //code review comment
        //IEnumerable<LCommissionBatchAllacationRulesViewModel> GetLCommissionBatchAllocationRulesByCompanyId(int CompanyId);
        //void DeleteCommissionAllocationRule(int id);

        //void SaveCommissionAllocationRule(string GridData, int CompanyId);
        ////to get the reporting analyst list
        //IEnumerable<AspnetUserViewModel> GetReportingAnalystByCompanyId(int CompanyId);

    }
}