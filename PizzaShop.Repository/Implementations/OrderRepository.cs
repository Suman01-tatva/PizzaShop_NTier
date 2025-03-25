using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly PizzashopContext _context;

    public async Task<IEnumerable<Order>> GetAllOrders()
    {
        IQueryable<Order> query = _context.Orders.Include(i => i.Customer).Include(i => i.Feedbacks).Include(i => i.Invoices).ThenInclude(i => i.Payments);
        return query;
    }
}