using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class UserTask{
        
        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(50)]
        public string title { get; set; }

        [Required]
        [MaxLength(40)]
        public string status { get; set; }

        [Required] 
        [MaxLength(40)]
        public string priority { get; set; } 

        [Required] 
        [Column(TypeName="date")]     
        public DateTime? startDate { get; set; }

        [Required] 
        [Column(TypeName="date")]
        public DateTime? endDate { get; set; }
        
        [MaxLength(512)]
        public string text { get; set; }

        [Required]
        public int userId { get; set; }

        [Required]
        [ForeignKey("userId")]
        public User User { get; set; }

        [DefaultValue(false)]
        public bool deleted { get; set; }
    }
}