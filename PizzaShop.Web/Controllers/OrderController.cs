using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PizzaShop.Web.Controllers;

public class OrderController : Controller
{
    // private readonly ITaxAndFeesService _taxesAndFeesService;
    // public TaxAndFeeController(ITaxAndFeesService taxesAndFeesService)
    // {
    //     _taxesAndFeesService = taxesAndFeesService;
    // }

    public IActionResult Orders()
    {
        return View();
    }
}
