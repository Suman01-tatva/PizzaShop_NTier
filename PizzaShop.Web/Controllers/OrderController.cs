using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PizzaShop.Entity.Constants;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Attributes;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;
// [CustomAuthorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [CustomAuthorize("Orders", "CanView")]
    [HttpGet]
    public async Task<IActionResult> Orders(string searchString = "", int pageIndex = 1, int pageSize = 5, bool isAsc = true, DateOnly? fromDate = null, DateOnly? toDate = null, string sortColumn = "", int status = 0, string dateRange = "AllTime")
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";
        var (orders, count) = await _orderService.GetAllOrders(searchString, pageIndex, pageSize, isAsc, fromDate, toDate, sortColumn, status, dateRange);

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

    [CustomAuthorize("Orders", "CanView")]
    [HttpGet]
    public async Task<IActionResult> GetOrders(string searchString, int pageIndex, int pageSize, bool isAsc, DateOnly? fromDate, DateOnly? toDate, string sortColumn = "", int status = 0, string dateRange = "AllTime")
    {
        var (orders, count) = await _orderService.GetAllOrders(searchString, pageIndex, pageSize, isAsc, fromDate, toDate, sortColumn, status, dateRange);

        ViewData["OrderIdSortParam"] = string.IsNullOrEmpty(sortColumn) ? "id_desc" : sortColumn == "id_desc" ? "id_asc" : "id_desc";
        ViewData["DateSortParam"] = sortColumn == "date_asc" ? "date_desc" : "date_asc";
        ViewData["CustomerSortParam"] = sortColumn == "cust_asc" ? "cust_desc" : "cust_asc";
        ViewData["TotalAmountSortParam"] = sortColumn == "amount_asc" ? "amount_desc" : "amount_asc";
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

    [HttpGet]
    public async Task<OrderPageViewModel> GetOrderPageDate(string searchString, int pageIndex, int pageSize, bool isAsc, DateOnly? fromDate, DateOnly? toDate, string sortColumn = "", int status = 0, string dateRange = "AllTime")
    {
        var (orders, count) = await _orderService.GetAllOrders(searchString, pageIndex, pageSize, isAsc, fromDate, toDate, sortColumn, status, dateRange);
        ViewData["OrderIdSortParam"] = string.IsNullOrEmpty(sortColumn) ? "id_desc" : sortColumn == "id_desc" ? "id_asc" : "id_desc";
        ViewData["DateSortParam"] = sortColumn == "date_asc" ? "date_desc" : "date_asc";
        ViewData["CustomerSortParam"] = sortColumn == "cust_asc" ? "cust_desc" : "cust_asc";
        ViewData["TotalAmountSortParam"] = sortColumn == "amount_asc" ? "amount_desc" : "amount_asc";

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

        return pageData;
    }

    public async Task<ActionResult> ExportToExcel(string searchString, int pageIndex, int pageSize, bool isAsc, DateOnly? fromDate, DateOnly? toDate, string sortColumn = "", int status = 0, string dateRange = "AllTime")
    {
        var (orders, count) = await _orderService.GetAllOrders(searchString, pageIndex, pageSize, isAsc, fromDate, toDate, sortColumn, status, dateRange);
        ViewData["OrderIdSortParam"] = string.IsNullOrEmpty(sortColumn) ? "id_desc" : sortColumn == "id_desc" ? "id_asc" : "id_desc";
        ViewData["DateSortParam"] = sortColumn == "date_asc" ? "date_desc" : "date_asc";
        ViewData["CustomerSortParam"] = sortColumn == "cust_asc" ? "cust_desc" : "cust_asc";
        ViewData["TotalAmountSortParam"] = sortColumn == "amount_asc" ? "amount_desc" : "amount_asc";

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


        using (ExcelPackage package = new ExcelPackage())
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Orders");

            worksheet.Cells[1, 1, 1, 4].Merge = true;
            worksheet.Cells[1, 1].Value = "Orders Report";
            worksheet.Cells[1, 1].Style.Font.Size = 20;
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 30;

            // Add total records and date range
            worksheet.Cells[2, 1, 3, 2].Merge = true;
            worksheet.Cells[2, 1].Value = $"Total Records: {pageData.TotalOrders}";
            worksheet.Cells[2, 1].Style.Font.Size = 14;
            worksheet.Cells[2, 1].Style.Font.Bold = true;
            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            // worksheet.Cells[2, 5, 3, 2].Merge = true;
            worksheet.Cells[2, 5].Value = $"Search Text: {pageData.SearchString}";

            // worksheet.Cells[4, 1, 5, 4].Merge = true;
            worksheet.Cells[4, 1].Value = $"Date: {pageData.DateRange}";

            // worksheet.Cells[4, 5, 5, 4].Merge = true;
            worksheet.Cells[4, 5].Value = $"Status: {(OrderConstants.OrderStatusEnum)(pageData.Status)}";

            // if (pageData.FromDate.HasValue && pageData.ToDate.HasValue)
            // {
            //     worksheet.Cells[4, 1, 5, 2].Merge = true;
            //     worksheet.Cells[4, 1].Value = $"Date Range: {pageData.FromDate.Value.ToString("dd/MM/yyyy")} - {pageData.ToDate.Value.ToString("dd/MM/yyyy")}";
            //     worksheet.Cells[4, 1].Style.Font.Size = 14;
            //     worksheet.Cells[4, 1].Style.Font.Bold = true;
            //     worksheet.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            //     worksheet.Cells[4, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            // }

            // Add column headers
            worksheet.Cells[6, 1].Value = "Order ID";
            worksheet.Cells[6, 2].Value = "Date";
            worksheet.Cells[6, 3, 6, 5].Merge = true;
            worksheet.Cells[6, 3].Value = "Customer";
            worksheet.Cells[6, 6, 6, 8].Merge = true;
            worksheet.Cells[6, 6].Value = "Status";
            worksheet.Cells[6, 9].Value = "Payment Mode";
            worksheet.Cells[6, 10].Value = "Rating";
            worksheet.Cells[6, 11].Value = "Total Amount";
            worksheet.Cells[6, 1, 6, 10].Style.Font.Bold = true;
            worksheet.Cells[6, 1, 6, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[6, 1, 6, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            // Add data
            int row = 7;
            foreach (var order in pageData.Orders)
            {
                worksheet.Cells[row, 1].Value = order.Id;
                worksheet.Cells[row, 2].Value = order.Date;
                worksheet.Cells[row, 3, row, 5].Merge = true;
                worksheet.Cells[row, 3].Value = order.CustomerName;
                worksheet.Cells[row, 6, row, 8].Merge = true;
                worksheet.Cells[row, 6].Value = (OrderConstants.OrderStatusEnum)(order.Status);
                worksheet.Cells[row, 9].Value = order.PaymentMode;
                worksheet.Cells[row, 10].Value = order.Rating;
                worksheet.Cells[row, 11].Value = order.TotalAmount;
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Download the Excel file
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            string fileName = string.Concat(DateTime.Now.ToString(), "Orders.xlsx");

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }

    [CustomAuthorize("Orders", "CanView")]
    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id)
    {
        if (id == 0 || id == null)
        {
            TempData["ToastrMessage"] = "Order Records Not Found";
            TempData["ToastrType"] = "error";
            return RedirectToAction("Orders", "Order");
        }
        var orderDetails = await _orderService.OrderDetails(id);
        if (orderDetails == null)
        {
            TempData["ToastrMessage"] = "Order Records Not Found";
            TempData["ToastrType"] = "error";
            return RedirectToAction("Orders", "Order");
        }
        return View(orderDetails);
    }

}