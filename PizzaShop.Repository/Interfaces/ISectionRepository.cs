using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface ISectionRepository
{
    List<SectionViewModel> GetAllSectionsAsync();

    Section GetSectionById(int sectionId);

}
