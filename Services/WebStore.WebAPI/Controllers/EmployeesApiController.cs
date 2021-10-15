using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.Models;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/employees")]  //если оставить "api/[controller]" - адрес будет api/employeesapi, а надо api/employees
    public class EmployeesApiController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesApiController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var employees = _employeeService.GetEmployeeList();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var employee = _employeeService.GetEmployee(id);
            if (employee is null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost]
        public IActionResult Add(Employee emp)
        {
            var employee = _employeeService.Add(emp);
            //return Ok(employee);
            return CreatedAtAction(nameof(GetById),  //имя в маршруте, по которому можно получить созданный объект
                new { id = employee.Id },   //доп. данные маршрута, по которому можно получить созданный объект
                employee);  //сам созданный объект
        }

        [HttpPut]
        public IActionResult Update(Employee emp)
        {
            var employee = _employeeService.Edit(emp);
            return Ok(employee);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var ok = _employeeService.Delete(id);
            return ok ? Ok(true) : NotFound(false);  //можем в качестве параметров продублировать результат
        }
    }
}
