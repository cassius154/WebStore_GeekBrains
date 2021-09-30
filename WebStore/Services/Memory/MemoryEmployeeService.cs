using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using WebStore.Data;
using WebStore.Domain.Entities;
using WebStore.Services.Interfaces;

namespace WebStore.Services.Memory
{
    public class MemoryEmployeeService : IEmployeeService
    {

        private readonly ILogger<MemoryEmployeeService> _logger;
        private int _maxId;

        public MemoryEmployeeService(ILogger<MemoryEmployeeService> logger)
        {
            _logger = logger;
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

        public IEnumerable<Employee> GetEmployeeList(Func<Employee, bool> predicate = null)
        {
            var ret = TestData.Employees.AsEnumerable();
            if (predicate != null)
            {
                ret = ret.Where(predicate);
            }

            return ret;
        }

        public Employee GetEmployee(int id) => TestData.Employees.SingleOrDefault(e => e.Id == id);

        public Employee Add(Employee emp)
        {
            if (emp == null)
            {
                throw new ArgumentNullException(nameof(emp));
            }

            if (TestData.Employees.Contains(emp))
            {
                return emp;
            }

            //emp.Id = (TestData.Employees.OrderByDescending(e => e.Id).FirstOrDefault()?.Id).GetValueOrDefault() + 1;
            //emp.Id = _getMaxId() + 1;
            emp.Id = ++_maxId;

            TestData.Employees.Add(emp);
            return emp;
        }

        public Employee Edit(Employee emp)
        {
            if (emp == null)
            {
                throw new ArgumentNullException(nameof(emp));
            }

            if (TestData.Employees.Contains(emp))  //Только для реализации "в памяти"!!
            {
                return emp;
            }

            var e = GetEmployee(emp.Id);
            if (e != null)
            {
                e.FirstName = emp.FirstName;
                e.LastName = emp.LastName;
                e.Patronymic = emp.Patronymic;
                e.BirthDate = emp.BirthDate;
            }

            return e;
        }

        public bool Delete(int id)
        {
            var e = GetEmployee(id);
            if (e != null)
            {
                TestData.Employees.Remove(e);
                return true;
            }

            return false;
        }
    }
}
