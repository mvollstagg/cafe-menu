using IAndOthers.Application.Category.Models;
using IAndOthers.Core.Data.Services;
using IAndOthers.Domain.Entities;
using IAndOthers.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IAndOthers.Admin.Controllers
{
    public class CategoryController : AdminController<Category, CategoryCreateModel, CategoryEditModel, CategoryViewModel, ApplicationDbContext>
    {
        public CategoryController(IIORepository<Category, ApplicationDbContext> repository)
            : base(repository)
        {
        }

        public override async Task<IActionResult> Index()
        {
            var result = await _repository
                                .Table
                                .Where(c => !c.Deleted)
                                .Select(c => new CategoryViewModel
                                {
                                    Id = c.Id,
                                    CategoryName = c.CategoryName,
                                    ParentCategoryId = c.ParentCategoryId,
                                    ParentCategoryName = c.ParentCategory.CategoryName,
                                    Deleted = c.Deleted
                                })
                                .ToListAsync();

            return View("Index", result);
        }

        public override async Task SetListViewData(object? model)
        {
            long? excludeId = null;
            if (model != null)
            {
                excludeId = (model as CategoryEditModel)?.Id;
            }

            var categories = await _repository.GetListAsync();
            var selectList = categories.Data
                .Where(c => c.Id != excludeId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                })
                .ToList();

            ViewData["CategoryList"] = selectList;
        }
    }
}
