using System;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Имя обязательно к заполнению")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Фамилия обязательна к заполнению")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "Дата рождения обязательна к заполнению")]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]  - при редактировании не работает!!
        public DateTime BirthDate { get; set; }

        [Display(Name = "Возраст")]
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
