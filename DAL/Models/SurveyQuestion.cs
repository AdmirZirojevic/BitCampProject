 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitClassroom.DAL.Models
{
    public class SurveyQuestion
    {
        public int Id {get; set; }
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
        [ForeignKey("Survey")]
        public int SurveyId { get; set; }
        public virtual Survey Survey { get; set; }
        

    }
}
