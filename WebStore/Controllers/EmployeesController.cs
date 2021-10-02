using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebStore.DTO;
using WebStore.Services.Interfaces;
using WebStore.ViewModels;

namespace WebStore.Controllers
{
    //[Route("Employees/[action]/{id?}")]
    //[Route("Staff/[action]/{id?}")]
    [Authorize(Roles = "Administrators")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _empService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService empService, ILogger<EmployeesController> logger)
        {
            _logger = logger;
            _empService = empService;
        }

        //[Route("~/employees/all")]
        public IActionResult Index() => View(_empService.GetEmployeeList().Select(e => DataMapper.CreateEmployeeViewModel(e)));

        //[Route("~/employees/info-{id}")]
        public IActionResult Details(int? id)
        {
            if (id is null || id.Value < 1)
            {
                return BadRequest();
            }

            var emp = _empService.GetEmployee(id.Value);
            if (emp is null)  //эквивалентно if (ReferenceEquals(emp, null))
            {
                return NotFound();
            }

            return View(DataMapper.CreateEmployeeViewModel(emp));
        }

        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return View(new EmployeeViewModel());
            }
            if (id.Value < 1)
            {
                return BadRequest();
            }

            var emp = _empService.GetEmployee(id.Value);
            if (emp is null)
            {
                return NotFound();
            }

            return View(DataMapper.CreateEmployeeViewModel(emp));
        }

        [HttpPost]
        public IActionResult Edit(EmployeeViewModel model)
        {
            _validateModel(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emp = DataMapper.CreateEmployee(model);
            if (model.Id == 0)
            {
               _ = _empService.Add(DataMapper.CreateEmployee(model));
            }
            else
            {
                emp = _empService.Edit(DataMapper.CreateEmployee(model));
                if (emp is null)
                {
                    return NotFound();
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Create1() => View();

        public IActionResult Create2() => View("Edit", new EmployeeViewModel());


        [HttpPost]
        public IActionResult Create1(EmployeeViewModel model)
        {
            _validateModel(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _empService.Add(DataMapper.CreateEmployee(model));

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id is null || id.Value < 1)
            {
                return BadRequest();
            }

            var emp = _empService.GetEmployee(id.Value);
            if (emp is null)
            {
                return NotFound();
            }

            return View(DataMapper.CreateEmployeeViewModel(emp));
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id != null)
            {
                _empService.Delete(id.Value);
            }

            return RedirectToAction("Index");
        }


        private void _validateModel(EmployeeViewModel model)
        {
            if (model.BirthDate.Value.Date > DateTime.Today)
            {
                ModelState.AddModelError("BirthDate", "Дата рождения больше текущей");
            }
            if (model.Age > 150)
            {
                ModelState.AddModelError("BirthDate", "Проверьте внимательно год рождения!");
            }

            if ((model.FirstName ?? "").Any(char.IsDigit) || (model.LastName ?? "").Any(char.IsDigit) || (model.Patronymic ?? "").Any(char.IsDigit))
            {
                ModelState.AddModelError("", "В именах не должно быть цифр");
            }
        }
    }
}
