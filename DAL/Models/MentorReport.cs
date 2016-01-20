using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitClassroom.DAL.Models
{
 
    public class MentorReport
    { 
        public int Id { get; set; }
        [ForeignKey ("Mentor")]
        public string MentorId { get; set; }
        public virtual ApplicationUser Mentor { get; set; }
        
        [ForeignKey("Student")]
        public string StudentId { get; set; }        
        public virtual ApplicationUser Student { get; set; }
        [ForeignKey("Surveys")]
        public int SurveyId { get; set; }
     
        public virtual List<Survey> Surveys { get; set; }

        public virtual Survey Survey { get; set; }
        [NotMapped]
        public List<ApplicationUser> Students { get; set; }
      
        public List<QuestionResponse> Responses { get; set; }

        public DateTime DateSubmitted { get; set; }

    }
}
