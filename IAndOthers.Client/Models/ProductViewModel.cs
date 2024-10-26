namespace IAndOthers.Client.Models
{
    public class ProductViewModel
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public string ProductName { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PriceInEUR { get; set; }
        public decimal PriceInGBP { get; set; }
        public decimal PriceInUSD { get; set; }

        public List<KeyValuePair<string, string>> Properties { get; set; } = new List<KeyValuePair<string, string>>();
    }
}
