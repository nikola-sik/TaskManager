using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Models;

namespace TaskManager.DTOs
{
    public class UserTaskUpdateDTO
    {

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

    }
}