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
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ICandidateRepository _candidateRepository;

        public AuthController(IUserRepository userRepository, IEmailService emailService, ICandidateRepository candidateRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _candidateRepository = candidateRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            var exist = _userRepository.GetByEmail(userDTO.Email);
            if (exist != null)
                return Conflict(new ApiResponse<object>(success: false, message: $"Account already Exist with {userDTO.Email}"));
            var user = _userRepository.AddUser(userDTO);
            try
            {
                await _emailService.RegisterEmail(user, userDTO.ClientUrl);

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(success: false, message: $"Issue on mail verification, please check Address: {userDTO.Email}"));
            }
            return Ok(new ApiResponse<object>(success: true, message: $"Verification mail sent on {userDTO.Email}")); 

        }
        [HttpGet("verify/{token}/{email}")]
        public async Task<IActionResult> Verify(string token, string email)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null)
                return BadRequest( new { message = "User does not Exist." });
            if (user.IsVerified)
                return Accepted(new { message = "Account is Already Verified." });
            var isValid = _userRepository.VerifyToken(token, user.UserId);
            if (!isValid)
                return BadRequest(new { message = "Token is Invalid" });
            return Ok(new{ message = "Email Verified Successfully"});

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var res = await _userRepository.Login(loginRequest);
            if(res.Success)
                return Ok(res);
            return BadRequest(res);
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var res = await _userRepository.LoginViaRefreshToken(refreshToken);
            if(res.Success)
                return Ok(res);
            return Unauthorized(res);
        }

        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin([FromBody] string password)
        {
            var ret = _userRepository.VerifyAdmin(password);
            //if(!ret.Success)
              //  return Unauthorized(ret);
            return Ok(ret);
        }
        [HttpPost("candidate-login")]
        public async Task<IActionResult> CandidateLogin([FromBody] LoginRequest loginRequest)
        {
            var ret = await _candidateRepository.Login(loginRequest);
            if (!ret.Success)
                return Unauthorized(ret);
            return Ok(ret);
        }
    }
}



