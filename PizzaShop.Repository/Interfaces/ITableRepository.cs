using PizzaShop.Entity.ViewModels;
using PizzaShop.Entity.Data;

namespace PizzaShop.Repository.Interfaces;

public interface ITableRepository
{
    List<TableViewModel> GetAllTablesAsync();
    public void DeleteTable(int id, int userId);

    public bool AddTable(Table model);

    public bool UpdateTable(Table model);

    public bool IsTableExist(string name, int sectionId);
}
