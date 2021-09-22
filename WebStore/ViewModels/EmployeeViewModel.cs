using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebStore.Models;

namespace WebStore.ViewModels
{
    public class EmployeeViewModel
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
        [Required(ErrorMessage = "Дата рождения обязательна к заполнению")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]  - при редактировании не работает!!
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Возраст")]
        public int Age
        {
            get
            {
                DateTime dtNow = DateTime.Today;
                int age = dtNow.Year - BirthDate.Value.Year;
                if (BirthDate > dtNow.AddYears(-age)) age--;
                return age;
            }
        }
    }
}
