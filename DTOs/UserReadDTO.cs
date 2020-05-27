using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Models
{
    public class UserReadDTO{
      
        public int id { get; set; }
        
        public string name { get; set; }

        public string surname { get; set; }

         public string email { get; set; }
       
        public bool isAdvancedUser { get; set; }


    }
}