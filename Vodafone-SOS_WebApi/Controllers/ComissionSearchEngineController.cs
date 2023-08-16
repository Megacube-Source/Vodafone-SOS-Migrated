using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;

namespace Vodafone_SOS_WebApi.Controllers
{
    public class ComissionSearchEngineController : ApiController
    {
        [System.Web.Http.HttpPost]
        public IHttpActionResult GetSummaryCount(SearchEngineViewModel model)
        {

            var tb = new DataTable();
            string Query = "Exec [dbo].[USPGetSearchEngineData] @ChkSubscriberNumber, @SubscriberNumber, @ChkCustomerSegment,@CustomerSegment, @ChkActivityType, @ActivityType, @ChkActivationOrder, @ActivationOrder, @ChkCommType, @CommissionType, @ChkChannel, @Channel, @ChkParentPayee, @PayeeParent, @ChkSubChannel, @SubChannel, @ChkPayee, @Payees, @ChkPeriod, @Period, @ChkBatchStatus, @BatchStatus,@CountryID, @sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,@SelectedTab,@LoggedInUserId";
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@ChkSubscriberNumber",model.ChkSubscriberNumber);
            cmd.Parameters.AddWithValue("@SubscriberNumber", string.IsNullOrEmpty(model.SubscriberNumber) ? (object)System.DBNull.Value : (object)model.SubscriberNumber);

            cmd.Parameters.AddWithValue("@ChkCustomerSegment", Convert.ToBoolean(model.ChkCustomerSegment) );
            cmd.Parameters.AddWithValue("@CustomerSegment", string.IsNullOrEmpty(model.CustomerSegment) ? (object)System.DBNull.Value : (object)model.CustomerSegment);

            cmd.Parameters.AddWithValue("@ChkActivityType", Convert.ToBoolean(model.ChkActivityType));
            cmd.Parameters.AddWithValue("@ActivityType", string.IsNullOrEmpty(model.ActivityType) ? (object)System.DBNull.Value : (object)model.ActivityType);

            cmd.Parameters.AddWithValue("@ChkActivationOrder", Convert.ToBoolean(model.ChkActivationOrder));
            cmd.Parameters.AddWithValue("@ActivationOrder", string.IsNullOrEmpty(model.ActivationOrder) ? (object)System.DBNull.Value : (object)model.ActivationOrder);

            cmd.Parameters.AddWithValue("@ChkCommType", Convert.ToBoolean(model.ChkCommType)) ;
            cmd.Parameters.AddWithValue("@CommissionType", string.IsNullOrEmpty(model.CommissionType) ? (object)System.DBNull.Value : (object)model.CommissionType);

            cmd.Parameters.AddWithValue("@ChkChannel", Convert.ToBoolean(model.ChkChannel) );
            cmd.Parameters.AddWithValue("@Channel", string.IsNullOrEmpty(model.Channel) ? (object)System.DBNull.Value : (object)model.Channel);

            cmd.Parameters.AddWithValue("@ChkParentPayee", Convert.ToBoolean(model.ChkParentPayee));
            cmd.Parameters.AddWithValue("@PayeeParent", string.IsNullOrEmpty(model.PayeeParent) ? (object)System.DBNull.Value : (object)model.PayeeParent);

            cmd.Parameters.AddWithValue("@ChkSubChannel", Convert.ToBoolean(model.ChkSubChannel));
            cmd.Parameters.AddWithValue("@SubChannel", string.IsNullOrEmpty(model.SubChannel) ? (object)System.DBNull.Value : (object)model.SubChannel);

            cmd.Parameters.AddWithValue("@ChkPayee", Convert.ToBoolean(model.ChkPayee));
            cmd.Parameters.AddWithValue("@Payees", string.IsNullOrEmpty(model.Payees) ? (object)System.DBNull.Value : (object)model.Payees);

            cmd.Parameters.AddWithValue("@ChkPeriod", Convert.ToBoolean(model.ChkPeriod));
            cmd.Parameters.AddWithValue("@Period", string.IsNullOrEmpty(model.Period) ? (object)System.DBNull.Value : (object)model.Period);

            cmd.Parameters.AddWithValue("@ChkBatchStatus", Convert.ToBoolean(model.ChkBatchStatus));
            cmd.Parameters.AddWithValue("@BatchStatus", string.IsNullOrEmpty(model.BatchStatus) ? (object)System.DBNull.Value : (object)model.BatchStatus);

            cmd.Parameters.AddWithValue("@CountryID", model.CountryID);
           // cmd.Parameters.AddWithValue("@UserName", string.IsNullOrEmpty(model.UserName) ? (object)System.DBNull.Value : (object)model.UserName);
            //cmd.Parameters.AddWithValue("@Workflow", string.IsNullOrEmpty(model.Workflow) ? (object)System.DBNull.Value : (object)model.Workflow);
            //cmd.Parameters.AddWithValue("@LoggedInUserId", string.IsNullOrEmpty(model.LoggedInUserId) ? (object)System.DBNull.Value : (object)model.LoggedInUserId);
            //cmd.Parameters.AddWithValue("@UserRole", string.IsNullOrEmpty(model.UserRole) ? (object)System.DBNull.Value : (object)model.UserRole);
            cmd.Parameters.AddWithValue("@sortdatafield", (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@sortorder", (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@pagesize",  999999);
            cmd.Parameters.AddWithValue("@pagenum", 0);
            cmd.Parameters.AddWithValue("@FilterQuery", (object)System.DBNull.Value );
            cmd.Parameters.AddWithValue("@SelectedTab", model.SelectedTab);
            cmd.Parameters.AddWithValue("@LoggedInUserId", model.LoggedInUserId);


            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            //The Ado.Net code ends here
            return Ok(tb.Rows.Count);

            
        }


        [System.Web.Http.HttpPost]
        public IHttpActionResult GetSummary(SearchEngineViewModel model, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {

            var tb = new DataTable();
            string Query = "Exec [dbo].[USPGetSearchEngineData] @ChkSubscriberNumber, @SubscriberNumber, @ChkCustomerSegment,@CustomerSegment, @ChkActivityType, @ActivityType, @ChkActivationOrder, @ActivationOrder, @ChkCommType, @CommissionType, @ChkChannel, @Channel, @ChkParentPayee, @PayeeParent, @ChkSubChannel, @SubChannel, @ChkPayee, @Payees, @ChkPeriod, @Period, @ChkBatchStatus, @BatchStatus,@CountryID, @sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,@SelectedTab,@LoggedInUserId";
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@ChkSubscriberNumber", Convert.ToBoolean(model.ChkSubscriberNumber));
            cmd.Parameters.AddWithValue("@SubscriberNumber", string.IsNullOrEmpty(model.SubscriberNumber) ? (object)System.DBNull.Value : (object)model.SubscriberNumber);

            cmd.Parameters.AddWithValue("@ChkCustomerSegment", Convert.ToBoolean(model.ChkCustomerSegment) );
            cmd.Parameters.AddWithValue("@CustomerSegment", string.IsNullOrEmpty(model.CustomerSegment) ? (object)System.DBNull.Value : (object)model.CustomerSegment);

            cmd.Parameters.AddWithValue("@ChkActivityType", Convert.ToBoolean(model.ChkActivityType) );
            cmd.Parameters.AddWithValue("@ActivityType", string.IsNullOrEmpty(model.ActivityType) ? (object)System.DBNull.Value : (object)model.ActivityType);

            cmd.Parameters.AddWithValue("@ChkActivationOrder", Convert.ToBoolean(model.ChkActivationOrder) );
            cmd.Parameters.AddWithValue("@ActivationOrder", string.IsNullOrEmpty(model.ActivationOrder) ? (object)System.DBNull.Value : (object)model.ActivationOrder);

            cmd.Parameters.AddWithValue("@ChkCommType", Convert.ToBoolean(model.ChkCommType));
            cmd.Parameters.AddWithValue("@CommissionType", string.IsNullOrEmpty(model.CommissionType) ? (object)System.DBNull.Value : (object)model.CommissionType);

            cmd.Parameters.AddWithValue("@ChkChannel", Convert.ToBoolean(model.ChkChannel) );
            cmd.Parameters.AddWithValue("@Channel", string.IsNullOrEmpty(model.Channel) ? (object)System.DBNull.Value : (object)model.Channel);

            cmd.Parameters.AddWithValue("@ChkParentPayee", Convert.ToBoolean(model.ChkParentPayee) );
            cmd.Parameters.AddWithValue("@PayeeParent", string.IsNullOrEmpty(model.PayeeParent) ? (object)System.DBNull.Value : (object)model.PayeeParent);

            cmd.Parameters.AddWithValue("@ChkSubChannel", Convert.ToBoolean(model.ChkSubChannel));
            cmd.Parameters.AddWithValue("@SubChannel", string.IsNullOrEmpty(model.SubChannel) ? (object)System.DBNull.Value : (object)model.SubChannel);

            cmd.Parameters.AddWithValue("@ChkPayee", Convert.ToBoolean(model.ChkPayee) );
            cmd.Parameters.AddWithValue("@Payees", string.IsNullOrEmpty(model.Payees) ? (object)System.DBNull.Value : (object)model.Payees);

            cmd.Parameters.AddWithValue("@ChkPeriod", Convert.ToBoolean(model.ChkPeriod));
            cmd.Parameters.AddWithValue("@Period", string.IsNullOrEmpty(model.Period) ? (object)System.DBNull.Value : (object)model.Period);

            cmd.Parameters.AddWithValue("@ChkBatchStatus", Convert.ToBoolean(model.ChkBatchStatus));
            cmd.Parameters.AddWithValue("@BatchStatus", string.IsNullOrEmpty(model.BatchStatus) ? (object)System.DBNull.Value : (object)model.BatchStatus);

            cmd.Parameters.AddWithValue("@CountryID", model.CountryID);
            //cmd.Parameters.AddWithValue("@UserName", string.IsNullOrEmpty(model.UserName) ? (object)System.DBNull.Value : (object)model.UserName);
            //cmd.Parameters.AddWithValue("@Workflow", string.IsNullOrEmpty(model.Workflow) ? (object)System.DBNull.Value : (object)model.Workflow);
            //cmd.Parameters.AddWithValue("@LoggedInUserId", string.IsNullOrEmpty(model.LoggedInUserId) ? (object)System.DBNull.Value : (object)model.LoggedInUserId);
            //cmd.Parameters.AddWithValue("@UserRole", string.IsNullOrEmpty(model.UserRole) ? (object)System.DBNull.Value : (object)model.UserRole);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : (object)sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : (object)sortorder);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : (object)FilterQuery);
            cmd.Parameters.AddWithValue("@SelectedTab", model.SelectedTab);
            cmd.Parameters.AddWithValue("@LoggedInUserId", model.LoggedInUserId);

            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            //The Ado.Net code ends here
            return Ok(tb);


        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult DownLoadFiles(SearchEngineViewModel model, string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {

            var tb = new DataTable();
            string Query = "Exec [dbo].[USPGetSearchEngineData] @ChkSubscriberNumber, @SubscriberNumber, @ChkCustomerSegment,@CustomerSegment, @ChkActivityType, @ActivityType, @ChkActivationOrder, @ActivationOrder, @ChkCommType, @CommissionType, @ChkChannel, @Channel, @ChkParentPayee, @PayeeParent, @ChkSubChannel, @SubChannel, @ChkPayee, @Payees, @ChkPeriod, @Period, @ChkBatchStatus, @BatchStatus,@CountryID, @sortdatafield,@sortorder,@pagesize,@pagenum,@FilterQuery,@SelectedTab,@LoggedInUserId";
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@ChkSubscriberNumber", Convert.ToBoolean(model.ChkSubscriberNumber));
            cmd.Parameters.AddWithValue("@SubscriberNumber", string.IsNullOrEmpty(model.SubscriberNumber) ? (object)System.DBNull.Value : (object)model.SubscriberNumber);

            cmd.Parameters.AddWithValue("@ChkCustomerSegment", Convert.ToBoolean(model.ChkCustomerSegment));
            cmd.Parameters.AddWithValue("@CustomerSegment", string.IsNullOrEmpty(model.CustomerSegment) ? (object)System.DBNull.Value : (object)model.CustomerSegment);

            cmd.Parameters.AddWithValue("@ChkActivityType", Convert.ToBoolean(model.ChkActivityType));
            cmd.Parameters.AddWithValue("@ActivityType", string.IsNullOrEmpty(model.ActivityType) ? (object)System.DBNull.Value : (object)model.ActivityType);

            cmd.Parameters.AddWithValue("@ChkActivationOrder", Convert.ToBoolean(model.ChkActivationOrder));
            cmd.Parameters.AddWithValue("@ActivationOrder", string.IsNullOrEmpty(model.ActivationOrder) ? (object)System.DBNull.Value : (object)model.ActivationOrder);

            cmd.Parameters.AddWithValue("@ChkCommType", Convert.ToBoolean(model.ChkCommType));
            cmd.Parameters.AddWithValue("@CommissionType", string.IsNullOrEmpty(model.CommissionType) ? (object)System.DBNull.Value : (object)model.CommissionType);

            cmd.Parameters.AddWithValue("@ChkChannel", Convert.ToBoolean(model.ChkChannel));
            cmd.Parameters.AddWithValue("@Channel", string.IsNullOrEmpty(model.Channel) ? (object)System.DBNull.Value : (object)model.Channel);

            cmd.Parameters.AddWithValue("@ChkParentPayee", Convert.ToBoolean(model.ChkParentPayee));
            cmd.Parameters.AddWithValue("@PayeeParent", string.IsNullOrEmpty(model.PayeeParent) ? (object)System.DBNull.Value : (object)model.PayeeParent);

            cmd.Parameters.AddWithValue("@ChkSubChannel", Convert.ToBoolean(model.ChkSubChannel));
            cmd.Parameters.AddWithValue("@SubChannel", string.IsNullOrEmpty(model.SubChannel) ? (object)System.DBNull.Value : (object)model.SubChannel);

            cmd.Parameters.AddWithValue("@ChkPayee", Convert.ToBoolean(model.ChkPayee));
            cmd.Parameters.AddWithValue("@Payees", string.IsNullOrEmpty(model.Payees) ? (object)System.DBNull.Value : (object)model.Payees);

            cmd.Parameters.AddWithValue("@ChkPeriod", Convert.ToBoolean(model.ChkPeriod));
            cmd.Parameters.AddWithValue("@Period", string.IsNullOrEmpty(model.Period) ? (object)System.DBNull.Value : (object)model.Period);

            cmd.Parameters.AddWithValue("@ChkBatchStatus", Convert.ToBoolean(model.ChkBatchStatus));
            cmd.Parameters.AddWithValue("@BatchStatus", string.IsNullOrEmpty(model.BatchStatus) ? (object)System.DBNull.Value : (object)model.BatchStatus);

            cmd.Parameters.AddWithValue("@CountryID", model.CountryID);
            //cmd.Parameters.AddWithValue("@UserName", string.IsNullOrEmpty(model.UserName) ? (object)System.DBNull.Value : (object)model.UserName);
            //cmd.Parameters.AddWithValue("@Workflow", string.IsNullOrEmpty(model.Workflow) ? (object)System.DBNull.Value : (object)model.Workflow);
            //cmd.Parameters.AddWithValue("@LoggedInUserId", string.IsNullOrEmpty(model.LoggedInUserId) ? (object)System.DBNull.Value : (object)model.LoggedInUserId);
            //cmd.Parameters.AddWithValue("@UserRole", string.IsNullOrEmpty(model.UserRole) ? (object)System.DBNull.Value : (object)model.UserRole);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : (object)sortdatafield);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : (object)sortorder);
            cmd.Parameters.AddWithValue("@pagesize", pagesize);
            cmd.Parameters.AddWithValue("@pagenum", pagenum);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : (object)FilterQuery);
            cmd.Parameters.AddWithValue("@SelectedTab", model.SelectedTab);
            cmd.Parameters.AddWithValue("@LoggedInUserId", model.LoggedInUserId);

            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            //The Ado.Net code ends here
            var FileName = string.Empty;
            if (model.SelectedTab == "Claims")
            {
                 FileName = "ExportClaimsCommissionSearch";
            }
            else
            {
                 FileName = "ExportCalculationCommissionSearch";
            }
           // Globals.ExportZipFromDataTable(null, model.CompanyCode, model.LoggedinUserName, FileName, tb);
            Globals.ExportSearchEngineFromDataTable(null, model.CompanyCode, model.LoggedinUserName, FileName, tb);
            
            return Ok(FileName+ ".xlsx");


        }

    }
}