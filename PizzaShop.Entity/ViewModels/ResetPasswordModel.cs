using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;

public class ResetPasswordModel
{

    [Required(ErrorMessage = "NewPassword is required")]
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string NewPassword { set; get; } = null!;

    [Required(ErrorMessage = "Confirm Password is required")]
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    [DataType(DataType.Password)]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { set; get; } = null!;
}
