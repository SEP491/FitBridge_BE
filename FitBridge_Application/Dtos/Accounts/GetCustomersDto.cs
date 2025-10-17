namespace FitBridge_Application.Dtos.Accounts
{
    public class GetCustomersDto
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsMale { get; set; }

        public string? AvatarUrl { get; set; }
    }
}