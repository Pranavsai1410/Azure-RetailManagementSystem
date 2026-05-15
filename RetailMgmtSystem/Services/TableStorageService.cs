using Azure.Data.Tables;

namespace RetailInventoryManagement.Services
{
	public class TableStorageService
	{
		private readonly string _connectionString;
		private readonly string s;

		public TableStorageService(IConfiguration config)
		{
			var keyVaultService = new KeyVaultService();

			_connectionString = keyVaultService.GetSecret("TableStorageConnection");
		}

		// Get Table (creates if not exists)
		private TableClient GetTable(string tableName)
		{
			var client = new TableClient(_connectionString, tableName);
			client.CreateIfNotExists();
			return client;
		}

		// Add Entity
		public async Task AddEntityAsync<T>(string tableName, T entity)
			where T : ITableEntity
		{
			var table = GetTable(tableName);
			await table.AddEntityAsync(entity);
		}

		// Get Single Entity
		public async Task<T> GetEntityAsync<T>(
			string tableName, string partitionKey, string rowKey)
			where T : class, ITableEntity, new()
		{
			var table = GetTable(tableName);
			var result = await table.GetEntityAsync<T>(partitionKey, rowKey);
			return result.Value;
		}

		// Get All Entities by PartitionKey
		public List<T> GetAllEntities<T>(string tableName, string partitionKey)
			where T : class, ITableEntity, new()
		{
			var table = GetTable(tableName);
			return table.Query<T>(x => x.PartitionKey == partitionKey).ToList();
		}

		// Update Entity
		public async Task UpdateEntityAsync<T>(string tableName, T entity)
			where T : ITableEntity
		{
			var table = GetTable(tableName);
			await table.UpdateEntityAsync(entity, Azure.ETag.All);
		}

		// Delete Entity
		public async Task DeleteEntityAsync(
			string tableName, string partitionKey, string rowKey)
		{
			var table = GetTable(tableName);
			await table.DeleteEntityAsync(partitionKey, rowKey);
		}
	}
}
