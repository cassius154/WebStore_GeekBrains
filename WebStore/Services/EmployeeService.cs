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

        private int _maxId;

        public EmployeeService()
        {
            _maxId = _getMaxId();
            //_maxId = _getMaxIntValue(TestData.Employees, e => e.Id);
        }

        private static int _getMaxId()
        {
            return TestData.Employees.Any() ? TestData.Employees.Max(e => e.Id) : 0;
        }

        private static int _getMaxIntValue<T>(IEnumerable<T> list, Func<T, int> selector)
        {
            return list.Any() ? list.Max(selector) : 0;
        }

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
                //Id = (TestData.Employees.OrderByDescending(e => e.Id).FirstOrDefault()?.Id).GetValueOrDefault() + 1,
                //Id = _getMaxId() + 1,
                Id = ++_maxId,
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
