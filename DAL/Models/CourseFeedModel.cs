using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitClassroom.DAL.Models
{
    [NotMapped]
    public class CourseFeedModel
    {
        public string UserName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        //public List<Announcement> Announcements { get; set; }
        //public List<Assignment> Assignments { get; set; }
        public List<Post> Posts { get; set; }

    }
}
