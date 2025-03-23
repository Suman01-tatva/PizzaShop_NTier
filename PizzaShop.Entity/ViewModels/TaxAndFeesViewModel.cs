namespace PizzaShop.Entity.ViewModels;
using System.ComponentModel.DataAnnotations;

public class TaxAndFeesViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Type is required.")]
    public bool? Type { get; set; }

    [Required(ErrorMessage = "Percentage is required.")]
    [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100.")]
    public decimal? Percentage { get; set; }

    [Required(ErrorMessage = "FlatAmount is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Flat amount must be non-negative.")]
    public decimal? FlatAmount { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDefault { get; set; } = false;

    [Range(0, double.MaxValue, ErrorMessage = "Tax value must be non-negative.")]
    public decimal? TaxValue { get; set; }

    public bool? IsDeleted { get; set; } = false;
}
