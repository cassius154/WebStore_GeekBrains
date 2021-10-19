using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Domain.Models
{
    /// <summary>
    /// Информация о сотруднике
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Идентификатор сотрудника
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime BirthDate { get; set; }


        //если определены 2 этих оператора - при сравнениях будут использованы они
        //и если будет сравнение == null, или != null - то в такой реализации будет ошибка,
        //потому что нет проверки на null. Поэтому надежнее использовать новый is null
        //public static bool operator ==(Employee e1, Employee e2)
        //{
        //    return e1.Id == e2.Id;
        //}

        //public static bool operator !=(Employee e1, Employee e2)
        //{
        //    return e1.Id != e2.Id;
        //}
    }

    //public record Employee2(int id, string FirstName, string LastName, string Patronymic, DateTime BirthDate);
}
