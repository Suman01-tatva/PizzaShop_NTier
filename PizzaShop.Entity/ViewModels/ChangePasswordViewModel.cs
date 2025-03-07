using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Current Password is Required")]
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    [DataType(DataType.Password)]
    public string CurrentPassword { set; get; } = null!;

    [Required(ErrorMessage = "New Password is Required")]
    public string NewPassword { set; get; } = null!;

    [Required(ErrorMessage = "Confirm New Password is Required")]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { set; get; } = null!;
}