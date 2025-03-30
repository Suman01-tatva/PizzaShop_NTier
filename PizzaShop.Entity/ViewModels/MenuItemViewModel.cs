namespace PizzaShop.Entity.ViewModels;

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using PizzaShop.Entity.Data;


public partial class MenuItemViewModel
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public int UnitId { get; set; }

    public string Name { get; set; } = null!;

    public bool Type { get; set; }

    public decimal Rate { get; set; }

    public int? Quantity { get; set; }

    public bool IsAvailable { get; set; }

    public string? Image { get; set; }

    public IFormFile? ProfileImagePath { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? TaxPercentage { get; set; }

    public bool? IsFavourite { get; set; }

    public string? ShortCode { get; set; }

    public bool IsDefaultTax { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public List<ItemModifierViewModel>? ItemModifiersList { get; set; }
    public List<Unit>? Units { get; set; }
    public List<MenuModifierGroupViewModel>? ModifierGroups { get; set; }
    public List<MenuCategoryViewModel>? Categories { get; set; }
}
