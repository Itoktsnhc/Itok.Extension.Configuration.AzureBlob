using System;
using System.IO;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
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
            var blobName = "rootConfig";
            var blobName1 = "rootConfig1";
            var blobService = new BlobServiceClient(connectionStr);
            var container = blobService.GetBlobContainerClient(containerName);
            container.CreateIfNotExists();
            var blobRef = container.GetBlobClient(blobName);
            blobRef.DeleteIfExists();
            blobRef.Upload("sample.json");
            var blob2Ref = container.GetBlobClient(blobName1);
            blob2Ref.DeleteIfExists();
            blob2Ref.Upload("sample2.json");
            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    builder.AddJsonFile("settings.json");
                    builder.AddAzureBlobJson(new AzureBlobConfigurationOption()
                    {
                        ConnStr = connectionStr,
                        BlobName = blobName,
                        ContainerName = containerName,
                        OnReloadException = Console.WriteLine,
                        ReloadOnChange = true,
                        PollingInterval = TimeSpan.FromSeconds(10)
                    });
                    builder.AddAzureBlobJson(new AzureBlobConfigurationOption()
                    {
                        ConnStr = connectionStr,
                        BlobName = blobName1,
                        ContainerName = containerName,
                        OnReloadException = Console.WriteLine,
                        ReloadOnChange = true,
                        PollingInterval = TimeSpan.FromSeconds(10)
                    });
                }).ConfigureServices((hostContext, svc) =>
                {
                    svc.Configure<SampleConfig>(hostContext.Configuration.GetSection(nameof(SampleConfig)));
                    svc.Configure<SampleConfig1>(hostContext.Configuration.GetSection(nameof(SampleConfig1)));
                }).Build();

            host.StartAsync().Wait();
            var samples = host.Services.GetRequiredService<IOptionsMonitor<SampleConfig>>();
            var sample2 = host.Services.GetRequiredService<IOptionsMonitor<SampleConfig1>>();
            Console.ReadLine();
        }
    }

    class SampleConfig
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class SampleConfig1
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}