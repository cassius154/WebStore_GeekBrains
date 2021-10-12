using System.ComponentModel.DataAnnotations;
using WebStore.Domain.Entities.Base.Interfaces;

namespace WebStore.Domain.Entities.Base
{
    //[Index(nameof(Name)] - можно здесь, для всех потомков - а можно у каждого в отдельности.
    //Зависит от того, будут ли индексы отличаться - например, уникальностью
    public abstract class NamedEntity : Entity, INamedEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
