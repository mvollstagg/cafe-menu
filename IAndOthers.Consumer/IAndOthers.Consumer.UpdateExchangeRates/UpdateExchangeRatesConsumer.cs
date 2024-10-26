using IAndOthers.Infrastructure.Data;
using IAndOthers.Core.Consumer;
using MassTransit;
using IAndOthers.Core.Data.Services;
using IAndOthers.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using IAndOthers.Application.Job;
using IAndOthers.Core.IoC;
using System.Globalization;
using System.Net;
using System.Xml;

namespace IAndOthers.Consumer.UpdateExchangeRates
{
    public class UpdateExchangeRatesConsumer : IConsumer<UpdateExchangeRatesMessage>, IIOConsumer
    {
        private readonly IIORepository<ExchangeRate, ApplicationDbContext> _repository;

        public string QueueName => "update_exchange_rates_queue";

        public UpdateExchangeRatesConsumer()
        {
            _repository = IODependencyResolver.Resolve<IIORepository<ExchangeRate, ApplicationDbContext>>();
        }

        public async Task Consume(ConsumeContext<UpdateExchangeRatesMessage> context)
        {
            try
            {
                var sourceUrl = "https://www.tcmb.gov.tr/kurlar/today.xml";
                decimal banknoteSellingPrice = 0.00M;

                HttpStatusCode exchangeRateHttpResponseStatus;
                using (var client = new HttpClient())
                {
                    exchangeRateHttpResponseStatus = client.GetAsync(sourceUrl).Result.StatusCode;
                }

                if (exchangeRateHttpResponseStatus != HttpStatusCode.NotFound)
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(sourceUrl);

                    var xmlDate = DateTime.ParseExact(xmlDoc.SelectSingleNode($"Tarih_Date/@Date").InnerXml, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    var exchangeRateDate = Convert.ToDateTime(xmlDate);

                    var currencies = new string[] { "USD", "EUR", "GBP" };
                    var exchangeRates = new List<ExchangeRate>();
                    foreach (var currency in currencies)
                    {
                        banknoteSellingPrice = Convert.ToDecimal(xmlDoc.SelectSingleNode($"Tarih_Date/Currency[@Kod='{currency}']/BanknoteSelling").InnerXml);

                        if (banknoteSellingPrice > 0.00M)
                        {
                            ExchangeRate newExchangeRate = new ExchangeRate
                            {
                                DateUtc = exchangeRateDate,
                                CurrencyCode = currency,
                                SellingPrice = banknoteSellingPrice,
                                XmlData = xmlDoc.InnerXml
                            };

                            exchangeRates.Add(newExchangeRate);
                        }
                    }

                    await _repository.InsertAsync(exchangeRates);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.UtcNow}] - An error occurred: {ex.Message}");
            }
        }
    }
}
