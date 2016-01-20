using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitClassroom.DAL.Models
{
    public class Course
    {
        public int Id { get; set; }
        [Display(Name="Course Name")]
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        public string Calendar { get; set; }
    }
}
