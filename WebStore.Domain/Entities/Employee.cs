using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Domain.Entities
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

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
