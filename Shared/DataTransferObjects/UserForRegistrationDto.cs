using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects
{
    //[Serializable]
    public record UserForRegistrationDto
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; init; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Minimum length for the password is 6 characters")]
        public string? Password { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public ICollection<string>? Roles { get; init; }
    }
}
