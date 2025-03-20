using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class TableRepository : ITableRepository
{
    private readonly PizzashopContext _context;

    public TableRepository(PizzashopContext context)
    {
        _context = context;
    }

    public List<TableViewModel> GetAllTablesAsync()
    {
        var tables = _context.Tables.Where(t => t.IsDeleted == false)
        .Select(t => new TableViewModel
        {
            Id = t.Id,
            SectionId = t.SectionId,
            Name = t.Name,
            Capacity = t.Capacity,
            IsAvailable = t.IsAvailable,
            IsDeleted = t.IsDeleted,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = t.CreatedBy,
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = t.ModifiedBy
        })
        .OrderBy(t => t.Name).ToList();
        return tables;
    }

    public void DeleteTable(int id, int userId)
    {
        var table = _context.Tables.FirstOrDefault(i => i.Id == id);
        table!.IsDeleted = true;
        table.ModifiedBy = userId;
        _context.SaveChanges();
    }

    public bool AddTable(Table model)
    {
        try
        {
            _context.Tables.Add(model);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
            }
            return false;
        }
    }

    public bool UpdateTable(Table model)
    {
        try
        {
            _context.Tables.Update(model);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public bool IsTableExist(string name, int sectionId)
    {
        name = name.Trim().ToLower();
        var table = _context.Tables.FirstOrDefault(t => t.Name.ToLower() == name && t.SectionId == sectionId);
        if (table != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}