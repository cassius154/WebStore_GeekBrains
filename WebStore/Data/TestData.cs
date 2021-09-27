using System;
using System.Collections.Generic;
using WebStore.Models;

namespace WebStore.Data
{
    public static class TestData
    {
        public static List<Employee> Employees { get; } = new()
        {
            new Employee { Id = 1, LastName = "Иванов", FirstName = "Иван", Patronymic = "Иванович", BirthDate = new DateTime(1985, 03, 23) },
            new Employee { Id = 2, LastName = "Петров", FirstName = "Петр", Patronymic = "Петрович", BirthDate = new DateTime(1995, 07, 27) },
            new Employee { Id = 3, LastName = "Сидоров", FirstName = "Сидор", Patronymic = "Сидорович", BirthDate = new DateTime(2001, 11, 21) },
        };
    }
}
