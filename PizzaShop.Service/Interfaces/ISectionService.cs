using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface ISectionService
{
    public List<SectionViewModel> GetAllSections();
}
