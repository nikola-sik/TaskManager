using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class InvalidToken{
        
        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(512)]
        public string token { get; set; }

        [Required]     
        public DateTime? expirationDate { get; set; }

        [Required]
        public int userId { get; set; }

        [Required]
        [ForeignKey("userId")]
        public User User { get; set; }
    }
}