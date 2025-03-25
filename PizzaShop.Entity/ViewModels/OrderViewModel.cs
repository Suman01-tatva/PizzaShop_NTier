namespace PizzaShop.Entity.ViewModels;

public class OrderViewModel
{
    public int Order { get; set; }

    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int? OrderNo { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? Tax { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? Discount { get; set; }

    public decimal? PaidAmount { get; set; }

    public string? Notes { get; set; }

    public bool? IsSgstSelected { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public int? OrderStatus { get; set; }
    public DateOnly? Date { get; set; }

    public string? CustomerName { get; set; }

    public string? Status { get; set; }

    public string? PaymentMode { get; set; }

    public int? Rating { get; set; }

}
