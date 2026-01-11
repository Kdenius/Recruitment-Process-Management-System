using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserController(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }
        
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAll();
            if (users == null)
            {
                return NotFound(new {message = "Active user does not exist"});
            }
            return Ok(users);
        }
        [Authorize(Roles = "Admin")]
        
        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleUpdateDTO ru)
        {
            var valid = _userRepository.UpdateUserRole(ru.userId, ru.roleId);
            if (!valid)
                return BadRequest(new { message = "Role change Failed" });
            return Ok(new { message= "User Roles changed" });
        }

        [HttpGet("interviewers")]
        public async Task<IActionResult> GetInterviewers()
        {
            var interviewers = await _userRepository.GetInterviewers();

            if (interviewers == null || !interviewers.Any())
            {
                return NotFound(new { message = "No interviewers found" });
            }

            return Ok(interviewers);
        }
    }
}
