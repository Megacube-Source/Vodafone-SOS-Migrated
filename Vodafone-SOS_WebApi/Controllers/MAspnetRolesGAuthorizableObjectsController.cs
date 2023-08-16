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

namespace Vodafone_SOS_WebApi.Controllers
{
    public class MAspnetRolesGAuthorizableObjectsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();


        //GetCount
        public IHttpActionResult GetCount(string UserRoleId, string CurrentActionKey)
        {

            /*
            var xx = (from ma in db.MAspnetRolesGAuthorizableObjects
                      join ga in db.GAuthorizableObjects on ma.MargaoAuthorizableObjectId equals ga.Id
                      select new
                      {
                          RoleId = ma.MargaoRoleId,
                          ActionKey = ga.GaoControllerName + " " + ga.GaoControllerMethodName,
                         
                      }).ToList();*/

            var xxx =(from ma in db.MAspnetRolesGAuthorizableObjects.Where(p => p.MargaoRoleId == UserRoleId)
                      join ga in db.GAuthorizableObjects.Where(q => q.GaoControllerName.Trim() + "-" + q.GaoControllerMethodName.Trim() == CurrentActionKey) 
                      on ma.MargaoAuthorizableObjectId equals ga.Id                      
                      select new
                      {
                          RoleId = ma.Id
                      });
                    return Ok(xxx.Count());

        }


        public IHttpActionResult GetList()
        {
            var xx = (from ma in db.MAspnetRolesGAuthorizableObjects
                      join ga in db.GAuthorizableObjects on ma.MargaoAuthorizableObjectId equals ga.Id
                      join ar in db.AspNetRoles on ma.MargaoRoleId equals ar.Id
                      select new
                      {
                          RoleId = ma.MargaoRoleId,
                          RoleName = ar.Name,
                          ActionKey = ga.GaoControllerName + "-" + ga.GaoControllerMethodName,
                      });
            return Ok(xx);
        }


        public IHttpActionResult GetRolesByCompanyId(int CompanyId)
        {
            //select * from AspNetRoles ar inner join GCompanies gc on ar.CompanyCode=gc.GcCode where gc.Id=6

            var xx = (from ar in db.AspNetRoles
                      join gc in db.GCompanies.Where(p => p.Id == CompanyId) 
                      on ar.CompanyCode equals gc.GcCode
                      select new
                      {
                          RoleName = ar.Name,
                          RoleId = ar.Id
                      });

            return Ok(xx);
        }


         public IHttpActionResult Get(string RoleId)
        {
           
          //  select ma.MargaoRoleId,GaoControllerMethodName,ga.GaoControllerName,
	       //case ma.MargaoRoleId when '63' Then 1	else 0  END as Flag from GAuthorizableObjects ga
        //left outer join MAspnetRolesGAuthorizableObjects ma on ga.Id = ma.MargaoAuthorizableObjectId

            var xx = (from  ga in db.GAuthorizableObjects
                      join ma in db.MAspnetRolesGAuthorizableObjects.DefaultIfEmpty() on ga.Id equals ma.MargaoAuthorizableObjectId
                     
                      select new
                      {
                          flag = (ma.MargaoRoleId.Equals(RoleId)? 1 : 0),
                          RoleId = ma.MargaoRoleId,
                          ga.GaoControllerName,
                          ga.GaoControllerMethodName,
                          ObjectId = ma.MargaoAuthorizableObjectId,
                          Id = ma.Id
                      }
                );

            return Ok(xx);
        }


        //This method will insert a new row/ update the existing one in the Table LCommissionBatchAllocationRules depending on the Id found or not
        [HttpPost]
        public IHttpActionResult PostGridData(GridDataViewModel model,string RoleId)
        {

            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    /* var mapObj = db.MAspnetRolesGAuthorizableObjects.Where(p => p.MargaoRoleId == RoleId).Select(p => new { p.Id }).ToList();
                     //If the passed RoleId already exists, delete those rows.
                     if (mapObj != null)
                     {
                         for (var i = 0; i < mapObj.Count; i++)
                         {
                             DataRow dRow = db.MAspnetRolesGAuthorizableObjects.Select("Id=" + mapObj[i].Id).FirstOrDefault();
                             db.MAspnetRolesGAuthorizableObjects.Remove(dRow);
                         }
                     }*/
                     //delete query is not working here......deferring this task as suggested by VG due to high priority tasks
                    string query = "delete from MAspnetRolesGAuthorizableObjects where MargaoRoleId =" + RoleId;
                   var xx = db.Database.SqlQuery<MAspnetRolesGAuthorizableObject>(query);
                  /*  MAspnetRolesGAuthorizableObject ObjToBeDeleted =  db.MAspnetRolesGAuthorizableObjects.Find(RoleId);
                    if (ObjToBeDeleted != null)
                    {
                        try
                        {
                            db.MAspnetRolesGAuthorizableObjects.Remove(ObjToBeDeleted);
                            db.SaveChanges();
                        }
                        catch(Exception ex)
                        {
                            throw ex;
                        }
                    }*/


                    string[] modelList = model.GridData.Split(',');
                    for (var i = 0; i < modelList.Length; i = i + 2)
                    {
                        //var RoleId  = modelList[i].ToString();
                      //  var objId = Convert.ToInt32(modelList[i + 1]);
                        bool flag = Convert.ToBoolean(modelList[i + 0]);
                        var Id = Convert.ToInt32(modelList[i + 1]);
                        //if flag is true, insert new entry into MAspnetRolesGAuthorizableObject; else do nothing
                        if (flag)
                        {
                            var ma = new MAspnetRolesGAuthorizableObject
                            {                                
                                MargaoRoleId =RoleId,
                                MargaoAuthorizableObjectId = Id,
                            };
                            if (!string.IsNullOrEmpty(ma.MargaoRoleId) && !(ma.MargaoAuthorizableObjectId == 0))
                            {
                                db.MAspnetRolesGAuthorizableObjects.Add(ma);
                                db.SaveChanges();
                            }
                        }
                       
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                 }
            }

            return Ok();
        }


        public IHttpActionResult GetGridData(string RoleId)
        {
           
            var Query = "Select  ga.GaoControllerName, ga.GaoControllerMethodName, ga.Id from GAuthorizableObjects ga";
            //The below lines of code converts the data returned from api to a datatable
            var tb = new DataTable();

            //using ADO.NET  in below code to execute sql command and get result in a datatable 
            string ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ADOConnectionString"];//"data source=euitdsards01.cbfto3nat8jg.eu-west-1.rds.amazonaws.com;initial catalog=SosDevDb;persist security info=True;user id=SosDevAPIUser;password=pass#word1;MultipleActiveResultSets=True;";
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(Query, conn);
            conn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(tb);
            conn.Close();
           // DataColumn flag = new DataColumn();
            // tb.Columns.Add(flag);
             tb.Columns.Add("flag", typeof(Boolean));
           
            //System.Diagnostics.Debugger.Break();
            var mObjIdList = db.MAspnetRolesGAuthorizableObjects.Where(p => p.MargaoRoleId == RoleId).Select(p => new { p.MargaoAuthorizableObjectId }).ToList();
            //loop for mObjId
            for (var i = 0; i < mObjIdList.Count(); i++)
            {

                var objId = db.GAuthorizableObjects.Find(mObjIdList[i].MargaoAuthorizableObjectId);
                if (objId != null)
                {
                   DataRow dRow = tb.Select("Id="+ objId.Id).FirstOrDefault();
                    dRow.SetField("flag", true);
                    //dRow.SetField("GaoControllerName", "changedName");
                    //dRow["flag"] = true;
                    /*foreach (DataRow dr in tb.Rows)
                    {
                        if (dr["Id"].ToString().Equals(objId.Id))
                        {
                            tb.Rows[objId.Id][flag] = true;
                           
                        }
                    }*/
                }
            }            
            //The Ado.Net code ends here
            return Ok(tb);

        }
        
    }
}