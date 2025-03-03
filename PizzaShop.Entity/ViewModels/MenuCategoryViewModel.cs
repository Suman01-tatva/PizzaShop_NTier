using System;
using System.Collections.Generic;
using PizzaShop.Entity.Data;

namespace PizzaShop.Entity.ViewModels;

public partial class MenuCategoryViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public virtual User? ModifiedByNavigation { get; set; }

    public List<MenuItemViewModel> ItemList { get; set; }
}