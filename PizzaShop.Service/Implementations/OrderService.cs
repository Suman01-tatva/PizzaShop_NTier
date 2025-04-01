using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.Constants;
using PizzaShop.Entity.Data;


namespace PizzaShop.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ITaxAndFeesRepository _taxAndFeesRepository;

        public OrderService(IOrderRepository orderRepository, ITableRepository tableRepository, ISectionRepository sectionRepository, ITaxAndFeesRepository taxAndFeesRepository)
        {
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _sectionRepository = sectionRepository;
            _taxAndFeesRepository = taxAndFeesRepository;
        }

        public async Task<(List<OrderViewModel> list, int count)> GetAllOrders(
            string searchString, int pageIndex, int pageSize, bool isAsc,
            DateOnly? fromDate, DateOnly? toDate, string sortColumn, int status, string dateRange)
        {
            var (orders, count) = await _orderRepository.GetAllOrders(searchString, pageIndex, pageSize, isAsc, fromDate, toDate, sortColumn, status, dateRange);
            orders = orders.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

            var tableId = order.TableOrderMappings?.FirstOrDefault()?.TableId ?? 0;
            TableViewModel? table = null;

            if (tableId != 0)
            {
                table = await _tableRepository.GetTableById(tableId);
            }
            Section? section = null;
            if (table!.SectionId != 0 || table.SectionId != null)
            {
                section = _sectionRepository.GetSectionById(table.SectionId);
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

            var Items = order.OrderedItems.Select(item => new OrderItemsViewModel
            {
                ItemName = item.Name,
                Quantity = item.Quantity,
                Price = item.Rate ?? 0,
                TotalAmount = item.TotalAmount,
                // Modifiers = string.Join(", ", item.OrderedItemModifierMappings.Select(m => m.Modifier.Name)),
                Modifiers = item.OrderedItemModifierMappings.Select(m => new OrderItemModifierViewModel
                {
                    Id = m.Id,
                    OrderItemid = m.OrderItemId,
                    Name = m.Modifier.Name,
                    Quantity = m.QuantityOfModifier,
                    Rate = m.RateOfModifier,
                    TotalAmount = m.RateOfModifier * m.QuantityOfModifier
                }).ToList(),
                QuantityOfModifier = item.OrderedItemModifierMappings.FirstOrDefault()?.QuantityOfModifier ?? 0,
                ModifiersPrice = item.OrderedItemModifierMappings.Sum(m => m.RateOfModifier ?? 0),
                TotalModifierAmount = item.TotalModifierAmount ?? 0
            }).ToList();

            var model = new OrderDetailsViewModel
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                InvoiceNo = order.Invoices.FirstOrDefault()!.Id.ToString(),
                OrderNo = order.OrderNo,
                TotalAmount = order.TotalAmount,
                Tax = order.Tax,
                SubTotal = order.SubTotal,
                Discount = order.Discount,
                PaidAmount = order.PaidAmount,
                Notes = order.Notes,
                IsSgstSelected = order.IsSgstSelected,
                CreatedAt = order.CreatedAt,
                CreatedBy = order.CreatedBy,
                ModifiedAt = order.ModifiedAt,
                ModifiedBy = order.ModifiedBy,
                Date = order.OrderDate,
                CustomerName = order.Customer?.Name,
                Status = (OrderConstants.OrderStatusEnum)order.OrderStatus,
                Rating = order.Feedbacks?.FirstOrDefault()?.AvgRating,
                CustomerEmail = order.Customer?.Email,
                CustomerPhone = order.Customer?.Phone,
                TableId = order.TableOrderMappings?.FirstOrDefault()?.TableId ?? 0,
                NoOfPeople = order.TableOrderMappings?.FirstOrDefault()?.NoOfPeople,
                TableName = table!.Name,
                SectionName = section!.Name,
                OrderedItems = Items,
                PaymentMode = (OrderConstants.PaymentModeEnum)order.Invoices.FirstOrDefault()?.Payments.FirstOrDefault()!.PaymentMethod!,
                OrderTaxes = OrderTaxesList!.ToList()
            };
            return model;
        }
    }
}