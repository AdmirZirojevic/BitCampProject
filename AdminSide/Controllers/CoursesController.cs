using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BitClassroom.DAL.Models;
using CloudinaryDotNet;
using PagedList;

namespace BitClassroom.MVC.Controllers
{
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Courses
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Index(string sortingOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortingOrder;
    
            ViewBag.CourseSort = sortingOrder == "courseAscending" ? "courseDescending" : "courseAscending";
          

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var listOfCourses = from x in db.Courses select x;

            if (!String.IsNullOrEmpty(searchString) && searchString.Trim() != "")
            {
                listOfCourses = listOfCourses.Where(x => x.Name.ToLower().Contains(searchString.ToLower()));
            }

          
                switch (sortingOrder)
                {           
                    case "courseDescending":
                        listOfCourses = listOfCourses.OrderByDescending(x => x.Name);
                        break;
                    case "courseAscending":
                        listOfCourses = listOfCourses.OrderBy(x => x.Name);
                        break;  
                    default:
                        listOfCourses = listOfCourses.OrderBy(x => x.Id);
                       break;
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);  //means that if page is null set pageNumber to 1
                return View(listOfCourses.ToPagedList(pageNumber, pageSize));    
           
        }

        // GET: Courses/Details/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }
        [Authorize(Roles = "Admin")]
        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Name,PictureUrl,Price,Calendar")] Course course)
        {
          
            HelperController hc = new HelperController();
            hc.CloudUploadCreate(course);

            if (ModelState.IsValid)
            {
                db.Courses.Add(course);             
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }
        [Authorize(Roles = "Admin")]
        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Name,PictureUrl")] Course course)
        {
             
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();

            //this block of code deletes the image from cloudinary, after the course is deleted
            Account acount = new Account("gigantor", "986286566519458", "GT87e1BTMnfLut1_gXhSH0giZPg");
            Cloudinary cloudinary = new Cloudinary(acount);

            if (course.PictureUrl != null && course.PictureUrl.StartsWith("http://res.cloudinary.com/gigantor/image/upload/")) 
            {
                //this here is just a string manipulation to get to the ImageID from cloudinary
                string assist = "http://res.cloudinary.com/gigantor/image/upload/";
                string part1 = course.PictureUrl.Remove(course.PictureUrl.IndexOf(assist), assist.Length);
                string part2 = part1.Remove(0, 12);
                string toDelete = part2.Remove(part2.Length - 4);
                cloudinary.DeleteResources(toDelete);  //this finally deletes the image
            }
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
