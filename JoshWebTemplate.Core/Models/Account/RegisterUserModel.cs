namespace JoshWebTemplate.Core.Models.Account;

public class RegisterUserModel
{
    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string Email { get; set; } = "";

    public string Password { get; set; } = "";

    public string? ConfirmPassword { get; set; }

    public string? PhoneNumber { get; set; }
}
