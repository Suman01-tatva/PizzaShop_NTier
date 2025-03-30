using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface IMenuModifierGroupRepository
{
    List<MenuModifierGroupViewModel> GetAllMenuModifierGroupsAsync();

}
