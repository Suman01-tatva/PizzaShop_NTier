using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PizzaShop.Entity.Data;

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
                    CreatedBy = 1,
                    CreatedAt = DateTime.Now
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

    }
}