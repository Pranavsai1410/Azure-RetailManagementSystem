using RetailInventoryManagement.Services;

namespace RetailInventoryManagement
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			//builder.Services.AddApplicationInsightsTelemetry();

			builder.Services.AddSingleton<KeyVaultService>();

			builder.Services.AddSingleton<TableStorageService>();

			builder.Services.AddSingleton<ServiceBusService>();

			var app = builder.Build();
			app.UseSwagger();
			app.UseSwaggerUI();
			// Configure the HTTP request pipeline.

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
