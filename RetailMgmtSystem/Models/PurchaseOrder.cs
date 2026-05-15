using Azure;
using Azure.Data.Tables;

namespace RetailInventoryManagement.Models
{
	public class PurchaseOrder : ITableEntity
	{
		public string PartitionKey { get; set; } = "ORDER";
		public string RowKey { get; set; } = Guid.NewGuid().ToString();
		public ETag ETag { get; set; }
		public DateTimeOffset? Timestamp { get; set; }

		public string ProductID { get; set; }
		public string SupplierID { get; set; }
		public int QuantityOrdered { get; set; }
		public string Status { get; set; } = "Pending";
	}
}
