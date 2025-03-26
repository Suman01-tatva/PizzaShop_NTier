using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.Constants;


namespace PizzaShop.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderViewModel>> GetAllOrders(
            string searchString, int pageIndex, int pageSize, bool isAsc,
            DateOnly? fromDate, DateOnly? toDate, string sortColumn, int status, string dateRange)
        {
            var orders = await _orderRepository.GetAllOrders(searchString, pageIndex, pageSize, isAsc, fromDate, toDate, sortColumn, status, dateRange);
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

            return filteredOrders;
        }


        public int TotalOrderCount()
        {
            int count = _orderRepository.TotalOrderCount();
            return count;
        }
    }
}