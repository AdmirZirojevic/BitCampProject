using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitClassroom.DAL.Models;
using BitClassroom.API.Models;
using System.Drawing;
using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using System.Net;
using System.Web.Http.Description;
using BitClassroom.API.Models.DTO;


namespace BitClassroom.API.Controllers
{

    public class HelperController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private string[] _allowedTypes = new string[] { "image/jpeg", "image/jpg", "image/png" };

        #region CloudUpload()

        /// <summary>
        /// Final method for uploading image to cloudinary and binding the link(result) to users profile image
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Users profile image, which is a link to the cloudinary image</returns>
        [Authorize(Roles = "Student,Admin,Mentor")]
        public string CloudUpload(Models.RegisterBindingModel user)
        {
            if (HandleFileUpload(ref user))
            {
                Account acount = new Account("gigantor", "986286566519458", "GT87e1BTMnfLut1_gXhSH0giZPg");
                Cloudinary cloudinary = new Cloudinary(acount);

                string userId = this.User.Identity.GetUserId();
                ApplicationUser user1 = db.Users.Find(userId);
                if (user1.ProfilePicUrl != null && user1.ProfilePicUrl.StartsWith("http://res.cloudinary.com/gigantor/image/upload/"))  //this block of code deletes the previous image if the user had one
                {
                    //this here is just a string manipulation to get to the ImageID from cloudinary
                    string assist = "http://res.cloudinary.com/gigantor/image/upload/";
                    string part1 = user1.ProfilePicUrl.Remove(user1.ProfilePicUrl.IndexOf(assist), assist.Length);
                    string part2 = part1.Remove(0, 12);
                    string toDelete = part2.Remove(part2.Length - 4);
                    cloudinary.DeleteResources(toDelete);  //this finally deletes the image
                }

                user1.ProfilePicUrl = CloudinaryUpload(user);
                db.Entry(user1).State = EntityState.Modified;
                db.SaveChanges();
                return user1.ProfilePicUrl;
            }
            return user.ProfileUrl;
        }

        #endregion


        #region CloudinaryUpload()

        /// <summary>
        /// Method for uploading image from local host to cloudinary
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Cloudinary link(string) of uploaded picture</returns>
        private string CloudinaryUpload(Models.RegisterBindingModel user)
        {
            var cloudPath = Server.MapPath(user.ProfileUrl);
            Account acount = new Account("gigantor", "986286566519458", "GT87e1BTMnfLut1_gXhSH0giZPg");
            Cloudinary cloudinary = new Cloudinary(acount);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(cloudPath)
            };
            var uploadResult = cloudinary.Upload(uploadParams);

            string n = uploadResult.Uri.AbsoluteUri;
            return user.ProfileUrl = n;
        }

        #endregion


        #region HandleFileUpload()

        /// <summary>
        /// Method for uploading images from local host (and securing that only images can be uploaded)
        /// </summary>
        /// <param name="user"></param>
        /// <returns>True or false, depending if the file has right extensions (in this case image extensions)</returns>
        private bool HandleFileUpload(ref Models.RegisterBindingModel user)
        {

            string filePath = @"~/Content/2.png";

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file.ContentLength > 0 && _allowedTypes.Contains(file.ContentType))
                {

                    try
                    {
                        using (var bitmap = new Bitmap(file.InputStream))
                        {
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("ImageUrl", "File type not supported");

                        return false;
                    }

                    string fileName = Path.GetFileName(file.FileName);
                    filePath = Path.Combine("~/Content", fileName);
                    string fullPath = Path.Combine(Server.MapPath(@"~/Content/"), fileName);
                    try
                    {
                        file.SaveAs(fullPath);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                    user.ProfileUrl = filePath;

                }

                else
                {
                    if (file.ContentLength > 0 && !_allowedTypes.Contains(file.ContentType))
                    {
                        ModelState.AddModelError("ImageUrl", "File type not supported");
                        return false;
                    }
                }

                if (user.ProfileUrl == null)
                {
                    user.ProfileUrl = @"~/Content/2.png";
                }

            }

            return true;

        }

        #endregion


        #region FillMentorReport()

        /// <summary>
        /// updating mentor reports for individual students
        /// </summary>
        /// <param name="MentorReportDTO">data selected / entered by the mentor filling a report</param>
        /// <returns></returns>
        //POST: api/Helper

        [ResponseType(typeof(MentorReport))]
        public void FillMentorReport(MentorReportDTO MentorReportDTO)
        {
            string userId = this.User.Identity.GetUserId();


            MentorReport mReport = new MentorReport();

            mReport.MentorId = User.Identity.GetUserId();
            mReport.StudentId = MentorReportDTO.StudentId;
            mReport.SurveyId = MentorReportDTO.SurveyId;
            mReport.Responses = db.QuestionResponses.Where(r => r.SurveyId == mReport.SurveyId).Include("Answer").ToList(); ;


            db.MentorReports.Add(mReport);

            db.SaveChanges();



        }

        #endregion

        #region GetStudents()

        public ActionResult GetStudents()
        {
            string userId = this.User.Identity.GetUserId();
            var role = db.Roles.Where(r => r.Name.Contains("Student")).FirstOrDefault();
            List<ApplicationUser> students = db.Users.Where(s => s.Roles.Select(x => x.RoleId).Contains(role.Id)).Where(s => s.MentorId == userId).ToList();

            return Json(students, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GetActiveSurveys()
        public ActionResult GetActiveSurveys()
        {
            List<Survey> surveys = db.Surveys.Where(x => x.IsActive == true).ToList();
            return Json(surveys, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region ForSurveyResponses()
        public ActionResult ForSurveyResponses(int surveyId)
        {
            return this.Json(
          new
          {
              Result = (from obj in db.SurveyQuestions.Where(r => r.SurveyId == surveyId) select new { Id = obj.Id, Name = obj.Question.QContent, QuestionId = obj.QuestionId, SurveyId = obj.SurveyId })
          }
          , JsonRequestBehavior.AllowGet
          );
        }

        #endregion

        #region ForMentorResponses()
        public ActionResult ForMentorResponses(int surveyId)
        {
            string userId = this.User.Identity.GetUserId();

            List<QuestionResponse> result = db.QuestionResponses.Where(x => x.SurveyId == surveyId && x.MentorId == userId).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region StudentSurveyCombo()
        public ActionResult StudentSurveyCombo(string studentId, int surveyId)
        {
            var students = db.MentorReports.Where(x => x.StudentId == studentId && x.SurveyId == surveyId).FirstOrDefault();
            return Json(students, JsonRequestBehavior.AllowGet);

        }

        #endregion





    }
}