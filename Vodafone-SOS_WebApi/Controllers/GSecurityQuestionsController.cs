using System;
using System.Data;
using System.Linq;
using System.Web.Http;
using Vodafone_SOS_WebApi.Models;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class GSecurityQuestionsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        [HttpGet]
        //GET: Method to Get Reset password Questions
        public IHttpActionResult GetGSecurityQuestions()
        {

            var xx = (from aa in db.GSecurityQuestions
                      select new
                      {
                          aa.Id,
                          aa.GsqQuestion
                      });

            return Ok(xx);
        }
        //GET: Method to Get Saved Question Answers
       public IHttpActionResult GetQuestionAnswersByUser(string userid)
        {
            var xx = (from aa in db.MAspnetUsersGSecurityQuestions.Where(p=>p.MAuqsqUserId==userid)
                      join bb in db.GSecurityQuestions on aa.MAuqsqQuestionId equals bb.Id
                      select new
                      {
                          aa.MAuqsqQuestionId,
                          aa.MAugsqAnswer,
                          bb.GsqQuestion
                         
                      }).OrderBy(p => p.MAuqsqQuestionId);
            return Ok(xx);
        }
        [HttpPost]
        //POST: Method to post Data into db
        public IHttpActionResult PostQuestionAnswers(MAspnetUsersGScurityQuestionViewModel model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {

                    var MAUGSQ = new MAspnetUsersGSecurityQuestion();
                    MAUGSQ.MAuqsqUserId = model.MAuqsqUserId;
                    MAUGSQ.MAuqsqQuestionId = model.Question1;
                    MAUGSQ.MAugsqAnswer = model.Answer1;
                    db.MAspnetUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    MAUGSQ.MAuqsqQuestionId = model.Question2;
                    MAUGSQ.MAugsqAnswer = model.Answer2;
                    db.MAspnetUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    MAUGSQ.MAuqsqQuestionId = model.Question3;
                    MAUGSQ.MAugsqAnswer = model.Answer3;
                    db.MAspnetUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }


            return Ok();



        }
        //PUT: Update Question Answer
        public IHttpActionResult PutQuestionAnswers(string userid, MAspnetUsersGScurityQuestionViewModel model)

        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var update = db.MAspnetUsersGSecurityQuestions.Where(p => p.MAuqsqUserId == userid).ToList();


                    db.MAspnetUsersGSecurityQuestions.RemoveRange(update);
                    db.SaveChanges();
                    var MAUGSQ = new MAspnetUsersGSecurityQuestion();
                    MAUGSQ.MAuqsqUserId = model.MAuqsqUserId;
                    MAUGSQ.MAuqsqQuestionId = model.Question1;
                    MAUGSQ.MAugsqAnswer = model.Answer1;
                    db.MAspnetUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    MAUGSQ.MAuqsqQuestionId = model.Question2;
                    MAUGSQ.MAugsqAnswer = model.Answer2;
                    db.MAspnetUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    MAUGSQ.MAuqsqQuestionId = model.Question3;
                    MAUGSQ.MAugsqAnswer = model.Answer3;
                    db.MAspnetUsersGSecurityQuestions.Add(MAUGSQ);
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }

            }
            return Ok();



        }
    }
}