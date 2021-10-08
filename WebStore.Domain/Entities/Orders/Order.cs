using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WebStore.Domain.Entities.Base;
using WebStore.Domain.Identity;

namespace WebStore.Domain.Entities.Orders
{
    public class Order : Entity
    {
        [Required]
        public User User { get; set; }

        [Required]
        [MaxLength(200)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; }

        //рекомендуемый тип для хранения даты
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;  //возможно, лучше DateTimeOffset.Now - надо выяснить

        public string Description { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        [NotMapped]
        public decimal Total => Items?.Sum(i => i.Total) ?? 0m;
    }


    public class OrderItem : Entity
    {
        [Required]
        public Product Product { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public Order Order { get; set; }

        [NotMapped]
        public decimal Total => Price * Quantity;
    }
}
