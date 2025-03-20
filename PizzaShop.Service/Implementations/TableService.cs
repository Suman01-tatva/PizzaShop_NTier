using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;
    private readonly ISectionRepository _sectionRepository;

    public TableService(
        ITableRepository tableService, ISectionRepository sectionRepository)
    {
        _tableRepository = tableService;
        _sectionRepository = sectionRepository;
    }

    public List<TableViewModel> GetAllTables()
    {
        var sections = _tableRepository.GetAllTablesAsync();
        return sections;
    }

    public void DeleteTable(int id, int userId)
    {
        _tableRepository.DeleteTable(id, userId);
    }

    public bool MultiDeleteTable(int[] tableIds, int userId)
    {
        try
        {
            foreach (var table in tableIds)
            {
                _tableRepository.DeleteTable(table, userId);
            }
            return true;
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool AdddTable(TableViewModel model, int userId)
    {
        bool isTable = _tableRepository.IsTableExist(model.Name, model.SectionId);

        if (isTable == false)
        {
            var newTable = new Table
            {
                Name = model.Name,
                SectionId = model.SectionId,
                Capacity = model.Capacity,
                IsAvailable = model.IsAvailable,
                CreatedBy = userId, // Ensure CreatedBy is set
            };

            return _tableRepository.AddTable(newTable);
        }
        return false;
    }
}
