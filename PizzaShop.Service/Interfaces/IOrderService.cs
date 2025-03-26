using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IOrderService
{
    Task<List<OrderViewModel>> GetAllOrders(string searchString, int pageIndex, int pageSize, bool isAsc, DateOnly? fromDate, DateOnly? toDate, string sortColumn, int status, string dateRange);
    public int TotalOrderCount();
}
