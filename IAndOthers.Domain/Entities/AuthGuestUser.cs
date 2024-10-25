using IAndOthers.Core.Data.Entity;

namespace IAndOthers.Domain.Entities
{
    public class AuthGuestUser : IOEntityBase
    {
        public DateTime CreationDateUtc { get; set; }
    }
}
