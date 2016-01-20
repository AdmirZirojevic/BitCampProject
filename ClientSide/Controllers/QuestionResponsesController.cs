using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using BitClassroom.DAL.Models;
using System.Threading;

namespace BitClassroom.API.Controllers
{
    public class QuestionResponsesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/QuestionResponses
        public IQueryable<QuestionResponse> GetQuestionResponses()
        {
            return db.QuestionResponses;
        }

        // GET: api/QuestionResponses/5
        [ResponseType(typeof(QuestionResponse))]
        public IHttpActionResult GetQuestionResponse(int id)
        {
            QuestionResponse questionResponse = db.QuestionResponses.Find(id);
            if (questionResponse == null)
            {
                return NotFound();
            }

            return Ok(questionResponse);
        }

        // PUT: api/QuestionResponses/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQuestionResponse(int id, QuestionResponse questionResponse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != questionResponse.Id)
            {
                return BadRequest();
            }

            db.Entry(questionResponse).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionResponseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/QuestionResponses
        [ResponseType(typeof(QuestionResponse))]
        public IHttpActionResult PostQuestionResponse(QuestionResponse questionResponse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Thread.Sleep(1000);
            questionResponse.MentorId = this.User.Identity.GetUserId();

            var surveyQuestion = db.SurveyQuestions.Where(x => x.Id == questionResponse.QuestionId).FirstOrDefault();
            questionResponse.SurveyQuestion = surveyQuestion;

            db.QuestionResponses.Add(questionResponse);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = questionResponse.Id }, questionResponse);
        }

        // DELETE: api/QuestionResponses/5
        [ResponseType(typeof(QuestionResponse))]
        public IHttpActionResult DeleteQuestionResponse(int id)
        {
            QuestionResponse questionResponse = db.QuestionResponses.Find(id);
            if (questionResponse == null)
            {
                return NotFound();
            }

            db.QuestionResponses.Remove(questionResponse);
            db.SaveChanges();

            return Ok(questionResponse);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuestionResponseExists(int id)
        {
            return db.QuestionResponses.Count(e => e.Id == id) > 0;
        }
    }
}