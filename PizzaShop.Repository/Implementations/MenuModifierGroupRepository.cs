using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class MenuModifierGroupRepository : IMenuModifierGroupRepository
{
    private readonly PizzashopContext _context;

    public MenuModifierGroupRepository(PizzashopContext context)
    {
        _context = context;
    }
    public async Task<List<MenuModifierGroupViewModel>> GetAllMenuModifierGroupsAsync()
    {
        var modifierGroups = await _context.ModifierGroups
        .Where(c => c.IsDeleted == false)
        .Select(c => new MenuModifierGroupViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToListAsync();

        return modifierGroups;
    }
}
