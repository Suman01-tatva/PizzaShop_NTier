namespace PizzaShop.Service.Interfaces;
using System.Threading.Tasks;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

public interface IUserService
{
    Task<bool> ChangePasswordAsync(ChangePasswordViewModel model, string userEmail);
    Task<UserViewModel> GetUserProfileAsync(string email);
    Task UpdateUserProfileAsync(UserViewModel model, string email);
    Task<List<Country>> GetAllCountriesAsync();
    Task<List<State>> GetStatesByCountryIdAsync(int? countryId);
    Task<List<City>> GetCitiesByStateIdAsync(int? stateId);
    IEnumerable<User> GetUserList(string searchString, string sortOrder, int pageIndex, int pageSize, out int count);
    User GetUserById(int id);
    Task<UserUpdateViewModel> GetUserByIdForUpdate(int id);
    void DeleteUser(int id);
    Task<bool> UserExistsAsync(string email);
    Task AddUserAsync(UserViewModel model, string currentUserEmail);
    Task<IEnumerable<Role>> GetAllRolesAsync();
    Task UpdateUserAsync(UserUpdateViewModel model);

    Task<User?> GetUserByEmailAsync(string email);
}