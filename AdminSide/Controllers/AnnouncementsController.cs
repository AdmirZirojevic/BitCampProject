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

namespace BitClassroom.MVC.Controllers
{
    public class AnnouncementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Announcements
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Index(string sortingOrder, string currentFilter, string searchString,int? page)
        {
            ViewBag.CurrentSort = sortingOrder;

            ViewBag.UsernameSort = sortingOrder == "nameAscending" ? "nameDescending" : "nameAscending";
            ViewBag.CourseSort = sortingOrder == "courseAscending" ? "courseDescending" : "courseAscending";
            ViewBag.TitleSort = string.IsNullOrEmpty(sortingOrder) ? "titleDescending" : "";
            ViewBag.DateSort = sortingOrder=="Date" ? "dateDescending" : "Date";


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;



            var listOfAnnouncements = from x in db.Announcements select x;


            if (!String.IsNullOrEmpty(searchString) && searchString.Trim() != "")
            {
                listOfAnnouncements = listOfAnnouncements.Where(x => x.ApplicationUser.Name.ToLower().Contains(searchString.ToLower())
                                       || x.ApplicationUser.Lastname.ToLower().Contains(searchString.ToLower())
                                       || x.Course.Name.ToLower().Contains(searchString.ToLower())
                                       || x.Title.ToLower().Contains(searchString.ToLower())
                                       || x.Content.ToLower().Contains(searchString.ToLower()));
            }

            if (User.IsInRole("Admin"))
            {
                switch (sortingOrder)
                {
                    case "nameDescending":
                        listOfAnnouncements = listOfAnnouncements.OrderByDescending(x => x.ApplicationUser.Name);
                        break;
                    case "nameAscending":
                        listOfAnnouncements = listOfAnnouncements.OrderBy(x => x.ApplicationUser.Name);
                        break;
                    case "courseDescending":
                        listOfAnnouncements = listOfAnnouncements.OrderByDescending(x => x.Course.Name);
                        break;
                    case "courseAscending":
                        listOfAnnouncements = listOfAnnouncements.OrderBy(x => x.Course.Name);
                        break;
                    case "titleDescending":
                        listOfAnnouncements = listOfAnnouncements.OrderByDescending(x => x.Title);
                        break;
                    case "dateDescending":
                        listOfAnnouncements = listOfAnnouncements.OrderByDescending(x => x.RecDate);
                        break;
                    case "Date":
                        listOfAnnouncements = listOfAnnouncements.OrderBy(x => x.RecDate);
                        break;
                    default:
                        listOfAnnouncements = listOfAnnouncements.OrderBy(x => x.Title);
                        break;
                }
               
           


                int pageSize = 10;
                int pageNumber = (page ?? 1);  //means that if page is null set pageNumber to 1
                return View(listOfAnnouncements.ToPagedList(pageNumber, pageSize));
               // return View(username.ToList());
            }

            //if user is teacher...
            switch (sortingOrder)
            {
                case "nameDescending":
                    listOfAnnouncements = listOfAnnouncements.OrderByDescending(x => x.ApplicationUser.Name);
                    break;
                case "nameAscending":
                    listOfAnnouncements = listOfAnnouncements.OrderBy(x => x.ApplicationUser.Name);
                    break;
                case "courseDescending":
                    listOfAnnouncements = listOfAnnouncements.OrderByDescending(x => x.Course.Name);
                    break;
                case "courseAscending":
                    listOfAnnouncements = listOfAnnouncements.OrderBy(x => x.Course.Name);
                    break;
                case "titleDescending":
                    listOfAnnouncements = listOfAnnouncements.OrderByDescending(x => x.Title);
                    break;
                case "dateDescending":
                    listOfAnnouncements = listOfAnnouncements.OrderByDescending(x => x.RecDate);
                    break;
                case "Date":
                    listOfAnnouncements = listOfAnnouncements.OrderBy(x => x.RecDate);
                    break;
                default:
                    listOfAnnouncements = listOfAnnouncements.OrderBy(x => x.Title);
                    break;
            }

            string userId = User.Identity.GetUserId();
            int pageSize2 = 10;
            int pageNumber2 = (page ?? 1);

            var announcements = listOfAnnouncements.Include(a => a.ApplicationUser).Include(a => a.Course);
            var result = announcements.Where(a => a.ApplicationUserId == userId).ToList();

            return View(result.ToPagedList(pageNumber2,pageSize2));

        }

        // GET: Announcements/Details/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcement announcement = db.Announcements.Find(id);
            if (announcement == null)
            {
                return HttpNotFound();
            }
            return View(announcement);
        }

        // GET: Announcements/Create
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

        // POST: Announcements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Create([Bind(Include = "Id,Title,Content,CourseId,VisibleToMentor")] Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                announcement.Type = "Announcement";
                announcement.ApplicationUserId = User.Identity.GetUserId();
                announcement.RecDate = DateTime.UtcNow.AddHours(2);
                db.Announcements.Add(announcement);
                db.SaveChanges();

              
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Name", announcement.ApplicationUserId);
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", announcement.CourseId);
            return View(announcement);
        }

        // GET: Announcements/Edit/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcement announcement = db.Announcements.Find(id);
            if (announcement == null)
            {
                return HttpNotFound();
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Name", announcement.ApplicationUserId);
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", announcement.CourseId);
            return View(announcement);
        }

        // POST: Announcements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Edit([Bind(Include = "Id,Title,Content,CourseId,VisibleToMentor")] Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                announcement.ApplicationUserId = User.Identity.GetUserId();
                announcement.Type = "Announcement";
                announcement.RecDate = DateTime.UtcNow.AddHours(2);
                db.Entry(announcement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ApplicationUserId = new SelectList(db.Users, "Id", "Name", announcement.ApplicationUserId);
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", announcement.CourseId);
            return View(announcement);
        }

        // GET: Announcements/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcement announcement = db.Announcements.Find(id);
            if (announcement == null)
            {
                return HttpNotFound();
            }
            return View(announcement);
        }

        // POST: Announcements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult DeleteConfirmed(int id)
        {
            Announcement announcement = db.Announcements.Find(id);
            db.Announcements.Remove(announcement);
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
