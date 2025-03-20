using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ITableService
{
    public List<TableViewModel> GetAllTables();
    public void DeleteTable(int id, int userId);
    public bool MultiDeleteTable(int[] tableIds, int userId);
    public bool AdddTable(TableViewModel model, int userId);

}
