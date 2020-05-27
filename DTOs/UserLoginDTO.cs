using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Models
{
    public class UserLoginDTO{
      
     
      
        [Required]
        [EmailAddress] 
        [MaxLength(100)]    
        public string email { get; set; }
        
        [Required]
        [MinLength(8)]
        [MaxLength(512)]  
        public string password { get; set; }


    }
}