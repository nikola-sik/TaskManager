using System;
using System.Collections.Generic;
using System.Linq;
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

        public void CreateTask(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            _context.Tasks.Add(task);
        }

        public void CreateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (_context.Users.FirstOrDefault(p => p.email == user.email) != null)
            {
                throw new Exception("Email alredy exists!");
            }

            _context.Users.Add(user);
        }

        public void DeleteTask(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            task.deleted = true;

        }

        public Task GetTaskById(int id)
        {
            return _context.Tasks.FirstOrDefault(p => p.id == id && p.deleted == false);
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(p => p.email.Equals(email));
        }

        public IEnumerable<Task> GetUserTasks(int userId)
        {
            return _context.Tasks.Where(t => t.userId == userId && t.deleted == false).ToList();
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateTask(Task task)
        {
            //don't need to define or implement Update method 
            //because AutoMapper and Entity Framework update changes after calling SaveChanges()
        }

        public bool ValidateToken(string tokenString)
        {
            return _context.InvalidTokens.FirstOrDefault(p => p.token.Equals(tokenString)) == null;
        }

        public void CreateInvalidToken(InvalidToken invalidToken)
        {
            if (invalidToken == null)
            {
                throw new ArgumentNullException(nameof(invalidToken));
            }
            _context.InvalidTokens.Add(invalidToken);
        }

        public bool IsAdvancedUser(int id)
        {
            return _context.Users.FirstOrDefault(p => p.id == id).isAdvancedUser;
        }
    }
}