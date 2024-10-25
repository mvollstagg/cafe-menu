using IAndOthers.Core.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAndOthers.Domain.Entities
{
    public class AuthRefreshToken : IOEntityBase
    {
        public long ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
    }

}
