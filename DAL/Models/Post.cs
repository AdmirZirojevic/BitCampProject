using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitClassroom.DAL.Models
{   
    public class Post
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime RecDate { get; set; }
        public string Type { get; set; }
        public Nullable<DateTime> Due { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public bool VisibleToMentor { get; set; }

        [NotMapped]
        public string Course_Assignment { get { return Course.Name + " - " +Title; } }
    }
}
