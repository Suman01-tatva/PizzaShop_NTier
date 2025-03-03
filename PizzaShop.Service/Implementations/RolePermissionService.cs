namespace PizzaShop.Service.Implementations;

using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

using PizzaShop.Service.Interfaces;


public class RolePermissionService : IRolePermissionService
{
    private readonly IRoleRepository _role;
    private readonly IRolePermissionRepository _rolePermission;

    public RolePermissionService(IRoleRepository role, IRolePermissionRepository rolePermission)
    {
        _role = role;
        _rolePermission = rolePermission;
    }

    public async Task<List<Role>> GetAllRoles()
    {
        return await _role.GetAllRoles();
    }

    public List<RolePermissionViewModel>? GetRolePermissionByRoleId(int roleId)
    {
        var rolePermission = _rolePermission.GetRolePermissionByRoleId(roleId);
        if (rolePermission != null && rolePermission.Count > 0)
            return rolePermission;
        return null;
    }

    public bool UpdateRolePermission(List<RolePermissionViewModel> model, string email)
    {
        bool isRolePermission = _rolePermission.UpdateRolePermission(model, email);
        return isRolePermission;
    }
}
