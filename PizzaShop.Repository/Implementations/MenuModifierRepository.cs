using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class MenuModifierRepository : IMenuModifierRepository
{
    private readonly PizzashopContext _context;

    public MenuModifierRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<MenuModifierViewModel>> GetModifiersByModifierGroupAsync(int id)
    {
        var itmes = await _context.Modifiers
            .Select(c => new MenuModifierViewModel
            {
                Id = c.Id,
                UnitName = c.Unit.ShortName,
                ModifierGroupId = c.ModifierGroupId,
                Name = c.Name,
                Description = c.Description,
                Rate = c.Rate,
                Quantity = c.Quantity,
            }).Where(c => c.ModifierGroupId == id).ToListAsync();

        return itmes;
    }
}
