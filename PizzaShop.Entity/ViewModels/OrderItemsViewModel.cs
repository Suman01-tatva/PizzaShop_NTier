namespace PizzaShop.Entity.ViewModels;

public class OrderItemsViewModel
{
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemModifierViewModel>? Modifiers { get; set; }
    public int QuantityOfModifier { get; set; }
    public decimal ModifiersPrice { get; set; }
    public decimal? TotalModifierAmount { get; set; }

}
