using Microsoft.AspNetCore.Identity;

namespace System_Rezerwacji.Models
{
    public class UserRoles
    {
        public IdentityUser User { get; set; }
        public IList<string> UsersRoles { get; set; }
        public IList<IdentityRole> AllRoles { get; set; }
    }
}
