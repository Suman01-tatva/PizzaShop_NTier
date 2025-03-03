using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    private readonly ITokenDataService _tokenDataService;

    private readonly IMailService _mailService;

    public UserController(IUserService userService, ITokenDataService tokenDataService, IMailService mailService)
    {
        _userService = userService;

        _tokenDataService = tokenDataService;

        _mailService = mailService;
    }

    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            // var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            // if (userEmailClaim == null)
            // {
            //     return Unauthorized("Email not found in token");
            // }

            // string userEmail = userEmailClaim.Value;
            string userEmail = Request.Cookies["email"];
            bool isPasswordChanged = await _userService.ChangePasswordAsync(model, userEmail);

            if (isPasswordChanged)
            {
                return RedirectToAction("AdminDashboard", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Please Enter valid password");
                return View();
            }
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        // var email = Request.Cookies["email"];
        var token = Request.Cookies["Token"];
        var email = await _tokenDataService.GetEmailFromToken(token);

        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Index", "Home");
        }

        var userViewModel = await _userService.GetUserProfileAsync(email);
        if (userViewModel == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var allCountries = await _userService.GetAllCountriesAsync();
        var allStates = await _userService.GetStatesByCountryIdAsync(userViewModel.CountryId);
        var allCities = await _userService.GetCitiesByStateIdAsync(userViewModel.StateId);

        ViewBag.Countries = new SelectList(allCountries, "Id", "Name", userViewModel.CountryId);
        ViewBag.States = new SelectList(allStates, "Id", "Name", userViewModel.StateId);
        ViewBag.Cities = new SelectList(allCities, "Id", "Name", userViewModel.CityId);

        return View(userViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(UserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var email = Request.Cookies["email"];
            await _userService.UpdateUserProfileAsync(model, email);

            ViewData["ProfileSuccessMessage"] = "Profile Updated Successfully";
            return RedirectToAction("AdminDashboard", "Home");
        }

        return View(model);
    }

    public IActionResult UserList(string searchString, int pageIndex = 1, int pageSize = 5, string sortOrder = "")
    {
        var users = _userService.GetUserList(searchString, sortOrder, pageIndex, pageSize, out int count);

        ViewData["UsernameSortParam"] = sortOrder == "username_asc" ? "username_desc" : "username_asc";
        ViewData["RoleSortParam"] = sortOrder == "role_asc" ? "role_desc" : "role_asc";

        ViewBag.count = count;
        ViewBag.pageIndex = pageIndex;
        ViewBag.pageSize = pageSize;
        ViewBag.totalPage = (int)Math.Ceiling(count / (double)pageSize);
        ViewBag.searchString = searchString;

        if (users == null || !users.Any())
        {
            ViewBag.ErrorMessage = "UserList is Empty";
            return View();
        }

        ViewBag.UserList = users;
        return View();
    }

    public async Task<IActionResult> CreateUser()
    {
        await PopulateDropdowns();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(UserViewModel model)
    {
        if (ModelState.IsValid)
        {

            if (await _userService.UserExistsAsync(model.Email))
            {
                ViewBag.UserExistError = "User Already Exist";
                await PopulateDropdowns();
                return View(model);
            }

            var token = Request.Cookies["Token"];
            var currentUserEmail = await _tokenDataService.GetEmailFromToken(token);
            await _userService.AddUserAsync(model, currentUserEmail);
            _mailService.SendMail(model.Email, "Welcome To Pizza Shop");

            return RedirectToAction("UserList");
        }
        else
        {
            await PopulateDropdowns();
            return View(model);
        }
    }

    private async Task PopulateDropdowns()
    {
        var countries = await _userService.GetAllCountriesAsync();
        var roles = await _userService.GetAllRolesAsync();

        ViewBag.Countries = new SelectList(countries, "Id", "Name");
        ViewBag.Roles = new SelectList(roles, "Id", "Name");
    }

    [HttpPost]
    public IActionResult DeleteUser(int id)
    {
        _userService.DeleteUser(id);
        return RedirectToAction(nameof(UserList));
    }

    public async Task<IActionResult> UpdateUser(int id)
    {
        ViewBag.Roles = new SelectList(await _userService.GetAllRolesAsync(), "Id", "Name");

        var model = await _userService.GetUserByIdForUpdate(id);
        if (model == null)
        {
            return NotFound();
        }

        ViewBag.Roles = new SelectList(await _userService.GetAllRolesAsync(), "Id", "Name");
        ViewBag.Countries = new SelectList(await _userService.GetAllCountriesAsync(), "Id", "Name", model.CountryId);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser(UserUpdateViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _userService.UpdateUserAsync(model);
            return RedirectToAction("UserList");
        }

        ViewBag.Roles = new SelectList(await _userService.GetAllRolesAsync(), "Id", "Name");
        ViewBag.Countries = new SelectList(await _userService.GetAllCountriesAsync(), "Id", "Name", model.CountryId);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetStates(int countryId)
    {
        var states = await _userService.GetStatesByCountryIdAsync(countryId);
        return Json(states);
    }

    [HttpGet]
    public async Task<IActionResult> GetCities(int stateId)
    {
        var cities = await _userService.GetCitiesByStateIdAsync(stateId);
        return Json(cities);
    }
}