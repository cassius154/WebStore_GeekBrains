using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebStore.Domain.Entities;
using WebStore.Domain.Entities.Orders;
using WebStore.Domain.Identity;

namespace WebStore.DAL.Context
{
    public class WebStoreDbContext : IdentityDbContext<User, Role, Guid>
    {

        public WebStoreDbContext(DbContextOptions<WebStoreDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        теперь это уносится в Startup
        //        var connStr = "Server=(localdb)\\mssqllocaldb;Database=WebStore-AK-2020-09-27;Trusted_Connection=True;";
        //        optionsBuilder.UseSqlServer(connStr);
        //        теперь это уносится в Startup
        //    }
        //}

        //private void _initData(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Employee>().HasData(TestData.Employees);

        //    modelBuilder.Entity<Brand>().HasData(TestData.Brands);
        //    modelBuilder.Entity<Section>().HasData(TestData.Sections);
        //    modelBuilder.Entity<Product>().HasData(TestData.Products);
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    _initData(modelBuilder);

        //    base.OnModelCreating(modelBuilder);
        //}


        //public DbSet<Employee> Employees { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }

        //необязательно - таблица создастся сама из Order по связи один-ко-многим
        //если нужен доступ непосредственно к OrderItems - то добавить
        //но если не добавлять, и атрибут имени таблицы у класса не использовать -
        //автоматическое имя таблице будет дано OrderItem
        //иначе - правильное OrderItems
        //public DbSet<OrderItem> OrderItems { get; set; } 
    }
}

