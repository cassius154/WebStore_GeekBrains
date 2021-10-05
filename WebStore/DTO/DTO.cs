using System.Collections.Generic;
using System.Linq;
using WebStore.Domain.Entities;
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

        public static EmployeeViewModel CreateEmployeeViewModel(Employee entity)
        {
            return new EmployeeViewModel
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Patronymic = entity.Patronymic,
                BirthDate = entity.BirthDate,
            };
        }

        public static SectionViewModel CreateSectionViewModel(Section entity)
        {
            return new SectionViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Order = entity.Order,
            };
        }


        public static ProductViewModel ToProductView(this Product product) => product is null
            ? null
            : new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Section = product.Section?.Name,
                Brand = product.Brand?.Name,
            };

        public static IEnumerable<ProductViewModel> ToProductView(this IEnumerable<Product> products) => products is null
            ? new List<ProductViewModel>()
            //: products.Select(p => p.ToProductView());
            : products.Select(ToProductView);
    }
}
