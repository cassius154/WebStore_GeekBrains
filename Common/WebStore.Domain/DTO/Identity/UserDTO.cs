using System;
using Microsoft.AspNetCore.Identity;
using WebStore.Domain.Identity;

namespace WebStore.Domain.DTO.Identity
{
    public abstract class UserDTO
    {
        public User User { get; set; }
    }

    //DTO для системы регистрации факта входа пользователя
    public class AddLoginDTO : UserDTO
    {
        public UserLoginInfo UserLoginInfo { get; set; }
    }

    //DTO для хранилища паролей
    public class PasswordHashDTO : UserDTO
    {
        public string Hash { get; set; }
    }

    //DTO для операций установки/снятия блокировок пользователей
    public class SetLockoutDTO : UserDTO
    {
        //дата-время окончания блокировки
        //если null - блокировка снимается
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
