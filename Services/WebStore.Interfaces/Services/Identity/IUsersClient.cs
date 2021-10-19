using Microsoft.AspNetCore.Identity;
using WebStore.Domain.Identity;

namespace WebStore.Interfaces.Services.Identity
{
    public interface IUsersClient :
        IUserRoleStore<User>,           //связи пользователей с ролями
        IUserPasswordStore<User>,       //хранилище паролей
        IUserEmailStore<User>,          //хранилище email
        IUserPhoneNumberStore<User>,    //хранилище телефонных номеров
        IUserTwoFactorStore<User>,      //двухфакторная авторизация
        IUserLoginStore<User>,          //хранилище фактов входа пользователя в систему
        IUserClaimStore<User>           //хранилище клэймов (доп. информация при авторизации)
    {
        
    }
}
