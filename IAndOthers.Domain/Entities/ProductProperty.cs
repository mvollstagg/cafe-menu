using IAndOthers.Core.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAndOthers.Domain.Entities
{
    public class ProductProperty : IOEntityBase
    {
        public long ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        
        public long PropertyId { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; }
    }
}
