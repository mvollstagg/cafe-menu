using IAndOthers.Core.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAndOthers.Domain.Entities
{
    public class Category : IOEntityDeletable
    {
        public long? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        public virtual Category ParentCategory { get; set; }

        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
