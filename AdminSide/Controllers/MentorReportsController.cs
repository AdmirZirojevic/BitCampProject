using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BitClassroom.DAL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace BitClassroom.MVC.Controllers
{
    public class MentorReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /MentorReports/
        public ActionResult Index()
        {
            var mentorreports = db.MentorReports.ToList();

            return View(mentorreports.ToList());
        }

        
        // GET: /MentorReports/Details/5
        /// <summary>
        /// Method for obtaining information on specific mentor report using survey id, mentor id, and student id.
        /// </summary>
        /// <param name="id">mentor report id</param>
        /// <returns>mentor report object</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            MentorReport mentorreport = db.MentorReports.Find(id);

            try
            {
                var qr = db.QuestionResponses.Where(x => x.MentorId == mentorreport.MentorId && x.StudentId == mentorreport.StudentId && x.SurveyId == mentorreport.SurveyId).ToList();

                mentorreport.Responses = qr;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
          
          
            if (mentorreport == null)
            {
                return HttpNotFound();
            }
            return View(mentorreport);
        }


        // GET: /MentorReports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MentorReport mentorreport = db.MentorReports.Find(id);
            if (mentorreport == null)
            {
                return HttpNotFound();
            }
            return View(mentorreport);
        }

        // POST: /MentorReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MentorReport mentorreport = db.MentorReports.Find(id);

            var qr = db.QuestionResponses.Where(x => x.MentorId == mentorreport.MentorId && x.StudentId == mentorreport.StudentId && x.SurveyId == mentorreport.SurveyId).ToList();

            foreach (var item in qr)
            {
                db.QuestionResponses.Remove(item);
            }

            db.MentorReports.Remove(mentorreport);
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
