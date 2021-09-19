using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebStore.Data;
using WebStore.Models;

namespace WebStore.Services
{
    public class EmployeeService
    {
        public IEnumerable<Employee> GetEmployeeList() => TestData.Employees;

        public Employee GetEmployee(int id) => TestData.Employees.SingleOrDefault(e => e.Id == id);

        public Employee Add(Employee emp)
        {
            var ret = new Employee
            {
                LastName = emp.LastName,
                FirstName = emp.FirstName,
                Patronymic = emp.Patronymic,
                BirthDate = emp.BirthDate,
                Id = (TestData.Employees.OrderBy(e => e.Id).LastOrDefault()?.Id).GetValueOrDefault() + 1,
            };

            TestData.Employees.Add(ret);
            return ret;
        }

        public Employee Edit(Employee emp)
        {
            var e = GetEmployee(emp.Id);
            if (e != null)
            {
                e.FirstName = emp.FirstName;
                e.LastName = emp.LastName;
                e.Patronymic = emp.Patronymic;
                e.BirthDate = emp.BirthDate;
            }

            return emp;
        }

        public void Delete(int id)
        {
            var e = GetEmployee(id);
            if (e != null)
            {
                TestData.Employees.Remove(e);
            }
        }
    }
}
