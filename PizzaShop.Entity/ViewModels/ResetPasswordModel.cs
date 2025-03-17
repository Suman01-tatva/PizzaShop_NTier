using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;

public class ResetPasswordModel
{

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "New Password is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&]).{6,}$",
            ErrorMessage = "Password must be at least 6 characters long and include uppercase, lowercase, number, and special character.")]
    public string NewPassword { set; get; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Confirm Password is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&]).{6,}$",
            ErrorMessage = "Password must be at least 6 characters long and include uppercase, lowercase, number, and special character.")]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { set; get; } = null!;
}
