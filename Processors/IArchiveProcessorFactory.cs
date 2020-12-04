using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzUnzipEverything.Processors
{
    /// <summary>
    /// ArchiveProcessor takes in a blob and returns many blobs
    /// </summary>
    public interface IArchiveProcessorFactory
    {
        IArchiveProcessor Create(string extension);
        bool Contains(string extension);
    }
}