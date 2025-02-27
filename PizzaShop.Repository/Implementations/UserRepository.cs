using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class UserRepostory : IUserRepository
{
    private readonly PizzashopContext _context;

    public UserRepostory(PizzashopContext context)
    {
        _context = context;
    }

    public async Task UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<Role?> CheckRole(string role)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == role);
    }
}