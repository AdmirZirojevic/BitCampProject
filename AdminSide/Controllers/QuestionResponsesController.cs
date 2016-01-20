using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BitClassroom.DAL.Models;

namespace BitClassroom.MVC.Controllers
{
    public class QuestionResponsesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: QuestionResponses
        public ActionResult Index()
        {
          
            var questionResponses = db.QuestionResponses;
          
            var surveyTitle = db.Surveys.Include(st => st.Title);
            return View(questionResponses.ToList());
        }


        public ActionResult SurveyQuestionResponses(int id)
        {
            var questionResponses = db.QuestionResponses.Where(qr => qr.SurveyId == id); //.Include(q => q.Question);
          
            return View(questionResponses.ToList());
        }

        // GET: QuestionResponses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionResponse questionResponse = db.QuestionResponses.Find(id);
            if (questionResponse == null)
            {
                return HttpNotFound();
            }
            return View(questionResponse);
        }

      

        // GET: QuestionResponses/Create
        public ActionResult Create()
        {
            ViewBag.MentorReportId = new SelectList(db.MentorReports.Include("Mentor").Include("Student"), "Id", "MentorStudentCombo");
       
            ViewBag.SurveyId = new SelectList(db.Surveys, "Id", "Title");
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "QContent");
        
            return View();
        }


        // POST: QuestionResponses/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MentorReportId,SurveyId,Answer,QuestionId")] QuestionResponse questionResponse)
        {
            if (ModelState.IsValid)
            {
                db.QuestionResponses.Add(questionResponse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MentorReportId = new SelectList(db.MentorReports.Include("Mentor").Include("Student"), "Id", "MentorStudentCombo");
            ViewBag.SurveyId = new SelectList(db.Surveys, "Id", "Title");
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "QContent", questionResponse.QuestionId);
            return View(questionResponse);
        }

        // GET: QuestionResponses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionResponse questionResponse = db.QuestionResponses.Find(id);
            if (questionResponse == null)
            {
                return HttpNotFound();
            }

            var MentorReportId = db.MentorReports.Include("Mentor").Include("Student");
            ViewBag.SurveyId = new SelectList(db.Surveys, "Id", "Title");
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "QContent", questionResponse.QuestionId);
            
            return View(questionResponse);
        }

        // POST: QuestionResponses/Edit/5
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MentorReportId,SurveyId,Answer,QuestionId")] QuestionResponse questionResponse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionResponse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MentorReportId = new SelectList(db.MentorReports.Include("Mentor").Include("Student"), "Id", "MentorStudentCombo");
            ViewBag.SurveyId = new SelectList(db.Surveys, "Id", "Title");
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "QContent", questionResponse.QuestionId);
            return View(questionResponse);
        }

        // GET: QuestionResponses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionResponse questionResponse = db.QuestionResponses.Find(id);
            if (questionResponse == null)
            {
                return HttpNotFound();
            }
            return View(questionResponse);
        }

        // POST: QuestionResponses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuestionResponse questionResponse = db.QuestionResponses.Find(id);
            db.QuestionResponses.Remove(questionResponse);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
