using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using BitClassroom.DAL.Models;
using BitClassroom.API.Providers;
using BitClassroom.API.Results;
using System.Security.Principal;
using BitClassroom.API.Models.DTO;

namespace BitClassroom.API.Controllers
{
    



    public class HomeController : Controller
    {
        private BitClassroom.DAL.Models.ApplicationDbContext db = new BitClassroom.DAL.Models.ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

      
        #region FindPost()
        /// <summary>
        /// Gets the specific assignment
        /// </summary>
        /// <param name="id">id of the assignment</param>
        /// <returns>Json of the assignment</returns>
        public ActionResult FindPost(int id)
        {
                Assignment assignment = db.Assignments.Find(id);
           
                Post post=new Post();
            
                try
                {      //try catch stoji cisto dok se ne napravi elegantnije rjesenje, jer ako neko proba u URL ubacit ID posta kojeg nema, Exception izbaci
                        post.Id =assignment.Id;
                                       
                        post.CourseId = assignment.CourseId;
                        post.Course = assignment.Course;
                        post.ApplicationUser = assignment.ApplicationUser;
                        post.Content = assignment.Content;
                        post.Title = assignment.Title;
                        post.Type = assignment.Type;
                        post.Due = assignment.DueDate;
                        post.RecDate = assignment.RecDate;

                List<Comment> comments = new List<Comment>();

                List<Assignment> assignments = db.Assignments.Where(x => x.Id == assignment.Id).ToList();
              
                var comm = db.Comments.Where(x => x.AssignId == assignment.Id).ToList();

                foreach (var item in comm)
                {
                    comments.Add(item);
                }

                post.Comments = comments;

               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
                      
           
            return Json(post, JsonRequestBehavior.AllowGet);
        }
        #endregion
        /// <summary>
        /// Gets the announcement
        /// </summary>
        /// <param name="id">id of announcement</param>
        /// <returns>Json of the announcement</returns>
        #region FindPost2()
        public ActionResult FindPost2(int id)
        {

            Announcement announcement = db.Announcements.Find(id);

            Post post = new Post();

            try
            {      //try catch stoji cisto dok se ne napravi elegantnije rjesenje, jer ako neko proba u URL ubacit ID posta kojeg nema, Exception izbaci



                      post.Id = announcement.Id;
                    post.CourseId = announcement.CourseId;
                    post.Course = announcement.Course;
                    post.ApplicationUser = announcement.ApplicationUser;
                    post.Content = announcement.Content;
                    post.Title = announcement.Title;
                    post.Type = announcement.Type;
                    post.RecDate = announcement.RecDate;
                
                List<Comment> comments = new List<Comment>();

             //   List<Assignment> assignments = db.Assignments.Where(x => x.Id == assignment.Id).ToList();
                List<Announcement> announcements = db.Announcements.Where(x => x.Id == announcement.Id).ToList();

                var comm = db.Comments.Where(x=>x.AnnounceId == announcement.Id).ToList();

                foreach (var item in comm)
                {
                    comments.Add(item);
                }

                post.Comments = comments;


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            return Json(post, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
