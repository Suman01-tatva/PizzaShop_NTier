using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class UserRepostory : IUserRepository
{
    private readonly PizzashopContext _context;

    public UserRepostory(PizzashopContext context)
    {
        _context = context;
    }
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task UpdateUserAsync(User user)
    {
        if (user == null) return;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public IEnumerable<User> GetUserList(string searchString, string sortOrder, int pageIndex, int pageSize, out int count)
    {
        var userQuery = _context.Users.Where(u => u.IsDeleted == false);

        switch (sortOrder)
        {
            case "username_asc":
                userQuery = userQuery.OrderBy(u => u.FirstName);
                break;

            case "username_desc":
                userQuery = userQuery.OrderByDescending(u => u.FirstName);
                break;

            case "role_asc":
                userQuery = userQuery.OrderBy(u => u.Role.Name);
                break;

            case "role_desc":
                userQuery = userQuery.OrderByDescending(u => u.Role.Name);
                break;

            default:
                userQuery = userQuery.OrderBy(u => u.Id);
                break;
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower();

            userQuery = userQuery.Where(n =>
                (n.FirstName + " " + n.LastName).ToLower().Contains(searchString) || // Full name search
                n.FirstName!.ToLower().Contains(searchString) ||
                n.LastName!.ToLower().Contains(searchString) ||
                n.Phone!.Contains(searchString) // Assuming phone numbers are numeric and case-insensitive search isn't needed
            );
        }

        count = userQuery.Count();

        return userQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public User GetUserById(int id)
    {
        return _context.Users.Find(id);
    }

    public void DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            user.IsDeleted = true;
            _context.SaveChanges();
        }
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

}