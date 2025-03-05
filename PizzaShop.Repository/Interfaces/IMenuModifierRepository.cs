using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface IMenuModifierRepository
{
    Task<List<MenuModifierViewModel>> GetModifiersByModifierGroupAsync(int categoryId);
}
