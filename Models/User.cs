using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Models
{
    public class User{

        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(100)]
        public string name { get; set; }
        [Required]
        [MaxLength(100)]
        public string surname { get; set; }
       
        [MaxLength(30)]
        public string phoneNumber { get; set; }   
        [Required]
        [EmailAddress] 
        [MaxLength(100)]    
        public string email { get; set; } // email is set to unique in class TaskManagerContext
        [Required]
        [MinLength(8)]
        [MaxLength(512)]  
        public string password { get; set; }
       
        [DefaultValue(false)]
        // isAdvancedUser is set to default value false using OnModelCreating method   
        public bool isAdvancedUser { get; set; }


    }
}