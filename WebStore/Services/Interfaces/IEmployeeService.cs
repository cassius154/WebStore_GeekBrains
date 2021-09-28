using System;
using System.Collections.Generic;
using WebStore.Domain.Entities;

namespace WebStore.Services.Interfaces
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> GetEmployeeList(Func<Employee, bool> predicate = null);

        Employee GetEmployee(int id);

        Employee Add(Employee emp);
        
        Employee Edit(Employee emp);

        bool Delete(int id);

    }
}