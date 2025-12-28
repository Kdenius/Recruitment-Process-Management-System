namespace WebApi.DTO
{
    public class UserRoleDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
