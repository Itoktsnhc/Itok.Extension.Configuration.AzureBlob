using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Itok.Extension.Configuration.AzureBlob
{
    public class AzureBlobConfigurationSource : JsonConfigurationSource
    {
        public readonly AzureBlobConfigurationOption Option;
        public readonly BlobStore BlobStore;

        public AzureBlobConfigurationSource(AzureBlobConfigurationOption option)
        {
            Option = option ?? throw new ArgumentNullException(nameof(option));
            BlobStore = new BlobStore(Option);
        }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureBlobConfigurationProvider(this);
        }
    }
}
