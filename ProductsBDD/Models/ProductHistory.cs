namespace ProductAPI.Models
{
    public class ProductHistory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;

        // Relacja z produktem
        public Product Product { get; set; } = null!;
    }

}
