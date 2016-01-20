using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using BitClassroom.DAL.Models;

namespace BitClassroom.API.Controllers
{
    public class CommentsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Comments
        public IQueryable<Comment> GetComments()
        {
            return db.Comments;
        }

        // GET: api/Comments/5
        [ResponseType(typeof(Comment))]
        public IHttpActionResult GetComment(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // PUT: api/Comments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutComment(int id, Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comment.Id)
            {
                return BadRequest();
            }

            string userId = this.User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(userId);

            if (comment.UserId != userId)
                return BadRequest();

            if (comment.AssignId != 0)
            {
                Assignment assignment = db.Assignments.Where(x => x.Id == comment.AssignId).First();
                comment.AssignmentTitle = assignment.Course.Name + " - " + assignment.Title;
            }

            if (comment.AnnounceId != 0)
            {
                Announcement announcement = db.Announcements.Where(x => x.Id == comment.AnnounceId).First();
                comment.AnnouncementTitle = announcement.Course.Name + " - " + announcement.Title;
            }

            comment.DateCreated = DateTime.UtcNow.AddHours(2);
            db.Entry(comment).State = EntityState.Modified;

            try
            {
               
                db.SaveChanges();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Comments
        [ResponseType(typeof(Comment))]
        public IHttpActionResult PostComment(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId=this.User.Identity.GetUserId();          
            ApplicationUser user = db.Users.Find(userId);
            comment.UserId = userId;
            comment.Username = user.FullName;
            comment.UserEmail = user.UserName;
            comment.DateCreated = DateTime.UtcNow.AddHours(2);

            if(comment.AssignId!=0)
            {
                Assignment assignment = db.Assignments.Where(x => x.Id == comment.AssignId).First();
                comment.AssignmentTitle = assignment.Course.Name + " - " + assignment.Title;
            }
           
            if(comment.AnnounceId!=0)
            {
                Announcement announcement = db.Announcements.Where(x => x.Id == comment.AnnounceId).First();
                comment.AnnouncementTitle = announcement.Course.Name + " - " + announcement.Title;
            }
            

            db.Comments.Add(comment);
            try  
            {     
                db.SaveChanges();
                return Ok(comment);
            }
           catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return Ok(comment);
        }

        // DELETE: api/Comments/5
        [ResponseType(typeof(Comment))]
        public IHttpActionResult DeleteComment(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            string userId = this.User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(userId);

            if (comment.UserId != userId)
                return BadRequest();


            db.Comments.Remove(comment);
            db.SaveChanges();

            return Ok(comment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CommentExists(int id)
        {
            return db.Comments.Count(e => e.Id == id) > 0;
        }


        
        [HttpPost]
        [ResponseType(typeof(Comment))]
        public IHttpActionResult EditComment(int id, Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comment.Id)
            {
                return BadRequest();
            }

            db.Entry(comment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}