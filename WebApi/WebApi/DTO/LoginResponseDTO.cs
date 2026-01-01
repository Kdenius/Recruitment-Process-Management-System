namespace WebApi.DTO
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string JwtToken { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public string RefreshToken { get; set; }

    }
}
