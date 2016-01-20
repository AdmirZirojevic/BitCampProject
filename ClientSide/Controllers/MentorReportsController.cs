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
    public class MentorReportsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/MentorReports
        public IQueryable<MentorReport> GetMentorReports()
        {
            return db.MentorReports;
        }

        // GET: api/MentorReports/5
        [ResponseType(typeof(MentorReport))]
        public IHttpActionResult GetMentorReport(int id)
        {
            MentorReport mentorReport = db.MentorReports.Find(id);
            if (mentorReport == null)
            {
                return NotFound();
            }

            return Ok(mentorReport);
        }

        // PUT: api/MentorReports/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMentorReport(int id, MentorReport mentorReport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mentorReport.Id)
            {
                return BadRequest();
            }

            db.Entry(mentorReport).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MentorReportExists(id))
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

        // POST: api/MentorReports
        [ResponseType(typeof(MentorReport))]
        public IHttpActionResult PostMentorReport(MentorReport mentorReport)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            mentorReport.DateSubmitted = DateTime.UtcNow;

           // Thread.Sleep(2000);
            mentorReport.MentorId = this.User.Identity.GetUserId();

            List<QuestionResponse> questionResponse = db.QuestionResponses.Where(x => x.MentorId == mentorReport.MentorId && x.SurveyId == mentorReport.SurveyId && x.StudentId == mentorReport.StudentId).ToList();
            mentorReport.Responses = questionResponse;

            db.MentorReports.Add(mentorReport);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = mentorReport.Id }, mentorReport);
        }

        // DELETE: api/MentorReports/5
        [ResponseType(typeof(MentorReport))]
        public IHttpActionResult DeleteMentorReport(int id)
        {
            MentorReport mentorReport = db.MentorReports.Find(id);
            if (mentorReport == null)
            {
                return NotFound();
            }

            db.MentorReports.Remove(mentorReport);
            db.SaveChanges();

            return Ok(mentorReport);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MentorReportExists(int id)
        {
            return db.MentorReports.Count(e => e.Id == id) > 0;
        }
    }
}