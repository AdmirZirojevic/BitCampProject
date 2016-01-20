using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitClassroom.DAL.Models
{
    public class Question
    {
        public int Id { get; set; }
        [Display(Name="Question")]
        [DataType(DataType.MultilineText)]
        public string QContent { get; set; }
    }
}
