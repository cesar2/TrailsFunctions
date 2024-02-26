using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;

namespace ImageFunctions
{
    public class KeyVaultSecrets
    {
        private static KeyVaultSecrets _instance;
        private static readonly object _lock = new object();

        public string BlobStorageConnectionString { get; private set; }

        private KeyVaultSecrets()
        {
            // Load the secrets from Azure Key Vault
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string keyVaultUrl = configuration["KeyVaultUrl"];
            var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

            BlobStorageConnectionString = GetSecretFromKeyVault(secretClient, "trails-storage-account-connection-string");
        }

        public static KeyVaultSecrets Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new KeyVaultSecrets();
                        }
                    }
                }
                return _instance;
            }
        }

        private string GetSecretFromKeyVault(SecretClient secretClient, string secretName)
        {
            try
            {
                KeyVaultSecret secret = secretClient.GetSecret(secretName);
                return secret.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


}
