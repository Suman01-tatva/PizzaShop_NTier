using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly PizzashopContext _context;

    public OrderRepository(PizzashopContext pizzashopContext)
    {
        _context = pizzashopContext;
    }
    public async Task<List<Order>> GetAllOrders(string searchString, int pageIndex, int pageSize, bool isAsc, DateOnly? fromDate, DateOnly? toDate, string sortColumn, int status, string dateRange)
    {

        var query = _context.Orders.Include(i => i.Customer).Include(i => i.Feedbacks).Include(i => i.Invoices).ThenInclude(i => i.Payments).AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.Trim().ToLower();
            query = query.Where(i => i.Customer.Name.ToLower().Contains(searchString) || i.Id.ToString().Contains(searchString));
        }

        if (status != 0)
        {
            query = query.Where(i => i.OrderStatus.ToString().Contains(status.ToString()));
        }

        switch (dateRange)
        {
            case "Last7Days":
                fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
                break;
            case "Last30Days":
                fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
                break;
            // case "CurrentMonth":
            //     var now = DateTime.Now;
            //     fromDate = new DateOnly(now.Year, now.Month, 1);
            //     toDate = fromDate.AddMonth(1).AddDays(-1);
            //     break;
            case "AllTime":
            default:
                break;
        }

        if (fromDate != null)
        {
            query = query.Where(i => i.OrderDate >= fromDate);
        }

        if (toDate != null)
        {
            query = query.Where(i => i.OrderDate <= toDate);
        }

        if (isAsc)
        {
            query = sortColumn.ToLower() switch
            {
                "customer" => query.OrderBy(i => i.Customer.Name),
                "totalamount" => query.OrderBy(i => i.PaidAmount),
                "date" => query.OrderBy(i => i.OrderDate),
                _ => query.OrderBy(i => i.Id),
            };
        }
        else
        {
            query = sortColumn.ToLower() switch
            {
                "customer" => query.OrderByDescending(i => i.Customer.Name),
                "totalamount" => query.OrderByDescending(i => i.PaidAmount),
                "date" => query.OrderByDescending(i => i.OrderDate),
                _ => query.OrderByDescending(i => i.Id),
            };
        }

        return await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public int TotalOrderCount()
    {
        int count = _context.Orders.Count();
        return count;
    }
}