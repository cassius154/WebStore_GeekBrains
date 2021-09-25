using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebStore.Domain.Entities;
using WebStore.Models;

namespace WebStore.Data
{
    public class WebStoreDbContext : DbContext
    {

        public WebStoreDbContext()
            : base()
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connStr = "Server=(localdb)\\mssqllocaldb;Database=WebStore;Trusted_Connection=True;";
                optionsBuilder.UseSqlServer(connStr);
            }
        }

        private void _initData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(TestData.Employees);
            modelBuilder.Entity<Brand>().HasData(TestData.Brands);
            modelBuilder.Entity<Section>().HasData(TestData.Sections);
            modelBuilder.Entity<Product>().HasData(TestData.Products);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _initData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Employee> Employees { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}

