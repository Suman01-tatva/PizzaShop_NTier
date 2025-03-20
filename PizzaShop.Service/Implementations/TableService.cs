using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;

    public TableService(
        ITableRepository tableService)
    {
        _tableRepository = tableService;
    }

    public List<TableViewModel> GetAllTables()
    {
        var sections = _tableRepository.GetAllTablesAsync();
        return sections;
    }
}
