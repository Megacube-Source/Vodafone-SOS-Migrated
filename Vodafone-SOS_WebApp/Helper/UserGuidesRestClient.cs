using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using System.Configuration;
using Newtonsoft.Json;
using Vodafone_SOS_WebApp.ViewModels;


namespace Vodafone_SOS_WebApp.Helper
{
    public class UserGuidesRestClient: IUserGuidesRestClient
    {
        private readonly RestClient _client;
        private readonly String _url = ConfigurationManager.AppSettings["webapibaseurl"];
        public UserGuidesRestClient()
        {
            _client = new RestClient { BaseUrl = new System.Uri(_url) };
        }
        public string  GetReleaseNotes()
        {
            var request = new RestRequest("api/UserGuides/GetReleaseNotes", Method.GET) { RequestFormat = DataFormat.Json };
            var response = _client.Execute<UserGuidesViewModel>(request);
            //string source = response.Content;
            //dynamic data = JsonConvert.DeserializeObject(source);
            //string xx = data.Message;
            if(response.Data!=null)
            return response.Data.GkvValue;
            return null;
        }
        


    }
    interface IUserGuidesRestClient
    {
        string GetReleaseNotes();
    }
}