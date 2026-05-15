using Microsoft.AspNetCore.Mvc;
using RetailInventoryManagement.Models;
using RetailInventoryManagement.Services;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
	private readonly TableStorageService _tableService;
	private readonly ServiceBusService _serviceBusService;

	public ProductController(
		TableStorageService tableService,
		ServiceBusService serviceBusService)
	{
		_tableService = tableService;
		_serviceBusService = serviceBusService;
	}

	// POST /api/product/add
	[HttpPost("add")]
	public async Task<IActionResult> AddProduct(
		[FromBody] Product product)
	{
		await _tableService.AddEntityAsync("Products", product);
		return Ok(new
		{
			message = "Product added successfully",
			productId = product.RowKey
		});
	}

	// GET /api/product/all
	[HttpGet("all")]
	public IActionResult GetAllProducts()
	{
		var products = _tableService
			.GetAllEntities<Product>("Products", "PRODUCT");
		return Ok(products);
	}

	// GET /api/product/{id}
	[HttpGet("{id}")]
	public async Task<IActionResult> GetProduct(string id)
	{
		var product = await _tableService
			.GetEntityAsync<Product>("Products", "PRODUCT", id);
		if (product == null)
			return NotFound(new { message = "Product not found" });
		return Ok(product);
	}

	// PUT /api/product/update
	[HttpPut("update")]
	public async Task<IActionResult> UpdateProduct(
		[FromBody] Product product)
	{
		await _tableService.UpdateEntityAsync("Products", product);
		return Ok(new { message = "Product updated successfully" });
	}

	// DELETE /api/product/{id}
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteProduct(string id)
	{
		await _tableService
			.DeleteEntityAsync("Products", "PRODUCT", id);
		return Ok(new { message = "Product deleted successfully" });
	}

	// GET /api/product/lowstock
	[HttpGet("lowstock")]
	public async Task<IActionResult> GetLowStockProducts()
	{
		var products = _tableService
			.GetAllEntities<Product>("Products", "PRODUCT");

		var lowStock = products
			.Where(p => p.StockQuantity < p.MinStockLevel)
			.ToList();

		// Send batch alert for all low stock products
		// Send batch alert for all low stock products
		if (lowStock.Any())
		{
			var messages = lowStock.Select(p =>
				System.Text.Json.JsonSerializer.Serialize(new
				{
					ProductName = p.ProductName,
					CurrentStock = p.StockQuantity,
					MinStockLevel = p.MinStockLevel,
					Message = $"LOW STOCK ALERT: {p.ProductName} only has {p.StockQuantity} units remaining!"
				})
			).ToList();

			await _serviceBusService.SendBatchAlertsAsync(messages);
		}

		return Ok(new
		{
			count = lowStock.Count,
			products = lowStock
		});
	}
}
