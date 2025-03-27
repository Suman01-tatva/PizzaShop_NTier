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

    public async Task<List<MenuModifierViewModel>> GetModifiersByModifierGroupAsync(int id, int pageSize, int pageIndex, string? searchString)
    {
        var modifierQuery = _context.Modifiers.Where(i => i.IsDeleted == false && i.ModifierGroupId == id);

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.Trim().ToLower();

            modifierQuery = modifierQuery.Where(n =>
                n.Name!.ToLower().Contains(searchString));
        }

        var filteredModifiers = modifierQuery.Select(c => new MenuModifierViewModel
        {
            Id = c.Id,
            UnitName = c.Unit.ShortName,
            ModifierGroupId = c.ModifierGroupId,
            Name = c.Name,
            Description = c.Description,
            Rate = c.Rate,
            Quantity = c.Quantity,
        }).OrderBy(m => m.Name).ToList();

        return filteredModifiers;
    }

    public int GetModifierCountByMId(int mId, string? searchString)
    {
        var modifierQuery = _context.Modifiers.Where(i => i.ModifierGroupId == mId && i.IsDeleted == false);
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.Trim().ToLower();

            modifierQuery = modifierQuery.Where(n =>
                n.Name!.ToLower().Contains(searchString)
            );
        }
        int count = modifierQuery.ToList()!.Count();
        return count;
    }
}
