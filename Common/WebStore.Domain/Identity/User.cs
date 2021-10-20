using Microsoft.AspNetCore.Identity;
using System;

namespace WebStore.Domain.Identity
{
    //если объявить вот так
    //public class User : IdentityUser<Guid>
    //тогда в API-контроллере надо UserStore объявлять вот так
    //private readonly UserStore<User, Role, WebStoreDbContext, Guid> _userStore;

    public class User : IdentityUser
    {
        public const string AdmLogin = "Admin";

        public const string DefaultAdmPassword = "AdPass_123";
    }
}
