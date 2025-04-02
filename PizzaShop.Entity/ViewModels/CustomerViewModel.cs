namespace PizzaShop.Entity.ViewModels;

public class CustomerViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public string Phone { get; set; } = null!;

    public string? TotalOrders { get; set; }
}