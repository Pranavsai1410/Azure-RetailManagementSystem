using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailInventoryManagement.Models;
using RetailInventoryManagement.Services;

namespace RetailInventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PurchaseOrderController : ControllerBase
	{
		private readonly TableStorageService _tableService;

		public PurchaseOrderController(TableStorageService tableService)
		{
			_tableService = tableService;
		}

		// POST /api/purchaseorder/create
		[HttpPost("create")]
		public async Task<IActionResult> CreateOrder(
			[FromBody] PurchaseOrder order)
		{
			await _tableService.AddEntityAsync("PurchaseOrders", order);
			return Ok(new
			{
				message = "Purchase order created",
				orderId = order.RowKey
			});
		}

		// GET /api/purchaseorder/all
		[HttpGet("all")]
		public IActionResult GetAllOrders()
		{
			var orders = _tableService
				.GetAllEntities<PurchaseOrder>("PurchaseOrders", "ORDER");
			return Ok(orders);
		}

		// PUT /api/purchaseorder/deliver/{id}
		[HttpPut("deliver/{id}")]
		public async Task<IActionResult> MarkAsDelivered(string id)
		{
			var order = await _tableService
				.GetEntityAsync<PurchaseOrder>("PurchaseOrders", "ORDER", id);

			order.Status = "Delivered";
			await _tableService.UpdateEntityAsync("PurchaseOrders", order);

			// Update stock quantity
			var product = await _tableService
				.GetEntityAsync<Product>("Products", "PRODUCT", order.ProductID);

			product.StockQuantity += order.QuantityOrdered;
			await _tableService.UpdateEntityAsync("Products", product);

			return Ok(new { message = "Order marked as delivered, stock updated" });
		}
	}
}
