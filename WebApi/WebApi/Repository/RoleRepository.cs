using WebApi.Models;
using WebApi.Services;

namespace WebApi.Repository
{
    public interface IRoleRepository
    {
        Role CreateRole(string roleName);
        List<Role> GetRoles();
        Role GetRole(string roleName);
    }
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _db;

        public RoleRepository(AppDbContext db)
        {
            _db = db;
        }

        Role IRoleRepository.CreateRole(string roleName)
        {
            Console.WriteLine("where it printed");
            Console.WriteLine(roleName);
            var role = new Role()
            {
                RoleName = roleName
            };
            _db.Roles.Add(role);
            _db.SaveChanges();
            return role;
        }
        Role IRoleRepository.GetRole(string roleName)
        {
            return _db.Roles.FirstOrDefault(r => r.RoleName == roleName);
        }

        List<Role> IRoleRepository.GetRoles() => _db.Roles.ToList();
    }
}