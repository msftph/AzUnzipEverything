using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Logging;
using SharpCompress.Archives.SevenZip;
using System.IO;
using System;
using Microsoft.WindowsAzure.Storage;

namespace AzUnzipEverything.Processors
{
    public class SevenZipProcessor : ArchiveProcessor
    {
        public SevenZipProcessor(ILogger log)
            : base(log, ".7z")
        {
        }
    }
}