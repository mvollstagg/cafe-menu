using IAndOthers.Application.Property.Models;
using IAndOthers.Core.Data.Services;
using IAndOthers.Domain.Entities;
using IAndOthers.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IAndOthers.Admin.Controllers
{
    public class PropertyController : AdminController<Property, PropertyCreateModel, PropertyEditModel, PropertyViewModel, ApplicationDbContext>
    {
        public PropertyController(IIORepository<Property, ApplicationDbContext> repository)
            : base(repository)
        {
        }

        public override async Task<IActionResult> Index()
        {
            var result = await _repository
                                .Table
                                .Where(p => !p.Deleted)
                                .Select(p => new PropertyViewModel
                                {
                                    Id = p.Id,
                                    Key = p.Key,
                                    Value = p.Value,
                                    Deleted = p.Deleted
                                })
                                .ToListAsync();

            ViewData["Title"] = typeof(Property).Name;
            return View("Index", result);
        }
    }
}
