using Microsoft.EntityFrameworkCore;
using WebStore.Domain.Entities;

namespace WebStore.DAL.Context
{
    public class WebStoreDbContext : DbContext
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
        //        var connStr = "Server=(localdb)\\mssqllocaldb;Database=WebStore-AK-2020-09-27;Trusted_Connection=True;";
        //        optionsBuilder.UseSqlServer(connStr);
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


        public DbSet<Employee> Employees { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}

