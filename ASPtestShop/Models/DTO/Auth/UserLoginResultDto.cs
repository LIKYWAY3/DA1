namespace ASPtestShop.Models.DTO.Auth
{
    public class UserLoginResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
