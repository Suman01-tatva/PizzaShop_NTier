using System.Threading.Tasks;
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

    public List<Modifier> GetModifiersByGroupId(int id)
    {
        var modifiers = _context.Modifiers.Where(i => i.ModifierGroupId == id && i.IsDeleted == false).OrderBy(i => i.Name).ToList();
        return modifiers;
    }

    public void DeleteModifier(int id)
    {
        var modifier = _context.Modifiers.FirstOrDefault(a => a.Id == id);
        modifier!.IsDeleted = true;
        _context.SaveChanges();
    }

    public void AddModifier(AddEditModifierViewModel model, int userId)
    {

        var newModifier = new Modifier
        {
            Name = model.Name,
            Rate = model.Rate,
            Quantity = model.Quantity,
            IsDeleted = false,
            Description = model.Description,
            UnitId = model.UnitId,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            ModifierGroupId = model.Modifiergroupid
        };

        _context.Modifiers.Add(newModifier);
        _context.SaveChanges();
    }

    public int isModifierExist(string name, int? modGrpId)
    {
        var modifier = _context.Modifiers.Where(m => m.IsDeleted == false && m.Name == name && m.ModifierGroupId == modGrpId).Count();
        return modifier;
    }

    public bool isEditModifierExist(string name, int? id, int? modGrpId)
    {
        var modifier = _context.Modifiers.Where(m => m.IsDeleted == false && m.Name == name && m.ModifierGroupId == modGrpId).ToList();
        foreach (var m in modifier)
        {
            if (m.Id != id)
                return true;
        }
        return false;
    }

    public void EditModifier(AddEditModifierViewModel model, int userId)
    {
        var modifier = _context.Modifiers.FirstOrDefault(m => m.Id == model.Id);
        modifier!.Name = model.Name;
        modifier.Rate = model.Rate;
        modifier.Quantity = model.Quantity;
        modifier.Description = model.Description;
        modifier.ModifierGroupId = modifier.ModifierGroupId;
        modifier.UnitId = model.UnitId;
        modifier.ModifiedBy = userId;
        modifier.ModifiedAt = DateTime.UtcNow;
        _context.SaveChanges();
    }

    public Modifier? GetModifierById(int id)
    {
        var modifier = _context.Modifiers.FirstOrDefault(m => m.Id == id);
        return modifier;
    }

    public async Task<List<MenuModifierViewModel>> GetAllModifiers(string? searchString)
    {
        var modifiers = _context.Modifiers
                                 .Where(m => m.IsDeleted == false)
                                 .OrderBy(m => m.Name)
                                 .AsQueryable();
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.Trim().ToLower();

            modifiers = modifiers.Where(n =>
                n.Name!.ToLower().Contains(searchString));
        }

        var filteredModifiers = modifiers.Select(c => new MenuModifierViewModel
        {
            Id = c.Id,
            UnitName = c.Unit.ShortName,
            ModifierGroupId = c.ModifierGroupId,
            Name = c.Name,
            Description = c.Description,
            Rate = c.Rate,
            Quantity = c.Quantity,
        }).ToList();

        return filteredModifiers;
    }
}
