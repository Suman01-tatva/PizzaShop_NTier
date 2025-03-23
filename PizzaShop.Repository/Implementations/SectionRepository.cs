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
    public Section GetSectionById(int sectionId)
    {
        return _context.Sections.FirstOrDefault(s => s.Id == sectionId);
    }

    public async Task<bool> AddSectionAsync(Section section)
    {
        try
        {
            bool isNameUnique = !await _context.Sections
           .AnyAsync(s => s.Name.ToLower() == section.Name.ToLower() && s.IsDeleted == false);
            if (!isNameUnique)
                throw new Exception("Section name must be unique.");

            await _context.Sections.AddAsync(section);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateSectionAsync(Section section)
    {
        bool isNameUnique = !await _context.Sections
            .AnyAsync(s => s.Name.ToLower() == section.Name.ToLower() && s.Id != section.Id && s.IsDeleted == false);
        if (!isNameUnique)
            throw new Exception("Section name must be unique.");

        _context.Sections.Update(section);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task DeleteSectionAsync(int id, bool softDelete)
    {
        var section = GetSectionById(id);
        if (section != null)
        {
            if (softDelete)
            {
                section.IsDeleted = true;
                _context.Sections.Update(section);

                var tables = await _context.Tables.Where(t => t.SectionId == id && t.IsDeleted == false).ToListAsync();
                foreach (var table in tables)
                {
                    table.IsDeleted = true;
                    _context.Tables.Update(table);
                }
            }
            else
            {
                _context.Sections.Remove(section);
            }

            await _context.SaveChangesAsync();
        }
    }
}
