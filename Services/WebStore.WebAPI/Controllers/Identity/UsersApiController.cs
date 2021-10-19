using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
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
    }
}
