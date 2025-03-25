using PizzaShop.Entity.Data;

namespace PizzaShop.Repository.Interfaces;

public interface IOrderRepository
{
    public Task<IEnumerable<Order>> GetAllOrders();

}
