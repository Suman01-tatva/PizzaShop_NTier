using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface IOrderRepository
{
    public Task<(List<Order> list, int count)> GetAllOrders(string searchString, int pageIndex, int pageSize, bool isAsc, DateOnly? fromDate, DateOnly? toDate, string sortColumn, int status, string dateRange);

    public Task<Order> OrderDetails(int id);
}
