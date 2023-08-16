using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class LEmailBucketController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        // GET: LEmailBucket
        //GET: Method to Get Min Date using LebCreatedDateTime from db
        public IHttpActionResult GetLEmailBucketMinDate()
        {
            DateTime? MinCreatedDate=null;
            var xx = (from aa in db.LEmailBuckets
                      where aa.LebStatus == "InQueue"
                      select new
                      {
                          aa.LebCreatedDateTime
                      }).OrderBy(p=>p.LebCreatedDateTime).FirstOrDefault();
            if (xx != null)
            {
                MinCreatedDate = xx.LebCreatedDateTime;
            }
            return Ok(MinCreatedDate);
        }

        //GET: Method to Get count where status is InQueue from LEmailBucket
        public IHttpActionResult GetLEmailBucketCount()
        {
           
            var xx = db.LEmailBuckets.Count(a=>a.LebStatus=="InQueue");
            return Ok(xx);
        }

        //GET: Method to Get Max Date  for that max LebCreatedDateTime from db
        //Query can be fail if case of  two Same CreateDateTime
        public IHttpActionResult GetLEmailBucketMaxDate()
        {

            var xx = (from aa in db.LEmailBuckets

                      select new
                      {
                          aa.LebCreatedDateTime,
                        
                          
                      }).Max(p=>p.LebCreatedDateTime);
             return Ok(xx);
        }

        public IHttpActionResult GetLEmailBucketMaxStatus()
        {

            var xx = (from aa in db.LEmailBuckets

                      select new
                      {
                          aa.LebStatus


                      }).Max(p => p.LebStatus);
            return Ok(xx);
        }

        
        public IHttpActionResult GetEmailBucketDetail(int pagesize, int pagenum, string sortdatafield, string sortorder, string FilterQuery)
        {
            string Qry = string.Empty;
            var SortQuery = "";
            if (!string.IsNullOrEmpty(sortdatafield))
            {
                SortQuery = " order by " + sortdatafield + " " + sortorder;
            }
            else
            {
                SortQuery = " ORDER BY LebCreatedDateTime desc";
            }

            if (FilterQuery == null)
            {
                //Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(select gc.GcCode, LebBody, LebRecipientList, LebSubject, LebEmailType, LebCreatedDateTime, LebCreatedById, LebStatus, LebUpdatedDateTime " +
                //   "FROM LEmailBucket leb INNER JOIN AspNetUsers ap  on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus = 'sent' and LebCreatedDateTime >= dateadd(dd, -30, getdate()))A " +
                //   ") B WHERE B.datacount > @P1 AND B.datacount <= @P2";

                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(select gc.GcCode, LebBody, LebRecipientList, LebSubject, LebEmailType, LebCreatedDateTime, LebCreatedById, LebStatus, LebUpdatedDateTime " +
                  //"FROM LEmailBucket leb INNER JOIN AspNetUsers ap  on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus In( 'sent', 'InQueue') )A " +
                  "FROM LEmailBucket leb INNER JOIN AspNetUsers ap  on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId  )A " +
                    ") B WHERE B.datacount > @P1 AND B.datacount <= @P2";
            }
            else
            {
                FilterQuery = "WHERE 1=1 " + FilterQuery;
                //Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(select gc.GcCode, LebBody, LebRecipientList, LebSubject, LebEmailType, LebCreatedDateTime, LebCreatedById, LebStatus, LebUpdatedDateTime " +
                //    "FROM LEmailBucket leb INNER JOIN AspNetUsers ap  on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus = 'sent' and LebCreatedDateTime >= dateadd(dd, -30, getdate()))A "+
                //     FilterQuery + ") B WHERE B.datacount > @P1 AND B.datacount <= @P2";

                Qry = "SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(select gc.GcCode, LebBody, LebRecipientList, LebSubject, LebEmailType, LebCreatedDateTime, LebCreatedById, LebStatus, LebUpdatedDateTime " +
                    //"FROM LEmailBucket leb INNER JOIN AspNetUsers ap  on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus In( 'sent', 'InQueue') )A " +
                    "FROM LEmailBucket leb INNER JOIN AspNetUsers ap  on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId )A " +
                     FilterQuery + ") B WHERE B.datacount > @P1 AND B.datacount <= @P2";
            }

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<LEmailBucketViewModel>(Qry, parameters).ToList();
            return Ok(xx);
           
        }

        
        public IHttpActionResult GetEmailBucketDetailCount()
        {
            //string Qry = "select gc.GcCode,LebBody,LebRecipientList, LebSubject, LebEmailType, LebCreatedDateTime, LebCreatedById, LebStatus, LebUpdatedDateTime FROM LEmailBucket leb INNER JOIN AspNetUsers ap on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus = 'sent' and LebCreatedDateTime>= dateadd(dd, -30, getdate())";
            string Qry = "select gc.GcCode,LebBody,LebRecipientList, LebSubject, LebEmailType, LebCreatedDateTime, LebCreatedById, LebStatus, LebUpdatedDateTime FROM LEmailBucket leb INNER JOIN AspNetUsers ap on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus In( 'sent', 'InQueue') ";
            var xx = db.Database.SqlQuery<LEmailBucketViewModel>(Qry).Count();
            return Ok(xx);
        }

       
        public IHttpActionResult GetEmailBucketSummary(string sortdatafield, string sortorder, int pagesize, int pagenum, string FilterQuery)
        {
            //SELECT* FROM(select gc.GcCode, count(LebStatus) as EmailSent,ROW_NUMBER() OVER(ORDER BY LebStatus desc) as datacount from LEmailBucket
            //inner join AspNetUsers ap  on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId
            // where LebStatus = 'sent'  and LebCreatedDateTime>= dateadd(dd, -90, getdate()) group by gc.GcCode,LebStatus) a where datacount >= 1 and datacount<= 2


            //string Qry = "select gc.GcCode, count(LebStatus) as EmailSent from LEmailBucket inner join AspNetUsers ap on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus = 'sent' and LebCreatedDateTime>= dateadd(dd, -90, getdate()) group by gc.GcCode";
            //var xx = db.Database.SqlQuery<LEmailBucketViewModel>(Qry).ToList();
            //return Ok(xx);
           // string Qry = string.Empty;
            var SortQuery = "";
            if (!string.IsNullOrEmpty(sortdatafield))
            {
                SortQuery = " order by " + sortdatafield + " " + sortorder;
            }
            else
            {
                SortQuery = " ORDER BY EmailSent desc";
            }
            string Qry = string.Empty;
            if (FilterQuery == null)
            {
                Qry = " SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(select gc.GcCode, count(LebStatus) " +
                    "as EmailSent FROM LEmailBucket leb INNER JOIN AspNetUsers ap  on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId " +
                    " where LebStatus In( 'sent', 'InQueue') and LebCreatedDateTime >= dateadd(dd, -30, getdate())  GROUP BY gc.GcCode, leb.LebStatus)A " +
                    ") B WHERE B.datacount > @P1 AND B.datacount <= @P2";
            }
            else
            {
                // Qry = "SELECT * FROM(select gc.GcCode, count(LebStatus) as EmailSent,ROW_NUMBER() OVER(ORDER BY LebStatus desc) as datacount from LEmailBucket inner join AspNetUsers ap  on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus = 'sent'  and LebCreatedDateTime>= dateadd(dd, -90, getdate()) group by gc.GcCode,LebStatus) a where datacount >= @P1 and datacount<= @P2" + FilterQuery;
                FilterQuery = "WHERE 1=1 " + FilterQuery;

                Qry = " SELECT * FROM(SELECT *, ROW_NUMBER() OVER (" + SortQuery + ") as datacount FROM(select gc.GcCode, count(LebStatus) " +
                   "as EmailSent FROM LEmailBucket leb INNER JOIN AspNetUsers ap  on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId "+
                   " where LebStatus In( 'sent', 'InQueue') and LebCreatedDateTime >= dateadd(dd, -30, getdate())  GROUP BY gc.GcCode, leb.LebStatus)A " +
                   FilterQuery + ") B WHERE B.datacount > @P1 AND B.datacount <= @P2";

            }
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@P1", pagenum * pagesize));
            parameterList.Add(new SqlParameter("@P2", (pagenum + 1) * pagesize));
            //parameterList.Add(new SqlParameter("@P3", qq));
            SqlParameter[] parameters = parameterList.ToArray();
            var xx = db.Database.SqlQuery<LEmailBucketViewModel>(Qry, parameters).ToList();

            return Ok(xx);
        }

        public IHttpActionResult GetEmailBucketChart()
        {            
            string Qry = string.Empty;
            //Qry = "select gc.GcCode, count(LebStatus) as EmailSent FROM LEmailBucket leb INNER JOIN AspNetUsers ap on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus = 'sent' and LebCreatedDateTime>= dateadd(dd, -30, getdate()) GROUP BY gc.GcCode,leb.LebStatus";
            Qry = "select gc.GcCode, count(LebStatus) as EmailSent FROM LEmailBucket leb INNER JOIN AspNetUsers ap on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus In( 'sent', 'InQueue')  GROUP BY gc.GcCode,leb.LebStatus";
            var xx = db.Database.SqlQuery<LEmailBucketViewModel>(Qry).ToList();
            return Ok(xx);
        }

        public IHttpActionResult GetEmailBucketSummaryCount()
        {
            //string Qry = "select gc.GcCode, count(LebStatus) as EmailSent FROM LEmailBucket leb INNER JOIN AspNetUsers ap on ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus = 'sent' and LebCreatedDateTime>= dateadd(dd, -30, getdate()) GROUP BY gc.GcCode,leb.LebStatus";

            string Qry = "select gc.GcCode, count(LebStatus) as EmailSent FROM LEmailBucket leb INNER JOIN AspNetUsers ap on" +
                " ap.Id = LebCreatedById inner join GCompanies gc on gc.Id = ap.GcCompanyId where LebStatus In( 'sent', 'InQueue')  " +
                " and LebCreatedDateTime >= dateadd(dd, -30, getdate()) GROUP BY gc.GcCode,leb.LebStatus";
            var xx = db.Database.SqlQuery<LEmailBucketViewModel>(Qry).Count();
            return Ok(xx);
        }

    }
}