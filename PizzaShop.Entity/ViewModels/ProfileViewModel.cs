using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;

public class ProfileViewModel
{
    [Required(ErrorMessage = "FirstName is required.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "LastName is required")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "UserName is required")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Phone is required")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Country is required")]
    public int CountryId { get; set; }

    [Required(ErrorMessage = "State is required")]
    public int StateId { get; set; }

    [Required(ErrorMessage = "City is required")]
    public int CityId { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "ZipCode is required")]
    public string ZipCode { get; set; } = null!;

    [Required(ErrorMessage = "ZipCode is required")]
    public int RoleId { get; set; }

    [Required(ErrorMessage = "ZipCode is required")]
    public string Email { get; set; } = null!;
}