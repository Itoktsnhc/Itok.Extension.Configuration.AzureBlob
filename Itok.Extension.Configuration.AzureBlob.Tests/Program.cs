using System;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Itok.Extension.Configuration.AzureBlob.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionStr = "UseDevelopmentStorage=true";
            var containerName = "configs";
            var rootConfig = "rootConfig";
            var blobService = new BlobServiceClient(connectionStr);
            var container = blobService.GetBlobContainerClient(containerName);
            container.CreateIfNotExists();
            var blobRef = container.GetBlobClient(rootConfig);
            blobRef.Upload("sample.json");
            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    builder.AddAzureBlobJson(new AzureBlobConfigurationOption()
                    {
                        ConnStr = connectionStr,
                        BlobName = rootConfig,
                        ContainerName = containerName,
                        OnReloadException = Console.WriteLine,
                        ReloadOnChange = true,
                        PollingInterval = TimeSpan.FromSeconds(10)
                    });
                }).ConfigureServices((hostContext, svc) =>
                {
                    svc.Configure<SampleConfig>(hostContext.Configuration.GetSection(nameof(SampleConfig)));
                }).Build();

            host.StartAsync().Wait();
            var samples = host.Services.GetRequiredService<IOptionsMonitor<SampleConfig>>();
            Console.ReadLine();
        }
    }

    class SampleConfig
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}