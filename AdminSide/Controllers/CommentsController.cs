using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using BitClassroom.DAL.Models;

namespace BitClassroom.MVC.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Index(string sortingOrder, string currentFilter, string searchString, int? page)
        {         
           

            ViewBag.CurrentSort = sortingOrder;

            ViewBag.ContentSort = sortingOrder == "contentAscending" ? "contentDescending" : "contentAscending";
            ViewBag.UsernameSort = sortingOrder == "usernameAscending" ? "usernameDescending" : "usernameAscending";
            ViewBag.AssignmentSort = sortingOrder == "assignmentAscending" ? "assignmentDescending" : "assignmentAscending";
            ViewBag.AnnouncementSort = sortingOrder == "announcementAscending" ? "announcementDescending" : "announcementAscending";
            ViewBag.DateSort = sortingOrder == "dateAscending" ? "dateDescending" : "dateAscending";


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;



            var listOfComments = from x in db.Comments select x;


            if (!String.IsNullOrEmpty(searchString) && searchString.Trim() != "")
            {
                listOfComments = listOfComments.Where(x => x.Content.ToLower().Contains(searchString.ToLower())
                                       || x.Username.ToLower().Contains(searchString.ToLower())
                                       || x.AssignmentTitle.ToLower().Contains(searchString.ToLower())
                                       || x.AnnouncementTitle.ToLower().Contains(searchString.ToLower()));
                                    
            }

            if (User.IsInRole("Admin"))
            {
                switch (sortingOrder)
                {
                    case "contentDescending":
                        listOfComments = listOfComments.OrderByDescending(x => x.Content);
                        break;
                    case "contentAscending":
                        listOfComments = listOfComments.OrderBy(x => x.Content);
                        break;
                    case "usernameDescending":
                        listOfComments = listOfComments.OrderByDescending(x => x.Username);
                        break;
                    case "usernameAscending":
                        listOfComments = listOfComments.OrderBy(x => x.Username);
                        break;
                    case "assignmentDescending":
                        listOfComments = listOfComments.OrderByDescending(x => x.AssignmentTitle);
                        break;
                    case "assignmentAscending":
                        listOfComments = listOfComments.OrderBy(x => x.AssignmentTitle);
                        break;
                    case "announcementDescending":
                        listOfComments = listOfComments.OrderByDescending(x => x.AnnouncementTitle);
                        break;
                    case "announcementAscending":
                        listOfComments = listOfComments.OrderBy(x => x.AnnouncementTitle);
                        break;
                    case "dateDescending":
                        listOfComments = listOfComments.OrderByDescending(x => x.DateCreated);
                        break;
                    case "dateAscending":
                        listOfComments = listOfComments.OrderBy(x => x.DateCreated);
                        break;
                    default:
                        listOfComments = listOfComments.OrderByDescending(x => x.DateCreated);
                        break;
                }


                int pageSize = 20;
                int pageNumber = (page ?? 1);  //means that if page is null set pageNumber to 1
                return View(listOfComments.ToPagedList(pageNumber, pageSize));
                // return View(username.ToList());
            }

            //if user is teacher...
            switch (sortingOrder)
            {
                case "contentDescending":
                    listOfComments = listOfComments.OrderByDescending(x => x.Content);
                    break;
                case "contentAscending":
                    listOfComments = listOfComments.OrderBy(x => x.Content);
                    break;
                case "usernameDescending":
                    listOfComments = listOfComments.OrderByDescending(x => x.Username);
                    break;
                case "usernameAscending":
                    listOfComments = listOfComments.OrderBy(x => x.Username);
                    break;
                case "assignmentDescending":
                    listOfComments = listOfComments.OrderByDescending(x => x.AssignmentTitle);
                    break;
                case "assignmentAscending":
                    listOfComments = listOfComments.OrderBy(x => x.AssignmentTitle);
                    break;
                case "announcementDescending":
                    listOfComments = listOfComments.OrderByDescending(x => x.AnnouncementTitle);
                    break;
                case "announcementAscending":
                    listOfComments = listOfComments.OrderBy(x => x.AnnouncementTitle);
                    break;
                case "dateDescending":
                    listOfComments = listOfComments.OrderByDescending(x => x.DateCreated);
                    break;
                case "dateAscending":
                    listOfComments = listOfComments.OrderBy(x => x.DateCreated);
                    break;
                default:
                    listOfComments = listOfComments.OrderByDescending(x => x.DateCreated);
                    break;
            }

            string userId = User.Identity.GetUserId();
            int pageSize2 = 20;
            int pageNumber2 = (page ?? 1);        

            var result = listOfComments.Where(x => x.UserId == userId).ToList();

            return View(result.ToPagedList(pageNumber2, pageSize2));

        }


        // GET: Comments/Details/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            string userId = this.User.Identity.GetUserId();

            if (User.IsInRole("Teacher"))
            {
                if (comment.UserId != userId)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(comment);
        }

        // GET: Comments/Create
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Create()
        {
            string userId = this.User.Identity.GetUserId();

            if(User.IsInRole("Teacher"))
            {
                ViewBag.AssignId = new SelectList(db.Assignments.Where(x=>x.ApplicationUserId==userId).Include("Course").OrderBy(x => x.Course.Name).ThenBy(z => z.Title), "Id", "Course_Assignment").ToList();
                ViewBag.AnnounceId = new SelectList(db.Announcements.Where(x => x.ApplicationUserId == userId).Include("Course").OrderBy(x => x.Course.Name).ThenBy(z => z.Title), "Id", "Course_Assignment").ToList();
            }
            else
            {
                ViewBag.AssignId = new SelectList(db.Assignments.Include("Course").OrderBy(x => x.Course.Name).ThenBy(z => z.Title), "Id", "Course_Assignment").ToList();
                ViewBag.AnnounceId = new SelectList(db.Announcements.Include("Course").OrderBy(x => x.Course.Name).ThenBy(z => z.Title), "Id", "Course_Assignment").ToList();
            }         
            return View();
        }

        // POST: Comments/Create
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Create([Bind(Include = "Id,Content,Username,AssignId,AnnounceId,UserId,UserEmail,DateCreated")] Comment comment)
        {
            
            if (ModelState.IsValid)
            {
                string userId = this.User.Identity.GetUserId();
                ApplicationUser user = db.Users.Find(userId);

                if(comment.AnnounceId==0&&comment.AssignId==0)
                    return View("Error2");

                if (comment.AnnounceId != 0 && comment.AssignId != 0)
                    return View("Error2");

                if (comment.AssignId != 0)
                {              

                    var assignment = db.Assignments.Where(x => x.Id == comment.AssignId).First();
                    comment.AssignmentTitle = assignment.Course_Assignment;                  
                }

                if (comment.AnnounceId != 0)
                {            

                    var announcement = db.Announcements.Where(x => x.Id == comment.AnnounceId).First();
                    comment.AnnouncementTitle = announcement.Course_Assignment;                  
                }
               


                comment.UserId = userId;
                comment.Username = user.FullName;
                comment.UserEmail = user.Email;
                comment.DateCreated = DateTime.UtcNow.AddHours(2);
                

                db.Comments.Add(comment);
                db.SaveChanges();

                if(comment.AssignId!=0)
                return RedirectToAction("GetAssignmentComment"+"/"+comment.AssignId,"WatchListItems",comment.AssignId);
                else
                return RedirectToAction("GetAnnouncementsComment" + "/" + comment.AnnounceId, "WatchListItems", comment.AnnounceId);
              
            }

            ViewBag.AssignId = new SelectList(db.Assignments.Include("Course").OrderBy(x => x.Course.Name).ThenBy(z => z.Title), "Id", "Course_Assignment",comment.AssignId).ToList();
            ViewBag.AnnounceId = new SelectList(db.Announcements.Include("Course").OrderBy(x => x.Course.Name).ThenBy(z => z.Title), "Id", "Course_Assignment",comment.AnnounceId).ToList();
            if (comment.AssignId != 0)
                return RedirectToAction("GetAssignmentComment" + "/" + comment.AssignId, "WatchListItems", comment.AssignId);
            else
                return RedirectToAction("GetAnnouncementsComment" + "/" + comment.AnnounceId, "WatchListItems", comment.AnnounceId);
        }

        // GET: Comments/Edit/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            string userId = this.User.Identity.GetUserId();

            if (User.IsInRole("Teacher"))
            {
                if (comment.UserId != userId)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (User.IsInRole("Admin"))
            {
                if (comment.UserId != userId)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if(comment.AssignId!=0)
             ViewBag.AssignId = new SelectList(db.Assignments.Include("Course").OrderBy(x => x.Course.Name).ThenBy(z => z.Title), "Id", "Course_Assignment").ToList();
            
            if(comment.AnnounceId!=0)
            ViewBag.AnnounceId = new SelectList(db.Announcements.Include("Course").OrderBy(x => x.Course.Name).ThenBy(z => z.Title), "Id", "Course_Assignment").ToList();

            return View(comment);
        }

        // POST: Comments/Edit/5
        [Authorize(Roles = "Admin,Teacher")]
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Content,Username,AssignId,AnnounceId,UserId,UserEmail,DateCreated")] Comment comment)
        {
            if (ModelState.IsValid)
            {

             
                string userId = this.User.Identity.GetUserId();
                ApplicationUser user = db.Users.Find(userId);

                if (comment.AnnounceId == 0 && comment.AssignId == 0)
                    return View("Error2");

                if (comment.AssignId != 0)
                {

                    var assignment = db.Assignments.Where(x => x.Id == comment.AssignId).First();
                    comment.AssignmentTitle = assignment.Course_Assignment;
                }
                
                if (comment.AnnounceId != 0)
                {
                  

                    var announcement = db.Announcements.Where(x => x.Id == comment.AnnounceId).First();
                    comment.AnnouncementTitle = announcement.Course_Assignment;
                }

                comment.UserId = userId;
                comment.Username = user.FullName;
                comment.UserEmail = user.Email;
                comment.DateCreated = DateTime.UtcNow.AddHours(2);
                

                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();

                if (comment.AssignId != 0)
                    return RedirectToAction("GetAssignmentComment" + "/" + comment.AssignId, "WatchListItems", comment.AssignId);
                else
                    return RedirectToAction("GetAnnouncementsComment" + "/" + comment.AnnounceId, "WatchListItems", comment.AnnounceId);
            }

            if (comment.AssignId != 0)
                ViewBag.AssignId = new SelectList(db.Assignments.Include("Course").OrderBy(x => x.Course.Name).ThenBy(z => z.Title), "Id", "Course_Assignment",comment.AssignId).ToList();

            if (comment.AnnounceId != 0)
                ViewBag.AnnounceId = new SelectList(db.Announcements.Include("Course").OrderBy(x => x.Course.Name).ThenBy(z => z.Title), "Id", "Course_Assignment",comment.AnnounceId).ToList();

            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            string userId = this.User.Identity.GetUserId();

            if (User.IsInRole("Teacher"))
            {
                if (comment.UserId != userId)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            if (comment.AssignId != 0)
                return RedirectToAction("GetAssignmentComment" + "/" + comment.AssignId, "WatchListItems", comment.AssignId);
            else
                return RedirectToAction("GetAnnouncementsComment" + "/" + comment.AnnounceId, "WatchListItems", comment.AnnounceId);
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
