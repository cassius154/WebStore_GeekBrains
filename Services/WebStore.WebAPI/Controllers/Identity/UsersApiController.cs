using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.DTO.Identity;
using WebStore.Domain.Identity;
using WebStore.Interfaces;

namespace WebStore.WebAPI.Controllers.Identity
{
    [ApiController]
    [Route(WebAPIAddresses.Identity.Users)]
    public class UsersApiController : ControllerBase
    {
        //если бы пользовались стандартными IdentityUser и IdentityRole
        //можно было бы просто UserStore _userStore
        //если свой класс User, а роль стандартная IdentityRole
        //то надо UserStore<User>
        //поскольку унаследовали оба класса - указываем их + еще контекст
        private readonly UserStore<User, Role, WebStoreDbContext> _userStore;
        //пришлось отказаться от User : IdentityUser<Guid>,
        //вернуться к стандартному User : IdentityUser (он же User : IdentityUser<string>),
        //так же поступить с Role
        //иначе _userStore объявить не удается

        public UsersApiController(WebStoreDbContext db)
        {
            //_userStore = new UserStore<User, Role, WebStoreDbContext>(db);
            //или в новом синтаксисе
            _userStore = new(db);
            //_userStore.AutoSaveChanges = false;
        }

        [HttpGet("all")]
        public async Task<IEnumerable<User>> GetAll() => await _userStore.Users.ToArrayAsync();

        #region Users

        [HttpPost("UserId")] // POST: api/users/UserId
        public async Task<string> GetUserIdAsync([FromBody] User user) => await _userStore.GetUserIdAsync(user);

        [HttpPost("UserName")]
        public async Task<string> GetUserNameAsync([FromBody] User user) => await _userStore.GetUserNameAsync(user);

        [HttpPost("UserName/{name}")] // api/users/UserName/TestUser
        public async Task<string> SetUserNameAsync([FromBody] User user, string name)
        {
            await _userStore.SetUserNameAsync(user, name);
            //user.UserName = name;
            await _userStore.UpdateAsync(user);
            return user.UserName;
        }

        [HttpPost("NormalUserName")]
        public async Task<string> GetNormalizedUserNameAsync([FromBody] User user) => await _userStore.GetNormalizedUserNameAsync(user);

        [HttpPost("NormalUserName/{name}")]
        public async Task<string> SetNormalizedUserNameAsync([FromBody] User user, string name)
        {
            await _userStore.SetNormalizedUserNameAsync(user, name);
            await _userStore.UpdateAsync(user);
            return user.NormalizedUserName;
        }

        [HttpPost("User")] // POST -> api/users/user
        public async Task<bool> CreateAsync([FromBody] User user)
        {
            var creation_result = await _userStore.CreateAsync(user);
            // добавление ошибок создания нового пользователя в журнал
            return creation_result.Succeeded;
        }

        [HttpPut("User")] // PUT -> api/users/user
        public async Task<bool> UpdateAsync([FromBody] User user)
        {
            var update_result = await _userStore.UpdateAsync(user);
            return update_result.Succeeded;
        }

        [HttpPost("User/Delete")] // POST api/users/user/delete
        public async Task<bool> DeleteAsync([FromBody] User user)
        {
            var delete_result = await _userStore.DeleteAsync(user);
            return delete_result.Succeeded;
        }

        [HttpGet("User/Find/{id}")] // api/users/user/Find/9E5CB5E7-41DE-4449-829E-45F4C97AA54B
        public async Task<User> FindByIdAsync(string id) => await _userStore.FindByIdAsync(id);

        [HttpGet("User/Normal/{name}")] // api/users/user/Normal/TestUser
        public async Task<User> FindByNameAsync(string name) => await _userStore.FindByNameAsync(name);

        [HttpPost("Role/{role}")] // POST -> api/users/role/admin - добавляет роль "admin" пользователю, который передаётся в теле запроса
        public async Task AddToRoleAsync([FromBody] User user, string role/*, [FromServices] WebStoreDB db*/)
        {
            await _userStore.AddToRoleAsync(user, role);
            await _userStore.Context.SaveChangesAsync();
            //раньше не было доступа к контексту из _userStore,
            //поэтому приходлось затягивать контекст посредством [FromServices] WebStoreDB db
            //await db.SaveChangesAsync();
        }

        [HttpDelete("Role/{role}")]
        [HttpPost("Role/Delete/{role}")]
        public async Task RemoveFromRoleAsync([FromBody] User user, string role/*, [FromServices] WebStoreDB db*/)
        {
            await _userStore.RemoveFromRoleAsync(user, role);
            await _userStore.Context.SaveChangesAsync();
            //await db.SaveChangesAsync();
        }

        [HttpPost("Roles")]
        public async Task<IList<string>> GetRolesAsync([FromBody] User user) => await _userStore.GetRolesAsync(user);

        [HttpPost("InRole/{role}")]
        public async Task<bool> IsInRoleAsync([FromBody] User user, string role) => await _userStore.IsInRoleAsync(user, role);

        [HttpGet("UsersInRole/{role}")]
        public async Task<IList<User>> GetUsersInRoleAsync(string role) => await _userStore.GetUsersInRoleAsync(role);

        [HttpPost("GetPasswordHash")]
        public async Task<string> GetPasswordHashAsync([FromBody] User user) => await _userStore.GetPasswordHashAsync(user);

        [HttpPost("SetPasswordHash")]
        public async Task<string> SetPasswordHashAsync([FromBody] PasswordHashDTO hash)
        {
            await _userStore.SetPasswordHashAsync(hash.User, hash.Hash);
            await _userStore.UpdateAsync(hash.User);
            return hash.User.PasswordHash;
        }

        [HttpPost("HasPassword")]
        public async Task<bool> HasPasswordAsync([FromBody] User user) => await _userStore.HasPasswordAsync(user);

        #endregion Users

        #region Claims

        [HttpPost("GetClaims")]
        public async Task<IList<Claim>> GetClaimsAsync([FromBody] User user) => await _userStore.GetClaimsAsync(user);

        [HttpPost("AddClaims")]
        public async Task AddClaimsAsync([FromBody] AddClaimDTO ClaimInfo/*, [FromServices] WebStoreDB db*/)
        {
            await _userStore.AddClaimsAsync(ClaimInfo.User, ClaimInfo.Claims);
            await _userStore.Context.SaveChangesAsync();
            //await db.SaveChangesAsync();
        }

        [HttpPost("ReplaceClaim")]
        public async Task ReplaceClaimAsync([FromBody] ReplaceClaimDTO ClaimInfo/*, [FromServices] WebStoreDB db*/)
        {
            await _userStore.ReplaceClaimAsync(ClaimInfo.User, ClaimInfo.Claim, ClaimInfo.NewClaim);
            await _userStore.Context.SaveChangesAsync();
            //await db.SaveChangesAsync();
        }

        [HttpPost("RemoveClaim")]
        public async Task RemoveClaimsAsync([FromBody] RemoveClaimDTO ClaimInfo/*, [FromServices] WebStoreDB db*/)
        {
            await _userStore.RemoveClaimsAsync(ClaimInfo.User, ClaimInfo.Claims);
            await _userStore.Context.SaveChangesAsync();
            //await db.SaveChangesAsync();
        }

        [HttpPost("GetUsersForClaim")]
        public async Task<IList<User>> GetUsersForClaimAsync([FromBody] Claim claim) =>
            await _userStore.GetUsersForClaimAsync(claim);

        #endregion Claims

        #region TwoFactor

        [HttpPost("GetTwoFactorEnabled")]
        public async Task<bool> GetTwoFactorEnabledAsync([FromBody] User user) => await _userStore.GetTwoFactorEnabledAsync(user);

        [HttpPost("SetTwoFactor/{enable}")]
        public async Task<bool> SetTwoFactorEnabledAsync([FromBody] User user, bool enable)
        {
            await _userStore.SetTwoFactorEnabledAsync(user, enable);
            await _userStore.UpdateAsync(user);
            return user.TwoFactorEnabled;
        }

        #endregion TwoFactor

        #region Email/Phone

        [HttpPost("GetEmail")]
        public async Task<string> GetEmailAsync([FromBody] User user) => await _userStore.GetEmailAsync(user);

        [HttpPost("SetEmail/{email}")]
        public async Task<string> SetEmailAsync([FromBody] User user, string email)
        {
            await _userStore.SetEmailAsync(user, email);
            await _userStore.UpdateAsync(user);
            return user.Email;
        }

        [HttpPost("GetEmailConfirmed")]
        public async Task<bool> GetEmailConfirmedAsync([FromBody] User user) => await _userStore.GetEmailConfirmedAsync(user);

        [HttpPost("SetEmailConfirmed/{enable}")]
        public async Task<bool> SetEmailConfirmedAsync([FromBody] User user, bool enable)
        {
            await _userStore.SetEmailConfirmedAsync(user, enable);
            await _userStore.UpdateAsync(user);
            return user.EmailConfirmed;
        }

        [HttpGet("UserFindByEmail/{email}")]
        public async Task<User> FindByEmailAsync(string email) => await _userStore.FindByEmailAsync(email);

        [HttpPost("GetNormalizedEmail")]
        public async Task<string> GetNormalizedEmailAsync([FromBody] User user) => await _userStore.GetNormalizedEmailAsync(user);

        [HttpPost("SetNormalizedEmail/{email?}")]
        public async Task<string> SetNormalizedEmailAsync([FromBody] User user, string email)
        {
            await _userStore.SetNormalizedEmailAsync(user, email);
            await _userStore.UpdateAsync(user);
            return user.NormalizedEmail;
        }

        [HttpPost("GetPhoneNumber")]
        public async Task<string> GetPhoneNumberAsync([FromBody] User user) => await _userStore.GetPhoneNumberAsync(user);

        [HttpPost("SetPhoneNumber/{phone}")]
        public async Task<string> SetPhoneNumberAsync([FromBody] User user, string phone)
        {
            await _userStore.SetPhoneNumberAsync(user, phone);
            await _userStore.UpdateAsync(user);
            return user.PhoneNumber;
        }

        [HttpPost("GetPhoneNumberConfirmed")]
        public async Task<bool> GetPhoneNumberConfirmedAsync([FromBody] User user) =>
            await _userStore.GetPhoneNumberConfirmedAsync(user);

        [HttpPost("SetPhoneNumberConfirmed/{confirmed}")]
        public async Task<bool> SetPhoneNumberConfirmedAsync([FromBody] User user, bool confirmed)
        {
            await _userStore.SetPhoneNumberConfirmedAsync(user, confirmed);
            await _userStore.UpdateAsync(user);
            return user.PhoneNumberConfirmed;
        }

        #endregion Email/Phone

        #region Login/Lockout

        [HttpPost("AddLogin")]
        public async Task AddLoginAsync([FromBody] AddLoginDTO login/*, [FromServices] WebStoreDB db*/)
        {
            await _userStore.AddLoginAsync(login.User, login.UserLoginInfo);
            await _userStore.Context.SaveChangesAsync();
            //await db.SaveChangesAsync();
        }

        [HttpPost("RemoveLogin/{LoginProvider}/{ProviderKey}")]
        public async Task RemoveLoginAsync([FromBody] User user, string LoginProvider, string ProviderKey/*, [FromServices] WebStoreDB db*/)
        {
            await _userStore.RemoveLoginAsync(user, LoginProvider, ProviderKey);
            await _userStore.Context.SaveChangesAsync();
            //await db.SaveChangesAsync();
        }

        [HttpPost("GetLogins")]
        public async Task<IList<UserLoginInfo>> GetLoginsAsync([FromBody] User user) => await _userStore.GetLoginsAsync(user);

        [HttpGet("User/FindByLogin/{LoginProvider}/{ProviderKey}")]
        public async Task<User> FindByLoginAsync(string LoginProvider, string ProviderKey) => await _userStore.FindByLoginAsync(LoginProvider, ProviderKey);

        [HttpPost("GetLockoutEndDate")]
        public async Task<DateTimeOffset?> GetLockoutEndDateAsync([FromBody] User user) => await _userStore.GetLockoutEndDateAsync(user);

        [HttpPost("SetLockoutEndDate")]
        public async Task<DateTimeOffset?> SetLockoutEndDateAsync([FromBody] SetLockoutDTO LockoutInfo)
        {
            await _userStore.SetLockoutEndDateAsync(LockoutInfo.User, LockoutInfo.LockoutEnd);
            await _userStore.UpdateAsync(LockoutInfo.User);
            return LockoutInfo.User.LockoutEnd;
        }

        [HttpPost("IncrementAccessFailedCount")]
        public async Task<int> IncrementAccessFailedCountAsync([FromBody] User user)
        {
            var count = await _userStore.IncrementAccessFailedCountAsync(user);
            await _userStore.UpdateAsync(user);
            return count;
        }

        [HttpPost("ResetAccessFailedCount")]
        public async Task<int> ResetAccessFailedCountAsync([FromBody] User user)
        {
            await _userStore.ResetAccessFailedCountAsync(user);
            await _userStore.UpdateAsync(user);
            return user.AccessFailedCount;
        }

        [HttpPost("GetAccessFailedCount")]
        public async Task<int> GetAccessFailedCountAsync([FromBody] User user) => await _userStore.GetAccessFailedCountAsync(user);

        [HttpPost("GetLockoutEnabled")]
        public async Task<bool> GetLockoutEnabledAsync([FromBody] User user) => await _userStore.GetLockoutEnabledAsync(user);

        [HttpPost("SetLockoutEnabled/{enable}")]
        public async Task<bool> SetLockoutEnabledAsync([FromBody] User user, bool enable)
        {
            await _userStore.SetLockoutEnabledAsync(user, enable);
            await _userStore.UpdateAsync(user);
            return user.LockoutEnabled;
        }

        #endregion Login/Lockout

    }
}
