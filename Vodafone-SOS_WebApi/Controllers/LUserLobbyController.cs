using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using Vodafone_SOS_WebApi.Models;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LUserLobbyController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        [HttpGet]
        public IHttpActionResult CheckUserInSOSAndCallSP(string User, int LobbyUserId)
        {
            string Email = db.LUserLobbies.Where(a => a.Id == LobbyUserId).Select(a => a.Email).FirstOrDefault();
            int? Id = null;
            if ("FinOps".Equals(User))
                Id = db.LUsers.Where(a => a.LuEmail == Email).Select(a => a.Id).FirstOrDefault();
            else if ("Payee".Equals(User))
                Id = db.LPayees.Where(a => a.LpEmail == Email).Select(a => a.Id).FirstOrDefault();
            if(Id != null && Id != 0 )
            {
                try
                {
                    SqlConnection cn = new SqlConnection();
                    cn.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                    cn.Open();
                    SqlCommand command = new SqlCommand("USPAcceptLobbyUsers", cn);
                    command.Parameters.AddWithValue("@LobbyId", LobbyUserId);
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                    cn.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return Ok(Id);
        }
        [HttpGet]
        public IHttpActionResult GetUsersFromLobby(string UserType,string CompanyCode)
        {
            string qry = "select Id,CompanyCode,FirstName,LastName,Email,Phone,UserType,Status,PayeeCode from LUserLobby where Status={0} and CompanyCode ={1} order by id desc";
            var data = db.Database.SqlQuery<LobbyForCreateUserViewModel>(qry, "NewUser",  CompanyCode).ToList();
            return Ok(data);
        }
        [HttpGet]
        public IHttpActionResult GetFinOpsUserById(int Id)
        {
            string qry = "select FirstName as LuFirstName,LastName as luLastName,Email as LuEmail,Phone as LuPhone from LUserLobby where Id = {0}";
            var data = db.Database.SqlQuery<LobbyForFinOpsViewModel>(qry, Id).FirstOrDefault();
            return Ok(data);
        }

        [HttpGet]
        public IHttpActionResult RejectUser(int Id,string LoggedInUser, int LoggedInRoleId)
        {
            string sql = "update LUserLobby set status='Rejected',UpdatedDateTime={0},UpdatedBy={1},UpdatedByRoleId={2} where Id = {3} ";
            db.Database.ExecuteSqlCommand(sql,DateTime.UtcNow, LoggedInUser, LoggedInRoleId,Id);
            return Ok();
        }
        //GetLobbyPayeeById
        [HttpGet]
        public IHttpActionResult GetLobbyPayeeById(int Id)
        {
            string qry = "select FirstName as LpFirstName,LastName as LpLastName,Email as LpEmail,Phone as LpPhone, PayeeCode as LpPayeeCode from LUserLobby where Id = {0}";
            var data = db.Database.SqlQuery<LobbyForPayeesViewModel>(qry, Id).FirstOrDefault();
            return Ok(data);
        }

        [HttpGet]
        public IHttpActionResult Getlobbycounts(string CompanyCode)
        {
            string qry = "select Id,CompanyCode,FirstName,LastName,Email,Phone,UserType,Status,PayeeCode from LUserLobby  order by id desc";
            int data = db.Database.SqlQuery<LobbyForCreateUserViewModel>(qry).Count();
            return Ok(data);
        }
        [HttpGet]
        public IHttpActionResult GetlobbylogGridData(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery, string CompanyCode)
        {
            var SortQuery = "";
            if (!string.IsNullOrEmpty(sortdatafield))
            {
                SortQuery = " order by " + sortdatafield + " " + sortorder;
            }
            else
            {
                SortQuery = " ORDER BY id  desc";
            }
            string Qry = string.Empty;
            if (FilterQuery == null)
            {

                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as row " +
                      "FROM(select Id,CompanyCode,FirstName,LastName,Email,Phone,UserType,Status,PayeeCode,IsVFADUser,UserGroup,NewUserGroup,NewEmail,RequestorEmail,ManagerEmail,RequestType,CreatedBy,CreatedDateTime,UpdatedBy,UpdatedDateTime,UpdatedByRoleId,Response,ResponseCode,ResponseMessage,Comments from LUserLobby )A " +
                      " )B WHERE B.row > @P1 AND B.row <= @P2";
            }
            else
            {

                FilterQuery = "WHERE 1=1" + FilterQuery;
                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as row " +
                      "FROM(select Id,CompanyCode,FirstName,LastName,Email,Phone,UserType,Status,PayeeCode, IsVFADUser,UserGroup,NewUserGroup,NewEmail,RequestorEmail,ManagerEmail,RequestType,CreatedBy,CreatedDateTime,UpdatedBy,UpdatedDateTime,UpdatedByRoleId,Response,ResponseCode,ResponseMessage,Comments from LUserLobby  )A " +
                      FilterQuery + " )B WHERE B.row > @P1 AND B.row <= @P2";

            }
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            //parameterList.Add(new SqlParameter("@P3",CompanyCode ));
            //parameterList.Add(new SqlParameter("@P3", qq));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<LobbyUserViewModel>(Qry, parameters).ToList();
            return Ok(xx);
        }

    }
}