using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class MySQLTaskManagerRepository : ITaskManagerRepository
    {
        private readonly TaskManagerContext _context;

        public MySQLTaskManagerRepository(TaskManagerContext context)
        {
            _context = context;
        }

        public async Task<UserTask> CreateTask(UserTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            await _context.UserTasks.AddAsync(task);
            return task;
        }

        public async Task<User> CreateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (_context.Users.FirstOrDefault(p => p.email == user.email) != null)
            {
                throw new Exception("Email alredy exists!");
            }

            await _context.Users.AddAsync(user);
            return user;
        }

        public async Task<bool> DeleteTask(UserTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
             task.deleted = true;
             _context.UserTasks.Update(task);
             await _context.SaveChangesAsync();
             return true;
        }

        public async Task<UserTask> GetTaskById(int id)
        {
            return await _context.UserTasks.FirstOrDefaultAsync(p => p.id == id && p.deleted == false);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(p => p.email.Equals(email));
        }

        public async Task<IEnumerable<UserTask>> GetUserTasks(int userId)
        {
            return await _context.UserTasks.Where(t => t.userId == userId && t.deleted == false).ToListAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

       public async Task<UserTask> UpdateTask(UserTask task)
        {
            _context.UserTasks.Update(task);
            await _context.SaveChangesAsync();
            return  task;
        }
 
        public async Task<bool> ValidateToken(string tokenString)
        {
             var token = await _context.InvalidTokens.FirstOrDefaultAsync(p => p.token.Equals(tokenString)) ;
             return token == null;
        }

        public async Task<InvalidToken> CreateInvalidToken(InvalidToken invalidToken)
        {
            if (invalidToken == null)
            {
                throw new ArgumentNullException(nameof(invalidToken));
            }
            await _context.InvalidTokens.AddAsync(invalidToken);
            return invalidToken;
        }

        public async Task<bool> IsAdvancedUser(int id)
        {
             User user = await _context.Users.FirstOrDefaultAsync(p => p.id == id);
             return user.isAdvancedUser;
        }



       
    }
}