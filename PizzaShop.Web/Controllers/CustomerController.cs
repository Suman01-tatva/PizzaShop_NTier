using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Web.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;

public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [CustomAuthorize("Customers", "CanView")]
    public async Task<IActionResult> Customers(string searchString = "", string sortOrder = "", int pageIndex = 1, int pageSize = 5, string dateRange = "AllTime", DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        var customers = await _customerService.GetCustomerList(searchString, sortOrder, pageIndex, pageSize, dateRange, fromDate, toDate);
        ViewData["nameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : sortOrder == "name_desc" ? "name_asc" : "name_desc";
        ViewData["dateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
        ViewData["totalOrderSortParam"] = sortOrder == "totalOrder_asc" ? "totalOrder_desc" : "totalOrder_asc";
        customers.SortOrder = sortOrder;

        return View(customers);
    }

    [CustomAuthorize("Customers", "CanView")]
    public async Task<IActionResult> GetCustomers(string searchString = "", string sortOrder = "", int pageIndex = 1, int pageSize = 5, string dateRange = "AllTime", DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        var customers = await _customerService.GetCustomerList(searchString, sortOrder, pageIndex, pageSize, dateRange, fromDate, toDate);
        ViewData["nameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : sortOrder == "name_desc" ? "name_asc" : "name_desc";
        ViewData["dateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
        ViewData["totalOrderSortParam"] = sortOrder == "totalOrder_asc" ? "totalOrder_desc" : "totalOrder_asc";
        customers.SortOrder = sortOrder;
        return PartialView("_CustomerList", customers);
    }

    public Task<FileContentResult> GenerateExcelFile(string searchString = "", string sortOrder = "", int pageIndex = 1, int pageSize = 5, string dateRange = "AllTime", DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        return _customerService.ExportCustomersExcel(searchString, sortOrder, pageIndex, pageSize, dateRange, fromDate, toDate);
    }
}