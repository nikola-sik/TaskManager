using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Data
{
    public interface ITaskManagerRepository
    {
        Task<bool> SaveChanges();

        Task<IEnumerable<UserTask>> GetUserTasks(int userId);

        Task<UserTask> GetTaskById (int id);
        
        Task<UserTask> CreateTask(UserTask userTask);

        Task<UserTask> UpdateTask(UserTask userTask);

        Task<bool> DeleteTask(UserTask userTask);

        Task<User> CreateUser(User user);

        Task<User> GetUserByEmail (string email);

        Task<bool> ValidateToken (string tokenString);

        Task<InvalidToken> CreateInvalidToken (InvalidToken invalidToken);

        Task<bool> IsAdvancedUser(int id);
    }
}