using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.Models;
using WebStore.Interfaces;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers
{
    /// <summary>
    /// Управление сотрудниками
    /// </summary>
    [ApiController]
    [Route(WebAPIAddresses.Employees)]  //если оставить "api/[controller]" - адрес будет api/employeesapi, а надо api/employees
    public class EmployeesApiController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesApiController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Получение всех сотрудников
        /// </summary>
        /// <returns>Список сотрудников</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var employees = _employeeService.GetEmployeeList();
            return Ok(employees);
        }

        /// <summary>
        /// Получение сотрудника по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Сотрудник с указанным идентификатором</returns>
        [HttpGet("{id}")]
        //если метод возвращает разные статусные коды - указываем их для Swagger
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Employee))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var employee = _employeeService.GetEmployee(id);
            if (employee is null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        /// <summary>
        /// Добавление сотрудника
        /// </summary>
        /// <param name="emp">Объект с данными сотрудника</param>
        /// <returns>Созданный сотрудник</returns>
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
