using IAndOthers.Core.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace IAndOthers.Domain.Entities
{
    public class ExchangeRate : IOEntityBase
    {
        public string CurrencyCode { get; set; }
        public DateTime DateUtc { get; set; }
        [Precision(18, 2)]
        public decimal SellingPrice { get; set; }
        public string XmlData { get; set; }
    }
}
