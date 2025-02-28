using PizzaShop.Entity.Data;

namespace PizzaShop.Repository.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);

    Task UpdateUserAsync(User user);

    IEnumerable<User> GetUserList(string searchString, string sortOrder, int pageIndex, int pageSize, out int count);
    User GetUserById(int id);
    void DeleteUser(int id);
}