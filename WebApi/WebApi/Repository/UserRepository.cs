using Microsoft.AspNetCore.Identity;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Repository
{
    public interface IUserRepository 
    {
        User AddUser(UserDTO userDTO);
    }
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _con;
        private readonly IRoleRepository _roleRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserRepository(AppDbContext con, IRoleRepository roleRepository)
        {
            _con = con;
            _roleRepository = roleRepository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public User AddUser(UserDTO userDTO)
        {
            var role = _roleRepository.GetRole("Viewer");
            var usr = new User()
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                Role = role,
                VerifyToken = TokenGenerator.GenerateRandomizeToken(),
            };
            usr.PasswordHash = _passwordHasher.HashPassword(usr, userDTO.Password);
            _con.Users.Add(usr);
            _con.SaveChanges();
            return usr;
        }
    }
}
