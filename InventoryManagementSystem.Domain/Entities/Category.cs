﻿namespace InventoryManagementSystem.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation Property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
