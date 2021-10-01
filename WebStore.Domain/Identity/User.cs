using Microsoft.AspNetCore.Identity;

namespace WebStore.Domain.Identity
{
    public class User : IdentityUser
    {
        public const string AdmLogin = "Admin";

        public const string DefaultAdmPassword = "AdPass_123";
    }
}
