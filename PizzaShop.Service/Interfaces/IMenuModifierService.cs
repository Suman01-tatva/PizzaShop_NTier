using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IMenuModifierService
{
    Task<List<MenuModifierGroupViewModel>> GetAllMenuModifierGroupAsync();
    public Task<List<MenuModifierViewModel>> GetModifiersByModifierGroup(int id, int pageSize, int pageIndex, string? searchString);
    public Task<ModifierTabViewModel> GetModifierTabDetails(int ModifierGroupId, int pageSize, int pageIndex, string? searchString);
    public int GetModifiersCountByCId(int mId, string? searchString);
    public bool AddModifier(AddEditModifierViewModel model, int role);
    public bool EditModifier(AddEditModifierViewModel model, int userId);
    public void DeleteModifier(int id);
    public void DeleteMultipleModifiers(int[] modifierIds);
    public AddEditModifierViewModel GetModifierByid(int id);
    public List<MenuModifierViewModel> GetModifiersByGroupId(int id);
}
