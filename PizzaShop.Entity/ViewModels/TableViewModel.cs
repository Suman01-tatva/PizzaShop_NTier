using System.ComponentModel.DataAnnotations;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace PizzaShop.Entity.ViewModels;

public class TableViewModel
{

    public int Id { get; set; }
    [Required(ErrorMessage = "Section is Required")]
    public int SectionId { get; set; }
    [Required(ErrorMessage = "Name is Required")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Capacity is Required")]
    [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
    public int? Capacity { get; set; }
    public bool? IsAvailable { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public int? ModifiedBy { get; set; }
}