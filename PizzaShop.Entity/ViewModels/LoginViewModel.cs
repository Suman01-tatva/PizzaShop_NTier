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
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }
}