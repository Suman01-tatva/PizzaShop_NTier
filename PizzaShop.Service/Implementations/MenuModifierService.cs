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
}
