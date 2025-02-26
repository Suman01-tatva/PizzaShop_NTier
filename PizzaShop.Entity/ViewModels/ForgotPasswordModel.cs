using System.ComponentModel.DataAnnotations;
namespace PizzaShop.Entity.ViewModels;
public class ForgotPasswordModel
{
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = null!;
}