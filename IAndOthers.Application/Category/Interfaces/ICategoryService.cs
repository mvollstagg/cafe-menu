using IAndOthers.Core.Data.Result;
using CategoryEntity = IAndOthers.Domain.Entities.Category;

namespace IAndOthers.Application.Category.Interfaces
{
    public interface ICategoryService
    {
        Task<IOResult<CategoryEntity>> GetCategoryAsync(long id);
        Task<IOResult<IList<CategoryEntity>>> GetAllCategoriesAsync();
        Task<IOResultMetadata> AddCategoryAsync(CategoryEntity category);
        Task<IOResultMetadata> UpdateCategoryAsync(CategoryEntity category);
        Task<IOResultMetadata> DeleteCategoryAsync(long id);
    }
}
