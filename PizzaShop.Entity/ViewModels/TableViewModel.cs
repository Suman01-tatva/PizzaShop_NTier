namespace PizzaShop.Entity.ViewModels;

public class TableViewModel
{

    public int Id { get; set; }
    public int SectionId { get; set; }
    public string Name { get; set; } = null!;
    public int? Capacity { get; set; }
    public bool? IsAvailable { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public int? ModifiedBy { get; set; }
}