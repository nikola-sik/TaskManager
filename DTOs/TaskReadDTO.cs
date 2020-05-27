using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.DTOs
{
    public class TaskReadDTO
    {
        public int id { get; set; }

        public string title { get; set; }

        public string status { get; set; }

        public string priority { get; set; } 
    
        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        public string text { get; set; }

    }
}