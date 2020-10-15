using System;
using Microsoft.Extensions.Configuration;

namespace Itok.Extension.Configuration.AzureBlob
{
    public static class AzureBlobConfigurationExtension
    {
        public static IConfigurationBuilder AddAzureBlobJson(this IConfigurationBuilder builder, AzureBlobConfigurationOption option)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Add(new AzureBlobConfigurationSource(option));
        }

        public static void ReadFromConfiguration(IConfiguration config)
        {
            
        }
    }
}
