using Azure;
using Azure.Data.Tables;

namespace RetailInventoryManagement.Models
{
	public class Product : ITableEntity
	{
		public string PartitionKey { get; set; } = "PRODUCT";
		public string RowKey { get; set; } = Guid.NewGuid().ToString();
		public ETag ETag { get; set; }
		public DateTimeOffset? Timestamp { get; set; }

		public string ProductName { get; set; }
		public string Category { get; set; }
		public double Price { get; set; }
		public int StockQuantity { get; set; }
		public int MinStockLevel { get; set; }
	}
}
