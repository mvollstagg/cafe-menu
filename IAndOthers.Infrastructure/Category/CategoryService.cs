using IAndOthers.Application.Authentication.Interfaces;
using IAndOthers.Application.Authentication.Models;
using IAndOthers.Application.Category.Interfaces;
using IAndOthers.Core.Configs;
using IAndOthers.Core.Data.Enumeration;
using IAndOthers.Core.Data.Result;
using IAndOthers.Core.Data.Services;
using IAndOthers.Core.IoC;
using IAndOthers.Domain.Entities;
using IAndOthers.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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

        public Task<IOResultMetadata> AddCategoryAsync(Category category, long userId)
        {
            throw new NotImplementedException();
        }

        public Task<IOResultMetadata> UpdateCategoryAsync(Category category, long userId)
        {
            throw new NotImplementedException();
        }

        public Task<IOResultMetadata> DeleteCategoryAsync(long id, long userId)
        {
            throw new NotImplementedException();
        }
    }
}
