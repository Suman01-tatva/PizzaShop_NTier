using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModels;

public class SectionViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Section Name is Required")]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public int? ModifiedBy { get; set; }
}

