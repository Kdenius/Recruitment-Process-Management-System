using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity.Data;
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
        [HttpGet("verify/{token}/{id}")]
        public async Task<IActionResult> Verify(string token, int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
                return BadRequest( new { message = "User is not Exist." });
            if (user.IsVerified)
                return BadRequest(new { message = "Account is Already Verified." });
            var isValid = _userRepository.VerifyToken(token, id);
            if (!isValid)
                return BadRequest(new { message = "Token is Invalid" });
            return Ok(new{ message = "Verification Successfully"});

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var res = await _userRepository.Login(loginRequest);
            if(res.Success)
                return Ok(res);
            return BadRequest(res);
        }
    }
}



