using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class CustomerRepositoy : ICustomerRepository
{
    private readonly PizzashopContext _context;

    public CustomerRepositoy(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<(List<CustomerViewModel>, int count)> GetCustomerList(string searchString, string sortOrder, int pageIndex, int pageSize, string dateRange, DateOnly? fromDate, DateOnly? toDate)
    {
        var customerQuery = _context.Customers
            .Include(c => c.Orders)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.Trim().ToLower();
            customerQuery = customerQuery.Where(n => n.Name.ToLower().Contains(searchString));
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
            case "CustomeDate":
                break;
            case "AllTime":
            default:
                fromDate = null;
                toDate = null;
                break;
        }

        if (fromDate.HasValue)
        {
            customerQuery = customerQuery.Where(i => DateOnly.FromDateTime((DateTime)i.CreatedAt!) >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            customerQuery = customerQuery.Where(i => DateOnly.FromDateTime((DateTime)i.CreatedAt!) <= toDate.Value);
        }

        switch (sortOrder)
        {
            case "name_asc":
                customerQuery = customerQuery.OrderBy(u => u.Name);
                break;
            case "name_desc":
                customerQuery = customerQuery.OrderByDescending(u => u.Name);
                break;
            case "date_asc":
                customerQuery = customerQuery.OrderBy(u => u.CreatedAt).ThenBy(c => c.Name);
                break;
            case "date_desc":
                customerQuery = customerQuery.OrderByDescending(u => u.CreatedAt).ThenBy(c => c.Name);
                break;
            case "totalOrder_asc":
                customerQuery = customerQuery.OrderBy(u => u.Orders.Count()).ThenBy(c => c.Name);
                break;
            case "totalOrder_desc":
                customerQuery = customerQuery.OrderByDescending(u => u.Orders.Count()).ThenBy(c => c.Name);
                break;
            default:
                customerQuery = customerQuery.OrderBy(u => u.Name);
                break;
        }

        List<CustomerViewModel> customers = await customerQuery
            .Select(c => new CustomerViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                ModifiedAt = c.ModifiedAt,
                ModifiedBy = c.ModifiedBy,
                TotalOrders = c.Orders.Count(),
                LastOrderDate = c.Orders.Max(o => o.OrderDate) ?? null
            })
            .ToListAsync();

        return (customers, customers.Count());
    }

    public async Task<Customer> CustomerDetails(int id)
    {
        var customer = _context.Customers.Where(c => c.Id == id)
        .Include(o => o.Orders)
        .ThenInclude(o => o.OrderedItems)
        .Include(o => o.Orders)
        .ThenInclude(i => i.Invoices)
        .ThenInclude(p => p.Payments)
        .FirstOrDefault();
        return customer!;
    }
}