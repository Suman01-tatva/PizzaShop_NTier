using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class MenuModifierService : IMenuModifierService
{
    private readonly IMenuModifierGroupRepository _menuModifierGroupRepository;
    private readonly IMenuModifierRepository _menuModifierRepository;

    public MenuModifierService(IMenuModifierGroupRepository menuModifierGroupRepository, IMenuModifierRepository menuModifierRepository)
    {
        _menuModifierGroupRepository = menuModifierGroupRepository ?? throw new ArgumentNullException(nameof(menuModifierGroupRepository));
        _menuModifierRepository = menuModifierRepository ?? throw new ArgumentNullException(nameof(menuModifierRepository));
    }

    public async Task<List<MenuModifierGroupViewModel>> GetAllMenuModifierGroupAsync()
    {
        var modifierGroups = await _menuModifierGroupRepository.GetAllMenuModifierGroupsAsync();
        return modifierGroups;
    }

    public async Task<List<MenuModifierViewModel>> GetModifiersByModifierGroup(int id, int pageSize, int pageIndex, string? searchString)
    {
        var modifierList = await _menuModifierRepository.GetModifiersByModifierGroupAsync(id, pageSize, pageIndex, searchString);
        var filteredModifiers = modifierList
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList().ToList();
        // var filteredModifiers = modifierList.Select(c => new MenuModifierViewModel
        // {
        //     Id = c.Id,
        //     UnitName = c.Unit.ShortName,
        //     ModifierGroupId = c.ModifierGroupId,
        //     Name = c.Name,
        //     Description = c.Description,
        //     Rate = c.Rate,
        //     Quantity = c.Quantity,
        // }).ToList();
        return filteredModifiers;
    }

    public async Task<ModifierTabViewModel> GetModifierTabDetails(int ModifierGroupId, int pageSize, int pageIndex, string? searchString)
    {
        var modifierGroups = await _menuModifierGroupRepository.GetAllMenuModifierGroupsAsync();

        var modifierList = await _menuModifierRepository.GetModifiersByModifierGroupAsync(ModifierGroupId, pageSize, pageIndex, searchString);
        var filteredModifiers = modifierList
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList().ToList();
        // var filteredModifiers = modifierList.Select(c => new MenuModifierViewModel
        // {
        //     Id = c.Id,
        //     UnitName = c.Unit.ShortName,
        //     ModifierGroupId = c.ModifierGroupId,
        //     Name = c.Name,
        //     Description = c.Description,
        //     Rate = c.Rate,
        //     Quantity = c.Quantity,
        // }).ToList();
        var totalModifiers = _menuModifierRepository.GetModifierCountByMId(ModifierGroupId, searchString!);

        var modifierTabViewModel = new ModifierTabViewModel
        {
            modifierGroup = modifierGroups,
            modifier = filteredModifiers,
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPage = (int)Math.Ceiling(totalModifiers / (double)pageSize),
            SearchString = searchString,
            TotalItems = totalModifiers
        };
        return modifierTabViewModel;
    }

    public int GetModifiersCountByCId(int mId, string? searchString)
    {
        return _menuModifierRepository.GetModifierCountByMId(mId, searchString!);
    }
}
