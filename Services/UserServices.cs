using LoginAndAuth.Data;
using LoginAndAuth.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace LoginAndAuth.Services
{
    public interface IUserService
    {
        public Task<UserDTO> GetUser(string userName);
        public Task<User> CreateUser(UserDTO userDTO);
    }
    public class UserServices: IUserService
    {
        private readonly LoginContext _dbContext;
        public UserServices(LoginContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserDTO> GetUser(string userName)
        {
            try
            {
                var userToGet = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == userName);
                if (userToGet != null)
                {
                    return new UserDTO { Name = userToGet.Name, Email = userToGet.Email };
                }
                else return null!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
                return null!;
            }
        }

        public async Task<User> CreateUser (UserDTO userDTO)
        {
            try
            {
                var checkUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == userDTO.Name);
                if (checkUser == null)
                {
                    var userToCreate = await _dbContext.Users.AddAsync(new User { Name = userDTO.Name, Email = userDTO.Email });
                    await _dbContext.SaveChangesAsync();
                    return userToCreate.Entity;
                }
                else
                {
                    return checkUser;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
                return null!;
            }
        }
    }
}
