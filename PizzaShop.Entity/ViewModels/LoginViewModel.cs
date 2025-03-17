using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;
public class LoginViewModel
{
    [EmailAddress]
    [Required(ErrorMessage = "Email is required.")]
    [StringLength(50, ErrorMessage = "Must be between 5 and 50 characters", MinimumLength = 5)]
    public string Email { get; set; } = null!;

    [PasswordPropertyText]
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password), MinLength(6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&]).{6,}$",
            ErrorMessage = "Password must be at least 6 characters long and include uppercase, lowercase, number, and special character.")]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }
}