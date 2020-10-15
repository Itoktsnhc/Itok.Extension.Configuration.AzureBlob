using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace Itok.Extension.Configuration.AzureBlob
{
    public class BlobStore
    {
        private readonly BlobServiceClient _account;
        private readonly string _containerName;
        private readonly string _blobName;

        private BlobStore(string connStr, string containerName, string blobName)
        {
            _account = new BlobServiceClient(connStr);
            _containerName = containerName;
            _blobName = blobName;
            _account.GetBlobContainerClient(containerName).CreateIfNotExists();
        }

        public BlobStore(AzureBlobConfigurationOption option) : this(option.ConnStr,
            option.ContainerName,
            option.BlobName)
        {
        }

        public async Task<(string, bool)> RetrieveIfUpdated(MemoryStream ms, string eTag)
        {
            var blobRef =
                _account.GetBlobContainerClient(_containerName)
                    .GetBlockBlobClient(_blobName);
            if (ms == null)
            {
                throw new ArgumentNullException(nameof(ms));
            }

            var props = await blobRef.GetPropertiesAsync();

            if (string.IsNullOrEmpty(props?.Value?.ETag.ToString()) || string.Equals(props.Value.ETag.ToString(), eTag))
            {
                return (props?.Value?.ETag.ToString(), false);
            }

            await blobRef.DownloadToAsync(ms);
            return (props.Value?.ETag.ToString(), true);
        }
    }
}