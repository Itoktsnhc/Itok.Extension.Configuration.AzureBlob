using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Json;

namespace Itok.Extension.Configuration.AzureBlob
{
    public class AzureBlobConfigurationProvider : JsonConfigurationProvider
    {
        private readonly AzureBlobConfigurationSource _source;
        private string _etag;

        public AzureBlobConfigurationProvider(AzureBlobConfigurationSource source) : base(source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            Load();
            if (_source.Option.ReloadOnChange)
            {
                Task.Factory.StartNew(ReloadOnChange);
            }
        }

        private async void ReloadOnChange()
        {
            while (true)
            {
                try
                {
                    await LoadAsync();
                }
                catch (Exception ex)
                {
                    _source.Option.OnReloadException?.Invoke(ex);
                }

                await Task.Delay(_source.Option.PollingInterval);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public sealed override void Load()
        {
            try
            {
                LoadAsync().Wait();
            }
            catch (AggregateException ae)
            {
                if (ae.InnerException != null)
                    throw ae.InnerException;
                throw;
            }
        }

        private async Task LoadAsync()
        {
            using (var ms = new MemoryStream())
            {
                var (etag, updated) = await _source.BlobStore.RetrieveIfUpdated(ms, _etag);

                if (!updated)
                {
                    return;
                }

                _etag = etag;
                ms.Seek(0, SeekOrigin.Begin);
                base.Load(ms);
            }
        }
    }
}