//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Web.Http;
//using System.Web.Http.Description;
//using System.Data.SqlClient; //being used in catch statement for identifying exception only.
//using Vodafone_SOS_WebApi.Models;
//using Vodafone_SOS_WebApi.Utilities;
//using System.Globalization;

//namespace Vodafone_SOS_WebApi.Controllers
//{   [CustomExceptionFilter]
//    public class LCommissionBatchAllocationRulesController : ApiController
//    {
//        private VodafoneSOSLiteEntities db = new VodafoneSOSLiteEntities();
//        public IHttpActionResult GetReportingAnalystByCompanyId(int CompanyId)
//        {
          
//          /*  var xx = (from lc in db.LCommissionBatchAllocationRules
//                      join lu in db.LUsers on lc.LrdbarAllocateToId equals lu.LuUserId
//                      select new
//                      {
//                         Id = lu.LuUserId,
//                         UserName = lu.LuFirstName + " " + lu.LuLastName
//                });*/
//            var querystring = " select(LU.LuFirstName + ' ' + LU.LuLastName) as UserName, LU.LuUserId as Id from LUsers LU"
//                                 + " inner join AspNetUserRoles ANUR on LU.LuUserId = ANUR.UserId inner join AspNetRoles ANR on ANUR.RoleId = ANR.Id"
//                                 + " where ANR.Name = 'Reporting Analyst' and LU.LuCompanyId = ";

//            var xx = db.Database.SqlQuery<AspnetUserViewModel>(querystring + CompanyId).ToList();


//            return Ok(xx);
//        }

//        [ResponseType(typeof(LCommissionBatchAllocationRule))]
//        public IHttpActionResult GetLCommissionBatchAllocationByCompanyId(int CompanyId)
//        {

            
//            var xx = (from lc in db.LCommissionBatchAllocationRules
//                      join lu in db.LUsers on lc.LrdbarAllocateToId equals lu.LuUserId
//                      where lc.LrdbarCompanyId == CompanyId
//                      select new
//                      {
//                          lc.Id,
//                          UserName = lu.LuFirstName + " " + lu.LuLastName,
//                          lc.LrdbarAllocateToId,
//                          lc.LrdbarCompanyId,
//                          lc.LrdbarPrimaryChannel,
//                          lc.LrdbarBusinessUnit,
//                          lc.LrdbarChannel,
//                          lc.LrdbarIsDefault
//                      });
//            return Ok(xx);
//        }

//        //This method will insert a new row/ update the existing one in the Table LCommissionBatchAllocationRules depending on the Id found or not
//        [HttpPost]
//        public IHttpActionResult PostGridData(GridDataViewModel model)
//        {

//            using (var tran = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    string[] modelList = model.GridData.Split(',');
//                    for (var i = 0; i < modelList.Length; i = i + 6)
//                    {
//                        var Id = Convert.ToInt32(modelList[i + 4]);
//                        var isDefault = Convert.ToBoolean(modelList[i + 5]);
//                       // var AnalystUser = modelList[i + 3].ToString();
//                       // var AllocatedTo = db.AspNetUsers.Where(p => p.UserName.Equals(AnalystUser)).FirstOrDefault().Id;
//                        if (Id == 0)
//                        {

//                            var LCommissionBatch = new LCommissionBatchAllocationRule
//                            {
//                                LrdbarCompanyId = model.CompanyId,
//                                LrdbarPrimaryChannel = (string.IsNullOrEmpty(modelList[i])) ? null : modelList[i].ToString(),
//                                LrdbarBusinessUnit = (string.IsNullOrEmpty(modelList[i + 1])) ? null : modelList[i + 1].ToString(),
//                                LrdbarChannel = (string.IsNullOrEmpty(modelList[i + 2])) ? null : modelList[i + 2].ToString(),
//                                LrdbarAllocateToId = (string.IsNullOrEmpty(modelList[i + 3])) ? null : modelList[i + 3].ToString(),
//                                LrdbarIsDefault = isDefault ? true : false
//                            };
//                            if (!string.IsNullOrEmpty(LCommissionBatch.LrdbarAllocateToId))
//                            {
//                                db.LCommissionBatchAllocationRules.Add(LCommissionBatch);
//                                db.SaveChanges();
//                            }

//                        }
//                        else
//                        {
//                            if (!string.IsNullOrEmpty(modelList[i + 3]))
//                            {
//                                var LCommissionBatch = db.LCommissionBatchAllocationRules.Find(Id);
//                                LCommissionBatch.LrdbarCompanyId = model.CompanyId;
//                                LCommissionBatch.LrdbarPrimaryChannel = (string.IsNullOrEmpty(modelList[i])) ? null : modelList[i].ToString();
//                                LCommissionBatch.LrdbarBusinessUnit = (string.IsNullOrEmpty(modelList[i + 1])) ? null : modelList[i + 1].ToString();
//                                LCommissionBatch.LrdbarChannel = (string.IsNullOrEmpty(modelList[i + 2])) ? null : modelList[i + 2].ToString();
//                                LCommissionBatch.LrdbarAllocateToId = (string.IsNullOrEmpty(modelList[i + 3])) ? null : modelList[i + 3].ToString();
//                                LCommissionBatch.LrdbarIsDefault = isDefault ? true : false;
//                                db.Entry(LCommissionBatch).State = EntityState.Modified;
//                                db.SaveChanges();
//                            }
//                        }

//                    }

//                    tran.Commit();
//                }
//                catch (Exception ex)
//                {
//                    tran.Rollback();
//                    throw ex;
//                    //Globals.SendEmail("ssharma@megacube.com.au",null, "AllocationRules", ex.Message + "-" + ex.InnerException.Message);
//                    //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//                }
//            }

//            return Ok();
//        }

//        // DELETE: api/LAllocationRules/5
//        [ResponseType(typeof(LCommissionBatchAllocationRule))]
//        public async Task<IHttpActionResult> Delete(int id)
//        {
//            LCommissionBatchAllocationRule LCommissionAllocationRule = await db.LCommissionBatchAllocationRules.FindAsync(id);
//            if (LCommissionAllocationRule == null)
//            {
//                //return NotFound();
//                //CITY/POST CODE could not be found. Send appropriate response to the request.
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ALLOCATION RULE")));
//            }
//            try
//            {
//                db.LCommissionBatchAllocationRules.Remove(LCommissionAllocationRule);
//                await db.SaveChangesAsync();
//                return Ok(LCommissionAllocationRule);
//            }
//            catch (Exception ex)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//            }
//        }



//        private string GetCustomizedErrorMessage(Exception ex)
//        {
//            //Convert the exception to SqlException to get the error message returned by database.
//            var SqEx = ex.GetBaseException() as SqlException;


//            //Depending upon the constraint failed return appropriate error message
//            //Something else failed return original error message as retrieved from database
//            return SqEx.Message;
//        }

//    }
//}
