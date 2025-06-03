using System;
using AzurePizza01.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

namespace AzurePizza01
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Lägg till Azure Key Vault om en miljövariabel KeyVaultName är satt
                    var builtConfig = config.Build();
                    var keyVaultName = builtConfig["KeyVaultName"];
                    if (!string.IsNullOrEmpty(keyVaultName))
                    {
                        var kvUri = $"https://{keyVaultName}.vault.azure.net/";
                        config.AddAzureKeyVault(
                            new SecretClient(new Uri(kvUri), new DefaultAzureCredential()),
                            new AzureKeyVaultConfigurationOptions()
                        );
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
