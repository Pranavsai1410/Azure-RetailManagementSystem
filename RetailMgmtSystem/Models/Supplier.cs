using Azure;
using Azure.Data.Tables;

namespace RetailInventoryManagement.Models
{
	public class Supplier : ITableEntity
	{
		public string PartitionKey { get; set; } = "SUPPLIER";
		public string RowKey { get; set; } = Guid.NewGuid().ToString();
		public ETag ETag { get; set; }
		public DateTimeOffset? Timestamp { get; set; }

		public string SupplierName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
	}
}
