using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitClassroom.DAL.Models
{
   public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }      
       [Display(Name="Created By")] 
       public string Username { get; set; }
        public int AssignId { get; set; }
        public int AnnounceId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public DateTime DateCreated { get; set; }

       
        public string AssignmentTitle { get; set; }
        
        public string  AnnouncementTitle { get; set; }
    }
}
