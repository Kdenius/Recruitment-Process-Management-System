using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Repository;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    //[EnableCors("AllowAll")]
    public class AuthController : ControllerBase
    {
        /*private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public void Login([FromBody] string userMail, [FromBody] string password)
        {
            return;
        }*/
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public AuthController(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            var user = _userRepository.AddUser(userDTO);
           await _emailService.RegisterEmail(user);
            return Ok(user);

        }
    }
}



