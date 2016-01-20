using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitClassroom.DAL.Models
{
    public class Survey
    {
        public int Id { get; set; }
        [Display(Name="Report Title")]
        [Required]
        public string Title { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [ReportDateValidator]
        [Required]
        [Display(Name="Date Created")]
        public DateTime DateCreated { get; set; }
        public List<QuestionResponse> Responses { get; set; }
        [Display(Name="Current")]
        public bool IsActive { get; set; }
    }
}
