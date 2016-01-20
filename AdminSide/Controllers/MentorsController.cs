
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitClassroom.DAL.Models;
using System.Data.Entity;
using System.Net;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;

namespace BitClassroom.MVC.Controllers
{
    public class MentorsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Mentors
         [Authorize(Roles = "Admin")]
        public ActionResult Index(string sortingOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortingOrder;

            ViewBag.MentorNameSort = sortingOrder == "mentorNameAscending" ? "mentorNameDescending" : "mentorNameAscending";
            ViewBag.MentorLastnameSort = sortingOrder == "mentorLastnameAscending" ? "mentorLastnameDescending" : "mentorLastnameAscending";
            ViewBag.StudentNameSort = sortingOrder == "studentNameAscending" ? "studentNameDescending" : "studentNameAscending";
            ViewBag.StudentLastnameSort = sortingOrder == "studentLastnameAscending" ? "studentLastnameDescending" : "studentLastnameAscending";


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var listOfAssignedMentors = from x in db.Users.Where(x => x.MentorId != null) select x;

            if (!String.IsNullOrEmpty(searchString) && searchString.Trim() != "")
            {
                listOfAssignedMentors = listOfAssignedMentors.Where(x => x.Mentor.Name.ToLower().Contains(searchString.ToLower())
                                       || x.Mentor.Lastname.ToLower().Contains(searchString.ToLower())
                                       || x.Name.ToLower().Contains(searchString.ToLower())
                                       || x.Lastname.ToLower().Contains(searchString.ToLower()));
            }
         
                switch (sortingOrder)
                {
                    case "mentorNameAscending":
                        listOfAssignedMentors = listOfAssignedMentors.OrderBy(x => x.Mentor.Name);
                        break;
                    case "mentorNameDescending":
                        listOfAssignedMentors = listOfAssignedMentors.OrderByDescending(x => x.Mentor.Name);
                        break;
                    case "mentorLastnameAscending":
                        listOfAssignedMentors = listOfAssignedMentors.OrderBy(x => x.Mentor.Lastname);
                        break;
                    case "mentorLastnameDescending":
                        listOfAssignedMentors = listOfAssignedMentors.OrderByDescending(x => x.Mentor.Lastname);
                        break;
                    case "studentNameAscending":
                        listOfAssignedMentors = listOfAssignedMentors.OrderBy(x => x.Name);
                        break;
                    case "studentNameDescending":
                        listOfAssignedMentors = listOfAssignedMentors.OrderByDescending(x => x.Name);
                        break;
                    case "studentLastnameAscending":
                        listOfAssignedMentors = listOfAssignedMentors.OrderBy(x => x.Lastname);
                        break;
                    case "studentLastnameDescending":
                        listOfAssignedMentors = listOfAssignedMentors.OrderByDescending(x => x.Lastname);
                        break;
                    default:
                        listOfAssignedMentors = listOfAssignedMentors.OrderBy(x => x.Id);
                        break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);  //means that if page is null set pageNumber to 1
                return View(listOfAssignedMentors.ToPagedList(pageNumber, pageSize));                         
           
            //return View(db.Users.Where(x => x.MentorId != null).ToList());

        }

        // GET
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
 
            var mentorRole = db.Roles.Where(x => x.Name.Contains("Mentor")).FirstOrDefault();
            var studentRole = db.Roles.Where(x => x.Name.Contains("Student")).FirstOrDefault();

            ViewBag.MentorId = new SelectList(db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(mentorRole.Id)).OrderBy(z=>z.Name), "Id","FullName");
            ViewBag.StudentId = new SelectList(db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(studentRole.Id)).OrderBy(z => z.Name), "Id", "FullName");
          
            return View();
        }

        // POST
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(string studentId, string mentorId)
        {
            ApplicationUser user = new ApplicationUser();
            string userId = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                var student = db.Users.Find(studentId);
                var mentor = db.Users.Find(mentorId);
                student.Mentor = mentor;
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();

                Notification notification = new Notification();
                notification.SenderId = mentorId;
                notification.RecipientId = studentId;
                notification.Content = "is assigned to you as new mentor";
                db.Notifications.Add(notification);

                Notification notification1 = new Notification();
                notification1.RecipientId = mentorId;
                notification1.SenderId = studentId;
                notification1.Content = "is assigned to you as new mentee";
                db.Notifications.Add(notification1);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.MentorId = new SelectList(db.Users, "Id", "FullName", user.MentorId);
            ViewBag.StudentId = new SelectList(db.Users, "Id", "FullName", user.Id);
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Details(string studentId, string mentorId)
        {
            if (studentId == null || mentorId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<ApplicationUser> user = db.Users.Where(x => x.Id == studentId && x.MentorId == mentorId).ToList();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user[0]);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string studentId, string mentorId)
        {
            if (studentId == null || mentorId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<ApplicationUser> user = db.Users.Where(x => x.Id == studentId && x.MentorId == mentorId).ToList();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user[0]);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(string studentId, string mentorId)
        {
            var student = db.Users.Find(studentId);            
            if (student == null)
            {
                return HttpNotFound();
            }
            student.MentorId = null;
            db.Entry(student).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }  

    }

}

