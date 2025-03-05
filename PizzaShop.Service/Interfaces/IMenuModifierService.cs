using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IMenuModifierService
{
    Task<List<MenuModifierGroupViewModel>> GetAllMenuModifierGroupAsync();

}
