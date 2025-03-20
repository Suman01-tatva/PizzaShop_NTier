using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class SectionService : ISectionService
{
    private readonly ISectionRepository _sectionRepository;

    public SectionService(
        ISectionRepository sectionRepository)
    {
        _sectionRepository = sectionRepository;
    }

    public List<SectionViewModel> GetAllSections()
    {
        var sections = _sectionRepository.GetAllSectionsAsync();
        return sections;
    }

}
