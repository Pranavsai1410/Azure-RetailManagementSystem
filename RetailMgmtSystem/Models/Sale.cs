using Azure;
using Azure.Data.Tables;

namespace RetailInventoryManagement.Models
{
	public class Sale : ITableEntity
	{
		public string PartitionKey { get; set; }
			= DateTime.UtcNow.ToString("yyyy-MM-dd");
		public string RowKey { get; set; } = Guid.NewGuid().ToString();
		public ETag ETag { get; set; }
		public DateTimeOffset? Timestamp { get; set; }

		public string ProductID { get; set; }
		public string ProductName { get; set; }
		public int QuantitySold { get; set; }
		public double SalePrice { get; set; }
		public double TotalAmount { get; set; }
	}
}
