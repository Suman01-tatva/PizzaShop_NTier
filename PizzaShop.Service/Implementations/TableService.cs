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

    public bool DeleteTable(int id, int userId)
    {
        var isDelete = _tableRepository.DeleteTable(id, userId);
        return isDelete;
    }

    public bool MultiDeleteTable(int[] tableIds, int userId)
    {
        try
        {
            var occupied = false;
            foreach (var table in tableIds)
            {
                var isDelete = _tableRepository.DeleteTable(table, userId);
                if (isDelete == false)
                {
                    occupied = true;
                    break;
                }
            }
            if (occupied == true)
                return false;
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
        bool isTable = _tableRepository.IsTableExist(model.Name, model.SectionId, model.Id);

        if (isTable == false)
        {
            var newTable = new Table
            {
                Name = model.Name,
                SectionId = model.SectionId,
                Capacity = model.Capacity,
                IsAvailable = model.IsAvailable,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            return _tableRepository.AddTable(newTable);
        }
        return false;
    }

    public async Task<TableViewModel> GetTableById(int id)
    {
        return await _tableRepository.GetTableById(id);
    }

    public bool UpdateTable(TableViewModel model, int userId)
    {
        bool isTable = _tableRepository.IsTableExist(model.Name, model.SectionId, model.Id);

        if (isTable == false)
        {
            var newTable = new Table
            {
                Name = model.Name,
                SectionId = model.SectionId,
                Capacity = model.Capacity,
                IsAvailable = model.IsAvailable,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            return _tableRepository.UpdateTable(newTable);
        }
        return false;
    }

}
