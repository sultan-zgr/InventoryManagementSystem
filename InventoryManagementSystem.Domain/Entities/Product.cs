namespace InventoryManagementSystem.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid CategoryId { get; set; }

        // Navigation Property
        public Category Category { get; set; }
        public string SKU { get; set; } // Stok Takip Kodu
    }
}
