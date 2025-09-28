using WebApi.Models;

namespace WebApi.Services
{
    public interface IRoleService
    {
        Role CreateRole(string roleName);
        List<Role> GetRoles();
    }
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _db;
        Role IRoleService.CreateRole(string roleName)
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

        List<Role> IRoleService.GetRoles() => _db.Roles.ToList();
    }
}
