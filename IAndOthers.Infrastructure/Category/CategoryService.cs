using IAndOthers.Application.Category.Interfaces;
using IAndOthers.Core.Data.Enumeration;
using IAndOthers.Core.Data.Result;
using IAndOthers.Core.Data.Services;
using IAndOthers.Core.IoC;
using IAndOthers.Domain.Entities;
using IAndOthers.Infrastructure.Data;

namespace IAndOthers.Infrastructure.Authentication
{
    public class CategoryService : ICategoryService, IIODependencyTransient<ICategoryService>
    {
        private readonly IIORepository<Category, ApplicationDbContext> _categoryRepository;

        public CategoryService(IIORepository<Category, ApplicationDbContext> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IOResult<Category>> GetCategoryAsync(long id)
        {
            var category = await _categoryRepository.GetAsync(c => c.Id == id);

            if (category == null)
            {
                return new IOResult<Category>(IOResultStatusEnum.Error, "Category not found.");
            }

            return category;
        }

        public async Task<IOResult<IList<Category>>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetListAsync();
            return categories;
        }

        public async Task<IOResultMetadata> AddCategoryAsync(Category category)
        {
            if (category == null)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, "Category cannot be null.");
            }

            var result = await _categoryRepository.InsertAsync(category);
            return result;
        }

        public async Task<IOResultMetadata> UpdateCategoryAsync(Category category)
        {
            if (category == null)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, "Category cannot be null.");
            }

            var existingCategory = await _categoryRepository.GetAsync(c => c.Id == category.Id);
            if (existingCategory == null)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, "Category not found.");
            }

            var result = await _categoryRepository.UpdateAsync(category);
            return result;
        }

        public async Task<IOResultMetadata> DeleteCategoryAsync(long id)
        {
            var existingCategory = await _categoryRepository.GetAsync(c => c.Id == id);
            if (existingCategory == null)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, "Category not found.");
            }

            var result = await _categoryRepository.DeleteAsync(id);
            return result;
        }
    }
}
