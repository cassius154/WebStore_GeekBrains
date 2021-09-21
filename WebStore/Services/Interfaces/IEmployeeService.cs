using System;
using System.Collections.Generic;
using WebStore.Models;

namespace WebStore.Services.Interfaces
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> GetEmployeeList(Func<Employee, bool> predicate = null);

        Employee GetEmployee(int id);

        Employee Add(Employee emp);
        
        void Delete(int id);
        
        Employee Edit(Employee emp);
    }
}