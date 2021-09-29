using System;
using System.Collections.Generic;
using WebStore.DAL.Models;

namespace WebStore.Services.Interfaces
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> GetEmployeeList(Func<Employee, bool> predicate = null);

        Employee GetEmployee(string id);

        Employee Add(Employee emp);
        
        Employee Edit(Employee emp);

        bool Delete(string id);

    }
}