using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace System_Rezerwacji.Models
{
    public class UserDetails
    {
        public IdentityUser User { get; set; }
        public IList<string> Roles { get; set; }

    }
}
