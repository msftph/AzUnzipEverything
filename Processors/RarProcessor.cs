using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Logging;
using SharpCompress.Archives.Rar;

namespace AzUnzipEverything.Processors
{
    public class RarProcessor : ArchiveProcessor
    {
        public RarProcessor(ILogger log)
            : base(log, ".rar")
        {
        }
    }
}