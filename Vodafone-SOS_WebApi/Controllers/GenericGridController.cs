using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
using System.Globalization;
using System.Data.Entity.Validation;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Reflection.Emit;
using System.Reflection;
using System.Configuration;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Amazon.S3;
using Amazon.S3.Model;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class GenericGridController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
        public GenericGridController()
        {
        }
        public GenericGridController(ApplicationUserManager userManager,
          ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private TypeBuilder CreateTypeBuilder(string assemblyName, string moduleName, string typeName)
        {
            TypeBuilder typeBuilder = AppDomain
                .CurrentDomain
                .DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName)
                .DefineType(typeName, TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }
        private void CreateAutoImplementedProperty(TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(string.Concat(PrivateFieldPrefix, propertyName), propertyType, FieldAttributes.Private);

            // Generate the property
            PropertyBuilder propertyBuilder = builder.DefineProperty(propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);

            // Property getter and setter attributes.
            MethodAttributes propertyMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(string.Concat(GetterPrefix, propertyName), propertyMethodAttributes, propertyType, Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(string.Concat(SetterPrefix, propertyName), propertyMethodAttributes, null, new Type[] { propertyType });

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            ILGenerator setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }
        [HttpGet]
        public IHttpActionResult WorkflowGridCounts(string LoggedInRoleId, string LoggedInUserId, string WorkflowName, int CompanyId, string PortfolioList)
        {

            //The below lines of code converts the data returned from api to a datatable
            var tb = new DataTable();
            string Query = "Exec [dbo].[SPGetGenericGridCounts] @LoggedInRoleId , @LoggedInUserId , @WorkflowName , @CompanyId, @PortfolioList";
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@LoggedInRoleId", string.IsNullOrEmpty(LoggedInRoleId) ? (object)System.DBNull.Value : (object)LoggedInRoleId);
            cmd.Parameters.AddWithValue("@LoggedInUserId", string.IsNullOrEmpty(LoggedInUserId) ? (object)System.DBNull.Value : (object)LoggedInUserId);
            cmd.Parameters.AddWithValue("@WorkflowName", string.IsNullOrEmpty(WorkflowName) ? (object)System.DBNull.Value : (object)WorkflowName);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@PortfolioList", string.IsNullOrEmpty(PortfolioList) ? (object)System.DBNull.Value : (object)PortfolioList);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            //The Ado.Net code ends here
            return Ok(tb);

        }
        //NOTE Apply all filters and Get Counts of Generic Grid Data and return to webApp
        public IHttpActionResult GetGenericGridDataCounts(int WorkflowConfigId, string LoggedInRoleId, string LoggedInUserId, string WorkflowName, int CompanyId, string TabName, string PortfolioList)
        {

            //GetWorkflow details
            var WFDetails = db.RWorkFlows.Where(p => p.RwfName == WorkflowName).FirstOrDefault();
            var LoginWFConfig = db.LWorkFlowConfigs.Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.LwfcWorkFlowId == WFDetails.Id).FirstOrDefault();
            //get current work flow
            var WorkflowConfig = db.LWorkFlowConfigs.Find(WorkflowConfigId);

            ////Get LPayee's Id for AspnetUserId of Logged In User
            //var Payee = db.LPayees.Where(p => p.LpUserId == LoggedInUserId).FirstOrDefault();
            //String PayeeStr = "";
            //if (Payee != null)
            //{
            //    //SS As per new Logic
            //    //In all tabs for all WFs (including Active, Finished tab), payee can see only those transactions where Requestor = him or his children
            //    var HierarchyPayee = db.Database.SqlQuery<string>("WITH parent AS (select LppParentPayeeId from LPayeeParents where LppParentPayeeId=" + Payee.Id + "), tree AS (SELECT x.LppParentPayeeId, x.LppPayeeId FROM LPayeeParents x INNER JOIN parent ON x.LppParentPayeeId = parent.LppParentPayeeId UNION ALL SELECT y.LppParentPayeeId, y.LppPayeeId FROM LPayeeParents y INNER JOIN tree t ON y.LppParentPayeeId = t.LppPayeeId ) SELECT AU1.Id FROM tree t inner join LPayees parent on t.LppParentPayeeId=parent.Id inner join AspNetUsers AU1 on AU1.Id=parent.LpUserId where parent.WFStatus='Completed' union select AU2.Id FROM tree t inner join LPayees child on t.LppPayeeId=child.Id inner join AspNetUsers AU2 on  AU2.Id=child.LpUserId where child.WFStatus='Completed' Union select Au3.Id FROM LPayees child inner join AspNetUsers AU3 on AU3.Id=child.LpUserId where WFStatus='Completed' and child.Id=" + Payee.Id).ToList();
            //    foreach (var Pye in HierarchyPayee)
            //    {
            //        PayeeStr += "'" + Pye + "',";
            //    }
            //    PayeeStr = PayeeStr.TrimEnd(',');
            //}

            ////Using Column list to return data from api. 
            //string Query = "select count(*) from " + WFDetails.RwfBaseTableName + " BT ";

            //if (TabName == "Finished")//For getting data for Finished Tab
            //{
            //    Query += " where WFStatus not in ('Saved', 'InProgress','Prelim')  AND ";
            //    //Logged In User could either be Payee or Normal User(Non Payee) . Since Payee Information is held under LPayees ,hence its Userid and Portfolios has to be acquired based on LPayees
            //    var CurrentRole = db.AspNetRoles.Find(LoggedInRoleId).Name;
            //    if (CurrentRole.Equals("Payee", StringComparison.OrdinalIgnoreCase))//WARNING : PAYEE ROLE HARDCODED - Make sure payees are created in all opcos using "Payee" role only . It cannot be Payee_qa
            //    {
            //        Query += "  BT.WFRequesterId in (" + PayeeStr + ") ";
            //        //Get LPayee's Id for AspnetUserId of Logged In User
            //        //var PayeeId = db.LPayees.Where(p => p.LpUserId == LoggedInUserId).FirstOrDefault().Id;
            //        ////Get the list of all portfolios which belongs to the logged in Payee
            //        //PortfolioQuery = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == PayeeId).Where(p => p.MepEntityType == "LPayees").Where(p=>p.MepRoleId==LoggedInRoleId).Select(p => p.MepPortfolioId));
            //    }
            //    else
            //    {
            //        //Get LUser's Id for AspnetUserId of Logged In User
            //        var UserId = db.LUsers.Where(p => p.LuUserId == LoggedInUserId).FirstOrDefault().Id;
            //        //Get the list of all portfolios which belongs to the logged in user
            //        //PortfolioQuery = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == UserId).Where(p => p.MepEntityType == "LUsers").Where(p => p.MepRoleId == LoggedInRoleId).Select(p => p.MepPortfolioId));
            //        //Get the List of All Entity Items which matchs with logged in user's portfolios
            //        //var PortfolioEntityList = " select MepEntityId from MEntityPortfolios where MepEntityType='" + WFDetails.RwfBaseTableName + "' and MepPortfolioId in (" + PortfolioQuery + ")";
            //        //SubQuery to get EntityIdList
            //        var PortfolioEntityList = " select MepEntityId from MEntityPortfolios where MepEntityType='" + WFDetails.RwfBaseTableName + "' and MepPortfolioId in (select MepPortfolioId from MEntityPortfolios where MepEntityType = 'LUsers' and MepRoleId = " + LoggedInRoleId + " and MepEntityId=" + UserId + ") ";

            //        //Get the data for Grid
            //        Query += "(BT.Id in (" + PortfolioEntityList + ") OR BT.WFAnalystId='" + LoggedInUserId + "' OR BT.WFManagerId='" + LoggedInUserId + "' OR BT.WFRequesterId  = ('" + LoggedInUserId + "') OR  (BT.WFCurrentOwnerId is null and BT.WFCompanyId=" + CompanyId + " )) ";
            //    }

            //}
            //else if (TabName == "Active")//display all avtive entry in workflow
            //{
            //    //Where clause of this query will depend upon whether the data is to be filtered for the logged in user or other users. 
            //    Query += " where WFStatus in ('Saved', 'InProgress','Prelim') AND";

            //    //Logged In User could either be Payee or Normal User(Non Payee) . Since Payee Information is held under LPayees ,hence its Userid and Portfolios has to be acquired based on LPayees
            //    var CurrentRole = db.AspNetRoles.Find(LoggedInRoleId).Name;
            //    if (CurrentRole.Equals("Payee", StringComparison.OrdinalIgnoreCase))//WARNING : PAYEE ROLE HARDCODED - Make sure payees are created in all opcos using "Payee" role only . It cannot be Payee_qa
            //    {
            //        Query += "  BT.WFRequesterId in (" + PayeeStr + ") ";
            //        //Get LPayee's Id for AspnetUserId of Logged In User
            //        //var PayeeId = db.LPayees.Where(p => p.LpUserId == LoggedInUserId).FirstOrDefault().Id;
            //        ////Get the list of all portfolios which belongs to the logged in Payee
            //        //PortfolioQuery = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == PayeeId).Where(p => p.MepEntityType == "LPayees").Where(p=>p.MepRoleId==LoggedInRoleId).Select(p => p.MepPortfolioId));
            //    }
            //    else
            //    {
            //        //Get LUser's Id for AspnetUserId of Logged In User
            //        var UserId = db.LUsers.Where(p => p.LuUserId == LoggedInUserId).FirstOrDefault().Id;
            //        //Get the list of all portfolios which belongs to the logged in user
            //        // PortfolioQuery = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == UserId).Where(p => p.MepEntityType == "LUsers").Where(p => p.MepRoleId == LoggedInRoleId).Select(p => p.MepPortfolioId));
            //        //Get the List of All Entity Items which matchs with logged in user's portfolios 
            //        //var PortfolioEntityList = " select MepEntityId from MEntityPortfolios where MepEntityType='" + WFDetails.RwfBaseTableName + "' and MepPortfolioId in (" + PortfolioQuery + ")";
            //        //SubQuery to get EntityIdList
            //        var PortfolioEntityList = " select MepEntityId from MEntityPortfolios where MepEntityType='" + WFDetails.RwfBaseTableName + "' and MepPortfolioId in (select MepPortfolioId from MEntityPortfolios where MepEntityType = 'LUsers' and MepRoleId = " + LoggedInRoleId + " and MepEntityId=" + UserId + ") ";

            //        //Get the data for Grid
            //        Query += "(BT.Id in (" + PortfolioEntityList + ") OR BT.WFAnalystId='" + LoggedInUserId + "' OR BT.WFManagerId='" + LoggedInUserId + "' OR BT.WFRequesterId  = ('" + LoggedInUserId + "') OR (BT.WFCurrentOwnerId is null and BT.WFCompanyId=" + CompanyId + ")) ";
            //    }


            //}
            //else
            //{
            //    //Where clause of this query will depend upon whether the data is to be filtered for the logged in user or other users. 
            //    //For Logged in users the data will be filtered as (1. Where current owner is logged in user. 2. Created by Logged in user)
            //    Query += " where WFStatus in ('Saved', 'InProgress','Prelim') AND BT.WFOrdinal=(select LwfcOrdinalNumber from LWorkflowConfig where Id=" + WorkflowConfigId + ") AND";
            //    //SS The code for my tab has been commented as directed by JS.As part of R1.5 in my tab we will show all records where Portfolio match like other tabs
            //    //In his Tab
            //    if (LoggedInRoleId.Equals(db.LWorkFlowConfigs.Find(WorkflowConfigId).LwfcRoleId, StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        Query += "((BT.WFCurrentOwnerId = '" + LoggedInUserId + "' OR BT.WFRequesterId = '" + LoggedInUserId + "') OR (BT.WFCurrentOwnerId is null and BT.WFCompanyId=" + CompanyId + " and BT.WFOrdinal=" + LoginWFConfig.LwfcOrdinalNumber + ")) ";
            //    }
            //    //In Other tabs
            //    else//display entity data  whose portfolio matches logged in user's portfolio(s)   
            //    {
            //        //Logged In User could either be Payee or Normal User(Non Payee) . Since Payee Information is held under LPayees ,hence its Userid and Portfolios has to be acquired based on LPayees
            //        var CurrentRole = db.AspNetRoles.Find(LoggedInRoleId).Name;
            //        if (CurrentRole.Equals("Payee", StringComparison.OrdinalIgnoreCase))//WARNING : PAYEE ROLE HARDCODED - Make sure payees are created in all opcos using "Payee" role only . It cannot be Payee_qa
            //        {
            //            Query += "  BT.WFRequesterId in (" + PayeeStr + ") ";
            //            //Get LPayee's Id for AspnetUserId of Logged In User
            //            //var PayeeId = db.LPayees.Where(p => p.LpUserId == LoggedInUserId).FirstOrDefault().Id;
            //            ////Get the list of all portfolios which belongs to the logged in Payee
            //            //PortfolioQuery = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == PayeeId).Where(p => p.MepEntityType == "LPayees").Where(p=>p.MepRoleId==LoggedInRoleId).Select(p => p.MepPortfolioId));
            //        }
            //        else
            //        {
            //            //Get LUser's Id for AspnetUserId of Logged In User
            //            var UserId = db.LUsers.Where(p => p.LuUserId == LoggedInUserId).FirstOrDefault().Id;
            //            //Get the list of all portfolios which belongs to the logged in user
            //            // PortfolioQuery = string.Join(",", db.MEntityPortfolios.Where(p => p.MepEntityId == UserId).Where(p => p.MepEntityType == "LUsers").Where(p => p.MepRoleId == LoggedInRoleId).Select(p => p.MepPortfolioId));
            //            //Get the List of All Entity Items which matchs with logged in user's portfolios 
            //            //var PortfolioEntityList = " select MepEntityId from MEntityPortfolios where MepEntityType='" + WFDetails.RwfBaseTableName + "' and MepPortfolioId in (" + PortfolioQuery + ")";
            //            //SubQuery to get EntityIdList
            //            var PortfolioEntityList = " select MepEntityId from MEntityPortfolios where MepEntityType='" + WFDetails.RwfBaseTableName + "'  and MepPortfolioId in (select MepPortfolioId from MEntityPortfolios where MepEntityType = 'LUsers' and MepRoleId = " + LoggedInRoleId + " and MepEntityId=" + UserId + ") ";
            //            //Get the data for Grid
            //            Query += "(BT.Id in (" + PortfolioEntityList + ") OR BT.WFAnalystId='" + LoggedInUserId + "' OR BT.WFManagerId='" + LoggedInUserId + "' OR BT.WFRequesterId  = ('" + LoggedInUserId + "') OR (BT.WFCurrentOwnerId is null and BT.WFCompanyId=" + CompanyId + "and BT.WFOrdinal=" + WorkflowConfig.LwfcOrdinalNumber + " )) ";
            //        }


            //    }
            //}
            ////If a user applies portfolio filter from UI the records needs to be restriced then
            //if (!string.IsNullOrEmpty(PortfolioList))
            //{
            //    Query += " and BT.Id in (select MepEntityId from MEntityPortfolios where MepEntityType='" + WFDetails.RwfBaseTableName + "' and MepPortfolioId in (SELECT CAST(Item AS INTEGER) FROM dbo.SplitString( '" + PortfolioList + "', ',')))";

            //}
            ////The below line checks that the Workflow Type of Base table should be equal to WfType of current workflow in RWorkflows 
            //Query += "  AND BT.WFType='" + WFDetails.RwfWFType + "'";

            ////Using the column list obtained above, and other parameters passed in the method create a SQL query to fetch the data from database
            ////Execute the query and return the result 
            //int xx = db.Database.SqlQuery<int>(Query).FirstOrDefault();

            //The below lines of code converts the data returned from api to a datatable
            var tb = new DataTable();
            string Query = "Exec [dbo].[SPGetGenericGridData] @WorkflowConfigId , @LoggedInRoleId , @LoggedInUserId , @WorkflowName , @CompanyId, @PageSize , @PageNumber, @sortdatafield, @sortorder,@FilterQuery,@TabName, @PortfolioList";
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@WorkflowConfigId", WorkflowConfigId);
            cmd.Parameters.AddWithValue("@LoggedInRoleId", string.IsNullOrEmpty(LoggedInRoleId) ? (object)System.DBNull.Value : (object)LoggedInRoleId);
            cmd.Parameters.AddWithValue("@LoggedInUserId", string.IsNullOrEmpty(LoggedInUserId) ? (object)System.DBNull.Value : (object)LoggedInUserId);
            cmd.Parameters.AddWithValue("@WorkflowName", string.IsNullOrEmpty(WorkflowName) ? (object)System.DBNull.Value : (object)WorkflowName);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@PageSize", 999999);
            cmd.Parameters.AddWithValue("@PageNumber", 0);
            cmd.Parameters.AddWithValue("@sortorder", (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@sortdatafield", (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@FilterQuery", (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@TabName", string.IsNullOrEmpty(TabName) ? (object)System.DBNull.Value : (object)TabName);
            cmd.Parameters.AddWithValue("@PortfolioList", string.IsNullOrEmpty(PortfolioList) ? (object)System.DBNull.Value : (object)PortfolioList);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            //The Ado.Net code ends here
            return Ok(tb.Rows.Count);

        }

        ////Method to get generic grid data in Grid
        //Commented by SG- R2.3public IHttpActionResult GetGenericGridData(int WorkflowConfigId, string LoggedInRoleId, string LoggedInUserId, string WorkflowName, int CompanyId, int PageSize, int PageNumber, string sortdatafield, string sortorder, string FilterQuery, string TabName, string PortfolioList)
        [HttpPost]
        public IHttpActionResult GetGenericGridData(GenericGridRequestData data)  /*GenericGridRequestData data*/
        {
            //Added by SG- R2.3
            int WorkflowConfigId = data.WorkflowConfigId;
            string LoggedInRoleId = data.LoggedInRoleId;
            string LoggedInUserId = data.LoggedInUserId;
            string WorkflowName = data.WorkflowName;
            int CompanyId = data.CompanyId;
            int PageSize = data.PageSize;
            int PageNumber = data.PageNumber;
            string sortdatafield = data.sortdatafield;
            string sortorder = data.sortorder;
            string FilterQuery = data.FilterQuery;
            string TabName = data.TabName;
            string PortfolioList = data.PortfolioList;
            // END by SG - R2.3



            //    //GetWorkflow details
            var WFDetails = db.RWorkFlows.Where(p => p.RwfName == WorkflowName).FirstOrDefault();
            var LoginWFConfig = db.LWorkFlowConfigs.Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.LwfcWorkFlowId == WFDetails.Id).FirstOrDefault();
            //    //get current work flow
            var WorkflowConfig = db.LWorkFlowConfigs.Find(WorkflowConfigId);//db.LWorkFlowConfigs.Where(p => p.RWorkFlow.RwfName == WorkflowName).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).FirstOrDefault();

            //Using the column list obtained above, and other parameters passed in the method create a SQL query to fetch the data from database
            //Execute the query and return the result 
            //dynamic xx = db.Database.SqlQuery(resultType, Query);

            //The below lines of code converts the data returned from api to a datatable
            var tb = new DataTable();
            string Query = "Exec [dbo].[SPGetGenericGridData] @WorkflowConfigId , @LoggedInRoleId , @LoggedInUserId , @WorkflowName , @CompanyId, @PageSize , @PageNumber, @sortdatafield, @sortorder,@FilterQuery,@TabName, @PortfolioList";
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@WorkflowConfigId", WorkflowConfigId);
            cmd.Parameters.AddWithValue("@LoggedInRoleId", string.IsNullOrEmpty(LoggedInRoleId) ? (object)System.DBNull.Value : (object)LoggedInRoleId);
            cmd.Parameters.AddWithValue("@LoggedInUserId", string.IsNullOrEmpty(LoggedInUserId) ? (object)System.DBNull.Value : (object)LoggedInUserId);
            cmd.Parameters.AddWithValue("@WorkflowName", string.IsNullOrEmpty(WorkflowName) ? (object)System.DBNull.Value : (object)WorkflowName);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@PageSize", PageSize);
            cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
            cmd.Parameters.AddWithValue("@sortorder", string.IsNullOrEmpty(sortorder) ? (object)System.DBNull.Value : (object)sortorder);
            cmd.Parameters.AddWithValue("@sortdatafield", string.IsNullOrEmpty(sortdatafield) ? (object)System.DBNull.Value : (object)sortdatafield);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : (object)FilterQuery);
            cmd.Parameters.AddWithValue("@TabName", string.IsNullOrEmpty(TabName) ? (object)System.DBNull.Value : (object)TabName);
            cmd.Parameters.AddWithValue("@PortfolioList", string.IsNullOrEmpty(PortfolioList) ? (object)System.DBNull.Value : (object)PortfolioList);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            //The Ado.Net code ends here

            //By pass this step of adding actions if it is finished tab with WFConfigId=0
            if (WorkflowConfigId != 0)
            {
                //convert xx into datatable
                for (var j = 0; j < tb.Rows.Count; j++)
                {
                    //Get Current Login ConfigId based on RoleId
                    var LoginWFConfigId = LoginWFConfig.Id;//db.LWorkFlowConfigs.Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.LwfcWorkFlowId == WorkflowConfig.LwfcWorkFlowId).FirstOrDefault().Id;
                    // 1.Get the action Items for the Tab (eg Review,Dashboard,Download) (Id,Url,Label) by using show in WFConfig Id Concept
                    var ActionItemsList = db.LWorkFlowActionItems.Where(p => p.LwfaiShowInTabWFConfigId == WorkflowConfigId && p.LwfaiLoginWFConfigId == LoginWFConfigId).Select(p => new { p.LwfaiActionItemName, p.Id, p.LwfaiUILabel, p.LwfaiActionURL, p.LwfaiOrdinal, p.LwfaiIsButtonOnForm, p.LwfaiIsButtonOnWfGrid }).OrderBy(p => p.LwfaiOrdinal).ToList();
                    // 2.Loop through action Items and for each item get the list of parameters (List of Parameters : Parameter Name,Parameter Type,Parameter Value)
                    string HtmlTemplate = "";
                    foreach (var ActionItem in ActionItemsList)
                    {

                        var ParameterList = db.LWorkFlowActionParameters.Where(p => p.WFActionItemId == ActionItem.Id).Select(p => new { p.Id, p.ParameterName, p.ParameterValue, p.ParameterValueType }).ToList();
                        //display withdraw and Edit  actions only if requestor Id is equal to logged In user Id or WF
                        if ((ActionItem.LwfaiActionItemName.Equals("Withdraw", StringComparison.OrdinalIgnoreCase) && ((LoggedInUserId.Equals(tb.Rows[j].Field<dynamic>("WFRequesterIdXX")) && LoggedInRoleId.Equals(tb.Rows[j].Field<dynamic>("WFRequesterRoleIdXX"))) || LoggedInUserId.Equals(tb.Rows[j].Field<dynamic>("WFAnalystIdXX")))) || !ActionItem.LwfaiActionItemName.Equals("Withdraw", StringComparison.OrdinalIgnoreCase))
                        {
                            //show edit action only if requester id is equal to logged in user id
                            if ((ActionItem.LwfaiActionItemName.Equals("Edit", StringComparison.OrdinalIgnoreCase) && LoggedInUserId.Equals(tb.Rows[j].Field<dynamic>("WFRequesterIdXX"))) || !ActionItem.LwfaiActionItemName.Equals("Edit", StringComparison.OrdinalIgnoreCase))
                            {
                                //show Prelim Action only if Wf Status is not equals to Prelim loggedinuserid=currentownerId
                                if ((ActionItem.LwfaiActionItemName.Equals("Prelim", StringComparison.OrdinalIgnoreCase) && !tb.Rows[j].Field<dynamic>("WFStatusXX").Equals("Prelim", StringComparison.OrdinalIgnoreCase) && LoggedInUserId.Equals(tb.Rows[j].Field<dynamic>("WFCurrentOwnerIdXX"))) || !ActionItem.LwfaiActionItemName.Equals("Prelim", StringComparison.OrdinalIgnoreCase))
                                {
                                    //show UnPrelim Action only if Wf Status  equals to Prelim loggedinuserid=currentownerId
                                    if ((ActionItem.LwfaiActionItemName.Equals("UnPrelim", StringComparison.OrdinalIgnoreCase) && tb.Rows[j].Field<dynamic>("WFStatusXX").Equals("Prelim", StringComparison.OrdinalIgnoreCase) && LoggedInUserId.Equals(tb.Rows[j].Field<dynamic>("WFCurrentOwnerIdXX"))) || !ActionItem.LwfaiActionItemName.Equals("UnPrelim", StringComparison.OrdinalIgnoreCase))
                                    {
                                        //Show approve action only when current owner id is equal to the logged in user id
                                        if ((((ActionItem.LwfaiActionItemName.Equals("Approve", StringComparison.OrdinalIgnoreCase) || ActionItem.LwfaiActionItemName.Equals("Reject", StringComparison.OrdinalIgnoreCase)) && LoggedInUserId.Equals(tb.Rows[j].Field<dynamic>("WFCurrentOwnerIdXX")))) || (!ActionItem.LwfaiActionItemName.Equals("Approve", StringComparison.OrdinalIgnoreCase) && !ActionItem.LwfaiActionItemName.Equals("Reject", StringComparison.OrdinalIgnoreCase)))
                                        {
                                            //Show SendToAnalyst action only when current owner id is equal to the logged in user id
                                            if ((((ActionItem.LwfaiActionItemName.Equals("SendToAnalyst", StringComparison.OrdinalIgnoreCase) && LoggedInUserId.Equals(tb.Rows[j].Field<dynamic>("WFCurrentOwnerIdXX")))) || (!ActionItem.LwfaiActionItemName.Equals("SendToAnalyst", StringComparison.OrdinalIgnoreCase))))
                                            {
                                                //Show SendToRequester action only when current owner id is equal to the logged in user id
                                                if ((((ActionItem.LwfaiActionItemName.Equals("SendToRequester", StringComparison.OrdinalIgnoreCase) && LoggedInUserId.Equals(tb.Rows[j].Field<dynamic>("WFCurrentOwnerIdXX")))) || (!ActionItem.LwfaiActionItemName.Equals("SendToRequester", StringComparison.OrdinalIgnoreCase))))
                                                {
                                                    //show self assign action only if currentowner id is null 
                                                    if ((ActionItem.LwfaiActionItemName.Equals("SelfAssign", StringComparison.OrdinalIgnoreCase) 
                                                        && !LoggedInUserId.Equals(tb.Rows[j].Field<dynamic>("WFRequesterIdXX")) //requestor cannot self assign to stop fraudulent approval as manager if we have both roles
                                                        && string.IsNullOrEmpty(tb.Rows[j].Field<dynamic>("WFCurrentOwnerIdXX"))) 
                                                        || !ActionItem.LwfaiActionItemName.Equals("SelfAssign", StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        //show reclaim only if wfstatus=rejected
                                                        if ((ActionItem.LwfaiActionItemName.Equals("ReClaim", StringComparison.OrdinalIgnoreCase) && (tb.Rows[j].Field<string>("WFStatusXX").Equals("Rejected", StringComparison.OrdinalIgnoreCase))) || !ActionItem.LwfaiActionItemName.Equals("ReClaim", StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            //show WashClaim action only if Analyst id is equal to logged in user id
                                                            if ((ActionItem.LwfaiActionItemName.Equals("WashClaim", StringComparison.OrdinalIgnoreCase) && LoggedInUserId.Equals(tb.Rows[j].Field<dynamic>("WFAnalystIdXX"))) || !ActionItem.LwfaiActionItemName.Equals("WashClaim", StringComparison.OrdinalIgnoreCase))
                                                            {
                                                                var UrlString = ActionItem.LwfaiActionURL;
                                                                /*NOTE: ActionName will not be taken from LWorkflowActionParameters bu it will be taken from LWorkflowActionItems(ActionName) as directed by JS*/
                                                                //Passing Extra  Parameter of WFConfigId and TransactionId to display buttons in secondary Forms in all Workflows
                                                                UrlString += "?WFConfigId=" + WorkflowConfig.Id + "&TransactionId=" + tb.Rows[j].Field<dynamic>("Id") + "&ActionName=" + ActionItem.LwfaiActionItemName + "&";
                                                                //3.Loop through List of parameters and create a template Html string
                                                                for (var i = 0; i < ParameterList.Count(); i++)
                                                                {
                                                                    if (ParameterList[i].ParameterValueType.Equals("Static", StringComparison.OrdinalIgnoreCase))
                                                                    {
                                                                        UrlString += ParameterList[i].ParameterName + "=" + ParameterList[i].ParameterValue + "&";
                                                                    }
                                                                    else
                                                                    {
                                                                        UrlString += ParameterList[i].ParameterName + "=" + tb.Rows[j].Field<dynamic>(ParameterList[i].ParameterValue) + "&";//tb.Rows[j].Field<string>(ParameterList[i].ParameterValue)
                                                                    }


                                                                }

                                                                UrlString = UrlString.Substring(0, (UrlString.Length - 1));//Remove the last  character(&) from Url string
                                                                                                                           /*The below string is used to frame the buttons html part in Generic Grid where each action is seperated by ## and Its Url,Label,IsDisplayedonGrid value is seperated by --*/
                                                                HtmlTemplate += UrlString + "--" + ActionItem.LwfaiUILabel + "--" + ActionItem.LwfaiIsButtonOnWfGrid + "--" + ActionItem.LwfaiActionItemName + "##"; //"< a href = '" + UrlString + "' > '" + ActionItem.LwfaiUILabel + "' </a>";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //Update Action Items column with the links obtained from above process
                    tb.Rows[j]["Actions"] = HtmlTemplate;
                }
            }

            return Ok(tb);
        }
        /// Desc: This function tells whether the workflow has reached its end or not
        public bool WfIsLastOrdinal(int WorkFlowId, int TransactionId, int CompanyId)
        {
            //get the Work Flow details
            var WorkFlow = db.RWorkFlows.Find(WorkFlowId);
            // Find max_Ordinal for that WorkflowId, Opco in WFConfig table
            int MaxWFOrdinal = db.LWorkFlowConfigs.Where(p => p.LwfcWorkFlowId == WorkFlowId).Where(p => p.LwfcCompanyId == CompanyId).Max(p => p.LwfcOrdinalNumber);
            // Find current_Ordinal for that BaseTable where Id = TransactionId
            int CurrentOrdinal = db.Database.SqlQuery<int>("select WFOrdinal from " + WorkFlow.RwfBaseTableName + " where Id=" + TransactionId).FirstOrDefault<int>();
            //check if current ordinal is last ordinal
            if (CurrentOrdinal == MaxWFOrdinal)
            {
                return true;
            }
            return false;
        }

        //This method will assign Current Owner to a transaction as per the Parameter passed
        //[HttpGet]
        //public IHttpActionResult SelfAssign(string WorkFlowName, int TransactionId, int CompanyId, string AssigneeId, string AssigneeName, string RoleId)
        //{
        //    //get work flow details based on Workflow name
        //    var WorkFlow = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkFlowName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        //    var WFColumns = db.Database.SqlQuery<WFColumnsViewModel>("select WFOrdinal,WFCurrentOwnerId from " + WorkFlow.RwfBaseTableName + " where Id =" + TransactionId).FirstOrDefault();
        //    if(string.IsNullOrEmpty(WFColumns.WFCurrentOwnerId))
        //    { 
        //    var ConfigData = db.LWorkFlowConfigs.Where(p => p.LwfcWorkFlowId == WorkFlow.Id).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcOrdinalNumber == WFColumns.WFOrdinal).FirstOrDefault();
        //        if (ConfigData.LwfcRoleId == RoleId)//check if loggedIn role is equal to the role of the current config data
        //        {
        //            //update base table record 
        //            switch (ConfigData.LwfcActingAs)
        //            {
        //                case "Analyst":
        //                    db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId='" + AssigneeId + "' ,WFAnalystId='" + AssigneeId + "' where Id=" + TransactionId);
        //                    break;
        //                case "Manager":
        //                    db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId='" + AssigneeId + "'  ,WFManagerId='" + AssigneeId + "' where Id=" + TransactionId);
        //                    break;
        //                //SS NEW CODE
        //                case "Requester":
        //                    db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId='" + AssigneeId + "'  ,WFRequesterId='" + AssigneeId + "',WFRequesterRoleId=" + RoleId + " where Id=" + TransactionId);
        //                    break;
        //                default:
        //                    //update the current owner Id and Name
        //                    db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId='" + AssigneeId + "'  where Id=" + TransactionId);
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Your Role  should be " + ConfigData.LwfcActingAs + " in order to self assign"));
        //        }
        //    }

        //    return Ok();
        //}
        //[HttpGet]
        //public IHttpActionResult AssignTo(string WorkFlowName, int TransactionId, int CompanyId, string AssigneeId, string AssigneeName, string RoleId)
        //{
        //    //get work flow details based on Workflow name
        //    var WorkFlow = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkFlowName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        //    int CurrentOrdinal = db.Database.SqlQuery<int>("select WFOrdinal from " + WorkFlow.RwfBaseTableName + " where Id =" + TransactionId).FirstOrDefault<int>();
        //    var ConfigData = db.LWorkFlowConfigs.Where(p => p.LwfcWorkFlowId == WorkFlow.Id).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcOrdinalNumber == CurrentOrdinal).FirstOrDefault();

        //    //update base table record 
        //    switch (ConfigData.LwfcActingAs)
        //    {
        //        case "Analyst":
        //            db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId='" + AssigneeId + "' ,WFAnalystId='" + AssigneeId + "' where Id=" + TransactionId);
        //            break;
        //        case "Manager":
        //            db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId='" + AssigneeId + "',WFManagerId='" + AssigneeId + "' where Id=" + TransactionId);
        //            break;
        //        //SS NEW CODE
        //        case "Requester":
        //            db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId='" + AssigneeId + "'  ,WFRequesterId='" + AssigneeId + "',WFRequesterRoleId=" + RoleId + " where Id=" + TransactionId);
        //            break;
        //        default:
        //            //update the current owner Id and Name
        //            db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId='" + AssigneeId + "' where Id=" + TransactionId);
        //            break;
        //    }

        //    return Ok();
        //}

        public IHttpActionResult GetAssigneeList(string WorkFlowName, int TransactionId, int CompanyId)
        {//
            //get work flow details based on Workflow name
            var WorkFlow = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkFlowName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var PossibleOwners = WFFindPossibleOwners(WorkFlow.Id, TransactionId, CompanyId, null).ToList();
            var UserList = db.LUsers.Where(p => PossibleOwners.Contains(p.LuUserId)).Select(p => new { FullName = p.LuFirstName + " " + p.LuLastName, Id = p.LuUserId }).ToList();
            return Ok(UserList);
        }

        //This method is called when we update WfSatatus in any Workflow against any transaction
        [HttpPost]
        public async Task<IHttpActionResult> UpdateActionStatus(OtherAPIData objTrans, string Action, string WorkFlowName, int CompanyId, string LoggedInUserId, string Comments, string LoggedInRoleId, string AssigneeId)
        {
            //Currently using two different db context as we were facing errors using a single context after execution of SP
            using (var database = new SOSEDMV10Entities())
            {
                //using (var transaction = db.Database.BeginTransaction())
                //{
                try
                {
                    //Shivani Changes for WIAM integration. 09 May 2019
                    string IsWIAMEnabled = db.Database.SqlQuery<string>("select [dbo].[FNIsWIAMEnabled]({0})",CompanyId).FirstOrDefault();
                    // var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
                    var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
                    string SPresult = Globals.ExecuteSPUpdateActionStatus(Action, WorkFlowName, objTrans.TransactionID, CompanyId, LoggedInUserId, Comments, LoggedInRoleId, ProjectEnviournment, AssigneeId);
                    //  db.SaveChanges();
                    var CanPerformFinalApprovalTasks = false;
                    if (string.IsNullOrEmpty(SPresult))
                    {
                        CanPerformFinalApprovalTasks = true;
                    }
                    else
                    {
                        CanPerformFinalApprovalTasks = true;
                        if (SPresult.Contains("Validation Message"))
                        {
                            CanPerformFinalApprovalTasks = false;
                        }
                    }
                    //After checking Conditions for Final Approval do subsequent action
                    if (CanPerformFinalApprovalTasks)
                    {
                        //get work flow name based on Workflow name
                        if (!string.IsNullOrEmpty(objTrans.TransactionID))
                        {
                            var TranIdList = objTrans.TransactionID.Split(',').ToList();
                            foreach (var TId in TranIdList)
                            {
                                var TranId = Convert.ToInt32(TId);
                                var WorkFlow = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkFlowName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                var Company = db.GCompanies.Find(CompanyId);
                                var WFColumns = db.Database.SqlQuery<WFColumnsViewModel>("select WFRequesterId,WFAnalystId,WFManagerId,WFOrdinal,WFCurrentOwnerId,WFStatus,WFType,WFRequesterRoleId,WFCompanyId,WFComments from " + WorkFlow.RwfBaseTableName + " where Id=" + TranId).FirstOrDefault();
                                switch (Action)
                                {
                                    case "Approve":
                                        //Allow action only if Current Owner Id is equal to LoggedIn UserId and Workflow is completed
                                        if (WFColumns.WFCurrentOwnerId == LoggedInUserId && WFColumns.WFStatus == "Completed")
                                        {
                                            if (WfIsLastOrdinal(WorkFlow.Id, Convert.ToInt32(TranId), CompanyId))
                                            {
                                                switch (WorkFlow.RwfName)
                                                {
                                                    case "Users":
                                                        var User = database.LUsers.Find(TranId);
                                                        var AspUser = database.AspNetUsers.Find(User.LuUserId);
                                                        AspUser.IsActive = true;
                                                        database.Entry(AspUser).State = EntityState.Modified;
                                                        await database.SaveChangesAsync();
                                                        //Creating The forzip folder as well.
                                                        string path = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + Company.GcCode + "/" + User.LuEmail + "/forzip";
                                                        if (!Directory.Exists(path))
                                                        {
                                                            Directory.CreateDirectory(path);
                                                        }

                                                        //******************************************AD changes
                                                        //Random password generator code used for creating AD account ---- commenting as now
                                                        var UserIdentifier = User.LuFirstName + " " + User.LuLastName;
                                                        //when IsWIAMEnabled value is yes, AD Account will not be created again
                                                        if (!string.IsNullOrEmpty(IsWIAMEnabled) && !"yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                                                            //Use Global Method to create AD User created on 09 June 2018
                                                            Globals.CreateUser(User.LuEmail, ProjectEnviournment, CompanyId, UserIdentifier, LoggedInUserId, Comments, null);
                                                        else if (Convert.ToBoolean(User.LuCreateLogin))
                                                        {
                                                            string ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, User.LuEmail).FirstOrDefault();
                                                            var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeEmail2UponApproval").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                                                            if (EmailTemplate != null)
                                                            {
                                                                string EmailBody = EmailTemplate.LetEmailBody;
                                                                EmailBody = EmailBody.Replace("###EmailAddress###", User.LuEmail);
                                                                string EmailSubject = EmailTemplate.LetEmailSubject;
                                                                Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject,EmailBody, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                                                            }
                                                        }
                                                        break;

                                                    case "Payees":
                                                        var PayeeDetails = database.LPayees.Find(TranId);
                                                        //check if Authorised payee verification is set to true else throw error message to user
                                                        //if (PayeeDetails.LpAuthorisedPayeeVerification)
                                                        //{
                                                        var user = new ApplicationUser() { UserName = PayeeDetails.LpEmail, Email = PayeeDetails.LpEmail, GcCompanyId = CompanyId, IsActive = true };//Is Active flag is set to false if a user is not approved from manager
                                                        IdentityResult result = UserManager.Create(user, ConfigurationManager.AppSettings["DefaultUserPassword"]);
                                                        //lines added by shubham to add roles to user after registering User Using Sql Query
                                                        var RegUser = UserManager.FindByEmail(user.Email);
                                                        //update UserId in LPayees
                                                        PayeeDetails.LpUserId = RegUser.Id;
                                                        database.Entry(PayeeDetails).State = EntityState.Modified;
                                                        await database.SaveChangesAsync();


                                                        //Creating The forzip folder as well.

                                                        string path1 = ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + Company.GcCode + "/" + PayeeDetails.LpEmail + "/forzip";
                                                        if (!Directory.Exists(path1))
                                                        {
                                                            Directory.CreateDirectory(path1);
                                                        }

                                                        //SS NEW CODE Check if any role exist for this user
                                                        if (UserManager.GetRoles(RegUser.Id).Count() == 0)
                                                        {
                                                            database.Database.ExecuteSqlCommand("insert into AspNetUserRoles(UserId,RoleId) values('" + RegUser.Id + "',(select Id from AspNetRoles where Name='Payee' and CompanyCode='" + Company.GcCode + "'))");
                                                        }
                                                        //Add an entry in LUsers if FinOps Roles are selected by User
                                                        if (!string.IsNullOrEmpty(PayeeDetails.LpFinOpsRoles))
                                                        {
                                                            //Max Ordinal in Users
                                                            int MaxOrdinal = db.LWorkFlowConfigs.Where(p => p.RWorkFlow.RwfName == "Users").Where(p => p.LwfcCompanyId == PayeeDetails.LpCompanyId).ToList().Max(p => p.LwfcOrdinalNumber);
                                                            //Add entry in LUsers
                                                            string FirstName = "";
                                                            string LastName = "";
                                                            string PhoneNumber = "";

                                                            var Query = "Exec USPGetLPayeeById @Id";
                                                            SqlCommand cmd = new SqlCommand(Query);
                                                            cmd.Parameters.AddWithValue("@Id", PayeeDetails.Id);
                                                            DataSet ds = Globals.GetData(cmd);
                                                            if (ds.Tables[0].Rows.Count == 1)
                                                            {
                                                                FirstName = Convert.ToString(ds.Tables[0].Rows[0]["LpFirstName"]);
                                                                LastName = Convert.ToString(ds.Tables[0].Rows[0]["LpLastName"]);
                                                                PhoneNumber = Convert.ToString(ds.Tables[0].Rows[0]["LpPhone"]);
                                                            }
                                                            //string FirstName = Globals.GetPayeeFirstOrLast(PayeeDetails.Id, "First");
                                                            //string LastName = Globals.GetPayeeFirstOrLast(PayeeDetails.Id, "Last");
                                                            var LUserModel = new LUser
                                                            {
                                                                LuIsAlteryxUser = false,
                                                                LuBlockNotification = false,
                                                                LuCompanyId = PayeeDetails.WFCompanyId,
                                                                LuCreatedById = PayeeDetails.LpCreatedById,
                                                                LuCreatedDateTime = DateTime.UtcNow,
                                                                LuEmail = PayeeDetails.LpEmail,
                                                                //LuFirstName = Convert.ToString(PayeeDetails.LpFirstName),//SS Decrypted
                                                                //LuLastName = Convert.ToString(PayeeDetails.LpLastName == null ? PayeeDetails.LpFirstName : PayeeDetails.LpLastName),//This line is added to resolve the error When Payee is created without lastname as Last name is not null in Users
                                                                LuFirstName = FirstName,
                                                                LuLastName = LastName,
                                                                LuPhone = PhoneNumber,
                                                                LuStatus = Convert.ToString(PayeeDetails.WFStatus),
                                                                LuUpdatedById = PayeeDetails.LpUpdatedById,
                                                                LuUpdatedDateTime = DateTime.UtcNow,
                                                                LuUserId = PayeeDetails.LpUserId,
                                                                WFAnalystId = PayeeDetails.WFAnalystId,
                                                                WFCompanyId = PayeeDetails.WFCompanyId,
                                                                WFCurrentOwnerId = PayeeDetails.WFCurrentOwnerId,
                                                                WFManagerId = PayeeDetails.WFManagerId,
                                                                WFRequesterRoleId = PayeeDetails.WFRequesterRoleId,
                                                                WFOrdinal = MaxOrdinal,
                                                                WFStatus = "Completed",
                                                                WFType = "LUsers",
                                                                WFRequesterId = PayeeDetails.WFRequesterId,
                                                                LuCreateLogin = PayeeDetails.LpCreateLogin,
                                                                WFComments = PayeeDetails.WFComments
                                                            };
                                                            database.LUsers.Add(LUserModel);
                                                            await database.SaveChangesAsync();
                                                            //Add entry in Aspnet roles
                                                            var RolesList = PayeeDetails.LpFinOpsRoles.Split(',').ToList();
                                                            foreach (var role in RolesList)
                                                            {
                                                                database.Database.ExecuteSqlCommand("insert into AspNetUserRoles(UserId,RoleId) values('" + RegUser.Id + "','" + role + "')");
                                                                //Provide same portfolios to User which Payee has
                                                                database.Database.ExecuteSqlCommand("insert into MEntityPortfolios (MepEntityId,MepEntityType,MepPortfolioId,MepRoleId) select " + LUserModel.Id + ",'LUsers',MepPortfolioId,'" + role + "' from MEntityPortfolios where MepEntityType='LPayees' and MepEntityId=" + PayeeDetails.Id);
                                                            }
                                                        }
                                                        //UserManager.AddToRole(RegUser.Id, role);//user manager method has stopped working since opco column has beed added in aspnet roles
                                                        //Create AD User if Create login is true
                                                        if (PayeeDetails.LpCreateLogin)
                                                        {
                                                            //***********************AD changes
                                                            //commenting random password now and setting Default password as of now.
                                                            //RandomPassword rp = new RandomPassword();
                                                            //string rndPwd = rp.Generate();
                                                            //Use Global Method to create AD User created on 09 June 2018
                                                            var Identifier = PayeeDetails.LpFirstName + " " + PayeeDetails.LpLastName;
                                                            //when IsWIAMEnabled value is yes, AD Account will not be created again
                                                            if (!string.IsNullOrEmpty(IsWIAMEnabled) && !"yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                                                            {
                                                                //Use Global Method to create AD User created on 09 June 2018
                                                                Globals.CreateUser(PayeeDetails.LpEmail, ProjectEnviournment, CompanyId, Identifier, LoggedInUserId, Comments, null);
                                                            }
                                                            else
                                                            {
                                                                string ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, PayeeDetails.LpEmail).FirstOrDefault();
                                                                //Globals.ExecuteSPLogEmail(PayeeDetails.LpEmail, null, null, null, "Vodafone LITE", "Hi ,<br>you are now being approved in LITE, you can start using LITE portal using credentials sent earlier. ", true, "Notifier", "High", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                                                                var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeEmail2UponApproval").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                                                                if (EmailTemplate != null)
                                                                {
                                                                    string EmailBody = EmailTemplate.LetEmailBody;
                                                                    EmailBody = EmailBody.Replace("###EmailAddress###", PayeeDetails.LpEmail);
                                                                    string EmailSubject = EmailTemplate.LetEmailSubject;
                                                                    Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                                                                }
                                                            }
                                                        }
                                                        //}
                                                        //else
                                                        //{
                                                        //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Payee needs to be authorised by Analyst before final approval"));
                                                        //}

                                                        break;
                                                    case "PayeesCR":
                                                        var CRDetails = db.LChangeRequests.Where(p => p.Id == TranId).FirstOrDefault();
                                                        switch (CRDetails.LcrAction)
                                                        {
                                                            case "Edit":
                                                                var PayeeDetail = database.LPayees.Find(CRDetails.LcrRowId);
                                                                if (CRDetails.LcrColumnName == "LpCreateLogin" && CRDetails.LcrNewValue.Equals("true", StringComparison.OrdinalIgnoreCase))
                                                                {
                                                                    //var IdentityUser = new ApplicationUser() { UserName = PayeeDetail.LpEmail, Email = PayeeDetail.LpEmail, GcCompanyId = CompanyId, IsActive = true };//Is Active flag is set to false if a user is not approved from manager
                                                                    //IdentityResult UM = UserManager.Create(IdentityUser, ConfigurationManager.AppSettings["DefaultUserPassword"]);
                                                                    ////lines added by shubham to add roles to user after registering User Using Sql Query
                                                                    //if (UM.Succeeded)
                                                                    //{
                                                                    var RegisteredUser = db.AspNetUsers.Where(p => p.Email == PayeeDetail.LpEmail).FirstOrDefault();//UserManager.FindByEmail(IdentityUser.Email);
                                                                      
                                                                    //***********************AD changes
                                                                    //Use Global Method to create AD User created on 09 June 2018
                                                                    var Identifier = PayeeDetail.LpFirstName + " " + PayeeDetail.LpLastName;
                                                                    //when IsWIAMEnabled value is yes, AD Account will not be created again
                                                                    if (!string.IsNullOrEmpty(IsWIAMEnabled) && !"yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                                                                        Globals.CreateUser(PayeeDetail.LpEmail, ProjectEnviournment, CompanyId, Identifier, LoggedInUserId, Comments, null);
                                                                    else if (PayeeDetail.LpCreateLogin)
                                                                    {
                                                                        //Globals.ExecuteSPLogEmail(PayeeDetail.LpEmail, null, null, null, "Vodafone LITE", "Hi ,<br>you are now being approved in LITE, you can start using LITE portal using credentials sent earlier. ", true, "Notifier", "High", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                                                                        string ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, PayeeDetail.LpEmail).FirstOrDefault();
                                                                        var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeEmail2UponApproval").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                                                                        if (EmailTemplate != null)
                                                                        {
                                                                            string EmailBody = EmailTemplate.LetEmailBody;
                                                                            EmailBody = EmailBody.Replace("###EmailAddress###", PayeeDetail.LpEmail);
                                                                            string EmailSubject = EmailTemplate.LetEmailSubject;
                                                                            Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                                                                        }
                                                                    }
                                                                }
                                                                ////RK R2.2 Commented below line as we do not take drastic action of deleting user and only make the aspnet account inactive.
                                                                //the disabling aspnetusers record is handled in spUpdateActionStatus
                                                                //else if (CRDetails.LcrColumnName == "LpCreateLogin" && CRDetails.LcrNewValue.Equals("false", StringComparison.OrdinalIgnoreCase))
                                                                //{
                                                                //    
                                                                //    ////delete AD account 
                                                                //    //var AdModel = new ADUserViewModel();
                                                                //    //AdModel.Email = PayeeDetail.LpEmail;

                                                                //    ////Globals.DeleteUser(AdModel);
                                                                //    ////make AspnetUser Account InActive
                                                                //do not do because it gives message 'User is InActive', which is confusing.Also, it creates problems if user has FinOps and Payee accounts both
                                                                //Change AspNetUser IsActive only when WIAM is disabled
                                                                if (!string.IsNullOrEmpty(IsWIAMEnabled) && !"yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                                                                {
                                                                    var AspnetUser = db.AspNetUsers.Where(p => p.Email == PayeeDetail.LpEmail).FirstOrDefault();
                                                                    if (AspnetUser != null)
                                                                    {
                                                                        AspnetUser.IsActive = false;
                                                                        db.Entry(AspnetUser).State = EntityState.Modified;
                                                                        await database.SaveChangesAsync();
                                                                    }
                                                                }
                                                                var ExistingUserDetails = db.LUsers.Where(p => p.LuEmail == PayeeDetail.LpEmail).FirstOrDefault();
                                                                //check for FinOps Roles checked
                                                                if (CRDetails.LcrColumnName == "LpFinOpsRoles" && string.IsNullOrEmpty(CRDetails.LcrNewValue))
                                                                {
                                                                    var previousRoles = CRDetails.LcrOldId.Split(',').ToList();
                                                                    foreach (var roleid in previousRoles)
                                                                    {
                                                                        //delete entry from Aspnet roles
                                                                        database.Database.ExecuteSqlCommand("delete from AspnetUserRoles where UserId='" + PayeeDetail.LpUserId + "' and RoleId=" + roleid);
                                                                    }
                                                                    if (ExistingUserDetails != null)
                                                                    {
                                                                        //delete portfolio
                                                                        database.Database.ExecuteSqlCommand("delete from MEntityPortfolios where MepEntityId=" + ExistingUserDetails.Id + " and MepEntityType='LUsers'");
                                                                    }
                                                                }
                                                                else if (CRDetails.LcrColumnName == "LpFinOpsRoles" && !string.IsNullOrEmpty(CRDetails.LcrNewId))
                                                                {
                                                                    //NOT Used It was a issue earlier that LpUserId was not generated from AspnetUser Table but now this user has been fixed.
                                                                    //if (string.IsNullOrEmpty(PayeeDetail.LpUserId)) 
                                                                    //{
                                                                    //    var IdentityUser = new ApplicationUser() { UserName = PayeeDetail.LpEmail, Email = PayeeDetail.LpEmail, GcCompanyId = CompanyId, IsActive = true };//Is Active flag is set to false if a user is not approved from manager
                                                                    //    IdentityResult UM = UserManager.Create(IdentityUser, ConfigurationManager.AppSettings["DefaultUserPassword"]);
                                                                    //}
                                                                    //var RegisteredUser = UserManager.FindByEmail(IdentityUser.Email);
                                                                    //lines added by shubham to add roles to user after registering User Using Sql Query
                                                                    //Max Ordinal in Users
                                                                    var RegisteredUser = db.AspNetUsers.Where(p => p.Email == PayeeDetail.LpEmail).FirstOrDefault();
                                                                    var MaxOrdinal = db.LWorkFlowConfigs.Where(p => p.RWorkFlow.RwfName == "Users").Where(p => p.LwfcCompanyId == PayeeDetail.LpCompanyId).ToList().Max(p => p.LwfcOrdinalNumber);
                                                                    var LUserId = 0;
                                                                    if (ExistingUserDetails != null)
                                                                    {
                                                                        LUserId = ExistingUserDetails.Id;
                                                                    }
                                                                    else
                                                                    {
                                                                        //Add entry in LUsers

                                                                        string FirstName = "";
                                                                        string LastName = "";
                                                                        string PhoneNumber = "";

                                                                        var Query = "Exec USPGetLPayeeById @Id";
                                                                        SqlCommand cmd = new SqlCommand(Query);
                                                                        cmd.Parameters.AddWithValue("@Id", PayeeDetail.Id);
                                                                        DataSet ds = Globals.GetData(cmd);
                                                                        if (ds.Tables[0].Rows.Count == 1)
                                                                        {
                                                                            FirstName = Convert.ToString(ds.Tables[0].Rows[0]["LpFirstName"]);
                                                                            LastName = Convert.ToString(ds.Tables[0].Rows[0]["LpLastName"]);
                                                                            PhoneNumber = Convert.ToString(ds.Tables[0].Rows[0]["LpPhone"]);
                                                                        }



                                                                        //string FirstName = Globals.GetPayeeFirstOrLast(PayeeDetails.Id, "First");
                                                                        //string LastName = Globals.GetPayeeFirstOrLast(PayeeDetails.Id, "Last");

                                                                        //string FirstName = Globals.GetPayeeFirstOrLast(PayeeDetail.Id, "First");
                                                                        //string LastName = Globals.GetPayeeFirstOrLast(PayeeDetail.Id, "Last");
                                                                        var LUserModel = new LUser
                                                                        {
                                                                            LuIsAlteryxUser = false,
                                                                            LuBlockNotification = false,
                                                                            LuCompanyId = PayeeDetail.LpCompanyId,
                                                                            LuCreatedById = PayeeDetail.LpCreatedById,
                                                                            LuCreatedDateTime = DateTime.UtcNow,
                                                                            LuEmail = PayeeDetail.LpEmail,
                                                                            //LuFirstName = Convert.ToString(PayeeDetail.LpFirstName),
                                                                            //LuLastName = Convert.ToString(PayeeDetail.LpLastName == null ? PayeeDetail.LpFirstName : PayeeDetail.LpLastName),//This line is added to resolve the error When Payee is created without lastname as Last name is not null in Users
                                                                            LuFirstName = FirstName,
                                                                            LuLastName = LastName,
                                                                            LuPhone = PhoneNumber,
                                                                            LuStatus = PayeeDetail.WFStatus,
                                                                            LuUpdatedById = PayeeDetail.LpUpdatedById,
                                                                            LuUpdatedDateTime = DateTime.UtcNow,
                                                                            LuUserId = PayeeDetail.LpUserId,
                                                                            WFAnalystId = PayeeDetail.WFAnalystId,
                                                                            WFCompanyId = PayeeDetail.WFCompanyId,
                                                                            WFCurrentOwnerId = PayeeDetail.WFCurrentOwnerId,
                                                                            WFManagerId = PayeeDetail.WFRequesterId = PayeeDetail.WFRequesterId,
                                                                            WFRequesterRoleId = CRDetails.WFRequesterRoleId,
                                                                            WFOrdinal = MaxOrdinal,
                                                                            WFRequesterId = CRDetails.WFRequesterId,
                                                                            WFType = "LUsers",
                                                                            WFStatus = "Completed"
                                                                        };
                                                                        database.LUsers.Add(LUserModel);
                                                                        await database.SaveChangesAsync();
                                                                        LUserId = LUserModel.Id;
                                                                    }
                                                                    //Add entry in Aspnet roles
                                                                    var RolesList = PayeeDetail.LpFinOpsRoles.Split(',').ToList();
                                                                    foreach (var role in RolesList)
                                                                    {
                                                                        database.Database.ExecuteSqlCommand("insert into AspNetUserRoles(UserId,RoleId) values('" + RegisteredUser.Id + "','" + role + "')");
                                                                        //Provide same portfolios to User which Payee has
                                                                        database.Database.ExecuteSqlCommand("insert into MEntityPortfolios (MepEntityId,MepEntityType,MepPortfolioId,MepRoleId) select " + LUserId + ",'LUsers',MepPortfolioId," + role + " from MEntityPortfolios where MepEntityType='LPayees' and MepEntityId=" + PayeeDetail.Id);
                                                                    }
                                                                }
                                                                break;
                                                        }
                                                        break;
                                                    #region commented
                                                    //case "RefFiles":
                                                    //    var RefFile = db.LRefFiles.Where(p => p.Id == TranId).FirstOrDefault();
                                                    //    var Analyst = db.AspNetUsers.Find(RefFile.WFAnalystId);
                                                    //    var Manager = db.AspNetUsers.Find(RefFile.WFManagerId);
                                                    //    var RefFileType = db.LRefFileTypes.Find(RefFile.LrfRefFileTypeId);
                                                    //    var CompanyDetails = db.GCompanies.Find(RefFile.LrfCompanyId);
                                                    //    var AttachedFileList = db.LAttachments.Where(p => p.LaEntityId == TranId).Where(p => p.LaEntityType == "LRefFiles").ToList();
                                                    //    for (var i = 0; i < AttachedFileList.Count(); i++)
                                                    //    {
                                                    //        string Previouspath = AttachedFileList[i].LaFilePath + AttachedFileList[i].LaFileName;//string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyDetails.GcCode + "/RefFiles/" + RefFileType.LrftName);
                                                    //        if (Analyst != null)
                                                    //        {
                                                    //            //Analyst copy of file is saved
                                                    //            var AnalystfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S2AS3BucketFolder"], CompanyDetails.GcCode + "/" + Analyst.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth);
                                                    //            //check if directory exists or not.If not create a new one
                                                    //            //SS Migrated the below section of copying File via SDK
                                                    //            bool exists = Globals.FolderExistsInA2S(AnalystfilePath);
                                                    //            if (!exists)
                                                    //            {
                                                    //                Globals.CreateFolderInA2S(AnalystfilePath, RefFile.LrfMonth);
                                                    //            }
                                                    //            var bytes = Globals.DownloadFromS3(AttachedFileList[i].LaFileName, AttachedFileList[i].LaFilePath);
                                                    //            using (var ms = new MemoryStream(bytes))
                                                    //            {
                                                    //                Globals.UploadToA2S(ms, AttachedFileList[i].LaFileName, AnalystfilePath);
                                                    //            }
                                                    //            //bool exists = System.IO.Directory.Exists(AnalystfilePath);
                                                    //            //if (!exists)
                                                    //            //{
                                                    //            //    System.IO.Directory.CreateDirectory(AnalystfilePath);
                                                    //            //}
                                                    //            ////check if same file exist then delete that file
                                                    //            //if (System.IO.File.Exists(AnalystfilePath + "/" + AttachedFileList[i].LaFileName))
                                                    //            //{
                                                    //            //    System.IO.File.Delete(AnalystfilePath + "/" + AttachedFileList[i].LaFileName);
                                                    //            //}
                                                    //            //System.IO.File.Copy(Previouspath + "/" + AttachedFileList[i].LaFileName, AnalystfilePath + "/" + AttachedFileList[i].LaFileName);
                                                    //        }
                                                    //        if (Manager != null)
                                                    //        {
                                                    //            //Manager Copy of File is saved
                                                    //            var ManagerfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S2AS3BucketFolder"], CompanyDetails.GcCode + "/" + Manager.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth);
                                                    //            //check if directory exists or not.If not create a new one
                                                    //            //SS Migrated the below section of copying File via SDK
                                                    //            bool exists = Globals.FolderExistsInA2S(ManagerfilePath);
                                                    //            if (!exists)
                                                    //            {
                                                    //                Globals.CreateFolderInA2S(ManagerfilePath, RefFile.LrfMonth);
                                                    //            }
                                                    //            var bytes = Globals.DownloadFromS3(AttachedFileList[i].LaFileName, AttachedFileList[i].LaFilePath);
                                                    //            using (var ms = new MemoryStream(bytes))
                                                    //            {
                                                    //                Globals.UploadToA2S(ms, AttachedFileList[i].LaFileName, ManagerfilePath);
                                                    //            }
                                                    //            ////check if directory exists or not.If not create a new one
                                                    //            //var exists = System.IO.Directory.Exists(ManagerfilePath);
                                                    //            //if (!exists)
                                                    //            //{
                                                    //            //    System.IO.Directory.CreateDirectory(ManagerfilePath);
                                                    //            //}
                                                    //            ////check if same file exist then delete that file
                                                    //            //if (System.IO.File.Exists(ManagerfilePath + "/" + AttachedFileList[i].LaFileName))
                                                    //            //{
                                                    //            //    System.IO.File.Delete(ManagerfilePath + "/" + AttachedFileList[i].LaFileName);
                                                    //            //}
                                                    //            //System.IO.File.Copy(Previouspath + "/" + AttachedFileList[i].LaFileName, ManagerfilePath + "/" + AttachedFileList[i].LaFileName);
                                                    //        }
                                                    //    }

                                                    //    //Find Supporting Documents and Save Them in the corresponding Analyst and Manager path
                                                    //    var SupportingDocumentList = db.LSupportingDocuments.Where(p => p.LsdEntityId == TranId).Where(p => p.LsdEntityType == "LRefFiles").ToList();
                                                    //    for (var j = 0; j < SupportingDocumentList.Count(); j++)
                                                    //    {
                                                    //        string Previouspath = SupportingDocumentList[j].LsdFilePath;//string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyDetails.GcCode + "/RefFiles/" + RefFileType.LrftName+ "/SupportingDocument");
                                                    //        if (Analyst != null)
                                                    //        {
                                                    //            //Analyst copy of file is saved
                                                    //            var AnalystfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S2AS3BucketFolder"], CompanyDetails.GcCode + "/" + Analyst.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth + "/SupportingDocuments");
                                                    //            //check if directory exists or not.If not create a new one
                                                    //            //bool exists = System.IO.Directory.Exists(AnalystfilePath);
                                                    //            //if (!exists)
                                                    //            //{
                                                    //            //    System.IO.Directory.CreateDirectory(AnalystfilePath);
                                                    //            //}
                                                    //            //System.IO.File.Copy(Previouspath + "/" + SupportingDocumentList[j].LsdFileName, AnalystfilePath + "/" + SupportingDocumentList[j].LsdFileName);
                                                    //            //SS Migrated the below section of copying File via SDK
                                                    //            bool exists = Globals.FolderExistsInA2S(AnalystfilePath);
                                                    //            if (!exists)
                                                    //            {
                                                    //                Globals.CreateFolderInA2S(AnalystfilePath, "SupportingDocuments");
                                                    //            }
                                                    //            var bytes = Globals.DownloadFromS3(SupportingDocumentList[j].LsdFileName, SupportingDocumentList[j].LsdFilePath);
                                                    //            using (var ms = new MemoryStream(bytes))
                                                    //            {
                                                    //                Globals.UploadToA2S(ms, SupportingDocumentList[j].LsdFileName, AnalystfilePath);
                                                    //            }
                                                    //        }
                                                    //        //If manager is not present
                                                    //        if (Manager != null)
                                                    //        {
                                                    //            //Manager Copy of File is saved
                                                    //            var ManagerfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S2AS3BucketFolder"], CompanyDetails.GcCode + "/" + Manager.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth + "/SupportingDocuments");
                                                    //            ////check if directory exists or not.If not create a new one
                                                    //            //var exists = System.IO.Directory.Exists(ManagerfilePath);
                                                    //            //if (!exists)
                                                    //            //{
                                                    //            //    System.IO.Directory.CreateDirectory(ManagerfilePath);
                                                    //            //}
                                                    //            //System.IO.File.Copy(Previouspath + "/" + SupportingDocumentList[j].LsdFileName, ManagerfilePath + "/" + SupportingDocumentList[j].LsdFileName);
                                                    //            //SS Migrated the below section of copying File via SDK
                                                    //            bool exists = Globals.FolderExistsInA2S(ManagerfilePath);
                                                    //            if (!exists)
                                                    //            {
                                                    //                Globals.CreateFolderInA2S(ManagerfilePath, "SupportingDocuments");
                                                    //            }
                                                    //            var bytes = Globals.DownloadFromS3(SupportingDocumentList[j].LsdFileName, SupportingDocumentList[j].LsdFilePath);
                                                    //            using (var ms = new MemoryStream(bytes))
                                                    //            {
                                                    //                Globals.UploadToA2S(ms, SupportingDocumentList[j].LsdFileName, ManagerfilePath);
                                                    //            }
                                                    //        }
                                                    //    }

                                                    //    break;
                                                    #endregion
                                                    case "UsersCR":
                                                        var UserCRDetails = db.LChangeRequests.Where(p => p.Id == TranId).FirstOrDefault();

                                                        if (UserCRDetails.LcrAction.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            using (var transaction = db.Database.BeginTransaction())
                                                            {
                                                                try
                                                                {
                                                                    //Added by Sachin --delete AD Account.Remove AD Account when User is terminated
                                                                    LUser Luser = db.LUsers.Where(p => p.Id == UserCRDetails.LcrRowId).FirstOrDefault();
                                                                    if (ProjectEnviournment.Equals("Prod", StringComparison.OrdinalIgnoreCase))
                                                                    {
                                                                        Globals.DeleteUser(new ADUserViewModel { Email = Luser.LuEmail });
                                                                    }

                                                                    Luser.WFStatus = "InActive";
                                                                    Luser.LuCreateLogin = false;
                                                                    Luser.LuUpdatedDateTime = DateTime.Now;
                                                                    Luser.WFComments = UserCRDetails.WFComments;
                                                                    db.Entry(Luser).State = EntityState.Modified;
                                                                    db.SaveChanges();
                                                                    //Commenting on 20Aug19 by ShivaniG, As per email and discussion for sync up WIAM/non-WIAM
                                                                    //db.Database.ExecuteSqlCommand("delete from AspNetUserRoles where UserId ='" + Luser.LuUserId 
                                                                    //    + "' and RoleId not in (select Id from AspNetRoles where Name='Payee' and CompanyCode ='" +Company.GcCode +"')");
                                                                    var userId = Luser.LuUserId;
                                                                    //ShivaniG commentted on 25/07/2019 - part of bug
                                                                    //do not do because it gives message 'User is InActive', which is confusing. Also, it creates problems if user has FinOps and Payee accounts both
                                                                    //Change AspNetUser IsActive only when WIAM is disabled
                                                                    if (!string.IsNullOrEmpty(IsWIAMEnabled) && !"yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                                                                    {
                                                                        AspNetUser AspUserdacti = db.AspNetUsers.Find(userId);
                                                                        AspUserdacti.IsActive = false;
                                                                        db.Entry(AspUserdacti).State = EntityState.Modified;
                                                                        // db.AspNetUsers.Remove(AspUser);
                                                                        db.SaveChanges();
                                                                    }

                                                                    //R3.1 - Delete User folder from S3- cleanup
                                                                    if (Globals.FolderExistsInS3(Luser.LuEmail, Company.GcCode))
                                                                        Globals.DeleteMultipleFilesFromS3(Company.GcCode + "/" + Luser.LuEmail );

                                                                    //if (System.IO.Directory.Exists(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + Company.GcCode + "/" + Luser.LuEmail))
                                                                    //{
                                                                    //    System.IO.Directory.Delete(ConfigurationManager.AppSettings["LocalTempFileFolder"] + "/" + Company.GcCode + "/" + Luser.LuEmail, true);
                                                                    //}
                                                                    //LPasswordHistory Phistory = db.LPasswordHistories.Where(x => x.UserId == userId).FirstOrDefault();
                                                                    //db.LPasswordHistories.Remove(Phistory);
                                                                    //db.SaveChanges();

                                                                    //var Portfolios = db.MEntityPortfolios.Where(p => p.MepEntityType == "LUsers").Where(p => p.MepEntityId == Luser.Id);
                                                                    //db.MEntityPortfolios.RemoveRange(Portfolios);
                                                                    //db.SaveChanges();

                                                                    //db.Database.ExecuteSqlCommand("Update  from XSchema.XReportUsers where UserId ='" + Luser.LuUserId + "'");
                                                                    // db.Database.ExecuteSqlCommand("Update  XSchema" + Company.GcCode.ToUpper() + ".XReportUsers set  XUserEmailID = XUserEmailID + 'X' where XUserEmailID=" + Luser.LuEmail + ")");

                                                                    transaction.Commit();
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    transaction.Rollback();

                                                                }
                                                            }

                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        break;
                                        //case "Reject":

                                        //    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, SPresult));//type 2 error
                    }
                }
                catch (DbEntityValidationException dbex)
                {

                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {

                    throw ex;//This exception will be handled in FilterConfig's CustomHandler
                }

            }
            return Ok();
        }
        [HttpPost]
        public async Task<IHttpActionResult> UpdateActionStatusNew(List<string> objTrans, string Action, string WorkFlowName, int CompanyId, string LoggedInUserId, string Comments, string LoggedInRoleId, string AssigneeId)
        {
            //Shivani Changes for WIAM integration. 09 May 2019
            string IsWIAMEnabled = db.Database.SqlQuery<string>("select [dbo].[FNIsWIAMEnabled]({0})", CompanyId).FirstOrDefault();
            //Currently using two different db context as we were facing errors using a single context after execution of SP
            using (var database = new SOSEDMV10Entities())
            {
                //using (var transaction = db.Database.BeginTransaction())
                //{
                string TransactionId = string.Empty;
                int counter = 1;
                foreach (string s in objTrans)
                {
                    if (counter == 1)
                    {
                        TransactionId = s;
                    }
                    else
                    {
                        TransactionId = TransactionId + ',' + s;
                    }
                    counter = counter + 1;
                }
                try
                {
                    //var ProjectEnviournment = ConfigurationManager.AppSettings["ProjectEnviournment"];
                    var ProjectEnviournment = db.Database.SqlQuery<string>("select [dbo].[FNReturnProjectEnv]()").FirstOrDefault();
                    string SPresult = Globals.ExecuteSPUpdateActionStatus(Action, WorkFlowName, TransactionId, CompanyId, LoggedInUserId, Comments, LoggedInRoleId, ProjectEnviournment, AssigneeId);
                    //  db.SaveChanges();
                    var CanPerformFinalApprovalTasks = false;
                    if (string.IsNullOrEmpty(SPresult))
                    {
                        CanPerformFinalApprovalTasks = true;
                    }
                    else
                    {
                        CanPerformFinalApprovalTasks = true;
                        if (SPresult.Contains("Validation Message"))
                        {
                            CanPerformFinalApprovalTasks = false;
                        }
                    }
                    //After checking Conditions for Final Approval do subsequent action
                    if (CanPerformFinalApprovalTasks)
                    {
                        //get work flow name based on Workflow name
                        if (!string.IsNullOrEmpty(TransactionId))
                        {
                            var TranIdList = TransactionId.Split(',').ToList();
                            foreach (var TId in TranIdList)
                            {
                                var TranId = Convert.ToInt32(TId);
                                var WorkFlow = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkFlowName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                var Company = db.GCompanies.Find(CompanyId);
                                var WFColumns = db.Database.SqlQuery<WFColumnsViewModel>("select WFRequesterId,WFAnalystId,WFManagerId,WFOrdinal,WFCurrentOwnerId,WFStatus,WFType,WFRequesterRoleId,WFCompanyId,WFComments from " + WorkFlow.RwfBaseTableName + " where Id=" + TranId).FirstOrDefault();
                                switch (Action)
                                {
                                    case "Approve":
                                        //Allow action only if Current Owner Id is equal to LoggedIn UserId and Workflow is completed
                                        if (WFColumns.WFCurrentOwnerId == LoggedInUserId && WFColumns.WFStatus == "Completed")
                                        {
                                            if (WfIsLastOrdinal(WorkFlow.Id, Convert.ToInt32(TranId), CompanyId))
                                            {

                                                switch (WorkFlow.RwfName)
                                                {
                                                    case "Users":
                                                        var User = database.LUsers.Find(TranId);
                                                        var AspUser = database.AspNetUsers.Find(User.LuUserId);
                                                        AspUser.IsActive = true;

                                                        database.Entry(AspUser).State = EntityState.Modified;
                                                        await database.SaveChangesAsync();
                                                        //******************************************AD changes
                                                        //Random password generator code used for creating AD account ---- commenting as now

                                                        //Use Global Method to create AD User created on 09 June 2018
                                                        var UserIdentifier = User.LuFirstName + " " + User.LuLastName;
                                                        //when IsWIAMEnabled value is yes, AD Account will not be created again
                                                        if (!string.IsNullOrEmpty(IsWIAMEnabled) && !"yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                                                            Globals.CreateUser(User.LuEmail, ProjectEnviournment, CompanyId, UserIdentifier, LoggedInUserId, Comments, null);
                                                        else if (Convert.ToBoolean(User.LuCreateLogin))
                                                        {
                                                            //Globals.ExecuteSPLogEmail(User.LuEmail, null, null, null, "Vodafone LITE", "Hi ,<br>you are now being approved in LITE, you can start using LITE portal using credentials sent earlier. ", true, "Notifier", "High", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                                                            string ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, User.LuEmail).FirstOrDefault();
                                                            var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeEmail2UponApproval").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                                                            if (EmailTemplate != null)
                                                            {
                                                                string EmailBody = EmailTemplate.LetEmailBody;
                                                                EmailBody = EmailBody.Replace("###EmailAddress###", User.LuEmail);
                                                                string EmailSubject = EmailTemplate.LetEmailSubject;
                                                                Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                                                            }
                                                        }
                                                        break;

                                                    case "Payees":
                                                        var PayeeDetails = database.LPayees.Find(TranId);

                                                        //check if Authorised payee verification is set to true else throw error message to user
                                                        //if (PayeeDetails.LpAuthorisedPayeeVerification)
                                                        //{
                                                        var user = new ApplicationUser() { UserName = PayeeDetails.LpEmail, Email = PayeeDetails.LpEmail, GcCompanyId = CompanyId, IsActive = true };//Is Active flag is set to false if a user is not approved from manager
                                                        IdentityResult result = UserManager.Create(user, ConfigurationManager.AppSettings["DefaultUserPassword"]);
                                                        //lines added by shubham to add roles to user after registering User Using Sql Query
                                                        var RegUser = UserManager.FindByEmail(user.Email);
                                                        //update UserId in LPayees
                                                        PayeeDetails.LpUserId = RegUser.Id;
                                                        database.Entry(PayeeDetails).State = EntityState.Modified;
                                                        await database.SaveChangesAsync();
                                                        //SS NEW CODE Check if any role exist for this user
                                                        if (UserManager.GetRoles(RegUser.Id).Count() == 0)
                                                        {
                                                            database.Database.ExecuteSqlCommand("insert into AspNetUserRoles(UserId,RoleId) values('" + RegUser.Id + "',(select Id from AspNetRoles where Name='Payee' and CompanyCode='" + Company.GcCode + "'))");
                                                        }
                                                        //Add an entry in LUsers if FinOps Roles are selected by User
                                                        if (!string.IsNullOrEmpty(PayeeDetails.LpFinOpsRoles))
                                                        {
                                                            //Max Ordinal in Users
                                                            int MaxOrdinal = db.LWorkFlowConfigs.Where(p => p.RWorkFlow.RwfName == "Users").Where(p => p.LwfcCompanyId == PayeeDetails.LpCompanyId).ToList().Max(p => p.LwfcOrdinalNumber);
                                                            //Add entry in LUsers
                                                            string FirstName = Globals.GetPayeeFirstOrLast(PayeeDetails.Id, "First");
                                                            string LastName = Globals.GetPayeeFirstOrLast(PayeeDetails.Id, "Last");

                                                            var LUserModel = new LUser
                                                            {
                                                                LuIsAlteryxUser = false,
                                                                LuBlockNotification = false,
                                                                LuCompanyId = PayeeDetails.WFCompanyId,
                                                                LuCreatedById = PayeeDetails.LpCreatedById,
                                                                LuCreatedDateTime = DateTime.UtcNow,
                                                                LuEmail = PayeeDetails.LpEmail,
                                                                //LuFirstName = Convert.ToString(PayeeDetails.LpFirstName),//SS Decrypted
                                                                //LuLastName = Convert.ToString(PayeeDetails.LpLastName == null ? PayeeDetails.LpFirstName : PayeeDetails.LpLastName),//This line is added to resolve the error When Payee is created without lastname as Last name is not null in Users
                                                                LuFirstName = FirstName,
                                                                LuLastName = LastName,
                                                                LuPhone = Convert.ToString(PayeeDetails.LpPhone),
                                                                LuStatus = Convert.ToString(PayeeDetails.WFStatus),
                                                                LuUpdatedById = PayeeDetails.LpUpdatedById,
                                                                LuUpdatedDateTime = DateTime.UtcNow,
                                                                LuUserId = PayeeDetails.LpUserId,
                                                                WFAnalystId = PayeeDetails.WFAnalystId,
                                                                WFCompanyId = PayeeDetails.WFCompanyId,
                                                                WFCurrentOwnerId = PayeeDetails.WFCurrentOwnerId,
                                                                WFManagerId = PayeeDetails.WFManagerId,
                                                                WFRequesterRoleId = PayeeDetails.WFRequesterRoleId,
                                                                WFOrdinal = MaxOrdinal,
                                                                WFStatus = "Completed",
                                                                WFType = "LUsers",
                                                                WFRequesterId = PayeeDetails.WFRequesterId,
                                                                LuCreateLogin = PayeeDetails.LpCreateLogin,
                                                                WFComments = PayeeDetails.WFComments

                                                            };
                                                            database.LUsers.Add(LUserModel);
                                                            await database.SaveChangesAsync();
                                                            //Add entry in Aspnet roles
                                                            var RolesList = PayeeDetails.LpFinOpsRoles.Split(',').ToList();
                                                            foreach (var role in RolesList)
                                                            {
                                                                database.Database.ExecuteSqlCommand("insert into AspNetUserRoles(UserId,RoleId) values('" + RegUser.Id + "','" + role + "')");
                                                                //Provide same portfolios to User which Payee has
                                                                database.Database.ExecuteSqlCommand("insert into MEntityPortfolios (MepEntityId,MepEntityType,MepPortfolioId,MepRoleId) select " + LUserModel.Id + ",'LUsers',MepPortfolioId,'" + role + "' from MEntityPortfolios where MepEntityType='LPayees' and MepEntityId=" + PayeeDetails.Id);
                                                            }
                                                        }

                                                        //UserManager.AddToRole(RegUser.Id, role);//user manager method has stopped working since opco column has beed added in aspnet roles

                                                        //Create AD User if Create login is true
                                                        if (PayeeDetails.LpCreateLogin)
                                                        {
                                                            //***********************AD changes
                                                            //commenting random password now and setting Default password as of now.
                                                            //RandomPassword rp = new RandomPassword();
                                                            //string rndPwd = rp.Generate();
                                                            //Use Global Method to create AD User created on 09 June 2018
                                                            var Identifier = PayeeDetails.LpFirstName + " " + PayeeDetails.LpLastName;
                                                            //when IsWIAMEnabled value is yes, AD Account will not be created again
                                                            if (!string.IsNullOrEmpty(IsWIAMEnabled) && !"yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                                                                Globals.CreateUser(PayeeDetails.LpEmail, ProjectEnviournment, CompanyId, Identifier, LoggedInUserId, Comments, null);
                                                            else
                                                            {
                                                                //Globals.ExecuteSPLogEmail(PayeeDetails.LpEmail, null, null, null, "Vodafone LITE", "Hi ,<br>you are now being approved in LITE, you can start using LITE portal using credentials sent earlier. ", true, "Notifier", "High", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                                                                string ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, PayeeDetails.LpEmail).FirstOrDefault();
                                                                var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeEmail2UponApproval").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                                                                if (EmailTemplate != null)
                                                                {
                                                                    string EmailBody = EmailTemplate.LetEmailBody;
                                                                    EmailBody = EmailBody.Replace("###EmailAddress###", PayeeDetails.LpEmail);
                                                                    string EmailSubject = EmailTemplate.LetEmailSubject;
                                                                    Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                                                                }
                                                            }
                                                        }
                                                        //}
                                                        //else
                                                        //{
                                                        //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Payee needs to be authorised by Analyst before final approval"));
                                                        //}

                                                        break;
                                                    case "PayeesCR":
                                                        var CRDetails = db.LChangeRequests.Where(p => p.Id == TranId).FirstOrDefault();
                                                        switch (CRDetails.LcrAction)
                                                        {
                                                            case "Edit":
                                                                var PayeeDetail = database.LPayees.Find(CRDetails.LcrRowId);
                                                                if (CRDetails.LcrColumnName == "LpCreateLogin" && CRDetails.LcrNewValue.Equals("true", StringComparison.OrdinalIgnoreCase))
                                                                {
                                                                    //var IdentityUser = new ApplicationUser() { UserName = PayeeDetail.LpEmail, Email = PayeeDetail.LpEmail, GcCompanyId = CompanyId, IsActive = true };//Is Active flag is set to false if a user is not approved from manager
                                                                    //IdentityResult UM = UserManager.Create(IdentityUser, ConfigurationManager.AppSettings["DefaultUserPassword"]);
                                                                    ////lines added by shubham to add roles to user after registering User Using Sql Query
                                                                    //if (UM.Succeeded)
                                                                    //{
                                                                    var RegisteredUser = db.AspNetUsers.Where(p => p.Email == PayeeDetail.LpEmail).FirstOrDefault();//UserManager.FindByEmail(IdentityUser.Email);
                                                                                                                                                                    //update UserId in LPayees
                                                                                                                                                                    //PayeeDetail.LpUserId = RegisteredUser.Id;
                                                                                                                                                                    //database.Entry(PayeeDetail).State = EntityState.Modified;
                                                                                                                                                                    //await database.SaveChangesAsync();
                                                                                                                                                                    //SS NEW CODE Check if any role exist for this user
                                                                                                                                                                    //if (UserManager.GetRoles(RegisteredUser.Id).Count() == 0)
                                                                                                                                                                    //{
                                                                                                                                                                    //    database.Database.ExecuteSqlCommand("insert into AspNetUserRoles(UserId,RoleId) values('" + RegisteredUser.Id + "',(select Id from AspNetRoles where Name='Payee' and CompanyCode='" + Company.GcCode + "'))");
                                                                                                                                                                    //}

                                                                    //***********************AD changes
                                                                    //Use Global Method to create AD User created on 09 June 2018
                                                                    var Identifier = PayeeDetail.LpFirstName + " " + PayeeDetail.LpLastName;
                                                                    //when IsWIAMEnabled value is yes, AD Account will not be created again
                                                                    if (!string.IsNullOrEmpty(IsWIAMEnabled) && !"yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                                                                        Globals.CreateUser(PayeeDetail.LpEmail, ProjectEnviournment, CompanyId, Identifier, LoggedInUserId, Comments, null);
                                                                    else if (PayeeDetail.LpCreateLogin)
                                                                    {
                                                                        //Globals.ExecuteSPLogEmail(PayeeDetail.LpEmail, null, null, null, "Vodafone LITE", "Hi ,<br>you are now being approved in LITE, you can start using LITE portal using credentials sent earlier. ", true, "Notifier", "High", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                                                                        string ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, PayeeDetail.LpEmail).FirstOrDefault();
                                                                        var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeEmail2UponApproval").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                                                                        if (EmailTemplate != null)
                                                                        {
                                                                            string EmailBody = EmailTemplate.LetEmailBody;
                                                                            EmailBody = EmailBody.Replace("###EmailAddress###", PayeeDetail.LpEmail);
                                                                            string EmailSubject = EmailTemplate.LetEmailSubject;
                                                                            Globals.ExecuteSPLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                                                                        }
                                                                    }
                                                                }
                                                                else if (CRDetails.LcrColumnName == "LpCreateLogin" && CRDetails.LcrNewValue.Equals("false", StringComparison.OrdinalIgnoreCase))
                                                                {
                                                                    //delete AD account 
                                                                    var AdModel = new ADUserViewModel();
                                                                    AdModel.Email = PayeeDetail.LpEmail;
                                                                    Globals.DeleteUser(AdModel);
                                                                    //make AspnetUser Account InActive
                                                                    //do not do because it gives message 'User is InActive', which is confusing.Also, it creates problems if user has FinOps and Payee accounts both
                                                                    //Change AspNetUser IsActive only when WIAM is disabled    
                                                                    if (!string.IsNullOrEmpty(IsWIAMEnabled) && !"yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                                                                    {
                                                                        var AspnetUser = db.AspNetUsers.Where(p => p.Email == PayeeDetail.LpEmail).FirstOrDefault();
                                                                        if (AspnetUser != null)
                                                                        {
                                                                            AspnetUser.IsActive = false;
                                                                            db.Entry(AspnetUser).State = EntityState.Modified;
                                                                            await database.SaveChangesAsync();
                                                                        }
                                                                    }
                                                                }
                                                                var ExistingUserDetails = db.LUsers.Where(p => p.LuEmail == PayeeDetail.LpEmail).FirstOrDefault();
                                                                //check for FinOps Roles checked
                                                                if (CRDetails.LcrColumnName == "LpFinOpsRoles" && string.IsNullOrEmpty(CRDetails.LcrNewValue))
                                                                {
                                                                    var previousRoles = CRDetails.LcrOldId.Split(',').ToList();
                                                                    foreach (var roleid in previousRoles)
                                                                    {
                                                                        //delete entry from Aspnet roles
                                                                        database.Database.ExecuteSqlCommand("delete from AspnetUserRoles where UserId='" + PayeeDetail.LpUserId + "' and RoleId=" + roleid);
                                                                    }
                                                                    if (ExistingUserDetails != null)
                                                                    {
                                                                        //delete portfolio
                                                                        database.Database.ExecuteSqlCommand("delete from MEntityPortfolios where MepEntityId=" + ExistingUserDetails.Id + " and MepEntityType='LUsers'");
                                                                    }
                                                                }
                                                                else if (CRDetails.LcrColumnName == "LpFinOpsRoles" && !string.IsNullOrEmpty(CRDetails.LcrNewId))
                                                                {
                                                                    //NOT Used It was a issue earlier that LpUserId was not generated from AspnetUser Table but now this user has been fixed.
                                                                    //if (string.IsNullOrEmpty(PayeeDetail.LpUserId)) 
                                                                    //{
                                                                    //    var IdentityUser = new ApplicationUser() { UserName = PayeeDetail.LpEmail, Email = PayeeDetail.LpEmail, GcCompanyId = CompanyId, IsActive = true };//Is Active flag is set to false if a user is not approved from manager
                                                                    //    IdentityResult UM = UserManager.Create(IdentityUser, ConfigurationManager.AppSettings["DefaultUserPassword"]);
                                                                    //}
                                                                    //var RegisteredUser = UserManager.FindByEmail(IdentityUser.Email);
                                                                    //lines added by shubham to add roles to user after registering User Using Sql Query
                                                                    //Max Ordinal in Users
                                                                    var RegisteredUser = db.AspNetUsers.Where(p => p.Email == PayeeDetail.LpEmail).FirstOrDefault();
                                                                    var MaxOrdinal = db.LWorkFlowConfigs.Where(p => p.RWorkFlow.RwfName == "Users").Where(p => p.LwfcCompanyId == PayeeDetail.LpCompanyId).ToList().Max(p => p.LwfcOrdinalNumber);
                                                                    var LUserId = 0;
                                                                    if (ExistingUserDetails != null)
                                                                    {
                                                                        LUserId = ExistingUserDetails.Id;
                                                                    }
                                                                    else
                                                                    {
                                                                        //Add entry in LUsers
                                                                        string FirstName = Globals.GetPayeeFirstOrLast(PayeeDetail.Id, "First");
                                                                        string LastName = Globals.GetPayeeFirstOrLast(PayeeDetail.Id, "Last");
                                                                        var LUserModel = new LUser
                                                                        {
                                                                            LuIsAlteryxUser = false,
                                                                            LuBlockNotification = false,
                                                                            LuCompanyId = PayeeDetail.LpCompanyId,
                                                                            LuCreatedById = PayeeDetail.LpCreatedById,
                                                                            LuCreatedDateTime = DateTime.UtcNow,
                                                                            LuEmail = PayeeDetail.LpEmail,
                                                                            //LuFirstName = Convert.ToString(PayeeDetail.LpFirstName),
                                                                            //LuLastName = Convert.ToString(PayeeDetail.LpLastName == null ? PayeeDetail.LpFirstName : PayeeDetail.LpLastName),//This line is added to resolve the error When Payee is created without lastname as Last name is not null in Users
                                                                            LuFirstName = FirstName,
                                                                            LuLastName = LastName,
                                                                            LuPhone = Convert.ToString(PayeeDetail.LpPhone),
                                                                            LuStatus = PayeeDetail.WFStatus,
                                                                            LuUpdatedById = PayeeDetail.LpUpdatedById,
                                                                            LuUpdatedDateTime = DateTime.UtcNow,
                                                                            LuUserId = PayeeDetail.LpUserId,
                                                                            WFAnalystId = PayeeDetail.WFAnalystId,
                                                                            WFCompanyId = PayeeDetail.WFCompanyId,
                                                                            WFCurrentOwnerId = PayeeDetail.WFCurrentOwnerId,
                                                                            WFManagerId = PayeeDetail.WFRequesterId = PayeeDetail.WFRequesterId,
                                                                            WFRequesterRoleId = CRDetails.WFRequesterRoleId,
                                                                            WFOrdinal = MaxOrdinal,
                                                                            WFRequesterId = CRDetails.WFRequesterId,
                                                                            WFType = "LUsers",
                                                                            WFStatus = "Completed"
                                                                        };
                                                                        database.LUsers.Add(LUserModel);
                                                                        await database.SaveChangesAsync();
                                                                        LUserId = LUserModel.Id;
                                                                    }
                                                                    //Add entry in Aspnet roles
                                                                    var RolesList = PayeeDetail.LpFinOpsRoles.Split(',').ToList();
                                                                    foreach (var role in RolesList)
                                                                    {
                                                                        database.Database.ExecuteSqlCommand("insert into AspNetUserRoles(UserId,RoleId) values('" + RegisteredUser.Id + "','" + role + "')");
                                                                        //Provide same portfolios to User which Payee has
                                                                        database.Database.ExecuteSqlCommand("insert into MEntityPortfolios (MepEntityId,MepEntityType,MepPortfolioId,MepRoleId) select " + LUserId + ",'LUsers',MepPortfolioId," + role + " from MEntityPortfolios where MepEntityType='LPayees' and MepEntityId=" + PayeeDetail.Id);
                                                                    }
                                                                }
                                                                break;
                                                        }
                                                        break;
                                                    #region commented
                                                    //case "RefFiles":
                                                    //    var RefFile = db.LRefFiles.Where(p => p.Id == TranId).FirstOrDefault();
                                                    //    var Analyst = db.AspNetUsers.Find(RefFile.WFAnalystId);
                                                    //    var Manager = db.AspNetUsers.Find(RefFile.WFManagerId);
                                                    //    var RefFileType = db.LRefFileTypes.Find(RefFile.LrfRefFileTypeId);
                                                    //    var CompanyDetails = db.GCompanies.Find(RefFile.LrfCompanyId);
                                                    //    var AttachedFileList = db.LAttachments.Where(p => p.LaEntityId == TranId).Where(p => p.LaEntityType == "LRefFiles").ToList();
                                                    //    for (var i = 0; i < AttachedFileList.Count(); i++)
                                                    //    {
                                                    //        string Previouspath = AttachedFileList[i].LaFilePath + AttachedFileList[i].LaFileName;//string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyDetails.GcCode + "/RefFiles/" + RefFileType.LrftName);
                                                    //        if (Analyst != null)
                                                    //        {
                                                    //            //Analyst copy of file is saved
                                                    //            var AnalystfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S2AS3BucketFolder"], CompanyDetails.GcCode + "/" + Analyst.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth);
                                                    //            //check if directory exists or not.If not create a new one
                                                    //            //SS Migrated the below section of copying File via SDK
                                                    //            bool exists = Globals.FolderExistsInA2S(AnalystfilePath);
                                                    //            if (!exists)
                                                    //            {
                                                    //                Globals.CreateFolderInA2S(AnalystfilePath, RefFile.LrfMonth);
                                                    //            }
                                                    //            var bytes = Globals.DownloadFromS3(AttachedFileList[i].LaFileName, AttachedFileList[i].LaFilePath);
                                                    //            using (var ms = new MemoryStream(bytes))
                                                    //            {
                                                    //                Globals.UploadToA2S(ms, AttachedFileList[i].LaFileName, AnalystfilePath);
                                                    //            }
                                                    //            //bool exists = System.IO.Directory.Exists(AnalystfilePath);
                                                    //            //if (!exists)
                                                    //            //{
                                                    //            //    System.IO.Directory.CreateDirectory(AnalystfilePath);
                                                    //            //}
                                                    //            ////check if same file exist then delete that file
                                                    //            //if (System.IO.File.Exists(AnalystfilePath + "/" + AttachedFileList[i].LaFileName))
                                                    //            //{
                                                    //            //    System.IO.File.Delete(AnalystfilePath + "/" + AttachedFileList[i].LaFileName);
                                                    //            //}
                                                    //            //System.IO.File.Copy(Previouspath + "/" + AttachedFileList[i].LaFileName, AnalystfilePath + "/" + AttachedFileList[i].LaFileName);
                                                    //        }
                                                    //        if (Manager != null)
                                                    //        {
                                                    //            //Manager Copy of File is saved
                                                    //            var ManagerfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S2AS3BucketFolder"], CompanyDetails.GcCode + "/" + Manager.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth);
                                                    //            //check if directory exists or not.If not create a new one
                                                    //            //SS Migrated the below section of copying File via SDK
                                                    //            bool exists = Globals.FolderExistsInA2S(ManagerfilePath);
                                                    //            if (!exists)
                                                    //            {
                                                    //                Globals.CreateFolderInA2S(ManagerfilePath, RefFile.LrfMonth);
                                                    //            }
                                                    //            var bytes = Globals.DownloadFromS3(AttachedFileList[i].LaFileName, AttachedFileList[i].LaFilePath);
                                                    //            using (var ms = new MemoryStream(bytes))
                                                    //            {
                                                    //                Globals.UploadToA2S(ms, AttachedFileList[i].LaFileName, ManagerfilePath);
                                                    //            }
                                                    //            ////check if directory exists or not.If not create a new one
                                                    //            //var exists = System.IO.Directory.Exists(ManagerfilePath);
                                                    //            //if (!exists)
                                                    //            //{
                                                    //            //    System.IO.Directory.CreateDirectory(ManagerfilePath);
                                                    //            //}
                                                    //            ////check if same file exist then delete that file
                                                    //            //if (System.IO.File.Exists(ManagerfilePath + "/" + AttachedFileList[i].LaFileName))
                                                    //            //{
                                                    //            //    System.IO.File.Delete(ManagerfilePath + "/" + AttachedFileList[i].LaFileName);
                                                    //            //}
                                                    //            //System.IO.File.Copy(Previouspath + "/" + AttachedFileList[i].LaFileName, ManagerfilePath + "/" + AttachedFileList[i].LaFileName);
                                                    //        }
                                                    //    }

                                                    //    //Find Supporting Documents and Save Them in the corresponding Analyst and Manager path
                                                    //    var SupportingDocumentList = db.LSupportingDocuments.Where(p => p.LsdEntityId == TranId).Where(p => p.LsdEntityType == "LRefFiles").ToList();
                                                    //    for (var j = 0; j < SupportingDocumentList.Count(); j++)
                                                    //    {
                                                    //        string Previouspath = SupportingDocumentList[j].LsdFilePath;//string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyDetails.GcCode + "/RefFiles/" + RefFileType.LrftName+ "/SupportingDocument");
                                                    //        if (Analyst != null)
                                                    //        {
                                                    //            //Analyst copy of file is saved
                                                    //            var AnalystfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S2AS3BucketFolder"], CompanyDetails.GcCode + "/" + Analyst.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth + "/SupportingDocuments");
                                                    //            //check if directory exists or not.If not create a new one
                                                    //            //bool exists = System.IO.Directory.Exists(AnalystfilePath);
                                                    //            //if (!exists)
                                                    //            //{
                                                    //            //    System.IO.Directory.CreateDirectory(AnalystfilePath);
                                                    //            //}
                                                    //            //System.IO.File.Copy(Previouspath + "/" + SupportingDocumentList[j].LsdFileName, AnalystfilePath + "/" + SupportingDocumentList[j].LsdFileName);
                                                    //            //SS Migrated the below section of copying File via SDK
                                                    //            bool exists = Globals.FolderExistsInA2S(AnalystfilePath);
                                                    //            if (!exists)
                                                    //            {
                                                    //                Globals.CreateFolderInA2S(AnalystfilePath, "SupportingDocuments");
                                                    //            }
                                                    //            var bytes = Globals.DownloadFromS3(SupportingDocumentList[j].LsdFileName, SupportingDocumentList[j].LsdFilePath);
                                                    //            using (var ms = new MemoryStream(bytes))
                                                    //            {
                                                    //                Globals.UploadToA2S(ms, SupportingDocumentList[j].LsdFileName, AnalystfilePath);
                                                    //            }
                                                    //        }
                                                    //        //If manager is not present
                                                    //        if (Manager != null)
                                                    //        {
                                                    //            //Manager Copy of File is saved
                                                    //            var ManagerfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S2AS3BucketFolder"], CompanyDetails.GcCode + "/" + Manager.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth + "/SupportingDocuments");
                                                    //            ////check if directory exists or not.If not create a new one
                                                    //            //var exists = System.IO.Directory.Exists(ManagerfilePath);
                                                    //            //if (!exists)
                                                    //            //{
                                                    //            //    System.IO.Directory.CreateDirectory(ManagerfilePath);
                                                    //            //}
                                                    //            //System.IO.File.Copy(Previouspath + "/" + SupportingDocumentList[j].LsdFileName, ManagerfilePath + "/" + SupportingDocumentList[j].LsdFileName);
                                                    //            //SS Migrated the below section of copying File via SDK
                                                    //            bool exists = Globals.FolderExistsInA2S(ManagerfilePath);
                                                    //            if (!exists)
                                                    //            {
                                                    //                Globals.CreateFolderInA2S(ManagerfilePath, "SupportingDocuments");
                                                    //            }
                                                    //            var bytes = Globals.DownloadFromS3(SupportingDocumentList[j].LsdFileName, SupportingDocumentList[j].LsdFilePath);
                                                    //            using (var ms = new MemoryStream(bytes))
                                                    //            {
                                                    //                Globals.UploadToA2S(ms, SupportingDocumentList[j].LsdFileName, ManagerfilePath);
                                                    //            }
                                                    //        }
                                                    //    }

                                                    //    break;
                                                    #endregion
                                                    case "UsersCR":
                                                        var UserCRDetails = db.LChangeRequests.Where(p => p.Id == TranId).FirstOrDefault();

                                                        if (UserCRDetails.LcrAction.Equals("Delete", StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            using (var transaction = db.Database.BeginTransaction())
                                                            {
                                                                try
                                                                {
                                                                    //Added by Sachin --delete AD Account.Remove AD Account when User is terminated
                                                                    LUser Luser = db.LUsers.Where(p => p.Id == UserCRDetails.LcrRowId).FirstOrDefault();
                                                                    if (ProjectEnviournment.Equals("Prod", StringComparison.OrdinalIgnoreCase))
                                                                    {
                                                                        Globals.DeleteUser(new ADUserViewModel { Email = Luser.LuEmail });
                                                                    }

                                                                    Luser.WFStatus = "InActive";
                                                                    Luser.LuCreateLogin = false;
                                                                    Luser.LuUpdatedDateTime = DateTime.Now;
                                                                    Luser.WFComments = UserCRDetails.WFComments;
                                                                    db.Entry(Luser).State = EntityState.Modified;
                                                                    db.SaveChanges();
                                                                    //db.Database.ExecuteSqlCommand("delete from AspNetUserRoles where UserId ='" + Luser.LuUserId + "' and RoleId not in (select Id from AspNetRoles where Name='Payee' and CompanyCode ='" + Company.GcCode +"')");
                                                                    var userId = Luser.LuUserId;
                                                                    //do not do because it gives message 'User is InActive', which is confusing.Also, it creates problems if user has FinOps and Payee accounts both
                                                                    //Change AspNetUser IsActive only when WIAM is disabled
                                                                    if (!string.IsNullOrEmpty(IsWIAMEnabled) && !"yes".Equals(IsWIAMEnabled, StringComparison.OrdinalIgnoreCase))
                                                                    {
                                                                        AspNetUser AspUserdacti = db.AspNetUsers.Find(userId);
                                                                        AspUserdacti.IsActive = false;
                                                                        db.Entry(AspUserdacti).State = EntityState.Modified;
                                                                        // db.AspNetUsers.Remove(AspUser);
                                                                        db.SaveChanges();
                                                                    }
                                                                    //LPasswordHistory Phistory = db.LPasswordHistories.Where(x => x.UserId == userId).FirstOrDefault();
                                                                    //db.LPasswordHistories.Remove(Phistory);
                                                                    //db.SaveChanges();
                                                                    //Commenting on 20Aug19 by ShivaniG, As per email and discussion for sync up WIAM/non-WIAM
                                                                    //var Portfolios = db.MEntityPortfolios.Where(p => p.MepEntityType == "LUsers").Where(p => p.MepEntityId == Luser.Id);
                                                                    //db.MEntityPortfolios.RemoveRange(Portfolios);
                                                                    //db.SaveChanges();

                                                                    //db.Database.ExecuteSqlCommand("Update  from XSchema.XReportUsers where UserId ='" + Luser.LuUserId + "'");
                                                                    // db.Database.ExecuteSqlCommand("Update  XSchema" + Company.GcCode.ToUpper() + ".XReportUsers set  XUserEmailID = XUserEmailID + 'X' where XUserEmailID=" + Luser.LuEmail + ")");

                                                                    transaction.Commit();
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    transaction.Rollback();

                                                                }
                                                            }

                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        break;
                                        //case "Reject":

                                        //    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, SPresult));//type 2 error
                    }
                }
                catch (DbEntityValidationException dbex)
                {

                    var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                    throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
                }
                catch (Exception ex)
                {

                    throw ex;//This exception will be handled in FilterConfig's CustomHandler
                }

            }
            return Ok();
        }

        //This method is called when we update WfSatatus in any Workflow against any transaction
        //[HttpGet]
        //public IHttpActionResult UpdateActionStatus(string Action, string WorkFlowName, int TransactionId, int CompanyId, string LoggedInUserId, string Comments)
        //{

        //    var Next_User = "";//Next Owner Name
        //    var NextRole = "";//NextRole in Workflow
        //    using (var transaction = db.Database.BeginTransaction())
        //    {
        //        //get work flow name based on Workflow name
        //        var WorkFlow = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkFlowName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        //        var Company = db.GCompanies.Find(CompanyId);
        //        var WFColumns = db.Database.SqlQuery<WFColumnsViewModel>("select WFRequesterId,WFAnalystId,WFManagerId,WFOrdinal,WFCurrentOwnerId,WFStatus,WFType,WFRequesterRoleId,WFCompanyId,WFComments from " + WorkFlow.RwfBaseTableName + " where Id=" + TransactionId).FirstOrDefault();
        //        switch (Action)
        //        {
        //            case "Approve":
        //                //Allow action only if Current Owner Id is equal to LoggedIn UserId
        //                if (WFColumns.WFCurrentOwnerId == LoggedInUserId)
        //                {
        //                    //##  Move the transaction to next ordinal (if there is no next ordinal (i.e. it was the last one) then change WFStatus to "Completed")
        //                    //##  Find the role at next ordinal and if multiple people found at the same role then find the most appropriate current owner
        //                    //##  (usig portfolio matching, if still multiple owner found then the one who is least loaded) and update CurrentOwner to this user
        //                    if (WfIsLastOrdinal(WorkFlow.Id, TransactionId, CompanyId))
        //                    {
        //                        //Add switch statement of workflows to cater different situations of approvals
        //                        // Then WFStatus = 'Completed'
        //                        if (WorkFlow.RwfBaseTableName == "LRefFiles")
        //                        {

        //                            db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFComments=WFComments+CHAR(13)+'" + Comments + "', WFStatus = 'Completed', LrfUPdatedById= '" + LoggedInUserId + "', LrfUpdatedDateTime = GETDATE() where Id=" + TransactionId);
        //                        }
        //                        else
        //                        {
        //                            db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFComments=WFComments+CHAR(13)+'" + Comments + "', WFStatus = 'Completed' where Id=" + TransactionId);
        //                        }

        //                        switch (WorkFlow.RwfName)
        //                        {
        //                            case "ManualAdjustments":
        //                                db.Database.ExecuteSqlCommand("insert into XSchema.XBatches ([XCompanyCode],[XUpdatedBy],[XBatchNumber],[XBatchType],[XRawDataType],[XStatus],[XUploadStartDateTime],[XUploadFinishDateTime],[XRecordCount],[XAlteryxBatchNumber],[XComments],[XCommissionPeriod],[XPrimaryChannel],[XBusinessUnit],[XChannel]) select (select GcCode from GCompanies where Id=LbCompanyId),null,[LbBatchNumber],[LbBatchType],null,[LbStatus],[LbUploadStartDateTime],[LbUploadFinishDateTime],[LbRecordCount],[LbAlteryxBatchNumber],[LbComments],[LbCommissionPeriod],[LbPrimaryChannel],[LbBusinessUnit],[LbChannel] from LBatches where Id =" + TransactionId);
        //                                db.Database.ExecuteSqlCommand("insert into XSchema" + Company.GcCode.ToUpper() + ".XCalc ([XBatchNumber],[XAlteryxTransactionNumber],[XPrimaryChannel],[XPayee],[XParentPayee],[XOrderDate],[XConnectionDate],[XTerminationDate],[XSubscriberNumber],[XBAN],[XActivityType],[XPlanDescrition],[XProductCode],[XUpgradeCode],[XIMEI],[XDevieCode],[XDeviceType],[XCommType],[XContractDuration],[XContractId],[XCommAmtExTax],[XTax],[XCommAmtIncTax],[XComments]) select  [LcSOSBatchNumber],[LcAlteryxTransactionNumber],[LcPrimaryChannel],[LcPayee],[LcParentPayee],[LcOrderDate],[LcConnectionDate],[LcTerminationDate],[LcSubscriberNumber],[LcBAN],[LcActivityType],[LcPlanDescrition],[LcProductCode],[LcUpgradeCode],[LcIMEI],[LcDevieCode],[LcDeviceType],[LcCommType],[LcContractDuration],[LcContractId],[LcCommAmtExTax],[LcTax],[LcCommAmtIncTax],[LcComments] from LCalc where LcSOSBatchNumber=(select LbbatchNumber from LBatches where Id=" + TransactionId + ")");
        //                                break;
        //                            case "Users":
        //                                var User = db.LUsers.Find(TransactionId);
        //                                var AspUser = db.AspNetUsers.Find(User.LuUserId);
        //                                AspUser.IsActive = true;

        //                                db.Entry(AspUser).State = EntityState.Modified;
        //                                db.SaveChanges();
        //                                //******************************************AD changes
        //                                //Random password generator code used for creating AD account ---- commenting as now
        //                                //  RandomPassword pwd = new RandomPassword();
        //                                //  string randompwd = pwd.Generate();

        //                                ADUserViewModel model = new ADUserViewModel();
        //                                model.Email = AspUser.Email;
        //                                model.Password = ConfigurationManager.AppSettings["DefaultUserPassword"];
        //                                Globals.CreateUser(model);

        //                                //Commented as we need to send email to created user - discussed with JS
        //                                /* if (User.LuBlockNotification)//If block notification is set to true then user will not receive email instead created by user will receive email
        //                                 {
        //                                     var CreatedByEmail = db.AspNetUsers.Where(p => p.Id == User.LuCreatedById).FirstOrDefault().UserName;
        //                                     //Send Welcome Email to Created By User
        //                                     Globals.SendEmail(CreatedByEmail, null, "Vodafone LITE", "This is to inform you that You are approved as a User in Vodafone Lite . You can Login using your email address as your username . Password will be sent separately.");
        //                                     Globals.SendEmail(CreatedByEmail, null, "Vodafone LITE", "Your Vodafone LITE Password is " + ConfigurationManager.AppSettings["DefaultUserPassword"]);
        //                                 }
        //                                 else
        //                                 {*/
        //                                //Send Welcome Email to Approved User
        //                                //##Temp added by Shubham till Shivani fixes email functionality
        //                                Globals.SendEmail(AspUser.Email, null, "Vodafone LITE", "This is to inform you that You are approved as a User in Vodafone Lite . You can Login using your email address as your username . Password will be sent separately.", Company.GcCode);
        //                                Globals.SendEmail(AspUser.Email, null, "Vodafone LITE", "Your Vodafone LITE Password is " + ConfigurationManager.AppSettings["DefaultUserPassword"], Company.GcCode);
        //                                // }
        //                                break;
        //                            case "UsersCR":
        //                                //Code to Update Base table details if change request is approved
        //                                var LChangeRequest = db.LChangeRequests.Find(TransactionId);
        //                                string sqlQuery = "";
        //                                switch (LChangeRequest.LcrAction)
        //                                {
        //                                    case "Edit":
        //                                        switch (LChangeRequest.LcrColumnName)
        //                                        {
        //                                            case "Roles":
        //                                                var LUser = db.LUsers.Find(LChangeRequest.LcrRowId);
        //                                                if (!string.IsNullOrEmpty(LChangeRequest.LcrOldValue))
        //                                                {
        //                                                    db.Database.ExecuteSqlCommand("delete from AspNetUserRoles where UserId ='" + LUser.LuUserId + "'");
        //                                                    //var ExistingRoles = UserManager.GetRoles(LUser.LuUserId).ToArray();
        //                                                    //UserManager.RemoveFromRoles(LUser.LuUserId, ExistingRoles);
        //                                                }
        //                                                if (!string.IsNullOrEmpty(LChangeRequest.LcrNewValue))
        //                                                {
        //                                                    string[] RoleList = LChangeRequest.LcrNewValue.Split(',').ToArray();//Update user roles
        //                                                    foreach (var role in RoleList)
        //                                                    {
        //                                                        db.Database.ExecuteSqlCommand("insert into AspNetUserRoles(UserId,RoleId) values('" + LUser.LuUserId + "',(select Id from AspNetRoles where Name='" + role + "' and CompanyCode='" + Company.GcCode + "'))");
        //                                                    }
        //                                                    //UserManager.AddToRoles(LUser.LuUserId, RoleList);
        //                                                }
        //                                                break;
        //                                            case "LuReportsToId":
        //                                                sqlQuery = "Update LUsers set " + LChangeRequest.LcrColumnName + " = '" + LChangeRequest.LcrNewId + "' where Id=" + LChangeRequest.LcrRowId;
        //                                                db.Database.ExecuteSqlCommand(sqlQuery);
        //                                                break;
        //                                            default:
        //                                                sqlQuery = "Update LUsers set " + LChangeRequest.LcrColumnName + " = '" + LChangeRequest.LcrNewValue + "' where Id=" + LChangeRequest.LcrRowId;
        //                                                db.Database.ExecuteSqlCommand(sqlQuery);
        //                                                break;
        //                                        }

        //                                        break;
        //                                    case "Delete":
        //                                        sqlQuery = "Update LUsers set LuStatus = InActive where Id=" + LChangeRequest.LcrRowId;
        //                                        db.Database.ExecuteSqlCommand(sqlQuery);
        //                                        sqlQuery = "Update AspnetUsers set IsActive = 0 where Id=( select LuUserId from LUsers where Id =" + LChangeRequest.LcrRowId;
        //                                        db.Database.ExecuteSqlCommand(sqlQuery);
        //                                        break;
        //                                }
        //                                break;
        //                            case "PayeesCR":
        //                                //update Base table records using Stored Procedure
        //                                var ItemId = TransactionId.ToString();
        //                                db.SpUpdateChangeRequestData(ItemId, "Approved", null, LoggedInUserId, DateTime.UtcNow);
        //                                break;
        //                            case "Payees":
        //                                var PayeeDetails = db.LPayees.Find(TransactionId);
        //                                //check if Authorised payee verification is set to true else throw error message to user
        //                                if (PayeeDetails.LpAuthorisedPayeeVerification)
        //                                {
        //                                    var user = new ApplicationUser() { UserName = PayeeDetails.LpEmail, Email = PayeeDetails.LpEmail, GcCompanyId = CompanyId, IsActive = true };//Is Active flag is set to false if a user is not approved from manager
        //                                    IdentityResult result = UserManager.Create(user, ConfigurationManager.AppSettings["DefaultUserPassword"]);
        //                                    //lines added by shubham to add roles to user after registering User Using Sql Query
        //                                    var RegUser = UserManager.FindByEmail(user.Email);
        //                                    //SS NEW CODE Check if any role exist for this user
        //                                    if (UserManager.GetRoles(RegUser.Id).Count() == 0)
        //                                    {
        //                                        db.Database.ExecuteSqlCommand("insert into AspNetUserRoles(UserId,RoleId) values('" + RegUser.Id + "',(select Id from AspNetRoles where Name='Payee' and CompanyCode='" + Company.GcCode + "'))");
        //                                    }
        //                                    //UserManager.AddToRole(RegUser.Id, role);//user manager method has stopped working since opco column has beed added in aspnet roles
        //                                    //update UserId in LPayees
        //                                    PayeeDetails.LpUserId = RegUser.Id;
        //                                    db.Entry(PayeeDetails).State = EntityState.Modified;
        //                                    db.SaveChanges();
        //                                    //Create AD User if Create login is true
        //                                    if (PayeeDetails.LpCreateLogin)
        //                                    {
        //                                        //***********************AD changes
        //                                        //commenting random password now and setting Default password as of now.
        //                                        // RandomPassword rp = new RandomPassword();
        //                                        //  string rndPwd = rp.Generate();
        //                                        ADUserViewModel ADmodel = new ADUserViewModel();
        //                                        ADmodel.Email = user.Email;
        //                                        ADmodel.Password = ConfigurationManager.AppSettings["DefaultUserPassword"];
        //                                        Globals.CreateUser(ADmodel);

        //                                        //need to send pwd in separate mail
        //                                        //send Email to give id and password

        //                                        Globals.SendEmail(PayeeDetails.LpEmail, null, "Vodafone LITE", "Welcome to Vodafone LITE your access to the Commission Portal.You have now been approved into system and you can access system from your Start Date :" + PayeeDetails.LpEffectiveStartDate.Date + ".Please use following credentials to Login" + Environment.NewLine + "Url:test.Vodafonelite.com " + Environment.NewLine + "Email : " + PayeeDetails.LpEmail + Environment.NewLine + " Password :Password will be shared separately.'" + Environment.NewLine + "You will be asked to change your password upon first login." + Environment.NewLine + "If you forget your password click on 'Forget password link'", Company.GcCode);
        //                                        Globals.SendEmail(PayeeDetails.LpEmail, null, "Vodafone LITE", "Your Vodafone LITE Password is " + ADmodel.Password, Company.GcCode);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Payee needs to be authorised by Analyst before final approval"));
        //                                }

        //                                break;
        //                            case "Calc":
        //                                //Update LBatch .. Comments are not used in current WF design
        //                                var SosBatch = db.LBatches.Where(p => p.Id == TransactionId).FirstOrDefault();
        //                                SosBatch.LbStatus = "CalcApproved";
        //                                db.Entry(SosBatch).State = EntityState.Modified;
        //                                db.SaveChanges();

        //                                //update XBatches table so that Alteryx can monitor the progress. 
        //                                var XBatch = db.XBatches.Where(p => p.XBatchNumber == SosBatch.LbBatchNumber).FirstOrDefault();
        //                                XBatch.XStatus = "CalcApproved";
        //                                db.Entry(XBatch).State = EntityState.Modified;
        //                                db.SaveChanges();
        //                                break;
        //                            case "RawData":
        //                                //Update LBatch .. Comments are not used in current WF design
        //                                var Batch = db.LBatches.Where(p => p.Id == TransactionId).FirstOrDefault();
        //                                Batch.LbStatus = "RawDataApproved";
        //                                Batch.WFStatus = "Completed";
        //                                db.Entry(Batch).State = EntityState.Modified;
        //                                db.SaveChanges();

        //                                //update XBatches table so that Alteryx can monitor the progress. 
        //                                var XBatchDetails = db.XBatches.Where(p => p.XBatchNumber == Batch.LbBatchNumber).FirstOrDefault();
        //                                XBatchDetails.XStatus = "RawDataApproved";
        //                                db.Entry(XBatchDetails).State = EntityState.Modified;
        //                                db.SaveChanges();
        //                                break;

        //                            case "RefFiles":
        //                                var RefFile = db.LRefFiles.Where(p => p.Id == TransactionId).FirstOrDefault();
        //                                var Analyst = db.AspNetUsers.Find(RefFile.WFAnalystId);
        //                                var Manager = db.AspNetUsers.Find(RefFile.WFManagerId);
        //                                var RefFileType = db.LRefFileTypes.Find(RefFile.LrfRefFileTypeId);
        //                                var CompanyDetails = db.GCompanies.Find(RefFile.LrfCompanyId);
        //                                var AttachedFileList = db.LAttachments.Where(p => p.LaEntityId == TransactionId).Where(p => p.LaEntityType == "LRefFiles").ToList();
        //                                for (var i = 0; i < AttachedFileList.Count(); i++)
        //                                {
        //                                    string Previouspath = AttachedFileList[i].LaFilePath;//string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyDetails.GcCode + "/RefFiles/" + RefFileType.LrftName);
        //                                    if (Analyst != null)
        //                                    {
        //                                        //Analyst copy of file is saved
        //                                        var AnalystfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_S2ACommomFilePath"], CompanyDetails.GcCode + "/" + Analyst.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth);
        //                                        //check if directory exists or not.If not create a new one
        //                                        bool exists = System.IO.Directory.Exists(AnalystfilePath);
        //                                        if (!exists)
        //                                        {
        //                                            System.IO.Directory.CreateDirectory(AnalystfilePath);
        //                                        }
        //                                        System.IO.File.Copy(Previouspath + "/" + AttachedFileList[i].LaFileName, AnalystfilePath + "/" + AttachedFileList[i].LaFileName);
        //                                    }
        //                                    if (Manager != null)
        //                                    {
        //                                        //Manager Copy of File is saved
        //                                        var ManagerfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_S2ACommomFilePath"], CompanyDetails.GcCode + "/" + Manager.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth);
        //                                        //check if directory exists or not.If not create a new one
        //                                        var exists = System.IO.Directory.Exists(ManagerfilePath);
        //                                        if (!exists)
        //                                        {
        //                                            System.IO.Directory.CreateDirectory(ManagerfilePath);
        //                                        }
        //                                        System.IO.File.Copy(Previouspath + "/" + AttachedFileList[i].LaFileName, ManagerfilePath + "/" + AttachedFileList[i].LaFileName);
        //                                    }
        //                                }

        //                                //Find Supporting Documents and Save Them in the corresponding Analyst and Manager path
        //                                var SupportingDocumentList = db.LSupportingDocuments.Where(p => p.LsdEntityId == TransactionId).Where(p => p.LsdEntityType == "LRefFiles").ToList();
        //                                for (var j = 0; j < SupportingDocumentList.Count(); j++)
        //                                {
        //                                    string Previouspath = SupportingDocumentList[j].LsdFilePath;//string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_SosFilePath"], CompanyDetails.GcCode + "/RefFiles/" + RefFileType.LrftName+ "/SupportingDocument");
        //                                    if (Analyst != null)
        //                                    {
        //                                        //Analyst copy of file is saved
        //                                        var AnalystfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_S2ACommomFilePath"], CompanyDetails.GcCode + "/" + Analyst.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth + "/SupportingDocuments");
        //                                        //check if directory exists or not.If not create a new one
        //                                        bool exists = System.IO.Directory.Exists(AnalystfilePath);
        //                                        if (!exists)
        //                                        {
        //                                            System.IO.Directory.CreateDirectory(AnalystfilePath);
        //                                        }
        //                                        System.IO.File.Copy(Previouspath + "/" + SupportingDocumentList[j].LsdFileName, AnalystfilePath + "/" + SupportingDocumentList[j].LsdFileName);
        //                                    }
        //                                    //If manager is not present
        //                                    if (Manager != null)
        //                                    {
        //                                        //Manager Copy of File is saved
        //                                        var ManagerfilePath = string.Format("{0}/{1}", ConfigurationManager.AppSettings["S3_S2ACommomFilePath"], CompanyDetails.GcCode + "/" + Manager.UserName + "/RefFiles/" + RefFileType.LrftName + "/" + RefFile.LrfYear + "/" + RefFile.LrfMonth + "/SupportingDocuments");
        //                                        //check if directory exists or not.If not create a new one
        //                                        var exists = System.IO.Directory.Exists(ManagerfilePath);
        //                                        if (!exists)
        //                                        {
        //                                            System.IO.Directory.CreateDirectory(ManagerfilePath);
        //                                        }
        //                                        System.IO.File.Copy(Previouspath + "/" + SupportingDocumentList[j].LsdFileName, ManagerfilePath + "/" + SupportingDocumentList[j].LsdFileName);
        //                                    }
        //                                }

        //                                break;
        //                            case "Claims":
        //                                //Queries commented by shubham as JS parked this processs of inserting values from LClaims to LCalc and XCalc
        //                                //insert into LCalc(LcOpCoCode, LcSource, LcAdjustmenCode, LcSOSBatchNumber, LcAlteryxTransactionNumber, LcPrimaryChannel, LcPayee, LcParentPayee, LcOrderDate, LcConnectionDate, LcTerminationDate, LcSubscriberNumber, LcBAN, LcActivityType, LcPlanDescrition, LcProductCode, LcUpgradeCode, LcIMEI, LcDevieCode, LcDeviceType, LcCommType, LcContractDuration, LcContractId, LcCommAmtExTax, LcTax, LcCommAmtIncTax, LcComments) select 'QA','Claim',null,null,null,null,LcPayeeCode,LcParentCode,LcOrderDate,LcConnectionDate,null,null,LcBAN,(select RatName from RActivityTypes where Id = LcActivityTypeId),null,LcProductCode,null,LcIMEI,null,(select RdtName from RDeviceTypes where Id = LcDeviceTypeId),(select RctName from RCommissionTypes where Id = LcCommissionTypeId),null,null,LcExpectedCommissionAmount,null,null,WFComments from LClaims where Id = 28
        //                                //insert into XSchemaQA.XCalc(XBatchNumber, XAlteryxTransactionNumber, XPrimaryChannel, XPayee, XParentPayee, XOrderDate, XConnectionDate, XTerminationDate, XSubscriberNumber, XBAN, XActivityType, XPlanDescrition, XProductCode, XUpgradeCode, XIMEI, XDevieCode, XDeviceType, XCommType, XContractDuration, XContractId, XCommAmtExTax, XTax, XCommAmtIncTax, XComments) select null,null,null,LcPayeeCode,LcParentCode,LcOrderDate,LcConnectionDate,null,null,LcBAN,(select RatName from RActivityTypes where Id = LcActivityTypeId),null,LcProductCode,null,LcIMEI,null,(select RdtName from RDeviceTypes where Id = LcDeviceTypeId),(select RctName from RCommissionTypes where Id = LcCommissionTypeId),null,null,LcExpectedCommissionAmount,null,null,WFComments from LClaims where Id = 28
        //                                break;
        //                            case "Schemes":
        //                                var Scheme = db.LSchemes.Find(TransactionId);
        //                                if (!Scheme.LsIsSchemeTested)//return error if scheme is not tested
        //                                {
        //                                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "This Scheme has not been tested yet. Please ask Analyst to attach test results before this Scheme can be approved"));
        //                                }
        //                                break;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //Else  
        //                        //Find what is the Current_WFOrdinal for that Transaction ID in the base table of RWorkFlow (based on workflow ID ) 
        //                        //## Increase the ordinal
        //                        //   update transaction set WFOrdinal = WFOrdinal + 1
        //                        //RK19/042017 hardcoaded condition for LRefFiles for the timebeing,need to fix the below region
        //                        if (WorkFlow.RwfBaseTableName == "LRefFiles")
        //                        {
        //                            db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFComments=WFComments+CHAR(13)+'" + Comments + "', WFOrdinal = WFOrdinal + 1,WFStatus = 'InProgress', LrfUPdatedById= '" + LoggedInUserId + "', LrfUpdatedDateTime = GETDATE() where Id=" + TransactionId);
        //                        }
        //                        else
        //                        {
        //                            db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFComments=WFComments+CHAR(13)+'" + Comments + "', WFOrdinal = WFOrdinal + 1,WFStatus = 'InProgress' where Id=" + TransactionId);
        //                        }

        //                        #region UpdateWhoColumns
        //                        //string strUpdatedByColumnName = "";
        //                        //string strUpdatedDateTimeColumnName = "";
        //                        //strUpdatedByColumnName = db.Database.SqlQuery<string>("select isnull(COLUMN_NAME,'') from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + WorkFlow.RwfBaseTableName + "' and lower(COLUMN_NAME) like '%updateby%'").FirstOrDefault<string>();
        //                        //strUpdatedDateTimeColumnName = db.Database.SqlQuery<string>("select isnull(COLUMN_NAME,'') from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + WorkFlow.RwfBaseTableName + "' and lower(COLUMN_NAME) like '%updateddatetime%'").FirstOrDefault<string>();
        //                        //if (strUpdatedByColumnName != "") db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set " + strUpdatedByColumnName + " = '" + LoggedInUserId + "' where Id=" + TransactionId);
        //                        //if (strUpdatedDateTimeColumnName != "") db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set " + strUpdatedDateTimeColumnName + " = GETDATE()" + " where Id=" + TransactionId);
        //                        #endregion
        //                        //Find Current Config data as per transactionId of Base table
        //                        //Replce select * from the selected column List from database which are required and make a new view model having required attributes
        //                        //## Replace section below with FnGetUserForAllocation
        //                        var ConfigData = db.Database.SqlQuery<LWorkFlowConfig>("select * from LWorkFlowConfig where LwfcWorkFlowId=" + WorkFlow.Id + " and LwfcCompanyId=" + CompanyId + " and  LwfcOrdinalNumber= (select WFOrdinal from " + WorkFlow.RwfBaseTableName + " where Id=" + TransactionId + ")").FirstOrDefault();
        //                        NextRole = ConfigData.LwfcActingAs;
        //                        var SkipFlag = ConfigData.LwfcSkip;
        //                        //Check Skip flag is true to skip the current ordinal 
        //                        if (SkipFlag)
        //                        {
        //                            //Execute the database skip function if it is present 
        //                            if (!string.IsNullOrEmpty(ConfigData.LwfcSkipFunctionName))
        //                            {
        //                                SkipFlag = db.Database.SqlQuery<bool>("select dbo." + ConfigData.LwfcSkipFunctionName + "()").FirstOrDefault<bool>();
        //                            }
        //                        }
        //                        //Use Do While Skip flag to call above commands multiple times
        //                        //So far skip flag is still true then call this function recursively to go to next level
        //                        while (SkipFlag)
        //                        {
        //                            SkipFlag = SkipOrdinal(WorkFlow.RwfBaseTableName, TransactionId, WorkFlow.Id, CompanyId);
        //                            //transaction.Commit();
        //                            //transaction.Dispose();
        //                            //UpdateActionStatus(Action, WorkFlowName,TransactionId,CompanyId,LoggedInUserId);
        //                        };

        //                        //   ## Set current owner
        //                        if (NextRole.Equals("Analyst", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            //find analyst id 
        //                            Next_User = db.Database.SqlQuery<string>("select WFAnalystId from " + WorkFlow.RwfBaseTableName + " BT inner join AspNetUsers AU on BT.WFAnalystId=AU.Id where BT.Id=" + TransactionId + " and AU.IsActive=1").FirstOrDefault<string>();
        //                        }
        //                        else if (NextRole.Equals("Manager", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            //find manager id 
        //                            Next_User = db.Database.SqlQuery<string>("select WFManagerId from " + WorkFlow.RwfBaseTableName + " BT inner join AspNetUsers AU on BT.WFManagerId=AU.Id  where WFManagerId!=WFRequesterId and  BT.Id=" + TransactionId + " and AU.IsActive=1").FirstOrDefault<string>();
        //                        }
        //                        if (string.IsNullOrEmpty(Next_User))//If still User Id is not found search for next suitable owner
        //                        {
        //                            //Find suitable owner if Next owner(Analyst/Manager) is not defined or is null
        //                            Next_User = WFFindSuitableOwner(WorkFlow.Id, TransactionId, CompanyId, NextRole);
        //                            //RK commented the above statement to find next user from WFFindSuitableOwner, now getting the next_user from db function.
        //                            //## COmmented by SS because it is returning No match found where as system accepts only Null or an AspnetUserId. Also the Ordinal should be IOrdinal+1 (TargetOrdinal)
        //                            //int iCurrentOrdinal = db.Database.SqlQuery<int>("select WFOrdinal from " + WorkFlow.RwfBaseTableName + " where id = " + TransactionId.ToString() + "").FirstOrDefault<int>();
        //                            //Next_User = db.Database.SqlQuery<string>("select dbo.FNGetUserForAllocation('" + WorkFlow.RwfBaseTableName + "', '" + WorkFlow.RwfWFType + "' , " + TransactionId.ToString() + " , " + iCurrentOrdinal.ToString() + " , " + CompanyId.ToString() + ") ").FirstOrDefault<string>();// (WorkFlow.Id, TransactionId, CompanyId, NextRole);
        //                            if (string.IsNullOrEmpty(Next_User))//if still next user is not found set current owner id to null and commit the transaction
        //                            {
        //                                db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId = null where Id=" + TransactionId);
        //                                //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "No suitable Owner found for next level"));
        //                            }

        //                            else
        //                            {

        //                                db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId = '" + Next_User + "'  where Id=" + TransactionId);

        //                                //Fill Analyst/manager Id 
        //                                if (NextRole.Equals("Analyst", StringComparison.OrdinalIgnoreCase))
        //                                {
        //                                    db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFAnalystId='" + Next_User + "' where Id=" + TransactionId);
        //                                }
        //                                else if (NextRole.Equals("Manager", StringComparison.OrdinalIgnoreCase))
        //                                {
        //                                    db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFManagerId='" + Next_User + "' where Id=" + TransactionId);
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId = '" + Next_User + "'  where Id=" + TransactionId);
        //                        }
        //                    }
        //                }
        //                break;
        //            case "Reject":
        //                //Allow action only if current owner is equal to logged in user
        //                if (WFColumns.WFCurrentOwnerId == LoggedInUserId)
        //                {
        //                    //update transaction set WFStatus = Rejected
        //                    db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFComments=WFComments+CHAR(13)+'" + Comments + "', WFStatus = 'Rejected' where Id=" + TransactionId);
        //                }
        //                break;
        //            case "Withdraw":
        //                if (WFColumns.WFRequesterId == LoggedInUserId)
        //                {
        //                    //update transaction set WFStatus = Withdrawn
        //                    //db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFStatus = 'Withdrawn' where Id=" + TransactionId);
        //                    db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFComments=WFComments+CHAR(13)+'" + Comments + "', WFCurrentOwnerId =  WFRequesterId ,WFOrdinal=(select LwfcOrdinalNumber from LWorkFlowConfig where LwfcRoleId=WFRequesterRoleId and LwfcCompanyId=" + CompanyId + " and LwfcWorkFlowId=" + WorkFlow.Id + " ) where Id=" + TransactionId);
        //                }
        //                break;
        //            case "SendToRequester":
        //                //update transaction set WFCurrentOwnerId =  WFRequesterId(opco,acting as,workflowid)
        //                // db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFCurrentOwnerId =  WFRequesterId ,WFOrdinal=(select LwfcOrdinalNumber from LWorkFlowConfig where LwfcActingAs='Requester' and LwfcCompanyId=" + CompanyId + " and LwfcWorkFlowId=" + WorkFlow.Id + " ) where Id=" + TransactionId);
        //                db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFComments=WFComments+CHAR(13)+'" + Comments + "', WFCurrentOwnerId =  WFRequesterId ,WFOrdinal=(select LwfcOrdinalNumber from LWorkFlowConfig where LwfcRoleId=WFRequesterRoleId and LwfcCompanyId=" + CompanyId + " and LwfcWorkFlowId=" + WorkFlow.Id + " ) where Id=" + TransactionId);
        //                break;
        //            case "SendToAnalyst":
        //                //update transaction set WFCurrentOwnerId =  WFAnalystId
        //                db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFComments=WFComments+CHAR(13)+'" + Comments + "', WFCurrentOwnerId =  WFAnalystId ,WFOrdinal=(select LwfcOrdinalNumber from LWorkFlowConfig where LwfcActingAs='Analyst' and LwfcCompanyId=" + CompanyId + " and LwfcWorkFlowId=" + WorkFlow.Id + " ) where Id=" + TransactionId);
        //                break;
        //            case "SendToManager":
        //                //Find the ordinal from WFConfig where ActingAS = "Manager" Set WFOrdinal of the transaction to that ordinal number
        //                //update transaction set WFCurrentOwnerId =  WFManagerId
        //                db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFComments=WFComments+CHAR(13)+'" + Comments + "', WFCurrentOwnerId =  WFManagerId,WFOrdinal=(select LwfcOrdinalNumber from LWorkFlowConfig where LwfcActingAs='Manager' and LwfcCompanyId=" + CompanyId + " and LwfcWorkFlowId=" + WorkFlow.Id + " ) where Id=" + TransactionId);
        //                break;
        //            case "Prelim":
        //                //Allow action only if current owner is equal to logged in user
        //                if (WFColumns.WFCurrentOwnerId == LoggedInUserId && !WFColumns.WFStatus.Equals("Prelim", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    switch (WorkFlow.RwfName)
        //                    {
        //                        case "Calc":
        //                            //Update LBatch
        //                            var SosBatch = db.LBatches.Where(p => p.Id == TransactionId).FirstOrDefault();
        //                            SosBatch.WFStatus = "Prelim";
        //                            SosBatch.WFComments = SosBatch.WFComments + Environment.NewLine + Comments;
        //                            db.Entry(SosBatch).State = EntityState.Modified;
        //                            db.SaveChanges();

        //                            //update XBatches table so that Alteryx can monitor the progress. 
        //                            var XBatch = db.XBatches.Where(p => p.XBatchNumber == SosBatch.LbBatchNumber).FirstOrDefault();
        //                            XBatch.XStatus = "Prelim";
        //                            db.Entry(XBatch).State = EntityState.Modified;
        //                            db.SaveChanges();
        //                            break;
        //                    }
        //                }
        //                break;
        //            case "ReClaim":
        //                if (WFColumns.WFRequesterId == LoggedInUserId)
        //                {
        //                    //update transaction set WFStatus = saved Current oner=Requester
        //                    db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set WFComments=WFComments+CHAR(13)+'" + Comments + "',WFStatus='Saved',LcIsReclaim=1, WFCurrentOwnerId =  WFRequesterId ,WFOrdinal=(select LwfcOrdinalNumber from LWorkFlowConfig where LwfcRoleId=WFRequesterRoleId and LwfcCompanyId=" + CompanyId + " and LwfcWorkFlowId=" + WorkFlow.Id + " ) where Id=" + TransactionId);
        //                }
        //                break;
        //            case "VerifyPayee":
        //                db.Database.ExecuteSqlCommand("update " + WorkFlow.RwfBaseTableName + " set  WFComments=WFComments+CHAR(13)+'" + Comments + "', LpAuthorisedPayeeVerification=1 where Id=" + TransactionId);
        //                break;
        //        }
        //        //For every action (Approve, Reject, Withdraw, Cancel, SendToRequester, SendToAnalyst, SendToManager), there will be 2 emails
        //        //1) Requester
        //        //2) New CurrentOwner
        //        if (string.IsNullOrEmpty(Next_User) && string.IsNullOrEmpty(NextRole))
        //        {
        //            NextRole = db.Database.SqlQuery<string>("select LwfcActingAs from LWorkFlowConfig where LwfcOrdinalNumber= (select WFOrdinal from " + WorkFlow.RwfBaseTableName + " where Id=" + TransactionId + ")").FirstOrDefault<string>();
        //            Next_User = db.Database.SqlQuery<string>("select WFCurrentOwnerId from " + WorkFlow.RwfBaseTableName + " where Id=" + TransactionId).FirstOrDefault<string>();
        //        }
        //        transaction.Commit();
        //        //RK Commented to test
        //        //WFNotifyEmail(WorkFlowName, CompanyId, TransactionId, Action, Next_User, NextRole);
        //    }

        //    return Ok();
        //}

        //This method is defined to to check update transaction and check for  skip flag at every level.
        private bool SkipOrdinal(string BaseTableName, int TransactionId, int WorkflowId, int CompanyId)
        {
            //## Increase the ordinal
            //   update transaction set WFOrdinal = WFOrdinal + 1
            db.Database.ExecuteSqlCommand("update " + BaseTableName + " set WFOrdinal = WFOrdinal + 1,WFStatus = 'InProgress' where Id=" + TransactionId);
            //Find Current Config data as per transactionId of Base table
            //Replce select * from the selected column List from database which are required and make a new view model having required attributes
            var ConfigData = db.Database.SqlQuery<LWorkFlowConfig>("select * from LWorkFlowConfig where LwfcWorkFlowId=" + WorkflowId + " and LwfcCompanyId=" + CompanyId + " and  LwfcOrdinalNumber= (select WFOrdinal from " + BaseTableName + " where Id=" + TransactionId + ")").FirstOrDefault();
            var SkipFlag = ConfigData.LwfcSkip;
            //Check Skip flag is true to skip the current ordinal 
            if (SkipFlag)
            {
                //Execute the database skip function if it is present 
                if (!string.IsNullOrEmpty(ConfigData.LwfcSkipFunctionName))
                {
                    SkipFlag = db.Database.SqlQuery<bool>("select dbo." + ConfigData.LwfcSkipFunctionName + "()").FirstOrDefault<bool>();
                }
            }

            return SkipFlag;

        }

        //    /*This emailing functionality will be achieved through a global function. It will be called from the UpdateActionStatus method which has information about the Workflow ID, OpCo and Request ID*/
        //    public bool WFNotifyEmail(string Workflow, int CompanyId, int TransactionId, string Action, string NextUser, string NextRole)
        //    {
        //        try
        //        {
        //            var Company = db.GCompanies.Find(CompanyId);
        //            var NextUserName = db.LUsers.Where(p => p.LuUserId == NextUser).Select(p => new { FullName = p.LuFirstName + " " + p.LuLastName }).FirstOrDefault().FullName;
        //            var NextRoleName = NextRole; //db.AspNetRoles.Where(p=>p.CompanyCode==Company.GcCode).Where(p=>p.Name).Name;
        //            var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow)).FirstOrDefault();
        //            var RequestIdentifier = "";
        //            var ActionLabel = "";
        //            /*<Request Identifier> = 
        //Case WorkFlowName = Payees, Users, PayeesCR, UsersCR then Fname + Lname
        //Case WorkFlowName = RawData, Calc, Pay, ManualAdjustments then BatchNumber
        //Case WorkFlowName = RawDataCR then BatchNumber-AlteryxTransactionNumber*/
        //            switch (Workflow)
        //            {
        //                case "Payees":
        //                    RequestIdentifier = db.Database.SqlQuery<string>("select (LpFirstName+' '+LpLastName) from " + WFDetails.RwfBaseTableName + " where Id=" + TransactionId).FirstOrDefault<string>();
        //                    break;
        //                case "PayeesCR":
        //                    RequestIdentifier = db.Database.SqlQuery<string>("select (LpFirstName+' '+LpLastName) from " + WFDetails.RwfWFType + " where Id=" + TransactionId).FirstOrDefault<string>();
        //                    break;
        //                case "Users":
        //                    RequestIdentifier = db.Database.SqlQuery<string>("select (LuFirstName+' '+LuLastName) from " + WFDetails.RwfBaseTableName + " where Id=" + TransactionId).FirstOrDefault<string>();
        //                    break;
        //                case "UsersCR":
        //                    RequestIdentifier = db.Database.SqlQuery<string>("select (LuFirstName+' '+LuLastName) from " + WFDetails.RwfWFType + " where Id=" + TransactionId).FirstOrDefault<string>();
        //                    break;
        //                case "ManualAdjustments":
        //                    RequestIdentifier = db.Database.SqlQuery<string>("select LbBatchNumber from " + WFDetails.RwfBaseTableName + " where Id=" + TransactionId).FirstOrDefault<string>();
        //                    break;
        //            }

        //            /*<Action Label> = 
        //            Case Approve, Approved
        //            Case Reject, Rejected
        //            Case Cancel, Cancelled
        //            Case Withdraw, Withrawn
        //            Case SendToRequester, Send To Requester
        //            Case SendToAnalyst, Send To Analyst
        //            Case SendToManager, Send To Manager*/
        //            switch (Action)
        //            {
        //                case "Approve":
        //                    ActionLabel = "Approved";
        //                    break;
        //                case "Reject":
        //                    ActionLabel = "Rejected";
        //                    break;
        //                case "Cancel":
        //                    ActionLabel = "Cancelled";
        //                    break;
        //                case "Withdraw":
        //                    ActionLabel = "Withrawn";
        //                    break;
        //                case "SendToRequester":
        //                    ActionLabel = "Send To Requester";
        //                    break;
        //                case "SendToAnalyst":
        //                    ActionLabel = "Send To Analyst";
        //                    break;
        //                case "SendToManager":
        //                    ActionLabel = "Send To Manager";
        //                    break;
        //            }



        //            //Check if requester is present in workflow config
        //            if (db.LWorkFlowConfigs.Where(p => p.LwfcActingAs.Equals("Requester")).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcWorkFlowId == WFDetails.Id).Count() > 0)
        //            {

        //                //Find Role for the WF Requester Id of the Request
        //                var RequestorRole = db.Database.SqlQuery<AspNetRole>("select * from AspnetRoles where Id =(select WFRequesterRoleId from " + WFDetails.RwfBaseTableName + " where Id=" + TransactionId + ")").FirstOrDefault<AspNetRole>();
        //                //For that Role in the WF Config check if do not notify flag is set
        //                var DONotNotify = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcWorkFlowId == WFDetails.Id).Where(p => p.LwfcRoleId == RequestorRole.Id).FirstOrDefault().LwfcDoNotNotify;
        //                /*For Requestor
        //              a) For that workflow (WorkflowID and OpCO combo), find the DoNotNotify flag where WFActingAs=Requester. If it is false, proceed to next check*/
        //                if (!DONotNotify)
        //                {
        //                    //             Then if Role = Payee
        //                    //* Send Email to Payee based on LPayees
        //                    switch (RequestorRole.Name)
        //                    {
        //                        case "Payee":////WARNING : PAYEE ROLE HARDCODED - Make sure payees are created in all opcos using "Payee" role only . It cannot be Payee_qa
        //                                     /* You will have the Payee for Requester in the Request itself.*/
        //                            var PayeeDetails = db.Database.SqlQuery<LPayee>("select * from LPayees where LpUserId=(select WFRequesterId from " + WFDetails.RwfBaseTableName + " where Id=" + TransactionId + ")").FirstOrDefault<LPayee>();
        //                            /*Subject: Your <Rworkflow.label>  (<Request Ideentifier>) has been <Action label>
        //                       Body: You <Rworkflow.label>  (<Request Ideentifier>) has been <Action label> on <current date> and is currently sitting with <new Role> <(Name(newOwenerID))>.*/
        //                            var EmailSubject = "Your " + WFDetails.RwfUILabel + " (" + RequestIdentifier + ") has been " + ActionLabel;
        //                            var EmailBody = "Your " + WFDetails.RwfUILabel + "( " + RequestIdentifier + " ) has been " + ActionLabel + " on " + DateTime.UtcNow.Date + " and is currently sitting with " + NextRoleName + " < (" + NextUserName + ") >";

        //                            //Send Email to Different Email Id as per Project Enviournment

        //                                    Globals.SendEmail(PayeeDetails.LpEmail, null, EmailSubject, EmailBody,Company.GcCode);
        //                            break;
        //                        default:
        //                            /* You will have the UserID for Requester in the Request itself.Check in Lusers table for LuBlockNotification. If False then proceed to send email. */
        //                            var RequestorDetails = db.Database.SqlQuery<LUser>("select * from LUsers where LuUserId=(select WFRequesterId from " + WFDetails.RwfBaseTableName + " where Id=" + TransactionId + ")").FirstOrDefault<LUser>();
        //                            /* If Block Notification=NO
        //       *            Send Email to User based on LUsers */
        //                            if (!RequestorDetails.LuBlockNotification)
        //                            {
        //                                /*Subject: Your <Rworkflow.label>  (<Request Ideentifier>) has been <Action label>
        //                           Body: You <Rworkflow.label>  (<Request Ideentifier>) has been <Action label> on <current date> and is currently sitting with <new Role> <(Name(newOwenerID))>.*/
        //                                var Subject = "Your " + WFDetails.RwfUILabel + " (" + RequestIdentifier + ") has been " + ActionLabel;
        //                                var Body = "Your " + WFDetails.RwfUILabel + "( " + RequestIdentifier + " ) has been " + ActionLabel + " on " + DateTime.UtcNow.Date + " and is currently sitting with " + NextRoleName + " < (" + NextUserName + ") >";

        //                                //Send Email to Different Email Id as per Project Enviournment
        //                                        Globals.SendEmail(RequestorDetails.LuEmail, null, Subject, Body,Company.GcCode);
        //                            }
        //                            break;
        //                    }
        //                }
        //            }
        //            /*For new CurrentOwner
        //a) Proceed if Ordinal is not MaxOrdinal
        //b) For that workflow (WorkflowID and OpCO combo), find the DoNotNotify flag for new WfconfigID. If it is false, proceed to next check
        //c) You will have the UserID for new CurrentOwner.Check in Lusers table for LuBlockNotification. If False then proceed to send email. */
        //            if (!WfIsLastOrdinal(WFDetails.Id, TransactionId, CompanyId))
        //            {
        //                int CurrentOrdinal = db.Database.SqlQuery<int>("select WFOrdinal from " + WFDetails.RwfBaseTableName + " where Id=" + TransactionId).FirstOrDefault<int>();
        //                var ConfigDetails = db.LWorkFlowConfigs.Where(p => p.LwfcOrdinalNumber == CurrentOrdinal).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcWorkFlowId == WFDetails.Id).FirstOrDefault();
        //                var CurrentRole = db.AspNetRoles.Find(ConfigDetails.LwfcRoleId);
        //                //If Payee Make a Email by getting details from LPayee else get details from LUsers
        //                if (!ConfigDetails.LwfcDoNotNotify)
        //                {
        //                    switch (CurrentRole.Name)
        //                    {
        //                        case "":

        //                            /* You will have the PayeeId for Current Owner in the Request itself.*/
        //                            var PayeeDetails = db.Database.SqlQuery<LPayee>("select * from LPayees where LpUserId=(select WFCurrentOwnerId from " + WFDetails.RwfBaseTableName + " where Id=" + TransactionId + ")").FirstOrDefault<LPayee>();
        //                            /*Subject: Your <Rworkflow.label>  (<Request Ideentifier>) has been <Action label>
        //                       Body: You <Rworkflow.label>  (<Request Ideentifier>) has been <Action label> on <current date> and is currently sitting with <new Role> <(Name(newOwenerID))>.*/
        //                            var EmailSubject = "New request - " + WFDetails.RwfUILabel + "  (" + RequestIdentifier + ")";
        //                            var EmailBody = "New request for " + WFDetails.RwfUILabel + "  (" + RequestIdentifier + ") has been placed in your queue.";

        //                            //Send Email to Different Email Id as per Project Enviournment
        //                                    Globals.SendEmail(PayeeDetails.LpEmail, null, EmailSubject, EmailBody,Company.GcCode);

        //                            break;
        //                        default:
        //                            /* You will have the UserID for Current Owner in the Request itself.Check in Lusers table for LuBlockNotification. If False then proceed to send email. */
        //                            var CurrentOwnerDetails = db.Database.SqlQuery<LUser>("select * from LUsers where LuUserId=(select WFCurrentOwnerId from " + WFDetails.RwfBaseTableName + " where Id=" + TransactionId + ")").FirstOrDefault<LUser>();
        //                            if (!CurrentOwnerDetails.LuBlockNotification)
        //                            {
        //                                /*Subject: Your <Rworkflow.label>  (<Request Ideentifier>) has been <Action label>
        //                           Body: You <Rworkflow.label>  (<Request Ideentifier>) has been <Action label> on <current date> and is currently sitting with <new Role> <(Name(newOwenerID))>.*/
        //                                var Subject = "New request - " + WFDetails.RwfUILabel + "  (" + RequestIdentifier + ")";
        //                                var Body = "New request for " + WFDetails.RwfUILabel + "  (" + RequestIdentifier + ") has been placed in your queue.";

        //                                //Send Email to Different Email Id as per Project Enviournment
        //                                        Globals.SendEmail(CurrentOwnerDetails.LuEmail, null, Subject, Body,Company.GcCode);

        //                            }
        //                            break;
        //                    }
        //                }
        //            }
        //            return true;
        //        }
        //        catch
        //        {
        //            return false;//As disscussed with JS we will not produce error if emailing fails and will commit the transaction
        //        }
        //    }

        [HttpGet]
        public List<string> WFFindPossibleOwners(int WorkFlowId, int TransactionId, int CompanyId, string NextRole)
        {
            //get work flow name based on Wf Config
            var WorkFlow = db.RWorkFlows.Find(WorkFlowId);
            //Find what is the Current_WFOrdinal for that Transaction ID in the base table of RWorkFlow (based on workflow ID ) 
            int CurrentOrdinal = db.Database.SqlQuery<int>("select isnull(WFOrdinal,0) from " + WorkFlow.RwfBaseTableName + " where Id=" + TransactionId).FirstOrDefault<int>();
            //  Look for the Current_Role_ID in the WFConfig table for that WorkflowId,Opco and CUrrent_WFOrdinal
            var CurrentRoleId = db.LWorkFlowConfigs.Where(p => p.LwfcWorkFlowId == WorkFlowId).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcOrdinalNumber == CurrentOrdinal).FirstOrDefault().LwfcRoleId;
            //Get the list of Portfolios that are related with this transactionid(MEntityPortfolios where entitytype = BaseTableName and entityid = TransactionId)
            var PortfolioList = from bb in db.MEntityPortfolios
                                where (bb.MepEntityType == WorkFlow.RwfBaseTableName && bb.MepEntityId == TransactionId)
                                select bb.MepPortfolioId;
            //Create an empty list of Users (UserList)
            var UserList = new List<string>();
            //For every entity(transaction) portfolio ,find the list of users who also own the same portfolio for the current wfrole 
            foreach (var Portfolio in PortfolioList)
            {
                // Find all users in ASPNetUsers matching Portfolio of base table
                var User = db.Database.SqlQuery<string>("select Lu.LuUserId from AspnetUsers AU inner join LUsers Lu on AU.Id=Lu.LuUserId inner join MEntityPortfolios EP on (Lu.Id = EP.MepEntityId and EP.MepEntityType = 'LUsers') inner join AspNetUserRoles AUR on AUR.UserId=AU.Id where AUR.RoleId='" + CurrentRoleId + "' and AU.IsActive=1 and EP.MepPortfolioId = " + Portfolio).ToList();
                foreach (var UR in User)
                {
                    if (!string.IsNullOrEmpty(UR))
                    {
                        UserList.Add(UR);
                    }
                }
            }
            //Remove duplicates
            UserList = UserList.Distinct().ToList();
            //Remove Requester Id if present in UserList in case of Next Role is Manager as Requester cannot be Manager while Approval
            if (NextRole == "Manager")
            {
                string Requester = db.Database.SqlQuery<string>("select WFRequesterId from " + WorkFlow.RwfBaseTableName + " where Id=" + TransactionId).FirstOrDefault<string>();
                UserList = UserList.Where(p => p != Requester).ToList();
            }
            return UserList;
        }

        //Desc: This function finds the most appropriate owner of the transaction belonging to a particular workflow based on portfolio matching. If 1 user matchesm then that user is returned. If multiple users match, then least loaded user is returned.
        public string WFFindSuitableOwner(int WorkFlowID, int TransactionID, int CompanyId, string NextRole)
        {
            //get work flow name based on Wf Config
            var WorkFlow = db.RWorkFlows.Find(WorkFlowID);
            // PossibleOwnersList = WFFindPossibleOwners(WorkFlowID, Transaction ID)
            var PossibleOwnersList = WFFindPossibleOwners(WorkFlowID, TransactionID, CompanyId, NextRole);
            //If PossibleOwnersList.rowcount = 0
            //   Return NULL
            if (PossibleOwnersList.Count() == 0)
            {
                return null;
            }
            //         If PossibleOwnersList.rowcount = 1
            //THEN
            //   Return that user ID
            else if (PossibleOwnersList.Count() == 1)
            {
                return PossibleOwnersList.ElementAt(0);
            }
            //Find count of transactions against each user for that base table where WFStatus = InProgress and WFCurrentOwbner is that user order by count ASC

            else
            {
                var MinimumLoadedUser = "";
                var MinRowCount = 9999;//Using a large value for reference
                foreach (var UserId in PossibleOwnersList)
                {
                    var RowCount = db.Database.SqlQuery<int>("select count(*) from " + WorkFlow.RwfBaseTableName + " where WFStatus = 'InProgress' and WFCurrentOwnerId='" + UserId + "'").FirstOrDefault<int>();
                    if (RowCount < MinRowCount)
                    {
                        MinRowCount = RowCount;
                        MinimumLoadedUser = UserId;
                    }
                }
                //Return First user ID(i.e.the user with minimum number of transactions assigned to him)
                return MinimumLoadedUser;
            }
        }

        //This method loads the buttons on the secondary form based on WFConfigId,transactionid,loggedinroleid passed
        public IHttpActionResult GetSecondaryFormActionItems(int WFConfigId, int TransactionId, string LoggedInRoleId, string LoggedInUserId)
        {
            //convert xx into datatable
            //get current work flow
            var WorkflowConfig = db.LWorkFlowConfigs.Find(WFConfigId);
            if (WorkflowConfig != null)
            {
                var WFDetails = db.RWorkFlows.Find(WorkflowConfig.LwfcWorkFlowId);
                //Get List of columns
                var WFGridColumns = string.Join(",", db.LWorkflowGridColumns.Where(p => p.LwfgcWfConfigId == WFConfigId).Where(p => p.LwfgcJoinTable == null).Select(p => p.LwfgcColumnName).ToList());
                //Get Base Table Value based on transactionId
                //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
                string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
                SqlConnection conn = new SqlConnection(ConnectionString);
                SqlCommand cmd = new SqlCommand("select WFRequesterRoleId,Id,WFCurrentOwnerId,WFRequesterId,WFAnalystId,WFStatus," + WFGridColumns + " from " + WFDetails.RwfBaseTableName + " where Id=" + TransactionId, conn);
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                var tb = new DataTable();
                sda.Fill(tb);
                conn.Close();
                //The Ado.Net code ends here


                //Get Current Login ConfigId based on RoleId
                var LoginWFConfigId = db.LWorkFlowConfigs.Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.LwfcWorkFlowId == WorkflowConfig.LwfcWorkFlowId).FirstOrDefault().Id;

                // 1.Get the action Items for the Secondary Form (eg Review,Dashboard,Download) (Id,Url,Label) by using show in WFConfig Id Concept  (Get buttons in Tabs Where Click is made)
                var ActionItemsList = db.LWorkFlowActionItems.Where(p => p.LwfaiShowInTabWFConfigId == WFConfigId && p.LwfaiLoginWFConfigId == LoginWFConfigId).Where(p => p.LwfaiIsButtonOnForm == true).Select(p => new { p.LwfaiActionItemName, p.Id, p.LwfaiUILabel, p.LwfaiActionURL, p.LwfaiOrdinal, p.LwfaiIsButtonOnForm, p.LwfaiIsButtonOnWfGrid }).OrderBy(p => p.LwfaiOrdinal).ToList();
                // 2.Loop through action Items and for each item get the list of parameters (List of Parameters : Parameter Name,Parameter Type,Parameter Value)
                string HtmlTemplate = "";
                foreach (var ActionItem in ActionItemsList)
                {

                    var ParameterList = db.LWorkFlowActionParameters.Where(p => p.WFActionItemId == ActionItem.Id).Select(p => new { p.Id, p.ParameterName, p.ParameterValue, p.ParameterValueType }).ToList();
                    //display withdraw action only if requestor Id is equal to logged In user Id or WFAnalystId is equal to logged In user Id
                    if ((ActionItem.LwfaiActionItemName.Equals("Withdraw", StringComparison.OrdinalIgnoreCase) && (LoggedInUserId.Equals(tb.Rows[0].Field<dynamic>("WFRequesterId")) && LoggedInRoleId.Equals(tb.Rows[0].Field<dynamic>("WFRequesterRoleId"))) || (LoggedInUserId.Equals(tb.Rows[0].Field<dynamic>("WFAnalystId"))) || !ActionItem.LwfaiActionItemName.Equals("Withdraw", StringComparison.OrdinalIgnoreCase)))
                    {
                        //show edit action only if requester id is equal to logged in user id
                        if ((ActionItem.LwfaiActionItemName.Equals("Edit", StringComparison.OrdinalIgnoreCase) && LoggedInUserId.Equals(tb.Rows[0].Field<dynamic>("WFRequesterId"))) || !ActionItem.LwfaiActionItemName.Equals("Edit", StringComparison.OrdinalIgnoreCase))
                        {
                            //show Prelim Action only if Wf Status is not equals to Prelim
                            if ((ActionItem.LwfaiActionItemName.Equals("Prelim", StringComparison.OrdinalIgnoreCase) && !tb.Rows[0].Field<dynamic>("WFStatus").Equals("Prelim", StringComparison.OrdinalIgnoreCase)) || !ActionItem.LwfaiActionItemName.Equals("Prelim", StringComparison.OrdinalIgnoreCase))
                            {
                                //show UnPrelim Action only if Wf Status  equals to Prelim loggedinuserid=currentownerId
                                if ((ActionItem.LwfaiActionItemName.Equals("UnPrelim", StringComparison.OrdinalIgnoreCase) && tb.Rows[0].Field<dynamic>("WFStatus").Equals("Prelim", StringComparison.OrdinalIgnoreCase) && LoggedInUserId.Equals(tb.Rows[0].Field<dynamic>("WFCurrentOwnerId"))) || !ActionItem.LwfaiActionItemName.Equals("UnPrelim", StringComparison.OrdinalIgnoreCase))
                                {
                                    //Show approve action only when current owner id is equal to the logged in user id
                                    if ((((ActionItem.LwfaiActionItemName.Equals("Approve", StringComparison.OrdinalIgnoreCase) || ActionItem.LwfaiActionItemName.Equals("Reject", StringComparison.OrdinalIgnoreCase)) && LoggedInUserId.Equals(tb.Rows[0].Field<dynamic>("WFCurrentOwnerId")))) || (!ActionItem.LwfaiActionItemName.Equals("Approve", StringComparison.OrdinalIgnoreCase) && !ActionItem.LwfaiActionItemName.Equals("Reject", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        //Show SendToRequester action only when current owner id is equal to the logged in user id
                                        if ((((ActionItem.LwfaiActionItemName.Equals("SendToRequester", StringComparison.OrdinalIgnoreCase) && LoggedInUserId.Equals(tb.Rows[0].Field<dynamic>("WFCurrentOwnerId")))) || (!ActionItem.LwfaiActionItemName.Equals("SendToRequester", StringComparison.OrdinalIgnoreCase))))
                                        {
                                            //Show SendToAnalyst action only when current owner id is equal to the logged in user id
                                            if ((((ActionItem.LwfaiActionItemName.Equals("SendToAnalyst", StringComparison.OrdinalIgnoreCase) && LoggedInUserId.Equals(tb.Rows[0].Field<dynamic>("WFCurrentOwnerId")))) || (!ActionItem.LwfaiActionItemName.Equals("SendToAnalyst", StringComparison.OrdinalIgnoreCase))))
                                            {
                                                //show self assign action only if currentowner id is null
                                                if ((ActionItem.LwfaiActionItemName.Equals("SelfAssign", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(tb.Rows[0].Field<dynamic>("WFCurrentOwnerId"))) || !ActionItem.LwfaiActionItemName.Equals("SelfAssign", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    //show reclaim only if wfstatus=rejected
                                                    if ((ActionItem.LwfaiActionItemName.Equals("ReClaim", StringComparison.OrdinalIgnoreCase) && (tb.Rows[0].Field<string>("WFStatus").Equals("Rejected", StringComparison.OrdinalIgnoreCase))) || !ActionItem.LwfaiActionItemName.Equals("ReClaim", StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        //show WashClaim action only if Analyst id is equal to logged in user id
                                                        if ((ActionItem.LwfaiActionItemName.Equals("WashClaim", StringComparison.OrdinalIgnoreCase) && LoggedInUserId.Equals(tb.Rows[0].Field<dynamic>("WFAnalystId"))) || !ActionItem.LwfaiActionItemName.Equals("WashClaim", StringComparison.OrdinalIgnoreCase))
                                                        {
                                                            var UrlString = ActionItem.LwfaiActionURL;
                                                            UrlString += "?WFConfigId=" + WorkflowConfig.Id + "&TransactionId=" + tb.Rows[0].Field<dynamic>("Id") + "&";
                                                            //3.Loop through List of parameters and create a template Html string
                                                            for (var i = 0; i < ParameterList.Count(); i++)
                                                            {

                                                                if (ParameterList[i].ParameterValueType.Equals("Static", StringComparison.OrdinalIgnoreCase))
                                                                {
                                                                    UrlString += ParameterList[i].ParameterName + "=" + ParameterList[i].ParameterValue + "&";
                                                                }
                                                                else
                                                                {
                                                                    UrlString += ParameterList[i].ParameterName + "=" + tb.Rows[0].Field<dynamic>(ParameterList[i].ParameterValue) + "&";//tb.Rows[j].Field<string>(ParameterList[i].ParameterValue)
                                                                }


                                                            }

                                                            UrlString = UrlString.Substring(0, (UrlString.Length - 1));//Remove the last  character(&) from Url string
                                                                                                                       /*The below string is used to frame the buttons html part in secondary form   window.location.href='" + UrlString + "';*/
                                                            HtmlTemplate += "<button type=\"button\" class=\"btn btn-red btn-cons\" onclick = \"FnClickFormButtons('" + ActionItem.LwfaiActionItemName + "'," + TransactionId + ")\" > " + ActionItem.LwfaiUILabel + "&ensp;" + GetGlymph(ActionItem.LwfaiActionItemName) + "</button>&ensp;";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return Ok(HtmlTemplate);
            }
            return Ok();
        }

        //This method loads the buttons on the bottom of grid based on WFConfigId,transactionid,loggedinroleid passed
        public IHttpActionResult GetGridBottomActionItems(string Workflow, int CompanyId, string LoggedInRoleId, string LoggedInUserId)
        {
            //Monitor role will have -ve ordinal hence we are only considering +ve ordinal numbers
            var ButtonsList = new List<string>();
            //Ordinal number less than zero are just used to display record they cannot in workflow.
            var ConfigList = db.LWorkFlowConfigs.Where(p => p.RWorkFlow.RwfName == Workflow).Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcOrdinalNumber > 0).OrderBy(p => p.LwfcOrdinalNumber).Select(p => p.Id).ToList();
            for (var j = 0; j < ConfigList.Count(); j++)
            {
                var ConfigId = Convert.ToInt32(ConfigList[j]);
                var Role = db.LWorkFlowConfigs.Include(path => path.AspNetRole).Where(p => p.Id == ConfigId).Select(p => p.AspNetRole.Name).FirstOrDefault().Replace(" ", "");
                //Get Current Login ConfigId based on RoleId
                var LoginWFConfigId = db.LWorkFlowConfigs.Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == Workflow).FirstOrDefault().Id;

                // 1.Get the action Items for the Secondary Form (eg Review,Dashboard,Download) (Id,Url,Label) by using show in WFConfig Id Concept  (Get buttons in Tabs Where Click is made)
                var ActionItemsList = db.LWorkFlowActionItems.Where(p => p.LwfaiShowInTabWFConfigId == ConfigId && p.LwfaiLoginWFConfigId == LoginWFConfigId).Where(p => p.LwfaiIsButtonOnWfGrid == true).Select(p => new { p.LwfaiActionItemName, p.Id, p.LwfaiUILabel, p.LwfaiActionURL, p.LwfaiOrdinal, p.LwfaiIsButtonOnForm, p.LwfaiIsButtonOnWfGrid }).OrderBy(p => p.LwfaiOrdinal).ToList();
                // 2.Loop through action Items and for each item get the list of parameters (List of Parameters : Parameter Name,Parameter Type,Parameter Value)
                string HtmlTemplate = "";
                foreach (var ActionItem in ActionItemsList)
                {
                    //onclick = "FnClickBottomButtons(\'' + xx[3] + '\',\'' + TabName + '\')"/*The below string is used to frame the buttons html part in secondary form   window.location.href='" + UrlString + "';*/
                    HtmlTemplate += "<button type=\"button\" class=\"btn btn-red btn-cons\" onclick = \"FnClickBottomButtons('" + ActionItem.LwfaiActionItemName + "','" + Role + "')\" > " + ActionItem.LwfaiUILabel + "&ensp;" + GetGlymph(ActionItem.LwfaiActionItemName) + "</button>&ensp;";
                }
                ButtonsList.Add(HtmlTemplate);
            }
            return Ok(ButtonsList);
        }

        //This method will return Glymph code by passing ActionName in it
        private string GetGlymph(string ActionName)
        {
            switch (ActionName)
            {
                case "Approve":
                    return ("<i class=\"fa fa-check\" aria-hidden=\"true\" style=\"color:white;\" title=\"Approve\"></i>");
                case "SendToAnalyst":
                    return ("<i class=\"fa fa-angle-left\" aria-hidden=\"true\" style=\"color:white;\" title=\"Send To Analyst\"></i>");
                case "Dashboard":
                    return ("<i class=\"fa fa-tachometer\" aria-hidden=\"true\" style=\"color:white;\" title=\"Dashboard\"></i>");
                case "Review":
                    return ("<i class=\"fa fa-th\" aria-hidden=\"true\" style=\"color:white;\" title=\"Review\"></i>");
                case "Withdraw":
                    return ("<i class=\"fa fa-reply\" aria-hidden=\"true\" style=\"color:white;\" title=\"Withdraw\"></i>");
                case "Prelim":
                    return ("<i class=\"fa fa-thumbs-up\" aria-hidden=\"true\" style=\"color:white;\" title=\"Prelim\"></i>");
                case "UnPrelim":
                    return ("<i class=\"fa fa-thumbs-down\" aria-hidden=\"true\" style=\"color:white;\" title=\"UnPrelim\"></i>");
                case "VerifyPayee":
                    return ("<i class=\"fa fa-thumbs-up\" aria-hidden=\"true\" style=\"color:white;\" title=\"Verify Payee\"></i>");
                case "AttachTestResults":
                    return ("<i class=\"fa fa-folder\" aria-hidden=\"true\" style=\"color:white;\" title=\"Attach Test Results\"></i>");
                case "Edit":
                    return ("<i class=\"fa fa-pencil-square-o\" aria-hidden=\"true\" style=\"color:white;\" title=\"Edit\"></i>");
                case "WashClaim":
                    return ("<i class=\"fa fa-pencil-square-o\" aria-hidden=\"true\" style=\"color:white;\" title=\"Edit\"></i>");
                case "Reject":
                    return ("<i class=\"fa fa-times\" aria-hidden=\"true\" style=\"color:white;\" title=\"Reject\"></i>");
                case "Download":
                    return ("<i class=\"fa fa-download\" aria-hidden=\"true\" style=\"color:white;\" title=\"Download\"></i>");
                case "SelfAssign":
                    return ("<i class=\"fa fa-user\" aria-hidden=\"true\" style=\"color:white;\" title=\"Self Assign\"></i>");
                case "SendToRequester":
                    return ("<i class=\"fa fa-angle-double-left\" aria-hidden=\"true\" style=\"color:white;\" title=\"Send To Requester\"></i>");
                case "AssignTo":
                    return ("<i class=\"fa fa-users\" aria-hidden=\"true\" style=\"color:white;\" title=\"Assign To\"></i>");
                case "ReClaim":
                    return ("<i class=\"fa fa-recycle\" aria-hidden=\"true\" style=\"color:white;\" title=\"ReClaim\"></i>");

            }
            return null;
        }

        //Method to save GenericGrid Data in a file 
        [HttpGet]
        public IHttpActionResult SaveGenericGridToFile(string Workflow, string UserName, string LoggedInRoleId, string CompanyId, int WFConfigId, string LoggedInUserId, string CompanyCode, string TabName, string PortfolioList, string FilterQuery)
        {
            /*Section to Get grid Columns */
            //GetWorkflow details
            var WFDetails = db.RWorkFlows.Where(p => p.RwfName == Workflow).FirstOrDefault();
            // var LoginWFConfig = db.LWorkFlowConfigs.Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.LwfcWorkFlowId == WFDetails.Id).FirstOrDefault();
            var WorkflowConfig = db.LWorkFlowConfigs.Find(WFConfigId);
            
            //The below lines of code converts the data returned from api to a datatable
            var tb = new DataTable();
            string Query = "Exec [dbo].[SPGetGenericGridData] @WorkflowConfigId , @LoggedInRoleId , @LoggedInUserId , @WorkflowName , @CompanyId, @PageSize , @PageNumber, @sortdatafield, @sortorder,@FilterQuery,@TabName, @PortfolioList";
            //using ADO.NET  in below code to execute sql command and get result in a datatable . Will replace this code with EF code if resolution found .
            string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            cmd.Parameters.AddWithValue("@WorkflowConfigId", WFConfigId);
            cmd.Parameters.AddWithValue("@LoggedInRoleId", string.IsNullOrEmpty(LoggedInRoleId) ? (object)System.DBNull.Value : (object)LoggedInRoleId);
            cmd.Parameters.AddWithValue("@LoggedInUserId", string.IsNullOrEmpty(LoggedInUserId) ? (object)System.DBNull.Value : (object)LoggedInUserId);
            cmd.Parameters.AddWithValue("@WorkflowName", string.IsNullOrEmpty(Workflow) ? (object)System.DBNull.Value : (object)Workflow);
            cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
            cmd.Parameters.AddWithValue("@PageSize", 99999);
            cmd.Parameters.AddWithValue("@PageNumber", 0);
            cmd.Parameters.AddWithValue("@sortorder", (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@sortdatafield", (object)System.DBNull.Value);
            // cmd.Parameters.AddWithValue("@FilterQuery",  (object)System.DBNull.Value);
            cmd.Parameters.AddWithValue("@FilterQuery", string.IsNullOrEmpty(FilterQuery) ? (object)System.DBNull.Value : (object)FilterQuery);
            cmd.Parameters.AddWithValue("@TabName", string.IsNullOrEmpty(TabName) ? (object)System.DBNull.Value : (object)TabName);
            cmd.Parameters.AddWithValue("@PortfolioList", string.IsNullOrEmpty(PortfolioList) ? (object)System.DBNull.Value : (object)PortfolioList);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
            //The Ado.Net code ends here
            var FileName = "Export" + Workflow + "(" + TabName + ").xlsx";

            // System.IO.File.Delete();
            //var FilesPath = ConfigurationManager.AppSettings["ClaimsDocumentPath"] + "/ExportGrid.csv";
            var WorkFlowId = db.RWorkFlows.Where(p => p.RwfName.Equals(Workflow, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Id;
            var xx = db.Database.SqlQuery<GenericGridViewModel>("select C.LwfgcJoinTable,C.LwfgcJoinTableColumn, C.LwfgcBaseTableJoinColumn, ISC.DATA_TYPE as DataType, AR.Name as UserRole,WC.LwfcBanner, C.LwfgcOrdinal ,WC.LwfcCanCreate,WC.LwfcOrdinalNumber, WF.RwfBaseTableName,WF.RwfName,WF.RwfCRAllowed,WF.RwfUILabel,C.LwfgcColumnName, C.Id,C.LwfgcAscDesc,C.LwfgcOrderByOrdinal,C.LwfgcShouldBeVisible,C.LwfgcUILabel ,C.LwfgcWfConfigId from LWorkflowGridColumns C join LWorkFlowConfig WC on C.LwfgcWfConfigId = WC.Id inner join RWorkFlows WF on(WC.LwfcWorkFlowId = WF.Id and LwfcRoleId ={0}) inner join AspNetRoles AR on WC.LwfcRoleId = AR.Id inner join INFORMATION_SCHEMA.COLUMNS ISC on(ISC.Table_Name in(WF.RwfBaseTableName,C.LwfgcJoinTable) and C.LwfgcColumnName = ISC.COLUMN_NAME) Where WC.LwfcWorkFlowId = {1} and WC.LwfcCompanyId = {2}  order by WC.LwfcOrdinalNumber,C.LwfgcOrdinal", LoggedInRoleId, WorkFlowId, CompanyId).ToList();
            for (var i = 0; i < xx.Count(); i++)
            {
                if (!string.IsNullOrEmpty(xx[i].LwfgcJoinTable))
                {
                    xx[i].LwfgcColumnName = xx[i].LwfgcJoinTable + xx[i].LwfgcOrdinal + "." + xx[i].LwfgcColumnName + "";
                }
            }

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            IRow row1 = sheet1.CreateRow(0);
            ICell cell = null;

            string columnName = "";

            //RS added this check for the Active tab to get owner role in downloaded file on 14th march 2019
            if (TabName =="Active")
            {                
                for (int k = 0; k < xx.Count+1; k++)
                {
                    int range = xx.Count + 1;
                    cell = row1.CreateCell(k);
                    if (k == (range-1))
                    {
                        columnName = "Owner Role";
                        cell.SetCellValue("Owner Role");
                    }
                    else
                    {
                         columnName = xx[k].LwfgcUILabel.ToString();
                        cell.SetCellValue(columnName);
                    }
                    
                }

                // GC is used to avoid error System.argument exception
                GC.Collect();
                
                //loops through data  
                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 1);
                    
                    for (int j = 0; j < xx.Count + 1; j++)
                    {
                        int range = xx.Count + 1;
                        cell = row.CreateCell(j);
                        if (j == range-1)
                        {
                            columnName = "Owner Role";
                        }
                        else
                        {
                            columnName = xx[j].LwfgcColumnName.ToString();
                        }
                        if (columnName == "Owner Role")
                        {
                            cell.SetCellValue(tb.Rows[i]["Arxx.Name"].ToString());
                        }
                        else
                        {
                            columnName = xx[j].LwfgcColumnName.ToString();
                            string value = tb.Rows[i][columnName].ToString();
                            //If columnName is datatime, convert its value to contain only date in dd/MM/yyyy format.
                            if (columnName.Contains("Date"))
                            {
                                if (!String.IsNullOrEmpty(value))
                                {
                                    value = DateTime.Parse(value).ToString("dd/MM/yyyy");
                                    cell.SetCellValue(value);
                                }
                            }
                            else
                            {
                                cell.SetCellValue(value);
                            }
                            //cell.SetCellValue(tb.Rows[i][columnName].ToString());
                        }

                    }
                }

            }

            //for all tabs other than active tab
            else
            {
                for (int k = 0; k < xx.Count; k++)
                {
                    cell = row1.CreateCell(k);
                     columnName = xx[k].LwfgcUILabel.ToString();
                    cell.SetCellValue(columnName);
                }
                // GC is used to avoid error System.argument exception
                GC.Collect();                
                //loops through data  
                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    IRow row = sheet1.CreateRow(i + 1);
                    for (int j = 0; j < xx.Count; j++)
                    {
                        cell = row.CreateCell(j);
                        columnName = xx[j].LwfgcColumnName.ToString();
                        string value = tb.Rows[i][columnName].ToString();
                        //If columnName is datatime, convert its value to contain only date in dd/MM/yyyy format.
                        if (columnName.Contains("Date"))
                        {
                            if (!String.IsNullOrEmpty(value))
                            {
                                value = DateTime.Parse(value).ToString("dd/MM/yyyy");
                                cell.SetCellValue(value);
                            }
                        }
                        else
                        {
                            cell.SetCellValue(value);
                        }
                        //cell.SetCellValue(tb.Rows[i][columnName].ToString());
                    }
                }
            }
            
            if (!System.IO.Directory.Exists(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyCode + "/" + UserName))//create directory if not present
            {
                System.IO.Directory.CreateDirectory(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyCode + "/" + UserName);
            }
            if (System.IO.File.Exists(Path.Combine(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyCode + "/" + UserName, FileName)))
            {
                System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyCode + "/" + UserName, FileName));
            }
            //Get fileName without datetimestamp
            //var FileNameArray = FileName.Split('_');
            //var FileNameInsideZip = FileNameArray[0];
            FileStream xfile = new FileStream(Path.Combine(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyCode + "/" + UserName, FileName), FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: false);
            workbook.Write(xfile);
            xfile.Close();
            //using (var fs = new FileStream(Path.Combine(ConfigurationManager.AppSettings["TempDocumentPath"] + "/" + CompanyCode + "/" + UserName, FileName), FileMode.Create, FileAccess.Write))
            //{
            //    workbook.Write(fs);
            using (StreamReader sr = new StreamReader(Path.Combine(ConfigurationManager.AppSettings["NewTempFolderPath"] + "/" + CompanyCode + "/" + UserName, FileName)))
            {
                //delete file if it exist
                if (Globals.FileExistsInS3(FileName, UserName, CompanyCode))//R3.1 CompanyCode.toLower() is removed
                {
                    Globals.DeleteFileFromS3(FileName, CompanyCode, UserName);
                }
                //Create folder if it does not exist
                if (!Globals.FolderExistsInS3(UserName, CompanyCode))
                {
                    Globals.CreateFolderInS3(UserName, CompanyCode);
                }

                Globals.UploadToS3(sr.BaseStream, FileName, UserName, CompanyCode);
            }

            return Ok(FileName);
        }

        public IHttpActionResult GetSudmitableornot(int Transactionid, string Workflow, string Role, int CompanyID)
        {
            Boolean Data = Globals.GetSudmitableorNot(Transactionid, Workflow, Role, CompanyID);
            return Ok(Data);
        }

        //public IHttpActionResult GetChangeRequestDetailbyId(int id)
        //{
        //    //Boolean Data = Globals.GetSudmitableorNot(Transactionid, Workflow, Role, CompanyID);
        //    //return Ok(Data);
        //    var data = (from a in db.LChangeRequests where a.Id == id select a);
        //    return Ok(data);
        //}

        //public IHttpActionResult GetChangeRequestDetailbyId(int id)
        //{
        //    var xx = (from aa in db.LChangeRequests where aa.Id == id
        //              select new { aa});


        //    return Ok(xx);
        //}

        public IHttpActionResult GetChangeRequestDetailbyId(int id)
        {
            int rowid;

            var xx = db.LChangeRequests.Where(p => p.Id == id).
                Select(p => new
                {
                    p.Id,
                    p.LcrRowId,
                    p.LcrNewValue,
                    p.LcrOldValue,
                    p.LcrEntityName,
                    p.LcrColumnLabel

                }).FirstOrDefault();
            if (xx == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "WORKFLOW ACTIONITEM")));
            }
            //else
            //{
            //    if (xx.LcrEntityName == "LUsers")
            //    {
            //       userEmail =   db.LUsers.Where(p => p.Id == xx.LcrRowId).Select(p=> p.LuEmail);
            //    }
            //    else if (xx.LcrEntityName == "LPayees")
            //    {
            //       payeedetail = db.LPayees.Where(p=> p.Id == xx.LcrRowId).Select(p=> new { p.LpPayeeCode, p.LpEmail})
            //    }   

            //}


            return Ok(xx);
        }
        public IHttpActionResult GetUserPreferenceData(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string SessionId)
        {
            try
            {
                DataTable dt = Globals.GetUserPreferenceData(UserId, EntityName, EntityItem, ConfigType, WFConfigId, SessionId);
                return Ok(dt);
            }
            catch (DbEntityValidationException dbex)
            {

                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {

                throw ex;//This exception will be handled in FilterConfig's CustomHandler
            }

        }
        [HttpGet]
        public IHttpActionResult SaveUserPreferenceData(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string Config, string SessionId)
        {
            try
            {
                Globals.ExecuteSPLogUserPreference(UserId, EntityName, EntityItem, ConfigType, WFConfigId, Config, SessionId);
                return Ok();
            }
            catch (DbEntityValidationException dbex)
            {

                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {

                throw ex;//This exception will be handled in FilterConfig's CustomHandler
            }

        }

        [HttpGet]
        public IHttpActionResult DeleteUserPreferenceData(string UserId, string EntityName, string EntityItem, string ConfigType, string WFConfigId, string SessionId)
        {
            try
            {
                Globals.DeleteUserPreference(UserId, EntityName, EntityItem, ConfigType, WFConfigId, SessionId);
                return Ok();
            }
            catch (DbEntityValidationException dbex)
            {

                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {

                throw ex;//This exception will be handled in FilterConfig's CustomHandler
            }

        }

        [HttpGet]
        public IHttpActionResult UpdateAttachmentCommon(int id, string FileName, string FilePath, string CreatedBy, string type)
        {
            string ReturnString = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(FileName))
                {
                    var FilesArray = FileName.Split(',').ToList();
                    foreach (var file in FilesArray)
                    {
                        var LSupportingDocuments = new LSupportingDocument { LsdCreatedById = CreatedBy, LsdUpdatedById = CreatedBy, LsdFileName = file, LsdFilePath = FilePath, LsdEntityType = type, LsdEntityId = id, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedDateTime = DateTime.UtcNow };
                        db.LSupportingDocuments.Add(LSupportingDocuments);
                        db.SaveChanges();

                        if (ReturnString == "")
                        {
                            ReturnString = file + ':' + LSupportingDocuments.Id;
                        }
                        else
                        {
                            ReturnString = ReturnString + ',' + file + ':' + LSupportingDocuments.Id;
                        }
                    }

                }
            }
            catch (DbEntityValidationException dbex)
            {

                var errorMessage = Globals.GetEntityValidationErrorMessage(dbex);
                throw new HttpResponseException(Request.CreateErrorResponse((HttpStatusCode)Globals.ExceptionType.Type2, errorMessage));//type 2 error
            }
            catch (Exception ex)
            {

                throw ex;//This exception will be handled in FilterConfig's CustomHandler
            }
            // return StatusCode(HttpStatusCode.NoContent);
            return Ok(ReturnString);
        }

    }


    public partial class UserModel
    {
        public string LuUserId { get; set; }
    }

}
