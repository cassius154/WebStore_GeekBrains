using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebStore.ViewModels
{
    public class EmployeeViewModel : IValidatableObject  //кроме атрибутов можно реализовать IValidatableObject
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Имя обязательно к заполнению")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Длина строки должна быть от 2 до 200 символов")]
        [RegularExpression(@"([А-ЯЁ][а-яё]+)|([A-Z][a-z]+)", ErrorMessage = "Неверный формат")]  //Первая буква - заглавная, дальше строчные, при этом кириллица и латиница не смешиваются
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Фамилия обязательна к заполнению")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Длина строки должна быть от 2 до 200 символов")]
        [RegularExpression(@"([А-ЯЁ][а-яё]+)|([A-Z][a-z]+)", ErrorMessage = "Неверный формат")]  //Первая буква - заглавная, дальше строчные, при этом кириллица и латиница не смешиваются
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
        //[Range(18, 80, ErrorMessage = "Возраст должен быть от 18 до 80")]  //для числовых можно использовать атрибут RangeAtribute
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


        //IValidatableObject
        //не работает - validationContext.MemberName приходит "model" - разобраться потом
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new[] { ValidationResult.Success };

            switch (validationContext.MemberName)
            {
                case nameof(Age):
                    if (Age < 15 && Age > 80)
                    {
                        return new[] { new ValidationResult("Странный возраст", new[] { nameof(Age) }) };
                    }
                    return new[] { ValidationResult.Success };

                default: return new[] { ValidationResult.Success };

            }
        }
    }
}
