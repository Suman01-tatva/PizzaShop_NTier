namespace PizzaShop.Entity.ViewModels;

public class TableSectionViewModel
{
    public List<TableViewModel> Tables { get; set; } = new List<TableViewModel>();
    public List<SectionViewModel> Sections { get; set; } = new List<SectionViewModel>();
}
