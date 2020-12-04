using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzUnzipEverything.Processors
{

    /// <summary>
    /// ArchiveProcessor takes in a blob and returns many blobs
    /// </summary>
    public abstract class ArchiveProcessor : IArchiveProcessor
    {
        public string Extension { get; }

        public ILogger Log { get; }

        protected ArchiveProcessor(ILogger log, string extension)
        {
            this.Log = log;
            this.Extension = extension;
        }

        public abstract Task ProcessAsync(CloudBlockBlob source, CloudBlobContainer target);
    }
}