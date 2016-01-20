using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BitClassroom.DAL.Models;

namespace BitClassroom.MVC.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize]
        public ActionResult Index()
        {
            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpPost]
        public ActionResult FindCourse(string name)
        {
            List<Course> course = new List<Course>();

            if (name != null)
            {
                course = db.Courses.Where(x => x.Name.Contains(name)).ToList();
            }

            return Json(course);
        }


       
        [HttpGet]
        public ActionResult SurveyQuestions(int id)
        {
            Survey survey = db.Surveys.Find(id);
          
            Survey template = new Survey();

            template.Id = id;
            template.Title = survey.Title;
            List<QuestionResponse> qResponses = db.QuestionResponses.Where(x => x.SurveyId == survey.Id).ToList();
          
            List<QuestionResponse> surveyQuestions = new List<QuestionResponse>();

            foreach (var item in qResponses)
            {
                surveyQuestions.Add(item);
            }

            surveyQuestions.OrderBy(x => x.Id).ToList();

            template.Responses = surveyQuestions.OrderByDescending(x => x.Id).ToList();

            return Json(template, JsonRequestBehavior.AllowGet);
        }

   

    }
}