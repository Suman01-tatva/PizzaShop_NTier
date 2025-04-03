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
    }
}