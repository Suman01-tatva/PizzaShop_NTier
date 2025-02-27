using PizzaShop.Entity.Data;

namespace PizzaShop.Repository.Interfaces;

public interface IUserRepository
{
    Task UpdateUser(User user);
}