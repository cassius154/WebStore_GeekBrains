using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebStore.Models;
using WebStore.Services.Interfaces;
using WebStore.ViewModels;

namespace WebStore.Controllers
{
    //[Route("Employees/[action]/{id?}")]
    //[Route("Staff/[action]/{id?}")]
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
        public IActionResult Index() => View(_empService.GetEmployeeList().Select(e => EmployeeViewModel.CreateEmployeeViewModel(e)));

        //[Route("~/employees/info-{id}")]
        public IActionResult Details(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            var emp = _empService.GetEmployee(id.Value);
            if (emp is null)
            {
                return NotFound();
            }

            return View(EmployeeViewModel.CreateEmployeeViewModel(emp));
        }

        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            var emp = _empService.GetEmployee(id.Value);
            if (emp is null)
            {
                return NotFound();
            }

            return View(EmployeeViewModel.CreateEmployeeViewModel(emp));
        }

        [HttpPost]
        public IActionResult Edit(EmployeeViewModel model)
        {
            _validateModel(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emp = _empService.Edit(EmployeeViewModel.CreateEmployee(model));
            if (emp is null)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(EmployeeViewModel model)
        {
            _validateModel(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _empService.Add(EmployeeViewModel.CreateEmployee(model));

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            var emp = _empService.GetEmployee(id.Value);
            if (emp is null)
            {
                return NotFound();
            }

            return View(EmployeeViewModel.CreateEmployeeViewModel(emp));
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
            if (model.BirthDate.Date > DateTime.Today)
            {
                ModelState.AddModelError("BirthDate", "Дата рождения больше текущей");
            }
        }
    }
}
