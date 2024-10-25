using IAndOthers.Core.Data.Entity;
using IAndOthers.Core.Data.Enumeration;

namespace IAndOthers.Application.Authentication.Models
{
    public class GenerateTokenModel : IOEntityBase
    {
        public IOUserRoleEnum Role { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
