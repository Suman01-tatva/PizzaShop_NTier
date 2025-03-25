namespace PizzaShop.Entity.ViewModels;

public class OrderPageViewModel
{
    public List<OrderViewModel> Orders { get; set; }

    public string SearchString { get; set; }

    public int Status { get; set; }

    public int Time { get; set; }

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

}
