using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitClassroom.DAL.Models
{
    public class Assignment : Post
    {

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [DateValidator]
        public DateTime DueDate { get; set; }

        [NotMapped]
        public TeacherUpload TeacherUpload { get; set; }
        public virtual List<TeacherUpload> TeacherUploads { get; set; }

        [NotMapped]
        public StudentUpload StudentUpload { get; set; }
        public virtual List<StudentUpload> StudentUploads { get; set; }

        //[NotMapped]
        //public StudentUpload MentorUpload { get; set; }
        //public virtual List<StudentUpload> MentorUploads { get; set; }
    }
}
