using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.Constants;
using PizzaShop.Entity.Data;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;


namespace PizzaShop.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ITaxAndFeesRepository _taxAndFeesRepository;
        private readonly IWebHostEnvironment _webHost;
        public OrderService(IOrderRepository orderRepository, ITableRepository tableRepository, ISectionRepository sectionRepository, ITaxAndFeesRepository taxAndFeesRepository, IWebHostEnvironment webHostEnvironment)
        {
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _sectionRepository = sectionRepository;
            _taxAndFeesRepository = taxAndFeesRepository;
            _webHost = webHostEnvironment;
        }

        public async Task<(List<OrderViewModel> list, int count)> GetAllOrders(
            string searchString, int pageIndex, int pageSize, bool isAsc,
            DateOnly? fromDate, DateOnly? toDate, string sortColumn, int status, string dateRange)
        {
            var (orders, count) = await _orderRepository.GetAllOrders(searchString, pageIndex, pageSize, isAsc, fromDate, toDate, sortColumn, status, dateRange);
            var filteredOrders = orders.Select(c => new OrderViewModel
            {
                Id = c.Id,
                CustomerName = c.Customer.Name,
                CustomerId = c.CustomerId,
                CreatedBy = c.CreatedBy,
                Date = c.OrderDate,
                Status = (OrderConstants.OrderStatusEnum)c.OrderStatus!,
                Rating = c.Feedbacks.FirstOrDefault()?.AvgRating,
                PaymentMode = (OrderConstants.PaymentModeEnum)c.Invoices.FirstOrDefault()?.Payments.FirstOrDefault()!.PaymentMethod!,
                TotalAmount = c.TotalAmount,
                PaidAmount = c.PaidAmount,
                Tax = c.Tax,
            }).ToList();

            return (filteredOrders, count);
        }

        public async Task<OrderDetailsViewModel> OrderDetails(int id)
        {
            if (id == null)
            {
                return null;
            }
            var order = await _orderRepository.OrderDetails(id);

            if (order == null)
            {
                throw new Exception("Order not found");
            }

            var taxes = order.OrderTaxMappings?.ToList();
            List<TaxesAndFee> OrderTaxesList = new List<TaxesAndFee>();
            if (taxes != null)
            {
                foreach (var i in taxes)
                {
                    var tax = await _taxAndFeesRepository.GetTaxById(i.TaxId);
                    OrderTaxesList.Add(tax);
                }
            }
            decimal subTotal = 0;
            foreach (var i in order.OrderedItems)
            {
                subTotal += i.TotalAmount;
                foreach (var modifier in i.OrderedItemModifierMappings)
                {
                    subTotal += (modifier.RateOfModifier * modifier.QuantityOfModifier) ?? 0;
                }
            }
            decimal total = subTotal;
            foreach (var i in order.OrderTaxMappings)
            {
                total += i.TaxValue ?? 0;
            }

            var Items = order.OrderedItems.Select(item => new OrderItemsViewModel
            {
                ItemName = item.Name,
                Quantity = item.Quantity,
                Price = item.Rate ?? 0,
                TotalAmount = item.TotalAmount,
                Modifiers = item.OrderedItemModifierMappings.Select(m => new OrderItemModifierViewModel
                {
                    Id = m.Id,
                    OrderItemid = m.OrderItemId,
                    Name = m.Modifier.Name,
                    Quantity = m.QuantityOfModifier,
                    Rate = m.RateOfModifier,
                    TotalAmount = m.RateOfModifier * m.QuantityOfModifier
                }).ToList(),
                // QuantityOfModifier = item.OrderedItemModifierMappings.Select(i => i.QuantityOfModifier).FirstOrDefault() ?? 0,
                ModifiersPrice = item.OrderedItemModifierMappings.Sum(m => m.RateOfModifier ?? 0),
                TotalModifierAmount = item.TotalModifierAmount ?? 0
            }).ToList();
            string invoiceNumber = "#DOM" + Convert.ToDateTime(order.CreatedAt).ToString("yyyyMMdd") + "-" + order.Invoices?.Select(i => i.Id).FirstOrDefault().ToString();
            var model = new OrderDetailsViewModel
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                InvoiceNo = invoiceNumber,
                // OrderNo = order.OrderNo,
                TotalAmount = total,
                Tax = order.Tax,
                SubTotal = subTotal,
                Discount = order.Discount ?? 0,
                PaidAmount = order.PaidAmount,
                CreatedAt = order.CreatedAt,
                CreatedBy = order.CreatedBy,
                ModifiedAt = order.ModifiedAt,
                ModifiedBy = order.ModifiedBy,
                Date = order.OrderDate,
                CustomerName = order.Customer?.Name,
                Status = (OrderConstants.OrderStatusEnum)order.OrderStatus,
                CustomerEmail = order.Customer?.Email,
                CustomerPhone = order.Customer?.Phone,
                NoOfPeople = order.TableOrderMappings?.FirstOrDefault()?.NoOfPeople,
                TableName = order.TableOrderMappings?.Select(t => t.Table.Name).FirstOrDefault(),
                SectionName = order?.TableOrderMappings?.Select(s => s.Table.Section.Name).FirstOrDefault(),
                OrderedItems = Items,
                // PaymentMode = (OrderConstants.PaymentModeEnum)order?.Invoices?.FirstOrDefault()?.Payments.FirstOrDefault()!.PaymentMethod!,
                // PaymentMode = (OrderConstants.PaymentModeEnum)order?.Invoices.Select(i => i.Payments).FirstOrDefault(),
                OrderTaxes = OrderTaxesList!.ToList()
            };
            return model;
        }

        public async Task<FileContentResult> ExportOrdersExcel(string searchString, int pageIndex, int pageSize, bool isAsc, DateOnly? fromDate, DateOnly? toDate, string sortColumn = "", int status = 0, string dateRange = "AllTime")
        {
            var (orders, count) = await _orderRepository.GetAllOrders(searchString, pageIndex, pageSize, isAsc, fromDate, toDate, sortColumn, status, dateRange);
            var filteredOrders = orders.Select(c => new OrderViewModel
            {
                Id = c.Id,
                CustomerName = c.Customer.Name,
                CustomerId = c.CustomerId,
                CreatedBy = c.CreatedBy,
                Date = c.OrderDate,
                Status = (OrderConstants.OrderStatusEnum)c.OrderStatus!,
                Rating = c.Feedbacks.FirstOrDefault()?.AvgRating,
                PaymentMode = (OrderConstants.PaymentModeEnum)c.Invoices.FirstOrDefault()?.Payments.FirstOrDefault()!.PaymentMethod!,
                TotalAmount = c.TotalAmount,
                PaidAmount = c.PaidAmount,
                Tax = c.Tax,
            }).ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("CustomerList");



                worksheet.Cells[9, 1, 9, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[9, 1, 9, 16].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));

                worksheet.Cells[2, 1, 3, 2].Merge = true;
                worksheet.Cells[2, 1, 3, 2].Value = "Total Records";
                worksheet.Cells[2, 1, 3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 3, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[2, 1, 3, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[2, 1, 3, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));
                worksheet.Cells[2, 1, 3, 2].Style.Font.Color.SetColor(System.Drawing.Color.White);

                worksheet.Cells[2, 3, 3, 6].Merge = true;
                worksheet.Cells[2, 3, 3, 6].Value = count;
                worksheet.Cells[2, 3, 3, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 3, 3, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[2, 3, 3, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[2, 3, 3, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                worksheet.Cells[2, 3, 3, 6].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                worksheet.Cells[2, 3, 3, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[2, 8, 3, 9].Merge = true;
                worksheet.Cells[2, 8, 3, 9].Value = "Search Text:";
                worksheet.Cells[2, 8, 3, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 8, 3, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[2, 8, 3, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[2, 8, 3, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));
                worksheet.Cells[2, 8, 3, 9].Style.Font.Color.SetColor(System.Drawing.Color.White);

                worksheet.Cells[2, 10, 3, 13].Merge = true;
                worksheet.Cells[2, 10, 3, 13].Value = searchString;
                worksheet.Cells[2, 10, 3, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 10, 3, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[2, 10, 3, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[2, 10, 3, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                worksheet.Cells[2, 10, 3, 13].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                worksheet.Cells[2, 10, 3, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 1, 6, 2].Merge = true;
                worksheet.Cells[5, 1, 6, 2].Value = "Date:";
                worksheet.Cells[5, 1, 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[5, 1, 6, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[5, 1, 6, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[5, 1, 6, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));
                worksheet.Cells[5, 1, 6, 2].Style.Font.Color.SetColor(System.Drawing.Color.White);

                worksheet.Cells[5, 3, 6, 6].Merge = true;
                worksheet.Cells[5, 3, 6, 6].Value = dateRange;
                worksheet.Cells[5, 3, 6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[5, 3, 6, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[5, 3, 6, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[5, 3, 6, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                worksheet.Cells[5, 3, 6, 6].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                worksheet.Cells[5, 3, 6, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 8, 6, 9].Merge = true;
                worksheet.Cells[5, 8, 6, 9].Value = $"Status:";
                worksheet.Cells[5, 8, 6, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[5, 8, 6, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[5, 8, 6, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[5, 8, 6, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 96, 152));
                worksheet.Cells[5, 8, 6, 9].Style.Font.Color.SetColor(System.Drawing.Color.White);

                worksheet.Cells[5, 10, 6, 13].Merge = true;
                worksheet.Cells[5, 10, 6, 13].Value = (OrderConstants.OrderStatusEnum)status;
                worksheet.Cells[5, 10, 6, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[5, 10, 6, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[5, 10, 6, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[5, 10, 6, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                worksheet.Cells[5, 10, 6, 13].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                worksheet.Cells[5, 10, 6, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                string path = _webHost.WebRootPath;
                string imagePath = Path.Combine(path, "images", "logos", "pizzashop_logo.png");
                if (System.IO.File.Exists(imagePath))
                {
                    using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                    {
                        var ExcelImage = worksheet.Drawings.AddPicture("logo", stream);
                        ExcelImage.SetPosition(1, 0, 14, 0);
                        ExcelImage.SetSize(150, 100);
                    }
                }



                // worksheet.Cells[9, 1, 9, 1].Merge = true;
                worksheet.Cells[9, 1].Value = "Order ID";
                worksheet.Cells[9, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[9, 2, 9, 4].Merge = true;
                worksheet.Cells[9, 2, 9, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[9, 2, 9, 4].Value = "Date";

                worksheet.Cells[9, 5, 9, 7].Merge = true;
                worksheet.Cells[9, 5, 9, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[9, 5, 9, 7].Value = "Customer";

                worksheet.Cells[9, 8, 9, 10].Merge = true;
                worksheet.Cells[9, 8, 9, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[9, 8, 9, 10].Value = "Status";

                worksheet.Cells[9, 11, 9, 12].Merge = true;
                worksheet.Cells[9, 11, 9, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[9, 11, 9, 12].Value = "Payment Mode";

                worksheet.Cells[9, 13, 9, 14].Merge = true;
                worksheet.Cells[9, 13, 9, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[9, 13, 9, 14].Value = "Rating";

                worksheet.Cells[9, 15, 9, 16].Merge = true;
                worksheet.Cells[9, 15, 9, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[9, 15, 9, 16].Value = "Total Amount";

                int row = 10;
                if (filteredOrders.Count > 0)
                {
                    foreach (var o in filteredOrders)
                    {
                        worksheet.Cells[row, 1, row, 1].Value = o.Id;
                        worksheet.Cells[row, 1, row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        worksheet.Cells[row, 2, row, 4].Merge = true;
                        worksheet.Cells[row, 2, row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 2, row, 4].Value = o.Date;

                        worksheet.Cells[row, 5, row, 7].Merge = true;
                        worksheet.Cells[row, 5, row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 5, row, 7].Value = o.CustomerName;

                        worksheet.Cells[row, 8, row, 10].Merge = true;
                        worksheet.Cells[row, 8, row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 8, row, 10].Value = (OrderConstants.OrderStatusEnum)(o.Status);

                        worksheet.Cells[row, 11, row, 12].Merge = true;
                        worksheet.Cells[row, 11, row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 11, row, 12].Value = o.PaymentMode;


                        worksheet.Cells[row, 13, row, 14].Merge = true;
                        worksheet.Cells[row, 13, row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 13, row, 14].Value = o.Rating;

                        worksheet.Cells[row, 15, row, 16].Merge = true;
                        worksheet.Cells[row, 15, row, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 15, row, 16].Value = o.TotalAmount;
                        row++;
                    }
                }
                else
                {
                    worksheet.Cells[10, 1, 10, 16].Merge = true;
                    worksheet.Cells[10, 1, 10, 16].Value = "No Record Found";
                    worksheet.Cells[10, 1, 10, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                }

                // Auto-fit columns for better readability
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                // Convert the package to a byte array
                var fileBytes = package.GetAsByteArray();
                return new FileContentResult(
                    fileBytes, //Excel File data in Byte Array
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" //Excel Sheet Mime Type
                )
                {
                    FileDownloadName = "Orders.xlsx" //File Name
                };
            }
        }
    }
}