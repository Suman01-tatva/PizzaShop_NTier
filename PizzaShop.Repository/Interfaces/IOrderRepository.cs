using PizzaShop.Entity.Data;

namespace PizzaShop.Repository.Interfaces;

public interface IOrderRepository
{
    public Task<List<Order>> GetAllOrders(string searchString, int pageIndex, int pageSize, bool isAsc, DateOnly? fromDate, DateOnly? toDate, string sortColumn, int status, string dateRange);
    public int TotalOrderCount();
}
