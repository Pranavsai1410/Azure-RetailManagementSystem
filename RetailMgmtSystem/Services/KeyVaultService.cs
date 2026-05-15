using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace RetailInventoryManagement.Services
{
	public class KeyVaultService
	{
		private readonly SecretClient _secretClient;

		public KeyVaultService()
		{
			string keyVaultUrl = "https://retailmanagement.vault.azure.net/";

			// For System Assigned Identity, no client ID is required
			var credential = new DefaultAzureCredential();

			_secretClient = new SecretClient(
				new Uri(keyVaultUrl),
				credential
			);
		}

		public string GetSecret(string secretName)
		{
			var secret = _secretClient.GetSecret(secretName);
			return secret.Value.Value;
		}
	}
}
