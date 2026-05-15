using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
using RetailInventoryManagement.Services;

public class ServiceBusService
{
	private readonly ServiceBusSender _sender;
	public ServiceBusService(IConfiguration config)
	{
		var keyVaultService = new KeyVaultService();
		var connectionString = keyVaultService.GetSecret("ServiceBusConnection");
		var queueName = keyVaultService.GetSecret("ServiceBusQueueName");

		var options = new ServiceBusClientOptions
		{
			TransportType = ServiceBusTransportType.AmqpWebSockets 
		};

		var client = new ServiceBusClient(connectionString, options);

		_sender = client.CreateSender(queueName);
	}

	public async Task SendAlertAsync(string message)
	{
		var serviceBusMessage = new ServiceBusMessage(message);
		await _sender.SendMessageAsync(serviceBusMessage);
	}

	public async Task SendBatchAlertsAsync(List<string> messages)
	{
		ServiceBusMessageBatch batch = await _sender.CreateMessageBatchAsync();

		foreach (var message in messages)
		{
			if (!batch.TryAddMessage(new ServiceBusMessage(message)))
			{
				await _sender.SendMessagesAsync(batch);
				batch = await _sender.CreateMessageBatchAsync();
				batch.TryAddMessage(new ServiceBusMessage(message));
			}
		}

		await _sender.SendMessagesAsync(batch);
	}
}
