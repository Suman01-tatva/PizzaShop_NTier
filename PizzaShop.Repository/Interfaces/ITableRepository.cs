using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface ITableRepository
{
    List<TableViewModel> GetAllTablesAsync();
}
