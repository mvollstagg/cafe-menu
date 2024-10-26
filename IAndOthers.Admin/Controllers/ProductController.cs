using IAndOthers.Application.Media.Interfaces;
using IAndOthers.Application.Product.Models;
using IAndOthers.Core.Data.Enumeration;
using IAndOthers.Core.Data.Services;
using IAndOthers.Domain.Entities;
using IAndOthers.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Omu.ValueInjecter;

namespace IAndOthers.Admin.Controllers
{
    public class ProductController : AdminController<Product, ProductCreateModel, ProductEditModel, ProductViewModel, ApplicationDbContext>
    {
        private readonly IMediaService _mediaService;
        private readonly IIORepository<Category, ApplicationDbContext> _categoryRepository;
        private readonly IIORepository<Property, ApplicationDbContext> _propertyRepository;
        private readonly IIORepository<ProductProperty, ApplicationDbContext> _productPropertyRepository;
        public ProductController(
            IIORepository<Product, ApplicationDbContext> repository,
            IMediaService mediaService,
            IIORepository<Category, ApplicationDbContext> categoryRepository,
            IIORepository<Property, ApplicationDbContext> propertyRepository,
            IIORepository<ProductProperty, ApplicationDbContext> productPropertyRepository)
            : base(repository)
        {
            _mediaService = mediaService;
            _categoryRepository = categoryRepository;
            _propertyRepository = propertyRepository;
            _productPropertyRepository = productPropertyRepository;
        }

        public override async Task<IActionResult> Index()
        {
            var result = await _repository
                .Table
                .Where(p => !p.Deleted)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    ImagePath = p.ImagePath,
                    PropertyNames = p.ProductProperties
                        .Select(pp => pp.Property.Value)
                        .ToList()
                })
                .ToListAsync();

            ViewData["Title"] = typeof(Product).Name;
            return View("Index", result);
        }

        public override async Task<IActionResult> Edit(long id)
        {
            var updateModel = await _repository
                            .Table
                            .Where(p => p.Id == id)
                            .Select(p => new ProductEditModel
                            {
                                Id = p.Id,
                                ProductName = p.ProductName,
                                Price = p.Price,
                                CategoryId = p.CategoryId,
                                ImagePath = p.ImagePath,
                                SelectedProductPropertyIds = p.ProductProperties
                                    .Select(pp => pp.PropertyId)
                                    .ToList()
                            })
                            .FirstOrDefaultAsync();

            await SetListViewData(updateModel);
            ViewData["Title"] = typeof(Product).Name;

            return View("Edit", updateModel);
        }

        public override async Task<Product> BeforeCreate(Product entity, ProductCreateModel model)
        {
            if (model.Media != null)
            {
                entity.ImagePath = await _mediaService.UploadAsync(model.Media);
            }
            if (model.SelectedProductPropertyIds != null)
            {
                entity.ProductProperties = model.SelectedProductPropertyIds
                    .Select(propertyId => new ProductProperty
                    {
                        PropertyId = propertyId
                    })
                    .ToList();
            }

            return entity;
        }

        public override async Task<Product> BeforeEdit(Product entity, ProductEditModel model)
        {
            if (model.Media != null)
            {
                entity.ImagePath = await _mediaService.UploadAsync(model.Media);
            }
            if (model.SelectedProductPropertyIds != null)
            {
                // Clear existing properties
                var existingProperties = await _productPropertyRepository.GetListAsync(pp => pp.ProductId == entity.Id);
                await _productPropertyRepository.DeleteAsync(existingProperties.Data);

                entity.ProductProperties = model.SelectedProductPropertyIds
                    .Select(propertyId => new ProductProperty
                    {
                        PropertyId = propertyId
                    })
                    .ToList();
            }

            return entity;
        }

        public override async Task SetListViewData(object? model)
        {
            // Set categories dropdown
            var categories = await _categoryRepository.GetListAsync(c => !c.Deleted);
            var categoryList = categories.Data
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                })
                .ToList();
            ViewData["CategoryList"] = categoryList;

            // Set properties checklist
            var properties = await _propertyRepository.GetListAsync(p => !p.Deleted);
            var propertyList = properties.Data
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Value
                })
                .ToList();
            ViewData["ProductPropertiesList"] = propertyList;
        }
    }
}
