using IAndOthers.Core.Data.Result;
using PropertyEntity = IAndOthers.Domain.Entities.Property;

namespace IAndOthers.Application.Property.Interfaces
{
    public interface IPropertyService
    {
        Task<IOResult<PropertyEntity>> GetPropertyAsync(long id);
        Task<IOResult<IList<PropertyEntity>>> GetAllCategoriesAsync();
        Task<IOResultMetadata> AddPropertyAsync(PropertyEntity category);
        Task<IOResultMetadata> UpdatePropertyAsync(PropertyEntity category);
        Task<IOResultMetadata> DeletePropertyAsync(long id);
    }
}
