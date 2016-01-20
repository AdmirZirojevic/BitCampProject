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
using PagedList;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using System.IO;

namespace BitClassroom.MVC.Controllers
{
    public class AssignmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Assignments
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Index(string sortingOrder, string currentFilter, string searchString, int? page)
        {
       

            ViewBag.CurrentSort = sortingOrder;

            ViewBag.UsernameSort = sortingOrder == "nameAscending" ? "nameDescending" : "nameAscending";
            ViewBag.CourseSort = sortingOrder == "courseAscending" ? "courseDescending" : "courseAscending";
            ViewBag.TitleSort = string.IsNullOrEmpty(sortingOrder) ? "titleDescending" : "";
            ViewBag.DueDateSort = sortingOrder == "DueDate" ? "dueDateDescending" : "DueDate";
            ViewBag.StartDateSort = sortingOrder == "StartDate" ? "startDateDescending" : "StartDate";
            ViewBag.TypeSort = sortingOrder == "typeAscending" ? "typeDescending" : "typeAscending";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            
            var listOfAssignments = from x in db.Assignments select x;


            if (!String.IsNullOrEmpty(searchString) && searchString.Trim() != "")
            {
                listOfAssignments = listOfAssignments.Where(x => x.ApplicationUser.Name.ToLower().Contains(searchString.ToLower())
                                       || x.ApplicationUser.Lastname.ToLower().Contains(searchString.ToLower())
                                       || x.Course.Name.ToLower().Contains(searchString.ToLower())
                                       || x.Title.ToLower().Contains(searchString.ToLower())
                                       || x.Type.ToLower().Contains(searchString.ToLower()));
            }

            if (User.IsInRole("Admin"))
            {
                switch (sortingOrder)
                {
                    case "nameDescending":
                        listOfAssignments = listOfAssignments.OrderByDescending(x => x.ApplicationUser.Name);
                        break;
                    case "nameAscending":
                        listOfAssignments = listOfAssignments.OrderBy(x => x.ApplicationUser.Name);
                        break;
                    case "courseDescending":
                        listOfAssignments = listOfAssignments.OrderByDescending(x => x.Course.Name);
                        break;
                    case "courseAscending":
                        listOfAssignments = listOfAssignments.OrderBy(x => x.Course.Name);
                        break;
                    case "titleDescending":
                        listOfAssignments = listOfAssignments.OrderByDescending(x => x.Title);
                        break;
                    case "dueDateDescending":
                        listOfAssignments = listOfAssignments.OrderByDescending(x => x.DueDate);
                        break;
                    case "DueDate":
                        listOfAssignments = listOfAssignments.OrderBy(x => x.DueDate);
                        break;
                    case "startDateDescending":
                        listOfAssignments = listOfAssignments.OrderByDescending(x => x.RecDate);
                        break;
                    case "StartDate":
                        listOfAssignments = listOfAssignments.OrderBy(x => x.RecDate);
                        break;
                    case "typeDescending":
                        listOfAssignments = listOfAssignments.OrderByDescending(x => x.Type);
                        break;
                    case "typeAscending":
                        listOfAssignments = listOfAssignments.OrderBy(x => x.Type);
                        break;
                    default:
                        listOfAssignments = listOfAssignments.OrderBy(x => x.Title);
                        break;
                }

            
                int pageSize = 10;
                int pageNumber = (page ?? 1);  //means that if page is null set pageNumber to 1
                return View(listOfAssignments.ToPagedList(pageNumber, pageSize));              
            }

            //if user is teacher...
            switch (sortingOrder)
            {
                case "nameDescending":
                    listOfAssignments = listOfAssignments.OrderByDescending(x => x.ApplicationUser.Name);
                    break;
                case "nameAscending":
                    listOfAssignments = listOfAssignments.OrderBy(x => x.ApplicationUser.Name);
                    break;
                case "courseDescending":
                    listOfAssignments = listOfAssignments.OrderByDescending(x => x.Course.Name);
                    break;
                case "courseAscending":
                    listOfAssignments = listOfAssignments.OrderBy(x => x.Course.Name);
                    break;
                case "titleDescending":
                    listOfAssignments = listOfAssignments.OrderByDescending(x => x.Title);
                    break;
                case "dueDateDescending":
                    listOfAssignments = listOfAssignments.OrderByDescending(x => x.DueDate);
                    break;
                case "DueDate":
                    listOfAssignments = listOfAssignments.OrderBy(x => x.DueDate);
                    break;
                case "startDateDescending":
                    listOfAssignments = listOfAssignments.OrderByDescending(x => x.RecDate);
                    break;
                case "StartDate":
                    listOfAssignments = listOfAssignments.OrderBy(x => x.RecDate);
                    break;
                case "typeDescending":
                    listOfAssignments = listOfAssignments.OrderByDescending(x => x.Type);
                    break;
                case "typeAscending":
                    listOfAssignments = listOfAssignments.OrderBy(x => x.Type);
                    break;
                default:
                    listOfAssignments = listOfAssignments.OrderBy(x => x.Title);
                    break;
            }


            

            string userId = User.Identity.GetUserId();
            int pageSize2 = 10;
            int pageNumber2 = (page ?? 1);

            var assignments = listOfAssignments.Include(a => a.ApplicationUser).Include(a => a.Course);
            var result = assignments.Where(a => a.ApplicationUserId == userId).ToList();

            return View(result.ToPagedList(pageNumber2, pageSize2));

        }

        // GET: Assignments/Details/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);

            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }






        // GET: Assignments/Create
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Create()
        {    
            if (User.IsInRole("Teacher"))
            {
                string userId = User.Identity.GetUserId();
                List<CoursesTeach> coursesTeach = db.CoursesTeaches.Where(x => x.ApplicationUserId == userId).OrderBy(z => z.ApplicationUser.Name).ToList();
                ViewBag.CourseId = new SelectList(coursesTeach.Select(c => c.Course).OrderBy(z => z.Name), "Id", "Name");
                return View();
            }
            ViewBag.CourseId = new SelectList(db.Courses.OrderBy(z => z.Name), "Id", "Name");
            return View();
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Create([Bind(Include = "Id,DueDate,Title,Content,CourseId,VisibleToMentor,FileUpload")] Assignment assignment)
        {

            if (ModelState.IsValid)
            {
                assignment.ApplicationUserId = User.Identity.GetUserId();
                assignment.Type = "Assignment";

                assignment.RecDate = DateTime.UtcNow.AddHours(2);
                
                db.Assignments.Add(assignment);             

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Name", assignment.ApplicationUserId);
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", assignment.CourseId);
            return View(assignment);
        }



        // GET: Assignments/Edit/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);

         

            if (assignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Name", assignment.ApplicationUserId);
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", assignment.CourseId);
            return View(assignment);
        }



        // POST: Assignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Edit([Bind(Include = "Id,DueDate,Title,Content,CourseId,VisibleToMentor,FileUpload")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                assignment.ApplicationUserId = User.Identity.GetUserId();
                assignment.Type = "Assignment";
                assignment.RecDate = DateTime.UtcNow.AddHours(2);
                db.Entry(assignment).State = EntityState.Modified;
                assignment.TeacherUploads = db.TeacherUploads.Where(x => x.AssignmentId == assignment.Id).ToList();
                db.SaveChanges();
                   

                return RedirectToAction("Index");
            }


            



            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Name", assignment.ApplicationUserId);
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", assignment.CourseId);
            return View(assignment);
        }

        // GET: Assignments/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult DeleteConfirmed(int id)
        {
            Assignment assignment = db.Assignments.Find(id);         

            db.Assignments.Remove(assignment);
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
