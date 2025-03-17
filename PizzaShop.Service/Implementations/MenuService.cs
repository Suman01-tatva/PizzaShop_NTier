using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PizzaShop.Entity.Data;
using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Service.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IMenuCategoryRepository _menuCategoryRepository;
        private readonly IMenuItemsRepository _menuItemRepository;

        private readonly IUnitRepository _unitRepository;

        public MenuService(IMenuCategoryRepository menuCategoryRepository, IMenuItemsRepository menuItemsRepository, IUnitRepository unitRepository)
        {
            _menuCategoryRepository = menuCategoryRepository ?? throw new ArgumentNullException(nameof(menuCategoryRepository));
            _menuItemRepository = menuItemsRepository ?? throw new ArgumentNullException(nameof(menuItemsRepository));
            _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
        }

        public async Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync()
        {
            return await _menuCategoryRepository.GetAllMenuCategoriesAsync();
        }

        public async Task<ItemTabViewModel> GetItemTabDetails(int categoryId, int pageSize, int pageIndex, string? searchString)
        {
            var categories = await _menuCategoryRepository.GetAllMenuCategoriesAsync();

            var itemList = await _menuItemRepository.GetItemsByCategory(categoryId, pageSize, pageIndex, searchString);
            var filteredItems = itemList.Select(c => new MenuItemViewModel
            {
                Id = c.Id,
                UnitId = c.UnitId,
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                Type = c.Type,
                Rate = c.Rate,
                Quantity = c.Quantity,
                IsAvailable = c.IsAvailable,
                ShortCode = c.ShortCode,
                IsDeleted = c.IsDeleted == null ? false : true,
            }).ToList();

            var totalCountOfItems = _menuItemRepository.GetItemsCountByCId(categoryId);

            var itemTabViewModel = new ItemTabViewModel
            {
                categoryList = categories,
                itemList = filteredItems,
                PageSize = pageSize,
                PageIndex = pageIndex,
                TotalPage = totalCountOfItems,
                // (int)Math.Ceiling(totalCountOfItems / (double)pageSize)
                SearchString = searchString
            };
            return itemTabViewModel;
        }

        // public bool AddNewCategory(string Name, string Description)
        // {
        //     return _menuCategoryRepository.AddNewCategory(Name, Description);
        // }

        public async Task<bool> AddNewCategory(string category, MenuCategoryViewModel model)
        {
            bool isCategory = _menuCategoryRepository.GetCategoryByName(category);

            if (isCategory == false)
            {
                var newCategory = new MenuCategory
                {
                    Name = model.Name,
                    Description = model.Description,
                    // CreatedBy = 1,
                    // CreatedAt = DateTime.Now
                };

                return _menuCategoryRepository.AddNewCategory(newCategory);
            }
            return false;
        }

        public async Task<MenuCategoryViewModel> GetCategoryDetailById(int id)
        {
            var category = await _menuCategoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return null;
            }

            return new MenuCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<bool> EditCategory(MenuCategoryViewModel model, int categoryId)
        {
            var category = await _menuCategoryRepository.GetCategoryByIdAsync(categoryId);

            if (category == null)
            {
                return false;
            }

            category.Name = model.Name;
            category.Description = model.Description;

            return await _menuCategoryRepository.UpdateCategoryBy(category);
        }

        public async Task<List<MenuItemViewModel>> GetItemsByCategory(int categoryId, int pageSize, int pageIndex, string? searchString)
        {
            var Items = await _menuItemRepository.GetItemsByCategory(categoryId, pageSize, pageIndex, searchString);
            var filteredItems = Items.Select(c => new MenuItemViewModel
            {
                Id = c.Id,
                UnitId = c.UnitId,
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                Type = c.Type,
                Rate = c.Rate,
                Quantity = c.Quantity,
                IsAvailable = c.IsAvailable,
                ShortCode = c.ShortCode,
                IsDeleted = c.IsDeleted == null ? false : true,
            }).ToList(); ;
            return filteredItems;
        }

        public bool FindCategoryByName(string name)
        {
            bool isCategory = _menuCategoryRepository.GetCategoryByName(name);
            if (isCategory)
            {
                return true;
            }
            return false;
        }

        public bool SoftDeleteCategory(int id)
        {
            return _menuCategoryRepository.DeleteCategory(id);
        }

        public int GetItemsCountByCId(int cId)
        {
            return _menuItemRepository.GetItemsCountByCId(cId);
        }
        public List<Unit> GetAllUnits()
        {
            return _unitRepository.GetAllUnits();
        }

        public bool AddNewItem(MenuItemViewModel model, int userId)
        {
            string ProfileImagePath = null;
            if (model.ProfileImagePath != null && model.ProfileImagePath.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ProfileImages");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImagePath.FileName);
                var filePath = Path.Combine(folderPath, filename);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImagePath.CopyTo(stream);
                }
                ProfileImagePath = "/ProfileImages/" + filename;
            }
            if (ProfileImagePath != null)
                model.Image = ProfileImagePath;

            // MenuItemModel
            var menuItem = new MenuItem
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                Type = model.Type,
                Rate = model.Rate,
                Quantity = model.Quantity,
                IsAvailable = model.IsAvailable,
                IsDeleted = false,
                UnitId = model.UnitId,
                IsDefaultTax = model.IsDefaultTax,
                TaxPercentage = model.TaxPercentage,
                ShortCode = model.ShortCode,
                Description = model.Description,
                Image = model.Image,
                // Createddate = DateTime.Now,
                CreatedBy = userId
            };

            bool item = _menuItemRepository.AddNewItem(menuItem);
            if (!item) return false;
            return true;
        }

        public bool IsItemExist(string name, int catId)
        {
            return _menuItemRepository.IsItemExist(name, catId);
        }

        [HttpGet]
        public MenuItemViewModel GetMenuItemById(int itemId)
        {
            MenuItem item = _menuItemRepository.GetMenuItemById(itemId);
            var menuItem = new MenuItemViewModel
            {
                Id = item.Id,
                CategoryId = (int)item.CategoryId,
                Name = item.Name,
                Type = item.Type,
                Rate = item.Rate,
                Quantity = item.Quantity,
                IsAvailable = (bool)(item.IsAvailable == null ? false : item.IsAvailable),
                IsDefaultTax = (bool)(item.IsDefaultTax == null ? false : item.IsDefaultTax),
                UnitId = (int)(item.UnitId == null ? 0 : item.UnitId),
                TaxPercentage = item.TaxPercentage,
                ShortCode = item.ShortCode,
                // ModifierGroupIds = item.ModifierGroupId,
                Description = item.Description,
                // ModifierGroups = modifierGroups,
                // Units = units,
                Image = item.Image,
                // Categories = categoryList
            };
            return menuItem;
        }

        public void DeleteMenuItem(int id)
        {
            _menuItemRepository.DeleteMenuItem(id);
        }


        public bool MultiDeleteMenuItem(int[] itemIds)
        {
            try
            {
                foreach (var item in itemIds)
                {
                    _menuItemRepository.DeleteMenuItem(item);
                }
                return true;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}