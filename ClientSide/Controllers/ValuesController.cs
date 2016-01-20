using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using BitClassroom.DAL.Models;

namespace BitClassroom.API.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        private BitClassroom.DAL.Models.ApplicationDbContext db = new BitClassroom.DAL.Models.ApplicationDbContext();
        [HttpGet]
       
        public CourseFeedModel CourseFeed(int id)
        {

            string userId = this.User.Identity.GetUserId();

            ApplicationUser user = db.Users.Find(userId);
            CourseFeedModel model = new CourseFeedModel();
            try
            {      //try catch stoji cisto dok se ne napravi elegantnije rjesenje, jer ako neko proba u URL ubacit ID kursa kojeg nema, Exception izbaci
                Course course = db.Courses.Find(id);

                model.UserName = user.Name;
                model.CourseId = id;
                model.CourseName = course.Name;

                List<Assignment> assignments = new List<Assignment>();
                List<Announcement> announcements = new List<Announcement>();
                if (User.IsInRole("Mentor"))
                {
                    assignments = db.Assignments.Where(x => x.CourseId == course.Id&&x.VisibleToMentor==true).ToList();
                    announcements = db.Announcements.Where(x => x.CourseId == course.Id&&x.VisibleToMentor==true).ToList();
                }
                else
                {
                    assignments = db.Assignments.Where(x => x.CourseId == course.Id).ToList();
                    announcements = db.Announcements.Where(x => x.CourseId == course.Id).ToList();
                }

                assignments.ForEach(x => {
                    x.Comments = db.Comments.Where(c => c.AssignId == x.Id).ToList();  
                });
                announcements.ForEach(x =>
                {
                    x.Comments = db.Comments.Where(c => c.AnnounceId == x.Id).ToList(); 
                });
                
                List<Post> posts = new List<Post>();

                foreach (var item in assignments)
                {
                    item.Due = item.DueDate;
                    posts.Add(item);
                }
                foreach (var item in announcements)
                {
                    posts.Add(item);
                }

                posts.OrderBy(x => x.RecDate).ToList();
                model.Posts = posts.OrderByDescending(x => x.RecDate).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return model;
        }
    }
}
