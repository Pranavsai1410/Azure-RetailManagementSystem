using Microsoft.AspNetCore.Mvc;
using RetailInventoryManagement.Models;
using RetailInventoryManagement.Services;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
	private readonly TableStorageService _tableService;
	private readonly ServiceBusService _serviceBusService;

	public SalesController(
		TableStorageService tableService,
		ServiceBusService serviceBusService)
	{
		_tableService = tableService;
		_serviceBusService = serviceBusService;
	}

	// POST /api/sales/record
	[HttpPost("record")]
	public async Task<IActionResult> RecordSale([FromBody] Sale sale)
	{
		// 1. Get Product
		var product = await _tableService.GetEntityAsync<Product>(
			"Products", "PRODUCT", sale.ProductID);

		if (product == null)
			return NotFound("Product not found");

		// ✅ 2. Prevent negative stock
		if (sale.QuantitySold <= 0)
			return BadRequest("Quantity sold must be greater than zero");

		if (product.StockQuantity < sale.QuantitySold)
		{
			return BadRequest(new
			{
				message = "Insufficient stock",
				availableStock = product.StockQuantity
			});
		}

		// 3. Calculate Total
		sale.TotalAmount = sale.QuantitySold * sale.SalePrice;

		// 4. Save Sale
		await _tableService.AddEntityAsync("Sales", sale);

		// 5. Reduce Stock (SAFE)
		product.StockQuantity -= sale.QuantitySold;
		await _tableService.UpdateEntityAsync("Products", product);

		// ✅ 6. Low Stock Alert (JSON)
		if (product.StockQuantity < product.MinStockLevel)
		{
			var alertMessage = System.Text.Json.JsonSerializer.Serialize(new
			{
				ProductName = product.ProductName,
				CurrentStock = product.StockQuantity,
				MinStockLevel = product.MinStockLevel,
				Message =
					$"LOW STOCK ALERT: {product.ProductName} " +
					$"is now at {product.StockQuantity} units! " +
					$"Minimum level is {product.MinStockLevel} units."
			});

			await _serviceBusService.SendAlertAsync(alertMessage);
		}

		return Ok(new
		{
			message = "Sale recorded successfully",
			saleId = sale.RowKey,
			totalAmount = sale.TotalAmount,
			remainingStock = product.StockQuantity
		});
	}

	// GET /api/sales/today
	[HttpGet("today")]
	public IActionResult GetTodaySales()
	{
		var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
		var sales = _tableService
			.GetAllEntities<Sale>("Sales", today);
		var totalRevenue = sales.Sum(s => s.TotalAmount);

		return Ok(new
		{
			date = today,
			totalSales = sales.Count,
			totalRevenue,
			sales
		});
	}
}
