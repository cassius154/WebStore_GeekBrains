using System;

namespace WebStore.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public DateTime BirthDate { get; set; }

        public int Age 
        {
            get 
            {
                DateTime now = DateTime.Today;
                int age = now.Year - BirthDate.Year;
                if (BirthDate > now.AddYears(-age)) age--;
                return age;
            }
        }
    }

    //public record Employee2(int id, string FirstName, string LastName, string Patronymic, DateTime BirthDate);
}
