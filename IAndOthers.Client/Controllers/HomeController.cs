using IAndOthers.Client.Models;
using IAndOthers.Core.Data.Services;
using IAndOthers.Domain.Entities;
using IAndOthers.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IAndOthers.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIORepository<Product, ApplicationDbContext> _productRepository;
        private readonly IIORepository<ExchangeRate, ApplicationDbContext> _exchangeRateRepository;

        public HomeController(IIORepository<Product, ApplicationDbContext> productRepository, 
                            IIORepository<ExchangeRate, ApplicationDbContext> exchangeRateRepository)
        {
            _productRepository = productRepository;
            _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch the latest exchange rates for EUR, GBP, and USD
            var exchangeRates = await _exchangeRateRepository
                .Table
                .Where(rate => rate.CurrencyCode == "EUR" || rate.CurrencyCode == "GBP" || rate.CurrencyCode == "USD")
                .OrderByDescending(rate => rate.DateUtc)
                .GroupBy(rate => rate.CurrencyCode)
                .Select(g => g.FirstOrDefault())
                .ToDictionaryAsync(rate => rate.CurrencyCode, rate => rate.SellingPrice);

            // Default to 1 if any currency rate is missing
            decimal eurRate = exchangeRates.ContainsKey("EUR") ? exchangeRates["EUR"] : 1;
            decimal gbpRate = exchangeRates.ContainsKey("GBP") ? exchangeRates["GBP"] : 1;
            decimal usdRate = exchangeRates.ContainsKey("USD") ? exchangeRates["USD"] : 1;

            // Fetch products and calculate prices in each currency, including properties
            var products = await _productRepository
                .Table
                .Where(p => !p.Deleted)
                .Include(p => p.ProductProperties)
                .ThenInclude(pp => pp.Property)
                .ToListAsync();

            var productViewModels = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                ImageUrl = "http://localhost:5292/" + p.ImagePath,
                ProductName = p.ProductName,
                BasePrice = p.Price,
                PriceInEUR = p.Price * eurRate,
                PriceInGBP = p.Price * gbpRate,
                PriceInUSD = p.Price * usdRate,
                Properties = p.ProductProperties
                    .Select(pp => new KeyValuePair<string, string>(pp.Property.Key, pp.Property.Value))
                    .ToList()
            }).ToList();

            return View(productViewModels);
        }
    }
}
