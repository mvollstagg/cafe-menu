using IAndOthers.Core.Data.Entity;

namespace IAndOthers.Domain.Entities
{
    public class Property : IOEntityDeletable
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public virtual ICollection<ProductProperty> ProductProperties { get; set; }
    }
}
