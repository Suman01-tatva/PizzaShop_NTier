using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface ICustomerRepository
{
    public Task<(List<CustomerViewModel>, int count)> GetCustomerList(string searchString, string sortOrder, int pageIndex, int pageSize, string dateRange, DateOnly? fromDate, DateOnly? toDate);
}