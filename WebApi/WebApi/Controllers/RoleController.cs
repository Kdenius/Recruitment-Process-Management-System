using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Role>> GetAllRoles()
        {
            return _roleService.GetRoles();
        }
        [HttpPost]
        public ActionResult<Role> AddRole([FromBody] string roleName)
        {
           return _roleService.CreateRole(roleName);
        }
    }
}
