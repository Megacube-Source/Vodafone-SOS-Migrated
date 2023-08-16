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
    public class LAllocationRulesRestClient:ILAllocationRulesRestClient
    {
        #region Dead code by VG/RK 10062017
    //    private readonly RestClient _client;
    //    private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];

    //    public LAllocationRulesRestClient()
    //    {
    //        _client = new RestClient { BaseUrl = new System.Uri(_url) };
    //    }

    //    public IEnumerable<LAllocationRuleViewModel> GetAll()
    //    {
    //        var request = new RestRequest("api/RBrands/GetRBrands", Method.GET) { RequestFormat = DataFormat.Json };

    //        var response = _client.Execute<List<LAllocationRuleViewModel>>(request);

    //        if (response.Data == null)
    //            throw new Exception(response.ErrorMessage);

    //        return response.Data;
    //    }

    //    public IEnumerable<LAllocationRuleViewModel> GetByCompanyId(int CompanyId)
    //    {
    //        var request = new RestRequest("api/LAllocationRules/GetLAllocationRulesByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
    //        request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
    //        var response = _client.Execute<List<LAllocationRuleViewModel>>(request);

    //        if (response.Data == null)
    //            throw new Exception(response.ErrorMessage);

    //        return response.Data;
    //    }

    //    //Method Added by shivanig GetCommissionBatchByCompanyId
    //  /* public IEnumerable<LCommissionBatchAllacationRulesViewModel> GetLCommissionBatchAllocationRulesByCompanyId(int CompanyId)
    //    {
    //        var request = new RestRequest("api/LAllocationRules/GetLCommissionBatchAllocationRulesByCompanyId?CompanyId={CompanyId}", Method.GET) { RequestFormat = DataFormat.Json };
    //        request.AddParameter("CompanyId", CompanyId, ParameterType.UrlSegment);
    //        var response = _client.Execute<List<LCommissionBatchAllacationRulesViewModel>>(request);

    //        if (response.Data == null)
    //            throw new Exception(response.ErrorMessage);

    //        return response.Data;
    //    }*/

    //    //This method saves the Grid data 
    //    /*
    //     * public void SaveCommissionAllocationRule(string GridData, int CompanyId)
    //    {
    //        var GridModel = new GridDataViewModel { CompanyId = CompanyId, GridData = GridData };
    //        var request = new RestRequest("api/LAllocationRules/SaveCommissionAllocationRule", Method.POST) { RequestFormat = DataFormat.Json };
    //        request.AddBody(GridModel);
    //        var response = _client.Execute<LCommissionBatchAllacationRulesViewModel>(request);

    //        if (response.StatusCode == HttpStatusCode.InternalServerError)
    //        {
    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }
    //        if (response.StatusCode == HttpStatusCode.BadRequest)
    //        {
    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }
    //    }*/
    //    //checking this method whether it is required or not
    ///*   public void AddCommissionAllocation(LCommissionBatchAllacationRulesViewModel serverData)
    //    {
    //        var request = new RestRequest("api/LAllocationRules/AddCommissionAllocationRule", Method.POST) { RequestFormat = DataFormat.Json };
    //        request.AddBody(serverData);

    //        var response = _client.Execute<LCommissionBatchAllacationRulesViewModel>(request);

    //        if (response.StatusCode == HttpStatusCode.InternalServerError)
    //        {
    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }
    //        if (response.StatusCode == HttpStatusCode.BadRequest)
    //        {
    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }
    //    }*/

    //    public LAllocationRuleViewModel GetById(int id)
    //    {
    //        var request = new RestRequest("api/LAllocationRules/GetLAllocationRule/{id}", Method.GET) { RequestFormat = DataFormat.Json };

    //        request.AddParameter("id", id, ParameterType.UrlSegment);
    //        var response = _client.Execute<LAllocationRuleViewModel>(request);

    //        if (response.Data == null)
    //            throw new Exception(response.ErrorMessage);

    //        return response.Data;
    //    }

    //    public void Add(LAllocationRuleViewModel serverData)
    //    {
    //        var request = new RestRequest("api/LAllocationRules/PostLAllocationRule", Method.POST) { RequestFormat = DataFormat.Json };
    //        request.AddBody(serverData);

    //        var response = _client.Execute<LAllocationRuleViewModel>(request);

    //        if (response.StatusCode == HttpStatusCode.InternalServerError)
    //        {
    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }
    //        if (response.StatusCode == HttpStatusCode.BadRequest)
    //        {
    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }
    //    }

    //    public void AddGridData(string GridData,int CompanyId)
    //    {
    //        var GridModel = new GridDataViewModel {CompanyId=CompanyId,GridData=GridData };
    //        var request = new RestRequest("api/LAllocationRules/PostLAllocationRulesGrid", Method.POST) { RequestFormat = DataFormat.Json };
    //        request.AddBody(GridModel);
    //        var response = _client.Execute<LAllocationRuleViewModel>(request);

    //        if (response.StatusCode == HttpStatusCode.InternalServerError)
    //        {
    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }
    //        if (response.StatusCode == HttpStatusCode.BadRequest)
    //        {
    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }
    //    }

    //    public void Update(LAllocationRuleViewModel serverData)
    //    {
    //        var request = new RestRequest("api/LAllocationRules/PutLAllocationRule/{id}", Method.PUT) { RequestFormat = DataFormat.Json };
    //        request.AddParameter("id", serverData.Id, ParameterType.UrlSegment);
    //        request.AddBody(serverData);

    //        var response = _client.Execute<LAllocationRuleViewModel>(request);

    //        if (response.StatusCode == HttpStatusCode.NotFound)
    //        {
    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }



    //        if (response.StatusCode == HttpStatusCode.InternalServerError)
    //        {
    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }
    //        if (response.StatusCode == HttpStatusCode.BadRequest)
    //        {
    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }

    //    }

    //    public void Delete(int id)
    //    {
    //        var request = new RestRequest("api/LAllocationRules/DeleteLAllocationRule/{id}", Method.DELETE);
    //        request.AddParameter("id", id, ParameterType.UrlSegment);

    //        var response = _client.Execute<LAllocationRuleViewModel>(request);

    //        if (response.StatusCode != HttpStatusCode.OK)
    //        {

    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }
    //    }
    //    /*
    //    public void DeleteCommissionAllocationRule(int id)
    //    {
    //        var request = new RestRequest("api/LAllocationRules/DeleteLCommissionBactchAllocationRule/{id}", Method.DELETE);
    //        request.AddParameter("id", id, ParameterType.UrlSegment);

    //        var response = _client.Execute<LCommissionBatchAllacationRulesViewModel>(request);

    //        if (response.StatusCode != HttpStatusCode.OK)
    //        {

    //            var ex = new Exception(String.Format("{0},{1}", response.ErrorMessage, response.StatusCode));
    //            ex.Data.Add("ErrorCode", response.StatusCode);
    //            string source = response.Content;
    //            dynamic data = JsonConvert.DeserializeObject(source);
    //            string xx = data.Message;
    //            ex.Data.Add("ErrorMessage", xx);
    //            throw ex;
    //        }
    //    }*/
        #endregion
    }

    interface ILAllocationRulesRestClient
    {
        #region Dead code by VG/RK 10062017
        //IEnumerable<LAllocationRuleViewModel> GetAll();
        //LAllocationRuleViewModel GetById(int id);
        //IEnumerable<LAllocationRuleViewModel> GetByCompanyId(int CompanyId);
        //// IEnumerable<LCommissionBatchAllacationRulesViewModel> GetLCommissionBatchAllocationRulesByCompanyId(int CompanyId);//Added by shivanig
        //// void AddCommissionAllocation(LCommissionBatchAllacationRulesViewModel LCommissionAllocationRuleViewModel);//added by shivani
        //void Add(LAllocationRuleViewModel LAllocationRuleViewModel);
        //void Update(LAllocationRuleViewModel LAllocationRuleViewModel);
        //void Delete(int id);
        //// void DeleteCommissionAllocationRule(int id);//added by shivanig

        ////      void SaveCommissionAllocationRule(string GridData, int CompanyId);
        //void AddGridData(string GridData, int CompanyId);
        #endregion  
    }
}