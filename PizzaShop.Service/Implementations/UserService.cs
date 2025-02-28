namespace PizzaShop.Service.Implementations;

using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.Service.Utils;

public class UserService : IUserService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IStateRepository _stateRepository;
    private readonly ICityRepository _cityRepository;

    public UserService(
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        ICountryRepository countryRepository,
        IStateRepository stateRepository,
        ICityRepository cityRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _countryRepository = countryRepository;
        _stateRepository = stateRepository;
        _cityRepository = cityRepository;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordViewModel model, string userEmail)
    {
        var user = await _userRepository.GetUserByEmailAsync(userEmail);
        var hashPassword = PasswordUtills.HashPassword(model.CurrentPassword);
        if (user?.Password == hashPassword)
        {
            string changedPassword = PasswordUtills.HashPassword(model.NewPassword);
            user.Password = changedPassword;
            await _userRepository.UpdateUserAsync(user);
            return true;
        }
        return false;
    }


    public async Task<UserViewModel> GetUserProfileAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        var userViewModel = new UserViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Phone = user.Phone,
            CountryId = user.CountryId,
            StateId = user.StateId,
            CityId = user.CityId,
            Zipcode = user.Zipcode,
            Address = user.Address,
            RoleId = user.RoleId,
            Email = email
        };

        return userViewModel;
    }

    public async Task UpdateUserProfileAsync(UserViewModel model, string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null) return;

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Username = model.Username;
        user.Phone = model.Phone;
        user.CityId = model.CityId;
        user.StateId = model.StateId;
        user.CountryId = model.CountryId;
        user.Address = model.Address;
        user.Zipcode = model.Zipcode;

        await _userRepository.UpdateUserAsync(user);
    }

    public IEnumerable<User> GetUsers(string searchString, string sortOrder, int pageIndex, int pageSize, out int count)
    {
        return _userRepository.GetUserList(searchString, sortOrder, pageIndex, pageSize, out count);
    }

    public User GetUserById(int id)
    {
        return _userRepository.GetUserById(id);
    }

    public void DeleteUser(int id)
    {
        _userRepository.DeleteUser(id);
    }

    public async Task<List<Country>> GetAllCountriesAsync()
    {
        return await _countryRepository.GetAllCountriesAsync();
    }

    public async Task<List<State>> GetStatesByCountryIdAsync(int? countryId)
    {
        return await _stateRepository.GetStatesByCountryIdAsync(countryId);
    }

    public async Task<List<City>> GetCitiesByStateIdAsync(int? stateId)
    {
        return await _cityRepository.GetCitiesByStateIdAsync(stateId);
    }
}