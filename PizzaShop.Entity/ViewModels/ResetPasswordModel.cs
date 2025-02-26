using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;

public class ResetPasswordModel
{

    [Required(ErrorMessage = "New Password is Required")]
    public string NewPassword { set; get; } = null!;

    [Required(ErrorMessage = "Confirm New Password is Required")]
    public string ConfirmNewPassword { set; get; } = null!;
}
