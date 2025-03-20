using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class TableSectionController : Controller
{
    private readonly ITableService _tableService;
    private readonly ISectionService _sectionService;

    public TableSectionController(ITableService tableService, ISectionService sectionService)
    {
        _tableService = tableService;
        _sectionService = sectionService;
    }
    [HttpGet]
    public IActionResult TableSection()
    {
        var sections = _sectionService.GetAllSections();
        var tables = _tableService.GetAllTables();

        var tebleSection = new TableSectionViewModel
        {
            Tables = tables,
            Sections = sections
        };
        return View(tebleSection);
    }

}
