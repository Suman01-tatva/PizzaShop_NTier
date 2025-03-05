// using PizzaShop.Service.Interfaces;
// using PizzaShop.Entity.ViewModels;
// using PizzaShop.Repository.Interfaces;
// using PizzaShop.Entity.Data;

// namespace PizzaShop.Service.Implementations;

// public class MenuService : IMenuService
// {
//     private readonly IMenuCategoryRepository _menuCategoryRepository;
//     private readonly IMenuItemsRepository _menuItemRepository;

//     public MenuService(IMenuCategoryRepository menuCategoryRepository, IMenuItemsRepository menuItemsRepository)
//     {
//         _menuCategoryRepository = menuCategoryRepository;
//         _menuItemRepository = menuItemsRepository;
//     }

//     public async Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync()
//     {
//         return await _menuCategoryRepository.GetAllMenuCategoriesAsync();
//     }

//     public bool AddNewCategory(MenuCategoryViewModel model)
//     {
//         bool result = _menuCategoryRepository.AddNewCategory(model);
//         return result;
//     }

//     public async Task<MenuCategoryViewModel> GetCategoryDetailById(int id)
//     {
//         var category = await _menuCategoryRepository.GetCategoryByIdAsync(id);
//         if (category == null)
//         {
//             return null;
//         }

//         return new MenuCategoryViewModel
//         {
//             Id = category.Id,
//             Name = category.Name,
//             Description = category.Description
//         };
//     }

//     public async Task<bool> EditCategory(MenuCategoryViewModel model, int categoryId)
//     {
//         var category = await _menuCategoryRepository.GetCategoryByIdAsync(categoryId);

//         if (category == null)
//         {
//             return false;
//         }

//         category.Name = model.Name;
//         category.Description = model.Description;

//         return await _menuCategoryRepository.UpdateCategoryBy(category);
//     }

//     public async Task<List<MenuItemViewModel>> GetItemsByCategory(int categoryId)
//     {
//         return await _menuItemRepository.GetItemsByCategory(categoryId);
//     }
// }
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaShop.Service.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IMenuCategoryRepository _menuCategoryRepository;
        private readonly IMenuItemsRepository _menuItemRepository;

        public MenuService(IMenuCategoryRepository menuCategoryRepository, IMenuItemsRepository menuItemsRepository)
        {
            _menuCategoryRepository = menuCategoryRepository ?? throw new ArgumentNullException(nameof(menuCategoryRepository));
            _menuItemRepository = menuItemsRepository ?? throw new ArgumentNullException(nameof(menuItemsRepository));
        }

        public async Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync()
        {
            return await _menuCategoryRepository.GetAllMenuCategoriesAsync();
        }

        public async Task<ItemTabViewModel> GetItemTabDetails(int categoryId)
        {
            var categories = await _menuCategoryRepository.GetAllMenuCategoriesAsync();

            List<MenuItemViewModel> itemList;
            // if (categoryId <= 0)
            // {
            //     itemList = await _menuItemRepository.GetItemsByCategory(1);
            // }
            // else
            // {
            //     itemList = await _menuItemRepository.GetItemsByCategory(categoryId);
            // }

            itemList = await _menuItemRepository.GetItemsByCategory(categoryId);

            var itemTabViewModel = new ItemTabViewModel
            {
                categoryList = categories,
                itemList = itemList
            };
            return itemTabViewModel;
        }

        public bool AddNewCategory(string Name, string Description)
        {
            return _menuCategoryRepository.AddNewCategory(Name, Description);
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

        public async Task<List<MenuItemViewModel>> GetItemsByCategory(int categoryId)
        {
            var Items = await _menuItemRepository.GetItemsByCategory(categoryId);
            return Items;
        }

        public bool SoftDeleteCategory(int id)
        {
            return _menuCategoryRepository.DeleteCategory(id);
        }
    }
}