using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagmentSystem.Data;
using TaskManagmentSystem.Models;

namespace TaskManagmentSystem.Repositories
{
    public class UserRepository
    {
        private readonly TaskManagmentDBContext _dbContext;
        public UserRepository(TaskManagmentDBContext dbContext)
        {
            _dbContext = dbContext;    
        }

        public User GetUserByUserId(int userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId);

            if(user is null)
            {
                return null!;
            }
            return user;
        }

        public User GetUserByUserName(string userName)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Username.ToLower() == userName.ToLower());

            if (user is null)
            {
                return null!;
            }
            return user;
        }

        public async Task<List<User>> GetAllAdmins()
        {
            return await _dbContext.Users.Where(x => x.UserRole.ToLower() == "companyadmin").ToListAsync();
        }

        public async Task<bool> AddUser(User user)
        {
            try
            {
                await _dbContext.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<User>> GetAllUsers()
        {
            var userList = await _dbContext.Users.ToListAsync();
            return userList;
        }
    }
}
