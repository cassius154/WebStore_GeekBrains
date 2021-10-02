﻿using System;
using Microsoft.AspNetCore.Identity;

namespace WebStore.Domain.Identity
{
    public class Role : IdentityRole<Guid>
    {
        public const string Administrators = "Administrators";

        public const string Users = "Users";
    }
}
