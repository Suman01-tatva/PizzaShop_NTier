using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IMenuModifierService
{
    Task<List<MenuModifierGroupViewModel>> GetAllMenuModifierGroupAsync();
    Task<List<MenuModifierViewModel>> GetModifiersByModifierGroup(int id);
    Task<ModifierTabViewModel> GetModifierTabDetails(int id);

}
