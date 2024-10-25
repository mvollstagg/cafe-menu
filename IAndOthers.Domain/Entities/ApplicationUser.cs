using IAndOthers.Core.Data.Entity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IAndOthers.Domain.Entities
{
    public class ApplicationUser : IdentityUser<long>, IIOEntity
    {
        [MaxLength(250)]
        public string FirstName { get; set; }
        [MaxLength(250)]
        public string LastName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
