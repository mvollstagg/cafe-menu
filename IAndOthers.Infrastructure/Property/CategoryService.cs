using IAndOthers.Application.Property.Interfaces;
using IAndOthers.Core.Data.Enumeration;
using IAndOthers.Core.Data.Result;
using IAndOthers.Core.Data.Services;
using IAndOthers.Core.IoC;
using IAndOthers.Domain.Entities;
using IAndOthers.Infrastructure.Data;

namespace IAndOthers.Infrastructure.Authentication
{
    public class PropertyService : IPropertyService, IIODependencyTransient<IPropertyService>
    {
        private readonly IIORepository<Property, ApplicationDbContext> _propertyRepository;

        public PropertyService(IIORepository<Property, ApplicationDbContext> propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<IOResult<Property>> GetPropertyAsync(long id)
        {
            var property = await _propertyRepository.GetAsync(c => c.Id == id);

            if (property == null)
            {
                return new IOResult<Property>(IOResultStatusEnum.Error, "Property not found.");
            }

            return property;
        }

        public async Task<IOResult<IList<Property>>> GetAllCategoriesAsync()
        {
            var categories = await _propertyRepository.GetListAsync();
            return categories;
        }

        public async Task<IOResultMetadata> AddPropertyAsync(Property property)
        {
            if (property == null)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, "Property cannot be null.");
            }

            var result = await _propertyRepository.InsertAsync(property);
            return result;
        }

        public async Task<IOResultMetadata> UpdatePropertyAsync(Property property)
        {
            if (property == null)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, "Property cannot be null.");
            }

            var existingProperty = await _propertyRepository.GetAsync(c => c.Id == property.Id);
            if (existingProperty == null)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, "Property not found.");
            }

            var result = await _propertyRepository.UpdateAsync(property);
            return result;
        }

        public async Task<IOResultMetadata> DeletePropertyAsync(long id)
        {
            var existingProperty = await _propertyRepository.GetAsync(c => c.Id == id);
            if (existingProperty == null)
            {
                return new IOResultMetadata(IOResultStatusEnum.Error, "Property not found.");
            }

            var result = await _propertyRepository.DeleteAsync(id);
            return result;
        }
    }
}
