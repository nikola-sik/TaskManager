using System;
using System.Collections.Generic;
using TaskManager.Models;

namespace TaskManager.Data
{
    public interface ITaskManagerRepository
    {
        bool SaveChanges();

        IEnumerable<Task> GetUserTasks(int userId);

        Task GetTaskById (int id);
        
        void CreateTask(Task task);

        void UpdateTask(Task task);

        void DeleteTask(Task task);

        void CreateUser(User user);

        User GetUserByEmail (string email);

        bool ValidateToken (string tokenString);

        void CreateInvalidToken (InvalidToken invalidToken);

        bool IsAdvancedUser(int id);
    }
}