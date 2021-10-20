using System;
using Microsoft.AspNetCore.Identity;

namespace WebStore.Domain.Identity
{
    //если объявить вот так
    //public class Role : IdentityRole<Guid>
    //тогда в API-контроллере надо RoleStore объявлять вот так
    //private readonly RoleStore<Role, WebStoreDbContext, Guid> _roleStore;

    public class Role : IdentityRole
    {
        public const string Administrators = "Administrators";

        public const string Users = "Users";
    }
}
