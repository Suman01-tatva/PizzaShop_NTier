using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class SectionRepository : ISectionRepository
{
    private readonly PizzashopContext _context;

    public SectionRepository(PizzashopContext context)
    {
        _context = context;
    }

    public List<SectionViewModel> GetAllSectionsAsync()
    {
        var sections = _context.Sections.Where(s => s.IsDeleted == false)
        .Select(s => new SectionViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            IsDeleted = s.IsDeleted,
            CreatedBy = s.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            ModifiedBy = s.ModifiedBy,
            ModifiedAt = DateTime.UtcNow
        })
        .OrderBy(s => s.Name).ToList();
        return sections;
    }

}
