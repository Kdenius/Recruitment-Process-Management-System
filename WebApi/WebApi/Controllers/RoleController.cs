using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Repository;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleService)
        {
            _roleRepository = roleService;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Role>> GetAllRoles()
        {
            return _roleRepository.GetRoles();
        }
        [HttpPost]
        public ActionResult<Role> AddRole(Role role)
        {
           return _roleRepository.CreateRole(role.RoleName);
        }
    }
}
