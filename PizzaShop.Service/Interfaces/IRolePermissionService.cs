using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IRolePermissionService
{
    public Task<List<Role>> GetAllRoles();
    public List<RolePermissionViewModel>? GetRolePermissionByRoleId(int roleId);
    public bool UpdateRolePermission(List<RolePermissionViewModel> model, string email);
}
