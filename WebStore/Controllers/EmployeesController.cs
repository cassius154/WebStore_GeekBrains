using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebStore.Data;
using WebStore.Models;
using WebStore.Services;

namespace WebStore.Controllers
{
    public class EmployeesController : Controller
    {
        //private readonly IEnumerable<Employee> _employees;

        private readonly EmployeeService _empService;

        public EmployeesController()
        {
            //_employees = TestData.Employees;
            _empService = new EmployeeService();
        }

        public IActionResult Index() => View(_empService.GetEmployeeList());

        public IActionResult Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var emp = _empService.GetEmployee(id.Value);
            if (emp is null)
            {
                return NotFound();
            }

            return View(emp);
        }

        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var emp = _empService.GetEmployee(id.Value);
            if (emp is null)
            {
                return NotFound();
            }

            return View(emp);
        }

        [HttpPost]
        public IActionResult Edit(Employee model)
        {
            _validateModel(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emp = _empService.Edit(model);
            if (emp is null)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Employee model)
        {
            _validateModel(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _empService.Add(model);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var emp = _empService.GetEmployee(id.Value);
            if (emp is null)
            {
                return NotFound();
            }

            return View(emp);
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


        private void _validateModel(Employee model)
        {
            if (model.BirthDate.Date > DateTime.Today)
            {
                ModelState.AddModelError("BirthDate", "Дата рождения больше текущей");
            }
        }
    }
}
