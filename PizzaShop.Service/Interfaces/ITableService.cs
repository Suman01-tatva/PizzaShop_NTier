using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ITableService
{
    public List<TableViewModel> GetAllTables();
}
