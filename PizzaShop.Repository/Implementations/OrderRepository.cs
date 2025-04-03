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
    public async Task<(List<Order> list, int count)> GetAllOrders(string searchString, int pageIndex, int pageSize, bool isAsc, DateOnly? fromDate, DateOnly? toDate, string sortColumn, int status, string dateRange)
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
            case "CurrentMonth":
                var now = DateTime.Now;
                fromDate = new DateOnly(now.Year, now.Month, 1);
                toDate = new DateOnly(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
                break;
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

        //Sorting
        switch (sortColumn)
        {
            case "id_asc":
                query = query.OrderBy(o => o.Id);
                break;
            case "id_desc":
                query = query.OrderByDescending(o => o.Id);
                break;

            case "date_asc":
                query = query.OrderBy(o => o.OrderDate);
                break;
            case "date_desc":
                query = query.OrderByDescending(o => o.OrderDate);
                break;

            case "cust_asc":
                query = query.OrderBy(o => o.Customer.Name).ThenBy(o => o.OrderDate);
                break;
            case "cust_desc":
                query = query.OrderByDescending(o => o.Customer.Name).ThenByDescending(o => o.OrderDate);
                break;
            case "amount_asc":
                query = query.OrderBy(o => o.TotalAmount).ThenBy(o => o.Customer.Name);
                break;
            case "amount_desc":
                query = query.OrderByDescending(o => o.TotalAmount).ThenByDescending(o => o.Customer.Name);
                break;
            default:
                query = query.OrderBy(o => o.Id);
                break;
        }

        // var orders = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return (query.ToList(), query.ToList().Count());
    }

    public async Task<Order> OrderDetails(int id)
    {
        var orderDetails = await _context.Orders.Where(o => o.Id == id)
                                    .Include(i => i.Customer)
                                    .Include(o => o.Invoices)
                                    .ThenInclude(p => p.Payments)
                                    .Include(o => o.OrderedItems)
                                    .ThenInclude(O => O.OrderedItemModifierMappings)
                                    .ThenInclude(m => m.Modifier)
                                    .Include(o => o.TableOrderMappings)
                                    .ThenInclude(o => o.Table)
                                    .ThenInclude(o => o.Section)
                                    .Include(o => o.OrderTaxMappings)
                                    .ThenInclude(o => o.Tax)
                                    .FirstOrDefaultAsync();
        return orderDetails!;

    }
}