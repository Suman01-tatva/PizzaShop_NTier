using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    [HttpGet]
    public async Task<IActionResult> Orders(string searchString = "", int pageIndex = 1, int pageSize = 5, bool isAsc = true, DateOnly? fromDate = null, DateOnly? toDate = null, string sortColumn = "", int status = 0, string dateRange = "AllTime")
    {
        var orders = await _orderService.GetAllOrders(searchString, pageIndex, pageSize, isAsc, fromDate, toDate, sortColumn, status, dateRange);
        var count = _orderService.TotalOrderCount();
        var pageData = new OrderPageViewModel
        {
            Orders = orders,
            PageIndex = pageIndex,
            PageSize = pageSize,
            IsAsc = isAsc,
            FromDate = fromDate,
            ToDate = toDate,
            Status = status,
            sortColumn = sortColumn,
            SearchString = searchString,
            TotalPage = (int)Math.Ceiling(count / (double)pageSize),
            TotalOrders = count,
            DateRange = dateRange,
        };

        return View(pageData);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(string searchString, int pageIndex, int pageSize, bool isAsc, DateOnly? fromDate, DateOnly? toDate, string sortColumn = "", int status = 0, string dateRange = "AllTime")
    {
        var orders = await _orderService.GetAllOrders(searchString, pageIndex, pageSize, isAsc, fromDate, toDate, sortColumn, status, dateRange);
        var count = _orderService.TotalOrderCount();

        var pageData = new OrderPageViewModel
        {
            Orders = orders,
            PageIndex = pageIndex,
            PageSize = pageSize,
            IsAsc = isAsc,
            FromDate = fromDate,
            ToDate = toDate,
            Status = status,
            sortColumn = sortColumn,
            SearchString = searchString,
            TotalPage = (int)Math.Ceiling(count / (double)pageSize),
            TotalOrders = count,
            DateRange = dateRange,
        };
        if (orders == null)
        {
            return PartialView("_OrderTablePartial", new OrderPageViewModel());
        }

        return PartialView("_OrderTablePartial", pageData);
    }
}
