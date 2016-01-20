using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitClassroom.DAL.Models
{
    public class QuestionResponse
    {
        public int Id { get; set; }

        public string MentorId { get; set; }
        public string StudentId { get; set; }
        [Display(Name = "Report Title")]
        public int SurveyId { get; set; }     

        [DataType(DataType.MultilineText)]
        public string Answer { get; set; }    
        
        [Display(Name="Question")]
        public int QuestionId { get; set; }
       
       public virtual SurveyQuestion SurveyQuestion { get; set; }

    }
}
