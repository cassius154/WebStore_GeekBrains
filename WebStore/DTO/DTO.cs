using WebStore.Models;
using WebStore.ViewModels;

namespace WebStore.DTO
{
    public static class DataMapper
    {
        public static Employee CreateEmployee(EmployeeViewModel model)
        {
            return new Employee
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Patronymic = model.Patronymic,
                BirthDate = model.BirthDate.Value,
            };
        }

        public static EmployeeViewModel CreateEmployeeViewModel(Employee employee)
        {
            return new EmployeeViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Patronymic = employee.Patronymic,
                BirthDate = employee.BirthDate,
            };
        }
    }
}
