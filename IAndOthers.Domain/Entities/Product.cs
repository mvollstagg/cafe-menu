using IAndOthers.Core.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAndOthers.Domain.Entities
{
    public class Product : IOEntityDeletable
    {
        public long CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public string ProductName { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
        public string ImagePath { get; set; }

        public virtual ICollection<ProductProperty> ProductProperties { get; set; }
    }
}
