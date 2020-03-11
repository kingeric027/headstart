using System;
using Microsoft.Extensions.Configuration;

namespace Marketplace.Helpers.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder TryAddAzureKeyVault(this IConfigurationBuilder builder, string vaultName)
        {
            // other checks can be added here 
            // for short term assume if null don't add

            //if (!string.IsNullOrEmpty(vaultName))
            //{
            //    var vaultUrl = $"https://{vaultName}.vault.azure.net/";
            //    builder.AddAzureKeyVault(vaultUrl);
            //}

            return builder;
        }
    }
}
