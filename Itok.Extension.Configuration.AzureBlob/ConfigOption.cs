﻿using System;

 namespace Itok.Extension.Configuration.AzureBlob
{
    public class AzureBlobConfigurationOption
    {
        public string ConnStr { get; set; }
        public bool ReloadOnChange { get; set; } = false;
        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(60);
        public string BlobName { get; set; }
        public string ContainerName { get; set; }
        public Action<Exception> OnReloadException;
    }
}
