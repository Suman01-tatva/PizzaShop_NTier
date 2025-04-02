using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

public class CustomerController : Controller
{
    // private readonly ITaxAndFeesService _taxesAndFeesService;
    // public TaxAndFeeController(ITaxAndFeesService taxesAndFeesService)
    // {
    //     _taxesAndFeesService = taxesAndFeesService;
    // }

    public IActionResult Customers()
    {
        return View();
    }
}