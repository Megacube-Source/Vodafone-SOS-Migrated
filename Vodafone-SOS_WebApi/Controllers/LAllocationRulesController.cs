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
//{
//    [CustomExceptionFilter]
//    public class LAllocationRulesController : ApiController
//    {
       
//        private VodafoneSOSLiteEntities db = new VodafoneSOSLiteEntities();

//        [HttpPost]
//        public IHttpActionResult PostLAllocationRulesGrid(GridDataViewModel model)
//        {

//            using (var tran = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    string[] modelList = model.GridData.Split(',');
//                    for (var i = 0; i < modelList.Length; i = i + 5)
//                    {
//                            var Id = Convert.ToInt32(modelList[i + 2]);
//                        var AnalystUser = modelList[i + 1].ToString();
//                            var AllocatedTo = db.AspNetUsers.Where(p => p.UserName.Equals(AnalystUser) ).FirstOrDefault().Id;
//                        if (Id == 0)
//                        {

//                            var LAllocationRules = new LAllocationRule
//                            {
//                                LarCompanyId = model.CompanyId,
//                                LarKey = (string.IsNullOrEmpty(modelList[i])) ? null : modelList[i].ToString(),
//                                LarAllocatedToId = (string.IsNullOrEmpty(modelList[i + 1])) ? null : AllocatedTo,
//                                LarValue = modelList[i + 3].ToString(),
//                                LarOrdinalPosition = Convert.ToInt32(modelList[i + 4])
//                            };
//                            if (!string.IsNullOrEmpty(LAllocationRules.LarValue))
//                            {
//                                db.LAllocationRules.Add(LAllocationRules);
//                                db.SaveChanges();
//                            }

//                        }
//                        else
//                        {
//                            if (!string.IsNullOrEmpty(modelList[i + 3]))
//                            {
//                                var LAllocationRule = db.LAllocationRules.Find(Id);
//                                LAllocationRule.LarCompanyId = model.CompanyId;
//                                LAllocationRule.LarKey = (string.IsNullOrEmpty(modelList[i])) ? null : modelList[i];
//                                LAllocationRule.LarAllocatedToId = (string.IsNullOrEmpty(modelList[i + 1])) ? null : AllocatedTo;
//                                LAllocationRule.Id = Convert.ToInt32(modelList[i + 2]);
//                                LAllocationRule.LarValue = modelList[i + 3];
//                                LAllocationRule.LarOrdinalPosition = Convert.ToInt32(modelList[i + 4]);
//                                db.Entry(LAllocationRule).State = EntityState.Modified;
//                                db.SaveChanges();
//                            }
//                        }
                       
//                    }

//                    tran.Commit();
//                }
//                catch (Exception ex)
//                {
//                    tran.Rollback();
//                    //Globals.SendEmail("ssharma@megacube.com.au",null, "AllocationRules", ex.Message + "-" + ex.InnerException.Message);
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//                }
//            }

//            return Ok();
//        }

//        [ResponseType(typeof(LAllocationRule))]
//        public IHttpActionResult GetLAllocationRulesByCompanyId(int CompanyId)
//        {
//            var xx = (from aa in db.LAllocationRules.Where(p => p.LarCompanyId == CompanyId)
//                      select new
//                      {
//                          aa.Id,aa.AspNetUser.UserName,aa.LarAllocatedToId,aa.LarCompanyId,aa.LarKey,aa.LarOrdinalPosition,aa.LarValue
//                      }).OrderBy(p=>p.LarOrdinalPosition);
//            return Ok(xx);
//        }

//        /*
//        //Added by shivanig- Commission to pay task
//        [ResponseType(typeof(LCommissionBatchAllocationRule))]
//        public IHttpActionResult GetLCommissionBatchAllocationRulesByCompanyId(int CompanyId)
//        {
//            //Applying JOIN on LUsers as we dont have implicit join here to get the username
//            var xx = (from lc in db.LCommissionBatchAllocationRules
//                      join lu in db.LUsers on lc.LrdbarAllocateToId equals lu.LuUserId
//                      select new
//                      {
//                          lc.Id,
//                          username = lu.LuFirstName + " " + lu.LuLastName,
//                          lc.LrdbarAllocateToId,
//                          lc.LrdbarCompanyId,
//                          lc.LrdbarPrimaryChannel,
//                          lc.LrdbarBusinessUnit,
//                          lc.LrdbarChannel
//                      });
//            return Ok(xx);
//        }*/
//        //added by shivanig
//        //This method will insert a new row/ edit the existing one in the Table LCommissionBatchAllocationRules

//        /*[HttpPost]
//        public IHttpActionResult SaveCommissionAllocationRule(GridDataViewModel model)
//        {

//            using (var tran = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    string[] modelList = model.GridData.Split(',');
//                    for (var i = 0; i < modelList.Length; i = i + 5)
//                    {
//                        var Id = Convert.ToInt32(modelList[i + 4]);
//                        var AllocatedTo = modelList[i + 3].ToString();
//                        //var AllocatedTo = db.AspNetUsers.Where(p => p.UserName.Equals(AnalystUser)).FirstOrDefault().Id;
//                        if (Id == 0)
//                        {

//                            var LCommissionBatch = new LCommissionBatchAllocationRule
//                            {
//                                LrdbarCompanyId = model.CompanyId,
//                                LrdbarPrimaryChannel = (string.IsNullOrEmpty(modelList[i])) ? null : modelList[i].ToString(),
//                                LrdbarBusinessUnit = (string.IsNullOrEmpty(modelList[i + 1])) ? null : modelList[i].ToString(),
//                                LrdbarChannel = (string.IsNullOrEmpty(modelList[i + 2])) ? null : modelList[i].ToString(),
//                                LrdbarAllocateToId = (string.IsNullOrEmpty(modelList[i + 3])) ? null : AllocatedTo,
                                
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
//                                LCommissionBatch.LrdbarAllocateToId = (string.IsNullOrEmpty(modelList[i + 3])) ? null : AllocatedTo;
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
//                    //Globals.SendEmail("ssharma@megacube.com.au",null, "AllocationRules", ex.Message + "-" + ex.InnerException.Message);
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//                }
//            }

//            return Ok();
//        }
//        */

//        // GET: api/LAllocationRules/5
//        [ResponseType(typeof(LAllocationRule))]
//        public async Task<IHttpActionResult> GetLAllocationRule(int id)
//        {
//            LAllocationRule LAllocationRule = await db.LAllocationRules.FindAsync(id);
//            if (LAllocationRule == null)
//            {
//                //return NotFound();
//                //ALLOCATION RULE to be displayed could not be found. Send appropriate response to the request.
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ALLOCATION RULE")));
//            }
//            return Ok(LAllocationRule);
//        }

//        // PUT: api/LAllocationRules/5
//        [ResponseType(typeof(void))]
//        public async Task<IHttpActionResult> PutLAllocationRule(int id, LAllocationRule LAllocationRule)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ALLOCATION RULE")));
//            }

//            if (id != LAllocationRule.Id)
//            {
//                //return BadRequest();
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "UPDATE", "ALLOCATION RULE")));
//            }
//            try
//            {
//                db.Entry(LAllocationRule).State = EntityState.Modified;
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                if (!LAllocationRuleExists(id))
//                {
//                    //return NotFound();
//                    //CITY/POST CODE could not be found. Send appropriate response to the request.
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ALLOCATION RULE")));
//                }
//                else
//                {
//                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//                }
//            }
//            return StatusCode(HttpStatusCode.NoContent);
//        }

//        // POST: api/LAllocationRules
//        [ResponseType(typeof(LAllocationRule))]
//        public async Task<IHttpActionResult> PostLAllocationRule(LAllocationRule LAllocationRule)
//        {
//            if (!ModelState.IsValid)
//            {
//                //return BadRequest(ModelState);
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Format(Globals.BadRequestErrorMessage, "CREATE", "ALLOCATION RULE")));
//            }
//            try
//            {
//                db.LAllocationRules.Add(LAllocationRule);
//                await db.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//            }
//            return CreatedAtRoute("DefaultApi", new { id = LAllocationRule.Id }, LAllocationRule);
//        }
//        /*
//        //added by shivanig
//        // DELETE: api/LAllocationRules/5
//        [ResponseType(typeof(LAllocationRule))]
//        public async Task<IHttpActionResult> DeleteLCommissionBactchAllocationRule(int id)
//        {
//           LCommissionBatchAllocationRule LCommissionAllocationRule = await db.LCommissionBatchAllocationRules.FindAsync(id);
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

//    */
//        // DELETE: api/LAllocationRules/5
//        [ResponseType(typeof(LAllocationRule))]
//        public async Task<IHttpActionResult> DeleteLAllocationRule(int id)
//        {
//            LAllocationRule LAllocationRule = await db.LAllocationRules.FindAsync(id);
//            if (LAllocationRule == null)
//            {
//                //return NotFound();
//                //CITY/POST CODE could not be found. Send appropriate response to the request.
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format(Globals.NotFoundErrorMessage, "ALLOCATION RULE")));
//            }
//            try
//            {
//                db.LAllocationRules.Remove(LAllocationRule);
//                await db.SaveChangesAsync();
//                return Ok(LAllocationRule);
//            }
//            catch (Exception ex)
//            {
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, GetCustomizedErrorMessage(ex)));
//            }
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        private bool LAllocationRuleExists(int id)
//        {
//            return db.LAllocationRules.Count(e => e.Id == id) > 0;
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
