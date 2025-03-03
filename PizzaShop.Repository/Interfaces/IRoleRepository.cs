using PizzaShop.Entity.Data;

namespace PizzaShop.Repository.Interfaces;

public interface IRoleRepository
{
    Task<Role> GetRoleByIdAsync(int roleId);
    Task<IEnumerable<Role>> GetAllRolesAsync();
    Task<List<Role>> GetAllRoles();
}