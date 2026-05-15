using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailInventoryManagement.Models;
using RetailInventoryManagement.Services;

namespace RetailInventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SupplierController : ControllerBase
	{
		private readonly TableStorageService _tableService;

		public SupplierController(TableStorageService tableService)
		{
			_tableService = tableService;
		}

		// POST /api/supplier/add
		[HttpPost("add")]
		public async Task<IActionResult> AddSupplier([FromBody] Supplier supplier)
		{
			await _tableService.AddEntityAsync("Suppliers", supplier);
			return Ok(new
			{
				message = "Supplier added successfully",
				supplierId = supplier.RowKey
			});
		}

		// GET /api/supplier/all
		[HttpGet("all")]
		public IActionResult GetAllSuppliers()
		{
			var suppliers = _tableService
				.GetAllEntities<Supplier>("Suppliers", "SUPPLIER");
			return Ok(suppliers);
		}

		// GET /api/supplier/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetSupplier(string id)
		{
			var supplier = await _tableService
				.GetEntityAsync<Supplier>("Suppliers", "SUPPLIER", id);
			if (supplier == null)
				return NotFound(new { message = "Supplier not found" });
			return Ok(supplier);
		}

		// DELETE /api/supplier/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteSupplier(string id)
		{
			await _tableService
				.DeleteEntityAsync("Suppliers", "SUPPLIER", id);
			return Ok(new { message = "Supplier deleted successfully" });
		}
	}
}
