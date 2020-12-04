using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzUnzipEverything.Processors
{
    public interface IArchiveProcessor
    {
        string Extension { get; }
        ILogger Log { get; }

        Task ProcessAsync(CloudBlockBlob source, CloudBlobContainer target);
    }
}