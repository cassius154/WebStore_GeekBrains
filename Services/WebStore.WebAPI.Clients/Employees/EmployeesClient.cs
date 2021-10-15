using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WebStore.Domain.Models;
using WebStore.Interfaces.Services;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Employees
{
    public class EmployeesClient : ClientBase, IEmployeeService
    {
        public EmployeesClient(HttpClient client, string address) : base(client, "api/employees") { }

        public IEnumerable<Employee> GetEmployeeList(Func<Employee, bool> predicate = null)
        {
            var employees = Get<IEnumerable<Employee>>(address);
            return employees;
        }

        public Employee GetEmployee(int id)
        {
            var ret = Get<Employee>($"{address}/{id}");
            return ret;
        }

        public Employee Add(Employee emp)
        {
            var response = Post(address, emp);
            var employee = response.Content.ReadFromJsonAsync<Employee>().Result;
            return employee;
        }

        public Employee Edit(Employee emp)
        {
            var response = Put(address, emp);
            var employee = response.Content.ReadFromJsonAsync<Employee>().Result;
            return employee;
        }

        public bool Delete(int id)
        {
            var response = Delete($"{address}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
