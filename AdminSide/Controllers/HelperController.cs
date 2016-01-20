using BitClassroom.DAL.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using BitClassroom.MVC.Models.DTO;



namespace BitClassroom.MVC.Controllers
{
    public class HelperController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private string[] _allowedTypes = new string[] { "image/jpeg", "image/jpg", "image/png" };



        [Authorize(Roles = "Student,Admin,Mentor")]
        public string CloudUploadCreate(Course course)
        {
            if (HandleFileUpload(ref course))
            {
                course.PictureUrl = CloudinaryUpload(course);
                return course.PictureUrl;
            }

            course.PictureUrl = "http://res.cloudinary.com/gigantor/image/upload/v1441017337/wwgb50xuqulq7utflsjo.gif";
            return course.PictureUrl;
        }


        public string CloudUploadEdit(Course course)
        {
            if (HandleFileUpload(ref course))
            {
                var courseCurrentPicture = db.Courses.Select(x => x.PictureUrl);
                var array = courseCurrentPicture.ToArray();
                var lastElement = array[array.Length - 1];
                Account acount = new Account("gigantor", "986286566519458", "GT87e1BTMnfLut1_gXhSH0giZPg");
                Cloudinary cloudinary = new Cloudinary(acount);

                if (lastElement != null && course.PictureUrl.StartsWith("http://res.cloudinary.com/gigantor/image/upload/"))  //this block of code deletes the previous image if the user had one
                {
                    //this here is just a string manipulation to get to the ImageID from cloudinary
                    string assist = "http://res.cloudinary.com/gigantor/image/upload/";
                    string part1 = lastElement.Remove(lastElement.IndexOf(assist), assist.Length);
                    string part2 = part1.Remove(0, 12);
                    string toDelete = part2.Remove(part2.Length - 4);
                    cloudinary.DeleteResources(toDelete);  //this finally deletes the image
                }
                   
                course.PictureUrl = CloudinaryUpload(course);
            
                return course.PictureUrl;
            }

            course.PictureUrl = "http://res.cloudinary.com/gigantor/image/upload/v1441017337/wwgb50xuqulq7utflsjo.gif";
            return course.PictureUrl;
        }



        /// <summary>
        /// Method for uploading image from local host to cloudinary
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Cloudinary link(string) of uploaded picture</returns>
        private string CloudinaryUpload(Course course)
        {
            var cloudPath = System.Web.Hosting.HostingEnvironment.MapPath(course.PictureUrl);
            Account acount = new Account("gigantor", "986286566519458", "GT87e1BTMnfLut1_gXhSH0giZPg");
            Cloudinary cloudinary = new Cloudinary(acount);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(cloudPath)
            };
            var uploadResult = cloudinary.Upload(uploadParams);

            string n = uploadResult.Uri.AbsoluteUri;
            return course.PictureUrl = n;

        }

        /// <summary>
        /// Method for uploading images from local host (and securing that only images can be uploaded)
        /// </summary>
        /// <param name="user"></param>
        /// <returns>True or false, depending if the file has right extensions (in this case image extensions)</returns>
        
        private bool HandleFileUpload(ref Course course)
        {
                                 
           
       
            string filePath = @"~/Content/2.png";

            if (System.Web.HttpContext.Current.Request.Files.Count > 0)
            {
                var file = System.Web.HttpContext.Current.Request.Files[0];
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
                    string fullPath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/"), fileName);
                    file.SaveAs(fullPath);
                    course.PictureUrl = filePath;

                }

                else
                {
                    if (file.ContentLength > 0 && !_allowedTypes.Contains(file.ContentType))
                    {
                        ModelState.AddModelError("ImageUrl", "File type not supported");
                        return false;
                    }
                }

                if (course.PictureUrl == null)
                {

               
                    course.PictureUrl = @"~/Content/2.png";
                }

            }
            return true;
        }


        
        /// <summary>
        /// Used to obtain complete information on surveys, including list of accompanying question. Needed to Mentor Reports (Templates only, no answers)
        /// </summary>
        /// <returns>JSON object</returns>
        public ActionResult GetSurveys()
        {
            List<Survey> activeSurveys = db.Surveys.Where(s => s.IsActive == true).ToList();

            List<SurveyTemplateDTO> newList = new List<SurveyTemplateDTO>();

            foreach (var survey in activeSurveys)
            {
                SurveyTemplateDTO template = new SurveyTemplateDTO();

                template.SurveyTemplateId = survey.Id;
                template.Title = survey.Title;
                template.IsCurrent = survey.IsActive;
                template.DateCreated = survey.DateCreated;

                List<SurveyQuestion> QList = db.SurveyQuestions.Where(sq => sq.SurveyId == survey.Id).ToList();
                template.Questions = QList;
                
                newList.Add(template);

            }
            return Json(newList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// gets a list of questions for a specific survey
        /// </summary>
        /// <param name="surveyId">survey for which we are obtaining the questions</param>
        /// <returns>json object</returns>
        public ActionResult ListSurveyQuestions(int surveyId)
        {
            List<SurveyQuestion> surveyQuestions = db.SurveyQuestions.Where(sq => sq.SurveyId == surveyId).ToList();
            return Json(surveyQuestions, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetStudentUploads(int assignmentId, string studentId)
        {
            List<StudentUpload> results = db.StudentUploads.Where(x => x.AssignmentId == assignmentId && x.StudentId == studentId).ToList();

            return PartialView("_StudentUploads", results);
        }
    }
}

