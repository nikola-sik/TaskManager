using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Models
{
    public class UserCreateDTO{
      
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
        public string email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(512)]  
        [PasswordPropertyText]
        public string password { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(512)]  
        [PasswordPropertyText]
        [Compare("password")]
        public string passwordConfirmation { get; set; }
        
        [DataType("bool")]
        public bool isAdvancedUser { get; set; }


    }
}