using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebStore.Domain.Entities.Base;
using WebStore.Domain.Entities.Base.Interfaces;

namespace WebStore.Domain.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Section : NamedEntity, IOrderedEntity
    {
        public int Order { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]  //при таком наименовании свойств (Parent - ParentId) атрибут необязателен - EF сама все сделает
        public Section Parent { get; set; }  //навигационное свойство

        //оптимальнее всего иименно ICollection (не List, IList, IEnumerable) - именно с ICollection EF работает оптимальнее всего
        public ICollection<Product> Products { get; set; }
    }

}
