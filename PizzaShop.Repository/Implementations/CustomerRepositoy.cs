using PizzaShop.Entity.Data;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class CustomerRepositoy 
{
    private readonly PizzashopContext _context;

    public CustomerRepositoy(PizzashopContext context)
    {
        _context = context;
    }

    // public IEnumerable<Customer> GetCustomerList(string searchString, string sortOrder, int pageIndex, int pageSize,string DateRange)
    // {
    //     var CustomerQuery = _context.Customers.AsQueryable();

    //     if (!string.IsNullOrEmpty(searchString))
    //     {
    //         searchString = searchString.Trim().ToLower();

    //         CustomerQuery = CustomerQuery.Where(n => n.Name.ToLower().Contains(searchString));
    //     }

    //     switch (sortOrder)
    //     {
    //         case "name_asc":
    //             CustomerQuery = CustomerQuery.OrderBy(u => u.Name);
    //             break;

    //         case "name_desc":
    //             CustomerQuery = CustomerQuery.OrderByDescending(u => u.Name);
    //             break;

    //         case "date_asc":
    //             CustomerQuery = CustomerQuery.OrderBy(u => u.CreatedAt);
    //             break;

    //         case "date_desc":
    //             CustomerQuery = CustomerQuery.OrderByDescending(u => u.CreatedAt);
    //             break;

    //          case "totalOrder_asc":
    //             CustomerQuery = CustomerQuery.OrderBy(u => u..Name).ThenBy(u => u.FirstName);
    //             break;

    //         case "totalOrder_desc":
    //             CustomerQuery = CustomerQuery.OrderByDescending(u => u.Role.Name).ThenBy(u => u.FirstName);
    //             break;
    //         default:
    //             CustomerQuery = CustomerQuery.OrderBy(u => u.Name);
    //             break;
    //     }

    //     return userQuery
    //         .Skip((pageIndex - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToList();

    // }
}