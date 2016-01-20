using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BitClassroom.DAL.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        [Display(Name="Last Name")]
        public string Lastname { get; set; }
        [NotMapped]
        [Display(Name="Full Name")]
        public string FullName { get { return Name + " " + Lastname; } }
        public string ProfilePicUrl { get; set; }

        [ForeignKey("MentorId")]
        public virtual ApplicationUser Mentor { get; set; }

        public virtual List<MentorReport> MentorReports { get; set; }

        public string MentorId { get; set; }
        [Display(Name="Skype Name")]
        public string SkypeName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.CoursesTeach> CoursesTeaches { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.CoursesTaken> CoursesTakens { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.Announcement> Announcements { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.Assignment> Assignments { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.StudentsAssignment> StudentsAssignments { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.QuestionResponse> QuestionResponses { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.Question> Questions { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.Survey> Surveys { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.SurveyQuestion> SurveyQuestions { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.IndividualAssignment> IndividualAssignments { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.Notification> Notifications { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.MentorReport> MentorReports { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.Comment> Comments { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.CourseRequest> CourseRequests { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.WatchListItem> WatchListItems { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.TeacherUpload> TeacherUploads { get; set; }
        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.StudentUpload> StudentUploads { get; set; }
        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.DailyQuestion> DailyQuestions { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.DailyReport> DailyReports { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.DailyQuestionResponse> DailyQuestionResponses { get; set; }

        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.MentorUpload> MentorUploads { get; set; }
        public System.Data.Entity.DbSet<BitClassroom.DAL.Models.IndividualAssignmentStudentUpload> IndividualAssignmentStudentUploads { get; set; }
    }
}